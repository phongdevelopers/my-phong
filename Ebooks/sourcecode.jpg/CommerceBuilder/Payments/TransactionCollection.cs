namespace CommerceBuilder.Payments
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;
    using CommerceBuilder.Users;
    using CommerceBuilder.Utility;

    /// <summary>
    /// This class implements a PersistentCollection of Transaction objects.
    /// </summary>
    public partial class TransactionCollection
    {

        /// <summary>
        /// Gets the last authorization transaction in this collection
        /// </summary>
        public Transaction LastAuthorization
        {
            get
            {
                int index = this.Count - 1;
                for (index = this.Count - 1; index >= 0; index--)
                {
                    if ((this[index].TransactionType == TransactionType.Authorize
                        || this[index].TransactionType == TransactionType.AuthorizeCapture
                        ) && (this[index].TransactionStatus == TransactionStatus.Successful))
                    {
                        return this[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last pending authorization transaction in this collection
        /// </summary>
        public Transaction LastAuthorizationPending
        {
            get
            {
                int index = this.Count - 1;
                for (index = this.Count - 1; index >= 0; index--)
                {
                    if ((this[index].TransactionType == TransactionType.Authorize
                        || this[index].TransactionType == TransactionType.AuthorizeCapture
                        ) && (this[index].TransactionStatus == TransactionStatus.Pending))
                    {
                        return this[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last recurring authorization transaction in this collection
        /// </summary>
        public Transaction LastRecurringAuthorization
        {
            get
            {
                int index = this.Count - 1;
                for (index = this.Count - 1; index >= 0; index--)
                {
                    if ((this[index].TransactionType == TransactionType.AuthorizeRecurring) && (this[index].TransactionStatus == TransactionStatus.Successful))
                    {
                        return this[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last capture transaction in this collection
        /// </summary>
        public Transaction LastCapture
        {
            get
            {
                int index = this.Count - 1;
                for (index = this.Count - 1; index >= 0; index--)
                {
                    if ((this[index].TransactionType == TransactionType.AuthorizeCapture
                          || this[index].TransactionType == TransactionType.Capture
                          || this[index].TransactionType == TransactionType.PartialCapture
                          ) && (this[index].TransactionStatus == TransactionStatus.Successful))
                    {
                        return this[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last pending capture transaction in this collection
        /// </summary>
        public Transaction LastCapturePending
        {
            get
            {
                int index = this.Count - 1;
                for (index = this.Count - 1; index >= 0; index--)
                {
                    if ((this[index].TransactionType == TransactionType.Capture)
                        || (this[index].TransactionType == TransactionType.PartialCapture)
                        && (this[index].TransactionStatus == TransactionStatus.Pending))
                    {
                        return this[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last pending refund transaction in this collection
        /// </summary>
        public Transaction LastRefundPending
        {
            get
            {
                int index = this.Count - 1;
                for (index = this.Count - 1; index >= 0; index--)
                {
                    if ((this[index].TransactionType == TransactionType.Refund)
                        || (this[index].TransactionType == TransactionType.PartialRefund)
                        && (this[index].TransactionStatus == TransactionStatus.Pending))
                    {
                        return this[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last pending void transaction in this collection
        /// </summary>
        public Transaction LastVoidPending
        {
            get
            {
                int index = this.Count - 1;
                for (index = this.Count - 1; index >= 0; index--)
                {
                    if ((this[index].TransactionType == TransactionType.Void)                        
                        && (this[index].TransactionStatus == TransactionStatus.Pending))
                    {
                        return this[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the total value of captured transactions in this collection
        /// </summary>
        /// <returns>Total value of captured transactions</returns>
        public LSDecimal GetTotalCaptured()
        {
            LSDecimal total = 0;
            foreach (Transaction tx in this)
            {
                if (tx.TransactionStatus == TransactionStatus.Successful)
                {
                    switch (tx.TransactionType)
                    {
                        case TransactionType.Authorize:
                        case TransactionType.Void:
                        case TransactionType.CancelRecurring:
                        case TransactionType.AuthorizeRecurring:
                            //do not include in total
                            break;
                        case TransactionType.PartialCapture:
                        case TransactionType.AuthorizeCapture:
                        case TransactionType.Capture:
                            //include in total
                            total += tx.Amount;
                            break;
                        case TransactionType.PartialRefund:
                        case TransactionType.Refund:
                            //subtract from total
                            total -= tx.Amount;
                            break;
                        default:
                            //THIS SHOULD NEVER HAPPEN IF ALL TRANSACTION TYPES ARE SPECIFIED ABOVE
                            throw new ArgumentOutOfRangeException("Invalid Transaction Type : " + tx.TransactionType.ToString());
                    }
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the total value of authorized transactions in this collection
        /// </summary>
        /// <returns>The total value of authorized transactions in this collection</returns>
        public LSDecimal GetTotalAuthorized()
        {
            LSDecimal total = 0;
            foreach (Transaction tx in this)
            {
                if (tx.TransactionStatus == TransactionStatus.Successful)
                {
                    switch (tx.TransactionType)
                    {
                        case TransactionType.Authorize:
                            //include in total
                            total += tx.Amount;
                            break;
                        case TransactionType.PartialCapture:
                        case TransactionType.AuthorizeCapture:
                        case TransactionType.Capture:
                        case TransactionType.PartialRefund:
                        case TransactionType.Refund:
                        case TransactionType.AuthorizeRecurring:
                        case TransactionType.CancelRecurring:
                            //do not include in total
                            break;
                        case TransactionType.Void:
                            //subtract from total
                            total -= tx.Amount;
                            break;
                        default:
                            //THIS SHOULD NEVER HAPPEN IF ALL TRANSACTION TYPES ARE SPECIFIED ABOVE
                            throw new ArgumentOutOfRangeException("Invalid Transaction Type : " + tx.TransactionType.ToString());
                    }
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the total value of transactions that require authorization
        /// </summary>
        /// <returns>The total value of transactions that require authorization</returns>
        public LSDecimal GetRemainingAuthorized()
        {
            LSDecimal total = 0;
            foreach (Transaction tx in this)
            {
                if (tx.TransactionStatus == TransactionStatus.Successful)
                {
                    switch (tx.TransactionType)
                    {
                        case TransactionType.Authorize:
                            //include in total
                            total += tx.Amount;
                            break;
                        case TransactionType.AuthorizeCapture:
                        case TransactionType.PartialRefund:
                        case TransactionType.Refund:
                            //do not include in total
                            break;
                        case TransactionType.PartialCapture:
                        case TransactionType.Capture:
                        case TransactionType.Void:
                            //subtract from total
                            total -= tx.Amount;
                            break;
                        default:
                            //THIS SHOULD NEVER HAPPEN IF ALL TRANSACTION TYPES ARE SPECIFIED ABOVE
                            throw new ArgumentOutOfRangeException("Invalid Transaction Type : " + tx.TransactionType.ToString());
                    }
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the total value of refunded transactions in this collection
        /// </summary>
        /// <returns>The total value of refunded transactions in this collection</returns>
        public LSDecimal GetTotalRefunded()
        {
            LSDecimal total = 0;
            foreach (Transaction tx in this)
            {
                if (tx.TransactionStatus == TransactionStatus.Successful)
                {
                    switch (tx.TransactionType)
                    {
                        case TransactionType.Authorize:
                        case TransactionType.AuthorizeCapture:
                        case TransactionType.PartialCapture:
                        case TransactionType.Capture:
                        case TransactionType.Void:
                            //do not include in total
                            break;
                        case TransactionType.PartialRefund:
                        case TransactionType.Refund:
                            //include in total
                            total += tx.Amount;                         
                            break;
                        default:
                            //THIS SHOULD NEVER HAPPEN IF ALL TRANSACTION TYPES ARE SPECIFIED ABOVE
                            throw new ArgumentOutOfRangeException("Invalid Transaction Type : " + tx.TransactionType.ToString());
                    }
                }
            }
            return total;
        }

    }
}
