<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="SalesSummary.aspx.cs" Inherits="Admin_Reports_SalesSummary" Title="Sales Summary" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="Caption" runat="server" Text="Sales Summary"></asp:Localize><asp:Localize
                            ID="ReportCaption" runat="server" Visible="false" EnableViewState="false"></asp:Localize>
                   </h1>
                </div>
            </div>
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
                        <uc1:PickerAndCalendar ID="PickerAndCalendar1" runat="server" />
                    </td>
                    <td>
                        <div style="text-align: right; vertical-align: middle">
                            <asp:Label ID="Label4" runat="server" Text="To:  " SkinID="FieldHeader"></asp:Label>
                        </div>
                    </td>
                    <td style="text-align: left">
                        <uc1:PickerAndCalendar ID="PickerAndCalendar2" runat="server" />
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
                <tr id="trResults" runat="server" visible="false">
                    <td class="dataSheet" colspan="4">
                        <table cellpadding="0" cellspacing="0" border="0" width="100%" class="summary">
                            <tr>
                                <th class="sectionHeader" colspan="2">
                                    <div style="text-align: left">
                                        Sales Summary</div>
                                </th>
                            </tr>
                            <tr class="oddRow">
                                <td class="rowHeader" width="50%"></strong>Product Sales:</td>
                                <td align="right"><asp:Label ID="ProductSales" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="evenRow">
                                <td class="rowHeader">Discounts:</td>
                                <td align="right"><asp:Label ID="ProductDiscounts" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="oddRow">
                                <td class="rowHeader">Product Sales (Less Discounts):</td>
                                <td align="right"><asp:Label ID="ProductSalesLessDiscounts" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr class="evenRow">
                                <td class="rowHeader" >Gift Wrap Charges:</td>
                                <td align="right"><asp:Label ID="GiftWrapCharges" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="oddRow">
                                <td class="rowHeader" >Coupons Redeemed:</td>
                                <td align="right"><asp:Label ID="CouponsRedeemed" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="evenRow">
                                <td class="rowHeader" >Tax Charges:</td>
                                <td align="right"><asp:Label ID="TaxesCollected" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="oddRow">
                                <td class="rowHeader" >Shipping Charges:</td>
                                <td align="right"><asp:Label ID="ShippingCharges" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="evenRow">
                                <td class="rowHeader" >Total Charges:</td>
                                <td align="right"><asp:Label ID="TotalCharges" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr class="oddRow">
                                <td class="rowHeader" >Total Number of Orders:</td>
                                <td align="right"><asp:Label ID="TotalOrders" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="evenRow">
                                <td class="rowHeader" >Total Items Sold:</td>
                                <td align="right"><asp:Label ID="TotalItemsSold" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="oddRow">
                                <td class="rowHeader" >Number of Customers:</td>
                                <td align="right"><asp:Label ID="NumberOfCustomers" runat="server" EnableViewState="false" /></td>
                            </tr>
                            <tr class="evenRow">
                                <td class="rowHeader" >Average Order Amount:</td>
                                <td align="right"><asp:Label ID="AverageOrderAmount" runat="server" EnableViewState="false" /></td>
                            </tr>
                        </table>                            
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

