<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="DeleteConfirm.aspx.cs"
    Inherits="Admin_Shipping_Providers_DHL_DeleteConfirm" Title="Confirmation DHLInternational Deletion" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" class="pageHeader">
                <h1>
                    <asp:Label ID="Caption" runat="server" Text="DHL&reg; - Delete Confirmation"></asp:Label>
                </h1>
            </td>
        </tr>
        <tr>
            <td align="center">
                <img src="Logo.jpg" align="Left" hspace="10" vspace="10" />
                <asp:Localize ID="InstructionText" runat="server">
                <p align="left"><b>Delete Confirmation</b></p>
                <p align="left">Your DHL&reg; activation has been deleted.  DHL real-time rates and tracking information will no longer be available to your store through the integration.</p>
                <p align="left">If at a later time you wish to re-enable DHL, simply visit the <a href="Default.aspx">activation</a> page and sign up again.</p>
                <p align="left">Thank you for using the DHL&reg;.</p>
                </asp:Localize>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="DHLCopyRight" runat="server" Text="DHL and DHL brandmark are trademarks of DHL Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
