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

public partial class Admin_People_Groups_EditGroupDialog : System.Web.UI.UserControl
{
    public event PersistentItemEventHandler ItemUpdated;

    public int GroupId
    {
        get { return AlwaysConvert.ToInt(ViewState["GroupId"]); }
        set { ViewState["GroupId"] = value; }
    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
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
    }

    protected void UpdateGroup()
    {
        if (Page.IsValid)
        {
            CommerceBuilder.Users.Group group = GroupDataSource.Load(this.GroupId);
            group.Name = Name.Text;
            group.GroupRoles.DeleteAll();
            foreach (ListItem roleListItem in RoleList.Items)
            {
                if (roleListItem.Selected)
                {
                    group.GroupRoles.Add(new GroupRole(this.GroupId, AlwaysConvert.ToInt(roleListItem.Value)));
                }
            }
            group.IsTaxExempt = IsTaxExempt.Checked;
            group.Save();
            SavedMessage.Text = string.Format(SavedMessage.Text, group.Name);
            SavedMessage.Visible = true;
            if (ItemUpdated != null) ItemUpdated(this, new PersistentItemEventArgs(this.GroupId, group.Name));
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            UpdateGroup();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        CommerceBuilder.Users.Group group = GroupDataSource.Load(this.GroupId);
        if (group != null)
        {
            Name.Text = group.Name;
            RoleList.SelectedIndex = -1;
            foreach (GroupRole groupRole in group.GroupRoles)
            {
                ListItem item = RoleList.Items.FindByValue(groupRole.RoleId.ToString());
                if (item != null) item.Selected = true;
            }
            IsTaxExempt.Checked = group.IsTaxExempt;
        }
        else
        {
            this.Controls.Clear();
        }
    }
}
