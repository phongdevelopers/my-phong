<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="TaxDetail.aspx.cs" Inherits="Admin_Reports_TaxDetail" Title="Tax Detail Report" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="ReportCaption" runat="server" Text="Tax Detail Report for {0}" EnableViewState="false"></asp:Localize><asp:Localize ID="ReportCaptionDateRange" runat="server" Text=": {0:d} to {1:d}" Visible="false" EnableViewState="false"></asp:Localize>
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
                        <asp:Label ID="TaxNameLabel" runat="server" Text="Tax Name: " SkinID="FieldHeader"></asp:Label>
                        <asp:HyperLink ID="TaxNameLink" runat="server"></asp:HyperLink>
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet">                       
                        <cb:SortedGridView ID="TaxesGrid" runat="server" AutoGenerateColumns="False" Width="100%"
                            AllowPaging="True" PageSize="20" AllowSorting="true" DefaultSortDirection="Ascending" 
                            DefaultSortExpression="O.OrderId" SkinID="Summary" DataSourceId="TaxesDs">
                            <Columns>
                                <asp:TemplateField HeaderText="Order" SortExpression="OrderNumber">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:HyperLink ID="OrderLink" runat="server" Text='<%# Eval("OrderNumber") %>' NavigateUrl='<%# GetOrderLink(Container.DataItem) %>'></asp:HyperLink>
                                        
                                    </itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Date" SortExpression="OrderDate">
                                    <headerstyle horizontalalign="Left" />
                                    <itemtemplate>
                                        <%# Eval("OrderDate") %>
                                    </itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total Collected" SortExpression="TaxAmount">
                                    <headerstyle horizontalalign="right" />
                                    <itemstyle horizontalalign="Right" />
                                    <itemtemplate>
                                        <%# Eval("TaxAmount", "{0:lc}") %>
                                    </itemtemplate>
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
            <asp:HiddenField ID="HiddenTaxName" runat="server" Value="" />
            <asp:ObjectDataSource ID="TaxesDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadDetail" 
                SortParameterName="sortExpression" EnablePaging="true" TypeName="CommerceBuilder.Reporting.TaxReportDataSource"
                SelectCountMethod="CountDetail" DataObjectTypeName="CommerceBuilder.Reporting.TaxReportDetailItem" EnableViewState="false">
                <SelectParameters>
                    <asp:ControlParameter ControlID="HiddenTaxName" Name="taxName" PropertyName="Value" Type="String" />
                    <asp:ControlParameter ControlID="StartDate" Name="startDate" PropertyName="SelectedStartDate" Type="DateTime" />
                    <asp:ControlParameter ControlID="EndDate" Name="endDate" PropertyName="SelectedEndDate" Type="DateTime" />
                </SelectParameters>                
            </asp:ObjectDataSource>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>