using System;
using System.ComponentModel;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// This class represents a Coupon object in the database.
    /// </summary>
    public partial class Coupon
    {
        /// <summary>
        /// The type of this coupon
        /// </summary>
        public CouponType CouponType
        {
            get
            {
                return (CouponType)this.CouponTypeId;
            }
            set
            {
                this.CouponTypeId = (byte)value;
            }
        }

        /// <summary>
        /// Product rule for this coupon
        /// </summary>
        public CouponRule ProductRule
        {
            get
            {
                return (CouponRule)this.ProductRuleId;
            }
            set
            {
                this.ProductRuleId = (byte)value;
            }
        }

        /// <summary>
        /// Creates a clone of this Coupon object
        /// </summary>
        /// <param name="deepClone">If <b>true</b> all the child elements are also copied. (TODO)</param>
        /// <returns>A clone of this coupon object</returns>
        public Coupon Clone(bool deepClone)
        {
            Coupon cloneItem = new Coupon();
            cloneItem.CouponTypeId = this.CouponTypeId;
            cloneItem.Name = this.Name;
            cloneItem.CouponCode = this.CouponCode;
            cloneItem.DiscountAmount = this.DiscountAmount;
            cloneItem.IsPercent = this.IsPercent;
            cloneItem.MaxValue = this.MaxValue;
            cloneItem.MinPurchase = this.MinPurchase;
            cloneItem.MinQuantity = this.MinQuantity;
            cloneItem.MaxQuantity = this.MaxQuantity;
            cloneItem.QuantityInterval = this.QuantityInterval;
            cloneItem.MaxUses = this.MaxUses;
            cloneItem.MaxUsesPerCustomer = this.MaxUsesPerCustomer;
            cloneItem.StartDate = this.StartDate;
            cloneItem.EndDate = this.EndDate;
            cloneItem.ProductRule = this.ProductRule;
            cloneItem.AllowCombine = this.AllowCombine;
            if (deepClone)
            {
                // COPY CHILD OBJECTS
                // WE HAVE TO SAVE THE COUPON
                if (cloneItem.Save() != SaveResult.Failed)
                {
                    // COPY GROUPS
                    foreach (CouponGroup grop in this.CouponGroups)
                    {
                        cloneItem.CouponGroups.Add(new CouponGroup(cloneItem.CouponId, grop.GroupId));
                    }
                    cloneItem.CouponGroups.Save();

                    // COPY PRODUCTS
                    foreach (CouponProduct product in this.CouponProducts)
                    {
                        cloneItem.CouponProducts.Add(new CouponProduct(cloneItem.CouponId, product.ProductId));
                    }
                    cloneItem.CouponProducts.Save();

                    // COPY SHIP METHODS
                    foreach (CouponShipMethod couponShipMethod in this.CouponShipMethods)
                    {
                        cloneItem.CouponShipMethods.Add(new CouponShipMethod(cloneItem.CouponId, couponShipMethod.ShipMethodId));
                    }
                    cloneItem.CouponShipMethods.Save();
                }
            }
            return cloneItem;
        }

        /// <summary>
        /// Determines if a coupon applies to a shipment
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="shipment">The basket shipment</param>
        /// <returns>True if the coupon is valid for the shipment; false otherwise.</returns>
        public bool AppliesToShipment(Basket basket, BasketShipment shipment)
        {
            // DO SOME VALIDATION ON THE INPUT PARAMETERS
            if (this.CouponType != CouponType.Shipping) throw new InvalidOperationException("This method is only applicable for shipping coupons.");
            if (basket == null) throw new ArgumentNullException("basket");
            if (shipment == null) throw new ArgumentNullException("shipment");
            if (basket.BasketId != shipment.BasketId) throw new ArgumentException("The shipment is not part of the basket.");

            // MAKE SURE THE SHIPPING METHOD APPLIES
            if (!this.AppliesToShipMethod(shipment.ShipMethodId)) return false;

            // DO WE NEED TO VALIDATE MINIMUM PURCHASE?
            if (this.MinPurchase > 0)
            {
                // GET THE TOTAL PRODUCT PRICE OF THE SHIPMENT
                BasketItemCollection shipmentItems = shipment.GetItems(basket);
                LSDecimal totalProductPrice = shipmentItems.TotalProductPrice();

                // IF THRESHOLD IS NOT MET, THE COUPON DOES NOT APPLY
                if (totalProductPrice < this.MinPurchase) return false;
            }

            // NOT OTHERWISE INVALID, THE COUPON APPLIES TO SHIPMENT
            return true;
        }

        /// <summary>
        /// Indicates whether a coupon applies to the given shipping method.
        /// </summary>
        /// <param name="shipMethodId">Shipping method to test.</param>
        /// <returns>True if the coupon applies to the shipping method; false otherwise.</returns>
        public bool AppliesToShipMethod(int shipMethodId)
        {
            if (this.CouponType != CouponType.Shipping) throw new InvalidOperationException("This method is only applicable for shipping coupons.");
            if (shipMethodId <= 0) return false;

            switch (this.ProductRule)
            {
                case CouponRule.All:
                    // IF NO FILTER, CONSIDER A POSITIVE SHIPPING METHOD ID AS VALID
                    return true;
                case CouponRule.AllowSelected:
                    // IF ALLOW FILTER, THE SHIPPING METHOD MUST BE IN THE LIST
                    foreach (CouponShipMethod couponShipMethod in this.CouponShipMethods)
                    {
                        if (shipMethodId == couponShipMethod.ShipMethodId) return true;
                    }
                    return false;
                default:
                    // WE MUST HAVE AN EXCLUDE FILTER, THE SHIPPING METHOD MUST NOT BE IN THE LIST
                    foreach (CouponShipMethod couponShipMethod in this.CouponShipMethods)
                    {
                        if (shipMethodId == couponShipMethod.ShipMethodId) return false;
                    }
                    return true;
            }
        }

        public bool AppliesToProduct(int productId)
        {
            if (this.CouponType == CouponType.Shipping) throw new InvalidOperationException("This method is only applicable for order and product coupons.");
            if (productId <= 0) return false;

            switch (this.ProductRule)
            {
                case CouponRule.All:
                    // IF NO FILTER, CONSIDER A POSITIVE PRODUCT ID AS VALID
                    return true;
                case CouponRule.AllowSelected:
                    // IF ALLOW FILTER, THE PRODUCT MUST BE IN THE LIST
                    foreach (CouponProduct couponProduct in this.CouponProducts)
                    {
                        if (productId == couponProduct.ProductId) return true;
                    }
                    return false;
                default:
                    // WE MUST HAVE AN EXCLUDE FILTER, THE PRODUCT MUST NOT BE IN THE LIST
                    foreach (CouponProduct couponProduct in this.CouponProducts)
                    {
                        if (productId == couponProduct.ProductId) return false;
                    }
                    return true;
            }
        }

        public LSDecimal CalculateValue(LSDecimal itemTotal)
        {
            // WE DO NOT NEED TO CALCULATE COUPONS FOR NON-POSITIVE VALUES
            if (itemTotal <= 0) return 0;
            
            // DETERMINE THE COUPON VALUE
            LSDecimal couponValue;
            if (this.IsPercent)
            {
                couponValue = Math.Round(((decimal)itemTotal * (decimal)this.DiscountAmount) / 100, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                couponValue = this.DiscountAmount;
            }
            if (couponValue > itemTotal) couponValue = itemTotal;
            if (this.MaxValue > 0 && couponValue > this.MaxValue) couponValue = this.MaxValue;
            return -1 * couponValue;
        }
    }
}
