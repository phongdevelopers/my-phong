using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using System.Globalization;


namespace CommerceBuilder.Shipping.Providers.USPS
{
    public class USPS : ShippingProviderBase
    {
        private string _UserId = string.Empty;
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        private string _Password = string.Empty;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private bool _UserIdActive = false;
        public bool UserIdActive
        {
            get { return _UserIdActive; }
            set { _UserIdActive = value; }
        }

        private bool _UseTestMode = false;
        public bool UseTestMode
        {
            get { return _UseTestMode; }
            set { _UseTestMode = value; }
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

        //Weight in Lbs
        private LSDecimal _MaxPackageWeight = 150;
        public LSDecimal MaxPackageWeight
        {
            get { return _MaxPackageWeight; }
            set { _MaxPackageWeight = value; }
        }

        private string _TestModeUrl = "http://testing.shippingapis.com/ShippingAPI.dll";
        public string TestModeUrl
        {
            get { return _TestModeUrl; }
            set { _TestModeUrl = value; }
        }

        private string _LiveModeUrl = "http://production.shippingapis.com/ShippingAPI.dll";
        public string LiveModeUrl
        {
            get { return _LiveModeUrl; }
            set { _LiveModeUrl = value; }
        }

        //http://trkcnfrm1.smi.usps.com/PTSInternetWeb/InterLabelInquiry.do?origTrackNum=EJ958088694US        
        private static string _DefaultTrackingUrl = "http://trkcnfrm1.smi.usps.com/PTSInternetWeb/InterLabelInquiry.do?origTrackNum={0}";
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
            get { return "U.S. Postal Service"; }
        }

        public override string Version
        {
            get { return "v6.0.0"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Shipping.Providers.USPS.Logo.gif");
            return string.Empty;
        }

        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "USPS/Default.aspx";            
        }

        public override string Description
        {
            get { return "The integrated U.S. Postal Service provider can calculate shipping rates for shipments originating in the United States, to both domestic and international destinations.  It also can provide tracking details."; }
        }

