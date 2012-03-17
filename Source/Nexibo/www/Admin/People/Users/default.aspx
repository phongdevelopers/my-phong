<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_People_Users_Default" Title="Manage Users" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <script type="text/javascript" language="javascript">
        function UsersSelected()
        {
            var count = 0;
            for(i = 0; i< document.forms[0].elements.length; i++){
                var e = document.forms[0].elements[i];
                var name = e.name;
                if ((e.type == 'checkbox') && (name.endsWith('SelectUserCheckBox')) && (e.checked))
                {
                    count ++;
                }
            }
            return (count > 0);
        }
    </script>
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Manage Users"></asp:Localize></h1></div>
    </div>
    <ajax:UpdatePanel ID="SearchAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
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
                    <td>
                        <asp:TextBox ID="SearchCompany" runat="server" Width="140px" MaxLength="200"></asp:TextBox>
                    </td>
                    <td colspan="2" align="right">
                        <asp:CheckBox ID="SearchIncludeAnonymous" runat="Server" />
                        <asp:Label ID="SearchIncludeAnonymousLabel" runat="server" Text="Include Anonymous" SkinID="FieldHeader" AssociatedControlID="SearchIncludeAnonymous" ToolTip="If checked, anonymous users will be included in the search results."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td colspan="5">
                        <asp:Button ID="SearchButton" runat="server" Text="Search"  SkinId="AdminButton" OnClick="SearchButton_Click" CausesValidation="false"/>
                        <asp:Button ID="ResetSearchButton" runat="server" Text="Reset" Visible="false" OnClick="ResetButton_Click" CausesValidation="false"/>
                        <asp:Button ID="AddUserLink" runat="server" Text="Add User" EnableViewState="false" />
                    </td>
                </tr>
            </table>
            <asp:Label ID="UserAddedMessage" runat="server" Text="User {0} added." SkinID="GoodCondition" Visible="False" EnableViewState="false"></asp:Label>
            <cb:SortedGridView ID="UserGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="UserId" 
                DataSourceId="UserDs" DefaultSortDirection="Ascending" DefaultSortExpression="LoweredUserName" 
                AllowPaging="true" PagerStyle-CssClass="paging" PageSize="20" PagerSettings-Position="Bottom" 
                AllowSorting="true" SkinID="PagedList" Width="100%" OnRowCommand="UserGrid_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Select" >
                        <HeaderStyle horizontalalign="Center" />
                        <ItemStyle horizontalalign="Center" Width="80px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="SelectUserCheckBox" runat="server"></asp:CheckBox>
                       </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="User Name" SortExpression="LoweredUserName">
                        <HeaderStyle horizontalalign="Left" />
                        <ItemStyle horizontalalign="Left" Width="250px" />
                        <ItemTemplate>
                            <asp:Label ID="UserNameLabel" runat="server" Text='<%#Eval("UserName")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" SortExpression="A.LastName">
                        <HeaderStyle horizontalalign="Left" />
                        <ItemStyle horizontalalign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="FullNameLabel" runat="server" Text='<%#GetFullName(Container.DataItem)%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Groups">
                        <HeaderStyle horizontalalign="Left" />
                        <ItemStyle horizontalalign="Left" Width="250px" />
                        <ItemTemplate>
                            <asp:Label ID="GroupsLabel" runat="server" Text='<%#GetUserGroups(Container.DataItem)%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle horizontalalign="Right" Width="80" Wrap="False" />
                        <ItemTemplate>
                            <asp:ImageButton ID="LoginUserButton" runat="server" CommandName="Login" OnClientClick='<%# Eval("UserName", "return confirm(\"Are you sure you want to login as {0}?\")") %>' CommandArgument='<%#Eval("UserId")%>' Visible='<%#IsNotMeOrAdmin(Container.DataItem)%>' SkinID="LoginIcon" AlternateText="Login as User" ToolTip="Login As User" />
                            <asp:HyperLink ID="EditUserLink" runat="server" NavigateUrl='<%# Eval("UserId", "EditUser.aspx?UserId={0}") %>' Visible='<%#IsEditable(Container.DataItem)%>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit User" ToolTip="Edit User" /></asp:HyperLink>
                            <asp:ImageButton ID="DeleteUserButton" runat="server" CommandName="Delete" OnClientClick='<%# Eval("UserName", "return confirm(\"Are you sure you want to delete {0}?\")") %>' Visible='<%#IsDeletable(Container.DataItem)%>' SkinID="DeleteIcon" AlternateText="Delete User" ToolTip="Delete User"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div align="center">
                        <asp:Label runat="server" ID="noUsersFound" enableViewState="false" Text="No users match the search criteria."/>
                    </div>
                </EmptyDataTemplate>
            </cb:SortedGridView>   
            <asp:Button ID="SendEmailSelected" runat="server" Text="Email Selected Users" OnClick="SendEmailSelected_Click" ValidationGroup="UserSelection" OnClientClick="if(!UsersSelected()){alert('No user(s) selected. Please select at least one user.'); return false;}"  />                          
            <asp:Button ID="SendEmailAll" runat="server" Text="Email All Users In Search" OnClick="SendEmailAll_Click" ValidationGroup="UserSelection" />                          
            <asp:CustomValidator ID="UserSelectionValidator" runat="server" ValidationGroup="UserSelection"  Text="No users selected." ErrorMessage="No user selected." EnableViewState="false"/>
            <asp:ObjectDataSource ID="UserDs" runat="server" SelectMethod="Search" TypeName="CommerceBuilder.Users.UserDataSource"
                SelectCountMethod="SearchCount" EnablePaging="True" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Users.User" 
                DeleteMethod="Delete" OnSelecting="UserDs_Selecting">
                <SelectParameters>
                    <asp:Parameter Type="object" Name="criteria" />
                </SelectParameters>
            </asp:ObjectDataSource>                              
            <asp:Panel ID="AddDialog" runat="server" Style="display:none;width:450px" CssClass="modalPopup">
                <asp:Panel ID="AddDialogHeader" runat="server" CssClass="modalPopupHeader" EnableViewState="false">
                    <asp:Localize ID="AddDialogCaption" runat="server" Text="Add User" EnableViewState="false"></asp:Localize>
                </asp:Panel>
                <div style="padding-top:5px;">
                    <table class="inputForm">
                        <tr>
                            <td colspan="2">
                                <asp:Localize ID="AddInstructionText" runat="server" Text="Provide the details for the new user below:"></asp:Localize>
                                <asp:ValidationSummary ID="AddValidationSummary" runat="server" ValidationGroup="AddUser" />
                                <asp:Label ID="ErrorMessage" runat="server" SkinID="ErrorCondition" EnableViewState="false" Visible="false"></asp:Label>
                                <asp:Label ID="AddedMessage" runat="server" SkinID="GoodCondition" EnableViewState="false" Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="AddEmailLabel" runat="server" Text="Email (User Name):" AssociatedControlID="AddEmail" ToolTip="Email address of the user to add.  This will also be their user name."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:TextBox ID="AddEmail" runat="server" MaxLength="200" Width="250px"></asp:TextBox>
                                <asp:PlaceHolder ID="phEmailValidation" runat="server"></asp:PlaceHolder>                                    
                                <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="AddEmail" ValidationGroup="AddUser" Required="true" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>                                
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="AddPasswordLabel" runat="server" Text="Password:" AssociatedControlID="AddPassword" ToolTip="Initial password for the new user."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:TextBox ID="AddPassword" runat="server" TextMode="Password" Columns="20"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="AddPasswordRequired" runat="server" ControlToValidate="AddPassword"
                                    ErrorMessage="Password is required." Text="*" ValidationGroup="AddUser" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:PlaceHolder ID="phPasswordValidation" runat="server"></asp:PlaceHolder>                                    
                            </td>
                       </tr>
                       <tr>     
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="AddConfirmPasswordLabel" runat="server" Text="Retype Password:" AssociatedControlID="AddConfirmPassword" ToolTip="Retype the initial password for the new user."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:TextBox ID="AddConfirmPassword" runat="server" TextMode="Password" Columns="20"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="AddConfirmPasswordRequired" runat="server" ControlToValidate="AddConfirmPassword"
                                    ErrorMessage="You must retype the password." Text="*" ValidationGroup="AddUser" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="AddPasswordCompare" runat="server" ControlToCompare="AddPassword"
                                    ControlToValidate="AddConfirmPassword" ErrorMessage="You did not retype the password correctly."
                                    Text="*" ValidationGroup="AddUser" Display="Dynamic"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <asp:CheckBox ID="ForceExpiration" runat="server" Checked="true" />
                                <asp:Label ID="ForceExpirationLabel" runat="server" AssociatedControlID="ForceExpiration" Text="User must change password at next login"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader" valign="top">
                                <cb:ToolTipLabel ID="AddGroupLabel" runat="server" Text="Group:" AssociatedControlID="AddGroup" ToolTip="If desired select a group that the user should be added to.  If you need to add to multiple groups, edit the user after saving."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:DropDownList ID="AddGroup" runat="server" DataTextField="Name" DataValueField="GroupId" AppendDataBoundItems="true">
                                    <asp:ListItem Text=""></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="SaveButton_Click" ValidationGroup="AddUser" />
                                <asp:Button ID="AddEditButton" runat="server" Text="Save and Edit" OnClick="SaveButton_Click" ValidationGroup="AddUser" />
                                <asp:Button ID="CancelAddButton" runat="server" Text="Cancel" CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <ajax:ModalPopupExtender ID="AddPopup" runat="server" 
                TargetControlID="AddUserLink"
                PopupControlID="AddDialog" 
                BackgroundCssClass="modalBackground"                         
                CancelControlID="CancelAddButton" 
                DropShadow="false"
                PopupDragHandleControlID="AddDialogHeader" />
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>