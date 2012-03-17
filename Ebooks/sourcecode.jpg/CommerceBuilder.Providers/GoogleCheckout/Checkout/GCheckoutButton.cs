using System;
using System.Web.UI;
using System.ComponentModel;
using System.Configuration;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.Checkout
{
    /// <summary>
    /// Google Checkout button that will display on your web page.
    /// </summary>
    public class GCheckoutButton : System.Web.UI.WebControls.ImageButton
    {

        private ButtonSize _Size = ButtonSize.Medium;
        private BackgroundType _Background = BackgroundType.White;
        private string _Currency = "USD";
        private int _CartExpirationMinutes = 0;
        private bool _UseHttps = false;
        private bool _IsConfigured = false;


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
        
        public bool IsConfigured
        {
            get{return _IsConfigured;}            
        }

        public GCheckoutButton() : base()
        {            
            if (GatewayInstance == null)
            {
                _IsConfigured = false;
            }
            else
            {
                if (string.IsNullOrEmpty(GatewayInstance.MerchantID) ||
                    string.IsNullOrEmpty(GatewayInstance.MerchantKey))
                {
                    _IsConfigured = false;
                }
                else
                {
                    _IsConfigured = true;
                }
            }

            if (!IsConfigured)
            {
                this.Visible = false;
            }
        }

        /// <summary>
        /// The <b>Size</b> property value determines the size of the 
        /// Google Checkout button that will display on your web page.
        /// Valid values for this property are "Small", "Medium" and 
        /// "Large". A small button is 160 pixels wide and 43 pixels high.
        /// A medium button is 168 pixels wide and 44 pixels high. A large
        /// button is 180 pixels wide and 46 pixels high.
        /// </summary>
        [Category("Google")]
        [Description("Small: 160 by 43 pixels\nMedium: 168 by 44 pixels\n" +
           "Large: 180 by 46 pixels")]
        public ButtonSize Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
                SetImageUrl();
            }
        }

        /// <summary>
        /// The <b>GoogleMerchantID</b> property value identifies your Google 
        /// Checkout Merchant ID. This value should be set in Payment Gateway 
        /// Configuration menu for Google Checkout.
        /// </summary>
        [Category("Google")]
        [Description("Your numeric Merchant ID. To see it, log in to Google, " +
           "select the Settings tab, click the Integration link.")]
        private string MerchantID
        {
            get
            {
                if (GatewayInstance == null)
                {
                    return string.Empty;
                }
                else
                {
                    return GatewayInstance.MerchantID;
                }
                //PaymentGatewayDataSource.
                //string RetVal = ConfigurationSettings.AppSettings["GoogleMerchantID"];
                //if (RetVal == null)
                //    RetVal = string.Empty;
                //return RetVal;
            }
        }

        /// <summary>
        /// The <b>GoogleMerchantKey</b> property value identifies your Google 
        /// Checkout Merchant key. This value should be set in Payment Gateway 
        /// Configuration menu for Google Checkout.
        /// </summary>
        [Category("Google")]
        [Description("Your alpha-numeric Merchant Key. To see it, log in to " +
           "Google, select the Settings tab, click the Integration link.")]
        private string MerchantKey
        {
            get
            {
                if (GatewayInstance == null)
                {
                    return string.Empty;
                }
                else
                {
                    return GatewayInstance.MerchantKey;
                }

                //string RetVal = ConfigurationSettings.AppSettings["GoogleMerchantKey"];
                //if (RetVal == null)
                //    RetVal = string.Empty;
                //return RetVal;
            }
        }

        /// <summary>
        /// The <b>GoogleEnvironment</b> property value identifies the environment 
        /// in which your application is running. In your test environment, the 
        /// value of the <b>GoogleEnvironment</b> property should be 
        /// <b>Sandbox</b>. In your production environment, the value of the 
        /// property should be <b>Production</b>. This property should be set in 
        /// Payment Gateway Configuration menu for Google Checkout.
        /// </summary>
        [Category("Google")]
        [Description("Sandbox is the test environment where no funds are ever " +
           "taken from or paid to anyone. In Production all transactions are " +
           "real.")]
        private EnvironmentType Environment
        {
            get
            {
                if (GatewayInstance == null)
                {
                    return EnvironmentType.Unknown;
                }
                else
                {
                    return GatewayInstance.Environment;
                }

                //EnvironmentType RetVal = EnvironmentType.Unknown;
                //string FromFile =
                //  ConfigurationSettings.AppSettings["GoogleEnvironment"];
                //if (FromFile != null && FromFile != string.Empty)
                //{
                //    RetVal = (EnvironmentType)
                //      Enum.Parse(typeof(EnvironmentType), FromFile, true);
                //}
                //return RetVal;
            }
        }

        /// <summary>
        /// The <b>Background</b> property value indicates whether the Google
        /// Checkout button should be displayed on a white background or a 
        /// transparent background. Valid values for this property are "White"
        /// and "Transparent".
        /// </summary>
        [Category("Google")]
        [Description("Use White if you're placing the button on a white " +
           "background, or Transparent if you're placing the button on a " +
           "colored background.")]
        public BackgroundType Background
        {
            get
            {
                return _Background;
            }
            set
            {
                _Background = value;
                SetImageUrl();
            }
        }

        /// <summary>
        /// The <b>Currency</b> property value identifies the currency that should
        /// be associated with prices in your Checkout API requests. The value of 
        /// this property should be a three-letter ISO 4217 currency code. 
        /// </summary>
        [Category("Google")]
        [Description("USD for US dollars, GBP for British pounds, SEK for " +
           "Swedish krona, EUR for Euro etc.")]
        public string Currency
        {
            get
            {
                return _Currency;
            }
            set
            {
                _Currency = value;
            }
        }

        /// <summary>
        /// The <b>CartExpirationMinutes</b> property value identifies the length
        /// of time, in minutes, after which an unsubmitted shopping cart will 
        /// become invalid. A value of <b>0</b> indicates that the shopping cart
        /// does not expire.
        /// </summary>
        [Category("Google")]
        [Description("How many minutes (after the user clicks the Google " +
           "Checkout button on this page) until the cart expires. 0 means the " +
           "cart doesn't expire.")]
        public int CartExpirationMinutes
        {
            get
            {
                return _CartExpirationMinutes;
            }
            set
            {
                if (value >= 0)
                {
                    _CartExpirationMinutes = value;
                }
            }
        }

        /// <summary>
        /// The <b>UseHttps</b> property sets whether the button graphic should
        /// be requested from Google with an HTTPS call. The default (false) is to
        /// use HTTP. If the page is fetched through HTTPS, some users (depending 
        /// on browser settings) will get security warnings if the button graphic
        /// is fetched with HTTP.
        /// </summary>
        [Category("Google")]
        [Description("If true, the button graphic will be fetched with a HTTPS " +
           "request.")]
        public bool UseHttps
        {
            get
            {
                return _UseHttps;
            }
            set
            {
                _UseHttps = value;
            }
        }

        /// <summary>
        /// On initialization, this class calls the SetImageUrl method.
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            //this.Context.Request
            SetImageUrl();
        }

        /// <summary>
        /// This method creates the URL for the Google Checkout button image. 
        /// This method uses the value of the "Size" property to set the width 
        /// and height of the image. It also uses the value of the "Background"
        /// property to set a style that dictates the background color for the 
        /// image. Finally, the method uses the value of the "Environment
        /// </summary>
        private void SetImageUrl()
        {
            int Width = 0;
            int Height = 0;
            switch (Size)
            {
                case ButtonSize.Small:
                    Width = 160;
                    Height = 43;
                    break;
                case ButtonSize.Medium:
                    Width = 168;
                    Height = 44;
                    break;
                case ButtonSize.Large:
                    Width = 180;
                    Height = 46;
                    break;
            }
            this.Width = Width;
            this.Height = Height;
            string StyleInUrl = "white";
            if (Background == BackgroundType.Transparent)
                StyleInUrl = "trans";
            string VariantInUrl = "text";
            if (!Enabled)
                VariantInUrl = "disabled";
            string Protocol = "http";
            if (UseHttps)
                Protocol = "https";
            if (Environment == EnvironmentType.Sandbox)
            {
                ImageUrl = string.Format(
                  "{0}://sandbox.google.com/checkout/buttons/checkout.gif?" +
                  "merchant_id={1}&w={2}&h={3}&style={4}&variant={5}",
                  Protocol, MerchantID, Width, Height, StyleInUrl, VariantInUrl);
            }
            else
            {
                ImageUrl = string.Format(
                  "{0}://checkout.google.com/buttons/checkout.gif?" +
                  "merchant_id={1}&w={2}&h={3}&style={4}&variant={5}",
                  Protocol, MerchantID, Width, Height, StyleInUrl, VariantInUrl);
            }
        }

        /// <summary>
        /// This method calls the <see cref="CheckoutShoppingCartRequest"/> class
        /// to initialize a new instance of that class. Before doing so, this method
        /// verifies that the MerchantID, MerchantKey and Environment properties
        /// have all been set.
        /// </summary>
        public CheckoutShoppingCartRequest CreateRequest()
        {
            if (MerchantID == string.Empty)
            {
                throw new ApplicationException("Set GoogleMerchantID in the " +
                  "web.config file.");
            }
            if (MerchantKey == string.Empty)
            {
                throw new ApplicationException("Set GoogleMerchantKey in the " +
                  "web.config file.");
            }
            if (Environment == EnvironmentType.Unknown)
            {
                throw new ApplicationException("Set GoogleEnvironment in the " +
                  "web.config file.");
            }
            return new CheckoutShoppingCartRequest(MerchantID, MerchantKey,
              Environment, _Currency, _CartExpirationMinutes);
        }

    }

    /// <summary>
    /// This enumeration defines valid sizes for the Google Checkout button.
    /// Valid values for the "Size" property are "Small", "Medium" and "Large".
    /// </summary>
    public enum ButtonSize
    {
        /// <summary>160 x 43 pixels</summary>
        Small = 0,
        /// <summary>168 by 44 pixels</summary>
        Medium = 1,
        /// <summary>180 x 46 pixels</summary>
        Large = 2
    }

    /// <summary>
    /// This enumeration defines valid background colors for the Google Checkout
    /// button. Valid values for the "Background" property are "White" and 
    /// "Transparent".
    /// </summary>
    public enum BackgroundType
    {
        /// <summary>You are placing the button on a white background</summary>
        White = 0,
        /// <summary>You are placing the button on a colored background</summary>
        Transparent = 1
    }
    
}
