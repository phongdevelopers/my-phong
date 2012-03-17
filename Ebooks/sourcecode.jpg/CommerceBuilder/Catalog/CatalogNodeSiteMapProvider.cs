using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Site map provider for Catalog Nodes
    /// </summary>
    public class CatalogNodeSiteMapProvider : System.Web.XmlSiteMapProvider
    {

        private List<SiteMapNode> GetSiteMapPathNodes(SiteMapNode currentNode)
        {
            List<SiteMapNode> pathNodes = new List<SiteMapNode>();
            pathNodes.Add(currentNode);
            SiteMapNode parentNode = currentNode.ParentNode;
            while (parentNode != null)
            {
                pathNodes.Insert(0, parentNode);
                parentNode = parentNode.ParentNode;
            }
            return pathNodes;
        }

        private int GetCatalogNodeIndex(List<SiteMapNode> pathNodes)
        {
            for (int i = 0; i < pathNodes.Count; i++)
            {
                string catalog = pathNodes[i]["catalog"];
                if (!string.IsNullOrEmpty(catalog) && catalog.Equals("true"))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Retrieves a site map node based on a search criterion. 
        /// </summary>
        /// <param name="rawUrl">A URL that identifies the page for which to retrieve a SiteMapNode</param>
        /// <returns>A site map node based on a search criterion</returns>
        public override System.Web.SiteMapNode FindSiteMapNode(string rawUrl)
        {
            //FIRST CHECK WHETHER THIS PAGE IS SPECIFICALLY KEYED IN THE CONFIG FILE
            SiteMapNode baseNode = base.FindSiteMapNode(rawUrl);
            if (baseNode != null) return baseNode;

            //PAGE NOT FOUND, CHECK WHETHER THIS URL MATCHES KNOWN CMS PAGES
            WebTrace.Write(this.GetType().ToString(), "FindSiteMapNode: " + rawUrl + ", Check For CategoryId");
            Match urlMatch = Regex.Match(rawUrl, "(?<baseUrl>.*)\\?.*CategoryId=(?<categoryId>[^&]*)", (RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase));
            if (urlMatch.Success)
            {
                //FIND BASE NODE
                WebTrace.Write(this.GetType().ToString(), "CategoryId Detected, Find Base Node");
                baseNode = base.FindSiteMapNode(urlMatch.Groups[1].Value);
                if (baseNode != null)
                {
                    WebTrace.Write(this.GetType().ToString(), "Base Node Found, Inject Catalog Path");
                    List<SiteMapNode> pathNodes = this.GetSiteMapPathNodes(baseNode);
                    int catalogNodeIndex = this.GetCatalogNodeIndex(pathNodes);
                    //IF CATALOG NODE IS NOT FOUND, RETURN THE BASE NODE FOUND BY PROVIDER
                    if (catalogNodeIndex < 0) return baseNode;
                    WebTrace.Write(this.GetType().ToString(), "Catalog Node Obtained, Building Dynamic Path");
                    //APPEND CMS PATH TO THE 
                    List<SiteMapNode> dynamicNodes = new List<SiteMapNode>();
                    List<CmsPathNode> activeCatalogPath = CmsPath.GetCmsPath(0, AlwaysConvert.ToInt(urlMatch.Groups[2].Value), CatalogNodeType.Category);
                    //IF THERE ARE IS NO PATH INFORMATION BEYOND THE ROOT NODE, RETURN THE BASE NODE FOUND BY PROVIDER
                    if (activeCatalogPath.Count < 2) return baseNode;
                    for (int i = 1; i < activeCatalogPath.Count; i++)
                    {
                        SiteMapNode newDynamicNode = new SiteMapNode(baseNode.Provider, activeCatalogPath[i].NodeId.ToString(), activeCatalogPath[i].Url, activeCatalogPath[i].Title, activeCatalogPath[i].Description);
                        if (dynamicNodes.Count > 0) newDynamicNode.ParentNode = dynamicNodes[dynamicNodes.Count - 1];
                        dynamicNodes.Add(newDynamicNode);
                    }
                    dynamicNodes[0].ParentNode = pathNodes[catalogNodeIndex];
                    if (catalogNodeIndex == pathNodes.Count - 1)
                    {
                        //THERE ARE NO PATH NODES FOLLOWING CATALOG, RETURN LAST DYNAMIC NODE
                        return dynamicNodes[dynamicNodes.Count - 1];
                    }
                    else
                    {
                        //THERE WERE PATH NODES FOLLOWING CATALOG, UPDATE PARENT TO LAST DYNAMIC NODE
                        SiteMapNode nextPathNode = pathNodes[catalogNodeIndex + 1];
                        nextPathNode.ReadOnly = false;
                        nextPathNode.ParentNode = dynamicNodes[dynamicNodes.Count - 1];
                        //THEN RETURN LAST PATH NODE
                        return pathNodes[pathNodes.Count - 1];
                    }
                }
            }

            //THIS PATH TO THIS PAGE CANNOT BE DETERMINED
            return null;
        }
    }
}
