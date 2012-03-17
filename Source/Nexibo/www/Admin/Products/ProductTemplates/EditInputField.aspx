<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditInputField.aspx.cs" Inherits="Admin_Products_ProductTemplates_EditInputField" Title="Edit Field"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="{0}: Edit {1} Field '{2}'"></asp:Localize></h1>
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
                                <th class="rowHeader" valign="top">
                                    <asp:Label ID="ChoicesLabel" runat="server" Text="Choices:" SkinID="FieldHeader"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:GridView ID="ChoicesGrid" runat="server" AutoGenerateColumns="false" 
                                        OnRowDeleting="ChoicesGrid_RowDeleting" SkinID="PagedList" EnableViewState="true">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Text">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="ChoiceText" runat="server" Text='<%#Eval("ChoiceText")%>' MaxLength="100"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="ChoiceTextValidator" runat="server" ErrorMessage="Maximum length for Choice Text is 100 characters." Text="*" ControlToValidate="ChoiceText" ValidationExpression=".{0,100}"  ></asp:RegularExpressionValidator>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Value (optional)">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="ChoiceValue" runat="server" Text='<%#Eval("ChoiceValue")%>' MaxLength="100"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="ChoiceValueValidator" runat="server" ErrorMessage="Maximum length for Choice Value is 100 characters." Text="*" ControlToValidate="ChoiceValue" ValidationExpression=".{0,100}"  ></asp:RegularExpressionValidator>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Selected">
                                                <ItemStyle HorizontalAlign="center" />
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="IsSelected" runat="server" Checked='<%#Eval("IsSelected")%>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="DeleteButton" runat="server" SkinID="DeleteIcon" CommandName="Delete" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:LinkButton id="AddChoiceButton" runat="server" Text="Add Choice" OnClick="AddChoiceButton_Click" SkinID="Link" CausesValidation="true"></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td class="submit" colspan="2">
                                    <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" CausesValidation="false" />
                                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>

