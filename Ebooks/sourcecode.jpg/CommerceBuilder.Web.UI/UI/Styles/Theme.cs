using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using CommerceBuilder.Utility;

namespace CommerceBuilder.UI.Styles
{
    public class Theme
    {
        private string _Name;
        private string _DisplayName;
        private bool _IsAdminTheme;

        public string Name
        {
            get { return _Name; }
        }

        public string DisplayName
        {
            get { return _DisplayName; }
        }

        public bool IsAdminTheme
        {
            get
            {
                return _IsAdminTheme;
            }
        }

        public Theme(string name, string displayName)
        {
            _Name = name;
            _DisplayName = displayName;
            _IsAdminTheme = _Name.ToLowerInvariant().EndsWith("admin");
        }

        public Theme(string directoryName)
        {
            _Name = directoryName;
            if (directoryName.ToLowerInvariant().EndsWith("admin"))
            {
                _DisplayName = (directoryName.Substring(0, directoryName.Length - 5)).Replace("_", " ");
                _IsAdminTheme = true;
            } else {
                _DisplayName = directoryName.Replace("_", " ");
                _IsAdminTheme  = false; 
            }
        }

        public static bool Exists(string name)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                return Directory.Exists(context.Server.MapPath("~/App_Themes/" + name));
            }
            return true;
        }

    }
}