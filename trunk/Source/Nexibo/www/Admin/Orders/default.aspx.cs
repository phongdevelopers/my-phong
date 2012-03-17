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
using CommerceBuilder.Payments;
using CommerceBuilder.Reporting;
using CommerceBuilder.Search;
using CommerceBuilder.Utility;
using System.Text;

public partial class Admin_Orders_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        //GET ORDER STATUSES FOR STORE
        OrderStatusCollection statuses = OrderStatusDataSource.LoadForStore();
        OrderStatusCollection validStatuses = new OrderStatusCollection();
        OrderStatusCollection invalidStatuses = new OrderStatusCollection();
        //BUILD STATUS FILTER
        String statusNamePrefix = "- ";
        foreach (OrderStatus status in statuses)
        {
            if (status.IsValid)
            {
                validStatuses.Add(status);
            }
            else
            {
                invalidStatuses.Add(status);
            }            
        }
        // ADD VALID STATUSES
        StatusFilter.Items.Add(new ListItem("All Valid", "-2"));
        foreach (OrderStatus status in validStatuses)
        {
            StatusFilter.Items.Add(new ListItem(statusNamePrefix + status.Name, status.OrderStatusId.ToString()));
        }
        // ADD INVALID STATUSES
        StatusFilter.Items.Add(new ListItem("All Invalid", "-3"));
        foreach (OrderStatus status in invalidStatuses)
        {
            StatusFilter.Items.Add(new ListItem(statusNamePrefix + status.Name, status.OrderStatusId.ToString()));
        }
        // SET THE DEFAULT AS ALL-VALID
        StatusFilter.SelectedIndex = 1;

        //APPEND ORDER STATUS ACTIONS TO BATCH LIST
        string updateText = "Update status to {0}";
        foreach (OrderStatus status in statuses)
        {
            
            BatchAction.Items.Add(new ListItem(string.Format(updateText, status.Name), "OS_" + status.OrderStatusId));
        }

        InitDateQuickPick();
    }

    private void InitDateQuickPick()
    {
        StringBuilder js = new StringBuilder();
        js.AppendLine("function dateQP(selectDom) {");
        js.AppendLine("var startPicker = " + OrderStartDate.GetPickerClientId());
        js.AppendLine("var endPicker = " + OrderEndDate.GetPickerClientId());
        js.AppendLine("switch(selectDom.selectedIndex){");
        string setStart= "startPicker.setSelectedDate(new Date('{0}'));";
        string setEnd= "endPicker.setSelectedDate(new Date('{0}'));";
        string clearStart= "startPicker.clearSelectedDate();";
        string clearEnd= "endPicker.clearSelectedDate();";
        int startIndex = 1;

        DateQuickPick.Items.Add(new ListItem("Today"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 7 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-7).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 14 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-14).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 30 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-30).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 60 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-60).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 90 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-90).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last 120 Days"));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, LocaleHelper.LocalNow.AddDays(-120).ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("This Week"));
        DateTime startDate = LocaleHelper.LocalNow.AddDays(-1 * (int)LocaleHelper.LocalNow.DayOfWeek);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last Week"));
        startDate = LocaleHelper.LocalNow.AddDays((-1 * (int)LocaleHelper.LocalNow.DayOfWeek) - 7);
        DateTime endDate = startDate.AddDays(6);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(string.Format(setEnd, endDate.ToString("d")));
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("This Month"));
        startDate = new DateTime(LocaleHelper.LocalNow.Year, LocaleHelper.LocalNow.Month, 1);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("Last Month"));
        DateTime lastMonth = LocaleHelper.LocalNow.AddMonths(-1);
        startDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
        endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(lastMonth.Year,lastMonth.Month));
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(string.Format(setEnd, endDate.ToString("d")));
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("This Year"));
        startDate = new DateTime(LocaleHelper.LocalNow.Year, 1, 1);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(string.Format(setStart, startDate.ToString("d")));
        js.Append(clearEnd);
        js.AppendLine("break;");

        DateQuickPick.Items.Add(new ListItem("All Dates"));
        startDate = new DateTime(LocaleHelper.LocalNow.Year, 1, 1);
        js.Append("case " + (startIndex++).ToString() + ": ");
        js.Append(clearStart);
        js.Append(clearEnd);
        js.AppendLine("break;");

        // close switch
        js.Append("}");

        // reset quick picker
        js.AppendLine("selectDom.selectedIndex = 0;");
        js.Append("}");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "dateQP", js.ToString(), true);
        DateQuickPick.Attributes.Add("onChange", "dateQP(this)");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // GET DATE FROM QUERYSTRING
            int dateFilter = AlwaysConvert.ToInt(Request.QueryString["DateFilter"]);
            if (dateFilter == 1) OrderStartDate.SelectedDate = LocaleHelper.LocalNow;
            else if (dateFilter == 5) OrderStartDate.SelectedDate = LocaleHelper.LocalNow.AddDays(-30);
            else if (dateFilter == 7) OrderStartDate.SelectedDate = LocaleHelper.LocalNow.AddDays(-90);
            else dateFilter = 0;

            // GET ORDER STATUS FROM QUERY STRING
            int orderStatusId = AlwaysConvert.ToInt(Request.QueryString["OrderStatusId"]);
            if (orderStatusId > 0)
            {
                ListItem item = StatusFilter.Items.FindByValue(orderStatusId.ToString());
                if (item != null)
                {
                    StatusFilter.SelectedIndex = StatusFilter.Items.IndexOf(item);
                }
                else orderStatusId = 0;
            }

            //DO NOT LOAD LAST SEARCH IF QUERY STRING PARAMETERS WERE NOT SET
            if (dateFilter == 0 && orderStatusId == 0) LoadLastSearch();
        }
    }

    private void LoadLastSearch()
    {
        // LOAD CRITERIA FROM SESSION?
        OrderFilter criteria = Session["OrderFilter"] as OrderFilter;
        int selectStatusId = AlwaysConvert.ToInt(Session["OrderFilterSelectedStatus"]);
        if (criteria != null)
        {
            // SET THE ORDER STATUS FILTER            
            if (selectStatusId == 0) selectStatusId = -2;
            ListItem statusItem = StatusFilter.Items.FindByValue(selectStatusId.ToString());
            if (statusItem != null) StatusFilter.SelectedIndex = StatusFilter.Items.IndexOf(statusItem);
            //SET THE PAYMENT STATUS FILTER
            statusItem = PaymentStatusFilter.Items.FindByValue(((int)criteria.PaymentStatus).ToString());
            if (statusItem != null) statusItem.Selected = true;
            //SET THE SHIPMENT STATUS FILTER
            statusItem = ShipmentStatusFilter.Items.FindByValue(((int)criteria.ShipmentStatus).ToString());
            if (statusItem != null) statusItem.Selected = true;
            // SET THE ORDERNUMBER
            OrderNumberFilter.Text = criteria.OrderNumberRange;
            // SET THE KEYWORD FILTER
            KeywordSearchText.Text = criteria.Keyword;
            ListItem searchFieldItem = KeywordSearchField.Items.FindByValue(criteria.KeywordField.ToString());
            if (searchFieldItem != null) KeywordSearchField.SelectedIndex = KeywordSearchField.Items.IndexOf(searchFieldItem);
            //IF ORDER START DATE IS SPECIFIED, DETERMINE DEFAULT FOR DATE RANGE
            if (criteria.OrderDateStart > DateTime.MinValue && criteria.OrderDateStart < DateTime.MaxValue)
                OrderStartDate.SelectedDate = criteria.OrderDateStart;
            if (criteria.OrderDateEnd > DateTime.MinValue && criteria.OrderDateEnd < DateTime.MaxValue)
                OrderEndDate.SelectedDate = criteria.OrderDateEnd;
            // DISPLAY A RESET BUTTON
            ResetSearchButton.Visible = true;
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        OrderGrid.PageIndex = 0;
        int pageSize = AlwaysConvert.ToInt(PageSize.SelectedValue);
        if (pageSize == 0) OrderGrid.AllowPaging = false;
        else
        {
            OrderGrid.AllowPaging = true;
            OrderGrid.PageSize = pageSize;
        }

        OrderGrid.DataBind();
        SearchResultAjax.Update();

        // DISPLAY THE RESET BUTTON
        ResetSearchButton.Visible = true;
    }

    protected void ResetButton_Click(object sender, EventArgs e)
    {
        // RESET THE SESSION
        Session.Remove("OrderFilter");
        Session.Remove("OrderFilterSelectedStatus");

        // RESET THE FORM
        // SET THE ORDER STATUS FILTER            
        ListItem statusItem = StatusFilter.Items.FindByValue("-2");
        if (statusItem != null) StatusFilter.SelectedIndex = StatusFilter.Items.IndexOf(statusItem);

        //SET THE PAYMENT STATUS FILTER
        statusItem = PaymentStatusFilter.Items.FindByValue("0");
        if (statusItem != null) PaymentStatusFilter.SelectedIndex = PaymentStatusFilter.Items.IndexOf(statusItem);

        //SET THE SHIPMENT STATUS FILTER
        statusItem = ShipmentStatusFilter.Items.FindByValue("0");
        if (statusItem != null) ShipmentStatusFilter.SelectedIndex = ShipmentStatusFilter.Items.IndexOf(statusItem);

        // SET THE ORDERNUMBER
        OrderNumberFilter.Text = string.Empty;

        // SET THE KEYWORD FILTER
        statusItem = KeywordSearchField.Items.FindByValue("OrderNotes");
        KeywordSearchField.SelectedIndex = KeywordSearchField.Items.IndexOf(statusItem);
        KeywordSearchText.Text = String.Empty;

        //RESET DATES
        OrderStartDate.SelectedDate = DateTime.MinValue;
        OrderEndDate.SelectedDate = DateTime.MinValue;

        // EXECUTE THE SEARCH AND HIDE THE RESET BUTTON
        SearchButton_Click(sender, e);
        ResetSearchButton.Visible = false;
    }

    protected void CreateOrderButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Create/CreateOrder1.aspx");
    }

    protected string GetOrderStatus(Object orderStatusId)
    {
        OrderStatus status = OrderStatusDataSource.Load((int)orderStatusId);
        if (status != null) return status.Name;
        return string.Empty;
    }

    protected string GetPaymentStatus(object dataItem)
    {
        Order order = (Order)dataItem;
        if (order.PaymentStatus == OrderPaymentStatus.Paid) return "Paid";
        if (order.Payments.Count > 0)
        {
            order.Payments.Sort("PaymentDate");
            Payment lastPayment = order.Payments[order.Payments.Count - 1];
            return StringHelper.SpaceName(lastPayment.PaymentStatus.ToString());
        }
        return order.PaymentStatus.ToString();
    }

    protected void OrderGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Capture") || e.CommandName.Equals("Ship"))
        {
            int orderId = (int)OrderGrid.DataKeys[AlwaysConvert.ToInt(e.CommandArgument)].Value;
            Order order = OrderDataSource.Load(orderId);
            if (order != null)
            {
                if (e.CommandName.Equals("Capture"))
                {
                    if ((order.Payments.Count == 1) && (order.Payments[0].PaymentStatus == PaymentStatus.Authorized))
                    {
                        Response.Redirect("Payments/CapturePayment.aspx?PaymentId=" + order.Payments[0].PaymentId.ToString());
                    }
                }
                else
                {
                    foreach (OrderShipment shipment in order.Shipments)
                    {
                        if (shipment.ShipDate == System.DateTime.MinValue)
                        {
                            Response.Redirect("Shipments/ShipOrder.aspx?OrderShipmentId=" + shipment.OrderShipmentId.ToString());
                        }
                    }
                }
            }
            OrderGrid.DataBind();
        }
    }

    private List<int> GetSelectedOrderIds()
    {
        int indexPeg = OrderGrid.PageSize * OrderGrid.PageIndex;

        List<int> selectedOrders = new List<int>();
        foreach (GridViewRow row in OrderGrid.Rows)
        {
            CheckBox selected = (CheckBox)PageHelper.RecursiveFindControl(row, "Selected");
            if ((selected != null) && selected.Checked)
            {
                selectedOrders.Add((int)OrderGrid.DataKeys[row.DataItemIndex - indexPeg].Values[0]);
            }
        }
        return selectedOrders;
    }


    protected void OrderGrid_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if ((e.Row != null) && (e.Row.RowType == DataControlRowType.DataRow) && (e.Row.DataItem != null))
        {
            //FORCE RELOAD OF DATA ITEM - NEED MORE EFFICIENT SOLUTION TO REFRESH GRID FROM BATCH UPDATES
            //THIS PREVENTS DATA FROM DATABASE AND GRID BECOMING UNSYNCRHONIZED
            Order order = OrderDataSource.Load(((Order)e.Row.DataItem).OrderId, false);
            e.Row.DataItem = order;
        }
    }

    protected void OrderGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if ((e.Row != null) && (e.Row.RowType == DataControlRowType.DataRow) && (e.Row.DataItem != null))
        {
            Order order = (Order)e.Row.DataItem;
            PlaceHolder phPaymentStatus = PageHelper.RecursiveFindControl(e.Row, "phPaymentStatus") as PlaceHolder;
            if (phPaymentStatus != null)
            {
                Image paymentStatus = new Image();
                paymentStatus.SkinID = GetPaymentStatusSkinID(order);
                phPaymentStatus.Controls.Add(paymentStatus);
            }
            PlaceHolder phShipmentStatus = PageHelper.RecursiveFindControl(e.Row, "phShipmentStatus") as PlaceHolder;
            if (phShipmentStatus != null)
            {
                Image shipmentStatus = new Image();
                shipmentStatus.SkinID = GetShipmentStatusSkinID(order);
                phShipmentStatus.Controls.Add(shipmentStatus);
            }
        }
    }

    protected string GetPaymentStatusSkinID(Order order)
    {
        //if (order.OrderStatus.Name.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase)) return "CodeGreen";
		if (!order.OrderStatus.IsValid) return "CodeRed";
        
        switch (order.PaymentStatus)
        {
            case OrderPaymentStatus.Unspecified:
            case OrderPaymentStatus.Unpaid:
                return "CodeYellow";
            case OrderPaymentStatus.Problem:
                return "CodeRed";
            default:
                return "CodeGreen";
        }
    }

    protected string GetShipmentStatusSkinID(Order order)
    {
        //if (order.OrderStatus.Name.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase)) return "CodeGreen";
        if (!order.OrderStatus.IsValid) return "CodeRed";
        
        switch (order.ShipmentStatus)
        {
            case OrderShipmentStatus.Unspecified:
            case OrderShipmentStatus.Unshipped:
                return "CodeYellow";
            default:
                return "CodeGreen";
        }
    }

    protected void OrderGrid_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in OrderGrid.Rows)
        {
            CheckBox cb = (CheckBox)gvr.FindControl("Selected");
            ScriptManager.RegisterArrayDeclaration(OrderGrid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
        }
        selectedOrdersPanel.Visible = (OrderGrid.Rows.Count > 0);
    }

    protected void BatchButton_Click(object sender, EventArgs e)
    {
        List<string> messages = new List<string>();
        List<int> orderIds = GetSelectedOrderIds();
        if (orderIds.Count > 0)
        {
            if (BatchAction.SelectedValue.StartsWith("OS_"))
            {
                //UPDATE ORDER STATUS REQUESTED
                int orderStatusId = AlwaysConvert.ToInt(BatchAction.SelectedValue.Substring(3));
                //VALIDATE STATUS
                OrderStatus status = OrderStatusDataSource.Load(orderStatusId);
                if (status != null)
                {
                    foreach (int orderId in orderIds)
                    {
                        Order order = OrderDataSource.Load(orderId);
                        if (order != null)
                        {
                            order.UpdateOrderStatus(status);
                        }
                    }
                }
            }
            else
            {
                switch (BatchAction.SelectedValue)
                {
                    case "INVOICE":
                        Token.Instance.Session.SelectedOrderIds = orderIds;
                        Response.Redirect("Print/Invoices.aspx");
                        break;
                    case "PACKSLIP":
                        Token.Instance.Session.SelectedOrderIds = orderIds;
                        Response.Redirect("Print/PackSlips.aspx");
                        break;
                    case "PULLSHEET":
                        Token.Instance.Session.SelectedOrderIds = orderIds;
                        Response.Redirect("Print/PullSheet.aspx");
                        break;
                    case "CANCEL":
                        Token.Instance.Session.SelectedOrderIds = orderIds;
                        Response.Redirect("Batch/Cancel.aspx");
                        break;
                    case "SHIPOPT":
                        Token.Instance.Session.SelectedOrderIds = orderIds;
                        Response.Redirect("Batch/Ship.aspx");
                        break;
                    case "SHIP":
                        int shipCount = 0;
                        foreach (int orderId in orderIds)
                        {
                            Order order = OrderDataSource.Load(orderId);
                            if (order != null && order.Shipments != null)
                            {
                                bool shipped = false;
                                int shipmentCount = order.Shipments.Count;
                                for (int i = 0; i < shipmentCount; i++)
                                {
                                    OrderShipment shipment = order.Shipments[i];
                                    if (shipment != null && !shipment.IsShipped)
                                    {
                                        shipment.Ship();
                                        shipped = true;
                                    }
                                }
                                if (shipped)
                                {
                                    messages.Add("Order #" + order.OrderNumber + " shipped.");
                                    shipCount++;
                                }
                                else messages.Add("Order #" + order.OrderNumber + " did not have any unshipped items.");
                            }
                        }
                        messages.Add(shipCount + " orders shipped.");
                        break;
                    case "PAY":
                        int payCount = 0;
                        foreach (int orderId in orderIds)
                        {
                            Order order = OrderDataSource.Load(orderId);
                            if (order != null)
                            {
                                bool paid = false;
                                int paymentCount = order.Payments.Count;
                                for (int i = 0; i < paymentCount; i++)
                                {
                                    Payment payment = order.Payments[i];
                                    if (payment.PaymentStatus == PaymentStatus.Authorized)
                                    {
                                        payment.Capture(payment.Amount, true);
                                        paid = true;
                                    }
                                    else if (payment.PaymentStatus == PaymentStatus.Unprocessed)
                                    {
                                        payment.Authorize();
                                        paid = true;
                                    }
                                }
                                if (paid)
                                {
                                    payCount++;
                                    messages.Add("Order " + order.OrderNumber.ToString() + " processed.");
                                }
                                else messages.Add("Order " + order.OrderNumber.ToString() + " does not have any payments to be processed.");
                            }
                        }
                        messages.Add(payCount + " orders processed.");
                        break;
                }
            }
        }
        if (messages.Count > 0)
        {
            trBatchMessage.Visible = true;
            BatchMessage.Text = string.Join("<br />", messages.ToArray());
        }
        BatchAction.SelectedIndex = -1;
        OrderGrid.DataBind();
    }

    protected OrderFilter GetOrderFilter()
    {
        // CREATE CRITERIA INSTANCE
        OrderFilter criteria = new OrderFilter();
        if (OrderStartDate.SelectedStartDate > DateTime.MinValue)
            criteria.OrderDateStart = OrderStartDate.SelectedStartDate;
        if (OrderEndDate.SelectedEndDate > DateTime.MinValue && OrderEndDate.SelectedEndDate < DateTime.MaxValue)
            criteria.OrderDateEnd = OrderEndDate.SelectedEndDate;
        criteria.OrderNumberRange = OrderNumberFilter.Text;
        criteria.PaymentStatus = (OrderPaymentStatus)AlwaysConvert.ToByte(PaymentStatusFilter.SelectedValue);
        criteria.ShipmentStatus = (OrderShipmentStatus)AlwaysConvert.ToByte(ShipmentStatusFilter.SelectedValue);
        // ADD IN ORDER STATUS FILTER
        int statusId = 0;
        if (StatusFilter.SelectedValue == "-2")
        {
            OrderStatusCollection statuses = OrderStatusDataSource.LoadForStore();            
            foreach (OrderStatus status in statuses)
            {
                if (status.IsValid)
                {
                    criteria.OrderStatus.Add(status.OrderStatusId);
                }
            }
        }
        else if (StatusFilter.SelectedValue == "-3")
        {
            OrderStatusCollection statuses = OrderStatusDataSource.LoadForStore();
            foreach (OrderStatus status in statuses)
            {
                if (!status.IsValid)
                {
                    criteria.OrderStatus.Add(status.OrderStatusId);
                }
            }
        }
        else
        {
            statusId =  AlwaysConvert.ToInt(StatusFilter.SelectedValue);
            if (statusId > 0) criteria.OrderStatus.Add(statusId);
        }
       
        // ADD IN KEYWORD FILTER
        criteria.Keyword = KeywordSearchText.Text;
        criteria.KeywordField = (OrderFilter.KeywordFieldType)Enum.Parse(typeof(OrderFilter.KeywordFieldType), KeywordSearchField.Text, true);

        // RETURN THE CRITERIA OBJECT
        Session["OrderFilter"] = criteria;
        Session["OrderFilterSelectedStatus"] = StatusFilter.SelectedValue;
        return criteria;
    }

    protected void OrderDs_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
    {
        // ADD IN THE SEARCH CRITERIA
        e.InputParameters["filter"] = GetOrderFilter();
    }

    protected void JumpToOrderButton_Click(Object sender, EventArgs e)
    {
        int tempOrderNumber = AlwaysConvert.ToInt(JumpToOrderNumber.Text);
        Order order = OrderDataSource.Load(OrderDataSource.LookupOrderId(tempOrderNumber));
        if (order != null)
        {
            Response.Redirect("~/Admin/Orders/ViewOrder.aspx?OrderNumber=" + order.OrderNumber.ToString() + "&OrderId=" + order.OrderId.ToString());
        }
        else
        {
            CustomValidator invalidOrderId = new CustomValidator();
            invalidOrderId.ControlToValidate = "JumpToOrderNumber";
            invalidOrderId.Text = "not found, try search!";
            invalidOrderId.IsValid = false;
            invalidOrderId.ValidationGroup = "JumpToOrder";
            phJumpToOrder.Controls.Add(invalidOrderId);
        }
    }
}
