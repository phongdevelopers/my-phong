<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PopularCategories.ascx.cs" Inherits="Admin_Dashboard_PopularCategories" EnableViewState="false" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ register tagprefix="web" namespace="WebChart" assembly="WebChart"%>
<table cellpadding="0" cellspacing="0" width="100%">
   <tr>
        <td valign="top" width="15">
            <ComponentArt:TabStrip ID="TabStrip1" runat="server" Width="100%" MultiPageId="MultiPage1" SkinID="HorizontalTab">
                <Tabs>
                    <ComponentArt:TabStripTab Text="by Views"></ComponentArt:TabStripTab>
                    <ComponentArt:TabStripTab Text="data"></ComponentArt:TabStripTab>
                </Tabs>
            </ComponentArt:TabStrip>
        </td>
    </tr>
    <tr>
        <td class="hTab_MultiPageOuterFrame" valign="top">
            <ComponentArt:MultiPage ID="MultiPage1" CssClass="hTab_MultiPage" runat="server">
                <ComponentArt:PageView runat="server">
                    <web:chartcontrol runat="server" id="ViewsChart" Width="440" Height="200" GridLines="Horizontal" HasChartLegend="false" YValuesFormat="{0:N0}" SkinID="HorizontalBar" CacheDuration="5"></web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <asp:Literal ID="CacheDate1" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Literal> 
                        <asp:LinkButton ID="RefreshLink1" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server">
                    <asp:GridView ID="ViewsGrid" runat="server" SkinID="Summary" AutoGenerateColumns="false" Width="100%">
                        <Columns>
                            <asp:TemplateField HeaderText="Category">
                                <HeaderStyle HorizontalAlign="left" />
                                <ItemStyle HorizontalAlign="left" />
                                <ItemTemplate>
                                    <asp:HyperLink ID="CategoryLink" NavigateUrl='<%#((ICatalogable)Eval("Key")).NavigateUrl%>' runat="server"><%#((ICatalogable)Eval("Key")).Name%></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Value" HeaderText="Views" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </asp:GridView>
                    <div align="center" style="padding:4px;">
                        <asp:Literal ID="CacheDate2" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Literal> 
                        <asp:LinkButton ID="RefreshLink2" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
            </ComponentArt:MultiPage>
        </td>
    </tr>
</table>