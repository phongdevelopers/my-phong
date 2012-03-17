<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="DeleteGroup.aspx.cs" Inherits="Admin_People_Groups_DeleteGroup" Title="Delete Group"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Delete {0}" EnableViewState="false"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top" width="300">
                <ajax:UpdatePanel ID="EditAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="InstructionText" runat="server" Text="This group has one or more associated users.  Indicate what group these users should be changed to when {0} is deleted." EnableViewState="false"></asp:Label>
                        <table class="inputForm" width="100%">
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="NameLabel" runat="server" Text="Move to Group:" AssociatedControlID="GroupList" ToolTip="New group for associated users"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:DropDownList ID="GroupList" runat="server" DataTextField="Name" DataValueField="GroupId" AppendDataBoundItems="True">
                                        <asp:ListItem Value="" Text="-- none --"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="submit" colspan="2">                                    
                                    <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false"/>
									<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
            <td valign="top" width="300">
                <div class="section">
                    <div class="header">
                        <h2><asp:Localize ID="UsersCaption" runat="server" Text="Assigned Users"></asp:Localize></h2>
                    </div>
                    <div class="content">
                        <ajax:UpdatePanel ID="PagingAjax" runat="server" UpdateMode="conditional">
                            <ContentTemplate>
                                <cb:SortedGridView ID="UsersGrid" runat="server" DataSourceID="UsersDs" AllowPaging="True" 
                                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="UserId" PageSize="20" 
                                    SkinID="PagedList" DefaultSortExpression="UserName" Width="100%">
                                    <Columns>
                                        <asp:TemplateField HeaderText="User" SortExpression="UserName">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="UserName" runat="server" Text='<%# Eval("UserName") %>' NavigateUrl='<%#Eval("UserId", "~/Admin/People/Users/EditUser.aspx?UserId={0}") %>'></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no users associated with this group."></asp:Label>
                                    </EmptyDataTemplate>
                                </cb:SortedGridView>
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </div>
                </div>
                <asp:ObjectDataSource ID="UsersDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}" 
                    SelectCountMethod="CountForGroup" SelectMethod="LoadForGroup" 
                    SortParameterName="sortExpression" TypeName="CommerceBuilder.Users.UserDataSource">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="groupId" QueryStringField="GroupId"
                            Type="Object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>