<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="FindDigitalGoods.aspx.cs" Inherits="Admin_DigitalGoods_FindDigitalGoods" Title="Find Digital Goods" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Find Digital Goods"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td style="padding-bottom:10px;">
                <asp:Label ID="InstructionText" runat="server" Text="Enter all or part of a digital good name.  Wildcard characters * and ? are accepted.  You can also leave the name field empty to show all."></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="itemList">
                <ajax:UpdatePanel ID="SearchAjax" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton">
                            <table class="inputForm">
                                <tr>
                                    <th class="rowHeader">
                                        <asp:Label ID="SearchNameLabel" runat="server" Text="Name Filter:" />
                                    </th>
                                    <td>
                                        <asp:TextBox ID="SearchName" runat="server" Text=""></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <cb:SortedGridView ID="SearchResultsGrid" runat="server" AutoGenerateColumns="false" 
                            ShowHeader="true" ShowFooter="false" DataKeyNames="DigitalGoodId" SkinID="Summary"
                            DataSourceID="DigitalGoodDs" Visible="false" OnRowCommand="SearchResultsGrid_RowCommand" Width="100%" 
                            AllowSorting="true" DefaultSortExpression="Name" AllowPaging="true" PageSize="20">
                            <Columns>
                                <asp:TemplateField HeaderText="Digital Good Name" SortExpression="Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="Name" runat="server" NavigateUrl='<%# string.Format("../DigitalGoods/EditDigitalGood.aspx?DigitalGoodId={0}", Eval("DigitalGoodId")) %>' Text='<%#Eval("Name")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="FileName" HeaderText="File" SortExpression="FileName">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Attached To">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="ProductName" runat="server" Text='<%#Eval("Name")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Size">
                                    <ItemStyle HorizontalAlign="center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Size" runat="server" Text='<%# GetFileSize((int)Eval("FileSize")) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="DownloadDigitalGood" runat="server" NavigateUrl='<%# Eval("DigitalGoodId", "Download.ashx?DigitalGoodId={0}") %>' Visible='<%#DGFileExists(Container.DataItem) %>'><asp:Image ID="DownloadIcon" runat="server" SkinID="DownloadIcon" /></asp:HyperLink>
                                        <asp:Literal ID="MissingDownloadText" runat="server" Text="[file missing or inaccessible]" EnableViewState="false" Visible='<%#!DGFileExists(Container.DataItem) %>'/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyMessage" runat="server" Text="There are no digital goods that match the search text."></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
                <asp:ObjectDataSource ID="DigitalGoodDs" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="FindByName" SortParameterName="sortExpression" TypeName="CommerceBuilder.DigitalDelivery.DigitalGoodDataSource">
                    <SelectParameters>
                        <asp:ControlParameter Name="nameToMatch" ControlID="SearchName" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

