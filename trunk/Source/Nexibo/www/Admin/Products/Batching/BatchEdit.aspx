<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="BatchEdit.aspx.cs" Inherits="Admin_Products_Batching_BatchEdit" Title="Product Batch Editing" EnableViewState="false" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register TagPrefix="FCKeditorV2" Namespace="FredCK.FCKeditorV2" Assembly="FredCK.FCKeditorV2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<style> #pageLayout { width:100%; } </style>
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Batch Edit"></asp:Localize></h1>
	</div>
</div>
<div class="section">
    <div class="content">
        <asp:UpdatePanel ID="EditAjax" runat="server">
            <ContentTemplate>
                <asp:PlaceHolder ID="SearchPanel" runat="server">
                    <table width="100%">
                        <tr>
                            <td valign="top" width="350px">
                                <b><asp:Localize ID="SearchHelpText" runat="server">
                                    Search for the product(s) to edit:
                                </asp:Localize></b>
                                <br /><br />
                                <table class="inputForm" cellpadding="2" width="100%">
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="NameFilterLabel" runat="server" Text="Name:" AssociatedControlID="NameFilter" ToolTip="Enter all or part of a product name to search for.  You can use the * and ? wildcards."></cb:ToolTipLabel>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="NameFilter" runat="server" MaxLength="50" Width="180px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="SkuFilterLabel" runat="server" Text="Sku:" AssociatedControlID="SkuFilter" ToolTip="Enter all or part of a product SKU to search for.  You can use the * and ? wildcards."></cb:ToolTipLabel>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="SkuFilter" runat="server" MaxLength="50" Width="180px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="CategoryFilterLabel" runat="server" Text="Category:" AssociatedControlID="CategoryFilter" ToolTip="Select the category to limit your search."></cb:ToolTipLabel>
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="CategoryFilter" runat="server" AppendDataBoundItems="True" DataTextField="Name" DataValueField="CategoryId" Width="180px">
                                                <asp:ListItem Text="- Any Category -" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="ManufacturerFilterLabel" runat="server" Text="Manufacturer:" AssociatedControlID="ManufacturerFilter" ToolTip="Select the manufacturer to limit your search."></cb:ToolTipLabel>
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="ManufacturerFilter" runat="server" AppendDataBoundItems="True" DataTextField="Name" DataValueField="ManufacturerId" Width="180px">
                                                <asp:ListItem Text="- Any Manufacturer -" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="VendorFilterLabel" runat="server" Text="Vendor:" AssociatedControlID="VendorFilter" ToolTip="Select the vendor to limit your search."></cb:ToolTipLabel>
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="VendorFilter" runat="server" AppendDataBoundItems="True" DataTextField="Name" DataValueField="VendorId" Width="180px">
                                                <asp:ListItem Text="- Any Vendor -" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="MaximumRowsLabel" runat="server" Text="Max Results:" AssociatedControlID="MaximumRows" ToolTip="Select the maximum number of matches to return."></cb:ToolTipLabel>
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="MaximumRows" runat="server" Width="180px">
                                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="20" Value="20" Selected="true"></asp:ListItem>
                                                <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                                <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                                <asp:ListItem Text="200" Value="200"></asp:ListItem>
                                                <asp:ListItem Text="500" Value="500"></asp:ListItem>
                                                <asp:ListItem Text="1000" Value="1000"></asp:ListItem>
                                                <asp:ListItem Text="All" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="EnableScrollingLabel" runat="server" Text="Enable Scrolling:" AssociatedControlID="EnableScrolling" ToolTip="If you select a large number of fields, checking this box will keep the editing grid within the bounds of the screen and use scrollbars.  If unchecked, the grid will stretch out as far as necessary."></cb:ToolTipLabel>
                                        </th>
                                        <td>
                                            <asp:CheckBox ID="EnableScrolling" runat="server" Checked="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <asp:LinkButton ID="SearchButton" runat="server" Text="Search" SkinID="Button" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Localize ID="NoResultsMessage" runat="server" Text="There were no results for the search." Visible="false"></asp:Localize>
                            </td>
                            <td valign="top">
                                <b><asp:Localize ID="SelectedFieldsHelpText" runat="server">
                                    Indicate the field(s) to be edited:
                                </asp:Localize></b>
                                <br /><br />
                                <table class="inputForm" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <th class="rowHeader" style="text-align:left" width="240px">
                                            <cb:ToolTipLabel ID="AvailableFieldsLabel" runat="server" Text="Available Fields:" AssociatedControlID="AvailableFields" ToolTip="These fields will not be displayed in the grid for editing.  You can double click a field name to move it to the selected fields box."></cb:ToolTipLabel>
                                        </th>
                                        <td width="46px">&nbsp;</td>
                                        <th class="rowHeader" style="text-align:left" width="240px">
                                            <cb:ToolTipLabel ID="SelectedFieldsLabel" runat="server" Text="Selected Fields:" AssociatedControlID="SelectedFields" ToolTip="These fields will be displayed in the grid for editing.  You can double click a field name to move it to the unselected fields box."></cb:ToolTipLabel>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td align="center" valign="top" width="240px">
                                            <asp:ListBox ID="AvailableFields" runat="server" Rows="10" SelectionMode="multiple" Width="250px"></asp:ListBox>
                                        </td>
                                        <td align="center" valign="middle" width="46px">
                                            <asp:Button ID="SelectAllFields" runat="server" Text=" >> " /><br />
                                            <asp:Button ID="SelectField" runat="server" Text=" > " /><br />
                                            <asp:Button ID="UnselectField" runat="server" Text=" < " /><br />
                                            <asp:Button ID="UnselectAllFields" runat="server" Text=" << " /><br />
                                        </td>
                                        <td align="center" valign="top" width="240px">
                                            <asp:ListBox ID="SelectedFields" runat="server" Rows="10" SelectionMode="multiple" Width="250px"></asp:ListBox>
                                            <asp:HiddenField ID="HiddenSelectedFields" runat="server" />
                                        </td>
                                    </tr>
                                </table><br />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="EditPanel" runat="server">
                    <asp:PlaceHolder ID="BatchEditGrid" runat="server"></asp:PlaceHolder>
                    <asp:LinkButton ID="NewSearchButton" runat="server" Text="New Search" SkinID="Button" />
                    &nbsp;<asp:LinkButton ID="SaveButton" runat="server" Text="Save" SkinID="Button" />
                    <asp:PlaceHolder ID="SavedMessagePanel" runat="server" Visible="false">
                        <br /><br />
                        <asp:Label ID="SavedMessage" runat="server" Text="Data updated at {0}." SkinID="GoodCondition"></asp:Label>
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
                <asp:Panel ID="EditHtmlDialog" runat="server" Style="display:none;width:800px" CssClass="modalPopup">
                    <asp:Panel ID="EditHtmlDialogHeader" runat="server" CssClass="modalPopupHeader">
                        Edit HTML
                    </asp:Panel>
                    <div style="padding-top:5px;">
                        <FCKeditorV2:FCKeditor id="Editor" runat="server" Height="460" />
                        <div style="text-align:center">
                            <asp:Button ID="EditHtmlSaveButton" runat="server" Text="OK" style="width:70px" CausesValidation="false" OnClientClick="return SaveEditHtmlDialog()" />
                            <asp:Button ID="EditHtmlCancelButton" runat="server" Text="Cancel" style="width:70px" CausesValidation="false" OnClientClick="return HideEditHtmlDialog()" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:HiddenField ID="EditHtmlTarget" runat="server" />
                <asp:HiddenField ID="EditHtmlCancelTarget" runat="server" />
                <ajax:ModalPopupExtender ID="EditHtmlPopup" runat="server" 
                    TargetControlID="EditHtmlTarget"
                    PopupControlID="EditHtmlDialog" 
                    BackgroundCssClass="modalBackground"                         
                    CancelControlID="EditHtmlCancelTarget" 
                    DropShadow="true"
                    PopupDragHandleControlID="EditHtmlDialogHeader" />
                <asp:Panel ID="EditLongTextDialog" runat="server" Style="display:none;width:800px" CssClass="modalPopup">
                    <asp:Panel ID="EditLongTextDialogHeader" runat="server" CssClass="modalPopupHeader">
                        Edit Text
                    </asp:Panel>
                    <div style="padding-top:5px;text-align:center">
                        <asp:TextBox ID="EditLongText" runat="server" TextMode="MultiLine" Width="760px" Height="400px"></asp:TextBox><br />
                        <asp:Button ID="EditLongTextSaveButton" runat="server" Text="OK" style="width:70px" CausesValidation="false" OnClientClick="return SaveEditLongTextDialog()" />
                        <asp:Button ID="EditLongTextCancelButton" runat="server" Text="Cancel" style="width:70px" CausesValidation="false" OnClientClick="return HideEditLongTextDialog()" />
                    </div>
                </asp:Panel>
                <asp:HiddenField ID="EditLongTextTarget" runat="server" />
                <asp:HiddenField ID="EditLongTextCancelTarget" runat="server" />
                <ajax:ModalPopupExtender ID="EditLongTextPopup" runat="server" 
                    TargetControlID="EditLongTextTarget"
                    PopupControlID="EditLongTextDialog" 
                    BackgroundCssClass="modalBackground"                         
                    CancelControlID="EditLongTextCancelTarget" 
                    DropShadow="true"
                    PopupDragHandleControlID="EditLongTextDialogHeader" />
                <asp:HiddenField ID="VS" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
</asp:Content>

