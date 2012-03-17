using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using CommerceBuilder.Catalog;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Marketing;
using CommerceBuilder.Messaging;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

public partial class Admin_Orders_Payments__Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId;
    private Order _Order;

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order == null) Response.Redirect("../Default.aspx");
        AddPaymentLink.NavigateUrl += "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
        BindPayments();
    }

    protected string AppendOrderId(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return "";
        }
        url = url + "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
        return url;
    }

    protected void BindPayments()
    {
        PaymentRepeater.DataSource = _Order.Payments;
        PaymentRepeater.DataBind();
    }

    protected void BuildTaskMenu(DropDownList taskMenu, Payment payment)
    {
        taskMenu.Items.Clear();
        //ADD BLANK ITEM TO START
        taskMenu.Items.Add(new ListItem());
        if (ShowButton("Received", payment)) taskMenu.Items.Add(new ListItem("Mark as Received", "RECEIVED"));
        if (ShowButton("Authorize", payment)) taskMenu.Items.Add(new ListItem("Authorize Payment", "AUTHORIZE"));
        if (ShowButton("RetryAuth", payment)) taskMenu.Items.Add(new ListItem("Retry Authorization", "RETRY"));
        if (ShowButton("RetryCapture", payment)) taskMenu.Items.Add(new ListItem("Retry Capture", "RETRY"));
        if (ShowButton("Capture", payment)) taskMenu.Items.Add(new ListItem("Capture Payment", "CAPTURE"));
        if (ShowButton("Void", payment))
            if ((payment.PaymentMethod != null && (payment.PaymentMethod.IsCreditOrDebitCard() || payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PayPal)) 
                && payment.PaymentStatus != PaymentStatus.Unprocessed)
                taskMenu.Items.Add(new ListItem("Void Authorization", "VOID"));
            else
                taskMenu.Items.Add(new ListItem("Void Payment", "VOID"));
        if (ShowButton("Refund", payment)) taskMenu.Items.Add(new ListItem("Issue Refund", "REFUND"));
        if (taskMenu.Items.Count > 1) taskMenu.Items.Add(new ListItem("---", String.Empty));
        taskMenu.Items.Add(new ListItem("Edit Payment", "EDIT"));
        taskMenu.Items.Add(new ListItem("Delete Payment", "DELETE"));
        
        // REGISTER CLIENT SCRIPT FOR DELETE TASK
        string warnScript = String.Empty;        
        warnScript += "function confirmDel(){";
        warnScript += " if( document.getElementById('" + taskMenu.ClientID.ToString() + "').value == 'DELETE'){";
        warnScript += "return confirm('This option will delete the payment and all transaction information, Are you sure you want to delete payments?');";
        warnScript += "}else return true;"; // END IF
        warnScript += "}"; // END Function
        
        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "checkDelete", warnScript, true);
        
    }

    public string GetSkinAVS(string code)
    {
        return string.Empty;
    }

    public string GetSkinCVV(string code)
    {
        return string.Empty;
    }

    protected void PaymentRepeater_ItemDataBound(object source, RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            DropDownList taskMenu = (DropDownList)e.Item.FindControl("PaymentAction");
            if (taskMenu != null) BuildTaskMenu(taskMenu, (Payment)e.Item.DataItem);
        }
    }

    protected void PaymentRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName.StartsWith("Do_"))
        {
            int paymentId = AlwaysConvert.ToInt(e.CommandArgument);
            int index = _Order.Payments.IndexOf(paymentId);
            if (index > -1)
            {
                Payment payment = _Order.Payments[index];

                string whichCommand = e.CommandName.Substring(3);
                if (whichCommand == "Task")
                {
                    //FIND TASK MENU
                    DropDownList taskMenu = (DropDownList)e.Item.FindControl("PaymentAction");
                    whichCommand = taskMenu.SelectedValue;
                }

                //TAKE ACTION BASED ON COMMAND
                if (!string.IsNullOrEmpty(whichCommand))
                {
                    whichCommand = whichCommand.ToUpperInvariant();
                    switch (whichCommand)
                    {
                        case "RETRY":
                            //IF THIS WAS A FAILED CAPTURE, RESET PAYMENT STATUS TO AUTHORIZED AND REDIRECT TO CAPTURE PAGE
                            if (payment.PaymentStatus == PaymentStatus.CaptureFailed)
                            {
                                payment.UpdateStatus(PaymentStatus.Authorized);
                                Response.Redirect("CapturePayment.aspx?PaymentId=" + payment.PaymentId.ToString());
                            }
                            //THIS WAS A FAILED AUTHORIZATION, SHOW THE RETRY PANEL
                            Panel retryPanel = (Panel)PageHelper.RecursiveFindControl(e.Item, "RetryPanel");
                            retryPanel.Visible = true; 
                            break;
                        case "SUBMITRETRY":
                            TextBox securityCode = (TextBox)PageHelper.RecursiveFindControl(e.Item, "RetrySecurityCode");
                            if (!string.IsNullOrEmpty(securityCode.Text))
                            {
                                AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
                                accountData["SecurityCode"] = securityCode.Text;
                                payment.AccountData = accountData.ToString();
                            }
                            payment.PaymentStatus = CommerceBuilder.Payments.PaymentStatus.Unprocessed;
                            payment.Authorize(false);
                            BindPayments();
                            break;
                        case "CANCELRETRY":
                            //NO ACTION REQUIRED, THE REBIND STEP WILL HIDE THE RETRY PANEL
                            BindPayments();
                            break;
                        case "EDIT":
                            Response.Redirect("EditPayment.aspx?PaymentId=" + payment.PaymentId.ToString());
                            break;
                        case "CAPTURE":
                            //REDIRECT TO CAPTURE FORM
                            Response.Redirect("CapturePayment.aspx?PaymentId=" + payment.PaymentId.ToString());
                            break;
                        case "DELETE":
                            string deleteNote = string.Format("A payment of type {0} and amount {1:c} is deleted.", payment.PaymentMethodName, payment.Amount);
                            _Order.Payments.DeleteAt(index);
                            _Order.Notes.Add(new OrderNote(_Order.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, deleteNote, NoteType.SystemPrivate));
                            _Order.Notes.Save();
                            BindPayments();
                            break;
                        case "AUTHORIZE":
                            payment.Authorize();
                            BindPayments();
                            break;
                        case "RECEIVED":
                            payment.PaymentStatus = PaymentStatus.Completed;
                            payment.Save();
                            BindPayments();
                            break;
                        case "REFUND":
                            Response.Redirect("RefundPayment.aspx?PaymentId=" + payment.PaymentId.ToString() + "&OrderId=" + _OrderId);
                            break;
                        case "VOID":
                            Response.Redirect("VoidPayment.aspx?PaymentId=" + payment.PaymentId.ToString() + "&OrderId=" + _OrderId);
                            break;
                        default:
                            throw new ArgumentException("Unrecognized command: " + whichCommand);
                    }
                }
            }
        }
    }
            
    public bool ShowButton(string buttonName, object dataItem)
    {
        Payment payment = (Payment)dataItem;
        switch (buttonName.ToUpperInvariant())
        {
            case "RETRYAUTH":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.AuthorizationFailed);
            case "RETRYCAPTURE":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.CaptureFailed);
            case "RECEIVED":                
                return (payment.PaymentStatus == PaymentStatus.Unprocessed);                
            case "AUTHORIZE":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.Unprocessed);      
            case "VOID":
				//Disable Void for Google Checkout AND PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod!=null && (payment.PaymentMethod.PaymentInstrument == PaymentInstrument.GoogleCheckout || payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall))
                {
                    return false;
                }
                else
                {
                    //VOID SHOULD ONLY BE SHOWN IF THE PAYMENT IS UNPROCESSED OR IN AN AUTHORIZED STATE
                    return (payment.PaymentStatus == PaymentStatus.Unprocessed 
                           || payment.PaymentStatus == PaymentStatus.AuthorizationFailed 
                           || payment.PaymentStatus == PaymentStatus.Authorized 
                           || payment.PaymentStatus == PaymentStatus.CaptureFailed);
                }
            case "CAPTURE":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.Authorized);
            case "CANCEL":
                // DISABLE FOR PHONE CALL PAYMENT METHOD
                if (payment.PaymentMethod != null && payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PhoneCall) return false;
                return (payment.PaymentStatus == PaymentStatus.Completed);
            case "REFUND":
                //SHOW REFUND IF THE PAYMENT WAS MADE WITHIN 60 DAYS
                //AND THE PAYMENT IS CAPTURED
                //AND THERE IS IS A POSTIVE TRANSACTION CAPTURE AMOUNT
                return ((payment.PaymentDate > DateTime.UtcNow.AddDays(-60)) 
                       && (payment.PaymentStatus == PaymentStatus.Captured) 
                       && (payment.Transactions.GetTotalCaptured() > 0) 
                       && (payment.PaymentMethod == null || payment.PaymentMethod.PaymentInstrument != PaymentInstrument.GiftCertificate));
            case "DELETE":
                //BY DEFAULT DO NOT SHOW THE DELETE BUTTON
                return false;       
            default:
                throw new ArgumentException("Invalid button name: '" + buttonName, buttonName);
        }
    }

    public bool ShowTransactionElement(string elementName, object dataItem)
    {
        Transaction transaction = (Transaction)dataItem;
        switch (elementName.ToUpperInvariant())
        {
            case "AVSCVV":
                return (transaction.TransactionType == TransactionType.Authorize || transaction.TransactionType == TransactionType.AuthorizeCapture || transaction.TransactionType == TransactionType.AuthorizeRecurring);
            default:
                throw new ArgumentException("Invalid element name: '" + elementName, elementName);
        }
    }

    protected bool isSuccessfulTransaction(Object obj)
    {
        if (obj is TransactionStatus)
        {
            return ((TransactionStatus)obj) == TransactionStatus.Successful;
        }
        return false;
    }

    protected bool isFailedTransaction(Object obj)
    {
        if (obj is TransactionStatus)
        {
            return ((TransactionStatus)obj) == TransactionStatus.Failed;
        }
        return false;
    }

    protected bool isPendingTransaction(Object obj)
    {
        if (obj is TransactionStatus)
        {
            return ((TransactionStatus)obj) == TransactionStatus.Pending;
        }
        return false;
    }

    protected string GetCvv(object dataItem)
    {
        Transaction transaction = (Transaction)dataItem;
        return StoreDataHelper.TranslateCVVCode(transaction.CVVResultCode) + " (" + transaction.CVVResultCode + ")";
    }
    
    protected string GetAvs(object dataItem)
    {
        Transaction transaction = (Transaction)dataItem;
        return StoreDataHelper.TranslateAVSCode(transaction.AVSResultCode) + " (" + transaction.AVSResultCode + ")";
    }
    
}
