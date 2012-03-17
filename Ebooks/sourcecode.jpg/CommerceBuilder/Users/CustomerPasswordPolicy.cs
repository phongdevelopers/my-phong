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
    /// Class that represents password policy for customers
    /// </summary>
    public class CustomerPasswordPolicy : PasswordPolicy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomerPasswordPolicy() {}

        /// <summary>
        /// Minimum length of the password
        /// </summary>
        public override int MinLength
        {
            get
            {
                //VALIDATE RANGE
                int tempValue = AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordMinLength));
                if (tempValue < 1) tempValue = 1;
                return tempValue;
            }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordMinLength, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a upper case letter
        /// </summary>
        public override bool RequireUpper
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordRequireUpper), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordRequireUpper, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a lower case letter
        /// </summary>
        public override bool RequireLower
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordRequireLower), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordRequireLower, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a number
        /// </summary>
        public override bool RequireNumber
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordRequireNumber), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordRequireNumber, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a symbol or special character
        /// </summary>
        public override bool RequireSymbol
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordRequireSymbol), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordRequireSymbol, value.ToString()); }
        }

        /// <summary>
        /// Whether to require a non alphabet character
        /// </summary>
        public override bool RequireNonAlpha
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordRequireNonAlpha), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordRequireNonAlpha, value.ToString()); }
        }

        /// <summary>
        /// Maximum age of the password
        /// </summary>
        public override int MaxAge
        {
            get { return AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordMaxAge)); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordMaxAge, value.ToString()); }
        }

        /// <summary>
        /// Days password is kept in history
        /// </summary>
        public override int HistoryDays
        {
            get { return AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordHistoryDays)); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordHistoryDays, value.ToString()); }
        }

        /// <summary>
        /// Number of passwords to keep in history
        /// </summary>
        public override int HistoryCount
        {
            get { return AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordHistoryCount)); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordHistoryCount, value.ToString()); }
        }

        /// <summary>
        /// Maximum login attempts allowed
        /// </summary>
        public override int MaxAttempts
        {
            get
            {
                //VALIDATE RANGE
                int tempValue = AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordMaxAttempts));
                if (tempValue < 1) tempValue = 1;
                if (tempValue > 20) tempValue = 20;
                return tempValue;
            }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordMaxAttempts, value.ToString()); }
        }

        /// <summary>
        /// Lockup period after login attempts fail
        /// </summary>
        public override int LockoutPeriod
        {
            get
            {
                //VALIDATE RANGE
                int tempValue = AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordLockoutPeriod));
                if (tempValue < 1) tempValue = 1;
                if (tempValue > 999) tempValue = 999;
                return tempValue;
            }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordLockoutPeriod, value.ToString()); }
        }

        /// <summary>
        /// Number of months that if the account remains unused, it will be deactivated
        /// </summary>
        public override int InactivePeriod
        {
            get { return AlwaysConvert.ToInt(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordInactivePeriod)); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordInactivePeriod, value.ToString()); }
        }

        /// <summary>
        /// Whether to use image captcha verification at the time of login 
        /// </summary>
        public override bool ImageCaptcha
        {
            get { return AlwaysConvert.ToBool(this.StoreSettings.GetValueByKey(SettingKeys.CustomerPasswordImageCaptcha), false); }
            set { this.StoreSettings.SetValueByKey(SettingKeys.CustomerPasswordImageCaptcha, value.ToString()); }
        }

        /// <summary>
        /// Saves customer password policy settings
        /// </summary>
        /// <returns><b>true</b> if saved successfuly, <b>false</b> otherwise</returns>
        public override bool Save()
        {
            return this.StoreSettings.Save();
        }

        private class SettingKeys
        {
            //CUSTOMER SETTINGS
            public const string CustomerPasswordMinLength = "CustomerPasswordMinLength";
            public const string CustomerPasswordRequireUpper = "CustomerPasswordRequireUpper";
            public const string CustomerPasswordRequireLower = "CustomerPasswordRequireLower";
            public const string CustomerPasswordRequireNumber = "CustomerPasswordRequireNumber";
            public const string CustomerPasswordRequireSymbol = "CustomerPasswordRequireSymbol";
            public const string CustomerPasswordRequireNonAlpha = "CustomerPasswordRequireNonAlpha";
            public const string CustomerPasswordMaxAge = "CustomerPasswordMaxAge";
            public const string CustomerPasswordHistoryDays = "CustomerPasswordHistoryDays";
            public const string CustomerPasswordHistoryCount = "CustomerPasswordHistoryCount";
            public const string CustomerPasswordMaxAttempts = "CustomerPasswordMaxAttempts";
            public const string CustomerPasswordLockoutPeriod = "CustomerPasswordLockoutPeriod";
            public const string CustomerPasswordInactivePeriod = "CustomerPasswordInactivePeriod";
            public const string CustomerPasswordImageCaptcha = "CustomerPasswordImageCaptcha";
        }
    }
}