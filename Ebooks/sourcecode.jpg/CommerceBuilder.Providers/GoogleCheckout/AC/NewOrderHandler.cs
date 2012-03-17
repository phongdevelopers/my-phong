using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Marketing;
using CommerceBuilder.Payments;
using CommerceBuilder.Shipping;

using CommerceBuilder.Payments.Providers.GoogleCheckout.AC;
using CommerceBuilder.Payments.Providers.GoogleCheckout.Util;
using CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen;
using CommerceBuilder.Payments.Providers.GoogleCheckout;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    
    public class NewOrderHandler
    {
        private NewOrderNotification N1;
        private GoogleCheckout _GatewayInstance = null;
        private GoogleCheckout GatewayInstance
        {
            get
            {
                if (_GatewayInstance != null)
                {
                    return _GatewayInstance;
                }
                else
                {
                    _GatewayInstance = GoogleCheckout.GetInstance();
                    return _GatewayInstance;
                }
            }
        }

        public NewOrderHandler(NewOrderNotification n1) {
            if (n1 == null)
            {
                throw new ArgumentNullException("n1 cant be null");
            }
            N1 = n1;
        }

        public void Process()
        {
            TraceContext trace = WebTrace.GetTraceContext();
            string traceKey = "GoogleCheckout.AC.NewOrderHandler";
            trace.Write(traceKey, "Begin NewOrderHandler.Process, Google order number " + N1.googleordernumber);

            Order order = OrderDataSource.LoadForGoogleOrderNumber(N1.googleordernumber);
            if (order == null) // ordernumber not already entered
            {
                trace.Write(traceKey, "Google order not present in database, get basket");
                Basket basket = AcHelper.GetAcBasket(N1.shoppingcart, true);
                if (basket == null)
                {
                    trace.Write(traceKey, "Basket could not be obtained (End NewOrderHandler.Process)");
                    return;
                }

                //basket is ready. check if there are any order adjustments to be made                                 
                trace.Write(traceKey, "Check for order adjustments");
                OrderAdjustment orderAdj = N1.orderadjustment;
                if (orderAdj != null)
                {
                    trace.Write(traceKey, "Order adjustments present, add to basket");
                    OrderAdjustmentHelper.DoOrderAdjustments(orderAdj, basket);
                }

                trace.Write(traceKey, "set billing address");
                Users.Address primaryAddress = basket.User.PrimaryAddress;
                AcHelper.PopulateAcAddress(primaryAddress, N1.buyerbillingaddress);
                trace.Write(traceKey, "set shipping address");
                Users.Address shipAddr = AcHelper.GetAcAddress(basket.User, N1.buyershippingaddress);
                basket.User.Addresses.Add(shipAddr);
                basket.User.Save();

                trace.Write(traceKey, "package the basket");
                basket.Package(false);

                if (basket.Shipments.Count > 0)
                {
                    //there are shippable items / shipments
                    //set shipment address and shipment method
                    trace.Write(traceKey, "shippable items present, get shipping method");
                    ShipMethod shipMethod = AcHelper.GetShipMethod(basket);
                    trace.Write(traceKey, "ship method is " + shipMethod.Name + " (ID" + shipMethod.ShipMethodId.ToString() + ")");
                    foreach (BasketShipment shipment in basket.Shipments)
                    {
                        shipment.AddressId = shipAddr.AddressId;
                        shipment.ShipMethodId = shipMethod.ShipMethodId;
                        shipment.Save();
                    }
                    //have to link the shipping charges with some shipment.
                    //we can't know which shipment. Just link to the first.
                    trace.Write(traceKey, "assign shipping charges to first shipment");
                    BasketShipment basketShipment = basket.Shipments[0];
                    foreach (BasketItem item in basket.Items)
                    {
                        if (item.OrderItemType == OrderItemType.Shipping)
                        {
                            item.BasketShipmentId = basketShipment.BasketShipmentId;
                            //update the sku and shipping method name so that scrubbed name is not used
                            item.Name = shipMethod.Name;
                            item.Sku = string.Empty;
                        }
                    }
                }

                trace.Write(traceKey, "save basket");
                basket.Save();

                //now checkout the order with null payment. 
                //this will alow payment to be processed later
                trace.Write(traceKey, "submit basket checkout");
                CheckoutRequest acCheckout = new CheckoutRequest(null);
                CheckoutResponse acResp = basket.Checkout(acCheckout);
                if (acResp.Success)
                {
                    trace.Write(traceKey, "checkout was successful, update the google order number for AC order number " + acResp.OrderNumber.ToString());
                    order = OrderDataSource.Load(acResp.OrderId, false);
                    if (order != null)
                    {
                        //update email address associated with order
                        order.BillToEmail = N1.buyerbillingaddress.email;
                        order.GoogleOrderNumber = N1.googleordernumber;

                        bool isPaidByGc = false;

                        //IF THERE IS ONE PAYMENT AND IT IS A GIFT CERTIFICATE
                        //AND IT COVERS THE BALANCE OF THE ORDER THEN THIS IS THE GOOGLE PAYMENT
                        if (order.Payments.Count == 1)
                        {
                            int gcPayMethodId = PaymentEngine.GetGiftCertificatePaymentMethod().PaymentMethodId;
                            Payment payment = order.Payments[0];
                            if (payment.PaymentMethodId == gcPayMethodId)
                            {
                                if (payment.Amount == order.TotalCharges) isPaidByGc = true;
                            }
                        }
                        if (!isPaidByGc)
                        {
                            //We need to create a new payment with status of authorization pending
                            Payment payment = new Payment();
                            payment.PaymentMethodId = AcHelper.GetGCPaymentMethodId(this.GatewayInstance);
                            payment.Amount = order.GetBalance(false);
                            payment.OrderId = order.OrderId;
                            payment.PaymentMethodName = "GoogleCheckout";
                            Transaction trans = new Transaction();
                            trans.TransactionType = TransactionType.Authorize;
                            trans.TransactionStatus = TransactionStatus.Pending;
                            trans.Amount = payment.Amount;
                            trans.PaymentGatewayId = this.GatewayInstance.PaymentGatewayId;
                            trans.ProviderTransactionId = N1.googleordernumber;
                            trans.TransactionDate = N1.timestamp;
                            payment.Transactions.Add(trans);
                            payment.PaymentStatus = PaymentStatus.AuthorizationPending;
                            order.Payments.Add(payment);                            
                        }
                        order.Save();
                    }
                    else OrderDataSource.UpdateGoogleOrderNumber(acResp.OrderId, N1.googleordernumber);
                }
                else
                {
                    trace.Write(traceKey, "checkout failed for google order");
                    CommerceBuilder.Utility.Logger.Warn("GoogleCheckout : New Order Checkout Failed.");
                }

                trace.Write(traceKey, "Send AC order number back to Google");
                AcNotifier.AddMerchantOrderNumber(GatewayInstance, N1.googleordernumber, acResp.OrderNumber.ToString());
            }
            else
            {
                //order number already entered. Just send notification
                trace.Write(traceKey, "Google order in database, send AC order number back to Google");
                AcNotifier.AddMerchantOrderNumber(GatewayInstance, N1.googleordernumber, order.OrderNumber.ToString());
            }
            trace.Write(traceKey, "End NewOrderHandler.Process");
        }
    }
}
