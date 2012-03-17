using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// Utility class for validating postal codes
    /// </summary>
    public static class PostalCodeValidator
    {
        /// <summary>
        /// Does the given postal code match the given pattern
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <param name="postalCode">The postal code to validate</param>
        /// <returns><b>true</b> if postal code matches the given pattern, <b>false</b> otherwise</returns>
        public static bool IsMatch(string pattern, string postalCode)
        {
            if (string.IsNullOrEmpty(pattern)) return true;
            if (string.IsNullOrEmpty(postalCode)) return false;
            //IF STRING STARTS WITH @, IT INDICATES TO USE THE ALTERNATIVE HANDLING
            if (pattern.StartsWith("@"))
            {
                return IsMatch2(pattern.Substring(1), postalCode);
            }
            string[] tokens = pattern.Split(",".ToCharArray());
            foreach (string token in tokens)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    if (token.Contains("*"))
                    {
                        Regex regex = new Regex(token.Replace("*", ".*"),RegexOptions.IgnoreCase);
                        if (regex.IsMatch(postalCode)) return true;
                    }
                    else
                    {
                        if (token.Equals(postalCode)) return true;
                    }
                }
            }
            return false;
        }

        private static bool IsMatch2(string pattern, string postalCode)
        {
            if (string.IsNullOrEmpty(pattern)) return true;
            if (string.IsNullOrEmpty(postalCode)) return false;
            string[] tokens = pattern.Split(";".ToCharArray());
            foreach (string token in tokens)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    Regex regex = new Regex(token, RegexOptions.IgnoreCase);
                    if (regex.IsMatch(postalCode)) return true;
                }
            }
            return false;
        }
    }
}
