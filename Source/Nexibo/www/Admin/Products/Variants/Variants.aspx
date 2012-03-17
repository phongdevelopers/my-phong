<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="Variants.aspx.cs" Inherits="Admin_Products_Variants_Variants" Title="Product Variants" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<ajax:UpdatePanel runat="server" ID="VariantPanel" UpdateMode="Always">
    <ContentTemplate>
        <div class="pageHeader">
            <div class="caption">
                <h1><asp:Localize ID="Caption" runat="server" Text="Variants: {0}" EnableViewState="false"></asp:Localize></h1>
            </div>
        </div>
        <div class="contentPanelBody" style="text-align:justify">
            <asp:Label ID="InstructionText" runat="server" Text="All the possible combinations of this product are displayed in the table below.  You can set the values such as SKU, Price, and Weight specifically for each variant if it is different from the default calculated value." EnableViewState="false"></asp:Label><br /><br />
        </div>
        <asp:PlaceHolder ID="TooManyVariantsPanel" runat="server" Visible="false" EnableViewState="false">
            <div class="contentPanelBody" style="text-align:justify">
                <asp:Label ID="TooManyVariantsMessage" runat="server" Text="WARNING: The product {0} has more than {1} options.  The variant grid below is limited to the first {1} options only." EnableViewState="false" SkinID="errorCondition"></asp:Label><br /><br />
            </div>
        </asp:PlaceHolder>
        <asp:Localize runat="server" ID="DisplayRangeLabel" EnableViewState="false" Text="Displaying {0:#,###} through {1:#,###} out of {2:#,###} unique variants" /><br />
        <asp:PlaceHolder ID="PagerPanel" runat="server" EnableViewState="false">
            <table cellpadding="4">
                <tr>
                    <td>
                        <b>Page:</b>
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="FirstLink" Text="&laquo; First" onclick="ChangePage" />
                        &nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="PreviousLink" Text="&laquo; Previous" onclick="ChangePage" />
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="JumpPage" AutoPostBack="true" OnSelectedIndexChanged="ChangePage"  />
                         / <asp:Label runat="server" ID="PageCountLabel" Style="font-size:10pt" EnableViewState="true" />&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="NextLink" Text="Next &raquo;" onclick="ChangePage" />
                        &nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="LastLink" Text="Last &raquo;" onclick="ChangePage" />
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <asp:Repeater ID="VariantGrid" Runat="server" OnItemCreated="VariantGrid_ItemCreated">
            <HeaderTemplate>
	        <table class="summary" cellspacing="0" border="0" style="width:100%;border-collapse:collapse;">
	            <tr>
	                <th><asp:Localize ID="RowHeader" runat="server" Text="Row" EnableViewState="False"></asp:Localize></th>
	                <th><asp:Localize ID="VariantHeader" runat="server" Text="Variant" EnableViewState="False"></asp:Localize></th>
	                <th><asp:Localize ID="SkuHeader" runat="server" Text="SKU" EnableViewState="False"></asp:Localize></th>
	                <th><asp:Localize ID="PriceHeader" runat="server" Text="Price" EnableViewState="False"></asp:Localize></th>
	                <th><asp:Localize ID="WeightHeader" runat="server" Text="Weight" EnableViewState="False"></asp:Localize></th>
	                <th><asp:Localize ID="CogsHeader" runat="server" Text="COGS" EnableViewState="False"></asp:Localize></th>
	                <th><asp:Localize ID="AvailableHeader" runat="server" Text="Available" EnableViewState="False"></asp:Localize></th>
	                <asp:PlaceHolder ID="phInventoryColumns" runat="server"></asp:PlaceHolder>
	            </tr>
	        </HeaderTemplate>
	        <ItemTemplate>
	            <asp:PlaceHolder ID="phVariantRow" runat="server"></asp:PlaceHolder>
	        </ItemTemplate>
	        <FooterTemplate>
	        </table>
	        </FooterTemplate>
        </asp:Repeater>
        <asp:Button ID="EditOptionsButton" runat="server" Text="< Manage Options" OnClick="EditOptionsButton_Click"></asp:Button>&nbsp;
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" OnClientClick="this.value='Saving...';" /><br />
        <asp:Label ID="SavedMessage" runat="server" Text="Data saved at {0:g}" EnableViewState="false" Visible="false" SkinID="GoodCondition"></asp:Label>
        <asp:HiddenField ID="VS_CustomState" runat="server" EnableViewState="false" />
    </ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>