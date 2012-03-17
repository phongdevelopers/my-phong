<%@ Page Language="C#" MasterPageFile="../Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Payment__Default" Title="Payment Setup"  %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Payment Setup"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
                    <asp:HyperLink ID="MethodsLabel" runat="server" Text="Methods" NavigateUrl="Methods.aspx"></asp:HyperLink><br />
                    <asp:Label ID="MethodsDescription" runat="server" Text="Use the Payment Method screen to indicate what methods of payment you accept for your store."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="GatewaysLabel" runat="server" Text="Gateways" NavigateUrl="Gateways.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewaysDescription" runat="server" Text="Configure a Gateway to allow real time processing of credit cards."></asp:Label>
                </li>
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>

