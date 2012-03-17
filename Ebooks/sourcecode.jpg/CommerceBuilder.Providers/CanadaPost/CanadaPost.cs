using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;

namespace CommerceBuilder.Shipping.Providers.CanadaPost
{
    public class CanadaPost : ShippingProviderBase
    {

        private string _MerchantCPCID = string.Empty;
        public string MerchantCPCID
        {
            get { return _MerchantCPCID; }
            set { _MerchantCPCID = value; }
        }

        private bool _EnablePackageBreakup = true;
        public bool EnablePackageBreakup
        {
            get { return _EnablePackageBreakup; }
            set { _EnablePackageBreakup = value; }
        }

        private LSDecimal _MinPackageWeight = (decimal)0.1;
        public LSDecimal MinPackageWeight
        {
            get { return _MinPackageWeight; }
            set { _MinPackageWeight = value; }
        }

        //max weight for CanadaPost 30 kgs
        private LSDecimal _MaxPackageWeight = 30;
        public LSDecimal MaxPackageWeight
        {
            get { return _MaxPackageWeight; }
            set { _MaxPackageWeight = value; }
        }

        private bool _UseTestMode = false;
        public bool UseTestMode
        {
            get { return _UseTestMode; }
            set { _UseTestMode = value; }
        }

        private bool _AccountActive = false;
        public bool AccountActive
        {
            get { return _AccountActive; }
            set { _AccountActive = value; }
        }

        private string _TestModeUrl = "http://sellonline.canadapost.ca:30000";
        public string TestModeUrl
        {
            get { return _TestModeUrl; }
            set { _TestModeUrl = value; }
        }

        private string _LiveModeUrl = "http://sellonline.canadapost.ca:30000";
        public string LiveModeUrl
        {
            get { return _LiveModeUrl; }
            set { _LiveModeUrl = value; }
        }

