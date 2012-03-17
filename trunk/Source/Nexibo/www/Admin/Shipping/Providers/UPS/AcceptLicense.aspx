<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="AcceptLicense.aspx.cs"
    Inherits="Admin_Shipping_Providers_UPS_AcceptLicense" Title="Untitled Page" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1>
                    <asp:Label ID="Caption" runat="server" Text="UPS OnLine&reg; Tools Licensing & Registration"></asp:Label><br />
                    Instance Name:
                    <asp:Label ID="InstanceNameLabel" runat="server" Text="${InstanceName}"></asp:Label>
                </h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" align="center">
                <asp:Label ID="InstructionText" runat="server" Text="Please review the licensing agreement and print a copy for your records:"
                    SkinID="Strong"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:TextBox ID="LicenseAgreement" runat="server" ReadOnly="true" TextMode="MultiLine"
                    Rows="20" Columns="60"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                <asp:Panel ID="agreePanel" runat="server">
                    <asp:RadioButtonList ID="rblAgree" runat="server">
                        <asp:ListItem Value="Yes" Text="Yes, I Do Agree"></asp:ListItem>
                        <asp:ListItem Value="No" Text="No, I Do Not Agree"></asp:ListItem>
                    </asp:RadioButtonList>
                </asp:Panel>
                <asp:RequiredFieldValidator ID="rblAgreeValidator" runat="server" ErrorMessage="You must accept the agreement to continue."
                    Text="*" ControlToValidate="rblAgree"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click"
                    CausesValidation="false" />
                <asp:Button ID="PrintButton" runat="server" Text="Print" CausesValidation="false" />
                <asp:Button ID="NextButton" runat="server" Text="Next" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="UPSCopyRight" runat="server" Text="UPS brandmark, and the Color Brown are trademarks of United Parcel Service of America, Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
