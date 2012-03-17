namespace CommerceBuilder.Stores
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;
    using CommerceBuilder.Utility;
    using CommerceBuilder.Users;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Collection of store settings
    /// </summary>
    public partial class StoreSettingCollection
    {
        Hashtable settingMap = new Hashtable();

        /// <summary>
        /// Add a new store setting to this collection
        /// </summary>
        /// <param name="item">The store setting to add</param>
        public new void Add(StoreSetting item)
        {
            settingMap[item.FieldName] = item;
            base.Add(item);
        }

        /// <summary>
        /// Save this collection
        /// </summary>
        /// <returns><b>true</b> if save successful, <b>false</b> otherwise</returns>
        public override bool Save()
        {
            bool result = base.Save();
            Store.ClearCachedSettings();
            return result;
        }

        /// <summary>
        /// Save this collection for the given store
        /// </summary>
        /// <param name="storeId">Id of the store to save this collection for</param>
        /// <returns><b>true</b> if save successful, <b>false</b> otherwise</returns>
        public bool Save(int storeId)
        {
            foreach (StoreSetting setting in this)
            {
                setting.StoreId = storeId;
            }
            return this.Save();
        }

        /// <summary>
        /// Gets the value of a setting for the given key
        /// </summary>
        /// <param name="key">Key for which to get the value</param>
        /// <returns>Value of the setting for the given key</returns>
        public string GetValueByKey(string key)
        {
            return GetValueByKey(key, string.Empty);
        }

        /// <summary>
        /// Gets the value of a setting for the given key
        /// </summary>
        /// <param name="key">Key for which to get the value</param>
        /// <param name="defaultValue">Default value to return if the keyed item is not present</param>
        /// <returns>Value of the setting for the given key</returns>
        public string GetValueByKey(string key, string defaultValue)
        {
            if (settingMap.ContainsKey(key)) return ((StoreSetting)settingMap[key]).FieldValue;
            return defaultValue;
        }

        /// <summary>
        /// Sets the value of a setting for given key
        /// </summary>
        /// <param name="key">The key for which to set the value</param>
        /// <param name="value">The value to set</param>
        public void SetValueByKey(string key, string value)
        {
            StoreSetting setting = (StoreSetting)settingMap[key];
            if (setting == null) setting = new StoreSetting();
            setting.FieldName = key;
            setting.FieldValue = value;
            this.Add(setting);
        }

        #region "Standard Settings"

        /// <summary>
        /// Password format setting
        /// </summary>
        public string PasswordFormat
        {
            get
            {
                string passwordFormat = GetValueByKey(SettingKeys.PasswordFormat);
                if (string.IsNullOrEmpty(passwordFormat)) passwordFormat = "SHA1";
                else
                {
                    string[] formats = { "CLEAR", "MD5", "SHA1", "SHA256", "SHA384", "SHA512", "AES", "3DES" };
                    if (Array.IndexOf(formats, passwordFormat.ToUpperInvariant()) < 0) passwordFormat = "SHA1";
                    else passwordFormat = passwordFormat.ToUpperInvariant();
                }
                return passwordFormat;
            }
            set
            {
                string passwordFormat;
                if (string.IsNullOrEmpty(value)) passwordFormat = "SHA1";
                else
                {
                    string[] formats = { "CLEAR", "MD5", "SHA1", "SHA256", "SHA384", "SHA512", "AES", "3DES" };
                    if (Array.IndexOf(formats, value.ToUpperInvariant()) < 0) passwordFormat = "SHA1";
                    else passwordFormat = value.ToUpperInvariant();
                }
                SetValueByKey(SettingKeys.PasswordFormat, passwordFormat);
            }
        }

        /// <summary>
        /// Store Theme setting
        /// </summary>
        public string StoreTheme
        {
            get { return GetValueByKey(SettingKeys.StoreTheme); }
            set { SetValueByKey(SettingKeys.StoreTheme, value); }
        }

        /// <summary>
        /// Admin Theme setting
        /// </summary>
        public string AdminTheme
        {
            get { return GetValueByKey(SettingKeys.AdminTheme); }
            set { SetValueByKey(SettingKeys.AdminTheme, value); }
        }

        /// <summary>
        /// Category Display Page setting
        /// </summary>
        public string CategoryDisplayPage
        {
            get { return GetValueByKey(SettingKeys.CategoryDisplayPage); }
            set { SetValueByKey(SettingKeys.CategoryDisplayPage, value); }
        }

        /// <summary>
        /// Product Display Page setting
        /// </summary>
        public string ProductDisplayPage
        {
            get { return GetValueByKey(SettingKeys.ProductDisplayPage); }
            set { SetValueByKey(SettingKeys.ProductDisplayPage, value); }
        }

        /// <summary>
        /// Webpage Display Page setting
        /// </summary>
        public string WebpageDisplayPage
        {
            get { return GetValueByKey(SettingKeys.WebpageDisplayPage); }
            set { SetValueByKey(SettingKeys.WebpageDisplayPage, value); }
        }

        /// <summary>
        /// Link Display Page setting
        /// </summary>
        public string LinkDisplayPage
        {
            get { return GetValueByKey(SettingKeys.LinkDisplayPage); }
            set { SetValueByKey(SettingKeys.LinkDisplayPage, value); }
        }

        /// <summary>
        /// SSL Enabled setting
        /// </summary>
        public bool SSLEnabled
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.SSLEnabled), false); }
            set { SetValueByKey(SettingKeys.SSLEnabled, value.ToString()); }
        }

        /// <summary>
        /// Encrypted Uri setting
        /// </summary>
        public string EncryptedUri
        {
            get { return GetValueByKey(SettingKeys.SSLEncryptedUri); }
            set { SetValueByKey(SettingKeys.SSLEncryptedUri, value); }
        }

        /// <summary>
        /// Unencrypted Uri setting
        /// </summary>
        public string UnencryptedUri
        {
            get { return GetValueByKey(SettingKeys.SSLUnencryptedUri); }
            set { SetValueByKey(SettingKeys.SSLUnencryptedUri, value); }
        }

        /// <summary>
        /// Page View Tracking Enabled setting
        /// </summary>
        public bool PageViewTrackingEnabled
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.PageViewTrackingEnabled), false); }
            set { SetValueByKey(SettingKeys.PageViewTrackingEnabled, value.ToString()); }
        }

        /// <summary>
        /// Page View Tracking Days setting
        /// </summary>
        public int PageViewTrackingDays
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.PageViewTrackingDays)); }
            set { SetValueByKey(SettingKeys.PageViewTrackingDays, value.ToString()); }
        }

        /// <summary>
        /// Page View Tracking Save Archive setting
        /// </summary>
        public bool PageViewTrackingSaveArchive
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.PageViewTrackingSaveArchive), false); }
            set { SetValueByKey(SettingKeys.PageViewTrackingSaveArchive, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether full text search is enabled
        /// </summary>
        public bool FullTextSearch
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.FullTextSearch), false); }
            set { SetValueByKey(SettingKeys.FullTextSearch, value.ToString()); }
        }

        /// <summary>
        /// Smtp Server setting
        /// </summary>
        public string SmtpServer
        {
            get { return GetValueByKey(SettingKeys.SmtpServer); }
            set { SetValueByKey(SettingKeys.SmtpServer, value); }
        }

        /// <summary>
        /// Smtp Port setting
        /// </summary>
        public string SmtpPort
        {
            get { return GetValueByKey(SettingKeys.SmtpPort); }
            set { SetValueByKey(SettingKeys.SmtpPort, value); }
        }

        /// <summary>
        /// Smtp Requires Authentication setting
        /// </summary>
        public bool SmtpRequiresAuthentication
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.SmtpRequiresAuthentication),false); }
            set { SetValueByKey(SettingKeys.SmtpRequiresAuthentication, value.ToString()); }
        }

        /// <summary>
        /// Smtp User Name setting
        /// </summary>
        public string SmtpUserName
        {
            get { return GetValueByKey(SettingKeys.SmtpUserName); }
            set { SetValueByKey(SettingKeys.SmtpUserName, value); }
        }

        /// <summary>
        /// Smtp Password setting
        /// </summary>
        public string SmtpPassword
        {
            get { return GetValueByKey(SettingKeys.SmtpPassword); }
            set { SetValueByKey(SettingKeys.SmtpPassword, value); }
        }

        /// <summary>
        /// Smtp use SSL secure connection enabled
        /// </summary>
        public bool SmtpEnableSSL
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.SmtpEnableSSL), false); }
            set { SetValueByKey(SettingKeys.SmtpEnableSSL, value.ToString()); }
        }

        /// <summary>
        /// Default Email Address setting
        /// </summary>
        public string DefaultEmailAddress
        {
            get { return GetValueByKey(SettingKeys.DefaultEmailAddress); }
            set { SetValueByKey(SettingKeys.DefaultEmailAddress, value); }
        }

        /// <summary>
        /// Subscription Email Address setting
        /// </summary>
        public string SubscriptionEmailAddress
        {
            get { return GetValueByKey(SettingKeys.SubscriptionEmailAddress); }
            set { SetValueByKey(SettingKeys.SubscriptionEmailAddress, value); }
        }

        /// <summary>
        /// Subscription Request Expiration Days setting
        /// </summary>
        public int SubscriptionRequestExpirationDays
        {
            get
            {
                int days = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.SubscriptionRequestExpirationDays));
                if (days < 1) days = 7;
                return days;
            }
            set { SetValueByKey(SettingKeys.SubscriptionRequestExpirationDays, value.ToString()); }
        }

        /// <summary>
        /// Affiliate Tracker Url setting
        /// </summary>
        public string AffiliateTrackerUrl
        {
            get { return GetValueByKey(SettingKeys.AffiliateTrackerUrl); }
            set { SetValueByKey(SettingKeys.AffiliateTrackerUrl, value); }
        }

        /// <summary>
        /// Anonymous User Lifespan setting
        /// </summary>
        public int AnonymousUserLifespan
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.AnonymousUserLifespan)); }
            set { SetValueByKey(SettingKeys.AnonymousUserLifespan, value.ToString()); }
        }

        /// <summary>
        /// Anonymous Affiliate User Lifespan setting
        /// </summary>
        public int AnonymousAffiliateUserLifespan
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.AnonymousAffiliateUserLifespan)); }
            set { SetValueByKey(SettingKeys.AnonymousAffiliateUserLifespan, value.ToString()); }
        }

        //REVIEWS
        /// <summary>
        /// To User Auth Filter setting
        /// </summary>
        private UserAuthFilter ToUserAuthFilter(int value)
        {
            if (value < 0) value = 0;
            if (value > 3) value = 3;
            return (UserAuthFilter)value;
        }

        /// <summary>
        /// Product Review Enabled setting
        /// </summary>
        public UserAuthFilter ProductReviewEnabled
        {
            get { return ToUserAuthFilter(AlwaysConvert.ToInt(GetValueByKey(SettingKeys.ProductReviewEnabled))); }
            set { SetValueByKey(SettingKeys.ProductReviewEnabled, ((int)value).ToString()); }
        }

        /// <summary>
        /// Product Review Approval setting
        /// </summary>
        public UserAuthFilter ProductReviewApproval
        {
            get { return ToUserAuthFilter(AlwaysConvert.ToInt(GetValueByKey(SettingKeys.ProductReviewApproval))); }
            set { SetValueByKey(SettingKeys.ProductReviewApproval, ((int)value).ToString()); }
        }

        /// <summary>
        /// Product Review Image Verification setting
        /// </summary>
        public UserAuthFilter ProductReviewImageVerification
        {
            get { return ToUserAuthFilter(AlwaysConvert.ToInt(GetValueByKey(SettingKeys.ProductReviewImageVerification))); }
            set { SetValueByKey(SettingKeys.ProductReviewImageVerification, ((int)value).ToString()); }
        }

        /// <summary>
        /// Product Review Email Verification setting
        /// </summary>
        public UserAuthFilter ProductReviewEmailVerification
        {
            get { return ToUserAuthFilter(AlwaysConvert.ToInt(GetValueByKey(SettingKeys.ProductReviewEmailVerification))); }
            set { SetValueByKey(SettingKeys.ProductReviewEmailVerification, ((int)value).ToString()); }
        }

        /// <summary>
        /// Product Review Email Verification Template setting
        /// </summary>
        public int ProductReviewEmailVerificationTemplate
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.ProductReviewEmailVerificationTemplate)); }
            set { SetValueByKey(SettingKeys.ProductReviewEmailVerificationTemplate, value.ToString()); }
        }

        /// <summary>
        /// Product Review Terms And Conditions setting
        /// </summary>
        public string ProductReviewTermsAndConditions
        {
            get { return GetValueByKey(SettingKeys.ProductReviewTermsAndConditions); }
            set { SetValueByKey(SettingKeys.ProductReviewTermsAndConditions, value); }
        }

        //IMAGES
        private const int DefaultIconImageSize = 50;
        /// <summary>
        /// Icon Image Height setting
        /// </summary>
        public int IconImageHeight
        {
            get
            {
                int height = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.IconImageHeight));
                if (height == 0) return DefaultIconImageSize;
                return height;
            }
            set { SetValueByKey(SettingKeys.IconImageHeight, value.ToString()); }
        }

        /// <summary>
        /// Icon Image Width setting
        /// </summary>
        public int IconImageWidth
        {
            get
            {
                int width = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.IconImageWidth));
                if (width == 0) return DefaultIconImageSize;
                return width;
            }
            set { SetValueByKey(SettingKeys.IconImageWidth, value.ToString()); }
        }

        private const int DefaultThumbnailImageSize = 120;
        /// <summary>
        /// Thumbnail Image Height setting
        /// </summary>
        public int ThumbnailImageHeight
        {
            get
            {
                int height = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.ThumbnailImageHeight));
                if (height == 0) return DefaultThumbnailImageSize;
                return height;
            }
            set { SetValueByKey(SettingKeys.ThumbnailImageHeight, value.ToString()); }
        }

        /// <summary>
        /// Thumbnail Image Width setting
        /// </summary>
        public int ThumbnailImageWidth
        {
            get
            {
                int width = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.ThumbnailImageWidth));
                if (width == 0) return DefaultThumbnailImageSize;
                return width;
            }
            set { SetValueByKey(SettingKeys.ThumbnailImageWidth, value.ToString()); }
        }

        private const int DefaultStandardImageSize = 500;
        /// <summary>
        /// Standard Image Height setting
        /// </summary>
        public int StandardImageHeight
        {
            get
            {
                int height = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.StandardImageHeight));
                if (height == 0) return DefaultStandardImageSize;
                return height;
            }
            set { SetValueByKey(SettingKeys.StandardImageHeight, value.ToString()); }
        }

        /// <summary>
        /// Standard Image Width setting
        /// </summary>
        public int StandardImageWidth
        {
            get
            {
                int width = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.StandardImageWidth));
                if (width == 0) return DefaultStandardImageSize;
                return width;
            }
            set { SetValueByKey(SettingKeys.StandardImageWidth, value.ToString()); }
        }

        /// <summary>
        /// Image Sku Lookup Enabled setting
        /// </summary>
        public bool ImageSkuLookupEnabled
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.ImageSkuLookupEnabled), false); }
            set { SetValueByKey(SettingKeys.ImageSkuLookupEnabled, value.ToString()); }
        }

        private const int DefaultOptionThumbnailSize = 24;
        /// <summary>
        /// Option Thumbnail Width setting
        /// </summary>
        public int OptionThumbnailWidth
        {
            get
            {
                int width = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.OptionThumbnailWidth));
                if (width == 0) return DefaultOptionThumbnailSize;
                return width;
            }
            set { SetValueByKey(SettingKeys.OptionThumbnailWidth, value.ToString()); }
        }

        /// <summary>
        /// Option Thumbnail Height setting
        /// </summary>
        public int OptionThumbnailHeight
        {
            get
            {
                int height = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.OptionThumbnailHeight));
                if (height == 0) return DefaultOptionThumbnailSize;
                return height;
            }
            set { SetValueByKey(SettingKeys.OptionThumbnailHeight, value.ToString()); }
        }

        private const int DefaultOptionThumbnailColumns = 6;
        /// <summary>
        /// Option Thumbnail Columns setting
        /// </summary>
        public int OptionThumbnailColumns
        {
            get
            {
                int columns = AlwaysConvert.ToInt(GetValueByKey(SettingKeys.OptionThumbnailColumns));
                if (columns == 0) return DefaultOptionThumbnailColumns;
                return columns;
            }
            set { SetValueByKey(SettingKeys.OptionThumbnailColumns, value.ToString()); }
        }

        //PRODUCTS
        /// <summary>
        /// Product Purchasing Disabled setting
        /// </summary>
        public bool ProductPurchasingDisabled
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.ProductPurchasingDisabled), false); }
            set { SetValueByKey(SettingKeys.ProductPurchasingDisabled, value.ToString()); }
        }

        //DISCOUNTS
        /// <summary>
        /// If true, variants are always treated as a single product in line item mode.  If false,
        /// variants are treated as separate products.
        /// </summary>
        /// <remarks>This is not tested as of 7.0 release and is known to be non-functional 
        /// if used with product level discounts.</remarks>
        public bool CombineVariantsInLineItemDiscountMode
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.CombineVariantsInLineItemDiscountMode, "true"), true); }
            set { SetValueByKey(SettingKeys.CombineVariantsInLineItemDiscountMode, value.ToString()); }
        }

        /// <summary>
        /// Inventory Display Details setting
        /// </summary>
        public bool InventoryDisplayDetails
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.InventoryDisplayDetails), true); }
            set { SetValueByKey(SettingKeys.InventoryDisplayDetails, value.ToString()); }
        }

        /// <summary>
        /// Inventory Out Of Stock Message setting
        /// </summary>
        public string InventoryOutOfStockMessage
        {
            get
            {
                String retValue = GetValueByKey(SettingKeys.InventoryOutOfStockMessage);
                if (String.IsNullOrEmpty(retValue))
                {
                    retValue = "The product is currently out of stock, current quantity is {0}";
                }
                return retValue;
            }
            set { SetValueByKey(SettingKeys.InventoryOutOfStockMessage, value); }
        }

        /// <summary>
        /// Inventory InStock Message setting
        /// </summary>
        public string InventoryInStockMessage
        {
            get
            {
                String retValue = GetValueByKey(SettingKeys.InventoryInStockMessage);
                if (String.IsNullOrEmpty(retValue))
                {
                    retValue = "{0} units available";
                }
                return retValue;
            }
            set { SetValueByKey(SettingKeys.InventoryInStockMessage, value); }
        }

        /// <summary>
        /// Checkout Terms And Conditions setting
        /// </summary>
        public string CheckoutTermsAndConditions
        {
            get { return GetValueByKey(SettingKeys.CheckoutTermsAndConditions); }
            set { SetValueByKey(SettingKeys.CheckoutTermsAndConditions, value); }
        }

        /// <summary>
        /// Order Minimum Amount setting
        /// </summary>
        public decimal OrderMinimumAmount
        {
            get { return AlwaysConvert.ToDecimal(GetValueByKey(SettingKeys.OrderMinimumAmount)); }
            set { SetValueByKey(SettingKeys.OrderMinimumAmount, value.ToString()); }
        }

        /// <summary>
        /// Order Maximum Amount setting
        /// </summary>
        public decimal OrderMaximumAmount
        {
            get { return AlwaysConvert.ToDecimal(GetValueByKey(SettingKeys.OrderMaximumAmount)); }
            set { SetValueByKey(SettingKeys.OrderMaximumAmount, value.ToString()); }
        }

        //PAYMENTS
        /// <summary>
        /// Payment Lifespan setting
        /// </summary>
        public int PaymentLifespan
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.PaymentLifespan)); }
            set { SetValueByKey(SettingKeys.PaymentLifespan, value.ToString()); }
        }

        /// <summary>
        /// Enable Credit Card Storage setting
        /// </summary>
        public bool EnableCreditCardStorage
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.EnableCreditCardStorage), true); }
            set { SetValueByKey(SettingKeys.EnableCreditCardStorage, value.ToString()); }
        }

        //CURRENCY
        /// <summary>
        /// Base Currency Id setting
        /// </summary>
        public int BaseCurrencyId
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.BaseCurrencyId)); }
            set { SetValueByKey(SettingKeys.BaseCurrencyId, value.ToString()); }
        }

        //GIFT CERTIFICATE
        /// <summary>
        /// Gift Certificate Days To Expire setting
        /// </summary>
        public int GiftCertificateDaysToExpire
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.GiftCertificateDaysToExpire)); }
            set { SetValueByKey(SettingKeys.GiftCertificateDaysToExpire, value.ToString()); }
        }

        /// <summary>
        /// Forex Provider Class Id setting
        /// </summary>
        public string ForexProviderClassId
        {
            get { return GetValueByKey(SettingKeys.ForexProviderClassId); }
            set { SetValueByKey(SettingKeys.ForexProviderClassId, value); }
        }

        /// <summary>
        /// Site Disclaimer Message setting
        /// </summary>
        public string SiteDisclaimerMessage
        {
            get { return GetValueByKey(SettingKeys.SiteDisclaimerMessage); }
            set { SetValueByKey(SettingKeys.SiteDisclaimerMessage, value); }
        }

        /// <summary>
        /// Postal Code Countries setting
        /// </summary>
        public string PostalCodeCountries
        {
            get
            {
                string countries = GetValueByKey(SettingKeys.PostalCodeCountries);
                if (string.IsNullOrEmpty(countries)) return "US,CA";
                return countries;
            }
            set { SetValueByKey(SettingKeys.PostalCodeCountries, value); }
        }

        /// <summary>
        /// Time Zone Code setting
        /// </summary>
        public string TimeZoneCode
        {
            get { return GetValueByKey(SettingKeys.TimeZoneCode); }
            set { SetValueByKey(SettingKeys.TimeZoneCode, value); }
        }

        /// <summary>
        /// Time Zone Offset setting
        /// </summary>
        public double TimeZoneOffset
        {
            get { return AlwaysConvert.ToDouble(GetValueByKey(SettingKeys.TimeZoneOffset)); }
            set { SetValueByKey(SettingKeys.TimeZoneOffset, value.ToString()); }
        }

        /// <summary>
        /// Google Urchin Id setting
        /// </summary>
        public string GoogleUrchinId
        {
            get { return GetValueByKey(SettingKeys.GoogleUrchinId); }
            set { SetValueByKey(SettingKeys.GoogleUrchinId, value); }
        }

        /// <summary>
        /// Enable Google Analytics Page Tracking setting
        /// </summary>
        public bool EnableGoogleAnalyticsPageTracking
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.EnableGoogleAnalyticsPageTracking), false); }
            set { SetValueByKey(SettingKeys.EnableGoogleAnalyticsPageTracking, value.ToString()); }
        }

        /// <summary>
        /// Enable Google Analytics Ecommerce Tracking setting
        /// </summary>
        public bool EnableGoogleAnalyticsEcommerceTracking
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.EnableGoogleAnalyticsEcommerceTracking), false); }
            set { SetValueByKey(SettingKeys.EnableGoogleAnalyticsEcommerceTracking, value.ToString()); }
        }

        /// <summary>
        /// Product Tell A Friend Email Template Id setting
        /// </summary>
        public int ProductTellAFriendEmailTemplateId
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.ProductTellAFriendEmailTemplateId)); }
            set { SetValueByKey(SettingKeys.ProductTellAFriendEmailTemplateId, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether tell a friend feature should use captcha
        /// </summary>
        public bool ProductTellAFriendCaptcha
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.ProductTellAFriendCaptcha), false); }
            set { SetValueByKey(SettingKeys.ProductTellAFriendCaptcha, value.ToString()); }
        }

        /// <summary>
        /// Default Email List Id setting
        /// </summary>
        public int DefaultEmailListId
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.DefaultEmailListId),0); }
            set { SetValueByKey(SettingKeys.DefaultEmailListId, value.ToString()); }
        }


        private bool _MaxRequestLengthLoaded = false;
        private int _MaxRequestLength = 4096;
        /// <summary>
        /// MaxRequestLength set in web.config
        /// </summary>
        public int MaxRequestLength
        {
            get 
            {
                if (!_MaxRequestLengthLoaded)
                {
                    LoadMaxRequestLength();
                }
                return _MaxRequestLength;            
            }
        }

        private void LoadMaxRequestLength()
        {
            string webConfFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web.config");
            //if (!System.IO.File.Exists(webConfFile)) return;
            string fileData = Utility.FileHelper.ReadText(webConfFile);
            
            Match runtimeNode = Regex.Match(fileData, "(<httpRuntime[\t\n ]+[a-zA-Z0-9\" =\\n\\t]*/>)");            
            if (!runtimeNode.Success) return;
            
            Match maxReqValNode = Regex.Match(runtimeNode.Groups[1].Value, "maxRequestLength[ \t]*=[ \t]*\"?([0-9]+)\"?");
            if (!maxReqValNode.Success) return;

            _MaxRequestLength = AlwaysConvert.ToInt(maxReqValNode.Groups[1].Value, _MaxRequestLength);
            _MaxRequestLengthLoaded = true;            
        }

        /// <summary>
        /// Message that will be displayed when store front is closed temporarily (e.g. for maintenence etc.)
        /// </summary>
        public string StoreFrontClosedMessage
        {
            get
            {
                String retValue = GetValueByKey(SettingKeys.StoreFrontClosedMessage);
                if (String.IsNullOrEmpty(retValue))
                {
                    retValue = "Store is temporarily closed, please visit later.";
                }
                return retValue;
            }
            set { SetValueByKey(SettingKeys.StoreFrontClosedMessage, value); }
        }

        /// <summary>
        /// Is the store front is temporarily closed, e.g. for maintenence etc.
        /// </summary>
        private const StoreClosureType StoreFrontClosedDefault = StoreClosureType.Open;
        /// <summary>
        /// Is the store front is temporarily closed, e.g. for maintenence etc.
        /// </summary>
        public StoreClosureType StoreFrontClosed
        {
            get { return ToStoreClosureType(AlwaysConvert.ToInt(GetValueByKey(SettingKeys.StoreFrontClosed), (int)StoreFrontClosedDefault)); }
            set { SetValueByKey(SettingKeys.StoreFrontClosed, ((int)value).ToString()); }
        }

        //REVIEWS
        private StoreClosureType ToStoreClosureType(int value)
        {
            if (value < 0) value = 0;
            if (value > (Enum.GetNames(typeof(StoreClosureType)).Length + 1)) value = Enum.GetNames(typeof(StoreClosureType)).Length + 1;
            return (StoreClosureType)value;
        }

        // EXTENSIONS
        /// <summary>
        /// Gets or sets the file extensions that are allowed for asset management
        /// </summary>
        public string FileExt_Assets
        {
            get { return GetValueByKey(SettingKeys.FileExt_Assets); }
            set { SetValueByKey(SettingKeys.FileExt_Assets, value); }
        }

        /// <summary>
        /// Gets or sets the file extensions that are allowed for theme management
        /// </summary>
        public string FileExt_Themes
        {
            get { return GetValueByKey(SettingKeys.FileExt_Themes); }
            set { SetValueByKey(SettingKeys.FileExt_Themes, value); }
        }

        /// <summary>
        /// Gets or sets the file extensions that are allowed for digital goods
        /// </summary>
        public string FileExt_DigitalGoods
        {
            get { return GetValueByKey(SettingKeys.FileExt_DigitalGoods); }
            set { SetValueByKey(SettingKeys.FileExt_DigitalGoods, value); }
        }

        
        /// <summary>
        /// Gets or sets the minimum search phrase length for searches on the retail side
        /// </summary>
        public int MinimumSearchLength
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.MinimumSearchLength), 1); }
            set { SetValueByKey(SettingKeys.MinimumSearchLength, value.ToString()); }
        }
        
        /// <summary>
        /// Gets or sets the affiliate self signup permission setting
        /// </summary>
        public bool AffiliateAllowSelfSignup
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.AffiliateAllowSelfSignup), false); }
            set { SetValueByKey(SettingKeys.AffiliateAllowSelfSignup, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the affiliate self signup referral rule
        /// </summary>
        public CommerceBuilder.Marketing.ReferralRule AffiliateReferralRule
        {
            get { return (CommerceBuilder.Marketing.ReferralRule)AlwaysConvert.ToInt16(GetValueByKey(SettingKeys.AffiliateReferralRule), 0); }
            set { SetValueByKey(SettingKeys.AffiliateReferralRule, ((short)value).ToString()); }
        }
        
        /// <summary>
        /// Gets or sets the affiliate persistence period.
        /// </summary>
        public CommerceBuilder.Marketing.AffiliateReferralPeriod AffiliatePersistence 
        {
            get { return (CommerceBuilder.Marketing.AffiliateReferralPeriod)AlwaysConvert.ToByte(GetValueByKey(SettingKeys.AffiliatePersistence), 0); }
            set { SetValueByKey(SettingKeys.AffiliatePersistence, ((byte)value).ToString()); }
        }


        /// <summary>
        /// Gets or sets the affiliate referral period
        /// </summary>
        public short AffiliateReferralPeriod 
        {
            get { return AlwaysConvert.ToInt16(GetValueByKey(SettingKeys.AffiliateReferralPeriod), 0); }
            set { SetValueByKey(SettingKeys.AffiliateReferralPeriod, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the affiliate commission type
        /// </summary>
        public bool AffiliateCommissionIsPercent
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.AffiliateCommissionIsPercent),false); }
            set { SetValueByKey(SettingKeys.AffiliateCommissionIsPercent, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the information to flag affiliate commision on order total.
        /// </summary>
        public bool AffiliateCommissionOnTotal 
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.AffiliateCommissionOnTotal), false); }
            set { SetValueByKey(SettingKeys.AffiliateCommissionOnTotal, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the affiliate commission rate
        /// </summary>
        public decimal AffiliateCommissionRate
        {
            get { return AlwaysConvert.ToDecimal(GetValueByKey(SettingKeys.AffiliateCommissionRate), 0); }
            set { SetValueByKey(SettingKeys.AffiliateCommissionRate, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the affiliate parameter name, the default parameter name is 'afid'.
        /// </summary>
        public string AffiliateParameterName 
        {
            get 
            {
                string parameterName = GetValueByKey(SettingKeys.AffiliateParameterName);
                if (string.IsNullOrEmpty(parameterName))
                    parameterName = "afid";
                return parameterName; 
            }
            set { SetValueByKey(SettingKeys.AffiliateParameterName, value); }
        }

        /// <summary>
        /// Get or set allowed extensions for SEO redirects as comma delimited list
        /// </summary>
        public string SeoAllowedExtensions
        {
            get { return GetValueByKey(SettingKeys.SeoAllowedExtensions); }
            set { SetValueByKey(SettingKeys.SeoAllowedExtensions, value); }
        }

        /// <summary>
        /// Gets or sets that no other extensions are allowed except default.
        /// </summary>
        public bool SeoAllowCustomExtensions 
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.SeoAllowCustomExtensions), false); }
            set { SetValueByKey(SettingKeys.SeoAllowCustomExtensions, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets that URLs without extensions are allowed.
        /// </summary>
        public bool SeoAllowUrlWithoutExtension
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.SeoAllowUrlWithoutExtension), false); }
            set { SetValueByKey(SettingKeys.SeoAllowUrlWithoutExtension, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the cache size for SEO redirects.
        /// </summary>
        public int SeoCacheSize 
        {
            get { return AlwaysConvert.ToInt(GetValueByKey(SettingKeys.SeoCacheSize), 1000); }
            set { SetValueByKey(SettingKeys.SeoCacheSize, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets a flag to indicate whether or not to track statistics for the redirects.
        /// </summary>
        public bool SeoTrackStatistics
        {
            get { return AlwaysConvert.ToBool(GetValueByKey(SettingKeys.SeoTrackStatistics), false); }
            set { SetValueByKey(SettingKeys.SeoTrackStatistics, value.ToString()); }
        }


        private class SettingKeys
        {
            private SettingKeys() { }
            
            //STORE SETTINGS
            public const string PasswordFormat = "PasswordFormat";
            public const string StoreTheme = "StoreTheme";
            public const string AdminTheme = "AdminTheme";
            public const string CategoryDisplayPage = "CategoryDisplayPage";
            public const string ProductDisplayPage = "ProductDisplayPage";
            public const string WebpageDisplayPage = "WebpageDisplayPage";
            public const string LinkDisplayPage = "LinkDisplayPage";
            public const string SSLEnabled = "SSLEnabled";
            public const string SSLEncryptedUri = "SSLEncryptedUri";
            public const string SSLUnencryptedUri = "SSLUnencryptedUri";
            public const string PageViewTrackingEnabled = "PageViewTrackingEnabled";
            public const string PageViewTrackingDays = "PageViewTrackingDays";
            public const string PageViewTrackingSaveArchive = "PageViewTrackingSaveArchive";
            public const string FullTextSearch = "FullTextSearch";

            //LOCALE SETTINGS
            public const string PostalCodeCountries = "PostalCodeCountries";
            public const string TimeZoneCode = "Store_TimeZoneCode";
            public const string TimeZoneOffset = "Store_TimeZoneOffset";

            //Email Settings
            public const string SmtpServer = "Email_SmtpServer";
            public const string SmtpPort = "Email_SmtpPort";
            public const string SmtpRequiresAuthentication = "Email_SmtpRequiresAuthentication";
            public const string SmtpUserName = "Email_SmtpUserName";
            public const string SmtpPassword = "Email_SmtpPassword";
            public const string SmtpEnableSSL = "Email_EnableSSL";
            
            public const string DefaultEmailAddress = "Email_DefaultAddress";
            public const string SubscriptionEmailAddress = "Email_SubscriptionAddress";
            public const string SubscriptionRequestExpirationDays = "SubscriptionRequestExpirationDays";
            public const string AffiliateTrackerUrl = "AffiliateTrackerUrl";
            public const string AnonymousUserLifespan = "AnonymousUserLifespan";
            public const string AnonymousAffiliateUserLifespan = "AnonymousAffiliateUserLifespan";
            //PAYMENTS
            public const string PaymentLifespan = "PaymentLifespan";
            public const string EnableCreditCardStorage = "EnableCreditCardStorage";
            //REVIEWS
            public const string ProductReviewEnabled = "ProductReviewEnabled";
            public const string ProductReviewApproval = "ProductReviewApproval";
            public const string ProductReviewImageVerification = "ProductReviewImageVerification";
            public const string ProductReviewEmailVerification = "ProductReviewEmailVerification";
            public const string ProductReviewEmailVerificationTemplate = "ProductReviewEmailVerificationTemplate";
            public const string ProductReviewTermsAndConditions = "ProductReviewTermsAndConditions";
            //IMAGES
            public const string IconImageWidth = "IconImageWidth";
            public const string IconImageHeight = "IconImageHeight";
            public const string ThumbnailImageWidth = "ThumbnailImageWidth";
            public const string ThumbnailImageHeight = "ThumbnailImageHeight";
            public const string StandardImageWidth = "StandardImageWidth";
            public const string StandardImageHeight = "StandardImageHeight";
            public const string ImageSkuLookupEnabled = "ImageSkuLookupEnabled";
            public const string OptionThumbnailWidth = "OptionThumbnailWidth";
            public const string OptionThumbnailHeight = "OptionThumbnailHeight";
            public const string OptionThumbnailColumns = "OptionThumbnailColumns";

            //PRODUCTS
            public const string ProductPurchasingDisabled = "ProductPurchasingDisabled";

            //DISCOUNTS
            public const string CombineVariantsInLineItemDiscountMode = "CombineVariantsInLineItemDiscountMode";

            // INVENTORY
            public const string InventoryDisplayDetails = "InventoryDisplayDetails";
            public const string InventoryInStockMessage = "InventoryInStockMessage";
            public const string InventoryOutOfStockMessage = "InventoryOutOfStockMessage";

            //ORDER
            public const string CheckoutTermsAndConditions = "CheckoutTermsAndConditions";
            public const string OrderMinimumAmount = "OrderMinimumAmount";
            public const string OrderMaximumAmount = "OrderMaximumAmount";

            //CURRENCY
            public const string BaseCurrencyId = "BaseCurrencyId";
            public const string ForexProviderClassId = "ForexProviderClassId";

            //GIFT CERTIFICATES
            public const string GiftCertificateDaysToExpire = "GiftCertificateDaysToExpire";

            //SITE DISCLAIMER MESSAGE
            public const string SiteDisclaimerMessage = "SiteDisclaimerMessage";

            //GOOGLE ANALYTICS SETTINGS
            public const string GoogleUrchinId = "GoogleUrchinId";
            public const string EnableGoogleAnalyticsPageTracking = "EnableGoogleAnalyticsPageTracking";
            public const string EnableGoogleAnalyticsEcommerceTracking = "EnableGoogleAnalyticsEcommerceTracking";

            // TELL A FRIEND EMAIL ABOUT A PRODUCT, EMAIL TEMPLATE
            public const string ProductTellAFriendEmailTemplateId = "ProductTellAFriendEmailTemplateId";
            public const string ProductTellAFriendCaptcha = "ProductTellAFriendCaptcha";

            // DEFAULT STORE EMAIL LIST ID
            public const string DefaultEmailListId = "DefaultEmailListId";

            //Settings from Web.config
            public const string MaxRequestLength = "MaxRequestLength";

            //Setting to close store front temporarily
            public const string StoreFrontClosed = "StoreFrontClosed";
            public const string StoreFrontClosedMessage = "StoreFrontClosedMessage"; 

            // EXTENSIONS
            public const string FileExt_Assets = "FileExt_Assets";
            public const string FileExt_Themes = "FileExt_Themes";
            public const string FileExt_DigitalGoods = "FileExt_DigitalGoods";

            //SEARCH
            public const string MinimumSearchLength = "MinimumSearchLength";

            //AFFILIATE PROGRAM
            public const string AffiliateAllowSelfSignup = "AffiliateAllowSelfSignup";
            public const string AffiliateReferralRule = "AffiliateReferralRule";
            public const string AffiliatePersistence = "AffiliatePersistence";
            public const string AffiliateReferralPeriod = "AffiliateReferralPeriod";
            public const string AffiliateCommissionIsPercent = "AffiliateCommissionIsPercent";
            public const string AffiliateCommissionOnTotal = "AffiliateCommissionOnTotal";
            public const string AffiliateCommissionRate = "AffiliateCommissionRate";
            public const string AffiliateParameterName = "AffiliateParameterName";

            // SEO
            public const string SeoAllowedExtensions = "SeoAllowedExtensions";
            public const string SeoAllowCustomExtensions = "SeoAllowCustomExtensions";
            public const string SeoAllowUrlWithoutExtension = "SeoAllowUrlWithoutExtension";
            public const string SeoCacheSize = "SeoCacheSize";
            public const string SeoTrackStatistics = "SeoTrackStatistics"; 

        }

        #endregion

    }
}
