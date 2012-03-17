<%@ Page Language="C#" MasterPageFile="../Product.master" CodeFile="EditAssets.aspx.cs" Inherits="Admin_Products_Assets_EditAssets" Title="Edit Images and Assets"  EnableViewState="false" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption">
        <h1><asp:Localize ID="Caption" runat="server" Text="Images and Assets"></asp:Localize></h1>
    </div>
    <div class="content">
        <asp:Label ID="SavedMessage" runat="server" Text="Images saved at {0:t}" SkinID="GoodCondition" EnableViewState="False" Visible="false"></asp:Label>
    </div>
</div>
<table cellpadding="3" cellspacing="0" class="inputForm" width="100%">
    <tr class="sectionHeader">
        <td colspan="2">
            <asp:Localize ID="BasicImagesCaption" runat="server" Text="Standard Images"></asp:Localize>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="IconImageUrlLabel" runat="server" Text="Icon:" AssociatedControlId="IconImageUrl" ToolTip="Specifies the icon image that may be associated with this product, such as in the mini basket."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="IconImageUrl" runat="server" width="400px" MaxLength="250"></asp:TextBox>
            <asp:ImageButton ID="BrowseIconUrl" runat="server" SkinID="FindIcon" AlternateText="Browse" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="IconImageAltTextLabel" runat="server" Text="Alternate Text:" AssociatedControlId="IconImageAltText" ToolTip="Specifies the alternate text that should be set on the icon image."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="IconImageAltText" runat="server" width="150px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="ThumbnailImageUrlLabel" runat="server" Text="Thumbnail:" AssociatedControlId="ThumbnailImageUrl" ToolTip="Specifies the thumbnail image that may be used with this item on some display pages."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="ThumbnailImageUrl" runat="server" width="400px" MaxLength="250"></asp:TextBox>
            <asp:ImageButton ID="BrowseThumbnailUrl" runat="server" SkinID="FindIcon" AlternateText="Browse" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="ThumbnailImageAltTextLabel" runat="server" Text="Alternate Text:" AssociatedControlId="ThumbnailImageAltText" ToolTip="Specifies the alternate text that should be set on the thumbnail image.  Leave blank to use the item name."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="ThumbnailImageAltText" runat="server" width="150px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="StandardImageUrlLabel" runat="server" Text="Standard Image:" AssociatedControlId="StandardImageUrl" ToolTip="Specifies the standard sized image that may be used with this item on some display pages, such as the product detail page."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="StandardImageUrl" runat="server" width="400px" MaxLength="250"></asp:TextBox>
            <asp:ImageButton ID="BrowseStandardUrl" runat="server" SkinID="FindIcon" AlternateText="Browse" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="StandardImageAltTextLabel" runat="server" Text="Alternate Text:" AssociatedControlId="StandardImageAltText" ToolTip="Specifies the alternate text that should be set on the standard image.  Leave blank to use the item name."></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="StandardImageAltText" runat="server" width="150px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <asp:Repeater ID="AdditionalImagesRepeater" runat="server" OnItemDataBound="AdditionalImagesRepeater_ItemDataBound" OnItemCommand="AdditionalImagesRepeater_ItemCommand">
        <ItemTemplate>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ImageUrlLabel" runat="server" Text="Additional Image:" AssociatedControlId="ImageUrlLabel" ToolTip="Specifies the URL of an additional image text that should be associated with the product."></cb:ToolTipLabel>
                </th>
                <td>
                    <asp:TextBox ID="ImageUrl" runat="server" width="400px" MaxLength="250" Text='<%# Eval("ImageUrl") %>'></asp:TextBox>
                    <asp:ImageButton ID="Browse" runat="server" SkinID="FindIcon" AlternateText="Browse" />
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="AltTextLabel" runat="server" Text="Alternate Text:" AssociatedControlId="AltText" ToolTip="Specifies the alternate text that is associated with the additional image."></cb:ToolTipLabel>
                </th>
                <td>
                    <asp:TextBox ID="AltText" runat="server" width="150px" MaxLength="100" Text='<%# Eval("ImageAltText") %>'></asp:TextBox>
                    &nbsp;<asp:Button ID="DeleteImageButton" runat="server" CommandArgument='<%#Eval("ProductImageId")%>' CommandName="Delete" Text="Remove Image" OnClientClick="return confirm('Are you sure you want to remove this image from the list?')" />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:Button ID="SaveButton" runat="server" Text="Save Changes" OnClick="SaveButton_Click"></asp:Button>
            <asp:Button ID="BasicImages" runat="server" Text="Basic Images" OnClick="SaveButton_Click"></asp:Button>
            <asp:Button ID="AdditionalImages" runat="server" Text="Additional Images" OnClick="SaveButton_Click"></asp:Button>
            <asp:Button ID="AddAdditionalImageButton" runat="server" Text="Add Image" OnClick="AddAdditionalImageButton_Click" />
        </td>
    </tr>
    <tr class="sectionHeader">
        <td colspan="2">
            <asp:Localize ID="AssetsCaption" runat="server" Text="Additional Assets"></asp:Localize>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <ajax:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="ProductAssetsGridView" runat="server" AutoGenerateColumns="false" ShowHeader="false" GridLines="Horizontal" 
                        DataKeyNames="ProductAssetId" 
                        OnRowDeleting="ProductAssetsGridView_RowDeleting">
                        <Columns>
                            <asp:TemplateField>
                                <ItemStyle VerticalAlign="top" />
                                <ItemTemplate>
                                    <asp:Label ID="ProductAssetUrl" runat="server" Text='<%#Eval("AssetUrl")%>' ></asp:Label><br />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemStyle VerticalAlign="top" />
                                <ItemTemplate>
                                    <asp:Button ID="ProductAssetDelete" runat="server" Text="Delete" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this asset?')" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView><br />
                    <asp:Button ID="AddProductAssetButton" runat="server" Text="Add Asset" OnClick="AddProductAssetButton_Click" /><br />
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
    </tr>    
</table>
<!--
<asp:Image ID="ThumbnailImagePreview" runat="server" /><br />
<asp:Image ID="StandardImagePreview" runat="server"/>
-->
</asp:Content>
