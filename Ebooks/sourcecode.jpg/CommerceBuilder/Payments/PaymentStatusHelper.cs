using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Helper class for payment status calculations
    /// </summary>
    public static class PaymentStatusHelper
    {
        /// <summary>
        /// Checks if the given status is one of the pending statuses
        /// </summary>
        /// <param name="status">The payment status to check</param>
        /// <returns><b>true</b> if the given status is one of the pending statuses, <b>false</b> otherwise</returns>
        public static bool IsPendingStatus(PaymentStatus status)
        {
            switch (status)
            {
                case PaymentStatus.AuthorizationPending:
                case PaymentStatus.CapturePending:
                case PaymentStatus.RefundPending:
                case PaymentStatus.VoidPending:
                    return true;
                default:
                    return false;                    
            }
            //return ((status != PaymentStatus.Captured) 
             //   && (status != PaymentStatus.Refunded) 
             //   && (status != PaymentStatus.Void));
        }

        /// <summary>
        /// Checks if the given payment status is one of the voidable statuses
        /// </summary>
        /// <param name="status">The payment status to check</param>
        /// <returns><b>true</b> if the given status is one of the voidable statuses, <b>false</b> otherwise</returns>
        public static bool IsVoidable(PaymentStatus status)
        {
            switch (status)
            {
                case PaymentStatus.Unprocessed:
                case PaymentStatus.CaptureFailed:
                case PaymentStatus.AuthorizationFailed:
                case PaymentStatus.Authorized:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a payment status that would become of the given payment after the specified transaction completes
        /// </summary>
        /// <param name="payment">The payment object</param>
        /// <param name="transaction">The transaction after which to determine the status</param>
        /// <returns>A payment status that would become of the given payment after the specified transaction completes</returns>
        public static PaymentStatus GetStatusAfterTransaction(Payment payment, Transaction transaction)
        {
            LSDecimal totalCaptured;
            LSDecimal totalAuthorized;
            LSDecimal temp;
            switch (transaction.TransactionType)
            {
                case TransactionType.Authorize:
                    switch (transaction.TransactionStatus)
                    {
                        case TransactionStatus.Successful:
                            return PaymentStatus.Authorized;
                        case TransactionStatus.Failed:
                            return PaymentStatus.AuthorizationFailed;
                        case TransactionStatus.Pending:
                            return PaymentStatus.AuthorizationPending;
                    }
                    break;
                case TransactionType.PartialCapture:
                    switch (transaction.TransactionStatus)
                    {
                        case TransactionStatus.Successful:
                            totalCaptured = payment.Transactions.GetTotalCaptured();
                            totalAuthorized = payment.Transactions.GetTotalAuthorized();
                            temp = totalCaptured + transaction.Amount;
                            if (totalAuthorized > temp)
                            {
                                return PaymentStatus.Authorized;
                            }
                            else
                            {
                                return PaymentStatus.Captured;
                            }
                        case TransactionStatus.Failed:
                            return PaymentStatus.CaptureFailed;
                        case TransactionStatus.Pending:
                            return PaymentStatus.CapturePending;
                    }
                    break;
                case TransactionType.AuthorizeCapture:
                    switch (transaction.TransactionStatus)
                    {
                        case TransactionStatus.Successful:
                            return PaymentStatus.Captured;
                        case TransactionStatus.Failed:
                            return PaymentStatus.AuthorizationFailed;
                        case TransactionStatus.Pending:
                            return PaymentStatus.AuthorizationPending;
                    }
                    break;
                case TransactionType.Capture:
                    switch (transaction.TransactionStatus)
                    {
                        case TransactionStatus.Successful:
                            return PaymentStatus.Captured;
                        case TransactionStatus.Failed:
                            return PaymentStatus.CaptureFailed;
                        case TransactionStatus.Pending:
                            return PaymentStatus.CapturePending;
                    }
                    break;
                case TransactionType.PartialRefund:
                case TransactionType.Refund:
                    switch (transaction.TransactionStatus)
                    {
                        case TransactionStatus.Successful:
                            totalCaptured = payment.Transactions.GetTotalCaptured();
                            if (totalCaptured > transaction.Amount)
                            {
                                //MAKE SURE THE TRANSACTION TYPE IS APPROPRIATE
                                transaction.TransactionType = TransactionType.PartialRefund;
                                return PaymentStatus.Captured;
                            }
                            else
                            {
                                return PaymentStatus.Refunded;
                            }
                        case TransactionStatus.Failed:
                            return PaymentStatus.Captured;
                        case TransactionStatus.Pending:
                            return PaymentStatus.RefundPending;
                    }
                    break;
                case TransactionType.Void:
                    switch (transaction.TransactionStatus)
                    {
                        case TransactionStatus.Successful:
                            totalCaptured = payment.Transactions.GetTotalCaptured();
                            if (totalCaptured > 0)
                            {
                                return PaymentStatus.Captured;
                            }
                            else
                            {
                                return PaymentStatus.Void;
                            }
                        case TransactionStatus.Failed:
                            //return PaymentStatus.VoidFailed;
                            return payment.PaymentStatus;
                        case TransactionStatus.Pending:
                            return PaymentStatus.VoidPending;
                    }
                    break;
                case TransactionType.AuthorizeRecurring:
                    if (transaction.TransactionStatus == TransactionStatus.Successful)
                    {
                        // USE COMPLETED INSTEAD OF CAPTURED BECAUSE THIS PAYMENT CANNOT BE REFUNDED
                        // THROUGH THE EXISTING API
                        return PaymentStatus.Completed;
                    }
                    else
                    {
                        // VOID BECAUSE WE CANNOT RETRY RECURRING TRANSACTIONS
                        return PaymentStatus.Void;
                    }
                case TransactionType.CancelRecurring:
                    return PaymentStatus.Void;
            }

            //THIS SHOULD NEVER HAPPEN IF ALL TRANSACTION TYPES AND TRANSACTION STATUSES ARE SPECIFIED ABOVE
            throw new ArgumentOutOfRangeException("Invalid Transaction Type or Transaction Status. Type:"
                + transaction.TransactionType.ToString() +
                "Status:" + transaction.TransactionStatus.ToString());
        }

    }
}
