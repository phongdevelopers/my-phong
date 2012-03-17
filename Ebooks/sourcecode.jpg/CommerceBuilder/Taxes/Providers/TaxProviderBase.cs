using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes.Providers
{
    /// <summary>
    /// Base class for tax provider implementations
    /// </summary>
    public abstract class TaxProviderBase : ITaxProvider
    {
        #region ITaxProvider Members

        private int _TaxGatewayId;

        /// <summary>
        /// The gateway Id of the tax provider. This is passed at the time of initialization.
        /// </summary>
        public int TaxGatewayId
        {
            get { return _TaxGatewayId; }
            set { _TaxGatewayId = value; }
        }

        /// <summary>
        /// The name of the tax provider implementation
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// A short description of the tax provider implementation
        /// </summary>
        public virtual string Description
        //Should have been abstract. But because it is added later 
        //we need to have a default implementation so that existing providers do not break;
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets a Url for the logo of the tax provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the logo of the tax provider implementation</returns>
        public virtual string GetLogoUrl(System.Web.UI.ClientScriptManager cs)
        //Should have been abstract. But because it is added later 
        //we need to have a default implementation so that existing providers do not break;
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets a Url for the configuration of the tax provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the configuration of the tax provider implementation</returns>
        public virtual string GetConfigUrl(System.Web.UI.ClientScriptManager cs)
        //Should have been abstract. But because it is added later 
        //we need to have a default implementation so that existing providers do not break;
        {
            return string.Empty;
        }

        /// <summary>
        /// The version of the tax provider implementation
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// Is the tax provider activated or not
        /// </summary>
        public abstract bool Activated { get;}

        /// <summary>
        /// Initialize the tax provider with the given configuration data. Called by AC at the time of initialization.
        /// </summary>
        /// <param name="taxGatewayId">The tax gateway id</param>
        /// <param name="configurationData">Configuration data in the form of name value paris</param>
        public virtual void Initialize(int taxGatewayId, Dictionary<string, string> configurationData)
        {
            this._TaxGatewayId = taxGatewayId;
        }

        /// <summary>
        /// Get the configuration data in the form on name value pairs
        /// </summary>
        /// <returns>The configuration data as name value pairs</returns>
        public virtual Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = new Dictionary<string, string>();
            return configData;
        }

        /// <summary>
        /// Cancel the taxes by this provider for the given basket
        /// </summary>
        /// <param name="basket">The basket for which to cancel the taxes</param>
        public abstract void Cancel(Basket basket);

        /// <summary>
        /// Cancel the taxes by this provider for the given order
        /// </summary>
        /// <param name="order">The order for which to cancel the taxes</param>
        public virtual void Cancel(Order order) { }

        /// <summary>
        /// Commit the taxes by this provider for the given order
        /// </summary>
        /// <param name="order">The order for which to commit the taxes</param>
        public abstract void Commit(Order order);

        /// <summary>
        /// Calculate the taxes using this provider for the given basket
        /// </summary>
        /// <param name="basket">The basket to calculate tax for</param>
        /// <returns>Total tax calculated</returns>
        public abstract LSDecimal Calculate(Basket basket);

        /// <summary>
        /// Clear out any existing taxes and then recalculate new line items.
        /// </summary>
        /// <param name="order">The order to calculate tax for</param>
        /// <returns>Total tax calculated</returns>
        public virtual LSDecimal Recalculate(Order order)
        {
            Logger.Warn("Tax recalculation is not supported by " + this.Name + ".");
            return 0;
        }

        /// <summary>
        /// Records a transaction message to the debug log.
        /// </summary>
        /// <param name="providerName">Name of the provider or gateway</param>
        /// <param name="direction">Indicates whether the data was sent or received</param>
        /// <param name="message">Content of the message</param>
        /// <param name="sensitiveData">A dictionary of key/value pairs that contains sensitive data that exists within the message (key) and the desired replacement (value).  Pass null if no replacements are required.</param>
        protected void RecordCommunication(string providerName, CommunicationDirection direction, string message, Dictionary<string, string> sensitiveData)
        {
            //CHECK FOR SENSITIVE DATA THAT MUST BE SCRUBBED
            if (sensitiveData != null)
            {
                foreach (string key in sensitiveData.Keys)
                {
                    message = message.Replace(key, sensitiveData[key]);
                }
            }
            //GET LOG DIRECTORY
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Logs\\");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string fileName = Path.Combine(directory, providerName + ".Log");
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(direction.ToString() + ": " + message);
                sw.WriteLine(string.Empty);
                sw.Close();
            }
        }

        /// <summary>
        /// Enumeration that represents the direction of communication between AC and the gateway
        /// </summary>
        public enum CommunicationDirection : int { 
            /// <summary>
            /// Sending data to the gateway
            /// </summary>
            Send,
 
            /// <summary>
            /// Receiving data from the gateway
            /// </summary>
            Receive 
        };
        #endregion
    }
}
