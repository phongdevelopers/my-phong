<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditVendor.aspx.cs" Inherits="Admin_People_Vendors_EditVendor" Title="Edit Vendor"  %>
<%@ Register Src="EditProducts.ascx" TagName="EditProducts" TagPrefix="uc1" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Edit Vendor"></asp:Localize></h1>
        </div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout" width="100%">
        <tr>
            <td valign="top" >
                <ajax:UpdatePanel ID="EditAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    <table  cellspacing="0" width="100%" class="inputForm">    
                        
                        <tr>
                            <td colspan="2" align="center">
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader" width="50%">
                                <br />
                                <asp:Label ID="NameLabel" runat="server" Text="Name:"></asp:Label><br />
                                <asp:Label ID="NameHelpText" runat="Server" class="helpText" Text="Enter the name of the vendor as it will appear in the merchant interface."></asp:Label>
                            </th>
                            <td valign="top" width="50%">
                                <br />
                                <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="NameValidator" runat="server" ErrorMessage="Maximum length for Name is 100 characters." Text="*" ControlToValidate="Name" ValidationExpression=".{0,100}"  ></asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                                        Display="Dynamic" ErrorMessage="Vendor name is required.">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader" width="50%">
                                <br />
                                <asp:Label ID="EmailLabel" runat="server" Text="Email Address:"></asp:Label><br />
                                <asp:Label ID="EmailHelpText" runat="Server" class="helpText" Text="Enter the email address of the vendor. You can also add comma delimitted multiple email addresses."></asp:Label>
                            </th>
                            <td valign="top" width="50%">
                                <br />
                                <asp:TextBox ID="Email" runat="server" MaxLength="255"></asp:TextBox>
                                <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" AllowMultpleAddresses="true" ControlToValidate="Email" Required="false" ErrorMessage="Email address should be in the format of name@domain.tld, you can also add comma delimited multiple email addresses." Text="*" EnableViewState="False"></cb:EmailAddressValidator>                
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <br />
				                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                                <asp:Button ID="CancelButton" runat="server" Text="Cancel"  OnClick="CancelButton_Click" CausesValidation="false" />                
                            </td>
                        </tr>
                    </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td valign="top" style="width:100%;">
                <uc1:EditProducts ID="EditProducts1" runat="server" />                
            </td>
        </tr>
    </table>
</asp:Content>

