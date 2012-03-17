using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

namespace CommerceBuilder.Payments.Providers
{
    /// <summary>
    /// A payment provider implementation used by AbleCommerce for processing payments made via gift certificates
    /// </summary>
    public class GiftCertificatePaymentProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        /// <summary>
        /// Name of the payment provider implementation.
        /// </summary>
        public override string Name
        {
            get { return "Gift Certificate Payment Provider"; }
        }

        /// <summary>
        /// Description of the payment provider implementation.
        /// </summary>
        public override string Description
        {
            get { return "Gift Certificate Payment Provider"; }
        }

        /// <summary>
        /// Not Implemented. Returns empty string
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public override string GetLogoUrl(ClientScriptManager cs)
        {
            return string.Empty;
        }

        /// <summary>
        /// Version of the payment provider implementation
        /// </summary>
        public override string Version
        {
            get { return "1.0"; }
        }

        /// <summary>
        /// Gets a reference string to identify a particular configuration.
        /// It will always be 'GiftCertificate' for this provider
        /// </summary>
        public override string ConfigReference
        {
            get { return "GiftCertificate"; }
        }

        /// <summary>
        /// Builds an input form for getting configuration data. Input form is buit inside the given
        /// ASP.NET parent control.
        /// </summary>
        /// <param name="parentControl">ASP.NET control to which to add the input form</param>
        public override void BuildConfigForm(Control parentControl)
        {
            HtmlTable configTable = new HtmlTable();
            configTable.CellPadding = 4;
            configTable.CellSpacing = 0;
            HtmlTableRow currentRow;
            HtmlTableCell currentCell;
            configTable.Attributes.Add("class", "inputForm");
            configTable.Attributes.Add("style", "border:none");

            //ADD CAPTION
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "sectionHeader");
            currentCell.ColSpan = 2;            
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">Gift Certificates Payment Provider is Pre-Configured.</p>"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //CREATE LITERAL CONTROL WITH HTML CONTENT
            parentControl.Controls.Add(configTable);
        }

        /// <summary>
        /// The transactions that are supported by the payment provider implementation.
        /// This provider supports 'Authorize' and 'AuthorizeCapture'
        /// </summary>
        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Authorize | SupportedTransactions.AuthorizeCapture);
            }
        }

        /// <summary>
        /// Initializes the payment provider implementation.
        /// </summary>
        /// <param name="PaymentGatewayId">Id of the payment gateway in database</param>
        /// <param name="ConfigurationData">Configuration data as name-value pairs</param>
        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
        }

        /// <summary>
        /// Submits an authorization request
        /// </summary>
        /// <param name="authorizeRequest">The authorization request</param>
        /// <returns>Transaction that represents the result of the authorization request</returns>
        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            //always use authorize capture.
            transaction.TransactionType = TransactionType.AuthorizeCapture;

            string serialNumber = authorizeRequest.Payment.AccountData;
            GiftCertificate gc = GiftCertificateDataSource.LoadForSerialNumber(serialNumber);

            string errorMessage = string.Empty;

            if (gc == null)
            {
                errorMessage = "No gift certificate found with given serial number.";
            }
            else if (gc.IsExpired())
            {
                errorMessage = "Gift certificate is expired.";
            }
            else if (gc.Balance < authorizeRequest.Amount)
            {
                errorMessage = "Gift certificate does not have enough balance to complete this transaction.";
            }
            else
            {
                LSDecimal newAmount = gc.Balance - authorizeRequest.Amount;
                gc.Balance = newAmount;
                GiftCertificateTransaction trans = new GiftCertificateTransaction();
                trans.TransactionDate = LocaleHelper.LocalNow;
                trans.Amount = authorizeRequest.Amount;
                trans.OrderId = authorizeRequest.Payment.OrderId;
                trans.Description = string.Format("An amount of {0:lc} used in purchase. Remaining balance is {1:lc}.",authorizeRequest.Amount, newAmount);                
                gc.Transactions.Add(trans);
                gc.Save();
            }

            if(string.IsNullOrEmpty(errorMessage)) {
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ResponseCode = "0";
                transaction.ResponseMessage = "SUCCESS";
                transaction.Amount = authorizeRequest.Amount;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = "1";
                transaction.ResponseMessage = errorMessage;
                transaction.Amount = authorizeRequest.Amount;
            }

            return transaction;
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="authorizeRequest"></param>
        /// <returns></returns>
        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            throw new NotSupportedException("Operation Not Supported.");
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="captureRequest"></param>
        /// <returns></returns>
        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            throw new NotSupportedException("Operation Not Supported.");
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="creditRequest"></param>
        /// <returns></returns>
        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            throw new NotSupportedException("Operation Not Supported.");
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="voidRequest"></param>
        /// <returns></returns>
        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            throw new NotSupportedException("Operation Not Supported.");
        }
    }
}