        public override void Initialize(int ShipGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(ShipGatewayId, ConfigurationData);
            //INITIALIZE MY FIELDS
            if (ConfigurationData.ContainsKey("UserId")) UserId = ConfigurationData["UserId"];
            if (ConfigurationData.ContainsKey("Password")) Password = ConfigurationData["Password"];
            if (ConfigurationData.ContainsKey("UserIdActive")) UserIdActive = AlwaysConvert.ToBool(ConfigurationData["UserIdActive"], false);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], false);
            if (ConfigurationData.ContainsKey("EnablePackageBreakup")) EnablePackageBreakup = AlwaysConvert.ToBool(ConfigurationData["EnablePackageBreakup"], true);
            if (ConfigurationData.ContainsKey("MinPackageWeight")) MinPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MinPackageWeight"], (decimal)MinPackageWeight);
            if (ConfigurationData.ContainsKey("MaxPackageWeight")) MaxPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MaxPackageWeight"], (decimal)MaxPackageWeight);
            if (ConfigurationData.ContainsKey("TestModeUrl")) TestModeUrl = ConfigurationData["TestModeUrl"];
            if (ConfigurationData.ContainsKey("LiveModeUrl")) LiveModeUrl = ConfigurationData["LiveModeUrl"];
            if (ConfigurationData.ContainsKey("TrackingUrl")) TrackingUrl = ConfigurationData["TrackingUrl"];
        }

        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData.Add("UserId", this.UserId);
            configData.Add("Password", this.Password);
            configData.Add("UserIdActive", this.UserIdActive.ToString());
            configData.Add("UseTestMode", this.UseTestMode.ToString());
            configData.Add("EnablePackageBreakup", this.EnablePackageBreakup.ToString());
            configData.Add("MinPackageWeight", this.MinPackageWeight.ToString());
            configData.Add("MaxPackageWeight", this.MaxPackageWeight.ToString());
            configData.Add("TestModeUrl", this.TestModeUrl);
            configData.Add("LiveModeUrl", this.LiveModeUrl);
            configData.Add("TrackingUrl", this.TrackingUrl);

            return configData;
        }

        public override ShipRateQuote GetShipRateQuote(Warehouse origin, CommerceBuilder.Users.Address destination, BasketItemCollection contents, string serviceCode)
        {
            //GET THE SHIP QUOTE FOR THE GIVEN SERVICE
            //GET THE RATE FOR ALL SERVICES
            Dictionary<String, ProviderShipRateQuote> allProviderQuotes = GetAllProviderShipRateQuotes(origin, destination, contents);
            if (allProviderQuotes != null && allProviderQuotes.ContainsKey(serviceCode))
            {
                ProviderShipRateQuote providerQuote = allProviderQuotes[serviceCode];
                ShipRateQuote quote = new ShipRateQuote();                
                quote.Rate = providerQuote.Rate;
                return quote;
            }
            return null;
        }

        private static Dictionary<string, string> _services;
        static USPS()
        {
            _services = new Dictionary<string, string>();
            _services.Add("Express Mail Sunday/Holiday Guarantee", "USPS Express Mail Sunday/Holiday Guarantee");            
            _services.Add("Express Mail Flat-Rate Envelope Sunday/Holiday Guarantee", "USPS Express Mail Flat-Rate Envelope Sunday/Holiday Guarantee");            
            _services.Add("Express Mail Hold For Pickup", "USPS Express Mail Hold For Pickup");
            _services.Add("Express Mail Flat Rate Envelope", "USPS Express Mail Flat Rate Envelope");
            _services.Add("Express Mail Flat Rate Envelope Hold For Pickup", "USPS Express Mail Flat Rate Envelope Hold For Pickup");
            _services.Add("Express Mail", "USPS Express Mail");
            
            _services.Add("Priority Mail", "USPS Priority Mail");
            _services.Add("Priority Mail Flat Rate Envelope", "USPS Priority Mail Flat Rate Envelope");
            _services.Add("Priority Mail Small Flat Rate Box", "USPS Priority Mail Small Flat Rate Box");
            _services.Add("Priority Mail Medium Flat Rate Box", "USPS Priority Mail Medium Flat Rate Box");
            _services.Add("Priority Mail Large Flat Rate Box", "USPS Priority Mail Large Flat-Rate Box"); 

            _services.Add("First-Class Mail", "USPS First-Class Mail");
            _services.Add("First-Class Mail Flat", "USPS First-Class Mail Flat");
            _services.Add("First-Class Mail Parcel", "USPS First-Class Mail Parcel");

            _services.Add("Parcel Post", "USPS Parcel Post");
            _services.Add("Bound Printed Matter", "USPS Bound Printed Matter");
            _services.Add("Library Mail", "USPS Library Mail");
            _services.Add("Media Mail", "USPS Media Mail");

            //INTERNATIONAL SERVICES
            _services.Add("Global Express Guaranteed (GXG)", "USPS Global Express Guaranteed");
            _services.Add("Global Express Guaranteed Non-Document Rectangular", "USPS Global Express Guaranteed Non-Document Rectangular");
            _services.Add("Global Express Guaranteed Non-Document Non-Rectangular", "USPS Global Express Guaranteed Non-Document Non-Rectangular");

            _services.Add("USPS GXG Envelopes", "USPS GXG Envelopes");

            _services.Add("Express Mail International", "USPS Express Mail International (EMS)");
            _services.Add("Express Mail International Flat Rate Envelope", "USPS Express Mail International (EMS) Flat Rate Envelope");
            
            _services.Add("Priority Mail International", "USPS Priority Mail International");
            _services.Add("Priority Mail International Flat Rate Envelope", "USPS Priority Mail International Flat Rate Envelope");
            _services.Add("Priority Mail International Small Flat Rate Box", "USPS Priority Mail International Small Flat Rate Box");
            _services.Add("Priority Mail International Medium Flat Rate Box", "USPS Priority Mail International Medium Flat Rate Box");
            _services.Add("Priority Mail International Large Flat Rate Box", "USPS Priority Mail International Large Flat Rate Box");
            
            _services.Add("First-Class Mail International Letter", "USPS First-Class Mail International Letter");
            _services.Add("First-Class Mail International Flat", "USPS First Class Mail International Flat");
            _services.Add("First-Class Mail International Parcel", "USPS First Class Mail International Parcel");
            _services.Add("First-Class Mail International Large Envelope", "USPS First-Class Mail International Large Envelope");
            _services.Add("First-Class Mail International Package", "USPS First-Class Mail International Package");

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
                

        private XmlDocument BuildProviderRequest(Package[] shipmentPackages, string originZip, string destinationZip)
        {
            //CREATE PROVIDER REQUEST DOCUMENT
            XmlDocument providerRequest = new XmlDocument();
            string _requestXml = string.Format("<?xml version=\"1.0\"?><RateV3Request USERID=\"{0}\"></RateV3Request>",this.UserId);
            providerRequest.LoadXml(_requestXml);
            //LOOP PACKAGES AND APPEND TO PROVIDER REQUEST
            int packageIndex = 0;
            foreach (Package package in shipmentPackages)
            {
                //GET WEIGHT OF PACKAGE IN WHOLE POUNDS AND OUNCES
                int pounds = (int)(package.Weight / 16);
                //For domestic rate requests ounces weight can be decimal
                LSDecimal ounces = (package.Weight - (pounds * 16));

                package.RearrangeDimensions();
                //BUILD XML PACKAGE ELEMENTS FOR RATE SHOPPING
                XmlElement packageElement = (XmlElement)providerRequest.DocumentElement.AppendChild(providerRequest.CreateElement("Package"));
                XmlUtility.SetAttributeValue(packageElement, "ID", packageIndex.ToString());
                XmlUtility.SetElementValue(packageElement, "Service", "ALL");
                XmlUtility.SetElementValue(packageElement, "ZipOrigination", originZip);
                XmlUtility.SetElementValue(packageElement, "ZipDestination", destinationZip);
                XmlUtility.SetElementValue(packageElement, "Pounds", pounds.ToString());
                XmlUtility.SetElementValue(packageElement, "Ounces", string.Format("{0:F1}", ounces));
                XmlUtility.SetElementValue(packageElement, "Container", GetContainerValue(package));
                XmlUtility.SetElementValue(packageElement, "Size", GetPackageSizeValue(package));
                if(package.Length > 0 && package.Width > 0) 
                {
                    XmlUtility.SetElementValue(packageElement, "Width", package.Width.ToString());
                    XmlUtility.SetElementValue(packageElement, "Length", package.Length.ToString());
                    XmlUtility.SetElementValue(packageElement, "Height", package.Height.ToString());
                    LSDecimal girth = 2 * (package.Width * package.Height);
                    XmlUtility.SetElementValue(packageElement, "Girth", string.Format("{0:F2}",girth));
                }

                XmlUtility.SetElementValue(packageElement, "Machinable", "true");
                packageIndex++;
            }
            return providerRequest;
        }

        private static string GetContainerValue(Package package)
        {
            if (package.Height <= 0)
            {
                //package is envelope type
                if (package.Length <= 12.5m && package.Width <= 9.5m)
                {
                    return "FLAT RATE ENVELOPE";
                }
            }
            else
            {
                //box type
                if (package.Length <= 14 && package.Width <= 12 && package.Height <= 3.5m)
                {
                    return "FLAT RATE BOX";
                }

                if (package.Length <= 11.25m && package.Width <= 8.75m && package.Height <= 6)
                {
                    return "FLAT RATE BOX";
                }
            }

            return "VARIABLE";
        }

        private static string GetPackageSizeValue(Package package)
        {

            //CALCULATE LENGTH PLUS GIRTH            
            //GIRTH IS 2H + 2W (OR 2(H+W))
            LSDecimal lengthPlusGirth = package.Length + (2 * (package.Height + package.Width));
            if (lengthPlusGirth <= 84)
            {
                return "REGULAR";
            }
            else if (lengthPlusGirth <= 108 )
            {
                return "LARGE";
            }
            else if (lengthPlusGirth <= 130)
            {
                return "OVERSIZE";
            }
            else
            {
                //what about extra large packages?? they can't ship?
                return "OVERSIZE";
            }
        }

        private XmlDocument BuildProviderRequestIntl(Package[] shipmentPackages, string destinationCountry)
        {
            //CREATE PROVIDER REQUEST DOCUMENT
            XmlDocument providerRequest = new XmlDocument();
            string _reqXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><IntlRateRequest USERID=\"{0}\"></IntlRateRequest>";
            providerRequest.LoadXml(string.Format(_reqXml,this.UserId));            
            //LOOP PACKAGES AND APPEND TO PROVIDER REQUEST
            int packageIndex = 0;
            foreach (Package package in shipmentPackages)
            {
                //GET WEIGHT OF PACKAGE IN WHOLE POUNDS AND OUNCES
                int pounds = (int)(package.Weight / 16);
                //For International rate requests ounces weight should also be integer value
                int ounces = (int)(package.Weight - (pounds * 16));

                //BUILD XML PACKAGE ELEMENTS FOR RATE SHOPPING
                XmlElement packageElement = (XmlElement)providerRequest.DocumentElement.AppendChild(providerRequest.CreateElement("Package"));
                XmlUtility.SetAttributeValue(packageElement, "ID", packageIndex.ToString());
                XmlUtility.SetElementValue(packageElement, "Pounds", pounds.ToString());
                XmlUtility.SetElementValue(packageElement, "Ounces", ounces.ToString());
                XmlUtility.SetElementValue(packageElement, "MailType", "Package");                
                XmlUtility.SetElementValue(packageElement, "Country", destinationCountry);
                packageIndex++;
            }
            return providerRequest;
        }

        private Dictionary<string, ProviderShipRateQuote> ParseProviderRateReponse(XmlDocument providerResponse, Package[] packageList)
        {
            //CREATE TEMPORARY STORAGE FOR PARSING THE QUOTED SERVICES
            Dictionary<string, ProviderShipRateQuote> availableServices = new Dictionary<string, ProviderShipRateQuote>();
            ProviderShipRateQuote serviceQuote;
            //LOOP EACH PACKAGE IN THE RESPONSE
            XmlNodeList packageNodeList = providerResponse.DocumentElement.SelectNodes("Package");
            foreach (XmlNode packageNode in packageNodeList)
            {
                XmlNodeList postageNodeList = packageNode.SelectNodes("Postage");
                foreach (XmlNode postageNode in postageNodeList)
                {
                    string serviceName = XmlUtility.GetElementValue(postageNode, "MailService", string.Empty);
                    LSDecimal postage = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(postageNode, "Rate", string.Empty));
                    if (postage > 0)
                    {
                        //GET THE INSTANCE OF SERVICEQUOTE TO BE UPDATED
                        if (availableServices.ContainsKey(serviceName))
                        {
                            serviceQuote = availableServices[serviceName];
                        }
                        else
                        {
                            //CREATE A NEW INSTANCE OF ProviderShipRateQuote
                            serviceQuote = new ProviderShipRateQuote();
                            serviceQuote.ServiceCode = serviceName;
                            availableServices.Add(serviceName, serviceQuote);
                        }
                        //GET THE PACKAGE ID FROM THE RATE
                        int packageId = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(packageNode, "ID", string.Empty),-1);                        
                        if (packageId != -1)
                        {
                            //FIND THE PACKAGE IN THE LIST
                            Package package = packageList[packageId];
                            serviceQuote.Rate += (postage * package.Multiplier);
                            serviceQuote.PackageCount += 1;
                        }
                    }
                }
            }
            //MOVE THROUGH LIST AND FIND SERVICES THAT ARE NOT AVAILABLE
            //FOR ALL PACKAGES IN THE SHIPMENT
            List<string> removeServices = new List<string>();
            foreach (string serviceName in availableServices.Keys)
            {
                serviceQuote = availableServices[serviceName];
                if (serviceQuote.PackageCount < packageList.Length)
                {
                    removeServices.Add(serviceName);
                }
            }
            //REMOVE ANY SERVICES THAT WERE IDENTIFIED
            foreach (string serviceName in removeServices)
            {
                availableServices.Remove(serviceName);
            }
            return availableServices;
        }

        private Dictionary<string, ProviderShipRateQuote> ParseProviderRateReponseIntl(XmlDocument providerResponse, Package[] packageList)
        {
            //CREATE TEMPORARY STORAGE FOR PARSING THE QUOTED SERVICES
            Dictionary<string, ProviderShipRateQuote> availableServices = new Dictionary<string, ProviderShipRateQuote>();
            ProviderShipRateQuote serviceQuote;
            //LOOP EACH PACKAGE IN THE RESPONSE
            XmlNodeList packageNodeList = providerResponse.DocumentElement.SelectNodes("Package");
            foreach (XmlNode packageNode in packageNodeList)
            {
                XmlNodeList serviceNodeList = packageNode.SelectNodes("Service");
                foreach (XmlNode serviceNode in serviceNodeList)
                {
                    string serviceName = XmlUtility.GetElementValue(serviceNode, "SvcDescription", string.Empty);
                    LSDecimal postage = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(serviceNode, "Postage", string.Empty));
                    if (postage > 0)
                    {
                        //GET THE INSTANCE OF SERVICEQUOTE TO BE UPDATED
                        if (availableServices.ContainsKey(serviceName))
                        {
                            serviceQuote = availableServices[serviceName];
                        }
                        else
                        {
                            //CREATE A NEW INSTANCE OF ProviderShipRateQuote
                            serviceQuote = new ProviderShipRateQuote();
                            serviceQuote.ServiceCode = serviceName;
                            availableServices.Add(serviceName, serviceQuote);
                        }
                        //GET THE PACKAGE ID FROM THE RATE
                        int packageId = AlwaysConvert.ToInt(XmlUtility.GetAttributeValue(packageNode, "ID", string.Empty),-1);
                        if (packageId != -1)
                        {
                            //FIND THE PACKAGE IN THE LIST
                            Package package = packageList[packageId];
                            serviceQuote.Rate += (postage * package.Multiplier);
                            serviceQuote.PackageCount += 1;
                        }
                    }
                }
            }
            //MOVE THROUGH LIST AND FIND SERVICES THAT ARE NOT AVAILABLE
            //FOR ALL PACKAGES IN THE SHIPMENT
            List<string> removeServices = new List<string>();
            foreach (string serviceName in availableServices.Keys)
            {
                serviceQuote = availableServices[serviceName];
                if (serviceQuote.PackageCount < packageList.Length)
                {
                    removeServices.Add(serviceName);
                }
            }
            //REMOVE ANY SERVICES THAT WERE IDENTIFIED
            foreach (string serviceName in removeServices)
            {
                availableServices.Remove(serviceName);
            }
            return availableServices;
        }

        private Dictionary<string, ProviderShipRateQuote> GetAllProviderShipRateQuotes(Warehouse origin, Address destination, BasketItemCollection contents)
        {
            //CHECK CACHE FOR QUOTES FOR THIS SHIPMEN
            string cacheKey = StringHelper.CalculateMD5Hash(Utility.Misc.GetClassId(this.GetType()) + "_" + origin.WarehouseId.ToString() + "_" + destination.AddressId.ToString() + "_" + contents.GenerateContentHash());
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                WebTrace.Write("Checking Cache for " + cacheKey);
                if (context.Items.Contains(cacheKey))
                {
                    return (Dictionary<string, ProviderShipRateQuote>)context.Items[cacheKey];
                }
            }

            //VERIFY WE HAVE A DESTINATION COUNTRY
            if (string.IsNullOrEmpty(destination.CountryCode)) return null;

            //BUILD A LIST OF USPS PACKAGES FROM THE SHIPMENT ITEMS
            PackageList plist = PreparePackages(origin, contents);
            if (plist == null || plist.Count == 0) return null;
            Package[] shipmentPackages = plist.ToArray();

            //BUILD THE REQUEST FOR THIS SHIPMENT
            XmlDocument providerRequest;
            bool domestic = (destination.CountryCode == "US");
            if (domestic)
            {
                if (string.IsNullOrEmpty(origin.PostalCode) || (origin.PostalCode.Length < 5)) throw new ArgumentException("origin.PostalCode is empty or invalid length");
                if (string.IsNullOrEmpty(destination.PostalCode) || (destination.PostalCode.Length < 5)) throw new ArgumentException("destination.PostalCode is empty or invalid length");
                providerRequest = BuildProviderRequest(shipmentPackages, origin.PostalCode.Substring(0, 5), destination.PostalCode.Substring(0, 5));
            }
            else
            {
                string countryName = destination.Country.Name;
                if (destination.CountryCode.Equals("GB"))
                {
                    countryName = "Great Britain";
                }
                providerRequest = BuildProviderRequestIntl(shipmentPackages, countryName);
            }
            //SEND THIS REQUEST TO THE PROVIDER
            XmlDocument providerResponse = SendRequestToProvider(providerRequest, domestic ? "RateV3" : "IntlRate");
            //PARSE THE THE RATE QUOTES FROM THE RESPONSE
            Dictionary<string, ProviderShipRateQuote> allServiceQuotes;
            if (domestic)
            {
                allServiceQuotes = ParseProviderRateReponse(providerResponse, shipmentPackages);
            }
            else
            {
                allServiceQuotes = ParseProviderRateReponseIntl(providerResponse, shipmentPackages);
            }
            //CACHE THE RATE QUOTES INTO REQUEST
            if (context != null) context.Items[cacheKey] = allServiceQuotes;
            return allServiceQuotes;
        }

        private PackageList PreparePackages(Warehouse origin, BasketItemCollection contents)
        {
            PackageList plist = PackageManager.GetPackageList(contents);
            if (plist == null || plist.Count == 0) return null;

            //GET UNITS USED BY STORE
            Store store = StoreDataSource.Load(Token.Instance.StoreId);
            MeasurementUnit storeMeasurementUnit = store.MeasurementUnit;
            WeightUnit storeWeightUnit = store.WeightUnit;

            bool requireMC = storeMeasurementUnit != MeasurementUnit.Inches;
            bool requireWC = storeWeightUnit != WeightUnit.Ounces;

            if (requireMC && requireWC)
            {
                plist.ConvertBoth(WeightUnit.Ounces,MeasurementUnit.Inches);
            }
            else if (requireWC)
            {
                plist.ConvertWeight(WeightUnit.Ounces);
            }
            else if (requireMC)
            {
                plist.ConvertDimensions(MeasurementUnit.Inches);
            }

            LSDecimal maxWeight = LocaleHelper.ConvertWeight(WeightUnit.Pounds, MaxPackageWeight, WeightUnit.Ounces);
            LSDecimal minWeight = LocaleHelper.ConvertWeight(WeightUnit.Pounds, MinPackageWeight, WeightUnit.Ounces);
            if (EnablePackageBreakup && maxWeight > 0)
            {
                //compose packages (splits items larger than the maximum carrier weight)
                plist.Compose(maxWeight, minWeight);
            }
            else
            {
                plist.EnsureMinimumWeight(minWeight);
            }

            //convert weight to whole number
            //plist.ConvertWeightToWholeNumber();
            //plist.ConvertDimsToWholeNumbers();            

            return plist;
        }

        public override TrackingSummary GetTrackingSummary(TrackingNumber trackingNumber)
        {
            TrackingSummary summary = new TrackingSummary();
            summary.TrackingResultType = TrackingResultType.ExternalLink;            
            summary.TrackingLink = string.Format(TrackingUrl, HttpUtility.UrlEncode(trackingNumber.TrackingNumberData));
            return summary;
        }

        //input format is xs:date and xs:time for strDate and strTime respectively
        // YYYY-MM-DD and hh:mm:ss
        //date, time returned by USPS are not in xs:date and xs:time format 
        //they are in 
        private DateTime ParseUspsDate(string dateValue, string timeValue)
        {
            try
            {
                DateTime dtDate;
                DateTime dtTime;
                CultureInfo culture = CultureInfo.InvariantCulture;
                if (!string.IsNullOrEmpty(dateValue))
                {
                    dtDate = DateTime.ParseExact(dateValue, "MMMM d, yyyy", culture);
                    int year = dtDate.Year;
                    int month = dtDate.Month;
                    int day = dtDate.Day;
                    if (!string.IsNullOrEmpty(timeValue))
                    {
                        dtTime = DateTime.ParseExact(timeValue, "h:mm tt", culture);
                        int hour = dtTime.Hour;
                        int minute = dtTime.Minute;
                        return new DateTime(year, month, day, hour, minute, 0);
                    }
                    return new DateTime(year, month, day);
                }
                return System.DateTime.MinValue;
            }
            catch (Exception exp)
            {
                //if there is a date parsing error, just return min date
                Logger.Warn("Date Parsing Failed on USPS Response.",exp);
                return System.DateTime.MinValue;
            }
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
        
        private XmlDocument SendRequestToProvider(XmlDocument request, string apiName)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(GetEffectiveUrl());
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }
            string requestData = "API=" + apiName + "&XML=" + HttpUtility.UrlEncode(request.OuterXml);

            //RECORD SEND HERE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, HttpUtility.UrlDecode(requestData));

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(requestData);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            //READ RESPONSEDATA FROM STREAM
            string responseData;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
            {
                responseData = responseStream.ReadToEnd();
                responseStream.Close();
            }

            //RECORD RESONSE HERE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData);

            //LOAD RESPONSE INTO XML DOCUMENT
            XmlDocument providerResponse = new XmlDocument();
            providerResponse.LoadXml(responseData);

            //RETURN THE RESPONSE DOCUMENT
            return providerResponse;
        }

        protected string SendGetRequestToProvider(string url, string encoding)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }

            //READ RESPONSEDATA FROM STREAM
            string responseData;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.GetEncoding(encoding)))
            {
                responseData = responseStream.ReadToEnd();
                responseStream.Close();
            }

            //RETURN THE RESPONSE 
            return responseData;
        }

    }
}
