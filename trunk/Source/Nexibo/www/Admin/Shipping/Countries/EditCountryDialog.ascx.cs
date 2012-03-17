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
using CommerceBuilder.Utility;
using CommerceBuilder.Shipping;

public partial class Admin_Shipping_Countries_EditCountryDialog : System.Web.UI.UserControl
{
    public event EventHandler ItemUpdated;
    public event EventHandler Cancelled;

    public string CountryCode
    {
        get { return AlwaysConvert.ToString(ViewState["CountryCode"]); }
        set { ViewState["CountryCode"] = value; }
    }

    public void LoadDialog(string countryCode)
    {
        this.CountryCode = countryCode;
        Country country = CountryDataSource.Load(countryCode);
        Caption.Text = string.Format(Caption.Text, country.Name);
        CountryCodeLabel2.Text = countryCode;
        Name.Text = country.Name;
        AddressFormat.Text = country.AddressFormat;
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        //TRIGER ANY EVENT ATTACHED TO THE CANCEL
        if (Cancelled != null) Cancelled(sender, e);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Country country = CountryDataSource.Load(CountryCode);
            country.Name = Name.Text;
            country.AddressFormat = AddressFormat.Text;
            country.Save();
            //TRIGER ANY EVENT ATTACHED TO THE UPDATE
            if (ItemUpdated != null) ItemUpdated(this, new EventArgs());
        }
    }
}
