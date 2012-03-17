<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ActivityByHour.ascx.cs" Inherits="Admin_Dashboard_ActivityByHour" EnableViewState="false" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ register tagprefix="web" namespace="WebChart" assembly="WebChart"%>
<table cellpadding="0" cellspacing="0" width="100%">
   <tr>
        <td valign="top">
            <ComponentArt:TabStrip ID="TabStrip1" runat="server" MultiPageId="MultiPage1" SkinID="HorizontalTab">
                <Tabs>
                    <ComponentArt:TabStripTab Text="Last 24 Hours"></ComponentArt:TabStripTab>
                    <ComponentArt:TabStripTab Text="by Hour"></ComponentArt:TabStripTab>
                    <ComponentArt:TabStripTab Text="by Day"></ComponentArt:TabStripTab>
                    <ComponentArt:TabStripTab Text="by Month"></ComponentArt:TabStripTab>
                </Tabs>
            </ComponentArt:TabStrip>
        </td>
    </tr>
    <tr>
        <td class="hTab_MultiPageOuterFrame" valign="top">
            <ComponentArt:MultiPage ID="MultiPage1" CssClass="hTab_MultiPage" runat="server">
                <ComponentArt:PageView ID="PageView1" runat="server">
                    <web:chartcontrol runat="server" id="Last24HoursChart" Width="440" Height="300" GridLines="Horizontal" HasChartLegend="false" SkinID="CompactColumn" EnableViewState="false"></web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <asp:Localize ID="CacheDate1" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Localize>
                        <asp:LinkButton ID="RefreshLink1" runat="server" Text="refresh" OnClick="RefreshLink_Click" EnableViewState="false"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server">
                    <web:chartcontrol runat="server" id="ViewsByHourChart" Width="440" Height="300" GridLines="Horizontal" HasChartLegend="false" SkinID="CompactColumn" EnableViewState="false"></web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <asp:Localize ID="CacheDate2" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Localize>
                        <asp:LinkButton ID="RefreshLink2" runat="server" Text="refresh" OnClick="RefreshLink_Click" EnableViewState="false"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server">
                    <web:chartcontrol runat="server" id="ViewsByDayChart" Width="440" Height="300" GridLines="Horizontal" HasChartLegend="false" SkinID="CompactColumn" EnableViewState="false"></web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <asp:Localize ID="CacheDate3" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Localize>
                        <asp:LinkButton ID="RefreshLink3" runat="server" Text="refresh" OnClick="RefreshLink_Click" EnableViewState="false"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server">
                    <web:chartcontrol runat="server" id="ViewsByMonthChart" Width="440" Height="300" GridLines="Horizontal" HasChartLegend="false" SkinID="Column" EnableViewState="false"></web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <asp:Localize ID="CacheDate4" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Localize>
                        <asp:LinkButton ID="RefreshLink4" runat="server" Text="refresh" OnClick="RefreshLink_Click" EnableViewState="false"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
            </ComponentArt:MultiPage>
        </td>
    </tr>
</table>