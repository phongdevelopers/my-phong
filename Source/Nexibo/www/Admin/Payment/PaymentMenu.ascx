<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PaymentMenu.ascx.cs" Inherits="Admin_Payment_PaymentMenu" %>
<div class="section">
    <div class="header">
        <h2><asp:Localize id="PaymentMenuCaption" runat="server" Text="Payment Setup"></asp:Localize></h2>    
    </div>
    <div class="content">
        <asp:HyperLink ID="Methods" runat="server" NavigateUrl="Methods.aspx" Text="Methods"></asp:HyperLink>
        <asp:HyperLink ID="Gateways" runat="server" NavigateUrl="Gateways.aspx" Text="Gateways"></asp:HyperLink>
        <asp:HyperLink ID="GiftCerts" runat="server" NavigateUrl="GiftCerts.aspx" Text="Gift Certificates"></asp:HyperLink>
    </div>
</div>