<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="PackingList.aspx.cs" Inherits="Admin_Orders_Shipments_PackingList" Title="Packing Slip" %>
<%@ Register Src="../Print/OrderItemDetail.ascx" TagName="OrderItemDetail" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">    
<div class="noPrint">
    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Label ID="Caption" runat="server" Text="Packing List - Order Number {0}"></asp:Label>
            </h1>            
        </div>
    </div>     
    <div style="text-align:center">    
        <br />
        <asp:Button ID="PrintButton" runat="server" Text="Print" OnClientClick="window.print();return false;" />
        <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />
        <br />         
    </div>    
</div>
<table cellpadding="4" cellspacing="0" width="500" align="center" style="background-color:White;border:solid 1px"  class="form" >    
    <tr>
        <th colspan="2">
            <asp:Label ID="PrintCaption" runat="server" Text="Packing List - Order Number {0}"></asp:Label>
        </th>
    </tr>
    <tr>
        <td colspan="2">
            <br />
            <table class="inputForm" width="100%" cellspacing="5" >
                <tr>                
                    <th align="left" width="140" >
                        <asp:Label ID="OrderDateLabel" runat="server" Text="Order Date:"></asp:Label>
                    </th>
                    <td align="left">
                        <asp:Label ID="OrderDate" runat="server" Text=""></asp:Label>            
                    </td>
                </tr>
                <tr>
                    <th align="left" >
                        <asp:Label ID="ShipmentNumberLabel" runat="server" Text="Shipment Number:"></asp:Label>
                    </th>
                    <td align="left">
                        <asp:Label ID="ShipmentNumber" runat="server" Text="{0} of {1}"></asp:Label>            
                    </td>
                </tr>
                <tr>
                    <th align="left" >
                        <asp:Label ID="ShipmentWeightLabel" runat="server" Text="Shipment Weight:"></asp:Label>
                    </th>
                    <td align="left">
                        <asp:Label ID="ShipmentWeight" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th align="left" >
                        <asp:Label ID="ShippingMethodLabel" runat="server" Text="Shipping Method:"></asp:Label>            
                    </th>
                    <td align="left">
                        <asp:Label ID="ShippingMethod" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th align="left" >
                        <asp:Label ID="AddressTypeLabel" runat="server" Text="Address Type:"></asp:Label>            
                    </th>
                    <td align="left">
                        <asp:Label ID="AddressType" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                  <tr>
                    <th align="left" >
                        <asp:Label ID="ShipMessageLabel" runat="server" Text="Customer Comment:"></asp:Label>            
                    </th>
                    <td align="left">
                        <asp:Label ID="ShipMessage" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <th class="columnHeader" width="50%" style="text-align:left;border-top:solid 1px black;border-right:solid 1px black;border-bottom:solid 1px black;">
            <asp:Label ID="ShipFromCaption" runat="server" Text="Ship From:"></asp:Label>
        </th>
        <th class="columnHeader" width="50%" style="text-align:left;border-top:solid 1px black;border-bottom:solid 1px black;">
            <asp:Label ID="ShipToCaption" runat="server" Text="Ship To:"></asp:Label>
        </th>
    </tr>
    <tr>
        <td nowrap style="border-right:solid 1px black;border-bottom:solid 1px black;">
            <asp:Label ID="ShipFrom" runat="server" Text=""></asp:Label>
        </td>
        <td nowrap style="border-bottom:solid 1px black;">
            <asp:Label ID="ShipTo" runat="server" Text="" ></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="padding:0px;" class="dataSheet">
            <asp:GridView ID="NonShippingItemsGrid" GridLines="none" runat="server" ShowHeader="true" 
                AutoGenerateColumns="false"  Width="100%" SkinID="ItemList" HeaderStyle-BackColor="lightgray" CellSpacing="1">
                <Columns>
                    <asp:TemplateField HeaderText="Qty">
                        <HeaderStyle Width="50" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="center" />
                        <ItemTemplate>
                            <asp:Label ID="Quantity" runat="server" Text='<%#Eval("Quantity")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="SKU">
                        <HeaderStyle Width="80" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="center" />
                        <ItemTemplate>
                            <asp:Literal ID="Sku" runat="server" Text='<%# GetSku(Container.DataItem) %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
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
</asp:Content>
