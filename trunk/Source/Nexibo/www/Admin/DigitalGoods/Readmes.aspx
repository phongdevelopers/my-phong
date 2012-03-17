<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Readmes.aspx.cs" Inherits="Admin_DigitalGoods_Readmes" Title="Manage Readmes" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Manage Readmes"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top" class="itemList">
                <ajax:UpdatePanel ID="GridAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cb:SortedGridView ID="ReadmeGrid" runat="server" AllowPaging="true" AllowSorting="true" PageSize="20"
                            AutoGenerateColumns="False" DataKeyNames="ReadmeId" DataSourceID="ReadmeDs" 
                            ShowFooter="False" DefaultSortExpression="DisplayName" SkinID="PagedList" Width="100%">
                            <Columns>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick='<%# Eval("DisplayName", "return confirm(\"Are you sure you want to delete {0}?\");") %>' Visible='<%# !HasProducts(Container.DataItem) %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:LinkButton>
                                        <asp:HyperLink ID="DeleteLink" runat="server" CausesValidation="False" NavigateUrl='<%# Eval("ReadmeId", "DeleteReadme.aspx?ReadmeId={0}")%>' Visible='<%# HasProducts(Container.DataItem) %>'><asp:Image ID="DeleteIcon2" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name" SortExpression="DisplayName">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="NameLink" runat="server" NavigateUrl='<%#Eval("ReadmeId", "EditReadme.aspx?ReadmeId={0}")%>' Text='<%# Eval("DisplayName") %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Products">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="ProductsLabel" runat="server" Text='<%#GetProductCount(Container.DataItem)%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyDataText" runat="server" Text="No readmes are defined for your store."></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
            <td valign="top">
                <div class="section">
                    <div class="header">
                        <h2 class="addreadme"><asp:Localize ID="AddCaption" runat="server" Text="Add Readme" /></h2>
                    </div>
                    <div class="content">
                        <ajax:UpdatePanel ID="AddAjax" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                <asp:Label ID="AddedMessage" runat="server" Text="{0} added.<br />" SkinID="GoodCondition" Visible="false"></asp:Label>
                                <table class="inputForm">
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="AddReadmeNameLabel" runat="server" Text="Name:" AssociatedControlID="AddReadmeName" ToolTip="Name of readme"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="AddReadmeName" runat="server" MaxLength="100"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="AddReadmeNameRequired" runat="server" ControlToValidate="AddReadmeName"
                                                Display="Static" ErrorMessage="Readme name is required." Text="*"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            <asp:Button ID="AddReadmeButton" runat="server" Text="Add" OnClick="AddReadmeButton_Click" />
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
    <asp:ObjectDataSource ID="ReadmeDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.DigitalDelivery.ReadmeDataSource" 
        SelectCountMethod="CountForStore" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.DigitalDelivery.Readme" 
        DeleteMethod="Delete" UpdateMethod="Update">
    </asp:ObjectDataSource>
</asp:Content>
