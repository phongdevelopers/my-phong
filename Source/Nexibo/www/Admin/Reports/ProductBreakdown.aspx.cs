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
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

public partial class Admin_Reports_ProductBreakdown : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            UpdateVendorList(VendorList);
        }

    }
    private void UpdateVendorList(DropDownList VendorListCtrl)
    {
        VendorCollection vendors = VendorDataSource.LoadForStore();
        VendorListCtrl.Items.Add(new ListItem("ANY", "0"));
        foreach (Vendor vendor in vendors)
        {
            VendorListCtrl.Items.Add(new ListItem(vendor.Name, vendor.VendorId.ToString()));
        }
        VendorListCtrl.SelectedIndex = 0;
        VendorListCtrl.DataBind();
    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        BindGrid();
    }

    protected void ProductBreakdownGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        ProductBreakdownGrid.PageIndex = e.NewPageIndex;
        BindGrid();
    }

    protected void BindGrid()
    {
        DateTime fromDate = PickerAndCalendar1.SelectedStartDate;
        DateTime toDate = PickerAndCalendar2.SelectedEndDate;
        List<ProductBreakdownSummary> breakdownReport = ReportDataSource.GetProductBreakdownSummary(fromDate, toDate, AlwaysConvert.ToInt(VendorList.SelectedValue), SortByList.SelectedValue);
        int totalQuantity = 0;
        LSDecimal totalAmount = 0;
        foreach (ProductBreakdownSummary pbs in breakdownReport)
        {
            totalQuantity += pbs.Quantity;
            totalAmount += pbs.Amount;
        }
        if (breakdownReport.Count > 0)
        {
            TotalQuantity.Visible = true;
            LblTotalQuantity.Visible = true;
            TotalQuantity.Text = totalQuantity.ToString();
            TotalAmount.Visible = true;
            LblTotalAmount.Visible = true;
            TotalAmount.Text = string.Format("{0:lc}", totalAmount);
        }
        else
        {
            TotalQuantity.Visible = false;
            LblTotalQuantity.Visible = false;
            TotalAmount.Visible = false;
            LblTotalAmount.Visible = false;
        }
        ProductBreakdownGrid.DataSource = breakdownReport;
        ProductBreakdownGrid.DataBind();
    }
    
}
