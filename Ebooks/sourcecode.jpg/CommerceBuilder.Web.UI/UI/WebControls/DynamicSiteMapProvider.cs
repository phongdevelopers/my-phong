using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
    public class DynamicSiteMapProvider : XmlSiteMapProvider
    {
        private bool _EnableHiding;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection attributes)
        {
            string enableHiding = attributes.Get("EnableHiding");
            _EnableHiding = (!string.IsNullOrEmpty(enableHiding) && enableHiding.Equals("true"));
            attributes.Remove("EnableHiding");
            base.Initialize(name, attributes);
        }

        private SiteMapNode FindVisibleNode(SiteMapNode testNode)
        {
            //CHECK VISIBILITY
            if (testNode != null)
            {
                string visible = testNode["visible"];
                if (string.IsNullOrEmpty(visible) || !visible.Equals("false")) return testNode;
                //NODE IS NOT VISIBLE, FALL BACK TO PARENT
                return FindVisibleNode(testNode.ParentNode);
            }
            return null;
        }

        public override SiteMapNode FindSiteMapNode(string rawUrl)
        {
            //FIRST CHECK WHETHER THIS PAGE IS SPECIFICALLY KEYED IN THE CONFIG FILE
            SiteMapNode baseNode = base.FindSiteMapNode(rawUrl);
            if (baseNode != null)
            {
                if (_EnableHiding) return FindVisibleNode(baseNode);
                return baseNode;
            }

            //PAGE NOT FOUND, CHECK WHETHER THIS URL MATCHES KNOWN CMS PAGES
            int categoryId = -1;
            WebTrace.Write(this.GetType().ToString(), "FindSiteMapNode: " + rawUrl + ", Check For CategoryId");
            Match urlMatch = Regex.Match(rawUrl, "(?<baseUrl>.*)\\?(?:.*&)?CategoryId=(?<categoryId>[^&]*)", (RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase));
            if (urlMatch.Success)
            {
                //CATEGORYID PROVIDED IN URL
                categoryId = AlwaysConvert.ToInt(urlMatch.Groups[2].Value);
            }
            else
            {
                WebTrace.Write(this.GetType().ToString(), "FindSiteMapNode: " + rawUrl + ", Check For Catalog Object Id");
                urlMatch = Regex.Match(rawUrl, "(?<baseUrl>.*)\\?(?:.*&)?(?<nodeType>ProductId|WebpageId|LinkId)=(?<catalogId>[^&]*)", (RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase));
                if (urlMatch.Success)
                {
                    string objectType = urlMatch.Groups[2].Value;
                    switch (objectType)
                    {
                        case "ProductId":
                            categoryId = CatalogDataSource.GetCategoryId(AlwaysConvert.ToInt(urlMatch.Groups[3].Value), CatalogNodeType.Product);
                            break;
                        case "WebpageId":
                            categoryId = CatalogDataSource.GetCategoryId(AlwaysConvert.ToInt(urlMatch.Groups[3].Value), CatalogNodeType.Webpage);
                            break;
                        default:
                            categoryId = CatalogDataSource.GetCategoryId(AlwaysConvert.ToInt(urlMatch.Groups[3].Value), CatalogNodeType.Link);
                            break;
                    }
                    WebTrace.Write("Found catalogobjectid, type: " + objectType + ", id: " + categoryId.ToString());
                }
            }

            if (categoryId > -1)
            {
                //FIND BASE NODE
                WebTrace.Write(this.GetType().ToString(), "CategoryId Detected, Find Base Node");
                baseNode = base.FindSiteMapNode(urlMatch.Groups[1].Value);
                if (baseNode != null)
                {
                    WebTrace.Write(this.GetType().ToString(), "Base Node Found, Inject Catalog Path");
                    List<SiteMapNode> pathNodes = this.GetSiteMapPathNodes(baseNode);
                    WebTrace.Write("default pathnodes count: " + pathNodes.Count.ToString());
                    int catalogNodeIndex = this.GetCatalogNodeIndex(pathNodes);
                    //IF CATALOG NODE IS NOT FOUND, RETURN THE BASE NODE FOUND BY PROVIDER
                    if (catalogNodeIndex < 0) return baseNode;
                    WebTrace.Write(this.GetType().ToString(), "Catalog Node Obtained, Building Dynamic Path");
                    //APPEND CMS PATH TO THE 
                    List<SiteMapNode> dynamicNodes = new List<SiteMapNode>();
                    List<CmsPathNode> activeCatalogPath = CmsPath.GetCmsPath(0, categoryId, CatalogNodeType.Category);
                    //IF THERE ARE IS NO PATH INFORMATION BEYOND THE ROOT NODE, RETURN THE BASE NODE FOUND BY PROVIDER
                    if ((activeCatalogPath == null) || (activeCatalogPath.Count < 1)) return baseNode;
                    WebTrace.Write("ActivePathCount: " + activeCatalogPath.Count.ToString());
                    for (int i = 0; i < activeCatalogPath.Count; i++)
                    {
                        SiteMapNode newDynamicNode = new SiteMapNode(baseNode.Provider, activeCatalogPath[i].NodeId.ToString(), activeCatalogPath[i].Url, activeCatalogPath[i].Title, activeCatalogPath[i].Description);
                        if (dynamicNodes.Count > 0) newDynamicNode.ParentNode = dynamicNodes[dynamicNodes.Count - 1];
                        dynamicNodes.Add(newDynamicNode);
                    }
                    dynamicNodes[0].ParentNode = pathNodes[catalogNodeIndex];
                    if (catalogNodeIndex == pathNodes.Count - 1)
                    {
                        //THERE ARE NO PATH NODES FOLLOWING CATALOG, RETURN LAST DYNAMIC NODE
                        WebTrace.Write("return last dynamic node");
                        return dynamicNodes[dynamicNodes.Count - 1];
                    }
                    else
                    {
                        //THERE WERE PATH NODES FOLLOWING CATALOG, UPDATE PARENT TO LAST DYNAMIC NODE
                        WebTrace.Write("append nodes following catalog");
                        //GET NODE THAT SHOULD BE LINKED FROM LAST DYNAMIC PATH NODE
                        //CLONE THE NODE TO PREVENT CACHING, THEN SET PARENT TO DYNAMIC NODE
                        SiteMapNode dynamicNextPathNode = pathNodes[catalogNodeIndex + 1].Clone(false);
                        pathNodes[catalogNodeIndex + 1] = dynamicNextPathNode;
                        dynamicNextPathNode.ReadOnly = false;
                        dynamicNextPathNode.ParentNode = dynamicNodes[dynamicNodes.Count - 1];
                        //LOOP THROUGH REMAINING PATH NODES, CLONE THEM AND SET PARENT TO PREVIOUS DYNAMIC NODE
                        for (int i = catalogNodeIndex + 2; i < pathNodes.Count; i++)
                        {
                            dynamicNextPathNode = pathNodes[i].Clone(false);
                            pathNodes[i] = dynamicNextPathNode;
                            dynamicNextPathNode.ReadOnly = false;
                            dynamicNextPathNode.ParentNode = pathNodes[i - 1];
                        }
                        //NOW RETURN LAST PATH NODE
                        return pathNodes[pathNodes.Count - 1];
                    }
                }
            }

            //THIS PATH TO THIS PAGE CANNOT BE DETERMINED
            return null;
        }

        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {

            SiteMapNodeCollection children = base.GetChildNodes(node);
            if (_EnableHiding)
            {
                SiteMapNodeCollection visibleChildren = new SiteMapNodeCollection();
                foreach(SiteMapNode childNode in children)
                {
                    string visible = childNode["visible"];
                    if (string.IsNullOrEmpty(visible) || !visible.ToLowerInvariant().Equals("false"))
                    {
                        visibleChildren.Add(childNode);
                    }
                }
                return visibleChildren;
            }
            return children;
        }

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
    }
}
