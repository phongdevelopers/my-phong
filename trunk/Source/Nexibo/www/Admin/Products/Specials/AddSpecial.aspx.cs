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
using System.Collections.Generic;

public partial class Admin_Products_Specials_AddSpecial : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ProductId;
    private Product _Product;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect("../../Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId());
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _Product.Name);
            BasePriceMessage.Text = string.Format(BasePriceMessage.Text, _Product.Price);
        }
        Groups.Attributes.Add("onclick", SelectedGroups.ClientID + ".checked = true");
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Special special = new Special();
            special.ProductId = _ProductId;
            special.Price = AlwaysConvert.ToDecimal(Price.Text);            
            special.StartDate = StartDate.SelectedDate;
            special.EndDate = EndDate.SelectedEndDate;
            if (SelectedGroups.Checked)
            {
                foreach (ListItem item in Groups.Items)
                {
                    if (item.Selected) special.SpecialGroups.Add(new SpecialGroup(0, AlwaysConvert.ToInt(item.Value)));
                }
            }
            special.Save();
            Response.Redirect("Default.aspx?ProductId=" + _ProductId.ToString());
        }
    }

}
