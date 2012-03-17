<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditSerialKey.aspx.cs" Inherits="Admin_Products_DigitalGoods_SerialKeyProviders_DefaultProvider_EditSerialKey" Title="Configure Serial Keys"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption">
        <h1><asp:Localize ID="Caption" runat="server" Text="Edit A Serial Key For '{0}'"></asp:Localize></h1>
    </div>
</div>
<table cellpadding="3" cellspacing="0" class="inputForm maxWidth">
    <tr class="sectionHeader">
        <td colspan="2">
            SERIAL KEY DATA
        </td>
    </tr>
    <tr>
        <th class="rowHeader" Width="30%">
            <cb:ToolTipLabel ID="SerialKeyDataLabel" runat="server" Text="Serial Key Data:" AssociatedControlID="SerialKeyData" ToolTip="Enter the full serial key here." />
        </th>
        <td>
            <asp:TextBox ID="SerialKeyData" runat="server" Rows="5" Width="90%" TextMode="multiLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>            
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
			<asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false" OnClick="CancelButton_Click" />
        </td>
    </tr>
</table>
</asp:Content>
