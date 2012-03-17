<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Browse.aspx.cs" Inherits="Admin_Catalog_Browse" Title="Browse Catalog" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Src="CatalogBreadCrumbs.ascx" TagName="CatalogBreadCrumbs" TagPrefix="uc" %>
<%@ Register Src="../UserControls/AdminBreadCrumbs.ascx" TagName="AdminBreadCrumbs" TagPrefix="uc4" %>
<%@ Register Src="../UserControls/SearchCategory.ascx" TagName="SearchCategory" TagPrefix="uc2" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BreadCrumbsPanel" Runat="Server">
    <ajax:UpdatePanel ID="BreadCrumbsAjax" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <uc:CatalogBreadCrumbs ID="CategoryBreadCrumbs" runat="server" />
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<script type="text/javascript">
    function checkSelected()
    {
        for(i = 0; i< document.forms[0].elements.length; i++)
        {
            var e = document.forms[0].elements[i];
            var name = e.name;
            if ((e.type == 'checkbox') && (name.indexOf('Selected') != -1))
            {                
                if(e.checked) return true;
            }            
        }
        return false;
    }
    
    function confirmSelection()
    {
        var bulkOptionsList = document.getElementById('<%=BulkOptions.ClientID%>'); 
        if(!checkSelected())
        { 
            alert("Please first select some catalog items.");
            bulkOptionsList.selectedIndex = 0;
            return false;
        }
        
        // Delete
        if(bulkOptionsList.selectedIndex == 2)
        {
            var proceed = confirm("Are you sure to delete all selected items?");
            if(!proceed) bulkOptionsList.selectedIndex = 0;
            return proceed;
        }
        return true;
    }
    
    function selectCatalogItems(items)
    {
        if(items == "All" || items == "None")
        {
            var checkState = false;
            if(items == "All") checkState = true;
            for(i = 0; i< document.forms[0].elements.length; i++){
                var e = document.forms[0].elements[i];
                var name = e.name;
                if ((e.type == 'checkbox') && (name.indexOf('Selected') != -1) && !e.disabled)
                {
                    e.checked = checkState;
                }
            }
        }
        else {
            if (items != null)
            {
                for (var i = 0; i < items.length; i++)
                {
                    var id = items[i];
                    var cb = document.getElementById(id);
                    if (cb != null)
                        cb.checked = true;
                }
            }
        }
    }    
