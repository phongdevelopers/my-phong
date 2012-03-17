using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Marketing;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Taxes;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Create_CreateOrder4 : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _UserId;
    private User _User;
    Basket _Basket;

    /// <summary>
    ///  For orders placed on behalf of unregistered users, this field holds the 
    /// user account that matches the email address given on the billing form (if any)
    /// </summary>
    private User _ExistingUser;

    protected void Page_Init(object sender, EventArgs e)
    {
        // LOCATE THE USER THAT THE ORDER IS BEING PLACED FOR
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UID"]);
        _User = UserDataSource.Load(_UserId);
        if (_User == null) Response.Redirect("CreateOrder1.aspx");
        _Basket = _User.Basket;
        MiniBasket1.BasketId = _Basket.BasketId;
        if (!Page.IsPostBack) _Basket.Recalculate();

        // INITIALIZE THE CAPTION
        string userName = _User.IsAnonymous ? "Unregistered User" : _User.UserName;
        Caption.Text = string.Format(Caption.Text, userName);

        // SHOW BILLING ADDRESS
        BillToAddress.Text = _User.PrimaryAddress.ToString(true);
        EditAddressesLink.NavigateUrl += "?UID=" + _UserId;

        // SHOW REGISTRATION PANEL IF USER IS ANONYMOUS
        if (_User.IsAnonymous)
        {
            RegisterPanel.Visible = true;
            string billToEmail = _User.PrimaryAddress.Email;
            UserCollection matchingUsers = UserDataSource.LoadForEmail(billToEmail, false);
            bool userExists = (matchingUsers.Count > 0);
            if (userExists)
            {
                _ExistingUser = matchingUsers[0];
                AccountUserName.Text = _ExistingUser.UserName;
                AccountEmail.Text = _ExistingUser.Email;
            }
            else
            {
                AccountUserName.Text = billToEmail;
                AccountEmail.Text = billToEmail;
            }
            RegisteredUserHelpText.Visible = userExists;
            UnregisteredUserHelpText.Visible = !userExists;
            LinkAccountLabel.Visible = userExists;
            CreateAccountLabel.Visible = !userExists;
            trAccountPassword.Visible = !userExists;
            trForceExpiration.Visible = !userExists;
        }

        // SHOW SHIPPING METHODS IF NECESSARY
        ShippingMethodPanel.Visible = _Basket.Items.HasShippableProducts;
        if (ShippingMethodPanel.Visible)
        {
            tdShipTo.Visible = true;
            Address shipAddress = this.ShippingAddress;
            if (shipAddress != null) ShipToAddress.Text = shipAddress.ToString(true);
            if (!Page.IsPostBack)
            {
                // ONLY BIND SHIPMENT LIST ON FIRST VISIT
                ShipmentList.DataSource = _Basket.Shipments;
                ShipmentList.DataBind();
            }
        }

        // SHOW PAYMENT METHODS
        BindPaymentMethodForms();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        PaymentMethodCaption.Text = string.Format(PaymentMethodCaption.Text, _Basket.Items.TotalPrice());
    }

    protected int ShipmentCount
    {
        get { return _Basket.Shipments.Count; }
    }

    protected void ShipmentList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //SHOW SHIPPING METHODS
        DropDownList ShipMethodList = (DropDownList)e.Item.FindControl("ShipMethodList");
        if (ShipMethodList != null)
        {
            BasketShipment shipment = (BasketShipment)e.Item.DataItem;
            if (shipment != null)
            {
                // CALCULATE THE SHIPPING RATES
                ICollection<ShipRateQuote> rateQuotes = ShipRateQuoteDataSource.QuoteForShipment(shipment);
                foreach (ShipRateQuote quote in rateQuotes)
                {
                    LSDecimal totalRate = TaxHelper.GetShopPrice(quote.TotalRate, quote.ShipMethod.TaxCodeId, null, new TaxAddress(shipment.Address));
                    string formattedRate = totalRate.ToString("ulc");
                    string methodName = (totalRate > 0) ? quote.Name + ": " + formattedRate : quote.Name;
                    ShipMethodList.Items.Add(new ListItem(methodName, quote.ShipMethodId.ToString()));
                }
            }
        }
    }

    protected void ShipMethodList_SelectedIndexChanged(object sender, EventArgs e)
    {
        // UPDATE SHIPMENTS
        for (int i = 0; i < ShipmentList.Items.Count; i++)
        {
            RepeaterItem item = ShipmentList.Items[i];
            if (item != null)
            {
                BasketShipment shipment = _Basket.Shipments[i];
                DropDownList ShipMethodList = (DropDownList)item.FindControl("ShipMethodList");
                if (shipment != null && ShipMethodList != null)
                {
                    shipment.ShipMethodId = AlwaysConvert.ToInt(ShipMethodList.Items[ShipMethodList.SelectedIndex].Value);
                }
            }
        }

        // UPDATE THE ORDER ITMES PANEL TO REFLECT ANY CHANGE
        _Basket.Save();
        _Basket.Recalculate();
    }

    private Address ShippingAddress
    {
        get
        {
            if (_Basket.Shipments.Count > 0)
            {
                int index = _User.Addresses.IndexOf(_Basket.Shipments[0].AddressId);
                if (index > -1) return _User.Addresses[index];
            }
            return null;
        }
    }

    private void BindPaymentMethodForms()
    {
        //CHECK ORDER TOTAL
        LSDecimal orderTotal = _Basket.Items.TotalPrice();
        HiddenField hiddenOrderTotal;

        if (phPaymentForms.Controls.Count > 0)
        {
            hiddenOrderTotal = phPaymentForms.FindControl("OT") as HiddenField;
            if (hiddenOrderTotal != null)
            {
                //DO NOT CONTINUE TO PROCESS METHOD IF ORDER TOTAL HAS NOT CHANGED
                //BETWEEN ZERO AND NON ZERO
                LSDecimal savedOrderTotal = AlwaysConvert.ToDecimal(hiddenOrderTotal.Value);
                if ((orderTotal == 0 && savedOrderTotal == 0) || (orderTotal != 0 && savedOrderTotal != 0))
                    //PAYMENT METHODS WOULD NOT CHANGE, EXIT METHOD
                    return;
            }
            //RESET PAYMENT FORMS
            phPaymentForms.Controls.Clear();
        }

        if (orderTotal > 0)
        {
            List<DictionaryEntry> paymentMethods = new List<DictionaryEntry>();
            //ADD PAYMENT FORMS
            bool creditCardAdded = false;
            PaymentMethodCollection availablePaymentMethods = StoreDataHelper.GetPaymentMethods(_UserId);
            foreach (PaymentMethod method in availablePaymentMethods)
            {
                switch (method.PaymentInstrument)
                {
                    case PaymentInstrument.AmericanExpress:
                    case PaymentInstrument.Discover:
                    case PaymentInstrument.JCB:
                    case PaymentInstrument.MasterCard:
                    case PaymentInstrument.Visa:
                    case PaymentInstrument.DinersClub:
                    case PaymentInstrument.Maestro:
                    case PaymentInstrument.SwitchSolo:
                    case PaymentInstrument.VisaDebit:
                        if (!creditCardAdded)
                        {
                            paymentMethods.Insert(0, new DictionaryEntry(0, "Credit/Debit Card"));
                            creditCardAdded = true;
                        }
                        break;
                    case PaymentInstrument.Check:
                    case PaymentInstrument.PurchaseOrder:
                    case PaymentInstrument.PayPal:
                    case PaymentInstrument.Mail:
                    case PaymentInstrument.PhoneCall:
                        paymentMethods.Add(new DictionaryEntry(method.PaymentMethodId, method.Name));
                        break;
                    default:
                        //types not supported
                        break;
                }
            }

            if (StoreDataHelper.HasGiftCertificates())
            {
                paymentMethods.Add(new DictionaryEntry(-1, "Gift Certificate"));
            }
            paymentMethods.Add(new DictionaryEntry(-2, "Defer Payment"));

            //BIND THE RADIO LIST FOR PAYMENT METHOD SELECTION
            PaymentMethodList.DataSource = paymentMethods;
            PaymentMethodList.DataBind();

            //CONTINUE IF PAYMENT METHODS ARE AVAILABLE
            if (paymentMethods.Count > 0)
            {
                //MAKE SURE THE CORRECT PAYMENT METHOD IS SELECTED
                int paymentMethodId = AlwaysConvert.ToInt(Request.Form[PaymentMethodList.UniqueID]);
                ListItem selectedListItem = PaymentMethodList.Items.FindByValue(paymentMethodId.ToString());
                if (selectedListItem != null)
                {
                    PaymentMethodList.SelectedIndex = PaymentMethodList.Items.IndexOf(selectedListItem);
                }
                else PaymentMethodList.SelectedIndex = 0;

                //GET THE CURRENTLY SELECTED METHOD
                paymentMethodId = AlwaysConvert.ToInt(PaymentMethodList.SelectedValue);
                if (paymentMethodId == 0)
                {
                    ASP.admin_orders_create_creditcardpaymentform_ascx cardPaymentForm = new ASP.admin_orders_create_creditcardpaymentform_ascx();
                    cardPaymentForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                    cardPaymentForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                    cardPaymentForm.ValidationGroup = "OPC";
                    cardPaymentForm.ValidationSummaryVisible = false;
                    phPaymentForms.Controls.Add(cardPaymentForm);
                }
                else if (paymentMethodId == -1)
                {
                    ASP.admin_orders_create_giftcertificatepaymentform_ascx gcForm = new ASP.admin_orders_create_giftcertificatepaymentform_ascx();
                    gcForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                    gcForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                    gcForm.ValidationGroup = "OPC";
                    gcForm.ValidationSummaryVisible = false;
                    phPaymentForms.Controls.Add(gcForm);
                }
                else if (paymentMethodId == -2)
                {
                    ASP.admin_orders_create_deferpaymentform_ascx deferForm = new ASP.admin_orders_create_deferpaymentform_ascx();
                    deferForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                    deferForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                    phPaymentForms.Controls.Add(deferForm);
                }
                else
                {
                    //DISPLAY FORM FOR SPECIFIC METHOD
                    PaymentMethod selectedMethod = availablePaymentMethods[availablePaymentMethods.IndexOf(paymentMethodId)];
                    switch (selectedMethod.PaymentInstrument)
                    {
                        case PaymentInstrument.Check:
                            ASP.admin_orders_create_checkpaymentform_ascx checkForm = new ASP.admin_orders_create_checkpaymentform_ascx();
                            checkForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            checkForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            checkForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            checkForm.ValidationGroup = "OPC";
                            checkForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(checkForm);
                            break;
                        case PaymentInstrument.PurchaseOrder:
                            ASP.admin_orders_create_purchaseorderpaymentform_ascx poForm = new ASP.admin_orders_create_purchaseorderpaymentform_ascx();
                            poForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            poForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            poForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            poForm.ValidationGroup = "OPC";
                            poForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(poForm);
                            break;
                        case PaymentInstrument.PayPal:
                            ASP.admin_orders_create_paypalpaymentform_ascx paypalForm = new ASP.admin_orders_create_paypalpaymentform_ascx();
                            paypalForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            paypalForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            paypalForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            paypalForm.ValidationGroup = "OPC";
                            paypalForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(paypalForm);
                            break;
                        case PaymentInstrument.Mail:
                            ASP.admin_orders_create_mailpaymentform_ascx mailForm = new ASP.admin_orders_create_mailpaymentform_ascx();
                            mailForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            mailForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            mailForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            mailForm.ValidationGroup = "OPC";
                            mailForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(mailForm);
                            break;
                        case PaymentInstrument.PhoneCall:
                            ASP.admin_orders_create_phonecallpaymentform_ascx phoneCallForm = new ASP.admin_orders_create_phonecallpaymentform_ascx();
                            phoneCallForm.PaymentMethodId = selectedMethod.PaymentMethodId;
                            phoneCallForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
                            phoneCallForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
                            phoneCallForm.ValidationGroup = "OPC";
                            phoneCallForm.ValidationSummaryVisible = false;
                            phPaymentForms.Controls.Add(phoneCallForm);
                            break;
                        default:
                            //types not supported
                            break;
                    }
                }
            }
        }
        else
        {
            ASP.admin_orders_create_zerovaluepaymentform_ascx freeForm = new ASP.admin_orders_create_zerovaluepaymentform_ascx();
            freeForm.CheckingOut += new CheckingOutEventHandler(CheckingOut);
            freeForm.CheckedOut += new CheckedOutEventHandler(CheckedOut);
            freeForm.ValidationGroup = "OPC";
            freeForm.ValidationSummaryVisible = false;
            phPaymentForms.Controls.Add(freeForm);
        }

        //STORE THE VALUE OF THE BASKET THAT THE PAYMENT FORMS WERE BOUND TO
        hiddenOrderTotal = new HiddenField();
        hiddenOrderTotal.ID = "OT";
        hiddenOrderTotal.Value = orderTotal.ToString();
        phPaymentForms.Controls.Add(hiddenOrderTotal);

        //WE DO NOT NEED THE PAYMENT SELECTION LIST IF THERE IS NOT MORE THAN ONE
        //AVAILABLE TYPE OF PAYMENT
        tdPaymentMethodList.Visible = (PaymentMethodList.Items.Count > 1) && (orderTotal > 0);
    }

    void CheckedOut(object sender, CheckedOutEventArgs e)
    {
        CheckoutResponse response = e.CheckoutResponse;
        if (response.Success)
        {
            // STOP TRACKING THE ANONYMOUS USER
            Session["CreateOrder_AnonUserId"] = null;

            // CREATE / LINK USER ACCOUNT IF NEEDED
            if (RegisterPanel.Visible && CreateAccount.Checked)
            {
                if (_ExistingUser == null)
                {
                    // THERE IS NO EXISTING ACCOUNT, SO CREATE A NEW USER
                    string email = _User.PrimaryAddress.Email;
                    _User.UserName = email;
                    _User.Email = email;
                    _User.IsApproved = true;
                    _User.Save();
                    _User.SetPassword(Password.Text);
                    _User.Passwords[0].ForceExpiration = ForceExpiration.Checked;
                    _User.Save();
                }
                else
                {
                    // THERE IS AN EXISTING USER, SO MIGRATE THIS ORDER
                    OrderDataSource.UpdateUser(_UserId, _ExistingUser.UserId);
                    AddressDataSource.UpdateUser(_UserId, _ExistingUser.UserId);
                }
            }

            // REDIRECT TO THE FINAL ORDER
            Response.Redirect("~/Admin/Orders/ViewOrder.aspx?OrderId=" + response.OrderId + "&OrderNumber=" + response.OrderNumber);
        }
    }

    void CheckingOut(object sender, CheckingOutEventArgs e)
    {
        //MAKE SURE WE HAVE VALIDATED THIS FORM
        Page.Validate("OPC");
        //IF ANYTHING WAS INVALID CANCEL CHECKOUT
        if (!Page.IsValid) e.Cancel = true;
        //MAKE SURE THE SHIPPING MESSAGE IS SET
        if (!e.Cancel)
        {
            int shipmentIndex = 0;
            foreach (RepeaterItem item in ShipmentList.Items)
            {
                BasketShipment shipment = _Basket.Shipments[shipmentIndex];
                TextBox shipMessage = (TextBox)item.FindControl("ShipMessage");
                if (shipMessage != null)
                {
                    shipment.ShipMessage = StringHelper.Truncate(shipMessage.Text, 200);
                    shipment.Save();
                }
                shipmentIndex++;
            }
        }
    }

    protected void CreateAccount_CheckedChanged(object sender, EventArgs e)
    {
        PasswordRequired.Enabled = CreateAccount.Checked;
    }
}
