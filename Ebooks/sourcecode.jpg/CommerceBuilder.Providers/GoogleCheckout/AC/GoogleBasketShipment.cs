using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Shipping;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class GoogleBasketShipment : IShipment
    {
        private Basket _basket;
        private ShipMethod _shipMethod;

        public GoogleBasketShipment(Basket basket, ShipMethod shipMethod)
        {
            this._basket = basket;
            this._shipMethod = shipMethod;
        }

        public BasketItemCollection GetItems()
        {
            return _basket.Items;            
        }

        public Warehouse Warehouse
        {
            get {
                if (_shipMethod.ShipMethodWarehouses != null &&
                    _shipMethod.ShipMethodWarehouses.Count > 0)
                {
                    return _shipMethod.ShipMethodWarehouses[0].Warehouse;
                }
                return null;
            }
        }

        public Address Address
        {
            get {
                return null; 
            }
        }

        #region IShipment Members


        public ShipZoneCollection ShipZones
        {
            get { return new ShipZoneCollection(); }
        }

        public int WarehouseId
        {
            get
            {
                Warehouse warehouse = this.Warehouse;
                if (warehouse != null) return warehouse.WarehouseId;
                return 0;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
