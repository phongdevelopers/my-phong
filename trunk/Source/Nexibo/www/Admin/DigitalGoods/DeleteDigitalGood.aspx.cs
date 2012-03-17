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
using CommerceBuilder.Orders;
using System.Collections.Generic;

public partial class Admin_DigitalGoods_DeleteDigitalGood : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _DigitalGoodId = 0;
    DigitalGood _DigitalGood;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _DigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["DigitalGoodId"]);
        _DigitalGood = DigitalGoodDataSource.Load(_DigitalGoodId);
        if (_DigitalGood == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _DigitalGood.Name);
        InstructionText.Text = string.Format(InstructionText.Text, _DigitalGood.Name);
        ProductsGrid.DataSource = _DigitalGood.ProductDigitalGoods;
        ProductsGrid.DataBind();
        //GET ALL ORDER ITEMS ASSOCIATED WITH DIGITAL GOOD
        OrderItemDigitalGoodCollection oidgs = OrderItemDigitalGoodDataSource.LoadForDigitalGood(_DigitalGoodId);
        //BUILD DISTINCT LIST OF ORDERS
        List<Order> orders = new List<Order>();
        foreach (OrderItemDigitalGood oidg in oidgs)
        {
            Order order = oidg.OrderItem.Order;
            if (orders.IndexOf(order) < 0) orders.Add(order);
        }
        //BIND TO GRID
        OrderGrid.DataSource = orders;
        OrderGrid.DataBind();

        if (!String.IsNullOrEmpty(_DigitalGood.FileName))
        {
            DigitalGoodCollection dgc = DigitalGoodDataSource.LoadForCriteria("FileName = '" + StringHelper.SafeSqlString(_DigitalGood.FileName) + "'");
            if (dgc != null && dgc.Count > 1)
            {
                DeleteAllowedPanel.Visible = false;
                DeletePreventedPanel.Visible = true;
                NoDeleteFileText.Text = string.Format(NoDeleteFileText.Text, _DigitalGood.FileName);
            }
            else
            {
                DeleteAllowedPanel.Visible = true;
                DeletePreventedPanel.Visible = false;
                DeleteFile.Text = string.Format(DeleteFile.Text, _DigitalGood.FileName);
            }
        }
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("DigitalGoods.aspx");
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            _DigitalGood.Delete(DeleteFile.Checked);
            Response.Redirect("DigitalGoods.aspx");
        }
    }
}
