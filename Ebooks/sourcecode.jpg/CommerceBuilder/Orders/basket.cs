using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Payments;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using CommerceBuilder.Stores;
using CommerceBuilder.Marketing;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using System.ComponentModel;
using System.Web;

namespace CommerceBuilder.Orders
{
    public partial class Basket
    {

        #region Checkout Methods
        /// <summary>
        /// Commits the contents of a basket to an order.  
        /// Commits all of the dynamic details to static order records and returns 
        /// the GUID of the new order.  Following the Checkout procedure, the contents 
        /// of the basket are automatically cleared.
        /// </summary>
        /// <returns>The OrderId of the created order.</returns>
        public CheckoutResponse Checkout(CheckoutRequest checkoutRequest)
        {
            return Checkout(checkoutRequest, true, true);
        }

        /// <summary>
        /// Commits the contents of a basket to an order.  
        /// Commits all of the dynamic details to static order records and returns 
        /// the GUID of the new order.  Following the Checkout procedure, the contents 
        /// of the basket are automatically cleared.
        /// </summary>
        /// <returns>The OrderId of the created order.</returns>
        public CheckoutResponse Checkout(CheckoutRequest checkoutRequest, bool validate)
        {
            return Checkout(checkoutRequest, validate, true);
        }

        /// <summary>
        /// Commits the contents of a basket to an order.  Commits all of the dynamic details to static order records and returns the GUID of the new order.  Following the Checkout procedure, the contents of the basket are automatically cleared.
        /// </summary>
        /// <returns>The OrderId of the created order.</returns>
        public CheckoutResponse Checkout(CheckoutRequest checkoutRequest, bool validate, bool triggerEvents)
        {
            //VALIDATE THE BASKET
            if (validate)
            {
                List<string> warningMessages;
                if (!this.Validate(out warningMessages))
                {
                    return new CheckoutResponse(false, warningMessages);
                }
            }

            //VALIDATE AFFILIATE (DOES NOT NEED TO BE PART OF TRANSACTION)
            this.User.ValidateAffiliate();

            //CONVERT GIFT CERTIFICATE BASKET ITEMS INTO PAYMENT OBJECTS
            //THIS IS THE ONLY ALTERATION TO THE BASKET STATE MADE DURING THE CHECKOUT (IN THE EVENT OF FAILURE)
            //THIS SHOULD BE ACCEPTABLE, SINCE UPON RETRY THE CUSTOMER WOULD EXPECT TO RE-ENTER
            //ANY PAYMENT DETAILS, WHICH INCLUDES GIFT CERTIFICATE PAYMENTS
            List<BasketPaymentItem> giftCertPayments = CheckoutHelper.ConvertGiftCertificateItemsToPayments(this);

            //THIS DATABASE ACTION DOES NOT NEED TO BE PART OF TRANSACTION
            int giftCertPaymentMethodId = 0;
            if (giftCertPayments.Count > 0)
                giftCertPaymentMethodId = PaymentEngine.GetGiftCertificatePaymentMethod().PaymentMethodId;

            //INITIALIZE VARIABLES FOR CHECKOUT
            Database database = Token.Instance.Database;
            Store store = Token.Instance.Store;
            Order order = null;
            try
            {
                //WE MUST ALLOW FOR A RETRY OF THE CHECKOUT TRANSACTION IN THE EVENT OF A DEADLOCK
                bool committed = false;
                int MAX_TRIES = Configuration.AbleCommerceApplicationSection.GetSection().CheckoutRetries;
                for (int i = 1; ((i <= MAX_TRIES) && (!committed)); i++)
                {
                    try
                    {
                        //BEGIN TRANSACTION FOR CHECKOUT
                        database.BeginTransaction(IsolationLevel.RepeatableRead);
                        order = CheckoutHelper.GenerateOrderObject(this);
                        Dictionary<string, int> idLookup = new Dictionary<string, int>();
                        CheckoutHelper.GenerateOrderShipmentObjects(this, order, idLookup);
                        CheckoutHelper.GenerateOrderItemObjects(this, order, idLookup);
                        CheckoutHelper.GenerateKitProducts(this, order, idLookup);
                        CheckoutHelper.GenerateOrderCoupons(this, order);
                        CheckoutHelper.UpdateOrderTotalsAndWishlistCount(order);
                        CheckoutHelper.GenerateOrderItemDigitalGoods(order);
                        CheckoutHelper.SortOrderItemsAndGenerateGiftCertificates(order);
                        Dictionary<int, Subscription[]> orderItemSubscriptions = new Dictionary<int, Subscription[]>();
                        CheckoutHelper.GenerateOrderSubscriptions(order, orderItemSubscriptions);
                        CheckoutHelper.GenerateOrderPayments(order, checkoutRequest, giftCertPayments, giftCertPaymentMethodId, orderItemSubscriptions);
                        order.Save(false);
                        database.CommitTransaction();
                        committed = true;
                    }
                    catch (Exception ex)
                    {
                        database.RollbackTransaction();
                        if (i == MAX_TRIES)
                        {
                            LogCheckoutError(i, false, ex);
                            List<string> warningMessages = new List<string>();
                            warningMessages.Add("An error occurred during the checkout process.  You have not been charged, and the administrator has been notified.");
                            return new CheckoutResponse(false, warningMessages);
                        }
                        else
                        {
                            LogCheckoutError(i, true, ex);
                        }
                    }
                }

                //THE ORDER HAS BEEN RECORDED TO DATABASE AND IS GUARANTEED TO BE NON-NULL
                //PERFORM POST ORDER TASKS THAT DO NOT NEED TO BE PART OF THE TRANSACTION

                //FINALIZE ANY TAXES FROM INTEGRATED PROVIDERS
                CheckoutHelper.CommitTaxes(order);

                //TRIGGER ANY EVENTS
                if (triggerEvents)
                {
                    StoreEventEngine.OrderPlaced(order);
                    // TRIGGER EVENTS FOR ORDERS WITHOUT VALUE (EITHER FROM COUPONS OR NON-PRICED ITEMS)
                    LSDecimal balance = order.GetBalance(false);
                    if (balance == 0) StoreEventEngine.OrderPaid(order);
                    else if (balance < 0) StoreEventEngine.OrderPaidCreditBalance(order);
                }

                //HANDLE POST-ORDER PAYMENT AUTHORIZATIONS
                foreach (Payment payment in order.Payments)
                {
                    if (payment.PaymentMethodId == giftCertPaymentMethodId)
                    {
                        payment.Authorize(false, order.RemoteIP);
                    }
                    else
                    {
                        if ((payment.PaymentMethod != null) && (payment.PaymentMethod.PaymentGateway != null))
                            payment.Authorize(false, order.RemoteIP);
                    }
                }

                //RELOAD ORDER STATUS ID FROM DATABASE TO PRESERVE ANY CHANGES FROM PAYMENT PROCESSING
                order.OrderStatusId = OrderDataSource.GetOrderStatusId(order.OrderId);

                //SAVE ORDER TO RECALCULATE ALL STATUSES
                order.Save(true);
            }
            catch (Exception ex)
            {
                //AN UNHANDED ERROR OCCURRED WHILE SENDING TAX COMMIT TO PROVIDER
                //OR AUTHORIZING PAYMENT VIA GATEWAY, OR RECALCULATING THE ORDER STATUS
                //THE ORDER WAS RECORDED SUCCESSFULLY BUT WILL NEED INTERVENTION TO COMPLETE TASKS
                LogPostCheckoutError(ex, order.OrderId, order.OrderNumber);
            }

            //RESET THE BASKET (FAILURE IS NOT CRITICAL TO THE CHECKOUT)
            try
            {
                Basket.Clear(this, false);
            }
            catch (Exception ex)
            {
                Logger.Info("Could not clear basket for user " + this.UserId, ex);
            }

            //RETURN THE SUCCESSFUL RESULT
            return new CheckoutResponse(true, order.OrderId, order.OrderNumber);
        }

