<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register.aspx.cs"
    Inherits="Admin_Shipping_Providers_DHL_Default" Title="DHLInternational&reg; Activation" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1>
                    <asp:Label ID="Caption" runat="server" Text="DHLInternational&reg; Activation"></asp:Label></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <img src="Logo.jpg" align="Left" hspace="20" vspace="20" alt="shield" />
                <asp:Localize ID="InstructionText" runat="server">
                
      <p align="justify">With DHL&reg;, customers can get real-time shipping 
        rates based on their shipping addresses and order weights. Customers can 
        also view DHL tracking information for their packages right from your 
        store website!</p>
                
      <p align="justify">In order to use DHL&reg;, you must activate your DHL account.</p>
                
      <p align="justify">If you do not wish to use any of the functions that utilize 
        the DHL&reg;, click 'Cancel' to return to the main menu. If, at a later 
        time, you wish to use the DHL&reg;, return to this section and complete 
        the DHL&reg; activation process.</p>
                </asp:Localize>
            </td>
        </tr>
        <tr>
            <td>            
            <table class="inputForm" align="center">                 
                    <tr>
                        <td colspan="2">
                            <p style="margin-bottom: 10px;">
                                <asp:Label ID="Label2" runat="server" Text="Please Provide DHL User ID and Password information below."></asp:Label>
                            </p>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                            <asp:Label ID="CustomErrorMessage" runat="server" Text="" SkinID="ErrorCondition"></asp:Label>
                        </td>
                    </tr>
			        <tr id="trInstanceName" runat="server" visible="false">
			            <th class="rowHeader">
			                 <asp:Label ID="InstanceNameLabel" runat="server" Text="Gateway Name:"></asp:Label>
			            </th>
			            <td>
			                <asp:TextBox ID="InstanceName" runat="server" Text="DHL International"></asp:TextBox>
			                <asp:RequiredFieldValidator ID="InstanceNameRequired" runat="server" 
			                    ErrorMessage="Gateway name is required." Text="*" Display="Static" 
			                    ControlToValidate="InstanceName"></asp:RequiredFieldValidator>
			            </td>
			        </tr>
                    <tr>
                        <th class="rowHeader" >
                            <asp:Label ID="DHLUserIDLbl" runat="server" Text="DHL User ID:" SkinID="FieldHeader"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="DHLUserID" runat="server"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="DHLUserIDValidator" runat="server" ErrorMessage="DHL User ID is required."
                                ControlToValidate="DHLUserID" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" align="right">
                            <asp:Label ID="DHLPasswordLbl" runat="server" Text="DHL Password:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="DHLPassword" runat="server"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="DHLPasswordValidator" runat="server" ErrorMessage="DHL Password is required."
                                ControlToValidate="DHLPassword" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>                    
                    <tr>
                        <td>
                            <div align="center">                                
                                <asp:Button ID="RegisterButton" runat="server" Text="Register" OnClick="RegisterButton_Click" />
								<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                            </div>
                        </td>
                    </tr>
                </table>          
            </td>
        </tr>        
        <tr>
            <td>
                <asp:Label ID="DHLCopyRight" runat="server" Text="DHL and DHL brandmark are trademarks of DHL Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
