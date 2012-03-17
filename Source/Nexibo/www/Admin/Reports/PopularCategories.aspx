<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="PopularCategories.aspx.cs" Inherits="Admin_Reports_PopularCategories" Title="Category Popularity by Views" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server">
        <ContentTemplate>
        <div class="pageHeader">
                <div class="caption">
                    <h1><asp:Localize ID="Caption" runat="server" Text="Category Popularity by Views"></asp:Localize><asp:Localize ID="ReportCaption" runat="server" Text=" for {0:d}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>
            
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="dataSheet" align="center">
                        <cb:SortedGridView ID="PopularCategoriesGrid" runat="server" AutoGenerateColumns="False"
                            PageSize="40" AllowSorting="True" AllowPaging="True" DataSourceID="CategoryViewsDs"
                            DefaultSortExpression="ViewCount" DefaultSortDirection="Descending" SkinID="Summary"
                            Width="90%">
                            <Columns>       
                                <asp:TemplateField HeaderText="Category" SortExpression="Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="CategoryLink" runat="server" Text='<%#Eval("Key.Name")%>' NavigateUrl='<%#Eval("Key.NavigateUrl")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Views" SortExpression="ViewCount">
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
    <asp:ObjectDataSource ID="CategoryViewsDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
        SelectCountMethod="GetViewsByCategoryCount" SelectMethod="GetViewsByCategory"
        SortParameterName="sortExpression" TypeName="CommerceBuilder.Reporting.PageViewDataSource">
    </asp:ObjectDataSource>
</asp:Content>


