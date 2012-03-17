<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="DigitalGoodFiles.aspx.cs" Inherits="Admin_DigitalGoods_DigitalGoodFiles" Title="View Files in Digital Goods Folder" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<ajax:UpdatePanel ID="PageAjax" runat="server">
    <Triggers>
        <ajax:PostBackTrigger ControlID="UploadButton" />
    </Triggers>
    <ContentTemplate>    
    <script type="text/javascript" language="javascript">
        var lastName1="";var lastName2="";function setNames(){var a=document.getElementById("<%= UploadFile.ClientID %>").value;if(a.length>0){var b;var c=a.lastIndexOf("\\");if(c<0){c=a.lastIndexOf("/");if(c<0){b=a}else{b=a.substring(c+1)}}else{b=a.substring(c+1)}var f=document.getElementById("<%= UploadFileName.ClientID %>");if((f.value.length==0)||(f.value==lastName2)){f.value=b;lastName2=b}}}
        function showUploadDialog(){$get("<%= UploadDialog.ClientID %>").style.display='block';}
        function hideUploadDialog(){$get("<%= UploadDialog.ClientID %>").style.display='none';}
        function ValidateFolderLength(source, arguments) { return arguments.Value.length <= 240;}
    </script>
    <div class="pageHeader">
        <div class="caption">
	        <h1><asp:Localize ID="Caption" runat="server" Text="View Files in Digital Goods Folder" EnableViewState="false"></asp:Localize></h1>
            <div class="links">
                <asp:LinkButton ID="AddFolderLink" runat="server" Text="Create Folder" EnableViewState="false"></asp:LinkButton>
                <asp:HyperLink ID="UploadFileLink" runat="server" Text="Upload File" NavigateUrl="javascript:showUploadDialog()" EnableViewState="false"></asp:HyperLink>
                <asp:HyperLink ID="StandardView" runat="server" Text="Standard View" NavigateUrl="DigitalGoods.aspx" EnableViewState="false"></asp:HyperLink>
            </div>
        </div>
    </div>
    <div class="searchPanel" style="padding:6px;">
        <asp:Label ID="FilePathLabel" runat="server" Text="Path:" SkinID="FieldHeader" AssociatedControlID="FilePath"></asp:Label>
        <asp:Literal ID="FilePath" runat="server"></asp:Literal>
        <asp:Panel ID="UploadDialog" runat="server" style="display:none">
            <table class="inputForm">
                <tr>
                    <td colspan="2">
                        You can upload a file with a maximum size of <asp:Literal ID="UploadMaxSize" runat="server" Text="{0}kB" EnableViewState="false"></asp:Literal>.  For larger or multiple files you may wish to use FTP instead.
                        <asp:ValidationSummary ID="UploadValidationSummmary" runat="server" ValidationGroup="Upload" />
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" style="white-space:nowrap">
                        <cb:ToolTipLabel ID="UploadFileTypesLabel" runat="server" Text="Valid Files:" AssociatedControlID="UploadFileTypes" ToolTip="A list of file extensions that are valid for uploaded files."></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:Literal ID="UploadFileTypes" runat="server" EnableViewState="false"></asp:Literal>
                        <asp:PlaceHolder ID="phUploadFileTypes" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" valign="top" style="white-space:nowrap">
                        <cb:ToolTipLabel ID="UploadFileLabel" runat="server" Text="Select File:" AssociatedControlID="UploadFile" ToolTip="Select the file to upload to the server."></cb:ToolTipLabel>
                    </th>
                    <td valign="top">
                        <asp:FileUpload ID="UploadFile" runat="server" Width="400px" OnBlur="setNames()"/>
                        <asp:RequiredFieldValidator ID="UploadFileRequired" runat="server" ControlToValidate="UploadFile"
                            Display="Static" ErrorMessage="You must select a file to upload." Text="*" ValidationGroup="Upload"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <cb:ToolTipLabel ID="UploadFileNameLabel" runat="server" Text="File Name:" AssociatedControlID="UploadFileName" ToolTip="Specify the name of the file to save your uploaded data to.  If you do not specify a name, the name of the uploaded file will be used."></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:TextBox ID="UploadFileName" runat="server" MaxLength="100" Width="200px" ValidationGroup="Upload"></asp:TextBox> (optional)
                        <asp:RegularExpressionValidator ID="FileNameValidator" runat="server" ControlToValidate="UploadFileName" ValidationGroup="Upload"
                            ErrorMessage="File name is not valid.  A valid name can be like 'test.pdf' or can include a relative path like 'test\test.pdf'.  It may only contain Uppercase (A - Z), Lowercase (a - z), Numbers (0 - 9), and Symbols (underscore, minus)." ValidationExpression="^([A-Za-z0-9_\- ]+\\)*([A-Za-z0-9_\- ]+)(\.[A-Za-z0-9_]+)*$">*</asp:RegularExpressionValidator></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:CheckBox ID="UploadOverwrite" runat="server" Text="Overwrite if the target file exists" />
                        <asp:PlaceHolder ID="phUploadOverwrite" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <br />
                        <asp:Button ID="UploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" ValidationGroup="Upload" />
                        <asp:Button ID="CancelUploadButton" runat="server" Text="Cancel" OnClientClick="hideUploadDialog();return false;" CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <asp:UpdatePanel ID="FileAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:GridView ID="FileGrid" runat="server" AllowPaging="false" AutoGenerateColumns="False" SkinID="PagedList" 
                Width="100%" OnRowCommand="FileGrid_RowCommand" OnRowDataBound="FileGrid_RowDataBound"
                EnableViewState="false">
                <Columns>
                    <asp:TemplateField HeaderText="File">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="200px" />
                        <ItemTemplate>
                            <asp:PlaceHolder ID="DirectoryName" runat="server" Visible='<%#Eval("IsDirectory")%>'>
                                <asp:Image ID="DirIcon" runat="server" SkinID="CategoryIcon" />
                                <asp:HyperLink ID="DirLink" runat="server" NavigateUrl='<%#Eval("PathQueryString", "DigitalGoodFiles.aspx{0}") %>'><%# Eval("FileName") %></asp:HyperLink>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="FileName" runat="server" Visible='<%#Eval("IsFile")%>'>
                                <%# Eval("FileName") %>
                            </asp:PlaceHolder>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Size">
                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                        <ItemTemplate>
                            <%# Eval("FileSize") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Associated Digital Good(s)">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Repeater ID="DigitalGoodsList" runat="server">
                                <ItemTemplate>
                                    <a href="EditDigitalGood.aspx?DigitalGoodId=<%#Eval("DigitalGoodId")%>"><%#Eval("Name")%></a>
                                </ItemTemplate>
                                <SeparatorTemplate>, </SeparatorTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Right" Width="110px" />
                        <ItemTemplate>
                            <asp:PlaceHolder ID="DirLinks" runat="server" Visible='<%#Eval("IsChildDirectory")%>'>
                                <asp:ImageButton ID="DeleteDirLink" runat="server" CommandName="DeleteDir" CommandArgument='<%# Eval("FileName") %>' AlternateText="Delete" SkinID="DeleteIcon" EnableViewState="false" OnClientClick='<%# Eval("FileName", "return confirm(\"WARNING: Deleting this folder will also delete any folders and files it contains.  Any associated digital goods will not not removed.\\n\\nDelete {0}?\");") %>' />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="FileLinks" runat="server" Visible='<%#Eval("IsFile")%>'>
                                <asp:HyperLink ID="DownloadLink" runat="server" NavigateUrl='<%#GetDownloadLink(Eval("FileName").ToString())%>' EnableViewState="false"><asp:Image ID="DownloadIcon" runat="server" SkinID="DownloadIcon" AlternateText="Download" ToolTip="Download" EnableViewState="false" /></asp:HyperLink>
                                <asp:ImageButton ID="AddLink" runat="server" CommandName="AddDigitalGood" CommandArgument='<%# Eval("FileName") %>' AlternateText="Add Digital Good" SkinID="AddIcon" EnableViewState="false" />
                                <asp:ImageButton ID="RenameLink" runat="server" CommandName="RenameFile" CommandArgument='<%# Eval("FileName") %>' AlternateText="Rename" SkinID="MoveIcon" EnableViewState="false" />
                                <asp:ImageButton ID="DeleteLink" runat="server" CommandName="DeleteFile" CommandArgument='<%# Eval("FileName") %>' AlternateText="Delete" SkinID="DeleteIcon" EnableViewState="false" />
                            </asp:PlaceHolder>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Localize ID="EmptyDataText" runat="server" Text="There are no files uploaded to the digital goods folder of your store." EnableViewState="false"></asp:Localize>
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:Panel ID="AddDialog" runat="server" Style="display:none;width:550px" CssClass="modalPopup">
                <asp:Panel ID="AddDialogHeader" runat="server" CssClass="modalPopupHeader">
                    Add Digital Good
                </asp:Panel>
                <div style="padding-top:5px;">
                    <table class="inputForm" cellpadding="3">
                        <tr>
                            <td colspan="2">
                                Create a digital good that can be associated with your products.  If you need to set additional options, click save and edit.
                                <asp:ValidationSummary ID="AddDigitalGoodValidationSummary" runat="server" ValidationGroup="Add" />
                            </td>
                        </tr>   
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="ServerFileNameLabel" runat="server" Text="File:" AssociatedControlID="ServerFileName" ToolTip="The physical file on the server being configured with digital delivery options." />
                            </th>
                            <td>
                                <asp:Literal ID="ServerFileName" runat="server" Text=""></asp:Literal> 
                                (<asp:Literal ID="ServerFileSize" runat="server" Text=""></asp:Literal>)
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Display Name:" AssociatedControlID="Name" ToolTip="This is the name that will be displayed in the merchant admin and in the download link on customer invoices." />
                            </th>
                            <td>
                                <asp:TextBox ID="Name" runat="server" Text="" MaxLength="100" Width="300px" ValidationGroup="Add" />
                                <asp:RequiredFieldValidator ID="NameRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="Name" ValidationGroup="Add" Text="*" ErrorMessage="Name is required." ></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="NameFormatValidator" runat="server" Display="Dynamic" ControlToValidate="Name" ValidationGroup="Add" Text="*" ErrorMessage="You may not use < or > characters in the name." ValidationExpression="[^<>]*"></asp:RegularExpressionValidator>
                                <asp:PlaceHolder ID="phUniqueName" runat="server"></asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="DownloadNameLabel" runat="server" Text="Download Name:" AssociatedControlID="DownloadName" ToolTip="The file name to send as the default 'Save As' name when the digital good is downloaded." />
                            </th>
                            <td>
                                <asp:TextBox ID="DownloadName" runat="server" Text="" MaxLength="100" Width="300px" />
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
                                <cb:ToolTipLabel ID="MaxDownloadsLabel" runat="server" Text="Max Downloads:" AssociatedControlID="MaxDownloads" ToolTip="The maximum number of times a digital good can be downloaded after it is purchased.  Leave blank for no limit." />
                            </th>
                            <td>
                                <asp:TextBox ID="MaxDownloads" runat="Server" Width="20px" MaxLength="3"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="ActivationTimeoutLabel" runat="server" Text="Absolute&nbsp;Timeout:" ToolTip="After the order is placed, the amount of time after which the download expires." />
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
                                <cb:ToolTipLabel ID="DownloadTimeoutLabel" runat="server" Text="Relative&nbsp;Timeout:" ToolTip="After the first download, the amount of time after which the download expires." />
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
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ValidationGroup="Add" />
                                <asp:Button ID="AddAndEditButton" runat="server" Text="Save and Edit" OnClick="AddButton_Click" ValidationGroup="Add" />
                                <asp:Button ID="CancelAddButton" runat="server" Text="Cancel" CausesValidation="false" /><br />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <asp:Panel ID="DeleteDialog" runat="server" Style="display:none;width:500px" CssClass="modalPopup">
                <asp:Panel ID="DeleteDialogHeader" runat="server" CssClass="modalPopupHeader">
                    Delete File
                </asp:Panel>
                <div style="padding-top:5px;">
                    <table class="inputForm" cellpadding="3">
                        <tr>
                            <td colspan="2">
                                Are you sure you want to delete this file from the server? <asp:Localize ID="DeleteGoodsMessage" runat="server" Text="It will not delete any associated digital goods unless indicated."></asp:Localize>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="DeleteFilenameLabel" runat="server" Text="File:" AssociatedControlID="ServerFileName" ToolTip="The physical file being deleted." />
                            </th>
                            <td>
                                <asp:Literal ID="DeleteFileName" runat="server" Text=""></asp:Literal> 
                                (<asp:Literal ID="DeleteFileSize" runat="server" Text=""></asp:Literal>)
                            </td>
                        </tr>
                        <tr id="trDeleteGoods" runat="server">
                            <td>&nbsp;</td>
                            <td>
                                This file is associated with the following digital goods:<br />
                                <asp:Repeater ID="DeleteDigitalGoodsList" runat="server">
                                    <HeaderTemplate><ul></HeaderTemplate>
                                    <ItemTemplate>
                                        <li><%#Eval("Name")%></li>
                                    </ItemTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                </asp:Repeater>
                                <asp:CheckBox ID="DeleteDigitalGoodsWithFile" runat="server" Text="Also Delete Associated Digital Good(s)" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <br />
                                <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" />
                                <asp:Button ID="CancelDeleteButton" runat="server" Text="Cancel" CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <asp:Panel ID="RenameDialog" runat="server" Style="display:none;width:500px" CssClass="modalPopup">
                <asp:Panel ID="RenameDialogHeader" runat="server" CssClass="modalPopupHeader">
                    <asp:Localize ID="RenameDialogCaption" runat="server" EnableViewState="false" Text="Move or Rename {0}"></asp:Localize>
                </asp:Panel>
                <div style="padding-top:5px;">
                    <table class="inputForm" cellpadding="3">
                        <tr>
                            <td colspan="2">
                                Enter the new name for this file. <asp:Localize ID="RenameGoodsMessage" runat="server" Text="This will also update the associated digital goods unless otherwise indicated."></asp:Localize>
                                <asp:ValidationSummary ID="RenameValidation" runat="server" ValidationGroup="Rename" />
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader" style="white-space:nowrap">
                                <cb:ToolTipLabel ID="RenameFileExtensionsLabel" runat="server" Text="Valid Files:" AssociatedControlID="RenameFileExtensions" ToolTip="A list of file extensions that can are valid for updated filename."></cb:ToolTipLabel>
                            </th>
                            <td>
                                <asp:Literal ID="RenameFileExtensions" runat="server" EnableViewState="false"></asp:Literal>
                                <asp:PlaceHolder ID="phRenameFileExtensions" runat="server"></asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="RenameFileNameLabel" runat="server" Text="Filename:" AssociatedControlID="ServerFileName" ToolTip="The physical file being renamed." />
                            </th>
                            <td>
                                <asp:Literal ID="RenameFileName" runat="server" Text=""></asp:Literal> 
                                (<asp:Literal ID="RenameFileSize" runat="server" Text=""></asp:Literal>)
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="RenameNewFolderLabel" runat="server" Text="Folder:" AssociatedControlID="RenameFileNewFolder" ToolTip="The desired folder for the physical file being renamed." />
                            </th>
                            <td>
                                <asp:DropDownList ID="RenameFileNewFolder" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <cb:ToolTipLabel ID="RenameNewFilenameLabel" runat="server" Text="Filename:" AssociatedControlID="RenameNewFilename" ToolTip="The physical file being renamed." />
                            </th>
                            <td>
                                <asp:TextBox ID="RenameNewFilename" runat="server" MaxLength="100" Width="200px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RenameNewFilenameRequiredValidator" runat="server" ControlToValidate="RenameNewFilename" ValidationGroup="Rename"
                                    Text="*" ErrorMessage="New filename is required."></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RenameNewFilenameFormatValidator" runat="server" ControlToValidate="RenameNewFilename" ValidationGroup="Rename"
                                    ErrorMessage="File name is not valid.  The name may only contain Uppercase (A - Z), Lowercase (a - z), Numbers (0 - 9), and Symbols (underscore, minus).  It cannot include path information." ValidationExpression="^([A-Za-z0-9_\- ]+)(\.[A-Za-z0-9_]+)*$">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <asp:CheckBox ID="RenameOverwrite" runat="server" Text="Allow overwrite if the target file exists" />
                                <asp:PlaceHolder ID="phRenameOverwrite" runat="server"></asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr id="trRenameGoods" runat="server">
                            <td>&nbsp;</td>
                            <td>
                                This file is associated with the following digital goods:<br />
                                <asp:Repeater ID="RenameDigitalGoodsList" runat="server">
                                    <HeaderTemplate><ul></HeaderTemplate>
                                    <ItemTemplate>
                                        <li><%#Eval("Name")%></li>
                                    </ItemTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                </asp:Repeater>
                                <asp:CheckBox ID="UpdateDigitalGoodsOnRename" runat="server" Text="Update these good(s) to point to the renamed file" Checked="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <br />
                                <asp:Button ID="RenameButton" runat="server" Text="Save" OnClick="RenameButton_Click" ValidationGroup="Rename" />
                                <asp:Button ID="CancelRenameButton" runat="server" Text="Cancel" CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <asp:HiddenField ID="DummyAddTarget" runat="server" />
            <asp:HiddenField ID="DummyDeleteTarget" runat="server" />
            <asp:HiddenField ID="DummyRenameTarget" runat="server" />
            <ajax:ModalPopupExtender ID="AddPopup" runat="server" 
                TargetControlID="DummyAddTarget"
                PopupControlID="AddDialog" 
                BackgroundCssClass="modalBackground"                         
                CancelControlID="CancelAddButton" 
                DropShadow="true"
                PopupDragHandleControlID="AddDialogHeader" />
            <ajax:ModalPopupExtender ID="DeletePopup" runat="server" 
                TargetControlID="DummyDeleteTarget"
                PopupControlID="DeleteDialog" 
                BackgroundCssClass="modalBackground"                         
                CancelControlID="CancelDeleteButton" 
                DropShadow="true"
                PopupDragHandleControlID="DeleteDialogHeader" />
            <ajax:ModalPopupExtender ID="RenamePopup" runat="server" 
                TargetControlID="DummyRenameTarget"
                PopupControlID="RenameDialog" 
                BackgroundCssClass="modalBackground"                         
                CancelControlID="CancelRenameButton" 
                DropShadow="true"
                PopupDragHandleControlID="RenameDialogHeader" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="NewFolderDialog" runat="server" Style="display:none;width:400px" CssClass="modalPopup" DefaultButton="NewFolderOkButton">
        <asp:Panel ID="NewFolderDialogHeader" runat="server" CssClass="modalPopupHeader">
            New Folder
        </asp:Panel>
        <div style="padding-top:5px;">
            <div style="padding:4px;">
                <asp:Localize ID="NewFolderInstructionText" runat="server">
                    New folder name may only contain letters A through Z and numbers 0 through 9.  It can also contain the symbols space, underscore, hyphen, and period if used between words.  It cannot include path information.
                </asp:Localize>
            </div>
            <table class="inputForm" cellpadding="3">
                <tr>
                    <th class="rowHeader" valign="top">
                        <asp:Label ID="NewFolderNameLabel" runat="server" Text="Name" AssociatedControlID="NewFolderName"></asp:Label>
                    </th>
                    <td valign="top">
                        <asp:TextBox ID="NewFolderName" runat="server" Width="200px"></asp:TextBox><br />
                        <asp:RequiredFieldValidator ID="NewFolderNameRequired" runat="server" Display="Dynamic" Text="Name is required." ControlToValidate="NewFolderName" ValidationGroup="NewFolder"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="NewFolderNameValidator" runat="server" Display="Dynamic" ControlToValidate="NewFolderName" ValidationGroup="NewFolder" 
                            Text="Name is invalid / not allowed." ValidationExpression="^\s*([A-Za-z0-9]+)([_\- \.][A-Za-z0-9]+)*\s*$"></asp:RegularExpressionValidator>
                        <asp:CustomValidator ID="FolderNameLengthValidator" runat="server" Display="Dynamic" ErrorMessage="Folder name is too long." Text="Folder name is too long." ControlToValidate="NewFolderName" ValidationGroup="NewFolder" ClientValidationFunction="ValidateFolderLength" OnServerValidate="ValidateFolderLength" ></asp:CustomValidator>
                        <asp:CustomValidator ID="DuplidateNameValidator" runat="server" Display="Dynamic" Text="A folder with the same name already exists." ControlToValidate="NewFolderName" ValidationGroup="NewFolder" OnServerValidate="ValidateDuplicateName"></asp:CustomValidator>
                    </td>
                    <td valign="top" style="white-space:nowrap">
                        <asp:Button ID="NewFolderOkButton" runat="server" Text="OK" ValidationGroup="NewFolder" OnClick="NewFolderOkButton_Click"/>
                        <asp:Button ID="NewFolderCancelButton" runat="server" Text="Cancel" CausesValidation="false" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td colspan="2">
                        <table cellpadding="3" width="80%">
                            <tr>
                                <th scope="col" align="left">
                                    <asp:Label ID="NewFolderValidExampleHeader" runat="server" Text="Valid Examples"></asp:Label>
                                </th>
                                <th scope="col" align="left">
                                    <asp:Label ID="NewFolderInvalidExampleHeader" runat="server" Text="Invalid Examples"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td style="padding-left:12px;">
                                    <asp:Localize ID="NewFolderValidExamples" runat="server">
                                        FolderName<br />
                                        Folder-Name<br />
                                        Folder Name<br />
                                    </asp:Localize>
                                </td>
                                <td style="padding-left:12px;">
                                    <asp:Localize ID="NewFolderInvalidExamples" runat="server">
                                        ..\FolderName<br />
                                        Folder--Name<br />
                                        Folder&nbsp;&nbsp;&nbsp;&nbsp;Name<br />
                                    </asp:Localize>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <ajax:ModalPopupExtender ID="NewFolderPopupExtender" runat="server" 
        TargetControlID="AddFolderLink"
        PopupControlID="NewFolderDialog" 
        BackgroundCssClass="modalBackground"                         
        CancelControlID="NewFolderCancelButton" 
        DropShadow="true"
        PopupDragHandleControlID="NewFolderDialogHeader" />
    </ContentTemplate>
</ajax:UpdatePanel>        
</asp:Content>