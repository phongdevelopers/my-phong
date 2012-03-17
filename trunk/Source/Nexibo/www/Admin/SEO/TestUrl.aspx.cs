using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using CommerceBuilder.Seo;
using System.Text;
using CommerceBuilder.Configuration;
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;

public partial class Admin_SEO_TestUrl : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageHelper.SetDefaultButton(RequestUrl, TestButton);
    }

    protected void TestButton_Click(object sender, EventArgs e)
    {
        string requestUrl = RequestUrl.Text.Trim();
        String redirectUrl = requestUrl;

        int appliedRedirectsCount = 0;
        int appliedRedirectsLimit = 20;
        StringBuilder redirectionLog = new StringBuilder();

        String applicationPath = Request.ApplicationPath;
        if (!applicationPath.EndsWith("/")) applicationPath += "/";

        AbleCommerceApplicationSection appConfig = AbleCommerceApplicationSection.GetSection();
        bool rewriteEnabled = appConfig.UrlRewriterSettings.Enabled;

        string tempUrl = String.Empty;
        do{

            // BACKUP THE REQUESTED URL BEFORE APPLYING RULES
            tempUrl = redirectUrl;

            // CHECK FOR 301 REDIRECT
            Redirect r = RedirectServiceLocator.Instance.LocateRedirect(redirectUrl);
            if (r != null)
            {
                redirectUrl = r.ApplyToUrl(redirectUrl);

                // CHECK IF THE REDIRECT URL IS CHANGED
                if (!tempUrl.Equals(redirectUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    appliedRedirectsCount++;
                    redirectionLog.Append(" is redirected to ").Append("<br>").Append(applicationPath).Append(redirectUrl).Append("<br>");
                }
            }
            else
            {
                // CHECK FOR REWRITE RULES
                // BACKUP THE REQUESTED URL BEFORE APPLYING RULES
                tempUrl = redirectUrl;


                if (rewriteEnabled)
                {
                    String rewriteUrl = GetRewriteUrl(redirectUrl);
                    // CHECK IF THE REDIRECT URL IS CHANGED
                    if (!tempUrl.Equals(rewriteUrl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        appliedRedirectsCount++;
                        redirectionLog.Append(" is rewritten to ").Append("<br>").Append(applicationPath).Append(rewriteUrl).Append("<br>");
                    }
                }
            }

        } while (!tempUrl.Equals(redirectUrl, StringComparison.InvariantCultureIgnoreCase) && appliedRedirectsCount < appliedRedirectsLimit);

        ResultsPanel.Visible = true;
        ResultsCaption.Text = String.Format(ResultsCaption.Text, requestUrl);
        if (appliedRedirectsCount > 0)
        {
            ResultsLabel.Visible = true;
            ResultsLabel.Text = String.Format(ResultsLabel.Text, applicationPath + requestUrl, redirectionLog.ToString());

            if (appliedRedirectsCount >= appliedRedirectsLimit)
            {
                CircularRedirectsLabel.Visible = true;
            }
        }
        else
        {
            NoResultsLabel.Visible = true;
        }
    }

    protected String GetRewriteUrl(String requestUrl)
    {
        // APPEND APPLICATION PATH
        String applicationPath = Request.ApplicationPath;
        if(!applicationPath.EndsWith("/")) applicationPath += "/";
        requestUrl = applicationPath + requestUrl;


        IUrlRewriter rewriter = Token.Instance.UrlRewriter;
        if (rewriter != null)
        {
            requestUrl = rewriter.RewriteUrl(requestUrl);
        }

        // REMOVE APPLICATION PATH
        if (requestUrl.StartsWith(applicationPath)) requestUrl = requestUrl.Substring(applicationPath.Length);

        return requestUrl;
    }


}
