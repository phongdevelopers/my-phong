using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Messaging;
using CommerceBuilder.Products;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Payments.Providers.GoogleCheckout;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// Utility class for processing store events
    /// </summary>
    public static class StoreEventEngine
    {   
        /// <summary>
        /// Updates status of an order based on the store event
        /// </summary>
        /// <param name="storeEvent">StoreEvent to determine the new order status</param>
        /// <param name="order">Order to update status of</param>
        public static void UpdateOrderStatus(StoreEvent storeEvent, Order order)
        {
            OrderStatus newStatus = OrderStatusTriggerDataSource.LoadForStoreEvent(storeEvent);
            if (newStatus != null)
            {
                OrderDataSource.UpdateOrderStatus(order, newStatus);
            }
        }

        /// <summary>
        /// Triggers the order status updated event.
        /// </summary>
        /// <param name="order">Order that had a status change</param>
        /// <param name="oldStatusName">Name of the prior status</param>
        public static void OrderStatusUpdated(Order order, string oldStatusName)
        {
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["oldstatusname"] = oldStatusName;
            ProcessEmails(StoreEvent.OrderStatusUpdated, parameters);
            ProcessEmails(order.OrderStatusId, parameters);
        }

        /// <summary>
        /// Triggers the low inventory item purchased event.
        /// </summary>
        /// <param name="products">dctionary of product ids and respective variants</param>
        public static void LowInventoryItemPurchased(Dictionary<int, string> products)
        {
            if (products != null && products.Count > 0)
            {
                List<Product> productCollection = new List<Product>();
                List<ProductVariant> variantCollection = new List<ProductVariant>();
                List<Vendor> vendorsCollection = new List<Vendor>();
                Dictionary<int, Vendor> vendors = new Dictionary<int, Vendor>();

                foreach (int productId in products.Keys)
                {
                    if (String.IsNullOrEmpty(products[productId]))
                    {
                        // ONLY INCLUDE PRODUCTS WITHOUT VARIANT INFORMATION
                        Product p = ProductDataSource.Load(productId);
                        if (p != null)
                        {
                            productCollection.Add(p);
                            if (p.Vendor != null && !vendors.ContainsKey(p.Vendor.VendorId))
                                vendors.Add(p.Vendor.VendorId, p.Vendor);
                        }
                    }
                    else
                    {
                        ProductVariant v = ProductVariantDataSource.LoadForOptionList(productId, products[productId]);
                        if (v != null)
                        {
                            variantCollection.Add(v);
                            if (v.Product.Vendor != null && !vendors.ContainsKey(v.Product.Vendor.VendorId))
                                vendors.Add(v.Product.Vendor.VendorId, v.Product.Vendor);
                        }
                    }
                }
                
                foreach (Vendor vendor in vendors.Values)
                    vendorsCollection.Add(vendor);

                Hashtable parameters = new Hashtable();
                parameters["products"] = productCollection.ToArray();
                parameters["variants"] = variantCollection.ToArray();
                parameters["vendors"]  = vendorsCollection.ToArray();
                ProcessEmails(StoreEvent.LowInventoryItemPurchased, parameters);
            }
        }

        /// <summary>
        /// Processes a payment authorize event
        /// </summary>
        /// <param name="payment">The payment that is authorized</param>
        public static void PaymentAuthorized(Payment payment)
        {
            Order order = payment.Order;
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.PaymentAuthorized, payment.Amount), NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.PaymentAuthorized, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payment"] = payment;
            ProcessEmails(StoreEvent.PaymentAuthorized, parameters);
        }

        /// <summary>
        /// Processes a payment void event
        /// </summary>
        /// <param name="payment">The payment that is voided</param>
        public static void PaymentVoided(Payment payment)
        {
            Order order = payment.Order;
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.PaymentVoided, payment.Amount), NoteType.SystemPrivate));
            order.Notes.Save();
        }

        /// <summary>
        /// Processes a partial void event
        /// </summary>
        /// <param name="payment">Payment that is voided</param>
        /// <param name="voidAmount">The amount that is voided</param>
        public static void PaymentVoidedPartial(Payment payment, LSDecimal voidAmount)
        {
            Order order = payment.Order;
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.PaymentVoidedPartial, voidAmount, payment.Amount), NoteType.SystemPrivate));
            order.Notes.Save();
        }

        /// <summary>
        /// Processes a payment refund event
        /// </summary>
        /// <param name="payment">The payment that is refunded</param>
        /// <param name="refundAmount">The amount refunded</param>
        public static void PaymentRefunded(Payment payment, LSDecimal refundAmount)
        {
            Order order = payment.Order;
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.PaymentRefunded, refundAmount), NoteType.SystemPrivate));
            order.Notes.Save();
        }

        /// <summary>
        /// Processes a payment capture event
        /// </summary>
        /// <param name="payment">The payment that is captured</param>
        /// <param name="captureAmount">The amount that is captured</param>
        public static void PaymentCaptured(Payment payment, LSDecimal captureAmount)
        {
            Order order = OrderDataSource.Load(payment.OrderId, false);
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.PaymentCaptured, captureAmount), NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.PaymentCaptured, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payment"] = payment;
            ProcessEmails(StoreEvent.PaymentCaptured, parameters);
            //A PAYMENT HAS BEEN COMPLETED, SO DETERMINE WHICH ORDER PAID EVENT TO TRIGGER
            LSDecimal balance = order.GetBalance(false);
            //ACTIVATE DIGITAL GOODS USING ActivationMode.OnPaidOrder 
            //TRIGGER APPROPRIATE ORDER PAID EVENT
            if (balance == 0) OrderPaid(order);
            else if (balance > 0) OrderPaidPartial(order, balance);
            else OrderPaidCreditBalance(order, balance);
        }

        /// <summary>
        /// Processes a customer password request event
        /// </summary>
        /// <param name="user">The user whose password is requested</param>
        /// <param name="passwordLink">The link to reset the password</param>
        public static void CustomerPasswordRequest(User user, string passwordLink)
        {
            Hashtable parameters = new Hashtable();
            parameters["customer"] = user;
            parameters["resetPasswordLink"] = passwordLink;
            ProcessEmails(StoreEvent.CustomerPasswordRequest, parameters);
        }

        /// <summary>
        /// Processes a partial capture event
        /// </summary>
        /// <param name="payment">The payment that is captured</param>
        /// <param name="captureAmount">The amount that is captured</param>
        public static void PaymentCapturedPartial(Payment payment, LSDecimal captureAmount)
        {
            Order order = payment.Order;
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.PaymentCapturedPartial, captureAmount), NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.PaymentCapturedPartial, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payment"] = payment;
            ProcessEmails(StoreEvent.PaymentCapturedPartial, parameters);
            //A PAYMENT HAS BEEN COMPLETED, SO DETERMINE WHICH ORDER PAID EVENT TO TRIGGER
            LSDecimal balance = order.GetBalance(false);
            if (balance == 0) OrderPaid(order);
            else if (balance > 0) OrderPaidPartial(order, balance);
            else OrderPaidCreditBalance(order, balance);
        }

        /// <summary>
        /// Processes a payment authorization failure event
        /// </summary>
        /// <param name="payment">Payment that failed authorization</param>
        public static void PaymentAuthorizationFailed(Payment payment)
        {
            Order order = payment.Order;
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.PaymentAuthorizationFailed, payment.Amount), NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.PaymentAuthorizationFailed, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payment"] = payment;
            ProcessEmails(StoreEvent.PaymentAuthorizationFailed, parameters);
        }

        /// <summary>
        /// Processes a payment capture failure event
        /// </summary>
        /// <param name="payment">Payment that failed to be captured</param>
        /// <param name="captureAmount">The amount of capture</param>
        public static void PaymentCaptureFailed(Payment payment, LSDecimal captureAmount)
        {
            Order order = payment.Order;
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.PaymentCaptureFailed, captureAmount), NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.PaymentCaptureFailed, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payment"] = payment;
            ProcessEmails(StoreEvent.PaymentCaptureFailed, parameters);
        }

        /// <summary>
        /// Processes an order placed event
        /// </summary>
        /// <param name="order">The order that has been placed</param>
        public static void OrderPlaced(Order order)
        {
            OrderPlaced(order, order.Payments);
        }

        /// <summary>
        /// Processes an order placed event
        /// </summary>
        /// <param name="order">The order that has been placed</param>
        /// <param name="payments">The payments that have been made against the order</param>
        public static void OrderPlaced(Order order, PaymentCollection payments)
        {
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, order.OrderDate, Properties.Resources.OrderPlaced, NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.OrderPlaced, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payments"] = payments;
            ProcessEmails(StoreEvent.OrderPlaced, parameters);
        }

        /// <summary>
        /// Processes an order cancelled event
        /// </summary>
        /// <param name="order">Order that has been cancelled</param>
        public static void OrderCancelled(Order order)
        {
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, Properties.Resources.OrderCancelled, NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.OrderCancelled, order);
            //if this is a google checkout order update google checkout
            if (!string.IsNullOrEmpty(order.GoogleOrderNumber))
            {
                GoogleCheckout instance = GoogleCheckout.GetInstance();
                string comment = order.GetLastPublicNote();
                if (string.IsNullOrEmpty(comment)) comment = "N/A";
                instance.CancelOrder(order.GoogleOrderNumber, comment);
            }

            // DELETE ALL ASSOCIATED SUBSCRIPTIONS
            SubscriptionDataSource.DeleteForOrder(order.OrderId);

            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payments"] = order.Payments;
            ProcessEmails(StoreEvent.OrderCancelled, parameters);
        }

        /// <summary>
        /// Processes a paid partial event for an order
        /// </summary>
        /// <param name="order">Order that has been paid partially</param>
        public static void OrderPaidPartial(Order order)
        {
            OrderPaidPartial(order, order.GetBalance(false));
        }

        /// <summary>
        /// Processes a paid partial event for an order
        /// </summary>
        /// <param name="order">Order that has been paid partially</param>
        /// <param name="balance">The remaining balance</param>
        private static void OrderPaidPartial(Order order, LSDecimal balance)
        {
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.OrderPaidPartial, balance) , NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.OrderPaidPartial, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payments"] = order.Payments;
            ProcessEmails(StoreEvent.OrderPaidPartial, parameters);
        }

        /// <summary>
        /// Processes an order paid event
        /// </summary>
        /// <param name="order">Order that has been paid</param>
        public static void OrderPaid(Order order)
        {
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, Properties.Resources.OrderPaid, NoteType.SystemPrivate));
            order.Notes.Save();
            ProcessPaidOrder(order);
            //UPDATE THE ORDER STATUS
            if (!order.HasShippableItems)
            {
                OrderPaidNoShipments(order);
            }
            else
            {
                UpdateOrderStatus(StoreEvent.OrderPaid, order);
                Hashtable parameters = new Hashtable();
                parameters["order"] = order;
                parameters["customer"] = order.User;
                parameters["payments"] = order.Payments;
                ProcessEmails(StoreEvent.OrderPaid, parameters);
            }
        }


        /// <summary>
        /// Processes an order paid event for an order with no shipments
        /// </summary>
        /// <param name="order">Order that has been paid</param>
        public static void OrderPaidNoShipments(Order order)
        {
            //UPDATE THE ORDER STATUS
            UpdateOrderStatus(StoreEvent.OrderPaidNoShipments, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payments"] = order.Payments;
            ProcessEmails(StoreEvent.OrderPaidNoShipments, parameters);
        }

        /// <summary>
        /// Performs operations that should be done when an order is paid or paid with credit balance.
        /// </summary>
        /// <param name="order">order that was paid</param>
        private static void ProcessPaidOrder(Order order)
        {
            //CHECK FOR SUBSCRIPTIONS
            try
            {
                //ANY SUBSCRIPTIONS THAT WERE "ONE TIME ONLY" CHARGES SHOULD BE ACTIVATED
                //THESE WILL NOT BE ACTIVATED BY REFERENCE TO PAYMENT, AND WILL NOT HAVE ANY PAYMENTS ASSOCIATED
                SubscriptionCollection subscriptions = SubscriptionDataSource.LoadForOrder(order.OrderId);
                foreach (Subscription s in subscriptions)
                {
                    if ((!s.IsActive) && (s.Payments.Count == 0)) s.Activate();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error generating subscriptions for order #" + order.OrderId.ToString(), ex);
            }

            try
            {
                IGiftCertKeyProvider provider = new DefaultGiftCertKeyProvider();
                GiftCertificateTransaction trans;
                foreach (OrderItem orderItem in order.Items)
                {
                    //IF THERE ARE ANY GIFT CERTS ATTACHED THEN ACTIVATE THOSE
                    if (orderItem.GiftCertificates != null && orderItem.GiftCertificates.Count > 0)
                    {
                        foreach (GiftCertificate gc in orderItem.GiftCertificates)
                        {
                            gc.SerialNumber = provider.GenerateGiftCertificateKey();
                            trans = new GiftCertificateTransaction();
                            trans.Amount = gc.Balance;
                            trans.Description = "Gift certificate activated.";
                            trans.OrderId = order.OrderId;
                            trans.TransactionDate = LocaleHelper.LocalNow;
                            gc.Transactions.Add(trans);
                            gc.Save();
                            // Trigger Gift Certificate Activated / Validate Event
                            StoreEventEngine.GiftCertificateValidated(gc, trans);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error activating gift certificates for order #" + order.OrderId.ToString(), ex);
            }

            
            foreach (OrderItem orderItem in order.Items)
            {
                //IF THERE ARE ANY DIGITAL GOODS ATTACHED THEN ACTIVATE/FULFILL THOSE
                if (orderItem.DigitalGoods != null && orderItem.DigitalGoods.Count > 0)
                {
                    foreach (OrderItemDigitalGood oidg in orderItem.DigitalGoods)
                    {
                        bool changed = false;
                        try
                        {
                            if (!oidg.IsActivated() && oidg.DigitalGood != null
                                && oidg.DigitalGood.ActivationMode != ActivationMode.Manual)
                            {
                                oidg.Activate();
                                changed = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error activating digital good for order #" + order.OrderId.ToString(), ex);
                        }

                        try
                        {
                            if (!oidg.IsSerialKeyAcquired() && oidg.DigitalGood != null
                                && oidg.DigitalGood.FulfillmentMode != FulfillmentMode.Manual)
                            {
                                oidg.AcquireSerialKey();
                                changed = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error fulfilling digital good for order #" + order.OrderId.ToString(), ex);
                        }

                        if (changed) oidg.Save();
                    }
                }
            }
        }


        /// <summary>
        /// Processes an order paid event with a credit balance
        /// </summary>
        /// <param name="order">Order that has been paid</param>
        public static void OrderPaidCreditBalance(Order order)
        {
            OrderPaidCreditBalance(order, order.GetBalance(false));
        }

        /// <summary>
        /// Processes an order paid event with a credit balance
        /// </summary>
        /// <param name="order">Order that has been paid</param>
        /// <param name="balance">The credit balance after the payment</param>
        private static void OrderPaidCreditBalance(Order order, LSDecimal balance)
        {
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.OrderPaidCreditBalance, balance), NoteType.SystemPrivate));
            order.Notes.Save();
            ProcessPaidOrder(order);
            UpdateOrderStatus(StoreEvent.OrderPaidCreditBalance, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payments"] = order.Payments;
            ProcessEmails(StoreEvent.OrderPaidCreditBalance, parameters);
        }

        /// <summary>
        /// Processes an shipment shipped event
        /// </summary>
        /// <param name="shipment">The shipment that has been shipped</param>
        public static void ShipmentShipped(OrderShipment shipment)
        {
            Order order = shipment.Order;
            UpdateOrderStatus(StoreEvent.ShipmentShipped, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["payments"] = order.Payments;
            parameters["customer"] = order.User;
            parameters["shipment"] = shipment;
            ProcessEmails(StoreEvent.ShipmentShipped, parameters);
            //A PAYMENT HAS BEEN COMPLETED, SO DETERMINE WHICH ORDER PAID EVENT TO TRIGGER
            int unshippedItems = CountUnshippedItems(order);
            if (unshippedItems == 0) OrderShipped(order);
            else OrderShippedPartial(order, unshippedItems);
        }

        private static int CountUnshippedItems(Order order)
        {
            int unshippedItems = 0;
            foreach (OrderShipment shipment in order.Shipments)
            {
                if (shipment.ShipDate == System.DateTime.MinValue)
                {
                    unshippedItems += shipment.OrderItems.Count;
                }
            }
            return unshippedItems;
        }

        /// <summary>
        /// Processes an order shipped event
        /// </summary>
        /// <param name="order">Order that has been shipped</param>
        public static void OrderShipped(Order order)
        {
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, Properties.Resources.OrderShipped, NoteType.SystemPrivate));
            order.Notes.Save();
            order.ShipmentStatus = OrderShipmentStatus.Shipped;
            order.Save();
            UpdateOrderStatus(StoreEvent.OrderShipped, order);
            //if the order was a google checkout order, update gooogle checkout
            if (!string.IsNullOrEmpty(order.GoogleOrderNumber))
            {
                GoogleCheckout instance = GoogleCheckout.GetInstance();
                TrackingNumber number = order.GetLastTrackingNumber();
                instance.DeliverOrder(order.GoogleOrderNumber, number);
            }            
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payments"] = order.Payments;
            ProcessEmails(StoreEvent.OrderShipped, parameters);
        }

        /// <summary>
        /// Processes a partial order shipped event
        /// </summary>
        /// <param name="order">Order that has been partially shipped</param>
        public static void OrderShippedPartial(Order order)
        {
            OrderShippedPartial(order, CountUnshippedItems(order));
        }

        /// <summary>
        /// Processes a partial order shipped event
        /// </summary>
        /// <param name="order">Order that has been partially shipped</param>
        /// <param name="unshippedItems">The number of unshipped items</param>
        private static void OrderShippedPartial(Order order, int unshippedItems)
        {
            order.Notes.Add(new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, string.Format(Properties.Resources.OrderShippedPartial, unshippedItems), NoteType.SystemPrivate));
            order.Notes.Save();
            UpdateOrderStatus(StoreEvent.OrderShippedPartial, order);
            Hashtable parameters = new Hashtable();
            parameters["order"] = order;
            parameters["customer"] = order.User;
            parameters["payments"] = order.Payments;
            ProcessEmails(StoreEvent.OrderShippedPartial, parameters);
        }

        /// <summary>
        /// Processes a gift certificate validation event
        /// </summary>
        /// <param name="gc">Gift certificate that has been validated</param>
        /// <param name="trans">GiftCertificateTransaction</param>
        public static void GiftCertificateValidated(GiftCertificate gc, GiftCertificateTransaction trans)
        {            
            Hashtable parameters = new Hashtable();
            parameters["order"] = gc.OrderItem.Order;
            parameters["customer"] = gc.OrderItem.Order.User;
            parameters["giftcertificate"] = gc;
            ProcessEmails(StoreEvent.GiftCertificateValidated, parameters);
        }

        /// <summary>
        /// Processes an order-note-added event
        /// </summary>
        /// <param name="note">Order note that has been added</param>
        public static void OrderNoteAdded(OrderNote note)
        {
            //PROCESSING ONLY REQUIRED FOR PUBLIC NOTES
            if (note.NoteType == NoteType.Public)
            {
                User author = note.User;
                Order order = note.Order;
                if ((author != null) && (order != null))
                {
                    Hashtable parameters = new Hashtable();
                    parameters["order"] = order;
                    parameters["customer"] = order.User;
                    parameters["note"] = note;
                    if (author.UserId != order.UserId) ProcessEmails(StoreEvent.OrderNoteAddedByMerchant, parameters);
                    else ProcessEmails(StoreEvent.OrderNoteAddedByCustomer, parameters);
                }
            }
        }

        /// <summary>
        /// Processes emails for the given order status using the given parameters
        /// </summary>
        /// <param name="orderStatusId">ID for the OrderStatus for which to process the emails</param>
        /// <param name="parameters">The parameters to be used in nVelocity email templates</param>
        public static void ProcessEmails(int orderStatusId, Hashtable parameters)
        {
            parameters["store"] = Token.Instance.Store;
            //NEED TO GET THE EMAIL TEMPLATES FOR THE ORDER STATUS
            EmailTemplateCollection emailTemplates = EmailTemplateDataSource.LoadForOrderStatus(orderStatusId);
            foreach (EmailTemplate template in emailTemplates)
            {
                if (template != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        template.Parameters[key] = parameters[key];
                    }
                    template.Send();
                }
            }
        }

        /// <summary>
        /// Processes emails for the given store event using the given parameters
        /// </summary>
        /// <param name="storeEvent">StoreEvent object for which to process the emails</param>
        /// <param name="parameters">The parameters to be used in nVelocity email templates</param>
        public static void ProcessEmails(StoreEvent storeEvent, Hashtable parameters)
        {
            parameters["store"] = Token.Instance.Store;
            //NEED TO GET THE EMAIL TEMPLATE FOR THE ORDER
            PersistentCollection<EmailTemplateTrigger> emailTriggers = EmailTemplateTriggerDataSource.LoadForStoreEvent(storeEvent);
            foreach (EmailTemplateTrigger trigger in emailTriggers)
            {
                EmailTemplate template = trigger.EmailTemplate;
                if (template != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        template.Parameters[key] = parameters[key];
                    }
                    template.Send();
                }
            }
        }

    }
}
