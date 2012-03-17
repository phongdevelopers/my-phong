<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="PackSlips.aspx.cs" Inherits="Admin_Orders_Print_PackSlips" Title="Packing Slips"%>
<%@ Register Src="OrderItemDetail.ascx" TagName="OrderItemDetail" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader noPrint">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Packing Lists"></asp:Localize></h1>
        </div>
        <div class="content">
            <h2><asp:Localize ID="OrderListLabel" runat="server" Text="Includes Order Numbers:"></asp:Localize></h2>
            <asp:Label ID="OrderList" runat="server" Text=""></asp:Label><br />
            <p><asp:Localize ID="PrintInstructions" runat="server" Text="This document includes a printable stylesheet. The latest versions of IE and Firefox browsers will print with appropriate styles and page breaks if needed. Website headers, footers, and this message will not be printed."></asp:Localize></p>
        </div>
    </div>
    <div class="noPrint">
        <asp:Button ID="Print" runat="server" Text="Print" OnClientClick="window.print();return false;" />
        <asp:Button ID="Back" runat="server" Text="Back" OnClientClick="window.history.go(-1);return false;" />
    </div>
    <asp:Repeater ID="ShipmentRepeater" runat="server" OnItemDataBound="ShipmentRepeater_ItemDataBound">
        <ItemTemplate>
            <table align="center" class="form<%# (Container.ItemIndex < (ShipmentCount - 1)) ? " breakAfter" : string.Empty %>" cellpadding="0" cellspacing="0" border="1">
                <tr>
                    <th colspan="3" class="header">
                        <asp:Label ID="ShipmentLabel" runat="server" Text="Packing List: Order #{0}" CssClass="caption"></asp:Label>
                        <asp:Label ID="ShipmentCountLabel" runat="server" Text=" (Shipment {0} of {1})" CssClass="caption"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label ID="OrderDateLabel" runat="server" Text="Order Date:" SkinID="fieldheader"></asp:Label>
                        <asp:Label ID="OrderDate" runat="server" Text='<%#Eval("Order.OrderDate", "{0:g}") %>'></asp:Label><br />
                        <asp:Label ID="ShippingMethodLabel" runat="server" Text="Shipping Method:" SkinID="fieldheader"></asp:Label>
                        <asp:Label ID="ShippingMethod" runat="server" Text='<%#Eval("ShipMethodName") %>'></asp:Label>
                    </td>
                    <td valign="top">
                        <asp:Label ID="ShipFromCaption" runat="server" Text="Ship From:" SkinID="FieldHeader"></asp:Label><br />
                        <asp:Label ID="ShipFrom" runat="server" Text='<%#((OrderShipment)Container.DataItem).FormatFromAddress()%>'></asp:Label>
                    </td>
                    <td valign="top">
                        <asp:Label ID="ShipToCaption" runat="server" Text="Ship To:" SkinID="FieldHeader"></asp:Label><br />
                        <asp:Label ID="ShipTo" runat="server" Text='<%#((OrderShipment)Container.DataItem).FormatToAddress()%>'></asp:Label>
                    </td>
                </tr>
                <tr id="trShipMessage" runat="server">
                    <td colspan="3">
                        <asp:Label ID="ShipMessageLabel" runat="server" Text="Customer Comment:"></asp:Label>
                        <asp:Label ID="ShipMessage" runat="server" Text='<%#Eval("ShipMessage")%>'></asp:Label><br /><br />
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="dataSheet">
                        <asp:GridView ID="ShipmentItems" runat="server" ShowHeader="true" 
                            AutoGenerateColumns="false" CellPadding=0 CellSpacing=0 GridLines="none" 
                            Width="100%" DataSource='<%#GetProducts(Container.DataItem)%>' CssClass="dataSheet">
                            <Columns>
                                <asp:BoundField DataField="Quantity" HeaderText="Quantity" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="Sku" HeaderText="Sku" ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Item">
                                    <ItemTemplate>
                                        <uc:OrderItemDetail ID="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="False" LinkProducts="False" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>