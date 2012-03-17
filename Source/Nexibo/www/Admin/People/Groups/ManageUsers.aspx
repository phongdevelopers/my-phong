<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="ManageUsers.aspx.cs" Inherits="Admin_People_Groups_ManageUsers" Title="Manage Users in Group"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Manage Users in {0}"></asp:Localize></h1>
    	</div>
    </div>
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td>
                        <div class="section">
                            <div class="header">
                                <h2 class="customers"><asp:Localize ID="GroupUsersCaption" runat="server" Text="Users in Group" /></h2>
                            </div>
                            <div class="content">
                                <cb:SortedGridView ID="UsersInGroupGrid" runat="server" AllowPaging="True" AllowSorting="true" AutoGenerateColumns="False"
                                    DataKeyNames="UserId" Width="100%" SkinID="PagedList" DataSourceID="UsersInGroupDs" DefaultSortExpression="Username">
                                    <Columns>
                                        <asp:TemplateField HeaderText="In Group">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:CheckBox ID="IsInGroup" runat="server" checked="true" Enabled='<%#EnableChange(Container.DataItem)%>' OnCheckedChanged="IsInGroup_CheckedChanged" AutoPostBack="true"></asp:CheckBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="User" SortExpression="Username">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="NameLink" runat="server" Text='<%# Eval("Username") %>' NavigateUrl='<%#Eval("UserId", "../Users/EditUser.aspx?UserId={0}")%>'></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Name">
                                            <HeaderStyle horizontalalign="Left" />
                                            <ItemStyle horizontalalign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="FullNameLabel" runat="server" Text='<%#GetFullName(Container.DataItem)%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle HorizontalAlign="Center" />
                                </cb:SortedGridView>
                                <asp:ObjectDataSource ID="UsersInGroupDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
                                    SelectCountMethod="CountForGroup" SelectMethod="LoadForGroup" SortParameterName="sortExpression"
                                    TypeName="CommerceBuilder.Users.UserDataSource">
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="groupId" QueryStringField="GroupId" Type="Object" />
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
                                <h2 class="addgroup"><asp:Localize ID="FindUsersCaption" runat="server" Text="Find User" /></h2>
                            </div>
                            <div class="content">
                                <asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton" CssClass="searchPanel">
                                    <asp:Label ID="SearchByLabel" runat="server" Text="Search by:" SkinID="FieldHeader" AssociatedControlID="SearchByDropDown" ToolTip="Search field"></asp:Label>
                                    <asp:DropDownList ID="SearchByDropDown" runat="server">
                                        <asp:ListItem Value="UserName" Text="User Name"></asp:ListItem>
                                        <asp:ListItem Value="Email" Text="Email"></asp:ListItem>
                                        <asp:ListItem Value="LastName" Text="Last Name"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Label ID="SearchTextLabel" runat="server" Text="for:" SkinID="FieldHeader" AssociatedControlID="SearchText" ToolTip="Search pattern"></asp:Label>
                                    <asp:TextBox ID="SearchText" runat="server"></asp:TextBox>
                                    <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
                                </asp:Panel>
                                <cb:SortedGridView ID="SearchUsersGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="UserId" 
                                    DefaultSortDirection="Ascending" DefaultSortExpression="UserName"
                                    AllowPaging="true" AllowSorting="true" Width="100%" SkinID="PagedList">
                                    <Columns>
                                        <asp:TemplateField HeaderText="In Group">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:CheckBox ID="IsInGroup2" runat="server" checked='<%#IsInGroup(Container.DataItem)%>' Enabled='<%#EnableChange(Container.DataItem)%>' OnCheckedChanged="IsInGroup_CheckedChanged" AutoPostBack="true" ></asp:CheckBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="User Name" SortExpression="UserName">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="UserNameLabel" runat="server" Text='<%#Eval("UserName")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Email" SortExpression="Email">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="EmailLabel" runat="server" Text='<%#Eval("Email")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Name" SortExpression="LastName">
                                            <HeaderStyle horizontalalign="Left" />
                                            <ItemStyle horizontalalign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="FullNameLabel2" runat="server" Text='<%#GetFullName(Container.DataItem)%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label runat="server" ID="noUsersFound" enableViewState="false" Text="No matching users found."/>
                                    </EmptyDataTemplate>
                                </cb:SortedGridView>
                                <asp:ObjectDataSource ID="SearchUsersDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="FindUsersByName" TypeName="CommerceBuilder.Users.UserDataSource"
                                     SelectCountMethod="CountUsersByName" EnablePaging="True" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Users.User" DeleteMethod="Delete">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="SearchText" Name="searchPattern" PropertyName="Text" Type="String" />
                                        <asp:ControlParameter ControlID="SearchByDropDown" Name="searchField" PropertyName="SelectedValue" Type="Object" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </div>
                        </div>            
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

