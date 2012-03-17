<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderHistoryDialog.ascx.cs" Inherits="Admin_People_Users_OrderHistoryDialog" EnableViewState="False" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<div class="section">
<script type="text/javascript">
    function ShowHoverPanel(event,Id)
    { 
        OrderHoverLookupPanel.startCallback(event,"OrderId=" + Id.toString(),null,OnError);    
    }
    function HideHoverPanel()
    {
        OrderHoverLookupPanel.hide();
        
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
    <div class="header">
        <h2 class="orderhistory"><asp:Localize ID="Caption" runat="server" Text="Order History"></asp:Localize></h2>
    </div>
    <div class="content">   
    <ajax:UpdatePanel ID="OrderHistoryAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>        
        <cb:SortedGridView ID="OrderGrid" runat="server" SkinID="PagedList" Width="100%" AllowPaging="True" PageSize="10" 
            AutoGenerateColumns="False" DataKeyNames="OrderId"             
            OnRowDataBound="OrderGrid_RowDataBound"  EnableViewState="True"  OnPageIndexChanging="OrderGrid_PageIndexChanging">
            <Columns>                
                <asp:TemplateField HeaderText="Order #" SortExpression="OrderNumber">
                    <ItemStyle HorizontalAlign="center" />
                    <ItemTemplate>
                        <asp:Label ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Status" SortExpression="OrderStatusId">
                    <ItemStyle HorizontalAlign="center" Height="30px" />
                    <ItemTemplate>
                        <asp:Label ID="OrderStatus" runat="server" Text='<%# GetOrderStatus(Eval("OrderStatusId")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>                
                <asp:TemplateField HeaderText="Amount" SortExpression="TotalCharges">
                    <ItemStyle HorizontalAlign="center" />
                    <ItemTemplate>
                        <asp:Label ID="Label5" runat="server" Text='<%# Eval("TotalCharges", "{0:lc}") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Date" SortExpression="OrderDate">
                    <ItemStyle HorizontalAlign="center" />
                    <ItemTemplate>
                        <asp:Label ID="Label6" runat="server" Text='<%# Eval("OrderDate", "{0:d}") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Payment">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phPaymentStatus" runat="server"></asp:PlaceHolder>
                        <asp:Label ID="PaymentStatus" runat="server" Text='<%# GetPaymentStatus(Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Shipment">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phShipmentStatus" runat="server"></asp:PlaceHolder>
                        <asp:Label ID="ShipmentStatus" runat="server" Text='<%# Eval("ShipmentStatus") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle HorizontalAlign="center" />
                    <ItemTemplate>
                        <asp:HyperLink ID="DetailsLink" runat="server" Text="details" SkinID="Link" NavigateUrl='<%# String.Format("~/Admin/Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId")) %>' OnMouseOver='<%# Eval("OrderId", "ShowHoverPanel(event, \"{0}\");")%>' OnMouseOut="HideHoverPanel();"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="EmptyMessage" runat="server" Text="No recent orders."></asp:Label>
            </EmptyDataTemplate>
        </cb:SortedGridView>   
        </ContentTemplate>
    </ajax:UpdatePanel>   
    </div>    
</div>   
<wwh:wwHoverPanel ID="OrderHoverLookupPanel"
    runat="server" 
    serverurl="~/Admin/Orders/OrderSummary.ashx"
    Navigatedelay="250"              
    scriptlocation="WebResource"
    style="display: none; background: white;" 
    panelopacity="0.89" 
    shadowoffset="8"
    shadowopacity="0.18"
    PostBackMode="None"
    AdjustWindowPosition="true">
</wwh:wwHoverPanel>