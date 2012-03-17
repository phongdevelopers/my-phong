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

public partial class Admin_Orders_Payments_AddPayment : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId = 0;
    protected int OrderId
    {
        get
        {
            if (_OrderId.Equals(0))
            {
                _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
            }
            return _OrderId;
        }
    }

    private Order _Order;
    protected Order Order
    {
        get
        {
            if (_Order == null)
            {
                _Order = OrderDataSource.Load(this.OrderId);
            }
            return _Order;
        }
    }

    protected void RecordPayment_CheckedChanged(object sender, EventArgs e)
    {
        ProcessPaymentPanel.Visible = ProcessPayment.Checked;
        RecordPaymentPanel.Visible = !ProcessPayment.Checked;
    }

    protected void ProcessPayment_CheckedChanged(object sender, EventArgs e)
    {
        ProcessPaymentPanel.Visible = ProcessPayment.Checked;
        RecordPaymentPanel.Visible = !ProcessPayment.Checked;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LSDecimal actualBalance = Order.GetBalance(false);
            LSDecimal pendingBalance = Order.GetBalance(true);
            Balance.Text = string.Format("{0:lc}", pendingBalance);
            PendingMessage.Visible = (actualBalance != pendingBalance);
        }
    }

}