        private void LogCheckoutError(int failCount, bool retry, Exception ex)
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.Append("An error occurred during the checkout process.");
            if (retry) errorMessage.Append("  The checkout will be attempted again.");
            else errorMessage.Append("  The checkout has failed " + failCount.ToString() + " times and will NOT BE attempted again.");
            if (this.User != null)
            {
                errorMessage.Append(" UserId: " + string.Format("{0} / {1}", this.User.UserId, this.User.UserName));
                Address a = this.User.PrimaryAddress;
                if (a != null)
                {
                    errorMessage.Append(" Customer: ");
                    errorMessage.Append(string.Format("{0} / {1} / {2}", a.FullName, this.User.Email, a.Phone));
                }
            }
            if (retry) Logger.Info(errorMessage.ToString(), ex);
            else Logger.Error(errorMessage.ToString(), ex);
        }

        private void LogPostCheckoutError(Exception ex, int orderId, int orderNumber)
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.Append("An error occurred after the checkout process.");
            errorMessage.Append(" OrderId: " + orderId);
            errorMessage.Append(" OrderNumber: " + orderNumber);
            Logger.Error(errorMessage.ToString(), ex);
        }
        #endregion

        #region Package Methods
        /// <summary>
        /// Organizes the basket items into default shipments in preparation for checkout
        /// </summary>
        public void Package()
        {
            Package(false);
        }

        /// <summary>
        /// Organizes the basket items into default shipments in preparation for checkout.
        /// </summary>
        /// <param name="resetGiftOptions">Indicates whether gift options should be cleared before the packaging process.</param>
        public void Package(bool resetGiftOptions)
        {
            Package(resetGiftOptions, true);
        }

        /// <summary>
        /// Organizes the basket items into default shipments in preparation for checkout.
        /// </summary>
        /// <param name="resetGiftOptions">Indicates whether gift options should be cleared before the packaging process.</param>
        /// <param name="doCombine">If <b>true</b> any matching basket items are combined</param>        
        public void Package(bool resetGiftOptions, bool doCombine)
        {
            //MAKE SURE A RECORD EXISTS IN THE AC_BASKETS TABLE
            this.Save();
            //BUILD A LIST OF VALID SHIPMENT IDS
            List<int> validShipmentIds = new List<int>();
            //GET A LOCAL REFERENCE TO SHIPMENTS COLLECTION
            //IMPROVES PERFORMANCE AND ENSURES COLLECTION IS INITIALIZED (BUG 7185)
            BasketShipmentCollection shipments = this.Shipments;
            //PROCEED WITH PACKAGING IF THE BASKET HAS ITEMS
            if (this.Items.Count > 0)
            {
                //IF INDICATED, RESET GIFT OPTIONS SET ON BASKET ITEMS
                if (resetGiftOptions) ResetGiftOptions(false);
                //GET DEFAULT ORIGIN AND DESTINATION
                int defaultWarehouseId = Token.Instance.Store.DefaultWarehouseId;
                int defaultAddressId = this.User.PrimaryAddress.AddressId;
                //LOOP ALL ITEMS IN THE BASKET
                foreach (BasketItem item in this.Items)
                {
                    //CHECK ALL SHIPPABLE PRODUCTS AND CHILD ITEMS
                    if ((item.OrderItemType == OrderItemType.Product || item.IsChildItem) && item.Shippable != Shippable.No)
                    {
                        //GET THE PARENT ITEM (IF APPLICABLE)
                        BasketItem parentItem;
                        if (item.OrderItemType == OrderItemType.Product && item.IsChildItem)
                        {
                            parentItem = item.GetParentItem(true);
                            if (parentItem.Product != null && parentItem.Product.Kit.ItemizeDisplay)
                            {
                                // THIS ITEM SHOULD NOT BE TREATED AS A CHILD FOR PACKAGING
                                parentItem = item;
                            }

                        } else parentItem = item.GetParentItem(false);
                        //If the corresponding product is null (possibly deleted) ignore this item
                        if (parentItem.Product == null) continue;
                        //DETERMINE THE WAREHOUSE ID OF THE ITEM
                        int warehouseId = parentItem.Product.WarehouseId;
                        BasketShipment selectedShipment = Basket.GetShipment(this.BasketId, shipments, warehouseId, defaultAddressId);
                        //RECORD THE SHIPMENT ID THAT WE HAVE USED
                        validShipmentIds.Add(selectedShipment.BasketShipmentId);
                        //UPDATE THE DESTINATION
                        item.BasketShipmentId = selectedShipment.BasketShipmentId;
                    }
                }
                if (doCombine)
                {
                    //COMBINE ANY MATCHING BASKET ITEMS
                    this.Items.Combine(true);
                }
            }
            //DELETE ANY SHIPMENTS THAT WERE NOT USED DURING THE PACKAGING SEQUENCE
            DeleteInvalidShipments(validShipmentIds);
        }

        /// <summary>
        /// Clears any gift option set for items in the basket.
        /// </summary>
        /// <param name="save">Flag indicating whether changes should be saved.</param>
        public void ResetGiftOptions(bool save)
        {
            foreach (BasketItem item in this.Items)
            {
                item.WrapStyleId = 0;
                item.GiftMessage = string.Empty;
            }
            if (save) this.Items.Save();
        }

        /// <summary>
        /// Clears shipments of data relating to origin, destination, shipping method and shipping message.  Also clears basket items of shipment assignments.
        /// </summary>
        /// <remarks>The method does not save changes the basket.  If you wish to persit any changes you must call the Save method.</remarks>
        public void ResetShipments()
        {
            foreach (BasketShipment shipment in this.Shipments)
            {
                shipment.AddressId = 0;
                shipment.WarehouseId = 0;
                shipment.ShipMethodId = 0;
                shipment.ShipMessage = string.Empty;
            }
            foreach (BasketItem item in this.Items)
            {
                item.BasketShipmentId = 0;
            }
        }

        /// <summary>
        /// Gets the shipment that matches the given origin and destination.
        /// </summary>
        /// <param name="basketId">ID of the basket</param>
        /// <param name="shipments">Shipment collection for the basket</param>
        /// <param name="warehouseId">The ID of the origin warehouse.</param>
        /// <param name="addressId">The ID of the destination address.</param>
        /// <returns>The shipment that matches the given origin and destination. If no match is found, a new shipment is added and returned.</returns>
        internal static BasketShipment GetShipment(int basketId, BasketShipmentCollection shipments, int warehouseId, int addressId)
        {
            foreach (BasketShipment shipment in shipments)
            {
                if ((shipment.WarehouseId == warehouseId) || (shipment.WarehouseId == 0))
                {
                    if ((shipment.AddressId == addressId) || (shipment.AddressId == 0))
                    {
                        shipment.WarehouseId = warehouseId;
                        shipment.AddressId = addressId;
                        shipment.Save();
                        return shipment;
                    }
                }
            }
            //IF WE COME THIS FAR, AN EXISTING SHIPMENT WAS NOT AVAILABLE
            //ADD A NEW SHIPMENT
            BasketShipment newShipment = new BasketShipment();
            newShipment.BasketId = basketId;
            newShipment.WarehouseId = warehouseId;
            newShipment.AddressId = addressId;
            shipments.Add(newShipment);
            newShipment.Save();
            return newShipment;
        }

        /// <summary>
        /// Deletes any shipments in this basket that do not appear in the list of valid shipments.
        /// </summary>
        internal void DeleteInvalidShipments(List<int> validShipmentIds)
        {
            for (int i = this.Shipments.Count - 1; i >= 0; i--)
            {
                BasketShipment testShipment = this.Shipments[i];
                if (validShipmentIds.IndexOf(testShipment.BasketShipmentId) < 0) this.Shipments.DeleteAt(i);
            }
        }

        /// <summary>
        /// Has this basket been packaged
        /// </summary>
        public bool IsPackaged
        {
            get
            {
                // LOOP ALL ITEMS
                foreach (BasketItem item in Items)
                {
                    // IS THIS ITEM SHIPPABLE?
                    if (item.Shippable != Shippable.No)
                    {
                        // IS THIS ITEM IN A SHIPMENT?  IF NOT, BASKET IS NOT PACKAGED
                        if (item.BasketShipmentId == 0) return false;
                    }
                }
                // EITEHR ALL SHIPPABLE ITEMS ARE IN SHIPMENTS OR THERE ARE NO SHIPPABLE ITEMS
                // BASKET IS CONSIDERED PACKAGED
                return true;
            }
        }
        #endregion

        /// <summary>
        /// Completely clears all contents of the basket.
        /// </summary>
        public void Clear()
        {
            Basket.Clear(this, true);
        }

        /// <summary>
        /// Completely clears all contents of the basket.
        /// </summary>
        /// <param name="basket">The basket to clear</param>
        /// <param name="cancelTaxes">If true, any pending tax items are cancelled through the 
        /// provider before the basket is cleared.</param>
        private static void Clear(Basket basket, bool cancelTaxes)
        {
            if (cancelTaxes)
            {
                //CANCEL ANY TAXES FROM INTEGRATED PROVIDERS
                TaxGatewayCollection taxGateways = Token.Instance.Store.TaxGateways;
                foreach (TaxGateway taxGateway in taxGateways)
                {
                    ITaxProvider provider = taxGateway.GetProviderInstance();
                    if (provider != null)
                    {
                        try
                        {
                            provider.Cancel(basket);
                        }
                        catch { }
                    }
                }
            }
            Database database = Token.Instance.Database;
            //DELETE BASKETITEMS
            DbCommand command = database.GetSqlStringCommand("DELETE FROM ac_BasketItems WHERE BasketId = @basketId");
            database.AddInParameter(command, "@basketId", System.Data.DbType.Int32, basket.BasketId);
            database.ExecuteNonQuery(command);
            //DELETE BASKETSHIPMENTS
            command = database.GetSqlStringCommand("DELETE FROM ac_BasketShipments WHERE BasketId = @basketId");
            database.AddInParameter(command, "@basketId", System.Data.DbType.Int32, basket.BasketId);
            database.ExecuteNonQuery(command);
            //DELETE COUPONS
            command = database.GetSqlStringCommand("DELETE FROM ac_BasketCoupons WHERE BasketId = @basketId");
            database.AddInParameter(command, "@basketId", System.Data.DbType.Int32, basket.BasketId);
            database.ExecuteNonQuery(command);
            //CLEAR THE IN MEMORY COLLECTIONS
            basket.Items.Clear();
            basket.Shipments.Clear();
            basket.BasketCoupons.Clear();
        }

        /// <summary>
        /// Moves the source basket to the target basket if the source basket is not empty.
        /// </summary>
        /// <param name="sourceId">The ID of the source basket.</param>
        /// <param name="targetId">The ID of the target basket.</param>
        /// <remarks>Any existing contents of the target basket are removed prior to transfer.</remarks>
        public static void Transfer(int sourceId, int targetId)
        {
            Transfer(sourceId, targetId, false);
        }

        /// <summary>
        /// Moves the source basket to the target basket.
        /// </summary>
        /// <param name="sourceId">The ID of the user with the source basket.</param>
        /// <param name="targetId">The ID of the user to transfer the basket to.</param>
        /// <param name="transferEmptyBasket">If false, the basket is not transferred when the source basket is empty.  If true, the basket is always transferred.</param>
        /// <remarks>Any existing contents of the target basket are removed prior to transfer.</remarks>
        public static void Transfer(int sourceId, int targetId, bool transferEmptyBasket)
        {
            if (sourceId != targetId)
            {
                //GET THE DEFAULT BASKET FOR THE SOURCE USER
                BasketCollection sourceBaskets = BasketDataSource.LoadForUser(sourceId);
                if (sourceBaskets.Count == 0) return;
                Basket sourceBasket = sourceBaskets[0];
                if (!transferEmptyBasket)
                {
                    //WE SHOULD NOT TRANSFER EMPTY BASKETS, COUNT THE SOURCE ITEMS
                    int sourceCount = BasketItemDataSource.CountForBasket(sourceBasket.BasketId);
                    if (sourceCount == 0) return;
                }
                //MAKE SURE TARGET USER HAS NO BASKETS
                Database database = Token.Instance.Database;
                DbCommand command = database.GetSqlStringCommand("DELETE FROM ac_Baskets WHERE UserId = @targetId");
                database.AddInParameter(command, "@targetId", System.Data.DbType.Int32, targetId);
                database.ExecuteNonQuery(command);
                //RESET SHIPMENT ASSIGNMENTS FOR SOURCE ITEMS
                database = Token.Instance.Database;
                command = database.GetSqlStringCommand("UPDATE ac_BasketItems SET BasketShipmentId = NULL WHERE BasketId = @sourceId");
                database.AddInParameter(command, "@sourceId", System.Data.DbType.Int32, sourceBasket.BasketId);
                database.ExecuteNonQuery(command);
                //NOW REMOVE SHIPMENTS FROM SOURCE BASKET (bug 5598)
                database = Token.Instance.Database;
                command = database.GetSqlStringCommand("DELETE FROM ac_BasketShipments WHERE BasketId = @sourceId");
                database.AddInParameter(command, "@sourceId", System.Data.DbType.Int32, sourceBasket.BasketId);
                database.ExecuteNonQuery(command);
                //NOW MOVE SOURCE BASKET TO TARGET USER
                database = Token.Instance.Database;
                command = database.GetSqlStringCommand("UPDATE ac_Baskets SET UserId = @targetId WHERE BasketId = @sourceId");
                database.AddInParameter(command, "@targetId", System.Data.DbType.Int32, targetId);
                database.AddInParameter(command, "@sourceId", System.Data.DbType.Int32, sourceBasket.BasketId);
                database.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Combines any shipments or items that have equivalent properties.
        /// </summary>
        public void Combine()
        {
            //COMBINE THE SHIPMENTS
            this.Shipments.Combine(true);
            //COMBINE THE BASKET ITEMS
            this.Items.Combine(true);
        }

        /// <summary>
        /// Recalculates shipping, coupons, discounts, taxes for the basket.
        /// </summary>
        public void Recalculate()
        {
            // RECALCULATE ITEMS IN THE BASKET
            this.Items.Recalculate();
            // CALCULATE GIFTWRAP CHARGES
            GiftWrapCalculator.Calculate(this);
            // RECALCULATE DISCOUNTS
            DiscountCalculator.Calculate(this);
            // COUPONS MUST BE CALCULATED PRIOR TO SHIPPING IN THE EVENT SHIPPING 
            // IS BASED ON ORDER TOTAL
            CouponCalculator.ProcessBasket(this, false);
            // RECALCULATE SHIPPING
            ShipRateCalculator.Calculate(this);
            // COUPONS MUST FOLLOW DISCOUNTS (SO PRODUCT COUPONS CALCULATE ON CORRECT VALUE - BUG 6243)
            // COUPONS MUST ADDITIONALLY BE CALCULATED AFTER SHIPPING IN THE EVENT THERE
            // IS A SHIPPING COUPON PRESENT
            CouponCalculator.ProcessBasket(this, false);
            //TAXES MUST FOLLOW ALL OTHER CALCULATIONS
            TaxCalculator.Calculate(this);
            //MAKE SURE NO GIFT CERTIFICATES ARE MARKED FOR MORE THAN BASKET VALUE
            ValidateGiftCertificates(this);
        }

        private static void ValidateGiftCertificates(Basket basket)
        {
            //GET TOTAL OF ALL ITEMS EXCEPT GIFT CERTIFICATES
            LSDecimal basketTotal = 0;
            foreach (BasketItem bi in basket.Items)
            {
                if (bi.OrderItemType != OrderItemType.GiftCertificatePayment)
                {
                    basketTotal += bi.ExtendedPrice;
                }
            }
            //MAKE SURE GIFT CERTIFICATES DO NOT EXCEED BASKET TOTAL
            LSDecimal giftCertTotal = 0;
            foreach (BasketItem bi in basket.Items)
            {
                if (bi.OrderItemType == OrderItemType.GiftCertificatePayment)
                {
                    //DETERMINE THE VALUE OF THIS GIFT CERTIFICATE TO APPLY
                    GiftCertificate gc = GiftCertificateDataSource.LoadForSerialNumber(bi.Sku);
                    bi.Price = (-1 * gc.Balance);
                    giftCertTotal += gc.Balance;
                    if (giftCertTotal > basketTotal)
                    {
                        //LOWER THE GIFT CERTIFICATE PAYMENT TO THE BASKET TOTAL
                        bi.Price += (giftCertTotal - basketTotal);
                        bi.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes an item from the basket, including all associated child items.
        /// </summary>
        /// <param name="basketItemId">The ID of the item to remove</param>
        public void DeleteItem(int basketItemId)
        {
            // LOCATE THE ITEM REFERENCE
            int index = this.Items.IndexOf(basketItemId);
            if (index > -1)
            {
                // GET A COMPLETE PARENT PATH AS WE MUST REMOVE ALL ITEMS IN THE PATH
                List<int> path = this.Items[index].GetPath();
                for (int i = this.Items.Count - 1; i >= 0; i--)
                {
                    BasketItem item = this.Items[i];
                    if (path.Contains(item.BasketItemId))
                    {
                        this.Items.DeleteAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Validates the contents of this basket
        /// </summary>
        /// <param name="warningMessages">contains any validation errors or warning messages</param>
        /// <returns><b>true</b> if there are no validation errors, <b>false</b> otherwise</returns>
        public bool Validate(out List<string> warningMessages)
        {
            // INITIALIZE THE WARNING MESSAGES FOR RETURN
            warningMessages = new List<string>();

            // IF STORE IS IN CATALOG MODE, THE BASKET SHOULD BE EMPTIED
            // NO WARNING MESSAGE IS RETURNED SO THAT USER IS NOT FORCED TO BASKET SCREEN
            if (Store.GetCachedSettings().ProductPurchasingDisabled)
            {
                this.Items.DeleteAll();
                return true;
            }

            // CATALOG MODE IS NOT ENABLED, VALIDATE BASKET CONTENTS
            string formattedMessage;
            List<int> invalidBasketItems = new List<int>();
            foreach (BasketItem item in this.Items)
            {
                // FIRST OBTAIN THE ROOT PARENT ITEM, AS SOME VALIDATION MUST BE DONE AGAINST IT
                BasketItem parentItem = item.GetParentItem(true);
                if (item.OrderItemType == OrderItemType.Product)
                {
                    if (item.Product == null || parentItem.Product.Visibility == CatalogVisibility.Private)
                    {
                        // EITHER THE PRODUCT IS INVALID
                        // OR THE PARENT ITEM IS NOT VISIBLE IN THE CATALOG
                        if (item.IsChildItem) formattedMessage = string.Format(Properties.Resources.BasketChildItemUnavailable, item.Name, parentItem.Name);
                        else formattedMessage = string.Format(Properties.Resources.BasketItemUnavailable, item.Name);
                        // THE PARENT ITEM IS UNAVAILABLE, ENSURE WE DO NOT DUPLICATE THE MESSAGE
                        if (!warningMessages.Contains(formattedMessage)) warningMessages.Add(formattedMessage);
                        // MARK THIS ITEM FOR REMOVAL
                        invalidBasketItems.Add(item.BasketItemId);
                    }
                    else
                    {
                        // DOES THIS ITEM HAVE PRODUCT OPTIONS ASSOCIATED?
                        if (item.Product.ProductOptions.Count > 0)
                        {
                            // CHECK WHETHER A VALID VARIANT IS SPECIFIED FOR THIS BASKET ITEM
                            ProductVariant pv = item.ProductVariant;
                            if (pv == null)
                            {
                                // VARIANT COULD NOT BE DETERMINED
                                if (item.IsChildItem) formattedMessage = string.Format(Properties.Resources.BasketChildItemUnavailable, item.Name, parentItem.Name);
                                else formattedMessage = string.Format(Properties.Resources.BasketItemUnavailable, item.Name);
                                if (!warningMessages.Contains(formattedMessage)) warningMessages.Add(formattedMessage);
                                invalidBasketItems.Add(item.BasketItemId);
                            }
                            else if (!pv.Available)
                            {
                                // VARIANT IS MARKED AS UNAVAILABLE
                                if (item.IsChildItem) formattedMessage = string.Format(Properties.Resources.BasketChildItemUnavailable, item.Name + " (" + pv.VariantName + ")", parentItem.Name);
                                else formattedMessage = string.Format(Properties.Resources.BasketItemUnavailable, item.Name);
                                if (!warningMessages.Contains(formattedMessage)) warningMessages.Add(formattedMessage);
                                invalidBasketItems.Add(item.BasketItemId);
                            }
                        }
                        else
                        {
                            // THE ITEM SHOULD NOT HAVE AN OPTION LIST ASSOCIATED
                            item.OptionList = string.Empty;
                            item.Save();
                        }

                        // VALIDATE KIT
                        if (item.Product.KitStatus == KitStatus.Master)
                        {
                            Kit kit = item.Product.Kit;
                            string originalList = item.KitList;
                            item.KitList = kit.RefreshKitProducts(item.KitList);
                            bool validKit = item.Product.Kit.ValidateChoices(item.KitList);
                            if (!validKit)
                            {
                                formattedMessage = string.Format(Properties.Resources.BasketItemUnavailable, item.Name);
                                if (!warningMessages.Contains(formattedMessage)) warningMessages.Add(formattedMessage);
                                invalidBasketItems.Add(item.BasketItemId);
                            }
                            else if (originalList != item.KitList)
                            {
                                formattedMessage = string.Format(Properties.Resources.BasketItemModified, item.Name);
                                if (!warningMessages.Contains(formattedMessage)) warningMessages.Add(formattedMessage);
                                item.Save();
                            }
                        }
                    }
                }
            }

            // NOW REMOVE ALL ITEMS MARKED AS INVALID (AND ALL LINKED ITEMS OF SUCH)
            foreach (int invalidBasketItemId in invalidBasketItems)
            {
                this.DeleteItem(invalidBasketItemId);
            }
            
            //VALIDATE INVENTORY
            Store store = Token.Instance.Store;
            if (store.EnableInventory)
            {
                Dictionary<string, InventoryInfo> inventories = new Dictionary<string, InventoryInfo>();
                string tempMessage;
                InventoryInfo info;
                foreach (BasketItem item in this.Items)
                {
                    if (item.OrderItemType != OrderItemType.Product || item.IsChildItem) continue;
                    info = GetInventoryInfo(inventories, item);
                    bool enforceInv = info.InventoryStatus.InventoryMode != InventoryMode.None 
                        && info.InventoryStatus.AllowBackorder == false;
                    if (enforceInv)
                    {
                        //inventory needs to be enforced                        
                        if (item.Quantity > info.NowAvailable)
                        {
                            tempMessage = GetInventoryStockMessage(item, info);
                            if (info.NowAvailable < 1) item.Quantity = 0;
                            else item.Quantity = (short)info.NowAvailable;
                            warningMessages.Add(tempMessage);
                        }

                        //if this was a kit product and some quanity of it getting included
                        //update the inventories for all its component products
                        if (item.Product.KitStatus == KitStatus.Master && item.Quantity > 0)
                        {
                            InventoryInfo info1;
                            foreach (InventoryManagerData invd1 in info.InventoryStatus.ChildItemInventoryData)
                            {
                                info1 = inventories[invd1.ProductId + "_" + invd1.OptionList + "_"];
                                if (invd1.InventoryMode != InventoryMode.None
                                    && invd1.AllowBackorder == false)
                                {
                                    //ensure that none of the component product exceeds inventory
                                    int efQty = item.Quantity * invd1.Multiplier;
                                    if (efQty > info1.NowAvailable)
                                    {
                                        tempMessage = GetInventoryStockMessage(item, info1);
                                        if (info1.NowAvailable < 1) item.Quantity = 0;
                                        else item.Quantity = (short)(info1.NowAvailable/invd1.Multiplier);
                                        warningMessages.Add(tempMessage);
                                    }
                                    info1.NowAvailable -= item.Quantity * invd1.Multiplier;
                                    inventories[invd1.ProductId + "_" + invd1.OptionList + "_"] = info1;
                                }
                            }
                        }

                        info.NowAvailable -= item.Quantity;
                    }

                    int curQty = item.Quantity;
                    tempMessage = ValidateMinMaxLimits(item, info);                    
                    if (!string.IsNullOrEmpty(tempMessage))
                    {
                        warningMessages.Add(tempMessage);
                        if (enforceInv)
                        {
                            info.NowAvailable += (curQty - item.Quantity);
                        }
                    }
                    
                    item.Save();
                }
                
                for (int i = (Items.Count - 1); i >= 0; i--)
                {
                    BasketItem bitem = this.Items[i];
                    if (bitem.Quantity < 1)
                    {
                        this.Items.DeleteAt(i);
                    }
                }
            }
            
            if (warningMessages.Count > 0) this.Recalculate();
            return (warningMessages.Count == 0);
        }

        private string GetInventoryStockMessage(BasketItem item, InventoryInfo info)
        {
            string message = "";
            if (info.NowAvailable < 1)
            {
                string tempItemName = (item.ProductVariant == null) ? item.Name : tempItemName = item.Name + " (" + item.ProductVariant.VariantName + ")";
                if (item.Product.KitStatus == KitStatus.Master)
                {
                    message = string.Format(Properties.Resources.KitBasketItemOutOfStock, tempItemName);
                }
                else
                {
                    message = string.Format(Properties.Resources.BasketItemOutOfStock, tempItemName);
                }
            }
            else
            {
                string tempItemName = (item.ProductVariant == null) ? item.Name : tempItemName = item.Name + " (" + item.ProductVariant.VariantName + ")";
                if (item.Product.KitStatus == KitStatus.Master)
                {
                    message = string.Format(Properties.Resources.KitBasketItemExceedsAvailableStock, tempItemName);
                }
                else
                {
                    message = string.Format(Properties.Resources.BasketItemExceedsAvailableStock, tempItemName, info.InventoryStatus.InStock);
                }
            }
            return message;
        }

        private InventoryInfo GetInventoryInfo(Dictionary<string, InventoryInfo> inventories, BasketItem item)
        {
            // KEY MUST BE UNIQUE FOR THE PRODUCT (INCLUDING OPTIONS AND KITS)
            string key = item.ProductId + "_" + item.OptionList + "_" + item.KitList;
            if (inventories.ContainsKey(key))
            {
                // RETURN THE INVENTORY INFO FROM CACHE
                return inventories[key];
            }
            else
            {
                // CREATE A NEW INVENTORY INFO STRUCTURE
                InventoryManagerData invd = InventoryManager.CheckStock(item);
                InventoryInfo info = new InventoryInfo(invd);
                inventories[key] = info;
                // POPULATE THE INVENTORY INFO FOR ANY CHILD PRODUCTS
                foreach (InventoryManagerData invd1 in invd.ChildItemInventoryData)
                {
                    // CHILD KEY WILL NOT CONTAIN THE KIT LIST PART AS WE DO NOT ALLOW TO NEXT KITS
                    string childKey = invd1.ProductId + "_" + invd1.OptionList + "_"; 
                    if (!inventories.ContainsKey(childKey))
                        inventories[childKey] = new InventoryInfo(invd1);
                }
                return info;
            }
        }

        private string ValidateMinMaxLimits(BasketItem item, InventoryInfo info)
        {
            if (item == null) return string.Empty;
            string tempMessage = string.Empty;
            if (item.Quantity > 0)
            {
                //validate min/max limits
                short tempMaxQuantity = item.Product.MaxQuantity;
                if ((tempMaxQuantity > 0) && (item.Quantity > tempMaxQuantity))
                {
                    string tempItemName = (item.ProductVariant == null) ? item.Name : tempItemName = item.Name + " (" + item.ProductVariant.VariantName + ")";
                    tempMessage = string.Format(Properties.Resources.BasketItemExceedsMaxQuantity, tempItemName, tempMaxQuantity);
                    item.Quantity = tempMaxQuantity;
                }
                short tempMinQuantity = item.Product.MinQuantity;
                if ((tempMinQuantity > 0) && (item.Quantity < tempMinQuantity))
                {
                    string tempItemName = (item.ProductVariant == null) ? item.Name : item.Name + " (" + item.ProductVariant.VariantName + ")";
                    if (tempMinQuantity <= info.NowAvailable)
                    {
                        tempMessage = string.Format(Properties.Resources.BasketItemBelowMinQuantity, tempItemName, tempMinQuantity);
                        item.Quantity = tempMinQuantity;
                    }
                    else
                    {
                        tempMessage = string.Format(Properties.Resources.BasketItemBelowMinQuantityAboveStock, tempItemName, tempMinQuantity);
                        item.Quantity = 0;
                    }
                }
            }
            return tempMessage;
        }

        private struct InventoryInfo
        {
            //public InventoryInfo() { }
            public InventoryInfo(InventoryManagerData invd)
            {
                InventoryStatus = invd;
                NowAvailable = invd.InStock;
            }            
            public InventoryManagerData InventoryStatus;
            public int NowAvailable;            
        }

        /// <summary>
        /// Saves this Basket object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation</returns>
        public SaveResult Save()
        {
            //AT THIS POINT WE SHOULD SAVE ANONYMOUS USERS
            if (this.UserId == 0)
            {
                //THE BASKET DOES NOT HAVE A USER ID ASSIGNED SO ASSUME IT BELONGS TO CURRENT USER
                //MAKE SURE CURRENT USER IS SAVED TO THE DATABASE
                Token.Instance.User.Save();
                //NOW UPDATE THIS BASKET WITH THE CORRECT USER ID
                this.UserId = Token.Instance.User.UserId;
            }
            return this.BaseSave();
        }

        /// <summary>
        /// Indicates whether a product with given product id exists in this basket
        /// </summary>
        /// <param name="productId">Id of the product to check for</param>
        /// <returns><b>true</b> if product exists, <b>false</b> otherwise</returns>
        public bool ContainsProduct(int productId)
        {
            if (productId <= 0) return false;
            foreach (BasketItem item in this.Items)
            {
                if (item.ProductId == productId && item.OrderItemType == OrderItemType.Product)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Indicates if this basket contains any products in its basket items
        /// </summary>
        /// <returns>true if there are products in basket items false otherwise</returns>
        public bool HasProducts()
        {
            foreach (BasketItem bitem in this.Items)
            {
                if (bitem.OrderItemType == OrderItemType.Product)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a flag indicating whether the basket contains shippable items
        /// </summary>
        [Obsolete("Use Items.HasShippableProducts instead.", false)]
        public bool HasShippableItems
        {
            get
            {
                foreach (BasketItem item in this.Items)
                {
                    if ((item.OrderItemType == OrderItemType.Product) && (item.Shippable != Shippable.No))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Generates a hash of the basket contents
        /// </summary>
        /// <param name="includeUserAddress">Whether to include user's primary address for computing hash?</param>
        /// <param name="includeShipments">Whether to include basket shipments for computing hash?</param>
        /// <param name="includeUserInputFields">Whether to include user input fields for computing hash?</param>
        /// <param name="args">The order item types to include in the content hash.</param>        
        /// <returns>A hash of the basket contents</returns>
        /// <remarks>Compare the hash to determine if the total price or weight of basket contents 
        /// have changed between two points in time.</remarks>
        public string GetContentHash(bool includeUserAddress, bool includeShipments, bool includeUserInputFields, params OrderItemType[] args)
        {
            StringBuilder contents = new StringBuilder();

            if (includeUserAddress && (this.User != null) && (this.User.PrimaryAddress != null))
                contents.Append(this.User.PrimaryAddress.ToString(false));

            if (includeShipments)
            {
                foreach (BasketShipment shipment in this.Shipments)
                {
                    contents.Append(shipment.BasketShipmentId + "|");
                    if (shipment.Address != null) contents.Append(shipment.Address.ToString(false));
                }
            }

            this.Items.Sort(new BasketItemComparer());
            foreach (BasketItem item in this.Items)
            {
                if ((args == null) || (Array.IndexOf(args, item.OrderItemType) > -1))
                {
                    contents.Append(item.ProductId.ToString() + "|");
                    if (includeShipments) contents.Append(item.BasketShipmentId.ToString() + "|");
                    contents.Append(item.Quantity + "|");
                    contents.Append(item.ExtendedPrice.ToString("F2") + "|");
                    contents.Append(item.ExtendedWeight.ToString("F2") + "|");
                    contents.Append(item.ParentItemId + "|");
                    contents.Append(item.OptionList + "|");
                    contents.Append(item.TaxCodeId + "|");
                    contents.Append(item.WrapStyleId + "|");
                    if (includeUserInputFields)
                    {
                        foreach (BasketItemInput bInput in item.Inputs)
                        {
                            contents.Append(bInput.InputFieldId + "|");
                            contents.Append(bInput.InputValue + "|");
                        }
                    }
                }
            }
            return StringHelper.CalculateMD5Hash(contents.ToString());
        }

        /// <summary>
        /// Generates a hash of the basket contents
        /// </summary>
        /// <param name="args">The order item types to include in the content hash.</param>
        /// <returns>A hash of the basket contents</returns>
        /// <remarks>Compare the hash to determine if the total price or weight of basket contents 
        /// have changed between two points in time.</remarks>
        public string GetContentHash(params OrderItemType[] args)
        {
            return GetContentHash(true, true, false, args);
        }

        /// <summary>
        /// Generates a hash of the basket contents
        /// </summary>        
        /// <returns>A hash of the basket contents</returns>
        /// <remarks>Compare the hash to determine if the total price or weight of basket contents 
        /// have changed between two points in time.</remarks>
        public string GetContentHash()
        {
            return GetContentHash(true, true, false, null);
        }
    }
}
