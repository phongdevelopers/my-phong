<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="EditProduct.aspx.cs" Inherits="Admin_Products_EditProduct" Title="Edit Product '{0}'" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<script>
function addManufacturer()
{
    var name = prompt("New manufacturer name?", "");
    if ((name == null) || (name.length == 0)) return false;
    var c = document.getElementById('<%= NewManufacturerName.ClientID %>');
    if (c == null) return false;
    c.value = name;
    return true;
}
</script>
<div class="pageHeader">
    <div class="caption">
        <h1><asp:Localize ID="Caption" runat="server" Text="{0}" EnableViewState="false"></asp:Localize></h1>
    </div>
    <div class="content">
        <asp:Label ID="SavedMessage" runat="server" Text="Product saved at {0:t}" SkinID="GoodCondition" EnableViewState="False" Visible="false"></asp:Label>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
    </div>
</div>
<table cellpadding="3" cellspacing="0" class="inputForm" width="100%">
    <tr>
        <td class="submit" colspan="4" >
			<asp:Button ID="SaveButton1" runat="server" Text="Save" OnClick="SaveButton_Click"></asp:Button>
			<asp:Button ID="SaveAndCloseButton1" runat="server" Text="Save and Close" OnClick="SaveButton_Click" />
            <asp:HyperLink ID="CancelButton1" runat="server" Text="Cancel" SkinID="Button" EnableViewState="false"></asp:HyperLink>
        </td>
    </tr>
    <tr class="sectionHeader">
        <td colspan="4">
            BASIC INFO
        </td>
    </tr>
    <tr>
        <th class="rowHeader" width="15%">
            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="Name of the product"></cb:ToolTipLabel>
        </th>
        <td width="35%">
            <asp:TextBox ID="Name" runat="server" Text="" width="200px" MaxLength="255" EnableViewState="false"></asp:TextBox>
            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Dynamic" ErrorMessage="Name is required." ControlToValidate="Name"></asp:RequiredFieldValidator><br />
        </td>
        <th class="rowHeader" width="15%">
            <cb:ToolTipLabel ID="SkuLabel" runat="server" Text="SKU:" ToolTip="Stock keeping unit, or a unique identifier for the product."></cb:ToolTipLabel><br />
        </th>
        <td width="35%">
                        <asp:TextBox ID="Sku" runat="server" Width="80px" EnableViewState="false" MaxLength="40"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="PriceLabel" runat="server" Text="Price:" ToolTip="Price of the product, prior to any modifications or adjustments."></cb:ToolTipLabel><br />
        </th>
        <td>
                        <asp:TextBox ID="Price" runat="server" Columns="8" EnableViewState="false" MaxLength="10"></asp:TextBox>
                        <asp:RangeValidator ID="PriceValidator" runat="server" ControlToValidate="Price"
                            ErrorMessage="Price should fall between 0.00 and 99999999.99" MaximumValue="99999999.99"
                            MinimumValue="0" Type="Currency">*</asp:RangeValidator></td>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="MsrpLabel" runat="server" Text="MSRP:" ToolTip="Manufacturer suggested retail price of the product."></cb:ToolTipLabel>
        </th>
        <td>
                        <asp:TextBox ID="Msrp" runat="server" Columns="8" EnableViewState="false" MaxLength="10"></asp:TextBox>
                        <asp:RangeValidator ID="MSRPValidator" runat="server" ControlToValidate="Msrp" ErrorMessage="MSRP should fall between 0.00 and 99999999.99"
                            MaximumValue="99999999.99" MinimumValue="0" Type="Currency">*</asp:RangeValidator></td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="ManufacturerLabel" runat="server" Text="Manufacturer:" ToolTip="Manufacturer of the product."></cb:ToolTipLabel>
        </th>
        <td>
            <ajax:UpdatePanel ID="ManufacturerAjax" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true" RenderMode="block">
                <ContentTemplate>
                    <asp:DropDownList ID="ManufacturerList" runat="server" DataTextField="Name" DataValueField="ManufacturerId" AppendDataBoundItems="True" EnableViewState="false">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:LinkButton ID="NewManufacturerLink" runat="server" Text="new" OnClick="NewManufacturerLink_Click" CausesValidation="false" SkinID="Link" OnClientClick="return addManufacturer()"></asp:LinkButton>
                    <asp:HiddenField ID="NewManufacturerName" runat="server" EnableViewState="false" />                    
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="ModelNumberLabel" runat="server" Text="Manuf. Part No.:" ToolTip="Manufacturer's model or part number for this product."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="ModelNumber" runat="server" Columns="8" MaxLength="40" EnableViewState="false"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top">
            <cb:ToolTipLabel ID="GiftCertificateLabel" runat="server" Text="Gift Certificate:" ToolTip="When checked, the purchase of this item creates a gift certificate for the purchase amount of the product."></cb:ToolTipLabel>
        </th>
        <td valign="top">
            <asp:CheckBox ID="GiftCertificate" runat="server" />
        </td>
        <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="UseVariablePriceLabel" runat="server" Text="Variable Price:" ToolTip="If checked, allow the customer to specify the price of the product." AssociatedControlID="UseVariablePrice"></cb:ToolTipLabel><br />
        </th>
        <td valign="top">
            <ajax:UpdatePanel ID="VariablePriceAjax" runat="server">
                <ContentTemplate>
                        <asp:CheckBox ID="UseVariablePrice" runat="server" AutoPostBack="true" />
                    <asp:PlaceHolder ID="VariablePriceFields" runat="server">
                        <table cellpadding="2" cellspacing="0">
                            <tr>
                                <td align="right" nowrap>
                                    <asp:Label ID="MinPriceLabel" runat="server" Text="Min:" AssociatedControlID="MinPrice"></asp:Label>
                                </td>
                                <td>
                                                <asp:TextBox ID="MinPrice" runat="server" Width="40px" MaxLength="10" ></asp:TextBox>
                                                <asp:RangeValidator ID="MinPriceValidator" runat="server" ControlToValidate="MinPrice"
                                                    ErrorMessage="Minimum Price should fall between 0.00 and 99999999.99" MaximumValue="99999999.99"
                                                    MinimumValue="0" Type="Currency">*</asp:RangeValidator>
                                </td>
                                <td align="right" nowrap>
                                    &nbsp;<asp:Label ID="MaxPriceLabel" runat="server" Text="Max:"></asp:Label>
                                </td>
                                <td>
                                                <asp:TextBox ID="MaxPrice" runat="server" Width="40px" MaxLength="10"></asp:TextBox>
                                                <asp:RangeValidator ID="MaxPriceValidator" runat="server" ControlToValidate="MaxPrice"
                                                    ErrorMessage="Maximum Price should fall between 0.00 and 99999999.99" MaximumValue="99999999.99"
                                                    MinimumValue="0" Type="Currency">*</asp:RangeValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:PlaceHolder>
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top">
            <cb:ToolTipLabel ID="IsProhibitedLabel" runat="server" Text="Prohibited:" ToolTip="When checked, the purchase of this item is prohibited with Google Checkout."></cb:ToolTipLabel>
        </th>
        <td valign="top">
            <asp:CheckBox ID="IsProhibited" runat="server" />
        </td>
		<th class="rowHeader" valign="top" nowrap>
			<cb:ToolTipLabel ID="HidePriceLabel" runat="server" Text="Hide Price" ToolTip="When checked, the price of this item is not shown when browsing categories or products.  The customer must click a link (or add to basket) to see the price."></cb:ToolTipLabel>
		</th>
		<td valign="top">
			<asp:CheckBox ID="HidePrice" runat="server" />
		</td>
    </tr>
    <tr id="trAllowReviews" runat="server" visible="false">
		<th class="rowHeader" valign="top">
            <cb:ToolTipLabel ID="AllowReviewsLabel" runat="server" Text="Allow Reviews:" ToolTip="When checked, customer reviews will be allowed for this product. If you don't want to allow customers to place reviews for this product simply uncheck it."></cb:ToolTipLabel>
		</th>
		<td valign="top">
			<asp:CheckBox ID="AllowReviews" runat="server" />
		</td>
		<th class="rowHeader" valign="top">
		    <cb:ToolTipLabel ID="CustomUrlLabel" runat="server" Text="Custom Url:" ToolTip="You can provide a custom URL to access your product. This URL will override the default one generated by system. The value provided should be a URL relative to the store directory. Absolute URLs are not supported."></cb:ToolTipLabel>
	    </th>
		<td valign="top">
		    <asp:TextBox ID="CustomUrl" runat="server" Width="250" MaxLength="150"></asp:TextBox>
            <cb:CustomUrlValidator ID="CustomUrlValidator" runat="server" ControlToValidate="CustomUrl" 
                Text="*" FormatErrorMessage="The custom url has an invalid format."
                DuplicateErrorMessage="This custom url is already used, please choose a unique value."></cb:CustomUrlValidator><br />
            <asp:Localize ID="CustomUrlExample" runat="server" Text="e.g. Fiction/Mystery/The-Confession.aspx"></asp:Localize>
		</td>
	</tr>
    <tr class="sectionHeader">
        <td colspan="4">
            DISPLAY OPTIONS
        </td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="VisibilityLabel" runat="server" Text="Visibility:" AssociatedControlID="Visibility" ToolTip="Visibility setting indicates how this product is accessed from the retail side. Public: Published, Hidden: Unpublished (available through direct link), Private: Access Prevented"></cb:ToolTipLabel>
        </th>
        <td>
            <asp:DropDownList ID="Visibility" runat="server" EnableViewState="false">
                <asp:ListItem Value="0" Text="Public"></asp:ListItem>
                <asp:ListItem Value="1" Text="Hidden"></asp:ListItem>
                <asp:ListItem Value="2" Text="Private"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="FeaturedLabel" runat="server" Text="Featured:" ToolTip="Featured products are highlighted in the store."></cb:ToolTipLabel><br />
        </th>
        <td>
            <asp:CheckBox ID="Featured" runat="server" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top">
            <cb:ToolTipLabel ID="DisplayPageLabel" runat="server" Text="Display Page:" ToolTip="The display page used for this product."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:DropDownList ID="DisplayPage" runat="server" EnableViewState="false">
                <asp:ListItem Text="Inherit" Value=""></asp:ListItem>
            </asp:DropDownList>
        </td>
        <th class="rowHeader" valign="top">
            <cb:ToolTipLabel ID="LocalThemeLabel" runat="server" Text="Theme:" ToolTip="The theme used to display this product." AssociatedControlID="LocalTheme"></cb:ToolTipLabel>
        </th>
        <td>
            <asp:DropDownList ID="LocalTheme" runat="server" EnableViewState="false">
                <asp:ListItem Text="Inherit" Value=""></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="DisablePurchaseLabel" runat="server" Text="Disable Purchase:" ToolTip="When purchase is disabled for a product, the display pages will not show a buy button for that product and it cannot be added to the basket."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:CheckBox ID="DisablePurchase" runat="server" />
        </td>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="ExcludeFromFeedLabel" runat="server" Text="Exclude From Feeds:" ToolTip="Indicates that this product should not be included in any generated shopping feeds."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:CheckBox ID="ExcludeFromFeed" runat="server" />
        </td>
    </tr>        
    <tr class="sectionHeader">
        <td colspan="4">
            TAXES &AMP; SHIPPING
        </td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="IsShippableLabel" runat="server" Text="Shippable:" ToolTip="Indicates whether this product is shippable. Yes means it can be shipped in a package with other items. Separately means it has to be shipped in its own package and its shipping charges are calculated separately."></cb:ToolTipLabel>
        </th>
        <td>
             <asp:DropDownList ID="IsShippable" runat="server" EnableViewState="false">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1">Yes</asp:ListItem>
                <asp:ListItem Value="2">Separately</asp:ListItem>
            </asp:DropDownList>
        </td>                
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="WeightLabel" runat="server" Text="Weight:" ToolTip="The shipping weight of the product."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="Weight" runat="server" Columns="4" EnableViewState="false" MaxLength="10"></asp:TextBox>
            <asp:RangeValidator ID="WeightValidator1" runat="server" ControlToValidate="Weight"
                    ErrorMessage="Weight should fall between 0.00 and 99999999.99" MaximumValue="99999999.99"
                    MinimumValue="0" Type="Double">*</asp:RangeValidator>
			<asp:Label ID="WeightUnit" runat="server" EnableViewState="false" Text=""></asp:Label>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="DimensionsLabel" runat="server" Text="Dimensions:" ToolTip="The shipping dimensions of the product."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="Length" runat="server" Columns="2" EnableViewState="false" MaxLength="10"></asp:TextBox>
            <asp:RangeValidator ID="LengthValidator" runat="server" ControlToValidate="Length"
                    ErrorMessage="Length should fall between 0.00 and 99999999.99" MaximumValue="99999999.99"
                    MinimumValue="0" Type="Double">*</asp:RangeValidator>
            <cb:ToolTipLabel ID="LengthLabel" runat="server" Text="L" ToolTip="Shipping Length"></cb:ToolTipLabel>
            <asp:TextBox ID="Width" runat="server" Columns="2" EnableViewState="false" MaxLength="10"></asp:TextBox>
            <asp:RangeValidator ID="WidthValidator1" runat="server" ControlToValidate="Width"
                    ErrorMessage="Width should fall between 0.00 and 99999999.99" MaximumValue="99999999.99"
                    MinimumValue="0" Type="Double">*</asp:RangeValidator>
            <cb:ToolTipLabel ID="WidthLabel" runat="server" Text="W" ToolTip="Shipping Width"></cb:ToolTipLabel>
            <asp:TextBox ID="Height" runat="server" Columns="2" EnableViewState="false" MaxLength="10"></asp:TextBox>
            <asp:RangeValidator ID="HeightValidator2" runat="server" ControlToValidate="Height"
                    ErrorMessage="Height should fall between 0.00 and 99999999.99" MaximumValue="99999999.99"
                    MinimumValue="0" Type="Double">*</asp:RangeValidator>
            <cb:ToolTipLabel ID="HeightLabel" runat="server" Text="H" ToolTip="Shipping Height"></cb:ToolTipLabel>
			<asp:Label ID="MeasurementUnit" runat="server" EnableViewState="false" Text=""></asp:Label>
        </td>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="WarehouseLabel" runat="server" Text="Warehouse:" ToolTip="The warehouse where this product ships from."></cb:ToolTipLabel>
        </th>
        <td>
                        <asp:DropDownList ID="Warehouse" runat="server" DataSourceID="WarehouseDs" DataTextField="Name" DataValueField="WarehouseId" OnDataBound="Warehouse_DataBound" EnableViewState="false">
                        </asp:DropDownList>
            <asp:ObjectDataSource ID="WarehouseDs" runat="server" OldValuesParameterFormatString="original_{0}"
                            SelectMethod="LoadForStore" TypeName="CommerceBuilder.Shipping.WarehouseDataSource">
                            <SelectParameters>
                                <asp:Parameter DefaultValue="Name" Name="sortExpression" Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="GiftWrapLabel" runat="server" Text="Gift Wrap:" ToolTip="The gift wrap group that specifies which wrapping options are available for this product."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:DropDownList ID="WrapGroup" runat="server" DataSourceID="GiftWrapDs" DataTextField="Name"
                            DataValueField="WrapGroupId" AppendDataBoundItems=True OnDataBound="WrapGroup_DataBound" EnableViewState="false">
                <asp:ListItem Value="" Text=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="GiftWrapDs" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="LoadForStore" TypeName="CommerceBuilder.Products.WrapGroupDataSource">
            </asp:ObjectDataSource>
        </td>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="TaxCodeLabel" runat="server" Text="Tax Code:" ToolTip="The tax code that links this product to your custom tax rules."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:DropDownList ID="TaxCode" runat="server" DataSourceID="TaxCodeDs" DataTextField="Name"
                            DataValueField="TaxCodeId" AppendDataBoundItems=True OnDataBound="TaxCode_DataBound" EnableViewState="false">
                <asp:ListItem Value="" Text=""></asp:ListItem>
            </asp:DropDownList><asp:ObjectDataSource ID="TaxCodeDs" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="LoadForStore" TypeName="CommerceBuilder.Taxes.TaxCodeDataSource">
            </asp:ObjectDataSource>
        </td>
    </tr>    
    <tr class="sectionHeader">
        <td colspan="4">
            INVENTORY CONTROL
        </td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="CostOfGoodsLabel" runat="server" Text="Cost of Goods:" ToolTip="Your cost for this product, used to calculate profit on a sale."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="CostOfGoods" runat="server" Columns="8" EnableViewState="false" MaxLength="10"></asp:TextBox>
            <asp:RangeValidator ID="CostOfGoodValidator" runat="server" ControlToValidate="CostOfGoods"
                ErrorMessage="Cost of Goods should fall between 0.00 and 99999999.99" MaximumValue="99999999.99"
                MinimumValue="0" Type="Currency">*</asp:RangeValidator></td>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="VendorLabel" runat="server" Text="Vendor:" ToolTip="The vendor that you acquire this product from."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:DropDownList ID="Vendor" runat="server" DataSourceID="VendorDs" DataTextField="Name"
                            DataValueField="VendorId" AppendDataBoundItems="True" OnDataBound="Vendor_DataBound" EnableViewState="false">
                <asp:ListItem Value="" Text=""></asp:ListItem>
            </asp:DropDownList><asp:ObjectDataSource ID="VendorDs" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="LoadForStore" TypeName="CommerceBuilder.Products.VendorDataSource">
                  <SelectParameters>
                    <asp:Parameter DefaultValue="Name" Name="sortExpression" Type="String" />
                  </SelectParameters>                  
            </asp:ObjectDataSource>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="MinQuantityLabel" runat="server" Text="Min Quantity:" ToolTip="The minimum quantity of this product that must be purchased for an order."></cb:ToolTipLabel>
        </th>
        <td>
                        <asp:TextBox ID="MinQuantity" runat="server" Columns="4" EnableViewState="false" MaxLength="10"></asp:TextBox>
        </td>
        <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="MaxQuantityLabel" runat="server" Text="Max Quantity:" ToolTip="The maximum quantity of this product that can be purchased for an order."></cb:ToolTipLabel>
        </th>
        <td>
                        <asp:TextBox ID="MaxQuantity" runat="server" Columns="4" EnableViewState="false" MaxLength="10"></asp:TextBox>
        </td>
    </tr>
    <tr ID="trInventory" runat="server">
        <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="CurrentInventoryModeLabel" runat="server" Text="Inventory Tracking:" ToolTip="Indicate whether to track inventory at the product level, or for individual variants (if applicable)."></cb:ToolTipLabel>
        </th>
        <td colspan="3">
            <ajax:UpdatePanel ID="InventoryAjax" runat="server" UpdateMode="conditional">
                <ContentTemplate>
                    <asp:DropDownList ID="CurrentInventoryMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CurrentInventoryMode_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="Disabled"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Track Product"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Track Variants"></asp:ListItem>
                    </asp:DropDownList>
                    <table id="ProductInventoryPanel" runat="server" cellpadding="4" style="margin-top:4px;">
                        <tr>
                            <th align="right" nowrap>
                                <cb:ToolTipLabel ID="InStockLabel" runat="server" Text="In Stock:" ToolTip="The current quantity in stock."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:TextBox ID="InStock" runat="server" Columns="4" EnableViewState="false"></asp:TextBox>
                            </td>
                            <th align="right" nowrap>
                                <cb:ToolTipLabel ID="LowStockLabel" runat="server" Text="Low Stock:" ToolTip="The quantity level at which you will start receiving alerts that the product is low in stock."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:TextBox ID="LowStock" runat="server" Columns="4" EnableViewState="false"></asp:TextBox>
                            </td>
                            <th align="right" nowrap>
                                <cb:ToolTipLabel ID="BackOrderLabel" runat="server" Text="Allow Backorder:" ToolTip="When backorder is allowed, this product can continue to be purchased.  When backorder is not allowed, customers cannot purchase the product once it is out of stock."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:CheckBox ID="BackOrder" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <table id="VariantInventoryPanel" runat="server" cellpadding="4" style="margin-top:4px;">
                        <tr>
                            <th align="right" nowrap>
                                <cb:ToolTipLabel ID="BackOrderLabel2" runat="server" Text="Allow Backorder:" ToolTip="When backorder is allowed, this product can continue to be purchased.  When backorder is not allowed, customers cannot purchase the product once it is out of stock."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:CheckBox ID="BackOrder2" runat="server" />
                            </td>
                            <td>
                                <p><asp:Localize ID="VariantInventoryHelpText" runat="server" Text="(To manage inventory, visit the variants page.)"></asp:Localize></p>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
    </tr>
    <tr class="sectionHeader">
        <td colspan="4">
            DESCRIPTIONS
        </td>
    </tr>
    <tr>
        <th class="rowHeader" style="vertical-align:top;">
            <cb:ToolTipLabel ID="SummaryLabel" runat="server" Text="Summary:" ToolTip="A brief description of the product that can be used for the category listings or featured products."></cb:ToolTipLabel>
        </th>
        <td colspan="3" style="padding-right:6px;">
            <asp:TextBox ID="Summary" runat="Server" Text="" Rows="5" TextMode="MultiLine" Columns="120" MaxLength="1000" /><asp:CustomValidator ID="SummaryValidator" runat="server" ControlToValidate="Summary" Text="*"></asp:CustomValidator>
            <asp:Label ID="SummaryCharCount" runat="server" Text="250"></asp:Label>
            <asp:Localize ID="SummaryCharCountLabel" runat="server" Text="characters remaining"></asp:Localize>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" style="vertical-align:top;">
            <asp:ImageButton ID="DescriptionHtml" runat="server" AlternateText="Edit HTML" ToolTip="Edit HTML" SkinID="HtmlIcon" OnClientClick="" CausesValidation="False" />&nbsp;
            <cb:ToolTipLabel ID="DescriptionLabel" runat="server" Text="Description:" ToolTip="The description of this product."></cb:ToolTipLabel>
        </th>
        <td colspan="3">
            <asp:TextBox ID="Description" runat="server" Columns="120" Height="200px" TextMode="multiLine" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader" style="vertical-align:top;">
            <asp:ImageButton ID="ExtendedDescriptionHtml" runat="server" AlternateText="Edit HTML" ToolTip="Edit HTML" SkinID="HtmlIcon" OnClientClick="" CausesValidation="False" />&nbsp;
            <cb:ToolTipLabel ID="ExtendedDescriptionLabel" runat="server" Text="More&nbsp;Details:" ToolTip="More details or additional description for this product."></cb:ToolTipLabel>
        </th>
        <td colspan="3">
            <asp:TextBox ID="ExtendedDescription" runat="server" Columns="120" Height="200px" TextMode="multiLine" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="HtmlHeadLabel" runat="server" Text="HTML HEAD:" ToolTip="Enter the data to include in the HTML HEAD portion of the display page, such as meta keywords and description."></cb:ToolTipLabel>
        </th>
        <td colspan="3">
            <asp:TextBox ID="HtmlHead" runat="server" Text="" TextMode="multiLine" Rows="5" Columns="120"></asp:TextBox><br />
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="SearchKeywordsLabel" runat="server" Text="Search Keywords:" ToolTip="Enter the data like keywords and frequent misspellings. It will always be searched on the retail site."></cb:ToolTipLabel>
        </th>
        <td colspan="3">
            <asp:TextBox ID="SearchKeywords" runat="server" Text="" TextMode="multiLine" Rows="5" Columns="120"></asp:TextBox><br />
        </td>
    </tr>
    <tr>
        <td class="sectionHeader" colspan="4">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td class="submit" colspan="4">
			<asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="SaveButton_Click"></asp:Button>
			<asp:Button ID="SaveAndCloseButton" runat="server" Text="Save and Close" OnClick="SaveButton_Click" />
            <asp:HyperLink ID="CancelButton" runat="server" Text="Cancel" SkinID="Button" EnableViewState="false"></asp:HyperLink>
        </td>
    </tr>
</table>
</asp:Content>
