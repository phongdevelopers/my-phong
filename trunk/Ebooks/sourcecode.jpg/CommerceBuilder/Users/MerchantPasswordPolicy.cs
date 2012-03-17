using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// Class that represents password policy for merchants
    /// </summary>
    public class MerchantPasswordPolicy : PasswordPolicy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MerchantPasswordPolicy() { }

        /// <summary>
        /// Minimum length of the password
        /// </summary>
        public override int MinLength
        {
            get
            {
                //VALIDATE RANGE
                int tempValue = AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordMinLength));
                if (tempValue < 1) tempValue = 1;
                return tempValue;
            }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordMinLength, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a upper case letter
        /// </summary>
        public override bool RequireUpper
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordRequireUpper), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordRequireUpper, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a lower case letter
        /// </summary>
        public override bool RequireLower
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordRequireLower), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordRequireLower, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a number
        /// </summary>
        public override bool RequireNumber
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordRequireNumber), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordRequireNumber, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a symbol or special character
        /// </summary>
        public override bool RequireSymbol
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordRequireSymbol), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordRequireSymbol, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a non alphabet character
        /// </summary>
        public override bool RequireNonAlpha
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordRequireNonAlpha), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordRequireNonAlpha, value.ToString()); }
        }

        /// <summary>
        /// Maximum age of the password
        /// </summary>
        public override int MaxAge
        {
            get
            {
                //VALIDATE RANGE
                int tempValue = AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordMaxAge));
                if (tempValue < 0) tempValue = 0;
                return tempValue;
            }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordMaxAge, value.ToString()); }
        }

        /// <summary>
        /// Days password is kept in history
        /// </summary>
        public override int HistoryDays
        {
            get { return AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordHistoryDays)); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordHistoryDays, value.ToString()); }
        }

        /// <summary>
        /// Number of passwords to keep in history
        /// </summary>
        public override int HistoryCount
        {
            get { return AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordHistoryCount)); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordHistoryCount, value.ToString()); }
        }

        /// <summary>
        /// Maximum login attempts allowed
        /// </summary>
        public override int MaxAttempts
        {
            get
            {
                //VALIDATE RANGE
                int tempValue = AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordMaxAttempts));
                if (tempValue < 1) tempValue = 1;
                if (tempValue > 20) tempValue = 20;
                return tempValue;
            }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordMaxAttempts, value.ToString()); }
        }

        /// <summary>
        /// Lockup period after login attempts fail
        /// </summary>
        public override int LockoutPeriod
        {
            get
            {
                //VALIDATE RANGE
                int tempValue = AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordLockoutPeriod));
                if (tempValue < 1) tempValue = 1;
                if (tempValue > 999) tempValue = 999;
                return tempValue;
            }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordLockoutPeriod, value.ToString()); }
        }

        /// <summary>
        /// Number of months that if the account remains unused, it will be deactivated
        /// </summary>
        public override int InactivePeriod
        {
            get
            {
                //VALIDATE RANGE
                int tempValue = AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordInactivePeriod));
                if (tempValue < 1) tempValue = 1;
                if (tempValue > 24) tempValue = 24;
                return tempValue;
            }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordInactivePeriod, value.ToString()); }
        }

        /// <summary>
        /// Whether to use image captcha verification at the time of login 
        /// </summary>
        public override bool ImageCaptcha
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.MerchantPasswordImageCaptcha), true); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.MerchantPasswordImageCaptcha, value.ToString()); }
        }

        /// <summary>
        /// Saves merchant password policy settings
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            return this.StoreSettings.Save();
        }

        private class SettingKeys
        {
            //MERCHANT SETTINGS
            public const string MerchantPasswordMinLength = "MerchantPasswordMinLength";
            public const string MerchantPasswordRequireUpper = "MerchantPasswordRequireUpper";
            public const string MerchantPasswordRequireLower = "MerchantPasswordRequireLower";
            public const string MerchantPasswordRequireNumber = "MerchantPasswordRequireNumber";
            public const string MerchantPasswordRequireSymbol = "MerchantPasswordRequireSymbol";
            public const string MerchantPasswordRequireNonAlpha = "MerchantPasswordRequireNonAlpha";
            public const string MerchantPasswordMaxAge = "MerchantPasswordMaxAge";
            public const string MerchantPasswordHistoryDays = "MerchantPasswordHistoryDays";
            public const string MerchantPasswordHistoryCount = "MerchantPasswordHistoryCount";
            public const string MerchantPasswordMaxAttempts = "MerchantPasswordMaxAttempts";
            public const string MerchantPasswordLockoutPeriod = "MerchantPasswordLockoutPeriod";
            public const string MerchantPasswordInactivePeriod = "MerchantPasswordInactivePeriod";
            public const string MerchantPasswordImageCaptcha = "MerchantPasswordImageCaptcha";
        }
    }
}