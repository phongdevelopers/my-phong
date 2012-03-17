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
using CommerceBuilder.Personalization;

public partial class Admin_Dashboard_AdminWebpartsPanel : System.Web.UI.UserControl
{
    WebPartManager _manager;
    bool isResetVisible = false;

    protected void Page_Init(object sender, EventArgs e)
    {
        this.Page.InitComplete += new EventHandler(Page_InitComplete);
    }

    protected void Page_InitComplete(object sender, EventArgs e)
    {
        _manager = WebPartManager.GetCurrentWebPartManager(this.Page);
        UserPersonalization up = UserPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, Context.User.Identity.Name, false);
        SharedPersonalization sp = SharedPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, false);
        isResetVisible = ((up != null) || (sp != null));

    }

    protected void CurrentMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        WebPartDisplayMode mode = _manager.SupportedDisplayModes[CurrentMode.SelectedValue];
        if (mode != null) _manager.DisplayMode = mode;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        ResetPagePanel.Visible = isResetVisible;
        isResetVisible = false;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (_manager != null)
        {
            Control[] zoneControls = PageHelper.FindControls(this.Page, typeof(CommerceBuilder.Web.UI.WebControls.WebParts.WebPartZone));
            if (zoneControls != null)
            {
                foreach (Control zoneControl in zoneControls)
                {
                    ((CommerceBuilder.Web.UI.WebControls.WebParts.WebPartZone)zoneControl).PartChromeType = PartChromeType.TitleAndBorder;
                }
            }
            CurrentMode.SelectedValue = _manager.DisplayMode.Name;
            trLayoutPanel.Visible = CurrentMode.SelectedValue.Equals("Catalog");
            trEditPanel.Visible = CurrentMode.SelectedValue.Equals("Edit");
            //FIND ALL WEBPART ZONES AND TURN ON CHROME
        }
    }

    protected void ResetPage_Click(object sender, EventArgs e)
    {
        bool refreshRequired = false;
        UserPersonalization up = UserPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, Context.User.Identity.Name, false);
        if (up != null)
        {
            up.Delete();
            refreshRequired = true;
        }
        SharedPersonalization sp = SharedPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, false);
        if (sp != null)
        {
            sp.Delete();
            refreshRequired = true;
        }
        //REDIRECT TO RELOAD WITHOUT PERSONALIZATION
        if (refreshRequired) Response.Redirect(Request.AppRelativeCurrentExecutionFilePath);
    }
}
