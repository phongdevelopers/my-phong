<%@ Page Language="C#" MasterPageFile="~/Admin/Catalog/Webpage-Link.master" CodeFile="EditLink.aspx.cs" Inherits="Admin_Catalog_EditLink" Title="Edit Link" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/Catalog/LinkMenu.ascx" TagName="LinkMenu" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PrimarySidebarContent" Runat="Server">
<uc:LinkMenu ID="LinkMenu1" runat="server" />
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Edit Link '{0}'"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
	    <tr>
	        <td class="validation">
	            <asp:ValidationSummary ID="ErrorSummary" runat="server" />
				<asp:Label ID="SavedMessage" runat="server" Text="Link saved at {0:t}" SkinID="GoodCondition" EnableViewState="False" Visible="false"></asp:Label>
	        </td>
	    </tr>
	    <tr>
	        <td>
                <table class="inputForm">
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" AssociatedControlId="Name" ToolTip="Name of the item."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="Name" runat="server" Text="" MaxLength="100" width="250px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NameRequiredValidator" runat="server" Text="*" Display="Dynamic" ErrorMessage="Name is required." ControlToValidate="Name"></asp:RequiredFieldValidator>
                        </td>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="VisibilityLabel" runat="server" Text="Visibility:" AssociatedControlID="Visibility" ToolTip="Public items are accessible and display in navigation and search results.  Hidden items are accessible only by direct link, and do not appear in navigation or search results.  Private items may not be accessed from the retail store."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:DropDownList ID="Visibility" runat="server">
                                <asp:ListItem Value="0" Text="Public"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Hidden"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Private"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" nowrap>
                            <cb:ToolTipLabel ID="NavigateUrlLabel" runat="server" Text="Link Url:" AssociatedControlID="TargetUrl" ToolTip="The url that is associated with this link."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="TargetUrl" runat="server" Text="" MaxLength="250" width="250px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NavigateUrlRequiredValidator" runat="server" Text="*" Display="Dynamic" ErrorMessage="Link URL is required" ControlToValidate="TargetUrl"></asp:RequiredFieldValidator>
                        </td>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="TargetLabel" runat="server" Text="TargetWindow:" AssociatedControlID="TargetWindow" ToolTip="The target window for the link when it is opened."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:DropDownList ID="TargetWindow" runat="server">
                                <asp:ListItem Value="" Text="Unspecified"></asp:ListItem>
                                <asp:ListItem Value="_self" Text="Current Frame (_self)"></asp:ListItem>
                                <asp:ListItem Value="_blank" Text="New Window (_blank)"></asp:ListItem>
                                <asp:ListItem Value="_top" Text="Top Window (_top)"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="ThumbnailUrlLabel" runat="server" Text="Thumbnail:" AssociatedControlId="ThumbnailUrl" ToolTip="Specifies the thumbnail image that may be used with this item on some display pages."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="ThumbnailUrl" runat="server" MaxLength="250" width="250px"></asp:TextBox>&nbsp;
                            <asp:ImageButton ID="BrowseThumbnailUrl" runat="server" SkinID="FindIcon" AlternateText="Browse" />
                        </td>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="ThumbnailAltTextLabel" runat="server" Text="Alt Text:" AssociatedControlId="ThumbnailAltText" ToolTip="Specifies the alternate text that should be set on the thumbnail image.  Leave blank to use the item name."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="ThumbnailAltText" runat="server" MaxLength="250"></asp:TextBox>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="SummaryLabel" runat="server" Text="Summary:" AssociatedControlId="Summary" ToolTip="Description of the item that may be shown on some display pages."></cb:ToolTipLabel>
                        </th>
                        <td colspan="3">
                            <asp:TextBox ID="Summary" runat="server" Text="" TextMode="multiLine" Rows="5" MaxLength="1000" Columns="130"></asp:TextBox><br />
                            <asp:Label ID="SummaryCharCount" runat="server" Text="1000"></asp:Label>
                            <asp:Label ID="SummaryCharCountLabel" runat="server" Text="characters remaining"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" style="vertical-align:top;">
                            <cb:ToolTipLabel ID="DescriptionLabel" runat="server" Text="Description:" ToolTip="This is used by some display pages to show a detailed description or HTML content."></cb:ToolTipLabel><br />
                            <asp:ImageButton ID="LinkDescriptionHtml" runat="server" SkinID="HtmlIcon" />                            
                        </th>
                        <td colspan="3">
                            <asp:TextBox ID="Description" runat="server" Text="" TextMode="multiLine" Rows="5" Columns="130"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top" nowrap>
                            <cb:ToolTipLabel ID="HtmlHeadLabel" runat="server" Text="HTML HEAD:" ToolTip="Enter the data to include in the HTML HEAD portion of the display page, such as meta keywords and description."></cb:ToolTipLabel>
                        </th>
                        <td colspan="3">
                            <asp:TextBox ID="HtmlHead" runat="server" Text="" TextMode="multiLine" Rows="5" Columns="130"></asp:TextBox><br />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top" nowrap>
                            <cb:ToolTipLabel ID="DisplayPageLabel" runat="server" Text="Display Page:" AssociatedControlId="DisplayPage" ToolTip="Specifies the script that will handle the display of this item."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <ajax:UpdatePanel ID="DisplayPageAjax" runat="server" UpdateMode="always">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DisplayPage" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DisplayPage_SelectedIndexChanged">
                                        <asp:ListItem Text="None (Use Direct Link)" Value=""></asp:ListItem>
                                    </asp:DropDownList><br /><br />
                                    <asp:Label ID="DisplayPageDescription" runat="Server" Text=""></asp:Label>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="LocalThemeLabel" runat="server" Text="Theme:" AssociatedControlId="LocalTheme" ToolTip="Specifies the theme that will be used to display this item.  This is only applicable if a display page is used.  For direct links, this value is ignored."></cb:ToolTipLabel>
                        </th>
                        <td valign="top">
                            <asp:DropDownList ID="LocalTheme" runat="server" AppendDataBoundItems="true" DataTextField="DisplayName" DataValueField="Name">
                                <asp:ListItem Text="Use store default" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td colspan="3">                            
                            <asp:Button ID="FinishButton" runat="server" Text="Save and Close"  OnClick="FinishButton_Click" />
							<asp:Button ID="SaveButton" runat="server" Text="Save"  OnClick="SaveButton_Click" />
							<asp:HyperLink ID="PreviewButton" runat="server" Text="Preview" SkinID="Button" Target="_blank" />
							<asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="CancelButton_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

