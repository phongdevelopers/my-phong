<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="SalesByReferrer.aspx.cs" Inherits="Admin_Reports_SalesByReferrer" Title="Sales by Referrer"  %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

		<div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="Caption" runat="server" Text="Sales by Referrer"></asp:Localize><asp:Localize ID="ReportDateCaption" runat="server" Text=" for {0:MMMM yyyy}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>



            <!--- <table cellpadding="2" cellspacing="0" class="innerLayout"> -->
            <br />
			<table align="center" cellpadding="0" cellspacing="0" border="0" width="90%">
                <tr class="noPrint">
                    <td align="center">
                        <asp:Button ID="PreviousButton" runat="server" Text="&laquo; Prev" OnClick="PreviousButton_Click" />&nbsp;<asp:Label ID="MonthLabel" runat="server" Text="Month: " SkinID="FieldHeader"></asp:Label>
                        <asp:DropDownList ID="MonthList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DateFilter_SelectedIndexChanged">
                            <asp:ListItem Value="1" Text="January"></asp:ListItem>
                            <asp:ListItem Value="2" Text="February"></asp:ListItem>
                            <asp:ListItem Value="3" Text="March"></asp:ListItem>
                            <asp:ListItem Value="4" Text="April"></asp:ListItem>
                            <asp:ListItem Value="5" Text="May"></asp:ListItem>
                            <asp:ListItem Value="6" Text="June"></asp:ListItem>
                            <asp:ListItem Value="7" Text="July"></asp:ListItem>
                            <asp:ListItem Value="8" Text="August"></asp:ListItem>
                            <asp:ListItem Value="9" Text="September"></asp:ListItem>
                            <asp:ListItem Value="10" Text="October"></asp:ListItem>
                            <asp:ListItem Value="11" Text="November"></asp:ListItem>
                            <asp:ListItem Value="12" Text="December"></asp:ListItem>
                        </asp:DropDownList>&nbsp;
                        <asp:Label ID="YearLabel" runat="server" Text="Year: " SkinID="FieldHeader"></asp:Label>
                        <asp:DropDownList ID="YearList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DateFilter_SelectedIndexChanged">
                        </asp:DropDownList>
                        &nbsp;
                        <asp:Button ID="NextButton" runat="server" Text="Next &raquo;" OnClick="NextButton_Click" />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet" >
                        <cb:SortedGridView ID="ReferrerSalesGrid" runat="server" AutoGenerateColumns="False"  DataSourceID="ReferrerSalesDs"
                            AllowPaging="True" PageSize="20"   SkinID="PagedList" AllowSorting="True" DefaultSortExpression="OrderCount" DefaultSortDirection="Descending" Visible="false" Width="100%" >
                            <Columns>
                                <asp:TemplateField HeaderText="Referrer" SortExpression="Referrer">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="ReferrerName" runat="server" Text='<%# ((String)Eval("Referrer")).Replace("/", "/<wbr />") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>                                                                
                                <asp:TemplateField HeaderText="Order Count" SortExpression="OrderCount">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle HorizontalAlign="Center"  />
                                    <ItemTemplate>
                                        <asp:Label ID="OrderCount" runat="server" Text='<%# Eval("OrderCount") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Products&nbsp;Total" SortExpression="ProductSubtotal">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="ProductSubtotal" runat="server" Text='<%# Eval("ProductSubtotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Orders&nbsp;Total" SortExpression="OrderTotal">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />                                    
                                    <ItemTemplate>
                                        <asp:Label ID="OrderTotal" runat="server" Text='<%# Eval("SalesTotal", "{0:lc}")  %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>                                                               
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyResult">
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                                </div>    
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                        <br /><i><asp:Label ID="ReportTimestamp" runat="server" Text="Report generated {0:MMM-dd-yyyy hh:mm tt}" EnableViewState="false" Visible="false"></asp:Label></i>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="HiddenStartDate" runat="server" Value="" />
            <asp:HiddenField ID="HiddenEndDate" runat="server" Value="" />
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="ReferrerSalesDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSalesByReferrer" 
        TypeName="CommerceBuilder.Reporting.ReportDataSource" SortParameterName="sortExpression" EnablePaging="true">
        <SelectParameters>
            <asp:ControlParameter ControlID="HiddenStartDate" Name="startDate" PropertyName="Value" Type="DateTime" />
            <asp:ControlParameter ControlID="HiddenEndDate" Name="endDate" PropertyName="Value" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

