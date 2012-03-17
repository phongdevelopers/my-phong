<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditShipmentItems.ascx.cs" Inherits="Admin_Orders_Shipments_EditShipmentItems" %>
<div class="sectionHeader">
    <strong><asp:Localize ID="Caption" runat="server" Text="Edit Shipment Items"></asp:Localize></strong>
</div>
<ajax:UpdatePanel ID="OrderItemsAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:GridView ID="OrderItemGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderItemId" 
            DataSourceId="OrderItemDs" AllowSorting="False" OnRowUpdating="OrderItemGrid_RowUpdating" SkinID="PagedList" 
            Width="100%" OnRowDeleted="OrderItemGrid_RowDeleted" EnableViewState="false">
            <Columns>
                <asp:TemplateField HeaderText="SKU" SortExpression="Sku">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%# Eval("Sku") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                    <ItemTemplate>
                        <%#Eval("Name")%>
                        <asp:Literal ID="VariantName" runat="Server" Text='<%#Eval("VariantName", " ({0})")%>' Visible='<%#!String.IsNullOrEmpty((string)Eval("VariantName"))%>'></asp:Literal><br />
                        <asp:PlaceHolder ID="InputPanel" runat="server" Visible='<%#ShowInputPanel(Container.DataItem)%>'>
                            <asp:DataList ID="InputList" runat="server" DataSource='<%#Eval("Inputs") %>' EnableViewState="false">
                                <ItemTemplate>
                                    <asp:Label ID="InputName" Runat="server" Text='<%#Eval("Name", "{0}: ")%>' SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                                    <%#Eval("InputValue")%>
                                </ItemTemplate>
                            </asp:DataList>
                        </asp:PlaceHolder>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Price" SortExpression="Price">
                    <itemstyle horizontalalign="Right" />
                    <ItemTemplate>
                        <asp:Label ID="Price" runat="server" Text='<%# Bind("Price", "{0:lc}") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="EditPrice" runat="server" Text='<%# Bind("Price", "{0:F2}") %>' Columns="4"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="Quantity" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="EditQuantity" runat="server" Text='<%# Bind("Quantity") %>' Columns="4"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Total" SortExpression="ExtendedPrice">
                    <itemstyle horizontalalign="Right" />
                    <ItemTemplate>
                        <%# Eval("ExtendedPrice", "{0:lc}") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle HorizontalAlign="Right" Wrap="false" />
                    <ItemTemplate>
                        <asp:ImageButton ID="EditButton" runat="server" CommandName="Edit" ToolTip="Edit" SkinID="EditIcon" EnableViewState="false" />
                        <asp:ImageButton ID="DeleteButton" runat="server" CommandName="Delete" ToolTip="Delete" SkinID="DeleteIcon" OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' EnableViewState="false" />
                        <asp:HyperLink ID="GiftCertsLink" runat="server" Visible='<%# IsGiftCert(Container.DataItem) %>' NavigateUrl='<%# String.Format("~/Admin/Orders/ViewGiftCertificates.aspx?OrderNumber={0}&OrderId={1}", Eval("Order.OrderNumber"), Eval("OrderId")) %>'>
    					    <asp:Image ID="GiftCertIcon" runat="server" SkinID="GiftCertIcon" AlternateText="Gift Certificate" EnableViewState="false" />
    				    </asp:HyperLink>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="SaveButton" runat="server" CausesValidation="True" ToolTip="Save" CommandName="Update"><asp:Image ID="SaveIcon" runat="server" SkinID="SaveIcon" EnableViewState="false" /></asp:LinkButton>
                        <asp:LinkButton ID="CancelButton" runat="server" CausesValidation="False" ToolTip="Cancel" CommandName="Cancel"><asp:Image ID="CancelIcon" runat="server" SkinID="CancelIcon" EnableViewState="false" /></asp:LinkButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <asp:Localize ID="EmptyMessage" runat="server" Text="There are no items in the order." EnableViewState="false"></asp:Localize>
            </EmptyDataTemplate>
        </asp:GridView>
    </ContentTemplate>
</ajax:UpdatePanel>
<asp:HyperLink ID="AddProductLink" runat="server" Text="Add Product" NavigateUrl="FindProduct.aspx" SkinID="Button"></asp:HyperLink>
<asp:HyperLink ID="AddOtherItemLink" runat="server" Text="Add Other Item" NavigateUrl="AddOther.aspx" SkinID="Button"></asp:HyperLink>
<asp:ObjectDataSource ID="OrderItemDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForOrderShipment" TypeName="CommerceBuilder.Orders.OrderItemDataSource" DataObjectTypeName="CommerceBuilder.Orders.OrderItem" DeleteMethod="Delete" SortParameterName="sortExpression">
    <SelectParameters>
        <asp:QueryStringParameter Name="orderShipmentId" QueryStringField="OrderShipmentId" Type="Object" />
    </SelectParameters>
</asp:ObjectDataSource>


