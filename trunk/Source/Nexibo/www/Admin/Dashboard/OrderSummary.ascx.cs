using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public partial class Admin_Dashboard_OrderSummary : System.Web.UI.UserControl
{
    private List<OrderStatusDetail> GetOrderSummaryDs()
    {
        List<OrderStatusDetail> summary = new List<OrderStatusDetail>();
        DateTime localNow = LocaleHelper.LocalNow;
        DateTime todayStart = LocaleHelper.FromLocalTime(new DateTime(localNow.Year, localNow.Month, localNow.Day, 0, 0, 0, DateTimeKind.Local));
        DateTime last30Start = todayStart.AddDays(-30);
        DateTime last90Start = todayStart.AddDays(-90);
        OrderStatusCollection statusCollection = Token.Instance.Store.OrderStatuses;
        foreach (OrderStatus statusItem in statusCollection)
        {
            OrderStatusDetail summaryItem = new OrderStatusDetail();
            summaryItem.StatusId = statusItem.OrderStatusId;
            summaryItem.Status = statusItem.Name;
            CommerceBuilder.Reporting.OrderSearchCriteria searchCriteria = new CommerceBuilder.Reporting.OrderSearchCriteria();
            searchCriteria.OrderStatus.Add(statusItem.OrderStatusId);
            searchCriteria.OrderDateStart = todayStart;
            summaryItem.DayCount = OrderDataSource.SearchCount(searchCriteria);
            searchCriteria.OrderDateStart = last30Start;
            summaryItem.Last30Count = OrderDataSource.SearchCount(searchCriteria);
            searchCriteria.OrderDateStart = last90Start;
            summaryItem.Last90Count = OrderDataSource.SearchCount(searchCriteria);
            summary.Add(summaryItem);
        }
        return summary;
    }

    private void BindDataSources()
    {
        OrderGrid.DataSource = GetOrderSummaryDs();
        OrderGrid.DataBind();
        RecentOrdersList.DataSource = OrderDataSource.LoadForStore(5, 0, "OrderDate DESC");
        RecentOrdersList.DataBind();
        CurrentTime.Text = string.Format("as of {0:T}", LocaleHelper.LocalNow);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (Token.Instance.User.IsInRole(Role.OrderAdminRoles))
        {
            BindDataSources();
        }
        else this.Controls.Clear();
    }

    protected void OrderGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        int orderStatusId = AlwaysConvert.ToInt(e.CommandArgument);
        int dateFilter = 0;
        if (e.CommandName == "Last30") dateFilter = 5;
        else if (e.CommandName == "Last90") dateFilter = 7;
        Response.Redirect("~/Admin/Orders/Default.aspx?OrderStatusId=" + orderStatusId.ToString() + "&DateFilter=" + dateFilter.ToString());
    }

    protected string GetSearchUrl(object dataItem, int dateFilter)
    {
        int statusId = ((OrderStatusDetail)dataItem).StatusId;
        if (dateFilter == 0) return string.Format("~/Admin/Orders/Default.aspx?OrderStatusId={0}", statusId);
        return string.Format("~/Admin/Orders/Default.aspx?OrderStatusId={0}&DateFilter={1}", statusId, dateFilter);
    }

    protected void RefreshButton_Click(object sender, System.EventArgs e)
    {
        BindDataSources();
    }

    protected void ViewOrderButton_Click(object sender, EventArgs e)
    {
        int tempOrderNumber = AlwaysConvert.ToInt(OrderNumber.Text);
        Order order = OrderDataSource.Load(OrderDataSource.LookupOrderId(tempOrderNumber));
        if (order != null)
        {
            Response.Redirect("~/Admin/Orders/ViewOrder.aspx?OrderNumber=" + order.OrderNumber.ToString() + "&OrderId=" + order.OrderId.ToString());
        }
        else
        {
            CustomValidator invalidOrderId = new CustomValidator();
            invalidOrderId.ControlToValidate = "OrderNumber";
            invalidOrderId.ErrorMessage = "*";
            invalidOrderId.Text = "Order number is not valid";
            invalidOrderId.IsValid = false;
            ViewOrderPanel.Controls.Add(invalidOrderId);
        }
    }

    public class OrderStatusDetail
    {
        private int _StatusId;
        private string _Status;
        private int _DayCount;
        private int _Last30Count;
        private int _Last90Count;
        public int StatusId
        {
            get { return _StatusId; }
            set { _StatusId = value; }
        }
        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        public int DayCount
        {
            get { return _DayCount; }
            set { _DayCount = value; }
        }
        public int Last30Count
        {
            get { return _Last30Count; }
            set { _Last30Count = value; }
        }
        public int Last90Count
        {
            get { return _Last90Count; }
            set { _Last90Count = value; }
        }
    }
}
