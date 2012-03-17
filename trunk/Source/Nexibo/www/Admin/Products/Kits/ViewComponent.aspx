<%@ Page Language="C#" MasterPageFile="../Product.master" CodeFile="ViewComponent.aspx.cs" Inherits="Admin_Products_Kits_ViewComponent" Title="Kits Using Component"  EnableViewState="false" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Sharing Details for {0}"></asp:Localize></h1>
        </div>
    </div>
    <asp:Label ID="InstructionText" runat="server" Text="Below is a list of kits that have this component attached."></asp:Label><br /><br />
    <asp:Repeater ID="KitList" runat="server">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li><asp:HyperLink ID="Name" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%# Eval("ProductId", "../EditProduct.aspx?ProductId={0}") %>'></asp:HyperLink></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
    <br /><asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />
</asp:Content>

