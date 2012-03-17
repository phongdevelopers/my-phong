<%@ Page Language="C#" MasterPageFile="../Order.master" CodeFile="ShipOrder.aspx.cs" Inherits="Admin_Orders_Shipments_ShipOrder" Title="Ship Order" EnableViewState="false" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Ship Order"></asp:Localize></h1>
        </div>
    </div>            
    <div style="clear: both;">
        <table cellpadding="0" cellspacing="5" width="100%">
            <tr id="trShipmentNumber" runat="server" visible="false">
                <th colspan="2" style="text-align:left;padding-left:4px;" class="rowHeader">
                    <asp:Label ID="ShipmentNumber" runat="server" Text="Shipment {0} of {1}:"></asp:Label>
                </th>
            </tr>
            <tr>
                <td align="left" style="width:33%" valign="top">
                    <asp:Label ID="OrderIdLabel" runat="server" Text="Order Number:" SkinID="FieldHeader"></asp:Label>
                    <asp:Label ID="OrderId" runat="server" Text=""></asp:Label><br />
                    <asp:Label ID="OrderDateLabel" runat="server" Text="Order Date:" SkinID="FieldHeader"></asp:Label>
                    <asp:Label ID="OrderDate" runat="server" Text=""></asp:Label><br />
                    <asp:Label ID="ShippingMethodLabel" runat="server" Text="Shipping Method:" SkinID="FieldHeader"></asp:Label>
                    <asp:Label ID="ShippingMethod" runat="server" Text=""></asp:Label><br />
                    <asp:Panel ID="ShipMessagePanel" runat="server" Visible="false">
                        <asp:Label ID="ShipMessageLabel" runat="server" Text="Customer Message:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="ShipMessage" runat="server" Text=""></asp:Label><br />
                    </asp:Panel>
                </td>
                <td width="33%" valign="top">
                    <asp:Label ID="ShipToCaption" runat="server" Text="Ship To:" SkinID="FieldHeader"></asp:Label><br />
                    <asp:Label ID="ShipTo" runat="server" Text=""></asp:Label>
                </td>
                <td width="34%" valign="top">
                    <asp:Label ID="ShipFromCaption" runat="server" Text="Ship From:" SkinID="FieldHeader"></asp:Label><br />
                    <asp:Label ID="ShipFrom" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:Label ID="InstructionText" runat="server" Text="Enter the items included in this shipment.  Remaining items will be placed in another shipment.  You may also enter a tracking number."></asp:Label><br /><br />
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" />
                    <asp:GridView ID="ShipmentItems" runat="server" ShowHeader="true"
                        AutoGenerateColumns="false" GridLines=none SkinID="PagedList">
                        <Columns>
                            <asp:TemplateField HeaderText="Quantity">
                                <ItemStyle HorizontalAlign="center" Width="70px" />
                                <ItemTemplate>
                                    <asp:HiddenField ID="Id" runat="server" Value='<%#Eval("OrderItemId")%>' />
                                    <asp:TextBox ID="Quantity" runat="server" Text='<%#Eval("Quantity")%>' Width="40px" MaxLength="4"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Quantity" HeaderText="Remaining">
                                <ItemStyle HorizontalAlign="Center" Width="70px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Sku" HeaderText="Sku">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="120px" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Item">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="300px" />
                                <ItemTemplate>
                                    <asp:Label ID="Name" runat="server" Text='<%#Eval("Name")%>'></asp:Label>
                                    <asp:Label ID="VariantName" runat="Server" Text='<%#Eval("VariantName", " ({0})")%>' Visible='<%#!String.IsNullOrEmpty((string)Eval("VariantName"))%>'></asp:Label><br />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:Label ID="AddTrackingNumberLabel" runat="server" Text="Tracking Number: " SkinID="FieldHeader"></asp:Label>
                    <asp:DropDownList ID="ShipGateway" runat="server" DataTextField="Name" DataValueField="ShipGatewayId" AppendDataBoundItems="true">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="AddTrackingNumber" runat="server" MaxLength="100"></asp:TextBox>
                    <br /><br />
                    <asp:HyperLink ID="CancelButton" runat="server" Text="Cancel" NavigateUrl="Default.aspx" SkinID="Button" />
                    &nbsp;<asp:Button ID="ShipButton" runat="server" Text="Ship" OnClick="ShipButton_Click" />
                    <asp:PlaceHolder ID="phQuantityValidation" runat="server"></asp:PlaceHolder>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

