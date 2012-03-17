using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Security;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
    /// <summary>A visual Reverse Turing Test (HIP).</summary>
    [ToolboxData("<{0}:CaptchaImage Runat=\"server\" Height=\"100px\" Width=\"300px\" ImageUrl=\"~/CaptchaImage.axd\" />")]
    public class CaptchaImage : Image
    {
        /// <summary>Whether Authenticate has previously succeeded in this HttpRequest.</summary>
        private bool _authenticated;

        private new string ImageUrl
        {
            get { return base.ImageUrl; }
            set { base.ImageUrl = value; }
        }

        /// <summary>Backing store for challenge text.</summary>
        private string _ChallengeText;

        /// <summary>Gets or sets the text to display in the challenge image.</summary>
        /// <value>The text to display in the challenge image.</value>
        [Category("Behavior")]
        [Description("The text to display in the challenge image.")]
        public string ChallengeText
        {
            get { return _ChallengeText; }
            set { _ChallengeText = value; }
        }

        /// <summary>Generates a new image and fills in the dynamic image and hidden field appropriately.</summary>
        /// <param name="e">Ignored.</param>
        protected sealed override void OnPreRender(EventArgs e)
        {
            if (this.Visible)
            {
                if (string.IsNullOrEmpty(this.ChallengeText)) this.ChallengeText = StringHelper.RandomNumber(6);
                //CREATE THE CHALLENGE IMAGE
                string encryptedText = HttpUtility.UrlEncode(EncryptionHelper.EncryptAES(this.ChallengeText));
                base.ImageUrl = "~/Captcha.ashx?w=" + (int)Width.Value + "&h=" + (int)Height.Value + "&id=" + encryptedText + "&sid=" + Token.Instance.StoreId.ToString() + "&t=" + DateTime.Now.ToString("hhmmss");
            }
            base.OnPreRender(e);
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
        }

        protected override void LoadControlState(object savedState)
        {
            ChallengeText = (string)(savedState ?? string.Empty);
        }

        protected override object SaveControlState()
        {
            return ChallengeText;
        }

        /// <summary>Authenticates user-supplied data against that retrieved using the challenge ID.</summary>
        /// <param name="userData">The user-supplied data.</param>
        /// <returns>Whether the user-supplied data matches that retrieved using the challenge ID.</returns>
        public bool Authenticate(string userData)
        {
            // We want to allow multiple authentication requests within the same HTTP request,
            // so we can the result as a member variable of the class (non-static)
            if (_authenticated == true) return _authenticated;

            // If no authentication has happened previously, and if the user has supplied text,
            // and if the ID is stored correctly in the page, and if the user text matches the challenge text,
            // then set the challenge text, note that we've authenticated, and return true.  Otherwise, failed authentication.
            if (string.IsNullOrEmpty(userData)) return false;
            _authenticated = (this.ChallengeText == userData);
            return _authenticated;
        }
    }
}
