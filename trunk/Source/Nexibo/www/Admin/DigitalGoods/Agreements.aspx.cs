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
using System.Collections.Generic;
using CommerceBuilder.DigitalDelivery;

public partial class Admin_DigitalGoods_Agreements : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void AddAgreementButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            LicenseAgreement agreement = new LicenseAgreement();
            agreement.DisplayName = AddAgreementName.Text;
            agreement.Save();
            Response.Redirect("EditAgreement.aspx?AgreementId=" + agreement.LicenseAgreementId);
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(AddAgreementName, AddAgreementButton.ClientID);
        AddAgreementName.Focus();
    }

    private Dictionary<int, int> _ProductCounts = new Dictionary<int, int>();
    protected int GetProductCount(object dataItem)
    {
        LicenseAgreement m = (LicenseAgreement)dataItem;
        if (_ProductCounts.ContainsKey(m.LicenseAgreementId)) return _ProductCounts[m.LicenseAgreementId];
        int count = DigitalGoodDataSource.CountForLicenseAgreement(m.LicenseAgreementId);
        _ProductCounts[m.LicenseAgreementId] = count;
        return count;
    }

    protected bool HasProducts(object dataItem)
    {
        return (GetProductCount(dataItem) > 0);
    }
}
