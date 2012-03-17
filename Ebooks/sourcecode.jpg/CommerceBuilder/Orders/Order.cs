using System;
using CommerceBuilder.Common;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Payments;
using CommerceBuilder.Utility;
using CommerceBuilder.Marketing;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Payments.Providers.GoogleCheckout;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// This class represents an order
    /// </summary>
    public partial class Order
    {
        /// <summary>
        /// Indicates the overall payment status of an order.
        /// </summary>
        public OrderPaymentStatus PaymentStatus
        {
            get { return (OrderPaymentStatus)this.PaymentStatusId; }
            set { this.PaymentStatusId = (byte)value; }
        }

        /// <summary>
        /// Indicates the overall shipment status of an order.
        /// </summary>
        public OrderShipmentStatus ShipmentStatus
        {
            get { return (OrderShipmentStatus)this.ShipmentStatusId; }
            set { this.ShipmentStatusId = (byte)value; }
        }

        /// <summary>
        /// Gets the remaining balance for this order
        /// </summary>
        /// <returns>remaining balance</returns>
        public LSDecimal GetBalance() { return this.GetBalance(true); }

        /// <summary>
        /// Gets the remaining balance for this order including the pending payments
        /// </summary>
        /// <param name="includePendingPayments">Whether to include pending payments in the balance or not</param>
        /// <returns>remaining balance</returns>
        public LSDecimal GetBalance(bool includePendingPayments)
        {
            return this.Items.TotalPrice() - this.Payments.Total(includePendingPayments);
        }

        /// <summary>
        /// Gets the order balance that should be displayed to a customer.
        /// </summary>
        /// <returns>The order balance for customer display.</returns>
        /// <remarks>The balance for customer display is not the actual balance.  It include payments that may not be fully processed.</remarks>
        public LSDecimal GetCustomerBalance()
        {
            LSDecimal balance = this.TotalCharges;
            foreach (Payment payment in this.Payments)
            {
                switch (payment.PaymentStatus)
                {
                    case CommerceBuilder.Payments.PaymentStatus.Unprocessed:
                    case CommerceBuilder.Payments.PaymentStatus.AuthorizationPending:
                    case CommerceBuilder.Payments.PaymentStatus.Authorized:
                    case CommerceBuilder.Payments.PaymentStatus.AuthorizationFailed:
                    case CommerceBuilder.Payments.PaymentStatus.CapturePending:
                    case CommerceBuilder.Payments.PaymentStatus.Captured:
                    case CommerceBuilder.Payments.PaymentStatus.CaptureFailed:
                    case CommerceBuilder.Payments.PaymentStatus.Completed:
                        balance -= payment.Amount;
                        break;
                }
            }
            return balance;
        }

        /// <summary>
        /// Country of the billing address
        /// </summary>
        public Country BillToCountry
        {
            get
            {
                return CountryDataSource.Load(this.BillToCountryCode);
            }
        }

        /// <summary>
        /// Gets the summary of order totals
        /// </summary>
        /// <returns>Summary of order totals</returns>
        public OrderTotalsSummary GetTotalsSummary()
        {
            OrderTotalsSummary summary = new OrderTotalsSummary();
            summary.TotalCharges = this.TotalCharges;

            foreach (Payment payment in this.Payments)
            {
                switch (payment.PaymentStatus)
                {
                    case CommerceBuilder.Payments.PaymentStatus.Unprocessed:
                    case CommerceBuilder.Payments.PaymentStatus.AuthorizationPending:
                    case CommerceBuilder.Payments.PaymentStatus.Authorized:
                    case CommerceBuilder.Payments.PaymentStatus.AuthorizationFailed:
                    case CommerceBuilder.Payments.PaymentStatus.CapturePending:
                    case CommerceBuilder.Payments.PaymentStatus.CaptureFailed:
                        summary.AddPayment(payment.Amount, false);
                        break;
                    case CommerceBuilder.Payments.PaymentStatus.Captured:
                    case CommerceBuilder.Payments.PaymentStatus.Completed:
                        summary.AddPayment(payment.Amount, true);
                        break;
                }
            }
            return summary;
        }

        /// <summary>
        /// Recalculates the current shipment status for this order.
        /// </summary>
        public void RecalculateShipmentStatus()
        {
            //KEEP TRACK OF UPDATED STATUS TO DETECT CHANGES
            OrderShipmentStatus newStatus;

            //CHECK WHETHER THE ORDER HAS ANY SHIPMENTS
            if (this.Shipments.Count > 0)
            {
                //MAKE SURE THERE ARE NO SHIPPABLE ITEMS
                bool foundShippable = false;
                int i = 0;
                while ((i < this.Shipments.Count) && (!foundShippable))
                {
                    OrderShipment shipment = this.Shipments[i];
                    if (!shipment.IsShipped) foundShippable = true;
                    i++;
                }
                if (foundShippable) newStatus = OrderShipmentStatus.Unshipped;
                else newStatus = OrderShipmentStatus.Shipped;
            }
            else
            {
                //THERE ARE NO SHIPMENTS, THIS ORDER IS NON-SHIPPABLE
                newStatus = OrderShipmentStatus.NonShippable;
            }

            //UNLESS WE KNOW THE ORDER IS UNSHIPED,
            //CHECK FOR SHIPPABLE PRODUCTS NOT IN A SHIPMENT
            if (this.ShipmentStatus != OrderShipmentStatus.Unshipped)
            {
                bool foundShippable = false;
                int i = 0;
                while ((i < this.Items.Count) && (!foundShippable))
                {
                    OrderItem item = this.Items[i];
                    if ((item.OrderItemType == OrderItemType.Product) && (item.OrderShipmentId == 0) && (item.Shippable != Shippable.No)) foundShippable = true;
                    i++;
                }
                if (foundShippable) newStatus = OrderShipmentStatus.Unshipped;
            }

            //CHECK FOR CHANGES
            if (this.ShipmentStatus != newStatus)
            {
                OrderDataSource.UpdateShipmentStatus(this, newStatus);
            }
        }

        /// <summary>
        /// Recalculate the current payment status of an order
        /// </summary>
        /// <remarks>We should not rely on this kind of recalculation for setting 
        /// our statuses.  If a status updates here, it is because it did not occur in 
        /// some other, more appropriate place.  Therefore we should log these occurrences as 
        /// the warnings so they can be analyzed and corrected.</remarks>
        public void RecalculatePaymentStatus()
        {
            RecalculatePaymentStatus(false);
        }

        /// <summary>
        /// Recalculate the current payment status of an order
        /// </summary>
        /// <param name="triggerEvents">If true events are triggered on change of payment status</param>
        /// <remarks>We should not rely on this kind of recalculation for setting 
        /// our statuses.  If a status updates here, it is because it did not occur in 
        /// some other, more appropriate place.  Therefore we should log these occurrences as
        /// the warnings so they can be analyzed and corrected.</remarks>
        public void RecalculatePaymentStatus(bool triggerEvents)
        {
            //FIRST RECALCULATE THE STORED VALUES
            LSDecimal productSubtotal = this.GetProductSubtotal();
            LSDecimal totalCharges = this.Items.TotalPrice();
            LSDecimal totalPayments = this.Payments.TotalProcessed();
            bool updated = OrderDataSource.UpdateCalculatedTotals(this, productSubtotal, totalCharges, totalPayments);

            //KEEP TRACK OF NEW STATUS TO DETECT CHANGES
            OrderPaymentStatus newStatus;

            //GET THE CURRENT BALANCE (EXCLUDING PENDING PAYMENTS)
            LSDecimal balance = (totalCharges - totalPayments);
            if (balance > 0)
            {
                //CHECK IF THE LAST PAYMENT RESULTED IN A PROBLEM
                Payment lastPayment = this.Payments.LastPayment;
                if ((lastPayment != null) && (lastPayment.IsFailed))
                    newStatus = OrderPaymentStatus.Problem;
                else newStatus = OrderPaymentStatus.Unpaid;
            }
            else newStatus = OrderPaymentStatus.Paid;

            //CHECK IF PAYMENT STATUS CHANGED
            if (this.PaymentStatus != newStatus)
            {
                OrderDataSource.UpdatePaymentStatus(this, newStatus);
                if (newStatus == OrderPaymentStatus.Paid)
                {
                    if (balance == 0)
                    {
                        if (triggerEvents)
                            StoreEventEngine.OrderPaid(this);
                        else if (this.HasShippableItems)
                            StoreEventEngine.UpdateOrderStatus(StoreEvent.OrderPaid,this);
                        else
                            StoreEventEngine.UpdateOrderStatus(StoreEvent.OrderPaidNoShipments, this);
                    }
                    else
                    {
                        if (triggerEvents)
                            StoreEventEngine.OrderPaidCreditBalance(this);
                        else StoreEventEngine.UpdateOrderStatus(StoreEvent.OrderPaidCreditBalance, this);
                    }
                }
            }
        }

        internal LSDecimal GetProductSubtotal()
        {
            LSDecimal productSubtotal = 0;
            foreach (OrderItem oi in this.Items)
            {
                if (oi.OrderItemType == OrderItemType.Product || oi.OrderItemType == OrderItemType.Discount) productSubtotal += oi.ExtendedPrice;
                else if (oi.OrderItemType == OrderItemType.Coupon)
                {
                    //ONLY ADD COUPON IF IT IS NOT A SHIPPING COUPON
                    string couponCode = oi.Sku;
                    Coupon coupon = CouponDataSource.LoadForCouponCode(couponCode);
                    if (coupon != null && coupon.CouponType != CouponType.Shipping) productSubtotal += oi.ExtendedPrice;
                }
            }
            return productSubtotal;
        }

        /// <summary>
        /// Save this order to database
        /// </summary>
        /// <returns>The result of the save operation</returns>
        /// <remarks>Defaults to recalculate payment and shipment statuses, but not trigger events associated with recaulculation.</remarks>
        public virtual SaveResult Save()
        {
            return this.Save(true, false);
        }


        /// <summary>
        /// Save this order to database
        /// </summary>
        /// <param name="recalculate">If true balances, payment stauses and shipment statuses are re-calculated.</param>
        /// <returns>The result of the save operation</returns>
        /// <remarks>Defaults to not trigger events associated with recaulculation.</remarks>
        public virtual SaveResult Save(bool recalculate)
        {
            return this.Save(recalculate, false);
        }

        /// <summary>
        /// Save this order to database
        /// </summary>
        /// <param name="recalculate">If true balances, payment stauses and shipment statuses are re-calculated.</param>
        /// <param name="triggerPaymentEvents">If true, triggers events associated with recalculated payment events.  This parameter only matters if recalculate is true.</param>
        /// <returns>The result of the save operation</returns>
        public virtual SaveResult Save(bool recalculate, bool triggerPaymentEvents)
        {
            //CHECK WHETHER WE SHOULD RECALCULATE VALUES
            if (recalculate)
            {
                //RECALCULATE STATIC VALUES
                this.RecalculatePaymentStatus(triggerPaymentEvents);
                this.RecalculateShipmentStatus();
            }
            //NEED TO DETECT ANY CHANGES TO ORDER STATUS
            int currentStatus = OrderDataSource.GetOrderStatusId(this.OrderId);
            int newStatus = 0;
            if ((currentStatus != 0) && (currentStatus != this.OrderStatusId))
            {
                //DELAY STATUS UPDATE UNTIL OTHER SAVES ARE PROCESSED
                newStatus = this.OrderStatusId;
                this.OrderStatusId = currentStatus;
            }
            //CALL THE BASE SAVE
            SaveResult result = this.BaseSave();
            //NOW PROCESS STATUS UPDATE IF REQUIRED
            if (newStatus != 0) this.UpdateOrderStatus(newStatus);
            return result;
        }

        /// <summary>
        /// Gets a formatted billing address
        /// </summary>
        /// <param name="isHtml">If true address if formatted in HTML</param>
        /// <returns>Formatted address</returns>
        public string FormatAddress(bool isHtml)
        {
            return FormatAddress(string.Empty, isHtml);
        }

        /// <summary>
        /// Gets a formatted billing address
        /// </summary>
        /// <param name="pattern">The patter to use for formatting the address</param>
        /// <param name="isHtml">If true address if formatted in HTML</param>
        /// <returns>Formatted address</returns>
        public string FormatAddress(string pattern, bool isHtml)
        {
            string name = (this.BillToFirstName + " " + this.BillToLastName).Trim();
            return AddressFormatter.Format(name, this.BillToCompany, this.BillToAddress1, this.BillToAddress2, this.BillToCity, this.BillToProvince, this.BillToPostalCode, this.BillToCountryCode, this.BillToPhone, this.BillToFax, this.BillToEmail, isHtml);
        }


        /// <summary>
        /// Cancel this order.
        /// </summary>
        public void Cancel()
        {
            this.Cancel(true);
        }

        /// <summary>
        /// Cancel this order.
        /// </summary>
        /// <param name="voidPayments">If true, any payments in a voidable state will have be voided.</param>
        public void Cancel(bool voidPayments)
        {
            //VOID ANY INCOMPLETED PAYMENTS
            if (voidPayments)
            {
                int paymentCount = this.Payments.Count;
                for (int i = 0; i < paymentCount; i++)
                {
                    Payment payment = this.Payments[i];
                    if (payment.IsVoidable) payment.Void();
                }
            }
            // CANCEL ANY TAXES FROM INTEGRATED PROVIDERS
            TaxGatewayCollection taxGateways = Token.Instance.Store.TaxGateways;
            foreach (TaxGateway taxGateway in taxGateways)
            {
                ITaxProvider provider = taxGateway.GetProviderInstance();
                if (provider != null)
                {
                    try
                    {
                        provider.Cancel(this);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Could not cancel with the configured tax provider: " + taxGateway.ClassId, ex);
                    }
                }
                else
                {
                    Logger.Error("Could not load the configured tax provider: " + taxGateway.ClassId);
                }
            }
            //TRIGGER THE ORDER CANCELLED EVENT
            StoreEventEngine.OrderCancelled(this);
        }

        /// <summary>
        /// Gets the last public note entered for this order
        /// </summary>
        /// <returns>Returns empty string if no public note is found</returns>
        public string GetLastPublicNote()
        {
            if (this.Notes.Count == 0) return string.Empty;
            for (int i = Notes.Count - 1; i >= 0; i--)
            {
                OrderNote note = Notes[i];
                if (note.NoteType == NoteType.Public)
                {
                    return note.Comment;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the last tracking number entered for this order
        /// </summary>
        /// <returns>Returns null if no tracking number is found</returns>
        public TrackingNumber GetLastTrackingNumber()
        {
            if (Shipments.Count == 0) return null;
            OrderShipment lastShipment = Shipments[Shipments.Count - 1];
            return lastShipment.GetLastTrackingNumber();
        }

        /// <summary>
        /// Update the order status for this order
        /// </summary>
        /// <param name="orderStatusId">Id of the new order status to set</param>
        public void UpdateOrderStatus(int orderStatusId)
        {
            OrderStatus orderStatus = OrderStatusDataSource.Load(orderStatusId);
            this.UpdateOrderStatus(orderStatus);
        }

        /// <summary>
        /// Update the order status for this order
        /// </summary>
        /// <param name="orderStatus">The new order status to set</param>
        public void UpdateOrderStatus(OrderStatus orderStatus)
        {
            if (orderStatus == null) throw new ArgumentNullException("orderStatus");
            if (this.IsDirty) throw new InvalidOperationException("This method can only be called when the object data has not been modified (IsDirty == false)");
            if (orderStatus.OrderStatusId != this.OrderStatusId)
                OrderDataSource.UpdateOrderStatus(this, orderStatus);
        }

        /// <summary>
        /// Does this order contain shippable items?
        /// </summary>
        public bool HasShippableItems
        {
            get
            {
                if (this.Shipments.Count > 0) return true;
                foreach (OrderItem item in this.Items)
                {
                    if ((item.OrderItemType == OrderItemType.Product) && (item.Shippable != Shippable.No)) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// List of all gift certificates attached to this order ( attached to order items of this order)
        /// </summary>
        public GiftCertificateCollection GiftCertificates
        {
            get
            {
                GiftCertificateCollection gcList = new GiftCertificateCollection();
                foreach (OrderItem oi in this.Items)
                {
                    foreach (GiftCertificate gc in oi.GiftCertificates)
                    {
                        if (gc != null) gcList.Add(gc);
                    }
                }
                return gcList;
            }
        }
    }
}
