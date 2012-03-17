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

public partial class Admin_Shipping_Countries_AddCountryDialog : System.Web.UI.UserControl
{
    public event EventHandler ItemAdded;

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            string code = CountryCode.Text.ToUpperInvariant();
            //VERIFY THIS CODE IS NOT ALREADY PRESENT
            Country t = CountryDataSource.Load(code);
            if (t == null)
            {
                t = new Country();
                t.CountryCode = code;
                t.Name = Name.Text.Trim();
                t.Save();
                CountryCode.Text = string.Empty;
                Name.Text = string.Empty;
                AddedMessage.Text = string.Format(AddedMessage.Text, t.Name);
                AddedMessage.Visible = true;
                //TRIGER ANY EVENT ATTACHED TO THE UPDATE
                if (ItemAdded != null) ItemAdded(this, new EventArgs());
            }
            else
            {
                CustomValidator existingCode = new CustomValidator();
                existingCode.ValidationGroup = "AddCountry";
                existingCode.IsValid = false;
                existingCode.Text = "*";
                existingCode.ErrorMessage = "A country with the code " + code + " is already defined.";
                tdCountryCode.Controls.Add(existingCode);
                //ValidationSummary1.Visible = true;
                //ValidationSummary1.Controls.Add(existingCode);
                //ValidationSummary1.Visible = true;
            }
        }
    }
}
