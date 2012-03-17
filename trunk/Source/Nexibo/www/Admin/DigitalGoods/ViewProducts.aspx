<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ViewProducts.aspx.cs" Inherits="Admin_DigitalGoods_ViewProducts" Title="Digital Good Products"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Products assigned to '{0}'"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="4" cellspacing="0" class="inputForm" width="100%" >
        <tr>
            <td valign="top">
                <ajax:UpdatePanel ID="MainContentAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table border="0" cellpadding="5" cellspacing="0" style="width:100%;">
                            <tr>
                                <td valign="top" style="width:50%;">
                                    <div class="section">
                                        <div class="header">
                                            <h2>Linked Products</h2>
                                        </div>
                                        <div class="content">
                                            <asp:GridView ID="RelatedProductGrid" runat="server" AutoGenerateColumns="False"
                                                DataSourceID="RelatedProductsDs" DataKeyNames="ProductId" ShowHeader="False" AllowPaging="true"
                                                Width="100%" GridLines="None" SkinID="PagedList" OnRowDeleting="RelatedProductGrid_RowDeleting">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Name">
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="ProductName2" runat="server" Text='<%#Eval("Product.Name")%>' NavigateUrl='<%#Eval("Product.ProductId", "~/Admin/Products/DigitalGoods/DigitalGoods.aspx?ProductId={0}")%>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="RemoveButton2" runat="server" SkinID="DeleteIcon" AlternateText="Remove"
                                                                ToolTip="Remove" CommandArgument='<%#Eval("Product.ProductId")%>' OnClick="RemoveButton2_Click" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>                                            
                                                <EmptyDataTemplate>
                                                    <asp:Label ID="EmptyMessage" runat="server" Text="There are no products for this digital good."></asp:Label>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:ObjectDataSource ID="RelatedProductsDs" runat="server" OldValuesParameterFormatString="original_{0}"
                                                SelectMethod="LoadForDigitalGood" TypeName="CommerceBuilder.DigitalDelivery.ProductDigitalGoodDataSource" SelectCountMethod="CountForDigitalGood"  SortParameterName="sortExpression">
                                                <SelectParameters>
                                                    <asp:QueryStringParameter Name="DigitalGoodId" QueryStringField="DigitalGoodId"
                                                        Type="Object" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </div>
                                    </div>
                                </td>
                                <td valign="top">
                                    <div class="section">
                                        <div class="header">
                                            <h2>
                                                <asp:Localize ID="FindProductsCaption" runat="server" Text="Find Products"></asp:Localize></h2>
                                        </div>
                                        <asp:Panel ID="SearchFormPanel" runat="server" CssClass="content" DefaultButton="SearchButton">
                                            <table class="inputForm">
                                                <tr>
                                                    <th class="rowHeader" style="text-align: left;">
                                                        <cb:ToolTipLabel ID="SearchNameLabel" runat="server" Text="Product Name:" ToolTip="Enter all or part of a product name.  Wildcard characters * and ? are accepted." />
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="SearchName" runat="server" Text=""></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader" style="text-align: left;">
                                                        <cb:ToolTipLabel ID="SkuLabel" runat="server" Text="SKU:" ToolTip="Enter all or part of a product SKU.  Wildcard characters * and ? are accepted." />
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="SKU" runat="server" Text=""></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader" style="text-align: left;">
                                                        <cb:ToolTipLabel ID="ShowImagesLabel" runat="server" Text="Show Thumbnails:" ToolTip="When checked, product images will be displayed in the search results." />
                                                    </th>
                                                    <td>
                                                        <asp:CheckBox ID="ShowImages" runat="server" />
                                                    </td>
                                                </tr>                                                
                                                <tr>
                                                    <td colspan="2" align="right">
                                                        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" /><br />
                                                    </td>
                                                </tr>
                                            </table>
                                            <cb:SortedGridView ID="SearchResultsGrid" runat="server" AutoGenerateColumns="False"
                                                DataKeyNames="ProductId" GridLines="None" SkinID="PagedList" DataSourceID="ProductSearchDs"
                                                Width="100%" Visible="false" AllowPaging="true" PageSize="20" AllowSorting="true"
                                                DefaultSortExpression="Name">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Thumbnail">
                                                        <itemstyle horizontalalign="Center" />
                                                        <itemtemplate>
                                                    <asp:HyperLink ID="NodeImageLink" runat="server" NavigateUrl='<%#Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}")%>'>
                                                        <asp:Image ID="NodeImage" runat="server" ImageUrl='<%# Eval("ThumbnailUrl") %>' Visible='<%# !string.IsNullOrEmpty((string)Eval("ThumbnailUrl")) %>' AlternateText='<%# Eval("Name") %>' />
                                                    </asp:HyperLink>
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product" SortExpression="Name">
                                                        <headerstyle horizontalalign="Left" />
                                                        <itemtemplate>
                                                    <asp:HyperLink ID="ProductName" runat="server" Text='<%#Eval("Name")%>' SkinID="FieldHeader" NavigateUrl='<%#Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}")%>' /><br />
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SKU">
                                                        <headerstyle horizontalalign="Center" />
                                                        <itemstyle horizontalalign="Center" />
                                                        <itemtemplate>
                                                    <asp:Label ID="SkuLabel2" runat="server" Text='<%#Eval("Sku")%>' SkinID="FieldHeader"  /><br />
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Related">
                                                        <itemstyle width="50px" horizontalalign="Center" />
                                                        <itemtemplate>
                                                    <asp:ImageButton ID="AttachButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Add" ToolTip="Assign to this digital good." SkinId="AddIcon" OnClientClick="this.visible=false" OnClick="AttachButton_Click" Visible='<%#!IsProductAssciated((Product)Container.DataItem)%>' />
                                                    <asp:ImageButton ID="RemoveButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Remove" ToolTip="Remove from this digital good." SkinId="DeleteIcon" OnClientClick="this.visible=false" OnClick="RemoveButton_Click" Visible='<%#IsProductAssciated((Product)Container.DataItem)%>' />
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <asp:HyperLink ID="EmptyMessage" runat="server" Text="There are no products that match the search text."></asp:HyperLink>
                                                </EmptyDataTemplate>
                                            </cb:SortedGridView>
                                            <asp:ObjectDataSource ID="ProductSearchDs" runat="server" OldValuesParameterFormatString="original_{0}"
                                                SelectMethod="FindProducts" SortParameterName="sortExpression" TypeName="CommerceBuilder.Products.ProductDataSource"
                                                OnSelecting="ProductSearchDs_Selecting" SelectCountMethod="FindProductsCount">
                                                <SelectParameters>
                                                    <asp:ControlParameter Name="name" ControlID="SearchName" PropertyName="Text" Type="String" />
                                                    <asp:ControlParameter Name="sku" ControlID="SKU" PropertyName="Text" Type="String"  />
                                                    <asp:Parameter Name="categoryId" Type="Object" />
                                                    <asp:Parameter Name="manufacturerId" Type="Object" DefaultValue="-1" />
                                                    <asp:Parameter Name="vendorId" Type="Object" />
                                                    <asp:Parameter Name="featured" Type="Object" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </asp:Panel>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <asp:HyperLink ID="BackLink" runat="server" Text="Back" SkinID="Button" NavigateUrl="DigitalGoods.aspx"></asp:HyperLink>
            </td>
        </tr>
    </table>
</asp:Content>

