using System;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Data;
using System.Text;

namespace CommerceBuilder.Payments
{
    public partial class Payment
    {
        /// <summary>
        /// Decrypted account data
        /// </summary>
        public string AccountData
        {
            get { return EncryptionHelper.DecryptAES(this.EncryptedAccountData); }
            set { this.EncryptedAccountData = EncryptionHelper.EncryptAES(value); }
        }

        /// <summary>
        /// Indicates whether data is present.
        /// </summary>
        public bool HasAccountData
        {
            get { return (!string.IsNullOrEmpty(this.EncryptedAccountData)); }
        }

        /// <summary>
        /// Indicates the status of this payment.
        /// </summary>
        public PaymentStatus PaymentStatus
        {
            get { return (PaymentStatus)this.PaymentStatusId; }
            set { this.PaymentStatusId = (short)value; }
        }

        /// <summary>
        /// Indicates whether the payment is in a failed state.
        /// </summary>
        public bool IsFailed
        {
            get
            {
                return (this.PaymentStatus == PaymentStatus.AuthorizationFailed ||
                    this.PaymentStatus == PaymentStatus.CaptureFailed); 
                    //|| this.PaymentStatus == PaymentStatus.RefundFailed ||
                    //this.PaymentStatus == PaymentStatus.VoidFailed);
            }
        }

        /// <summary>
        /// Indicates whether the payment is in a voidable state.
        /// </summary>
        public bool IsVoidable
        {
            get { return (PaymentStatusHelper.IsVoidable(this.PaymentStatus)); }
        }

        /// <summary>
        /// Indicates whether the payment is in a pending state.
        /// </summary>
        public bool IsPending
        {
            get
            {
                return (this.PaymentStatus == PaymentStatus.AuthorizationPending ||
                    this.PaymentStatus == PaymentStatus.CapturePending ||
                    this.PaymentStatus == PaymentStatus.RefundPending ||
                    this.PaymentStatus == PaymentStatus.VoidPending);
            }
        }

        /// <summary>
        /// Saves this payment object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            return Save(true);
        }

        /// <summary>
        /// Saves this payment object to the database
        /// <param name="triggerEvents">If true events are triggered on change of associated order's payment status</param>
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save(bool triggerEvents)
        {
            //save the payment method name for future reference
            if (string.IsNullOrEmpty(this.PaymentMethodName) && (this.PaymentMethod != null))
            {
                this.PaymentMethodName = this.PaymentMethod.Name;
            }
            //Check whether the payment is in a finalized state
            if (this.PaymentStatus == PaymentStatus.Captured || this.PaymentStatus == PaymentStatus.Completed || this.PaymentStatus == PaymentStatus.Refunded || this.PaymentStatus == PaymentStatus.Void)
            {
                //Payment is finalized.  Start tracking the date of completion if not done already.
                if (this.CompletedDate == DateTime.MinValue)
                {
                    this.CompletedDate = LocaleHelper.LocalNow;
                }
            }
            else
            {
                //this payment is not finalized, make sure a completion date is not set
                this.CompletedDate = DateTime.MinValue;
            }
            //get store settings to process payment
            StoreSettingCollection storeSettings = Store.GetCachedSettings();
            //if the payment has been completed, calculate whether the lifespan is elapsed
            if (this.CompletedDate > DateTime.MinValue)
            {
                //get the payment lifepsan
                Store store = Token.Instance.Store;
                if (LocaleHelper.LocalNow >= this.CompletedDate.Add(new TimeSpan(storeSettings.PaymentLifespan, 0, 0, 0)))
                {
                    //the payment lifespan is elapsed, remove any account data
                    this.AccountData = string.Empty;
                }
            }
            //ENSURE THAT CREDIT CARDS RESPECT CARD STORAGE SETTING
            if ((!string.IsNullOrEmpty(this.EncryptedAccountData)) && (this.PaymentMethod != null) 
                && (this.PaymentMethod.IsCreditOrDebitCard()) && (!storeSettings.EnableCreditCardStorage))
            {
                this.EncryptedAccountData = string.Empty;
            }
            //ENSURE THAT CARD SECURITY CODE IS NEVER STORED
            if (!string.IsNullOrEmpty(this.EncryptedAccountData) && (this.AccountData.ToLowerInvariant().Contains("securitycode")))
            {
                AccountDataDictionary accountData = new AccountDataDictionary(this.AccountData);
                accountData.Remove("SecurityCode");
                this.AccountData = accountData.ToString();
            }
            //ACTIVATE SUBSCRIPTIONS IF THIS PAYMENT IS COMPLETED (ARB PAYMENT)
            if ((this.PaymentStatus == PaymentStatus.Completed) && (this.Subscription != null) && (!this.Subscription.IsActive))
            {
                this.Subscription.Activate();
            }
            //CALL THE BASE SAVE METHOD
            SaveResult result = BaseSave();
            //TRIGGER UPDATE OF ORDER TOTAL PAYMENTS
            this.Order.RecalculatePaymentStatus(triggerEvents);
            //RETURN THE RESULT
            return result;
        }

