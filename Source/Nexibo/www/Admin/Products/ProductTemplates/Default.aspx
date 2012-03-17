<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Products_ProductTemplates_Default" Title="Product Templates"  %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Product Templates"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" style="padding:10px 0px 10px 0px; text-indent:20px;">
                <asp:Label ID="InstructionText" runat="server" Text="Use templates to define additional custom fields for your products."></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="itemList">
                <ajax:UpdatePanel ID="ProductTemplateAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="ProductTemplateGrid" runat="server" AutoGenerateColumns="False" DataSourceID="ProductTemplateDs" 
                            DataKeyNames="ProductTemplateId" AllowPaging="True" OnRowCommand="ProductTemplateGrid_RowCommand"
                            SkinID="PagedList" width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Template Name">
                                    <ItemTemplate>
                                        <asp:Label ID="Name" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Merchant Fields">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="MerchantFields" runat="server" Text='<%# CountMerchantFields(Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Customer Fields">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="CustomerFields" runat="server" Text='<%# CountCustomerFields(Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Products">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="Products" runat="server" Text='<%# CountProducts(Container.DataItem) %>' NavigateUrl='<%#Eval("ProductTemplateId", "ViewProducts.aspx?ProductTemplateId={0}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Wrap="false">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("ProductTemplateId", "EditProductTemplate.aspx?ProductTemplateId={0}")%>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" /></asp:HyperLink>
                                        <asp:ImageButton ID="CopyButton" runat="server" AlternateText="Copy" SkinID="CopyIcon" CommandName="Copy" CommandArgument='<%#Eval("ProductTemplateId")%>' />
                                        <asp:ImageButton ID="DeleteButton" runat="server" AlternateText="Delete" SkinID="DeleteIcon" CommandName="Delete" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div style="text-align:center;margin-top:10px;margin-bottom:10px;padding-left:10px;padding-right:10px">
                                <asp:Label ID="NoProductTemplatesText" runat="server" Text="<i>There are no product templates defined.</i>"></asp:Label>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
            <td class="detailPanel">
                <div class="section">
                    <div class="header">
                        <h2 class="addtemplate"><asp:Localize ID="AddCaption" runat="server" Text="Add Template" /></h2>
                    </div>
                    <div class="content">
                        <ajax:UpdatePanel ID="AddAjax" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" />
                                <asp:Label ID="AddNameLabel" runat="server" Text="Name:" SkinID="FieldHeader"></asp:Label>
                                <asp:TextBox ID="AddName" runat="server" MaxLength="100"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="AddNameValidator" runat="server" ErrorMessage="Maximum length for Name is 100 characters." Text="*" ControlToValidate="AddName" ValidationExpression=".{1,100}"  ValidationGroup="Add"></asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="AddNameRequired" runat="server" ControlToValidate="AddName" ValidationGroup="Add" Text="*" ErrorMessage="Name is required."></asp:RequiredFieldValidator>
                                <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ValidationGroup="Add" />
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="ProductTemplateDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForStore" TypeName="CommerceBuilder.Products.ProductTemplateDataSource" DataObjectTypeName="CommerceBuilder.Products.ProductTemplate" DeleteMethod="Delete"></asp:ObjectDataSource>
</asp:Content>

