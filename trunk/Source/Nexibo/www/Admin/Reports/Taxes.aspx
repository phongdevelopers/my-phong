<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Taxes.aspx.cs" Inherits="Admin_Reports_Taxes" Title="Tax Summary Report" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1> <asp:Localize ID="Localize1" runat="server" Text="Tax Summary Report"></asp:Localize><asp:Localize
                    ID="ReportCaption" runat="server" EnableViewState="false" Text=" {0:d} to {1:d}"
                    Visible="false"></asp:Localize></h1>
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
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet">                       
                        <cb:SortedGridView ID="TaxesGrid" runat="server" AutoGenerateColumns="False" Width="100%"
                            AllowPaging="True" PageSize="20" AllowSorting="true" DefaultSortDirection="ascending" 
                            DefaultSortExpression="TaxName" SkinID="Summary" DataSourceId="TaxesDs">
                            <Columns>
                                <asp:TemplateField HeaderText="Tax Name" SortExpression="TaxName">
                                    <headerstyle horizontalalign="Left" />
                                    <itemtemplate>
                                        <asp:HyperLink ID="TaxName" runat="server" Text='<%# Eval("TaxName") %>' NavigateUrl='<%#GetTaxLink(Container.DataItem)%>'></asp:HyperLink>
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
            <asp:HiddenField ID="HiddenStartDate" runat="server" />
            <asp:HiddenField ID="HiddenEndDate" runat="server" />
            <asp:ObjectDataSource ID="TaxesDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadSummaries" 
                SortParameterName="sortExpression" EnablePaging="true" TypeName="CommerceBuilder.Reporting.TaxReportDataSource"
                SelectCountMethod="CountSummaries" DataObjectTypeName="CommerceBuilder.Reporting.TaxReportSummaryItem" EnableViewState="false">
                <SelectParameters>
                    <asp:ControlParameter ControlID="HiddenStartDate" Name="startDate" PropertyName="Value" Type="DateTime" />
                    <asp:ControlParameter ControlID="HiddenEndDate" Name="endDate" PropertyName="Value" Type="DateTime" />
                </SelectParameters>                
            </asp:ObjectDataSource>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
