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
using CommerceBuilder.Common;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;
using System.IO;

public partial class Admin_Products_DigitalGoods_AttachDigitalGood : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _ProductId = 0;
    private string _OptionList = string.Empty;
    private Product _Product;
    private ProductVariantManager _VariantManager;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        string optionList = EncryptionHelper.DecryptAES(Request.QueryString["Options"]);
        _OptionList = ProductVariant.ValidateOptionList(optionList);
        _Product = ProductDataSource.Load(_ProductId);
        string productName = _Product.Name;
        _VariantManager = new ProductVariantManager(_ProductId);
        ProductVariant v = _VariantManager.GetVariantFromOptions(_OptionList);
        if (v != null) productName = _Product.Name + " (" + v.VariantName + ")";
        else _OptionList = string.Empty;
        Caption.Text = string.Format(Caption.Text, productName);
        CancelButton.NavigateUrl = "DigitalGoods.aspx?ProductId=" + _ProductId.ToString();
        if (!Page.IsPostBack)
        {
            SearchResultsGrid.Visible = false;
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.DataBind();
    }

    protected string GetFileSize(long fileSize)
    {
        if (fileSize < 1024) return fileSize.ToString();
        LSDecimal tempSize = fileSize / 1024;
        if (tempSize < 1024) return string.Format("{0:0.#}kb", tempSize);
        tempSize = tempSize / 1024;
        return string.Format("{0:F1}mb", tempSize);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        SearchName.Focus();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        AttachButton.Visible = (SearchResultsGrid.Visible && SearchResultsGrid.Rows.Count > 0);
    }

    protected void AttachButton_Click(object sender, EventArgs e)
    {
        string attach = Request.Form["attach"];
        if (!string.IsNullOrEmpty(attach))
        {
            string[] ids = attach.Replace(" ", "").Split(",".ToCharArray());
            foreach (string strId in ids)
            {
                int dgid = AlwaysConvert.ToInt(strId);
                DigitalGood dg = DigitalGoodDataSource.Load(dgid);
                if (dg != null)
                {
                    int index = _Product.DigitalGoods.IndexOfValue(dg.DigitalGoodId, _OptionList);
                    if (index < 0)
                    {
                        ProductDigitalGood pdg = new ProductDigitalGood();
                        pdg.ProductId = _ProductId;
                        pdg.OptionList = _OptionList;
                        pdg.DigitalGoodId = dg.DigitalGoodId;
                        _Product.DigitalGoods.Add(pdg);
                    }
                }
            }
            _Product.DigitalGoods.Save();
        }
        Response.Redirect(CancelButton.NavigateUrl);
    }

    protected bool DGFileExists(object dataItem)
    {
        DigitalGood dg = (DigitalGood)dataItem;
        return File.Exists(dg.AbsoluteFilePath);
    }
}