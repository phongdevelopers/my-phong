<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="EditOrderItems.aspx.cs" Inherits="Admin_Orders_Edit_EditOrderItems" Title="Edit Order Items" %>
<%@ Register Src="../../UserControls/OrderItemDetail.ascx" TagName="OrderItemDetail" TagPrefix="uc" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Edit Order Items"></asp:Localize></h1></div>
    </div>
    <br /><asp:Literal ID="EditInstructions" runat="server" Text="If you modify the items in the order, you may need to recalculate taxes or shipping charges. When adding Gift Certificates, Digital Goods, or Subscriptions to an order, you must take steps to manually activate these items. When editing the value of a Gift Certificate type order item, after it has been added to an order, you must manually adjust the respective Gift Certificate value."></asp:Literal><br /><br />
    <ajax:UpdatePanel ID="OrderItemsAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:GridView ID="OrderItemGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderItemId" 
                DataSourceId="OrderItemDs" AllowSorting="False" OnRowUpdating="OrderItemGrid_RowUpdating" SkinID="PagedList" 
                Width="100%" OnRowDeleted="OrderItemGrid_RowDeleted" EnableViewState="false">
                <Columns>
                    <asp:TemplateField HeaderText="SKU" SortExpression="Sku">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Literal ID="Sku" runat="server" Text='<%# ProductHelper.GetSKU(Container.DataItem) %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <ItemTemplate>
                            <uc:OrderItemDetail ID="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="False" LinkProducts="True" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price" SortExpression="Price">
                        <itemstyle horizontalalign="Right" />
                        <ItemTemplate>
                            <asp:Label ID="Price" runat="server" Text='<%# Bind("Price", "{0:lc}") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="EditPrice" runat="server" Text='<%# Bind("Price", "{0:F2}") %>' Width="60px" MaxLength="10"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="Quantity" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="EditQuantity" runat="server" Text='<%# Bind("Quantity") %>' Width="40px" MaxLength="8"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tax Rate" SortExpression="TaxRate" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="TaxRate" runat="server" Text='<%# Bind("TaxRate", "{0:0.####}") %>'></asp:Label>%
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="EditTaxRate" runat="server" Text='<%# Bind("TaxRate", "{0:0.00##}") %>' width="40px" MaxLength="8"></asp:TextBox>%
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
    <asp:Button ID="RecalculateTaxesButton" runat="server" Text="Recalculate Taxes" OnClick="RecalculateTaxesButton_OnClick" OnClientClick="return confirm('All existing tax line items will be removed and taxes will be re-calculated based on current tax rules and settings. Continue?');" EnableViewState="false" />
    <asp:HyperLink ID="AddProductLink" runat="server" Text="Add Product" NavigateUrl="FindProduct.aspx" SkinID="Button"></asp:HyperLink>
    <asp:HyperLink ID="AddOtherItemLink" runat="server" Text="Add Other Item" NavigateUrl="AddOther.aspx" SkinID="Button"></asp:HyperLink>
    <asp:ObjectDataSource ID="OrderItemDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForOrder" TypeName="CommerceBuilder.Orders.OrderItemDataSource" DataObjectTypeName="CommerceBuilder.Orders.OrderItem" DeleteMethod="Delete" SortParameterName="sortExpression">
        <SelectParameters>
            <asp:QueryStringParameter Name="orderId" QueryStringField="OrderId" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

