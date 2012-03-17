<%@ Page Language="C#" MasterPageFile="../../Admin.master" AutoEventWireup="true" CodeFile="AddTemplate.aspx.cs" Inherits="Admin_Store_EmailTemplates_AddTemplate" Title="Add Email Template" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Add Email Template"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top">
                <ComponentArt:TabStrip ID="EmailTemplateTabs" runat="server" MultiPageId="EmailTemplatePages" SkinID="HorizontalTab">
                    <Tabs>
                        <ComponentArt:TabStripTab ID="MessageTab" Text="Message Content"></ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TriggerTab" Text="Event Triggers"></ComponentArt:TabStripTab>
                    </Tabs>
                </ComponentArt:TabStrip>
                <ComponentArt:MultiPage ID="EmailTemplatePages" CssClass="hTab_MultiPage" runat="server">
                    <ComponentArt:PageView ID="DeliveryPage" runat="server">
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                        <table class="inputForm" cellpadding="3" width="100%">
                            <tr>
                                <th class="rowHeader" width="130px">
                                    <cb:ToolTipLabel ID="NameLabel" runat="Server" Text="Template Name:" AssociatedControlID="Name" ToolTip="Enter the name of the email template. This is not shown to customers and is for your reference only."></cb:ToolTipLabel>
                                </th>
                                <td>
                                    <asp:TextBox ID="Name" runat="Server" MaxLength="100" Width="250px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="NameValidator" runat="server" Display="Static" 
                                        ErrorMessage="Template name is required." ControlToValidate="Name" Text="*" />
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="ToAddressLabel" runat="Server" Text="To:" AssociatedControlID="ToAddress" ToolTip="Enter the recipient(s) for the email message. You can enter multiple addresses separated by a comma."></cb:ToolTipLabel>
                                </th>
                                <td>
                                    <asp:TextBox ID="ToAddress" runat="Server" MaxLength="250" Width="250px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ToAddressValidator" runat="server" ErrorMessage="To address is required." ControlToValidate="ToAddress" Text="*"></asp:RequiredFieldValidator>
                                    <asp:Label ID="ToAddressHintText" runat="server" Text="<i>Hint: Some event triggers support the email aliases <b>customer</b> and/or <b>vendor</b>.</i>"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="FromAddressLabel" runat="Server" Text="From:" AssociatedControlID="FromAddress" ToolTip="Specify the email address the message will be sent from."></cb:ToolTipLabel>
                                </th>
                                <td>
                                    <asp:TextBox ID="FromAddress" runat="Server" MaxLength="250" Width="250px"></asp:TextBox>                            
                                    <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="FromAddress" Required="true" ErrorMessage="From email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator><br />
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="CCAddressLabel" runat="Server" Text="CC:" AssociatedControlID="CCAddress" ToolTip="Additional recipient(s) for the email message.  Addresses are visible to all recipients.  You cannot use email aliases 'customer' and/or 'vendor'."></cb:ToolTipLabel>
                                </th>
                                <td>
                                    <asp:TextBox ID="CCAddress" runat="Server" MaxLength="250" Width="250px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="BCCAddressLabel" runat="Server" Text="BCC:" AssociatedControlID="BCCAddress" ToolTip="Additional recipient(s) for the email message.  Addresses are not visible to any recipients.  You cannot use email aliases 'customer' and/or 'vendor'."></cb:ToolTipLabel>
                                </th>
                                <td>
                                    <asp:TextBox ID="BCCAddress" runat="Server" MaxLength="250" Width="250px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:Label ID="BodyInstructionText" runat="Server" Text="Enter the subject and message body below. Both subject and body use the NVelocity template engine, providing scripting capability and dynamic variable support. For details and examples on how to leverage NVelocity scripting, view the <a href='help.aspx' target='_blank'>help</a> (new window)."></asp:Label><br />
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="SubjectLabel" runat="Server" Text="Subject:" AssociatedControlID="Subject" ToolTip="Subject of the message" />
                                </th>
                                <td>
                                    <asp:TextBox ID="Subject" runat="Server" Columns="60" MaxLength="250"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="SubjectValidator" runat="server" ErrorMessage="Subject is required." ControlToValidate="Subject" Text="*"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="MailFormatLabel" runat="Server" Text="Message Format:" AssociatedControlID="MailFormat" ToolTip="Templates can be created for either HTML or text only email."></cb:ToolTipLabel>
                                </th>
                                <td>
                                    <asp:DropDownList ID="MailFormat" runat="server">
                                        <asp:ListItem Text="HTML"></asp:ListItem>
                                        <asp:ListItem Text="Text"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader" valign="top">
                                    <cb:ToolTipLabel ID="MessageLabel" runat="Server" Text="Message:" AssociatedControlID="Message" ToolTip="Content of the message" /><br />
                                    <asp:ImageButton ID="MessageHtml" runat="server" SkinID="HtmlIcon" />
                                </th>
                                <td>
                                    <asp:TextBox ID="Message" runat="Server" TextMode="multiLine" Rows="15" Width="95%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="MessageValidator" runat="server" ErrorMessage="Message content is required." ControlToValidate="Message" Text="*" Display="Static"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
							        <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false" OnClick="CancelButton_Click" />
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView><ComponentArt:PageView ID="TriggerPage" runat="server">
                        <div class="pageContent">
                            <asp:Label ID="TriggersInstructionText" runat="Server" Text="Optional: Place a check next to the events that cause this message to be sent automatically.  Depending on the events chosen, you may have access to email aliases to use with the to address and/or you may gain access to dynamic data through additional nVelocity variables.  Note that all email templates always have access to the $store variable and any template can be used to manually generate messages in the merchant admin."></asp:Label>
                        </div>
                        <asp:GridView ID="TriggerGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="EventId" 
                            AllowPaging="false" AllowSorting="false" SkinID="PagedList" Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Select" >
                                    <HeaderStyle horizontalalign="Center" />
                                    <ItemStyle horizontalalign="Center" Width="80px" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="Selected" runat="server"></asp:CheckBox>
                                   </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Event" DataField="Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Email Aliases" DataField="EmailAliases">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NVelocity Variables" DataField="NVelocityVariables">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>   
                    </ComponentArt:PageView>
                </ComponentArt:MultiPage>
            </td>
        </tr>
    </table>
</asp:Content>
