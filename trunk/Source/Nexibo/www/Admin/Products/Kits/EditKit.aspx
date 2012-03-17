<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="EditKit.aspx.cs" Inherits="Admin_Products_Kits_EditKit" Title="Edit Kit" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="PageAjax" runat="server">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1><asp:Localize ID="Caption" runat="server" Text="Kitting: {0}" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>
            <asp:PlaceHolder ID="NewKitPanel" runat="server" Visible="false" EnableViewState="false">
                <div class="section">
                    <div class="content">
                        <asp:Localize ID="NewKitMessage" runat="server">
                            In order to create a kit from this product, you must add at least one component.
                        </asp:Localize><br /><br />
                        <asp:Button ID="AddComponentButton2" runat="server" Text="Add Component" OnClick="AddComponentButton_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="ExistingKitPanel" runat="server" EnableViewState="false">
                <table width="350">
                    <tr>
                        <th>
                            <asp:Label ID="PriceRangeLabel" runat="server" Text="Price Range:"></asp:Label>
                        </th>
                        <td>
                            <asp:Label ID="PriceRange" runat="server" Text=""></asp:Label>
                        </td>
                        <th>
                            <asp:Label ID="DefaultPriceLabel" runat="server" Text="Default:"></asp:Label>
                        </th>
                        <td>
                            <asp:Label ID="DefaultPrice" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Label ID="WeightRangeLabel" runat="server" Text="Weight Range:"></asp:Label>
                        </th>
                        <td>
                            <asp:Label ID="WeightRange" runat="server" Text=""></asp:Label>
                        </td>
                        <th>
                            <asp:Label ID="DefaultWeightLabel" runat="server" Text="Default:"></asp:Label>
                        </th>
                        <td>
                            <asp:Label ID="DefaultWeight" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Localize ID="ItemizedDisplayLabel" runat="server" Text="Invoice Style:"></asp:Localize>
                        </th>
                        <td>
                            <asp:LinkButton ID="ItemizedDisplay" runat="server" Text="Bundled"></asp:LinkButton>
                        </td>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                </table><br />
                <asp:Button ID="AddComponentButton" runat="server" Text="Add Component" OnClick="AddComponentButton_Click" />
                <asp:HyperLink ID="SortComponents" runat="server" Text="Sort Components" NavigateUrl="SortComponents.aspx" SkinID="Button" EnableViewState="false" /><br /><br />
                <asp:DataList ID="ComponentList" runat="server" DataKeyField="KitComponentId" OnItemCommand="ComponentList_ItemCommand" Width="100%" EnableViewState="false">
                    <ItemTemplate>
                        <table width="100%" cellpadding="4" cellspacing="0" style="margin-top:4px;margin-right:30px;">
                            <tr class="sectionHeader">
                                <td colspan="2">
                                    <asp:Localize ID="ComponentName" runat="server" Text='<%#Eval("Name")%>'></asp:Localize>
                                    &nbsp;(<asp:Localize ID="ComponentType" runat="server" Text='<%#FixInputTypeName(Eval("InputType").ToString())%>'></asp:Localize>)
                                    &nbsp;&nbsp;
                                    <asp:HyperLink ID="EditComponent" runat="server" NavigateUrl='<%#string.Format("EditComponent.aspx?CategoryId={0}&ProductId={1}&KitComponentId={2}", _CategoryId, _ProductId, Eval("KitComponentId"))%>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" ToolTip="Edit" /></asp:HyperLink>
                                            <asp:ImageButton ID="DeleteComponent" runat="server" visible='<%#(int)Eval("ProductKitComponents.Count") == 1%>' OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete the component {0}?\");") %>' CommandArgument='<%#Container.ItemIndex%>' CommandName="Delete" SkinID="DeleteIcon" ToolTip="Delete" />
                                    <asp:ImageButton ID="DeleteSharedComponent" runat="server" visible='<%#(int)Eval("ProductKitComponents.Count") > 1%>' CommandArgument='<%#Container.ItemIndex%>' CommandName="DeleteShared" SkinID="DeleteIcon" ToolTip="Delete" />
                                </td>
                            </tr>
                            <tr id="SharedRow" runat="server" visible='<%#(int)Eval("ProductKitComponents.Count") > 1%>'>
                                <td colspan="2">
                                    <asp:Label ID="SharedLabel" runat="server" Text="This component is shared by more than one kit."></asp:Label>
                                    &nbsp;<asp:HyperLink ID="SharedDetails" runat="server" Text="details" NavigateUrl='<%#string.Format("ViewComponent.aspx?CategoryId={0}&ProductId={1}&KitComponentId={2}", _CategoryId, _ProductId, Eval("KitComponentId"))%>'></asp:HyperLink>
                                    &nbsp;<asp:LinkButton ID="BranchComponent" runat="server" Text="branch" CommandName="Branch" CommandArgument='<%#Container.ItemIndex%>'></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:GridView ID="KitProductList" runat="server" DataSource='<%#Eval("KitProducts")%>' OnRowCommand="KitProductList_RowCommand" 
                                        AutoGenerateColumns="false" GridLines="None" ShowHeader="true" SkinID="PagedList" EnableViewState="true" Width="100%">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Qty">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Quantity" runat="Server" Text='<%#Eval("Quantity")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Item">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="NameLink" runat="Server" Text='<%#Server.HtmlEncode(Eval("DisplayName").ToString())%>' NavigateUrl='<%#String.Format("../EditProduct.aspx?ProductId={0}", Eval("ProductId"))%>'></asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Price">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Price" runat="Server" Text='<%#Eval("CalculatedPrice", "{0:lc}")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Weight">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Weight" runat="Server" Text='<%# string.Format("{0} {1}", Eval("CalculatedWeight", "{0:F2}"), Token.Instance.Store.WeightUnit)%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Selected">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Label ID="IsSelected" runat="Server" Text="X" Visible='<%#((bool)Eval("IsSelected"))%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemStyle HorizontalAlign="Right" Width="60px" />
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#GetEditPartLink(Container.DataItem)%>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" ToolTip="Edit" /></asp:HyperLink>
                                                    <asp:ImageButton ID="DeleteButton" runat="server" SkinID="DeleteIcon" CommandName="DoDelete" CommandArgument='<%#Eval("KitProductId")%>' OnClientClick="javascript: return confirm('Are you sure you wish to delete the kit product?');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <div style="margin:10px 0px;">
                                                <asp:HyperLink ID="AddProduct" runat="server" NavigateUrl='<%# string.Format("AddKitProducts.aspx?CategoryId={0}&ProductId={1}&KitComponentId={2}", PageHelper.GetCategoryId(), _ProductId, Eval("KitComponentId"))%>' Text="Add Product(s)" SkinID="Button"></asp:HyperLink>
                                        <asp:HyperLink ID="SortProducts" runat="server" NavigateUrl='<%# string.Format("SortParts.aspx?ProductId={0}&KitComponentId={1}", _ProductId, Eval("KitComponentId"))%>' Visible='<%# (((KitComponent)Container.DataItem).KitProducts.Count > 1) %>' Text="Sort Products" SkinID="Button"></asp:HyperLink>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:DataList>
            </asp:PlaceHolder>
            <asp:Panel ID="ItemizedDisplayDialog" runat="server" Style="display:none;width:550px" CssClass="modalPopup">
                <asp:Panel ID="ItemizedDisplayHeader" runat="server" CssClass="modalPopupHeader">
                    <asp:Localize ID="ItemizedDisplayCaption" runat="server" Text="Invoice Display Style"></asp:Localize>
                </asp:Panel>
                <div style="padding-top:5px;">
                    <asp:Localize ID="ItemizedDisplayHelpText" runat="server">
                        You can choose to show this kit as a single line item in the basket and invoice pages (bundle) or you can choose to itemize the contents.  Regardless of your choice, "included hidden" items are never displayed in itemized fashion.
                    </asp:Localize>
                    <asp:RadioButtonList ID="ItemizedDisplayOption" runat="server">
                        <asp:ListItem Text="Bundle" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Itemized" Value="1"></asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Button ID="ItemizedDisplayOkButton" runat="server" Text="OK" OnClick="ItemizedDisplayOkButton_Click" />
                </div>
            </asp:Panel>
            <asp:Panel ID="AddComponentDialog" runat="server" Style="display:none;width:550px" CssClass="modalPopup">
                <asp:Panel ID="AddComponentDialogHeader" runat="server" CssClass="modalPopupHeader">
                    Add Kit Component
                </asp:Panel>
                <div style="padding-top:5px;">
                    <table class="inputForm" cellpadding="3">
                        <tr>
                            <td colspan="2">
                                A kit must contain at least one component which can contain one or more products.
                                <asp:ValidationSummary ID="AddComponentValidationSummary" runat="server" ValidationGroup="AddComponent" />
                            </td>
                        </tr>
                        <tr>   
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="AddComponentNameLabel" runat="server" Text="Name:" CssClass="toolTip" ToolTip="Name of the component."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:TextBox ID="AddComponentName" runat="server" Text="" Width="300px" MaxLength="255"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="AddComponentNameValidator" runat="server" Text="*" Display="Dynamic" ErrorMessage="Name is required." ControlToValidate="AddComponentName" ValidationGroup="AddComponent"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="AddComponentInputTypeLabel" runat="server" Text="Input Type:" ToolTip="Determines the type of input control that will be used for the products in this component." AssociatedControlID="AddComponentInputTypeId"></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:DropDownList ID="AddComponentInputTypeId" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <asp:Button ID="AddComponentSaveButton" runat="server" Text="Next" OnClick="AddComponentSaveButton_Click" ValidationGroup="AddComponent" Width="60px" />
                                <asp:Button ID="AddComponentCancelButton" runat="server" Text="Cancel" OnClick="AddComponentCancelButton_Click" CausesValidation="false" Width="60px" /><br />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <asp:HiddenField ID="FakeAddComponentSaveButton" runat="server" EnableViewState="false" />
            <asp:HiddenField ID="FakeAddComponentCancelButton" runat="server" EnableViewState="false" />
            <ajax:ModalPopupExtender ID="AddComponentPopup" runat="server" 
                TargetControlID="FakeAddComponentSaveButton"
                PopupControlID="AddComponentDialog" 
                BackgroundCssClass="modalBackground"                         
                CancelControlID="FakeAddComponentCancelButton" 
                DropShadow="true"
                PopupDragHandleControlID="AddComponentDialogHeader" />
            <ajax:ModalPopupExtender ID="ItemizedDisplayPopUp" runat="server" 
                TargetControlID="ItemizedDisplay"
                PopupControlID="ItemizedDisplayDialog" 
                BackgroundCssClass="modalBackground"                         
                DropShadow="true"
                PopupDragHandleControlID="ItemizedDisplayDialogHeader" />
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>