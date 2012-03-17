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

public partial class Admin_People_Groups_ManageUsers : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _GroupId;
    private CommerceBuilder.Users.Group _Group;
    protected void Page_Load(object sender, EventArgs e)
    {
        _GroupId = AlwaysConvert.ToInt(Request.QueryString["GroupId"]);
        _Group  = GroupDataSource.Load(_GroupId);
        if (_Group == null) Response.Redirect("Default.aspx");

        // PREVENT NON-SYSTEM USERS TO EDIT SYSTEM GROUPGS
        if (!Token.Instance.User.IsSystemAdmin && _Group.IsInRole("System"))
        {
            Response.Redirect("Default.aspx");
        }
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _Group.Name);
            SearchUsersGrid.Visible = false;
        }
    }

    protected bool IsInGroup(object dataItem)
    {
        User user = (User)dataItem;
        return (user.IsInGroup(_GroupId));
    }

    protected bool EnableChange(object dataItem)
    {
        if (_Group.IsReadOnly)
        {
            User user = (User)dataItem;
            string id = HttpContext.Current.User.Identity.Name;
            return (user.UserId != Token.Instance.UserId);
        }
        return true;
    }
    
    protected void IsInGroup_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox isInGroup = (CheckBox)sender;
        GridViewRow row = (GridViewRow)isInGroup.NamingContainer;
        GridView grid = row.NamingContainer as GridView;
		int dataItemIndex = (row.DataItemIndex - (grid.PageSize * grid.PageIndex));	
        int userId = (int)grid.DataKeys[dataItemIndex].Value;
        
		User user = UserDataSource.Load(userId);
        int index = user.UserGroups.IndexOf(userId, _GroupId);
        if (isInGroup.Checked)
        {
            //IN ROLE WAS CHECKED, ADD ROLE IF NOT FOUND
            if (index < 0)
            {
                user.UserGroups.Add(new UserGroup(userId, _GroupId));
                user.UserGroups.Save();
            }
        }
        else
        {
            //IN ROLE WAS UNCHECKED, DELETE ROLE IF FOUND
            if (index > -1) user.UserGroups.DeleteAt(index);
        }
        //REBIND GRIDS
        UsersInGroupGrid.DataBind();
        BindSearchPanel();
    }
    
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        BindSearchPanel();
    }

    private void BindSearchPanel()
    {
        SearchUsersGrid.Visible = true;
        SearchUsersGrid.DataSourceID = "SearchUsersDs";
        SearchUsersGrid.DataBind();
    }

    protected string GetFullName(object dataItem)
    {
        User user = (User)dataItem;
        Address address = user.PrimaryAddress;
        if (address != null)
        {
            if (!string.IsNullOrEmpty(address.FirstName) && !string.IsNullOrEmpty(address.LastName)) return ((string)(address.LastName + ", " + address.FirstName)).Trim();
            return ((string)(address.LastName + address.FirstName)).Trim();
        }
        return string.Empty;
    }    

}
