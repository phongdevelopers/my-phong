<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="SplitShipment.aspx.cs" Inherits="Admin_Orders_Shipments_SplitShipment" Title="Split Shipment" EnableViewState="false" %>

 <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server"> 
<div class="pageHeader"> 
    <div class="caption"> 
        <h1><asp:Label ID="Caption" runat="server" Text="Split Shipment #{0}"></asp:Label></h1> 
    </div> 
</div>
<table class="inputForm" border="0" cellpadding="3"> 
    <tr> 
        <td colspan="2">
            Select the destination for items in this shipment:
        </td> 
    </tr> 
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary" runat="server" />
            <asp:GridView ID="ShipmentItems" runat="server" ShowHeader="true" 
                AutoGenerateColumns="false" Width="100%" SkinID="PagedList">
                <Columns>
                    <asp:TemplateField HeaderText="Quantity">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Eval("Quantity")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Move">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HiddenField ID="Id" runat="server" Value='<%#Eval("OrderItemId")%>' />
                            <asp:TextBox ID="MoveQty" runat="server" Text='<%#Eval("Quantity")%>' Width="30px"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Move To">
                        <ItemTemplate>
                            <asp:DropDownList ID="Shipment" runat="server"></asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
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
                    <asp:TemplateField HeaderText="Price">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# Eval("Price", "{0:lc}") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="UpdateButton" runat="server" Text="Move Items" OnClick="UpdateButton_Click" />
            <asp:HyperLink ID="CancelLink" runat="server" Text="Cancel" NavigateUrl="Default.aspx" SkinID="Button" />
            <asp:PlaceHolder ID="phQuantityValidation" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
</table> 
</asp:Content> 
