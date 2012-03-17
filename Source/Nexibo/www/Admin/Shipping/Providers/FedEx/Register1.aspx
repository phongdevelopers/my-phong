<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register1.aspx.cs"
    Inherits="Admin_Shipping_Providers_FedEx_Register1" Title="FedEx&reg; Registration" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" class="pageHeader">
                <h1>
                    <asp:Label ID="Caption" runat="server" Text="FedEx&reg; Registration"></asp:Label>
                </h1>
            </td>
        </tr>
        <tr>
            <td>
                <table class="inputForm">
                    <tr>
                        <td colspan="2">
                            <img src="LOGO_S.gif" align="Left" hspace="20" vspace="20" />
                            <p align="justify" style="margin-bottom: 10px;">
                                <asp:Label ID="InstructionText" runat="server" Text="In order to enable FedEx&reg;, you must register your FedEx&reg; account and obtain a meter number.	<br>If you have already registered your account for online use and have your meter number available, ">
                                    <asp:HyperLink ID="Register2Link" runat="server" Text="Click Here" NavigateUrl="Register2.aspx"></asp:HyperLink>
                                </asp:Label>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <p style="margin-bottom: 10px;">
                                <asp:Label ID="Label1" runat="server" Text="Complete the form below to register.  &quot;R&quot; indicates a required field."></asp:Label>
                            </p>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                            <asp:Label ID="CustomErrorMessage" runat="server" Text="" SkinID="ErrorCondition"></asp:Label>
                        </td>
                    </tr>                    
                    <tr id="trInstanceName" runat="server" visible="false">
			            <th class="rowHeader">
			                 <asp:Label ID="InstanceNameLabel" runat="server" Text="Gateway Name:"></asp:Label>
			            </th>
                        <td>
                            <asp:TextBox ID="InstanceName" runat="server" Text="FedEx"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter a name for this instance of FedEx."
                                ControlToValidate="InstanceName" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="FedExAccountNameLbl" runat="server" Text="FedEx&reg; Account Number:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="FedExAccountName" runat="server"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="FedExAccountNameValidator" runat="server" ErrorMessage="FedEx Account Number is required."
                                ControlToValidate="FedExAccountName" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="ContactNameLabel" runat="server" Text="Contact Name:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="ContactName" runat="server"></asp:TextBox>
                            &nbsp;R
                            <asp:RequiredFieldValidator ID="ContactNameValidator" runat="server" ErrorMessage="Contact name is required."
                                ControlToValidate="ContactName" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="CompanyNameLabel" runat="server" Text="Company Name:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="CompanyName" runat="server"></asp:TextBox>
                            &nbsp;R
                            <asp:RequiredFieldValidator ID="CompanyNameValidator" runat="server" ErrorMessage="Company name is required."
                                ControlToValidate="CompanyName" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="Address1Label" runat="server" Text="Street Address 1:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Address1" runat="server"></asp:TextBox>
                            &nbsp;R
                            <asp:RequiredFieldValidator ID="Address1Validator" runat="server" ErrorMessage="Street address 1 is required."
                                ControlToValidate="Address1" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Address2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="CityLabel" runat="server" Text="City:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="City" runat="server"></asp:TextBox>
                            &nbsp;R
                            <asp:RequiredFieldValidator ID="CityValidator" runat="server" ErrorMessage="City is required."
                                ControlToValidate="City" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="ProvinceLabel" runat="server" Text="State / Province:"></asp:Label>
                        </th>
                        <td id="ProvinceCell" runat="server">
                            <asp:TextBox ID="Province" runat="server" Columns="2" MaxLength="2"></asp:TextBox>
                            &nbsp;(US &amp; CA only)
                            <asp:RegularExpressionValidator ID="ProvinceValidator" runat="server" ErrorMessage="You must enter a valid 2 letter state or province abbreviation."
                                ControlToValidate="Province" Text="*" ValidationExpression="^(AL|AK|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|MP|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY|AE|AA|AE|AE|AP|AB|BC|MB|NB|NL|NT|NS|NU|ON|PE|QC|SK|YT)$"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="PostalCodeLabel" runat="server" Text="Zip / Postal Code:"></asp:Label>
                        </th>
                        <td id="PostalCodeCell" runat="server">
                            <asp:TextBox ID="PostalCode" runat="server" Columns="6" MaxLength="7"></asp:TextBox>
                            &nbsp;(US &amp; CA only)
                            <asp:RegularExpressionValidator ID="PostalCodeValidator" runat="server" ErrorMessage="You must enter a valid 5 digit US ZIP or 6 character CA Postal Code."
                                ControlToValidate="PostalCode" Text="*" ValidationExpression="^(\d{5}(( |-)\d{4})?)|([A-Za-z]\d[A-Za-z]( |-)*\d[A-Za-z]\d)$"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="CountryLabel" runat="server" Text="Country:"></asp:Label>
                        </th>
                        <td>
                            <asp:DropDownList ID="Country" runat="server">
                                <asp:ListItem Value="AR">Argentina</asp:ListItem>
                                <asp:ListItem Value="AU">Australia</asp:ListItem>
                                <asp:ListItem Value="AT">Austria</asp:ListItem>
                                <asp:ListItem Value="BS">Bahamas</asp:ListItem>
                                <asp:ListItem Value="BE">Belgium</asp:ListItem>
                                <asp:ListItem Value="BR">Brazil</asp:ListItem>
                                <asp:ListItem Value="CA">Canada</asp:ListItem>
                                <asp:ListItem Value="CL">Chile</asp:ListItem>
                                <asp:ListItem Value="CR">Costa Rica</asp:ListItem>
                                <asp:ListItem Value="DK">Denmark</asp:ListItem>
                                <asp:ListItem Value="DO">Dominican Republic</asp:ListItem>
                                <asp:ListItem Value="FI">Finland</asp:ListItem>
                                <asp:ListItem Value="FR">France</asp:ListItem>
                                <asp:ListItem Value="DE">Germany</asp:ListItem>
                                <asp:ListItem Value="GR">Greece</asp:ListItem>
                                <asp:ListItem Value="GT">Guatemala</asp:ListItem>
                                <asp:ListItem Value="HK">Hong Kong</asp:ListItem>
                                <asp:ListItem Value="IL">Israel</asp:ListItem>
                                <asp:ListItem Value="IT">Italy</asp:ListItem>
                                <asp:ListItem Value="MY">Malaysia</asp:ListItem>
                                <asp:ListItem Value="MX">Mexico</asp:ListItem>
                                <asp:ListItem Value="NL">Netherlands</asp:ListItem>
                                <asp:ListItem Value="NZ">New Zealand</asp:ListItem>
                                <asp:ListItem Value="NO">Norway</asp:ListItem>
                                <asp:ListItem Value="PA">Panama</asp:ListItem>
                                <asp:ListItem Value="PT">Portugal</asp:ListItem>
                                <asp:ListItem Value="PR">Puerto Rico</asp:ListItem>
                                <asp:ListItem Value="IE">Republic of Ireland</asp:ListItem>
                                <asp:ListItem Value="SG">Singapore</asp:ListItem>
                                <asp:ListItem Value="ES">Spain</asp:ListItem>
                                <asp:ListItem Value="SE">Sweden</asp:ListItem>
                                <asp:ListItem Value="CH">Switzerland</asp:ListItem>
                                <asp:ListItem Value="TW">Taiwan</asp:ListItem>
                                <asp:ListItem Value="TH">Thailand</asp:ListItem>
                                <asp:ListItem Value="GB">United Kingdom</asp:ListItem>
                                <asp:ListItem Value="US">United States</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;R
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="PhoneLabel" runat="server" Text="Phone:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Phone" runat="server"></asp:TextBox>
                            &nbsp;R
                            <asp:RequiredFieldValidator ID="PhoneValidator" runat="server" ErrorMessage="Phone is required."
                                ControlToValidate="Phone" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="PhoneValidator2" runat="server" ErrorMessage="You must enter a valid 10-16 digit phone number."
                                ControlToValidate="Phone" Text="*" ></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="EmailLabel" runat="server" Text="Email:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                            &nbsp;R
                            <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="Email"  Required="true" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">                            
                            <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
							<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click"
                                CausesValidation="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <asp:Label ID="FedExCopyRight" runat="server" Text="FedEx and FedEx brandmark are trademarks of FedEx Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>
        </td>
        </tr>
        
    </table>
</asp:Content>
