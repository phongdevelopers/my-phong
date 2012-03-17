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
using CommerceBuilder.Shipping;

public partial class Admin_Shipping_Countries_AddProvinceDialog : System.Web.UI.UserControl
{
    public event EventHandler ItemAdded;

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Province province = new Province();
            province.CountryCode = Request.QueryString["CountryCode"];
            province.Name = Name.Text.Trim();
            province.ProvinceCode = ProvinceCode.Text.Trim();
            province.Save();
            //RESET FORM
            Name.Text = string.Empty;
            ProvinceCode.Text = string.Empty;
            AddedMessage.Text = string.Format(AddedMessage.Text, province.Name);
            AddedMessage.Visible = true;
            //TRIGER ANY EVENT ATTACHED TO THE UPDATE
            if (ItemAdded != null) ItemAdded(this, new EventArgs());
        }
    }
}
