<%@ Page Language="C#" MasterPageFile="../Product.master" CodeFile="Images.aspx.cs" Inherits="Admin_Products_Assets_Images" Title="Basic Images"  EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Basic Images for {0}"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
	<tr>
        <td>
            <asp:Localize ID="InstructionText" runat="server" Text="Shown below are the icon, thumbnail, and detail image, if available for this product.  You can upload a new image or attach additional images to the product.  Use Advanced Options for more control, such as the ability to specify image paths and set alt text."></asp:Localize>
            <table class="inputForm">
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="IconPreviewLabel" runat="server" Text="Icon:" AssociatedControlID="IconPreview"></asp:Label>
                    </th>
                    <td>
                        <asp:Image ID="IconPreview" runat="server"/>
                        <asp:Literal ID="IconPreviewNoImage" runat="server" Text="No Image Specified" Visible="false"/>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="ThumbnailPreviewLabel" runat="server" Text="Thumbnail:" AssociatedControlID="ThumbnailPreview"></asp:Label>
                    </th>
                    <td>
                        <asp:Image ID="ThumbnailPreview" runat="server" />
                        <asp:Literal ID="ThumbnailPreviewNoImage" runat="server" Text="No Image Specified" Visible="false"/>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="ImagePreviewLabel" runat="server" Text="Image:" AssociatedControlID="ImagePreview"></asp:Label>
                    </th>
                    <td>
                        <asp:Image ID="ImagePreview" runat="server" />
                        <asp:Literal ID="ImagePreviewNoImage" runat="server" Text="No Image Specified" Visible="false"/>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button ID="UploadButton" runat="server" Text="Upload Image" OnClick="UploadButton_Click" />                        
                        <asp:Button ID="AdvancedButton" runat="server" Text="Pick or Edit Images" OnClick="AdvancedButton_Click" />
                        <asp:Button ID="AdditionalButton" runat="server" Text="Additional Images" OnClick="AdditionalButton_Click" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</asp:Content>