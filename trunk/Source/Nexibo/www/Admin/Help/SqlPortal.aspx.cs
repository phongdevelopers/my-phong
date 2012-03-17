using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;

public partial class Admin_Help_SqlPortal : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    // TO DISABLE THE SCRIPT, SET THE PortalEnabled VARIABLE TO false
    // e.g. private static bool PortalEnabled = false;
    // TO ENABLE THE SCRIPT, SET THE PortalEnabled VARIABLE TO true
    // e.g. private static bool PortalEnabled = true;
    private static bool PortalEnabled = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PortalEnabled)
        {
            SqlQuery.Visible = false;
            ExecuteButton.Visible = false;
            SqlQueryLabel.Text = "The sql portal must be enabled by making a modification to the script file.";
        }
    }

    protected void ExecuteButton_Click(object sender, EventArgs e)
    {
        if (PortalEnabled)
        {
            DateTime beginTime = DateTime.Now;
            StringBuilder dataTable = new StringBuilder();
            try
            {
                DataSet ds = Token.Instance.Database.ExecuteDataSet(CommandType.Text, SqlQuery.Text);
                int tableCount = ds.Tables.Count;
                foreach (DataTable table in ds.Tables)
                {
                    int columnCount = table.Columns.Count;
                    dataTable.Append("<div style=\"overflow:auto\"><table border=\"1\"><tr>");
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn col = table.Columns[i];
                        dataTable.Append("<th>" + col.ColumnName + "</th>");
                    }
                    dataTable.Append("</tr>");
                    foreach (DataRow row in table.Rows)
                    {
                        dataTable.Append("<tr>");
                        for (int i = 0; i < columnCount; i++)
                        {
                            dataTable.Append("<td>" + row[i].ToString() + "</td>");
                        }
                        dataTable.Append("</tr>");
                    }
                    dataTable.Append("</table></div><br />");
                }
            }
            catch { }
            DateTime endTime = DateTime.Now;
            phQueryResult.Controls.Add(new LiteralControl(dataTable.ToString()));
            TimeSpan ts = endTime - beginTime;
            phQueryResult.Controls.Add(new LiteralControl("Query executed in " + Math.Round(ts.TotalMilliseconds, 0) + "ms"));
        }
    }
}