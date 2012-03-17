<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddressBook.ascx.cs" Inherits="Admin_People_Users_AddressBook" %>
<asp:UpdatePanel ID="ManageStoreAjax" runat="server">
    <ContentTemplate>
        <asp:ValidationSummary ID="AddressValidationSummary" runat="server" ValidationGroup="Address" />
        <asp:Label ID="SavedMessage" runat="server" Text="Billing address saved at {0:t}." SkinID="GoodCondition" Visible="false"></asp:Label>
        <table class="inputForm">
           <tr class="sectionHeader">
                <th colspan="4">
                    <asp:Label ID="BillingAddressCaption" runat="server" Text="Billling Address:"></asp:Label>
                </th>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="FirstName"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="FirstName" runat="server" MaxLength="30"></asp:TextBox> 
                </td>
                <th class="rowHeader">
                    <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="LastName"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="LastName" runat="server" MaxLength="50"></asp:TextBox> 
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="CompanyLabel" runat="server" Text="Company:" AssociatedControlID="Company"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Company" runat="server" MaxLength="50"></asp:TextBox> 
                </td>
                <th class="rowHeader" valign="top">
                    <asp:Label ID="PhoneLabel" runat="server" Text="Phone:" AssociatedControlID="Phone"></asp:Label>
                </th>
                <td colspan="3">
                    <asp:TextBox ID="Phone" runat="server" MaxLength="50"></asp:TextBox><br />
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="Address1Label" runat="server" Text="Street Address 1:" AssociatedControlID="Address1"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Address1" runat="server" MaxLength="100"></asp:TextBox> 
                </td>
                
                <th class="rowHeader">
                    <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:" AssociatedControlID="Address2"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Address2" runat="server" MaxLength="100"></asp:TextBox> 
                </td>
                
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="City" runat="server" MaxLength="50"></asp:TextBox> 
                </td>
                <th class="rowHeader">
                    <asp:Label ID="ProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="Province"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Province" runat="server" MaxLength="50"></asp:TextBox> 
                </td>
                
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="PostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="PostalCode"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="PostalCode" runat="server" MaxLength="15"></asp:TextBox> 
                </td>
                <th class="rowHeader">
                    <asp:Label ID="CountryLabel" runat="server" Text="Country:" AssociatedControlID="CountryCode"></asp:Label>
                </th>
                <td>
                    <asp:DropDownList ID="CountryCode" runat="server" DataTextField="Name" DataValueField="CountryCode"></asp:DropDownList>
                </td>
                
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="FaxLabel" runat="server" Text="Fax:"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Fax" runat="server"></asp:TextBox> 
                </td>
                <th class="rowHeader" valign="top">
                    <asp:Label ID="ResidenceLabel" runat="server" Text="Type:" AssociatedControlID="Residence"></asp:Label>
                </th>
                <td valign="top">
                    <asp:DropDownList ID="Residence" runat="server">
                        <asp:ListItem Text="This is a residence" Value="1"></asp:ListItem>
                        <asp:ListItem Text="This is a business" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="3">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                </td>
            </tr>
        </table><br />
    </ContentTemplate>
</asp:UpdatePanel>