using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Contains payment account data
    /// </summary>
    public class AccountDataDictionary : Dictionary<string,string>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AccountDataDictionary() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialValue">A URL encoded string of name/value pairs</param>
        public AccountDataDictionary(string initialValue) : base()
        {
            this.Parse(initialValue);
        }

        /// <summary>
        /// Returns a value from the dictionary for the given key.
        /// </summary>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <returns>The value stored for the key, or string.Empty if the key is not present in the dictionary.</returns>
        public string GetValue(string key)
        {
            if (this.ContainsKey(key)) return this[key];
            return string.Empty;
        }

        /// <summary>
        /// Write the dictionary to a URL encoded string of name/value pairs.
        /// </summary>
        /// <returns>A URL encoded string of name/value pairs.</returns>
        public override string ToString()
        {
            List<string> pairs = new List<string>();
            foreach (string key in this.Keys)
            {
                pairs.Add(key + "=" + System.Web.HttpUtility.UrlEncode(this[key]));
            }
            return String.Join("&", pairs.ToArray());
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
