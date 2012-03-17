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
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Utility;

public partial class Admin_DigitalGoods_EditAgreement : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _AgreementId = 0;
    LicenseAgreement _Agreement;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _AgreementId = AlwaysConvert.ToInt(Request.QueryString["AgreementId"]);
        _Agreement = LicenseAgreementDataSource.Load(_AgreementId);
        if (_Agreement == null) Response.Redirect("Agreements.aspx");
        if (!Page.IsPostBack)
        {
            DisplayName.Text = _Agreement.DisplayName;
            IsHtml.Checked = _Agreement.IsHTML;
            AgreementText.Text = _Agreement.AgreementText;
        }
        DigitalGoodsGrid.DataSource = _Agreement.DigitalGoods;
        DigitalGoodsGrid.DataBind();
        Caption.Text = string.Format(Caption.Text, _Agreement.DisplayName);
        PageHelper.SetHtmlEditor(AgreementText, HtmlButton);
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Agreements.aspx");
    }

    private bool Save()
    {
        if (Page.IsValid)
        {
            _Agreement.DisplayName = DisplayName.Text;
            _Agreement.IsHTML = IsHtml.Checked;
            _Agreement.AgreementText = AgreementText.Text;
            _Agreement.Save();
            return true;
        }
        return false;
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Save())
        {
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        }
    }

    protected void SaveAndCloseButton_Click(object sender, System.EventArgs e)
    {
        if (Save()) Response.Redirect("Agreements.aspx");
    }
}
