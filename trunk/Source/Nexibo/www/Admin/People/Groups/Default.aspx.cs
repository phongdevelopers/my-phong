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

public partial class Admin_People_Groups_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void Page_Init(object sender, EventArgs e)
    {
        AddGroupDialog1.ItemAdded += new PersistentItemEventHandler(AddGroupDialog1_ItemAdded);
        EditGroupDialog1.ItemUpdated += new PersistentItemEventHandler(EditGroupDialog1_ItemUpdated);
        User user = Token.Instance.User;
        IsSuperUser = user.IsInRole("System");
        IsSecurityAdmin = IsSuperUser || (user.IsInRole("Admin"));
    }

    private void AddGroupDialog1_ItemAdded(object sender, PersistentItemEventArgs e)
    {
        GroupGrid.DataBind();
        GroupAjax.Update();
    }

    private void EditGroupDialog1_ItemUpdated(object sender, PersistentItemEventArgs e)
    {
        GroupGrid.EditIndex = -1;
        GroupGrid.DataBind();        
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        AddEditAjax.Update();
        GroupAjax.Update();
    }

    protected void GroupGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int groupId = (int)GroupGrid.DataKeys[e.NewEditIndex].Value;
        CommerceBuilder.Users.Group group = GroupDataSource.Load(groupId);
        if (group != null)
        {
            AddPanel.Visible = false;
            EditPanel.Visible = true;
            EditCaption.Text = string.Format(EditCaption.Text, group.Name);
            ASP.admin_people_groups_editgroupdialog_ascx editDialog = EditPanel.FindControl("EditGroupDialog1") as ASP.admin_people_groups_editgroupdialog_ascx;
            if (editDialog != null) editDialog.GroupId = groupId;
            AddEditAjax.Update();
        }
    }

    protected void GroupGrid_RowCancelingEdit(object sender, EventArgs e)
    {
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        AddEditAjax.Update();
    }

    protected string GetRoles(object dataItem)
    {
        CommerceBuilder.Users.Group group = (CommerceBuilder.Users.Group)dataItem;
        List<string> roles = new List<string>();
        foreach (GroupRole groupRole in group.GroupRoles)
        {
            roles.Add(groupRole.Role.Name);
        }
        return string.Join(", ", roles.ToArray());
    }
    
    protected int CountUsers(object dataItem)
    {
        CommerceBuilder.Users.Group group = (CommerceBuilder.Users.Group)dataItem;
        return UserDataSource.CountForGroup(group.GroupId);
    }

    protected bool IsSecurityAdmin = false;
    protected bool IsSuperUser = false;

    protected bool IsEditableGroup(object dataItem)
    {
        CommerceBuilder.Users.Group group = (CommerceBuilder.Users.Group)dataItem;
        if(group.IsInRole("System")){
            if(Token.Instance.User.IsSystemAdmin)
            {
                return (IsSecurityAdmin && (!group.IsReadOnly));
            }else{
                return false;
            }
        }
        else return (IsSecurityAdmin && (!group.IsReadOnly));
    }

    protected bool ShowDeleteButton(object dataItem)
    {
        CommerceBuilder.Users.Group g = (CommerceBuilder.Users.Group)dataItem;
        if (!IsEditableGroup(dataItem)) return false;
        return (UserDataSource.CountForGroup(g.GroupId) <= 0);
    }

    protected bool ShowDeleteLink(object dataItem)
    {
        CommerceBuilder.Users.Group g = (CommerceBuilder.Users.Group)dataItem;
        if (!IsEditableGroup(dataItem)) return false;
        return (UserDataSource.CountForGroup(g.GroupId) > 0);
    }

}
