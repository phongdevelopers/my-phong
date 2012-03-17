<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="RegisterConfirm.aspx.cs"
    Inherits="Admin_Shipping_Providers_UPS_RegisterConfirm" Title="Untitled Page" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" class="pageHeader">
                <h1>
                    <asp:Label ID="Caption" runat="server" Text="UPS OnLine&reg; Tools Licensing & Registration -- Successful"></asp:Label><br />
                    Instance Name:
                    <asp:Label ID="InstanceNameLabel" runat="server" Text="${InstanceName}"></asp:Label>
                </h1>
            </td>
        </tr>
        <tr>
            <td align="center">
                <img src="shield.jpg" align="Left" hspace="10" vspace="10" />
                <asp:Localize ID="InstructionText" runat="server">
                <p align="justify"><b>Registration Successful!</b></p>
                <p align="justify">Thank you for registering to use the UPS OnLine&reg; Tools.  To learn more about the tools, please visit <a href="http://www.ec.ups.com" target="_blank">www.ec.ups.com</a>.</p>
                <p align="justify">Still handwriting your UPS shipping labels? <b>UPS Internet Shipping</b> allows you to electronically prepare domestic and international shipments from the convenience of any computer with Internet access. To learn more or to begin using UPS Internet Shipping, <a href="http://ups.com/bussol/solutions/internetship.html" target="_blank">click here</a>.</p>
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
