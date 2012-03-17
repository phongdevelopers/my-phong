<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="CustomerHistory.aspx.cs" Inherits="Admin_Reports_CustomerHistory" Title="Customers History"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
	<div class="pageHeader">
		<div class="caption">
			<h1><asp:Localize ID="Caption" runat="server" Text="Customer Navigation History"></asp:Localize></h1>
		</div>
	</div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td align="left" valign="top">
                View history for user 
                <asp:HyperLink ID="userLink" runat="server"></asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td align="left" valign="top">
                <cb:SortedGridView ID="PageViewsGrid" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="PageViewId" 
                    DataSourceID="PageViewDs" PageSize="20" DefaultSortExpression="ActivityDate DESC" OnRowDataBound="PageViewsGrid_RowDataBound" SkinID="PagedList">
                    <Columns>
                        <asp:BoundField DataField="ActivityDate" HeaderText="Date" SortExpression="ActivityDate" />
                        <asp:BoundField DataField="RemoteIP" HeaderText="IP" SortExpression="RemoteIP" />
                        <asp:BoundField DataField="RequestMethod" HeaderText="Method" SortExpression="RequestMethod" />
                        <asp:TemplateField HeaderText="Page">
                            <ItemTemplate>
                                <%#GetUri(Container.DataItem)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Time">
                            <ItemTemplate>
                                <%#Eval("TimeTaken", "{0}ms")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referrer" HeaderText="Referrer" SortExpression="Referrer" />
                        <asp:TemplateField HeaderText="Browser">
                            <ItemTemplate>
                                <%#GetBrowserName((string)Eval("UserAgent"))%>
                            </ItemTemplate>
                        </asp:TemplateField>                        
                        <asp:TemplateField HeaderText="Item">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="phCatalogNode" runat="server"></asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </cb:SortedGridView>
                <asp:ObjectDataSource ID="PageViewDs" runat="server" 
                    OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForUser" 
                    TypeName="CommerceBuilder.Reporting.PageViewDataSource" SortParameterName="sortExpression"
                    EnablePaging="True">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="userId" QueryStringField="UserId" Type="Object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

