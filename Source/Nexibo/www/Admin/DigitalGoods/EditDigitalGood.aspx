<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="EditDigitalGood.aspx.cs" Inherits="Admin_DigitalGoods_EditDigitalGood" Title="Edit Digital Good" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<ajax:UpdatePanel ID="PageAjax" runat="server">
    <ContentTemplate>
        <div class="pageHeader">
            <div class="caption">
                <h1><asp:Localize ID="Caption" runat="server" Text="Edit Digital Good {0}" EnableViewState="false"></asp:Localize></h1>
            </div>
        </div>
        <asp:Label ID="SavedMessage" runat="server" Text="Digital good saved at {0:t}" EnableViewState="false" Visible="false" SkinID="GoodCondition"></asp:Label>
        <table cellpadding="3" cellspacing="0" class="inputForm" width="100%">
            <tr class="sectionHeader">
                <td colspan="2">
                    BASIC INFO
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Name" ToolTip="The display name of the digital good." />
                </th>
                <td>
                    <asp:TextBox ID="Name" runat="server" Text="" MaxLength="100" Width="300px" />
                    <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Static" ErrorMessage="Name is required." ControlToValidate="Name"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="HtmlValidator" runat="server" ControlToValidate="Name" ValidationExpression="[^<>]*" Text="*" ErrorMessage="You may not use < or > characters in the name."></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" valign="top">
                    <cb:ToolTipLabel ID="ServerFileNameLabel" runat="server" Text="File Name:" AssociatedControlID="ServerFileName" ToolTip="Physical path for the file associated with this digital good.  If an absolute path is not provided, the file is expected to be found in your App_Data/DigitalGoods folder." />
                </th>
                <td>
                    <asp:Literal ID="ServerFileName" runat="server" EnableViewState="false"></asp:Literal>
                    <asp:ImageButton ID="EditServerFileName" runat="server" SkinID="EditIcon" EnableViewState="False" />
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="CurrentFileSizeLabel" runat="server" Text="File Size:" AssociatedControlID="CurrentFileSize" ToolTip="Size of the current file" />
                </th>
                <td>
                    <asp:Label ID="CurrentFileSize" runat="server" EnableViewState="false"></asp:Label>
                    <asp:HyperLink ID="CurrentFileDownloadLink" runat="server" NavigateUrl="Download.ashx?DigitalGoodId={0}"><asp:Image ID="DownloadIcon" runat="server" SkinID="DownloadIcon" AlternateText="Download" ToolTip="Download" /></asp:HyperLink>
                    <asp:Literal ID="MissingDownloadText" runat="server" Text="[file missing or inaccessible]" EnableViewState="false" Visible="false"/>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="FileNameLabel" runat="server" Text="Save As Name:" AssociatedControlID="FileName" ToolTip="The file name to send as the default 'Save As' name when the digital good is downloaded." />
                </th>
                <td>
                    <asp:TextBox ID="FileName" runat="server" Text="" MaxLength="100" Width="300px" />
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="MediaKeyLabel" runat="server" Text="Media Key:" AssociatedControlID="MediaKey" ToolTip="If a key is required to open the download (for example, a password protected zip) enter it here." />
                </th>
                <td>
                    <asp:TextBox ID="MediaKey" runat="server" Text="" MaxLength="100" />
                </td>
            </tr>
            <tr class="sectionHeader">
                <td colspan="2">
                    DOWNLOAD POLICIES
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ActivationModeLabel" runat="server" Text="Activation Mode:" AssociatedControlID="ActivationMode" ToolTip="Indicates when a digital good becomes activated for download on an order." />
                </th>
                <td>
                    <asp:DropDownList ID="ActivationMode" runat="server">
                        <asp:ListItem Text="Activate Manually" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Activate Immediately On Order Placement" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Activate on Full Payment" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ToolTipLabel1" runat="server" Text="Activation Email:" AssociatedControlID="ActivationEmailTemplateList" ToolTip="Select the email template that you want to send when a serial key is assigned." />
                </th>
                <td>
                    <asp:DropDownList ID="ActivationEmailTemplateList" runat="server">
			            <asp:ListItem Value="0" Text="None"></asp:ListItem>
                    </asp:DropDownList>            
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="MaxDownloadsLabel" runat="server" Text="Max Downloads:" AssociatedControlID="MaxDownloads" ToolTip="The maximum number of times a digital good can be downloaded after it is purchased.  Leave blank for no limit." />
                </th>
                <td>
                    <asp:TextBox ID="MaxDownloads" runat="Server" Width="25px" MaxLength="3"></asp:TextBox>
                    <asp:RangeValidator ID="MaxDownloadRangeValidator" runat="server" MinimumValue="1" MaximumValue="127" Text="*"  ErrorMessage="Max downloads value must be from 1 to 127." ControlToValidate="MaxDownloads" Type="integer" ></asp:RangeValidator>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ActivationTimeoutLabel" runat="server" Text="Absolute Timeout:" ToolTip="After the order is placed, the amount of time after which the download expires." />
                </th>
                <td>
                    <asp:TextBox ID="ActivationTimeoutDays" runat="server" Columns="2" MaxLength="3"></asp:TextBox>
                    <asp:Label ID="ActivationTimeoutDaysLabel" runat="server" Text="Days"></asp:Label>&nbsp;
                    <asp:TextBox ID="ActivationTimeoutHours" runat="server" Columns="2" MaxLength="3"></asp:TextBox>
                    <asp:Label ID="ActivationTimeoutHoursLabel" runat="server" Text="Hours"></asp:Label>&nbsp;
                    <asp:TextBox ID="ActivationTimeoutMinutes" runat="server" Columns="2" MaxLength="3"></asp:TextBox>
                    <asp:Label ID="ActivationTimeoutMinutesLabel" runat="server" Text="Minutes"></asp:Label>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="DownloadTimeoutLabel" runat="server" Text="Relative Timeout:" ToolTip="After the first download, the amount of time after which the download expires." />
                </th>
                <td>
                    <asp:TextBox ID="DownloadTimeoutDays" runat="server" Columns="2" MaxLength="3"></asp:TextBox>
                    <asp:Label ID="DownloadTimeoutDaysLabel" runat="server" Text="Days"></asp:Label>&nbsp;
                    <asp:TextBox ID="DownloadTimeoutHours" runat="server" Columns="2" MaxLength="3"></asp:TextBox>
                    <asp:Label ID="DownloadTimeoutHoursLabel" runat="server" Text="Hours"></asp:Label>&nbsp;
                    <asp:TextBox ID="DownloadTimeoutMinutes" runat="server" Columns="2" MaxLength="3"></asp:TextBox>
                    <asp:Label ID="DownloadTimeoutMinutesLabel" runat="server" Text="Minutes"></asp:Label>
                </td>
            </tr>
            <tr class="sectionHeader">
                <td colspan="2">
                    LICENSE AGREEMENT AND README
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="LicenseAgreementLabel" runat="server" Text="License Agreement:" AssociatedControlID="LicenseAgreement" ToolTip="Select the license agreement to associate with this digital good."></asp:Label>
                </th>
                <td>
                    <asp:DropDownList ID="LicenseAgreement" runat="server" AppendDataBoundItems="true" DataTextField="DisplayName" DataValueField="LicenseAgreementId">
                        <asp:ListItem Text=""></asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;<asp:HyperLink ID="ManageAgreements" runat="server" NavigateUrl="Agreements.aspx" Text="manage >"></asp:HyperLink>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="LicenseAgreementModeLabel" runat="server" Text="Agreement Required:" AssociatedControlID="LicenseAgreementMode" ToolTip="Indicate when the customer should be required to accept the license agreement." />
                </th>
                <td>
                    <asp:DropDownList ID="LicenseAgreementMode" runat="server">
                        <asp:ListItem Value="0" Text="Never"></asp:ListItem>
                        <asp:ListItem Value="1" Text="On Add To Basket"></asp:ListItem>
                        <asp:ListItem Value="2" Text="On Download"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Both"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ReadmeLabel" runat="server" Text="Readme:" AssociatedControlID="Readme" ToolTip="Select the readme to associate with this digital good.  Links to the readme will appear in the basket and on the order page." />
                </th>
                <td>
                    <asp:DropDownList ID="Readme" runat="server" AppendDataBoundItems="true" DataTextField="DisplayName" DataValueField="ReadmeId">
                        <asp:ListItem Text=""></asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;<asp:HyperLink ID="ManageReadmes" runat="server" NavigateUrl="Readmes.aspx" Text="manage >"></asp:HyperLink>
                </td>
            </tr>
            <tr class="sectionHeader">
                <td colspan="2">
                    SERIAL KEY POLICY
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="EnableSerialKeysLabel" runat="server" Text="Enable Serial Keys:" AssociatedControlID="EnableSerialKeys" ToolTip="Indicate whether this digital good supports serial keys." />
                </th>
                <td>
                    <asp:CheckBox ID="EnableSerialKeys" runat="server" />
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="KeySourceLabel" runat="server" Text="Key Source:" AssociatedControlID="KeySource" ToolTip="Select the Serial Key Provider." />
                </th>
                <td>
                    <asp:DropDownList ID="KeySource" runat="server">
			            <asp:ListItem Value="0" Text="Manual Entry"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:HyperLink ID="ProviderConfigLink" runat="server" NavigateUrl="http://www.google.com">Configure</asp:HyperLink>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="FulfillmentModeLabel" runat="server" Text="Key Fulfillment:" AssociatedControlID="FulfillmentMode" ToolTip="Indicate when serial keys will be assigned to digital goods in an order." />
                </th>
                <td>
                    <asp:DropDownList ID="FulfillmentMode" runat="server">
                        <asp:ListItem Value="0" Text="Manual"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Auto, When Order Is Placed"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Auto, When Full Payment Is Received"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ToolTipLabel2" runat="server" Text="Key Notification:" AssociatedControlID="FulfillmentEmailTemplateList" ToolTip="Select the email template that you want to use for sending Fulfillment email." />
                </th>
                <td>
                    <asp:DropDownList ID="FulfillmentEmailTemplateList" runat="server">
			            <asp:ListItem Value="0" Text="None"></asp:ListItem>
                    </asp:DropDownList>            
                </td>
            </tr> 
            <tr>
                <td colspan="2">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                </td>
            </tr>   
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="UpdateButton" runat="server" Text="Update" OnClick="UpdateButton_Click" />
                    <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
                    <asp:HyperLink ID="CancelButton" runat="server" Text="Cancel" SkinID="Button" NavigateUrl="DigitalGoods.aspx" />
                </td>
            </tr>
        </table>
        <asp:Panel ID="RenameDialog" runat="server" Style="display:none;width:500px" CssClass="modalPopup">
            <asp:Panel ID="RenameDialogHeader" runat="server" CssClass="modalPopupHeader">
                Update File Name
            </asp:Panel>
            <div style="padding-top:5px;">
                <table class="inputForm" cellpadding="3">
                    <tr>
                        <td colspan="2">
                            <asp:Localize ID="RenameHelpText" runat="server" EnableViewState="false">
                                Enter the updated file name for this digital good.  Updating the file name does not rename or move the file on the server.  If you alter this value, ensure the given file name points to an existing file and uses a valid extension.
                            </asp:Localize>
                            <asp:ValidationSummary ID="RenameValidation" runat="server" ValidationGroup="Rename" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" nowrap>
                            <cb:ToolTipLabel ID="RenameFileExtensionsLabel" runat="server" Text="Valid Extensions:" AssociatedControlID="RenameFileExtensions" ToolTip="A list of file extensions that can are valid for updated file name."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:Literal ID="RenameFileExtensions" runat="server" EnableViewState="false"></asp:Literal>
                            <asp:PlaceHolder ID="phRenameFileExtensions" runat="server"></asp:PlaceHolder>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="RenameFileNameLabel" runat="server" Text="File Name:" AssociatedControlID="RenameFileName" ToolTip="The updated file name." />
                        </th>
                        <td>
                            <asp:TextBox ID="RenameFileName" runat="server" MaxLength="100" Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RenameFileNameRequiredValidator" runat="server" ControlToValidate="RenameFileName" ValidationGroup="Rename"
                                Text="*" ErrorMessage="Updated filename is required."></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RenameFileNameFormatValidator" runat="server" ControlToValidate="RenameFileName" ValidationGroup="Rename"
                                ErrorMessage="File name is not valid.  A valid name can be like 'test.pdf' or can include a relative path like 'test\test.pdf'.  It may only contain Uppercase (A - Z), Lowercase (a - z), Numbers (0 - 9), and Symbols (underscore, minus)." ValidationExpression="^([A-Za-z0-9_\- ]+\\)*([A-Za-z0-9_\- ]+)(\.[A-Za-z0-9_]+)*$">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <br />
                            <asp:Button ID="RenameButton" runat="server" Text="Update" OnClick="RenameButton_Click" ValidationGroup="Rename" />
                            <asp:Button ID="CancelRenameButton" runat="server" Text="Cancel" CausesValidation="false" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <ajax:ModalPopupExtender ID="RenamePopup" runat="server" 
            TargetControlID="EditServerFileName"
            PopupControlID="RenameDialog" 
            BackgroundCssClass="modalBackground"                         
            CancelControlID="CancelRenameButton" 
            DropShadow="true"
            PopupDragHandleControlID="RenameDialogHeader" />
    </ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>

