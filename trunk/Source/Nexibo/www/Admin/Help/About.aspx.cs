using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Reflection;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;
using System.Xml;

public partial class Admin_Help_About : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AbleCommerceVersion version = AbleCommerceVersion.Instance;
        Caption.Text = string.Format(Caption.Text, version.Version);
        //BUILD DICTIONARY OF DLLS
        SortableCollection<FileVersionInfo> fileVersions = new SortableCollection<FileVersionInfo>();
        string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
        DirectoryInfo di = new DirectoryInfo(binPath);
        FileInfo[] files = di.GetFiles("*.dll");
        foreach (FileInfo file in files)
        {
            try
            {
                Assembly a = Assembly.LoadFile(file.FullName);
                fileVersions.Add(new FileVersionInfo(a.GetName().Name, a.GetName().Version.ToString()));
            }
            catch { }
        }
        fileVersions.Sort("Name");

        // LIST BUILD NUMBERS
        StringBuilder sb = new StringBuilder();
        sb.Append("AbleCommerce for ASP.NET\r\n");
        sb.Append("VERSION: " + version.Version + "." + version.Build + "\r\n");
        
        // ADD DATABASE INFORMATION
        Database database = Token.Instance.Database;
        int majorVersion = database.SqlServerMajorVersion;
        string majorVersionLabel;
        switch (majorVersion)
        {
            case 8:
                majorVersionLabel = "2000";
                break;
            case 9:
                majorVersionLabel = "2005";
                break;
            case 10:
                majorVersionLabel = "2008";
                break;
            default:
                if (majorVersion > 10) majorVersionLabel = ">2008";
                else majorVersionLabel = "Unknown";
                break;
        }
        sb.Append("MSSQL v" + majorVersionLabel + "\r\n");
        sb.Append("AC SCHEMA v" + database.AbleCommerceSchemaVersion + "\r\n");
        sb.Append(".NET CLR v" + System.Environment.Version.ToString() + "\r\n");
        sb.Append("ASP.NET TRUST: " + HttpContextHelper.AspNetTrustLevel.ToString() + "\r\n\r\n");

        // APPEND DLL VERSIONS
        foreach (FileVersionInfo f in fileVersions)
        {
            sb.Append(f.Name + ": " + f.Version + "\r\n");
        }
        DllVersions.Text = sb.ToString();
    }

    public class FileVersionInfo : IComparable
    {
        private string _Name;
        private string _Version;
        public string Name { get { return _Name; } }
        public string Version { get { return _Version; } }
        public FileVersionInfo(string name, string version)
        {
            _Name = name;
            _Version = version;
        }
        public int CompareTo(object obj)
        {
            return this.Name.CompareTo(((FileVersionInfo)obj).Name);
        }
    }
}
