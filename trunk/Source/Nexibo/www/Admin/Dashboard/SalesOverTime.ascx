<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SalesOverTime.ascx.cs" Inherits="Admin_Dashboard_SalesOverTime" EnableViewState="false" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ register tagprefix="web" namespace="WebChart" assembly="WebChart"%>
<table cellpadding="0" cellspacing="0" width="100%">
   <tr>
        <td valign="top">
            <ComponentArt:TabStrip ID="TabStrip1" runat="server" Width="100%" MultiPageId="MultiPage1" SkinID="HorizontalTab">
                <Tabs>
                    <ComponentArt:TabStripTab Text="Past 7 Days"></ComponentArt:TabStripTab>
                    <ComponentArt:TabStripTab Text="Past 6 Months"></ComponentArt:TabStripTab>
                </Tabs>
            </ComponentArt:TabStrip>
        </td>
    </tr>
    <tr>
        <td class="hTab_MultiPageOuterFrame" valign="top">
                <ComponentArt:MultiPage ID="MultiPage1" CssClass="hTab_MultiPage" runat="server">
                <ComponentArt:PageView runat="server">
                    <web:chartcontrol runat="server" id="SalesByDayChart" Width="440" Height="300" GridLines="Horizontal" HasChartLegend="false" SkinID="Column" CacheDuration="5">
                    </web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <%=string.Format("as of {0:t}", CacheDate)%>  <asp:LinkButton ID="RefreshLink1" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server">
                    <web:chartcontrol runat="server" id="SalesByMonthChart" Width="440" Height="300" GridLines="horizontal" HasChartLegend="false" SkinID="Column" CacheDuration="5">
                    </web:chartcontrol>
                    <div align="center" style="padding:4px;">
                        <%=string.Format("as of {0:t}", CacheDate)%>  <asp:LinkButton ID="RefreshLink2" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
                </ComponentArt:PageView>
            </ComponentArt:MultiPage>
        </td>
    </tr>
</table>