<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Marketing_Feeds_Default" Title="Product Feeds" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Product Feeds"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="2" class="innerLayout">
        <tr>
            <td valign="top" align="left" class="itemlist">
                <ul class="menuList">
					<li><asp:HyperLink ID="GoogleBaseFeedLink" runat="server" NavigateUrl="GoogleBase.aspx" Text="Google Base Feed"></asp:HyperLink></li>
					<li><asp:HyperLink ID="YahooShoppingFeedLink" runat="server" NavigateUrl="YahooShopping.aspx" Text="Yahoo! Shopping Feed"></asp:HyperLink></li>
					<li><asp:HyperLink ID="ShoppingComFeedLink" runat="server" NavigateUrl="Shopping.com.aspx" Text="Shopping.com Feed"></asp:HyperLink></li>
				</ul>                
            </td>
        </tr>
    </table>
</asp:Content>



