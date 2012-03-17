using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// Interface to be implemented by Forex Providers
    /// </summary>
    public interface IForexProvider
    {
        /// <summary>
        /// Helper property to return the fully qualified assembly name
        /// </summary>
        string ClassId { get; }

        /// <summary>
        /// Return the friendly name of the provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Helper property to return the version of the assembly
        /// </summary>
        string Version { get;}

        /// <summary>
        /// Get the exchange rate from the source currency to the target currency (1 source = ? target)
        /// </summary>
        /// <param name="sourceCurrency">3 letter ISO 4127 currency code for the source currency</param>
        /// <param name="targetCurrency">3 letter ISO 4127 currency code for the target currency</param>
        /// <returns>The exchange rate from the source currency to the target currency (1 source = ? target)</returns>
        LSDecimal GetExchangeRate(string sourceCurrency, string targetCurrency);
    }
}
