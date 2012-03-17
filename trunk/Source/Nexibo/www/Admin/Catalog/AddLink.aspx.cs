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
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using System.Collections.Generic;

public partial class Admin_Catalog_AddLink : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _CategoryId;
    private Category _Category;

    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.SetPickImageButton(ThumbnailUrl, BrowseThumbnailUrl);
        PageHelper.SetHtmlEditor(Description, LinkDescriptionHtml);
        PageHelper.SetPageDefaultButton(Page, FinishButton);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        if (_Category == null) Response.Redirect("Browse.aspx");
        if (!Page.IsPostBack)
        {
            Name.Focus();
            Caption.Text = string.Format(Caption.Text, _Category.Name);
            BindDisplayPage();
            BindThemes();
            UpdateDescription();
        }
        PageHelper.ConvertEnterToTab(Name);
        PageHelper.SetMaxLengthCountDown(Summary, SummaryCharCount);

        PageHelper.PreventFirefoxSubmitOnKeyPress(Summary, Summary.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(Description, Description.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(HtmlHead, HtmlHead.ClientID);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        // RETURN TO BROWSE PARENT CATEGORY
        Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + _CategoryId.ToString());
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        Link link = new Link();
        link.Name = Name.Text;
        link.Visibility = (CatalogVisibility)Visibility.SelectedIndex;
        link.TargetUrl = TargetUrl.Text;
        link.TargetWindow = TargetWindow.SelectedValue;
        link.DisplayPage = DisplayPage.SelectedValue;
        link.Theme = LocalTheme.SelectedValue;
        link.Summary = StringHelper.Truncate(Summary.Text, Summary.MaxLength);
        link.Description = Description.Text;
        link.ThumbnailUrl = ThumbnailUrl.Text;
        link.ThumbnailAltText = ThumbnailAltText.Text;
        link.HtmlHead = HtmlHead.Text;
        link.Categories.Add(_CategoryId);
        link.Save();
        // RETURN TO BROWSE PARENT CATEGORY
        Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + _CategoryId.ToString());
    }

    protected void BindDisplayPage()
    {
        List<CommerceBuilder.UI.Styles.DisplayPage> displayPages = CommerceBuilder.UI.Styles.DisplayPageDataSource.Load();
        foreach (CommerceBuilder.UI.Styles.DisplayPage displayPage in displayPages)
        {
            if (displayPage.NodeType == CatalogNodeType.Link)
            {
                string displayName = string.Format("{0}", displayPage.Name);
                DisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
            }
        }
    }

    protected void DisplayPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDescription();
    }

    private void UpdateDescription()
    {
        DisplayPageDescription.Text = "&nbsp;";
        if (DisplayPage.SelectedValue != string.Empty)
        {
            CommerceBuilder.UI.Styles.DisplayPage page = CommerceBuilder.UI.Styles.DisplayPageDataSource.ParseFromFile(Server.MapPath("~/" + DisplayPage.SelectedValue), "/");
            if (page != null)
            {
                DisplayPageDescription.Text = "<strong>" + DisplayPage.SelectedItem.Text + ":</strong> " + page.Description;
            }
        }
        else
        {
            DisplayPageDescription.Text = "<strong>" + DisplayPage.Items[0].Text + ":</strong> Displays a direct link to the specified url, eliminating the need for a display page.";
        }
    }

    protected void BindThemes()
    {
        LocalTheme.DataSource = StoreDataHelper.GetStoreThemes();
        LocalTheme.DataBind();
    }
}
