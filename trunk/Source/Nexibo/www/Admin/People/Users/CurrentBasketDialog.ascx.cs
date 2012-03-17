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
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using CommerceBuilder.Common;

public partial class Admin_People_Users_CurrentBasketDialog : System.Web.UI.UserControl
{
    private int _UserId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UserId"]);
        if (!Page.IsPostBack)
        {
            User user = UserDataSource.Load(_UserId);
            BasketItemsGrid.DataSource = user.Basket.Items;
            BasketItemsGrid.DataBind();
            EditBasketLink.NavigateUrl += _UserId;

            // HIDE THE CREATE ORDER OPTION IF THE USER IS ANONYMOUS
            if (user.IsAnonymous)
            {
                EditBasketLink.Visible = false;
            }
        }
    }
}
