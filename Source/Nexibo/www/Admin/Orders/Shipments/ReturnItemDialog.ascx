<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReturnItemDialog.ascx.cs" Inherits="Admin_Orders_Shipments_ReturnItemDialog" %>
<table class="inputForm" cellpadding="4" cellspacing="0" style="margin-top:8px;" width="400px">        
    <tr class="sectionHeader">
        <th colspan="2">                
            <asp:Label ID="ReturnItemsLabel" runat="server" Text="Return Order Item:"></asp:Label>
        </th>
    </tr>
    <tr>
        <th class="rowHeader" style="width:40%">
            <asp:Label ID="OrderItemIdLabel" runat="server" Text="Order Item Id:"></asp:Label>
        </th>
        <td >
            <asp:Label ID="OrderItemId" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top">
            <asp:Label ID="NameLabel" runat="server" Text="Name:"></asp:Label>
        </th>
        <td >
            <asp:Label ID="Name" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" >
            <asp:Label ID="QuantityLabel" runat="server" Text="Quantity Purchased:"></asp:Label>
        </th>
        <td >
            <asp:Label ID="Quantity" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" >
            <asp:Label ID="QuantityReturnLabel" runat="server" Text="Quantity To Return:"></asp:Label>
        </th>
        <td >
            <asp:TextBox ID="QuantityReturn" runat="server" MaxLength="4" Columns="4" ValidationGroup="ReturnItemGroup"></asp:TextBox>
            <asp:RequiredFieldValidator ID="QuantityValidator" runat="server" ControlToValidate="QuantityReturn"
                ErrorMessage="Quantity to return is required." ValidationGroup="ReturnItemGroup">*</asp:RequiredFieldValidator>
            <asp:RangeValidator ID="QuantityRangeValidator" runat="server" ControlToValidate="QuantityReturn"
                ErrorMessage="Quantity to return should be valid." Type="Integer" MinimumValue="1" MaximumValue="1" ValidationGroup="ReturnItemGroup">*</asp:RangeValidator>
        </td>             
    </tr>    
    <tr id="trInventory" runat="server">
        <th class="rowHeader"  valign="top">
            <asp:Label ID="ReturnToInvRadioLabel" runat="server" Text="Return to Inventory:"></asp:Label>
        </th>
        <td >
            <asp:RadioButtonList ID="ReturnToInvRadio" runat="server" >
                <asp:ListItem Text="Yes" Value="Yes" Selected="True"></asp:ListItem>
                <asp:ListItem Text="No" Value="No" ></asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <th colspan="2" align="left">
           <asp:Literal ID="OrderNoteLabel" runat="server" Text="Add Note To Order:" ></asp:Literal><br />
           <asp:TextBox ID="OrderNote" runat="server" TextMode="MultiLine" Columns="60" Rows="5">
           </asp:TextBox>
        </th>
    </tr>
    <tr>
        <td colspan="2" align="right">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="ReturnItemGroup"/>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="right">
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="CancelButton_Click" />
            <asp:Button ID="UpdateButton" runat="server" Text="Update" OnClick="UpdateButton_Click" ValidationGroup="ReturnItemGroup" />
        </td>
    </tr>
</table>