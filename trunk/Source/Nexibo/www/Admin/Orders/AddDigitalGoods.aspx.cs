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
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;
using CommerceBuilder.Shipping;

public partial class Admin_Orders_AddDigitalGoods : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _OrderId;    
    private Order _Order;

    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order == null) Response.Redirect("Default.aspx");
        SearchName.Focus();
        CancelLink.NavigateUrl = String.Format(CancelLink.NavigateUrl, _Order.OrderNumber, _OrderId);
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.DataBind();
    }

    protected void OkButton_Click(object sender, EventArgs e)
    {
        // Looping through all the rows in the GridView
        foreach (GridViewRow row in SearchResultsGrid.Rows)
        {
            CheckBox checkbox = (CheckBox)row.FindControl("SelectCheckbox");
            if ((checkbox != null) && (checkbox.Checked))
            {
                // Retreive the DigitalGood
                int digitalGoodId = Convert.ToInt32(SearchResultsGrid.DataKeys[row.RowIndex].Value);
                DigitalGood dg = DigitalGoodDataSource.Load(digitalGoodId);

                // ( Bug 8262 ) CREATE A NEW ORDER ITEM FOR EACH DIGITAL GOOD
                OrderItem oi = new OrderItem();
                oi.OrderId = _OrderId;
                oi.CustomFields["DigitalGoodIndicator"] = "1";
                oi.Name = dg.Name;
                oi.OrderItemType = OrderItemType.Product;
                oi.Price = 0;
                oi.Quantity = 1;
                oi.Shippable = Shippable.No;
                oi.IsHidden = true;
                oi.Save();

                OrderItemDigitalGood orderItemDigitalGood = new OrderItemDigitalGood();
                orderItemDigitalGood.OrderItemId = oi.OrderItemId;
                orderItemDigitalGood.DigitalGoodId = digitalGoodId;
                orderItemDigitalGood.Name = dg.Name;
                orderItemDigitalGood.Save();
                orderItemDigitalGood.Save();
            }
        }
        Response.Redirect(String.Format("ViewDigitalGoods.aspx?OrderNumber={0}&OrderId={1}", _Order.OrderNumber, _OrderId));
    }
}
