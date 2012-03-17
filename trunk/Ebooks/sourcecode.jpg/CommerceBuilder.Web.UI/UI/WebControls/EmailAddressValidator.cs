//-----------------------------------------------------------------------
// <copyright file="EmailAddressValidator.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Web.UI.WebControls
{
    using System;
    using Sample.Web.UI.Compatibility;
    using CommerceBuilder.Utility;
    using System.ComponentModel;

    /// <summary>
    /// Validator for email addresses so a common regular expression can be employed
    /// Using the "Required" properly we can define if the email address is optional or required.
    /// </summary>
    public class EmailAddressValidator : RequiredRegularExpressionValidator
    {
        /// <summary>
        /// Initializes a new instance of the EmailAddressValidator class.
        /// </summary>
        public EmailAddressValidator() : base()
        {
            this.ValidationExpression = ValidationHelper.EmailRegex;
        }

        [Description("Indicates whether multiple email addresses are allowed."), DefaultValue(false)]
        public bool AllowMultpleAddresses
        {
            get 
            {
                object o = ViewState["AllowMultpleAddresses"];
                if (o == null) return false;
                else return (bool)o;
            }
            set 
            { 
                ViewState["AllowMultpleAddresses"] = value;
                if (value) this.ValidationExpression = ValidationHelper.MultiEmailRegex;
                else this.ValidationExpression = ValidationHelper.EmailRegex;
            }
        }
    }
}
