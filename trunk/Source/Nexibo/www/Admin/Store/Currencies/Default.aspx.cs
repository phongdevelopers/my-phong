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
using System.Collections.Generic;

public partial class Admin_Store_Currencies_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void Page_Init(object sender, EventArgs e)
    {
        AddCurrencyDialog1.ItemAdded += new PersistentItemEventHandler(AddCurrencyDialog1_ItemAdded);
        EditCurrencyDialog1.ItemUpdated += new PersistentItemEventHandler(EditCurrencyDialog1_ItemUpdated);
        EditCurrencyDialog1.Cancelled += new EventHandler(EditCurrencyDialog1_Cancelled);
    }

    private void AddCurrencyDialog1_ItemAdded(object sender, PersistentItemEventArgs e)
    {
        CurrencyGrid.DataBind();
        CurrencyAjax.Update();
    }

    private void EditCurrencyDialog1_ItemUpdated(object sender, PersistentItemEventArgs e)
    {
        FinishEdit(true);
    }

    private void FinishEdit(bool rebind)
    {
        CurrencyGrid.EditIndex = -1;
        if (rebind) CurrencyGrid.DataBind();
        CurrencyAjax.Update();
    }
    
    private void EditCurrencyDialog1_Cancelled(object sender, EventArgs e)
    {
        FinishEdit(false);
    }

    protected void CurrencyGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int currencyId = (int)CurrencyGrid.DataKeys[e.NewEditIndex].Value;
        Currency currency = CurrencyDataSource.Load(currencyId);
        if (currency != null)
        {
            ASP.admin_store_currencies_editcurrencydialog_ascx editDialog = CurrencyAjax.FindControl("EditCurrencyDialog1") as ASP.admin_store_currencies_editcurrencydialog_ascx;
            if (editDialog != null) editDialog.CurrencyId = currencyId;
        }
    }    

    protected void Page_PreRender(object sender, EventArgs e)
    {
        BaseCurrencyMessage.Text = string.Format(BaseCurrencyMessage.Text, Token.Instance.Store.BaseCurrency.Name);
    }

    protected string GetSample(object dataItem)
    {
        Currency currency = (Currency)dataItem;
        LSDecimal sample = 1234.56M;
        return sample.ToString("lcf", currency);
    }

    protected void CurrencyGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "UpdateRate")
        {
            Currency currency = CurrencyDataSource.Load(AlwaysConvert.ToInt(e.CommandArgument));
            if (currency != null)
            {
                try
                {
                    currency.UpdateExchangeRate(true);                    
                    CurrencyGrid.DataBind();
                    CurrencyAjax.Update();
                }
                catch (Exception ex)
                {
                    // Throw the exception with proper error message
                    throw new Exception("Unable to update the exchange rate.  The rate provider may be experiencing technical difficulties, please try again later.  Exception message: " + ex.Message, ex);
                }
            }
        }
    }

    protected void ForexProvider_SelectedIndexChanged(object sender, EventArgs e)
    {
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        settings.ForexProviderClassId = ForexProvider.SelectedValue;
        settings.Save();
    }

    protected void ForexProvider_DataBound(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            ListItem item = ForexProvider.Items.FindByValue(settings.ForexProviderClassId);
            if (item != null) item.Selected = true;
        }
    }

}
