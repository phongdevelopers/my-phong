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

public partial class Admin_DigitalGoods_EditReadme : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _ReadmeId = 0;
    Readme _Readme;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _ReadmeId = AlwaysConvert.ToInt(Request.QueryString["ReadmeId"]);
        _Readme = ReadmeDataSource.Load(_ReadmeId);
        if (_Readme == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            DisplayName.Text = _Readme.DisplayName;
            IsHtml.Checked = _Readme.IsHTML;
            ReadmeText.Text = _Readme.ReadmeText;
        }
        DigitalGoodsGrid.DataSource = _Readme.DigitalGoods;
        DigitalGoodsGrid.DataBind();
        Caption.Text = string.Format(Caption.Text, _Readme.DisplayName);
        PageHelper.SetHtmlEditor(ReadmeText, HtmlButton);
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Readmes.aspx");
    }

    private bool Save()
    {
        if (Page.IsValid)
        {
            _Readme.DisplayName = DisplayName.Text;
            _Readme.IsHTML = IsHtml.Checked;
            _Readme.ReadmeText = ReadmeText.Text;
            _Readme.Save();
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
        if (Save()) Response.Redirect("Readmes.aspx");
    }
}
