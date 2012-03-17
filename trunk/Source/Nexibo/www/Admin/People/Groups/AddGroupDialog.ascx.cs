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

public partial class Admin_People_Groups_AddGroupDialog : System.Web.UI.UserControl
{
    public event PersistentItemEventHandler ItemAdded;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        //INITIALIZE ROLELIST
        RoleCollection roles = RoleDataSource.LoadAll();

        // IF THE USER IS A NON-SYSTEM USER REMOVE THE SYSTEM ROLE FROM THE LIST
        if (!Token.Instance.User.IsSystemAdmin)
        {
            // REMOVE THE SYSTEM ROLE
            Role systemRole = null;
            foreach (Role role in roles)
            {
                if (role.Name == "System")
                {
                    systemRole = role;
                    break;
                }
            }
            if (systemRole != null) roles.Remove(systemRole);
        }
        RoleList.DataSource = roles;
        RoleList.DataBind();
    }

    protected void CreateGroup()
    {
        if (Page.IsValid)
        {
            CommerceBuilder.Users.Group group = new CommerceBuilder.Users.Group();
            group.Name = Name.Text;
            //ADD IN ROLES AND RESET INPUT
            foreach (ListItem roleListItem in RoleList.Items)
            {
                if (roleListItem.Selected)
                {
                    group.GroupRoles.Add(new GroupRole(group.GroupId, AlwaysConvert.ToInt(roleListItem.Value)));
                    roleListItem.Selected = false;
                }
            }
            group.IsTaxExempt = IsTaxExempt.Checked;
            group.Save();
            AddedMessage.Text = string.Format(AddedMessage.Text, group.Name);
            AddedMessage.Visible = true;
            Name.Text = String.Empty;
            IsTaxExempt.Checked = false;
            Name.Focus();
            if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(group.GroupId, group.Name));
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            CreateGroup();
        }
    }
}
