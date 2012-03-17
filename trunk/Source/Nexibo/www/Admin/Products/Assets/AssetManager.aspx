<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="AssetManager.aspx.cs" Inherits="Admin_Products_Assets_AssetManager" Title="Asset Manager" EnableViewState="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script language="javascript" type="text/javascript">    
    var clientid;
    function fnSetFocus(txtClientId) { clientid=txtClientId; setTimeout("fnFocus()",500); }
    function fnFocus() { eval("document.getElementById('"+clientid+"').focus()"); }
    
    function IsValidFileName(fldId)
    {
        var fldValue = document.getElementById(fldId).value;
        // TRIM THE VALUE
        fldValue = fldValue.replace(/^\s*/, "").replace(/\s*$/, "");
        document.getElementById(fldId).value = fldValue;
        if(fldValue == ""){
            return false;
        }
        return true;
    }
    
    var lastName = "";
    function setName()
    {
        var filePath = document.getElementById("<%= UploadedFile.ClientID %>").value;
        if (filePath.length > 0) {
	        var fileName;
	        var charIndex = filePath.lastIndexOf("\\");
	        if (charIndex < 0) {
		        charIndex = filePath.lastIndexOf("/");
		        if (charIndex < 0) {
			        fileName = filePath;
		        } else {
			        fileName = filePath.substring(charIndex+1);
		        }
	        } else {
		        fileName = filePath.substring(charIndex+1);
	        }
	        //SET DISPLAY NAME
	        var ctlName = document.getElementById("<%= BaseFileName.ClientID %>");
	        if ((ctlName.value.length == 0) || (ctlName.value == lastName)) {
	            fileName = fileName.replace(/ /g, "_");
    	        ctlName.value = fileName;
    	        lastName = fileName;
    	    }
        }
    }
    
    function FilesChecked()
    {
        var count = 0;
        for(i = 0; i< document.forms[0].elements.length; i++){
            var e = document.forms[0].elements[i];
            var name = e.name;
            if ((e.type == 'checkbox') && (name == 'selected') && (e.checked))
            {
                count ++;
            }
        }
        return (count > 0);
    }
</script>
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Asset Manager"></asp:Localize></h1>
	</div>
</div>
<ajax:UpdatePanel ID="HeaderAjax" runat="server" UpdateMode="Always">
    <ContentTemplate>
    <div>
        <table cellpadding="2" cellspacing="0" class="innerLayout">
            <tr runat="server" id="trBrowseImage" visible="False">
		        <th colspan="3" style="vertical-align:top; text-align:left; padding:4px 0px 4px 10px;border-top:solid 1px black;border-bottom:solid 1px black;">
	                <asp:Label ID="BrowseImageCaption" runat="server" Text="Browse for {0}"></asp:Label>
	                <asp:Label ID="ProductName" runat="server" Text=" for {0}"></asp:Label>
	                <asp:Button ID="CancelBrowseButton" runat="server" Text="Cancel" OnClick="CancelBrowseButton_Click" />&nbsp;
                    <asp:Button ID="SelectImageButton" runat="server" Text="Select Image" OnClick="SelectImageButton_Click" Visible="false" />
		        </th>
            </tr>
            <tr>
		        <td colspan="3" style="vertical-align:top; text-align:left; padding:6px 0px 0px 10px;">                    
                    <div style="overflow:auto; width:950px;" >
                        <asp:Label ID="CurrentFolderLabel" runat="server" Text="Current Folder:" SkinID="FieldHeader"></asp:Label>
                        <asp:Localize ID="CurrentFolder" runat="server" Text="\"></asp:Localize>
                    </div>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="VS_CustomState" runat="server" EnableViewState="false" />
    </div>
    </ContentTemplate>
