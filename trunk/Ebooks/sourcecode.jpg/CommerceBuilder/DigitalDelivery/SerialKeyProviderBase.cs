using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Orders;

namespace CommerceBuilder.DigitalDelivery
{
    /// <summary>
    /// Base class for SerialKeyProvider implementations
    /// </summary>
    public abstract class SerialKeyProviderBase : ISerialKeyProvider
    {
        /// <summary>
        /// Digital Good Object
        /// </summary>
        protected DigitalGood DigitalGood_;
        /// <summary>
        /// The digital good to to provide serial keys for
        /// </summary>
        /// <returns></returns>
        public DigitalGood GetDigitalGood() 
        {
            return DigitalGood_;
        }

        /// <summary>
        /// Initializes the configuration provider.
        /// </summary>
        /// <param name="digitalGood">The digital good</param>
        /// <param name="configurationData">The configuration data</param>
        public virtual void Initialize(DigitalGood digitalGood, Dictionary<string, string> configurationData)
        {
            this.DigitalGood_ = digitalGood;
        }

        /// <summary>
        /// The configuration data
        /// </summary>
        /// <returns>The configuration data</returns>
        public virtual Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = new Dictionary<string, string>();            
            return configData;
        }

        /// <summary>
        /// Name of the provider implementation
        /// </summary>
        public abstract string Name { get;}

        /// <summary>
        /// Implementation version
        /// </summary>
        public abstract string Version { get;}

        /// <summary>
        /// A description of the provider implementation
        /// </summary>
        public abstract string Description { get;}

        /// <summary>
        /// The configuration URL where the provider can be configured
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>The configuration URL where the provider can be configured</returns>
        public abstract string GetConfigUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Acquires a new serial key from provider
        /// </summary>
        /// <returns>Serial key acquisition response</returns>
        public abstract AcquireSerialKeyResponse AcquireSerialKey();

        /// <summary>
        /// Acquires a new serial key from provider
        /// </summary>
        /// <param name="oidg">OrderItemDigitalGood for which to acquire the key</param>
        /// <returns>Serial key acquisition response</returns>
        public virtual AcquireSerialKeyResponse AcquireSerialKey(OrderItemDigitalGood oidg)
        {
            return AcquireSerialKey();
        }

        /// <summary>
        /// Returns a serial key back to the provider store
        /// </summary>
        /// <param name="key">The key to return</param>
        public abstract void ReturnSerialKey(string key);

        /// <summary>
        /// Returns a serial key back to the provider store
        /// </summary>
        /// <param name="keyData">The key to return</param>
        /// <param name="oidg">OrderItemDigitalGood the returned key is associated to</param>
        public virtual void ReturnSerialKey(string keyData, OrderItemDigitalGood oidg)
        {
            ReturnSerialKey(keyData);
        }

    }
}
