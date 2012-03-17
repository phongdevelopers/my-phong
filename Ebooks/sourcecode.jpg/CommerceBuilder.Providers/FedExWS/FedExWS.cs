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
using CommerceBuilder.Shipping.Providers.FedExWS.RateServiceWebReference;


namespace CommerceBuilder.Shipping.Providers.FedExWS
{
    public class FedExWS : ShippingProviderBase
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

        private DropoffType _DropoffType = DropoffType.REGULAR_PICKUP;
        public DropoffType DropoffType
        {
            get
            {
                return _DropoffType;
            }
            set
            {
                _DropoffType = value;
            }
        }

        private PackagingType _PackagingType = PackagingType.YOUR_PACKAGING;
        public PackagingType PackagingType
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

        private string _PayorAccountNumber = string.Empty;
        public string PayorAccountNumber
        {
            get
            {
                return _PayorAccountNumber;
            }
            set
            {
                _PayorAccountNumber = value;
            }
        }

        private string _Key = string.Empty;
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        private string _Password = string.Empty;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private LSDecimal _MinPackageWeight = (decimal)0.1;
        public LSDecimal MinPackageWeight
        {
            get { return _MinPackageWeight; }
            set { _MinPackageWeight = value; }
        }

        //max weight for FedEx is 150lb (bug 6786)
        private LSDecimal _MaxPackageWeight = 150;
        public LSDecimal MaxPackageWeight
        {
            get { return _MaxPackageWeight; }
            set { _MaxPackageWeight = value; }
        }

/*      
  private PaymentType _PaymentType = PaymentType.SENDER;
        public PaymentType PaymentType
        {
            get
            {
                return _PaymentType;
            }
            set
            {
                _PaymentType = value;
            }
        }
*/
        
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
            get { return "FedEx Web Services Module"; }
        }

        public override string Version
        {
            get { return "Rate v5. " + new VersionId().ToString(); }
        }

        private static Dictionary<string, string> _services;
        static FedExWS()
        {
            _services = new Dictionary<string, string>();
            _services.Add(ServiceType.EUROPE_FIRST_INTERNATIONAL_PRIORITY.ToString(),"FedEx Europe First International Priority");
            _services.Add(ServiceType.FEDEX_1_DAY_FREIGHT.ToString(), "FedEx 1 Day Freight");
            _services.Add(ServiceType.FEDEX_2_DAY.ToString(), "FedEx 2 Day");
            _services.Add(ServiceType.FEDEX_2_DAY_FREIGHT.ToString(), "FedEx 2 Day Freight");
            _services.Add(ServiceType.FEDEX_3_DAY_FREIGHT.ToString(), "FedEx 3 Day Freight");
            _services.Add(ServiceType.FEDEX_EXPRESS_SAVER.ToString(), "FedEx Express Saver");
            _services.Add(ServiceType.FEDEX_GROUND.ToString(), "FedEx Ground");
            _services.Add(ServiceType.FIRST_OVERNIGHT.ToString(), "FedEx Overnight");
            _services.Add(ServiceType.GROUND_HOME_DELIVERY.ToString(), "FedEx Ground Home Delivery");
            _services.Add(ServiceType.INTERNATIONAL_ECONOMY.ToString(), "FedEx International Economy");
            _services.Add(ServiceType.INTERNATIONAL_ECONOMY_FREIGHT.ToString(), "FedEx International Economy Freight");
            _services.Add(ServiceType.INTERNATIONAL_FIRST.ToString(), "FedEx International First");
            _services.Add(ServiceType.INTERNATIONAL_PRIORITY.ToString(), "FedEx International Priority");
            _services.Add(ServiceType.INTERNATIONAL_PRIORITY_FREIGHT.ToString(), "FedEx International Priority Freight");
            _services.Add(ServiceType.PRIORITY_OVERNIGHT.ToString(), "FedEx Priority Overnight");
            _services.Add(ServiceType.STANDARD_OVERNIGHT.ToString(), "FedEx Standard Overnight");
        }

        #region Interface_Implementation_methods

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Shipping.Providers.FedExWS.Logo.gif");
            return string.Empty;
        }

        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "FedExWS/Default.aspx";
        }

        public override string Description
        {
            get { return "This is an untested FedEx integration using web services and is not ready for general public release."; }
        }

        public override void Initialize(int ShipGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(ShipGatewayId, ConfigurationData);
            //INITIALIZE MY FIELDS
            if (ConfigurationData.ContainsKey("AccountNumber")) AccountNumber = ConfigurationData["AccountNumber"];
            if (ConfigurationData.ContainsKey("MeterNumber")) MeterNumber = ConfigurationData["MeterNumber"];
            if (ConfigurationData.ContainsKey("Key")) Key = ConfigurationData["Key"];
            if (ConfigurationData.ContainsKey("Password")) Password = ConfigurationData["Password"];
            if (ConfigurationData.ContainsKey("PayorAccountNumber")) PayorAccountNumber = ConfigurationData["PayorAccountNumber"];
            if (ConfigurationData.ContainsKey("EnablePackageBreakup")) EnablePackageBreakup = AlwaysConvert.ToBool(ConfigurationData["EnablePackageBreakup"], true);
            if (ConfigurationData.ContainsKey("MinPackageWeight")) MinPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MinPackageWeight"], (decimal)MinPackageWeight);
            if (ConfigurationData.ContainsKey("MaxPackageWeight")) MaxPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MaxPackageWeight"], (decimal) MaxPackageWeight);
            if (ConfigurationData.ContainsKey("IncludeDeclaredValue")) IncludeDeclaredValue = AlwaysConvert.ToBool(ConfigurationData["IncludeDeclaredValue"], true);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], false);
            if (ConfigurationData.ContainsKey("AccountActive")) AccountActive = AlwaysConvert.ToBool(ConfigurationData["AccountActive"], false);
            if (ConfigurationData.ContainsKey("DropoffType")) DropoffType = (DropoffType)AlwaysConvert.ToEnum(typeof(DropoffType), ConfigurationData["DropoffType"],DropoffType.REGULAR_PICKUP,true);
            if (ConfigurationData.ContainsKey("PackagingType")) PackagingType = (PackagingType)AlwaysConvert.ToEnum(typeof(PackagingType), ConfigurationData["PackagingType"], PackagingType.YOUR_PACKAGING, true);
            if (ConfigurationData.ContainsKey("TrackingUrl")) TrackingUrl = ConfigurationData["TrackingUrl"];
        }

        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData.Add("AccountNumber", this.AccountNumber);
            configData.Add("MeterNumber", this.MeterNumber);
            configData.Add("Key", this.Key);
            configData.Add("Password", this.Password);
            configData.Add("PayorAccountNumber", this.PayorAccountNumber);
            configData.Add("EnablePackageBreakup", this.EnablePackageBreakup.ToString());
            configData.Add("MinPackageWeight", this.MinPackageWeight.ToString());
            configData.Add("MaxPackageWeight", this.MaxPackageWeight.ToString());
            configData.Add("IncludeDeclaredValue", this.IncludeDeclaredValue.ToString());
            configData.Add("UseTestMode", this.UseTestMode.ToString());
            configData.Add("AccountActive", this.AccountActive.ToString());
            configData.Add("DropoffType", this.DropoffType.ToString());
            configData.Add("PackagingType", this.PackagingType.ToString());
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
                return allQuotes[serviceCode];
            else
                return null;
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

        private Dictionary<string, ProviderShipRateQuote> GetAllServiceQuotes(Warehouse origin, CommerceBuilder.Users.Address destination, BasketItemCollection contents)
        {            
            string cacheKey = StringHelper.CalculateMD5Hash(Misc.GetClassId(this.GetType()) + "_" + origin.WarehouseId.ToString() + "_" + destination.AddressId.ToString() + "_" + contents.GenerateContentHash());
            HttpContext context = HttpContext.Current;
            if ((context != null) && (context.Items.Contains(cacheKey)))
                return (Dictionary<string, ProviderShipRateQuote>)context.Items[cacheKey];

            //VERIFY WE HAVE A DESTINATION COUNTRY
            if (string.IsNullOrEmpty(destination.CountryCode)) return null;

            PackageList plist = PreparePackages(origin, destination, contents);
            if (plist == null || plist.Count == 0) return null;

            Dictionary<string, ProviderShipRateQuote> allQuotes;
            allQuotes = GetProviderQuotes(origin, destination, plist);

            if (context != null) context.Items.Add(cacheKey, allQuotes);
            
            return allQuotes;
        }

        private string GetServiceName(String serviceCode)
        {
            string serviceName = string.Empty;
            if (_services.ContainsKey(serviceCode)) serviceName = _services[serviceCode];
            if (string.IsNullOrEmpty(serviceName)) serviceName = serviceCode + " : Unknown";
            return serviceName;
        }

        private PackageList PreparePackages(Warehouse origin, CommerceBuilder.Users.Address destination, BasketItemCollection contents)
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

            //convert weight and dimensions to whole numbers
            plist.ConvertDimsToWholeNumbers();
            plist.RoundWeight(1);

            return plist;
        }


        #endregion Implementation_Support_methods



        struct ProviderUnits {
            public MeasurementUnit MeasurementUnit;
            public WeightUnit WeightUnit;
        }

        private Dictionary<string, ProviderShipRateQuote> GetProviderQuotes(Warehouse origin, CommerceBuilder.Users.Address destination, PackageList packageList)
        {            
            RateRequest request = CreateRateRequest(origin, destination, packageList);
            RateService rateService = new RateService();
            RateReply reply;
            this.RecordCommunication("FedExWS", CommunicationDirection.Send, new UTF8Encoding().GetString(XmlUtility.Serialize(request)));
            // This is the call to the web service passing in a RateRequest and returning a RateReply
            try
            {
                reply = rateService.getRates(request); // Service call
            }
            catch (System.Web.Services.Protocols.SoapException se)
            {
                Logger.Error("Soap Exception", se);
                Logger.Debug(se.Detail.InnerXml);
                return new Dictionary<string, ProviderShipRateQuote>();
            }
            this.RecordCommunication("FedExWS", CommunicationDirection.Receive, new UTF8Encoding().GetString(XmlUtility.Serialize(reply)));

            if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) // check if the call was successful
            {
                //ShowRateReply(reply);
                return ParseRates(reply);
            }
            else
            {
                //Console.WriteLine(reply.Notifications[0].Message);
                return new Dictionary<string, ProviderShipRateQuote>();
            }
        }

        private Dictionary<string, ProviderShipRateQuote> ParseRates(RateReply reply)
        {
            Dictionary<string, ProviderShipRateQuote> quotes = new Dictionary<string, ProviderShipRateQuote>();
            for (int i = 0; i < reply.RateReplyDetails.Length; i++)
            {
                RateReplyDetail rdetail = reply.RateReplyDetails[i];
                RatedShipmentDetail rsdetail = GetRatedShipmentDetail(rdetail.RatedShipmentDetails);
                if (rsdetail != null)
                {
                    ProviderShipRateQuote psrq = new ProviderShipRateQuote();                    
                    psrq.ServiceCode = rdetail.ServiceType.ToString();
                    //psrq.Name = GetServiceName(psrq.ServiceCode);
                    psrq.ServiceName = GetServiceName(psrq.ServiceCode);                    
                    psrq.Rate = rsdetail.ShipmentRateDetail.TotalNetCharge.Amount;
                    quotes.Add(psrq.ServiceCode, psrq);
                }
            }
            return quotes;
        }

        private RatedShipmentDetail GetRatedShipmentDetail(RatedShipmentDetail[] rsdetails)
        {
            //For this integration of FedEx we are only concerned with Payour Account shipping rates
            foreach(RatedShipmentDetail rsd in rsdetails) 
            {
                if (rsd.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT)
                {
                    return rsd;
                }
            }
            return null;
        }

        private RateRequest CreateRateRequest(Warehouse origin, CommerceBuilder.Users.Address destination, PackageList packageList)
        {
            // Build the RateRequest
            RateRequest request = new RateRequest();
            //
            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = this.Key;
            request.WebAuthenticationDetail.UserCredential.Password = this.Password;            
            //
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = this.AccountNumber; 
            request.ClientDetail.MeterNumber = this.MeterNumber; 
            //
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "*** Rate v5 Request ***";
            //
            request.Version = new VersionId(); // WSDL version information, value is automatically set from wsdl            
            // 
            // Origin information
            request.CarrierCodes = new CarrierCodeType[2];
            request.CarrierCodes[0] = CarrierCodeType.FDXE;
            request.CarrierCodes[1] = CarrierCodeType.FDXG;
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new CommerceBuilder.Shipping.Providers.FedExWS.RateServiceWebReference.Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { origin.Address1 };
            request.RequestedShipment.Shipper.Address.City = origin.City;
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = origin.Province;
            request.RequestedShipment.Shipper.Address.PostalCode = origin.PostalCode;
            request.RequestedShipment.Shipper.Address.CountryCode = origin.CountryCode;            
            //
            // Destination Information
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new CommerceBuilder.Shipping.Providers.FedExWS.RateServiceWebReference.Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { destination.Address1 };
            request.RequestedShipment.Recipient.Address.City = destination.City;
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = destination.Province;
            request.RequestedShipment.Recipient.Address.PostalCode = destination.PostalCode;
            request.RequestedShipment.Recipient.Address.CountryCode = destination.CountryCode;
            if (destination.Residence)
            {
                request.RequestedShipment.Recipient.Address.Residential = destination.Residence;
                request.RequestedShipment.Recipient.Address.ResidentialSpecified = true;
            }
            //
            // Payment Information
            request.RequestedShipment.ShippingChargesPayment = new Payment();
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER; 
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            if (!string.IsNullOrEmpty(this.PayorAccountNumber))
            {
                request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
                request.RequestedShipment.ShippingChargesPayment.Payor.AccountNumber = this.PayorAccountNumber; 
            }

            //
            request.RequestedShipment.DropoffType = this.DropoffType;                         
            request.RequestedShipment.PackagingType = this.PackagingType; 
            request.RequestedShipment.PackagingTypeSpecified = true;
            request.RequestedShipment.ServiceTypeSpecified = false;
            
            //
            //request.RequestedShipment.TotalInsuredValue = new Money();
            //request.RequestedShipment.TotalInsuredValue.Amount = 100;
            //request.RequestedShipment.TotalInsuredValue.Currency = "USD";
            //request.RequestedShipment.ShipTimestamp = DateTime.Now; // Shipping date and time
            //request.RequestedShipment.ShipTimestampSpecified = true;
            request.RequestedShipment.RateRequestTypes = new RateRequestType[1];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
            //request.RequestedShipment.RateRequestTypes[0] = RateRequestType.LIST; //????
            
            //
            // The RateRequest can be populated with one of the following:
            //
            // RequestedPackageSummary - Details of multi piece shipment rate request - Use this to rate a total piece total weight shipment.
            // Array of RequestedPackage - Details of single piece shipment rate request or multiple packages with differing parameters.
            //
            
            //bool bPassRateRequestPackageSummary = false;
            //
            //if (bPassRateRequestPackageSummary)
            //{
                // -----------------------------------------
                // Passing multi piece shipment rate request
                // -----------------------------------------
           //     request.RequestedShipment.TotalWeight = new Weight();
           //     request.RequestedShipment.TotalWeight.Value = 20.0M;
           //     request.RequestedShipment.TotalWeight.Units = WeightUnits.LB;
                //
           //     request.RequestedShipment.PackageCount = "2";
           //     request.RequestedShipment.PackageDetail = RequestedPackageDetailType.PACKAGE_SUMMARY;
                //
           //     request.RequestedShipment.RequestedPackageSummary = new RequestedPackageSummary();
           //     request.RequestedShipment.RequestedPackageSummary.Dimensions = new Dimensions(); // package dimensions, applies to each package 
           //     request.RequestedShipment.RequestedPackageSummary.Dimensions.Length = "10";
           //     request.RequestedShipment.RequestedPackageSummary.Dimensions.Width = "10";
           //     request.RequestedShipment.RequestedPackageSummary.Dimensions.Height = "3";
           //     request.RequestedShipment.RequestedPackageSummary.Dimensions.Units = LinearUnits.IN;
           // }
           // else
           // {
                // ------------------------------------------
                // Passing individual pieces rate request
                // ------------------------------------------
                request.RequestedShipment.PackageCount = packageList.Count.ToString();
                request.RequestedShipment.PackageDetail = RequestedPackageDetailType.INDIVIDUAL_PACKAGES;
                //
                request.RequestedShipment.RequestedPackages = new RequestedPackage[packageList.Count];

                ProviderUnits pUnits = GetProviderUnits(origin.Country);

                for(int i=0; i< packageList.Count; i++)
                {
                    Package pkg = packageList[i];
                    request.RequestedShipment.RequestedPackages[i] = new RequestedPackage();
                    request.RequestedShipment.RequestedPackages[i].SequenceNumber = (i+1).ToString(); 
                    //
                    request.RequestedShipment.RequestedPackages[i].Weight = new Weight(); // package weight
                    request.RequestedShipment.RequestedPackages[i].Weight.Units = GetWeightUnits(pUnits.WeightUnit);
                    request.RequestedShipment.RequestedPackages[i].Weight.Value = (decimal)pkg.Weight;
                    //
                    request.RequestedShipment.RequestedPackages[i].Dimensions = new Dimensions(); // package dimensions
                    request.RequestedShipment.RequestedPackages[i].Dimensions.Length = pkg.Length.ToString();
                    request.RequestedShipment.RequestedPackages[i].Dimensions.Width = pkg.Width.ToString();
                    request.RequestedShipment.RequestedPackages[i].Dimensions.Height = pkg.Height.ToString();
                    request.RequestedShipment.RequestedPackages[i].Dimensions.Units = GetLinearUnits(pUnits.MeasurementUnit);
                    //
                    if (IncludeDeclaredValue)
                    {
                        request.RequestedShipment.RequestedPackages[i].InsuredValue = new Money(); // insured value
                        request.RequestedShipment.RequestedPackages[i].InsuredValue.Amount = (decimal)pkg.RetailValue;
                        request.RequestedShipment.RequestedPackages[i].InsuredValue.Currency = Token.Instance.Store.BaseCurrency.ISOCode;
                    }
                }
                
            //}

            return request;
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
        
        private WeightUnits GetWeightUnits(WeightUnit wu)
        {
            switch (wu)
            {
                case WeightUnit.Kilograms:
                    return WeightUnits.KG;
                case WeightUnit.Pounds:
                    return WeightUnits.LB;
                default:
                    return WeightUnits.LB;
            }
        }

        private LinearUnits GetLinearUnits(MeasurementUnit mu)
        {
            switch (mu)
            {
                case MeasurementUnit.Inches:
                    return LinearUnits.IN;
                case MeasurementUnit.Centimeters:
                    return LinearUnits.CM;
                default:
                    return LinearUnits.IN;
            }
        }

    }
}
