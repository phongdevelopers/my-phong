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
using System.Text;

namespace CommerceBuilder.Shipping.Providers.AustraliaPost
{
    public class AustraliaPost : ShippingProviderBase
    {
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

        //Max package weight in Kgs
        private LSDecimal _MaxPackageWeight = 20;
        public LSDecimal MaxPackageWeight
        {
            get { return _MaxPackageWeight; }
            set { _MaxPackageWeight = value; }
        }

        private bool _AccountActive = false;
        public bool AccountActive
        {
            get { return _AccountActive; }
            set { _AccountActive = value; }
        }

        private string _LiveModeUrl = "http://drc.edeliver.com.au/rateCalc.asp";
        public string LiveModeUrl
        {
            get { return _LiveModeUrl; }
            set { _LiveModeUrl = value; }
        }
                
        //No tracking URL
        public string TrackingUrl
        {
            get
            {
                return string.Empty;
            }
        }

        public override string Name
        {
            get { return "AustraliaPost"; }
        }

        public override string Version
        {
            get { return "AustraliaPost eDeliver Delivery Rate Calculator Sep2006"; }
        }

        private static Dictionary<string, string> _services;
        static AustraliaPost()
        {
            _services = new Dictionary<string, string>();
            _services.Add("STANDARD", "Domestic Parcel Post");
            _services.Add("EXPRESS", "Domestic Express Parcel Post");
            _services.Add("SEA", "International Sea Parcel Post");
            _services.Add("AIR", "International Air Parcel Post");
            _services.Add("EPI", "International Express Parcel Post");
            _services.Add("ECI_D", "International Express Courier - Document");
            _services.Add("ECI_M", "International Express Courier - Parcel");
        }

        private bool IsDomesticService(string serviceCode)
        {
            return serviceCode.Equals("EXPRESS") || serviceCode.Equals("STANDARD");
        }

