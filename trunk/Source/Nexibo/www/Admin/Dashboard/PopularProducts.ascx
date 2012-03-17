<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PopularProducts.ascx.cs" Inherits="Admin_Dashboard_PopularProducts" EnableViewState="false" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ register tagprefix="web" namespace="WebChart" assembly="WebChart"%>
<table cellpadding="0" cellspacing="0" width="100%">
   <tr>
        <td valign="top">
            <ComponentArt:TabStrip ID="TabStrip1" runat="server" Width="100%" MultiPageId="MultiPage1" SkinID="HorizontalTab">
                <Tabs>
                    <ComponentArt:TabStripTab Text="By Sales"></ComponentArt:TabStripTab>
                    <ComponentArt:TabStripTab Text="Sales Data"></ComponentArt:TabStripTab>
                    <ComponentArt:TabStripTab Text="By Views"></ComponentArt:TabStripTab>
                    <ComponentArt:TabStripTab Text="Views Data"></ComponentArt:TabStripTab>
                </Tabs>
            </ComponentArt:TabStrip>
        </td>
    </tr>
    <tr>
        <td class="hTab_MultiPageOuterFrame" valign="top">
            <ComponentArt:MultiPage ID="MultiPage1" CssClass="vTab_MultiPage" runat="server">
                <ComponentArt:PageView runat="server">
                    <web:chartcontrol runat="server" id="SalesChart" Width="440" Height="300" GridLines="Horizontal" HasChartLegend="false" YValuesFormat="{0:N0}" SkinID="HorizontalBar" CacheDuration="5"></web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <asp:Literal ID="CacheDate1" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Literal> 
                        <asp:LinkButton ID="RefreshLink1" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server">
                    <asp:GridView ID="SalesGrid" runat="server" SkinID="Summary" AutoGenerateColumns="false" Width="100%">
                        <Columns>
                            <asp:TemplateField HeaderText="Product">
                                <HeaderStyle HorizontalAlign="left" />
                                <ItemStyle HorizontalAlign="left" />
                                <ItemTemplate>
                                    <asp:HyperLink ID="ProductLink" runat="server" NavigateUrl='<%# Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}") %>' Text='<%# Eval("Name") %>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sales">
                                <ItemStyle HorizontalAlign="Right" />
                                <ItemTemplate>
                                    <asp:Label ID="TotalPrice" runat="server" Text='<%# Eval("TotalPrice", "{0:lc}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="TotalQuantity" HeaderText="Quantity" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </asp:GridView>
                    <div align="center" style="padding:4px;">
                        <asp:Literal ID="CacheDate2" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Literal> 
                        <asp:LinkButton ID="RefreshLink2" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server">
                    <web:chartcontrol runat="server" id="ViewsChart" Width="440" Height="300" GridLines="Horizontal" HasChartLegend="false" SkinID="HorizontalBar" CacheDuration="5">
                    </web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <asp:Literal ID="CacheDate3" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Literal> 
                        <asp:LinkButton ID="RefreshLink3" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server">
                    <asp:GridView ID="ViewsGrid" runat="server" SkinID="Summary" AutoGenerateColumns="false" Width="100%">
                        <Columns>
                            <asp:TemplateField HeaderText="Product">
                                <HeaderStyle HorizontalAlign="left" />
                                <ItemStyle HorizontalAlign="left" />
                                <ItemTemplate>
                                    <asp:HyperLink ID="ProductLink" NavigateUrl='<%#((ICatalogable)Eval("Key")).NavigateUrl%>' runat="server"><%#((ICatalogable)Eval("Key")).Name%></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Value" HeaderText="Views" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </asp:GridView>
                    <div align="center" style="padding:4px;">
                        <asp:Literal ID="CacheDate4" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Literal> 
                        <asp:LinkButton ID="RefreshLink4" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
            </ComponentArt:MultiPage>
        </td>
    </tr>
</table>