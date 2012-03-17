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

public partial class Admin_DigitalGoods_DeleteReadme : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _ReadmeId = 0;
    Readme _Readme;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _ReadmeId = AlwaysConvert.ToInt(Request.QueryString["ReadmeId"]);
        _Readme = ReadmeDataSource.Load(_ReadmeId);
        if (_Readme == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _Readme.DisplayName);
        InstructionText.Text = string.Format(InstructionText.Text, _Readme.DisplayName);
        BindReadmes();
        ProductsGrid.DataSource = _Readme.DigitalGoods;
        ProductsGrid.DataBind();
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Readmes.aspx");
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            _Readme.Delete(AlwaysConvert.ToInt(ReadmeList.SelectedValue));
            Response.Redirect("Readmes.aspx");
        }
    }

    private void BindReadmes()
    {
        ReadmeCollection readmes = ReadmeDataSource.LoadForStore("DisplayName");
        int index = readmes.IndexOf(_ReadmeId);
        if (index > -1) readmes.RemoveAt(index);
        ReadmeList.DataSource = readmes;
        ReadmeList.DataBind();
    }
}
