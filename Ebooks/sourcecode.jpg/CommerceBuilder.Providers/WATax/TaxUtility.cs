using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Shipping;
using CommerceBuilder.Users;

namespace CommerceBuilder.Taxes.Providers.WATax
{
    internal static class TaxUtility
    {
        /// <summary>
        /// Builds a lookup table of tax addresses keyed by shipment id
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <returns>A lookup table of tax addresses</returns>
        public static Dictionary<int, TaxAddress> GenerateAddressLookup(Basket basket)
        {
            Dictionary<int, TaxAddress> addressLookup = new Dictionary<int, TaxAddress>();
            addressLookup[0] = GetBillingAddress(basket);
            foreach (BasketShipment shipment in basket.Shipments)
            {
                addressLookup[shipment.BasketShipmentId] = GetShippingAddress(basket, shipment.BasketShipmentId, addressLookup[0]);
            }
            return addressLookup;
        }

        /// <summary>
        /// Gets the billing address for the basket
        /// </summary>
        /// <param name="basket">The basket to parse; if null the billing address of the current user is returned</param>
        /// <returns>The billing address for the basket, or current user if basket is null</returns>
        private static TaxAddress GetBillingAddress(Basket basket)
        {
            // GET THE ACTIVE BILLING ADDRESS FOR THIS BASKET
            Address billingAddress = basket.User.PrimaryAddress;
            if (billingAddress.IsValid)
            {
                return new TaxAddress(billingAddress.Address1, billingAddress.City, billingAddress.Province, billingAddress.PostalCode);
            }
            else
            {
                // USER DOES NOT HAVE VALID BILLING ADDRESS, SO SUBSTITUTE STORE ADDRESS
                Warehouse storeAddress = Token.Instance.Store.DefaultWarehouse;
                return new TaxAddress(storeAddress.Address1, storeAddress.City, storeAddress.Province, storeAddress.PostalCode);
            }
        }

        /// <summary>
        /// Get the shipping address for the specified shipment
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="shipmentId">The shipment to get shipping address for</param>
        /// <param name="billingAddress">The billing address</param>
        /// <returns>The shipping address for the specified shipment</returns>
        private static TaxAddress GetShippingAddress(Basket basket, int shipmentId, TaxAddress billingAddress)
        {
            // LOCATE THE SPECIFIED SHIPMENT
            int index = basket.Shipments.IndexOf(shipmentId);
            if (index < 0) throw new ArgumentOutOfRangeException("shipmentId");

            // CHECK SHIPPING ADDRESS
            Address shippingAddress = basket.Shipments[index].Address;
            if (shippingAddress.IsValid)
            {
                // VALID SHIPPING ADDRESS, CREATE TAXADDRESS INSTANCE
                return new TaxAddress(shippingAddress.Address1, shippingAddress.City, shippingAddress.Province, shippingAddress.PostalCode);
            }
            else
            {
                // INVALID SHIPPING ADDRESS, DEFAULT TO BILLING ADDRESS
                return billingAddress;
            }
        }

        /// Builds a lookup table of tax addresses keyed by shipment id
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A lookup table of tax addresses</returns>
        public static Dictionary<int, TaxAddress> GenerateAddressLookup(Order order)
        {
            Dictionary<int, TaxAddress> addressLookup = new Dictionary<int, TaxAddress>();
            addressLookup[0] = new TaxAddress(order.BillToAddress1, order.BillToCity, order.BillToProvince, order.BillToPostalCode);
            foreach (OrderShipment shipment in order.Shipments)
            {
                addressLookup[shipment.OrderShipmentId] = new TaxAddress(shipment.ShipToAddress1, shipment.ShipToCity, shipment.ShipToProvince, shipment.ShipToPostalCode);
            }
            return addressLookup;
        }

        /// <summary>
        /// Calculates tax for the basket item using the specified rule
        /// </summary>
        /// <param name="basketId">Id of the basket</param>
        /// <param name="shipmentId">Id of the shipment</param>
        /// <param name="taxableValue">Total value being taxed</param>
        /// <param name="taxInfo">The tax info structure to apply</param>
        /// <returns>The new basket item</returns>
        /// <remarks>This method does no checking for validity, it only handles calculations
        /// and generation of an item for the given values.</remarks>
        public static BasketItem GenerateBasketItem(int basketId, int shipmentId, decimal taxableValue, TaxInfo taxInfo)
        {
            BasketItem taxLineItem = new BasketItem();
            taxLineItem.BasketId = basketId;
            taxLineItem.OrderItemType = OrderItemType.Tax;
            taxLineItem.BasketShipmentId = shipmentId;
            taxLineItem.ParentItemId = 0;
            taxLineItem.Name = taxInfo.TaxName;
            taxLineItem.Sku = taxInfo.TaxSku;
            taxLineItem.Price = taxInfo.Calculate(taxableValue);
            taxLineItem.Quantity = 1;
            return taxLineItem;
        }

        /// <summary>
        /// Calculates tax for the order item using the specified rule
        /// </summary>
        /// <param name="orderId">Id of the order</param>
        /// <param name="shipmentId">Id of the shipment</param>
        /// <param name="taxableValue">Total value being taxed</param>
        /// <param name="taxInfo">The tax info structure to apply</param>
        /// <returns>The new order item</returns>
        /// <remarks>This method does no checking for validity, it only handles calculations
        /// and generation of an item for the given values.</remarks>
        public static OrderItem GenerateOrderItem(int orderId, int shipmentId, decimal taxableValue, TaxInfo taxInfo)
        {
            OrderItem taxLineItem = new OrderItem();
            taxLineItem.OrderId = orderId;
            taxLineItem.OrderItemType = OrderItemType.Tax;
            taxLineItem.OrderShipmentId = shipmentId;
            taxLineItem.ParentItemId = 0;
            taxLineItem.Name = taxInfo.TaxName;
            taxLineItem.Sku = taxInfo.TaxSku;
            taxLineItem.Price = taxInfo.Calculate(taxableValue);
            taxLineItem.Quantity = 1;
            return taxLineItem;
        }
    }
}