<%@ Page Language="C#" MasterPageFile="~/Admin/Catalog/Webpage-Link.master" CodeFile="EditWebpageCategories.aspx.cs" Inherits="Admin_Catalog_EditWebpageCategories" Title="Edit Webpage Categories" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls"
    TagPrefix="cb" %>
<%@ Register Src="~/Admin/Catalog/WebpageMenu.ascx" TagName="WebpageMenu" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PrimarySidebarContent" runat="Server">
    <uc:WebpageMenu ID="WebpageMenu1" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:localize id="Caption" runat="server" text="Categories for {0}"></asp:localize>
            </h1>
        </div>
    </div>
    <asp:label id="SuccessMessage" runat="server" text="" skinid="GoodCondition" visible="false"></asp:label>
    <asp:label id="FailureMessage" runat="server" text="" skinid="ErrorCondition" visible="false"></asp:label>
    <div style="clear: both; margin-top: 6px;">
        <componentart:treeview id="CategoryTree" runat="server" enableviewstate="true">
        </componentart:treeview>
    </div>
    <br />
    <asp:button id="SaveButton" runat="server" text="Save" onclick="SaveButton_Click" />
</asp:Content>
