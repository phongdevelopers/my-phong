using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Marketing;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Payments;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Static methods used to process the transactional checkout
    /// </summary>
    internal static class CheckoutHelper
    {
        /// <summary>
        /// Converts basket items that represent gift certificate payments into basket payment objects.
        /// </summary>
        /// <param name="basket">The basket to convert</param>
        /// <returns>A list of BasketPaymentItems generated from the gift certificate basket items.</returns>
        /// <remarks>This step is taken just prior to conversion of basket to order.  The basket is modified by removing the 
        /// gift certificate basket items and converting them to payment placeholders.</remarks>
        internal static List<BasketPaymentItem> ConvertGiftCertificateItemsToPayments(Basket basket)
        {
            List<BasketPaymentItem> giftCertificatePayments = new List<BasketPaymentItem>();
            for (int i = basket.Items.Count - 1; i >= 0; i--)
            {
                BasketItem item = basket.Items[i];
                if (item.OrderItemType == OrderItemType.GiftCertificatePayment)
                {
                    BasketPaymentItem payItem = new BasketPaymentItem();
                    payItem.OrderItemType = OrderItemType.GiftCertificatePayment;
                    payItem.Name = item.Name;
                    payItem.AccountData = item.Sku;
                    payItem.Amount = -item.ExtendedPrice;
                    giftCertificatePayments.Add(payItem);
                    basket.Items.DeleteAt(i);
                }
            }
            return giftCertificatePayments;
        }

        /// <summary>
        /// This creates the base order object / record without child data
        /// </summary>
        /// <param name="basket">The basket to generate an order record for.</param>
        /// <returns>The order generated from the basket</returns>
        /// <remarks>This is the first stage of the checkout sequence, following order
        /// generation all child data must be recorded.  This method makes no alteration
        /// to the basket object.</remarks>
        internal static Order GenerateOrderObject(Basket basket)
        {
            //CREATE THE ORDER RECORD
            Order order = new Order();
            order.OrderNumber = StoreDataSource.GetNextOrderNumber(true);
            order.OrderStatusId = OrderStatusDataSource.GetNewOrderStatus().OrderStatusId;
            order.OrderDate = LocaleHelper.LocalNow;

            //SET USER DATA
            order.UserId = basket.UserId;

            //CHECK USER AFFILIATE
            User user = basket.User;
            Affiliate affiliate = user.Affiliate;
            if (affiliate != null)
            {
                //IF AFFILIATE IS NOT NULL IT MEANS THAT ITS ALREADY VALIDATED BY User.ValidateAffiliate METHOD SO ASSING IT TO THE ORDER 
                order.AffiliateId = affiliate.AffiliateId;
            }

            //SET BILLING ADDRESS
            CheckoutHelper.RecordBillingAddress(user, order);

            //SET VALUES FROM HTTPCONTEXT
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                if (context.Session != null)
                {
                    string url = (string)context.Session["SessionReferrerUrl"];
                    if (!string.IsNullOrEmpty(url)) order.Referrer = StringHelper.Truncate(url, 255);
                }
                order.RemoteIP = StringHelper.Truncate(context.Request.UserHostAddress, 39);
            }

            //SAVE THE ORDER
            order.Save(false);
            return order;
        }

        /// <summary>
        /// Resets the affiliate association for a user
        /// </summary>
        /// <param name="user">The user to reset</param>
        /// <remarks>This is done when the affiliation is invalid or expired.</remarks>
        private static void ResetUserAffiliate(User user)
        {
            bool tempDirty = user.IsDirty;
            Database database = Token.Instance.Database;
            //UPDATE THE ORDER TOTAL CHARGES AND PRODUCT SUBTOTAL
            DbCommand command = database.GetSqlStringCommand("UPDATE ac_Users SET AffiliateId = 0, AffiliateReferralDate = NULL WHERE UserId = @userId");
            database.AddInParameter(command, "@userId", System.Data.DbType.Int32, user.UserId);
            database.ExecuteNonQuery(command);
            user.AffiliateId = 0;
            user.AffiliateReferralDate = DateTime.MinValue;
            user.IsDirty = tempDirty;
        }

        /// <summary>
        /// Transcribes the billing address from the user to the order
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="order">The order being placed</param>
        private static void RecordBillingAddress(User user, Order order)
        {
            //REMOVAL OF DEMO ENFORCMENT IS A VIOLATION OF THE LICENSE AGREEMENT
            if (Token.Instance.License.UseDemoMode)
            {
                order.BillToFirstName = "DEMO";
                order.BillToLastName = "DEMO";
                order.BillToCompany = string.Empty;
                order.BillToAddress1 = "DEMO";
                order.BillToAddress2 = string.Empty;
                order.BillToCity = "DEMO";
                order.BillToProvince = "DEMO";
                order.BillToPostalCode = "DEMO";
                order.BillToCountryCode = "US";
                order.BillToPhone = "000-000-0000";
                order.BillToFax = string.Empty;
                order.BillToEmail = string.Empty;
            }
            else
            {
                Address address = user.PrimaryAddress;
                order.BillToFirstName = address.FirstName;
                order.BillToLastName = address.LastName;
                order.BillToCompany = address.Company;
                order.BillToAddress1 = address.Address1;
                order.BillToAddress2 = address.Address2;
                order.BillToCity = address.City;
                order.BillToProvince = address.Province;
                order.BillToPostalCode = address.PostalCode;
                order.BillToCountryCode = address.CountryCode;
                order.BillToPhone = address.Phone;
                order.BillToFax = address.Fax;
                order.BillToEmail = address.Email;
                if (string.IsNullOrEmpty(order.BillToEmail))
                {
                    if (ValidationHelper.IsValidEmail(user.Email))
                        order.BillToEmail = user.Email;
                    else if (ValidationHelper.IsValidEmail(user.UserName))
                        order.BillToEmail = user.UserName;
                }
            }
        }

        /// <summary>
        /// Generates order shipments from the basket data
        /// </summary>
        /// <param name="basket">The basket checking out</param>
        /// <param name="order">The order being created</param>
        /// <param name="idLookup">A translation table to map basket ids to order ids</param>
        /// <remarks>This method does not modify the basket object</remarks>
        internal static void GenerateOrderShipmentObjects(Basket basket, Order order, Dictionary<string, int> idLookup)
        {
            foreach (BasketShipment bs in basket.Shipments)
            {
                OrderShipment os = new OrderShipment();
                os.OrderId = order.OrderId;
                os.WarehouseId = bs.WarehouseId;
                RecordShippingAddress(bs, os);
                if (bs.ShipMethod != null)
                {
                    os.ShipMethodId = bs.ShipMethodId;
                    os.ShipMethodName = bs.ShipMethod.Name;
                }
                os.ShipMessage = bs.ShipMessage;
                os.Save();
                //TODO: WHY IS THE IF STATEMENT NECESSARY?  IF NEW ORDERSHIPMENT IS CREATED?
                //THIS SHOULD ALWAY BE TRUE AND WILL ALWAYS BE EXECUTED?
                if (order.Shipments.IndexOf(os.OrderShipmentId) < 0) order.Shipments.Add(os);
                idLookup.Add("S" + bs.BasketShipmentId, os.OrderShipmentId);
            }
        }

        /// <summary>
        /// Transcribes shipping address from the basket to the order
        /// </summary>
        /// <param name="bs">Basket shipment object</param>
        /// <param name="os">Order shipment object</param>
        private static void RecordShippingAddress(BasketShipment bs, OrderShipment os)
        {
            //REMOVAL OF DEMO ENFORCMENT IS A VIOLATION OF THE LICENSE AGREEMENT
            if (Token.Instance.License.UseDemoMode)
            {
                os.ShipToFirstName = "DEMO";
                os.ShipToLastName = "DEMO";
                os.ShipToCompany = string.Empty;
                os.ShipToAddress1 = "DEMO";
                os.ShipToAddress2 = string.Empty;
                os.ShipToCity = "DEMO";
                os.ShipToProvince = "DEMO";
                os.ShipToPostalCode = "DEMO";
                os.ShipToCountryCode = "US";
                os.ShipToPhone = "000-000-0000";
                os.ShipToFax = string.Empty;
                os.ShipToEmail = string.Empty;
                os.ShipToResidence = false;
            }
            else
            {
                Address address = bs.Address;
                os.ShipToFirstName = address.FirstName;
                os.ShipToLastName = address.LastName;
                os.ShipToCompany = address.Company;
                os.ShipToAddress1 = address.Address1;
                os.ShipToAddress2 = address.Address2;
                os.ShipToCity = address.City;
                os.ShipToProvince = address.Province;
                os.ShipToPostalCode = address.PostalCode;
                os.ShipToCountryCode = address.CountryCode;
                os.ShipToPhone = address.Phone;
                os.ShipToFax = address.Fax;
                os.ShipToEmail = address.Email;
                os.ShipToResidence = address.Residence;
            }
        }

        /// <summary>
        /// Generates order items from the basket data
        /// </summary>
        /// <param name="basket">The basket checking out</param>
        /// <param name="order">The order being created</param>
        /// <param name="idLookup">A translation table to map basket ids to order ids</param>
        /// <remarks>This method does not modify the basket object</remarks>
        internal static void GenerateOrderItemObjects(Basket basket, Order order, Dictionary<string, int> idLookup)
        {
            //MAKE SURE ITEMS ARE SORTED SO THAT THE IDLOOKUP IS VALID
            //(WE NEED TO PROCESS PARENT ITEMS BEFORE CHILD ITEMS)
            basket.Items.Sort(new BasketItemComparer());
            foreach (BasketItem bi in basket.Items)
            {
                OrderItem oi = new OrderItem();
                if (idLookup.ContainsKey("I" + bi.ParentItemId))
                    oi.ParentItemId = idLookup["I" + bi.ParentItemId];
                oi.OrderId = order.OrderId;
                if (idLookup.ContainsKey("S" + bi.BasketShipmentId))
                    oi.OrderShipmentId = idLookup["S" + bi.BasketShipmentId];
                oi.ProductId = bi.ProductId;
                oi.OptionList = bi.OptionList;
                if (bi.ProductVariant != null)
                {
                    oi.VariantName = bi.ProductVariant.VariantName;
                    if (bi.ProductVariant.CostOfGoods > 0) oi.CostOfGoods = bi.ProductVariant.CostOfGoods;
                }
                oi.TaxCodeId = bi.TaxCodeId;
                oi.ShippableId = bi.ShippableId;
                oi.Name = bi.Name;
                oi.Sku = bi.Sku;
                oi.Price = bi.Price;
                oi.Weight = bi.Weight;
                if (bi.Product != null && oi.CostOfGoods == 0) oi.CostOfGoods = bi.Product.CostOfGoods;
                oi.Quantity = bi.Quantity;
                oi.LineMessage = bi.LineMessage;
                oi.OrderItemTypeId = bi.OrderItemTypeId;
                oi.OrderBy = bi.OrderBy;
                oi.WrapStyleId = bi.WrapStyleId;
                oi.GiftMessage = bi.GiftMessage;
                oi.WishlistItemId = bi.WishlistItemId;
                oi.InventoryStatus = InventoryStatus.None;
                oi.TaxRate = bi.TaxRate;
                oi.TaxAmount = bi.TaxAmount;
                oi.KitList = bi.KitList;
                if (oi.Product != null && oi.Product.Kit.ItemizeDisplay)
                    oi.ItemizeChildProducts = true;
                order.Items.Add(oi);

                // COPY THE CUSTOM FIELDS
                foreach (KeyValuePair<string, string> customField in bi.CustomFields)
                {
                    oi.CustomFields.Add(customField.Key, customField.Value);
                }

                oi.Save();
                idLookup.Add("I" + bi.BasketItemId, oi.OrderItemId);
                //COPY ANY ITEM INPUTS
                foreach (BasketItemInput bii in bi.Inputs)
                {
                    InputField inputField = bii.InputField;
                    if (inputField != null)
                    {
                        OrderItemInput oii = new OrderItemInput();
                        oii.OrderItemId = oi.OrderItemId;
                        oii.IsMerchantField = inputField.IsMerchantField;
                        oii.Name = inputField.Name;
                        oii.InputValue = bii.InputValue;
                        oi.Inputs.Add(oii);
                        oii.Save();
                        // SAVE THE ASSOCIATION AS WELL
                        oi.Inputs.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Converts kit product assoications into order line items for a basket that is 
        /// being finalized.
        /// </summary>
        /// <param name="basket">The basket checking out</param>
        /// <param name="order">The order being created</param>
        /// <param name="idLookup">A translation table to map basket ids to order ids</param>
        internal static void GenerateKitProducts(Basket basket, Order order, Dictionary<string, int> idLookup)
        {
            foreach (BasketItem basketItem in basket.Items)
            {
                if (basketItem.OrderItemType == OrderItemType.Product)
                {
                    int[] kitProductIds = AlwaysConvert.ToIntArray(basketItem.KitList);
                    if (kitProductIds != null && kitProductIds.Length > 0)
                    {
                        //keep track of the price/weight of the master line item
                        //decrement these values for each line item registered
                        LSDecimal masterPrice = basketItem.Price;
                        LSDecimal masterWeight = basketItem.Weight;
                        foreach (int kitProductId in kitProductIds)
                        {
                            // WE ONLY NEED TO GENERATE RECORDS FOR THE HIDDEN ITEMS
                            // VISIBLE KIT MBMER PRODUCTS ARE GENERATED DURING THE BASKET RECALCULATION
                            KitProduct kp = KitProductDataSource.Load(kitProductId);
                            if (kp.KitComponent.InputType == KitInputType.IncludedHidden)
                            {
                                Product p = kp.Product;
                                ProductVariant pv = kp.ProductVariant;
                                OrderItem item = new OrderItem();
                                item.OrderId = order.OrderId;

                                // SET THE PARENT ITEM ID FOR THIS ITEM
                                if (idLookup.ContainsKey("I" + basketItem.BasketItemId))
                                    item.ParentItemId = idLookup["I" + basketItem.BasketItemId];

                                item.OrderItemType = OrderItemType.Product;
                                if (idLookup.ContainsKey("S" + basketItem.BasketShipmentId))
                                    item.OrderShipmentId = idLookup["S" + basketItem.BasketShipmentId];
                                if (idLookup.ContainsKey("I" + basketItem.BasketItemId))
                                    item.ParentItemId = idLookup["I" + basketItem.BasketItemId];
                                item.ProductId = kp.ProductId;
                                item.Name = kp.DisplayName;
                                item.OptionList = kp.OptionList;
                                if (pv != null)
                                {
                                    item.VariantName = pv.VariantName;
                                    item.Sku = pv.Sku;
                                }
                                else
                                {
                                    item.Sku = p.Sku;
                                }
                                item.Quantity = (short)(kp.Quantity * basketItem.Quantity);
                                item.TaxCodeId = p.TaxCodeId;
                                //THE CALCULATED PRICE IS FOR ALL ITEMS (EXT PRICE)
                                //TO GET A LINE ITEM PRICE WE MUST DIVIDE BY QUANTITY
                                item.Price = kp.CalculatedPrice / kp.Quantity;
                                item.Weight = kp.CalculatedWeight / kp.Quantity;
                                item.CostOfGoods = p.CostOfGoods;
                                item.WishlistItemId = basketItem.WishlistItemId;
                                item.WrapStyleId = basketItem.WrapStyleId;
                                item.IsHidden = (kp.KitComponent.InputType == KitInputType.IncludedHidden);
                                
                                //USE PARENT SHIPPABLE STATUS FOR HIDDEN KITTED PRODUCTS
                                item.Shippable = basketItem.Shippable;
                                item.Save();
                                order.Items.Add(item);
                                masterPrice -= kp.CalculatedPrice;
                                masterWeight -= kp.CalculatedWeight;
                            }
                        }

                        //UPDATE THE PRICE OF THE KIT LINE ITEM (BASE PRICE OF PRODUCT LESS KIT PARTS)
                        if (idLookup.ContainsKey("I" + basketItem.BasketItemId))
                        {
                            int index = order.Items.IndexOf(idLookup["I" + basketItem.BasketItemId]);
                            if (index > -1)
                            {
                                order.Items[index].Price = masterPrice;
                                order.Items[index].Weight = masterWeight;
                                order.Items[index].Save();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Transcribes coupons associated with the basket into the new order
        /// </summary>
        /// <param name="basket">The basket checking out</param>
        /// <param name="order">The order being created</param>
        /// <remarks>This method does not modify the basket object</remarks>
        internal static void GenerateOrderCoupons(Basket basket, Order order)
        {
            foreach (BasketCoupon bc in basket.BasketCoupons)
            {
                //THIS MAKES SURE THE COUPON IS STILL VALID IN THE DATABASE?
                //TODO: CHECK BASKETCOUPON.DELETE METHOD FOR DB INTEGRITY RULES
                if (bc.Coupon != null)
                {
                    OrderCoupon oc = new OrderCoupon();
                    oc.OrderId = order.OrderId;
                    oc.CouponCode = bc.Coupon.CouponCode;
                    oc.Save();
                    order.Coupons.Add(oc);
                }
            }
        }

        /// <summary>
        /// Generates digital goods from order items
        /// </summary>
        /// <param name="order">Order to generate digital goods for</param>
        internal static void GenerateOrderItemDigitalGoods(Order order)
        {
            //GENERATE ORDER ITEM DIGITAL GOODS
            List<OrderItemDigitalGood> oidgList = new List<OrderItemDigitalGood>();
            try
            {
                string query = "SELECT OI.OrderItemId, OI.Quantity, PDG.DigitalGoodId FROM ac_OrderItems OI, ac_Products P, ac_ProductDigitalGoods PDG WHERE OrderId = @orderId AND OI.ProductId = P.ProductId AND P.ProductId = PDG.ProductId AND ((PDG.OptionList IS NULL) OR (OI.OptionList = PDG.OptionList))";
                Database database = Token.Instance.Database;
                DbCommand command = database.GetSqlStringCommand(query);
                database.AddInParameter(command, "@orderId", System.Data.DbType.Int32, order.OrderId);
                using (IDataReader dr = database.ExecuteReader(command))
                {
                    while (dr.Read())
                    {
                        int orderItemId = dr.GetInt32(0);
                        short quantity = dr.GetInt16(1);
                        int digitalGoodId = dr.GetInt32(2);
                        for (short i = 0; i < quantity; i++)
                        {
                            OrderItemDigitalGood oidg = new OrderItemDigitalGood();
                            oidg.OrderItemId = orderItemId;
                            oidg.DigitalGoodId = digitalGoodId;
                            oidgList.Add(oidg);
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error generating digital goods manifest for order " + order.OrderId, ex);
            }

            //SAVE THE GENERATED ITEMS TO THE DATABASE
            foreach (OrderItemDigitalGood oidg in oidgList)
            {
                DigitalGood dg = oidg.DigitalGood;
                if (dg != null)
                {
                    if (dg.ActivationMode == ActivationMode.OnOrder)
                    {
                        try
                        {
                            oidg.Activate();
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn("Could not activate digital good " + oidg.DigitalGoodId + " for order " + order.OrderId, ex);
                        }
                    }

                    if (dg.FulfillmentMode == FulfillmentMode.OnOrder)
                    {
                        try
                        {
                            oidg.AcquireSerialKey();                            
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn("Could not fulfill digital good " + oidg.DigitalGoodId + " for order " + order.OrderId, ex);
                        }
                    }
                }
                oidg.Name = dg.Name;
                oidg.Save();
            }
        }

        /// <summary>
        /// Sorts the order items according to the busines rules, and generates
        /// any gift certificate items
        /// </summary>
        /// <param name="order">The order being created</param>
        internal static void SortOrderItemsAndGenerateGiftCertificates(Order order)
        {
            order.Items.Sort(new OrderItemComparer());
            short orderBy = 0;
            foreach (OrderItem oItem in order.Items)
            {
                oItem.GenerateGiftCertificates(false);
                oItem.OrderBy = orderBy;
                oItem.Save();
                orderBy++;
            }
        }

        /// <summary>
        /// Generate subscriptions for order items
        /// </summary>
        /// <param name="order">The order being created</param>
        /// <param name="orderItemSubscriptions">Generated subscriptions are returned in this parameter</param>
        internal static void GenerateOrderSubscriptions(Order order, Dictionary<int, Subscription[]> orderItemSubscriptions)
        {
            //GENERATE (BUT DO NOT ACTIVATE) SUBSCRIPTIONS
            //BUILD A LIST OF RECURRING SUBSCRIPTIONS GROUPED BY ORDER ITEM
            OrderItemCollection subscriptionItems = OrderItemDataSource.LoadSubscriptionItems(order.OrderId);
            if (subscriptionItems.Count > 0)
            {
                foreach (OrderItem orderItem in subscriptionItems)
                {
                    Subscription[] allSubs = orderItem.GenerateSubscriptions(false);
                    if ((allSubs != null) && (allSubs.Length > 0))
                    {
                        List<Subscription> recurringSubs = new List<Subscription>();
                        foreach (Subscription sub in allSubs)
                            if (sub.SubscriptionPlan.IsRecurring) recurringSubs.Add(sub);
                        if (recurringSubs.Count > 0)
                            orderItemSubscriptions.Add(orderItem.OrderItemId, recurringSubs.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Generates payment records for an order
        /// </summary>
        /// <param name="order">The order being created</param>
        /// <param name="checkoutRequest">The checkout request</param>
        /// <param name="giftCertPayments">The collection of gift certificate payments for this order</param>
        /// <param name="giftCertPaymentMethodId">The ID of the gift certificate payment method</param>
        /// <param name="orderItemSubscriptions">Order Item subscriptions</param>
        internal static void GenerateOrderPayments(Order order, CheckoutRequest checkoutRequest, List<BasketPaymentItem> giftCertPayments, int giftCertPaymentMethodId, Dictionary<int, Subscription[]> orderItemSubscriptions)
        {
            //THIS VARIABLE SHOULD ONLY CONTAIN DATA IF POST-CHECKOUT GATEWAY PROCESSING IS REQUIRED
            string saveAccountData = string.Empty;
            //USE A COMMON DATE FOR ALL PAYMENTS REGISTERED
            DateTime paymentDate = LocaleHelper.LocalNow;
            //CONVERT GIFT CERTIFICATE PLACEHOLDERS INTO PAYMENT ITEMS
            LSDecimal totalGiftCertPayment = 0;
            foreach (BasketPaymentItem giftCertItem in giftCertPayments)
            {
                Payment giftCertPayment = giftCertItem.GetPaymentObject();
                totalGiftCertPayment += giftCertPayment.Amount;
                giftCertPayment.OrderId = order.OrderId;
                giftCertPayment.PaymentMethodId = giftCertPaymentMethodId;
                giftCertPayment.PaymentDate = paymentDate;
                order.Payments.Add(giftCertPayment);
                giftCertPayment.Save();
            }
            //IF PAYMENT DATA WAS PASSED WITH CHECKOUT REQUEST, ADD TO ORDER RECORD NOW
            if (checkoutRequest != null && checkoutRequest.Payment != null)
            {
                //BUILD A LIST OF PAYMENTS TO ADD TO THE ORDER BASED ON CONTENTS
                Payment originalPayment = checkoutRequest.Payment;
                //DETERMINE TOTAL PAYMENT REQUIRED FOR ITEMS
                LSDecimal remainingPaymentAmount = order.Items.TotalPrice();
                //PRESERVE ACCOUNT DATA
                saveAccountData = originalPayment.AccountData;
                //DECIDE WHETHER PAYMENTS MUST BE DIVIDED BECAUSE OF ARB SUBSCRIPTIONS
                if (orderItemSubscriptions.Count > 0)
                {
                    //LOOP EACH ORDER ITEM WITH A RECURRING SUBSCRIPTION
                    foreach (int orderItemId in orderItemSubscriptions.Keys)
                    {
                        // THIS STORES THE DISCOUNT TO APPLY TO EACH SUBSCRIPTION PAYMENT
                        LSDecimal subscriptionDiscount = 0;
                        // THIS STORES ADDITIONAL DISCOUNT TO APPLY ONLY TO THE FIRST SUBSCRIPTION
                        LSDecimal firstSubscriptionAdjustment = 0;
                        // GET THE TOTAL DISCOUNTS APPLIED TO THIS ORDER ITEM (VALUE SHOULD BE NEGATIVE)
                        LSDecimal totalItemAdjustments = GetOrderItemAdjustments(order, orderItemId);
                        // IF THERE IS A DISCOUNT, DETERMINE PRO-RATED DISCOUNT FOR EACH PAYMENT (ITEM)
                        if (totalItemAdjustments != 0)
                        {
                            // THERE IS A DISCOUNT THAT APPLIES TO THIS ORDER ITEM, WE NEED TO DETERMINE AMOUNT
                            // SUBSCRIPTION DISCOUNT IS TOTAL DISCOUNT BY THE NUMBER OF SUBSCRIPTION ITEMS
                            subscriptionDiscount = (LSDecimal)Math.Floor((decimal)(totalItemAdjustments / orderItemSubscriptions[orderItemId].Length));
                            // DETERMINE THE TOTAL DISCOUNT AMOUNT USING CALCULATED PER-SUBSCRIPTION VALUE
                            LSDecimal calculatedTotal = subscriptionDiscount * orderItemSubscriptions[orderItemId].Length;
                            // THE TOTAL DISCOUNT MUST MATCH THE LUMP SUM, SO CALCULATE ANY LEFTOVER DISCOUNT FOR THE FIRST PAYMENT
                            firstSubscriptionAdjustment = (totalItemAdjustments - calculatedTotal);
                        }

                        // WE MUST DETERMINE THE PRICE OF THE ITEM (INCLUDING TAX)
                        // GET THE ORDER ITEM FOR THIS RECURRING SUBSCRIPTION
                        OrderItem orderItem = order.Items[order.Items.IndexOf(orderItemId)];
                        // GET THE PRICE OF THE ITEM (INCLUDING TAX)
                        int billToProvinceId = ProvinceDataSource.GetProvinceIdByName(order.BillToCountryCode, order.BillToProvince);
                        TaxAddress billingAddress = new TaxAddress(order.BillToCountryCode, billToProvinceId, order.BillToPostalCode);
                        LSDecimal subscriptionPrice = TaxHelper.GetPriceWithTax(orderItem.Price + subscriptionDiscount, orderItem.TaxCodeId, billingAddress, billingAddress);

                        // LOOP ALL THE SUBSCRIPTIONS AND CREATE CORRESPONDING PAYMENT ITEMS
                        foreach (Subscription s in orderItemSubscriptions[orderItemId])
                        {
                            // ADD PAYMENT ITEM ASSOCIATED WITH THE SUBSCRIPTION
                            Payment arbPayment = new Payment();
                            arbPayment.SubscriptionId = s.SubscriptionId;
                            arbPayment.Amount = subscriptionPrice + firstSubscriptionAdjustment;
                            arbPayment.CurrencyCode = originalPayment.CurrencyCode;
                            arbPayment.OrderId = order.OrderId;
                            arbPayment.PaymentDate = paymentDate;
                            arbPayment.PaymentMethodId = originalPayment.PaymentMethodId;
                            arbPayment.PaymentMethodName = originalPayment.PaymentMethodName;
                            arbPayment.PaymentStatus = PaymentStatus.Unprocessed;
                            arbPayment.ReferenceNumber = originalPayment.ReferenceNumber;
                            arbPayment.AccountData = saveAccountData;
                            order.Payments.Add(arbPayment);
                            arbPayment.Save();

                            //ACCOUNT DATA IS RESET AFTER SAVE IN CASE THE SAVE METHOD ALTERS IT BASED 
                            //ON MERCHANT SECURITY SETTINGS, WE STILL NEED THE VALUE FOR THE CHECKOUT PROCESS
                            arbPayment.AccountData = saveAccountData;
                            arbPayment.IsDirty = false;

                            // SUBTRACT THIS PAYMENT FROM THE REMAINING TOTAL
                            remainingPaymentAmount -= arbPayment.Amount;

                            // RESET ADJUSTMENT THAT SHOULD APPLY TO FIRST SUBSCRIPTION ONLY
                            firstSubscriptionAdjustment = 0;
                        }
                    }

                    //CREATE AN ADDITIONAL PAYMENT IF THERE IS ANY AMOUNT LEFT TO BE COLLECTED
                    if (remainingPaymentAmount > 0)
                    {
                        //NEED ONE PAYMENT FOR EACH SUBSCRIPTION
                        Payment remainingPayment = new Payment();
                        remainingPayment.SubscriptionId = 0;
                        if (remainingPaymentAmount >= originalPayment.Amount)
                        {
                            remainingPayment.Amount = originalPayment.Amount;
                        }
                        else
                        {
                            remainingPayment.Amount = remainingPaymentAmount;
                        }
                        remainingPayment.CurrencyCode = originalPayment.CurrencyCode;
                        remainingPayment.OrderId = order.OrderId;
                        remainingPayment.PaymentDate = paymentDate;
                        remainingPayment.PaymentMethodId = originalPayment.PaymentMethodId;
                        remainingPayment.PaymentMethodName = originalPayment.PaymentMethodName;
                        remainingPayment.PaymentStatus = PaymentStatus.Unprocessed;
                        remainingPayment.ReferenceNumber = originalPayment.ReferenceNumber;
                        remainingPayment.AccountData = saveAccountData;
                        order.Payments.Add(remainingPayment);
                        remainingPayment.Save();
                        //ACCOUNT DATA IS RESET AFTER SAVE IN CASE THE SAVE METHOD ALTERS IT BASED 
                        //ON MERCHANT SECURITY SETTINGS, WE STILL NEED THE VALUE FOR THE CHECKOUT PROCESS
                        remainingPayment.AccountData = saveAccountData;
                        remainingPayment.IsDirty = false;
                    }
                }
                else
                {
                    originalPayment.PaymentId = 0;
                    originalPayment.SubscriptionId = 0;
                    originalPayment.PaymentStatus = PaymentStatus.Unprocessed;
                    originalPayment.OrderId = order.OrderId;
                    originalPayment.PaymentDate = paymentDate;
                    order.Payments.Add(originalPayment);
                    originalPayment.Save();
                    //ACCOUNT DATA IS RESET AFTER SAVE IN CASE THE SAVE METHOD ALTERS IT BASED 
                    //ON MERCHANT SECURITY SETTINGS, WE STILL NEED THE VALUE FOR THE CHECKOUT PROCESS
                    originalPayment.AccountData = saveAccountData;
                    originalPayment.IsDirty = false;
                }
            }
        }

        /// <summary>
        /// Recalculates stored values in the database to ensure they are correct
        /// </summary>
        /// <param name="order">The order to recalculate</param>
        internal static void UpdateOrderTotalsAndWishlistCount(Order order)
        {
            Database database = Token.Instance.Database;
            //UPDATE THE ORDER TOTAL CHARGES AND PRODUCT SUBTOTAL
            StringBuilder query = new StringBuilder();
            query.Append("UPDATE ac_Orders SET TotalCharges = (SELECT SUM(Quantity * Price) As TotalCharges FROM ac_OrderItems WHERE OrderId = @orderId)");
            query.Append(", ProductSubtotal = @productSubtotal WHERE OrderId = @orderId");
            DbCommand command = database.GetSqlStringCommand(query.ToString());
            database.AddInParameter(command, "@productSubtotal", System.Data.DbType.Decimal, order.GetProductSubtotal());
            database.AddInParameter(command, "@orderId", System.Data.DbType.Int32, order.OrderId);
            database.ExecuteNonQuery(command);
            //UPDATE THE WISHLISTITEM COUNTS
            query = new StringBuilder();
            query.Append("UPDATE W SET W.Received = W.Received + O.Quantity");
            query.Append(" FROM ac_WishlistItems W, ac_OrderItems O");
            query.Append(" WHERE W.WishlistItemId=O.WishlistItemId AND O.OrderId = @orderId");
            command = database.GetSqlStringCommand(query.ToString());
            database.AddInParameter(command, "@orderId", System.Data.DbType.Int32, order.OrderId);
            database.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Commits taxes after an order is placed
        /// </summary>
        /// <param name="order">The order that was placed</param>
        /// <remarks>This step is necessary for some providers such as CertiTAX for the purposes
        /// of tax reporting.  The native AbleCommerce provider does not require this step.</remarks>
        internal static void CommitTaxes(Order order)
        {
            TaxGatewayCollection taxGateways = Token.Instance.Store.TaxGateways;
            foreach (TaxGateway taxGateway in taxGateways)
            {
                ITaxProvider provider = taxGateway.GetProviderInstance();
                if (provider != null) provider.Commit(order);
            }
        }

        private static OrderItemType[] adjustmentTypes = new OrderItemType[] { OrderItemType.Charge, OrderItemType.Coupon, OrderItemType.Credit, OrderItemType.Discount };

        /// <summary>
        /// Gets the total adjustments 
        /// </summary>
        /// <param name="order">The order</param>
        /// <param name="orderItemId">The item to find adjustments to</param>
        /// <returns>The total of all child item adjustments</returns>
        private static LSDecimal GetOrderItemAdjustments(Order order, int orderItemId)
        {
            LSDecimal totalAdjustment = 0;
            foreach (OrderItem item in order.Items)
            {
                if (item.IsChildItem
                    && item.ParentItemId == orderItemId
                    && (Array.IndexOf(adjustmentTypes, item.OrderItemType) > -1))
                {
                    totalAdjustment += item.ExtendedPrice;
                }
            }
            return totalAdjustment;
        }
    }
}
