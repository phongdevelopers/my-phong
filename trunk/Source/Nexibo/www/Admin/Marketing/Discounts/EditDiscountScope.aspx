<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="EditDiscountScope.aspx.cs" Inherits="Admin_Marketing_Discounts_EditDiscountScope" Title="Discount Scope" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">    		
    	    <h1><asp:Localize ID="Caption" runat="server" Text="Manage Scope for '{0}'"></asp:Localize></h1>             
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">

        <tr>
            <td colspan="2" width="100%">
                <p align="justify">
                    <asp:Label ID="InstructionText" runat="server" Text="This discount will be applied to all of the assigned categories and/or products.  Use the tree on the left to navigate your categories and check the ones to be assigned to this discount.  You can also use the search form below to locate the specific products to be assigned.  Click Finish when you are done managing the categories and products for this discount."></asp:Label><br /><br />
                </p>
            </td>
        </tr>
        <tr>
            <td valign="top" width="30%">
                <div class="section">
                    <div class="header">
                        <h2>Assigned Categories</h2>
                    </div>
                    <div class="content">
                        <ComponentArt:TreeView id="CategoryTree" runat="server" Width="100%" EnableViewState="true" BackColor="Transparent" />
                    </div>
                </div>
            </td>
            <td valign="top" width="70%">
                <ajax:UpdatePanel ID="MainContentAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="section">
                            <div class="header">
                                <h2><asp:Localize ID="FindProductsCaption" runat="server" Text="Find Products"></asp:Localize></h2>
                            </div>
                            <asp:Panel ID="SearchFormPanel" runat="server" CssClass="content" DefaultButton="SearchButton">
                                <table class="inputForm">
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="SearchNameLabel" runat="server" Text="Product Name:" ToolTip="Enter all or part of a product name.  Wildcard characters * and ? are accepted." />
                                        </th>
                                        <td>
                                            <asp:TextBox ID="SearchName" runat="server" Text=""></asp:TextBox>
                                        </td>
                                        <td colspan="2">
                                            <asp:Button ID="SearchButton" runat="server" Text="Search"  OnClick="SearchButton_Click" />                                        
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="ShowImagesLabel" runat="server" Text="Show Thumbnails:" ToolTip="When checked, product images will be displayed in the search results." />
                                        </th>
                                        <td>
                                            <asp:CheckBox ID="ShowImages" runat="server" />
                                        </td>
                                        <td colspan="2">&nbsp;</td>
                                    </tr>
                                </table>
                                <cb:SortedGridView ID="SearchResultsGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="ProductId" GridLines="None" SkinId="PagedList" DataSourceId="ProductSearchDs"
                                    Width="100%" Visible="false" AllowPaging="true" PageSize="20" AllowSorting="true" DefaultSortExpression="Name">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Attached">
                                            <ItemStyle Width="100px" HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Button ID="AttachButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' Text="Add" OnClientClick="this.value='Adding'" OnClick="AttachButton_Click" Visible='<%#!IsProductLinked(((Product)Container.DataItem).ProductId)%>' />
                                                <asp:Button ID="RemoveButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' Text="Remove" OnClientClick="this.value='Removing'" OnClick="RemoveButton_Click" Visible='<%#IsProductLinked(((Product)Container.DataItem).ProductId)%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item" SortExpression="Name">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="ProductName" runat="server" Text='<%#Eval("Name")%>' SkinID="FieldHeader" /><br />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Thumbnail">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="NodeImageLink" runat="server" NavigateUrl='<%# UrlGenerator.GetBrowseUrl((int)Eval("ProductId"), CatalogNodeType.Product, (string)Eval("Name")) %>'>
                                                    <asp:Image ID="NodeImage" runat="server" ImageUrl='<%# Eval("ThumbnailUrl") %>' Visible='<%# !string.IsNullOrEmpty((string)Eval("ThumbnailUrl")) %>' AlternateText='<%# Eval("Name") %>' />
                                                </asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no products that match the search text."></asp:Label>
                                    </EmptyDataTemplate>
                                </cb:SortedGridView>
                                <asp:ObjectDataSource ID="ProductSearchDs" runat="server"
                                    OldValuesParameterFormatString="original_{0}" SelectMethod="FindProducts" SortParameterName="sortExpression"
                                    TypeName="CommerceBuilder.Products.ProductDataSource" OnSelecting="ProductSearchDs_Selecting" 
                                    SelectCountMethod="FindProductsCount">
                                    <SelectParameters>
                                        <asp:Parameter Name="name" Type="String" />
                                        <asp:Parameter Name="sku" Type="String" />
                                        <asp:Parameter Name="categoryId" Type="Object" />
                                        <asp:Parameter Name="manufacturerId" Type="Object" />
                                        <asp:Parameter Name="vendorId" Type="Object" />
                                        <asp:Parameter Name="featured" Type="Object" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </asp:Panel>
                        </div>
                        <div class="section">
                            <div class="header">
                                <h2>Assigned Products</h2>
                            </div>
                            <div class="content">
                                <cb:SortedGridView ID="ProductGrid" runat="server" AutoGenerateColumns="False" DataSourceID="VolumeDiscountProductsDs"
                                    DataKeyNames="ProductId" ShowHeader="False" Width="100%" GridLines="None" SkinID="PagedList" 
                                    AllowPaging="True" DefaultSortExpression="Name" DefaultSortDirection="Ascending" ShowWhenEmpty="False">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="RemoveButton2" runat="server" SkinID="DeleteIcon" CommandArgument='<%#Container.DataItemIndex%>' OnClick="RemoveImageButton_Click" AlternateText="Remove" ToolTip="Remove" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                            <ItemTemplate>
                                                <asp:Label ID="ProductName2" runat="server" Text='<%#Eval("Name")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no products associated with the discount."></asp:Label>
                                    </EmptyDataTemplate>
                                </cb:SortedGridView>
                                <asp:ObjectDataSource ID="VolumeDiscountProductsDs" runat="server"
                                    OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForVolumeDiscount" SortParameterName="sortExpression"
                                    TypeName="CommerceBuilder.Products.ProductDataSource" SelectCountMethod="CountForVolumeDiscount">
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="volumeDiscountId" QueryStringField="VolumeDiscountId" Type="Object" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </div>
                        </div>                        
                    </ContentTemplate>
                </ajax:UpdatePanel>
                <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" /><br /><br />
            </td>
        </tr>
    </table>
</asp:Content>

