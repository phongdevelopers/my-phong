<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="CreateOrder2.aspx.cs" Inherits="Admin_Orders_Create_CreateOrder2" Title="Place Order" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register Src="~/Admin/Orders/Create/BasketItemDetail.ascx" TagName="BasketItemDetail" TagPrefix="uc" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<asp:HiddenField ID="AnonymousUserId" Value="0" runat="server" />
<div class="pageHeader">
    <div class="caption">
        <h1>
            <asp:Localize ID="Caption" runat="server" Text="Create Order for {0} (Step 2 of 4)"></asp:Localize>
        </h1>
    </div>
</div>
<table cellpadding="0" cellspacing="0" class="innerLayout">
    <tr>
        <td>
            <ajax:UpdatePanel ID="BasketAjax" runat="server" UpdateMode="Conditional">
                <ContentTemplate>                                        
	                <asp:DataList ID="WarningMessageList" runat="server" EnableViewState="False">
	                    <HeaderTemplate><ul></HeaderTemplate>
	                    <ItemTemplate>
	                        <li><asp:Label ID="WarningMessage" runat="server" text="<%# Container.DataItem %>"></asp:Label></li>
	                    </ItemTemplate>
	                    <FooterTemplate></ul></FooterTemplate>
	                </asp:DataList>
                    <asp:GridView ID="BasketGrid" runat="server" AutoGenerateColumns="False"
                        ShowFooter="True" DataKeyNames="BasketItemId" OnRowCommand="BasketGrid_RowCommand"
                        OnDataBound="BasketGrid_DataBound" Width="100%" OnRowDataBound="BasketGrid_RowDataBound"
                        SkinID="Summary">
                        <Columns>
                            <asp:TemplateField>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="DeleteItem" CommandArgument='<%# Eval("BasketItemId") %>' OnClientClick='<%# GetConfirmDelete(Container.DataItem) %>' Visible='<%# CanDeleteBasketItem(Container.DataItem) %>' EnableViewState="true"><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" EnableViewState="false" /></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SKU">
                                <ItemStyle HorizontalAlign="center" Width="120px" />
                                <ItemTemplate>
                                    <%# ProductHelper.GetSKU(Container.DataItem) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Item">
								<HeaderStyle CssClass="columnHeader" HorizontalAlign="left" VerticalAlign="top" />
                                <ItemStyle HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <uc:BasketItemDetail ID="BasketItemDetail1" runat="server" BasketItem="<%# Container.DataItem %>" LinkProducts="true" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price">
                                <ItemStyle HorizontalAlign="right" Width="100px" />
                                <ItemTemplate>
                                    <%# Eval("KitPrice", "{0:lc}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Qty">
                                <ItemStyle HorizontalAlign="center" Width="100px" />
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="ProductQuantityPanel" runat="server" Visible='<%#((OrderItemType)Eval("OrderItemType") == OrderItemType.Product)%>'>
                                        <asp:TextBox ID="Quantity" runat="server" Text='<%# Eval("Quantity") %>' MaxLength="3" Width="40px" onFocus="this.select()"></asp:TextBox>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="OtherQuantityPanel" runat="server" Visible='<%#((OrderItemType)Eval("OrderItemType") != OrderItemType.Product)%>'>
                                        <%#Eval("Quantity")%>
                                    </asp:PlaceHolder>
                                </ItemTemplate>
                                <FooterStyle HorizontalAlign="right" VerticalAlign="top" />
                                <FooterTemplate>
                                    <asp:Label ID="SubtotalLabel" runat="server" Text="Subtotal" SkinID="FieldHeader"></asp:Label><br />
                                    <asp:Label ID="SubtotalHelpText" runat="server" Text="(subtotal does not include tax or shipping)" Font-Bold="false"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total">
                                <ItemStyle HorizontalAlign="right" Width="100px" />
                                <ItemTemplate>
                                    <%# Eval("KitExtendedPrice", "{0:lc}") %>
                                </ItemTemplate>
                                <FooterStyle HorizontalAlign="right" VerticalAlign="top" />
                                <FooterTemplate>
                                    <asp:Label ID="Subtotal" runat="server" Text='<%# String.Format("{0:lc}", GetBasketSubtotal()) %>' SkinID="FieldHeader"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>                      
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Localize ID="EmptyMessage" runat="server" Text="Add one or more products to create the new order."></asp:Localize>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <asp:Panel ID="OrderButtonPanel" runat="server">
                        <asp:Button ID="ClearBasketButton" runat="server" OnClientClick="return confirm('Are you sure you want to clear the order contents?')" Text="Clear Order" OnClick="ClearBasketButton_Click"></asp:Button>
                        <asp:Button ID="UpdateButton" runat="server" OnClick="UpdateButton_Click" Text="Recalculate"></asp:Button>
                        <asp:Button ID="CheckoutButton" runat="server" OnClick="CheckoutButton_Click" Text="Place Order"></asp:Button>
                    </asp:Panel>
                    <hr />                    
                    <asp:Panel ID="FindProductPanel" runat="server" DefaultButton="FindProductSearchButton">
                        <table width="100%">
                            <tr>
                                <td valign="top" width="250px" >
                                    <div class="section">
                                        <div class="header">
                                            <h2 class="product"><asp:localize ID="Localize1" runat="server" Text=" Search for a product to add"></asp:localize></h2>
                                        </div>
                                    <div class="content">
                                    <table class="inputForm" cellpadding="2"  width="100%">
                                        
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Localize ID="FindProductNameLabel" runat="server" text="Name:"></asp:Localize>
                                            </th>
                                            <td>
                                                <asp:TextBox ID="FindProductName" runat="server" Width="150px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Localize ID="FindProductSkuLabel" runat="server" text="SKU:"></asp:Localize>
                                            </th>
                                            <td>
                                                <asp:TextBox ID="FindProductSku" runat="server" Width="150px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:Button ID="FindProductSearchButton" runat="server" Text="Search" OnClick="FindProductSearchButton_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                    </div></div>
                                </td>
                                <td valign="top">
                                    <asp:GridView ID="FindProductSearchResults" runat="server" AutoGenerateColumns="false" 
                                        DataSourceID="AddProductDs" AllowPaging="true" PageSize="10" AllowSorting="true" 
                                        Visible="false" OnRowCommand="FindProductSearchResults_RowCommand" SkinID="PagedList" Width="100%" OnRowCreated="FindProductSearchResults_RowCreated">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemStyle Width="50px" HorizontalAlign="center" />
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="AddButton" runat="server" CommandName="Add" CommandArgument='<%#Eval("ProductId")%>' SkinID="AddIcon" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Sku" HeaderText="SKU" SortExpression="Sku" />
                                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                                            <asp:TemplateField HeaderText="Price" SortExpression="Price">
                                                <ItemStyle HorizontalAlign="right" Width="80px" />
                                                <ItemTemplate>
                                                    <uc:ProductPrice ID="ProductPrice1" runat="server" Product='<%#Container.DataItem%>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <asp:Label ID="FindProductEmptyResultsMessage" runat="server" Text="No products match the search criteria."></asp:Label>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <asp:ObjectDataSource ID="AddProductDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="FindProducts" TypeName="CommerceBuilder.Products.ProductDataSource" SortParameterName="sortExpression">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="FindProductName" Name="name" PropertyName="Text" Type="String" />
                                            <asp:ControlParameter ControlID="FindProductSku" Name="sku" PropertyName="Text" Type="String" />
                                            <asp:Parameter DefaultValue="0" Name="categoryId" Type="Object" />
                                            <asp:Parameter DefaultValue="0" Name="manufacturerId" Type="Object" />
                                            <asp:Parameter DefaultValue="0" Name="vendorId" Type="Object" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:HiddenField ID="AddProductId" runat="server" />
                    <asp:Panel ID="AddDialog" runat="server" Style="display:none;width:600px" CssClass="modalPopup">
                        <asp:Panel ID="AddDialogHeader" runat="server" CssClass="modalPopupHeader" EnableViewState="false">
                            <asp:Localize ID="AddDialogCaption" runat="server" Text="Add Product to Order" EnableViewState="false"></asp:Localize>
                        </asp:Panel>
                        <asp:PlaceHolder ID="AddProductPanel" runat="server" EnableViewState="false">
                            <div style="padding-top:5px;">
                            <table class="inputForm">
                                <tr>
                                    <th class="rowHeader">
                                        <asp:Label ID="AddProductNameLabel" runat="server" Text="Product:"></asp:Label>        
                                    </th>
                                    <td>
                                        <asp:Label ID="AddProductName" runat="server" EnableViewState="false"></asp:Label>
                                    </td>
                                </tr>
                                <asp:PlaceHolder runat="server" id="phOptions" EnableViewState="false"></asp:PlaceHolder>
                                <tr>
                                    <th class="rowHeader">
                                        <asp:Label ID="AddProductPriceLabel" runat="server" Text="Price:"></asp:Label>        
                                    </th>
                                    <td>
                                        <asp:Literal ID="AddProductPrice" runat="server" OnPreRender="AddProductPrice_PreRender" EnableViewState="false"></asp:Literal>
                                        <asp:TextBox ID="AddProductVariablePrice" runat="server" EnableViewState="false" Width="50px" MaxLength="8" Visible="false"></asp:TextBox>
                                        <asp:PlaceHolder ID="phVariablePrice" runat="server" EnableViewState="false"></asp:PlaceHolder>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader">
                                        <asp:Label ID="AddProductQuantityLabel" runat="server" Text="Quantity:"></asp:Label>        
                                    </th>
                                    <td>
                                        <cb:updowncontrol id="AddProductQuantity" runat="server" DownImageUrl="~/images/down.gif" UpImageUrl="~/images/up.gif" Columns="2" Text="1"></cb:updowncontrol>
                                    </td>
                                </tr>
                                <tr id="trInventoryWarning" runat="server" visible="false" enableviewstate="false">
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:Literal ID="InventoryWarningMessage" runat="server" EnableViewState="false"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="AddToBasket" />					
                                        <asp:Button ID="AddProductSaveButton" runat="server" Visible="False" Text="Add To Order" OnClick="AddProductSaveButton_Click" ValidationGroup="AddToBasket"/>&nbsp;
									    <asp:Button ID="AddProductCancelButton" runat="server" Text="Cancel" OnClick="AddProductCancelButton_Click" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>
                    </asp:Panel>
                    <asp:HiddenField ID="DummyCancel" runat="server" />
                    <ajax:ModalPopupExtender ID="AddPopup" runat="server" 
                        TargetControlID="AddProductId"
                        PopupControlID="AddDialog" 
                        BackgroundCssClass="modalBackground"                         
                        CancelControlID="DummyCancel" 
                        DropShadow="false"
                        PopupDragHandleControlID="AddDialogHeader" />
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
    </tr>
</table>
</asp:Content>