<%@ Page Language="C#" MasterPageFile="../Order.master" CodeFile="ViewTrackingNumber.aspx.cs" Inherits="Admin_Orders_Shipments_ViewTrackingNumber" Title="View Tracking Details" %>
<%@ Import Namespace="CommerceBuilder.Shipping.Providers"%>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:Panel runat="server" ID="DetailsPanel">
    <table class="contentPanel" cellpadding="4" cellspacing="0" width="500px" align="center">
        <caption><asp:Label ID="Caption" runat="server" Text="Tracking Details - Order Number {0}"></asp:Label></caption>
        <tr>
            <td colspan="2">
                <br />
                <asp:Label ID="ShipmentNumberLabel" runat="server" Text="Shipment Number:"></asp:Label>
                <asp:Label ID="ShipmentNumber" runat="server" Text="{0} of {1}"></asp:Label><br />
                <asp:Label ID="ShippingMethodLabel" runat="server" Text="Shipping Method:"></asp:Label>
                <asp:Label ID="ShippingMethod" runat="server" Text=""></asp:Label><br />
                <asp:Label ID="TrackingNumberDataLabel" runat="server" Text="Tracking Number:"></asp:Label>
                <asp:Label ID="TrackingNumberData" runat="server" Text=""></asp:Label><br />
                <asp:Label ID="PackageCountLabel" runat="server" Text="Packages:"></asp:Label>
                <asp:Label ID="PackageCount" runat="server" Text=""></asp:Label><br />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:DataList ID="PackageList" runat="server">
                    <ItemTemplate>
                        <asp:Label ID="PackageNumberLabel" runat="server" Text="Package #" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="PackageNumber" runat="server" Text='<%#Container.ItemIndex+1%>'></asp:Label><br />
                        <asp:Label ID="StatusLabel" runat="server" Text="Current Status:" SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="Status" runat="server" Text='<%#Eval("StatusName")%>'></asp:Label><br />
                        <asp:GridView ID="ActivityGrid" runat="server" AutoGenerateColumns="false" DataSource='<%#Eval("ActivityCollection")%>'>
                            <Columns>
                                <asp:BoundField DataField="ActivityDate" headertext="Date" />
                                <asp:BoundField DataField="City" headertext="City" />
                                <asp:BoundField DataField="Province" headertext="Province" />
                                <asp:BoundField DataField="CountryCode" headertext="Country" />
                                <asp:BoundField DataField="Status" headertext="Status" />
                                <asp:BoundField DataField="Comment" headertext="Comment" />
                            </Columns>
                        </asp:GridView><br />
                    </ItemTemplate>
                </asp:DataList>
            </td>
        </tr>
    </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="LinkPanel">
        <p>
           <asp:Label runat="server" ID="TrackingLinkLabel" Text="Visit the following link for full tracking details"></asp:Label><br />
           <asp:HyperLink runat="server" ID="TrackingLink" NavigateUrl="" Target="_blank" Text=""></asp:HyperLink>
        </p>
    </asp:Panel>
</asp:Content>
