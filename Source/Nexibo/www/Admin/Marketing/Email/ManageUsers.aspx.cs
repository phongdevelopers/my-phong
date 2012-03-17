using System;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Messaging;
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using System.Collections.Generic;

public partial class Admin_Marketing_Email_ManageUsers : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _EmailListId;
    private EmailList _EmailList;

    protected void Page_Load(object sender, EventArgs e)
    {
        _EmailListId = AlwaysConvert.ToInt(Request.QueryString["EmailListId"]);
        _EmailList = EmailListDataSource.Load(_EmailListId);
        if (_EmailList == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _EmailList.Name);
            EmailListUsersCaption.Text = string.Format(EmailListUsersCaption.Text, _EmailList.Name);
            SearchUsersGrid.Visible = false;
        }

        AlphabetRepeater.DataSource = GetAlphabetDS();
        AlphabetRepeater.DataBind();
        PageHelper.SetDefaultButton(SearchEmail, SearchButton.ClientID);
        GroupCollection storeGroups = GroupDataSource.LoadForStore();
        SearchGroup.DataSource = storeGroups;
        SearchGroup.DataTextField = "Name";
        SearchGroup.DataValueField = "GroupId";
        SearchGroup.DataBind();
    }

    protected bool IsInEmailList(object dataItem)
    {
        return _EmailList.IsMember(((User)dataItem).Email);
    }

    protected void IsInEmailList_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox isInEmailList = (CheckBox)sender;
        GridViewRow row = (GridViewRow)isInEmailList.NamingContainer;
        GridView grid = row.NamingContainer as GridView;
        string email = grid.DataKeys[(row.DataItemIndex - (grid.PageSize * grid.PageIndex))].Value.ToString();
        bool isMember = _EmailList.IsMember(email);
        if (isInEmailList.Checked && !isMember)
            _EmailList.AddMember(email, LocaleHelper.LocalNow, Request.UserHostAddress);
        else if (!isInEmailList.Checked && isMember)
            _EmailList.RemoveMember(email);
        //REBIND GRIDS
        UsersInEmailListGrid.DataBind();
        BindSearchPanel();
    }

    protected string GetEditUserUrl(object dataItem)
    {
        User user = this.GetUser(dataItem);
        if (user != null)
        {
            return "~/Admin/People/Users/EditUser.aspx?UserId=" + user.UserId;
        }
        return string.Empty;
    }

    protected string GetFullName(object dataItem)
    {
        User user = this.GetUser(dataItem);
        if (user != null)
        {
            Address address = user.PrimaryAddress;
            if (address != null)
            {
                if (!string.IsNullOrEmpty(address.FirstName) && !string.IsNullOrEmpty(address.LastName)) return ((string)(address.LastName + ", " + address.FirstName)).Trim();
                return ((string)(address.LastName + address.FirstName)).Trim();
            }
        }
        return string.Empty;
    }

    protected string GetUserGroups(object dataItem)
    {
        User user = (User)dataItem;
        List<string> groups = new List<string>();
        foreach (UserGroup userGroup in user.UserGroups)
        {
            groups.Add(userGroup.Group.Name);
        }
        return string.Join(", ", groups.ToArray());
    }

    private User GetUser(object dataItem)
    {
        User user = dataItem as User;
        if (user != null) return user;
        EmailListUser elu = dataItem as EmailListUser;
        if (elu == null) return null;
        if ((_LastUser != null) && (_LastUser.Email == elu.Email)) return _LastUser;
        UserCollection users = UserDataSource.LoadForEmail(elu.Email);
        if (users.Count > 0) _LastUser = users[0];
        else _LastUser = null;        
        return _LastUser;
    }

    private User _LastUser;

    
    protected string[] GetAlphabetDS()
    {
        string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "All" };
        return alphabet;
    }

    protected void AlphabetRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if ((e.CommandArgument.ToString().Length == 1))
        {
            SearchUserName.Text = (e.CommandArgument.ToString() + "*");
        }
        else
        {
            SearchUserName.Text = String.Empty;
        }
        // CLEAR OUT OTHER CRITERIA
        SearchEmail.Text = string.Empty;
        SearchFirstName.Text = string.Empty;
        SearchLastName.Text = string.Empty;
        SearchGroup.SelectedIndex = 0;
        SearchCompany.Text = string.Empty;

        BindSearchPanel();
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        BindSearchPanel();
    }

    private void BindSearchPanel()
    {
        SearchUsersGrid.PageIndex = 0;
        SearchUsersGrid.Visible = true;
        SearchUsersGrid.DataSourceID = "UserDs";
        SearchUsersGrid.DataBind();
    }

    protected void UserDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        UserSearchCriteria criteria = GetSearchCriteria();        
        e.InputParameters["criteria"] = criteria;
    }

    private UserSearchCriteria GetSearchCriteria()
    {
        // BUILD THE SEARCH CRITERIA
        UserSearchCriteria criteria = new UserSearchCriteria();
        criteria.UserName = SearchUserName.Text;
        criteria.Email = SearchEmail.Text;
        criteria.FirstName = SearchFirstName.Text;
        criteria.LastName = SearchLastName.Text;
        criteria.Company = SearchCompany.Text;
        criteria.GroupId = AlwaysConvert.ToInt(SearchGroup.SelectedValue);
        criteria.IncludeAnonymous = false;
        return criteria;
    }
    
}
