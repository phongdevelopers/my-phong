<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register.aspx.cs"
    Inherits="Admin_Shipping_Providers_UPS_Register" Title="UPS OnLine&reg; Tools Licensing &amp; Registration" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1>
                    <asp:Label ID="Caption" runat="server" Text="UPS OnLine&reg; Tools Licensing &amp; Registration"></asp:Label>
                </h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <img src="shield.jpg" align="Left" hspace="20" vspace="20" alt="shield" />
                <asp:Localize ID="Localize1" runat="server">
                <p align="justify">With UPS OnLine&reg; Tools, customers can get real-time shipping rates based on their shipping addresses and order weights. Customers can also view UPS tracking information for their packages right from your store website!</p>
                <p align="justify">In order to enable UPS OnLine&reg; Tools, you must register with UPS. Registering with UPS makes sure you stay up-to-date with their latest services, updates, and enhancements. Please note that this registration is designed to establish a relationship between your company and UPS.</p>
                <p align="justify">If you do not wish to use any of the functions that utilize the UPS OnLine&reg; Tools, click 'Cancel' to return to the main menu. If, at a later time, you wish to use the UPS OnLine&reg; Tools, return to this section and complete the UPS OnLine&reg; Tools licensing and registration process.</p>
                </asp:Localize>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div align="center">
                <table class="inputForm">
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Label ID="InstructionText" runat="server" Text="Please review the licensing agreement and print a copy for your records:"
                                SkinID="Strong"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center" Class="PrintArea">
                            <asp:TextBox ID="LicenseAgreement" runat="server" ReadOnly="true" TextMode="MultiLine"
                                Rows="20" Width="600px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:RadioButtonList ID="rblAgree" runat="server">
                                <asp:ListItem Value="yes" Text="Yes, I Do Agree"></asp:ListItem>
                                <asp:ListItem Value="no" Text="No, I Do Not Agree"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click"
                                CausesValidation="false" />
                            <asp:Button ID="PrintButton" runat="server" Text="Print" CausesValidation="false" OnClientClick="window.open('PrintAgreement.aspx');return false;" />
                            <asp:Button ID="NextButton" runat="server" Text="Next" OnClick="NextButton_Click" />
                        </td>
                    </tr>
                </table>
                </div>
            </td>
        </tr>
        <tr>
        <td>
        <br />
    <asp:Label ID="UPSCopyRight" runat="server" Text="UPS brandmark, and the Color Brown are trademarks of United Parcel Service of America, Inc. All Rights Reserved."
        SkinID="Copyright"></asp:Label>
        </td>
        </tr>
    </table>
    
</asp:Content>
