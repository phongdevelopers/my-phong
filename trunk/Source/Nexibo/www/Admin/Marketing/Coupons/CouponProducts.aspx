<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="CouponProducts.aspx.cs" Inherits="Admin_Marketing_Coupons_CouponProducts" Title="Products Linked to Coupon" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="pageHeader">
            	<div class="caption">
            		<h1><asp:Localize ID="Caption" runat="server" Text="{0}: Linked Products"></asp:Localize></h1>
            	</div>
            </div>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td valign="top" width="30%" class="content">
                        <div class="section">
                            <div class="header">
                                <h2><asp:Localize ID="ProductRestrictionCaption" runat="server" Text="Product Restriction"></asp:Localize></h2>
                            </div>
                            <div class="content">
                                <asp:Label ID="AllowSelectedLabel" runat="server" Text="This coupon is only valid for the linked products listed below."></asp:Label>
                                <asp:Label ID="ExcludeSelectedLabel" runat="server" Text="This coupon is valid for all products, except for the linked products listed below."></asp:Label>
                            </div>
                        </div>
                        <div class="section">
                            <div class="header">
                                <h2>Linked Products</h2>
                            </div>
                            <div class="content">
                                <cb:SortedGridView ID="ProductGrid" runat="server" AutoGenerateColumns="False" DataSourceID="CouponProductsDs"
                                    DataKeyNames="ProductId" ShowHeader="False" Width="100%" GridLines="None" SkinID="PagedList" 
                                    AllowPaging="True" DefaultSortExpression="Name" DefaultSortDirection="Ascending" ShowWhenEmpty="False">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="RemoveButton2" runat="server" SkinID="DeleteIcon" CommandArgument='<%#Container.DataItemIndex%>' OnClick="RemoveButton_Click" AlternateText="Remove" ToolTip="Remove" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                            <ItemTemplate>
                                                <asp:Label ID="ProductName2" runat="server" Text='<%#Eval("Name")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="EmptyMessage" runat="server" Text="none"></asp:Label>
                                    </EmptyDataTemplate>
                                </cb:SortedGridView>
                                <asp:ObjectDataSource ID="CouponProductsDs" runat="server"
                                    OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForCoupon" SortParameterName="sortExpression"
                                    TypeName="CommerceBuilder.Products.ProductDataSource" SelectCountMethod="CountForCoupon">
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="couponId" QueryStringField="CouponId" Type="Object" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </div>
                        </div>
                        <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
                    </td>
                    <td valign="top" class="content">
                        <div class="section">
                            <div class="header">
                                <h2><asp:Localize ID="FindProductsCaption" runat="server" Text="Find Products"></asp:Localize></h2>
                            </div>
                            <div class="content">
                                <table class="inputForm">
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="SearchNameLabel" runat="server" Text="Product Name:" ToolTip="Enter all or part of a product name.  Wildcard characters * and ? are accepted." />
                                        </th>
                                        <td>
                                            <asp:TextBox ID="SearchName" runat="server" Text=""></asp:TextBox>
                                        </td>
                                        <td colspan="2">
                                            <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />                                        
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
                                        <asp:TemplateField HeaderText="Linked">
                                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Button ID="AttachButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' Text="Add" OnClientClick="this.value='adding...'" OnClick="AttachButton_Click" Visible='<%#!IsProductLinked(((Product)Container.DataItem).ProductId)%>' />
                                                <asp:Button ID="RemoveButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' Text="Remove" OnClientClick="this.value='removing...'" OnClick="RemoveButton_Click" Visible='<%#IsProductLinked(((Product)Container.DataItem).ProductId)%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Thumbnail">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="NodeImageLink" runat="server" NavigateUrl='<%# UrlGenerator.GetBrowseUrl((int)Eval("ProductId"), CatalogNodeType.Product, (string)Eval("Name")) %>'>
                                                    <asp:Image ID="NodeImage" runat="server" ImageUrl='<%# Eval("ThumbnailUrl") %>' Visible='<%# !string.IsNullOrEmpty((string)Eval("ThumbnailUrl")) %>' AlternateText='<%# Eval("Name") %>' />
                                                </asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item" SortExpression="Name">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="ProductName" runat="server" Text='<%#Eval("Name")%>' /><br />
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
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>




