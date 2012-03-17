using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments
{
    public partial class PaymentGateway
    {
        /// <summary>
        /// Updates the configuration data for this payment gateway
        /// </summary>
        /// <param name="ConfigData">The configuration data to use</param>
        public void UpdateConfigData(Dictionary<string, string> ConfigData)
        {
            StringBuilder configBuilder = new StringBuilder();
            //urlencode the dictionary
            foreach (string key in ConfigData.Keys)
            {
                if (configBuilder.Length > 0) configBuilder.Append("&");
                configBuilder.Append(key + "=" + System.Web.HttpUtility.UrlEncode(ConfigData[key]));
            }
            this.ConfigData = EncryptionHelper.EncryptAES(configBuilder.ToString());
        }

        /// <summary>
        /// Parses the configuration data from a string (as saved in database) to name value pairs
        /// </summary>
        /// <returns>The parsed configuration data</returns>
        public Dictionary<string, string> ParseConfigData()
        {
            Dictionary<string, string> ConfigData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(this.ConfigData)) {
                string[] pairs = EncryptionHelper.DecryptAES(this.ConfigData).Split("&".ToCharArray());
                foreach (string thisPair in pairs)
                {
                    if (!string.IsNullOrEmpty(thisPair) && thisPair.Contains("=")) {
                        string[] ConfigDataItem = thisPair.Split("=".ToCharArray());
                        string key = ConfigDataItem[0];
                        string keyValue = System.Web.HttpUtility.UrlDecode(ConfigDataItem[1]);
                        if (!string.IsNullOrEmpty(key))
                        {
                            ConfigData.Add(key, keyValue);
                        }
                    }
                }
            }
            return ConfigData;
        }

        /// <summary>
        /// Gets the instance of the provider implementation for this payment gateway
        /// </summary>
        /// <returns>An instance of the provider implementation</returns>
        public IPaymentProvider GetInstance()
        {
            IPaymentProvider instance;
            try
            {
                instance = Activator.CreateInstance(Type.GetType(this.ClassId)) as IPaymentProvider;
            }
            catch
            {
                instance = null;
            }
            if (instance != null)
            {
                instance.Initialize(this.PaymentGatewayId, this.ParseConfigData());
            }
            return instance;
        }
    }
}
