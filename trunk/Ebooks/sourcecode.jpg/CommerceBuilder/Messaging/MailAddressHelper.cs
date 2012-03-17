namespace CommerceBuilder.Messaging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net.Mail;

    /// <summary>
    /// Provides helper functions for working with email addresses
    /// </summary>
    public sealed class MailAddressHelper
    {
        /// <summary>
        /// Convets a list of comma separated email addresses to a list of MailAddress objects
        /// </summary>
        /// <param name="addresses">A comma separated list of email addresses</param>
        /// <returns>A list of MailAddress objects parsed from the comma separated list</returns>
        public static Collection<MailAddress> ParseList(string addresses)
        {
            Collection<MailAddress> addressList = new Collection<MailAddress>();
            if (!string.IsNullOrEmpty(addresses))
            {
                string[] addressArray = addresses.Split(",".ToCharArray());
                foreach (string thisAddress in addressArray)
                {
                    try
                    {
                        MailAddress checkAddress = new MailAddress(thisAddress.Trim());
                        addressList.Add(checkAddress);
                    }
                    catch (FormatException)
                    {
                        // IGNORE INVALID ADDRESSES IN RECIPIENT LIST
                    }
                }
            }
            return addressList;
        }

        public static string ConvertToList(IEnumerable<MailAddress> addresses)
        {
            if (addresses == null) return string.Empty;
            List<string> addressList = new List<string>();
            foreach (MailAddress address in addresses)
            {
                addressList.Add(address.Address);
            }
            if (addressList.Count == 0) return string.Empty;
            return string.Join(",", addressList.ToArray());
        }

        /// <summary>
        /// Checks if the given string is a valid email address according to the format rules 
        /// followed by System.Net.Mail.MailAddress.
        /// </summary>
        /// <param name="email">The email address to validate</param>
        /// <returns><b>true</b> if given email is valid, <b>false</b> otherwise</returns>
        public static bool IsValidEmail(string email, bool useRegex)
        {
            if (string.IsNullOrEmpty(email)) return false;
            bool isValid = true;
            try
            {
                MailAddress address = new MailAddress(email);
            }
            catch (FormatException)
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
