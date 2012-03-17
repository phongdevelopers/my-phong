<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="LowInventory.aspx.cs" Inherits="Admin_Reports_LowInventory" Title="Low Inventory" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
 <div class="pageHeader">
                <div class="caption">
                    <h1><asp:Localize ID="Localize1" runat="server" Text="Products with Low Inventory"></asp:Localize></h1>
                </div>
            </div>
<table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
    
    <tr>
        <td class="dataSheet">
            <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
                <ContentTemplate>                    
                    <asp:Label ID="SavedMessage" runat="server" Text="Data saved at {0:g}" EnableViewState="false" Visible="false" SkinID="GoodCondition"></asp:Label>
                    <cb:SortedGridView ID="InventoryGrid" runat="server" AutoGenerateColumns="False" DataSourceID="InventoryDs" DataKeyNames="ProductId,ProductVariantId"
                        DefaultSortExpression="Name" AllowPaging="True" AllowSorting="true" PageSize="20" 
                        CellPadding="3" RowStyle-CssClass="odd" AlternatingRowStyle-CssClass="even" 
                        BorderColor="white" OnDataBound="InventoryGrid_DataBound" SkinID="Summary" Width="100%">
                        <Columns>
                            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                <ItemTemplate>
                                    <asp:HyperLink ID="ProductLink" runat="server" Text='<%# GetName(Container.DataItem) %>' NavigateUrl='<%#Eval("ProductId", "../Products/EditProduct.aspx?ProductId={0}")%>'></asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="In Stock" SortExpression="InStock">
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:TextBox ID="InStock" runat="server" Text='<%# Eval("InStock") %>' Columns="4"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Low Stock">
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:TextBox ID="LowStock" runat="server" Text='<%# Eval("InStockWarningLevel") %>' Columns="4"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="paging" HorizontalAlign="right" />
                        <PagerSettings NextPageText="" PreviousPageText="" />
                        <EmptyDataTemplate>
                            <div class="emptyResult">
                                <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no items currently at or below the low stock level."></asp:Label>
                            </div>
                        </EmptyDataTemplate>
                    </cb:SortedGridView>
                    <br /><asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />                   
            </ContentTemplate>
        </ajax:UpdatePanel>
     </td>
</tr>
</table>
        
    <asp:ObjectDataSource ID="InventoryDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetLowProductInventory" 
        TypeName="CommerceBuilder.Reporting.ProductInventoryDataSource" SortParameterName="sortExpression" EnablePaging="true" 
        SelectCountMethod="GetLowProductInventoryCount">
    </asp:ObjectDataSource>
</asp:Content>

