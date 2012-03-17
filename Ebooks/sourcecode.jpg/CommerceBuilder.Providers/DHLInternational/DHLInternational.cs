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

namespace CommerceBuilder.Shipping.Providers.DHLInternational
{
    public class DHLInternational : ShippingProviderBase
    {
        private string _UserID = string.Empty;
        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private string _Password = string.Empty;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private string _ShippingKey = string.Empty;
        public string ShippingKey
        {
            get { return _ShippingKey; }
            set { _ShippingKey = value; }
        }

        private string _AccountNumber = string.Empty;
        public string AccountNumber
        {
            get { return _AccountNumber; }
            set { _AccountNumber = value; }
        }

        private int _DaysToShip = 0;
        public int DaysToShip
        {
            get { return _DaysToShip; }
            set { _DaysToShip = value; }
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

        //Max package weight for DHL 150lb?
        private LSDecimal _MaxPackageWeight = 150;
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
        
        private bool _DOSFlag = false;
        public bool DOSFlag
        {
            get { return _DOSFlag; }
            set { _DOSFlag = value; }
        }

        private bool _DutiableFlag = false;
        public bool DutiableFlag
        {
            get { return _DutiableFlag; }
            set { _DutiableFlag = value; }
        }

        private LSDecimal _CustomsValueMultiplier = 1;
        public LSDecimal CustomsValueMultiplier
        {
            get { return _CustomsValueMultiplier; }
            set { _CustomsValueMultiplier = value; }
        }

        private bool _CommerceLicensed = false;
        public bool CommerceLicensed
        {
            get { return _CommerceLicensed; }
            set { _CommerceLicensed = value; }
        }

        private FilingTypeFlags _FilingType = FilingTypeFlags.ITN;
        public FilingTypeFlags FilingType
        {
            get { return _FilingType; }
            set { _FilingType = value; }
        }

        private string _FTRExemptionCode = string.Empty;
        public string FTRExemptionCode
        {
            get { return _FTRExemptionCode; }
            set { _FTRExemptionCode = value; }
        }

        private string _ITNNumber = string.Empty;
        public string ITNNumber
        {
            get { return _ITNNumber; }
            set { _ITNNumber = value; }
        }

        private string _EINCode = "0";
        public string EINCode
        {
            get { return _EINCode; }
            set { _EINCode = value; }
        }

        private string _TestModeUrl = "https://eCommerce.airborne.com/ApiLandingTest.asp";
        public string TestModeUrl
        {
            get { return _TestModeUrl; }
            set { _TestModeUrl = value; }
        }

        private string _LiveModeUrl = "https://eCommerce.airborne.com/ApiLanding.asp";
        public string LiveModeUrl
        {
            get { return _LiveModeUrl; }
            set { _LiveModeUrl = value; }
        }

        //http://track.dhl-usa.com/TrackByNbr.asp?ShipmentNumber=123
        private static string _DefaultTrackingUrl = "http://track.dhl-usa.com/TrackByNbr.asp?ShipmentNumber={0}";
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
            get { return "DHL International"; }
        }

        public override string Version
        {
            get { return "ShipIT 2.1 - International Shipping"; }
        }
        
        private static Dictionary<string, string> _services;
        static DHLInternational()
        {
            _services = new Dictionary<string, string>();
            _services.Add("IE","DHL International Express");
        }

