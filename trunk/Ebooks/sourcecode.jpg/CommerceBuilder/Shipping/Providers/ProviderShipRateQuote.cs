using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Shipping.Providers
{
    /// <summary>
    /// Class that holds a provider ship rate quote data
    /// </summary>
    public class ProviderShipRateQuote : ShipRateQuote
    {
        /// <summary>
        /// The service code
        /// </summary>
        public string ServiceCode;

        /// <summary>
        /// the service name
        /// </summary>
        public string ServiceName;

        /// <summary>
        /// The number of packages
        /// </summary>
        public int PackageCount;

        /// <summary>
        /// Adds a provider ship rate quote to this quote
        /// </summary>
        /// <param name="quote">The quote to add to this quote</param>
        public void AddPackageQoute(ProviderShipRateQuote quote)
        {
            this.Rate = this.Rate + quote.Rate;
            this.AddWarnings(quote.Warnings);
            PackageCount++;
        }
    }
}
