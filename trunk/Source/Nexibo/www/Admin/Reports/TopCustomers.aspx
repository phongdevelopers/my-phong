<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="TopCustomers.aspx.cs" Inherits="Admin_Reports_TopCustomers" Title="Top Customers"  %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                     <h1><asp:Localize ID="Caption" runat="server" Text="Sales by Customer"></asp:Localize>
                         <asp:Localize ID="ReportFromDate" runat="server" Text=" from {0:d}" EnableViewState="false" Visible="false"></asp:Localize>
                         <asp:Localize ID="ReportToDate" runat="server" Text=" to {0:d}" EnableViewState="false" Visible="false"></asp:Localize>
                     </h1>
                </div>
            </div>
            
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr class="noPrint">
                    <td>
                        <table align="center" class="form" cellpadding="0" cellspacing="0" border="0" width="100%">
                            <tr>
                                <th class="sectionHeader" colspan="4">
                                    <div style="text-align: left">
                                        Report Period</div>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <div style="text-align: right; vertical-align: middle">
                                        <asp:Label ID="Label1" runat="server" Text="From:  " SkinID="FieldHeader"></asp:Label>
                                    </div>
                                </td>
                                <td style="text-align: left">
                                    <uc1:PickerAndCalendar ID="StartDate" runat="server" />
                                </td>
                                <td>
                                    <div style="text-align: right; vertical-align: middle">
                                        <asp:Label ID="Label4" runat="server" Text="To:  " SkinID="FieldHeader"></asp:Label>
                                    </div>
                                </td>
                                <td style="text-align: left">
                                    <uc1:PickerAndCalendar ID="EndDate" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="text-align: right; vertical-align: middle">
                                        <asp:Button ID="ProcessButton" runat="server" Text="GO.." OnClick="ProcessButton_Click" />
                                    </div>
                                </td>
                                <td colspan="3">
                                </td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
                
                <tr>
                    <td class="dataSheet">
                        <cb:SortedGridView ID="TopCustomerGrid" runat="server" AutoGenerateColumns="False" DataSourceID="TopCustomerDs"
                            DefaultSortExpression="OrderTotal" DefaultSortDirection="Descending" AllowPaging="True" AllowSorting="true"
                            PageSize="40" OnSorting="TopCustomerGrid_Sorting" Width="100%" SkinID="Summary">
                            <Columns>
                                <asp:TemplateField HeaderText="User">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="UserLink" runat="server" Text='<%# Eval("User.UserName") %>' NavigateUrl='<%#Eval("UserId", "../People/Users/EditUser.aspx?UserId={0}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Count" SortExpression="OrderCount">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("OrderCount") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Total" SortExpression="OrderTotal">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("OrderTotal", "{0:c}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyResult">
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                                </div>    
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="HiddenStartDate" runat="server" />
            <asp:HiddenField ID="HiddenEndDate" runat="server" />
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="TopCustomerDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSalesByUser" 
        TypeName="CommerceBuilder.Reporting.ReportDataSource" SortParameterName="sortExpression" EnablePaging="true" 
        SelectCountMethod="GetSalesByUserCount">
        <SelectParameters>
            <asp:ControlParameter ControlID="HiddenStartDate" Name="startDate" PropertyName="Value" Type="DateTime" />
            <asp:ControlParameter ControlID="HiddenEndDate" Name="endDate" PropertyName="Value" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

