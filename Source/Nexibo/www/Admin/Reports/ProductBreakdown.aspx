<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" Title="Product Breakdown" CodeFile="ProductBreakdown.aspx.cs" Inherits="Admin_Reports_ProductBreakdown"%>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="Caption" runat="server" Text="Product Sales Breakdown"></asp:Localize><asp:Localize
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
                <tr valign="top">
                    <td>
                        <div style="text-align: right; vertical-align: middle">
                            <asp:Label ID="LabelVendor" runat="server" Text="Vendor:  " SkinID="FieldHeader"></asp:Label>
                        </div>
                    </td>
                    <td>
                        <div class="noPrint" style="text-align: left; padding-left: 12px;">
                            <asp:DropDownList ID="VendorList" runat="server">
                            </asp:DropDownList>&nbsp;</div>
                    </td>
                    <td>
                        <div style="text-align: right; vertical-align: middle">
                            <asp:Label ID="Label8" runat="server" Text="Sort By:  " SkinID="FieldHeader"></asp:Label>
                        </div>
                    </td>
                    <td>
                        <div class="noPrint" style="text-align: left; padding-left: 12px;">
                            <asp:DropDownList ID="SortByList" runat="server">
                                <asp:ListItem Selected="True">Quantity</asp:ListItem>
                                <asp:ListItem>Amount</asp:ListItem>
                            </asp:DropDownList>
                        </div>
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
                <tr>
                    <td class="dataSheet" colspan="4">
                        <asp:GridView ID="ProductBreakdownGrid" runat="server" AutoGenerateColumns="False"
                            AllowPaging="true" PageSize="20" Width="100%" SkinID="PagedList" OnPageIndexChanging="ProductBreakdownGrid_PageIndexChanging">
                            <Columns>
                                <asp:TemplateField HeaderText="Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>                                        
                                        <asp:HyperLink ID="ProductLink" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%#String.Format("~/Admin/Products/EditProduct.aspx?ProductId={0}",Eval("ProductId"))%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sku">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label0" runat="server" Text='<%# Eval("Sku", "{0}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Vendor">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("Vendor", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Quantity">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="QuantityLabel" runat="server" Text='<%# Eval("Quantity") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Amount">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="AmountLabel" runat="server" Text='<%# string.Format("{0:lc}", Eval("Amount")) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyResult">
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no products to report for the selected dates."></asp:Label>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;" colspan="4">
                        <div>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="LblTotalQuantity" runat="server" SkinID="FieldHeader" Text="Total Quantity: "
                                            Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="TotalQuantity" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="LblTotalAmount" runat="server" SkinID="FieldHeader" Text="Total Amount: "
                                            Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="TotalAmount" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
