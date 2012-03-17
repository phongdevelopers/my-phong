<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdminAlerts.ascx.cs" Inherits="Admin_Dashboard_AdminAlerts" EnableViewState="false" %>
<ajax:UpdatePanel ID="AlertAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <ul>
        <asp:Repeater ID="AlertList" runat="server">
            <ItemTemplate>
                <li><%#Container.DataItem%></li>
            </ItemTemplate>
        </asp:Repeater>
        </ul>
        <asp:Localize ID="CachedAtLabel" runat="server" Text="Cached at "></asp:Localize>
        <asp:Literal ID="CachedAt" runat="server"></asp:Literal>
        <asp:LinkButton ID="RefreshButton" runat="server" Text="Refresh" OnClick="RefreshButton_Click"></asp:LinkButton>
    </ContentTemplate>
</ajax:UpdatePanel>