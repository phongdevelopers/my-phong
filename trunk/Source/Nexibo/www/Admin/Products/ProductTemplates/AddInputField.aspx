<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="AddInputField.aspx.cs" Inherits="Admin_Products_ProductTemplates_AddInputField" Title="Add Field"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Add {0} Field to '{1}'"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <ajax:UpdatePanel ID="InputFieldAjax" runat="server">
                    <ContentTemplate>
                        <table class="inputForm">
                            <tr>
                                <td colspan="2">
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="InputTypeIdLabel" runat="server" Text="Input Type:" SkinID="FieldHeader"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:DropDownList ID="InputTypeId" runat="server" AutoPostBack="true" OnSelectedIndexChanged="InputTypeId_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="NameLabel" runat="server" Text="Name:" SkinID="FieldHeader"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:TextBox ID="Name" runat="server" Text="" Columns="20" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Dynamic" ErrorMessage="Name is required." ControlToValidate="Name"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="AddNameValidator" runat="server" ErrorMessage="Maximum length for Name is 100 characters." Text="*" ControlToValidate="Name" ValidationExpression=".{1,100}"  ></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="UserPromptLabel" runat="server" Text="Prompt:" SkinID="FieldHeader"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:TextBox ID="UserPrompt" runat="server" Text="" Columns="50" MaxLength="255"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserPromptRequired" runat="server" Text="*" Display="Dynamic" ErrorMessage="UserPrompt is required." ControlToValidate="UserPrompt"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="UserPromptValidator" runat="server" ErrorMessage="Maximum length for user prompt is 100 characters." Text="*" ControlToValidate="UserPrompt" ValidationExpression=".{1,255}"  ></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr id="trRows" runat="server">
                                <th class="rowHeader">
                                    <asp:Label ID="RowsLabel" runat="server" Text="Rows:" SkinID="FieldHeader"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:TextBox ID="Rows" runat="server" Text="" Columns="4" MaxLength="3"></asp:TextBox>
                                    <asp:RangeValidator ID="RowsValidator1" runat="server" Text="*" Type="Integer" ErrorMessage="Rows value should fall between 0 and 255." ControlToValidate="Rows" MinimumValue="1" MaximumValue="255"/>
                                </td>
                            </tr>
                            <tr id="trColumns" runat="server">
                                <th class="rowHeader">
                                    <asp:Label ID="ColumnsLabel" runat="server" Text="Columns:" SkinID="FieldHeader"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:TextBox ID="Columns" runat="server" Text="" Columns="4" MaxLength="3"></asp:TextBox>
                                    <asp:RangeValidator ID="ColumnsValidator" runat="server" Text="*" Type="Integer" ErrorMessage="Columns value should fall between 0 and 255." ControlToValidate="Columns" MinimumValue="1" MaximumValue="255"/>
                                </td>
                            </tr>
                            <tr id="trMaxLength" runat="server">
                                <th class="rowHeader">
                                    <asp:Label ID="MaxLengthLabel" runat="server" Text="Max Length:" SkinID="FieldHeader"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:TextBox ID="MaxLength" runat="server" Text="" Columns="4" MaxLength="4"></asp:TextBox>
                                    <asp:RangeValidator ID="MaxLengthValidator1" runat="server" Text="*" Type="Integer" ErrorMessage="Max Length value should fall between 0 and 1000." ControlToValidate="MaxLength" MinimumValue="1" MaximumValue="1000"/>
                                </td>
                            </tr>
                            <tr id="trChoices" runat="server" visible="false">
                                <th class="rowHeader">
                                    <asp:Label ID="ChoicesLabel" runat="server" Text="Choices:" SkinID="FieldHeader"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:Label ID="ChoicesHelpText" runat="server" Text="After you click Next, you will be able to configure the user input choices." SkinID="HelpText"></asp:Label><br />
                                </td>
                            </tr>
                            <tr>
                                <td class="submit" colspan="2">                                    
                                    <asp:Button ID="SaveButton" runat="server" Text="Finish" OnClick="SaveButton_Click" />
									<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                                    <asp:Button ID="NextButton" runat="server" Text="Next" OnClick="NextButton_Click" Visible="false" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>

