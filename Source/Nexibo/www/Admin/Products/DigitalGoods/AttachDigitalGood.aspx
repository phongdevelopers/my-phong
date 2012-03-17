<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="AttachDigitalGood.aspx.cs" Inherits="Admin_Products_DigitalGoods_AttachDigitalGood" Title="Attach Digital Good" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script type="text/javascript">
       function toggleSelected(checkState)
       {
            var cb = document.getElementsByName("attach");
            if (cb != null)
            {
                for (var i = 0; i < cb.length; i++)
                    cb[i].checked = checkState.checked;
            }
       }    
    </script>	
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Attach Digital Good to {0}"></asp:Localize></h1>
    	</div>
    </div>
    <div style="padding:6px;">
        <asp:Label ID="InstructionText" runat="server" Text="Enter all or part of a digital good name.  Wildcard characters * and ? are accepted.  You can also leave the name field empty to show all."></asp:Label><br /><br />
        <ajax:UpdatePanel ID="SearchAjax" runat="server">
            <ContentTemplate>
                <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton">
                    <table class="inputForm">
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="SearchNameLabel" runat="server" Text="Name:" />
                            </th>
                            <td>
                                <asp:TextBox ID="SearchName" runat="server" Text=""></asp:TextBox>
                            </td>
                            <td nowrap>
                                <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
                                <asp:HyperLink ID="CancelButton" runat="server" Text="Cancel" SkinID="Button" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel><br />
                <cb:SortedGridView ID="SearchResultsGrid" runat="server" AutoGenerateColumns="false" 
                    ShowHeader="true" ShowFooter="false" DataKeyNames="DigitalGoodId" SkinID="Summary"
                    DataSourceID="DigitalGoodDs" Width="100%" AllowSorting="true" DefaultSortExpression="Name"
                    AllowPaging="true" PageSize="20" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Attach">
                            <HeaderTemplate>
                                <input type="checkbox" onclick="toggleSelected(this)" />
                            </HeaderTemplate>
                            <ItemStyle HorizontalAlign="center" />
                            <ItemTemplate>
                                <input type="checkbox" name="attach" value="<%# Eval("DigitalGoodId") %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Name" SortExpression="Name">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:HyperLink ID="Name" runat="server" Text='<%#Eval("Name")%>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="FileName" HeaderText="File" SortExpression="FileName">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Size">
                            <ItemStyle HorizontalAlign="center" />
                            <ItemTemplate>
                                <asp:Label ID="Size" runat="server" Text='<%# GetFileSize((long)Eval("FileSize")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemStyle HorizontalAlign="center" />
                            <ItemTemplate>
                                <asp:HyperLink ID="DownloadDigitalGood" runat="server" NavigateUrl='<%# Eval("DigitalGoodId", "~/Admin/DigitalGoods/Download.ashx?DigitalGoodId={0}") %>' Visible='<%#DGFileExists(Container.DataItem) %>'><asp:Image ID="DownloadIcon" runat="server" SkinID="DownloadIcon" /></asp:HyperLink>
                                <asp:Literal ID="MissingDownloadText" runat="server" Text="[file missing or inaccessible]" EnableViewState="false" Visible='<%#!DGFileExists(Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no digital goods that match the search text."></asp:Label>
                    </EmptyDataTemplate>
                </cb:SortedGridView><br />
                <asp:Button ID="AttachButton" runat="server" Text="Attach Selected" EnableViewState="false" OnClick="AttachButton_Click" />
            </ContentTemplate>
        </ajax:UpdatePanel>
        <asp:ObjectDataSource ID="DigitalGoodDs" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="FindByName" SortParameterName="sortExpression" TypeName="CommerceBuilder.DigitalDelivery.DigitalGoodDataSource">
            <SelectParameters>
                <asp:ControlParameter Name="nameToMatch" ControlID="SearchName" PropertyName="Text" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
</asp:Content>