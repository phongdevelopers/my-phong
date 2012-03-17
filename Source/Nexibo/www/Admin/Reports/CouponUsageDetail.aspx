<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="CouponUsageDetail.aspx.cs" Inherits="Admin_Reports_CouponUsageDetail" Title="Sales by Coupon"  %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportFilterAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="innerLayout noPrint" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <table class="inputForm">
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="CouponLabel" runat="server" Text="Coupon Code:" AssociatedControlID="CouponList" ToolTip="Coupon to report on"></asp:Label> 
                                </th>
                                <td>
                                    <asp:DropDownList ID="CouponList" runat="server" AppendDataBoundItems="true">
                                        <asp:ListItem Text="All Coupons" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="DateFilterLabel" runat="server" Text="Time Period:" AssociatedControlID="DateFilter" ToolTip="Date range"></asp:Label> 
                                </th>
                                <td>
                                    <asp:DropDownList ID="DateFilter" runat="server" OnSelectedIndexChanged="DateFilter_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value=""></asp:ListItem>
                                        <asp:ListItem Value="TODAY">Today</asp:ListItem>
                                        <asp:ListItem Value="THISWEEK">This Week</asp:ListItem>
                                        <asp:ListItem Value="LASTWEEK">Last Week</asp:ListItem>
                                        <asp:ListItem Value="THISMONTH">This Month</asp:ListItem>
                                        <asp:ListItem Value="LASTMONTH">Last Month</asp:ListItem>
                                        <asp:ListItem Value="LAST30">Last 30 Days</asp:ListItem>
                                        <asp:ListItem Value="LAST60">Last 60 Days</asp:ListItem>
                                        <asp:ListItem Value="LAST90">Last 90 Days</asp:ListItem>
                                        <asp:ListItem Value="LAST120">Last 120 Days</asp:ListItem>
                                        <asp:ListItem Value="THISYEAR">This Year</asp:ListItem>
                                        <asp:ListItem Value="">All Dates</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="StartDateLabel" runat="server" Text="Start Date:" AssociatedControlID="StartDate" ToolTip="Start date for report"></asp:Label> 
                                </th>
                                <td>
                                    <uc1:PickerAndCalendar id="StartDate" runat="server"></uc1:PickerAndCalendar>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="EndDateLabel" runat="server" Text="End Date:" AssociatedControlID="EndDate" ToolTip="End date for report"></asp:Label> 
                                </th>
                                <td>
                                    <uc1:PickerAndCalendar id="EndDate" runat="server"></uc1:PickerAndCalendar>
                                </td>
                                <td>
                                    <asp:Button ID="ReportButton" runat="server" Text="Report" OnClick="ReportButton_Click" ToolTip="Generate Report" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
            	<div class="caption">
            		<h1>
                            <asp:Localize ID="Caption" runat="server" Text="Sales by Coupon Code"></asp:Localize>
                            <asp:Localize ID="ReportFromDate" runat="server" Text=" from {0:d}" EnableViewState="false" Visible="false"></asp:Localize>
                            <asp:Localize ID="ReportToDate" runat="server" Text=" to {0:d}" EnableViewState="false" Visible="false"></asp:Localize>
                        </h1>
            	</div>
            </div>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <asp:Repeater ID="CouponSalesRepeater" runat="server" DataSourceID="CouponSalesDs" Visible="false">
                    <ItemTemplate>
                        <tr class="sectionHeader">
                            <th>
                                <asp:Localize ID="CouponName" runat="server" Text='<%# Eval("CouponCode", "Orders that used {0}") %>'></asp:Localize>
                            </th>
                        </tr>
                       <tr>
                            <td>
                                <asp:GridView ID="OrdersGrid" runat="server" AllowPaging="False" AllowSorting="False" 
                                    AutoGenerateColumns="False" DataKeyNames="OrderId" SkinID="PagedList" Width="100%"
                                    DataSource='<%# GetCouponOrders(Container.DataItem) %>'>
                                    <Columns>
                                        <asp:TemplateField HeaderText="Order #" SortExpression="OrderId">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>' NavigateUrl='<%#String.Format("../Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId")) %>' SkinId="Link"></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date" SortExpression="OrderDate">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="OrderDate" runat="server" Text='<%# Eval("OrderDate", "{0:d}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Products" SortExpression="ProductSubtotal">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="Subtotal" runat="server" Text='<%# Eval("ProductSubtotal", "{0:lc}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" SortExpression="TotalCharges">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="Total" runat="server" Text='<%# Eval("TotalCharges", "{0:lc}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no associated orders for this month."></asp:Label>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <SeparatorTemplate><tr><td>&nbsp;</td></tr></SeparatorTemplate>
                </asp:Repeater>
            </table>
            <div align="center">
                <br /><i><asp:Label ID="ReportTimestamp" runat="server" Text="Report generated {0:MMM-dd-yyyy hh:mm tt}" EnableViewState="false"></asp:Label></i>
                <div class="noPrint">
                    <asp:HyperLink ID="SummaryLink" runat="server" Text="Go to Summary Report" NavigateUrl="CouponUsage.aspx" EnableViewState="false"></asp:HyperLink><br /><br />
                </div>
            </div>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="CouponSalesDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSalesByCoupon" 
        TypeName="CommerceBuilder.Reporting.ReportDataSource">
        <SelectParameters>
            <asp:ControlParameter ControlID="StartDate" Name="startDate" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="EndDate" Name="endDate" PropertyName="SelectedEndDate" Type="DateTime" />
            <asp:ControlParameter ControlID="CouponList" Name="couponCode" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>