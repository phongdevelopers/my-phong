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
using CommerceBuilder.Utility;
using CommerceBuilder.DigitalDelivery;

public partial class Admin_DigitalGoods_FindDigitalGoods : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.DataBind();
    }

    protected string GetFileSize(int fileSize)
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

    protected void SearchResultsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Download")
        {
            int digitalGoodId = AlwaysConvert.ToInt(e.CommandArgument);
            DigitalGood dg = DigitalGoodDataSource.Load(digitalGoodId);
            if (dg != null) PageHelper.SendFileDataToClient(dg);
        }
    }

    protected bool DGFileExists(object dataItem)
    {
        DigitalGood dg = (DigitalGood)dataItem;
        return System.IO.File.Exists(dg.AbsoluteFilePath);
    }
}
