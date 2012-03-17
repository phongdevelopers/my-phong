using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
    [ToolboxData("<{0}:OptionPicker runat=server></{0}:OptionPicker>")]
    public class OptionPicker : WebControl, IPostBackDataHandler
    {
        private Option _Option;

        [Bindable(true), Category("Behavior"), DefaultValue("0"), Localizable(false)]
        public int OptionId
        {
            get
            {
                if (ViewState["OptionId"] == null) return 0;
                else return (int)ViewState["OptionId"];
            }
            set
            {
                ViewState["OptionId"] = value;
            }
        }

        [Bindable(true), Category("Behavior"), DefaultValue("false"), Localizable(false)]
        public bool AutoPostBack
        {
            get
            {
                if (ViewState["AutoPostBack"] == null) return false;
                else return (bool)ViewState["AutoPostBack"];
            }
            set
            {
                ViewState["AutoPostBack"] = value;
            }
        }


        [Bindable(true), Category("Behavior"), DefaultValue("true"), Localizable(false)]
        public bool RenderOptions
        {
            get
            {
                if (ViewState["RenderOptions"] == null) return true;
                else return (bool)ViewState["RenderOptions"];
            }
            set
            {
                ViewState["RenderOptions"] = value;
            }
        }
        
        [Bindable(true), Category("Behavior"), DefaultValue("0"), Localizable(false)]
        public int SelectedChoiceId
        {
            get
            {
                if (ViewState["SelectedChoiceId"] == null) return 0;
                else return (int)ViewState["SelectedChoiceId"];
            }
            set { ViewState["SelectedChoiceId"] = value; }
        }

        private Dictionary<int, int> _SelectedChoices;
        public Dictionary<int, int> SelectedChoices
        {
            get { return _SelectedChoices; }
            set { _SelectedChoices = value; }
        }

        private bool _ForceToLoadAllChoices = false;
        public bool ForceToLoadAllChoices
        {
            get { return _ForceToLoadAllChoices; }
            set { _ForceToLoadAllChoices = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //BUILD SCRIPT TO HANDLE CLIENT CLICK EVENT
            StringBuilder clickScript = new StringBuilder();
            clickScript.Append("\n");
            clickScript.Append("<script type=\"text/javascript\">\n");
            clickScript.Append("    function OptionPickerClick(e, name)\n");
            clickScript.Append("    {\n");
            clickScript.Append("        var td = (e.target) ? e.target : e.srcElement; \n");
            clickScript.Append("        var optid = td.style.zIndex;\n");
            clickScript.Append("        document.getElementById(name).value = td.style.zIndex;\n");
            clickScript.Append("        var t = document.getElementById(name + 'Text')\n");
            clickScript.Append("        if (t.innerText != undefined) { t.innerText = td.title }\n");
            clickScript.Append("        else {t.textContent = td.title}\n");
            clickScript.Append("        var images = eval(name + '_Images');\n");
            clickScript.Append("        if ((images != undefined) && (images[optid] != undefined)) {\n");
            clickScript.Append("            var pImage = document.getElementById('ProductImage');\n");
            clickScript.Append("            if (pImage != undefined) pImage.src = images[optid];\n");
            clickScript.Append("        }\n");
            clickScript.Append("    }\n");
            clickScript.Append("</script>\n");

            //CREATE VARIABLES TO STORE IMAGE ARRAY
            StringBuilder imageScript = new StringBuilder();
            imageScript.Append("<script type=\"text/javascript\">\n");
            imageScript.Append("    " + this.ClientID + "_Images = new Array();\n");

            _Option = OptionDataSource.Load(this.OptionId);
            if (_Option != null)
            {
                //CHECK FOR LARGER IMAGES
                foreach (OptionChoice choice in _Option.Choices)
                {
                    if (!string.IsNullOrEmpty(choice.ImageUrl))
                    {
                        imageScript.Append("    " + this.ClientID + "_Images[" + choice.OptionChoiceId.ToString() + "] = '" + this.Page.ResolveUrl(choice.ImageUrl) + "';\n");
                    }
                }
            }
            imageScript.Append("</script>\n");

            //TRY TO GET SCRIPT MANAGER FOR CURRENT PAGE
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            //ADD SCRIPTS TO PAGE
            if (scriptManager != null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "OptionPickerClick", clickScript.ToString(), false);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), this.ClientID, imageScript.ToString(), false);
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OptionPickerClick", clickScript.ToString());
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ClientID, imageScript.ToString());
            }
        }

        private OptionChoice GetSelectedChoice(OptionChoiceCollection options)
        {
            foreach (OptionChoice choice in options)
            {
                if (choice.OptionChoiceId == this.SelectedChoiceId) return choice;
            }
            return null;
        }

        protected override void Render(HtmlTextWriter output)
        {
            StringBuilder html = new StringBuilder();
            if (this.RenderOptions)
            {
                if (_Option != null)
                {
                    int productId = 0;
                    if (_Option.ProductOptions.Count > 0) productId = _Option.ProductOptions[0].ProductId;
                    OptionChoiceCollection availableChoices = null;
                    if (ForceToLoadAllChoices)
                        availableChoices = _Option.Choices;
                    else
                        availableChoices = OptionChoiceDataSource.GetAvailableChoices(productId, _Option.OptionId, _SelectedChoices);
                    if (availableChoices.Count > 0)
                    {
                        //ADD REQUIRED STYLING FOR SWATCH
                        //SET THE HEIGHT AND WIDTH
                        html.Append("<style>\n");
                        html.Append("     ." + this.ClientID + " td { padding:0px !important;width:" + _Option.ActiveThumbnailWidth + "px;height:" + _Option.ActiveThumbnailHeight + "px;cursor:pointer; }\n");
                        html.Append("</style>\n");
                        html.Append("<div class=\"" + this.ClientID);
                        if (!string.IsNullOrEmpty(this.CssClass)) html.Append(" " + this.CssClass);
                        html.Append("\">\n");

                        //OUTPUT THE CURRENT SELECTION
                        html.Append("<span id=\"" + this.ClientID + "Text\">");
                        OptionChoice selOpt = GetSelectedChoice(availableChoices);
                        if (selOpt != null) html.Append(selOpt.Name);
                        html.Append("</span>\n");
                        
                        //OUTPUT THE SWATCH PICKER
                        html.Append("     <table cellpadding=\"0\" cellspacing=\"2\" onclick=\"javascript:OptionPickerClick(event, '" + this.ClientID + "');");
                        if (this.AutoPostBack)
                        {
                            PostBackOptions options = new PostBackOptions(this, string.Empty);
                            options.AutoPostBack = true;
                            html.Append(this.Page.ClientScript.GetPostBackEventReference(options, true));
                        }
                        html.Append("\">\n");
                        for (int i = 0; i < availableChoices.Count; i++)
                        {
                            if (i % _Option.ActiveThumbnailColumns == 0)
                            {
                                if (i > 0) html.Append("            </tr>\n");
                                html.Append("            <tr>\n");
                            }
                            OptionChoice choice = availableChoices[i];
                            html.Append("                <td");
                            if (choice.OptionChoiceId == this.SelectedChoiceId) html.Append(" class=\"selected\"");
                            html.Append(" title=\"" + HttpUtility.HtmlEncode(choice.Name) + "\"");
                            html.Append(" style=\"background-image:url(" + this.Page.ResolveUrl(choice.ThumbnailUrl) + ");z-index:" + choice.OptionChoiceId.ToString() + "\">&nbsp;</td>\n");
                        }
                        html.Append("            </tr>\n");
                        html.Append("     </table>\n");
                        html.Append("</div>\n");
                    }
                    else
                    {
                        html.Append("(no options available)");
                    }
                    html.Append("<input type=\"hidden\" id=\"" + this.ClientID + "\" name=\"" + this.UniqueID + "\" value=\"" + this.SelectedChoiceId.ToString() + "\">\n");
                }
                else
                {
                    WebTrace.Write(this.UniqueID, "Invalid Option " + this.OptionId.ToString());
                }
            }
            output.Write(html.ToString());
        }

        public bool LoadPostData(String postDataKey, NameValueCollection values)
        {
            SelectedChoiceId = AlwaysConvert.ToInt(values[this.UniqueID]);
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
        }
    }
}