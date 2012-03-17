namespace CommerceBuilder.Users
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;
    using CommerceBuilder.Utility;

    /// <summary>
    /// Collection class that contains user settings.
    /// </summary>
    public partial class UserSettingCollection
    {
        Hashtable settingMap = new Hashtable();

        /// <summary>
        /// Add a UserSetting object to the collection
        /// </summary>
        /// <param name="item">The UserSetting object to add</param>
        public new void Add(UserSetting item)
        {
            settingMap[item.FieldName] = item;
            base.Add(item);
        }

        /// <summary>
        /// Gets a setting value for given key
        /// </summary>
        /// <param name="key">The key for which to get the setting value</param>
        /// <returns>The setting value for the given key</returns>
        public string GetValueByKey(string key)
        {
            if (settingMap.ContainsKey(key)) return ((UserSetting)settingMap[key]).FieldValue;
            return string.Empty;
        }

        /// <summary>
        /// Sets the setting value for a given key
        /// </summary>
        /// <param name="key">The key for which to set the value</param>
        /// <param name="value">The value to set</param>
        public void SetValueByKey(string key, string value)
        {
            UserSetting setting = (UserSetting)settingMap[key];
            if (setting == null) setting = new UserSetting();
            setting.FieldName = key;
            setting.FieldValue = value;
            this.Add(setting);
        }

        /// <summary>
        /// Gets the CurrencyId of the user's selected currency
        /// </summary>
        public int UserCurrencyId
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.UserCurrencyId)); }
            set { SetValueByKey(SettingKeys.UserCurrencyId, value.ToString()); }
        }
        
        private class SettingKeys
        {
            private SettingKeys() { }
            public const string UserCurrencyId = "UserCurrencyId";
        }
    }
}
