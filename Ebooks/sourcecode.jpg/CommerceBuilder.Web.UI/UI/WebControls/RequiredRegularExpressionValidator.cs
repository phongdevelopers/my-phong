namespace CommerceBuilder.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>Validates whether the value of an associated input control matches the pattern specified by a regular expression.</summary>
    [ToolboxData("<{0}:RequiredRegularExpressionValidator runat=\"server\" ErrorMessage=\"RequiredRegularExpressionValidator\"></{0}:RequiredRegularExpressionValidator>"), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class RequiredRegularExpressionValidator : Sample.Web.UI.Compatibility.BaseValidator
    {


        private const string ValidatorScriptBlock = @"
<script language=""javascript"">
<!--

function RequiredRegularExpressionValidatorEvaluateIsValid(val) {
    var value = ValidatorTrim(ValidatorGetValue(val.controltovalidate));
    if (value.length == 0) return (!val.required);
    var rx = new RegExp(val.validationexpression);
    var matches = rx.exec(value);
    return (matches != null && value == matches[0]);
}

// -->
</script>
        ";

        public RequiredRegularExpressionValidator()
            : base()
        {
            this.Init += new EventHandler(RequiredRegularExpressionValidator_Init);
        }

        private void RequiredRegularExpressionValidator_Init(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RequiredRegularExpressionValidatorEvaluateIsValid", ValidatorScriptBlock, false);
        }

        /// <summary>Adds to the specified <see cref="T:System.Web.UI.HtmlTextWriter"></see> object the HTML attributes and styles that need to be rendered for the control. </summary>
        /// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter"></see> that represents the output stream to render HTML content on the client.</param>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            if (base.RenderUplevel)
            {
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "evaluationfunction", (this.Required ? "Required" : String.Empty) + "RegularExpressionValidatorEvaluateIsValid", true);
                if (this.ValidationExpression.Length > 0)
                {
                    ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "validationexpression", this.ValidationExpression, true);
                }
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "required", this.Required.ToString(), true);
            }
        }

        /// <summary>Indicates whether the value in the input control is valid.</summary>
        /// <returns>true if the value in the input control is valid; otherwise, false.</returns>
        protected override bool EvaluateIsValid()
        {
            bool isValid;
            string controlValue = base.GetControlValidationValue(base.ControlToValidate);
            if ((controlValue == null) || (controlValue.Trim().Length == 0))
            {
                //invalid value if input required
                return !this.Required;
            }
            try
            {
                Match match = Regex.Match(controlValue, this.ValidationExpression);
                isValid = (match.Success && (match.Index == 0)) && (match.Length == controlValue.Length);
            }
            catch
            {
                isValid = true;
            }
            return isValid;
        }


        [DefaultValue(false), Category("Behavior")]
        public bool Required
        {
            get
            {
                object required = this.ViewState["Required"];
                if (required != null)
                {
                    return (bool)required;
                }
                return false;
            }
            set
            {
                this.ViewState["Required"] = value;
            }
        }

        /// <summary>Gets or sets the regular expression that determines the pattern used to validate a field.</summary>
        /// <returns>A string that specifies the regular expression used to validate a field for format. The default is <see cref="F:System.String.Empty"></see>.</returns>
        /// <exception cref="T:System.Web.HttpException">The regular expression is not properly formed. </exception>
        [Editor("System.Web.UI.Design.WebControls.RegexTypeEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), Description("RegularExpressionValidator_ValidationExpression"), Category("Behavior"), Themeable(false)]
        public string ValidationExpression
        {
            get
            {
                object obj1 = this.ViewState["ValidationExpression"];
                if (obj1 != null)
                {
                    return (string)obj1;
                }
                return string.Empty;
            }
            set
            {
                try
                {
                    Regex.IsMatch(string.Empty, value);
                }
                catch (Exception ex)
                {
                    throw new HttpException("Invalid Regular Expression", ex);
                }
                this.ViewState["ValidationExpression"] = value;
            }
        }

    }
}