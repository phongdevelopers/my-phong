using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net;
using System.Collections.Specialized;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Payments;
using CommerceBuilder.Products;

namespace CommerceBuilder.Payments.Providers.AcTestProvider
{
    public class AcTestProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        bool _UseAuthCapture = false;
        //bool _UseTestMode = true;

        public bool UseAuthCapture
        {
            get { return _UseAuthCapture; }
            set { _UseAuthCapture = value; }
        }

        public override string Name
        {
            get { return "AbleCommerce Test Gateway"; }
        }

        public override string Description
        {
            get { return "AbleCommerce Test Gateway is a dummy gateway for testing purposes only."; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.AcTestProvider.Logo.jpg");
            return string.Empty;
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override string ConfigReference
        {
            get { return "AcTestGateway"; }
        }

        private GatewayExecutionMode _ExecutionMode = GatewayExecutionMode.AlwaysAccept; 
        public GatewayExecutionMode ExecutionMode
        {
            get { return _ExecutionMode; }
            set { _ExecutionMode = value; }
        }

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
            HyperLink gatewayLink = new HyperLink();
            gatewayLink.Text = this.Name;
            gatewayLink.NavigateUrl = "http://www.ablecommerce.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">As the name suggests, this gateway is for testing purposes only. You can use this gateway to test how various features in AbleCommerce will work without having to create a real payment gateway account.</p>"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //display assembly information
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Assembly:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            Label lblAssembly = new Label();
            lblAssembly.ID = "AssemblyInfo";
            lblAssembly.Text = this.GetType().Assembly.GetName().Name.ToString() + "&nbsp;(v" + this.GetType().Assembly.GetName().Version.ToString() + ")";
            currentCell.Controls.Add(lblAssembly);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Execution Mode
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Execution Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">How the test gateway should behave.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            DropDownList emodeList = new DropDownList();
            emodeList.ID = "Config_ExecutionMode";
            foreach (GatewayExecutionMode emode in Enum.GetValues(typeof(GatewayExecutionMode)))
            {
                ListItem newItem = new ListItem(StringHelper.SpaceName(emode.ToString()), emode.ToString());
                emodeList.Items.Add(newItem);
                if (this.ExecutionMode == emode)
                {
                    newItem.Selected = true;
                }
            }
            currentCell.Controls.Add(emodeList);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Charge Type
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Authorization Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Use \"Authorize\" to request authorization without capturing funds at the time of purchase. You can capture authorized transactions through the order admin interface. Use \"Authorize & Capture\" to capture funds immediately at the time of purchase.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblChargeType = new RadioButtonList();
            rblChargeType.ID = "Config_UseAuthCapture";
            rblChargeType.Items.Add(new ListItem("Authorize (recommended)", "false"));
            rblChargeType.Items.Add(new ListItem("Authorize & Capture", "true"));
            rblChargeType.Items[UseAuthCapture ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblChargeType);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            currentCell.Controls.Add(new LiteralControl("<br />"));
            
            //GET THE DEBUG MODE
            /*currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Debug Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between AbleCommerce and PayflowPro is recorded in the store \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblDebugMode = new RadioButtonList();
            rblDebugMode.ID = "Config_UseDebugMode";
            rblDebugMode.Items.Add(new ListItem("Off", "false"));
            rblDebugMode.Items.Add(new ListItem("On", "true"));
            rblDebugMode.Items[UseDebugMode ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblDebugMode);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);
            */ 

            //CREATE LITERAL CONTROL WITH HTML CONTENT
            parentControl.Controls.Add(configTable);
        }

        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Authorize
                    | SupportedTransactions.AuthorizeCapture
                    | SupportedTransactions.Capture
                    | SupportedTransactions.PartialCapture
                    | SupportedTransactions.Refund
                    | SupportedTransactions.PartialRefund
                    | SupportedTransactions.Void                    
                    //| SupportedTransactions.RecurringAuthorize
                    //| SupportedTransactions.RecurringCancel
                    //| SupportedTransactions.RecurringModify                    
                    );
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("ExecutionMode")) ExecutionMode = ParseExecutionMode(ConfigurationData["ExecutionMode"]);
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            if (authorizeRequest.Capture || this.UseAuthCapture)
            {
                return CreateTransaction(TransactionType.AuthorizeCapture, authorizeRequest.Amount);
            }
            else
            {
                return CreateTransaction(authorizeRequest.TransactionType, authorizeRequest.Amount);
            }
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            Payment payment = captureRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            return CreateTransaction(captureRequest.TransactionType, captureRequest.Amount);
        }

        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            Payment payment = creditRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            return CreateTransaction(creditRequest.TransactionType, creditRequest.Amount);
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            Payment payment = voidRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Transaction authorizeTransaction = voidRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("voidRequest.AuthorizeTransaction");
            return CreateTransaction(voidRequest.TransactionType, voidRequest.Amount);
        }

        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            LSDecimal amount = authorizeRequest.RecurringChargeSpecified ? authorizeRequest.RecurringCharge : authorizeRequest.Amount;
            Transaction trans = CreateTransaction(authorizeRequest.TransactionType, authorizeRequest.Amount);
            trans.Amount = amount;
            if (trans.TransactionStatus == TransactionStatus.Successful)
            {
                trans.ProviderTransactionId = Guid.NewGuid().ToString();
            }
            return new AuthorizeRecurringTransactionResponse(trans);
        }

        /*public override Transaction DoCancelRecurring(CancelRecurringRequest request)
        {
            Transaction errTrans;
            string profileId = request.ProviderReference;
            if (string.IsNullOrEmpty(profileId))
            {
                errTrans = Transaction.CreateErrorTransaction(PaymentGatewayId, request.TransactionType, request.Subscription.EffectiveRecurringAmount, "E", "The original profile id is null.", request.RemoteIP);
                return errTrans;
            }

            Transaction trans = CreateTransaction(null, request.TransactionType);
            trans.Amount = request.Subscription.EffectiveRecurringAmount;

            return trans;
        }*/

        /*public override Transaction DoModifyRecurring(ModifyRecurringRequest request)
        {
            Transaction errTrans;
            string profileId = request.ProviderReference;
            if (string.IsNullOrEmpty(profileId))
            {
                errTrans = Transaction.CreateErrorTransaction(PaymentGatewayId, request.TransactionType, request.EffectiveRecurringAmount, "E", "The original profile id is null.", request.RemoteIP);
                return errTrans;
            }

            Transaction trans = CreateTransaction(null, request.TransactionType);
            trans.Amount = request.EffectiveRecurringAmount;

            return trans;
        }*/

        /*public override FetchUpdateRecurringResponse DoFetchUpdateRecurring(FetchUpdateRecurringRequest request)
        {
            throw new NotImplementedException("Not yet implemented");
        }*/

        private Transaction CreateTransaction(TransactionType transactionType, LSDecimal transactionAmount)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transactionType;

            bool accept = true;
            if (ExecutionMode == GatewayExecutionMode.AlwaysAccept)
            {
                accept = true;
            }
            else if (ExecutionMode == GatewayExecutionMode.AlwaysReject)
            {
                accept = false;
            }
            else if (ExecutionMode == GatewayExecutionMode.Random)
            {
                accept = RandomAccept();
            }
                        
            if (accept)
            {
                //successful
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = Guid.NewGuid().ToString();
                transaction.TransactionDate = DateTime.UtcNow;
                //if (payment != null)
                //{
                //    transaction.Amount = payment.Amount;
                //}
                transaction.Amount = transactionAmount;
                transaction.ResponseCode = "0";
                transaction.ResponseMessage = "Transaction Successful";
                transaction.AuthorizationCode = "";
                transaction.AVSResultCode = "S"; // NOT SUPPORTED
                transaction.CVVResultCode = "X"; // NO RESPONSE

                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                    transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                }
            }
            else
            {
                //failed
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = "999";
                transaction.ResponseMessage = "Transaction Failed.";
            }

            return transaction;
        }

        private bool RandomAccept()
        {
            Random m_RNG1 = new Random(Environment.TickCount);
            if (m_RNG1.Next(0, 2) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public enum GatewayExecutionMode
        {
            AlwaysAccept=1,
            AlwaysReject=2,
            Random=3
        }

        private GatewayExecutionMode ParseExecutionMode(string value)
        {
            if (string.IsNullOrEmpty(value)) return GatewayExecutionMode.AlwaysAccept;
            if (value.Equals(GatewayExecutionMode.AlwaysAccept.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return GatewayExecutionMode.AlwaysAccept;
            }
            else if (value.Equals(GatewayExecutionMode.AlwaysReject.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return GatewayExecutionMode.AlwaysReject;
            }
            else if (value.Equals(GatewayExecutionMode.Random.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return GatewayExecutionMode.Random;
            }
            else
            {
                return GatewayExecutionMode.AlwaysAccept;
            }
        }

    }
}
