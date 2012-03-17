<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true"
    CodeFile="CustomURLs.aspx.cs" Inherits="Admin_SEO_CustomURLs" Title="Custom URLs" %>

<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls"
    TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Custom URLs"></asp:Localize></h1>
        </div>
    </div>
    <ajax:UpdatePanel ID="RedirectGridAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cb:SortedGridView ID="CustomUrlGrid" runat="server" AutoGenerateColumns="False"
                AllowSorting="true" DefaultSortExpression="CustomUrlId" DefaultSortDirection="Ascending"
                AllowPaging="True" PagerStyle-CssClass="paging" PageSize="40" PagerSettings-Position="Bottom"
                SkinID="PagedList" DataSourceID="CustomUrlDS" DataKeyNames="CustomUrlId" ShowWhenEmpty="False"
                Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="Type" SortExpression="CatalogNodeTypeId">
                        <HeaderStyle HorizontalAlign="center" Width="54px" />
                        <ItemStyle Width="78px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <img src="<%# GetCatalogIconUrl(Container.DataItem) %>" border="0" alt="<%#Eval("CatalogNodeType")%>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="200" />
                        <ItemTemplate>
                            <a href='<%#GetEditUrl(Container.DataItem) %>'>
                            <%#GetCatalogItemName(Container.DataItem)%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Custom Url" SortExpression="Url">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="400" />
                        <ItemTemplate>
                            <%#Eval("Url")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="paging" />
                <EmptyDataTemplate>
                    You do not have any custom URLs configured. To create one, edit the item in your catalog and set the Custom URL field. 
                </EmptyDataTemplate>
            </cb:SortedGridView>
            <asp:ObjectDataSource ID="CustomUrlDS" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="LoadForStore" SelectCountMethod="CountForStore" TypeName="CommerceBuilder.Catalog.CustomUrlDataSource"
                EnablePaging="true" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Catalog.CustomUrl">
            </asp:ObjectDataSource>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
