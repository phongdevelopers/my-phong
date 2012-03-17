using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;

public partial class Admin_Catalog_AddWebpage : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _CategoryId = 0;
    private Category _Category = null;

    protected void Page_Init(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        CancelButton.NavigateUrl = "Browse.aspx?CategoryId=" + _CategoryId.ToString();
        if (_Category == null) Response.Redirect(CancelButton.NavigateUrl);
        if (!Page.IsPostBack)
        {
            //INITIALIZE PAGE ON FIRST VISIT
            Name.Focus();
            Caption.Text = string.Format(Caption.Text, _Category.Name);
            BindDisplayPage();
            BindThemes();
            UpdateDescription();
        }
        PageHelper.ConvertEnterToTab(Name);
        PageHelper.SetMaxLengthCountDown(Summary, SummaryCharCount);
        PageHelper.SetHtmlEditor(WebpageContent, WebpageContentHtml);
        PageHelper.SetPickImageButton(ThumbnailUrl, BrowseThumbnailUrl);
        PageHelper.SetPageDefaultButton(Page, FinishButton);


        PageHelper.PreventFirefoxSubmitOnKeyPress(Summary, Summary.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(WebpageContent, WebpageContent.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(HtmlHead, HtmlHead.ClientID);
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Webpage webpage = new Webpage();
            webpage.Name = Name.Text;
            webpage.Visibility = (CatalogVisibility)Visibility.SelectedIndex;
            webpage.ThumbnailUrl = ThumbnailUrl.Text;
            webpage.ThumbnailAltText = ThumbnailAltText.Text;
            webpage.CustomUrl = CustomUrl.Text;
            webpage.Summary = StringHelper.Truncate(Summary.Text, Summary.MaxLength);
            webpage.Description = WebpageContent.Text;
            webpage.HtmlHead = HtmlHead.Text;
            webpage.DisplayPage = DisplayPage.SelectedValue;
            webpage.Theme = LocalTheme.SelectedValue;
            webpage.Categories.Add(_CategoryId);
            webpage.Save();
            // RETURN TO BROWSE PARENT CATEGORY
            Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + _CategoryId.ToString());
        }
    }

    protected void BindDisplayPage()
    {
        List<CommerceBuilder.UI.Styles.DisplayPage> displayPages = CommerceBuilder.UI.Styles.DisplayPageDataSource.Load();
        foreach (CommerceBuilder.UI.Styles.DisplayPage displayPage in displayPages)
        {
            if (displayPage.NodeType == CatalogNodeType.Webpage)
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
                DisplayPageDescription.Text = page.Description;
            }
        }
        else
        {
            DisplayPageDescription.Text = string.Empty;
        }
    }

    protected void BindThemes()
    {
        LocalTheme.DataSource = StoreDataHelper.GetStoreThemes();
        LocalTheme.DataBind();
    }
}