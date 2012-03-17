<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register.aspx.cs"
    Inherits="Admin_Shipping_Providers_AustraliaPost_Default" Title="AustraliaPost Activation" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
	<div class="pageHeader">
		<div class="caption">
			<h1><asp:Localize ID="Caption" runat="server" Text="AustraliaPost Activation"></asp:Localize></h1>
		</div>
	</div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <img src="Logo.gif" align="left" hspace="20" vspace="20" alt="Logo" />
	       <asp:Localize ID="InstructionText1" runat="server">                
				  <p align="justify">With AustraliaPost, customers can get shipping rate estimates based on their shipping addresses and order weights. </p>
						
				  <p align="justify"><b>AustraliaPost Terms of Use require that the following notice be included in checkout terms and conditions of your store.</b> </p>
		  </asp:Localize>
			<p align="justify"> 
			<div style="border-width: 2px;  border-style: solid; border-color: grey; padding:0px; 30px; 0px; 30px; margin:30px 30px 20px 30px;">			
			<asp:Label ID="StoreName" runat="server" Text="Store Name"/> accepts responsibility for 
			any loss of, damage to, late delivery or non-delivery of goods ordered from our web site. 
			To the maximum extent permitted by law, you agree to release our carriers from any 
			liability relating to loss of, damage to, late delivery or non-delivery of any goods you 
			order from this web site and to assign all rights to claim compensation or insurance 
			against our carriers to us and expressly and irrevocably do so by submitting your order.			
			</div>
			</p>
	
		<asp:Localize ID="InstructionText2" runat="server">                
			<p align="justify" style="margin:0 30px; 0 30px;">Please make sure that the above notice is included in your store's checkout terms and conditions and click <b>Next</b> to complete the registration. Otherwise click <b>Cancel</b> to return to the main menu. If, at a later 
		    time, you wish to use the AustraliaPost, return to this section and complete the registration process.</p>
		</asp:Localize>			
		<asp:Panel ID="ErrorPanel" runat="server" Visible="False">
			<br/>
			<p align="justify" class="errorCondition" style="margin:0 30px; 0 30px;">
			<asp:Label Id="ErrorMessage" runat="server" Text="It appears that the store's checkout terms and conditions do not include the above notice. Please make sure that the above notice is included in checkout terms and conditions before proceeding. You can edit the store's checkout terms and conditions from <a href='{0}'>store settings</a> page."/>
			</p>
			<br/>
		</asp:Panel>
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
			                <asp:TextBox ID="InstanceName" runat="server" Text="AustraliaPost"></asp:TextBox>
			                <asp:RequiredFieldValidator ID="InstanceNameRequired" runat="server" 
			                    ErrorMessage="Gateway instance name is required." Text="*" Display="Static" 
			                    ControlToValidate="InstanceName"></asp:RequiredFieldValidator>
			            </td>
			        </tr>
                    <tr>
                        <td colspan="2">
                            <div align="center">                                
                                <asp:Button ID="RegisterButton" runat="server" Text="Next" OnClick="RegisterButton_Click" />
								<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false"/>
                            </div>
                        </td>
                    </tr>
            </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Label ID="AustraliaPostCopyRight" runat="server" Text="AustraliaPost and AustraliaPost brandmark are trademarks of AustraliaPost Inc. All Rights Reserved." SkinID="Copyright"></asp:Label>

            </td>
        </tr>
    </table>
</asp:Content>
