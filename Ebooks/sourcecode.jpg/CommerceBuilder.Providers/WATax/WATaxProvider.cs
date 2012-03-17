using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web.UI;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes.Providers.WATax
{
    public class WATaxProvider : CommerceBuilder.Taxes.Providers.TaxProviderBase
    {
        private IntegerList _TaxCodes = new IntegerList();
        private string _TaxName = "Washington Sales Tax";
        private bool _UseDebugMode;

        /// <summary>
        /// Gets a value that indicates whether the provider is active
        /// </summary>
        public override bool Activated
        {
            get { return true; }
        }

        /// <summary>
        /// Calculates tax for the given basket
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <returns>Tax items are added to the basket and total calculated tax is returned</returns>
        public override LSDecimal Calculate(Basket basket)
        {
            return Calculate(basket, true);
        }

        /// <summary>
        /// Calculates tax for the given basket
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="save">If true, items added to the basket are saved</param>
        /// <returns>Tax items are added to the basket and total calculated tax is returned</returns>
        public LSDecimal Calculate(Basket basket, bool save)
        {
            // CREATE THE DOR GATEWAY INSTANCE
            DorGateway gateway = new DorGateway(basket, this.TaxName, this.UseDebugMode);

            // BUILD A DICTIONARY OF TAXABLE TOTALS BY SHIPMENT
            Dictionary<int, decimal> shipmentTotals = new Dictionary<int, decimal>();

            // LOOP ITEMS IN BASKET TO CALCULATE TAX
            BasketItemCollection newTaxItems = new BasketItemCollection();
            foreach (BasketItem item in basket.Items)
            {
                // SEE IF ITEM HAS A TAX CODE THAT APPLIES
                if (item.TaxCodeId > 0 && this.TaxCodes.Contains(item.TaxCodeId))
                {
                    if (shipmentTotals.ContainsKey(item.BasketShipmentId))
                    {
                        shipmentTotals[item.BasketShipmentId] += (decimal)item.ExtendedPrice;
                    }
                    else
                    {
                        shipmentTotals[item.BasketShipmentId] = (decimal)item.ExtendedPrice;
                    }
                }
            }

            // LOOP TAXABLE SHIPMENT TOTALS
            LSDecimal totalTax = 0;
            foreach (int shipmentId in shipmentTotals.Keys)
            {
                // SEE IF THERE IS A TAXINFO INSTANCE FOR THIS ITEM
                TaxInfo taxInfo = gateway.GetTaxInfo(shipmentId);
                if (taxInfo != null)
                {
                    // A TAXINFO STRUCTURE EXISTS, SO CREATE TAX ITEM AND ADD TO BASKET
                    BasketItem taxItem = TaxUtility.GenerateBasketItem(basket.BasketId, shipmentId, shipmentTotals[shipmentId], taxInfo);
                    totalTax += taxItem.ExtendedPrice;
                    basket.Items.Add(taxItem);
                }
            }

            // SAVE BASKET IF INDICATED
            if (save) basket.Items.Save();

            // return total tax added to basekt
            return totalTax;
        }

        /// <summary>
        /// Generates a configuration data dictionary for this provider instance
        /// </summary>
        /// <returns>A string dictionary of configuration parameters</returns>
        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData.Add("UseDebugMode", this.UseDebugMode.ToString());
            configData.Add("TaxName", this.TaxName);
            configData.Add("TaxCodes", this.TaxCodes.ToString());
            return configData;
        }

        /// <summary>
        /// Cancels pending tax
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <remarks>Does not apply to the WATax provider</remarks>
        public override void Cancel(Basket basket)
        {
            // NO ACTION REQUIRED
        }

        /// <summary>
        /// Commits pending tax
        /// </summary>
        /// <param name="order">The order</param>
        /// <remarks>Does not apply to the WATax provider</remarks>
        public override void Commit(Order order)
        {
            // NO ACTION REQUIRED
        }

        /// <summary>
        /// Gets a description of the provider
        /// </summary>
        public override string Description
        {
            get { return "Businesses that must collect destination based sales tax in Washington state  may wish to enable this integration with the tax rate lookup service provided by Washington Department of Revenue.  When enabled, AbleCommerce can accurately calculate and charge tax for items being shipped to Washington.  (This integration is not certified by nor affiliated with Washington DOR.)"; }
        }

        /// <summary>
        /// Gets the url that points to the provider configuration
        /// </summary>
        /// <param name="cs">Client script manager</param>
        /// <returns>The url that points to the provider configuration</returns>
        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "WATax/Default.aspx";
        }

        /// <summary>
        /// Gets the image url for the provider
        /// </summary>
        /// <param name="cs">Client script manager</param>
        /// <returns>The url that points to the provider logo url</returns>
        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
            {
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Taxes.Providers.WATax.Logo.gif");
            }
            return string.Empty;
        }

        /// <summary>
        /// Initializes this tax gateway with the given configuration data
        /// </summary>
        /// <param name="taxGatewayId">ID of this tax gateway</param>
        /// <param name="configurationData">configuration dictionary</param>
        public override void Initialize(int taxGatewayId, Dictionary<string, string> configurationData)
        {
            base.Initialize(taxGatewayId, configurationData);
            if (configurationData.ContainsKey("UseDebugMode")) this.UseDebugMode = AlwaysConvert.ToBool(configurationData["UseDebugMode"], false);
            if (configurationData.ContainsKey("TaxName")) this.TaxName = configurationData["TaxName"];
            if (configurationData.ContainsKey("TaxCodes")) this.TaxCodes.AddRange(configurationData["TaxCodes"]);
        }

        /// <summary>
        /// Gets the name of the provider
        /// </summary>
        public override string Name
        {
            get { return "WA State Department of Revenue"; }
        }

        /// <summary>
        /// Calculates and applies taxes for the given order
        /// </summary>
        /// <param name="basket">The order to calculate taxes on.</param>
        /// <returns>The total amount of tax applied.</returns>
        /// <remarks>Any pre-existing tax line items must be removed from the order before the calculation.</remarks>
        public override LSDecimal Recalculate(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            // CREATE THE DOR GATEWAY INSTANCE
            DorGateway gateway = new DorGateway(order, this.TaxName, this.UseDebugMode);

            // BUILD A DICTIONARY OF TAXABLE TOTALS BY SHIPMENT
            Dictionary<int, decimal> shipmentTotals = new Dictionary<int, decimal>();

            // LOOP ITEMS IN BASKET TO CALCULATE TAX
            OrderItemCollection newTaxItems = new OrderItemCollection();
            foreach (OrderItem item in order.Items)
            {
                // SEE IF ITEM HAS A TAX CODE THAT APPLIES
                if (item.TaxCodeId > 0 && this.TaxCodes.Contains(item.TaxCodeId))
                {
                    if (shipmentTotals.ContainsKey(item.OrderShipmentId))
                    {
                        shipmentTotals[item.OrderShipmentId] += (decimal)item.ExtendedPrice;
                    }
                    else
                    {
                        shipmentTotals[item.OrderShipmentId] = (decimal)item.ExtendedPrice;
                    }
                }
            }

            // LOOP TAXABLE SHIPMENT TOTALS
            LSDecimal totalTax = 0;
            foreach (int shipmentId in shipmentTotals.Keys)
            {
                // SEE IF THERE IS A TAXINFO INSTANCE FOR THIS ITEM
                TaxInfo taxInfo = gateway.GetTaxInfo(shipmentId);
                if (taxInfo != null)
                {
                    // A TAXINFO STRUCTURE EXISTS, SO CREATE TAX ITEM AND ADD TO BASKET
                    OrderItem taxItem = TaxUtility.GenerateOrderItem(order.OrderId, shipmentId, shipmentTotals[shipmentId], taxInfo);
                    totalTax += taxItem.ExtendedPrice;
                    order.Items.Add(taxItem);
                }
            }

            // SAVE ORDER
            order.Items.Save();

            // return total tax added to basekt
            return newTaxItems.TotalPrice();
        }

        /// <summary>
        /// The tax codes that this provider calculates taxes for
        /// </summary>
        public IntegerList TaxCodes
        {
            get { return _TaxCodes; }
        }

        /// <summary>
        /// Gets or sets the name to set for the taxes that are calculated
        /// </summary>
        public string TaxName
        {
            get { return _TaxName; }
            set { _TaxName = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether communication with the tax gatewy should be logged
        /// </summary>
        public bool UseDebugMode
        {
            get { return _UseDebugMode; }
            set { _UseDebugMode = value; }
        }

        /// <summary>
        /// Get the version of this provider
        /// </summary>
        public override string Version
        {
            get { return "7.5"; }
        }
    }
}