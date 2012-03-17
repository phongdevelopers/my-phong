<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditProductTemplate.aspx.cs" Inherits="Admin_Products_ProductTemplates_EditProductTemplate" Title="Edit Product Template"  %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Edit Product Template: {0}"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <table cellpadding="4" cellspacing="0" class="inputForm">
                    <tr>
                        <th class="rowHeader"> 
                            <asp:Label ID="NameLabel" runat="server" Text="Template Name:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Dynamic" ErrorMessage="Name is required." ControlToValidate="Name"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="AddNameValidator" runat="server" ErrorMessage="Maximum length for Name is 100 characters." Text="*" ControlToValidate="Name" ValidationExpression=".{1,100}"  ></asp:RegularExpressionValidator>
                        </td>
                        <th class="rowHeader"> 
                            <asp:Label ID="AssignedProductsLabel" runat="server" Text="Assigned Products:"></asp:Label>
                        </th>
                        <td>
                            <asp:HyperLink ID="AssignedProducts" runat="server" Text="" NavigateUrl="ViewProducts.aspx"></asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td colspan"4" class="submit">
                            <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" CausesValidation="false" />
                            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <th class="sectionHeader">
                Merchant Fields
            </th>
        </tr>
        <tr>
            <td>
                <p align="justify">
                    Specify additional data to be collected on the merchant add / edit product forms.
                </p>
                <ajax:UpdatePanel ID="MerchantFieldAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="MerchantFieldGrid" runat="server" AutoGenerateColumns="False" 
                            DataKeyNames="InputFieldId" DataSourceID="MerchantFieldDs" 
                            OnRowCommand="FieldGrid_RowCommand" SkinID="PagedList" Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Order">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="UpButton" runat="server" SkinID="UpIcon" CommandName="MoveUp" CommandArgument='<%#Container.DataItemIndex%>' />
                                        <asp:ImageButton ID="DownButton" runat="server" SkinID="DownIcon" CommandName="MoveDown" CommandArgument='<%#Container.DataItemIndex%>' />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name">
                                    <ItemTemplate>
                                        <asp:Localize ID="InputName" runat="server" Text='<%# Eval("Name") %>'></asp:Localize>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Type">
                                    <ItemTemplate>
                                        <asp:Localize ID="InputType" runat="server" Text='<%# GetInputType(Container.DataItem) %>'></asp:Localize>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Choices">
                                    <ItemStyle HorizontalAlign="center" />
                                    <ItemTemplate>
                                        <asp:Localize ID="Choices" runat="server" Text='<%# GetChoices(Container.DataItem)%>'></asp:Localize>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <div align="center">
                                            <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("InputFieldId", "EditInputField.aspx?InputFieldId={0}")%>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" /></asp:HyperLink>
                                            <asp:ImageButton ID="CopyButton" runat="server" SkinID="CopyIcon" CommandName="Copy" CommandArgument='<%#Eval("InputFieldId")%>' />
                                            <asp:ImageButton ID="DeleteButton" runat="server" SkinID="DeleteIcon" CommandName="Delete" OnClientClick='<%#Eval("Name", "javascript:return confirm(\"Are you sure you wish to delete {0}?\")")%>' />
                                        </div>
                                    </ItemTemplate>
                                    <FooterStyle VerticalAlign="Top" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
                <asp:Button ID="AddMerchantFieldButton" runat="server" Text="Add Field" OnClick="AddMerchantFieldButton_Click" />
                <asp:ObjectDataSource ID="MerchantFieldDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForProductTemplate" TypeName="CommerceBuilder.Products.InputFieldDataSource"
                 SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Products.InputField" DeleteMethod="Delete">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="productTemplateId" QueryStringField="ProductTemplateId" Type="Object" />
                        <asp:Parameter DefaultValue="Merchant" Name="scope" Type="Object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <th class="sectionHeader">
                Customer Fields
            </th>
        </tr>
        <tr>
            <td>
                <p align="justify">
                    Specify additional data to be collected from the customer when a product is purchased.
                </p>
                <ajax:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="CustomerFieldGrid" runat="server" AutoGenerateColumns="False" 
                            DataKeyNames="InputFieldId" DataSourceID="CustomerFieldDs" 
                            OnRowCommand="FieldGrid_RowCommand" SkinID="PagedList" Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Order">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="UpButton" runat="server" SkinID="UpIcon" CommandName="MoveUp" CommandArgument='<%#Container.DataItemIndex%>' />
                                        <asp:ImageButton ID="DownButton" runat="server" SkinID="DownIcon" CommandName="MoveDown" CommandArgument='<%#Container.DataItemIndex%>' />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name">
                                    <ItemTemplate>
                                        <asp:Localize ID="InputName" runat="server" Text='<%# Eval("Name") %>'></asp:Localize>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Type">
                                    <ItemTemplate>
                                        <asp:Localize ID="InputType" runat="server" Text='<%# GetInputType(Container.DataItem) %>'></asp:Localize>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Choices">
                                    <ItemStyle HorizontalAlign="center" />
                                    <ItemTemplate>
                                        <asp:Localize ID="Choices" runat="server" Text='<%# GetChoices(Container.DataItem)%>'></asp:Localize>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <div align="center">
                                            <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("InputFieldId", "EditInputField.aspx?InputFieldId={0}")%>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" /></asp:HyperLink>
                                            <asp:ImageButton ID="CopyButton" runat="server" SkinID="CopyIcon" CommandName="Copy" CommandArgument='<%#Eval("InputFieldId")%>' />
                                            <asp:ImageButton ID="DeleteButton" runat="server" SkinID="DeleteIcon" CommandName="Delete" OnClientClick='<%#Eval("Name", "javascript:return confirm(\"Are you sure you wish to delete {0}?\")")%>' />
                                        </div>
                                    </ItemTemplate>
                                    <FooterStyle VerticalAlign="Top" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
                <asp:Button ID="AddCustomerFieldButton" runat="server" Text="Add Field" OnClick="AddCustomerFieldButton_Click" />
                <asp:ObjectDataSource ID="CustomerFieldDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForProductTemplate" TypeName="CommerceBuilder.Products.InputFieldDataSource"
                 SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Products.InputField" DeleteMethod="Delete">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="productTemplateId" QueryStringField="ProductTemplateId"
                            Type="Object" />
                        <asp:Parameter DefaultValue="Customer" Name="scope" Type="Object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

