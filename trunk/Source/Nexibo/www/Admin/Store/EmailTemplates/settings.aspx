<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="Admin_Store_EmailTemplates_Settings" Title="Configure Email Settings" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Email Settings"></asp:Localize></h1>
        </div>
    </div>
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td valign="top" width="50%">
                        <div class="section">
                            <div class="header">
                                <h2 class="commonicon"><asp:Localize ID="GeneralCaption" runat="server" Text="General"></asp:Localize></h2>
                            </div>
                            <div class="content">
                                <asp:Label ID="GeneralSavedMessage" runat="server" Text="General email settings saved at {0:t}." Visible="false" SkinID="GoodCondition" EnableViewState="false"></asp:Label>
                                <asp:ValidationSummary ID="GeneralValidationSummary" runat="server" ValidationGroup="General" />
                                <table cellpadding="3" cellspacing="0" class="inputForm" width="100%">
                                    <tr>
                                        <th class="rowHeader" valign="top" style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="DefaultAddressLabel" runat="server" Text="Default 'From' Address:" ToolTip="This is the address that will appear in the from field by default when you create new email message templates.  You can always alter the from address on a per-message basis. " />
                                        </th>
                                        <td>
                                            <asp:TextBox ID="DefaultAddress" runat="server" Width="250px" MaxLength="200" ValidationGroup="General"></asp:TextBox>                                            
                                            <cb:EmailAddressValidator ID="DefaultFromEmailAddressValidator" runat="server" ControlToValidate="DefaultAddress" ValidationGroup="General" Required="true" ErrorMessage="Default 'from' email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator><br />
                                            <asp:CheckBox ID="UpdateAllEmailTemplates" runat="server" /><cb:ToolTipLabel ID="UpdateExistingTemplateLabel" runat="server" Text="Update Existing Templates" ToolTip="If you change the default from address, check this box to automatically update all existing email templates that use the current default." />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" valign="top" style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="SubscriptionAddressLabel" runat="server" Text="Service 'From' Address:" ToolTip="If you configure mailing lists, this is the email address used for opt-in confirmation and/or verification service notifications.  It will also be the from address for some generated system messages.  It is recommended this be set to an unattended email address like noreply@yourdomain.xyz." />
                                        </th>
                                        <td>
                                            <asp:TextBox ID="SubscriptionAddress" runat="server" Width="250px" MaxLength="200" ValidationGroup="General"></asp:TextBox>
                                            <cb:EmailAddressValidator ID="SubscriptionEmailAddressValidator" runat="server" ControlToValidate="SubscriptionAddress" ValidationGroup="General" Required="true" ErrorMessage="The service 'from' email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" valign="top" style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="SubscriptionRequestExpirationDaysLabel" runat="server" Text="List Request Expiration:" ToolTip="If you configure an opt-in mailing list with verification, this is the number of days a customer has to verify the request before it is considered expired and removed from the database." />
                                        </th>
                                        <td>
                                            <asp:TextBox ID="SubscriptionRequestExpirationDays" runat="server" width="30px" MaxLength="3" ValidationGroup="General"></asp:TextBox>&nbsp;
                                            <asp:Localize ID="DaysLabel" runat="server" Text="days"></asp:Localize>
                                            <asp:RequiredFieldValidator ID="SubscriptionRequestExpirationDaysRequired" runat="server" ControlToValidate="SubscriptionRequestExpirationDays" ValidationGroup="General"
                                                ErrorMessage="You must enter the number of days before a list request is expired." Text="*"></asp:RequiredFieldValidator>
                                            <asp:RangeValidator ID="RequestExpirationRangeValidator" runat="server" ControlToValidate="SubscriptionRequestExpirationDays" ValidationGroup="General"
                                                ErrorMessage="The list request expiration must be in the range of 1 to 30." Text="*"
                                                Type="Integer" MinimumValue="1" MaximumValue="30"></asp:RangeValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" valign="top" style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="DefaultEmailListLabel" runat="server" Text="Default Email List:" ToolTip="This is the default email list for your store - if you allow anonymous users to sign up for your mailing list this is the one that will be used." />
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="DefaultEmailList" runat="server" DataTextField="Name" DataValueField="EmailListId" AppendDataBoundItems="true" EnableViewState="false">
    	                                        <asp:ListItem Text="None" Value="0"></asp:ListItem>
                                            </asp:DropDownList><br /><br />
                                        </td>
                                    </tr>
                                    <tr class="sectionHeader">
                                        <th colspan="2">
                                            <asp:Localize ID="ProductTellAFriendCaption" runat="server" Text="Product 'Send to Friend' Feature"></asp:Localize>
                                        </th>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" valign="top"style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="EmailTemplatesListLabel" runat="server" Text="Email Template:" AssociatedControlID="EmailTemplatesList" ToolTip="When a customer uses the 'Send to Friend' feature on the product detail page, select the template that will be used to populate the message." EnableViewState="false" />
                                        </th>
                                        <td>
    	                                    <asp:DropDownList ID="EmailTemplatesList" runat="server" DataTextField="Name" DataValueField="EmailTemplateId" AppendDataBoundItems="true" EnableViewState="false" Width="300px">
    	                                        <asp:ListItem Text="None" Value="0"></asp:ListItem>
    	                                    </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <asp:CheckBox ID="TellAFriendCaptcha" runat="server" />
                                            <cb:ToolTipLabel ID="TellAFriendCaptchaLabel" runat="server" Text="Use CAPTCHA:" ToolTip="If checked, an image CAPTCHA must be solved to use the product 'Send to Friend' feature."></cb:ToolTipLabel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <br />
                                            <asp:Button ID="SaveGeneralButton" runat="server" OnClick="SaveGeneralButton_Click" Text="Save Settings" ValidationGroup="General" CausesValidation="true" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                    <td valign="top" width="50%">
                        <div class="section">
                            <div class="header">
                                <h2 class="SMTPserver"><asp:Localize ID="ServerCaption" runat="server" Text="Server Configuration"></asp:Localize></h2>
                            </div>
                            <div class="content">
                                <asp:PlaceHolder ID="SMTPTestResultPanel" runat="server"></asp:PlaceHolder>
                                <asp:ValidationSummary ID="SMTPValidationSummary" runat="server" ValidationGroup="SMTP" />
                                <asp:Label ID="WarningLabel" runat="server" SkinID="warnCondition" Text="Warning: Your email server settings have been removed. You will not be able to send email notifications." EnableViewState="false" Visible="false"></asp:Label>
                                <table cellpadding="3" cellspacing="0" class="inputForm">
                                    <tr>
                                        <td colspan="2">
                                        </td>
                                    </tr>                                    
                                    <tr>
                                        <th class="rowHeader" valign="top" style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="SmtpServerLabel" runat="server" Text="SMTP Server:" ToolTip="This is the IP address or dns name of your SMTP host, for example mail.yourdomain.xyz." />
                                        </th>
                                        <td>
                                            <asp:TextBox ID="SmtpServer" runat="server" Width="200px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="SmtpServerRequired" runat="server" ControlToValidate="SmtpServer" ValidationGroup="SMTP"
                                                ErrorMessage="You must provide the DNS name or IP address of the SMTP server." Text="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="SmtpPortLabel" runat="server" Text="SMTP Port:" ToolTip="This is the port that will be used to communicate with the server.  This will almost always port 25.  It may also be port 465 if you are using SMTP over SSL." />
                                        </th>
                                        <td>
                                            <asp:TextBox ID="SmtpPort" runat="server" Text="25" Columns="3" MaxLength="5"></asp:TextBox>&nbsp;&nbsp;
                                                <asp:RequiredFieldValidator ID="SmtpPortRequired" runat="server" ControlToValidate="SmtpPort" ValidationGroup="SMTP"
                                                    ErrorMessage="The SMTP port is required.  If you aren't sure of the value, use 25." Text="*"></asp:RequiredFieldValidator>
                                                <asp:RangeValidator ID="SmtpPortRangeValidator" runat="server" ControlToValidate="SmtpPort" ValidationGroup="SMTP"
                                                    ErrorMessage="The SMTP port must be in the range of 1 to 65535.  The values 25 and 465 are most common." Text="*"
                                                    Type="Integer" MinimumValue="1" MaximumValue="65535"></asp:RangeValidator>
                                            <asp:CheckBox ID="SmtpEnableSSL" runat="server" />
                                            <cb:ToolTipLabel ID="SSLEnabledLabel" runat="server" Text="Enable SSL:" ToolTip="Check here if your server uses SMTP over SSL.  This is not the same as having an SSL certificate on your website." />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="SmtpUserNameLabel" runat="server" Text="SMTP Username:" ToolTip="If your server requires authentication, provide the username here." />
                                        </th>
                                        <td><asp:TextBox ID="SmtpUserName" runat="server" Width="120px" MaxLength="50"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader" style="white-space:nowrap">
                                            <cb:ToolTipLabel ID="SmtpPasswordLabel" runat="server" Text="SMTP Password:" ToolTip="If your server requires authentication, provide the password here." />
                                        </th>
                                        <td><asp:TextBox ID="SmtpPassword" runat="server" TextMode="Password" Width="120px" MaxLength="50"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <asp:CheckBox ID="RequiresAuth" runat="server"></asp:CheckBox>&nbsp;
                                            <cb:ToolTipLabel ID="RequiresAuthLabel" runat="server" Text="Use Authentication:" ToolTip="Check here if your server requires authentication." />
                                        </td>
                                    </tr>
                                    <tr>    
                                        <td>&nbsp;</td>
                                        <td>
                                            <br /><asp:Button ID="TestButton" runat="server" Text="Save &amp; Test Config" OnClick="TestButton_Click" ValidationGroup="SMTP" CausesValidation="true" />&nbsp;
                                            <asp:Button ID="RemoveButton" runat="server" Text="Remove Config" OnClick="RemoveButton_Click" CausesValidation="false" OnClientClick="return confirm('If your email server settings are removed, no email notifications will be sent. Are you sure to remove email settings?')"/>
                                            <asp:PlaceHolder ID="TestPanel" runat="server" Visible="false">
                                                <asp:Localize ID="TestHelpText" runat="server" Text="A test message will be sent to confirm the new server configuration.  Provide the address where the message should be sent."></asp:Localize><br /><br />
                                                <asp:Label ID="TestSendToLabel" runat="server" Text="Send To: " SkinID="FieldHeader" AssociatedControlID="TestSendTo"></asp:Label>
                                                <asp:TextBox ID="TestSendTo" runat="server" Width="200px" MaxLength="200" ValidationGroup="SMTP"></asp:TextBox>
                                                <cb:EmailAddressValidator ID="TestSendToEmailAddressValidator" runat="server" ControlToValidate="TestSendTo" ValidationGroup="SMTP" Required="true" ErrorMessage="Send to email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>
                                                <br /><br />
                                                <asp:Button ID="SendTestButton" runat="server" Text="Finish" OnClick="SendTestButton_Click" ValidationGroup="SMTP" />&nbsp;
                                                <asp:Button ID="CancelTestButton" runat="server" Text="Cancel" OnClick="CancelTestButton_Click" CausesValidation="false" />
                                            </asp:PlaceHolder>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>