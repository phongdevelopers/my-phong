using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.Util
{
    public abstract class BasicAuthenticationHandler : IHttpHandler
    {
        private GoogleCheckout _GatewayInstance = null;
        public GoogleCheckout GatewayInstance
        {
            get
            {
                if (_GatewayInstance != null)
                {
                    return _GatewayInstance;
                }
                else
                {
                    _GatewayInstance = GoogleCheckout.GetInstance();
                    return _GatewayInstance;
                }
            }
        }


        /// <summary>
        /// Gets the name of the user from the "Authorization" HTTP header. That header has
        /// user name and password (as typed by the user) in a Base64-encoded form.
        /// </summary>
        /// <param name="AuthHeader">
        /// The value of the "Authorization" HTTP header.
        /// </param>
        /// <returns>The name of the user as typed by the user in the browser.</returns>
        public static string GetUserName(string AuthHeader)
        {
            return GetDecodedAndSplitAuthorizatonHeader(AuthHeader)[0];
        }

        /// <summary>
        /// Gets the password from the "Authorization" HTTP header. That header has
        /// user name and password (as typed by the user) in a Base64-encoded form.
        /// </summary>
        /// <param name="AuthHeader">
        /// The value of the "Authorization" HTTP header.
        /// </param>
        /// <returns>The password as typed by the user in the browser.</returns>
        public static string GetPassword(string AuthHeader)
        {
            return GetDecodedAndSplitAuthorizatonHeader(AuthHeader)[1];
        }

        private static string[] GetDecodedAndSplitAuthorizatonHeader(
          string AuthHeader)
        {
            string[] RetVal = new string[2] { "", "" };
            if (AuthHeader != null && AuthHeader.StartsWith("Basic "))
            {
                try
                {
                    string EncodedString = AuthHeader.Substring(6);
                    byte[] DecodedBytes = Convert.FromBase64String(EncodedString);
                    string DecodedString = new ASCIIEncoding().GetString(DecodedBytes);
                    RetVal = DecodedString.Split(new char[] { ':' });
                }
                catch
                {
                }
            }
            return RetVal;
        }

        public virtual bool UserHasAccess(string UserName, string Password)
        {
            if (GatewayInstance == null)
            {
                throw new ApplicationException("Google Checkout is not configured.");
            }
            else
            {
                //IF BASIC AUTH IS DISABLED, USER HAS ACCESS
                if (!GatewayInstance.UseBasicAuth) return true;

                string ID = GatewayInstance.MerchantID;
                if (string.IsNullOrEmpty(ID))
                    throw new ApplicationException("Set the 'Google Merchant ID' in Google Checkout Configration.");

                string Key = GatewayInstance.MerchantKey;
                if (string.IsNullOrEmpty(Key))
                    throw new ApplicationException("Set the 'Google Merchant Key' in Google Checkout Configration.");

                return (UserName == ID && Password == Key);
            }
        }

        public abstract bool IsReusable
        {
            get;
        }

        public abstract void ProcessRequest(HttpContext context);
        

    }
}
