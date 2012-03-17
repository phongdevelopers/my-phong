<%@ Page Language="C#" MasterPageFile="Order.master" CodeFile="EditAddresses.aspx.cs" Inherits="Admin_Orders_EditAddresses" Title="Edit Addresses"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="EditAddressesLabel" runat="server" Text="Edit Addresses"></asp:Localize></h1>
        </div>
    </div>
    <ajax:UpdatePanel ID="EditAddressAjax" runat="server" UpdateMode="conditional">
        <ContentTemplate>
            <asp:Label ID="SavedMessage" runat="server" text="Address(es) updated at {0:t}" Visible="false" Width="100%" EnableViewState="false" SkinID="GoodCondition"></asp:Label>
            <table class="inputForm" cellpadding="4" cellspacing="0" style="margin-top:8px;">
                <tr class="sectionHeader">
                    <th colspan="4">
                        <asp:Label ID="BillToCaption" runat="server" Text="Bill To:"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="BillToFirstNameLabel" runat="server" Text="First Name:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BillToFirstName" runat="server"></asp:TextBox> 
                        <asp:RequiredFieldValidator ID="BillToFirstNameValidator" runat="server" ControlToValidate="BillToFirstName" ErrorMessage="First name is required." Text="*"></asp:RequiredFieldValidator>
                    </td>
                    <th class="rowHeader">
                        <asp:Label ID="BillToLastNameLabel" runat="server" Text="Last Name:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BillToLastName" runat="server"></asp:TextBox> 
                        <asp:RequiredFieldValidator ID="BillToLastNameValidator" runat="server" ControlToValidate="BillToLastName" ErrorMessage="Last name is required." Text="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="BillToCompanyLabel" runat="server" Text="Company:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BillToCompany" runat="server"></asp:TextBox> 
                    </td>
                    <th class="rowHeader" valign="top">
                        <asp:Label ID="BillToPhoneLabel" runat="server" Text="Phone:"></asp:Label>
                    </th>
                    <td colspan="3">
                        <asp:TextBox ID="BillToPhone" runat="server"></asp:TextBox><br />
                    </td>
                </tr>
                <tr>
                <th class="rowHeader">
                        <asp:Label ID="BillToAddress1Label" runat="server" Text="Street Address 1:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BillToAddress1" runat="server"></asp:TextBox> 
                        <asp:RequiredFieldValidator ID="BillToAddress1Validator" runat="server" ControlToValidate="BillToAddress1" ErrorMessage="First line of street address is required." Text="*"></asp:RequiredFieldValidator>
                    </td>
                    <th class="rowHeader">
                        <asp:Label ID="BillToAddress2Label" runat="server" Text="Street Address 2:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BillToAddress2" runat="server"></asp:TextBox> 
                    </td>
                    
                </tr>
                <tr>
                <th class="rowHeader">
                        <asp:Label ID="BillToCityLabel" runat="server" Text="City:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BillToCity" runat="server"></asp:TextBox> 
                        <asp:RequiredFieldValidator ID="BillToCityValidator" runat="server" ControlToValidate="BillToCity" ErrorMessage="City is required." Text="*"></asp:RequiredFieldValidator>
                    </td>
                    <th class="rowHeader">
                        <asp:Label ID="BillToProvinceLabel" runat="server" Text="State / Province:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BillToProvince" runat="server"></asp:TextBox> 
                    </td>
                    
                </tr>
                <tr>
                <th class="rowHeader">
                        <asp:Label ID="BillToPostalCodeLabel" runat="server" Text="ZIP / Postal Code:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BillToPostalCode" runat="server"></asp:TextBox> 
                    </td>
                    <th class="rowHeader">
                        <asp:Label ID="BillToCountryLabel" runat="server" Text="Country:"></asp:Label>
                    </th>
                    <td>
                        <asp:DropDownList ID="BillToCountryCode" runat="server" DataTextField="Name" DataValueField="CountryCode"></asp:DropDownList>
                    </td>
                    
                </tr>
            </table>
            <asp:Repeater ID="ShipmentRepeater" runat="server" OnItemDataBound="ShipmentRepeater_ItemDataBound">
                <ItemTemplate>
                    <table class="inputForm" cellpadding="4" cellspacing="0" style="margin-top:8px;">
                        <tr class="sectionHeader">
                            <th colspan="4">
                                <asp:Label ID="ShipToCaption" runat="server" Text='<%# string.Format("Shipment #{0}", (Container.ItemIndex + 1)) %>'></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="ShipToFirstNameLabel" runat="server" Text="First Name:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToFirstName" runat="server"></asp:TextBox> 
                                <asp:RequiredFieldValidator ID="ShipToFirstNameValidator" runat="server" ControlToValidate="ShipToFirstName" ErrorMessage="First name is required." Text="*"></asp:RequiredFieldValidator>
                            </td>
                            <th class="rowHeader">
                                <asp:Label ID="ShipToLastNameLabel" runat="server" Text="Last Name:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToLastName" runat="server"></asp:TextBox> 
                                <asp:RequiredFieldValidator ID="ShipToLastNameValidator" runat="server" ControlToValidate="ShipToLastName" ErrorMessage="Last name is required." Text="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="ShipToCompanyLabel" runat="server" Text="Company:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToCompany" runat="server"></asp:TextBox> 
                            </td>
                            <th class="rowHeader" valign="top">
                                <asp:Label ID="ShipToPhoneLabel" runat="server" Text="Phone:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToPhone" runat="server"></asp:TextBox><br />
                            </td>
                            
                        </tr>
                        <tr>
                        <th class="rowHeader">
                                <asp:Label ID="ShipToAddress1Label" runat="server" Text="Street Address 1:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToAddress1" runat="server"></asp:TextBox> 
                                <asp:RequiredFieldValidator ID="ShipToAddress1Validator" runat="server" ControlToValidate="ShipToAddress1" ErrorMessage="First line of street address is required." Text="*"></asp:RequiredFieldValidator>
                            </td>
                            <th class="rowHeader">
                                <asp:Label ID="ShipToAddress2Label" runat="server" Text="Street Address 2:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToAddress2" runat="server"></asp:TextBox> 
                            </td>
                            
                        </tr>
                        <tr>
                        <th class="rowHeader">
                                <asp:Label ID="ShipToCityLabel" runat="server" Text="City:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToCity" runat="server"></asp:TextBox> 
                                <asp:RequiredFieldValidator ID="ShipToCityValidator" runat="server" ControlToValidate="ShipToCity" ErrorMessage="City is required." Text="*"></asp:RequiredFieldValidator>
                            </td>
                            <th class="rowHeader">
                                <asp:Label ID="ShipToProvinceLabel" runat="server" Text="State / Province:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToProvince" runat="server"></asp:TextBox> 
                            </td>
                            
                        </tr>
                        <tr>
                        <th class="rowHeader">
                                <asp:Label ID="ShipToPostalCodeLabel" runat="server" Text="ZIP / Postal Code:"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ShipToPostalCode" runat="server"></asp:TextBox> 
                            </td>
                            <th class="rowHeader">
                                <asp:Label ID="ShipToCountryCodeLabel" runat="server" Text="Country:"></asp:Label>
                            </th>
                            <td>
                                <asp:DropDownList ID="ShipToCountryCode" runat="server" DataTextField="Name" DataValueField="CountryCode"></asp:DropDownList>
                            </td>
                            
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:Repeater>
            <br />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            <asp:Button ID="SaveButton" runat="server" text="Save Changes" OnClick="SaveButton_Click" />
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

