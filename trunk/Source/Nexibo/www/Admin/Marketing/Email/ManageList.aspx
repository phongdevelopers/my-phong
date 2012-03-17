<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ManageList.aspx.cs" Inherits="Admin_Marketing_Email_ManageList" Title="Manage List" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="EditEmailListDialog.ascx" TagName="EditEmailListDialog" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="MainContentAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
            	<div class="caption">
            		<h1><asp:Localize ID="Caption" runat="server" Text="Manage List '{0}'"></asp:Localize></h1>
            	</div>
            </div>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td width="450" valign="top">
                        <div class="statusMessage">
                            <asp:Label ID="UpdatedMessage" runat="server" Text="List updated at {0:T}" SkinID="GoodCondition" EnableViewState="false" Visible="false"></asp:Label>
                        </div>
                        <div class="section">
                            <div class="header">
                                <h2 class="maillist"><asp:Localize ID="AddCaption" runat="server" Text="Edit List Details" /></h2>
                            </div>
                            <div class="content">
                                <uc:EditEmailListDialog id="EditEmailListDialog1" runat="server"></uc:EditEmailListDialog>
                            </div>
                        </div>
                    </td>
                    <td align="left" valign="top">
                        <table class="summary">
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="MembersLabel" runat="server" Text="Members:"></asp:Label>
                                </th>
                                <td>
                                    <asp:Label ID="Members" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="LastSendDateLabel" runat="server" Text="Last Send:"></asp:Label>
                                </th>
                                <td>
                                    <asp:Label ID="LastSendDate" runat="server" Text="{0:f}" Visible=></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="buttons" colspan="2">
                                    <asp:HyperLink ID="ManageMembersLink" runat="server" Text="Manage Users" NavigateUrl="ManageUsers.aspx" SkinId="Button"></asp:HyperLink>
                                    <asp:HyperLink ID="ExportListLink" runat="server" Text="Export List" NavigateUrl="ExportList.ashx" SkinId="Button"></asp:HyperLink>
                                    <asp:HyperLink ID="SendMessageLink" runat="server" Text="Send Message" NavigateUrl="SendMail.aspx" SkinID="Button" />
                                </td>
                            </tr>
                        </table>
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