using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Interface that a Gift Certificate Key Provider must implement
    /// </summary>
    public interface IGiftCertKeyProvider
    {
        /// <summary>
        /// Generate a gift certificate key
        /// </summary>
        /// <returns>The gift certificate key generated</returns>
        string GenerateGiftCertificateKey();

        /// <summary>
        /// Validate a given gift certificate key
        /// </summary>
        /// <param name="key">The key to validate.</param>
        /// <returns><b>true</b> if key is valid, <b>false</b> otherwise</returns>
        bool ValidateGiftCertificateKey(string key);
    }
}
