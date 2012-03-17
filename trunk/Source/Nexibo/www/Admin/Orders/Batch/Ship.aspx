<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Ship.aspx.cs" Inherits="Admin_Orders_Batch_Ship" %>
<%@ Register Namespace="Westwind.Web.Controls" assembly="wwhoverpanel" TagPrefix="wwh" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script type="text/javascript">
    function ShowHoverPanel(event,Id)
    { 
        ShipmentHoverLookupPanel.startCallback(event,"OrderShipmentId=" + Id.toString(),null,OnError);    
    }
    function HideHoverPanel()
    {
        ShipmentHoverLookupPanel.hide();
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
<asp:Panel ID="UpdatePanel" runat="server">
    <div class="pageHeader">
	    <div class="caption">
		    <h1><asp:Localize ID="Caption" runat="server" Text="Ship Orders"></asp:Localize></h1>
        </div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                You have requested to ship these orders: <asp:Label ID="OrderList" runat="server"></asp:Label>
                <asp:Panel ID="InvalidOrdersPanel" runat="server">
                    <br /><br />
                    WARNING: These orders do not currently have unshipped shipments: <asp:Label ID="InvalidOrderList" runat="server"></asp:Label>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="ShipmentGrid" runat="server" SkinID="PagedList" Width="100%" AllowPaging="False"
                    AllowSorting="false" AutoGenerateColumns="False" DataKeyNames="OrderShipmentId" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Order #">
                            <ItemStyle HorizontalAlign="center" />
                            <ItemTemplate>
                                <asp:Label ID="OrderNumber" runat="server" Text='<%# Eval("Order.OrderNumber") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipment #">
                            <ItemStyle HorizontalAlign="center" Height="30px" />
                            <ItemTemplate>
                                <asp:HyperLink ID="ShipmentNumber" runat="server" Text='<%# Eval("ShipmentNumber") %>' SkinID="Link" NavigateUrl="#" OnMouseOver='<%# Eval("OrderShipmentId", "ShowHoverPanel(event, \"{0}\");")%>' OnMouseOut="HideHoverPanel();"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Bill To">
                            <ItemStyle HorizontalAlign="center" />
                            <ItemTemplate>
                                <asp:Label ID="BillToName" runat="server" Text='<%# string.Format("{1}, {0}", Eval("Order.BillToFirstName"), Eval("Order.BillToLastName")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ship To">
                            <ItemStyle HorizontalAlign="center" />
                            <ItemTemplate>
                                <asp:Label ID="ShipToName" runat="server" Text='<%# string.Format("{1}, {0}", Eval("ShipToFirstName"), Eval("ShipToLastName")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ship Date">
                            <ItemStyle HorizontalAlign="center" />
                            <ItemTemplate>
                                <asp:TextBox ID="ShipDate" runat="server" MaxLength="10" Text='<%# LocaleHelper.LocalNow.ToString("MM/dd/yyyy") %>' Width="80px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tracking #">
                            <ItemStyle HorizontalAlign="center" />
                            <ItemTemplate>
                                <asp:DropDownList ID="ShipGateway" runat="server" DataTextField="Name" DataValueField="ShipGatewayId" AppendDataBoundItems="true" DataSourceID="ShipGatewayDs">
                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="TrackingNumber" runat="server" MaxLength="50" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="ShipGatewayDs" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="LoadForStore" TypeName="CommerceBuilder.Shipping.ShipGatewayDataSource">
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HyperLink ID="BackButton" runat="server" Text="&laquo; Back" SkinID="Button" NavigateUrl="../Default.aspx" />
                <asp:Button ID="ShipButton" runat="server" Text="Ship Orders" OnClick="ShipButton_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="ConfirmPanel" runat="server" Visible="false">
	<div class="pageHeader">
		<div class="caption">
			<h1><asp:Localize ID="Caption2" runat="server" Text="Ship Orders"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                The selected orders have been shipped.
            </td>
        </tr>
        <tr>
            <td>
                <asp:HyperLink ID="FinishButton" runat="server" Text="Finish &raquo;" SkinID="Button" NavigateUrl="../Default.aspx" /><br /><br />
            </td>
        </tr>
    </table>
</asp:Panel>
<wwh:wwHoverPanel ID="ShipmentHoverLookupPanel"
    runat="server" 
    serverurl="~/Admin/Orders/ShipmentSummary.ashx"
    Navigatedelay="250"              
    scriptlocation="WebResource"
    style="display: none; background: white;" 
    panelopacity="0.89" 
    shadowoffset="8"
    shadowopacity="0.18"
    PostBackMode="None"
    AdjustWindowPosition="true">
</wwh:wwHoverPanel>
</asp:Content>