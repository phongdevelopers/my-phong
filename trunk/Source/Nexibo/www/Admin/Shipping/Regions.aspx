<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Regions.aspx.cs" Inherits="Admin_Shipping_Regions" Title="Regions Menu" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Localize1" runat="server" Text="Configure Regions"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
                    <asp:HyperLink ID="CountriesLink" runat="server" Text="Countries" NavigateUrl="Countries/Default.aspx"></asp:HyperLink><br />
                    <asp:Label ID="CountriesDescription" runat="server" Text="Edit the country list (including provinces and address formats) for your store."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="ZonesLink" runat="server" Text="Zones" NavigateUrl="Zones/Default.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ZonesDescription" runat="server" Text="Define zones that can apply to shipping and taxes in your store."></asp:Label>
                </li>
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>

