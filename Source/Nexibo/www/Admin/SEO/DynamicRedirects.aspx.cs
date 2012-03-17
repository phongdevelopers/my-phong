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

public partial class Admin_SEO_DynamicRedirects : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // DISPLAY WARNING MESSAGE FOR IIS VERSION BELOW 7
        IISVersionWarningText.Visible = (HttpContextHelper.GetIISVersion(Request.ServerVariables["SERVER_SOFTWARE"]) < 7.0d);

        // STATISTICS TRACKING ENABLED
        if (!Store.GetCachedSettings().SeoTrackStatistics)
        {
            RedirectsGrid.Columns[4].Visible = false;
            RedirectsGrid.Columns[5].Visible = false;
        }
    }

    protected void SaveButton_Click(Object sender, EventArgs e)
    {
        // VALIDATE UNIQUE REQUEST PATH
        if(RedirectDataSource.LoadForSourceUrl(SourcePath.Text.Trim()) != null)
        {
            UniqueSourcePathValidator.IsValid = false;
            return;
        }

        if (Page.IsValid)
        {
            Redirect redirect = new Redirect();
            redirect.SourceUrl = SourcePath.Text.Trim();
            redirect.TargetUrl = TargetPath.Text.Trim();
            redirect.UseRegEx = true;
            redirect.StoreId = Token.Instance.StoreId;
            redirect.Save();            

            // RESET THE ADD NEW FORM
            SourcePath.Text = String.Empty;
            TargetPath.Text = String.Empty;

            RedirectsGrid.DataBind();
            RedirectGridAjax.Update();
        }
    }

    protected void RedirectsGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName.StartsWith("Do_"))
        {
            int redirectId = (int)AlwaysConvert.ToInt(e.CommandArgument);
            int index;
            switch (e.CommandName)
            {
                case "Do_Up":
                    RedirectCollection redirectRules = RedirectDataSource.LoadForCriteria("UseRegEx = 1 AND StoreId = " + Token.Instance.StoreId, "OrderBy");
                    index = redirectRules.IndexOf(redirectId);
                    if (index > 0)
                    {
                        Redirect tempRedirect = redirectRules[index - 1];
                        redirectRules[index - 1] = redirectRules[index];
                        redirectRules[index] = tempRedirect;
                    }
                    index = 0;
                    foreach (Redirect redirect in redirectRules)
                    {
                        redirect.OrderBy = (short)index;
                        redirect.Save();
                        index++;
                    }
                    RedirectsGrid.DataBind();
                    break;
                case "Do_Down":
                    redirectRules = RedirectDataSource.LoadForCriteria("UseRegEx = 1 AND StoreId = " + Token.Instance.StoreId, "OrderBy");
                    index = redirectRules.IndexOf(redirectId);
                    if (index < redirectRules.Count - 1)
                    {
                        Redirect tempRedirect = redirectRules[index + 1];
                        redirectRules[index + 1] = redirectRules[index];
                        redirectRules[index] = tempRedirect;
                    }
                    index = 0;
                    foreach (Redirect redirect in redirectRules)
                    {
                        redirect.OrderBy = (short)index;
                        redirect.Save();
                        index++;
                    }
                    RedirectsGrid.DataBind();
                    break;
            }
        }
    }
}