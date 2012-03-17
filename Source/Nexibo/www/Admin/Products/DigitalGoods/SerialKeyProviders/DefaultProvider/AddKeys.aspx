<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="AddKeys.aspx.cs" Inherits="Admin_Products_DigitalGoods_SerialKeyProviders_DefaultProvider_AddKeys" Title="Configure Serial Keys"  EnableViewState="false" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption">
        <h1><asp:Localize ID="Caption" runat="server" Text="Add Keys to '{0}'"></asp:Localize></h1>
    </div>
</div>
<div class="pageContent">
    <asp:Localize ID="InstructionText" runat="server" Text="There are {0} keys currently associated with this digital good.  To view and manage existing keys, <a href='{1}'>click here</a>.  To add new keys to this digital good, enter them into the form below."></asp:Localize>
</div>
<table cellpadding="3" cellspacing="0" class="inputForm">
    <tr>
        <th class="rowHeader" valign="top">
            <cb:ToolTipLabel ID="KeyDelimiterLabel" runat="server" Text="Key Delimiter:" AssociatedControlID="KeyDelimiter" ToolTip="Indicate whether your serial keys are separated by a single line break, a double line break, or a comma." />
        </th>
        <td>
            <asp:DropDownList ID="KeyDelimiter" runat="server">
                <asp:ListItem Text="Single Line Break"></asp:ListItem>
                <asp:ListItem Text="Double Line Break"></asp:ListItem>
                <asp:ListItem Text="Comma"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>    
    <tr>
        <th class="rowHeader" valign="top">
            <cb:ToolTipLabel ID="SerialKeyDataLabel" runat="server" Text="Serial Keys:" AssociatedControlID="SerialKeyData" ToolTip="Type or paste a list of serial keys to add.  Keys must be separated by an empty line." />
        </th>
        <td>
            <asp:TextBox ID="SerialKeyData" runat="server" Rows="10" Width="500px" TextMode="multiLine"></asp:TextBox>
        </td>
    </tr>    
	<asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" >
        <th valign="top">
            <asp:Label ID="ErrorMessagesLabel" runat="server" Text="Errors:" SkinID="ErrorCondition"/>
        </th>
        <td>
			<asp:Label ID="DuplicateErrorMessageLabel" runat="server" Text="Can not add duplicate keys. <br/>The following keys are already associated with this digital good." SkinID="ErrorCondition"/>
			<asp:BulletedList ID="ErrorMessageList" runat="server">
			</asp:BulletedList>
        </td>
	</asp:Panel>
    <tr>
        <td>&nbsp;</td>
        <td>            
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
			<asp:Button ID="CancelButton" runat="server" Text="Close" CausesValidation="false" OnClick="CancelButton_Click" />
        </td>
    </tr>
</table>
</asp:Content>
