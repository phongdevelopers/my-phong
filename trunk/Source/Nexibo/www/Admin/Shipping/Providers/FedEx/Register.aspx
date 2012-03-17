<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register.aspx.cs"
    Inherits="Admin_Shipping_Providers_FedEx_Default" Title="FedEx&reg; Registration" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1>
                    <asp:Label ID="Caption" runat="server" Text="FedEx&reg; Registration"></asp:Label>
                </h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <tr>
                <td>
                    <img src="LOGO_S.gif" align="Left" hspace="20" vspace="20" alt="shield" />
                    <asp:Localize ID="InstructionText" runat="server">
                
      <p align="justify">With FedEx&reg;, customers can get real-time shipping 
        rates based on their shipping addresses and order weights. Customers can 
        also view FedEx tracking information for their packages right from your 
        store website!</p>
                
      <p align="justify">In order to enable FedEx&reg;, you must register with 
        FedEx. Registering with FedEx makes sure you stay up-to-date with their 
        latest services, updates, and enhancements. Please note that this registration 
        is designed to establish a relationship between your company and FedEx.</p>
                
      <p align="justify">If you do not wish to use any of the functions that utilize 
        the FedEx&reg;, click 'Cancel' to return to the main menu. If, at a later 
        time, you wish to use the FedEx&reg;, return to this section and complete 
        the FedEx&reg; licensing and registration process.</p>
                    </asp:Localize>
                </td>
            </tr>            
            <tr>
                <td>
                    <div align="center">
                        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                        <asp:Button ID="NextButton" runat="server" Text="Next" OnClick="NextButton_Click" />
                    </div>
                </td>
            </tr>
            <tr>
                <td align="justify">
                    <asp:Label ID="FedExCopyRight" runat="server" Text="FedEx and FedEx brandmark are trademarks of FedEx Inc. All Rights Reserved."
                        SkinID="Copyright"></asp:Label>
                </td>
            </tr>
    </table>
</asp:Content>
