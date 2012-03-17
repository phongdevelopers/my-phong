<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="DeleteConfirm.aspx.cs"
    Inherits="Admin_Shipping_Providers_UPS_DeleteConfirm" Title="UPS Delete Confirmation" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" class="pageHeader">
                <h1>
                    <asp:Label ID="Caption" runat="server" Text="UPS OnLine&reg; Tools - Delete Confirmation"></asp:Label>
                </h1>
            </td>
        </tr>
        <tr>
            <td align="center">
                <img src="shield.jpg" align="Left" hspace="10" vspace="10" />
                <asp:Localize ID="InstructionText" runat="server">
                <p align="justify"><b>Delete Confirmation</b></p>
                <p align="justify">Your UPS OnLine&reg; Tools registration has been deleted.  UPS real-time rates and tracking information will no longer be available to your store through the integration.</p>
                <p align="justify">If at a later time you wish to re-enable the tools, simply visit the <a href="Default.aspx">registration</a> page and sign up again.</p>
                <p align="justify">Thank you for using the UPS OnLine&reg; Tools.</p>
                </asp:Localize>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="justify">
                <br />
                <asp:Label ID="UPSCopyRight" runat="server" Text="UPS brandmark, and the Color Brown are trademarks of United Parcel Service of America, Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
