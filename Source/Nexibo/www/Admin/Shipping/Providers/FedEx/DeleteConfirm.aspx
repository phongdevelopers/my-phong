<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="DeleteConfirm.aspx.cs"
    Inherits="Admin_Shipping_Providers_FedEx_DeleteConfirm" Title="Confirmation FedEx Deletion" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" class="pageHeader">
                <h1>
                    <asp:Label ID="Caption" runat="server" Text="FedEx&reg; - Delete Confirmation" ></asp:Label>
                    </h1>
            </td>
        </tr>
        <tr>
            <td align="center">
                <img src="LOGO_S.gif" align="Left" hspace="10" vspace="10" />
                <asp:Localize ID="InstructionText" runat="server">
                <p align="left"><b>Delete Confirmation</b></p>
                <p align="left">Your FedEx OnLine&reg; registration has been deleted.  FedEx real-time rates and tracking information will no longer be available to your store through the integration.</p>
                <p align="left">If at a later time you wish to re-enable the tools, simply visit the <a href="Default.aspx">registration</a> page and sign up again.</p>
                <p align="left">Thank you for using the FedEx&reg;.</p>
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
                <br />
                <asp:Label ID="FedExCopyRight" runat="server" Text="FedEx and FedEx brandmark are trademarks of FedEx Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
