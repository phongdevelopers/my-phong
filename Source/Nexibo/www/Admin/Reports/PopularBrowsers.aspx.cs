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
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Reporting;
using WebChart;
using System.Collections.Generic;

public partial class Admin_Reports_PopularBrowsers : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            drawChart();
        }
    }
    
    private void drawChart()
    {
        //GET THE TOP 4 BROWSERS BY VIEW
        SortableCollection<KeyValuePair<string, int>> topBrowsers = PageViewDataSource.GetViewsByBrowser(4, 0, "ViewCount DESC, Browser ASC");
        if (topBrowsers.Count > 0)
        {
            //FIND OUT HOW MANY VIEWS TOTAL
            int totalPageViews = PageViewDataSource.CountForStore();
            int pageViewsCharted = 0;
            //List<double> viewCounts = new double[topBrowsers.Count];
            //string[] browserNames = new string[topBrowsers.Count];
            List<int> viewCounts = new List<int>();
            List<string> browserNames = new List<string>();
            foreach (KeyValuePair<string, int> browserView in topBrowsers)
            {
                browserNames.Add(browserView.Key);
                viewCounts.Add(browserView.Value);
                pageViewsCharted += browserView.Value;
            }
            if (pageViewsCharted < totalPageViews)
            {
                //NEED TO ADD AN "OTHER" PIE SLICE
                browserNames.Add("Other");
                viewCounts.Add(totalPageViews - pageViewsCharted);
            }
            //BUILD BAR CHART
            WebChart.PieChart chart = (PieChart)BrowserChart.Charts[0];
            chart.Data.Clear();
            for (int i = 0; i < topBrowsers.Count; i++)
            {
                chart.Data.Add(new ChartPoint(browserNames[i], viewCounts[i]));
            }
            //int internalHeight = (int)ViewsChart.Height.Value - (int)ViewsChart.TopPadding - (int)ViewsChart.TopChartPadding - (int)ViewsChart.BottomChartPadding - (int)(ViewsChart.ChartPadding * 2);
            //int columnHeight = (internalHeight / categoryViews.Count);
            //columnHeight -= 3;
            //if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
            //BIND THE CHART
            BrowserChart.RedrawChart();
        }
        else
        {
            //NO CATEGORIES HAVE BEEN VIEWED YET OR PAGE TRACKING IS NOT AVAIALBEL
            this.Controls.Clear();
            Panel noViewsPanel = new Panel();
            noViewsPanel.CssClass = "emptyData";
            Label noViewsMessage = new Label();
            noViewsMessage.Text = "No categories have been viewed or page tracking is disabled.";
            noViewsPanel.Controls.Add(noViewsMessage);
            this.Controls.Add(noViewsPanel);
        }
    }

}
