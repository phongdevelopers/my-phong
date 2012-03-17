using System;
using CommerceBuilder.Shipping;
using System.Text.RegularExpressions;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility helper class for formatting addresses
    /// </summary>
    public class AddressFormatter
    {
        /// <summary>
        /// Formats the address using the default address format for the given country.
        /// </summary>
        /// <param name="name">The name in address</param>
        /// <param name="company">The company name</param>
        /// <param name="address1">Address line 1</param>
        /// <param name="address2">Address line 2</param>
        /// <param name="city">The city</param>
        /// <param name="province">The province</param>
        /// <param name="postalCode">The postal code</param>
        /// <param name="countryCode">The country code</param>
        /// <param name="phone">The phone number</param>
        /// <param name="fax">The fax number</param>
        /// <param name="email">The email address</param>
        /// <param name="isHtml">If true address is formatted in HTML otherwise plain text.</param>
        /// <returns>Formatted address</returns>
        public static string Format(string name, string company, string address1, string address2, string city, string province, string postalCode, string countryCode, string phone, string fax, string email, bool isHtml)
        {
            string pattern = string.Empty;
            string countryName = countryCode;

            //NEED TO GET FORMAT FOR pattern FOR THIS COUNTRY
            Country country = CountryDataSource.Load(countryCode);
            if (country != null)
            {
                //GET pattern FORMAT FROM COUNTRY RECORD
                pattern = country.AddressFormat;
                countryName = country.Name;
            }

            return AddressFormatter.Format(pattern, name, company, address1, address2, city, province, postalCode, countryName, phone, fax, email, isHtml);
        }

        /// <summary>
        /// Formats the address using the given format pattern.
        /// </summary>
        /// <param name="pattern">The format pattern</param>
        /// <param name="name">The name in address</param>
        /// <param name="company">The company name</param>
        /// <param name="address1">Address line 1</param>
        /// <param name="address2">Address line 2</param>
        /// <param name="city">The city</param>
        /// <param name="province">The province</param>
        /// <param name="postalCode">The postal code</param>
        /// <param name="country">The country name</param>
        /// <param name="phone">The phone number</param>
        /// <param name="fax">The fax number</param>
        /// <param name="email">The email address</param>
        /// <param name="isHtml">If true address is formatted in HTML otherwise plain text.</param>
        /// <returns>Formatted address</returns>
        public static string Format(string pattern, string name, string company, string address1, string address2, string city, string province, string postalCode, string country, string phone, string fax, string email, bool isHtml)
        {
            //if no pattern is provided, use US as default
            if (string.IsNullOrEmpty(pattern)) 
                pattern = "[Company]\r\n[Name]\r\n[Address1]\r\n[Address2]\r\n[City_U], [Province_U] [PostalCode]\r\n[Country_U]";

            //REPLACE MIXED CASE PATTERNS
            pattern = pattern.Replace("[Company]", company);
            pattern = pattern.Replace("[Name]", name);
            pattern = pattern.Replace("[Address1]", address1);
            pattern = pattern.Replace("[Address2]", address2);
            pattern = pattern.Replace("[City]", city);
            pattern = pattern.Replace("[Province]", province);
            pattern = pattern.Replace("[PostalCode]", postalCode);
            pattern = pattern.Replace("[Country]", country);
            pattern = pattern.Replace("[Phone]", phone);
            pattern = pattern.Replace("[Fax]", fax);
            pattern = pattern.Replace("[Email]", email);
            //REPLACE UPPPERCASE PATTERNS
            pattern = pattern.Replace("[Company_U]", SafeToUpper(company));
            pattern = pattern.Replace("[Name_U]", SafeToUpper(name));
            pattern = pattern.Replace("[Address1_U]", SafeToUpper(address1));
            pattern = pattern.Replace("[Address2_U]", SafeToUpper(address2));
            pattern = pattern.Replace("[City_U]", SafeToUpper(city));
            pattern = pattern.Replace("[Province_U]", SafeToUpper(province));
            pattern = pattern.Replace("[PostalCode_U]", SafeToUpper(postalCode));
            pattern = pattern.Replace("[Country_U]", SafeToUpper(country));
            pattern = pattern.Replace("[Phone_U]", SafeToUpper(phone));
            pattern = pattern.Replace("[Fax_U]", SafeToUpper(fax));
            pattern = pattern.Replace("[Email_U]", SafeToUpper(email));

            //REPLACE MULTIPLE LINE BREAKS
            pattern = Regex.Replace(pattern, "(\\r\\n)+", "\r\n");
            //TRIM LINE BREAKS FROM ENDS
            pattern = pattern.TrimEnd("\r\n".ToCharArray());
            pattern = pattern.TrimStart("\r\n".ToCharArray());
            if (isHtml) pattern = Regex.Replace(pattern, "(\\r\\n)+", "<br />");
            return pattern;
        }

        private static string SafeToUpper(string value)
        {
            if (value == null) return string.Empty;
            return value.ToUpperInvariant();
        }
    }
}
