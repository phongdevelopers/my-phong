//-----------------------------------------------------------------------
// <copyright file="SearchKeywordValidator.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;

    public class SearchKeywordValidator : Sample.Web.UI.Compatibility.BaseValidator
    {
        private System.Web.UI.WebControls.TextBox _KeywordField;

        [Description("The minimum length required for the search keyword."), DefaultValue(1)]
        public int MinimumLength
        {
            get
            {
                object o = ViewState["MinimumLength"];
                if (o == null) return 1;
                else return (int)o;
            }
            set
            {
                ViewState["MinimumLength"] = value;
            }
        }

        [Description("Indicates whether a keyword is required."), DefaultValue(false)]
        public bool KeywordRequired
        {
            get
            {
                object o = ViewState["KeywordRequired"];
                if (o == null) return false;
                else return (bool)o;
            }
            set
            {
                ViewState["KeywordRequired"] = value;
            }
        }

        protected override bool ControlPropertiesValid()
        {
            Control control = FindControl(ControlToValidate);
            if (control != null)
            {
                _KeywordField = control as TextBox;
                return (_KeywordField != null);
            }
            else return false;
        }

        protected override bool EvaluateIsValid()
        {
            return EvaluateIsValid(_KeywordField.Text);
        }

        public bool EvaluateIsValid(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return !KeywordRequired;
            string tempKeyword = Regex.Replace(keyword, "[ \\*]", "");
            if (tempKeyword.Length < this.MinimumLength) return false;
            return true;
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            // Add the client-side code (if needed)
            if (this.RenderUplevel)
            {
                // Indicate the mustBeChecked value and the client-side function to used for evaluation
                // Use AddAttribute if Helpers.EnableLegacyRendering is true; otherwise, use expando attributes                
                if (EnableLegacyRendering())
                {
                    writer.AddAttribute("evaluationfunction", "SearchKeywordEvaluateIsValid" + (string)(KeywordRequired ? "Required" : ""), false);
                    writer.AddAttribute("minimumLength", this.MinimumLength.ToString(), false);
                }
                else
                {
                    this.Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "evaluationfunction", "SearchKeywordEvaluateIsValid" + (string)(KeywordRequired ? "Required" : ""), false);
                    this.Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "minimumLength", this.MinimumLength.ToString(), false);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.RenderUplevel && this.Page != null)
            {
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), "SearchKeywordValidator"))
                {
                    string url = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Web.UI.WebControls.SearchKeywordValidator.js");
                    ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "SearchKeywordValidator", url);
                }
            }
        }

        // Replicates the functionality of the internal Page.EnableLegacyRendering property
        private static bool EnableLegacyRendering()
        {
            bool result;
            try
            {
                string webConfigFile = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "web.config");
                XmlTextReader webConfigReader = new XmlTextReader(new StreamReader(webConfigFile));
                result = ((webConfigReader.ReadToFollowing("xhtmlConformance")) && (webConfigReader.GetAttribute("mode") == "Legacy"));
                webConfigReader.Close();
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
