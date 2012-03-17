using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes.Providers.WATax
{
    internal class DorGateway
    {
        Dictionary<int, TaxAddress> _TaxAddressLookup;
        Dictionary<string, TaxInfo> _TaxInfoLookup;
        private string _TaxName;
        private bool _UseDebug;

        public DorGateway(Basket basket, string taxName, bool useDebug)
            : this(taxName, useDebug)
        {
            _TaxAddressLookup = TaxUtility.GenerateAddressLookup(basket);
        }

        public DorGateway(Order order, string taxName, bool useDebug)
            : this(taxName, useDebug)
        {
            _TaxAddressLookup = TaxUtility.GenerateAddressLookup(order);
        }

        private DorGateway(string taxName, bool useDebug)
        {
            _TaxInfoLookup = new Dictionary<string, TaxInfo>();
            _TaxName = taxName;
            _UseDebug = useDebug;
        }

        public TaxInfo GetTaxInfo(int shipmentId)
        {
            if (!_TaxAddressLookup.ContainsKey(shipmentId)) throw new ArgumentException("shipmentId is not a valid value", "shipmentId");
            TaxAddress taxAddress = _TaxAddressLookup[shipmentId];
            string cacheKey = taxAddress.CacheKey;
            if (!_TaxInfoLookup.ContainsKey(cacheKey))
            {
                _TaxInfoLookup[cacheKey] = GetTaxRate(taxAddress);
            }
            return _TaxInfoLookup[cacheKey];
        }

        /// <summary>
        /// Gets the tax rate that applies to the given address
        /// </summary>
        /// <param name="address">The address to find tax rate for</param>
        /// <returns>A TaxInfo structure that details the applicable tax</returns>
        /// <remarks>If no tax applies to the address, null is returned</remarks>
        private TaxInfo GetTaxRate(TaxAddress address)
        {
            // INITIALIZE TAX RATE AND SKU VALUES
            decimal taxRate = 0M;
            string taxSku = string.Empty;

            // ONLY WASHINGTON ADDRESS WILL HAVE A TAX RATE
            if (address.Province == "WA")
            {

                // PREPARE TAX RATE REQUEST
                //http://dor.wa.gov/AddressRates.aspx?output=xml&addr=6500%20%20Linderson%20way&city=&zip=98501
                StringBuilder requestBuilder = new StringBuilder();
                requestBuilder.Append("http://dor.wa.gov/AddressRates.aspx?output=xml");
                requestBuilder.Append("&addr=").Append(HttpUtility.UrlEncode(address.StreetAddress));
                requestBuilder.Append("&city=").Append(HttpUtility.UrlEncode(address.City));
                requestBuilder.Append("&zip=").Append(HttpUtility.UrlEncode(address.PostalCode));

                // LOG THE REQUEST
                if (this._UseDebug) Logger.WriteProviderLog("WATax", "Send", requestBuilder.ToString());

                // SEND REQUEST
                string xmlResponse = HttpClient.DoGetRequest(requestBuilder.ToString());

                // LOG THE RESPONSE
                if (this._UseDebug) Logger.WriteProviderLog("WATax", "Receive", xmlResponse);

                // LOAD THE RESPONSE XML
                XmlDocument response = new XmlDocument();
                if (!string.IsNullOrEmpty(xmlResponse))
                {
                    try
                    {
                        response.LoadXml(xmlResponse);
                    }
                    catch (XmlException xmlex)
                    {
                        Logger.Error("Error reading tax rate response from WA Department of Revenue.", xmlex);
                    }
                }

                // LOOK FOR TAX RATE
                XmlNode responseNode = response.SelectSingleNode("response");
                if (responseNode != null)
                {
                    XmlAttribute rateAttribute = responseNode.Attributes["rate"];
                    if (rateAttribute != null)
                    {
                        taxRate = AlwaysConvert.ToDecimal(rateAttribute.Value);
                    }
                }

                // LOOK FOR TAX "SKU"
                XmlNode rateNode = responseNode.SelectSingleNode("rate");
                if (rateNode != null)
                {
                    XmlAttribute nameAttribute = rateNode.Attributes["name"];
                    taxSku = nameAttribute.Value;
                }
            }

            // IF THERE IS NO APPLICABLE TAX RETURN NULL
            if (taxRate == 0) return null;

            // RETURN THE TAXINFO DATA
            return new TaxInfo(this._TaxName, taxSku, taxRate);
        }
    }
}