using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Create_DeferPaymentForm : System.Web.UI.UserControl
{
    private int _UserId;
    private User _User;
    Basket _Basket;
    
    //DEFINE EVENTS TO TRIGGER FOR CHECKOUT
    public event CheckingOutEventHandler CheckingOut;
    public event CheckedOutEventHandler CheckedOut;

    protected void Page_Init(object sender, EventArgs e)
    {
        // LOCATE THE USER THAT THE ORDER IS BEING PLACED FOR
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UID"]);
        _User = UserDataSource.Load(_UserId);
        if (_User == null) return;
        _Basket = _User.Basket;
    }

    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        //PROCESS CHECKING OUT EVENT
        bool checkOut = true;
        if (CheckingOut != null)
        {
            CheckingOutEventArgs c = new CheckingOutEventArgs();
            CheckingOut(this, c);
            checkOut = !c.Cancel;
        }
        if (checkOut)
        {
            //PROCESS THE CHECKOUT
            CheckoutRequest checkoutRequest = new CheckoutRequest(null);
            CheckoutResponse checkoutResponse = _Basket.Checkout(checkoutRequest, false);
            if (checkoutResponse.Success)
            {
                if (CheckedOut != null) CheckedOut(this, new CheckedOutEventArgs(checkoutResponse));
                Response.Redirect(NavigationHelper.GetReceiptUrl(checkoutResponse.OrderId));
            }
            else
            {
                List<string> warningMessages = checkoutResponse.WarningMessages;
                if (warningMessages.Count == 0)
                    warningMessages.Add("The order could not be submitted at this time.  Please try again later or contact us for assistance.");
                if (CheckedOut != null) CheckedOut(this, new CheckedOutEventArgs(checkoutResponse));
            }
        }
    }
}
