using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments.Providers.PayPal
{
    public class ExpressCheckoutSession
    {
        private string _Token;
        private DateTime _TokenExpiration;
        private string _PayerID;
        private string _Payer;

        public string Token
        {
            get { return _Token; }
            set { _Token = value; }
        }
 
        public DateTime TokenExpiration
        {
            get { return _TokenExpiration; }
            set { _TokenExpiration = value; }
        }

        public string PayerID
        {
            get { return _PayerID; }
            set { _PayerID = value; }
        }

        public string Payer
        {
            get { return _Payer; }
            set { _Payer = value; }
        }

        /// <summary>
        /// Saves the express checkout data to persistent session state.
        /// </summary>
        public void Save()
        {
            CommerceBuilder.Common.Token acToken = CommerceBuilder.Common.Token.Instance;
            Save(acToken.User);
        }

        /// <summary>
        /// Saves the express checkout data to persistent session state.
        /// </summary>
        /// <param name="user">The user to associate with the express checkout session</param>
        public void Save(User user)
        {
            UserSettingCollection acUserSettings = user.Settings;
            SetPayPalSettings(acUserSettings, this.Token, this.TokenExpiration, this.PayerID, this.Payer, user.UserId);
            WebTrace.Write("Saved token for user " + user.UserId);
        }

        private static void RemovePayPalSettings(UserSettingCollection settings)
        {
            for (int i = settings.Count - 1; i >= 0; i--)
            {
                if (settings[i].FieldName.StartsWith("PayPal_"))
                    settings.DeleteAt(i);
            }
        }

        private static void SetPayPalSettings(UserSettingCollection settings, string token, DateTime expiration, string payerId, string payer, int userId)
        {
            RemovePayPalSettings(settings);
            if (!string.IsNullOrEmpty(token)) settings.SetValueByKey("PayPal_Token", token);
            if (expiration > System.DateTime.MinValue) settings.SetValueByKey("PayPal_TokenExpiration", expiration.ToString());
            if (!string.IsNullOrEmpty(payerId)) settings.SetValueByKey("PayPal_PayerID", payerId);
            if (!string.IsNullOrEmpty(payer)) settings.SetValueByKey("PayPal_Payer", payer);
            foreach (UserSetting userSetting in settings) { userSetting.UserId = userId; userSetting.IsDirty = true; }
            settings.Save();
        }

        /// <summary>
        /// Deletes the express checkout data from persistent session state.
        /// </summary>
        public void Delete()
        {
            CommerceBuilder.Common.Token acToken = CommerceBuilder.Common.Token.Instance;
            ExpressCheckoutSession.Delete(acToken.User);
        }

        /// <summary>
        /// Deletes the express checkout data from persistent session state.
        /// </summary>
        /// <param name="user">The user to delete express checkout session for</param>
        public static void Delete(User user)
        {
            UserSettingCollection acUserSettings = user.Settings;
            RemovePayPalSettings(acUserSettings);
            WebTrace.Write("Deleted token for user " + user.UserId);
        }

        /// <summary>
        /// Gets the express checkout session for the current user
        /// </summary>
        public static ExpressCheckoutSession Current
        {
            get
            {
                CommerceBuilder.Common.Token acToken = CommerceBuilder.Common.Token.Instance;
                User acUser = acToken.User;
                WebTrace.Write("Load paypal settings for user id " + acUser.UserId.ToString());
                UserSettingCollection acUserSettings = acUser.Settings;
                string token = acUserSettings.GetValueByKey("PayPal_Token");
                //CHECK FOR MISSING TOKEN
                if (string.IsNullOrEmpty(token)) return null;
                string tokenExpirationString = acUserSettings.GetValueByKey("PayPal_TokenExpiration");
                DateTime tokenExpiration = AlwaysConvert.ToDateTime(tokenExpirationString, System.DateTime.MinValue);
                //CHECK FOR EXPIRED TOKEN
                if (tokenExpiration <= DateTime.UtcNow) return null;
                ExpressCheckoutSession currentSession = new ExpressCheckoutSession();
                currentSession.Token = token;
                currentSession.TokenExpiration = tokenExpiration;
                currentSession.PayerID = acUserSettings.GetValueByKey("PayPal_PayerID");
                currentSession.Payer = acUserSettings.GetValueByKey("PayPal_Payer");
                return currentSession;
            }
        }
    }
}
