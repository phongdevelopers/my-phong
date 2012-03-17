//-----------------------------------------------------------------------
// <copyright file="EmailAddressValidator.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Web.UI.WebControls
{
    using System;
    using System.Text.RegularExpressions;
    using CommerceBuilder.Utility;
    using CommerceBuilder.Catalog;
    using System.ComponentModel;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Validator for email addresses so a common regular expression can be employed
    /// Using the "Required" properly we can define if the email address is optional or required.
    /// </summary>
    public class CustomUrlValidator : Sample.Web.UI.Compatibility.RegularExpressionValidator
    {
        /// <summary>
        /// Initializes a new instance of the EmailAddressValidator class.
        /// </summary>
        public CustomUrlValidator() : base()
        {
            this.ValidationExpression = "^([A-Za-z0-9_\\-\\.\\+%]+)(/[A-Za-z0-9_\\-\\.\\+%]+)*/?$";
        }

        [DefaultValue(""), Category("Data")]
        public string OriginalValue
        {
            get
            {
                object originalValue = this.ViewState["OriginalValue"];
                if (originalValue != null)
                {
                    return (string)originalValue;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["OriginalValue"] = value;
            }
        }

        [DefaultValue(""), Category("Behavior")]
        public new string ErrorMessage
        {
            get { return base.ErrorMessage; }
            protected set { base.ErrorMessage = value; }
        }

        [DefaultValue(""), Category("Behavior")]
        public string FormatErrorMessage
        {
            get
            {
                object FormatErrorMessage = this.ViewState["FormatErrorMessage"];
                if (FormatErrorMessage != null)
                {
                    return (string)FormatErrorMessage;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["FormatErrorMessage"] = value;
                this.ErrorMessage = value;
            }
        }

        [DefaultValue(""), Category("Behavior")]
        public string DuplicateErrorMessage
        {
            get
            {
                object DuplicateErrorMessage = this.ViewState["DuplicateErrorMessage"];
                if (DuplicateErrorMessage != null)
                {
                    return (string)DuplicateErrorMessage;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["DuplicateErrorMessage"] = value;
            }
        }

        public bool IsValidFormat(string customUrl)
        {
            if (string.IsNullOrEmpty(customUrl)) return true;
            return Regex.IsMatch(customUrl, this.ValidationExpression);
        }

        /// <summary>
        /// Indicates whether the given value is unique in the database
        /// </summary>
        /// <param name="customUrl"></param>
        /// <returns></returns>
        public bool IsUnique(string customUrl)
        {
            // is a non-empty value provided?
            if (!string.IsNullOrEmpty(customUrl))
            {
                // is the new value different from the existing value?
                if (this.OriginalValue.ToUpperInvariant() != customUrl.ToUpperInvariant())
                {
                    // make sure the new value does not appear in the database
                    return !CustomUrlDataSource.IsAlreadyUsed(customUrl);
                }
            }
            return true;
        }

        /// <summary>Indicates whether the value in the input control is valid.</summary>
        /// <returns>true if the value in the input control is valid; otherwise, false.</returns>
        protected override bool EvaluateIsValid()
        {
            string customUrl = base.GetControlValidationValue(base.ControlToValidate);
            if (!this.IsValidFormat(customUrl))
            {
                this.ErrorMessage = this.FormatErrorMessage;
                return false;
            }
            if (!this.IsUnique(customUrl))
            {
                this.ErrorMessage = this.DuplicateErrorMessage;
                return false;
            }
            return true;
        }
    }
}
