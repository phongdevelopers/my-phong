<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="ReturnShipment.aspx.cs" Inherits="Admin_Orders_Shipments_ReturnShipment" Title="Return Order Shipment" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Import Namespace="CommerceBuilder.Shipping.Providers" %>
<%@ Register Src="~/Admin/Orders/Shipments/ReturnItemDialog.ascx" TagName="ReturnItemDialog" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
 <div class="pageHeader">
    <div class="caption">
        <h1><asp:Localize ID="Caption" runat="server" Text="Return Shipment #{0}"></asp:Localize></h1>
    </div>
</div>
<ajax:UpdatePanel ID="OrderItemsAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

    <table class="inputForm" cellpadding="4" cellspacing="0" style="margin-top:8px;" width="100%">
        <tr>
            <th colspan="2">                
                <asp:Label ID="InstructionsText" runat="server" Text="If you modify/return the order items in a shipment, you may need to manually adjust any taxes, shipping charges, or payments already made."></asp:Label>
            </th>
        </tr>
        <%--<tr>--%>
        <tr class="sectionHeader">
            <th colspan="2">                
                <asp:Label ID="OrderItemsLabel" runat="server" Text="Shipment Items:"></asp:Label>
            </th>
        </tr>
        <tr>
            <td colspan="2">
                <asp:GridView ID="ShipmentItems" runat="server" ShowHeader="true" 
                    AutoGenerateColumns="false" Width="100%" SkinID="PagedList" OnPreRender="ShipmentItems_OnPreRender" OnRowCommand="ShipmentItems_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Sku">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <%#Eval("Sku")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <%#Eval("Name")%>
                                <asp:Literal ID="VariantName" runat="Server" Text='<%#Eval("VariantName", " ({0})")%>' Visible='<%#!String.IsNullOrEmpty((string)Eval("VariantName"))%>'></asp:Literal><br />
                                <asp:Panel ID="InputPanel" runat="server" Visible='<%#(((ICollection)Eval("Inputs")).Count > 0)%>'>
                                    <asp:DataList ID="InputList" runat="server" DataSource='<%#Eval("Inputs") %>'>
                                        <ItemTemplate>
                                            <asp:Label ID="InputName" Runat="server" Text='<%#Eval("Name") + ":"%>' SkinID="fieldheader"></asp:Label>
                                            <asp:Label ID="InputValue" Runat="server" Text='<%#Eval("InputValue")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </asp:Panel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quantity">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:HiddenField ID="Id" runat="server" Value='<%#Eval("OrderItemId")%>' />
                                <asp:Label ID="Qty" runat="server" Text='<%#Eval("Quantity")%>' Width="50px" MaxLength="4" ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Price">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="Price" runat="server" Text='<%# Eval("Price", "{0:F2}") %>' Width="60px" MaxLength="10"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total">
                            <ItemStyle HorizontalAlign="Right" />
                            <ItemTemplate>
                                <asp:Label ID="ExtendedPrice" runat="server" Text='<%# Eval("ExtendedPrice", "{0:lc}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Return">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Button ID="ReturnButton" runat="server" Text="Return" CommandName="Return" CommandArgument='<%#Eval("OrderItemId")%>' /> 
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no items to return in this shipment."></asp:Label>
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>    
        <tr>
            <td colspan="2">
                <uc1:ReturnItemDialog ID="ReturnItemDialog1" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>            
            <td colspan="2" align="right">             
                <asp:HyperLink ID="CancelLink" runat="server" Text="Back" NavigateUrl="Default.aspx" SkinID="Button" />
            </td>
        </tr>
    </table>
    </ContentTemplate>
 </ajax:UpdatePanel>    
</asp:Content>

