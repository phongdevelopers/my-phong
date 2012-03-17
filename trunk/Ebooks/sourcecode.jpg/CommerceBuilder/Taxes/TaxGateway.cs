using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// Class that represents a TaxGateway object in database
    /// </summary>
    public partial class TaxGateway
    {

        /// <summary>
        /// Update the configuration data for this TaxGateway
        /// </summary>
        /// <param name="ConfigData">The cofiguration data for updating</param>
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
            if (!string.IsNullOrEmpty(this.ConfigData))
            {
                string[] pairs = EncryptionHelper.DecryptAES(this.ConfigData).Split("&".ToCharArray());
                foreach (string thisPair in pairs)
                {
                    if (!string.IsNullOrEmpty(thisPair) && thisPair.Contains("="))
                    {
                        string[] ConfigDataItem = thisPair.Split("=".ToCharArray());
                        string key = ConfigDataItem[0];                        
                        if (!string.IsNullOrEmpty(key) & ConfigDataItem.Length > 1)
                        {
                            string keyValue = System.Web.HttpUtility.UrlDecode(ConfigDataItem[1]);
                            ConfigData.Add(key, keyValue);
                        }
                    }
                }
            }
            return ConfigData;
        }


        /// <summary>
        /// Creates an instance of the provider implementation associated with this gateway object
        /// </summary>
        /// <returns>An object instance that implements the ITaxProvider interface or null if provider instance could not be obtained.</returns>
        public ITaxProvider GetProviderInstance()
        {
            ITaxProvider instance;
            try
            {
                instance = Activator.CreateInstance(Type.GetType(this.ClassId)) as ITaxProvider;
            }
            catch
            {
                instance = null;
            }
            if (instance != null)
            {
                instance.Initialize(this.TaxGatewayId, this.ParseConfigData());
            }
            return instance;
        }
    }
}
