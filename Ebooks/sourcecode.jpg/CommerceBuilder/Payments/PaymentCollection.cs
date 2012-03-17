namespace CommerceBuilder.Payments
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;

    /// <summary>
    /// Class represents a collection of Payment objects
    /// </summary>
    public partial class PaymentCollection
    {

        /// <summary>
        /// Returns the last payment (in chronological order)
        /// </summary>
        /// <remarks>This causes the collection to be resorted by date.</remarks>
        public Payment LastPayment
        {
            get
            {
                if (this.Count == 0) return null;
                this.Sort("PaymentDate");
                return this[this.Count - 1];
            }
        }

        /// <summary>
        /// Get the total of all payments in this collection
        /// </summary>
        /// <returns></returns>
        public LSDecimal Total()
        {
            return this.Total(true);
        }

        /// <summary>
        /// Get the total of all payments in this collection
        /// </summary>
        /// <param name="includePending">If <b>true</b> pending payments are also included in calculation</param>
        /// <returns></returns>
        public LSDecimal Total(bool includePending)
        {
            LSDecimal total = 0;
            foreach (Payment item in this)
            {
                if ((item.PaymentStatus == PaymentStatus.Captured) || (item.PaymentStatus == PaymentStatus.Completed) || (includePending && PaymentStatusHelper.IsPendingStatus(item.PaymentStatus)))
                {
                    total += item.Amount;
                }
                else if (item.PaymentStatus == PaymentStatus.Authorized)
                {
                    //THE PAYMENT MAY HAVE PARTIAL CAPTURES ATTACHED
                    total += item.Transactions.GetTotalCaptured();
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the total amount for processed payments
        /// </summary>
        /// <returns>The total amount for processed payments</returns>
        public LSDecimal TotalProcessed()
        {
            LSDecimal total = 0;
            foreach (Payment item in this)
            {
                if ((item.PaymentStatus == PaymentStatus.Captured) || (item.PaymentStatus == PaymentStatus.Completed))
                {
                    total += item.Amount;
                }
                else if (item.PaymentStatus == PaymentStatus.Authorized)
                {
                    //THE PAYMENT MAY HAVE PARTIAL CAPTURES ATTACHED
                    total += item.Transactions.GetTotalCaptured();
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the total amount for unprocessed payments
        /// </summary>
        /// <returns>The total amount for unprocessed payments</returns>
        public LSDecimal TotalUnprocessed()
        {
            LSDecimal total = 0;
            foreach (Payment item in this)
            {
                if ((item.PaymentStatus != PaymentStatus.Captured) 
                    && (item.PaymentStatus != PaymentStatus.Completed)
                    && (item.PaymentStatus != PaymentStatus.Refunded)
                    && (item.PaymentStatus != PaymentStatus.RefundPending)
                    && (item.PaymentStatus != PaymentStatus.Void)
                    && (item.PaymentStatus != PaymentStatus.VoidPending)
                    )
                {
                    if (item.PaymentStatus == PaymentStatus.Authorized)
                    {
                        //THE PAYMENT MAY HAVE PARTIAL CAPTURES ATTACHED
                        total += (item.Amount - item.Transactions.GetTotalCaptured());
                    }
                    else
                    {
                        total += item.Amount;
                    }
                }
            }
            return total;
        }

    }
}
