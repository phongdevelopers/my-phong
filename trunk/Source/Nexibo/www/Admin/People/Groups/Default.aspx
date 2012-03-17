<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_People_Groups_Default" Title="User Groups"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="AddGroupDialog.ascx" TagName="AddGroupDialog" TagPrefix="uc1" %>
<%@ Register Src="EditGroupDialog.ascx" TagName="EditGroupDialog" TagPrefix="uc1" %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Manage Groups"></asp:Localize></h1>
    	</div>
    </div>
     <ajax:UpdatePanel ID="GroupAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <table cellpadding="2" cellspacing="0" class="innerLayout">
            <tr>
                <td colspan="2">
                    <asp:Label ID="InstructionText" runat="server" Text="Use groups to provide access to certain features or set user specific price points."></asp:Label>            
                </td>                
            </tr>
            <tr>    
                <td valign="top" class="itemList">     
                    <cb:SortedGridView ID="GroupGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="GroupId" DataSourceID="GroupDs" 
                        width="100%" SkinID="Summary" AllowSorting="True" DefaultSortExpression="Name" DefaultSortDirection="Ascending" 
                        ShowWhenEmpty="False" OnRowCancelingEdit="GroupGrid_RowCancelingEdit" OnRowEditing="GroupGrid_RowEditing">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="Group" SortExpression="Name" ReadOnly="true">
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Permissions">
                                <ItemStyle HorizontalAlign="Left" />
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <asp:Label ID="Roles" runat="server" Text='<%# GetRoles(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>                                
                            <asp:CheckBoxField DataField="IsTaxExempt" HeaderText="Tax Exempt" SortExpression="IsTaxExempt" ReadOnly="true">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:CheckBoxField>
                            <asp:TemplateField HeaderText="Users">
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="UserCount" runat="server" Text='<%# CountUsers(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>                                
                            <asp:TemplateField>
                                <EditItemTemplate>
                                    <asp:ImageButton ID="SaveButton" runat="server" SkinID="SaveIcon" CommandName="Update" CausesValidation="True" />
                                    <asp:ImageButton ID="CancelButton" runat="server" SkinID="CancelIcon" CommandName="Cancel" CausesValidation="false" />
                                </EditItemTemplate>
                                <ItemStyle HorizontalAlign="Left" Width="81px" Wrap="false" />
                                <ItemTemplate>
                                    <asp:HyperLink ID="UsersLink" runat="server" NavigateUrl='<%# Eval("GroupId", "ManageUsers.aspx?GroupId={0}") %>' Visible='<%# (!((CommerceBuilder.Users.Group)Container.DataItem).IsInRole("System") || Token.Instance.User.IsSystemAdmin)%>'><asp:Image ID="UsersIcon" runat="server" SkinID="GroupIcon" AlternateText="Manage Users" /></asp:HyperLink>
                                    <asp:ImageButton ID="EditButton" runat="server" SkinID="EditIcon" CommandName="Edit" AlternateText="Edit" Visible='<%#IsEditableGroup(Container.DataItem)%>' />
                                    <asp:ImageButton ID="DeleteButton" runat="server" SkinID="DeleteIcon" CommandName="Delete" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' Visible='<%#ShowDeleteButton(Container.DataItem)%>' AlternateText="Delete" />
                                    <asp:HyperLink ID="DeleteLink" runat="server" NavigateUrl='<%# Eval("GroupId", "DeleteGroup.aspx?GroupId={0}")%>' Visible='<%# ShowDeleteLink(Container.DataItem) %>'><asp:Image ID="DeleteIcon2" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </cb:SortedGridView>                                                    
                    <asp:ObjectDataSource ID="GroupDs" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Users.GroupDataSource" DataObjectTypeName="CommerceBuilder.Users.Group"
                        DeleteMethod="Delete" UpdateMethod="Update" SortParameterName="sortExpression">
                    </asp:ObjectDataSource>
                </td>
                <td valign="top">
                    <ajax:UpdatePanel ID="AddEditAjax" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="section">
                                <asp:Panel ID="AddPanel" runat="server">
                                    <div class="header">
                                        <h2 class="addgroup"><asp:Localize ID="AddCaption" runat="server" Text="Add Group" /></h2>
                                    </div>
                                    <div class="content">
                                        <uc1:AddGroupDialog ID="AddGroupDialog1" runat="server"></uc1:AddGroupDialog>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="EditPanel" runat="server" Visible="false">
                                    <div class="header">
                                        <h2 class="addgroup"><asp:Localize ID="EditCaption" runat="server" Text="Edit {0}" /></h2>
                                    </div>
                                    <div class="content">
                                        <uc1:EditGroupDialog ID="EditGroupDialog1" runat="server"></uc1:EditGroupDialog>
                                    </div>
                                </asp:Panel>
                            </div>
                        </ContentTemplate>
                    </ajax:UpdatePanel>
                </td>
            </tr>
        </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

