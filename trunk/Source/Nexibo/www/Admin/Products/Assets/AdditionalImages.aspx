<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="AdditionalImages.aspx.cs" Inherits="Admin_Products_Assets_AdditionalImages" Title="Additional Images" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Additional Images for {0}"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
	<tr>
        <td>
            <p align="justify"><asp:Localize ID="InstructionText" runat="server" Text="Shown below are any additional images you have attached to this product.  You can upload an additional image or return to the basic images for the product.  Use Advanced Options for more control, such as the ability to specify image paths and set alt text."></asp:Localize></p>
            <table class="inputForm">
                <asp:Repeater ID="AdditionalImagesRepeater" runat="server" OnItemCommand="AdditionalImagesRepeater_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <th class="rowHeader" nowrap>
                                <cb:ToolTipLabel ID="ImageUrlLabel" runat="server" Text="Additional Image:" AssociatedControlId="ImageUrlLabel" ToolTip="Specifies the URL of an additional image text that should be associated with the product."></cb:ToolTipLabel><br /><br />
                                <asp:Button ID="DeleteImageButton" runat="server" CommandArgument='<%#Eval("ProductImageId")%>' CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to remove this image from the list?')" />
                            </th>
                            <td>
            	                <div style="overflow:auto; height:300px; width:550px; padding:0px;" class="contentSection">
                                    <asp:Image ID="ImageUrl" runat="server" ImageUrl='<%# GetImageUrl(Container.DataItem) %>' />
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr class="empty" id="trEmpty" runat="server">
                    <td colspan="2">
                        <asp:Localize ID="EmptyMessage" runat="server" Text="There are no additional images attached to this product."></asp:Localize>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <br />
                        <asp:Button ID="UploadButton" runat="server" Text="Upload Image" OnClick="UploadButton_Click" />
                        <asp:Button ID="AdvancedButton" runat="server" Text="Pick or Edit Images" OnClick="AdvancedButton_Click" />
                        <asp:Button ID="BasicButton" runat="server" Text="Basic Images" OnClick="BasicButton_Click" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</asp:Content>