using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class to support validation
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates a given parameter
        /// </summary>
        /// <param name="param">parameter to validate</param>
        /// <param name="checkForNull">If <b>true</b> parameter is checked for null</param>
        /// <param name="checkIfEmpty">If <b>true</b> parameter is checked for emptiness</param>
        /// <param name="checkForCommas">If <b>true</b> parameter is checked for commas</param>
        /// <param name="maxSize">The maximum size of the parameter</param>
        /// <returns><b>true</b> if parameter is validated, <b>false</b> otherwise</returns>
        public static bool ValidateParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize)
        {
            if (param == null)
            {
                return !checkForNull;
            }
            param = param.Trim();
            if (((!checkIfEmpty || (param.Length >= 1)) && ((maxSize <= 0) || (param.Length <= maxSize))) && (!checkForCommas || !param.Contains(",")))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Regular expression for email validation
        /// </summary>
        public static string EmailRegex
        {
            get
            {
                return @"^[A-Za-z0-9._%+-]+@([A-Za-z0-9-]+\.)+[A-Za-z]{2,6}$";
            }
        }

        /// <summary>
        /// Regular expression to validate multip comma delimited email addresses
        /// </summary>
        public static string MultiEmailRegex
        {
            get
            {
                return @"^[A-Za-z0-9._%+-]+@([A-Za-z0-9-]+\.)+[A-Za-z]{2,6}(,\s*[A-Za-z0-9._%+-]+@([A-Za-z0-9-]+\.)+[A-Za-z]{2,6})*$";
            }
        }

        /// <summary>
        /// Checks if the given string is a valid email address
        /// </summary>
        /// <param name="email">The email address to validate</param>
        /// <returns><b>true</b> if given email is valid, <b>false</b> otherwise</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            if (email.StartsWith("zz_anonymous_")) return false;
            return Regex.Match(email, ValidationHelper.EmailRegex).Success;
        }

        /// <summary>
        /// Checks if the given steet address is a Post Office Box address
        /// </summary>
        /// <param name="street">The street address to validate</param>
        /// <returns></returns>
        private static Regex _POB1 = new Regex(@"^(P(OST)?\.?\s?O(FFICE)?\.?\s?)?BOX\s\d+(-\d+)?$", RegexOptions.ExplicitCapture);
        private static Regex _POB2 = new Regex(@"^P\.?\s?O\.?\s?B\.?\s?\d+(-\d+)?$", RegexOptions.ExplicitCapture);
        public static bool IsPostOfficeBoxAddress(string street)
        {
            if (!string.IsNullOrEmpty(street))
            {
                street = street.Trim().ToUpperInvariant();
                if (_POB1.IsMatch(street)) return true;
                return _POB2.IsMatch(street);
            }
            return false;
        }
    }
}
