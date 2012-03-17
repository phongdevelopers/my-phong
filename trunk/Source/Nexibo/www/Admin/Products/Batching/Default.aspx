<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Products_Batching_Default" Title="Products Batching" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Products Batching"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top">
            <ul class="menuList">
                <li>
	                <asp:HyperLink ID="BatchEditLink" runat="server" Text="Batch Edit" NavigateUrl="BatchEdit.aspx"></asp:HyperLink><br />
	                <asp:Label ID="BatchEditDescription" runat="server" Text="Allows you to edit multiple products at once. Products are filtered with search criteria and presented with editable fields of your choosing. "></asp:Label>
                </li>
            </ul>
	    </td>
	</tr>
</table>    
</asp:Content>