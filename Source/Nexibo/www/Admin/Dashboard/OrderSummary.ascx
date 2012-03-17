<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderSummary.ascx.cs" Inherits="Admin_Dashboard_OrderSummary" EnableViewState="false" %>
<ajax:UpdatePanel ID="BasketPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="contentSection">
            <asp:GridView ID="OrderGrid" runat="server" AutoGenerateColumns="false" ShowHeader="True" 
                Width="100%" OnRowCommand="OrderGrid_RowCommand" EnableViewState="false" SkinID="Summary">
                <Columns>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label ID="Status" runat="Server" Text='<%# ((OrderStatusDetail)Container.DataItem).Status %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Today" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="SearchDay" runat="server" Text='<%# ((OrderStatusDetail)Container.DataItem).DayCount %>' NavigateUrl='<%# GetSearchUrl(Container.DataItem, 0) %>'></asp:HyperLInk>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Last 30" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="Search30" runat="server" Text='<%# ((OrderStatusDetail)Container.DataItem).Last30Count %>' NavigateUrl='<%# GetSearchUrl(Container.DataItem, 5) %>'></asp:HyperLInk>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Last 90" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="Search90" runat="server" Text='<%# ((OrderStatusDetail)Container.DataItem).Last90Count %>' NavigateUrl='<%# GetSearchUrl(Container.DataItem, 7) %>'></asp:HyperLInk>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <div style="text-align:center;margin-top:6px;">
                <asp:Label ID="CurrentTime" runat="server" Text="as of {0:T}"></asp:Label>&nbsp;|&nbsp;
                <asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click" Text="Refresh Data"></asp:LinkButton>&nbsp;|&nbsp;
                <asp:HyperLink ID="SearchLink" runat="server" NavigateUrl="~/Admin/Orders/Default.aspx" Text="Search Orders"></asp:HyperLink>
            </div>
        </div>
		<table cellpadding="2" cellspacing="0" border="0" width="100%" class="inputForm">
            <tr>
                <th class="colHeader">
                    <asp:Localize ID="RecentOrdersCaption" runat="server" Text="Recent Orders"></asp:Localize>
                </th>
				<th class="colHeader" align="center">
                    <asp:Localize ID="ViewOrderNumberCaption" runat="server" Text="View Order Number"></asp:Localize>
				</th>
            </tr>
            <tr>
                <td>
                    <asp:DataList ID="RecentOrdersList" runat="server" Width="100%">
                        <ItemTemplate>
                           <asp:HyperLink ID="OrderLink" runat="server" Text='<%#string.Format("Order #{0} for {1:lc} on {2:g}", Eval("OrderNumber"), Eval("TotalCharges"), Eval("OrderDate"))%>' NavigateUrl='<%#String.Format("~/Admin/Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:DataList>
                </td>
				<td valign="top" align="center">
				    <asp:Panel ID="ViewOrderPanel" runat="server" DefaultButton="ViewOrderButton">
                        <asp:TextBox ID="OrderNumber" runat="server" Width="40px" ValidationGroup="OrderSummary"></asp:TextBox>
                        <asp:Button ID="ViewOrderButton" runat="server" ValidationGroup="OrderSummary" OnClick="ViewOrderButton_Click" Text="Go" /><br/>
				    </asp:Panel>
				</td>
			</tr>
		</table>
    </ContentTemplate>
</ajax:UpdatePanel>