        #region Interface_Implementation_methods

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Shipping.Providers.DHLInternational.Logo.jpg");
            return string.Empty;
        }

        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "DHLInternational/Default.aspx";
        }

        public override string Description
        {
            get { return "The integrated DHL International module can generate real-time shipping rates and provides tracking support for international shipments."; }
        }

        public override void Initialize(int ShipGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(ShipGatewayId, ConfigurationData);
            //INITIALIZE MY FIELDS
            if (ConfigurationData.ContainsKey("UserID")) UserID = ConfigurationData["UserID"];
            if (ConfigurationData.ContainsKey("Password")) Password = ConfigurationData["Password"];
            if (ConfigurationData.ContainsKey("EnablePackageBreakup")) EnablePackageBreakup = AlwaysConvert.ToBool(ConfigurationData["EnablePackageBreakup"], true);
            if (ConfigurationData.ContainsKey("AccountNumber")) AccountNumber = ConfigurationData["AccountNumber"];
            if (ConfigurationData.ContainsKey("ShippingKey")) ShippingKey = ConfigurationData["ShippingKey"];
            if (ConfigurationData.ContainsKey("DaysToShip")) DaysToShip = AlwaysConvert.ToInt(ConfigurationData["DaysToShip"], 0) ;
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], false);
            if (ConfigurationData.ContainsKey("AccountActive")) AccountActive = AlwaysConvert.ToBool(ConfigurationData["AccountActive"], false);
            if (ConfigurationData.ContainsKey("TestModeUrl")) TestModeUrl = ConfigurationData["TestModeUrl"];
            if (ConfigurationData.ContainsKey("LiveModeUrl")) LiveModeUrl = ConfigurationData["LiveModeUrl"];
            if (ConfigurationData.ContainsKey("TrackingUrl")) TrackingUrl = ConfigurationData["TrackingUrl"];

            if (ConfigurationData.ContainsKey("DOSFlag")) DOSFlag = AlwaysConvert.ToBool(ConfigurationData["DOSFlag"],false);
            if (ConfigurationData.ContainsKey("DutiableFlag")) DutiableFlag = AlwaysConvert.ToBool(ConfigurationData["DutiableFlag"],false);
            if (ConfigurationData.ContainsKey("CustomsValueMultiplier")) CustomsValueMultiplier = AlwaysConvert.ToDecimal(ConfigurationData["CustomsValueMultiplier"],1);
            if (ConfigurationData.ContainsKey("CommerceLicensed")) CommerceLicensed = AlwaysConvert.ToBool(ConfigurationData["CommerceLicensed"],false);
            if (ConfigurationData.ContainsKey("FilingType")) FilingType = (FilingTypeFlags)AlwaysConvert.ToEnum(typeof(FilingTypeFlags), ConfigurationData["FilingType"], FilingTypeFlags.ITN, true);
            if (ConfigurationData.ContainsKey("FTRExemptionCode")) FTRExemptionCode = ConfigurationData["FTRExemptionCode"];
            if (ConfigurationData.ContainsKey("ITNNumber")) ITNNumber = ConfigurationData["ITNNumber"];
            if (ConfigurationData.ContainsKey("EINCode")) EINCode = ConfigurationData["EINCode"];

            if (ConfigurationData.ContainsKey("MinPackageWeight")) MinPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MinPackageWeight"], (decimal)MinPackageWeight);
            if (ConfigurationData.ContainsKey("MaxPackageWeight")) MaxPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MaxPackageWeight"], (decimal)MaxPackageWeight);
        }

        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData.Add("UserID", this.UserID);
            configData.Add("Password", this.Password);
            configData.Add("EnablePackageBreakup", this.EnablePackageBreakup.ToString());
            configData.Add("AccountNumber", this.AccountNumber);
            configData.Add("ShippingKey", this.ShippingKey);
            configData.Add("DaysToShip", this.DaysToShip.ToString());
            configData.Add("UseTestMode", this.UseTestMode.ToString());
            configData.Add("AccountActive", this.AccountActive.ToString());
            configData.Add("TestModeUrl", this.TestModeUrl);
            configData.Add("LiveModeUrl", this.LiveModeUrl);
            configData.Add("TrackingUrl", this.TrackingUrl);

            configData.Add("DOSFlag", this.DOSFlag.ToString());
            configData.Add("DutiableFlag", this.DutiableFlag.ToString());
            configData.Add("CustomsValueMultiplier", this.CustomsValueMultiplier.ToString());
            configData.Add("CommerceLicensed", this.CommerceLicensed.ToString());
            configData.Add("FilingType", this.FilingType.ToString());
            configData.Add("FTRExemptionCode", this.FTRExemptionCode);
            configData.Add("ITNNumber", this.ITNNumber);
            configData.Add("EINCode", this.EINCode);

            configData.Add("MinPackageWeight", this.MinPackageWeight.ToString());
            configData.Add("MaxPackageWeight", this.MaxPackageWeight.ToString());

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
            Dictionary<string, ProviderShipRateQuote> allQuotes = GetAllSericeQuotes(origin, destination, contents);

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
       
        private Dictionary<string, ProviderShipRateQuote> GetAllSericeQuotes(Warehouse origin, Address destination, BasketItemCollection contents)
        {            
            string cacheKey = StringHelper.CalculateMD5Hash(Utility.Misc.GetClassId(this.GetType()) + "_" + origin.WarehouseId.ToString() + "_" + destination.AddressId.ToString() + "_" + contents.GenerateContentHash());
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                //WebTrace.Write("Checking Cache for " + cacheKey);
                if (context.Items.Contains(cacheKey))
                {
                    return (Dictionary<string, ProviderShipRateQuote>)context.Items[cacheKey];
                }
            }

            //VERIFY WE HAVE A DESTINATION COUNTRY
            if (string.IsNullOrEmpty(destination.CountryCode)) return null;

            PackageList plist = PreparePackages(origin, contents);
            if (plist == null || plist.Count == 0) return null;
            Dictionary<string, ProviderShipRateQuote> allQuotes = new Dictionary<string, ProviderShipRateQuote>();
            List<ProviderShipRateQuote> providerQuotes;
            ProviderShipRateQuote tempQuote;
            
            foreach (Package item in plist)
            {
                bool isHolidayIssue = false;
                providerQuotes = GetProviderQuotes(origin, destination, item,ref isHolidayIssue,false);
                // IF NO QUOTES ARE RETURNED DUE TO HOLIDAY THEN RETRY (DHL DID NOT RETURN QUOTES FOR SHIP DATE ON HOLIDAYS)
                if (providerQuotes.Count == 0 && isHolidayIssue)
                {
                    // TRY AGAIN FOR ONE TIME
                    providerQuotes = GetProviderQuotes(origin, destination, item, ref isHolidayIssue, true);
                }

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
        
       /// <summary>
       /// RETURN QUOTES
       /// </summary>
       /// <param name="origin">warehouse to ship from</param>
       /// <param name="destination">address to ship at address</param>
       /// <param name="package">Package that will be shiped</param>
       /// <param name="isHolidayIssue">a bool parameter by ref, will be returned ture if no rates are returned from provider due to the issue that the shipment date is a holiday, otherwise false</param>
       /// <param name="needHolidayAdjustment">set true if you are re-trying the rates and shipment date needs to be adjusted by one or two days due to the holiday issue.</param>
       /// <returns></returns>
       private List<ProviderShipRateQuote> GetProviderQuotes(Warehouse origin, Address destination, Package package, ref bool isHolidayIssue, bool needHolidayAdjustment)
       {
           XmlDocument providerRequest = BuildProviderRequest(origin, destination, package, needHolidayAdjustment);
           
           if (this.UseDebugMode)
           {
                this.RecordCommunication("DHLInternational", CommunicationDirection.Send, providerRequest.OuterXml);   
           }

           XmlDocument providerResponse;
           if(this.UseTestMode) {
               providerResponse = SendRequestToProvider(providerRequest, TestModeUrl, "iso-8859-1");
           }else {
               providerResponse = SendRequestToProvider(providerRequest, LiveModeUrl, "iso-8859-1");
           }
           
           if (this.UseDebugMode)
           {
               this.RecordCommunication("DHLInternational", CommunicationDirection.Receive, providerResponse.OuterXml);
           }

           List < ProviderShipRateQuote >  quotes = new List<ProviderShipRateQuote>();
           if (providerResponse != null)
            {
               quotes = ParseProviderResponse(providerResponse, destination, ref isHolidayIssue);                 
            }
            return quotes;
        }

        private List<ProviderShipRateQuote> ParseProviderResponse(XmlDocument providerResponse, Address destination, ref bool isHolidayIssue)
        {
			XmlNodeList nlsShipments;
			string serviceCode;			
			LSDecimal curRate;
			
            ProviderShipRateQuote pquote;
            List<ProviderShipRateQuote> pquotes = new List<ProviderShipRateQuote>();

            nlsShipments = providerResponse.DocumentElement.SelectNodes("IntlShipment");
            foreach(XmlNode nodeShipment in nlsShipments) 
            {
			    serviceCode = XmlUtility.GetElementValue(nodeShipment, "TransactionTrace",string.Empty);
				if(string.IsNullOrEmpty(serviceCode)) {
                    serviceCode = XmlUtility.GetElementValue(nodeShipment, "ShipmentDetail/Service/Code", string.Empty);
                }
                if(string.IsNullOrEmpty(serviceCode)) continue;
                
                curRate = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(nodeShipment, "ShipmentDetail/RateEstimate/TotalChargeEstimate", "0"));
				if(curRate == 0) continue;
                pquote = new ProviderShipRateQuote();
                pquote.Rate = curRate;
                pquote.PackageCount = 1;
                pquote.ServiceCode = serviceCode;
                pquote.ServiceName = GetServiceName(serviceCode);
                pquotes.Add(pquote);                
			}

            return pquotes;
        }

        private string GetServiceName(String serviceCode)
        {
            string serviceName = _services[serviceCode];
            if (serviceName == null) serviceName = serviceCode + " : Unknown";
            return serviceName;
        }

        private XmlDocument BuildProviderRequest(Warehouse warehouse, Address destination, Package package, bool needHolidayAdjustment)
        {
            //CREATE PROVIDER REQUEST DOCUMENT
            XmlDocument providerRequest = new XmlDocument();
            providerRequest.LoadXml("<?xml version=\"1.0\"?><eCommerce action=\"Request\" version=\"1.1\"></eCommerce>");

            XmlElement nodRequestor = XmlUtility.GetElement(providerRequest.DocumentElement, "Requestor", true);
            XmlUtility.SetElementValue(nodRequestor, "ID", this.UserID);
            XmlUtility.SetElementValue(nodRequestor, "Password", this.Password);

            //DETERMINE THE SHIP DATE
            DateTime shipDate = DateTime.Now.AddDays(this.DaysToShip);
            // ALSO ENSURE THATE SHIP DATE IS NOT A WEEKEND
            if (shipDate.DayOfWeek == DayOfWeek.Saturday)
            {
                shipDate = shipDate.AddDays(2);
            }
            else if (shipDate.DayOfWeek == DayOfWeek.Sunday)
            {
                shipDate = shipDate.AddDays(1);
            }
            //TAKE CARE OF THE ISSUE THAT DHL DOES NOT RETURN RATES FOR HOLIDAYS
            if (needHolidayAdjustment)
            {
                shipDate = shipDate.AddDays(1);
            }

            foreach (string serviceCode in _services.Keys)
            {
                //initialize shipment node
                XmlElement nodShipment = (XmlElement)providerRequest.DocumentElement.AppendChild(providerRequest.CreateElement("IntlShipment"));
                XmlUtility.SetAttributeValue(nodShipment, "action", "GenerateLabel");
                XmlUtility.SetAttributeValue(nodShipment, "version", "2.0");

                //set shipping credentials
                XmlElement nodTemp = XmlUtility.GetElement(nodShipment, "ShippingCredentials", true);
                XmlUtility.SetElementValue(nodTemp, "ShippingKey", this.ShippingKey);
                XmlUtility.SetElementValue(nodTemp, "AccountNbr", this.AccountNumber);

                //set shipment details
                XmlElement nodShipmentDetail = XmlUtility.GetElement(nodShipment, "ShipmentDetail", true);
                                
                XmlUtility.SetElementValue(nodShipmentDetail, "ShipDate", shipDate.ToString("yyyy-MM-dd"));
                XmlUtility.SetElementValue(nodShipmentDetail, "Service/Code", serviceCode);
                XmlUtility.SetElementValue(nodShipmentDetail, "ShipmentType/Code", "P");

                //set package weight
                // MUST BE A WHOLE NUMBER FOR DHL
                XmlUtility.SetElementValue(nodShipmentDetail, "Weight", String.Format("{0:d}", package.Weight.ToInt32(null)));
                XmlUtility.SetElementValue(nodShipmentDetail, "ContentDesc", String.Format("Shipment From {0}", Token.Instance.Store.Name));
                
                // DIMENSIONS
                if (package.Length > 0 && package.Width > 0 && package.Height > 0)
                {
                    XmlUtility.SetElementValue(nodShipmentDetail, "Dimensions/Length", String.Format("{0:d}", package.Length.ToInt32(null)));
                    XmlUtility.SetElementValue(nodShipmentDetail, "Dimensions/Width", String.Format("{0:d}", package.Width.ToInt32(null)));
                    XmlUtility.SetElementValue(nodShipmentDetail, "Dimensions/Height", String.Format("{0:d}", package.Height.ToInt32(null)));
                }

                XmlUtility.SetElementValue(nodShipment, "ExportCompliance/DOSFlag", DOSFlag ? "Y" : "N"); 
                XmlUtility.SetElementValue(nodShipment, "Dutiable/DutiableFlag", DutiableFlag ? "Y" : "N");
                if (DutiableFlag)
                    XmlUtility.SetElementValue(nodShipment, "Dutiable/CustomsValue", string.Format("{0:F2}", package.RetailValue * CustomsValueMultiplier));
                else
                    XmlUtility.SetElementValue(nodShipment, "Dutiable/CustomsValue", "0");
                

                if (DutiableFlag)
                {
                    XmlUtility.SetElementValue(nodShipment, "Dutiable/CommerceLicensed", CommerceLicensed ? "Y" : "N");
                    XmlUtility.SetElementValue(nodShipment, "Dutiable/Filing/FilingType", FilingType.ToString());

                    switch (FilingType)
                    {
                        case FilingTypeFlags.FTR:
                            XmlUtility.SetElementValue(nodShipment, "Dutiable/Filing/FTSR", FTRExemptionCode);
                            break;
                        case FilingTypeFlags.ITN:
                            XmlUtility.SetElementValue(nodShipment, "Dutiable/Filing/ITN", ITNNumber); 
                            break;
                        case FilingTypeFlags.AES4:
                            XmlUtility.SetElementValue(nodShipment, "Dutiable/Filing/AES4/EIN", EINCode); 
                            break;
                        default: 
                            break;
                    }
                }

                //set origin and destination
                XmlUtility.SetElementValue(nodShipment, "Billing/Party/Code", "S");
                XmlUtility.SetElementValue(nodShipment, "Billing/DutyPaymentType", "S");

                XmlElement nodReceiver = XmlUtility.GetElement(nodShipment, "Receiver", true);
                XmlUtility.SetElementValue(nodReceiver, "Address/Street", destination.Address1);
                XmlUtility.SetElementValue(nodReceiver, "Address/City", destination.City);
                XmlUtility.SetElementValue(nodReceiver, "Address/State", destination.Province);
                XmlUtility.SetElementValue(nodReceiver, "Address/Country", destination.CountryCode);
                XmlUtility.SetElementValue(nodReceiver, "Address/PostalCode", destination.PostalCode);

                //set the transaction trace to simplify the SAT/1030 recognition
                XmlUtility.SetElementValue(nodShipment, "TransactionTrace", serviceCode);
            }
            return providerRequest;
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

            LSDecimal maxWeight = LocaleHelper.ConvertWeight(WeightUnit.Pounds, MaxPackageWeight, pUnits.WeightUnit);
            LSDecimal minWeight = LocaleHelper.ConvertWeight(WeightUnit.Pounds, MinPackageWeight, pUnits.WeightUnit);
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
            plist.ConvertWeightToWholeNumber();

            return plist;
        }

        private ProviderUnits GetProviderUnits(Country originCountry)
        {
            ProviderUnits pUnits = new ProviderUnits();
            //DETERMINE DHL REQUIRED UNITS
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

        //input format is xs:data and xs:time for strDate and strTime respectively
        // YYYY-MM-DD and hh:mm:ss
        private DateTime ParseDhlDate(string dateValue, string timeValue)
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

        #endregion Implementation_Support_methods

        #region Configuration_Support_Methods
        public bool IsActive
        {
            get { return (this.IsRegistered && !string.IsNullOrEmpty(this.UserID)); }
        }

        public bool IsRegistered
        {
            get { return (!(string.IsNullOrEmpty(this.UserID) || string.IsNullOrEmpty(this.Password))); }
        }

        #endregion Configuration_Support_Methods
                        
        struct ProviderUnits
        {
            public MeasurementUnit MeasurementUnit;
            public WeightUnit WeightUnit;
        }

        public enum FilingTypeFlags
        {
            FTR,
            ITN,
            AES4
        }

    }
}
