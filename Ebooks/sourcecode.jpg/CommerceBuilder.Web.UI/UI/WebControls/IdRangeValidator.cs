//-----------------------------------------------------------------------
// <copyright file="OrderRangeValidator.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Web.UI.WebControls
{
    using System;
    using Sample.Web.UI.Compatibility;
    using CommerceBuilder.Utility;
    using CommerceBuilder.Search;
    using System.ComponentModel;

    /// <summary>
    /// Validator for ID ranges so a common regular expression can be employed
    /// Using the "Required" properly we can define if the ID number range is optional or required
    /// </summary>
    public class IdRangeValidator : RequiredRegularExpressionValidator
    {
        /// <summary>
        /// Initializes a new instance of the OrderRangeValidator class.
        /// </summary>
        public IdRangeValidator() : base()
        {
            this.ValidationExpression = IdRangeParser.RangeRegex.ToString();
        }
    }
}