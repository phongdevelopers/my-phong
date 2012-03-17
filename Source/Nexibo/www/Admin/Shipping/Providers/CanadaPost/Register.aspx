<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register.aspx.cs"
    Inherits="Admin_Shipping_Providers_CanadaPost_Default" Title="CanadaPost Activation" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
	<div class="pageHeader">
		<div class="caption">
			<h1><asp:Localize ID="Caption" runat="server" Text="CanadaPost Activation"></asp:Localize></h1>
		</div>
	</div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <img src="logo_sen.gif" align="left" hspace="20" vspace="20" alt="Logo" />
                <asp:Localize ID="InstructionText" runat="server">
                
      <p align="justify">With CanadaPost, customers can get real-time shipping 
        rates based on their shipping addresses and order weights. </p>
                
      <p align="justify">In order to use CanadaPost, you must activate your CanadaPost account. </p>
                
      <p align="justify">If you do not wish to use any of the functions that utilize 
        the CanadaPost, click 'Cancel' to return to the main menu. If, at a later 
        time, you wish to use the CanadaPost, return to this section and complete 
        the CanadaPost activation.</p>
                </asp:Localize>
            </td>
        </tr>
        <tr>
            <td>            
            <table class="inputForm" align="center">
			        <tr id="trInstanceName" runat="server" visible="false">
			            <th class="rowHeader">
			                 <asp:Label ID="InstanceNameLabel" runat="server" Text="Gateway Name:"></asp:Label>
			            </th>
			            <td>
			                <asp:TextBox ID="InstanceName" runat="server" Text="CanadaPost"></asp:TextBox>
			                <asp:RequiredFieldValidator ID="InstanceNameRequired" runat="server" 
			                    ErrorMessage="Gateway name is required." Text="*" Display="Static" 
			                    ControlToValidate="InstanceName"></asp:RequiredFieldValidator>
			            </td>
			        </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="CanadaPostMerchantCPCIDLbl" runat="server" Text="CanadaPost Merchant CPC ID:"></asp:Label>
                        </th>
                        <td>
                            
                                <asp:TextBox ID="CanadaPostMerchantCPCID" runat="server"></asp:TextBox>&nbsp;R
                                <asp:RequiredFieldValidator ID="CanadaPostMerchantCPCIDValidator" runat="server"
                                    ErrorMessage="CanadaPost Merchant CPC ID is required." ControlToValidate="CanadaPostMerchantCPCID"
                                    Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                            
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div align="center">                                
                                <asp:Button ID="RegisterButton" runat="server" Text="Register" OnClick="RegisterButton_Click" />
								<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false"/>
                            </div>
                        </td>
                    </tr>
            </table>
            </td>
        </tr>
        <tr>
            <td align="justify">

                <br />
                <asp:Label ID="CanadaPostCopyRight" runat="server" Text="CanadaPost and CanadaPost brandmark are trademarks of CanadaPost Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>

            </td>
        </tr>
    </table>
</asp:Content>
