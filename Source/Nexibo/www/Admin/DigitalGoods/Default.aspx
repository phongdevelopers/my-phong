<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_DigitalGoods_Default" Title="Manage Digital Goods" EnableViewState="false"%>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Manage Digital Goods"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
                    <asp:HyperLink ID="DigitalGoodsLink" runat="server" Text="Manage Digital Goods" NavigateUrl="DigitalGoods.aspx"></asp:HyperLink><br />
                    <asp:Label ID="DigitalGoodsDescription" runat="server" Text="Manage the digital goods in your catalog."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="FindLink" runat="server" Text="Find Digital Goods" NavigateUrl="FindDigitalGoods.aspx"></asp:HyperLink><br />
                    <asp:Label ID="FindDescription" runat="server" Text="Search for digital goods attached to products in your catalog."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="FilesLink" runat="server" Text="Digital Good Files" NavigateUrl="DigitalGoodFiles.aspx"></asp:HyperLink><br />
                    <asp:Label ID="FilesDescription" runat="server" Text="List of digital good files and the associated digital goods."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="ReadmesLink" runat="server" Text="Readmes" NavigateUrl="Readmes.aspx"></asp:HyperLink><br />
                    <asp:Label ID="ReadmesDescription" runat="server" Text="Manage the readmes that are available to attach to your digital goods."></asp:Label>
                </li>
                <li>
                    <asp:HyperLink ID="AgreementsLink" runat="server" Text="License Agreements" NavigateUrl="Agreements.aspx"></asp:HyperLink><br />
                    <asp:Label ID="AgreementsDescription" runat="server" Text="Manage the license agreements that are available to attach to your digital goods."></asp:Label>
                </li>
            </ul>
	    </td>
	</tr>
</table>
</asp:Content>
