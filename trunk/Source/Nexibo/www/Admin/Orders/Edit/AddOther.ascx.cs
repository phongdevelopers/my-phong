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
using CommerceBuilder.Marketing;

public partial class Admin_Orders_Edit_AddOtherControl: System.Web.UI.UserControl
{
    private int _OrderId;
    private Order _Order;
    private int _OrderShipmentId;
    private OrderShipment _OrderShipment;

    protected int OrderId
    {
        set
        {
            _OrderId = value;
            _Order = OrderDataSource.Load(_OrderId);
        }
        get { return _OrderId; }
    }

    protected int OrderShipmentId
    {
        set
        {
            _OrderShipmentId = value;
            _OrderShipment = OrderShipmentDataSource.Load(_OrderShipmentId);
            if (_OrderShipment != null) _Order = _OrderShipment.Order;
        }
        get { return _OrderShipmentId; }
    }

    public void Page_Init(object sender, EventArgs e)
    {
        if (_Order == null) OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        if (_OrderShipment == null) OrderShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        if (_Order == null) Response.Redirect("~/Admin/Orders/Default.aspx");

        if (_OrderShipment != null) CancelLink.NavigateUrl = "~/Admin/Orders/Shipments/EditShipment.aspx?OrderShipmentId=" + _OrderShipmentId.ToString();
        else CancelLink.NavigateUrl = "~/Admin/Orders/Edit/EditOrderItems.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();

        //INITIALIZE ORDERITEMTYPE DROPDOWN
        foreach (OrderItemType oit in Enum.GetValues(typeof(OrderItemType)))
        {
            if (oit != OrderItemType.Product && oit != OrderItemType.GiftCertificate && oit != OrderItemType.GiftCertificatePayment && oit != OrderItemType.GiftWrap && oit != OrderItemType.Coupon)
                ItemType.Items.Add(new ListItem(StringHelper.SpaceName(oit.ToString()), ((int)oit).ToString()));
        }

        trShipmentList.Visible = (_OrderShipment == null);
        if (trShipmentList.Visible)
        {

            //BIND SHIPMENT LIST
            foreach (OrderShipment shipment in _Order.Shipments)
            {
                string address = string.Format("{0} {1} {2} {3}", shipment.ShipToFirstName, shipment.ShipToLastName, shipment.ShipToAddress1, shipment.ShipToCity);
                if (address.Length > 50) address = address.Substring(0, 47) + "...";
                string name = "Shipment #" + shipment.ShipmentNumber + " to " + address;
                ShipmentsList.Items.Add(new ListItem(name, shipment.OrderShipmentId.ToString()));
            }
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (AlwaysConvert.ToInt16(ItemType.SelectedValue) == (Int16)OrderItemType.Coupon)
            {
                //IF IT IS A COUPON, VALIDATE THE ENTERED SKU ( COUPON CODE )
                if (CouponDataSource.LoadForCouponCode(Sku.Text) == null)
                {
                    CustomValidator skuValidator = new CustomValidator();
                    skuValidator.IsValid = false;
                    skuValidator.ControlToValidate = "Sku";
                    skuValidator.ValidationGroup = ValidationSummary.ValidationGroup;
                    skuValidator.ErrorMessage = "Sku is not a valid coupon code.";
                    skuValidator.Text = "*";
                    phSkuValidators.Controls.Add(skuValidator);
                    return;
                }
            }
            //add the custom item
            OrderItem oi = new OrderItem();
            oi.OrderId = _OrderId;
            oi.OrderItemTypeId = AlwaysConvert.ToInt16(ItemType.SelectedValue);
            if (_OrderShipment == null) oi.OrderShipmentId = AlwaysConvert.ToInt(ShipmentsList.SelectedValue);
            else oi.OrderShipmentId = _OrderShipmentId;
            oi.Name = Name.Text;
            oi.Sku = Sku.Text;
            Decimal price = AlwaysConvert.ToDecimal(Price.Text);

            //We should not allow positive values for Discount or Credit.  We should not
            //  allow negative values for Charge.
            switch (oi.OrderItemTypeId)
            {
                case ((short)OrderItemType.Discount):
                case ((short)OrderItemType.Credit):
                    if (price > 0) price = Decimal.Negate(price);
                    break;

                case ((short)OrderItemType.Charge):
                    if (price < 0) price = Decimal.Negate(price);
                    break;

            }

            oi.Price = price;
            oi.Quantity = AlwaysConvert.ToInt16(Quantity.Text);
            _Order.Items.Add(oi);
            _Order.Save();
            Response.Redirect(CancelLink.NavigateUrl);
        }
    }
}
