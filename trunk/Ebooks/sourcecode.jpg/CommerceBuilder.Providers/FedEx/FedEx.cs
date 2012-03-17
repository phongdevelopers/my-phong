using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;


namespace CommerceBuilder.Shipping.Providers.FedEx
{
    public class FedEx : ShippingProviderBase
    {
        private string _AccountNumber = string.Empty;
        public string AccountNumber
        {
            get { return _AccountNumber; }
            set { _AccountNumber = value; }
        }

        private string _MeterNumber = string.Empty;
        public string MeterNumber
        {
            get { return _MeterNumber; }
            set { _MeterNumber = value; }
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

        private LSDecimal _MaxPackageWeight;
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

        private FDXDropOffType _DropOffType = FDXDropOffType.REGULARPICKUP;
        public FDXDropOffType DropOffType
        {
            get
            {
                return _DropOffType;
            }
            set
            {
                _DropOffType = value;
            }
        }

        private FDXPackagingType _PackagingType = FDXPackagingType.YOURPACKAGING;
        public FDXPackagingType PackagingType
        {
            get
            {
                return _PackagingType;
            }
            set
            {
                _PackagingType = value;
            }
        }

        private string _DefaultTestModeUrl = "https://gatewaybeta.fedex.com/GatewayDC";
        private string _TestModeUrl;
        public string TestModeUrl
        {
            get{
                if (string.IsNullOrEmpty(_TestModeUrl))
                {
                    return _DefaultTestModeUrl;
                }
                else
                {
                    return _TestModeUrl; 
                }
            }
            set { _TestModeUrl = value; }
        }

        private string _DefaultLiveModeUrl = "https://gateway.fedex.com/GatewayDC";
        private string _LiveModeUrl;
        public string LiveModeUrl 
        {
            get {
                if (string.IsNullOrEmpty(_LiveModeUrl))
                {
                    return _DefaultLiveModeUrl;
                }
                else
                {
                    return _LiveModeUrl;
                }
            }
            set { _LiveModeUrl = value; }
        }
        
        //string link = "http://www.fedex.com/Tracking?ascend_header=1&clienttype=dotcom&tracknumbers={0}";
        private static string _DefaultTrackingUrl = "http://www.fedex.com/Tracking?tracknumbers={0}";
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

        private bool _IncludeDeclaredValue = true;
        public bool IncludeDeclaredValue
        {
            get { return _IncludeDeclaredValue; }
            set { _IncludeDeclaredValue = value; }
        }

        public override string Name
        {
            get { return "FedEx"; }
        }

        public override string Version
        {
            get { return "1.0.0"; }
        }

        private static Dictionary<string, string> _services;
        static FedEx()
        {
            _services = new Dictionary<string, string>();
            _services.Add("PRIORITYOVERNIGHT", "FedEx Priority");
            _services.Add("STANDARDOVERNIGHT", "FedEx Standard Overnight");
            _services.Add("FIRSTOVERNIGHT", "FedEx First Overnight");
            _services.Add("FEDEX2DAY", "FedEx 2day");
            _services.Add("FEDEXEXPRESSSAVER", "FedEx Express Saver");
            _services.Add("FEDEX1DAYFREIGHT", "FedEx 1day Freight");
            _services.Add("FEDEX2DAYFREIGHT", "FedEx 2day Freight");
            _services.Add("FEDEX3DAYFREIGHT", "FedEx 3day Freight");
            _services.Add("FEDEXGROUND", "FedEx Ground");
            _services.Add("GROUNDHOMEDELIVERY", "FedEx Ground Home Delivery");
            _services.Add("INTERNATIONALPRIORITY", "FedEx International Priority");
            _services.Add("INTERNATIONALECONOMY", "FedEx International Economy");
            _services.Add("INTERNATIONALFIRST", "FedEx International First");
            _services.Add("INTERNATIONALPRIORITYFREIGHT", "FedEx International Priority Freight");
            _services.Add("INTERNATIONALECONOMYFREIGHT", "FedEx International Economy Freight");
            _services.Add("EUROPEFIRSTINTERNATIONALPRIORITY", "FedEx Euro First International Priority");            
        }


        public FedEx()
        {
            if (Token.Instance.Store != null)
            {
                // DEFAULT VALUES FOR MAX PACKAGE WEIGHT AS PER STORE WEIGHT UNIT SETTINGS
                // DETAILS ON (bug # 6786 and 8821)
                if (Token.Instance.Store.WeightUnit == WeightUnit.Grams || Token.Instance.Store.WeightUnit == WeightUnit.Kilograms)
                {
                    // 68 KGS
                    _MaxPackageWeight = 68;
                }
                // 150 LBS
                else _MaxPackageWeight = 150;
            }
        }

        #region Interface_Implementation_methods

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Shipping.Providers.FedEx.Logo.gif");
            return string.Empty;
        }

        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "FedEx/Default.aspx";
        }

