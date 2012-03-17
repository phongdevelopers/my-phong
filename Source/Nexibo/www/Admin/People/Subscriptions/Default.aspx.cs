using System;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Users;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text;
using CommerceBuilder.Utility;

public partial class Admin_People_Subscriptions_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        SubscriptionPlanCollection plans = SubscriptionPlanDataSource.LoadForStore("SP.Name");
        SubscriptionGrid.Columns[6].Visible = this.ShowGroup(plans);
        SubscriptionPlan.DataSource = plans;
        SubscriptionPlan.DataBind();
        if (!Page.IsPostBack)
        {
            int planId = AlwaysConvert.ToInt(Request.QueryString["PlanId"]);
            if (planId > 0)
            {
                ListItem item = SubscriptionPlan.Items.FindByValue(planId.ToString());
                if (item != null) SubscriptionPlan.SelectedIndex = SubscriptionPlan.Items.IndexOf(item);
            }
            string expDays = Request.QueryString["Exp"];
            if (!string.IsNullOrEmpty(expDays))
            {
                ExpirationEnd.SelectedDate = LocaleHelper.LocalNow.AddDays(Math.Abs(AlwaysConvert.ToInt(expDays)));
            }
            string activeState = Request.QueryString["ActiveState"];
            if (!string.IsNullOrEmpty(activeState))
            {
                int active = AlwaysConvert.ToInt(activeState);
                if (active == -1) ActiveOnly.SelectedIndex = 2;
                else if (active == 0) ActiveOnly.SelectedIndex = 1;
            }
        }
    }

    private bool ShowGroup(SubscriptionPlanCollection plans)
    {
        foreach (SubscriptionPlan sp in plans)
        {
            if (sp.Group != null) return true;
        }
        return false;
    }

    protected string GetGroupName(object dataItem)
    {
        Subscription s = (Subscription)dataItem;
        Group g = GroupDataSource.Load(s.GroupId);
        if (g != null) return g.Name;
        else return string.Empty;
    }

    protected string GetIsActiveCheckbox(object dataItem)
    {
        Subscription s = (Subscription)dataItem;
        if (s.IsActive) return "<input type=\"checkbox\" disabled=\"disabled\" checked />";
        return "<input type=\"checkbox\" disabled=\"disabled\" />";
    }

    protected void SubscriptionGrid_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in SubscriptionGrid.Rows)
        {
            CheckBox cb = gvr.FindControl("Selected") as CheckBox;
            if (cb != null)
            {
                ScriptManager.RegisterArrayDeclaration(SubscriptionGrid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
            }
        }
        TasksPanel.Visible = (SubscriptionGrid.Rows.Count > 0);
    }
    
    protected List<DataKey> GetSelectedItems()
    {
        List<DataKey> selectedItems = new List<DataKey>();
        foreach (GridViewRow row in SubscriptionGrid.Rows)
        {
            CheckBox selected = (CheckBox)PageHelper.RecursiveFindControl(row, "Selected");
            if (selected != null && selected.Checked)
            {
                selectedItems.Add(SubscriptionGrid.DataKeys[row.DataItemIndex]);
            }
        }
        return selectedItems;
    }

    private void SendEmail(bool all, bool perUser)
    {
        List<int> selectedIds = new List<int>();
        if (all)
        {
            if (perUser)
            {
                int subscriptionPlanId = AlwaysConvert.ToInt(SubscriptionPlan.SelectedValue);
                SubscriptionCollection subscriptions = SubscriptionDataSource.Search(subscriptionPlanId, OrderNumber.Text, string.Empty, FirstName.Text, LastName.Text, Email.Text, ExpirationStart.SelectedDate, ExpirationEnd.SelectedDate, (BitFieldState)Enum.Parse(typeof(BitFieldState), ActiveOnly.SelectedValue));
                foreach (Subscription subscription in subscriptions)
                {
                    int userId = subscription.UserId;
                    if (!selectedIds.Contains(userId)) selectedIds.Add(userId);
                }
            }
            else
            {
                int subscriptionPlanId = AlwaysConvert.ToInt(SubscriptionPlan.SelectedValue);
                SubscriptionCollection subscriptions = SubscriptionDataSource.Search(subscriptionPlanId, OrderNumber.Text, string.Empty, FirstName.Text, LastName.Text, Email.Text, ExpirationStart.SelectedDate, ExpirationEnd.SelectedDate, (BitFieldState)Enum.Parse(typeof(BitFieldState), ActiveOnly.SelectedValue));
                foreach (Subscription subscription in subscriptions)
                {
                    selectedIds.Add(subscription.SubscriptionId);
                }
            }
        }
        else
        {
            if (perUser)
            {
                List<DataKey> selectedItems = GetSelectedItems();
                foreach (DataKey item in selectedItems)
                {
                    if (item.Values.Count > 1)
                    {
                        int userId = AlwaysConvert.ToInt(item.Values[1].ToString());
                        if (!selectedIds.Contains(userId)) selectedIds.Add(userId);
                    }
                }
            }
            else
            {
                List<DataKey> selectedItems = GetSelectedItems();
                foreach (DataKey item in selectedItems)
                {
                    int subscriptionId = AlwaysConvert.ToInt(item.Value.ToString());
                    selectedIds.Add(subscriptionId);
                }
            }
        }

        if (selectedIds.Count < 1) return;
        Session["SendMail_IdList"] = (perUser ? "UserId:" : "SubscriptionId:") + AlwaysConvert.ToList(",", selectedIds);
        Response.Redirect("../Users/SendMail.aspx?ReturnUrl=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Admin/People/Subscriptions/Default.aspx")));
    }

    private void ExportSubscriptions(bool all, bool perUser)
    {
        List<int> selectedSubscriptions = new List<int>();
        if (all)
        {
            int subscriptionPlanId = AlwaysConvert.ToInt(SubscriptionPlan.SelectedValue);
            SubscriptionCollection subscriptions = SubscriptionDataSource.Search(subscriptionPlanId, OrderNumber.Text, string.Empty, FirstName.Text, LastName.Text, Email.Text, ExpirationStart.SelectedDate, ExpirationEnd.SelectedDate, BitFieldState.Any);
            foreach (Subscription subscription in subscriptions)
                selectedSubscriptions.Add(subscription.SubscriptionId);
        }
        else
        {
            List<DataKey> selectedItems = GetSelectedItems();
            foreach (DataKey item in selectedItems)
                selectedSubscriptions.Add((int)item.Value);
        }
        if (selectedSubscriptions.Count <= 0)
            return;
        Session["Subscriptions_To_Export"] = selectedSubscriptions;
        StringBuilder exportJS = new StringBuilder();
        exportJS.AppendLine("var iframe = document.createElement(\"iframe\");");
        exportJS.AppendLine("iframe.src = \"ExportSubscriptions.ashx?PerUser=" + perUser + "\";");
        exportJS.AppendLine("iframe.style.display = \"none\";");
        exportJS.AppendLine("document.body.appendChild(iframe);");
        ScriptManager.RegisterClientScriptBlock(SubscriptionsAjax, typeof(UpdatePanel), "downloadexportfile", exportJS.ToString(), true);
    }

    protected void EmailOkButton_Click(object sender, EventArgs e)
    {
        bool sendToAll = EmailSelected.SelectedIndex == 1;
        bool sendPerUser = EmailDistinct.SelectedIndex == 1;
        SendEmail(sendToAll, sendPerUser);
    }

    protected void ExportOkButton_Click(object sender, EventArgs e)
    {
        bool sendToAll = ExportSelected.SelectedIndex == 1;
        bool sendPerUser = ExportDistinct.SelectedIndex == 1;
        ExportSubscriptions(sendToAll, sendPerUser);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        List<DataKey> selectedItems = GetSelectedItems();
        foreach (DataKey item in selectedItems)
        {
            int subscriptionId = AlwaysConvert.ToInt(item.Value);
            Subscription subscription = SubscriptionDataSource.Load(subscriptionId);
            if (subscription != null) subscription.Delete();
        }
        SubscriptionGrid.DataBind();
    }
}