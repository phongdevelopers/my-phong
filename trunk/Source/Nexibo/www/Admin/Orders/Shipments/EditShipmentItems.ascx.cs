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
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Shipments_EditShipmentItems : System.Web.UI.UserControl
{
    private int _ShipmentId;
    private OrderShipment _OrderShipment;


    [Personalizable(), WebBrowsable()]
    public String CaptionText
    {
        get { return Caption.Text; }
        set { Caption.Text = value; }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        _ShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        _OrderShipment = OrderShipmentDataSource.Load(_ShipmentId);
        if (_OrderShipment == null) Response.Redirect("Default.aspx");

        AddProductLink.NavigateUrl += "?OrderShipmentId=" + _ShipmentId.ToString();
        AddOtherItemLink.NavigateUrl += "?OrderShipmentId=" + _ShipmentId.ToString();
    }

    protected void OrderItemGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int orderItemId = (int)OrderItemGrid.DataKeys[e.RowIndex].Value;
        int index = _OrderShipment.OrderItems.IndexOf(orderItemId);
        if (index > -1)
        {
            OrderItem orderItem = _OrderShipment.OrderItems[index];
            orderItem.Price = AlwaysConvert.ToDecimal(e.NewValues["Price"]);
            orderItem.Quantity = AlwaysConvert.ToInt16(e.NewValues["Quantity"]);
            //UPDATE THE CALCULATED SUMMARY VALUES OF ORDER
            _OrderShipment.Save();
        }
        OrderItemGrid.EditIndex = -1;
        e.Cancel = true;
    }

    protected bool IsGiftCert(object dataItem)
    {
        OrderItem orderItem = dataItem as OrderItem;
        if (orderItem == null) return false;
        return (orderItem.GiftCertificates.Count > 0);
    }


    protected bool ShowInputPanel(object dataItem)
    {
        return (((OrderItem)dataItem).Inputs.Count > 0);
    }

    protected void OrderItemGrid_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        //UPDATE THE CALCULATED SUMMARY VALUES OF ORDER
        _OrderShipment.Save();
    }
}
