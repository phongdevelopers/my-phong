using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using System.Web.UI;

namespace CommerceBuilder.Taxes.Providers.CCH
{
    public class CertiCalc : CommerceBuilder.Taxes.Providers.TaxProviderBase
    {
        //THIS IS ABLECOMMERCE IDENTIFIER
        private const string CT_SERIAL_NUMBER = "6C05-4802-B903-FC81";

        private bool _ConfirmAddresses;
        private bool _IgnoreFailedConfirm;
        private bool _ReportBreakdown;
        private string _Location;
        private Nexus _Nexus;
        private bool _UseLineItems = true;
        private string _ReferredID;

        public override bool Activated
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        private void BuildTaxOrderAddress(CertiTAX.Order taxOrder, Address address)
        {
            taxOrder.Address = new CommerceBuilder.Taxes.Providers.CCH.CertiTAX.Address();
            taxOrder.Address.Name = address.FullName;
            taxOrder.Address.Street1 = address.Address1;
            taxOrder.Address.Street2 = address.Address2;
            taxOrder.Address.City = address.City;
            taxOrder.Address.State = address.Province;
            taxOrder.Address.PostalCode = address.PostalCode;
            taxOrder.Address.Nation = address.CountryCode;
        }

        private void BuildTaxOrderAddress(CertiTAX.Order taxOrder, Warehouse address)
        {
            taxOrder.Address = new CommerceBuilder.Taxes.Providers.CCH.CertiTAX.Address();
            taxOrder.Address.Name = address.Name;
            taxOrder.Address.Street1 = address.Address1;
            taxOrder.Address.Street2 = address.Address2;
            taxOrder.Address.City = address.City;
            taxOrder.Address.State = address.Province;
            taxOrder.Address.PostalCode = address.PostalCode;
            taxOrder.Address.Nation = address.CountryCode;
        }

        private void BuildTaxOrder(CertiTAX.Order taxOrder, Basket basket, int shipmentId, Dictionary<string, string> existingTransactions)
        {
            if (existingTransactions.ContainsKey(shipmentId.ToString())) taxOrder.CertiTAXTransactionId = existingTransactions[shipmentId.ToString()];
            LSDecimal shippingCharge = 0;
            LSDecimal handlingCharge = 0;
            GetShipCharges(basket, shipmentId, out shippingCharge, out handlingCharge);
            taxOrder.SerialNumber = CT_SERIAL_NUMBER;
            taxOrder.ReferredId = ReferredID;
            taxOrder.CalculateTax = true;
            taxOrder.ConfirmAddress = this.ConfirmAddresses;
            taxOrder.DefaultProductCode = 0;
            taxOrder.HandlingCharge = (Decimal)handlingCharge;
            taxOrder.ShippingCharge = (Decimal)shippingCharge;
            taxOrder.Location = Location;
            taxOrder.MerchantTransactionId = basket.BasketId.ToString();
            WebTrace.Write("Processing Items");
            BuildTaxOrderItems(taxOrder, basket, shipmentId);
        }

        private void BuildTaxOrderItems(CertiTAX.Order taxOrder, Basket basket, int shipmentId)
        {
            if (this.UseLineItems)
            {
                WebTrace.Write("Process Tax Items -- Line Items Mode");
                LSDecimal productTotal = 0;
                List<CertiTAX.OrderLineItem> taxLineItems = new List<CertiTAX.OrderLineItem>();
                foreach (BasketItem item in basket.Items)
                {
                    if (item.OrderItemType == OrderItemType.Product)
                    {
                        CertiTAX.OrderLineItem taxLineItem = new CertiTAX.OrderLineItem();
                        taxLineItem.ItemId = item.ProductId.ToString();
                        taxLineItem.StockingUnit = item.Sku;
                        taxLineItem.Quantity = item.Quantity;
                        taxLineItem.ExtendedPrice = (Decimal)item.ExtendedPrice;
                        productTotal += item.ExtendedPrice;
                        taxLineItems.Add(taxLineItem);
                    }
                }
                taxOrder.LineItems = taxLineItems.ToArray();
                taxOrder.Total = (Decimal)productTotal;
            }
            else
            {
                WebTrace.Write("Process Tax Items -- Order Total Mode");
                OrderItemType[] productTypes = { OrderItemType.Product, OrderItemType.Coupon, OrderItemType.Discount };
                if (shipmentId == 0)
                {
                    //SET TOTAL FOR THE BASKET
                    taxOrder.Total = (Decimal)basket.Items.TotalPrice(productTypes);
                }
                else
                {
                    //SET TOTAL FOR THE SHIPMENT
                    BasketShipment shipment = this.GetShipment(basket, shipmentId);
                    if (shipment != null) taxOrder.Total = (Decimal)shipment.GetItems().TotalPrice(productTypes);
                    else taxOrder.Total = 0;
                }
            }
        }

