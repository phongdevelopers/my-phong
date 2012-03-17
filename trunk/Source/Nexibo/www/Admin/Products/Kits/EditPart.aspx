<%@ Page Language="C#" MasterPageFile="../Product.master" CodeFile="EditPart.aspx.cs" Inherits="Admin_Products_Kits_EditPart" Title="Edit Kit Product"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Edit Kit Part in {0}"></asp:Localize></h1>
        </div>
    </div>
    <ajax:UpdatePanel id="PageAjax" runat="server">
        <ContentTemplate>
            <table class="inputForm">
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="KitNameLabel" runat="server" Text="Kit:" AssociatedControlID="KitName"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="KitName" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="KitComponentNameLabel" runat="server" Text="Component:" AssociatedControlID="KitComponentName"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="KitComponentName" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="SelectedProductNameLabel" runat="server" Text="Product:"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="SelectedProductName" runat="server" Text="No Product Selected"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <cb:ToolTipLabel ID="DisplayNameLabel" runat="server" Text="Display Name:" AssociatedControlID="DisplayName" ToolTip="Enter the display name for the kit product.  This is how the item will be shown on the customer invoice.  You can use the following string tokens: $name, $options, $quantity, $price."></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:TextBox ID="DisplayName" runat="server" Width="300px" MaxLength="100" />
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <cb:ToolTipLabel ID="KitQuantityLabel" runat="server" Text="Quantity:" AssociatedControlID="KitQuantity" ToolTip="The quantity of this product to include in the kit."></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:TextBox ID="KitQuantity" runat="server" Columns="2" MaxLength="3" Text="1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="ProductPriceLabel" runat="server" Text="Product Price:"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="ProductPrice" runat="server" Text="$0.00"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="PriceLabel" runat="server" Text="Kit Price:"></asp:Label>
                    </th>
                    <td valign="top">
                        <ajax:UpdatePanel id="PriceModePanel" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="PriceMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PriceMode_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="Use Product Price" Selected="true"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Modify Product Price"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Fixed Price"></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:Label ID="ModifyPriceLabel" runat="server" text="Enter Price Modifier: " SkinID="FieldHeader" Visible="false"></asp:Label>
                                <asp:Label ID="FixedPriceLabel" runat="server" text="Enter Price: " SkinID="FieldHeader" Visible="false"></asp:Label>
                                <asp:TextBox ID="Price" runat="server" Columns="4" Visible="false" MaxLength="8" /><br />
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="ProductWeightLabel" runat="server" Text="Product Weight:"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="ProductWeight" runat="server" Text="0.00"></asp:Label>
						<asp:Label ID="WeightUnit" runat="server" EnableViewState="false" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="WeightLabel" runat="server" Text="Kit Weight:"></asp:Label>
                    </th>
                    <td valign="top">
                        <ajax:UpdatePanel id="WeightModePanel" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="WeightMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="WeightMode_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="Use Product Weight" Selected="true"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Modify Product Weight"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Fixed Weight"></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:Label ID="ModifyWeightLabel" runat="server" text="Enter Weight Modifier: " SkinID="FieldHeader" Visible="false"></asp:Label>
                                <asp:Label ID="FixedWeightLabel" runat="server" text="Enter Weight: " SkinID="FieldHeader" Visible="false"></asp:Label>
                                <asp:TextBox ID="Weight" runat="server" Columns="4" Visible="false" MaxLength="8" /><br />
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </td>
                </tr>
                <asp:PlaceHolder ID="phOptions" runat="server"></asp:PlaceHolder>
                <tr>
                    <th class="rowHeader" nowrap>
                        <asp:Label ID="IsSelectedLabel" runat="server" Text="Selected:"></asp:Label>
                    </th>
                    <td valign="top">
                        <asp:CheckBox ID="IsSelected" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
						<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

