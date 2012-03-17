<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Marketing_Email_Default" Title="Manage Email Lists" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="AddEmailListDialog.ascx" TagName="AddEmailListDialog" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="MainContentAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
            	<div class="caption">
            		<h1><asp:Localize ID="Caption" runat="server" Text="Email Lists"></asp:Localize></h1>
            	</div>
            </div>
            <table cellpadding="2" cellspacing="2" class="innerLayout">
                <tr>
                    <td align="left" valign="top" class="itemList">
                        <cb:SortedGridView ID="EmailListGrid" runat="server" AllowPaging="true" AllowSorting="true" PageSize="20"
                            AutoGenerateColumns="False" DataKeyNames="EmailListId" DataSourceID="EmailListDs" 
                            ShowFooter="False" DefaultSortExpression="Name" SkinID="PagedList" Width="100%" OnRowCommand="EmailListGrid_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="List" SortExpression="Name">
                                    <ItemTemplate>
                                        <asp:HyperLInk ID="NameLink" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%#Eval("EmailListId", "ManageList.aspx?EmailListId={0}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Members">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLInk ID="UsersLabel" runat="server" Text='<%#GetUserCount(Container.DataItem)%>' NavigateUrl='<%#Eval("EmailListId", "ManageUsers.aspx?EmailListId={0}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Last Sent">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="LastSendDate" runat="server" text='<%#Eval("LastSendDate", "{0:d}")%>' Visible='<%#((DateTime)Eval("LastSendDate") != System.DateTime.MinValue)%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemStyle HorizontalAlign="Center" Width="81px" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("EmailListId", "ManageList.aspx?EmailListId={0}")%>' ToolTip="Edit" ><asp:Image ID="EditIcon" SkinID="Editicon" runat="server" /></asp:HyperLink>
                                        <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" ToolTip="Delete" CommandName="Delete" CommandArgument='<%#Eval("EmailListId")%>' OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" /></asp:LinkButton>
                                        <asp:HyperLink ID="EmailLink" runat="server" NavigateUrl='<%#String.Format("SendMail.aspx?EmailListId={0}&ReturnUrl={1}", Eval("EmailListId"), Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Admin/Marketing/Email/Default.aspx"))) %>' ToolTip="Send Email" Visible='<%#GetUserCount(Container.DataItem) > 0 %>'><asp:Image ID="EmailIcon" SkinID="EmailIcon" runat="server" /></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyDataText" runat="server" Text="No email lists are defined for your store."></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </td>
                    <td width="50%" valign="top">
                        <div class="section">
                            <div class="header">
                                <h2 class="addemaillist"><asp:Localize ID="AddCaption" runat="server" Text="Add Email List" /></h2>
                            </div>
                            <div class="content">
                                <uc:AddEmailListDialog id="AddEmailListDialog1" runat="server"></uc:AddEmailListDialog>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="EmailListDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Marketing.EmailListDataSource" 
        SelectCountMethod="CountForStore" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Marketing.EmailList" 
        DeleteMethod="Delete" UpdateMethod="Update">
    </asp:ObjectDataSource>
</asp:Content>