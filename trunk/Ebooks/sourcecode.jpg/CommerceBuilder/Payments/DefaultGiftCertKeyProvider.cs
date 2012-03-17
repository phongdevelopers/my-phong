using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Default gift certificate key generator that generates gift certificates similar to
    /// credit card numbers. The keys can be validated with Luhn/mod10 
    /// </summary>
    public class DefaultGiftCertKeyProvider:IGiftCertKeyProvider
    {
        /// <summary>
        /// Generate a gift certificate key
        /// </summary>
        /// <returns>The gift certificate key generated</returns>
        public string GenerateGiftCertificateKey()
        {
            Random random = new Random();
            string serialKey = GenerateRandomKey(random);
            while (!IsUnique(serialKey))
            {
                serialKey = GenerateRandomKey(random);
            }
            return serialKey;
        }

        /// <summary>
        /// Validates the given gift certificate key
        /// </summary>
        /// <param name="key">The key to validate</param>
        /// <returns><b>true</b> if key is valid, <b>false</b> otherwise</returns>
        public bool ValidateGiftCertificateKey(string key)
        {
            //1. Length should be 16.
            //2. Card number must start with '7'
            //3. Must be validated with Luhn/mod10
            
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            //remove any '-' from the key
            key = key.Trim().Replace("-","");
            if (key.Length != 16)
            {
                return false;
            }

            if (!key.StartsWith("7"))
            {
                return false;
            }

            //Luhn/mod10 : same as credit card numbers
            return ValidateCardNumber(key);
        }

        private static bool IsUnique(string serialKey)
        {
            GiftCertificate gc = GiftCertificateDataSource.LoadForSerialNumber(serialKey);
            return gc == null;
        }

        private static string GenerateRandomKey(Random random)
        {            
            StringBuilder sb = new StringBuilder();
            // 7 -- 16 length
            sb.Append("7");
            sb.Append(RandomNumber(random,14));
            sb.Append(CalculateLastDigit(sb.ToString()));            
            return sb.ToString();
        }

        private static int CalculateLastDigit(string cardNumber)
        {
            int cardLength = cardNumber.Length;
            int multiplier, digit, sum, total = 0;
            for (int i = 1; i <= cardLength; i++)
            {
                multiplier = 1 + (i % 2);
                digit = AlwaysConvert.ToInt(cardNumber.Substring(i - 1, 1));
                sum = digit * multiplier;
                if (sum > 9) sum -= 9;
                total += sum;
            }
            return (10 - (total % 10));
        }

        private static string RandomNumber(Random random, int size)
        {
            const string charSet = "1234567890";
            const int charSetLength = 10;
            StringBuilder sb = new StringBuilder();            
            for (int i = 0; i < size; i++)
            {
                sb.Append(charSet.Substring(random.Next(0, charSetLength), 1));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Validates a credit card number using the standard Luhn/mod10
        /// validation algorithm.
        /// </summary>
        /// <param name="cardNumber">Card number, without punctuation or spaces.</param>
        /// <returns>True if card number appears valid, false if not
        /// </returns>
        private static bool ValidateCardNumber(string cardNumber)
        {
            try
            {
                if (String.IsNullOrEmpty(cardNumber)) return false;
                int cardLength = cardNumber.Length;
                if ((cardLength < 9) || (cardLength > 19)) return false;
                int multiplier = 1;
                int digit, sum, total = 0;
                for (int i = cardLength; i > 0; i--)
                {
                    digit = AlwaysConvert.ToInt(cardNumber.Substring(i - 1, 1));
                    sum = digit * multiplier;
                    if (sum > 9) sum -= 9;
                    total += sum;
                    multiplier = (multiplier == 1 ? 2 : 1);
                }
                return (total % 10 == 0);
            }
            catch
            {
                return false;
            }
        }

    }
}
