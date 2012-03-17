<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="SalesByAffiliateDetail.aspx.cs" Inherits="Admin_Reports_SalesByAffiliateDetail" Title="Sales by Affiliate" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

			<div class="pageHeader">
                <div class="caption">
                     <h1><asp:Localize ID="Caption" runat="server" Text="Sales by Affiliate"></asp:Localize><asp:Localize ID="ReportDateCaption" runat="server" Text=" for {0:MMMM yyyy}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>


            <!-- <table class="innerLayout noPrint" cellpadding="0" cellspacing="0"> -->
			<table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
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
                        &nbsp;
                        <asp:Label ID="AffiliateListLabel" runat="server" Text="Affiliate: " SkinID="FieldHeader"></asp:Label>
                        <asp:DropDownList ID="AffiliateList" runat="server" DataTextField="Name" DataValueField="AffiliateId" AppendDataBoundItems="True" AutoPostBack="true" OnSelectedIndexChanged="DateFilter_SelectedIndexChanged">
                            <asp:ListItem Text="All Affiliates" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                        <br />
                    </td>
                </tr>
            </table>
            <asp:Repeater ID="AffiliateSalesRepeater" runat="server" DataSourceID="AffiliateSalesDs" Visible="false">
                <ItemTemplate>
                    <table align="center" class="form<%# (Container.ItemIndex < (AffiliateCount - 1)) ? " breakAfter" : string.Empty %>" cellpadding="0" cellspacing="0" border="0">
                        <tr class="sectionHeader">
                            <th colspan="3" class="header">
                                <asp:Localize ID="AffiliateName" runat="server" Text='<%# Eval("AffiliateName") %>'></asp:Localize>
                                <asp:Localize ID="SalesForText" runat="server" Text=" Sales for "></asp:Localize>
                                <asp:Localize ID="ReportDate" runat="server" Text='<%# Eval("StartDate", "{0:MMMM yyyy}") %>'></asp:Localize>
                            </th>
                        </tr>
                        <tr>
                            <td valign="top" width="30%">
                                <asp:Label ID="ReferralCountLabel" runat="server" Text="Referrals:" SkinID="fieldheader"></asp:Label>
                                <asp:Label ID="ReferralCount" runat="server" Text='<%#Eval("ReferralCount") %>'></asp:Label><br />
                                <asp:Label ID="ConversionRateLabel" runat="server" Text="Conversion Rate:" SkinID="FieldHeader"></asp:Label>
                                <asp:Label ID="ConversionRate" runat="server" Text='<%#GetConversionRate(Container.DataItem)%>'></asp:Label><br />
                            </td>
                            <td valign="top" width="30%">
                                <asp:Label ID="OrderCountLabel" runat="server" Text="Orders:" SkinID="fieldheader"></asp:Label>
                                <asp:Label ID="OrderCount" runat="server" Text='<%#Eval("OrderCount") %>'></asp:Label><br />
                                <asp:Label ID="ProductSubtotalLabel" runat="server" Text="Products:" SkinID="FieldHeader"></asp:Label>
                                <asp:Label ID="ProductSubtotal" runat="server" Text='<%#Eval("ProductSubtotal", "{0:lc}") %>'></asp:Label><br />
                                <asp:Label ID="OrderTotalLabel" runat="server" Text="Total:" SkinID="FieldHeader"></asp:Label>
                                <asp:Label ID="OrderTotal" runat="server" Text='<%#Eval("OrderTotal", "{0:lc}") %>'></asp:Label><br />
                            </td>
                            <td valign="top" width="40%">
                                <asp:Label ID="CommissionRateLabel" runat="server" Text="Rate:" SkinID="FieldHeader"></asp:Label>
                                <asp:Label ID="CommissionRate" runat="server" Text='<%# GetCommissionRate(Container.DataItem) %>'></asp:Label><br />
                                <asp:Label ID="CommissionLabel" runat="server" Text="Commission:" SkinID="FieldHeader"></asp:Label>
                                <asp:Label ID="Commission" runat="server" Text='<%# Eval("Commission", "{0:lc}") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:GridView ID="OrdersGrid" runat="server" AllowPaging="False" AllowSorting="False" 
                                    AutoGenerateColumns="False" DataKeyNames="OrderId" SkinID="PagedList" Width="100%"
                                    DataSource='<%# GetAffiliateOrders(Container.DataItem) %>'>
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
                    </table>
                </ItemTemplate>
            </asp:Repeater>
            <div align="center">
                <br /><i><asp:Label ID="ReportTimestamp" runat="server" Text="Report generated {0:MMM-dd-yyyy hh:mm tt}" EnableViewState="false"></asp:Label></i>
                <div class="noPrint">
                    <asp:HyperLink ID="SummaryLink" runat="server" Text="Go to Summary Report" NavigateUrl="SalesByAffiliate.aspx?ReportDate={0:MMddyyyy}" SkinID="Link" EnableViewState="false"></asp:HyperLink>
                </div>
            </div>
            <asp:HiddenField ID="HiddenStartDate" runat="server" Value="" />
            <asp:HiddenField ID="HiddenEndDate" runat="server" Value="" />
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="AffiliateSalesDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSalesByAffiliate" 
        TypeName="CommerceBuilder.Reporting.ReportDataSource">
        <SelectParameters>
            <asp:ControlParameter ControlID="HiddenStartDate" Name="startDate" PropertyName="Value" Type="DateTime" />
            <asp:ControlParameter ControlID="HiddenEndDate" Name="endDate" PropertyName="Value" Type="DateTime" />
            <asp:ControlParameter ControlID="AffiliateList" Name="affiliateId" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>