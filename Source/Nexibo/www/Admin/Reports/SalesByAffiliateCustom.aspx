<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="SalesByAffiliateCustom.aspx.cs" Inherits="Admin_Reports_SalesByAffiliateCustom" Title="Sales by Affiliate" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr class="noPrint">
                    <td align="center">
                        <table class="inputForm" width="100%">
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="DateFilterLabel" runat="server" Text="Dates: " SkinID="FieldHeader"></asp:Label> 
                                </th>
                                <td>
                                    <asp:DropDownList ID="DateFilter" runat="server" OnSelectedIndexChanged="DateFilter_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value=""></asp:ListItem>
                                        <asp:ListItem Value="0">Today</asp:ListItem>
                                        <asp:ListItem Value="1">This Week</asp:ListItem>
                                        <asp:ListItem Value="2">Last Week</asp:ListItem>
                                        <asp:ListItem Value="3">This Month</asp:ListItem>
                                        <asp:ListItem Value="4">Last Month</asp:ListItem>
                                        <asp:ListItem Value="5">Last 30 Days</asp:ListItem>
                                        <asp:ListItem Value="6">Last 60 Days</asp:ListItem>
                                        <asp:ListItem Value="7">Last 90 Days</asp:ListItem>
                                        <asp:ListItem Value="8">Last 120 Days</asp:ListItem>
                                        <asp:ListItem Value="9">This Year</asp:ListItem>
                                        <asp:ListItem Value="10">All Dates</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="StartDateLabel" runat="server" Text="Start:" ></asp:Label> 
                                </th>
                                <td>
                                    <uc1:PickerAndCalendar id="StartDate" runat="server"></uc1:PickerAndCalendar>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="EndDateLabel" runat="server" Text="End:" ></asp:Label> 
                                </th>
                                <td>
                                    <uc1:PickerAndCalendar id="EndDate" runat="server"></uc1:PickerAndCalendar>
                                </td>
                                <td>
                                    <asp:Button ID="ReportButton" runat="server" Text="Report" OnClick="ReportButton_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="pageHeader">
                        <h1><asp:Localize ID="Caption" runat="server" Text="Sales by Affiliate"></asp:Localize><asp:Localize ID="ReportFrom" runat="server" Text=" from {0:d}" Visible="false" EnableViewState="false"></asp:Localize><asp:Localize ID="ReportTo" runat="server" Text=" to {0}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet" align="center">
                        <cb:SortedGridView ID="AffiliateSalesGrid" runat="server" AutoGenerateColumns="False" DataSourceID="AffiliateSalesDs"
                            DefaultSortExpression="OrderTotal" DefaultSortDirection="Descending" AllowPaging="True" AllowSorting="true"
                            PageSize="40" OnSorting="AffiliateSalesGrid_Sorting" Width="100%" SkinID="Summary" OnDataBound="AffiliateSalesGrid_DataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="Affiliate">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="AffiliateLink" runat="server" Text='<%# Eval("Affiliate.Name") %>' NavigateUrl='<%#Eval("AffiliateId", "../Marketing/Affiliates/EditAffiliate.aspx?AffiliateId={0}")%>'></asp:HyperLink>
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
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("OrderTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="DetailLink" runat="server" Text="Detail" NavigateUrl='<%#GetDetailUrl(Container.DataItem)%>' SkinID="Link"></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyResult">
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                                </div>    
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                        <br /><i><asp:Label ID="ReportTimestamp" runat="server" Text="Report generated {0:MMM-dd-yyyy hh:mm tt}" EnableViewState="false"></asp:Label></i>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="AffiliateSalesDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSalesByAffiliate" 
        TypeName="CommerceBuilder.Reporting.ReportDataSource" SortParameterName="sortExpression" EnablePaging="true" 
        SelectCountMethod="GetSalesByAffiliateCount">
        <SelectParameters>
            <asp:ControlParameter ControlID="StartDate" Name="startDate" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="EndDate" Name="endDate" PropertyName="SelectedEndDate" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

