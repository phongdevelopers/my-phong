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
using CommerceBuilder.Payments;

public partial class Admin_Orders_OrderMenu : System.Web.UI.UserControl
{
    private int _OrderId;
    private Order _Order;

    private void InitOrder()
    {
        //FIRST CHECK FOR ORDER
        _Order = OrderHelper.GetOrderFromContext();
        if (_Order != null)
        {
            _OrderId = _Order.OrderId;
        }
        else
        {
            //NEXT CHECK FOR PAYMENT
            int _PaymentId = AlwaysConvert.ToInt(Request.QueryString["PaymentId"]);
            Payment _Payment = PaymentDataSource.Load(_PaymentId);
            if (_Payment != null)
            {
                _OrderId = _Payment.OrderId;
                _Order = _Payment.Order;
            }
            else
            {
                //NEXT CHECK FOR SHIPMENT
                int _OrderShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
                OrderShipment _OrderShipment = OrderShipmentDataSource.Load(_OrderShipmentId);
                if (_OrderShipment != null)
                {
                    _OrderId = _OrderShipment.OrderId;
                    _Order = _OrderShipment.Order;
                }
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        InitOrder();
        if (_Order != null)
        {
            string suffix = "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
            Summary.NavigateUrl += suffix;
            Payments.NavigateUrl += suffix;
            Shipments.NavigateUrl += suffix;
            History.NavigateUrl += suffix;
            DigitalGoods.NavigateUrl += suffix;
            GiftCertificates.NavigateUrl += suffix;
            Subscriptions.NavigateUrl += suffix;
            EditAddresses.NavigateUrl += suffix;
            EditOrderItems.NavigateUrl += suffix;
            string packslipUrl = Page.ResolveUrl("PackingList.aspx");
            Customer.NavigateUrl += "?UserId=" + _Order.UserId.ToString();
            string fileName = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLowerInvariant();
            switch (fileName)
            {
                case "addpayment.aspx":
                case "editpayment.aspx":
                case "capturepayment.aspx":
                case "voidpayment.aspx":
                case "refundpayment.aspx":
                    Payments.CssClass = "contextMenuButtonSelected";
                    break;
                case "default.aspx":
                    string filePath = Request.Url.AbsolutePath.ToString();
                    if (filePath.EndsWith("Payments/Default.aspx")) Payments.CssClass = "contextMenuButtonSelected";
                    else if (filePath.EndsWith("Shipments/Default.aspx")) Shipments.CssClass = "contextMenuButtonSelected";
                    break;
                case "addshipment.aspx":
                case "deleteshipment.aspx":
                case "editshipment.aspx":
                case "mergeshipment.aspx":
                case "splitshipment.aspx":
                case "returnshipment.aspx":
                    Shipments.CssClass = "contextMenuButtonSelected";
                    break;
                case "viewdigitalgoods.aspx":
                case "adddigitalgoods.aspx":
                case "adddigitalgoodserialkey.aspx":
                case "viewdigitalgoodserialkey.aspx":
                case "viewdownloads.aspx":
                    DigitalGoods.CssClass = "contextMenuButtonSelected";
                    break;
                case "orderhistory.aspx":
                    History.CssClass = "contextMenuButtonSelected";
                    break;
                case "viewsubscriptions.aspx":
                    Subscriptions.CssClass = "contextMenuButtonSelected";
                    break;
                case "editaddresses.aspx":
                    EditAddresses.CssClass = "contextMenuButtonSelected";
                    break;
                case "editorderitems.aspx":
                    EditOrderItems.CssClass = "contextMenuButtonSelected";
                    break;
                case "addproduct.aspx":
                case "findproduct.aspx":
                case "addother.aspx":
                    string tempFilePath = Request.Url.AbsolutePath.ToString();
                    if (tempFilePath.Contains("/Shipments/")) Shipments.CssClass = "contextMenuButtonSelected";
                    else EditOrderItems.CssClass = "contextMenuButtonSelected";
                    break;
                case "viewgiftcertificates.aspx":
                    GiftCertificates.CssClass = "contextMenuButtonSelected";
                    break;
                default:
                    Summary.CssClass = "contextMenuButtonSelected"; break;
            }
        }
        else OrderMenuPanel.Visible = false;
    }
}
