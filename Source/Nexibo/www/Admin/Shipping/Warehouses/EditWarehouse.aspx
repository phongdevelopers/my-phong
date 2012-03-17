<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditWarehouse.aspx.cs" Inherits="Admin_Shipping_Warehouses_EditWarehouse" Title="Edit Warehouse"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Edit Warehouse '{0}'" EnableViewState="False"></asp:Localize></h1>
    	</div>
    </div>
<ajax:UpdatePanel ID="EditAddressAjax" runat="server">
    <ContentTemplate>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>       
                        <table class="inputForm" cellpadding="3">
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Name" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Name" runat="server" MaxLength="50" EnableViewState="false"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                                            Display="Static" ErrorMessage="Warehouse name is required.">*</asp:RequiredFieldValidator>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="Label1" runat="server" Text="Street Address 1:" AssociatedControlID="Address1" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Address1" runat="server" MaxLength="100" EnableViewState="false"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="Address1Required" runat="server" Text="*"
                                        ErrorMessage="Address is required." Display="Static" ControlToValidate="Address1"
                                        EnableViewState="false"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:" AssociatedControlID="Address2" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Address2" runat="server" MaxLength="100" EnableViewState="false"></asp:TextBox> 
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="City" runat="server" MaxLength="50" EnableViewState="false"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="CityRequired" runat="server" Text="*"
                                        ErrorMessage="City is required." Display="Static" ControlToValidate="City"
                                        EnableViewState="false"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="Province" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Province" runat="server" MaxLength="30"></asp:TextBox> 
                                    <asp:DropDownList ID="Province2" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="Province2Required" runat="server" Text="*"
                                        ErrorMessage="State or province is required." Display="Static" ControlToValidate="Province2"></asp:RequiredFieldValidator>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="PostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="PostalCode" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="PostalCode" runat="server" MaxLength="10" EnableViewState="false"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="PostalCodeRequired" runat="server" Text="*"
                                        ErrorMessage="ZIP or Postal Code is required." Display="Static" ControlToValidate="PostalCode"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="CountryLabel" runat="server" Text="Country:" AssociatedControlID="Country" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:DropDownList ID="Country" runat="server" DataTextField="Name" DataValueField="CountryCode" 
                                        OnSelectedIndexChanged="Country_Changed" AutoPostBack="true" ></asp:DropDownList>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="PhoneLabel" runat="server" Text="Phone:" AssociatedControlID="Phone" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Phone" runat="server" MaxLength="30" EnableViewState="false"></asp:TextBox> 
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader" valign="top">
                                    <asp:Label ID="FaxLabel" runat="server" Text="Fax:" AssociatedControlID="Fax" EnableViewState="false"></asp:Label>
                                </th>
                                <td valign="top">
                                    <asp:TextBox ID="Fax" runat="server" MaxLength="30" EnableViewState="false"></asp:TextBox> 
                                </td>
                                <th class="rowHeader" valign="top">
                                    <asp:Label ID="EmailLabel" runat="server" Text="Email:" AssociatedControlID="Email" EnableViewState="false"></asp:Label>
                                </th>
                                <td valign="top">
                                    <asp:TextBox ID="Email" runat="server" MaxLength="100" EnableViewState="false"></asp:TextBox> 
                                    <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="Email" Required="false" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td colspan="3">
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableViewState="false" />
                                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" EnableViewState="false"></asp:Button>
                                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" EnableViewState="false"></asp:Button>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:updatePanel>
            </td>
        </tr>
    </table>
    </ContentTemplate>
</ajax:updatePanel>
</asp:Content>
