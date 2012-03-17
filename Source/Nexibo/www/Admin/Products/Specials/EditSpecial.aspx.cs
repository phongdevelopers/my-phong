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

public partial class Admin_Products_Specials_EditSpecial : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _SpecialId;
    private Special _Special;    
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _SpecialId = AlwaysConvert.ToInt(Request.QueryString["SpecialId"]);
        _Special = SpecialDataSource.Load(_SpecialId);        
        if (_Special == null) Response.Redirect("../../Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId());
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _Special.Product.Name);
            BasePriceMessage.Text = string.Format(BasePriceMessage.Text, _Special.Product.Price);
            BindSpecial();
        }
        Groups.Attributes.Add("onclick", SelectedGroups.ClientID + ".checked = true");
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx?ProductId=" + _Special.ProductId.ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            SaveSpecial();
            if (((Button)sender).ID == "SaveAndCloseButton") Response.Redirect("Default.aspx?ProductId=" + _Special.ProductId.ToString());
            else
            {
                SavedMessage.Visible = true;
                SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
            }
        }
    }
   
    protected void BindSpecial()
    {
        Price.Text = string.Format("{0:F2}", _Special.Price);
        StartDate.SelectedDate = _Special.StartDate;
        EndDate.SelectedDate = _Special.EndDate;
        if (_Special.SpecialGroups != null && _Special.SpecialGroups.Count > 0)
        {
            SelectedGroups.Checked = true;
            Groups.DataBind();
            foreach (SpecialGroup group in _Special.SpecialGroups)
            {
                ListItem listItem = Groups.Items.FindByValue(group.GroupId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
        }
    }

    protected void SaveSpecial()
    {
        _Special.Price = AlwaysConvert.ToDecimal(Price.Text);
        _Special.StartDate = StartDate.SelectedDate;
        _Special.EndDate = EndDate.SelectedEndDate;
        _Special.SpecialGroups.DeleteAll();
        if (SelectedGroups.Checked)
        {            
            foreach (ListItem item in Groups.Items)
            {
                if (item.Selected) _Special.SpecialGroups.Add(new SpecialGroup(0, AlwaysConvert.ToInt(item.Value)));
            }
            if (_Special.SpecialGroups.Count == 0)
            {
                SelectedGroups.Checked = false;
                AllGroups.Checked = true;
            }
        }        
        _Special.Save();
        
    }

}
