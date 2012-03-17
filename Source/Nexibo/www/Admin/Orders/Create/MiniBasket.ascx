<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MiniBasket.ascx.cs" Inherits="Admin_Orders_Create_MiniBasket" %>
<%@ Register Src="~/Admin/Orders/Create/MiniBasketItemDetail.ascx" TagName="MiniBasketItemDetail" TagPrefix="uc1" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<ajax:UpdatePanel ID="BasketPanel" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:Panel ID="MiniBasketHolder" runat="server">
        <div class="section">
            <div class="header">
                <h2 class="commonicon"><asp:localize ID="Caption" runat="server" Text="Order Contents"></asp:localize></h2>
            </div>
            <asp:Panel ID="ContentPanel" runat="server" CssClass="content">
                <asp:PlaceHolder ID="BasketTable" runat="server">
                    <table width="100%">
                        <asp:Repeater ID="BasketRepeater" runat="server">
                            <HeaderTemplate>
                                <tr>
                                    <th>Qty</th>
                                    <th align="left">Item</th>
                                    <th align="right">Price</th>
                                </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td align="center" valign="top">
                                        <%# Eval("Quantity") %>
                                    </td>
                                    <td valign="top">
                                        <uc1:MiniBasketItemDetail ID="BasketItemDetail1" runat="server" BasketItem='<%#(BasketItem)Container.DataItem%>' LinkProducts="true" ShowAssets="false" ShowSubscription="false" />
                                    </td>
                                    <td align="right" valign="top" width="60px">
                                        <%#GetItemShopPrice((BasketItem)Container.DataItem).ToString("ulc")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr id="trDiscounts" runat="server" enableviewstate="false" visible="false">
                            <th align="right" colspan="2">
                                <asp:Localize ID="DiscountsLabel" runat="server" Text="Discounts:"></asp:Localize>
                            </th>
                            <td align="right" width="60px">
                                <asp:Literal ID="Discounts" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr id="trCoupons" runat="server" enableviewstate="false" visible="false">
                            <th align="right" colspan="2">
                                <asp:Localize ID="CouponsLabel" runat="server" Text="Coupons:"></asp:Localize>
                            </th>
                            <td align="right" width="60px">
                                <asp:Literal ID="Coupons" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr id="trShipping" runat="server" enableviewstate="false" visible="false">
                            <th align="right" colspan="2">
                                <asp:Localize ID="ShippingLabel" runat="server" Text="Shipping:"></asp:Localize>
                            </th>
                            <td align="right" width="60px">
                                <asp:Literal ID="Shipping" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr id="trTaxes" runat="server" enableviewstate="false" visible="false">
                            <th align="right" colspan="2">
                                <asp:Localize ID="TaxesLabel" runat="server" Text="Taxes:"></asp:Localize>
                            </th>
                            <td align="right" width="60px">
                                <asp:Literal ID="Taxes" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr id="trOther" runat="server" enableviewstate="false" visible="false">
                            <th align="right" colspan="2">
                                <asp:Localize ID="OtherLabel" runat="server" Text="Other:"></asp:Localize>
                            </th>
                            <td align="right" width="60px">
                                <asp:Literal ID="Other" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <th align="right" colspan="2">
                                <asp:Localize ID="TotalLabel" runat="server" Text="Total:"></asp:Localize>
                            </th>
                            <td align="right" width="60px">
                                <asp:Literal ID="Total" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        
                    </table>
                    <asp:HyperLink ID="EditOrderLink" runat="server" Text="Edit Order" NavigateUrl="CreateOrder2.aspx" SkinID="Button" EnableViewState="false"></asp:HyperLink>
                </asp:PlaceHolder>
                <asp:Panel ID="EmptyBasketPanel" runat="server" CssClass="emptyBasketDialogPanel" Visible="false">
                    <asp:Label ID="EmptyBasketMessage" runat="Server" Text="Empty" CssClass="message"></asp:Label>
                </asp:Panel>
            </asp:Panel>
        </div>
        </asp:Panel>
    </ContentTemplate>
</ajax:UpdatePanel>