        public override LSDecimal Calculate(CommerceBuilder.Orders.Basket basket)
        {
            if (basket.Items.Count == 0) return 0;
            Dictionary<string, string> existingTransactions = ClearExistingTaxes(basket);
            switch (this.Nexus)
            {
                case Nexus.PointOfBilling:
                    return Calculate_PointOfBilling(basket, existingTransactions);
                case Nexus.PointOfDelivery:
                    return Calculate_PointOfDelivery(basket, existingTransactions);
                case Nexus.PointOfSale:
                    return Calculate_PointOfSale(basket, existingTransactions);
                case Nexus.PointOfShipping:
                    return Calculate_PointOfShipping(basket, existingTransactions);
                default:
                    throw new ApplicationException("Unrecognized Nexus: " + this.Nexus.ToString());
            }
        }

        private LSDecimal Calculate_PointOfBilling(Basket basket, Dictionary<string, string> existingTransactions)
        {
            WebTrace.Write("CertiTAX: Begin Calculate POB");
            CertiTAX.Order taxOrder = new CertiTAX.Order();
            //SET THE TAXORDER ADDRESS
            BuildTaxOrderAddress(taxOrder, basket.User.PrimaryAddress);
            //BUILD THE TAXORDER OBJECT
            BuildTaxOrder(taxOrder, basket, 0, existingTransactions);
            taxOrder.Nexus = "POB";
            //EXECUTE THE TRANSACTION
            CertiTAX.TaxTransaction taxTransaction = null;
            try
            {
                taxTransaction = (new CertiTAX.CertiCalc()).Calculate(taxOrder);
            }
            catch (Exception ex)
            {
                WebTrace.Write("CertiTax could not calculate tax.  The error was: " + ex.Message);
                if (!this.IgnoreFailedConfirm) throw;
            }
            //PARSE THE RESULTS
            LSDecimal totalTax = ParseTaxTransaction(taxTransaction, basket, 0);
            WebTrace.Write("CertiTAX: End Calculate POB");
            return totalTax;
        }

        private LSDecimal Calculate_PointOfDelivery(Basket basket, Dictionary<string, string> existingTransactions)
        {
            WebTrace.Write("CertiTAX: Begin Calculate POD");
            LSDecimal totalTax = 0;
            foreach (BasketShipment shipment in basket.Shipments)
            {
                CertiTAX.Order taxOrder = new CertiTAX.Order();
                //SET THE TAXORDER ADDRESS
                BuildTaxOrderAddress(taxOrder, shipment.Address);
                //BUILD THE TAXORDER OBJECT
                BuildTaxOrder(taxOrder, basket, shipment.BasketShipmentId, existingTransactions);
                taxOrder.Nexus = "POD";
                //EXECUTE THE TRANSACTION
                CertiTAX.TaxTransaction taxTransaction = null;
                try
                {
                    taxTransaction = (new CertiTAX.CertiCalc()).Calculate(taxOrder);
                }
                catch (Exception ex)
                {
                    WebTrace.Write("CertiTax could not calculate tax.  The error was: " + ex.Message);
                    if (!this.IgnoreFailedConfirm) throw;
                }
                //PARSE THE RESULTS
                totalTax += ParseTaxTransaction(taxTransaction, basket, shipment.BasketShipmentId);
            }
            WebTrace.Write("CertiTAX: End Calculate POD");
            //RETURN THE TOTAL TAX
            return totalTax;
        }