        #region Interface_Implementation_methods

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Shipping.Providers.AustraliaPost.Logo.gif");
            return string.Empty;
        }

        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "AustraliaPost/Default.aspx";
        }

        public override string Description
        {
            get { return "The AustraliaPost module can generate shipping rate estimates for your packages. It does not support tracking."; }
        }

        public override void Initialize(int ShipGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(ShipGatewayId, ConfigurationData);
            //INITIALIZE MY FIELDS
            if (ConfigurationData.ContainsKey("EnablePackageBreakup")) EnablePackageBreakup = AlwaysConvert.ToBool(ConfigurationData["EnablePackageBreakup"], true);
            if (ConfigurationData.ContainsKey("AccountActive")) AccountActive = AlwaysConvert.ToBool(ConfigurationData["AccountActive"], false);
            if (ConfigurationData.ContainsKey("MinPackageWeight")) MinPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MinPackageWeight"], (decimal)MinPackageWeight);
            if (ConfigurationData.ContainsKey("MaxPackageWeight")) MaxPackageWeight = AlwaysConvert.ToDecimal(ConfigurationData["MaxPackageWeight"], (decimal)MaxPackageWeight);
        }

        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData.Add("EnablePackageBreakup", this.EnablePackageBreakup.ToString());
            configData.Add("AccountActive", this.AccountActive.ToString());
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
            //VERIFY WE HAVE A DESTINATION COUNTRY
            if (string.IsNullOrEmpty(destination.CountryCode)) return null;

            //VERIFY THAT ORIGIN COUNTRY IS AU
            if (string.IsNullOrEmpty(origin.CountryCode) || !origin.CountryCode.Equals("AU")) return null;

            if (destination.CountryCode.Equals("AU"))
            {
                //if destination is also AU only domestic services are applicable
                if (!IsDomesticService(serviceCode)) return null;
            }
            else
            {
                //only international services are available
                if (IsDomesticService(serviceCode)) return null;
            }

            PackageList plist = PreparePackages(origin, contents);
            if (plist == null || plist.Count == 0) return null;

            ProviderShipRateQuote providerQuote = null;
            ProviderShipRateQuote tempQuote;

            foreach (Package item in plist)
            {
                tempQuote = GetProviderQuote(origin, destination, item, serviceCode);
                if (providerQuote == null) providerQuote = tempQuote;
                else providerQuote.AddPackageQoute(tempQuote);
            }

            if (providerQuote != null && providerQuote.PackageCount < plist.Count) return null;
            return providerQuote;
        }

        public override TrackingSummary GetTrackingSummary(TrackingNumber trackingNumber)
        {
            throw new NotImplementedException("Tracking Support Not Available");
        }

        #endregion Interface_Implementation_methods

        #region Implementation_Support_methods

        private ProviderShipRateQuote GetProviderQuote(Warehouse origin, Address destination, Package package, string serviceCode)
        {
            string providerRequest = BuildProviderRequest(origin, destination, package, serviceCode);

            if (this.UseDebugMode)
            {
               this.RecordCommunication("AustraliaPost", CommunicationDirection.Send, providerRequest);
            }

            string providerResponse = SendRequestToProvider(providerRequest, this.LiveModeUrl, "iso-8859-1");

            if (this.UseDebugMode)
            {
               this.RecordCommunication("AustraliaPost", CommunicationDirection.Receive, providerResponse);
            }
                        
            return ParseProviderResponse(providerResponse, destination, serviceCode);
        }

        private ProviderShipRateQuote ParseProviderResponse(string providerResponse, Address destination, string serviceCode)
        {
            ProviderShipRateQuote quote = new ProviderShipRateQuote();
            if (string.IsNullOrEmpty(providerResponse)) return null;
            StringReader sr = new StringReader(providerResponse);
            string chargeLine = sr.ReadLine();
            string days = sr.ReadLine();
            string errmsg = sr.ReadLine();
            if (!string.IsNullOrEmpty(errmsg) && errmsg.EndsWith("OK"))
            {
                int index;
                if (!string.IsNullOrEmpty(chargeLine))
                {
                    index = chargeLine.IndexOf("charge=");                    
                    quote.Rate = AlwaysConvert.ToDecimal(chargeLine.Substring(index + 7));
                    quote.PackageCount = 1;
                    quote.ServiceCode = serviceCode;
                    quote.ServiceName = GetServiceName(serviceCode);
                }
            }
            return quote;
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

        private string BuildProviderRequest(Warehouse warehouse, Address destination, Package package, string serviceCode)
        {
            //WEIGHT IN GRAMS
            int weight = (int)(package.Weight);
            if (weight == 0) weight = 100;
            //NEED DIMENSIONS IN MILLIMETERS, DEFAULTS TO 20
            int length = (int)(package.Length * 10);
            if (length == 0) length = 100;
            int width = (int)(package.Width * 10);
            if (width == 0) width = 100;
            int height = (int)(package.Height * 10);
            if (height == 0) height = 100;
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Pickup_Postcode={0}&Destination_Postcode={1}&Country={2}", warehouse.PostalCode, destination.PostalCode, destination.CountryCode));
            sb.Append(string.Format("&Service_Type={0}&Weight={1}", serviceCode, weight));
            sb.Append(string.Format("&Length={0}&Width={1}", length, width));
            sb.Append(string.Format("&Height={0}&Quantity={1}", height, package.Multiplier));
            return sb.ToString();
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

            return plist;
        }

        private ProviderUnits GetProviderUnits(Country originCountry)
        {
            ProviderUnits pUnits = new ProviderUnits();
            pUnits.MeasurementUnit = MeasurementUnit.Centimeters;
            pUnits.WeightUnit = WeightUnit.Grams;
            return pUnits;
        }

        #endregion Implementation_Support_methods

        #region Configuration_Support_Methods

        public bool IsActive
        {
            get { return this.AccountActive; }
        }

        #endregion Configuration_Support_Methods

        struct ProviderUnits
        {
            public MeasurementUnit MeasurementUnit;
            public WeightUnit WeightUnit;
        }

    }
}