</script>
    <ajax:UpdatePanel ID="PageAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
	            <div class="caption">
                    <h1>
                        <asp:Localize ID="CategoryNameLabel" runat="server" Text="Browse: " EnableViewState="false"></asp:Localize>
                        <asp:Localize ID="CategoryName" runat="server" Text="Catalog" EnableViewState="false"></asp:Localize>
                    </h1>
                    <div class="links">
                        <asp:HyperLink ID="EditCategory" runat="server" Text="Edit" EnableViewState="false"></asp:HyperLink>
                        <asp:HyperLink ID="ViewCategory" runat="server" Text="Preview" Target="_blank" EnableViewState="false"></asp:HyperLink>
                    </div>
	            </div>
            </div>
            <div style="margin-left:4px;margin-right:4px;">
                <table cellpadding="4" cellspacing="0" width="100%">
                    <tr>
	                    <td style="padding:10px 4px 10px 0px;width:250px; vertical-align:top; text-align:left;">
	                        <div class="section">
	                            <div class="header">
	                                <h2 class="selectcategory"><asp:Localize ID="CategoryTreeCaption" runat="server" Text="Select Category" EnableViewState="false"></asp:Localize></h2>
	                            </div>
	                            <div class="content">
	                                <asp:PlaceHolder ID="phCategoryTree" runat="server"></asp:PlaceHolder>
	                                <asp:HiddenField ID="LastCategory" runat="server" />
	                            </div>
	                        </div>
	                    </td>
	                    <td style="padding:10px 0px 10px 0px; vertical-align:top; text-align:left;">
                            <div class="section">
                                <div class="header">
                                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                        <tr>
                                            <td>
                                            <h2 class="contentsofcatalog">                                        
                                                <asp:Localize ID="ContentsCaption" runat="server" Text="Contents of {0}" EnableViewState="false"></asp:Localize>
                                            </h2>
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton ID="ParentCategory" runat="server" SkinID="ParentCategoryIcon" OnClick="ParentCategory_Click" EnableViewState="false" ToolTip="Parent Category" />
                                            </td>
                                        </tr>
                                    </table>
                                    
                                </div>
                                <div class="content">
                                    <asp:GridView ID="CGrid" runat="server" AutoGenerateColumns="False" Width="100%" DataKeyNames="CatalogNodeId,CatalogNodeType" 
                                        AllowSorting="False" AllowPaging="True" PageSize="40" OnRowCommand="CGrid_RowCommand" OnRowDataBound="CGrid_RowDataBound"
                                        SkinId="PagedList" ShowHeader="false" DataSourceID="CatalogDs" EnableViewState="false" OnDataBound="CGrid_DataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Select">                                            
                                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                <HeaderStyle Width="20px" />
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="Selected" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Sort">
                                                <HeaderStyle HorizontalAlign="center" Width="54px" />
                                                <ItemStyle Width="78px" HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="MU" runat="server" CommandName="Do_Up" ToolTip="Move Up" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>'><img src="<%# GetIconUrl("arrow_up.gif") %>" border="0" alt="Move Up" /></asp:LinkButton>
                                                    <asp:LinkButton ID="MD" runat="server" CommandName="Do_Down" ToolTip="Move Down"  CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>'><img src="<%# GetIconUrl("arrow_down.gif") %>" border="0" alt="Move Down" /></asp:LinkButton>
                                                    <img src="<%# GetCatalogIconUrl(Container.DataItem) %>" border="0" alt="<%#Eval("CatalogNodeType")%>" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="N" runat="server" Text='<%# Eval("Name") %>' CommandName="Do_Open" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>'></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
													<a href="<%# GetPreviewUrl(Eval("CatalogNodeType"), Eval("CatalogNodeId"), Eval("Name")) %>" Title="Preview" Target="_blank"><img src="<%# GetIconUrl("Preview.gif") %>" border="0" alt="Preview" /></a>

                                                    <asp:LinkButton ID="C" runat="server" ToolTip="Copy" CommandName="Do_Copy" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>' Visible='<%#((CatalogNodeType)Eval("CatalogNodeType") != CatalogNodeType.Category) %>'><img src="<%# GetIconUrl("copy.gif") %>" alt="Copy" border="0" / ></asp:LinkButton>

                                                    <asp:LinkButton ID="P" runat="server" ToolTip='<%#string.Format("Visibility : {0}",Eval("Visibility"))%>' CommandName="Do_Pub" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>'><img src="<%# GetVisibilityIconUrl(Container.DataItem) %>" border="0" alt="<%#Eval("Visibility")%>" /></asp:LinkButton>

                                                    <a href="<%# GetEditUrl(Eval("CatalogNodeType"), Eval("CatalogNodeId")) %>" Title="Edit"><img src="<%# GetIconUrl("edit.gif") %>" border="0" alt="Edit" /></a>
                                                    
													<asp:LinkButton ID="D" runat="server" ToolTip="Delete" CommandName="Do_Delete" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>'><img src="<%# GetIconUrl("delete.gif") %>" border="0" alt="Delete" /></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="right" Width="160px" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <AlternatingRowStyle CssClass="even" />
                                        <EmptyDataTemplate>
                                            <div class="emptyResult">The category is empty.</div>
                                        </EmptyDataTemplate>
                                        <PagerSettings Position="TopAndBottom" />
                                    </asp:GridView>
                                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="SelectHeaderText" runat="server" SkinID="FieldHeader" Text="Select:" EnableViewState="false"></asp:Label>
                                                &nbsp;<asp:LinkButton ID="SelectAll" runat="server" Text="All" EnableViewState="false" OnClientClick="selectCatalogItems('All');return false;"></asp:LinkButton>
                                                &nbsp;<asp:LinkButton ID="SelectNone" runat="server" Text="None" EnableViewState="false" OnClientClick="selectCatalogItems('None');return false;"></asp:LinkButton>
                                                &nbsp;<asp:LinkButton ID="SelectCategories" runat="server" Text="Categories" EnableViewState="false" OnClientClick="return false;"></asp:LinkButton>
                                                &nbsp;<asp:LinkButton ID="SelectProducts" runat="server" Text="Products" EnableViewState="false" OnClientClick="return false;"></asp:LinkButton>
                                                &nbsp;<asp:LinkButton ID="SelectLinks" runat="server" Text="Links" EnableViewState="false" OnClientClick="return false;"></asp:LinkButton>
                                                &nbsp;<asp:LinkButton ID="SelectWebpages" runat="server" Text="Webpages" EnableViewState="false" OnClientClick="return false;"></asp:LinkButton>
                                            </td>
                                            <td align="right">
                                                <asp:DropDownList ID="BulkOptions" runat="server" AutoPostBack="true" EnableViewState="false" OnSelectedIndexChanged="BulkOptions_SelectedIndexChanged">
                                                    <asp:ListItem Text="<Choose Operation>" Selected="true" Value=""/>
                                                    <asp:ListItem Text="Move Selected" Value="Move" />
                                                    <asp:ListItem Text="Delete Selected" Value="Delete" />
                                                    <asp:ListItem Text="Change Visibility" Value="ChangeVisibility" />
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <asp:Button ID="SortCategoryButton" runat="server" Text="Sort Category" OnClick="SortCategoryButton_Click" OnClientClick="this.enabled=false" EnableViewState="false" />
                                    <asp:ObjectDataSource ID="CatalogDs" runat="server" OldValuesParameterFormatString="original_{0}" 
                                        SelectMethod="LoadForCategory" TypeName="CommerceBuilder.Catalog.CatalogDataSource"
                                        SelectCountMethod="CountForCategory" OnSelecting="CatalogDs_Selecting" EnablePaging="true" EnableViewState="false">
                                        <SelectParameters>
                                            <asp:Parameter Name="categoryId" Type="Object" />
                                            <asp:Parameter Name="publicOnly" Type="Boolean" DefaultValue="false" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </div>
                            </div>
	                    <td style="padding:10px 0px 0px 4px; width:200px; vertical-align:top; text-align:left;">
                            <asp:Panel ID="ActionMenuPanel" runat="server">
                                <asp:Panel ID="AddCategoryPanel" runat="server" CssClass="section" DefaultButton="AddCategory" EnableViewState="false">
                                    <div class="header">
                                        <h2 class="newcategory"><asp:Localize ID="ActionMenuCaption" runat="server" Text="New Category:" EnableViewState="false"></asp:Localize></h2>
                                    </div>
                                    <div class="content" style="margin:0px 0px 4px 0px;">
                                        <table class="inputForm" width="100%">
                                            <tr>
                                                <td align="right">
                                                    <asp:TextBox ID="AddCategoryName" runat="server" width="100px" MaxLength="100" EnableViewState="false"></asp:TextBox>
                                                </td>
                                                <td align="left">
                                                    <asp:Button ID="AddCategory" runat="server" Text="Add" OnClick="AddCategory_Click" OnClientClick="this.value='Adding';this.enabled=false" EnableViewState="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="center">
                                                    <asp:Label ID="CategoryAddedMessage" runat="server" Text="{0} added." SkinID="GoodCondition" Visible="false" EnableViewState="False"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="AddCategoryHelpText" runat="server" Text="For more options, click the 'Category' icon below." EnableViewState="false"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="AddContentPanel" runat="server" CssClass="section">
                                    <div class="header">
                                        <h2 class="additems"><asp:Localize ID="AddContentCaption" runat="server" Text="Add Item:" EnableViewState="false"></asp:Localize></h2>
                                    </div>
                                    <div class="content" style="padding:6px 10px; margin:0px 0px 4px 0px; text-align:center">
                                        <table width="90%">
                                            <tr>
                                                <td>
                                                    <asp:HyperLink ID="AddCategoryLink" runat="server" NavigateUrl="AddCategory.aspx" SkinID="Link" EnableViewState="false">
                                                        <asp:Image ID="CategoryIcon" runat="server" SkinID="BigCategoryIcon" EnableViewState="false" /><br />
                                                        <asp:Localize ID="AddCategoryLabel" runat="server" Text="Category" EnableViewState="false"></asp:Localize>
                                                    </asp:HyperLink>
                                                </td>
                                                <td>
                                                    <asp:HyperLink ID="AddProductLink" runat="server" NavigateUrl="../Products/AddProduct.aspx" EnableViewState="false">
                                                        <asp:Image ID="ProductIcon" runat="server" SkinID="BigProductIcon" EnableViewState="false" /><br />
                                                        <asp:Localize ID="AddProductLabel" runat="server" Text="Product" EnableViewState="false"></asp:Localize>
                                                    </asp:HyperLink><br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:HyperLink ID="AddWebpageLink" runat="server" NavigateUrl="AddWebpage.aspx" EnableViewState="false">
                                                        <asp:Image ID="WebpageIcon" runat="server" SkinID="BigWebpageIcon" EnableViewState="false" /><br />
                                                        <asp:Localize ID="AddWebpageLabel" runat="server" Text="Webpage" EnableViewState="false"></asp:Localize>
                                                    </asp:HyperLink>
                                                </td>
                                                <td>
                                                    <asp:HyperLink ID="AddLinkLink" runat="server" NavigateUrl="AddLink.aspx" EnableViewState="false">
                                                    <asp:Image ID="LinkIcon" runat="server" SkinID="BigLinkIcon" EnableViewState="false" /><br />
                                                        <asp:Localize ID="AddLinkLabel" runat="server" Text="Link" EnableViewState="false"></asp:Localize>
                                                    </asp:HyperLink>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
        		                <asp:Panel ID="SearchPanel" runat="server" CssClass="section" DefaultButton="Search" EnableViewState="false">
			                        <div class="header">
				                        <h2 class="searchthiscategory"><asp:Localize ID="SearchCaption" runat="server" Text="Search This Category" EnableViewState="false"></asp:Localize></h2>
				                    </div>
				                    <div class="content">
		                                <asp:Label ID="SearchInstructionText" runat="server" Text="Use of * wildcard allowed." EnableViewState="false"></asp:Label><br />
                                        <asp:Label ID="SearchPhraseLabel" runat="server" Text="Keyword: " SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                                        <asp:TextBox ID="SearchPhrase" runat="server" EnableViewState="false"></asp:TextBox><br />
                                        <asp:CheckBox ID="TitlesOnly" runat="server" Text="Search Titles Only" Checked="true" EnableViewState="false" /><br />
                                        <asp:CheckBox ID="Recursive" runat="server" Text="Include Subcategories" Checked="true" EnableViewState="false" /><br />
                                        <asp:Label ID="TypeFilterLabel" runat="server" Text="Include:" SkinID="FieldHeader" EnableViewState="false"></asp:Label><br />
                                        <asp:CheckBox ID="IncludeCategories" runat="server" Text="Categories" Checked="false" EnableViewState="false" />
                                        <asp:CheckBox ID="IncludeProducts" runat="server" Text="Products" Checked="true" EnableViewState="false" /><br />
                                        <asp:CheckBox ID="IncludeWebpages" runat="server" Text="Webpages" Checked="false" EnableViewState="false" />
                                        <asp:CheckBox ID="IncludeLinks" runat="server" Text="Links" Checked="false" EnableViewState="false" /><br /><br />
                                        <asp:Button ID="Search" runat="server" Text="Search" OnClientClick="this.value='Searching...'" OnClick="Search_Click" EnableViewState="false" />
			                        </div>
		                        </asp:Panel>
                            </asp:Panel>
	                    </td>	
                    </tr>
                </table>
            </div>
            
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

