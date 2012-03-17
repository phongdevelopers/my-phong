<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="CouponUsage.aspx.cs" Inherits="Admin_Reports_CouponUsage" Title="Sales by Coupon"  %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            
            <div class="pageHeader">
                <div class="caption">
                     <h1>
                            <asp:Localize ID="Caption" runat="server" Text="Sales by Coupon"></asp:Localize>
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
                    </td>
                </tr>
               
                <tr>
                    <td class="dataSheet">
                        <cb:SortedGridView ID="CouponSalesGrid" runat="server" AutoGenerateColumns="False" DataSourceID="CouponUsageDs"
                            DefaultSortExpression="OrderTotal" DefaultSortDirection="Descending" AllowPaging="True" AllowSorting="true"
                            PageSize="40" OnSorting="CouponSalesGrid_Sorting" Width="100%" SkinID="Summary">
                            <Columns>
                                <asp:TemplateField HeaderText="Coupon" SortExpression="CouponCode">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="CouponLink" runat="server" Text='<%# Eval("CouponCode") %>' NavigateUrl='<%#Eval("Coupon.CouponId", "../Marketing/Coupons/EditCoupon.aspx?CouponId={0}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Count" SortExpression="OrderCount">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="OrderCount" runat="server" Text='<%# Eval("OrderCount") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Total" SortExpression="OrderTotal">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="OrderTotal" runat="server" Text='<%# Eval("OrderTotal", "{0:c}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Usage Details">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="UsageDetailLink" runat="server" Text="details" NavigateUrl='<%# string.Format("CouponUsageDetail.aspx?CouponCode={0}&StartDate={1}&EndDate={2}", Server.UrlEncode(Eval("CouponCode").ToString()),StartDate.SelectedDate.ToShortDateString(),EndDate.SelectedDate.ToShortDateString()) %>' />
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
    <asp:ObjectDataSource ID="CouponUsageDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSalesByCoupon" 
        TypeName="CommerceBuilder.Reporting.ReportDataSource" SortParameterName="sortExpression" EnablePaging="true" 
        SelectCountMethod="GetSalesByCouponCount">
        <SelectParameters>
            <asp:ControlParameter ControlID="HiddenStartDate" Name="startDate" PropertyName="Value" Type="DateTime" />
            <asp:ControlParameter ControlID="HiddenEndDate" Name="endDate" PropertyName="Value" Type="DateTime" />
            <asp:Parameter Name="couponCode" Type="String" DefaultValue="" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