        private LSDecimal Calculate_PointOfSale(Basket basket, Dictionary<string, string> existingTransactions)
        {
            WebTrace.Write("CertiTAX: Begin Calculate POS");
            CertiTAX.Order taxOrder = new CertiTAX.Order();
            //SET THE TAXORDER ADDRESS
            BuildTaxOrderAddress(taxOrder, StoreDataSource.Load().DefaultWarehouse);
            //BUILD THE TAXORDER OBJECT
            BuildTaxOrder(taxOrder, basket, 0, existingTransactions);
            taxOrder.Nexus = "POS";
            //EXECUTE THE TRANSACTION
            CertiTAX.TaxTransaction taxTransaction = null;
            try
            {
                taxTransaction = (new CertiTAX.CertiCalc()).Calculate(taxOrder);
            }
            catch (Exception ex)
            {
                WebTrace.Write("CertiTax could not calculate tax.  The error was: " + ex.Message);
                if (!this.IgnoreFailedConfirm) throw;
            }
            //PARSE THE RESULTS
            LSDecimal totalTax = ParseTaxTransaction(taxTransaction, basket, 0);
            WebTrace.Write("CertiTAX: End Calculate POS");
            return totalTax;
        }

        private LSDecimal Calculate_PointOfShipping(Basket basket, Dictionary<string, string> existingTransactions)
        {
            WebTrace.Write("CertiTAX: Begin Calculate POD");
            LSDecimal totalTax = 0;
            foreach (BasketShipment shipment in basket.Shipments)
            {
                if (shipment.Warehouse != null)
                {
                    CertiTAX.Order taxOrder = new CertiTAX.Order();
                    //SET THE TAXORDER ADDRESS
                    BuildTaxOrderAddress(taxOrder, shipment.Warehouse);
                    //BUILD THE TAXORDER OBJECT
                    BuildTaxOrder(taxOrder, basket, shipment.BasketShipmentId, existingTransactions);
                    taxOrder.Nexus = "POSH";
                    //EXECUTE THE TRANSACTION
                    CertiTAX.TaxTransaction taxTransaction = null;
                    try
                    {
                        taxTransaction = (new CertiTAX.CertiCalc()).Calculate(taxOrder);
                    }
                    catch (Exception ex)
                    {
                        WebTrace.Write("CertiTax could not calculate tax.  The error was: " + ex.Message);
                        if (!this.IgnoreFailedConfirm) throw;
                    }
                    //PARSE THE RESULTS
                    totalTax += ParseTaxTransaction(taxTransaction, basket, shipment.BasketShipmentId);
                }
            }
            WebTrace.Write("CertiTAX: End Calculate POD");
            //RETURN THE TOTAL TAX
            return totalTax;
        }

        public override void Cancel(CommerceBuilder.Orders.Basket basket)
        {
            WebTrace.Write("Cancel Existing Taxes");
            List<string> uniqueTransactionIds = new List<string>();
            Dictionary<string, string> existingTransactions = ClearExistingTaxes(basket);
            CertiTAX.CertiCalc comm = new CertiTAX.CertiCalc();
            foreach (string transactionId in existingTransactions.Values)
            {
                if (uniqueTransactionIds.IndexOf(transactionId) < 0)
                {
                    uniqueTransactionIds.Add(transactionId);
                    comm.Cancel(transactionId, CT_SERIAL_NUMBER, this.ReferredID);
                }
            }
        }

        public override void Commit(CommerceBuilder.Orders.Order order)
        {
            WebTrace.Write("Commit Existing Taxes");
            List<string> uniqueTransactionIds = new List<string>();
            CertiTAX.CertiCalc comm = new CertiTAX.CertiCalc();
            foreach (OrderItem item in order.Items)
            {
                if ((item.OrderItemType == OrderItemType.Tax) && (item.Sku.StartsWith("CT:") && (!item.LineMessage.Equals("Committed"))))
                {
                    string transactionId = item.Sku.Substring(3);
                    if (uniqueTransactionIds.IndexOf(transactionId) < 0)
                    {
                        CommerceBuilder.Taxes.Providers.CCH.CertiTAX.TaxTransaction tx = new CommerceBuilder.Taxes.Providers.CCH.CertiTAX.TaxTransaction();
                        uniqueTransactionIds.Add(transactionId);
                        comm.Commit(transactionId, CT_SERIAL_NUMBER, this.ReferredID);
                        item.LineMessage = "Committed";
                    }
                }
            }

        }

