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
using CommerceBuilder.Payments;
using CommerceBuilder.Utility;

public partial class Admin_Payment_ucMethodGateways : System.Web.UI.UserControl
{
    PaymentMethodCollection methods = null;
    PaymentGatewayCollection gateways = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        methods = PaymentMethodDataSource.LoadForStore();
        gateways = PaymentGatewayDataSource.LoadForStore();
        if (!Page.IsPostBack)
        {
            GatewayGrid.DataSource = methods;
            GatewayGrid.DataBind();
        }
    }

    protected void GatewayGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //GET THE GATEWAY DROP DOWN
            DropDownList gateway = PageHelper.RecursiveFindControl(e.Row, "Gateway") as DropDownList;
            if (gateway != null)
            {
                gateway.Items.Clear();
                gateway.Items.Add(new ListItem(string.Empty));
                gateway.DataSource = gateways;
                gateway.DataBind();
                //FIND THE SELECTED
                PaymentMethod method = (PaymentMethod)e.Row.DataItem;
                if (method.PaymentGatewayId != 0)
                {
                    ListItem item = gateway.Items.FindByValue(method.PaymentGatewayId.ToString());
                    if (item != null) item.Selected = true;
                }
            }
        }
    }

    private string GetControlValue(GridViewRow row, string controlId)
    {
        DropDownList ddl = row.FindControl(controlId) as DropDownList;
        if (ddl != null)
        {
            return ddl.SelectedValue;
        }
        return String.Empty;
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in GatewayGrid.Rows)
        {
            PaymentMethod method = methods[row.DataItemIndex];
            int gatewayId = AlwaysConvert.ToInt(GetControlValue(row, "Gateway").Trim());
            method.PaymentGatewayId = gatewayId;
        }
        methods.Save();
        SavedMessage.Visible = true;
    }
}
