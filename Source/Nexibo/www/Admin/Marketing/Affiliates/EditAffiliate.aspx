<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="EditAffiliate.aspx.cs" Inherits="Admin_Marketing_Affiliates_EditAffiliate" Title="Edit Affiliate" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Edit {0}" EnableViewState="false"></asp:Localize></h1>
        </div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" align="center">
                <asp:Label ID="InstructionText" runat="server" Text="To associate a link with this affiliate, add <b>{0}={1}</b> to the url. For example:<br />{2}?{0}={1}<br /><br />"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <ajax:UpdatePanel ID="EditAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="inputForm">
                            <tr>
                                <td class="validation" colspan="2">
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                    <asp:Label ID="SavedMessage" runat="server" Text="Saved at {0:t}" Visible="false"
                                        SkinID="GoodCondition" EnableViewState="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Affiliate Name:" ToolTip="The name of the affiliate as it will appear on reports and in the merchant admin." />
                                </th>
                                <td>
                                    <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                                        Display="Static" ErrorMessage="Affiliate name is required." Text="*"></asp:RequiredFieldValidator>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" 
                                        Text="Email:"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Email" runat="server" MaxLength="255"></asp:TextBox>
                                    <cb:EmailAddressValidator ID="FromEmailAddressValidator" runat="server" 
                                        ControlToValidate="Email" EnableViewState="False" 
                                        ErrorMessage="Email address should be in the format of name@domain.tld." 
                                        Required="true" Text="*">
                                    &nbsp;&nbsp;
                                    </cb:EmailAddressValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="CommissionRateLabel" runat="server" Text="Commission Rate:"
                                        ToolTip="The rate used for the calculation of commission - either a dollar amount or a percentage." />
                                </th>
                                <td>
                                    <asp:TextBox ID="CommissionRate" runat="server" Width="60px" MaxLength="8"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="CommissionRateValidator" runat="server" Display="Static"
                                        ErrorMessage="Commission rate is invalid." Text="*" ControlToValidate="CommissionRate"
                                        ValidationExpression="\d{0,4}(\.\d{0,3})?"></asp:RegularExpressionValidator>
                                </td>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="CommissionTypeLabel" runat="server" Text="Commission Type:"
                                        ToolTip="Indicates the way the commission should be calculated.  Flat rate pays a fixed amount for each order.  Percentage of products subtotal calculates on the order total less taxes, shipping, etc.  Percentage of order total calculates on the order total including taxes and shipping." />
                                </th>
                                <td>
                                    <asp:DropDownList ID="CommissionType" runat="server">
                                        <asp:ListItem Text="Flat rate"></asp:ListItem>
                                        <asp:ListItem Text="% of product subtotal"></asp:ListItem>
                                        <asp:ListItem Text="% of order total"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="WebsiteUrlLabel" runat="server" Text="Website Url:" AssociatedControlID="WebsiteUrl"
                                        ToolTip="Enter the Affiliate's website URL.  This information is for your reference only." />
                                </th>
                                <td>
                                    <asp:TextBox ID="WebsiteUrl" runat="server" MaxLength="255"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="AffiliateGroupLabel" runat="server" Text="Owner Group:"
                                        ToolTip="Any member of the selected group has the ability to view sales reports for this affiliate from their member account page." />
                                </th>
                                <td>
                                    <asp:DropDownList ID="AffiliateGroup" runat="server" DataSourceID="GroupDs" DataValueField="GroupId" DataTextField="Name" AppendDataBoundItems="true">
                                        <asp:ListItem Text="" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="GroupDs" runat="server" 
                                        DataObjectTypeName="CommerceBuilder.Users.Group" 
                                        OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForStore" 
                                        SortParameterName="sortExpression" 
                                        TypeName="CommerceBuilder.Users.GroupDataSource"></asp:ObjectDataSource>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="PersistenceLabel" runat="server" Text="Persistence:"
                                        ToolTip="Indicate how long orders will count for commissions after an affiliate refers a customer." />
                                </th>
                                <td>
                                    <asp:DropDownList ID="ReferralPeriod" runat="server" 
                                        onselectedindexchanged="ReferralPeriod_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Text="Persistent" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="First Order" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="First X Days" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="First Order Within X Days" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th class="rowHeader">
                                    
                                    <cb:ToolTipLabel ID="ReferralDaysLabel" runat="server" Text="Referral Period:" 
                                        ToolTip="Time length of time (in days) that the affiliate will get credit for a sale made by a referred customer." Visible="false" />
                                    
                                </th>
                                <td>
                                    
                                    <asp:TextBox ID="ReferralDays" runat="server" MaxLength="4" Width="60px" Visible="false"></asp:TextBox>
                                    <asp:Localize ID="ReferralDaysLabel2" runat="server" Text="days" Visible="false"></asp:Localize>
                                    <asp:RequiredFieldValidator ID="ReferralPeriodRequiredValidator" runat="server" ControlToValidate="ReferralDays" Text="*" ErrorMessage="You must specify number of days for referral."></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="ReferralPeriodValidator" runat="server" Type="Integer" MinimumValue="1"  MaximumValue="9999" ControlToValidate="ReferralDays" ErrorMessage="Referral period must be a numeric value greater than 0." Text="*" ></asp:RangeValidator>
                                    
                                </td>
                            </tr>
                            <tr class="sectionHeader">
                                <th colspan="4">
                                    <asp:Label ID="AddressCaption" runat="server" Text="Address Information"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="FirstName"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="FirstName" runat="server" MaxLength="30"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="LastName"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="LastName" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="CompanyLabel" runat="server" Text="Company:" AssociatedControlID="Company"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Company" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="Address1Label" runat="server" Text="Street Address 1:" AssociatedControlID="Address1"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Address1" runat="server" MaxLength="100"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:" AssociatedControlID="Address2"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Address2" runat="server" MaxLength="100"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="City" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="Province"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Province" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="PostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="PostalCode"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="PostalCode" runat="server" MaxLength="15"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="CountryCodeLabel" runat="server" Text="Country:" AssociatedControlID="CountryCode"></asp:Label>
                                </th>
                                <td>
                                    <asp:DropDownList ID="CountryCode" runat="server" DataTextField="Name" DataValueField="CountryCode">
                                    </asp:DropDownList>
                                </td>
                                <th class="rowHeader" valign="top">
                                    <asp:Label ID="PhoneNumberLabel" runat="server" Text="Phone:" AssociatedControlID="PhoneNumber"></asp:Label>
                                </th>
                                <td colspan="3">
                                    <asp:TextBox ID="PhoneNumber" runat="server" MaxLength="50"></asp:TextBox><br />
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="FaxNumberLabel" runat="server" Text="Fax Number:" AssociatedControlID="FaxNumber"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="FaxNumber" runat="server" MaxLength="20"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="MobileNumberLabel" runat="server" Text="Mobile Number:" AssociatedControlID="MobileNumber"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="MobileNumber" runat="server" MaxLength="20"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="submit" colspan="4">                                    
                                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
									<asp:Button ID="CancelButton" runat="server" Text="Close" OnClick="CancelButton_Click" CausesValidation="false" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
       
        <tr>
            <td class="dataSheet">
                <div class="section">
                    <div class="header">
                        <h2><asp:Localize ID="OrdersCaption" runat="server" Text="Associated Orders"></asp:Localize></h2>
                    </div>
                    <div class="content">
                <ajax:UpdatePanel ID="PagingAjax" runat="server" UpdateMode="conditional">
                    <ContentTemplate>
                        <cb:SortedGridView ID="OrdersGrid" runat="server" DataSourceID="OrdersDs" AllowPaging="True"
                            AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="OrderId" PageSize="20"
                            SkinID="PagedList" DefaultSortExpression="OrderDate" DefaultSortDirection="Descending"
                            Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" SortExpression="OrderId">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:HyperLink ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>' NavigateUrl='<%#String.Format("../../Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId")) %>' SkinId="Link"></asp:HyperLink>
                                    </itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date" SortExpression="OrderDate">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:Label ID="OrderDate" runat="server" Text='<%# Eval("OrderDate", "{0:d}") %>'></asp:Label>
                                    </itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Subtotal" SortExpression="ProductSubtotal">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:Label ID="Subtotal" runat="server" Text='<%# Eval("ProductSubtotal", "{0:lc}") %>'></asp:Label>
                                    </itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total" SortExpression="TotalCharges">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:Label ID="Total" runat="server" Text='<%# Eval("TotalCharges", "{0:lc}") %>'></asp:Label>
                                    </itemtemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyMessage" runat="server" Text="There are no orders associated with this affiliate."></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="OrdersDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
        SelectCountMethod="CountForAffiliate" SelectMethod="LoadForAffiliate" SortParameterName="sortExpression"
        TypeName="CommerceBuilder.Orders.OrderDataSource">
        <SelectParameters>
            <asp:QueryStringParameter Name="affiliateId" QueryStringField="AffiliateId" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
