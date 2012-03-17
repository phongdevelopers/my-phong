<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Store_GiftWrap_Default" Title="Gift Wrap" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Gift Wrap"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2">
                <asp:Label ID="InstructionText" runat="server" Text="Configure your gift wrap groups - sets of wrapping styles that can be applied to your products."></asp:Label><br /><br />
            </td>
        </tr>
        <tr>
            <td valign="top">
                <ajax:UpdatePanel ID="WrapGroupAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cb:SortedGridView ID="WrapGroupGrid" runat="server" AutoGenerateColumns="False" DataSourceID="WrapGroupDs" 
                            DataKeyNames="WrapGroupId" AllowPaging="False" AllowSorting="false" Width="100%" 
                            OnRowCommand="WrapGroupGrid_RowCommand" SkinID="PagedList" DefaultSortExpression="Name">
                            <Columns>
                                <asp:TemplateField HeaderText="Wrap Group" SortExpression="Name">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Name" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Wrap Styles">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="WrapStyles" runat="server" Text='<%# CountWrapStyles(Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Products">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="Products" runat="server" Text='<%# CountProducts(Container.DataItem) %>' NavigateUrl='<%#Eval("WrapGroupId", "ViewProducts.aspx?WrapGroupId={0}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("WrapGroupId", "EditWrapGroup.aspx?WrapGroupId={0}")%>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" ToolTip="Edit" /></asp:HyperLink>
                                        <asp:ImageButton ID="CopyButton" runat="server" AlternateText="Copy" SkinID="CopyIcon" CommandName="Copy" ToolTip="Copy" CommandArgument='<%#Container.DataItemIndex%>' />
                                        <asp:ImageButton ID="DeleteButton" runat="server" AlternateText="Delete" SkinID="DeleteIcon" ToolTip="Delete" CommandName="Delete" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'/>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="NoWrapGroupsText" runat="server" Text="There are no gift wrap groups defined."></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
            <td valign="top">
                <div class="section">
                    <div class="header">
                        <h2 class="addwrapgroup"><asp:Localize ID="AddCaption" runat="server" Text="Add Wrap Group" /></h2>
                    </div>
                    <div class="content">
                        <ajax:UpdatePanel ID="AddAjax" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" />
                                <table class="inputForm">
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="AddNameLabel" runat="server" Text="Name:" AssociatedControlID="AddName"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="AddName" runat="server" MaxLength="50"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="AddNameRequired" runat="server" ControlToValidate="AddName" ValidationGroup="Add" Text="*" ErrorMessage="Name is required."></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ValidationGroup="Add" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="WrapGroupDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForStore" 
        TypeName="CommerceBuilder.Products.WrapGroupDataSource" DataObjectTypeName="CommerceBuilder.Products.WrapGroup" 
        DeleteMethod="Delete" SortParameterName="sortExpression"></asp:ObjectDataSource>
</asp:Content>

