using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using CommerceBuilder.Catalog;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Marketing;
using CommerceBuilder.Messaging;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

public partial class Admin_Orders_OrderHistory : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId = 0;
    private Order _Order;

    protected void Page_Load(object sender, EventArgs e)
    {
        _Order = OrderHelper.GetOrderFromContext();
        if (_Order != null)
        {
            _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
            if (!Page.IsPostBack)
            {
                BindOrderNotes();
                AddIsPrivate.Checked = true;
            }
        }
    }

    protected void BindOrderNotes()
    {
        OrderNotesGrid.DataSource = _Order.Notes;
        OrderNotesGrid.DataBind();
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(AddComment.Text))
        {
            _Order.Notes.Add(new OrderNote(_OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, AddComment.Text, AddIsPrivate.Checked ? NoteType.Private : NoteType.Public));
            _Order.Notes.Save();
            
            BindOrderNotes();
            AddComment.Text = string.Empty;
        }
        AddIsPrivate.Checked = true;
    }

    protected void OrderNotesGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        OrderNotesGrid.EditIndex = e.NewEditIndex;
        BindOrderNotes();
        AddPanel.Visible = false;
    }

    protected void OrderNotesGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        OrderNotesGrid.EditIndex = -1;
        BindOrderNotes();
        AddPanel.Visible = true;
    }
    
    protected void OrderNotesGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        OrderNote note = _Order.Notes[e.RowIndex];
        GridViewRow row = OrderNotesGrid.Rows[e.RowIndex];
        TextBox editComment = PageHelper.RecursiveFindControl(row, "EditComment") as TextBox;
        CheckBox editIsPrivate = PageHelper.RecursiveFindControl(row, "EditIsPrivate") as CheckBox;
        note.Comment = editComment.Text;
        note.NoteType = editIsPrivate.Checked ? NoteType.Private : NoteType.Public;
        note.Save();
        OrderNotesGrid.EditIndex = -1;
        BindOrderNotes();
        AddPanel.Visible = true;
    }

    protected void OrderNotesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        _Order.Notes.DeleteAt(e.RowIndex);
        BindOrderNotes();
    }

}
