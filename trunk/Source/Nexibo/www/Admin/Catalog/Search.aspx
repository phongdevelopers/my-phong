<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Admin_Catalog_Search" Title="Search Catalog" %>
<%@ Register Src="CatalogBreadCrumbs.ascx" TagName="CatalogBreadCrumbs" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="PageAjax" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <uc:CatalogBreadCrumbs ID="CategoryBreadCrumbs" runat="server" />
            <div class="pageHeader">
	            <div class="caption">
                    <h1>
                        <asp:Localize ID="CategoryNameLabel" runat="server" Text="Search " EnableViewState="false"></asp:Localize>
                        <asp:Localize ID="CategoryName" runat="server" Text="Catalog" EnableViewState="false"></asp:Localize>
                    </h1>
	            </div>
            </div>
            <div style="margin-left:4px;margin-right:4px;">
                <table cellpadding="4" cellspacing="0" width="100%">
                    <tr>
	                    <td style="padding:10px 4px 10px 0px;width:250px; vertical-align:top; text-align:left;">
    		                <asp:Panel ID="SearchPanel" runat="server" CssClass="section" DefaultButton="Search" EnableViewState="false">
		                        <div class="header">
			                        <h2 class="searchthiscategory"><asp:Localize ID="SearchCaption" runat="server" Text="Search Criteria" EnableViewState="false"></asp:Localize></h2>
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
                                    <asp:Button ID="Search" runat="server" Text="Search" OnClientClick="this.value='Searching...'" EnableViewState="false" />
		                        </div>
	                        </asp:Panel>
	                    </td>
	                    <td style="padding:10px 0px 10px 0px; vertical-align:top; text-align:left;">
                            <div class="section">
                                <div class="header">
                                    <h2 class="contentsofcatalog">
                                        <asp:Localize ID="ContentsCaption" runat="server" Text="Search Results" EnableViewState="false"></asp:Localize>
                                    </h2>
                                </div>
                                <div class="content">
                                    <asp:GridView ID="CGrid" runat="server" AutoGenerateColumns="False" Width="100%" DataKeyNames="CatalogNodeId,CatalogNodeType" 
                                        AllowSorting="False" AllowPaging="True" PageSize="20" OnRowCommand="CGrid_RowCommand" OnRowDataBound="CGrid_RowDataBound"
                                        SkinId="PagedList" ShowHeader="false" DataSourceID="CatalogDs" EnableViewState="false">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sort">
                                                <HeaderStyle HorizontalAlign="center" Width="27px" />
                                                <ItemStyle Width="27px" HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <img src="<%# GetCatalogIconUrl(Container.DataItem) %>" border="0" alt="<%#Eval("CatalogNodeType")%>" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <%--
                                                    <uc:CatalogBreadCrumbs ID="CategoryBreadCrumbs" runat="server" CategoryId='<%#Eval("CategoryId")%>' />
                                                    --%>
                                                    <asp:LinkButton ID="N" runat="server" Text='<%# Eval("Name") %>' CommandName="Do_Open" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>'></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="C" runat="server" CommandName="Do_Copy" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>' Visible='<%#((CatalogNodeType)Eval("CatalogNodeType") != CatalogNodeType.Category) %>'><img src="<%# GetIconUrl("copy.gif") %>" alt="Copy" border="0" /></asp:LinkButton>
                                                    <asp:LinkButton ID="P" runat="server" CommandName="Do_Pub" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>'><img src="<%# GetVisibilityIconUrl(Container.DataItem) %>" border="0" alt="<%#Eval("Visibility")%>" /></asp:LinkButton>
                                                    <a href="<%# GetEditUrl(Eval("CatalogNodeType"), Eval("CatalogNodeId")) %>"><img src="<%# GetIconUrl("edit.gif") %>" border="0" alt="Edit" /></a>
                                                    <a href="<%# GetMoveUrl(Eval("CatalogNodeType"), Eval("CatalogNodeId")) %>"><img src="<%# GetIconUrl("move.gif") %>" border="0" alt="Move" /></a>
                                                    <asp:LinkButton ID="D" runat="server" CommandName="Do_Delete" CommandArgument='<%#string.Format("{0}|{1}", Eval("CatalogNodeTypeId"), Eval("CatalogNodeId"))%>'><img src="<%# GetIconUrl("delete.gif") %>" border="0" alt="Delete" /></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="right" Width="135px" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <AlternatingRowStyle CssClass="even" />
                                        <EmptyDataTemplate>
                                            <div class="emptyResult">There are no items that match the search criteria.</div>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <asp:ObjectDataSource ID="CatalogDs" runat="server" OldValuesParameterFormatString="original_{0}" 
                                        SelectMethod="Search" TypeName="CommerceBuilder.Catalog.CatalogDataSource"
                                        SelectCountMethod="SearchCount" OnSelecting="CatalogDs_Selecting" EnablePaging="true" 
                                        SortParameterName="sortExpression">
                                        <SelectParameters>
                                            <asp:Parameter Name="categoryId" Type="Object" />
                                            <asp:ControlParameter Name="searchPhrase" ControlID="SearchPhrase" PropertyName="Text" Type="String" />
                                            <asp:ControlParameter Name="titlesOnly" ControlID="TitlesOnly" PropertyName="Checked" Type="Boolean" />
                                            <asp:Parameter Name="publicOnly" Type="Boolean" DefaultValue="false" />
                                            <asp:ControlParameter Name="recursive" ControlID="Recursive" PropertyName="Checked" Type="Boolean" />
                                            <asp:Parameter Name="catalogNodeTypes" Type="Object" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:HiddenField ID="LastCategory" runat="server" />
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

