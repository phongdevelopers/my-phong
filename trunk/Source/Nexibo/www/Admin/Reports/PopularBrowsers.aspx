<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="PopularBrowsers.aspx.cs" Inherits="Admin_Reports_PopularBrowsers" Title="Browser Popularity Report" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ register tagprefix="web" namespace="WebChart" assembly="WebChart"%>
<%@ Import Namespace="WebChart" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <div class="pageHeader">
        <div class="caption">
        <h1><asp:Localize ID="Caption" runat="server" Text="Most Popular Browsers"></asp:Localize></h1>
        </div>
    </div>
            
    <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="dataSheet" align="center">
                <web:chartcontrol runat="server" id="BrowserChart" Width="600px" Height="300px" SkinID="Pie"></web:chartcontrol>
            </td>
        </tr>
        <tr>
        <td class="dataSheet" align="center">
                <ajax:UpdatePanel ID="ReportAjax" runat="server">
                    <ContentTemplate>
                        <cb:SortedGridView ID="PopularBrowsersGrid" runat="server" AutoGenerateColumns="False"
                            PageSize="40" AllowSorting="True" AllowPaging="True" DataSourceID="BrowserViewsDs"
                            DefaultSortExpression="ViewCount" DefaultSortDirection="Descending" SkinID="Summary"
                            Width="100%">
                            <Columns>       
                                <asp:TemplateField HeaderText="Browser" SortExpression="Browser">
                                    <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="BrowserName" runat="server" Text='<%#Eval("Key")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Views" SortExpression="ViewCount">
                                    <HeaderStyle Wrap="false" Width="100px" HorizontalAlign="Center" />
                                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                                    <ItemTemplate>                    
                                        <asp:Label ID="CountLabel" runat="server" Text='<%#Eval("Value")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </cb:SortedGridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="BrowserViewsDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
        SelectCountMethod="GetViewsByBrowserCount" SelectMethod="GetViewsByBrowser"
        SortParameterName="sortExpression" TypeName="CommerceBuilder.Reporting.PageViewDataSource">
    </asp:ObjectDataSource>
</asp:Content>


