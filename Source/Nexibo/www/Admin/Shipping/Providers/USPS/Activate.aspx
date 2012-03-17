<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Activate.aspx.cs" Inherits="Admin_Shipping_Providers_USPS_Activate" title="U.S. Postal Service&reg; Testing & Activation" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Localize1" runat="server" Text="U.S. Postal Service&reg; Testing &amp; Activation"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <asp:Image ID="Logo" runat="server" ImageUrl="LOGO_S.gif" />
        </td>
        <td style="padding-left:10px;">
			<p style="text-align:justify;">
			If your USPS&reg; account has not been activated and granted access to the production server, follow the steps below.
			</P>
			<OL>
			<LI>
			Contact the USPS ICCC (Internet Customer Care Center) to get your username activated:<BR>
			<BR>
			E-mail: <A href="mailto:icustomercare@USPS.com" class="link">icustomercare@USPS.com</A><BR>
			Phone: 1-800-344-7779 (7AM to 11PM Eastern Time)<BR><br>
			</LI>
			<LI>
			Once your account is activated, click the checkbox below and click Finish.</LI></OL>
            <table class="inputForm">
                <tr>
                    <th class="rowHeader" style="height: 21px">
                        <asp:Label ID="UserIdLabel" runat="server" Text="User ID:"></asp:Label>
                    </th>
                    <td style="height: 21px">
                        <asp:Label ID="UserId" runat="server"></asp:Label>
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
                        <asp:Button ID="CancelButton" runat="server" Text="Back" OnClick="CancelButton_Click" />
                        <asp:Button ID="NextButton" runat="server" Text="Finish" OnClick="NextButton_Click" />
                    </td>
                </tr>
            </table>
       </td>
    </tr>
</table>
</asp:Content>

