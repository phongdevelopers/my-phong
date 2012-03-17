using System.IO;
using System.Web;
using System.ComponentModel;
using System.Collections.Generic;
using CommerceBuilder.Utility;

namespace CommerceBuilder.UI.Styles
{
    [DataObject(true)]
    public class LayoutDataSource
    {

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Layout> Load()
        {
            return LayoutDataSource.Load("~/Layouts");
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Layout> Load(string virtualPath)
        {
            List<Layout> layouts = new List<Layout>();
            if (HttpContext.Current != null)
            {
                HttpServerUtility server = HttpContext.Current.Server;
                string mappedPath = server.MapPath(virtualPath);
                if (Directory.Exists(mappedPath))
                {
                    string[] layoutFiles = Directory.GetFiles(mappedPath, "*.master", SearchOption.TopDirectoryOnly);
                    if (layoutFiles != null)
                    {
                        for (int i = 0; i < layoutFiles.Length; i++)
                        {
                            string fileName = Path.GetFileName(layoutFiles[i]);
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                            string layoutName = StringHelper.SpaceName(fileNameWithoutExtension);
                            string masterPageFile = virtualPath + "/" + fileName;
                            layouts.Add(new Layout(layoutName, masterPageFile));
                        }
                    }
                }
            }
            return layouts;
        }

    }
}
