<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Marketing_Discounts_Default" Title="Volume Discounts" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Manage Volume Discounts"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="2" class="innerLayout">
        <tr>
            <td style="padding:10px 0 0 0;">
                <asp:Label ID="InstructionText" runat="server" Text="Manage the volume discounts available for your store.  Volume discounts are applied to purchases automatically when all criteria are met.  You can discount items by quantity or by value.  Discounts can be applied to categories or individual products."></asp:Label><br /><br />
            </td>
        </tr>
        <tr>
            <td class="itemList">
                <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <cb:SortedGridView ID="VolumeDiscountGrid" runat="server" AutoGenerateColumns="False" DataSourceID="VolumeDiscountDs" 
                            DataKeyNames="VolumeDiscountId" AllowPaging="True" OnRowCommand="VolumeDiscountGrid_RowCommand" SkinID="PagedList" 
                            Width="100%" AllowSorting="true" DefaultSortExpression="Name">
                            <Columns>
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="NameLink" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# Eval("VolumeDiscountId", "EditDiscount.aspx?VolumeDiscountId={0}") %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Basis">
                                    <ItemStyle HorizontalAlign="center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Basis" runat="server" Text='<%# (bool)Eval("IsValueBased") ? "Value" : "Quantity" %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Scope">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Panel ID="LinksPanel" runat="server" Visible='<%#!((bool)Eval("IsGlobal"))%>'>
                                            <asp:Label ID="Categories" runat="server" Text='<%#Eval("CategoryVolumeDiscounts.Count")%>'></asp:Label>
                                            <asp:Label ID="CategoriesLabel" runat="server" text=" categories"></asp:Label>, 
                                            <asp:Label ID="Products" runat="server" Text='<%#Eval("ProductVolumeDiscounts.Count")%>' />
                                            <asp:Label ID="ProductsLabel" runat="server" text=" products"></asp:Label>
                                        </asp:Panel>
                                        <asp:Label ID="IsGlobalLabel" runat="server" Text="Global" Visible='<%#Eval("IsGlobal")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" Width="81px" Wrap="false" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%# Eval("VolumeDiscountId", "EditDiscount.aspx?VolumeDiscountId={0}") %>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit"></asp:Image></asp:HyperLink>
                                        <asp:ImageButton ID="CopyButton" runat="server" AlternateText="Copy" ToolTip="Copy" SkinID="CopyIcon" CommandName="Copy" CommandArgument='<%# Eval("VolumeDiscountId")%>' />
                                        <asp:ImageButton ID="DeleteButton" runat="server" AlternateText="Delete" SkinID="DeleteIcon" CommandName="Delete" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="NoVolumeDiscountsText" runat="server" Text="<i>There are no volume discounts defined.</i>"></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="padding:2px 0 10px 0;">
                <asp:Button ID="AddButton" runat="server" Text="Add Discount" OnClick="AddButton_Click"></asp:Button>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="VolumeDiscountDs" runat="server" OldValuesParameterFormatString="original_{0}" 
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Marketing.VolumeDiscountDataSource" 
        DataObjectTypeName="CommerceBuilder.Marketing.VolumeDiscount" DeleteMethod="Delete" SortParameterName="sortExpression">
    </asp:ObjectDataSource>

</asp:Content>




