using System;
using System.IO;
using System.Web.UI;
using System.Xml;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

public partial class Admin_SEO_Settings : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    string _WebConfigPath;
    protected void Page_Load(object sender, EventArgs e)
    {
        _WebConfigPath = Server.MapPath("~/web.config");
        if (!Page.IsPostBack)
        {
            Store store = Token.Instance.Store;
            StoreSettingCollection settings = store.Settings;
            CacheSize.Text = settings.SeoCacheSize.ToString();
            EnableTracking.Checked = settings.SeoTrackStatistics;
            IntializeCustomExtensionsPanel();
        }
        AllowCustomExtensions.Attributes.Add("onclick", "if(this.checked){document.getElementById('" + CustomExtensionsPanel.ClientID + "').style.display='block';document.getElementById('" + RemoveWebConfigurationPanel.ClientID + "').style.display='none'}else{document.getElementById('" + CustomExtensionsPanel.ClientID + "').style.display='none';document.getElementById('" + RemoveWebConfigurationPanel.ClientID + "').style.display='block'}");
    }

    protected void IntializeCustomExtensionsPanel() 
    {
        double iisVersion = HttpContextHelper.GetIISVersion(Request.ServerVariables["SERVER_SOFTWARE"]);
        if (iisVersion >= 7.0d)
        {
            if (IsIntegratedPipelineMode())
            {
                if (IsHandleAllRequestsConfigured())
                {
                    StoreSettingCollection settings = Token.Instance.Store.Settings;
                    phCustomExtensionsConfigured.Visible = true;
                    AllowCustomExtensions.Checked = settings.SeoAllowCustomExtensions;
                    AllowedExtensions.Text = settings.SeoAllowedExtensions;
                    CustomExtensionsPanel.Style["display"] = AllowCustomExtensions.Checked ? "block" : "none";
                    AllowUrlWithoutExtensions.Checked = settings.SeoAllowUrlWithoutExtension;
                    RemoveWebConfigurationPanel.Style["display"] = AllowCustomExtensions.Checked ? "none" : "block";
                }
                else
                {
                    phCustomExtensionsUnconfigured.Visible = true;
                }
            }
            else
            {
                DisableCustomExtensions("Custom extensions can only be supported when Integrated Pipeline mode is used. We detected that your application is using Classic ASP.NET mode. You will need to update your IIS application settings in order to take advantage of this feature.");
            }
        }
        else
        {
            DisableCustomExtensions("Custom extensions can only be supported on IIS 7.0 or higher. We detected version IIS " + iisVersion.ToString() + ", so this feature is not available. You may only create redirects with the aspx extension.");
        }
    }

    protected void DisableCustomExtensions(string message)
    {
        phCustomExtensionsUnavailable.Visible = true;
        CustomExtensionsMessage.Text = message;
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Store store = Token.Instance.Store;
            StoreSettingCollection settings = store.Settings;
            settings.SeoAllowCustomExtensions = AllowCustomExtensions.Checked;
            settings.SeoAllowedExtensions = AllowedExtensions.Text;
            settings.SeoAllowUrlWithoutExtension = AllowUrlWithoutExtensions.Checked;
            settings.SeoCacheSize = AlwaysConvert.ToInt(CacheSize.Text);
            settings.SeoTrackStatistics = EnableTracking.Checked;
            settings.Save();
            IntializeCustomExtensionsPanel();
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        }
    }

    protected bool IsIntegratedPipelineMode() 
    {
        string mode;
        try
        {
            Response.Headers.Add("AbleCommerce", "7");
            Response.Headers.Remove("AbleCommerce");
            mode = "Integrated";
        }
        catch
        {
            mode = "Classic";
        }
        return (mode == "Integrated");
    }

    protected bool IsHandleAllRequestsConfigured() 
    {
        bool isConfigured = false;

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(_WebConfigPath);
        XmlNode xmlNode = xmlDocument.SelectSingleNode("/configuration/system.webServer/modules");
        if (xmlNode != null)
            isConfigured = AlwaysConvert.ToBool(((XmlElement)xmlNode).GetAttribute("runAllManagedModulesForAllRequests"), false);
        return isConfigured;
    }

    protected void SetWebConfiguration_Click(object sender, EventArgs e)
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(_WebConfigPath);
        XmlNode xmlNode = xmlDocument.SelectSingleNode("/configuration/system.webServer/modules");
        if (xmlNode != null)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            settings.SeoAllowCustomExtensions = true;
            settings.Save();
            ((XmlElement)xmlNode).SetAttribute("runAllManagedModulesForAllRequests", "true");
            string fileContent = XmlUtility.XmlToString(xmlDocument);
            File.WriteAllText(_WebConfigPath, fileContent);
            Response.Redirect(Request.Url.ToString());
        }
    }

    protected void RemoveWebConfigurationButton_Click(object sender, EventArgs e)
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(_WebConfigPath);
        XmlNode xmlNode = xmlDocument.SelectSingleNode("/configuration/system.webServer/modules");
        if (xmlNode != null)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            settings.SeoAllowCustomExtensions = false;
            settings.Save();
            ((XmlElement)xmlNode).RemoveAttribute("runAllManagedModulesForAllRequests");
            string fileContent = XmlUtility.XmlToString(xmlDocument);
            File.WriteAllText(_WebConfigPath, fileContent);
            Response.Redirect(Request.Url.ToString());
        }
    }
}
