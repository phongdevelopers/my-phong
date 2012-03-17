<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsReader.ascx.cs" Inherits="Admin_Dashboard_NewsReader" EnableViewState="false" %>
<ajax:UpdatePanel ID="NewsReaderAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:DataList ID="RssList" runat="server" EnableViewState="false">
            <ItemTemplate>
                <asp:Label ID="DateLabel" runat="server" Text='<%# string.Format("{0:d}", ((RssNewsItem)Container.DataItem).PubDate) %>' SkinID="FieldHeader"></asp:Label><br />
                <asp:HyperLink ID="TitleLink" runat="server" Text='<%# ((RssNewsItem)Container.DataItem).Title %>' NavigateUrl='<%# ((RssNewsItem)Container.DataItem).Link %>' Target="_blank"></asp:HyperLink><br />
                <asp:Literal ID="DescriptionLabel" runat="server" Text='<%# ((RssNewsItem)Container.DataItem).Description %>'></asp:Literal>
            </ItemTemplate>
            <SeparatorTemplate>
                <hr />
            </SeparatorTemplate>
            <FooterTemplate>
                <hr />
            </FooterTemplate>
        </asp:DataList>
        <asp:Localize ID="CachedAtLabel" runat="server" Text="Cached at "></asp:Localize>
        <asp:Literal ID="CachedAt" runat="server"></asp:Literal>
        <asp:LinkButton ID="RefreshButton" runat="server" Text="Refresh" OnClick="RefreshButton_Click"></asp:LinkButton>
    </ContentTemplate>
</ajax:UpdatePanel>