        //https://obc.canadapost.ca/emo/basicPin.do?trackingCode=PIN&referenceNumberPressed=false&language=en&action=query&fromPage=basicPin&trackingId={0}            
        private static string _DefaultTrackingUrl = "https://obc.canadapost.ca/emo/basicPin.do?trackingCode=PIN&referenceNumberPressed=false&language=en&action=query&fromPage=basicPin&trackingId={0}";
        private string _TrackingUrl;
        public string TrackingUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_TrackingUrl))
                {
                    return _DefaultTrackingUrl;
                }
                else
                {
                    return _TrackingUrl;
                }
            }
            set { _TrackingUrl = value; }
        }

        public override string Name
        {
            get { return "CanadaPost"; }
        }

        public override string Version
        {
            get { return "Sell Online v3.0 June 2000"; }
        }

        private static Dictionary<string, string> _services;
        static CanadaPost()
        {
            _services = new Dictionary<string, string>();
            _services.Add("1010", "CanadaPost Regular");
            _services.Add("1020", "CanadaPost Expedited");
            _services.Add("1030", "CanadaPost XPressPost");
            _services.Add("1040", "CanadaPost Priority Courier");
            _services.Add("1120", "CanadaPost Expedited Evening");
            _services.Add("1130", "CanadaPost XPressPost Evening");
            _services.Add("1220", "CanadaPost Expedited Saturday");
            _services.Add("1230", "CanadaPost XPressPost Saturday");
            _services.Add("2010", "CanadaPost Small Packets - Surface");
            _services.Add("2015", "CanadaPost Small Packets Air");
            _services.Add("2020", "CanadaPost Air US");
            _services.Add("2025", "CanadaPost Expedited US Commercial");
            _services.Add("2030", "CanadaPost Xpresspost USA");
            _services.Add("2040", "CanadaPost US Purolator Courier");
            _services.Add("2050", "CanadaPost Puropak US");
            _services.Add("3010", "CanadaPost Surface International");
            _services.Add("3020", "CanadaPost Air International");
            _services.Add("3040", "CanadaPost Purolator International");
            _services.Add("3050", "CanadaPost Puropak International");
        }

        #region Interface_Implementation_methods

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Shipping.Providers.CanadaPost.Logo.gif");
            return string.Empty;
        }

        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "CanadaPost/Default.aspx";
        }

        public override string Description
        {
            get { return "The integrated CanadaPost module can generate real-time shipping rates for your packages. It also can provide tracking details."; }
        }

        public override void Initialize(int ShipGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(ShipGatewayId, ConfigurationData);
            //INITIALIZE MY FIELDS
            if (ConfigurationData.ContainsKey("MerchantCPCID")) MerchantCPCID = ConfigurationData["MerchantCPCID"];
            if (ConfigurationData.ContainsKey("EnablePackageBreakup")) EnablePackageBreakup = AlwaysConvert.ToBool(ConfigurationData["EnablePackageBreakup"], true);
            if (ConfigurationData.ContainsKey("MinPackageWeight")) MinPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MinPackageWeight"], (decimal)MinPackageWeight);
            if (ConfigurationData.ContainsKey("MaxPackageWeight")) MaxPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MaxPackageWeight"], (decimal)MaxPackageWeight);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], false);
            if (ConfigurationData.ContainsKey("AccountActive")) AccountActive = AlwaysConvert.ToBool(ConfigurationData["AccountActive"], false);
            if (ConfigurationData.ContainsKey("TestModeUrl")) TestModeUrl = ConfigurationData["TestModeUrl"];
            if (ConfigurationData.ContainsKey("LiveModeUrl")) LiveModeUrl = ConfigurationData["LiveModeUrl"];
            if (ConfigurationData.ContainsKey("TrackingUrl")) TrackingUrl = ConfigurationData["TrackingUrl"];
        }

        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData.Add("MerchantCPCID", this.MerchantCPCID);
            configData.Add("EnablePackageBreakup", this.EnablePackageBreakup.ToString());
            configData.Add("MinPackageWeight", this.MinPackageWeight.ToString());
            configData.Add("MaxPackageWeight", this.MaxPackageWeight.ToString());
            configData.Add("UseTestMode", this.UseTestMode.ToString());
            configData.Add("AccountActive", this.AccountActive.ToString());
            configData.Add("TestModeUrl", this.TestModeUrl);
            configData.Add("LiveModeUrl", this.LiveModeUrl);
            configData.Add("TrackingUrl", this.TrackingUrl);
            return configData;
        }

        public override ListItem[] GetServiceListItems()
        {
            List<ListItem> services = new List<ListItem>();
            string value;
            foreach (string key in _services.Keys)
            {
                value = _services[key];
                services.Add(new ListItem(value, key));
            }
            return services.ToArray();
        }

        public override ShipRateQuote GetShipRateQuote(Warehouse origin, CommerceBuilder.Users.Address destination, BasketItemCollection contents, string serviceCode)
        {
            Dictionary<string, ProviderShipRateQuote> allQuotes = GetAllServiceQuotes(origin, destination, contents);

            if ((allQuotes != null) && allQuotes.ContainsKey(serviceCode))
            {
                return allQuotes[serviceCode];
            }
            else
            {
                return null;
            }
        }

        public override TrackingSummary GetTrackingSummary(TrackingNumber trackingNumber)
        {
            TrackingSummary summary = new TrackingSummary();
            summary.TrackingResultType = TrackingResultType.ExternalLink;
            summary.TrackingLink = string.Format(TrackingUrl, HttpUtility.UrlEncode(trackingNumber.TrackingNumberData));
            return summary;
        }

        #endregion Interface_Implementation_methods

        #region Implementation_Support_methods

        private TrackingSummary ParseTrackResponse(XmlDocument trackResponse)
        {
            //TODO
            throw new NotImplementedException("Feature Not Implemented");
        }

        private Dictionary<string, ProviderShipRateQuote> GetAllServiceQuotes(Warehouse origin, Address destination, BasketItemCollection contents)
        {
            string cacheKey = StringHelper.CalculateMD5Hash(Utility.Misc.GetClassId(this.GetType()) + "_" + origin.WarehouseId.ToString() + "_" + destination.AddressId.ToString() + "_" + contents.GenerateContentHash());
            HttpContext context = HttpContext.Current;
            if ((context != null) && (context.Items.Contains(cacheKey)))
                return (Dictionary<string, ProviderShipRateQuote>)context.Items[cacheKey];

            //VERIFY WE HAVE A DESTINATION COUNTRY
            if (string.IsNullOrEmpty(destination.CountryCode)) return null;

            PackageList plist = PreparePackages(origin, contents);
            if (plist == null || plist.Count == 0) return null;
            Dictionary<string, ProviderShipRateQuote> allQuotes = new Dictionary<string, ProviderShipRateQuote>();
            List<ProviderShipRateQuote> providerQuotes;
            ProviderShipRateQuote tempQuote;
            
            foreach (Package item in plist)
            {
                providerQuotes = GetProviderQuotes(origin, destination, item);
                foreach (ProviderShipRateQuote quote in providerQuotes)
                {
                    if (allQuotes.ContainsKey(quote.ServiceCode))
                    {
                        tempQuote = allQuotes[quote.ServiceCode];
                        tempQuote.AddPackageQoute(quote);
                    }
                    else
                    {
                        allQuotes.Add(quote.ServiceCode, quote);
                    }
                }
            }
            
            RemoveInEffectiveQuotes(allQuotes, plist.Count);
            
            if (context != null) context.Items.Add(cacheKey, allQuotes);

            return allQuotes;
        }

       private void RemoveInEffectiveQuotes(Dictionary<string,ProviderShipRateQuote> allQuotes, int count) 
       {
           foreach (ProviderShipRateQuote quote in allQuotes.Values)
           {
               if (quote.PackageCount < count || quote.Rate <= 0)
               {
                   allQuotes.Remove(quote.ServiceCode);
               }
           }
       }

       private List<ProviderShipRateQuote> GetProviderQuotes(Warehouse origin, Address destination, Package package)
       {
           if (origin.CountryCode == "CA")
           {
               XmlDocument providerRequest = BuildProviderRequest(origin, destination, package);

               if (this.UseDebugMode)
               {
                   this.RecordCommunication("CanadaPost", CommunicationDirection.Send, providerRequest.OuterXml);
               }

               XmlDocument providerResponse = SendRequestToProvider(providerRequest, GetEffectiveUrl(), "iso-8859-1");

               if (this.UseDebugMode)
               {
                   this.RecordCommunication("CanadaPost", CommunicationDirection.Receive, providerResponse.OuterXml);
               }

               if (providerResponse != null)
                   return ParseProviderResponse(providerResponse, destination);
           }
           return new List<ProviderShipRateQuote>();
       }

        private List<ProviderShipRateQuote> ParseProviderResponse(XmlDocument providerResponse, Address destination)
        {
            List<ProviderShipRateQuote> providerQuotes = new List<ProviderShipRateQuote>();
            XmlElement respDocElement = providerResponse.DocumentElement;
            XmlNodeList errorList = respDocElement.GetElementsByTagName("error");
            if (errorList.Count > 0)
            {
                string errorCode = XmlUtility.GetElementValue(respDocElement, "error/statusCode",string.Empty);
                string errorMessage = XmlUtility.GetElementValue(respDocElement, "error/statusMessage", "Unknown Error.");
                //IF AN ERROR IS THROWN HERE, SHIPPING RATES WILL NEVER BE CACHED
                //SO THE PROCESS WILL CONTINUE TO REQUEST AGAINST CP SERVER FOR EACH SHIPPING METHOD
                //INSTEAD THE ERROR IS LOGGED AND WE SHOULD RETURN AN EMPTY RATE LIST
                Logger.Error("Error retrieving CanadaPost rates; Code = " + errorCode + "; Message = " + errorMessage);
            }
            else
            {
                XmlElement nodEntry = null;
                string tempServiceCode = string.Empty;
                LSDecimal tempRate;
                ProviderShipRateQuote quote;
                XmlNodeList nlsPackages = respDocElement.GetElementsByTagName("product");
                foreach (XmlNode nodeItem in nlsPackages)
                {
                    nodEntry = (XmlElement)nodeItem;
                    tempServiceCode = nodEntry.GetAttribute("id");
                    tempRate = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodEntry, "rate"), 0);
                    if (tempRate > 0)
                    {
                        quote = new ProviderShipRateQuote();
                        quote.ServiceCode = tempServiceCode;
                        quote.ServiceName = GetServiceName(tempServiceCode);
                        quote.Rate = tempRate;
                        quote.PackageCount = 1;
                        providerQuotes.Add(quote);
                    }
                }
                nodEntry = null;
                nlsPackages = null;
            }
            return providerQuotes;
        }

        private string GetServiceName(String serviceCode)
        {
            string serviceName = serviceCode + " : Unknown";
            if(_services.ContainsKey(serviceCode)){
                serviceName = _services[serviceCode];
                if (serviceName == null) serviceName = serviceCode + " : Unknown";
            }
            return serviceName;
        }

        private XmlDocument BuildProviderRequest(Warehouse warehouse, Address destination, Package package)
        {
            XmlDocument xmlRequest = new XmlDocument();
            xmlRequest.LoadXml(RequestXMLString);
            XmlElement reqDocElement = xmlRequest.DocumentElement;
            //XmlUtility.SetElementValue(reqDocElement, "language", "en");
            XmlUtility.SetElementValue(reqDocElement, "ratesAndServicesRequest/merchantCPCID", this.MerchantCPCID);
            XmlUtility.SetElementValue(reqDocElement, "ratesAndServicesRequest/itemsPrice", "" + package.RetailValue);
            //ORIGIN
            XmlUtility.SetElementValue(reqDocElement, "ratesAndServicesRequest/fromPostalCode", warehouse.PostalCode);
            //Destination
            XmlUtility.SetElementValue(reqDocElement, "ratesAndServicesRequest/city", destination.City);
            XmlUtility.SetElementValue(reqDocElement, "ratesAndServicesRequest/provOrState", destination.Province);
            XmlUtility.SetElementValue(reqDocElement, "ratesAndServicesRequest/postalCode", destination.PostalCode);
            XmlUtility.SetElementValue(reqDocElement, "ratesAndServicesRequest/country", destination.CountryCode);

            //for (int i = 0; i < package.Multiplier; i++)
            //{
                CreateShipItem(xmlRequest, package);
            //}

            return xmlRequest;
        }

        private void CreateShipItem(XmlDocument xmlRequest, Package pitem)
        {
            XmlElement nodItem = xmlRequest.CreateElement("item");
            XmlUtility.SetElementValue(nodItem, "quantity", "" + pitem.Multiplier);
            //XmlUtility.SetElementValue(nodItem, "quantity", "1");
            //WE DO NOT WANT TO SET MINIMUM WEIGHT (BUG 5426)
            //if (pitem.Weight < 0.1M) pitem.Weight = 0.1M;
            XmlUtility.SetElementValue(nodItem, "weight", pitem.Weight.ToString("F1"));
            XmlUtility.SetElementValue(nodItem, "length", "" + pitem.Length);
            XmlUtility.SetElementValue(nodItem, "width", "" + pitem.Width);
            XmlUtility.SetElementValue(nodItem, "height", "" + pitem.Height);
            if (!string.IsNullOrEmpty(pitem.Name))
            {
                XmlUtility.SetElementValue(nodItem, "description", pitem.Name);
            }
            else
            {
                XmlUtility.SetElementValue(nodItem, "description", "None");
            }
            XmlUtility.SetElementValue(nodItem, "readyToShip", "true");
            XmlUtility.GetElement(xmlRequest.DocumentElement, "ratesAndServicesRequest/lineItems", true).AppendChild(nodItem);
        }

        private PackageList PreparePackages(Warehouse origin, BasketItemCollection contents)
        {
            PackageList plist = PackageManager.GetPackageList(contents);
            if (plist == null || plist.Count == 0) return null;

            ProviderUnits pUnits = GetProviderUnits(origin.Country);
            //GET UNITS USED BY STORE
            Store store = StoreDataSource.Load(Token.Instance.StoreId);
            MeasurementUnit storeMeasurementUnit = store.MeasurementUnit;
            WeightUnit storeWeightUnit = store.WeightUnit;

            bool requireMC = storeMeasurementUnit != pUnits.MeasurementUnit;
            bool requireWC = storeWeightUnit != pUnits.WeightUnit;

            if (requireMC && requireWC)
            {
                plist.ConvertBoth(pUnits.WeightUnit, pUnits.MeasurementUnit);
            }
            else if (requireWC)
            {
                plist.ConvertWeight(pUnits.WeightUnit);
            }
            else if (requireMC)
            {
                plist.ConvertDimensions(pUnits.MeasurementUnit);
            }
                        
            LSDecimal maxWeight = LocaleHelper.ConvertWeight(WeightUnit.Kilograms, MaxPackageWeight, pUnits.WeightUnit);
            LSDecimal minWeight = LocaleHelper.ConvertWeight(WeightUnit.Kilograms, MinPackageWeight, pUnits.WeightUnit);
            if (EnablePackageBreakup && maxWeight > 0)
            {
                //compose packages (splits items larger than the maximum carrier weight)
                plist.Compose(maxWeight, minWeight);
            }
            else
            {
                plist.EnsureMinimumWeight(minWeight);
            }
            
            //convert weights to whole numbers 
            //plist.ConvertWeightToWholeNumber();

            return plist;
        }

        private ProviderUnits GetProviderUnits(Country originCountry)
        {
            ProviderUnits pUnits = new ProviderUnits();
            pUnits.MeasurementUnit = MeasurementUnit.Centimeters;
            pUnits.WeightUnit = WeightUnit.Kilograms;
            return pUnits;
        }

        private string GetMeasurementUnitCode(MeasurementUnit munit)
        {
            switch (munit)
            {
                case MeasurementUnit.Inches:
                    return "IN";
                case MeasurementUnit.Centimeters:
                    return "CM";
                default:
                    return "";
            }
        }

        private string GetWeightUnitCode(WeightUnit wunit)
        {
            switch (wunit)
            {
                case WeightUnit.Grams:
                    return "GM";
                case WeightUnit.Kilograms:
                    return "KG";
                case WeightUnit.Ounces:
                    return "OZ";
                case WeightUnit.Pounds:
                    return "LB";
                default:
                    return "";
            }
        }

        #endregion Implementation_Support_methods

        #region Configuration_Support_Methods

        public bool IsActive
        {
            get { return (!string.IsNullOrEmpty(this.MerchantCPCID)); }
        }

        #endregion Configuration_Support_Methods

        struct ProviderUnits
        {
            public MeasurementUnit MeasurementUnit;
            public WeightUnit WeightUnit;
        }


        private string GetEffectiveUrl()
        {
            if (this.UseTestMode)
            {
                return TestModeUrl;
            }
            else
            {
                return LiveModeUrl;
            }
        }
        
        private const String RequestXMLString = "<?xml version=\"1.0\" ?> "
        + "<eparcel><language>en</language><ratesAndServicesRequest>"
        + "<merchantCPCID></merchantCPCID><fromPostalCode></fromPostalCode>"
        + "<itemsPrice></itemsPrice><lineItems></lineItems><city></city>"
        + "<provOrState></provOrState> <country></country> <postalCode></postalCode> "
        + "</ratesAndServicesRequest></eparcel>";
    }
}
