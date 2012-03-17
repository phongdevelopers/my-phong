using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
    [Designer("System.Web.UI.Design.WebControls.SiteMapPathDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), ToolboxData("<{0}:DynamicSiteMapPath runat=server></{0}:DynamicSiteMapPath>")]
    public class DynamicSiteMapPath : SiteMapPath
    {

        private int _CategoryId = 0;
        private int _CurrentNodeId = 0;
        private CatalogNodeType _CurrentNodeType;

        public int CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }

        public int CurrentNodeId
        {
            get { return _CurrentNodeId; }
            set { _CurrentNodeId = value; }
        }

        public CatalogNodeType CurrentNodeType
        {
            get { return _CurrentNodeType; }
            set { _CurrentNodeType = value; }
        }

        protected override void CreateControlHierarchy()
        {
            if (HttpContext.Current != null)
            {
                WebTrace.Write(this.GetType().ToString(), "CreateControlHierarchy, CategoryId " + this.CategoryId);
                WebTrace.Write(this.GetType().ToString(), "CurrentNodeId: " + CurrentNodeId);
                WebTrace.Write(this.GetType().ToString(), "CurrentNodeType: " + CurrentNodeType);
                SiteMapNodeItemType itemType;
                List<CmsPathNode> cmsPath = CmsPath.GetCmsPath(CategoryId, CurrentNodeId, CurrentNodeType);
                int pathIndex = 0;
                int nodeIndex = 1;
                foreach(CmsPathNode node in cmsPath)
                {
                    if (nodeIndex == cmsPath.Count)
                    {
                        WebTrace.Write(node.Title + ": Current");
                        itemType = SiteMapNodeItemType.Current;
                    }
                    else if (nodeIndex == 1)
                    {
                        WebTrace.Write(node.Title + ": Root");
                        itemType = SiteMapNodeItemType.Root;
                    }
                    else
                    {
                        WebTrace.Write(node.Title + ": Parent");
                        itemType = SiteMapNodeItemType.Parent;
                    }
                    if (nodeIndex > 1)
                    {
                        this.CreateItemFromCmsPathNode(pathIndex, SiteMapNodeItemType.PathSeparator, null);
                        pathIndex++;
                    }
                    CreateItemFromCmsPathNode(pathIndex, itemType, node);
                    pathIndex++;
                    nodeIndex++;
                }
            }
            else
            {
                base.CreateControlHierarchy();
            }
        }

        private SiteMapNodeItem CreateItemFromCmsPathNode(int itemIndex, SiteMapNodeItemType itemType, CmsPathNode pathNode)
        {
            SiteMapNodeItem item1 = new SiteMapNodeItem(itemIndex, itemType);
            int num1 = (this.PathDirection == PathDirection.CurrentToRoot) ? 0 : -1;
            SiteMapNodeItemEventArgs args1 = new SiteMapNodeItemEventArgs(item1);
            if (pathNode != null)
            {
                string nodeUrl = string.Empty;
                switch (pathNode.NodeType)
                {
                    case CatalogNodeType.Category:
                        nodeUrl = "Default.aspx?CategoryId=" + pathNode.NodeId;
                        break;
                    case CatalogNodeType.Product:
                        nodeUrl = "../products/editproduct.aspx?ProductId=" + pathNode.NodeId;
                        break;
                    case CatalogNodeType.Webpage:
                        nodeUrl = "../webpages/editwebpage.aspx?WebpageId=" + pathNode.NodeId;
                        break;
                    case CatalogNodeType.Link:
                        nodeUrl = "viewlink.aspx?LinkId=" + pathNode.NodeId;
                        break;
                }
                SiteMapNode mapNode = new SiteMapNode(new XmlSiteMapProvider(), pathNode.NodeId.ToString(), nodeUrl, pathNode.Title, pathNode.Description);
                item1.SiteMapNode = mapNode;
            }
            this.InitializeItem(item1);
            this.OnItemCreated(args1);
            this.Controls.AddAt(num1, item1);
            item1.DataBind();
            this.OnItemDataBound(args1);
            item1.SiteMapNode = null;
            item1.EnableViewState = false;
            return item1;
        }

        public void Rebuild()
        {
            WebTrace.Write(this.GetType().ToString(), "Rebuild on CategoryId " + this.CategoryId);
            this.Controls.Clear();
            this.CreateControlHierarchy();
        }

    }
}