        /// <summary>
        /// Updates payment status of this payment to the given status
        /// </summary>
        /// <param name="status">The payment status to set</param>
        public void UpdateStatus(PaymentStatus status)
        {
            string updateQuery = "UPDATE ac_Payments SET PaymentStatusId = @paymentStatusId WHERE PaymentId = @paymentId";
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@paymentStatusId", System.Data.DbType.Int16, (short)status);
                database.AddInParameter(updateCommand, "@paymentId", System.Data.DbType.Int32, this.PaymentId);
                int recordsAffected = database.ExecuteNonQuery(updateCommand);
                if (recordsAffected > 0) _PaymentStatusId = (short)status;
            }
        }

        /// <summary>
        /// Authorizes this payment
        /// </summary>
        public virtual void Authorize()
        {
            this.Authorize(false);
        }

        /// <summary>
        /// Authorizes this payment
        /// </summary>
        /// <param name="async">If <b>true</b> payment is authorized asynchronously</param>
        public virtual void Authorize(bool async)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null) throw new ArgumentException("You must specify remoteIP when HttpContext.Current is null.", "remoteIP");
            this.Authorize(async, context.Request.UserHostAddress);
        }

        /// <summary>
        /// Authorizes this payment
        /// </summary>
        /// <param name="async">If <b>true</b> payment is authorized asynchronously</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public virtual void Authorize(bool async, string remoteIP)
        {
            if (SubscriptionId == 0)
            {
                AuthorizeTransactionRequest request = new AuthorizeTransactionRequest(this, remoteIP);
                if (!async) PaymentEngine.DoAuthorize(request);
                else PaymentEngine.AsyncDoAuthorize(request);
            }
            else
            {
                AuthorizeRecurringTransactionRequest request = new AuthorizeRecurringTransactionRequest(this, this.Subscription.SubscriptionPlan, remoteIP);
                PaymentEngine.DoAuthorizeRecurring(request);
            }
        }

        /// <summary>
        /// Captures this payment
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        public virtual void Capture(LSDecimal amount)
        {
            this.Capture(amount, true);
        }

        /// <summary>
        /// Captures this payment
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        /// <param name="final">If <b>true</b> this capture is considered to be the final capture</param>
        public virtual void Capture(LSDecimal amount, bool final)
        {
            this.Capture(amount, final, false);
        }

        /// <summary>
        /// Captures this payment
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        /// <param name="final">If <b>true</b> this capture is considered to be the final capture</param>
        /// <param name="async">If <b>true</b> payment is captured asynchronously</param>
        public virtual void Capture(LSDecimal amount, bool final, bool async)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null) throw new ArgumentException("You must specify remoteIP when HttpContext.Current is null.", "remoteIP");
            this.Capture(amount, final, async, context.Request.UserHostAddress);
        }

        /// <summary>
        /// Captures this payment
        /// </summary>
        /// <param name="amount">The amount to capture</param>
        /// <param name="final">If <b>true</b> this capture is considered to be the final capture</param>
        /// <param name="async">If <b>true</b> payment is captured asynchronously</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public virtual void Capture(LSDecimal amount, bool final, bool async, string remoteIP)
        {
            CaptureTransactionRequest request = new CaptureTransactionRequest(this, remoteIP);
            request.Amount = amount;
            request.IsFinal = final;
            if (!async) PaymentEngine.DoCapture(request);
            else PaymentEngine.AsyncDoCapture(request);
        }

        /// <summary>
        /// Refunds this payment
        /// </summary>
        /// <param name="remoteIp">Remote IP of the user initiating the request</param>
        public virtual void Refund(string remoteIp)
        {
            RefundTransactionRequest request = new RefundTransactionRequest(this, remoteIp);
            PaymentEngine.DoRefund(request);
        }

        /// <summary>
        /// Voids this payment
        /// </summary>
        public virtual void Void()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null) throw new ArgumentException("You must specify remoteIP when HttpContext.Current is null.", "remoteIP");
            this.Void(context.Request.UserHostAddress);
        }

        /// <summary>
        /// Voids this payment
        /// </summary>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public virtual void Void(string remoteIP)
        {
            VoidTransactionRequest request = new VoidTransactionRequest(this, remoteIP);
            PaymentEngine.DoVoid(request);
        }


        /// <summary>
        /// Creates a reference number from the given credit card number
        /// </summary>
        /// <param name="accountNumber">The credit card number to create reference for</param>
        /// <returns>A reference number from the given credit card number</returns>
        public static string GenerateReferenceNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber)) return string.Empty;
            int length = accountNumber.Length;
            if (length < 5) return accountNumber;
            return ("x" + accountNumber.Substring((length - 4)));
        }


    }

}
