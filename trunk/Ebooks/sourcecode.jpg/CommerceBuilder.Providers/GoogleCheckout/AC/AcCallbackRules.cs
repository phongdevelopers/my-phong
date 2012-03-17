using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Payments.Providers.GoogleCheckout.MerchantCalculation;
using CommerceBuilder.Shipping;
using System.Web;
using CommerceBuilder.Utility;
using CommerceBuilder.Marketing;


namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{    
    public class AcCallbackRules : CallbackRules
    {
        public override MerchantCodeResult GetMerchantCodeResult(Order ThisOrder, AnonymousAddress Address, string MerchantCode)
        {
            MerchantCodeResult RetVal = new MerchantCodeResult();
            RetVal.Valid = false;

            CommerceBuilder.Orders.Basket basket = ThisOrder.AcBasket;
            if (basket == null)
            {
                basket = AcHelper.GetAcBasket(ThisOrder.ShoppingCart);
                if (basket != null) basket.Package(false);
                ThisOrder.AcBasket = basket;
            }

            if (basket != null)
            {
                Marketing.Coupon coupon = AcHelper.GetAcCoupon(MerchantCode);
                if (coupon != null)
                {
                    RetVal.Type = MerchantCodeType.Coupon;
                    if (!Marketing.CouponCalculator.IsCouponValid(basket, coupon))
                    {
                        RetVal.Message = "Coupon '" + MerchantCode + "' is not valid."; 
                        return RetVal;
                    }
                
                    if (coupon.AllowCombine)
                    {
                        if (HasNoCombineCoupon(basket))
                        {
                            //Basket already has a no combine coupon. 
                            RetVal.Message = "The existing coupon applied to the basket can not combine other coupons.";                            
                            return RetVal;
                        }
                    }
                    else
                    {
                        if (basket.BasketCoupons.Count > 0)
                        {
                            RetVal.Message = "Coupon " + MerchantCode + " can not combine with other coupons.";                            
                            return RetVal;
                        }
                    }

                    if (!Marketing.CouponCalculator.IsCouponAlreadyUsed(basket, coupon))
                    {
                        //Coupon can be applied now.
                        CommerceBuilder.Orders.BasketCoupon recentCoupon = new CommerceBuilder.Orders.BasketCoupon(basket.BasketId, coupon.CouponId);
                        basket.BasketCoupons.Add(recentCoupon);
                        basket.Save();
                    }

                    CouponCalculator.ProcessBasket(basket, false);
                    //basket.Recalculate();

                    CommerceBuilder.Orders.BasketItem couponItem = GetAppliedCouponItem(basket, coupon);

                    if (couponItem != null)
                    {
                        RetVal.Valid = true;
                        RetVal.Message = "Coupon '" + MerchantCode + "' has been applied.";
                        RetVal.Amount = Math.Abs(couponItem.ExtendedPrice.ToDecimal(null));                        
                    }
                    else
                    {
                        RetVal.Message = "Coupon '" + MerchantCode + "' could not be applied.";
                    }

                    return RetVal;
                }
                else
                {
                    //check giftcertificate
                    Payments.GiftCertificate giftCert = AcHelper.GetAcGiftCert(MerchantCode);
                    if (giftCert != null)
                    {
                        RetVal.Type = MerchantCodeType.GiftCertificate;
                        RetVal.Amount = giftCert.Balance;
                        if (DateTime.Compare(giftCert.ExpirationDate, DateTime.UtcNow) < 0)
                        {
                            RetVal.Valid = true;
                        }
                        else
                        {
                            RetVal.Message = "Giftcertificate '" + MerchantCode + "' is expired.";
                        }
                    }
                    else
                    {
                        RetVal.Message = "Sorry, we didn't the recognize code '" + MerchantCode + "'.";
                    }
                }
            }

            if (!RetVal.Valid)
            {
                RetVal.Message = "Sorry, we didn't the recognize code '" + MerchantCode + "'.";
            }
            return RetVal;
        }

        public override LSDecimal GetTaxResult(Order ThisOrder, AnonymousAddress Address, LSDecimal ShippingRate)
        {            
            CommerceBuilder.Orders.Basket basket = ThisOrder.AcBasket;
            if (basket == null)
            {
                basket = AcHelper.GetAcBasket(ThisOrder.ShoppingCart, true);
                if (basket != null) basket.Package(false);
                ThisOrder.AcBasket = basket;
            }

            if (basket != null)
            {
                Orders.BasketItem basketItem = null;
                if (ShippingRate > 0)
                {
                    //only temporarily add basket item for tax calculations
                    basketItem = new Orders.BasketItem();
                    basketItem.OrderItemType = Orders.OrderItemType.Shipping;
                    basketItem.Price = ShippingRate;
                    basketItem.Quantity = 1;
                    basketItem.Name = "Temp_GoogleCheckout";
                    //this basket item should be linked to the shipment
                    if (basket.Shipments.Count > 0)
                    {
                        basketItem.BasketShipmentId = basket.Shipments[0].BasketShipmentId;
                    }
                    basket.Items.Add(basketItem);
                    basketItem.BasketId = basket.BasketId;
                    basketItem.Save();
                }

                CommerceBuilder.Users.Address acAddress = AcHelper.GetAnonAcAddress(basket.User, Address);
                UpdateBillingAddress(basket.User, acAddress);
                foreach (Orders.BasketShipment shipment in basket.Shipments) 
                {
                    UpdateShipmentAddress(shipment, acAddress);                    
                }
                 
                LSDecimal RetVal = Taxes.TaxCalculator.Calculate(basket);

                //now that the tax rate is calculated, we can remove the additional basket item
                if (basketItem != null)
                {
                    basket.Items.Remove(basketItem);
                    basketItem.Delete();
                }
                return RetVal;
            }
            else
            {
                return 0;
            }
        }

        private void UpdateShipmentAddress(Orders.BasketShipment shipment, Users.Address acAddress)
        {
            if (shipment.Address == null)
            {
                acAddress.Save();
                shipment.SetAddress(acAddress);
            }
            else
            {
                shipment.Address.FirstName = acAddress.FirstName;
                shipment.Address.LastName = acAddress.LastName;
                shipment.Address.Address1 = acAddress.Address1;
                shipment.Address.City = acAddress.City;
                shipment.Address.CountryCode = acAddress.CountryCode;
                shipment.Address.PostalCode = acAddress.PostalCode;
                shipment.Address.Province = acAddress.Province;
                shipment.Address.Residence = acAddress.Residence;
            }
        }

        private void UpdateBillingAddress(CommerceBuilder.Users.User user, CommerceBuilder.Users.Address acAddress)
        {
            if (user.Addresses.Count == 0)
            {
                user.Addresses.Add(acAddress);
            }
            else
            {
                user.PrimaryAddress.FirstName = acAddress.FirstName;
                user.PrimaryAddress.LastName = acAddress.LastName;
                user.PrimaryAddress.Address1 = acAddress.Address1;
                user.PrimaryAddress.City = acAddress.City;
                user.PrimaryAddress.CountryCode = acAddress.CountryCode;
                user.PrimaryAddress.PostalCode = acAddress.PostalCode;
                user.PrimaryAddress.Province = acAddress.Province;
                user.PrimaryAddress.Residence = acAddress.Residence;
            }
        }

        public override ShippingResult GetShippingResult(string ShipMethodName, Order ThisOrder, AnonymousAddress Address)
        {
            TraceContext trace = WebTrace.GetTraceContext();
            ShippingResult RetVal = new ShippingResult();
            RetVal.Shippable = false;

            CommerceBuilder.Orders.Basket basket = ThisOrder.AcBasket;
            if (basket == null)
            {
                basket = AcHelper.GetAcBasket(ThisOrder.ShoppingCart, true);
                if (basket != null) basket.Package(false);                
                ThisOrder.AcBasket = basket;
            }

            if (basket == null || basket.Shipments.Count == 0) return RetVal;

            ShipMethodCollection shipMethods = ThisOrder.AcShipMethods;
            if (shipMethods == null)
            {
                shipMethods = ShipMethodDataSource.LoadForStore();
                ThisOrder.AcShipMethods = shipMethods;
            }

            if (shipMethods == null || shipMethods.Count == 0) return RetVal;

            ShipMethod shipMethod;
            string methodName = "";
            int shipMethodId = AcHelper.ExtractShipMethodId(ShipMethodName, out methodName);
            if (shipMethodId != 0)
            {                
                shipMethod = AcHelper.FindShipMethod(shipMethods, shipMethodId);
            }
            else
            {
                shipMethod = AcHelper.FindShipMethod(shipMethods, methodName);
            }
            if (shipMethod == null) return RetVal;

            CommerceBuilder.Users.Address acAddress = AcHelper.GetAnonAcAddress(basket.User, Address);
            if (!shipMethod.IsApplicableTo(acAddress)) return RetVal;

            ShipRateQuote rateQuote;
            //TODO : should assign a default ship rate
            RetVal.ShippingRate = 0;
            bool isValid = true;
            foreach (Orders.BasketShipment bshipment in basket.Shipments)
            {
                bshipment.SetAddress(acAddress);
                if (!bshipment.IsShipMethodApplicable(shipMethod))
                {
                    isValid = false;
                    break;
                }
                rateQuote = shipMethod.GetShipRateQuote(bshipment);
                if (rateQuote != null && rateQuote.TotalRate > 0)
                {
                    RetVal.ShippingRate += rateQuote.TotalRate;                    
                }
                else if (rateQuote == null)
                {
                    //this ship method is not applicable
                    isValid = false;
                    break;
                }
            }

            if (isValid)  RetVal.Shippable = true;

            return RetVal;
        }

        private bool HasNoCombineCoupon(CommerceBuilder.Orders.Basket basket)
        {
            foreach (CommerceBuilder.Orders.BasketCoupon cpn in basket.BasketCoupons)
            {
                if (!cpn.Coupon.AllowCombine) return true;
            }
            return false;
        }

        private CommerceBuilder.Orders.BasketItem GetAppliedCouponItem(CommerceBuilder.Orders.Basket basket, Coupon coupon)
        {
            foreach (CommerceBuilder.Orders.BasketItem item in basket.Items)
            {
                if (item.OrderItemType == CommerceBuilder.Orders.OrderItemType.Coupon &&
                    item.Sku.Equals(coupon.CouponCode)) return item;                
            }
            return null;
        }

    }
}
