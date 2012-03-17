using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Shipping;

using CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen;
using CommerceBuilder.Products;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class AcHelper
    {
        public static CommerceBuilder.Users.Address GetAcAddress(CommerceBuilder.Users.User user, CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen.Address gcAddress)
        {
            if (user == null || user.Addresses.Count == 0) return GetAcAddress(gcAddress);
            string gcAddressHash = GetAddressHash(gcAddress);
            foreach (Users.Address addr in user.Addresses)
            {
                if (gcAddressHash == GetAddressHash(addr)) return addr;
            }
            return GetAcAddress(gcAddress);
        }

        private static string GetAddressHash(AutoGen.Address gcAddress)
        {
            StringBuilder hash = new StringBuilder();
            hash.Append(gcAddress.countrycode.ToLowerInvariant());
            hash.Append(gcAddress.region.ToLowerInvariant());
            hash.Append(gcAddress.city.ToLowerInvariant());
            hash.Append(gcAddress.postalcode.ToLowerInvariant());
            hash.Append(gcAddress.address1.ToLowerInvariant());
            hash.Append(gcAddress.address2.ToLowerInvariant());
            hash.Append(gcAddress.companyname.ToLowerInvariant());
            hash.Append(gcAddress.contactname.ToLowerInvariant());
            hash.Append(gcAddress.email.ToLowerInvariant());
            hash.Append(gcAddress.phone.ToLowerInvariant());
            hash.Append(gcAddress.fax.ToLowerInvariant());
            return hash.ToString();
        }

        private static string GetAddressHash(Users.Address acAddress)
        {
            StringBuilder hash = new StringBuilder();
            hash.Append(acAddress.CountryCode.ToLowerInvariant());
            hash.Append(acAddress.Province.ToLowerInvariant());
            hash.Append(acAddress.City.ToLowerInvariant());
            hash.Append(acAddress.PostalCode.ToLowerInvariant());
            hash.Append(acAddress.Address1.ToLowerInvariant());
            hash.Append(acAddress.Address2.ToLowerInvariant());
            hash.Append(acAddress.Company.ToLowerInvariant());
            hash.Append(acAddress.FullName.ToLowerInvariant());
            hash.Append(acAddress.Email.ToLowerInvariant());
            hash.Append(acAddress.Phone.ToLowerInvariant());
            hash.Append(acAddress.Fax.ToLowerInvariant());
            return hash.ToString();
        }

        public static CommerceBuilder.Users.Address GetAcAddress(CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen.Address gcAddress)
        {
            CommerceBuilder.Users.Address addr = new CommerceBuilder.Users.Address();
            addr.Address1 = gcAddress.address1;
            addr.Address2 = gcAddress.address2;
            addr.City = gcAddress.city;
            addr.Company = gcAddress.companyname;
            addr.CountryCode = gcAddress.countrycode;
            addr.FullName = gcAddress.contactname;
            addr.Email = gcAddress.email;
            addr.Fax = gcAddress.fax;
            addr.Phone = gcAddress.phone;
            addr.PostalCode = gcAddress.postalcode;
            addr.Province = gcAddress.region;
            //TODO
            //GoogleCheckout does not tell us whether an address is residence or commercial.
            //We assume by default that it is commercial. This should probably be a 
            //configuration parameter
            addr.Residence = false;
            return addr;
        }

        public static Users.Address GetAnonAcAddress(Users.User user, MerchantCalculation.AnonymousAddress anonAddress)
        {
            if (user == null || user.Addresses.Count == 0) return GetAnonAcAddress(anonAddress);
            string anonAddressHash = GetAnonAddressHash(anonAddress);
            foreach (Users.Address addr in user.Addresses)
            {
                if (anonAddressHash == GetAnonAddressHash(addr)) return addr;
            }
            return GetAnonAcAddress(anonAddress);
        }
                
        private static string GetAnonAddressHash(Users.Address acAddress)
        {
            StringBuilder hash = new StringBuilder();
            hash.Append(acAddress.CountryCode.ToLowerInvariant());
            hash.Append(acAddress.Province.ToLowerInvariant());
            hash.Append(acAddress.City.ToLowerInvariant());
            hash.Append(acAddress.PostalCode.ToLowerInvariant());
            return hash.ToString();
        }

        private static string GetAnonAddressHash(MerchantCalculation.AnonymousAddress anonAddress)
        {
            StringBuilder hash = new StringBuilder();
            hash.Append(anonAddress.CountryCode.ToLowerInvariant());
            hash.Append(anonAddress.Region.ToLowerInvariant());
            hash.Append(anonAddress.City.ToLowerInvariant());
            hash.Append(anonAddress.PostalCode.ToLowerInvariant());
            return hash.ToString();
        }
        
        public static CommerceBuilder.Users.Address GetAnonAcAddress(MerchantCalculation.AnonymousAddress anonAddress)
        {
            CommerceBuilder.Users.Address addr = new CommerceBuilder.Users.Address();
            addr.FirstName = "Anonymous";
            addr.LastName = "Anonymous";
            addr.Address1 = "Anonymous";
            addr.City = anonAddress.City;
            addr.CountryCode = anonAddress.CountryCode;
            addr.PostalCode = anonAddress.PostalCode;
            addr.Province = anonAddress.Region;
            //TODO : get from configuration
            addr.Residence = false;
            return addr;
        }

        public static void PopulateAcAddress(CommerceBuilder.Users.Address addr, AutoGen.Address gcAddress)
        {
            addr.Address1 = gcAddress.address1;
            addr.Address2 = gcAddress.address2;
            addr.City = gcAddress.city;
            addr.Company = gcAddress.companyname;
            addr.CountryCode = gcAddress.countrycode;
            addr.FullName = gcAddress.contactname;
            addr.Fax = gcAddress.fax;
            addr.Phone = gcAddress.phone;
            addr.PostalCode = gcAddress.postalcode;
            addr.Province = gcAddress.region;
            addr.Email = gcAddress.email;
        }

        public static int GetAcBasketId(ShoppingCart shoppingCart)
        {
            int basketId = 0;
            XmlNode[] privateNodes = shoppingCart.merchantprivatedata.Any;
            foreach (XmlNode node in privateNodes)
            {                
                if (node.Name.Equals("BasketId"))
                {
                    basketId = AlwaysConvert.ToInt(node.InnerText);
                    break;
                }
            }
            return basketId;
        }

        public static int GetGCPaymentMethodId(GoogleCheckout gatewayInstance)
        {
            PaymentMethodCollection gcPayMethods = 
                PaymentMethodDataSource.LoadForPaymentGateway(gatewayInstance.PaymentGatewayId);
            if (gcPayMethods == null || gcPayMethods.Count == 0)
            {
                PaymentMethod gcPayMethod = new PaymentMethod();
                gcPayMethod.Name = "GoogleCheckout";
                gcPayMethod.PaymentGatewayId = gatewayInstance.PaymentGatewayId;
                gcPayMethod.PaymentInstrument = PaymentInstrument.GoogleCheckout;
                gcPayMethod.Save();
                return gcPayMethod.PaymentMethodId;
            }
            else
            {
                return gcPayMethods[0].PaymentMethodId;
            }
        }

        public static PaymentStatus GetPaymentStatus(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.Authorize:
                case TransactionType.PartialCapture:
                    return PaymentStatus.Authorized;
                case TransactionType.AuthorizeCapture:
                case TransactionType.Capture:
                case TransactionType.PartialRefund:
                    return PaymentStatus.Captured;
                case TransactionType.Refund:
                    return PaymentStatus.Refunded;
                case TransactionType.Void:
                    return PaymentStatus.Void;
                default:
                    //THIS SHOULD NEVER HAPPEN IF ALL TRANSACTION TYPES ARE SPECIFIED ABOVE
                    throw new ArgumentOutOfRangeException("Invalid Transaction Type : " + transactionType.ToString());
            }
        }

        public static Payment GetGCPayment(Order order, GoogleCheckout instance, bool createNew)
        {
            int GCPayMethodId = AcHelper.GetGCPaymentMethodId(instance);
            foreach (Payment pmnt in order.Payments)
            {
                if (pmnt.PaymentMethodId == GCPayMethodId)
                {
                    return pmnt;
                }
            }
            //IF THERE IS ONE PAYMENT
            //AND IT IS A GIFT CERTIFICATE
            //AND IT COVERS THE BALANCE OF THE ORDER
            //THEN THIS IS THE GOOGLE PAYMENT
            if (order.Payments.Count == 1)
            {
                int gcPayMethodId = PaymentEngine.GetGiftCertificatePaymentMethod().PaymentMethodId;
                Payment payment = order.Payments[0];
                if (payment.PaymentMethodId == gcPayMethodId)
                {
                    if (payment.Amount == order.TotalCharges) return payment;
                }
            }

            if (createNew)
            {
                Payment payment = new Payment();
                payment.PaymentMethodId = GCPayMethodId;
                payment.Amount = order.GetBalance(false);
                payment.OrderId = order.OrderId;
                payment.PaymentMethodName = "GoogleCheckout";
                payment.PaymentStatus = PaymentStatus.Unprocessed;
                order.Payments.Add(payment);
                //payment.Save();
                order.Save();
                return payment;
            }
            else
            {
                return null;
            }
        }

        public static Basket GetAcBasket(AutoGen.ShoppingCart shoppingcart)
        {
            return GetAcBasket(shoppingcart, false);
        }

        public static Basket GetAcBasket(AutoGen.ShoppingCart shoppingcart, bool clearShipNTax)
        {
            TraceContext trace = WebTrace.GetTraceContext();
            string traceKey = "GoogleCheckout.AC.AcHelper";
            trace.Write(traceKey, "Begin AcHelper.GetAcBasket");

            int basketId = AcHelper.GetAcBasketId(shoppingcart);
            trace.Write(traceKey, "Look for basket ID " + basketId.ToString());
            Basket basket = BasketDataSource.Load(basketId, false);

            if (basket == null)
            {
                trace.Write(traceKey, "Basket not found. Creating New Basket.");
                basket = new Basket();
            }
            else
            {
                //basket found. check if content hash matches
                string contentHash = GetReportedBasketHash(shoppingcart);
                if (contentHash.Equals(GetAcBasketHash(basket)))
                {
                    //hash matched. basket has not changed.
                    if (clearShipNTax) ClearShippingAndTaxes(basket);
                    return basket;
                }
            }

            trace.Write(traceKey, "Clear existing Basket contents and populate with Google Input.");
            basket.Clear();

            if (shoppingcart.items != null)
                trace.Write(traceKey, "Looping " + shoppingcart.items.Length.ToString() + " items in Google cart");
            foreach (Item thisItem in shoppingcart.items)
            {
                trace.Write(traceKey, "itemName: " + thisItem.itemname);
                BasketItem basketItem = new BasketItem();
                basketItem.Name = thisItem.itemname;
                basketItem.Quantity = (short)thisItem.quantity;
                basketItem.Price = thisItem.unitprice.Value;

                XmlNode[] privateNodes = thisItem.merchantprivateitemdata.Any;
                foreach (XmlNode privateNode in privateNodes)
                {
                    trace.Write(traceKey, "privateNode.Name: " + privateNode.Name);
                    switch (privateNode.Name)
                    {
                        case "productId":
                            basketItem.ProductId = AlwaysConvert.ToInt(privateNode.InnerText);
                            break;
                        case "orderItemType":
                            basketItem.OrderItemType = (OrderItemType)AlwaysConvert.ToEnum(typeof(OrderItemType), privateNode.InnerText, OrderItemType.Product, true);
                            break;
                        case "shippable":
                            basketItem.Shippable = (Shippable)AlwaysConvert.ToEnum(typeof(Shippable), privateNode.InnerText, Shippable.Yes, true);
                            break;
                        case "taxCodeId":
                            basketItem.TaxCodeId = AlwaysConvert.ToInt(privateNode.InnerText);
                            break;
                        case "weight":
                            basketItem.Weight = AlwaysConvert.ToDecimal(privateNode.InnerText);
                            break;
                        case "wrapStyleId":
                            basketItem.WrapStyleId = AlwaysConvert.ToInt(privateNode.InnerText);
                            break;
                        case "optionList":
                            basketItem.OptionList = privateNode.InnerText;
                            break;
                        case "giftMessage":
                            basketItem.GiftMessage = privateNode.InnerText;
                            break;
                        case "lineMessage":
                            basketItem.LineMessage = privateNode.InnerText;
                            break;
                        case "lastModifiedDate":
                            basketItem.LastModifiedDate = AlwaysConvert.ToDateTime(privateNode.InnerText, LocaleHelper.LocalNow);
                            break;
                        case "orderBy":
                            basketItem.OrderBy = AlwaysConvert.ToInt16(privateNode.InnerText);
                            break;
                        case "parentItemId":
                            basketItem.ParentItemId = AlwaysConvert.ToInt(privateNode.InnerText);
                            break;
                        case "sku":
                            basketItem.Sku = privateNode.InnerText;
                            break;
                        case "wishlistItemId":
                            basketItem.WishlistItemId = AlwaysConvert.ToInt(privateNode.InnerText);
                            break;
                        case "basketItemKitProducts":
                            List<string> kitList = new List<string>();
                            foreach (XmlNode subNode in privateNode.ChildNodes)
                            {
                                if (subNode.Name.Equals("kitProductId"))
                                {
                                    int kitProductId = AlwaysConvert.ToInt(subNode.InnerText);                                    
                                    KitProduct kitProd = KitProductDataSource.Load(kitProductId);
                                    if (kitProd != null)
                                    {
                                        kitList.Add(kitProductId.ToString());
                                    }
                                }
                            }
                            if (kitList.Count > 0) basketItem.KitList = string.Join(",", kitList.ToArray());
                            break;
                        case "inputs":
                            foreach (XmlNode subNode in privateNode.ChildNodes)
                            {
                                if (subNode.Name.Equals("itemInput"))
                                {
                                    int inputFieldId = 0;
                                    foreach (XmlAttribute attr in subNode.Attributes)
                                    {
                                        if (attr.Name.Equals("inputFieldId"))
                                        {
                                            inputFieldId = AlwaysConvert.ToInt(attr.InnerText);
                                            break;
                                        }
                                    }
                                    InputField inputField = InputFieldDataSource.Load(inputFieldId);
                                    if (inputField != null)
                                    {
                                        BasketItemInput bInput = new BasketItemInput();
                                        bInput.InputFieldId = inputFieldId;
                                        bInput.InputValue = subNode.InnerText;
                                        basketItem.Inputs.Add(bInput);
                                    }
                                }
                            }
                            break;
                        case "couponCode":
                            basketItem.Sku = privateNode.InnerText;
                            break;
                    }
                }
                basket.Items.Add(basketItem);
            }            
            trace.Write(traceKey, "Saving basket");
            basket.Save();

            string key = "Basket_" + basket.BasketId.ToString();
            ContextCache.SetObject(key, basket);

            trace.Write(traceKey, "Basket created, returning to caller (End GetAcBasket)");
            return basket;
        }

        private static void ClearShippingAndTaxes(Basket basket)
        {
            for (int i = basket.Items.Count - 1; i >= 0; i--)
            {
                BasketItem item = basket.Items[i];
                if ((item.OrderItemType == OrderItemType.Shipping) 
                    || (item.OrderItemType == OrderItemType.Handling
                    || (item.OrderItemType == OrderItemType.Tax) ))
                {
                    basket.Items.DeleteAt(i);
                }
            }
        }

        public static string GetAcBasketHash(Basket basket)
        {
            return basket.GetContentHash(false, false, true, OrderItemType.Charge, OrderItemType.Credit, 
                OrderItemType.Discount, OrderItemType.GiftWrap, OrderItemType.Product);
        }

        public static string GetReportedBasketHash(ShoppingCart shoppingCart)
        {
            string contentHash = "";
            XmlNode[] privateNodes = shoppingCart.merchantprivatedata.Any;
            foreach (XmlNode node in privateNodes)
            {
                if (node.Name.Equals("BasketContentHash"))
                {
                    contentHash = node.InnerText;
                    break;
                }
            }
            return contentHash;
        }

        public static ShipMethod GetShipMethod(Basket basket)
        {
            bool shipMethodNameFound = false;
            string shipMethName;
            BasketItem sItem = basket.Items.Find(delegate(BasketItem item) { return (item.OrderItemType == OrderItemType.Shipping); });
            if (sItem == null)
            {
                shipMethName = "Unknown(GoogleCheckout)";
            }
            else
            {
                shipMethName = sItem.Name;
                shipMethodNameFound = true;
            }

            ShipMethodCollection shipMethods = ShipMethodDataSource.LoadForStore();
            ShipMethod shipMethod;
            string methodName = "";
            int shipMethodId = ExtractShipMethodId(shipMethName, out methodName);
            if (shipMethodId != 0)
            {
                shipMethod = FindShipMethod(shipMethods, shipMethodId);
            }
            else
            {
                shipMethod = FindShipMethod(shipMethods, methodName);
            }
            
            if(shipMethod==null && !shipMethName.Equals("Unknown(GoogleCheckout)")) 
            {
                shipMethod = FindShipMethod(shipMethods,"Unknown(GoogleCheckout)");
            }
            
            if (shipMethod == null)
            {
                //create a new ship method
                shipMethod = new ShipMethod();
                shipMethod.Name = "Unknown(GoogleCheckout)";
                shipMethod.ShipMethodType = ShipMethodType.FlatRate;
                shipMethod.MinPurchase = 999999;
                shipMethod.Save();
            }

            if (shipMethod.Name.Equals("Unknown(GoogleCheckout)"))
            {
                if (shipMethodNameFound)
                {
                    Logger.Warn(string.Format("Shipping method named '{0}' used by GoogleCheckout could not be matched to any shipping method in the store.", shipMethName));
                    Logger.Warn("Using 'Unknown(GoogleCheckout)' shipping method.");
                }
                else
                {
                    Logger.Warn("No shipping method found in the GoogleCheckout notification.");
                    Logger.Warn("Using 'Unknown(GoogleCheckout)' shipping method.");
                }
            }

            return shipMethod;
        }

        public static int ExtractShipMethodId(string shipMethodName, out string methName)
        {
            methName = shipMethodName;
            if (string.IsNullOrEmpty(shipMethodName)) return 0;
            if (!shipMethodName.StartsWith("[")) return 0;
            int clbIndex = shipMethodName.IndexOf("]");
            if (clbIndex < 1) return 0;
            string idStr = shipMethodName.Substring(1, clbIndex - 1);
            methName = shipMethodName.Substring(clbIndex + 1);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(idStr, 0);
        }

        public static ShipMethod FindShipMethod(ShipMethodCollection shipMethods, int shipMethodId)
        {
            foreach (ShipMethod shipMeth in shipMethods)
            {
                if (shipMeth.ShipMethodId == shipMethodId) return shipMeth;
            }
            return null;
        }

        public static ShipMethod FindShipMethod(ShipMethodCollection shipMethods, string sanitizedName)
        {
            if (string.IsNullOrEmpty(sanitizedName)) return null;
            string sanName;
            foreach (ShipMethod shipMeth in shipMethods)
            {
                sanName = AcHelper.SanitizeText(shipMeth.Name);
                if (sanitizedName.Equals(sanName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return shipMeth;
                }
            }
            return null;
        }

        public static string CleanShipMethodName(string methodName)
        {
            if (string.IsNullOrEmpty(methodName)) return string.Empty;
            if (methodName.StartsWith("["))
            {
                int clbIndex = methodName.IndexOf("]");
                if (clbIndex < 1) return methodName;
                return methodName.Substring(clbIndex + 1);
            }
            else
            {
                return methodName;
            }
        }

        private static BasketItem GetParentItem(Basket basket, BasketItem childItem)
        {
            if (childItem.ParentItemId.Equals(Guid.Empty)) return childItem;
            foreach (BasketItem item in basket.Items)
            {
                if (item.BasketItemId.Equals(childItem.ParentItemId)) return item;
            }
            return childItem;
        }

        public static Marketing.Coupon GetAcCoupon(string couponCode)
        {
            return Marketing.CouponDataSource.LoadForCouponCode(couponCode);            
        }

        public static Payments.GiftCertificate GetAcGiftCert(string giftCertCode)
        {
            return Payments.GiftCertificateDataSource.LoadForSerialNumber(giftCertCode);            
        }

        public static string GetGCCarrierName(TrackingNumber number)
        {
            string gwName = string.Empty;
            if (number != null && number.ShipGateway != null)
            {
                gwName = number.ShipGateway.Name;
            }
            return GetGCCarrierName(gwName);
        }

        public static string GetGCCarrierName(string gatewayName)
        {
            if (string.IsNullOrEmpty(gatewayName)) return "Other";
            if (gatewayName.Contains("UPS"))
            {
                return "UPS";
            }
            else if (gatewayName.Contains("USPS"))
            {
                return "USPS";
            }
            else if (gatewayName.Contains("FedEx"))
            {
                return "FedEx";
            }
            else if (gatewayName.Contains("DHL"))
            {
                return "DHL";
            }
            return "Other";
        }

        public static string SanitizeText(string input)
        {
            //STRIP ALL HTML OUT OF INPUT
            string output = System.Text.RegularExpressions.Regex.Replace(input, "<[^>]*>", "");
            //STRIP OUT HIGH ASCII CHARACTERS
            
            int thisCharValue;
            System.Text.StringBuilder returnString = new System.Text.StringBuilder();
            foreach (char thisChar in output)
            {
                thisCharValue = Convert.ToInt16(thisChar); //string.Asc(thisChar);
                if (thisCharValue > 31 && thisCharValue < 127)
                {
                    returnString.Append(thisChar);
                }
            }
            return returnString.ToString();
        }

        private String ToAsciiString(String UnicodeString)
        {
            Byte[] asciiBytes = System.Text.Encoding.GetEncoding(1252).GetBytes(UnicodeString);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < (asciiBytes.Length + 1) / 2; i++)
            {
                int c = (int)asciiBytes[i * 2];
                if ((2 * i + 1) < asciiBytes.Length)
                    c += 256 * (int)asciiBytes[2 * i + 1];
                sb.Append((char)c);
            }

            return sb.ToString();
        }

        private static bool IsShippableProduct(BasketItem item)
        {
            return (item.OrderItemType.Equals(OrderItemType.Product) 
                && (item.Shippable != Shippable.No));
        }

    }

}
