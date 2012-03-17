using System.IO;
using System.Web;
using System.ComponentModel;
using System.Collections.Generic;
using CommerceBuilder.Utility;

namespace CommerceBuilder.UI.Styles
{
    [DataObject(true)]
    public class ThemeDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Theme> Load()
        {
            List<Theme> themes = new List<Theme>();
            if (HttpContext.Current != null)
            {
                HttpServerUtility server = HttpContext.Current.Server;
                if (Directory.Exists(server.MapPath("~/App_Themes")))
                {
                    string[] themeDirectories = Directory.GetDirectories(server.MapPath("~/App_Themes"));
                    if (themeDirectories != null)
                    {
                        for (int i = 0; i < themeDirectories.Length; i++)
                        {
                            string directoryName = Path.GetFileNameWithoutExtension(themeDirectories[i]);
                            if (!string.IsNullOrEmpty(directoryName))
                            {
                                themes.Add(new Theme(directoryName));
                            }
                        }
                    }
                }
            }
            return themes;
        }

    }
}