        public override string Description
        {
            get { return "The integrated FedEx module can generate real-time shipping rates for your packages. It also can provide tracking details."; }
        }

        public override void Initialize(int ShipGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(ShipGatewayId, ConfigurationData);
            //INITIALIZE MY FIELDS
            if (ConfigurationData.ContainsKey("AccountNumber")) AccountNumber = ConfigurationData["AccountNumber"];
            if (ConfigurationData.ContainsKey("MeterNumber")) MeterNumber = ConfigurationData["MeterNumber"];
            if (ConfigurationData.ContainsKey("EnablePackageBreakup")) EnablePackageBreakup = AlwaysConvert.ToBool(ConfigurationData["EnablePackageBreakup"], true);
            if (ConfigurationData.ContainsKey("MinPackageWeight")) MinPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MinPackageWeight"], (decimal)MinPackageWeight);
            if (ConfigurationData.ContainsKey("MaxPackageWeight")) MaxPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MaxPackageWeight"], (decimal)MaxPackageWeight);
            if (ConfigurationData.ContainsKey("IncludeDeclaredValue")) IncludeDeclaredValue = AlwaysConvert.ToBool(ConfigurationData["IncludeDeclaredValue"], true);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], false);
            if (ConfigurationData.ContainsKey("AccountActive")) AccountActive = AlwaysConvert.ToBool(ConfigurationData["AccountActive"], false);
            if (ConfigurationData.ContainsKey("DropOffType")) DropOffType = (FDXDropOffType)AlwaysConvert.ToEnum(typeof(FDXDropOffType), ConfigurationData["DropOffType"],FDXDropOffType.REGULARPICKUP,true);
            if (ConfigurationData.ContainsKey("PackagingType")) PackagingType = (FDXPackagingType)AlwaysConvert.ToEnum(typeof(FDXPackagingType), ConfigurationData["PackagingType"], FDXPackagingType.YOURPACKAGING, true);
            if (ConfigurationData.ContainsKey("TestModeUrl")) TestModeUrl = ConfigurationData["TestModeUrl"];
            if (ConfigurationData.ContainsKey("LiveModeUrl")) LiveModeUrl = ConfigurationData["LiveModeUrl"];
            if (ConfigurationData.ContainsKey("TrackingUrl")) TrackingUrl = ConfigurationData["TrackingUrl"];
        }

        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData.Add("AccountNumber", this.AccountNumber);
            configData.Add("MeterNumber", this.MeterNumber);
            configData.Add("EnablePackageBreakup", this.EnablePackageBreakup.ToString());
            configData.Add("MinPackageWeight", this.MinPackageWeight.ToString());
            configData.Add("MaxPackageWeight", this.MaxPackageWeight.ToString());
            configData.Add("IncludeDeclaredValue", this.IncludeDeclaredValue.ToString());
            configData.Add("UseTestMode", this.UseTestMode.ToString());
            configData.Add("AccountActive", this.AccountActive.ToString());
            configData.Add("DropOffType", this.DropOffType.ToString());
            configData.Add("PackagingType", this.PackagingType.ToString());
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
                //ProviderShipRateQuote providerQuote = allQuotes[serviceCode];
                //quote = new ShipRateQuote();                
                //quote.Rate = providerQuote.Postage;
                //quote.Warnings = providerQuote.Warnings;
                //quote = providerQuote;
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

        private WeightUnit ParseWeightUnit(String weightUnit)
        {
            if (string.IsNullOrEmpty(weightUnit))
            {
                return WeightUnit.Pounds;
            }            
            if(weightUnit.Equals("KGS",StringComparison.CurrentCultureIgnoreCase)) 
            {
                return WeightUnit.Kilograms;
            }
            else if (weightUnit.Equals("LBS", StringComparison.CurrentCultureIgnoreCase))
            {
                return WeightUnit.Pounds;
            }
            else if (weightUnit.Equals("GMS", StringComparison.CurrentCultureIgnoreCase))
            {
                return WeightUnit.Grams;
            }
            else if (weightUnit.Equals("OZS", StringComparison.CurrentCultureIgnoreCase))
            {
                return WeightUnit.Ounces;
            }
            else
            {
                return WeightUnit.Pounds;
            }
        }

        //input format is xs:data and xs:time for strDate and strTime respectively
        // YYYY-MM-DD and hh:mm:ss
        private DateTime ParseFedExDate(string dateValue, string timeValue)
        {            
            if ((!string.IsNullOrEmpty(dateValue)) && (dateValue.Length == 10))
            {
                int year = int.Parse(dateValue.Substring(0, 4));
                int month = int.Parse(dateValue.Substring(5, 2));
                int day = int.Parse(dateValue.Substring(8, 2));
                if ((!string.IsNullOrEmpty(timeValue)) && (timeValue.Length == 8))
                {
                    int hour = int.Parse(timeValue.Substring(0, 2));
                    int minute = int.Parse(timeValue.Substring(3, 2));
                    int second = int.Parse(timeValue.Substring(6, 2));
                    return (new DateTime(year, month, day, hour, minute, second));
                }
                return new DateTime(year, month, day);
            }
            return System.DateTime.MinValue;
        }

        private Dictionary<string, ProviderShipRateQuote> GetAllServiceQuotes(Warehouse origin, Address destination, BasketItemCollection contents)
        {            
            string cacheKey = StringHelper.CalculateMD5Hash(Misc.GetClassId(this.GetType()) + "_" + origin.WarehouseId.ToString() + "_" + destination.AddressId.ToString() + "_" + contents.GenerateContentHash());
            HttpContext context = HttpContext.Current;
            if ((context != null) && (context.Items.Contains(cacheKey)))
                return (Dictionary<string, ProviderShipRateQuote>)context.Items[cacheKey];

            //VERIFY WE HAVE A DESTINATION COUNTRY
            if (string.IsNullOrEmpty(destination.CountryCode)) return null;

            PackageList plist = PreparePackages(origin, destination, contents);
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

        private void RemoveInEffectiveQuotes(Dictionary<string, ProviderShipRateQuote> allQuotes, int count) 
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
           XmlDocument providerRequest = BuildProviderRequest(origin, destination, package);
           
           if (this.UseDebugMode)
           {
                this.RecordCommunication("FedEx", CommunicationDirection.Send, providerRequest.OuterXml);   
           }

           XmlDocument providerResponse;           
           try
           {
               providerResponse = SendRequestToProvider(providerRequest, GetEffectiveUrl(), "iso-8859-1");
           }
           catch (Exception exp)
           {
               Logger.Warn("Failed to send data to FedEx. ", exp);
               throw exp;
           }

           if (this.UseDebugMode)
           {
               this.RecordCommunication("FedEx", CommunicationDirection.Receive, providerResponse.OuterXml);
           }

           if (providerResponse != null)
            {   
                return ParseProviderResponse(providerResponse, destination, package);
            }
            else
            {
                return new List<ProviderShipRateQuote>();
            }
        }

        private List<ProviderShipRateQuote> ParseProviderResponse(XmlDocument providerResponse, Address destination, Package package)
        {
            List<ProviderShipRateQuote> providerQuotes = new List<ProviderShipRateQuote>();
            string softError = "";

            //check for error node
            XmlElement nodError = XmlUtility.GetElement(providerResponse, "Error", false);
            if (nodError == null) {
                //check doc element too
                nodError = XmlUtility.GetElement(providerResponse.DocumentElement, "Error", false);
            }

            if (nodError != null) {
                //error in rate request                
                string errorCode = XmlUtility.GetElementValue(nodError, "Code", string.Empty);
                string errorMessage = XmlUtility.GetElementValue(nodError, "Message", "Unknown Error."); 
                throw new ShipProviderException(errorCode + " : " + errorMessage);
            } else {
            	//check for soft errors
                XmlElement nodSoftError = XmlUtility.GetElement(providerResponse, "SoftError", false);
                if (nodSoftError != null) {
                    //soft errors/warnings in rate request
                    softError = XmlUtility.GetElementValue(nodSoftError, "Type");
                    softError = softError + " : " + XmlUtility.GetElementValue(nodSoftError, "Code");
                    softError = softError + " : " + XmlUtility.GetElementValue(nodSoftError, "Message");
                }
            	
                //look for services
                XmlNodeList nlsEntries = providerResponse.DocumentElement.GetElementsByTagName("Entry");
                XmlElement nodEntry = null;
                bool hasGroundHomeDelivery = false;
                bool hasFedExGround = false;
                String tempServiceCode;
                ProviderShipRateQuote quote;

                foreach (XmlNode nodeItem in nlsEntries)
                {
                    nodEntry = (XmlElement)nodeItem;                    
                    tempServiceCode = XmlUtility.GetElementValue(nodEntry, "Service", "");
                    if (string.IsNullOrEmpty(tempServiceCode)) continue;
                    if (tempServiceCode.Equals("GROUNDHOMEDELIVERY")) hasGroundHomeDelivery = true;
                    if (tempServiceCode.Equals("FEDEXGROUND")) hasFedExGround = true;

                    quote = new ProviderShipRateQuote();
                    quote.ServiceCode = tempServiceCode;
                    quote.ServiceName = GetServiceName(tempServiceCode);
                    quote.Rate = package.Multiplier * AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodEntry, "EstimatedCharges/DiscountedCharges/NetCharge", "0"), 0);
                    quote.PackageCount = 1;
                    if (softError.Length > 0)
                    {
                        quote.AddWarning(softError);
                    }
                    providerQuotes.Add(quote);                    
                }

                //bug 6786
                //if a rate for GROUNDHOMEDELIVERY is returned, and it's a US residence, ignore FEDEXGROUND
                //if a rate for FEDEXGROUND is returned, and it's a US business, ignore GROUNDHOMEDELIVERY
                if (destination.CountryCode.Equals("US", StringComparison.InvariantCultureIgnoreCase)
                     && hasGroundHomeDelivery && hasFedExGround)
                {
                    for (int i = providerQuotes.Count - 1; i >= 0; i--)
                    {
                        quote = providerQuotes[i];
                        if ((destination.Residence && quote.ServiceCode.Equals("FEDEXGROUND"))
                            || (!destination.Residence && quote.ServiceCode.Equals("GROUNDHOMEDELIVERY")))
                        {
                            providerQuotes.RemoveAt(i);
                            break;
                        }
                    }
                }

                nodEntry = null;
                nlsEntries = null;
            }
            return providerQuotes;
        }

        private string GetServiceName(String serviceCode)
        {
            string serviceName = string.Empty;
            if (_services.ContainsKey(serviceCode)) serviceName = _services[serviceCode];
            if (string.IsNullOrEmpty(serviceName)) serviceName = serviceCode + " : Unknown";
            return serviceName;
        }

        private XmlDocument BuildProviderRequest(Warehouse warehouse, Address destination, Package package)
        {
            ProviderUnits pUnits = GetProviderUnits(warehouse.Country);

            //CREATE PROVIDER REQUEST DOCUMENT
            XmlDocument providerRequest = new XmlDocument();
            providerRequest.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><FDXRateAvailableServicesRequest xmlns:api=\"http://www.fedex.com/fsmapi\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"FDXRateAvailableServicesRequest.xsd\"></FDXRateAvailableServicesRequest>");

            XmlElement nodRequestHeader = (XmlElement)providerRequest.DocumentElement.AppendChild(providerRequest.CreateElement("RequestHeader"));
            XmlUtility.SetElementValue(nodRequestHeader, "AccountNumber", this.AccountNumber);
            XmlUtility.SetElementValue(nodRequestHeader, "MeterNumber", this.MeterNumber);

            //update the shipment settings
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "ShipDate", DateTime.Now.ToString("yyyy'-'MM'-'dd"));
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "DropoffType", DropOffType.ToString());
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "Packaging", PackagingType.ToString());
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "WeightUnits", GetWeightUnitCode(pUnits.WeightUnit));
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "Weight", String.Format("{0:F1}",package.Weight));

            XmlElement nodOriginAddress = (XmlElement)providerRequest.DocumentElement.AppendChild(providerRequest.CreateElement("OriginAddress"));
            XmlUtility.SetElementValue(nodOriginAddress, "StateOrProvinceCode", StringHelper.Truncate(warehouse.Province,2));
            XmlUtility.SetElementValue(nodOriginAddress, "PostalCode", warehouse.PostalCode);
            XmlUtility.SetElementValue(nodOriginAddress, "CountryCode", warehouse.CountryCode);
            
            //set the destination
            XmlElement nodDestinationAddress = (XmlElement)providerRequest.DocumentElement.AppendChild(providerRequest.CreateElement("DestinationAddress"));
            XmlUtility.SetElementValue(nodDestinationAddress, "StateOrProvinceCode", StringHelper.Truncate(destination.Province,2) );
            XmlUtility.SetElementValue(nodDestinationAddress, "PostalCode", destination.PostalCode);
            XmlUtility.SetElementValue(nodDestinationAddress, "CountryCode", destination.CountryCode);
            
            //set payment type
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "Payment/PayorType", "SENDER");

            if (PackagingType == FDXPackagingType.YOURPACKAGING)
            {
                //set the package dimensions
                LSDecimal decPkgLength = package.Length;
                LSDecimal decPkgWidth = package.Width;
                LSDecimal decPkgHeight = package.Height;

                if (decPkgLength > 0 && decPkgWidth > 0 && decPkgHeight > 0)
                {
                    //REORDER DIMENSIONS
                    LSDecimal[] arrDims = new LSDecimal[3];
                    arrDims[0] = decPkgLength;
                    arrDims[1] = decPkgWidth;
                    arrDims[2] = decPkgHeight;
                    Array.Sort(arrDims);
                    //length is longest dimension;
                    decPkgLength = arrDims[2];
                    //width is middle dimension
                    decPkgWidth = arrDims[1];
                    //height is shortest dimension
                    decPkgHeight = arrDims[0];

                    //trial and error, these were the smallest supported dimensions I could find
                    if (decPkgHeight < LocaleHelper.ConvertMeasurement(MeasurementUnit.Inches, 2, pUnits.MeasurementUnit))
                    {
                        decPkgHeight = LocaleHelper.ConvertMeasurement(MeasurementUnit.Inches, 2, pUnits.MeasurementUnit);
                    }
                    if (decPkgLength < LocaleHelper.ConvertMeasurement(MeasurementUnit.Inches, 7, pUnits.MeasurementUnit))
                    {
                        decPkgLength = LocaleHelper.ConvertMeasurement(MeasurementUnit.Inches, 7, pUnits.MeasurementUnit);
                    }
                    if (decPkgWidth < LocaleHelper.ConvertMeasurement(MeasurementUnit.Inches, 4, pUnits.MeasurementUnit))
                    {
                        decPkgWidth = LocaleHelper.ConvertMeasurement(MeasurementUnit.Inches, 4, pUnits.MeasurementUnit);
                    }

                    XmlElement nodDimensions = (XmlElement)providerRequest.DocumentElement.AppendChild(providerRequest.CreateElement("Dimensions"));
                    XmlUtility.SetElementValue(nodDimensions, "Length", "" + ((int)decPkgLength));
                    XmlUtility.SetElementValue(nodDimensions, "Width", "" + ((int)decPkgWidth));
                    XmlUtility.SetElementValue(nodDimensions, "Height", "" + ((int)decPkgHeight));
                    XmlUtility.SetElementValue(nodDimensions, "Units", GetMeasurementUnitCode(pUnits.MeasurementUnit));
                }
            }

            //CHECK DECLARED VALUE OPTION
            if (this.IncludeDeclaredValue)
            {
                //SET DECLARED VALUE OF SHIPMENT, USE BASE CURRENCY OF STORE
                XmlElement nodDeclaredValue = (XmlElement)providerRequest.DocumentElement.AppendChild(providerRequest.CreateElement("DeclaredValue"));
                XmlUtility.SetElementValue(nodDeclaredValue, "Value", String.Format("{0:F2}", package.RetailValue));
                string currencyCode = Token.Instance.Store.BaseCurrency.ISOCode;
                if (currencyCode.Length != 3) currencyCode = "USD";
                XmlUtility.SetElementValue(nodDeclaredValue, "CurrencyCode", currencyCode);
            }
            //SET RESIDENTIAL FLAG
            if (destination.Residence) XmlUtility.SetElementValue(providerRequest.DocumentElement, "SpecialServices/ResidentialDelivery", "1");
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "PackageCount", "1");

            //RETURN THE REQUEST
            return providerRequest;
        }

        private PackageList PreparePackages(Warehouse origin, Address destination, BasketItemCollection contents)
        {
            PackageList plist = PackageManager.GetPackageList(contents);
            if (plist == null || plist.Count == 0) return null;

            ProviderUnits pUnits = GetProviderUnits(origin.Country);
            //GET UNITS USED BY STORE
            Store store = Token.Instance.Store;
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

            WeightUnit sourceUnit = (Token.Instance.Store.WeightUnit == WeightUnit.Grams || Token.Instance.Store.WeightUnit == WeightUnit.Kilograms) ? WeightUnit.Kilograms : WeightUnit.Pounds;
            LSDecimal maxWeight = LocaleHelper.ConvertWeight(sourceUnit, MaxPackageWeight, pUnits.WeightUnit);
            LSDecimal minWeight = LocaleHelper.ConvertWeight(sourceUnit, MinPackageWeight, pUnits.WeightUnit);
            if (EnablePackageBreakup && maxWeight > 0)
            {
                //compose packages (splits items larger than the maximum carrier weight)                
                plist.Compose(maxWeight, minWeight);
            }
            else
            {
                plist.EnsureMinimumWeight(minWeight);
            }

            //convert weight and dimensions to whole numbers
            plist.ConvertDimsToWholeNumbers();
            plist.RoundWeight(1);

            return plist;
        }

        private ProviderUnits GetProviderUnits(Country originCountry)
        {
            ProviderUnits pUnits = new ProviderUnits();
            //DETERMINE FEDEX REQUIRED UNITS
            string[] imperialUnits = { "US", "PR", "DO", "BS" };
            if (Array.IndexOf(imperialUnits, originCountry.CountryCode) > -1)
            {
                pUnits.MeasurementUnit = MeasurementUnit.Inches;
                pUnits.WeightUnit = WeightUnit.Pounds;
            }
            else
            {
                pUnits.MeasurementUnit = MeasurementUnit.Centimeters;
                pUnits.WeightUnit = WeightUnit.Kilograms;
            }
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
                    return "GMS";
                case WeightUnit.Kilograms:
                    return "KGS";
                case WeightUnit.Ounces:
                    return "OZS";
                case WeightUnit.Pounds:
                    return "LBS";
                default:
                    return "";
            }
        }

        #endregion Implementation_Support_methods


        #region Configuration_Support_Methods
 
        public bool IsActive
        {
            get { return (this.IsRegistered && !string.IsNullOrEmpty(this.AccountNumber)); }
        }

        public bool IsRegistered
        {
            get { return (!(string.IsNullOrEmpty(this.AccountNumber) || string.IsNullOrEmpty(this.MeterNumber))); }
        }

        public FedExRegistrationResult Register(FedExRegistrationInformation registrationInformation)
        {
            XmlDocument registrationRequest = new XmlDocument();
            registrationRequest.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><FDXSubscriptionRequest xmlns:api=\"http://www.fedex.com/fsmapi\" xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"FDXSubscriptionRequest.xsd\"></FDXSubscriptionRequest>");

            XmlElement nodRequestHeader = XmlUtility.GetElement(registrationRequest.DocumentElement, "RequestHeader", true);
            XmlUtility.SetElementValue(nodRequestHeader, "AccountNumber", registrationInformation.ShipperAccount);

            XmlElement nodContact = XmlUtility.GetElement(registrationRequest.DocumentElement, "Contact", true);
            XmlUtility.SetElementValue(nodContact, "PersonName", "" + registrationInformation.ContactName);
            XmlUtility.SetElementValue(nodContact, "CompanyName", "" + registrationInformation.Company);
            XmlUtility.SetElementValue(nodContact, "PhoneNumber", "" + registrationInformation.ContactPhone);            
            XmlUtility.SetElementValue(nodContact, "E-MailAddress", "" + registrationInformation.ContactEmail);

            XmlElement nodAddress = XmlUtility.GetElement(registrationRequest.DocumentElement, "Address", true);
            XmlUtility.SetElementValue(nodAddress, "Line1", "" + registrationInformation.Address1);
            if (!string.IsNullOrEmpty(registrationInformation.Address2))
            {
                XmlUtility.SetElementValue(nodAddress, "Line2", "" + registrationInformation.Address2);
            }
            XmlUtility.SetElementValue(nodAddress, "City", "" + registrationInformation.City);
            XmlUtility.SetElementValue(nodAddress, "StateOrProvinceCode", "" + registrationInformation.StateProvinceCode);
            XmlUtility.SetElementValue(nodAddress, "PostalCode", "" + registrationInformation.PostalCode);
            XmlUtility.SetElementValue(nodAddress, "CountryCode", "" + registrationInformation.CountryCode);
            
            if (this.UseDebugMode)
            {
                this.RecordCommunication("FedEx", CommunicationDirection.Send, registrationRequest.OuterXml);
            }

            XmlDocument registrationResponse = SendRequestToProvider(registrationRequest, GetEffectiveUrl(), "iso-8859-1");

            if (this.UseDebugMode)
            {
                this.RecordCommunication("FedEx", CommunicationDirection.Receive, registrationResponse.OuterXml);
            }

            //check the result
            FedExRegistrationResult regResult = new FedExRegistrationResult();

            if (registrationResponse.DocumentElement.Name.Equals("Error")) {
                //error in subscription request               
                regResult.errorCode = XmlUtility.GetElementValue(registrationResponse.DocumentElement, "Code");
                regResult.errorMessage = XmlUtility.GetElementValue(registrationResponse.DocumentElement, "Message");
                regResult.successful = false;
            } 
            else if (XmlUtility.GetElementValue(registrationResponse.DocumentElement, "Error/Code", "").Length > 0) 
            {
                //error in subscription request
                regResult.errorCode = XmlUtility.GetElementValue(registrationResponse.DocumentElement, "Error/Code");
                regResult.errorMessage = XmlUtility.GetElementValue(registrationResponse.DocumentElement, "Error/Message");
                regResult.successful = false;
            }
            else if (XmlUtility.GetElementValue(registrationResponse.DocumentElement, "SoftError/Code", "").Length > 0)
            {
                //error in subscription request
                regResult.errorCode = XmlUtility.GetElementValue(registrationResponse.DocumentElement, "SoftError/Code");
                regResult.errorMessage = XmlUtility.GetElementValue(registrationResponse.DocumentElement, "SoftError/Message");
                regResult.successful = false;
            } 
            else 
            {
                this.AccountNumber = registrationInformation.ShipperAccount;                
                this.MeterNumber = XmlUtility.GetElementValue(registrationResponse.DocumentElement, "MeterNumber");
                regResult.successful = true;                
            }

            return regResult;
        }

        #endregion

        private string GetEffectiveUrl(){
            if (this.UseTestMode)
            {
                return TestModeUrl;
            }
            else
            {
                return LiveModeUrl;
            }
        }       

        public class FedExRegistrationResult
        {
            public bool successful;
            public string errorCode;
            public string errorMessage;

            public FedExRegistrationResult()
            {
                successful = false;
                errorCode = "";
                errorMessage = "";
            }
        }

        public class FedExRegistrationInformation
        {
            public string ContactName;
            public string Company;
            public string Address1;
            public string Address2;
            public string Address3;
            public string City;
            public string StateProvinceCode;
            public string PostalCode;
            public string CountryCode;
            public string ContactPhone;
            public string ContactEmail;
            public string ShipperAccount;
        }

        struct ProviderUnits {
            public MeasurementUnit MeasurementUnit;
            public WeightUnit WeightUnit;
        }

        public enum FDXDropOffType
        {
            REGULARPICKUP,
            REQUESTCOURIER,
            DROPBOX,
            BUSINESSSERVICECENTER,
            STATION
        }

        public enum FDXPackagingType
        {
            FEDEXENVELOPE,
            FEDEXPAK,
            FEDEXBOX,
            FEDEXTUBE,
            FEDEX10KGBOX,
            FEDEX25KGBOX,
            YOURPACKAGING
        }

        public enum FDXHomeDeliveryType
        {
            DATECERTAIN, 
            EVENING,
            APPOINTMENT
        }
    }
}
