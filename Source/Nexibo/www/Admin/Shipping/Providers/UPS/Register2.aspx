<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register2.aspx.cs" Inherits="Admin_Shipping_Providers_UPS_Register2" Title="Register for UPS" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1>
                    <asp:Label ID="Caption" runat="server" Text="UPS OnLine&reg; Tools Licensing & Registration"></asp:Label>
                </h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <th colspan="2" class="sectionHeader">
                <asp:Label ID="InstructionText" runat="server" Text="This form is intended for debugging or testing purposes only.  As a general rule, users of UPS OnLine&reg; Tools DO NOT have access to the user credentials requested below.  If you have reached this form inadvertantly, or if you do not have your user credentials, <a href='Register1.aspx'>click here</a> to register."></asp:Label>
            </th>
        </tr>
        <tr>
            <td>
                <table class="inputForm" cellpadding="4" width="100%">
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="UserIdLabel" runat="server" Text="UPS User Id:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="UserId" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="PasswordLabel" runat="server" Text="UPS Password:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Password" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="AccessKeyLabel" runat="server" Text="Access:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="AccessKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false" />
                            <asp:Button ID="NextButton" runat="server" Text="Next" OnClick="SaveButton_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
        <td align="justify">
        <br />
    <asp:Label ID="UPSCopyRight" runat="server" Text="UPS brandmark, and the Color Brown are trademarks of United Parcel Service of America, Inc. All Rights Reserved."
        SkinID="Copyright"></asp:Label>
        </td>
        </tr>
    </table>
    
</asp:Content>
