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
using CommerceBuilder.Products;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

public partial class Admin_Orders_Shipments_ReturnItemDialog : System.Web.UI.UserControl
{
    private OrderItem _OrderItem;
    public OrderItem OrderItem
    {
        get
        {
            if (_OrderItem == null)
            {
                // TRY TO LOAD FROM FORM
                int orderItemId = AlwaysConvert.ToInt(OrderItemId.Text);
                _OrderItem = OrderItemDataSource.Load(orderItemId);
            }
            return _OrderItem;
        }
        set
        {
            _OrderItem = value;
            UpdateForm();
        }
    }

    protected void UpdateForm()
    {
        OrderItemId.Text = OrderItem.OrderItemId.ToString();

        if (OrderItem.VariantName.Length > 0)
        {
            Name.Text = OrderItem.Name + " (" + OrderItem.VariantName + ")";
        }
        else Name.Text = OrderItem.Name;
        Quantity.Text = OrderItem.Quantity.ToString();
        QuantityReturn.Text = OrderItem.Quantity.ToString();
        QuantityRangeValidator.MaximumValue = OrderItem.Quantity.ToString();
        trInventory.Visible = ((OrderItem.Product != null) && (OrderItem.Product.InventoryMode != InventoryMode.None));
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        short returnQuantity = AlwaysConvert.ToInt16(QuantityReturn.Text);
        if (returnQuantity > 0)
        {
            if ((ReturnToInvRadio.SelectedIndex == 0) && (OrderItem.Product != null) && (OrderItem.Product.InventoryMode != InventoryMode.None))
            {
                //RETURN PRODUCT TO INVENTORY
                try
                {
                    InventoryManager.Restock(returnQuantity, OrderItem.ProductId, OrderItem.OptionList);
                }
                catch (InvalidProductException ex)
                {
                    Logger.Error("Error while returning order item with productid = " + OrderItem.ProductId.ToString(), ex);
                }
            }
            //UPDATE QUANTITY ON ORDER ITEM
            OrderItem.Quantity -= returnQuantity;
            if (OrderItem.Quantity == 0) OrderItem.Delete();
            else OrderItem.Save();
            Order order = OrderItem.Order;
            //ADD ORDER NOTE IF PROVIDED
            if (OrderNote.Text.Length > 0)
            {
                OrderNote note = new OrderNote();
                note.OrderId = OrderItem.OrderId;
                note.UserId = Token.Instance.UserId;
                note.Comment = OrderNote.Text;
                note.NoteType = NoteType.Public;
                order.Notes.Add(note);
            }
            order.Save();
            order.RecalculateShipmentStatus();
            order.RecalculatePaymentStatus();
        }
        // REDIRECT TO THE REQUESTING PAGE
        Response.Redirect(Request.Url.ToString(), true);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        this.Visible = false;
    }
}
