using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Reporting;
using CommerceBuilder.Taxes;
using System.Collections.Generic;

public partial class Admin_Products_EditSubscription : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _ProductId;
    private Product _Product;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _Product = ProductDataSource.Load(_ProductId);
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        BillingOption.Items[0].Text = string.Format(BillingOption.Items[0].Text, _Product.Price);
        BillingOption.Items[1].Text = string.Format(BillingOption.Items[1].Text, _Product.Price);
        BillingOption.Items[2].Text = string.Format(BillingOption.Items[2].Text, _Product.Price);
        InitialCharge.Text = string.Format(InitialCharge.Text, _Product.Price);
        if (_Product.SubscriptionPlan != null) ShowEditForm();
    }

    private void BindSubscriptionGroup()
    {
        SubscriptionGroup.Items.Clear();
        SubscriptionGroup.Items.Add(new ListItem(string.Empty));
		CommerceBuilder.Users.GroupCollection groupCol = GroupDataSource.LoadForStore("Name");

		CommerceBuilder.Users.Group group;
		for(int i=groupCol.Count -1; i>=0; i--) 
		{
			group = groupCol[i];
			if(group.GroupRoles.Count > 0)
			{
				groupCol.RemoveAt(i);
			}
		}

        SubscriptionGroup.DataSource = groupCol;
        SubscriptionGroup.DataBind();
    }

    protected void ShowAddForm_Click(object sender, EventArgs e)
    {
        NoSubscriptionPlanPanel.Visible = false;
        SubscriptionPlanForm.Visible = true;
        Name.Text = _Product.Name;
        BillingOption.SelectedIndex = 0;
        UpdateBillingOption();
        AddSubscriptionPlanButtons.Visible = true;
        EditSubscriptionPlanButtons.Visible = false;
    }

    private void ShowNoForm()
    {
        NoSubscriptionPlanPanel.Visible = true;
        SubscriptionPlanForm.Visible = false;
        AddSubscriptionPlanButtons.Visible = false;
        EditSubscriptionPlanButtons.Visible = false;
    }

    private void ShowEditForm()
    {
        NoSubscriptionPlanPanel.Visible = false;
        SubscriptionPlanForm.Visible = true;
        AddSubscriptionPlanButtons.Visible = false;
        EditSubscriptionPlanButtons.Visible = true;
        if (!Page.IsPostBack)
        {
            //INITIALIZE FORM
            SubscriptionPlan sp = _Product.SubscriptionPlan;
            Name.Text = sp.Name;
            //DETERMINE BILLING OPTION
            if (sp.NumberOfPayments == 1)
                //ONETIME CHARGE
                BillingOption.SelectedIndex = 0;
            else if (!sp.RecurringChargeSpecified)
                //RECURRING USING INITIAL PAYMENT AMOUNT
                BillingOption.SelectedIndex = 1;
            else 
                //RECURING WITH INITIAL PAYMENT AND SEPARATE RECURRING AMOUNT
                BillingOption.SelectedIndex = 2;
            UpdateBillingOption();
            if (sp.PaymentFrequency > 0) PaymentFrequency.Text = sp.PaymentFrequency.ToString();
            else PaymentFrequency.Text = string.Empty;
            if (sp.PaymentFrequencyUnit == PaymentFrequencyUnit.Day) ddlPaymentFrequencyUnit.SelectedIndex = 0;
            if (sp.PaymentFrequencyUnit == PaymentFrequencyUnit.Month) ddlPaymentFrequencyUnit.SelectedIndex = 1;
            if (sp.NumberOfPayments > 0) NumberOfPayments.Text = sp.NumberOfPayments.ToString();
            else NumberOfPayments.Text = string.Empty;
            RecurringCharge.Text = sp.RecurringCharge.ToString("F2");
            ListItem item = TaxCode.Items.FindByValue(sp.TaxCodeId.ToString());
            if (item != null) TaxCode.SelectedIndex = TaxCode.Items.IndexOf(item);
            item = SubscriptionGroup.Items.FindByValue(sp.GroupId.ToString());
            if (item != null) SubscriptionGroup.SelectedIndex = SubscriptionGroup.Items.IndexOf(item);
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ShowNoForm();
    }

    private void Save(SubscriptionPlan sp)
    {
        sp.ProductId = _ProductId;
        sp.Name = Name.Text;
        switch (BillingOption.SelectedIndex)
        {
            case 0:
                // ONE TIME CHARGE
                sp.NumberOfPayments = 1;
                sp.RecurringCharge = 0;
                sp.RecurringChargeSpecified = false;
                sp.TaxCodeId = 0;
                break;
            case 1:
                // RECURRING CHARGE
                sp.NumberOfPayments = AlwaysConvert.ToInt16(NumberOfPayments.Text);
                sp.RecurringCharge = 0;
                sp.RecurringChargeSpecified = false;
                sp.TaxCodeId = 0;
                break;
            case 2:
                // INITIAL CHARGE WITH DIFFERENT RECURRING CHARGE
                sp.NumberOfPayments = AlwaysConvert.ToInt16(NumberOfPayments.Text);
                sp.RecurringCharge = AlwaysConvert.ToDecimal(RecurringCharge.Text); ;
                sp.RecurringChargeSpecified = true;
                sp.TaxCodeId = AlwaysConvert.ToInt(TaxCode.SelectedValue);
                break;
        }
        switch (ddlPaymentFrequencyUnit.SelectedIndex)
        {
            case 0:
                //DAYS
                sp.PaymentFrequency = AlwaysConvert.ToInt16(PaymentFrequency.Text);
                sp.PaymentFrequencyUnit = PaymentFrequencyUnit.Day;
                break;
            case 1:
                //MONTHS
                sp.PaymentFrequency = AlwaysConvert.ToInt16(PaymentFrequency.Text);
                sp.PaymentFrequencyUnit = PaymentFrequencyUnit.Month;
                break;
        }
        sp.GroupId = AlwaysConvert.ToInt(SubscriptionGroup.SelectedValue);
        sp.Save();
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        SubscriptionPlan sp = new SubscriptionPlan();
        this.Save(sp);
        ShowEditForm();
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        if (_Product.SubscriptionPlan != null) 
            this.Save(_Product.SubscriptionPlan);
        else ShowNoForm();
    }

    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        if (_Product.SubscriptionPlan != null)
        {
            if (_Product.SubscriptionPlan.Subscriptions == null || _Product.SubscriptionPlan.Subscriptions.Count == 0)
            {
                _Product.SubscriptionPlan.Delete();
                ShowNoForm();
            }
            else
            {
                ErrorMessageLabel.Text = "Subscription plan can not be deleted because it is currently active.";
                ErrorMessageLabel.Visible = true;
            }
        }        
    }

    protected void NewGroupButton_Click(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(NewName.Value))
        {
            CommerceBuilder.Users.Group group = new CommerceBuilder.Users.Group();
            group.Name = NewName.Value;
            group.Save();
            BindSubscriptionGroup();
            ListItem item = SubscriptionGroup.Items.FindByValue(group.GroupId.ToString());
            if (item != null) SubscriptionGroup.SelectedIndex = SubscriptionGroup.Items.IndexOf(item);
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        //WE DO THIS EACH TIME SO THAT LIST ITEMS DO NOT HAVE TO RESIDE IN VIEWSTATE
        BindSubscriptionGroup();
        TaxCode.DataSource = TaxCodeDataSource.LoadForStore("Name");
        TaxCode.DataBind();
    }

    protected void BillingOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateBillingOption();
    }

    private void UpdateBillingOption()
    {
        if (BillingOption.SelectedIndex == 0)
        {
            // ONE TIME CHARGE
            SubscriptionExpirationLabel.Visible = true;
            PaymentFrequencyLabel.Visible = false;
            PaymentFrequencyEveryLabel.Visible = false;
            trInitialCharge.Visible = false;
            trRecurringCharge.Visible = false;
            trNumberOfPayments.Visible = false;
        }
        else if (BillingOption.SelectedIndex == 1)
        {
            // RECURRING CHARGE
            SubscriptionExpirationLabel.Visible = false;
            PaymentFrequencyLabel.Visible = true;
            PaymentFrequencyEveryLabel.Visible = true;
            trInitialCharge.Visible = false;
            trRecurringCharge.Visible = false;
            trNumberOfPayments.Visible = true;
        }
        else
        {
            // INITIAL CHARGE WITH DIFFERENT RECURRING CHARGE
            SubscriptionExpirationLabel.Visible = false;
            PaymentFrequencyLabel.Visible = true;
            PaymentFrequencyEveryLabel.Visible = true;
            trInitialCharge.Visible = true;
            trRecurringCharge.Visible = true;
            trNumberOfPayments.Visible = true;
        }
    }
}
