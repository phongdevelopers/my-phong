<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="DeleteManufacturer.aspx.cs" Inherits="Admin_People_Manufacturers_DeleteManufacturer" Title="Delete Manufacturer"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Delete {0}" EnableViewState="false"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top" width="300">
                <ajax:UpdatePanel ID="EditAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="InstructionText" runat="server" Text="This manufacturer has one or more associated products.  Indicate what manufacturer these products should be changed to when {0} is deleted." EnableViewState="false"></asp:Label>
                        <table class="inputForm" width="100%">
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="NameLabel" runat="server" Text="Move to Manufacturer:" AssociatedControlID="ManufacturerList" ToolTip="New manufacturer for associated products"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:DropDownList ID="ManufacturerList" runat="server" DataTextField="Name" DataValueField="ManufacturerId" AppendDataBoundItems="True">
                                        <asp:ListItem Value="" Text="-- none --"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="submit" colspan="2">                                    
                                    <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" />
									<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
            <td valign="top" width="300">
                <div class="section">
                    <div class="header">
                        <h2><asp:Localize ID="ProductsCaption" runat="server" Text="Assigned Products"></asp:Localize></h2>
                    </div>
                    <div class="content">
                        <ajax:UpdatePanel ID="PagingAjax" runat="server" UpdateMode="conditional">
                            <ContentTemplate>
                                <cb:SortedGridView ID="ProductsGrid" runat="server" DataSourceID="ProductsDs" AllowPaging="True" 
                                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ProductId" PageSize="20" 
                                    SkinID="PagedList" DefaultSortExpression="Name" Width="100%">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Product" SortExpression="Name">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="ProductName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%#Eval("ProductId", "../../Products/EditProduct.aspx?ProductId={0}") %>'></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no products associated with this manufacturer."></asp:Label>
                                    </EmptyDataTemplate>
                                </cb:SortedGridView>
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </div>
                </div>
                <asp:ObjectDataSource ID="ProductsDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}" 
                    SelectCountMethod="CountForManufacturer" SelectMethod="LoadForManufacturer" 
                    SortParameterName="sortExpression" TypeName="CommerceBuilder.Products.ProductDataSource">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="manufacturerId" QueryStringField="ManufacturerId"
                            Type="Object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>