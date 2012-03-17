<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register.aspx.cs" Inherits="Admin_Shipping_Providers_USPS_Register" title="USPS Setup" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="U.S. Postal Service&reg; Registration"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <asp:Image ID="Logo" runat="server" ImageUrl="LOGO_S.gif" />
        </td>
        <td style="padding-left:10px;">
			<p style="text-align:justify;">
			You must register with the U.S. Postal Service in order to enable online rate estimates and tracking. Please follow the 
			instructions below to complete this one-time setup process:</p>
			<ol>
			<li>
			Register with USPS to obtain your User Id.  You must do this step at the following USPS website:<br>
			<br>
			<a href="https://secure.shippingapis.com/Registration/" target="_blank" class="link">https://secure.shippingapis.com/Registration/</a><br><br>
			<li>
			Provide the User Id in the space provided below.<br><br>
			<li>
			Then, contact the USPS ICCC (Internet Customer Care Center) and ask to have your User Id activated for 'Production' use:<br>
			<br>
			E-mail: <a href="mailto:icustomercare@USPS.com" class="link">icustomercare@USPS.com</a><br/>
			Phone: 1-800-344-7779 (7AM to 11PM Eastern Time)<br><br></li>
			<li>
			Once your account is activated, click the "Activated" checkbox below and click Finish.</li></ol>
			<asp:ValidationSummary ID="ValidationSummary1" runat="server" />
			<table class="inputForm">
			    <tr id="trInstanceName" runat="server" visible="false">
			        <th class="rowHeader">
			             <asp:Label ID="InstanceNameLabel" runat="server" Text="Gateway Name:"></asp:Label>
			        </th>
			        <td>
			            <asp:TextBox ID="InstanceName" runat="server" Text="USPS"></asp:TextBox>
			            <asp:RequiredFieldValidator ID="InstanceNameRequired" runat="server" 
			                ErrorMessage="Gateway name is required." Text="*" Display="Static" 
			                ControlToValidate="InstanceName"></asp:RequiredFieldValidator>
			        </td>
			    </tr>
			    <tr>
			        <th class="rowHeader">
			            <asp:Label ID="UserIdLabel" runat="server" Text="User ID:"></asp:Label>
			        </th>
			        <td>
			            <asp:TextBox ID="UserId" runat="server"></asp:TextBox>
			            <asp:RequiredFieldValidator ID="UserIdRequired" runat="server" 
			                ErrorMessage="User Id is required." Text="*" Display="Static" 
			                ControlToValidate="UserId"></asp:RequiredFieldValidator>
			        </td>
			    </tr>
			    <tr>
                    <th class="rowHeader">
                        <asp:Label ID="UserIdActiveLabel" runat="server" Text="Activated:"></asp:Label>        
                    </th>
                    <td>
                        <asp:CheckBox ID="UserIdActive" runat="Server" />
                    </td>
                </tr>
			    <tr>
			        <td>&nbsp;</td>
			        <td>
			            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
			            <asp:Button ID="NextButton" runat="server" Text="Finish" OnClick="NextButton_Click" />
			        </td>
			    </tr>
			</table>
		    
        </td>
    </tr>
</table>
</asp:Content>

