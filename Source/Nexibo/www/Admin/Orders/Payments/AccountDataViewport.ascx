<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountDataViewport.ascx.cs" Inherits="Admin_Orders_Payments_AccountDataViewport" %>
<ajax:UpdatePanel ID="AccountDataAjax" runat="server">
    <ContentTemplate>
        <asp:LinkButton ID="ShowAccountData" runat="server" Text="Show Account Details" OnClick="ShowAccountData_Click" EnableViewState="false" />
        <asp:Repeater ID="AccountData" runat="server" EnableViewState="false">
            <ItemTemplate>
                <asp:Literal ID="K" runat="server" Text='<%#string.Format("{0}: ", StringHelper.SpaceName(Eval("Key").ToString()))%>'></asp:Literal>
                <asp:Literal ID="V" runat="server" Text='<%#Eval("Value")%>'></asp:Literal><br />
            </ItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="SSLRequiredPanel" runat="server" EnableViewState="false">
            <asp:Localize ID="SSLRequiredMessage" runat="server" Text="Detailed account data is available, but you must enable SSL in order to view it." EnableViewState="false"></asp:Localize>
        </asp:PlaceHolder>
    </ContentTemplate>
</ajax:UpdatePanel>
