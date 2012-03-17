using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Common
{
    /// <summary>
    /// A Dictionary class with string keys and values. The values are kept in Url encoded format
    /// </summary>
    public class UrlEncodedDictionary : Dictionary<string, string>
    {
       /// <summary>
       /// Default constructor
       /// </summary>
        public UrlEncodedDictionary() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialValue">Initial value that could be a URL encoded name value parameter string</param>
        public UrlEncodedDictionary(string initialValue)
            : base()
        {
            this.Parse(initialValue);
        }

        /// <summary>
        /// Returns a value from the dictionary for the given key.
        /// </summary>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <returns>The value stored for the key, or string.Empty if the key is not present in the dictionary.</returns>
        /// <remarks>This is useful over the base TryGetValue if you do not care whether the item is found, you can 
        /// just obtain a return value or string.empty as default rather than using a local out variable.</remarks>
        public string TryGetValue(string key)
        {
            string returnValue;
            this.TryGetValue(key, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Write the dictionary to a URL encoded string of name/value pairs.
        /// </summary>
        /// <returns>A URL encoded string of name/value pairs.</returns>
        public override string ToString()
        {
            StringBuilder pairs = new StringBuilder();
            string delimiter = string.Empty;
            foreach (string key in this.Keys)
            {
                pairs.Append(delimiter);
                pairs.Append(key + "=" + System.Web.HttpUtility.UrlEncode(this[key]));
                delimiter = "&";
            }
            return pairs.ToString();
        }

        /// <summary>
        /// Load a URL encoded string of name/value pairs into the dictionary.
        /// </summary>
        /// <param name="value">A URL encoded string.</param>
        public void Parse(string value)
        {
            this.Clear();
            if (!string.IsNullOrEmpty(value))
            {
                string[] pairs = value.Split("&".ToCharArray());
                foreach (string thisPair in pairs)
                {
                    if (!string.IsNullOrEmpty(thisPair) && thisPair.Contains("="))
                    {
                        string[] keyValue = thisPair.Split("=".ToCharArray());
                        this[keyValue[0]] = System.Web.HttpUtility.UrlDecode(keyValue[1]);
                    }
                }
            }
        }
    }
}