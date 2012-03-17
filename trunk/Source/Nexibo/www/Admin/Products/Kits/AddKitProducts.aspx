<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="AddKitProducts.aspx.cs" Inherits="Admin_Products_Kits_AddKitProducts" Title="Add Product(s) to Kit" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Add Product(s) to Kit '{0}'" EnableViewState="false"></asp:Localize></h1>
        </div>
    </div>
    <script type="text/javascript">
       function ToggleCheckbox(id, checkState)
       {
          var cb = document.getElementById(id);
          if (cb != null) cb.checked = checkState;
       }

       function ToggleCheckboxes(sender)
       {
          if (CheckBoxIDs != null)
          {
             for (var i = 0; i < CheckBoxIDs.length; i++)
                ToggleCheckbox(CheckBoxIDs[i], sender.checked);
          }
          return true;
       }
    </script>
    <ajax:UpdatePanel id="PageAjax" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td valign="top" width="400px">
                        <div style="border:inset 2px black;padding:2px;">
                            <asp:Panel ID="SearchForm" runat="Server" DefaultButton="SearchButton">
                            <table class="inputForm" width="100%">
                                <tr>
                                    <th class="rowHeader">
                                        <asp:Localize ID="KitNameLabel" runat="server" Text="Kit:"></asp:Localize>
                                    </th>
                                    <td>
                                        <asp:Literal ID="KitName" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader">
                                        <asp:Localize ID="KitComponentNameLabel" runat="server" Text="Component:"></asp:Localize>
                                    </th>
                                    <td>
                                        <asp:Literal ID="KitComponentName" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr class="sectionHeader">
                                    <td colspan="2" align="center">
                                        <asp:Localize ID="InstructionText" runat="server" Text="Search for the product(s) to add:"></asp:Localize>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader">
                                        <cb:ToolTipLabel ID="NameFilterLabel" runat="server" Text="Name:" AssociatedControlID="NameFilter" ToolTip="Enter all or part of a product name to search for.  You can use the * and ? wildcards."></cb:ToolTipLabel>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="NameFilter" runat="server" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader">
                                        <cb:ToolTipLabel ID="SkuFilterLabel" runat="server" Text="Sku:" AssociatedControlID="SkuFilter" ToolTip="Enter all or part of a product SKU to search for.  You can use the * and ? wildcards."></cb:ToolTipLabel>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="SkuFilter" runat="server" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader">
                                        <cb:ToolTipLabel ID="CategoryFilterLabel" runat="server" Text="Category:" AssociatedControlID="CategoryFilter" ToolTip="Select the category to limit your search."></cb:ToolTipLabel>
                                    </th>
                                    <td>
                                        <asp:DropDownList ID="CategoryFilter" runat="server" AppendDataBoundItems="True" DataTextField="Name" DataValueField="CategoryId" EnableViewState="false">
                                            <asp:ListItem Text="- Any Category -" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader">
                                        <cb:ToolTipLabel ID="ManufacturerFilterLabel" runat="server" Text="Manufacturer:" AssociatedControlID="ManufacturerFilter" ToolTip="Select the manufacturer to limit your search."></cb:ToolTipLabel>
                                    </th>
                                    <td>
                                        <asp:DropDownList ID="ManufacturerFilter" runat="server" AppendDataBoundItems="True" DataTextField="Name" DataValueField="ManufacturerId" EnableViewState="false">
                                            <asp:ListItem Text="- Any Manufacturer -" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader">
                                        <cb:ToolTipLabel ID="VendorFilterLabel" runat="server" Text="Vendor:" AssociatedControlID="VendorFilter" ToolTip="Select the vendor to limit your search."></cb:ToolTipLabel>
                                    </th>
                                    <td>
                                        <asp:DropDownList ID="VendorFilter" runat="server" AppendDataBoundItems="True" DataTextField="Name" DataValueField="VendorId" EnableViewState="false">
                                            <asp:ListItem Text="- Any Vendor -" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" OnClientClick="this.value='searching'" Width="70px" />&nbsp;
                                        <asp:HyperLink ID="CancelLink" runat="server" Text="Cancel" NavigateUrl="EditKit.aspx" SkinID="Button"></asp:HyperLink>
                                    </td>
                                </tr>
                            </table>
                            </asp:Panel>
                        </div>
                    </td>
                    <td valign="top">
                        <asp:PlaceHolder ID="SearchPanel" runat="server">
                            <asp:Button ID="AddProductsButton" runat="server" Text="Add Selected Products" Visible="false" OnClick="AddProductsButton_Click" />
                            <asp:GridView ID="ProductSearchResults" runat="server" AutoGenerateColumns="False" DataSourceId="ProductDs" DataKeyNames="ProductId" 
                                Width="100%" SkinID="PagedList" AllowPaging="true" PageSize="25" AllowSorting="true" Visible="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="SelectAll" runat="server" onclick="ToggleCheckboxes(this)" />
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="center" Width="60px" />
                                        <ItemTemplate>
                                            <asp:CheckBox ID="Selected" runat="server" Visible='<%# IsNotKit(Container.DataItem) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sku" HeaderText="Sku" SortExpression="Sku">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Localize ID="EmptyMessage" runat="server" Text="- no matching products -"></asp:Localize>
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="ProductDs" runat="server" DataObjectTypeName="CommerceBuilder.Products.Product"
                                OldValuesParameterFormatString="original_{0}" SelectMethod="FindProducts" SortParameterName="sortExpression"
                                TypeName="CommerceBuilder.Products.ProductDataSource">
                                <SelectParameters>
                                    <asp:ControlParameter Name="name" Type="String" ControlID="NameFilter" PropertyName="Text" />
                                    <asp:ControlParameter Name="sku" Type="String" ControlID="SkuFilter" PropertyName="Text" />
                                    <asp:ControlParameter Name="categoryId" Type="Int32" ControlID="CategoryFilter" PropertyName="SelectedValue" />
                                    <asp:ControlParameter Name="manufacturerId" Type="Int32" ControlID="ManufacturerFilter" PropertyName="SelectedValue" />
                                    <asp:ControlParameter Name="vendorId" Type="Int32" ControlID="VendorFilter" PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ConfigurePanel" runat="server" Visible="false">
                            <div class="sectionHeader">
                                <h2>Add Selected Product(s)</h2>
                            </div>
                            <asp:Repeater ID="SelectedProductRepeater" runat="server" OnItemDataBound="SelectedProductRepeater_ItemDataBound">
                                <ItemTemplate>
                                    <table class="inputForm">
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Label ID="SelectedProductNameLabel" runat="server" Text="Product:"></asp:Label>
                                            </th>
                                            <td>
                                                <%# Eval("Name") %>
                                                <asp:HiddenField ID="PID" runat="server" value='<%#Eval("ProductId")%>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th class="rowHeader">
                                                <cb:ToolTipLabel ID="NameFormatLabel" runat="server" Text="Display Name:" AssociatedControlID="NameFormat" ToolTip="Enter the display name for the kit product.  This is how the item will be shown on the customer invoice.  You can use the following string tokens: $name, $options, $quantity, $price."></cb:ToolTipLabel>
                                            </th>
                                            <td>
                                                <asp:TextBox ID="NameFormat" runat="server" Width="300px" MaxLength="255" Text='<%# GetNameFormat(Container.DataItem) %>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th class="rowHeader">
                                                <cb:ToolTipLabel ID="KitQuantityLabel" runat="server" Text="Quantity:" AssociatedControlID="KitQuantity" ToolTip="The quantity of this product to include in the kit."></cb:ToolTipLabel>
                                            </th>
                                            <td>
                                                <asp:TextBox ID="KitQuantity" runat="server" Columns="2" MaxLength="3" Text="1"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Label ID="ProductPriceLabel" runat="server" Text="Product Price:"></asp:Label>
                                            </th>
                                            <td>
                                                <asp:Literal ID="ProductPrice" runat="server" Text='<%# Eval("Price", "{0:lc}") %>'></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Label ID="PriceLabel" runat="server" Text="Kit Price:"></asp:Label>
                                            </th>
                                            <td valign="top">
                                                <asp:DropDownList ID="PriceMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PriceMode_SelectedIndexChanged">
                                                    <asp:ListItem Value="0" Text="Use Product Price" Selected="true"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Modify Product Price"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Fixed Price"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="trPrice" runat="server" visible="false">
                                            <th class="rowHeader">
                                                <asp:Label ID="ModifyPriceLabel" runat="server" text="Price Modifier: " SkinID="FieldHeader" Visible="false"></asp:Label>
                                                <asp:Label ID="FixedPriceLabel" runat="server" text="Fixed Price: " SkinID="FieldHeader" Visible="false"></asp:Label>
                                            </th>
                                            <td>
                                                <asp:TextBox ID="Price" runat="server" Columns="4" MaxLength="8" /><br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Label ID="ProductWeightLabel" runat="server" Text="Product Weight:"></asp:Label>
                                            </th>
                                            <td>
                                                <%# Eval("Weight", "{0:F2}") %>
						                        <%# Token.Instance.Store.WeightUnit.ToString() %>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Label ID="WeightLabel" runat="server" Text="Kit Weight:"></asp:Label>
                                            </th>
                                            <td valign="top">
                                                <asp:DropDownList ID="WeightMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="WeightMode_SelectedIndexChanged">
                                                    <asp:ListItem Value="0" Text="Use Product Weight" Selected="true"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Modify Product Weight"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Fixed Weight"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="trWeight" runat="server" visible="false">
                                            <th class="rowHeader">
                                                <asp:Label ID="ModifyWeightLabel" runat="server" text="Weight Modifier: " SkinID="FieldHeader" Visible="false"></asp:Label>
                                                <asp:Label ID="FixedWeightLabel" runat="server" text="Fixed Weight: " SkinID="FieldHeader" Visible="false"></asp:Label>
                                            </th>
                                            <td>
                                                <asp:TextBox ID="Weight" runat="server" Columns="4" MaxLength="8" /><br />
                                            </td>
                                        </tr>
                                        <tr id="trOptionWarning" runat="server" visible="false">
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:Label ID="OptionWarning" runat="server" SkinID="ErrorCondition" Text="You must select the options:" EnableViewState="false"></asp:Label>
                                            </td>
                                        </tr>
                                        <asp:PlaceHolder ID="phOptions" runat="server"></asp:PlaceHolder>
                                        <tr id="trInvalidVariant" runat="server" visible="false" enableviewstate="false">
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:Localize ID="InvalidVariantWarning" runat="server" Text="<i>NOTE: The variant you have selected is marked as unavailable.</i>" EnableViewState="false"></asp:Localize>
                                            </td>
                                        </tr>
                                        <tr id="trSelected" runat="server" visible='<%# IsNotHiddenPart(Container.DataItem) %>'>
                                            <th class="rowHeader">
                                                <asp:Label ID="IsSelectedLabel" runat="server" Text="Selected:"></asp:Label>
                                            </th>
                                            <td valign="top">
                                                <asp:CheckBox ID="IsSelected" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                                <SeparatorTemplate>
                                    <hr />
                                </SeparatorTemplate>
                            </asp:Repeater><hr />
                            <asp:Button ID="FinishButton" runat="server" Text="Add Products" OnClick="FinishButton_Click" />
                        </asp:PlaceHolder>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="VS" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>