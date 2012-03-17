using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Seo;
using System.Text.RegularExpressions;
using CommerceBuilder.Stores;

public partial class Admin_SEO_FixedRedirects : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // DISPLAY WARNING MESSAGE FOR IIS VERSION BELOW 7
        IISVersionWarningText.Visible = (HttpContextHelper.GetIISVersion(Request.ServerVariables["SERVER_SOFTWARE"]) < 7.0d);

        // STATISTICS TRACKING ENABLED
        if(!Store.GetCachedSettings().SeoTrackStatistics)
        {
            RedirectsGrid.Columns[3].Visible = false;
            RedirectsGrid.Columns[4].Visible = false;
        }
    }

    protected void SaveButton_Click(Object sender, EventArgs e)
    {
        // VALIDATE UNIQUE REQUEST PATH
        if (RedirectDataSource.LoadForSourceUrl(SourcePath.Text.Trim()) != null)
        {
            UniqueSourcePathValidator.IsValid = false;
            return;
        }

        if (Page.IsValid)
        {
            Redirect redirect = new Redirect();
            redirect.SourceUrl = SourcePath.Text;
            redirect.TargetUrl = TargetPath.Text;
            redirect.UseRegEx = false;
            redirect.StoreId = Token.Instance.StoreId;
            redirect.Save();

            // RESET THE ADD NEW FORM
            SourcePath.Text = String.Empty;
            TargetPath.Text = String.Empty;

            RedirectsGrid.DataBind();
            RedirectGridAjax.Update();
        }
    }
}