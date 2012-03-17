//-----------------------------------------------------------------------
// <copyright file="OptionPickerValidator.cs" company="Able Solutions Corporation">
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

    public class OptionPickerValidator : Sample.Web.UI.Compatibility.BaseValidator
    {
        OptionPicker _OptionPicker = null;
        protected override bool ControlPropertiesValid()
        {
            OptionPicker control = FindControl(ControlToValidate) as OptionPicker;
            if (control != null)
            {
                _OptionPicker = control;
                return (_OptionPicker != null);
            }
            else return false;
        }

        protected override bool EvaluateIsValid()
        {
            return (_OptionPicker.SelectedChoiceId != 0);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            // No client-side code             
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
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
