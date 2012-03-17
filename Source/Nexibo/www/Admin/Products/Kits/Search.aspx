<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Search.aspx.cs" Inherits="Admin_Products_Kits_Search" Title="Manage Kits"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Find Kits"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td style="padding:10px 0px 10px 0px; text-indent:20px;">
                <asp:Label ID="InstructionText" runat="server" Text="Enter all or part of a kit name.  Wildcard characters * and ? are accepted.  You can also leave the name field empty to show all."></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="itemList">
                <ajax:UpdatePanel ID="SearchAjax" runat="server">
                    <ContentTemplate>
                        <table align="center">
                            <tr>
                                <td  align="center" valign="top">
                                    <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton">
                                        <table class="inputForm" align="center">
                                            <tr>
                                                <th class="rowHeader" valign="top">
                                                    <asp:Label ID="SearchNameLabel" runat="server" Text="Name:" />
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="SearchName" runat="server" Text=""></asp:TextBox><br />
                                                    <asp:CheckBox ID="ShowImages" runat="server" Text="show thumbnails" />
                                                </td>
                                                <td valign="top">
                                                    <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <cb:SortedGridView ID="SearchResultsGrid" runat="server" AutoGenerateColumns="false" 
                                        ShowHeader="true" ShowFooter="false" DataKeyNames="ProductId" SkinID="Summary"
                                        DataSourceID="KitDs" Visible="false" Width="100%" AllowSorting="true" DefaultSortExpression="Name"
                                        AllowPaging="true" PageSize="20">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Thumbnail">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="NodeImageLink" runat="server" NavigateUrl='<%# UrlGenerator.GetBrowseUrl((int)Eval("ProductId"), CatalogNodeType.Product, (string)Eval("Name")) %>' Visible='<%# !string.IsNullOrEmpty((string)Eval("ThumbnailUrl")) %>'>
                                                        <asp:Image ID="NodeImage" runat="server" ImageUrl='<%# Eval("ThumbnailUrl") %>' AlternateText='<%# Eval("Name") %>' />
                                                    </asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="Name" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%#Eval("ProductId", "EditKit.aspx?ProductId={0}")%>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Components">
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Components" runat="server" Text='<%#Eval("ProductKitComponents.Count")%>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <asp:Label ID="EmptyMessage" runat="server" Text="There are no kits that match the search text."></asp:Label>
                                        </EmptyDataTemplate>
                                    </cb:SortedGridView>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
                <asp:ObjectDataSource ID="KitDs" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="FindByName" SortParameterName="sortExpression" TypeName="CommerceBuilder.Products.KitDataSource">
                    <SelectParameters>
                        <asp:ControlParameter Name="nameToMatch" ControlID="SearchName" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

