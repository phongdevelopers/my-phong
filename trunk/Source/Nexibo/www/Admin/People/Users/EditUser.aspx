<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditUser.aspx.cs" Inherits="Admin_People_Users_EditUser" Title="Edit User" EnableViewState="false" %>
<%@ Register Src="AddressBook.ascx" TagName="AddressBook" TagPrefix="uc" %>
<%@ Register Src="CurrentBasketDialog.ascx" TagName="CurrentBasketDialog" TagPrefix="uc" %>
<%@ Register Src="OrderHistoryDialog.ascx" TagName="OrderHistoryDialog" TagPrefix="uc" %>
<%@ Register Src="ViewHistoryDialog.ascx" TagName="ViewHistoryDialog" TagPrefix="uc" %>
<%@ Register Src="PurchaseHistoryDialog.ascx" TagName="PurchaseHistoryDialog" TagPrefix="uc" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="../../UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:UpdatePanel ID="PageAjax" runat="server">
        <ContentTemplate>
            <div class="pageHeader">
    	        <div class="caption">
    		        <h1><asp:Localize ID="Caption" runat="server" Text="Edit User: {0}"></asp:Localize></h1>
    	        </div>
            </div>
            <table cellpadding="0" cellspacing="0" width="100%">
               <tr>
                    <td valign="top">
                        <ComponentArt:TabStrip ID="EditUserTabs" runat="server" MultiPageId="EditUserPages" SkinID="HorizontalTab">
                            <Tabs>
                                <ComponentArt:TabStripTab ID="AccountTab" Text="Account"></ComponentArt:TabStripTab>
                                <ComponentArt:TabStripTab ID="AddressTab" Text="Address"></ComponentArt:TabStripTab>
                                <ComponentArt:TabStripTab ID="PurchaseHistoryTab" Text="Purchase History"></ComponentArt:TabStripTab>
                                <ComponentArt:TabStripTab ID="OrdersTab" Text="Orders"></ComponentArt:TabStripTab>
                                <ComponentArt:TabStripTab ID="PageViewsTab" Text="Page Views"></ComponentArt:TabStripTab>
                            </Tabs>
                        </ComponentArt:TabStrip>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <ComponentArt:MultiPage ID="EditUserPages" CssClass="hTab_MultiPage" runat="server">
                            <ComponentArt:PageView ID="AccountPage" runat="server">
                                <asp:Label ID="SavedMessage" runat="server" Text="User saved at {0:t}" Visible="false" SkinID="GoodCondition"></asp:Label>
                                <table>
                                    <tr>
                                        <td style="border-right:solid 1px grey;width:450px" valign="top">
                                            <asp:ValidationSummary ID="AccountValidationSummary" runat="server" ValidationGroup="Account" />
                                            <asp:Label ID="AccountSavedMessage" runat="server" Text="Account settings saved at {0:T}." SkinID="GoodCondition" Visible="false"></asp:Label>
                                            <table class="inputForm">
                                                <tr>
                                                    <th class="rowHeader" style="width:80px">
                                                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="User Name:"></asp:Label>
                                                    </th>
                                                    <td>
	                                                    <asp:TextBox ID="UserName" runat="server" Columns="45" MaxLength="250"></asp:TextBox>
                                                        <asp:CustomValidator ID="UserNameAvailableValidator" runat="server" ControlToValidate="Email" ErrorMessage="The username '{0}' is already registered to another user." Text="*" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" Text="Email:"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="Email" runat="server" Columns="45" MaxLength="250"></asp:TextBox>
                                                        <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="Email" Required="true" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*"></cb:EmailAddressValidator>
                                                        <asp:CustomValidator ID="EmailAvailableValidator" runat="server" ControlToValidate="Email" ErrorMessage="The email address '{0}' is already registered to another user." Text="*" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader" valign="top">
                                                        <asp:Label ID="GroupListLabel" runat="server" AssociatedControlID="GroupList" Text="Group(s):"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="GroupList" runat="server" Text="-"></asp:Label>
                                                        <asp:HiddenField ID="GroupListChanged" runat="server" />
                                                        <asp:HiddenField ID="HiddenSelectedGroups" runat="server" />
                                                        <asp:LinkButton ID="ChangeGroupListButton" runat="server" Text="Change" />
                                                        <asp:Panel ID="ChangeGroupListDialog" runat="server" Style="display: none" CssClass="modalPopup" Width="600px">
                                                            <asp:Panel ID="ChangeGroupListDialogHeader" runat="server" CssClass="modalPopupHeader">
                                                                Change Assigned Groups
                                                            </asp:Panel>
                                                            <div align="center">
                                                                <asp:PlaceHolder ID="SubGroupPanel" runat="server">
                                                                    <asp:Label ID="SubGroupListLabel" runat="server" Text="The following groups are assigned via subscription and cannot be altered:"></asp:Label>
                                                                    <asp:Literal ID="SubGroupList" runat="server"></asp:Literal>
                                                                </asp:PlaceHolder>
                                                                <br />
                                                                Hold CTRL to select multiple groups.  Double click to move a group to the other list.
                                                                <br /><br />
                                                                <table cellpadding="0" cellspacing="0">
                                                                    <tr>
                                                                        <td valign="top" width="42%">
                                                                            <b>Available Groups</b><br />
                                                                            <asp:ListBox ID="AvailableGroups" runat="server" Rows="12" SelectionMode="multiple" Width="220"></asp:ListBox>
                                                                        </td>
                                                                        <td valign="middle" width="6%">
                                                                            <asp:Button ID="SelectAllGroups" runat="server" Text=" >> " /><br />
                                                                            <asp:Button ID="SelectGroup" runat="server" Text=" > " /><br />
                                                                            <asp:Button ID="UnselectGroup" runat="server" Text=" < " /><br />
                                                                            <asp:Button ID="UnselectAllGroups" runat="server" Text=" << " /><br />
                                                                        </td>
                                                                        <td valign="top" width="42%">
                                                                            <b>Selected Groups</b><br />
                                                                            <asp:ListBox ID="SelectedGroups" runat="server" Rows="12" SelectionMode="multiple" Width="220"></asp:ListBox>
                                                                        </td>
                                                                    </tr>
                                                                </table><br />
                                                                <asp:PlaceHolder ID="phMyGroupsWarning" runat="server" Visible="false">
                                                                    <br />
                                                                    <asp:Label ID="MyGroupsWarning" runat="server" Text="WARNING: You are modifying your own groups.  Be careful not to lock yourself out!" SkinID="ErrorCondition"></asp:Label>
                                                                    <br /><br />
                                                                </asp:PlaceHolder>
                                                                <asp:Button ID="ChangeGroupListOKButton" runat="server" Text="OK" OnClientClick="changeGroupList()" OnClick="ChangeGroupListOKButton_Click" />
                                                                <asp:Button ID="ChangeGroupListCancelButton" runat="server" Text="Cancel" />
                                                                <br /><br />
                                                            </div>
                                                        </asp:Panel>
                                                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender" runat="server" 
                                                            TargetControlID="ChangeGroupListButton"
                                                            PopupControlID="ChangeGroupListDialog" 
                                                            BackgroundCssClass="modalBackground" 
                                                            CancelControlID="ChangeGroupListCancelButton" 
                                                            DropShadow="true"
                                                            PopupDragHandleControlID="ChangeGroupListDialogHeader" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="IsDisabledLabel" runat="server" AssociatedControlID="IsDisabled" Text="Disabled:"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:CheckBox ID="IsDisabled" runat="server" ForeColor="Red"></asp:CheckBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="AffiliateLabel" runat="server" AssociatedControlID="Affiliate" Text="Referred By: "></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:DropDownList ID="Affiliate" runat="server" AppendDataBoundItems="true" DataSourceID="AffiliateDS" DataTextField="Name" DataValueField="AffiliateId" OnDataBound="Affiliate_DataBound">
                                                            <asp:ListItem Selected="True" Value="0" Text="No Affliate"></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:Label ID="ReferredDate" runat="server" Text=""></asp:Label>
                                                        <asp:ObjectDataSource ID="AffiliateDS" runat="server" SelectMethod="LoadForStore" TypeName="CommerceBuilder.Marketing.AffiliateDataSource" DataObjectTypeName="CommerceBuilder.Marketing.Affiliate">
                                                            <SelectParameters>
                                                                <asp:Parameter Name="sortExpression" DbType="String" DefaultValue="Name" />
                                                            </SelectParameters>
                                                        </asp:ObjectDataSource>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                    <td>
                                                        <asp:Button ID="SaveAccountButton" runat="server" Text="Save" OnClick="SaveAccountButton_Click" />
                                                        <asp:Button ID="BackButton" runat="server" OnClick="BackButton_Click" Text="Cancel" CausesValidation="false" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td valign="top">
                                            <table class="inputForm">
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="RegisteredSinceDateLabel" runat="server" AssociatedControlID="RegisteredSinceDate" Text="Registered Since:"></asp:Label>
                                                    </th>
                                                    <td align="left">
                                                        <asp:Literal ID="RegisteredSinceDate" runat="server" Text=""></asp:Literal>
                                                    </td>
                                                </tr>                
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="LastActiveDateLabel" runat="server" AssociatedControlID="LastActiveDate" Text="Last Active:"></asp:Label>
                                                    </th>
                                                    <td align="left">
                                                        <asp:Literal ID="LastActiveDate" runat="server" Text="-"></asp:Literal>
                                                    </td>
                                                </tr>                
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="FailedLoginCountLabel" runat="server" AssociatedControlID="FailedLoginCount" Text="Failed Logins:"></asp:Label>
                                                    </th>
                                                    <td align="left">
                                                        <asp:Literal ID="FailedLoginCount" runat="server" Text=""></asp:Literal>
                                                    </td>
                                                </tr>                
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="LastLockoutDateLabel" runat="server" AssociatedControlID="LastLockoutDate" Text="Last Lockout:"></asp:Label>
                                                    </th>
                                                    <td align="left">
                                                        <asp:Literal ID="LastLockoutDate" runat="server" Text="-"></asp:Literal>
                                                    </td>
                                                </tr>                
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ChangePasswordLabel" runat="server" AssociatedControlID="ChangePasswordButton" Text="Password:"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="PasswordLastChangedText" runat="server" Text="Password last changed {0} ago."></asp:Label>
                                                        <asp:LinkButton ID="ChangePasswordButton" runat="server" Text="Change" />
                                                        <asp:Panel ID="ChangePasswordDialog" runat="server" Style="display:none;width:400px" CssClass="modalPopup">
                                                            <asp:Panel ID="ChangePasswordDialogHeader" runat="server" CssClass="modalPopupHeader">
                                                                <asp:Label ID="ChangePasswordDialogCaption" runat="server" Text="Change Password"></asp:Label>
                                                            </asp:Panel>
                                                            <div style="padding-top:5px;">
                                                                <table class="inputForm" cellpadding="3">
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <asp:Label ID="ChangePasswordHelpText" runat="server" Text="Set the new password below.  Changed password takes effect immediately - it is not required to click save.  Set the new password according to the policy rules:"></asp:Label><br /><br />
                                                                            <asp:Localize ID="PasswordPolicyLength" runat="server" Text="* The new password must be at least {0} characters long."></asp:Localize><br />
                                                                            <asp:Localize ID="PasswordPolicyRequired" runat="server" Text="* The password must include at least one {0}."></asp:Localize><br /><br />
                                                                            <asp:ValidationSummary ID="ChangePasswordValidationSummary" runat="server" ValidationGroup="ChangePassword" />
                                                                        </td>
                                                                    </tr>   
                                                                    <tr>
                                                                        <th class="rowHeader">
                                                                            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword" Text="Password:"></asp:Label>
                                                                        </th>
                                                                        <td align="left">
                                                                            <asp:TextBox ID="NewPassword" runat="server" TextMode="Password" Columns="20" MaxLength="50"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="PasswordRequiredValidator" runat="server" 
                                                                                ControlToValidate="NewPassword" ErrorMessage="New password is required."
                                                                                Text="*" ValidationGroup="ChangePassword"></asp:RequiredFieldValidator>
                                                                            <asp:CustomValidator ID="PasswordPolicyValidator" runat="server" ErrorMessage="Password does not meet the policy requirements."
                                                                                Text="*" ValidationGroup="ChangePassword"></asp:CustomValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <th class="rowHeader">
                                                                            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword" Text="Retype Password:"></asp:Label>
                                                                        </th>
                                                                        <td align="left">
                                                                            <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password" Columns="20" MaxLength="50"></asp:TextBox>
                                                                            <asp:CompareValidator ID="NewPasswordComparer" runat="server" ControlToCompare="ConfirmNewPassword"
                                                                                ControlToValidate="NewPassword" Display="Static" ErrorMessage="You did not retype the password correctly."
                                                                                Text="*" ValidationGroup="ChangePassword"></asp:CompareValidator>
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
                                                                        <td>&nbsp;</td>
                                                                        <td>
                                                                            <br />
                                                                            <asp:Button ID="ChangePasswordOKButton" runat="server" Text="OK" OnClick="ChangePasswordOKButton_Click" OnClientClick="return Page_ClientValidate('ChangePassword')" ValidationGroup="ChangePassword" />
                                                                            <asp:Button ID="ChangePasswordCancelButton" runat="server" Text="Cancel" OnClick="ChangePasswordCancelButton_Click" CausesValidation="false" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </asp:Panel>
                                                        <ajaxToolkit:ModalPopupExtender ID="ChangePasswordPopup" runat="server" 
                                                            TargetControlID="ChangePasswordButton"
                                                            PopupControlID="ChangePasswordDialog" 
                                                            BackgroundCssClass="modalBackground"                         
                                                            CancelControlID="ChangePasswordCancelButton" 
                                                            DropShadow="true"
                                                            PopupDragHandleControlID="ChangePasswordDialogHeader" />
                                                    </td>
                                                </tr>                
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <asp:PlaceHolder ID="SubscriptionsPanel" runat="server">
                                    <div class="sectionHeader">
                                        <h2><asp:Label ID="SubscriptionsCaption" runat="server" Text="Suscriptions"></asp:Label></h2>
                                    </div>
                                    <cb:SortedGridView ID="SubscriptionGrid" runat="server" AutoGenerateColumns="False" DataSourceID="SubscriptionDs"  DataKeyNames="SubscriptionId"
                                        SkinID="PagedList" AllowSorting="true" ShowWhenEmpty="False" Width="100%" AllowPaging="true" PageSize="20" EnableViewState="False" 
                                        DefaultSortDirection="Ascending" DefaultSortExpression="S.ExpirationDate" OnRowCommand="SubscriptionGrid_RowCommand" OnRowUpdating="SubscriptionGrid_RowUpdating">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Plan" SortExpression="SP.Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <a href="../../Products/EditSubscription.aspx?ProductId=<%#Eval("ProductId")%>"><%#Eval("SubscriptionPlan.Name")%></a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Order" SortExpression="O.OrderNumber">
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="orderLink" runat="server" NavigateUrl='<%#String.Format("../../Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderItem.Order.OrderNumber"), Eval("OrderItem.OrderId"))%>' Text='<%# Eval("OrderItem.Order.OrderNumber") %>' SkinID="Link"></asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Expiration" SortExpression="S.ExpirationDate">
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Label ID="Expiration" runat="server" text='<%#Eval("ExpirationDate", "{0:d}")%>' visible='<%# ((DateTime)Eval("ExpirationDate") != DateTime.MinValue) %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="EditExpiration" runat="server" Text='<%# Bind("ExpirationDate", "{0:d}") %>' width="80px" />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Group">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <%#GetSubGroupName(Container.DataItem)%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Active">
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="Active" runat="server" Checked='<%#Eval("IsActive")%>' Enabled="False" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>                    
                                                <ItemStyle HorizontalAlign="right" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="ActivateLink" runat="server" visible='<%#(!(bool)Eval("IsActive"))%>' text="activate" SkinID="Button" CommandName="Activate" CommandArgument='<%#Eval("SubscriptionId")%>' />
                                                    <asp:LinkButton ID="EditButton" runat="server" visible='<%#((bool)Eval("IsActive"))%>' CommandName="Edit"><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" /></asp:LinkButton>
                                                    <asp:LinkButton ID="CancelLink" runat="server"  SkinID="Link" CommandName="CancelSubscription" CommandArgument='<%#Eval("SubscriptionId")%>' OnClientClick='javascript:return confirm("Are you sure you want to cancel the subscription?")'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" /></asp:LinkButton>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:ImageButton ID="EditSaveButton" runat="server" CommandName="Update" SkinID="SaveIcon" ToolTip="Save"></asp:ImageButton>
                                                    <asp:ImageButton ID="EditCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" SkinID="CancelIcon" ToolTip="Cancel Editing"></asp:ImageButton>
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate> 
                                            <asp:Label ID="EmptyMessage" runat="server" Text="There are no active or pending subscriptions for this user."></asp:Label>
                                        </EmptyDataTemplate>
                                    </cb:SortedGridView>
                                    <asp:ObjectDataSource ID="SubscriptionDs" runat="server" SelectMethod="Search" SelectCountMethod="SearchCount"
                                        TypeName="CommerceBuilder.Orders.SubscriptionDataSource" DataObjectTypeName="CommerceBuilder.Orders.Subscription"
                                        SortParameterName="sortExpression" EnablePaging="true">
                                        <SelectParameters>
                                            <asp:Parameter Name="subscriptionPlanId" Type="int32" DefaultValue="0" />
                                            <asp:Parameter Name="orderRange" Type="String" DefaultValue="" />
                                            <asp:QueryStringParameter Name="userIdRange" Type="String" QueryStringField="UserId" />
                                            <asp:Parameter Name="firstName" Type="String" DefaultValue="" />
                                            <asp:Parameter Name="lastName" Type="String" DefaultValue="" />
                                            <asp:Parameter Name="email" Type="String" DefaultValue="" />
                                            <asp:Parameter Name="expirationStart" DefaultValue="" />
                                            <asp:Parameter Name="expirationEnd" DefaultValue="" />
                                            <asp:Parameter Name="active" DefaultValue="Any" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </asp:PlaceHolder>
                            </ComponentArt:PageView>
                            <ComponentArt:PageView ID="AddressBookPage" runat="server">
                                <uc:AddressBook ID="AddressBook" runat="server" />
                            </ComponentArt:PageView>
                             <ComponentArt:PageView ID="PurchaseHistoryPage" runat="server">
                                <uc:PurchaseHistoryDialog ID="PurchaseHistoryDialog1" runat="server" />
                            </ComponentArt:PageView>
                            <ComponentArt:PageView ID="OrdersPage" runat="server">
                                <uc:CurrentBasketDialog ID="CurrentBasketDialog1" runat="server" /><br />
                                <uc:OrderHistoryDialog ID="OrderHistoryDialog1" runat="server" />
                            </ComponentArt:PageView>
                            <ComponentArt:PageView ID="PageViewsPage" runat="server">
                                <uc:ViewHistoryDialog id="ViewHistoryDialog1" runat="server" />                    
                            </ComponentArt:PageView>
                        </ComponentArt:MultiPage>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