</ajax:UpdatePanel>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td width="235" align="left" style="padding:0px 4px 10px 4px; vertical-align:top;">
            <div class="section">
                <div class="header">
                    <h2 class="browse">
                        <asp:Localize ID="BrowseCaption" runat="server" Text="Folder Contents"></asp:Localize>
                    </h2>
                </div>
            </div>
            <ajax:UpdatePanel ID="FileListAjax" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <Triggers>
                    <ajax:AsyncPostBackTrigger ControlID="NewFolderOKButton" EventName="Click" />
                    <ajax:AsyncPostBackTrigger ControlID="DeleteSelectedButton" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div style="overflow:auto; height:300px; width:221px;" class="contentSection">
                        <table>
                            <asp:Repeater ID="FileListRepeater" runat="server" OnItemCommand="FileListRepeater_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td width="10px">
                                            <asp:PlaceHolder ID="selectBox" runat="server" Visible='<%# (Container.ItemIndex > 0) || (string.IsNullOrEmpty(this.CurrentPath)) %>'>
                                                <input type="checkbox" name="selected" value='<%#Eval("Name")%>' />
                                            </asp:PlaceHolder>
                                        </td>
                                        <td>
		                                    <asp:Image ID="DirectoryIcon" runat="server" Visible='<%#ShowFileIcon(Container.DataItem, FileItemType.Directory)%>' SkinID="CategoryIcon" />
		                                    <asp:Image ID="ImageIcon" runat="server" Visible='<%#ShowFileIcon(Container.DataItem, FileItemType.Image)%>' SkinID="ImageIcon" />
		                                    <asp:Image ID="FileIcon" runat="server" Visible='<%#ShowFileIcon(Container.DataItem, FileItemType.Other)%>' SkinID="WebpageIcon" />
                                        </td>
                                        <td>
		                                    <asp:LinkButton ID="FileItem" runat="server" Text='<%#Eval("Name")%>' CommandName='<%#Eval("FileItemType")%>' CommandArgument='<%#Eval("Name")%>'></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
	                </div>
                    <asp:Button ID="NewFolderButton" runat="server" Text="New Folder" /><asp:Button ID="DeleteSelectedButton" runat="server" Text="Delete Selected" OnClick="DeleteSelectedButton_Click" OnClientClick="if(FilesChecked()){return  confirm('Are you sure you want to delete the selected items?');} else {alert ('Please select at least one folder or file.'); return false;}" />
                    <asp:Label ID="ErrorMessage2" runat="server" EnableViewState="false" Visible="false" SkinID="errorCondition" Text="<br>Invalid name, folder not created."></asp:Label>
                    <asp:Panel ID="NewFolderDialog" runat="server" Style="display: none" CssClass="modalPopup">
                        <asp:Panel ID="NewFolderDialogHeader" runat="server" CssClass="modalPopupHeader">
                            Add Folder
                        </asp:Panel>
                        <div align="center">
                            <br />
	                        <asp:Label ID="NewFolderNameLabel" runat="server" Text="Name: " AssociatedControlID="NewFolderName" SkinID="FieldHeader"></asp:Label>
	                        <asp:TextBox ID="NewFolderName" runat="server" MaxLength="100" ValidationGroup="NewFolderName"></asp:TextBox>
	                        <asp:Button ID="NewFolderOKButton" runat="server" Text="OK" OnClick="NewFolderOKButton_Click" ValidationGroup="NewFolderName" />
	                        <asp:Button ID="NewFolderCancelButton" runat="server" Text="Cancel" CausesValidation="false" /><br />
	                        <asp:RequiredFieldValidator ID="FolderNameValidator" runat="server" Text="Please specify valid folder name." ErrorMessage="Please specify folder name." ValidationGroup="NewFolderName" ControlToValidate="NewFolderName" EnableViewState="false"></asp:RequiredFieldValidator>
	                        <br />
                        </div>
                    </asp:Panel>
                    <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender" runat="server" 
                        TargetControlID="NewFolderButton"
                        PopupControlID="NewFolderDialog" 
                        BackgroundCssClass="modalBackground" 
                        CancelControlID="NewFolderCancelButton" 
                        DropShadow="true"
                        PopupDragHandleControlID="NewFolderDialogHeader" />
                        <br />
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
        <td align="left" valign="top" style="padding:0px 4px 10px 4px;">
            <div class="section">
                <div class="header">
                    <h2 class="filedetails">File Details</h2>
                </div>
            </div>
            <div class="contentSection">
                <ajax:UpdatePanel ID="FileDetailsAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:PlaceHolder runat="server" ID="NoFileSelectedPanel" Visible="false">
                            <div align="center">
                                <p style="width:200px;text-align:justify"><asp:Localize ID="NoFileSelectedMessage" runat="server" Text="Select a file from the directory list to view the details and preview here."></asp:Localize></p>
                            </div>
                        </asp:PlaceHolder>
                        <asp:Panel runat="server" ID="FileDetails" Visible="false">
                            <div align="center">
                                 <div style="overflow:auto; width:400px;" class="contentSection">
                                    <asp:Label ID="FileName" runat="server" SkinID="FieldHeader"></asp:Label> <asp:Label ID="Dimensions" runat="server"></asp:Label> <asp:Literal ID="FileSize" runat="server"></asp:Literal>
                                 </div>
                                &nbsp;<asp:Button ID="CopyButton" runat="server" Text="Copy" />
                                <asp:Button ID="RenameButton" runat="server" Text="Rename" />
                            </div>
                            <asp:Panel ID="RenameDialog" runat="server" Style="display: none" CssClass="modalPopup">
                                <asp:Panel ID="RenameDialogHeader" runat="server" CssClass="modalPopupHeader">
                                    Rename File
                                </asp:Panel>
                                <div style="margin:6px">
                                    <asp:ValidationSummary ID="RenameValidationSummary" runat="server" ValidationGroup="Rename" />
                                    <asp:Label ID="RenameValidFilesLabel" runat="server" Text="Valid Extensions:" SkinID="fieldHeader" EnableViewState="false"></asp:Label>
                                    <asp:Literal ID="RenameValidFiles" runat="server" EnableViewState="false"></asp:Literal>
                                    <asp:PlaceHolder ID="phRenameValidFiles" runat="server"></asp:PlaceHolder><br /><br />
                                    <asp:Label ID="RenameNameLabel" runat="server" Text="Rename To: " AssociatedControlID="RenameName" SkinID="FieldHeader"></asp:Label>
                                    <asp:TextBox ID="RenameName" runat="server" MaxLength="100"></asp:TextBox>
                                    <asp:Button ID="RenameOKButton" runat="server" Text="OK" OnClick="RenameOKButton_Click" />
                                    <asp:Button ID="RenameCancelButton" runat="server" Text="Cancel" /><br /><br />
                                </div>
                            </asp:Panel>
                            <ajaxToolkit:ModalPopupExtender ID="RenamePopup" runat="server" 
                                TargetControlID="RenameButton"
                                PopupControlID="RenameDialog" 
                                BackgroundCssClass="modalBackground" 
                                OkControlID="RenameOKButton"
                                CancelControlID="RenameCancelButton" 
                                DropShadow="true"
                                PopupDragHandleControlID="RenameDialogHeader" />
                            <asp:Panel ID="CopyDialog" runat="server" Style="display: none" CssClass="modalPopup">
                                <asp:Panel ID="CopyDialogHeader" runat="server" CssClass="modalPopupHeader">
                                    Copy File
                                </asp:Panel>
                                <div style="margin:6px">
                                    <asp:ValidationSummary ID="CopyValidationSummary" runat="server" ValidationGroup="Copy" />
                                    <asp:Label ID="CopyValidFilesLabel" runat="server" Text="Valid Extensions:" SkinID="fieldHeader" EnableViewState="false"></asp:Label>
                                    <asp:Literal ID="CopyValidFiles" runat="server" EnableViewState="false"></asp:Literal>
                                    <asp:PlaceHolder ID="phCopyValidFiles" runat="server"></asp:PlaceHolder><br /><br />
                                    <asp:Label ID="CopyNameLabel" runat="server" Text="Copy To: " AssociatedControlID="CopyName" SkinID="FieldHeader"></asp:Label>
                                    <asp:TextBox ID="CopyName" runat="server" MaxLength="100"></asp:TextBox>
                                    <asp:Button ID="CopyOKButton" runat="server" Text="OK" OnClick="CopyOKButton_Click" />
                                    <asp:Button ID="CopyCancelButton" runat="server" Text="Cancel" /><br /><br />
                                </div>
                            </asp:Panel>
                            <ajaxToolkit:ModalPopupExtender ID="CopyPopup" runat="server" 
                                TargetControlID="CopyButton"
                                PopupControlID="CopyDialog" 
                                BackgroundCssClass="modalBackground" 
                                OkControlID="CopyOKButton"
                                CancelControlID="CopyCancelButton" 
                                DropShadow="true"
                                PopupDragHandleControlID="CopyDialogHeader" />
                            <div align="center">
                                <div style="width:400px;height:300px;overflow:auto;border:solid 1px gray;padding:2px;">
                                    <asp:Image ID="ImagePreview" CssClass="image_view" runat="server" />
                                </div><br />
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </div>
        </td>
        <td align="left" valign="top" width="240" style="padding:0px 10px 10px 0px;">
            <div class="section">
                <div class="header">
                    <h2 class="uplode">Upload</h2>
                </div>
            </div>
            <div class="contentSection" style="line-height:24px">
                <div style="margin-bottom:6px;">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                <asp:Label ID="ValidFilesLabel" runat="server" Text="Valid Files:" SkinID="fieldHeader" EnableViewState="false"></asp:Label>
                <asp:Literal ID="ValidFiles" runat="server" EnableViewState="false"></asp:Literal>
                <asp:PlaceHolder ID="phValidFiles" runat="server"></asp:PlaceHolder><br />
                <asp:FileUpload ID="UploadedFile" runat="server" Size="25" OnBlur="setName()" /></div>
                <asp:Label ID="BaseFileNameLabel" runat="server" Text="Save as:" SkinID="fieldHeader"></asp:Label>
                <asp:TextBox ID="BaseFileName" runat="server" MaxLength="100"></asp:TextBox>                
                <asp:RegularExpressionValidator ID="FileNameValidator" runat="server" ControlToValidate="BaseFileName"
                        ErrorMessage="Invalid save as name. A valid name can only contain Uppercase (A - Z), Lowercase (a - z), Numbers (0 - 9), and Symbols (underscore, minus). It may not contain path information." ValidationExpression="^[A-Za-z0-9_\- ]+(\.[A-Za-z0-9_]+)*$">*</asp:RegularExpressionValidator><br />
                <ajax:UpdatePanel ID="ResizeAjax" runat="server">
                    <ContentTemplate>
                        <fieldset style="border: 1px solid #7E94D5; padding:2px;">
                            <legend style="border: 1px solid #7E94D5; color:Black;">Image Options</legend>
                            <asp:RadioButton ID="NoResize" runat="server" GroupName="Resize" Text="Do not resize" AutoPostBack="true" /><br />
                            <asp:RadioButton ID="StandardResize" runat="server" GroupName="Resize" Text="Standard image sizes" AutoPostBack="true" Checked="true" /><br />
                            <asp:PlaceHolder ID="StandardResizePanel" runat="server" EnableViewState="false">
                                <div style="padding-left:25px;">
                                    <asp:CheckBox ID="ResizeIcon" runat="server" Text="Icon ({0}w X {1}h)" Checked="true" /><br />
                                    <asp:CheckBox ID="ResizeThumbnail" runat="server" Text="Thumbnail ({0}w X {1}h)" Checked="true" /><br />
                                    <asp:CheckBox ID="ResizeStandard" runat="server" Text="Standard ({0}w X {1}h)" Checked="true" /><br />
                                    <asp:CheckBox ID="StandardMaintainAspectRatio" runat="server" Text="Maintain Aspect Ratio" Checked="true" /><br />
                                    <asp:Label ID="StandardJpgQualityLabel" runat="server" Text="Quality: " EnableViewState="false" AssociatedControlID="StandardJpgQuality"></asp:Label>
                                    <asp:TextBox ID="StandardJpgQuality" runat="server" Width="30px" Text="100" EnableViewState="false"></asp:TextBox>
                                    <asp:RangeValidator ID="StandardJpgQualityValidator" runat="server" Type="integer" MinimumValue="0" MaximumValue="100" Text="*" ErrorMessage="Please enter a valid value (from 0 to 100) for quality." ControlToValidate="StandardJpgQuality"></asp:RangeValidator>
                                    <asp:Label ID="StandardJpgQualityHelpText" runat="server" Text="% (jpg only)" EnableViewState="false"></asp:Label>
                                </div>
                            </asp:PlaceHolder>
                            <asp:RadioButton ID="CustomResize" runat="server" GroupName="Resize" Text="Custom size" AutoPostBack="true" /><br />
                            <asp:PlaceHolder ID="CustomResizePanel" runat="server" Visible="false">
                                <div style="padding-left:25px;">
                                    <asp:TextBox ID="CustomUploadWidth" runat="server" Columns="1" MaxLength="4">&nbsp;</asp:TextBox>
                                    <asp:RangeValidator ID="CustomUploadWidthValidator1" runat="server" Type="integer" MinimumValue="1" MaximumValue="1200" Text="*" ErrorMessage="Please enter a valid value (from 1 to 1200) for width." ControlToValidate="CustomUploadWidth"></asp:RangeValidator>&nbsp;w&nbsp;&nbsp;
                                    <asp:TextBox ID="CustomUploadHeight" runat="server" Columns="1" MaxLength="4">&nbsp;</asp:TextBox>
                                    <asp:RangeValidator ID="CustomUploadHeightValidator" runat="server" Type="integer" MinimumValue="1" MaximumValue="1200" Text="*" ErrorMessage="Please enter a valid value (from 1 to 1200) for height." ControlToValidate="CustomUploadHeight"></asp:RangeValidator>&nbsp;h<br />
                                    <asp:CheckBox ID="CustomMaintainAspectRatio" runat="server" Text="Maintain Aspect Ratio" Checked="true" /><br />
                                    <asp:Label ID="CustomJpgQualityLabel" runat="server" Text="Quality: " EnableViewState="false" AssociatedControlID="CustomJpgQuality"></asp:Label>
                                    <asp:TextBox ID="CustomJpgQuality" runat="server" Width="30px" Text="100" EnableViewState="false" MaxLength="3"></asp:TextBox>
                                    <asp:RangeValidator ID="CustomJpgQualityValidator" runat="server" Type="integer" MinimumValue="0" MaximumValue="100" Text="*" ErrorMessage="Please enter a valid value (from 0 to 100) for quality." ControlToValidate="CustomJpgQuality"></asp:RangeValidator>
                                    <asp:Label ID="CustomJpgQualityHelpText" runat="server" Text="% (jpg only)" EnableViewState="false"></asp:Label>
                                </div>
                            </asp:PlaceHolder>
                        </fieldset>
                    </ContentTemplate>
                </ajax:UpdatePanel>           
                <asp:Button ID="UploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" />
                <asp:Localize ID="FileDataMaxSize" runat="server" Text="(max file size: {0}KB)" EnableViewState="false"></asp:Localize><br />
                <asp:Label ID="ErrorMessage" runat="server" Visible="false" EnableViewState="false" SkinID="ErrorCondition"></asp:Label>
            </div>
        </td>
    </tr>
</table>
</asp:Content>
