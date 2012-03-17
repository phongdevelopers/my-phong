<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="PopularProducts.aspx.cs" Inherits="Admin_Reports_PopularProducts" Title="Product Popularity by Views" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server">
        <ContentTemplate>
        <div class="pageHeader">
                <div class="caption">
                    <h1><asp:Localize ID="Caption" runat="server" Text="Product Popularity by Views"></asp:Localize><asp:Localize ID="ReportCaption" runat="server" Text=" for {0:d}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>
            
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="dataSheet" align="center">
                        <cb:SortedGridView ID="PopularProductsGrid" runat="server" AutoGenerateColumns="False"
                            PageSize="40" AllowSorting="True" AllowPaging="True" DataSourceID="ProductViewsDs"
                            DefaultSortExpression="ViewCount" DefaultSortDirection="Descending" SkinID="Summary"
                            Width="90%">
                            <Columns>       
                                <asp:TemplateField HeaderText="Product" SortExpression="Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ProductLink" runat="server" Text='<%#Eval("Key.Name")%>' NavigateUrl='<%#String.Format("~/Admin/Products/EditProduct.aspx?ProductId={0}",Eval("Key.ProductId"))%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Views" SortExpression="ViewCount">
                                    <ItemStyle HorizontalAlign="center" width="80" />
                                    <ItemTemplate>                    
                                        <asp:Label ID="CountLabel" runat="server" Text='<%#Eval("Value")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </cb:SortedGridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="ProductViewsDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
        SelectCountMethod="GetViewsByProductCount" SelectMethod="GetViewsByProduct"
        SortParameterName="sortExpression" TypeName="CommerceBuilder.Reporting.PageViewDataSource">
    </asp:ObjectDataSource>
</asp:Content>