<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="AddShipment.aspx.cs" Inherits="Admin_Orders_Shipments_AddShipment" Title="Add Shipment" EnableViewState="false" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">    
    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Add New Shipment"></asp:Localize>
            </h1>
        </div>
    </div>
    <ajax:UpdatePanel ID="AddressAjax" runat="server">
        <ContentTemplate>
            <asp:ValidationSummary ID="ValidationSummary" runat="server" />
            <table class="inputForm" cellpadding="4" cellspacing="0" style="margin-top:8px;">
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="AddressLabel" runat="server" Text="Ship To:"></asp:Label>
                    </th>
                    <td>
                        <asp:DropDownList ID="AddressList" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="-- select address --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="AddressValidator" runat="server" ControlToValidate="AddressList"
                            Text="*" ErrorMessage="Ship to address is required."></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr id="trNewAddress" runat="server">
                    <td>&nbsp;</td>
                    <td>
                        <table class="inputForm" cellpadding="4" cellspacing="0" style="margin-top:8px;">
                            <tr class="sectionHeader">
                                <th colspan="4">
                                    <asp:Label ID="ShipToCaption" runat="server" Text="Enter New Address"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToFirstNameLabel" runat="server" Text="First Name:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToFirstName" runat="server" ValidationGroup="NewAddress"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ShipToFirstNameValidator" runat="server" ControlToValidate="ShipToFirstName" ErrorMessage="First name is required." Text="*" ValidationGroup="NewAddress"></asp:RequiredFieldValidator>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToLastNameLabel" runat="server" Text="Last Name:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToLastName" runat="server" ValidationGroup="NewAddress"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ShipToLastNameValidator" runat="server" ControlToValidate="ShipToLastName" ErrorMessage="Last name is required." Text="*" ValidationGroup="NewAddress"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToCompanyLabel" runat="server" Text="Company:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToCompany" runat="server"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToAddress1Label" runat="server" Text="Street Address 1:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToAddress1" runat="server" ValidationGroup="NewAddress"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ShipToAddress1Validator" runat="server" ControlToValidate="ShipToAddress1" ErrorMessage="First line of street address is required." Text="*" ValidationGroup="NewAddress"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToAddress2Label" runat="server" Text="Street Address 2:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToAddress2" runat="server"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToCityLabel" runat="server" Text="City:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToCity" runat="server" ValidationGroup="NewAddress"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ShipToCityValidator" runat="server" ControlToValidate="ShipToCity" ErrorMessage="City is required." Text="*" ValidationGroup="NewAddress"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToProvinceLabel" runat="server" Text="State / Province:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToProvince" runat="server"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToPostalCodeLabel" runat="server" Text="ZIP / Postal Code:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToPostalCode" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ShipToCountryCodeLabel" runat="server" Text="Country:"></asp:Label>
                                </th>
                                <td>
                                    <asp:DropDownList ID="ShipToCountryCode" runat="server" DataTextField="Name" DataValueField="CountryCode"></asp:DropDownList>
                                </td>
                                <th class="rowHeader" valign="top">
                                    <asp:Label ID="ShipToPhoneLabel" runat="server" Text="Phone:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="ShipToPhone" runat="server"></asp:TextBox><br />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader"><asp:Label ID="ShipMethodLabel" runat="server" Text="Shipping Method:"></asp:Label></th>
                    <td>
                        <asp:DropDownList ID="ShipMethodList" runat="server" DataSourceID="ShipMethodDs" DataTextField="Name" DataValueField="ShipMethodId" AppendDataBoundItems="True" AutoPostBack="true">
                            <asp:ListItem Text=""></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="trShipCharge" runat="server">
                    <th class="rowHeader"><asp:Label ID="ShipRate" runat="server" Text="Shipping Charge:"></asp:Label></th>
                    <td><asp:TextBox ID="ShipCharges" runat="server" Text="" Width="40px"></asp:TextBox></td>
                </tr>
                <tr>
                    <th class="rowHeader" valign="top"><asp:Label ID="ShipMessageLabel" runat="server" Text="Shipping Message:"></asp:Label></th>
                    <td><asp:TextBox ID="ShipMessage" runat="server" Height="74px" Rows="3" Width="257px" Wrap="true" TextMode="multiline"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td><asp:Button ID="SaveButton" runat="server" Text="Save New Shipment" OnClick="SaveButton_Click" /></td>
                </tr>
                </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="ShipMethodDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Shipping.ShipMethodDataSource">
    </asp:ObjectDataSource>
</asp:Content>
