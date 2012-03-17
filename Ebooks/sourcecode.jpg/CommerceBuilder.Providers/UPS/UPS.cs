using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Web;
using System.IO;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace CommerceBuilder.Shipping.Providers.UPS
{
    public class UPS : ShippingProviderBase
    {

        private string _UserId;
        private string _Password;
        private string _AccessKey;
        private string _ShipperNumber;
        private UpsCustomerType _CustomerType = UpsCustomerType.Retail;
        private bool _UseInsurance;

        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public string AccessKey
        {
            get { return _AccessKey; }
            set { _AccessKey = value; }
        }

        public string ShipperNumber
        {
            get { return _ShipperNumber; }
            set { _ShipperNumber = value; }
        }

        public UpsCustomerType CustomerType
        {
            get { return _CustomerType; }
            set { _CustomerType = value; }
        }

        public bool UseInsurance
        {
            get { return _UseInsurance; }
            set { _UseInsurance = value; }
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

        //lbs
        private LSDecimal _MaxPackageWeight = 150;
        public LSDecimal MaxPackageWeight
        {
            get { return _MaxPackageWeight; }
            set { _MaxPackageWeight = value; }
        }

        private string _TestModeUrl = "https://wwwcie.ups.com/ups.app/xml/";
        public string TestModeUrl
        {
            get { return _TestModeUrl; }
            set { _TestModeUrl = value; }
        }

        private string _LiveModeUrl = "https://www.ups.com/ups.app/xml/";
        public string LiveModeUrl
        {
            get { return _LiveModeUrl; }
            set { _LiveModeUrl = value; }
        }

        //http://wwwapps.ups.com/WebTracking/processInputRequest?tracknum=1Z335E1F0306460390&AgreeToTermsAndConditions=yes        
        private static string _DefaultTrackingUrl = "http://wwwapps.ups.com/WebTracking/processInputRequest?tracknum={0}&AgreeToTermsAndConditions=yes";
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

        public bool IsActive
        {
            get { return (this.IsRegistered && !string.IsNullOrEmpty(this.AccessKey)); }
        }

        public bool IsRegistered
        {
            get { return (!(string.IsNullOrEmpty(this.UserId) || string.IsNullOrEmpty(this.Password))); }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Shipping.Providers.UPS.Logo.jpg");
            return string.Empty;
        }

        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "UPS/Default.aspx";  
        }

        public override string Description
        {
            get { return "The integrated UPS OnLine&reg; Tools can calculate shipping rates for shipments originating from 37 countries to international destinations.  It also can provide tracking details."; }
        }

        public override void Initialize(int ShipGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(ShipGatewayId, ConfigurationData);
            //INITIALIZE MY FIELDS
            if (ConfigurationData.ContainsKey("UserId")) UserId = ConfigurationData["UserId"];
            if (ConfigurationData.ContainsKey("Password")) Password = ConfigurationData["Password"];
            if (ConfigurationData.ContainsKey("AccessKey")) AccessKey = ConfigurationData["AccessKey"];
            if (ConfigurationData.ContainsKey("ShipperNumber")) ShipperNumber = ConfigurationData["ShipperNumber"];
            if (ConfigurationData.ContainsKey("CustomerType")) CustomerType = (UpsCustomerType)AlwaysConvert.ToInt(ConfigurationData["CustomerType"]);
            if (ConfigurationData.ContainsKey("UseInsurance")) UseInsurance = AlwaysConvert.ToBool(ConfigurationData["UseInsurance"], false);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], false);
            if (ConfigurationData.ContainsKey("AccountActive")) AccountActive = AlwaysConvert.ToBool(ConfigurationData["AccountActive"], false);
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
            configData.Add("AccessKey", this.AccessKey);
            configData.Add("ShipperNumber", this.ShipperNumber);
            configData.Add("CustomerType", ((int)this.CustomerType).ToString());
            configData.Add("UseInsurance", this.UseInsurance.ToString());
            configData.Add("UseTestMode", this.UseTestMode.ToString());
            configData.Add("AccountActive", this.AccountActive.ToString());
            configData.Add("EnablePackageBreakup", this.EnablePackageBreakup.ToString());
            configData.Add("MinPackageWeight", this.MinPackageWeight.ToString());
            configData.Add("MaxPackageWeight", this.MaxPackageWeight.ToString());
            configData.Add("TestModeUrl", this.TestModeUrl);
            configData.Add("LiveModeUrl", this.LiveModeUrl);
            configData.Add("TrackingUrl", this.TrackingUrl);

            return configData;
        }

        public static string GetAgreement() { return UPS.GetAgreement("US", "EN"); }

        public static string GetAgreement(string countryCode, string languageCode)
        {
            XmlDocument providerRequest = new XmlDocument();
            providerRequest.LoadXml("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><AccessLicenseAgreementRequest><Request><TransactionReference><CustomerContext/><XpciVersion Version=\"1.0001\"/></TransactionReference><RequestAction>AccessLicense</RequestAction><RequestOption>AllTools</RequestOption></Request><DeveloperLicenseNumber>1B5C79535710B12C</DeveloperLicenseNumber><AccessLicenseProfile><CountryCode/><LanguageCode/></AccessLicenseProfile><OnLineTool><ToolID>RateXML</ToolID><ToolVersion>1.0</ToolVersion></OnLineTool><OnLineTool><ToolID>TrackXML</ToolID><ToolVersion>1.0</ToolVersion></OnLineTool></AccessLicenseAgreementRequest>");
            providerRequest.PreserveWhitespace = true;
            providerRequest.DocumentElement.SelectSingleNode("AccessLicenseProfile/CountryCode").InnerText = countryCode;
            providerRequest.DocumentElement.SelectSingleNode("AccessLicenseProfile/LanguageCode").InnerText = languageCode;
            XmlDocument providerResponse = UPS.SendRequestToProviderWithoutCredentials(providerRequest, "https://www.ups.com/ups.app/xml/License");
            return XmlUtility.GetElementValue(providerResponse.DocumentElement, "AccessLicenseText");
        }

        public void Register(ShipGateway shipGateway, UpsRegistrationInformation registrationInformation)
        {
            //SET THE GATEWAY REFERENCE
            _ShipGateway = shipGateway;
            //GENERATE A RANDOM USERID AND PASSWORD
            string randomUserId = "AbleC" + StringHelper.RandomNumber(10);
            string randomPassword = StringHelper.RandomString(9);

            //BUILD THE REGISTRATION REQUEST DOCUMENT
            XmlDocument providerRequest = new XmlDocument();
            providerRequest.PreserveWhitespace = true;
            providerRequest.LoadXml("<?xml version=\"1.0\"?><RegistrationRequest><Request><TransactionReference><CustomerContext/><XpciVersion Version=\"1.0001\"/></TransactionReference><RequestAction>Register</RequestAction><RequestOption>suggest</RequestOption></Request><UserId/><Password/><RegistrationInformation><UserName/><CompanyName/><Title/><Address><AddressLine1/><AddressLine2/><AddressLine3/><City/><StateProvinceCode/><PostalCode/><CountryCode/></Address><PhoneNumber/><PhoneExtension/><EMailAddress/></RegistrationInformation></RegistrationRequest>");
            providerRequest.DocumentElement.SelectSingleNode("Password").InnerText = randomPassword;
            XmlNode node = providerRequest.DocumentElement.SelectSingleNode("RegistrationInformation");
            node.SelectSingleNode("UserName").InnerText = registrationInformation.ContactName;
            node.SelectSingleNode("CompanyName").InnerText = registrationInformation.Company;
            node.SelectSingleNode("Title").InnerText = registrationInformation.ContactTitle;
            node.SelectSingleNode("PhoneNumber").InnerText = registrationInformation.ContactPhone;
            node.SelectSingleNode("PhoneExtension").InnerText = registrationInformation.ContactPhoneExtension;
            node.SelectSingleNode("EMailAddress").InnerText = registrationInformation.ContactEmail;
            node = node.SelectSingleNode("Address");
            node.SelectSingleNode("AddressLine1").InnerText = registrationInformation.Address1;
            node.SelectSingleNode("AddressLine2").InnerText = registrationInformation.Address2;
            node.SelectSingleNode("AddressLine3").InnerText = registrationInformation.Address3;
            node.SelectSingleNode("City").InnerText = registrationInformation.City;
            node.SelectSingleNode("StateProvinceCode").InnerText = registrationInformation.StateProvinceCode;
            node.SelectSingleNode("PostalCode").InnerText = registrationInformation.PostalCode;
            node.SelectSingleNode("CountryCode").InnerText = registrationInformation.CountryCode;
            if (!string.IsNullOrEmpty(registrationInformation.ShipperNumber))
            {
                node.AppendChild(providerRequest.CreateElement("ShipperAccount"));
                node = node.SelectSingleNode("ShipperAccount");
                node.AppendChild(providerRequest.CreateElement("ShipperNumber"));
                node.SelectSingleNode("ShipperNumber").InnerText = registrationInformation.ShipperNumber;
            }

            //SEND REQUEST TO PROVIDER AND GET RESPONSE
            //KEEP TRYING REGISTER REQUEST UNTIL THE USERID IS ACCEPTED
            int iterations = 0;
            String suggestedUserId = string.Empty;
            do
            {
                providerRequest.DocumentElement.SelectSingleNode("UserId").InnerText = randomUserId;
                XmlDocument providerResponse = UPS.SendRequestToProviderWithoutCredentials(providerRequest, GetEffectiveUrl("Register"));
                int responseCode = AlwaysConvert.ToInt(XmlUtility.GetElementValue(providerResponse.DocumentElement, "Response/ResponseStatusCode"));
                if (responseCode == 1)
                {
                    suggestedUserId = XmlUtility.GetElementValue(providerResponse.DocumentElement, "UserId");
                    if (string.IsNullOrEmpty(suggestedUserId))
                    {
                        //USER ID ACCEPTED BY UPS, SAVE THE CURRENT REGISTRATION DETAILS FOR THE GATEWAY
                        this.UserId = randomUserId;
                        this.Password = randomPassword;
                        this.AccessKey = string.Empty;
                        this.ShipperNumber = registrationInformation.ShipperNumber != null ? registrationInformation.ShipperNumber : string.Empty;
                        this.ShipGateway.UpdateConfigData(this.GetConfigData());
                        this.ShipGateway.Save();
                        //NOW ATTEMPT TO COMPLETE LICENSING
                        this.License(registrationInformation);
                    }
                    else
                    {
                        //A SUGGESTED USERID WAS GIVEN.  ACCEPT THE SUGGESTION AND TRY AGAIN
                        randomUserId = suggestedUserId;
                    }
                }
                else
                {
                    //UPS RETURNED AN ERROR
                    ShipGateway.Delete();
                    HandleProviderError(providerResponse);
                }
                iterations++;
            }
            while (!string.IsNullOrEmpty(suggestedUserId) && (iterations < 5));
        }

        private ShipGateway _ShipGateway;
        private ShipGateway ShipGateway
        {
            get
            {
                //LOADS AN INSTANCE OF THE SHIPGATEWAY
                if (_ShipGateway == null)
                {
                    _ShipGateway = new ShipGateway();
                    if (!_ShipGateway.Load(this.ShipGatewayId))
                    {
                        _ShipGateway.Name = this.Name;
                        _ShipGateway.ClassId = Utility.Misc.GetClassId(this.GetType());
                        _ShipGateway.Save();
                        this.ShipGatewayId = _ShipGateway.ShipGatewayId;
                    }
                }
                return _ShipGateway;
            }
        }

        public void License(UpsRegistrationInformation registrationInformation)
        {
            //ONLY SUBMIT TO LICENSING IF A VALID USERID AND PASSWORD ARE AVAILABLE
            //AND THE ACCESS KEY IS NOT SET
            if (this.IsRegistered && string.IsNullOrEmpty(this.AccessKey))
            {
                //BUILD REQUEST
                XmlDocument providerRequest = new XmlDocument();
                providerRequest.LoadXml("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><AccessLicenseRequest><Request><TransactionReference><CustomerContext>Access License</CustomerContext><XpciVersion Version=\"1.0001\"/></TransactionReference><RequestAction>AccessLicense</RequestAction></Request><CompanyName/><Address><AddressLine1/><City/><StateProvinceCode/><PostalCode/><CountryCode/></Address><PrimaryContact><Name/><Title/><EMailAddress/><PhoneNumber/></PrimaryContact><CompanyURL/><DeveloperLicenseNumber/><AccessLicenseProfile><CountryCode/><LanguageCode/><AccessLicenseText></AccessLicenseText></AccessLicenseProfile><OnLineTool><ToolID>RateXML</ToolID><ToolVersion>1.0</ToolVersion></OnLineTool><OnLineTool><ToolID>TrackXML</ToolID><ToolVersion>1.0</ToolVersion></OnLineTool><ClientSoftwareProfile><SoftwareInstaller/><SoftwareProductName/><SoftwareProvider/><SoftwareVersionNumber/></ClientSoftwareProfile></AccessLicenseRequest>");
                providerRequest.PreserveWhitespace = true;
                XmlUtility.SetElementValue(providerRequest.DocumentElement, "CompanyName", registrationInformation.Company);
                XmlUtility.SetElementValue(providerRequest.DocumentElement, "CompanyURL", registrationInformation.CompanyUrl);
                XmlUtility.SetElementValue(providerRequest.DocumentElement, "DeveloperLicenseNumber", "1B5C79535710B12C");
                if (!string.IsNullOrEmpty(registrationInformation.ShipperNumber))
                {
                    XmlUtility.SetElementValue(providerRequest.DocumentElement, "ShipperNumber", registrationInformation.ShipperNumber);
                }

                XmlElement node = XmlUtility.GetElement(providerRequest.DocumentElement, "Address", true);
                XmlUtility.SetElementValue(node, "AddressLine1", registrationInformation.Address1);
                XmlUtility.SetElementValue(node, "City", registrationInformation.City);
                XmlUtility.SetElementValue(node, "StateProvinceCode", registrationInformation.StateProvinceCode);
                XmlUtility.SetElementValue(node, "PostalCode", registrationInformation.PostalCode);
                XmlUtility.SetElementValue(node, "CountryCode", registrationInformation.CountryCode);

                node = XmlUtility.GetElement(providerRequest.DocumentElement, "PrimaryContact", true);
                XmlUtility.SetElementValue(node, "Name", registrationInformation.ContactName);
                XmlUtility.SetElementValue(node, "Title", registrationInformation.ContactTitle);
                XmlUtility.SetElementValue(node, "PhoneNumber", registrationInformation.ContactPhone);
                XmlUtility.SetElementValue(node, "EMailAddress", registrationInformation.ContactEmail);

                node = XmlUtility.GetElement(providerRequest.DocumentElement, "AccessLicenseProfile", true);
                XmlUtility.SetElementValue(node, "CountryCode", "US");
                XmlUtility.SetElementValue(node, "LanguageCode", "EN");
                XmlUtility.SetElementValue(node, "AccessLicenseText", UPS.GetAgreement());

                node = XmlUtility.GetElement(providerRequest.DocumentElement, "ClientSoftwareProfile", true);
                XmlUtility.SetElementValue(node, "SoftwareProductName", "AbleCommerce");
                XmlUtility.SetElementValue(node, "SoftwareProvider", "Able Solutions Corporation");
                XmlUtility.SetElementValue(node, "SoftwareVersionNumber", "6.0");
                //next field is for sales rep contact
                XmlUtility.SetElementValue(node, "SoftwareInstaller", registrationInformation.RequestContact ? "YES" : "NO");

                //SEND REQUEST AND RECEIVE RESPONES
                XmlDocument providerResponse = UPS.SendRequestToProviderWithoutCredentials(providerRequest, GetEffectiveUrl("License"));
                int responseCode = AlwaysConvert.ToInt(XmlUtility.GetElementValue(providerResponse.DocumentElement, "Response/ResponseStatusCode", string.Empty));
                if (responseCode == 1)
                {
                    //license succesful
                    this.AccessKey = XmlUtility.GetElementValue(providerResponse.DocumentElement, "AccessLicenseNumber");
                    ShipGateway.UpdateConfigData(this.GetConfigData());
                    ShipGateway.Save();
                }
                else
                {
                    ShipGateway.Delete();
                    //UPS RETURNED AN ERROR, THROW EXCEPTION
                    HandleProviderError(providerResponse);
                }
            }
        }

        private void HandleProviderError(XmlDocument providerResponse)
        {
            int responseCode = AlwaysConvert.ToInt(XmlUtility.GetElementValue(providerResponse.DocumentElement, "Response/ResponseStatusCode"));
            XmlElement errorNode = XmlUtility.GetElement(providerResponse.DocumentElement, "Response/Error", false);
            if (errorNode != null)
            {
                string errorMessage = XmlUtility.GetElementValue(errorNode, "ErrorSeverity", string.Empty) + "(" + XmlUtility.GetElementValue(errorNode, "ErrorCode", responseCode.ToString()) + "): " + XmlUtility.GetElementValue(errorNode, "ErrorDescription", "Unknown Error");
                throw new ShipProviderException(errorMessage);
            }
            throw new ShipProviderException();
        }

        private static XmlDocument SendRequestToProviderWithoutCredentials(XmlDocument request, string url)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }

            byte[] requestBytes = System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(request.OuterXml);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            //READ RESPONSEDATA FROM STREAM
            string responseData;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.GetEncoding("iso-8859-1")))
            {
                responseData = responseStream.ReadToEnd();
                responseStream.Close();
            }

            //LOAD RESPONSE INTO XML DOCUMENT
            XmlDocument providerResponse = new XmlDocument();
            providerResponse.LoadXml(responseData);

            //RETURN THE RESPONSE DOCUMENT
            return providerResponse;
        }

        //THE INSTANCE VERSION CAN SEND ACCESS CREDENTIALS WITH REQUEST
        private XmlDocument SendRequestToProvider(XmlDocument request, string url)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }

            //RECORD SEND HERE
            if (this.UseDebugMode) this.RecordCommunication("UPS", CommunicationDirection.Send, request.OuterXml);

            XmlDocument credentials = new XmlDocument();
            credentials.LoadXml("<?xml version=\"1.0\"?><AccessRequest><AccessLicenseNumber>" + this.AccessKey + "</AccessLicenseNumber><UserId>" + this.UserId + "</UserId><Password>" + this.Password + "</Password></AccessRequest>");
            byte[] requestBytes = System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(credentials.OuterXml + request.OuterXml);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            //READ RESPONSEDATA FROM STREAM
            string responseData;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.GetEncoding("iso-8859-1")))
            {
                responseData = responseStream.ReadToEnd();
                responseStream.Close();
            }

            //LOAD RESPONSE INTO XML DOCUMENT
            XmlDocument providerResponse = new XmlDocument();
            providerResponse.LoadXml(responseData);

            //RECORD RECEIVE HERE
            if (this.UseDebugMode) this.RecordCommunication("UPS", CommunicationDirection.Receive, responseData);

            //RETURN THE RESPONSE DOCUMENT
            return providerResponse;
        }

        public override string Name
        {
            get { return "UPS OnLine® Tools"; }
        }

        public override string Version
        {
            get { return "v6.0.0.0"; }
        }
        
        /*public override TrackingSummary GetTrackingSummary(TrackingNumber trackingNumber)
        {
            //BUILD THE REQUEST FOR THIS SHIPMENT
            XmlDocument providerRequest = BuildTrackingRequest(trackingNumber);
            //SEND THIS REQUEST TO THE PROVIDER
            XmlDocument providerResponse = SendRequestToProvider(providerRequest, GetEffectiveUrl("Track"));
            //PARSE THE THE RATE QUOTES FROM THE RESPONSE
            TrackingSummary summary = ParseTrackingResponse(providerResponse, trackingNumber);
            //CACHE THE RATE QUOTES INTO REQUEST
            return summary;
        }*/

        public override TrackingSummary GetTrackingSummary(TrackingNumber trackingNumber)
        {
            TrackingSummary summary = new TrackingSummary();
            summary.TrackingResultType = TrackingResultType.ExternalLink;            
            summary.TrackingLink = string.Format(TrackingUrl, HttpUtility.UrlEncode(trackingNumber.TrackingNumberData));
            return summary;
        }

        public override ShipRateQuote GetShipRateQuote(Warehouse origin, CommerceBuilder.Users.Address destination, BasketItemCollection contents, string serviceCode)
        {
            //GET THE SHIP QUOTE FOR THE GIVEN SERVICE
            //GET THE RATE FOR ALL SERVICES
            ShipRateQuote quote = null;
            Dictionary<String, ProviderShipRateQuote> allProviderQuotes = GetAllProviderShipRateQuotes(origin, destination, contents);
            if ((allProviderQuotes != null) && allProviderQuotes.ContainsKey(serviceCode))
            {
                ProviderShipRateQuote providerQuote = allProviderQuotes[serviceCode];
                quote = new ShipRateQuote();                
                quote.Rate = providerQuote.Rate;
            }
            return quote;
        }


        private static Dictionary<string, string> _services;
        static UPS()
        {
            _services = new Dictionary<string, string>();
            _services.Add("01", "UPS Next Day Air®");
            _services.Add("02", "UPS 2nd Day Air®");
            _services.Add("03", "UPS Ground");
            _services.Add("07", "UPS Worldwide Express(SM)");
            _services.Add("08", "UPS Worldwide Expedited(SM)");
            _services.Add("11", "UPS Standard");
            _services.Add("12", "UPS 3-Day Select®");
            _services.Add("13", "UPS Next Day Air Saver®");
            _services.Add("14", "UPS Next Day Air® Early AM®");
            _services.Add("54", "UPS WorldWide Express Plus(SM)");
            _services.Add("59", "UPS 2nd Day Air AM®");
            _services.Add("65", "UPS Saver");
            //POLAND DOMESTIC ONLY 82 - 86
            _services.Add("82", "UPS Today Standard(SM)");
            _services.Add("83", "UPS Today Dedicated Courrier(SM)");
            _services.Add("84", "UPS Today Intercity");
            _services.Add("85", "UPS Today Express");
            _services.Add("86", "UPS Today Express Saver");
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

        private Dictionary<string, ProviderShipRateQuote> GetAllProviderShipRateQuotes(Warehouse origin, Address destination, BasketItemCollection contents)
        {
            //CHECK CACHE FOR QUOTES FOR THIS SHIPMENT
            string cacheKey = StringHelper.CalculateMD5Hash(Utility.Misc.GetClassId(this.GetType()) + "_" + origin.WarehouseId.ToString() + "_" + destination.AddressId.ToString() + "_" + contents.GenerateContentHash());
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                context.Trace.Write("UPS.GetAllProviderShipRateQuotes", "Check cache for " + cacheKey);
                if (context.Items.Contains(cacheKey))
                {
                    context.Trace.Write("UPS.GetAllProviderShipRateQuotes", "Returned cached shipping rates.");
                    return (Dictionary<string, ProviderShipRateQuote>)context.Items[cacheKey];
                }
            }

            //VERIFY WE HAVE A DESTINATION COUNTRY
            if (string.IsNullOrEmpty(destination.CountryCode)) return null;
            //VERIFY ITS NOT A POST OFFICE BOX ADDRESS
            if (destination.CountryCode.Trim().ToUpper() == "US" && IsPostOfficeBoxAddress(destination)) return null;

            //BUILD A LIST OF USPS PACKAGES FROM THE SHIPMENT ITEMS
            PackageList plist = PreparePackages(origin, contents);
            if (plist == null || plist.Count == 0) return null;
            Package[] shipmentPackages = plist.ToArray();
            if (shipmentPackages == null || shipmentPackages.Length == 0) return null;
            
            //BUILD THE REQUEST FOR THIS SHIPMENT
            XmlDocument providerRequest = BuildProviderRequest(shipmentPackages, origin, destination);
            //SEND THIS REQUEST TO THE PROVIDER
            XmlDocument providerResponse = SendRequestToProvider(providerRequest, GetEffectiveUrl("Rate"));
            //PARSE THE THE RATE QUOTES FROM THE RESPONSE
            Dictionary<string, ProviderShipRateQuote> allServiceQuotes = ParseProviderRateReponse(providerResponse, shipmentPackages);
            //CACHE THE RATE QUOTES INTO REQUEST
            if (context != null)
            {
                context.Trace.Write("UPS.GetAllProviderShipRateQuotes", "Rates retrieved from gateway.");
                context.Items[cacheKey] = allServiceQuotes;
            }
            return allServiceQuotes;
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

            //round off the measurements
            plist.RoundAll(2);

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

        private XmlDocument BuildTrackingRequest(TrackingNumber trackingNumber)
        {
            //CREATE UPS TRACKING REQUEST DOCUMENT
            XmlDocument trackingRequest = new XmlDocument();
            trackingRequest.LoadXml("<?xml version=\"1.0\"?><TrackRequest xml:lang=\"en-US\"><Request><TransactionReference><CustomerContext>Tracking Request</CustomerContext><XpciVersion>1.0001</XpciVersion></TransactionReference><RequestAction>Track</RequestAction><RequestOption>activity</RequestOption></Request><TrackingNumber /></TrackRequest>");
            XmlUtility.SetElementValue(trackingRequest.DocumentElement, "TrackingNumber", trackingNumber.TrackingNumberData.Replace(" ", string.Empty));
            return trackingRequest;
        }

        private XmlDocument BuildProviderRequest(Package[] shipmentPackages, Warehouse warehouse, Address destination)
        {
            //CREATE PROVIDER REQUEST DOCUMENT
            XmlDocument providerRequest = new XmlDocument();
            providerRequest.LoadXml("<?xml version=\"1.0\"?><RatingServiceSelectionRequest xml:lang=\"en-US\"><Request><TransactionReference><CustomerContext>Rating and Service</CustomerContext><XpciVersion>1.0001</XpciVersion></TransactionReference><RequestAction>rate</RequestAction><RequestOption>shop</RequestOption></Request><PickupType><Code /></PickupType><CustomerClassification><Code /></CustomerClassification><Shipment><Shipper><Address><City/><StateProvinceCode/><PostalCode/><CountryCode/></Address></Shipper><ShipTo><Address><City/><StateProvinceCode/><PostalCode/><CountryCode/></Address></ShipTo><ShipmentWeight><UnitOfMeasurement><Code/></UnitOfMeasurement><Weight/></ShipmentWeight></Shipment></RatingServiceSelectionRequest>");
            switch (this.CustomerType)
            {
                case UpsCustomerType.DailyPickup:
                    providerRequest.DocumentElement.SelectSingleNode("PickupType/Code").InnerText = "01";
                    providerRequest.DocumentElement.SelectSingleNode("CustomerClassification/Code").InnerText = "01";
                    break;
                case UpsCustomerType.Occasional:
                    providerRequest.DocumentElement.SelectSingleNode("PickupType/Code").InnerText = "03";
                    providerRequest.DocumentElement.SelectSingleNode("CustomerClassification/Code").InnerText = "03";
                    break;
                case UpsCustomerType.Retail:
                    providerRequest.DocumentElement.SelectSingleNode("PickupType/Code").InnerText = "11";
                    providerRequest.DocumentElement.SelectSingleNode("CustomerClassification/Code").InnerText = "04";
                    break;
            }

            //GET THE SHIPMENT NODE
            XmlElement shipmentNode = XmlUtility.GetElement(providerRequest.DocumentElement, "Shipment", true);
            if (!string.IsNullOrEmpty(this.ShipperNumber))
            {
                XmlUtility.SetElementValue(shipmentNode, "Shipper/ShipperNumber", this.ShipperNumber, true);
                XmlUtility.SetElementValue(shipmentNode, "RateInformation/NegotiatedRatesIndicator", string.Empty, true);
            }

            XmlElement tempNode = XmlUtility.GetElement(shipmentNode, "Shipper/Address", true);
            tempNode.SelectSingleNode("City").InnerText = warehouse.City;
            tempNode.SelectSingleNode("StateProvinceCode").InnerText = warehouse.Province;
            tempNode.SelectSingleNode("PostalCode").InnerText = warehouse.PostalCode;
            tempNode.SelectSingleNode("CountryCode").InnerText = warehouse.CountryCode;
            tempNode = XmlUtility.GetElement(shipmentNode, "ShipTo/Address", true);
            tempNode.SelectSingleNode("City").InnerText = destination.City;
            tempNode.SelectSingleNode("StateProvinceCode").InnerText = destination.Province;
            tempNode.SelectSingleNode("PostalCode").InnerText = destination.PostalCode;
            tempNode.SelectSingleNode("CountryCode").InnerText = destination.CountryCode;
            if (destination.Residence) tempNode.AppendChild(providerRequest.CreateElement("ResidentialAddressIndicator"));

            //COMPUTE SHIPMENT WEIGHT
            LSDecimal shipmentWeight = 0;
            foreach (Package package in shipmentPackages) { shipmentWeight += package.Weight; }
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "Shipment/ShipmentWeight/UnitOfMeasurement/Code", (shipmentPackages[0].WeightUnit == WeightUnit.Pounds) ? "LBS" : "KGS");
            XmlUtility.SetElementValue(providerRequest.DocumentElement, "Shipment/ShipmentWeight/Weight", string.Format("{0:f1}", shipmentWeight));

            //LOOP PACKAGES AND APPEND TO PROVIDER REQUEST
            foreach (Package package in shipmentPackages)
            {
                for (int i = 1; i <= package.Multiplier; i++)
                {
                    //BUILD XML PACKAGE ELEMENTS FOR RATE SHOPPING
                    XmlElement packageElement = (XmlElement)shipmentNode.AppendChild(providerRequest.CreateElement("Package"));
                    XmlUtility.SetElementValue(packageElement, "PackagingType/Code", "02");

                    //CHECK FOR DIMENSION DETAILS
                    bool hasDims = ((package.Height > 0) && (package.Width > 0) && (package.Length > 0));
                    if (hasDims)
                    {
                        XmlNode dimNode = packageElement.AppendChild(providerRequest.CreateElement("Dimensions"));
                        XmlUtility.SetElementValue(dimNode, "UnitOfMeasurement/Code", (package.MeasurementUnit == MeasurementUnit.Inches) ? "IN" : "CM");
                        XmlUtility.SetElementValue(dimNode, "Length", string.Format("{0:f2}", package.Length));
                        XmlUtility.SetElementValue(dimNode, "Width", string.Format("{0:f2}", package.Width));
                        XmlUtility.SetElementValue(dimNode, "Height", string.Format("{0:f2}", package.Height));
                    }

                    //SET CODE FOR CUSTOM PACKAGING
                    XmlUtility.SetElementValue(packageElement, "PackageWeight/UnitOfMeasurement/Code", (package.WeightUnit == WeightUnit.Pounds) ? "LBS" : "KGS");
                    XmlUtility.SetElementValue(packageElement, "PackageWeight/Weight", string.Format("{0:f1}", package.Weight));

                    //CHECK FOR OVERSIZE
                    if (hasDims)
                    {
                        //CALCULATE LENGTH PLUS GIRTH
                        //LENGTH IS LONGEST SIDE
                        //GIRTH IS 2H + 2W (OR 2(H+W))
                        LSDecimal lengthPlusGirth = package.Length + (2 * (package.Height + package.Width));
                        if ((package.Weight < 30) && (lengthPlusGirth > 84) && (lengthPlusGirth <= 108))
                        {
                            XmlUtility.SetElementValue(packageElement, "OversizePackage", "1");
                        }
                        else if ((package.Weight < 70) && (lengthPlusGirth > 108))
                        {
                            XmlUtility.SetElementValue(packageElement, "OversizePackage", "2");
                        }
                        else if ((package.Weight < 90) && (lengthPlusGirth > 130) && (lengthPlusGirth <= 165))
                        {
                            XmlUtility.SetElementValue(packageElement, "OversizePackage", "2");
                        }
                    }

                    if (this.UseInsurance)
                    {
                        //TODO: NEED CORRECT CURRENCY CODE HERE (NEED PRIMARY CURRENCY DEFINED ON STORE OBJECT)
                        XmlUtility.SetElementValue(packageElement, "PackageServiceOptions/InsuredValue/CurrencyCode", "USD");
                        XmlUtility.SetElementValue(packageElement, "PackageServiceOptions/InsuredValue/MonetaryValue", string.Format("{0:f2}", package.RetailValue));
                    }
                }
            }

            //RETURN THE REQUEST
            return providerRequest;
        }

        //attempt to parse ups date format
        private DateTime ParseDate(string dateValue, string timeValue)
        {
            if ((!string.IsNullOrEmpty(dateValue)) && (dateValue.Length == 8))
            {
                int year = int.Parse(dateValue.Substring(0, 4));
                int month = int.Parse(dateValue.Substring(4, 2));
                int day = int.Parse(dateValue.Substring(6, 2));
                if ((!string.IsNullOrEmpty(timeValue)) && (timeValue.Length == 6))
                {
                    int hour = int.Parse(timeValue.Substring(0, 2));
                    int minute = int.Parse(timeValue.Substring(2, 2));
                    int second = int.Parse(timeValue.Substring(4, 2));
                    return (new DateTime(year, month, day, hour, minute, second));
                }
                return new DateTime(year, month, day);
            }
            return System.DateTime.MinValue;
        }

        private TrackingSummary ParseTrackingResponse(XmlDocument trackingResponse, TrackingNumber trackingNumber)
        {
            int responseCode = AlwaysConvert.ToInt(XmlUtility.GetElementValue(trackingResponse.DocumentElement, "Response/ResponseStatusCode"));
            if (responseCode == 1)
            {
                //TRACKING REQUEST SUCCEEDED
                TrackingSummary summary = new TrackingSummary();
                XmlElement shipmentElement = XmlUtility.GetElement(trackingResponse.DocumentElement, "Shipment", true);
                summary.OriginAddress1 = XmlUtility.GetElementValue(shipmentElement, "Shipper/Address/Address1");
                summary.OriginCity = XmlUtility.GetElementValue(shipmentElement, "Shipper/Address/City");
                summary.OriginProvince = XmlUtility.GetElementValue(shipmentElement, "Shipper/Address/StateProvinceCode");
                summary.OriginPostalCode = XmlUtility.GetElementValue(shipmentElement, "Shipper/Address/PostalCode");
                summary.OriginCountryCode = XmlUtility.GetElementValue(shipmentElement, "Shipper/Address/CountryCode");
                summary.DestinationAddress1 = XmlUtility.GetElementValue(shipmentElement, "ShipTo/Address/Address1");
                summary.DestinationCity = XmlUtility.GetElementValue(shipmentElement, "ShipTo/Address/City");
                summary.DestinationProvince = XmlUtility.GetElementValue(shipmentElement, "ShipTo/Address/StateProvinceCode");
                summary.DestinationPostalCode = XmlUtility.GetElementValue(shipmentElement, "ShipTo/Address/PostalCode");
                summary.DestinationCountryCode = XmlUtility.GetElementValue(shipmentElement, "ShipTo/Address/CountryCode");

                //LOOP THE PACKAGES AND ADD TO TRACKING SUMMARY
                XmlNodeList packageNodeList = shipmentElement.SelectNodes("Package");
                foreach (XmlElement packageNode in packageNodeList)
                {
                    TrackingPackage package = new TrackingPackage();
                    package.StatusCode = XmlUtility.GetElementValue(packageNode, "Activity/Status/StatusType/Code").ToUpperInvariant();
                    package.StatusName = this.GetStatusName(package.StatusCode);
                    package.StatusMessage = XmlUtility.GetElementValue(packageNode, "Activity/Status/StatusType/Description");
                    package.TrackingNumber = XmlUtility.GetElementValue(packageNode, "TrackingNumber");
                    package.Weight = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue(packageNode, "PackageWeight/Weight"));
                    package.PickupDate = ParseDate(XmlUtility.GetElementValue(shipmentElement, "PickupDate"), string.Empty);
                    if (package.StatusCode.Equals("D"))
                    {
                        //JUST SET THE DELIVERED DATE
                        package.DeliveryDate = ParseDate(XmlUtility.GetElementValue(packageNode, "Activity/Date"), XmlUtility.GetElementValue(shipmentElement, "Activity/Time"));
                    }
                    else
                    {
                        //CHECK FOR RESCHEDULED DELIVERY DATE
                        package.DeliveryDate = ParseDate(XmlUtility.GetElementValue(shipmentElement, "RescheduledDeliveryDate"), XmlUtility.GetElementValue(shipmentElement, "RescheduledDeliveryTime"));
                        if (package.DeliveryDate.Equals(System.DateTime.MinValue))
                        {
                            //RESCHEDULED DELIVERY NOT FOUND, CHECK FOR SCHEDULED DELIVERY
                            package.DeliveryDate = ParseDate(XmlUtility.GetElementValue(shipmentElement, "ScheduledDeliveryDate"), XmlUtility.GetElementValue(shipmentElement, "ScheduledDeliveryTime"));
                        }
                    }

                    //LOOP THE ACTIVITY AND ADD TO PACKAGE
                    XmlNodeList activityNodeList = packageNode.SelectNodes("Activity");
                    foreach (XmlElement activityElement in activityNodeList)
                    {
                        TrackingActivity activity = new TrackingActivity();
                        activity.ActivityDate = ParseDate(XmlUtility.GetElementValue(activityElement, "Date"), XmlUtility.GetElementValue(activityElement, "Time"));
                        activity.City = XmlUtility.GetElementValue(activityElement, "ActivityLocation/Address/City");
                        activity.Province = XmlUtility.GetElementValue(activityElement, "ActivityLocation/Address/StateProvinceCode");
                        activity.CountryCode = XmlUtility.GetElementValue(activityElement, "ActivityLocation/Address/CountryCode");
                        activity.Status = XmlUtility.GetElementValue(activityElement, "Status/StatusType/Description");
                        activity.SignedBy = XmlUtility.GetElementValue(activityElement, "ActivityLocation/SignedForByName");
                        activity.Comment = XmlUtility.GetElementValue(activityElement, "ActivityLocation/Description");
                        package.ActivityCollection.Add(activity);
                    }
                    summary.PackageCollection.Add(package);
                }
                return summary;
            }
            return null;
        }

        private Dictionary<string, ProviderShipRateQuote> ParseProviderRateReponse(XmlDocument providerResponse, Package[] packages)
        {
            //CREATE TEMPORARY STORAGE FOR PARSING THE QUOTED SERVICES
            Dictionary<string, ProviderShipRateQuote> availableServices = new Dictionary<string, ProviderShipRateQuote>();
            ProviderShipRateQuote serviceQuote;
            int responseCode = AlwaysConvert.ToInt(XmlUtility.GetElementValue(providerResponse.DocumentElement, "Response/ResponseStatusCode"));
            if (responseCode == 1)
            {
                //LOOP EACH SERVICE IN THE RESPONSE
                XmlNodeList ratedShipmentList = providerResponse.DocumentElement.SelectNodes("RatedShipment");
                foreach (XmlNode ratedShipmentNode in ratedShipmentList)
                {
                    //CREATE A NEW INSTANCE OF ProviderShipRateQuote
                    serviceQuote = new ProviderShipRateQuote();
                    serviceQuote.ServiceCode = XmlUtility.GetElementValue((XmlElement)ratedShipmentNode, "Service/Code");
                    decimal rateValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue((XmlElement)ratedShipmentNode, "TotalCharges/MonetaryValue"));
                    //check for negotiated rate.
                    XmlNode negRatesNode = ratedShipmentNode.SelectSingleNode("NegotiatedRates");
                    if (!string.IsNullOrEmpty(this.ShipperNumber) && negRatesNode != null)
                    {
                        rateValue = AlwaysConvert.ToDecimal(XmlUtility.GetElementValue((XmlElement)negRatesNode, "NetSummaryCharges/GrandTotal/MonetaryValue"), rateValue);
                    }
                    serviceQuote.Rate = rateValue;                    
                    availableServices.Add(serviceQuote.ServiceCode, serviceQuote);
                }
            }
            return availableServices;
        }

        private string GetStatusName(string statusCode)
        {
            switch (statusCode.ToUpperInvariant())
            {
                case "I": return "In Transit";
                case "D": return "Delivered";
                case "X": return "Exception";
                case "P": return "Pickup";
                case "M": return "Manifest Pickup";
            }
            return "Unknown";
        }

        private string GetEffectiveUrl(string method)
        {
            if (this.UseTestMode)
            {
                if (TestModeUrl.EndsWith("/"))
                {
                    return TestModeUrl + method;
                }
                else
                {
                    return TestModeUrl + "/" + method;
                }
            }
            else
            {
                if (LiveModeUrl.EndsWith("/"))
                {
                    return LiveModeUrl + method;
                }
                else
                {
                    return LiveModeUrl + "/" + method;
                }                
            }
        }

        private bool IsPostOfficeBoxAddress(Address destination) 
        {
            bool isPostOfficeBoxAddress = false;
            isPostOfficeBoxAddress = (ValidationHelper.IsPostOfficeBoxAddress(destination.Address1) || ValidationHelper.IsPostOfficeBoxAddress(destination.Address2));
            return isPostOfficeBoxAddress;
        }

        struct ProviderUnits
        {
            public MeasurementUnit MeasurementUnit;
            public WeightUnit WeightUnit;
        }

        public class UpsRegistrationInformation
        {
            public string ContactName;
            public string ContactTitle;
            public string Company;
            public string CompanyUrl;
            public string Address1;
            public string Address2;
            public string Address3;
            public string City;
            public string StateProvinceCode;
            public string PostalCode;
            public string CountryCode;
            public string ContactPhone;
            public string ContactPhoneExtension;
            public string ContactEmail;
            public string ShipperNumber;
            public bool RequestContact;
        }

        public enum UpsCustomerType
        {
            DailyPickup, Occasional, Retail
        }
    }
}
