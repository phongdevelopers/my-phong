using System;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Class that represents a Gift Certificate
    /// </summary>
    public partial class GiftCertificate
    {
        /// <summary>
        /// Is this gift certificate expired?
        /// </summary>
        /// <returns><b>true</b> if the gift certificate is expired, <b>false</b> otherwise</returns>
        public bool IsExpired()
        {
            if (this.ExpirationDate == null 
                || this.ExpirationDate.Equals(DateTime.MinValue))
            {
                return false;
            }

            return this.ExpirationDate <= LocaleHelper.LocalNow;
        }

        /// <summary>
        /// Adds a new transaction to the gift certificate for updating its balance
        /// </summary>
        /// <param name="oldBalance">The old balance</param>
        /// <param name="newBalance">The new balance</param>
        public void AddBalanceUpdatedTransaction(LSDecimal oldBalance, LSDecimal newBalance)
        {
            GiftCertificateTransaction trans = new GiftCertificateTransaction();
            trans.Amount = newBalance - oldBalance;
            trans.Description = string.Format("Gift certificate balance updated manually from {0:lc} to {1:lc}.", oldBalance,newBalance);
            trans.TransactionDate = LocaleHelper.LocalNow;
            this.Transactions.Add(trans);
        }

        /// <summary>
        /// Adds a new transaction to the gift certificate for updating its expiry date
        /// </summary>
        /// <param name="oldDate">The old expiry date</param>
        /// <param name="newDate">The new expiry date</param>
        public void AddExpiryUpdatedTransaction(DateTime oldDate, DateTime newDate)
        {
            GiftCertificateTransaction trans = new GiftCertificateTransaction();
            trans.Amount = this.Balance;
            trans.Description = string.Format("Gift certificate expire date updated manually from {0:d} to {1:d}.", oldDate, newDate);
            trans.TransactionDate = LocaleHelper.LocalNow;
            this.Transactions.Add(trans); 
        }

        /// <summary>
        /// Add a new transaction to the gift certificate activating the gift certificate in the process
        /// </summary>
        public void AddActivatedTransaction()
        {
            GiftCertificateTransaction trans = new GiftCertificateTransaction();
            trans.Amount = this.Balance;
            trans.Description = "Gift certificate activated.";
            trans.TransactionDate = LocaleHelper.LocalNow;
            this.Transactions.Add(trans);
        }

        /// <summary>
        /// Add a new transaction to the gift certificate de-activating the gift certificate in the process
        /// </summary>
        public void AddDeactivatedTransaction()
        {
            GiftCertificateTransaction trans = new GiftCertificateTransaction();
            trans.Amount = this.Balance;
            trans.Description = "Gift certificate deactivated.";
            trans.TransactionDate = LocaleHelper.LocalNow;
            this.Transactions.Add(trans);
        }

        /// <summary>
        /// Add a new transaction to the gift certificate indicating that the gift certificate is created manually
        /// </summary>
        public void AddCreatedManuallyTransaction()
        {
            GiftCertificateTransaction trans = new GiftCertificateTransaction();
            trans.Amount = this.Balance;
            trans.Description = "Gift certificate created manually.";
            trans.TransactionDate = LocaleHelper.LocalNow;            
            this.Transactions.Add(trans);
        }

        /// <summary>
        /// Add a new transaction to the gift certificate indicating that the gift certificate is created
        /// </summary>
        public void AddCreatedTransaction()
        {
            GiftCertificateTransaction trans = new GiftCertificateTransaction();
            trans.Amount = this.Balance;
            trans.Description = "Gift certificate created.";
            trans.TransactionDate = LocaleHelper.LocalNow;
            this.Transactions.Add(trans);
        }

    }
}
