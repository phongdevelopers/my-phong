using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommerceBuilder.Shipping;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// This class represents a BasketShipment object in the database.
    /// </summary>
    public partial class BasketShipment : IShipment
    {
        private ShipZoneCollection _ShipZones;
        private ShipMethodCollection _ApplicableShipMethods;

        /// <summary>
        /// Collection of ShipZone objects associated with this BasketShipment
        /// </summary>
        public ShipZoneCollection ShipZones
        {
            get
            {
                if (_ShipZones == null || _IsDirty)
                {
                    Address address = this.Address;
                    _ShipZones = ShipZoneDataSource.LoadForAddress(address.CountryCode, address.ProvinceId, address.PostalCode);
                }
                return this._ShipZones;
            }
        }

        /// <summary>
        /// Collection of ShipMethods applicable to this BasketShipment
        /// </summary>
        public ShipMethodCollection ApplicableShipMethods
        {
            get
            {
                if (this._ApplicableShipMethods == null)
                {
                    this._ApplicableShipMethods = ShipMethodDataSource.LoadForShipment(this);
                }
                return this._ApplicableShipMethods;
            }
        }

        /// <summary>
        /// Indicates whether the given shipmethod is applicable on this BasketShipment or not
        /// </summary>
        /// <param name="shipMeth">The ship method to check</param>
        /// <returns><b>true</b> if ship method is applicable, <b>false</b> otherwise</returns>
        public bool IsShipMethodApplicable(ShipMethod shipMeth)
        {
            ShipMethodCollection shipMeths = this.ApplicableShipMethods;
            foreach (ShipMethod meth in shipMeths)
            {
                if (meth.ShipMethodId == shipMeth.ShipMethodId) return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the address of this basket shipment
        /// </summary>
        /// <param name="address">The address to set</param>
        public void SetAddress(Address address)
        {
            this.AddressId = address.AddressId;
            this._Address = address;
        }

        /// <summary>
        /// Apply a ship method to this basket shipment
        /// </summary>
        /// <param name="shipMethodId">Id of the ship method to apply</param>
        public void ApplyShipMethod(int shipMethodId)
        {
            ApplyShipMethod(ShipMethodDataSource.Load(shipMethodId));
        }

        /// <summary>
        /// Apply a ship method to this basket shipment
        /// </summary>
        /// <param name="shipMethod">The ship method to apply</param>
        public void ApplyShipMethod(ShipMethod shipMethod)
        {
            if (shipMethod == null) throw new ArgumentNullException("shipMethod");
            //WIPE OUT ANY SHIPPING CHARGES CURRENTLY IN THIS SHIPMENT
            this.ShipMethodId = shipMethod.ShipMethodId;
            this._ShipMethod = shipMethod;
            BasketItem item;
            BasketItemCollection basketItems = this.Basket.Items;
            for (int i = basketItems.Count - 1; i >= 0; i--)
            {
                item = basketItems[i];
                if (item.BasketShipmentId.Equals(this.BasketShipmentId) && ((item.OrderItemType == OrderItemType.Shipping || item.OrderItemType == OrderItemType.Handling)))
                {
                    basketItems.DeleteAt(i);
                }
            }
            ShipRateQuote quote = shipMethod.GetShipRateQuote(this);
            if (quote == null) throw new ArgumentException("The specified shipping method is not valid for these items.", "shipMethod");
            item = new BasketItem();
            item.BasketId = this.BasketId;
            item.BasketShipmentId = this.BasketShipmentId;
            item.Name = quote.ShipMethod.Name;
            item.OrderItemType = OrderItemType.Shipping;
            item.Price = quote.Rate;
            item.Weight = 0;
            item.Quantity = 1;
            basketItems.Add(item);
            if (quote.Surcharge > 0)
            {
                item = new BasketItem();
                item.BasketId = this.BasketId;
                item.BasketShipmentId = this.BasketShipmentId;
                item.Name = quote.ShipMethod.Name;
                item.OrderItemType = OrderItemType.Handling;
                item.Price = quote.Surcharge;
                item.Weight = 0;
                item.Quantity = 1;
                basketItems.Add(item);
            }
            basketItems.Save();
            this.Save();
        }

        /// <summary>
        /// Indicates whether two shipments have identical origin and destination.
        /// </summary>
        /// <param name="other">The item to compare.</param>
        /// <returns>True if the items have identical origin and destination.</returns>
        public bool CanCombine(BasketShipment other)
        {
            return (this.WarehouseId.Equals(other.WarehouseId) && this.AddressId.Equals(other.AddressId));
        }

        /// <summary>
        /// Gets a collection of basket items in this basket shipment
        /// </summary>
        /// <returns>A collection of basket items in this basket shipment</returns>
        public BasketItemCollection GetItems()
        {
            List<BasketItem> itemList = this.Basket.Items.FindAll(delegate(BasketItem item) { return (item.BasketShipmentId.Equals(this.BasketShipmentId)); });
            BasketItemCollection items = new BasketItemCollection();
            items.AddRange(itemList);
            return items;
        }

        /// <summary>
        /// Gets a collection of basket items in this basket shipment from the given basket
        /// </summary>
        /// <param name="basket">The basket object</param>
        /// <returns>A collection of basket items in this basket shipment</returns>
        public BasketItemCollection GetItems(Basket basket)
        {
            List<BasketItem> itemList = basket.Items.FindAll(delegate(BasketItem item) { return (item.BasketShipmentId.Equals(this.BasketShipmentId)); });            
            BasketItemCollection items = new BasketItemCollection();
            items.AddRange(itemList);
            return items;
        }

        /// <summary>
        /// Gets the basket items attached to this shipment.
        /// </summary>
        /// <remarks>The returned collection is created every time this property is accessed.  If you need to refer to the collection repeatedly, store the return value in a local variable for best performance.</remarks>
        public BasketItemCollection Items
        {
            get
            {
                BasketItemCollection shipmentItems = new BasketItemCollection();
                foreach (BasketItem item in this.Basket.Items)
                {
                    if (item.BasketShipmentId == this.BasketShipmentId) shipmentItems.Add(item);
                }
                return shipmentItems;
            }
        }

        /// <summary>
        /// Delete this basket shipment object from database
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise</returns>
        public virtual bool Delete()
        {
            //DELETE ANY BASKET ITEMS IN THIS SHIPMENT 
            BasketItemCollection itemCollection = this.Basket.Items;
            for (int i = (itemCollection.Count - 1); i >= 0; i--)
            {
                BasketItem item = itemCollection[i];
                if (item.BasketShipmentId.Equals(this.BasketShipmentId))
                {
                    itemCollection.DeleteAt(i);
                }
            }
            return this.BaseDelete();
        }

        /// <summary>
        /// Gets the shipment number of this basket shipment
        /// </summary>
        public int ShipmentNumber
        {
            get
            {
                Basket basket = this.Basket;
                if (basket == null) return 0;
                int index = basket.Shipments.IndexOf(this.BasketShipmentId);
                return (index + 1);
            }
        }

    }
}
