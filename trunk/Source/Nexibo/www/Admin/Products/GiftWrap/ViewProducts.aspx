<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ViewProducts.aspx.cs" Inherits="Admin_Products_GiftWrap_ViewProducts" Title="Gift Wrap: View assigned products" %>
<%@ Register Src="EditProducts.ascx" TagName="EditProducts" TagPrefix="uc1" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="{0}: Assigned Products"></asp:Localize></h1>
    	</div>
    </div>
    <uc1:EditProducts ID="EditProducts1" runat="server" /><br />        
</asp:Content>

