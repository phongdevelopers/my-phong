<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="ViewProducts.aspx.cs" Inherits="Admin_Products_ProductTemplates_ViewProducts" Title="Assigned Products"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="{0}: Assigned Products"></asp:Localize></h1>
    	</div>
    </div>
    <table border="0" cellpadding="5" cellspacing="0" style="width:100%;">
        <tr>
            <td valign="top" style="width:50%;">
                <div class="section">
                    <div class="header">
                        <h2 class="currentlyfeatured">Assigned Products</h2>
                    </div>
                    <div class="content">
                        <asp:GridView ID="AssociatedProductGrid" runat="server" AutoGenerateColumns="False"
                            DataSourceID="AssociatedProductsDs" DataKeyNames="ProductId" ShowHeader="False" AllowPaging="true"
                            Width="100%" GridLines="None" SkinID="PagedList" OnRowDeleting="AssociatedProductGrid_RowDeleting">
                            <Columns>
                                <asp:TemplateField HeaderText="Name">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ProductName2" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%#Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="RemoveButton2" runat="server" SkinID="DeleteIcon" AlternateText="Remove"
                                            ToolTip="Remove" CommandArgument='<%#Eval("ProductId")%>' OnClick="RemoveButton2_Click" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>                                            
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyMessage" runat="server" Text="There are no products associated with this product template."></asp:Label>
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="AssociatedProductsDs" runat="server" OldValuesParameterFormatString="original_{0}"
                            SelectMethod="LoadForProductTemplate" TypeName="CommerceBuilder.Products.ProductDataSource">
                            <SelectParameters>
                                <asp:QueryStringParameter Name="ProductTemplateId" QueryStringField="ProductTemplateId"
                                    Type="Object" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                        <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
                    </div>
                </div>
            </td>
            <td valign="top">
                <div class="section">
                    <div class="header">
                        <h2 class="findfeatured"><asp:Localize ID="FindProductsCaption" runat="server" Text="Find and Assign Products"></asp:Localize></h2>
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
                                    <cb:ToolTipLabel ID="ShowImagesLabel" runat="server" Text="Show Thumbnails:" ToolTip="When checked, product images will be displayed in the search results." />
                                </th>
                                <td>
                                    <asp:CheckBox ID="ShowImages" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
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
                                <asp:TemplateField HeaderText="Link">
                                    <itemstyle width="50px" horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:ImageButton ID="AttachButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Add" ToolTip="Associate with this product template" SkinId="AddIcon" OnClientClick="this.visible=false" OnClick="AttachButton_Click" Visible='<%#!IsProductLinked((Product)Container.DataItem)%>' />
                                        <asp:ImageButton ID="RemoveButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Remove" ToolTip="Unlink from this product template" SkinId="DeleteIcon" OnClientClick="this.visible=false" OnClick="RemoveButton_Click" Visible='<%#IsProductLinked((Product)Container.DataItem)%>' />
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
                                <asp:Parameter Name="sku" Type="String" />
                                <asp:Parameter Name="categoryId" Type="Object" />
                                <asp:Parameter Name="manufacturerId" Type="Object" />
                                <asp:Parameter Name="vendorId" Type="Object" />
                                <asp:Parameter Name="featured" Type="Object" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

