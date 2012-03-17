<%@ Control Language="C#" AutoEventWireup="true" CodeFile="KitProductMenu.ascx.cs" Inherits="Admin_Products_Kits_KitProductMenu" %>
<asp:Panel ID="KitComponentPanel" runat="server">
    <div class="section">
        <div class="header">
            <h2><asp:Localize id="Caption" runat="server" Text="Kit Components:"></asp:Localize></h2>
        </div>
        <div class="content">
            <asp:DataList ID="ComponentList" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><asp:Label ID="Name" runat="server" text='<%#Eval("Name")%>'></asp:Label></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:DataList>
        </div>
    </div>
</asp:Panel>