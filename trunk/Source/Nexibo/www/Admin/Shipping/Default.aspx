<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Shipping_Default" Title="Shipping Setup"  %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Localize1" runat="server" Text="Configure Shipping"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
                    <asp:HyperLink ID="MethodsLink" runat="server" Text="Methods" NavigateUrl="Methods/Default.aspx"></asp:HyperLink><br />
                    <asp:Label ID="MethodsDescription" runat="server" Text="Configure the methods of shipping that are available to customers of your store."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="GatewaysLink" runat="server" Text="Integrated Providers" NavigateUrl="Providers/Default.aspx"></asp:HyperLink><br />
                    <asp:Label ID="GatewaysDescription" runat="server" Text="Configure an integrated provider to allow real time calculation of shipping carges."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="WarehousesLink" runat="server" Text="Warehouses" NavigateUrl="Warehouses/Default.aspx"></asp:HyperLink><br />
                    <asp:Label ID="WarehousesDescription" runat="server" Text="Define the locations where your products are shipped from."></asp:Label>
                </li>
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>

