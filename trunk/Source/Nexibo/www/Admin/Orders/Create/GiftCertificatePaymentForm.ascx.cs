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

public partial class Admin_Orders_Create_GiftCertificatePaymentForm : System.Web.UI.UserControl
{
    private int _UserId;
    private User _User;
    Basket _Basket;

    //DEFINE EVENTS TO TRIGGER FOR CHECKOUT
    public event CheckingOutEventHandler CheckingOut;
    public event CheckedOutEventHandler CheckedOut;

    //private List<string> _GiftCertErrorMessages=null;
    private string _GiftCertErrorMessage = string.Empty;
    private string _ValidationGroup = "GiftCertificate";
    public string ValidationGroup
    {
        get { return _ValidationGroup; }
        set { _ValidationGroup = value; }
    }

    private bool _ValidationSummaryVisible = true;
    public bool ValidationSummaryVisible
    {
        get { return _ValidationSummaryVisible; }
        set { _ValidationSummaryVisible = value; }
    }

    private void UpdateValidationOptions()
    {
        GiftCertificateNumber.ValidationGroup = _ValidationGroup;
        GiftCertificateNumberRequired.ValidationGroup = _ValidationGroup;
        GiftCertificateButton.ValidationGroup = _ValidationGroup;
        ValidationSummary1.ValidationGroup = _ValidationGroup;
        ValidationSummary1.Visible = _ValidationSummaryVisible;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        // LOCATE THE USER THAT THE ORDER IS BEING PLACED FOR
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UID"]);
        _User = UserDataSource.Load(_UserId);
        if (_User == null) return;
        _Basket = _User.Basket;

        UpdateValidationOptions();
    }

    private GridView _NonShippingItemsGrid = null;

    public GridView NonShippingItemsGrid
    {
        set { _NonShippingItemsGrid = value; }
        get { return _NonShippingItemsGrid; }
    }

    private bool HasGiftCertErrorMessage()
    {
        return !string.IsNullOrEmpty(_GiftCertErrorMessage);
    }

    private void SetGiftCertErrorMessage(string message)
    {
        _GiftCertErrorMessage = message;
    }

    private string GetGiftCertErrorMessage()
    {
        return _GiftCertErrorMessage;
    }

    private void RebindNonShippingItems()
    {
        if (NonShippingItemsGrid != null)
        {
            BasketItemCollection nonShippingItems = BasketHelper.GetNonShippingItems(_Basket);
            if (nonShippingItems.Count > 0)
            {
                //NonShippingItemsPanel.Visible = true;
                NonShippingItemsGrid.Parent.Visible = true;
                NonShippingItemsGrid.DataSource = nonShippingItems;
                NonShippingItemsGrid.DataBind();
            }
        }
    }

    protected void GiftCertificateButton_Click(object sender, EventArgs e)
    {
        SetGiftCertErrorMessage("");
        if (Page.IsValid)
        {
            bool checkOut = true;
            if (CheckingOut != null)
            {
                CheckingOutEventArgs c = new CheckingOutEventArgs();
                CheckingOut(this, c);
                checkOut = !c.Cancel;
            }
            if (checkOut)
            {
                GiftCertificateNumber.Text = StringHelper.StripHtml(GiftCertificateNumber.Text);
                GiftCertificate gc = GiftCertificateDataSource.LoadForSerialNumber(GiftCertificateNumber.Text);
                if (gc == null)
                {
                    SetGiftCertErrorMessage("This is not a valid gift certificate : " + GiftCertificateNumber.Text);
                }
                else if (gc.Balance <= 0)
                {
                    SetGiftCertErrorMessage("There is no balance left for this gift certificate : " + GiftCertificateNumber.Text);
                }
                else if (gc.IsExpired())
                {
                    SetGiftCertErrorMessage("This gift certificate is expired : " + GiftCertificateNumber.Text);
                }
                else if (AlreadyInUse(_Basket, gc))
                {
                    SetGiftCertErrorMessage("This gift certificate is already applied to your _Basket : " + GiftCertificateNumber.Text);
                }
                else
                {
                    //process this gift certificate
                    LSDecimal basketTotal = _Basket.Items.TotalPrice();
                    BasketItem bitem = new BasketItem();
                    bitem.OrderItemType = OrderItemType.GiftCertificatePayment;
                    bitem.Price = -(gc.Balance > basketTotal ? basketTotal : gc.Balance);
                    bitem.Quantity = 1;
                    bitem.Name = gc.Name;
                    bitem.Sku = gc.SerialNumber;
                    _Basket.Items.Add(bitem);
                    _Basket.Save();
                    LSDecimal remBalance = _Basket.Items.TotalPrice();
                    if (remBalance > 0)
                    {
                        SetGiftCertErrorMessage(string.Format("A payment of {0:ulc} will be made using gift certificate {1}. It will leave a balance of {2:ulc} for this order. Please make additional payments.", gc.Balance, GiftCertificateNumber.Text, remBalance));
                    }
                    else
                    {
                        //payment done. process checkout
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
                    RebindNonShippingItems();
                }
                if (HasGiftCertErrorMessage())
                {
                    GiftCertErrorsPanel.Visible = true;
                    GiftCertPaymentErrors.Text = GetGiftCertErrorMessage();
                }
            }
        }
    }

    private bool AlreadyInUse(Basket basket, GiftCertificate gc)
    {
        foreach (BasketItem item in basket.Items)
        {
            if (item.OrderItemType == OrderItemType.GiftCertificatePayment
                && item.Sku == gc.SerialNumber)
            {
                return true;
            }
        }
        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GiftCertificateNumber.Attributes.Add("autocomplete", "off");
    }
}
