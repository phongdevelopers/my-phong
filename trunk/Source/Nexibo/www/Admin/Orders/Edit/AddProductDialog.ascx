<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddProductDialog.ascx.cs" Inherits="Admin_Orders_Edit_AddProductDialog" EnableViewState="false"%>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<ajax:UpdatePanel ID="ProductAjax" runat="server">
    <ContentTemplate>
        <table class="inputForm">
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="NameLabel" runat="server" Text="Product:"></asp:Label>        
                </th>
                <td>
                    <asp:Label ID="Name" runat="server" EnableViewState="false"></asp:Label>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" id="phOptions"></asp:PlaceHolder>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="PriceLabel" runat="server" Text="Price:"></asp:Label>        
                </th>
                <td>
                    <asp:TextBox ID="Price" runat="server" EnableViewState="false" OnPreRender="Price_PreRender" Width="50px" MaxLength="8"></asp:TextBox>
                    <asp:PlaceHolder ID="phVariablePrice" runat="server"></asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="QuantityLabel" runat="server" Text="Quantity:"></asp:Label>        
                </th>
                <td>
                    <cb:updowncontrol id="Quantity" runat="server" DownImageUrl="~/images/down.gif" UpImageUrl="~/images/up.gif" Columns="2" Text="1"></cb:updowncontrol>
                </td>
            </tr>
            <tr id="trShipment" runat="server">
                <th class="rowHeader">
                    <asp:Label ID="ShipmentsListLabel" runat="server" Text="Shipment:"></asp:Label>
                </th>
                <td>
                    <asp:DropDownList ID="ShipmentsList" runat="server">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="ShipmentRequired" runat="server"
                        Text="*" ErrorMessage="You must select a shipment for this item."
                        ControlToValidate="ShipmentsList"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr id="trInventoryWarning" runat="server" visible="false" enableviewstate="false">
                <td>&nbsp;</td>
                <td>
                    <asp:Literal ID="InventoryWarningMessage" runat="server" EnableViewState="false"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" />
                    <asp:HiddenField ID="HiddenProductId" runat="server" /><br />                                       
                    <asp:Button ID="SaveButton" runat="server" Visible="False" Text="Save" OnClick="SaveButton_Click" />
			        <asp:HyperLink ID="BackButton" runat="server" Text="Cancel"  SkinID="Button" />&nbsp;
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:UpdatePanel>