        private static Dictionary<string, string> ClearExistingTaxes(Basket basket)
        {
            WebTrace.Write("Clear Existing Taxes");
            Dictionary<string, string> existingTransactions = new Dictionary<string, string>();
            for (int i = basket.Items.Count - 1; i >= 0; i--)
            {
                BasketItem item = basket.Items[i];
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    if (!existingTransactions.ContainsKey(item.BasketShipmentId.ToString()) && item.Sku.StartsWith("CT:"))
                    {
                        existingTransactions[item.BasketShipmentId.ToString()] = item.Sku.Substring(3);
                    }
                    basket.Items.DeleteAt(i);
                }
            }
            return existingTransactions;
        }

        public bool ConfirmAddresses
        {
            get { return _ConfirmAddresses; }
            set { _ConfirmAddresses = value; }
        }

        private void CreateTaxLineItem(Basket basket, int shipmentId, string authorityName, string certiTaxTransactionId, LSDecimal amount)
        {
            BasketItem taxLineItem = new BasketItem();
            taxLineItem.BasketId = basket.BasketId;
            taxLineItem.OrderItemType = OrderItemType.Tax;
            taxLineItem.BasketShipmentId = shipmentId;
            taxLineItem.Name = authorityName;
            taxLineItem.Sku = string.Format("CT:" + certiTaxTransactionId);
            taxLineItem.Price = amount;
            taxLineItem.Quantity = 1;
            taxLineItem.Save();
            basket.Items.Add(taxLineItem);
            WebTrace.Write("Tax Line Item Added");
        }

        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData.Add("ConfirmAddresses", this.ConfirmAddresses.ToString());
            configData.Add("IgnoreFailedConfirm", this.IgnoreFailedConfirm.ToString());
            configData.Add("ReportBreakdown", this.ReportBreakdown.ToString());
            configData.Add("Location", this.Location);
            configData.Add("Nexus", ((int)this.Nexus).ToString());
            configData.Add("UseLineItems", this.UseLineItems.ToString());
            return configData;
        }

        private void GetShipCharges(Basket basket, int shipmentId, out LSDecimal shippingCharge, out LSDecimal handlingCharge)
        {
            shippingCharge = 0;
            handlingCharge = 0;
            foreach (BasketItem item in basket.Items)
            {
                if (item.BasketShipmentId.Equals(shipmentId))
                {
                    if (item.OrderItemType == OrderItemType.Shipping) shippingCharge += item.ExtendedPrice;
                    else if (item.OrderItemType == OrderItemType.Handling) handlingCharge += item.ExtendedPrice;
                }
            }
        }

        private BasketShipment GetShipment(Basket basket, int shipmentId)
        {
            foreach (BasketShipment shipment in basket.Shipments)
            {
                if (shipment.BasketShipmentId.Equals(shipmentId)) return shipment;
            }
            return null;
        }

        public bool IgnoreFailedConfirm
        {
            get { return true; } //_IgnoreFailedConfirm; 
            set { _IgnoreFailedConfirm = value; }
        }

        public override void Initialize(int taxGatewayId, Dictionary<string, string> configurationData)
        {
            base.Initialize(taxGatewayId, configurationData);
            if (configurationData.ContainsKey("ConfirmAddresses")) this.ConfirmAddresses = AlwaysConvert.ToBool(configurationData["ConfirmAddresses"], false);
            if (configurationData.ContainsKey("IgnoreFailedConfirm")) this.IgnoreFailedConfirm = AlwaysConvert.ToBool(configurationData["IgnoreFailedConfirm"], false);
            if (configurationData.ContainsKey("ReportBreakdown")) this.ReportBreakdown = AlwaysConvert.ToBool(configurationData["ReportBreakdown"], true);
            if (configurationData.ContainsKey("Location")) this.Location = configurationData["Location"];
            if (configurationData.ContainsKey("Nexus")) this.Nexus = (Nexus)AlwaysConvert.ToInt(configurationData["Nexus"]);
            if (configurationData.ContainsKey("UseLineItems")) this.UseLineItems = AlwaysConvert.ToBool(configurationData["UseLineItems"], true);
        }

        public string Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        public override string Name
        {
            get { return "CertiTAX"; }
        }
        
        /// <summary>
        /// A short description of the tax provider
        /// </summary>
        public override string Description
        {
            get { return "If your business maintains offices or employees in a state, regularly sends employees or agents into a state or own property in a state you most likely have &quot;nexus&quot; with that state. We have the software solution for all merchants who have &quot;nexus&quot;. It's for the e-merchant – and for the traditional merchant who uses the Internet to sell products and services."; }
        }

        public Nexus Nexus
        {
            get { return _Nexus; }
            set { _Nexus = value; }
        }

        private LSDecimal ParseTaxTransaction(CertiTAX.TaxTransaction taxTransaction, Basket basket, int shipmentId)
        {
            if (taxTransaction == null) return 0;
            LSDecimal totalTaxes = 0;
            if (this.ReportBreakdown)
            {
                if (taxTransaction.CityTax > 0)
                {
                    totalTaxes += taxTransaction.CityTax;
                    CreateTaxLineItem(basket, shipmentId, taxTransaction.CityTaxAuthority, taxTransaction.CertiTAXTransactionId, taxTransaction.CityTax);
                }
                if (taxTransaction.CountyTax > 0)
                {
                    totalTaxes += taxTransaction.CountyTax;
                    CreateTaxLineItem(basket, shipmentId, taxTransaction.CountyTaxAuthority, taxTransaction.CertiTAXTransactionId, taxTransaction.CountyTax);
                }
                if (taxTransaction.LocalTax > 0)
                {
                    totalTaxes += taxTransaction.LocalTax;
                    CreateTaxLineItem(basket, shipmentId, taxTransaction.LocalTaxAuthority, taxTransaction.CertiTAXTransactionId, taxTransaction.LocalTax);
                }
                if (taxTransaction.StateTax > 0)
                {
                    totalTaxes += taxTransaction.StateTax;
                    CreateTaxLineItem(basket, shipmentId, taxTransaction.StateTaxAuthority, taxTransaction.CertiTAXTransactionId, taxTransaction.StateTax);
                }
                if (taxTransaction.NationalTax > 0)
                {
                    totalTaxes += taxTransaction.NationalTax;
                    CreateTaxLineItem(basket, shipmentId, taxTransaction.NationalTaxAuthority, taxTransaction.CertiTAXTransactionId, taxTransaction.NationalTax);
                }
                if (taxTransaction.OtherTax > 0)
                {
                    totalTaxes += taxTransaction.OtherTax;
                    CreateTaxLineItem(basket, shipmentId, taxTransaction.OtherTaxAuthority, taxTransaction.CertiTAXTransactionId, taxTransaction.OtherTax);
                }
            }
            else
            {
                totalTaxes += taxTransaction.CityTax;
                totalTaxes += taxTransaction.CountyTax;
                totalTaxes += taxTransaction.LocalTax;
                totalTaxes += taxTransaction.StateTax;
                totalTaxes += taxTransaction.NationalTax;
                totalTaxes += taxTransaction.OtherTax;
                CreateTaxLineItem(basket, shipmentId, "Tax", taxTransaction.CertiTAXTransactionId, totalTaxes);
            }
            return totalTaxes;
        }

        public bool ReportBreakdown
        {
            get { return _ReportBreakdown; }
            set { _ReportBreakdown = value; }
        }

        public string ReferredID
        {
            get
            {
                if (string.IsNullOrEmpty(_ReferredID))
                    _ReferredID = Token.Instance.StoreId.ToString();
                return _ReferredID;
            }
        }

        public bool UseLineItems
        {
            get { return _UseLineItems; }
            set { _UseLineItems = value; }
        }

        public override string Version
        {
            get { return "6.0.0.0"; }
        }

        public static string GetEnrollmentUrl(int storeId, string storeName,string successUrl)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("https://www.esalestax.com/CertiTAX/referred.exe?WSYD_EVENT=ES4roF&amp;RefMrchID=775&amp;RefrMrchID=");
            sb.Append(storeId.ToString());
            sb.Append("&amp;ApplicationName=Able+Commerce+Store+Administration&amp;StoreName=" + System.Web.HttpUtility.UrlEncode(storeName));
            sb.Append("&amp;ReferringName=Able+Commerce&amp;RuleTemplate=Able+Commerce");
            sb.Append("&amp;SuccessURL=" + System.Web.HttpUtility.UrlEncode(successUrl));
            return sb.ToString();
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Taxes.Providers.CCH.CertiTaxLogo.gif");
            return string.Empty;
        }

        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "CCH/Default.aspx";
        }

    }
}
