<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="EditShipZone.aspx.cs" Inherits="Admin_Shipping_Zones_EditShipZone" Title="Edit Zone" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Edit Zone '{0}'" EnableViewState="false"></asp:Localize></h1>
    	</div>
    </div>
    <ajax:UpdatePanel ID="ShipZoneAjax" runat="server">
        <ContentTemplate>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            <table class="inputForm" cellspacing="0" cellpadding="4" width="720px">
                <tr>
                    <th class="rowHeader">
                        <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="Name of the zone for merchant reference.  This value is not displayed to customers."></cb:ToolTipLabel>
                    </th>
                    <td valign="top">
                        <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                            Display="Static" ErrorMessage="Zone name is required.">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <cb:ToolTipLabel ID="CountryRuleLabel" runat="server" Text="Country Filter:" ToolTip="Indicate how countries are filtered for this zone." AssociatedControlId="CountryRule"></cb:ToolTipLabel>
                    </th>
                    <td valign="top">
                        <asp:DropDownList ID="CountryRule" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CountryRule_SelectedIndexChanged">
                            <asp:ListItem Text="Include All Countries" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Include Selected Countries" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Exclude Selected Countries" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="trCountryList" runat="server">
                    <th class="rowHeader" valign="top">
                        <cb:ToolTipLabel ID="CountryListLabel" runat="server" Text="Selected&nbsp;Countries:" ToolTip="The list of countries to use in conjunction with the country filter." AssociatedControlId="CountryList" />
                    </th>
                    <td valign="top">
                        <asp:Literal ID="CountryList" runat="server"></asp:Literal>
                        <asp:HiddenField ID="HiddenSelectedCountries" runat="server" />
                        &nbsp;<asp:LinkButton ID="ChangeCountryListButton" runat="server" Text="change"></asp:LinkButton>
                        <asp:Panel ID="ChangeCountryListDialog" runat="server" Style="display: none" CssClass="modalPopup" Width="600px">
                            <asp:Panel ID="ChangeCountryListDialogHeader" runat="server" CssClass="modalPopupHeader">
                                Change Selected Countries
                            </asp:Panel>
                            <div align="center">
                                <br />
                                Hold CTRL to select multiple countries.  Double click to move a country to the other list.
                                <br /><br />
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top" width="42%">
                                            <b>Available Countries</b><br />
                                            <asp:ListBox ID="AvailableCountries" runat="server" Rows="12" SelectionMode="multiple" Width="220"></asp:ListBox>
                                        </td>
                                        <td valign="middle" width="6%">
                                            <asp:Button ID="SelectAllCountries" runat="server" Text=" >> " /><br />
                                            <asp:Button ID="SelectCountry" runat="server" Text=" > " /><br />
                                            <asp:Button ID="UnselectCountry" runat="server" Text=" < " /><br />
                                            <asp:Button ID="UnselectAllCountries" runat="server" Text=" << " /><br />
                                        </td>
                                        <td valign="top" width="42%">
                                            <b>Selected Countries</b><br />
                                            <asp:ListBox ID="SelectedCountries" runat="server" Rows="12" SelectionMode="multiple" Width="220"></asp:ListBox>
                                        </td>
                                    </tr>
                                </table><br />
                                <asp:Button ID="ChangeCountryListOKButton" runat="server" Text="OK" OnClick="ChangeCountryListOKButton_Click" />
                                <asp:Button ID="ChangeCountryListCancelButton" runat="server" Text="Cancel" />
                                <br /><br />
                            </div>
                        </asp:Panel>
                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender" runat="server" 
                            TargetControlID="ChangeCountryListButton"
                            PopupControlID="ChangeCountryListDialog" 
                            BackgroundCssClass="modalBackground" 
                            CancelControlID="ChangeCountryListCancelButton" 
                            DropShadow="true"
                            PopupDragHandleControlID="ChangeCountryListDialogHeader" />
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <cb:ToolTipLabel ID="ProvinceRuleLabel" runat="server" Text="Province Filter:" ToolTip="Indicate how provinces are filtered for this zone." AssociatedControlId="ProvinceRule"></cb:ToolTipLabel>
                    </th>
                    <td valign="top">
                        <asp:DropDownList ID="ProvinceRule" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ProvinceRule_SelectedIndexChanged">
                            <asp:ListItem Text="Include All Provinces" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Include Selected Provinces" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Exclude Selected Provinces" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="trProvinceList" runat="server">
                    <th class="rowHeader" valign="top">
                        <cb:ToolTipLabel ID="ProvinceListLabel" runat="server" Text="Selected&nbsp;Provinces:" ToolTip="The list of provinces to use in conjunction with the province filter." AssociatedControlId="ProvinceList" />
                    </th>
                    <td valign="top">
                        <asp:Literal ID="ProvinceList" runat="server"></asp:Literal>
                        <asp:HiddenField ID="HiddenSelectedProvinces" runat="server" />
                        &nbsp;<asp:LinkButton ID="ChangeProvinceListButton" runat="server" Text="change"></asp:LinkButton>
                        <asp:Panel ID="ChangeProvinceListDialog" runat="server" Style="display: none" CssClass="modalPopup" Width="600px">
                            <asp:Panel ID="ChangeProvinceListDialogHeader" runat="server" CssClass="modalPopupHeader">
                                Change Selected Provinces
                            </asp:Panel>
                            <div align="center">
                                <br />
                                Hold CTRL to select multiple Provinces.  Double click to move a Province to the other list.
                                <br /><br />
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top" width="42%">
                                            <b>Available Provinces</b><br />
                                            <asp:ListBox ID="AvailableProvinces" runat="server" Rows="12" SelectionMode="multiple" Width="220"></asp:ListBox>
                                        </td>
                                        <td valign="middle" width="6%">
                                            <asp:Button ID="SelectAllProvinces" runat="server" Text=" >> " /><br />
                                            <asp:Button ID="SelectProvince" runat="server" Text=" > " /><br />
                                            <asp:Button ID="UnselectProvince" runat="server" Text=" < " /><br />
                                            <asp:Button ID="UnselectAllProvinces" runat="server" Text=" << " /><br />
                                        </td>
                                        <td valign="top" width="42%">
                                            <b>Selected Provinces</b><br />
                                            <asp:ListBox ID="SelectedProvinces" runat="server" Rows="12" SelectionMode="multiple" Width="220"></asp:ListBox>
                                        </td>
                                    </tr>
                                </table><br />
                                <asp:Button ID="ChangeProvinceListOKButton" runat="server" Text="OK" OnClick="ChangeProvinceListOKButton_Click" />
                                <asp:Button ID="ChangeProvinceListCancelButton" runat="server" Text="Cancel" />
                                <br /><br />
                            </div>
                        </asp:Panel>
                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" 
                            TargetControlID="ChangeProvinceListButton"
                            PopupControlID="ChangeProvinceListDialog" 
                            BackgroundCssClass="modalBackground" 
                            CancelControlID="ChangeProvinceListCancelButton" 
                            DropShadow="true"
                            PopupDragHandleControlID="ChangeProvinceListDialogHeader" />
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <cb:ToolTipLabel ID="PostalCodeFilterLabel" runat="server" Text="Postal Code(s):" ToolTip='Any ZIP or postal code matching this filer will be included in the zone.  You may enter multiple codes separated by a comma.  You may also use regular expressoins - see help for details.'></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:TextBox ID="PostalCodeFilter" runat="server" Text="" Width="500px" MaxLength="255"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <cb:ToolTipLabel ID="ExcludePostalCodeFilterLabel" runat="server" Text="Exclude Postal Code(s):" ToolTip='ZIP or postal code matching this filter will be excluded from the zone.  You may enter multiple codes separated by a comma.  You may also use regular expressions - see help for details.'></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:TextBox ID="ExcludePostalCodeFilter" runat="server" Text="" Width="500px" MaxLength="255"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <br />                            
                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
				        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>