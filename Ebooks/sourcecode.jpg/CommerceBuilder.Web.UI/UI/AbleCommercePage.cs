using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Web.UI;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;
using CommerceBuilder.Configuration;
using CommerceBuilder.Users;
using CommerceBuilder.Stores;
using CommerceBuilder.Personalization;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace CommerceBuilder.Web.UI
{
    /* This module is responsible for:
     * - applying dynamic theme to page
     */

    public class AbleCommercePage : System.Web.UI.Page
    {
        private bool _HasBasketControl;
        private bool _PreserveWhitespace = true;

        public AbleCommercePage()
        {
            this.PreInit += new EventHandler(AbleCommercePage_PreInit);
        }

        /// <summary>
        /// Gets or sets the flag indicating whether this page has a control that will
        /// display updated basket contents on a page postback.  When set to false,
        /// some actions will cause the user to be redirected to the basket page
        /// in order to see changes made to the contents.
        /// </summary>
        public bool HasBasketControl
        {
            get { return _HasBasketControl; }
            set { _HasBasketControl = value; }
        }

        /// <summary>
        /// Enables or disables whitespace compression.  When true, whitespace at the beginning 
        /// and ending of the generated HTML page is retained.  If false, this whitespace is 
        /// trimmed to reduce the render page size.  Default is false.
        /// </summary>
        public bool PreserveWhitespace
        {
            get { return _PreserveWhitespace; }
            set { _PreserveWhitespace = value; }
        }

        void AbleCommercePage_PreInit(object sender, EventArgs e)
        {
            this.Load += new System.EventHandler(this.AbleCommercePage_Load);
            this.InitializeTheme();
        }

        protected virtual void InitializeTheme()
        {
            string theme = string.Empty;
            //GET DEFAULT STORE THEME
            Store store = Token.Instance.Store;
            if (store != null)
            {
                theme = store.Settings.StoreTheme;
                if (!string.IsNullOrEmpty(theme) && !CommerceBuilder.UI.Styles.Theme.Exists(theme))
                {
                    //INVALID THEME SELECTED
                    theme = string.Empty;
                }
            }
            //GET PAGE PATH
            SharedPersonalization personalization = SharedPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, false);
            if (personalization != null)
            {
                if ((personalization.Theme != string.Empty) && (CommerceBuilder.UI.Styles.Theme.Exists(personalization.Theme)))
                {
                    theme = personalization.Theme;
                }
                if (personalization.MasterPageFile != string.Empty)
                {
                    this.MasterPageFile = personalization.MasterPageFile;
                }
            }
            //SET THE THEME
            if (!string.IsNullOrEmpty(theme)) this.Theme = theme;
        }

        protected override void InitializeCulture()
        {
            base.InitializeCulture();
            // UPDATE THE CURRENCY NEGATIVE PATTERN USING CURRENT CULTURE
            CultureInfo culture = new CultureInfo(Thread.CurrentThread.CurrentCulture.LCID);
            culture.NumberFormat.CurrencyNegativePattern = 1;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string html = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    base.Render(hw);
                    html = sw.ToString();
                    //FIND VIEWSTATE
                    int start = html.IndexOf(@"<input type=""hidden""name=""__VIEWSTATE""");
                    //IF FOUND, MOVE VIEWSTATE TO BOTTOM OF PAGE
                    if (start > -1)
                    {
                        int end = html.IndexOf("/>", start) + 2;
                        //EXTRACT VIEWSTATE
                        string viewstate = html.Substring(start, end - start);
                        html = html.Remove(start, end - start);
                        //INSERT AT END OF FORM
                        int formend = html.IndexOf("") - 1;
                        html = html.Insert(formend, viewstate);
                    }
                    if (!this.PreserveWhitespace)
                    {
                        int bodystart = html.IndexOf("<body");
                        if (bodystart > -1)
                        {
                            int bodyend = html.LastIndexOf("</body>");
                            if (bodyend > -1)
                            {
                                //EXTRACT VIEWSTATE
                                string body = html.Substring(bodystart, bodyend - bodystart);
                                html = html.Remove(bodystart, bodyend - bodystart);
                                //REMOVE WHITESPACE FROM BODY
                                body = Regex.Replace(body, "^\\s+", string.Empty, RegexOptions.Multiline);
                                //FOR NOW LET'S DISABLE REMOVING MULTIPLE LINE BREAKS
                                //THIS MIGHT HAVE A SIGNIFICANT IMPACT ON USER PROVIDED CONENT
                                //LIKE ON EDIT PRODUCT TEXT AREAS (SEE BUG 5233)
                                //TO BE MORE CORRECT, WE SHOULD FIND AND EXTRACT TEXTAREAS
                                //AND NOT PROCESS THAT CONTENT IN THE WHITESPACE COMPRESSION
                                //body = Regex.Replace(body, "(\r\n){2,}", "\r\n");
                                //INSERT BACK INTO PAGE
                                html = html.Insert(bodystart, body);
                            }
                        }
                    }
                    hw.Close();
                }
                sw.Close();
            }
            writer.Write(html);
        }

        protected override void SavePageStateToPersistenceMedium(object state)
        {
            base.SavePageStateToPersistenceMedium(state);
            try
            {
                HttpContext context = HttpContext.Current;
                AbleCommerceApplicationSection appConfig = AbleCommerceApplicationSection.GetSection(context.ApplicationInstance);
                if ((context.Trace.IsEnabled) || (appConfig.ViewStateWarning > 0))
                {
                    //THE SERIALIZATION CODE WILL FAIL IF THE PAGE CONTAINS THE FCKEDITOR
                    //AND THE APP IS RUNNING UNDER MEDIUM TRUST
                    LosFormatter format = new LosFormatter();
                    StringWriter writer = new StringWriter();
                    format.Serialize(writer, state);
                    int viewStateLength = writer.ToString().Length;
                    context.Trace.Warn("ViewState Size", viewStateLength.ToString() + "b");
                    if ((appConfig.ViewStateWarning > 0) && (viewStateLength > appConfig.ViewStateWarning))
                    {
                        string viewStateWarning = string.Format("ViewState Size {0}! Page: {1}", viewStateLength, context.Request.Path);
                        Logger.Error(viewStateWarning);
                    }
                }
            }
            catch { }
        }

        protected void AbleCommercePage_Load(Object sender,EventArgs e) 
        {
            this.Page.Form.Action = HttpContext.Current.Request.RawUrl;
        }
    }
}
