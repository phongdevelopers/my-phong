using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Payments.Providers.GoogleCheckout;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// This class represents a OrderShipment object in the database.
    /// </summary>
    public partial class OrderShipment : IShipment
    {
        /// <summary>
        /// Ship To country
        /// </summary>
        public Country ShipToCountry
        {
            get
            {
                return CountryDataSource.Load(this.ShipToCountryCode);
            }
        }

        /// <summary>
        /// Gets formatted Ship To address
        /// </summary>
        /// <returns>Formatted Ship To address</returns>
        public string FormatToAddress()
        {
            return FormatToAddress(true);
        }

        /// <summary>
        /// Gets formatted Ship To address
        /// </summary>
        /// <param name="isHtml">If <b>true</b>, address is formatted in HTML format</param>
        /// <returns>Formatted Ship To address</returns>
        public string FormatToAddress(bool isHtml)
        {
            return FormatToAddress(string.Empty, isHtml);
        }

        /// <summary>
        /// Gets formatted Ship To address
        /// </summary>
        /// <param name="pattern">Format pattern to use for formatting</param>
        /// <param name="isHtml">If <b>true</b>, address is formatted in HTML format</param>
        /// <returns>Formatted Ship To address</returns>
        public string FormatToAddress(string pattern, bool isHtml)
        {
            string name = (this.ShipToFirstName + " " + this.ShipToLastName).Trim();
            return AddressFormatter.Format(name, this.ShipToCompany, this.ShipToAddress1, this.ShipToAddress2, this.ShipToCity, this.ShipToProvince, this.ShipToPostalCode, this.ShipToCountryCode, this.ShipToPhone, this.ShipToFax, this.ShipToEmail, isHtml);
        }

        /// <summary>
        /// Gets formatted Ship From address
        /// </summary>
        /// <returns>Formatted Ship From address</returns>
        public string FormatFromAddress()
        {
            return FormatFromAddress(true);
        }

        /// <summary>
        /// Gets formatted Ship From address
        /// </summary>
        /// <param name="isHtml">If <b>true</b>, address is formatted in HTML format</param>
        /// <returns>Formatted Ship From address</returns>
        public string FormatFromAddress(bool isHtml)
        {
            return FormatFromAddress(string.Empty, isHtml);
        }

        /// <summary>
        /// Gets formatted Ship From address
        /// </summary>
        /// <param name="pattern">Format pattern to use for formatting</param>
        /// <param name="isHtml">If <b>true</b>, address is formatted in HTML format</param>
        /// <returns>Formatted Ship From address</returns>
        public string FormatFromAddress(string pattern, bool isHtml)
        {
            Warehouse warehouse = this.Warehouse;
            if (warehouse == null) return string.Empty;
            return AddressFormatter.Format(warehouse.Name, string.Empty, warehouse.Address1, warehouse.Address2, warehouse.City, warehouse.Province, warehouse.PostalCode, warehouse.CountryCode, warehouse.Phone, warehouse.Fax, warehouse.Email, isHtml);
        }

        /// <summary>
        /// Gets Full Ship To Name
        /// </summary>
        public string ShipToFullName
        {
            get
            {
                return string.Format("{0} {1}", this.ShipToFirstName, this.ShipToLastName).Trim();
            }
        }

        /// <summary>
        /// Returns the number of this shipment within the order.
        /// </summary>
        /// <remarks>This method is intended for display only, it is not intended to be used as 
        /// a key value and could change if the sort order of the shipment collection is modified.</remarks>
        public int ShipmentNumber
        {
            get
            {
                Order order = this.Order;
                if (order == null) return 0;
                int index = order.Shipments.IndexOf(this.OrderShipmentId);
                return (index + 1);
            }
        }

        /// <summary>
        /// Register that a shipment has been shipped.
        /// </summary>
        /// <remarks>The current date is used for the ShipDate.</remarks>
        public void Ship()
        {
            this.Ship(LocaleHelper.LocalNow, true);
        }

        /// <summary>
        /// Register that a shipment has been shipped.
        /// </summary>
        /// <param name="shipDate">Date the shipment has been shipped.</param>
        public void Ship(DateTime shipDate)
        {
            Ship(shipDate, true);
        }

        /// <summary>
        /// Register that a shipment has been shipped.
        /// </summary>
        /// <param name="updateGoogleCheckout">If the order associated with this shipment is a GoogleCheckout 
        /// order, this flag  indicates whether to send an update notification to GoogleCheckout or not.</param>
        public void Ship(bool updateGoogleCheckout)
        {
            Ship(LocaleHelper.LocalNow, updateGoogleCheckout);
        }

        /// <summary>
        /// Register that a shipment has been shipped.
        /// </summary>
        /// <param name="shipDate">Date the shipment has been shipped.</param>
        /// <param name="updateGoogleCheckout">If the order associated with this shipment is a GoogleCheckout 
        /// order, this flag  indicates whether to send an update notification to GoogleCheckout or not.</param>
        public void Ship(DateTime shipDate, bool updateGoogleCheckout)
        {
            this.ShipDate = shipDate;
            this.Save();
            //FORCE ASSOCIATED ORDER TO RELOAD FROM DATABASE
            this._Order = OrderDataSource.Load(this.OrderId, false);
            //TRIGGER THE SHIPMENT SHIPPED EVENT
            Stores.StoreEventEngine.ShipmentShipped(this);            
            //RECALCULATE SHIPMENT STATUS
            this.Order.RecalculateShipmentStatus();

            //if the order was a google checkout order, update gooogle checkout
            if (updateGoogleCheckout)
            {                
                if (!string.IsNullOrEmpty(this.Order.GoogleOrderNumber))
                {
                    GoogleCheckout instance = GoogleCheckout.GetInstance();
                    TrackingNumber number = GetLastTrackingNumber();
                    string carrier = GetGCCarrier(number);
                    string trNumber = (number == null || string.IsNullOrEmpty(number.TrackingNumberData))
                                      ? null : number.TrackingNumberData;                    
                    instance.AddTrackingData(this.Order.GoogleOrderNumber, carrier, trNumber);

                    //If order has not yet shipped completely, notify that it is under processing
                    if (this.Order.ShipmentStatus == OrderShipmentStatus.Unshipped
                        || this.Order.ShipmentStatus == OrderShipmentStatus.Unspecified)
                    {
                        instance.ProcessOrder(this.Order.GoogleOrderNumber);
                    }
                }
            }
        }

        private string GetGCCarrier(TrackingNumber number)
        {
            if (number == null)
            {
                string gwName = ShipMethod.ShipGateway == null ? string.Empty : ShipMethod.ShipGateway.Name;
                return Payments.Providers.GoogleCheckout.AC.AcHelper.GetGCCarrierName(gwName);
            }
            else
            {
                return Payments.Providers.GoogleCheckout.AC.AcHelper.GetGCCarrierName(number);
            }
        }

        /// <summary>
        /// Gets the last tracking number entered for this shipment
        /// </summary>
        /// <returns>Returns null if no tracking number is found</returns>
        public TrackingNumber GetLastTrackingNumber()
        {
            if (this.TrackingNumbers.Count == 0) return null;
            TrackingNumber number = this.TrackingNumbers[TrackingNumbers.Count-1];
            return number;
        }

        /// <summary>
        /// Indicates whether the shipment is shipped.
        /// </summary>
        public bool IsShipped
        {
            get
            {
                return (this.ShipDate > DateTime.MinValue);
            }
        }

        /// <summary>
        /// Creates a copy of specified OrderShipment object
        /// </summary>
        /// <param name="orderShipmentId">Id of the OrderShipment object to create copy of</param>
        /// <param name="deepCopy">If <b>true</b> child objects are also copied</param>
        /// <returns>The created copy of specified OrderShipment object</returns>
        public static OrderShipment Copy(int orderShipmentId, bool deepCopy)
        {
            OrderShipment copy = OrderShipmentDataSource.Load(orderShipmentId, false);
            if (deepCopy)
            {
                //THROW NEW
                throw new ArgumentException("Deep copy not implemented for this object.");
            }
            copy.OrderShipmentId = 0;
            return copy;
        }


        /// <summary>
        /// Collection of ShipZone objects associated with this BasketShipment
        /// </summary>
        public ShipZoneCollection ShipZones
        {
            get
            {
                return this.Address.ShipZones;
            }
        }

        /// <summary>
        /// Address object for this shipment
        /// </summary>
        public Address Address
        {
            get
            {
                Address address = new Address();
                address.Address1 = this.ShipToAddress1;
                address.Address2 = this.ShipToAddress2;
                address.City = this.ShipToCity;
                address.Company = this.ShipToCompany;
                address.CountryCode = this.ShipToCountryCode;
                address.Email = this.ShipToEmail;
                address.Fax = this.ShipToFax;
                address.FirstName = this.ShipToFirstName;
                address.LastName = this.ShipToLastName;
                address.Phone = this.ShipToPhone;
                address.PostalCode = this.ShipToPostalCode;
                address.Province = this.ShipToProvince;
                address.Residence = this.ShipToResidence;
                if (this.Order != null) address.UserId = this.Order.UserId;
                return address;
            }
        }
        
        /// <summary>
        /// Gets a collection of items represented by this shipment
        /// </summary>
        /// <returns></returns>
        public BasketItemCollection GetItems()
        {
            BasketItemCollection items = new BasketItemCollection();
            if (this.Order != null)
            {
                foreach (OrderItem item in this.Order.Items)
                {
                    items.Add(item.GetBasketItem());
                }
            }
            return items;
        }
    }
}
