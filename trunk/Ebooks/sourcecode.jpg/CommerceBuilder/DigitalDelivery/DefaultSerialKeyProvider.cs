using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.DigitalDelivery
{
   /// <summary>
   /// Default implementation of serial key provider for digital goods
   /// </summary>
    public class DefaultSerialKeyProvider : SerialKeyProviderBase
    {
        /// <summary>
        /// Name of the implementation
        /// </summary>
        public override string Name
        {
            get
            {
                return "Default Serial Key Provider";
            }
        }

        /// <summary>
        /// Implementation version
        /// </summary>
        public override string Version
        {
            get
            {
                return "1.0";
            }
        }

        /// <summary>
        /// Description of implementation
        /// </summary>
        public override string Description
        { 
            get
            {
                return "Default Serial Key Provider";
            }
        }

        /// <summary>
        /// The configuration URL where the provider can be configured
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>The configuration URL where the provider can be configured</returns>
        public override string GetConfigUrl(System.Web.UI.ClientScriptManager cs)
        {   
            return "DefaultProvider/AddKeys.aspx";
        }

        /// <summary>
        /// Acquires a new serial key from provider
        /// </summary>
        /// <returns>Serial key acquisition response</returns>
        public override AcquireSerialKeyResponse AcquireSerialKey()
        {
            AcquireSerialKeyResponse response = new AcquireSerialKeyResponse();
            if (DigitalGood_.SerialKeys.Count > 0)
            {
                SerialKey key = DigitalGood_.SerialKeys[0];
                DigitalGood_.SerialKeys.RemoveAt(0);         
                string keyData = key.SerialKeyData;
                key.Delete();
                response.Successful = true;
                response.SerialKey = keyData;
                return response;
            }
            else
            {
                response.ErrorMessage = "There are no serial keys available for the digital good '" + DigitalGood_.Name + "'";
                return response;
            }
        }

        /// <summary>
        /// Returns a serial key back to the provider store
        /// </summary>
        /// <param name="keyData">The key to return</param>
        public override void ReturnSerialKey(string keyData)
        {
            if (!string.IsNullOrEmpty(keyData))
            {
                SerialKey skey = new SerialKey();
                skey.SerialKeyData = keyData;
                skey.DigitalGoodId = DigitalGood_.DigitalGoodId;
                skey.Save();
            }
        }
    }
}
