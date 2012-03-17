<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="DailyAbandonedBaskets.aspx.cs" Inherits="Admin_Reports_DailyAbandonedBaskets" Title="Daily Abandoned Baskets"
     %>
<%@ Register Namespace="Westwind.Web.Controls" Assembly="wwhoverpanel" TagPrefix="wwh" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript">
    function ShowBasket(event,Id)
    { 
        BasketHoverLookupPanel.startCallback(event,"BasketId=" + Id.toString(),null,OnError);    
    }
    function HideBasket()
    {
        BasketHoverLookupPanel.hide();
        
        // *** If you don't use shadows, you can fade out
        //LookupPanel.fadeout();
    }
    function OnCompletion(Result)
    {
        //alert('done it!\r\n' + Result);
    }
    function OnError(Result)
    {
        alert("*** Error:\r\n\r\n" + Result.message);    
    }
</script>

    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="Caption" runat="server" Text="Abandoned Baskets"></asp:Localize><asp:Localize
                            ID="ReportCaption" runat="server" Text=" for {0:d}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>
           <%-- <table cellpadding="2" cellspacing="0" class="innerLayout">--%>
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <div class="noPrint" style="text-align: center;">
                        <asp:Label ID="ReportLabel" runat="server" Text="Abandoned Baskets for: " SkinID="FieldHeader"></asp:Label>
                        <uc1:PickerAndCalendar ID="ReportDate" runat="server" />    
                        <asp:Button ID="ProcessButton" runat="server" Text="GO.." OnClick="ProcessButton_Click" />
                        </div>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet">
                        <asp:GridView ID="DailyAbandonedBasketsGrid" runat="server" AutoGenerateColumns="False"
                            Width="100%" ShowFooter="False" SkinID="PagedList" >
                            <Columns>
                                <asp:TemplateField HeaderText="Customer">
                                    <ItemTemplate>
                                        <asp:Label ID="CustomerLabel" runat="server" Text='<%# Eval("Customer") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Items in Basket">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label0" runat="server" Text='<%# Eval("ItemCount", "{0}") %>'></asp:Label>
                                        <a href="../Orders/Create/CreateOrder2.aspx?UID=<%#GetUserId((int)Eval("BasketId")) %>" onmouseover='ShowBasket(event,"<%# Eval("BasketId") %>");' onmouseout='HideBasket();'>
                                            <asp:Image ID="PreviewIcon" runat="server" SkinID="PreviewIcon" />
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Basket Total">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("BasketTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Last Activity">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("LastActivity") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyResult">
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no abandoned baskets for the selected date."></asp:Label>
                                </div>
                            </EmptyDataTemplate>
                       </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <wwh:wwHoverPanel ID="BasketHoverLookupPanel" runat="server" ServerUrl="~/Admin/Reports/BasketSummary.ashx"
        NavigateDelay="250" ScriptLocation="WebResource" Style="display: none; background: white;"
        PanelOpacity="0.89" ShadowOffset="8" ShadowOpacity="0.18" CssClass="HoverBasketPanel">
        <div id="BasketHoverPanelHeader" class="gridheader" style="padding: 2px">
            Basket Contents</div>
        <div id="BasketHoverPanelContent" style="padding: 10px; background: cornsilk;">
        </div>
    </wwh:wwHoverPanel>
</asp:Content>
