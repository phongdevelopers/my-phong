<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditCountryDialog.ascx.cs" Inherits="Admin_Shipping_Countries_EditCountryDialog" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<div class="section">
    <div class="header">
        <h2><asp:Localize ID="Caption" runat="server" Text="Edit {0}" EnableViewState="false" /></h2>
    </div>
    <div class="content">
        <table class="inputForm">
            <tr>
                <th class="rowHeader" nowrap>
                    <asp:Label ID="CountryCodeLabel1" runat="server" Text="Country Code:" />
                </th>
                <td>
                    <asp:Label ID="CountryCodeLabel2" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" nowrap>
                    <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="Enter the full name of the country as it should appear." />
                </th>
                <td>
                    <asp:TextBox ID="Name" runat="server" MaxLength="50" width="250px"></asp:TextBox>
                    <cb:RequiredRegularExpressionValidator ID="RequiredRegularExpressionValidator1" runat="server" ControlToValidate="Name"
                        Display="Static" ErrorMessage="Country name must be between 1 and 50 characters in length." Text="*"
                        ValidationGroup="EditCountry" ValidationExpression=".{1,50}" Required="true">
                    </cb:RequiredRegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" valign="top" nowrap>
                    <cb:ToolTipLabel ID="AddressFormatLabel" runat="server" Text="Address Format:" ToolTip="Specify the formatting rules for addresses from this country, if different from the default (US standard) format." /><br />
                    <asp:HyperLink ID="AddressFormatHelpLink" runat="server" Text="what's this?" NavigateUrl="AddressFormat.aspx" SkinID="Link" Target="_blank"></asp:HyperLink>
                </th>
                <td valign="top">
                    <asp:TextBox ID="AddressFormat" runat="server" Text="" TextMode="MultiLine" Rows="6" Wrap="false" width="250px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="AddressFormatValidator" runat="server" ControlToValidate="AddressFormat"
                        Display="Static" ErrorMessage="Address format cannot exceed 200 characters in length." Text="*"
                        ValidationGroup="EditCountry" ValidationExpression="[^\x00]{1,200}">
                    </asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="validation">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="EditCountry" />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="submit">                    
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ValidationGroup="EditCountry" />
					<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                </td>
            </tr>
        </table>
    </div>
</div>

