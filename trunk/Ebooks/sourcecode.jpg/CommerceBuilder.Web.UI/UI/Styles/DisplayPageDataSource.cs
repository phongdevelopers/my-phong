using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.ComponentModel;
using System.Collections.Generic;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using System.Xml;

namespace CommerceBuilder.UI.Styles
{
    [DataObject(true)]
    public class DisplayPageDataSource
    {

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<DisplayPage> Load()
        {
            return DisplayPageDataSource.Load("~/");
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<DisplayPage> Load(string virtualPath)
        {
            List<DisplayPage> displayPages = new List<DisplayPage>();
            if (HttpContext.Current != null)
            {
                HttpServerUtility server = HttpContext.Current.Server;
                string mappedPath = server.MapPath(virtualPath);
                if (Directory.Exists(mappedPath))
                {
                    string joinPath = string.Empty;
                    string[] aspxFiles = Directory.GetFiles(mappedPath, "*.aspx", SearchOption.TopDirectoryOnly);
                    if (aspxFiles != null)
                    {
                        for (int i = 0; i < aspxFiles.Length; i++)
                        {
                            string virtualFilePath = Path.GetFileName(aspxFiles[i]);
                            DisplayPage dp = DisplayPageDataSource.ParseFromFile(aspxFiles[i], virtualFilePath);
                            if (dp != null)
                            {

                                displayPages.Add(dp);
                            }
                        }
                    }
                }
            }
            return displayPages;
        }

        /// <summary>
        /// Parses the display page comment header.
        /// </summary>
        /// <param name="filePath">The full path to the file to parse.</param>
        /// <param name="virtualPath">The virtual path of the file to register with the DisplayPage instance.</param>
        /// <returns>If the display page comment header is found, returns the DisplayPage instance.  Returns null if the comment header is not found or the file does not exist.</returns>
        public static DisplayPage ParseFromFile(string filePath, string virtualFilePath)
        {
            DisplayPage displayPage = null;
            if (File.Exists(filePath))
            {                
                try
                {
                    string fileContent = File.ReadAllText(filePath);
                    Match headerMatch = Regex.Match(fileContent, "<DisplayPage>[^\x00]*?</DisplayPage>");
                    if (headerMatch.Success)
                    {
                        XmlDocument header = new XmlDocument();
                        header.LoadXml(headerMatch.Value);


                        string name = XmlUtility.GetElementValue(header.DocumentElement, "Name");
                        CatalogNodeType nodeType = (CatalogNodeType)Enum.Parse(typeof(CatalogNodeType), XmlUtility.GetElementValue(header.DocumentElement, "NodeType"));
                        string description = XmlUtility.GetElementValue(header.DocumentElement, "Description");
                        displayPage = new DisplayPage(virtualFilePath, name, nodeType, description);

                    }
                }
                catch(Exception ex)
                {
                    displayPage = null;
                    Logger.Warn("Error while parsing file '" + filePath + "' for display page. " , ex);
                }
            }
            return displayPage;
        }

    }
}
