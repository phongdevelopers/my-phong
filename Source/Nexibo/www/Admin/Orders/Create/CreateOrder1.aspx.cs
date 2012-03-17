using System;
using System.Web.UI.WebControls;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Create_CreateOrder1 : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private string FixSearchString(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Contains("*") || value.Contains("?")) return value;
        return "*" + value + "*";
    }

    private UserSearchCriteria GetSearchCriteria()
    {
        // BUILD THE SEARCH CRITERIA
        UserSearchCriteria criteria = new UserSearchCriteria();
        criteria.Email = FixSearchString(SearchEmail.Text);
        criteria.FirstName = FixSearchString(SearchFirstName.Text);
        criteria.LastName = FixSearchString(SearchLastName.Text);
        criteria.IncludeAnonymous = false;
        return criteria;
    }

    protected void UserDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        UserSearchCriteria criteria = GetSearchCriteria();
        e.InputParameters["criteria"] = criteria;
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

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        UserGrid.Visible = true;
        UserGrid.DataBind();
    }

    protected void NewUserButton_Click(object sender, EventArgs e)
    {
        // SEE IF WE HAVE AN ANONYMOUS USER MARKED IN THE SESSION
        User user = UserDataSource.Load(AlwaysConvert.ToInt(Session["CreateOrder_AnonUserId"]));
        if (user == null)
        {
            // CREATE A NEW ANONYMOUS USER AND BASKET FOR THIS ORDER
            user = UserDataSource.CreateUserInstance();
            user.Save();
            user.Basket.Save();
            Session["CreateOrder_AnonUserId"] = user.UserId;
        }
        Response.Redirect("CreateOrder2.aspx?UID=" + user.UserId);
    }
}
