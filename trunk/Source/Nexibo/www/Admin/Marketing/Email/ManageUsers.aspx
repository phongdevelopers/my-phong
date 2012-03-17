<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ManageUsers.aspx.cs" Inherits="Admin_Marketing_Email_ManageUsers" Title="Manage Email List Members" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<ajax:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="pageHeader">
        	<div class="caption">
        		<h1><asp:Localize ID="Caption" runat="server" Text="Manage Users in {0}"></asp:Localize></h1>
        	</div>
        </div>
        <table cellpadding="2" cellspacing="0" class="innerLayout">
            <tr>    
                <td>
                    <div class="section">
                        <div class="header">
                            <h2 class="customers"><asp:Localize ID="EmailListUsersCaption" runat="server" Text="Users in {0}" /></h2>
                        </div>
                        <div class="content">
                            <cb:SortedGridView ID="UsersInEmailListGrid" runat="server" AllowPaging="True" PageSize="20" AllowSorting="True" AutoGenerateColumns="False"
                                DataKeyNames="Email" Width="100%" SkinID="PagedList" DataSourceID="EmailListUsersDs"
                                DefaultSortExpression="Email" DefaultSortDirection="Ascending" ShowWhenEmpty="False">
                                <Columns>
                                    <asp:TemplateField HeaderText="In List">
                                        <ItemStyle HorizontalAlign="Center" Width="80" />
                                        <ItemTemplate>
                                            <asp:CheckBox ID="IsInEmailList" runat="server" checked="true" OnCheckedChanged="IsInEmailList_CheckedChanged" AutoPostBack="true"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" SortExpression="Email">
                                        <ItemStyle HorizontalAlign="Left" Width="300" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:HyperLink ID="NameLink" runat="server" Text='<%# Eval("Email") %>' NavigateUrl='<%# GetEditUserUrl(Container.DataItem) %>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name">
                                        <ItemStyle horizontalalign="Left" />
                                        <HeaderStyle horizontalalign="Left" />
                                        <ItemTemplate>
                                            <asp:Label ID="FullNameLabel" runat="server" Text='<%# GetFullName(Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle HorizontalAlign="Center" />
                            </cb:SortedGridView>
                            <asp:ObjectDataSource ID="EmailListUsersDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
                                SelectCountMethod="CountForEmailList" SelectMethod="LoadForEmailList" SortParameterName="sortExpression"
                                TypeName="CommerceBuilder.Marketing.EmailListUserDataSource">
                                <SelectParameters>
                                    <asp:QueryStringParameter Name="emailListId" QueryStringField="EmailListId" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="section">
                        <div class="header">
                            <h2 class="adduser"><asp:Localize ID="FindUsersCaption" runat="server" Text="Add User" /></h2>
                        </div>
                        <div class="content">
                            <div class="section">
                                <div class="content">
                                    <asp:Label ID="AlphabetRepeaterLabel" AssociatedControlID="AlphabetRepeater" runat="server" Text="Quick Search:" SkinID="FieldHeader"></asp:Label>
                                    <asp:Repeater runat="server" id="AlphabetRepeater" OnItemCommand="AlphabetRepeater_ItemCommand">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="LinkButton1" CommandName="Display" CommandArgument="<%#Container.DataItem%>" Text="<%#Container.DataItem%>" ValidationGroup="Search" />
                                        </ItemTemplate>                                    
                                    </asp:Repeater><br />
                                    <asp:Localize ID="InstructionText" runat="server" Text="Wildcard characters * and ? are accepted."></asp:Localize>
                                </div>
                            </div>
                            <table class="inputForm">
                                <tr>
                                    <th class="rowHeader">
                                        <asp:Localize ID="SearchUserNameLabel" runat="server" Text="User Name:" EnableViewState="false"></asp:Localize>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="SearchUserName" runat="server" Width="200px" MaxLength="200"></asp:TextBox>
                                    </td>
                                    <th class="rowHeader">
                                        <asp:Localize ID="SearchEmailLabel" runat="server" Text="Email:" EnableViewState="false"></asp:Localize>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="SearchEmail" runat="server" Width="200px" MaxLength="200"></asp:TextBox>
                                    </td>
                                 </tr>
                                 <tr>
                                    <th class="rowHeader">
                                        <asp:Localize ID="SearchFirstNameLabel" runat="server" Text="First Name:" EnableViewState="false"></asp:Localize>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="SearchFirstName" runat="server" Width="140px" MaxLength="40"></asp:TextBox>
                                    </td>
                                    <th class="rowHeader">
                                        <asp:Localize ID="SearchLastNameLabel" runat="server" Text="Last Name:" EnableViewState="false"></asp:Localize>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="SearchLastName" runat="server" Width="140px" MaxLength="40"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader">
                                        <asp:Localize ID="SearchGroupLabel" runat="server" Text="Group:" EnableViewState="false"></asp:Localize>
                                    </th>
                                    <td>
                                        <asp:DropDownList ID="SearchGroup" runat="server" Width="200px" AppendDataBoundItems="true">
                                            <asp:ListItem Text=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th class="rowHeader">
                                        <asp:Localize ID="SearchCompanyLabel" runat="server" Text="Company:" EnableViewState="false"></asp:Localize>
                                    </th>
                                    <td colspan="3">
                                        <asp:TextBox ID="SearchCompany" runat="server" Width="140px" MaxLength="200"></asp:TextBox>
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td colspan="5">
                                        <asp:Button ID="SearchButton" runat="server" Text="Search"  SkinId="AdminButton" OnClick="SearchButton_Click" CausesValidation="false"/>                                        
                                    </td>
                                </tr>
                            </table><br /><br />
                            <cb:SortedGridView ID="SearchUsersGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="Email" 
                                DefaultSortDirection="Ascending" DefaultSortExpression="U.UserName"
                                AllowPaging="true" PageSize="20" AllowSorting="true" Width="100%" SkinID="PagedList">
                                <Columns>
                                    <asp:TemplateField HeaderText="In List">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:CheckBox ID="IsInEmailList2" runat="server" checked='<%#IsInEmailList(Container.DataItem)%>' OnCheckedChanged="IsInEmailList_CheckedChanged" AutoPostBack="true" ></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Name" SortExpression="U.UserName">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:HyperLink ID="EditLink1" runat="server" Text='<%# Eval("UserName") %>' NavigateUrl='<%#GetEditUserUrl(Container.DataItem)%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" SortExpression="U.Email">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:HyperLink ID="EditLink2" runat="server" Text='<%# Eval("Email") %>' NavigateUrl='<%#GetEditUserUrl(Container.DataItem)%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" SortExpression="A.LastName">
                                        <HeaderStyle horizontalalign="Left" />
                                        <ItemStyle horizontalalign="Left" Width="200px"/>
                                        <ItemTemplate>
                                            <asp:Label ID="FullNameLabel2" runat="server" Text='<%#GetFullName(Container.DataItem)%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Groups">
                                        <HeaderStyle horizontalalign="Left" />
                                        <ItemStyle horizontalalign="Left" Width="200px" />
                                        <ItemTemplate>
                                            <asp:Label ID="GroupsLabel" runat="server" Text='<%#GetUserGroups(Container.DataItem)%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div align="center">
                                        <asp:Label runat="server" ID="noUsersFound" enableViewState="false" Text="No matching users found."/>
                                    </div>
                                </EmptyDataTemplate>
                            </cb:SortedGridView>
                            <asp:ObjectDataSource ID="UserDs" runat="server" SelectMethod="Search" TypeName="CommerceBuilder.Users.UserDataSource"
                                SelectCountMethod="SearchCount" EnablePaging="True" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Users.User" 
                                DeleteMethod="Delete" OnSelecting="UserDs_Selecting">
                                <SelectParameters>
                                    <asp:Parameter Type="object" Name="criteria" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </div>
                    </div>            
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:HyperLink ID="FinishLink" runat="server" Text="Return to Email Lists" NavigateUrl="Default.aspx"></asp:HyperLink><br /><br />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>

