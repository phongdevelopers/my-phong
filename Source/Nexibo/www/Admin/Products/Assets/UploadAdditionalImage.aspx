<%@ Page Language="C#" MasterPageFile="../Product.master" CodeFile="UploadAdditionalImage.aspx.cs" Inherits="Admin_Products_Assets_UploadAdditionalImage" Title="Upload Image"  EnableViewState="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script language="javascript">
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
	        //SET FILE NAME
	        var ctlName = document.getElementById("<%= BaseFileName.ClientID %>");
	        if ((ctlName.value.length == 0) || (ctlName.value == lastName)) {
	            fileName = fileName.replace(/ /g, "_");
    	        ctlName.value = fileName;
    	        lastName = fileName;
    	    }
        }
    }
</script>

<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Upload Image for {0}"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
	<tr>
        <td>
            <table class="inputForm">
                <tr>
                    <td colspan="2">
                        <asp:Localize ID="UploadHelpText" runat="server" Text="Upload your additional photo for this product.  If desired, we can resize the image to your specifications."></asp:Localize>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="ProductNameLabel" runat="server" Text="Product:"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="ProductName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr id="trSku" runat="server">
                    <th class="rowHeader">
                        <asp:Label ID="SkuLabel" runat="server" Text="Sku:"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="Sku" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Localize ID="ValidFilesLabel" runat="server" Text="Valid Files:" EnableViewState="false"></asp:Localize>
                    </th>
                    <td>
                        <asp:Literal ID="ValidFiles" runat="server" EnableViewState="false"></asp:Literal>
                        <asp:PlaceHolder ID="phValidFiles" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="UploadedFileLabel" runat="server" Text="Additional Image:" AssociatedControlID="UploadedFile"></asp:Label>
                    </th>
                    <td>
                        <asp:FileUpload ID="UploadedFile" runat="server" CssClass="fileUpload" Size="50" OnBlur="setName()" />
                        <asp:Localize ID="FileDataMaxSize" runat="server" Text="({0}KB max)" EnableViewState="false"></asp:Localize>
                        <asp:RequiredFieldValidator ID="UploadedFileRequired" runat="server" ControlToValidate="UploadedFile"
                            Text="*" ErrorMessage="You must specify the photo to upload."></asp:RequiredFieldValidator>
                        <asp:PlaceHolder ID="phInvalidFile" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="BaseFileNameLabel" runat="server" Text="Save as:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="BaseFileName" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="BaseFileNameValidator" runat="server" ControlToValidate="BaseFileName"
                            Text="*" ErrorMessage="You must enter the name to save the file as."></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="FileNameValidator" runat="server" ControlToValidate="BaseFileName"
                            ErrorMessage="Invalid save as name. A valid name can only contain Uppercase (A - Z), Lowercase (a - z), Numbers (0 - 9), and Symbols (underscore, minus). It may not contain path information." ValidationExpression="^[A-Za-z0-9_\- ]+(\.[A-Za-z0-9_]+)*$">*</asp:RegularExpressionValidator>
                        <asp:PlaceHolder ID="phInvalidFileName" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" valign="top">
                        <asp:Label ID="ResizeLabel" runat="server" Text="Resize Option:" AssociatedControlID="Resize" ToolTip="Indicates whether the image should be resized on upload."></asp:Label>
                    </th>
                    <td valign="top">
                        <table cellpadding="2">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="Resize" runat="server" Text="Resize Uploaded Image" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="CustomSizeLabel" runat="server" Text="New size: "></asp:Label><asp:TextBox ID="CustomWidth" runat="server" Width="25px" MaxLength="4">&nbsp;</asp:TextBox>&nbsp;w&nbsp;&nbsp;<asp:TextBox ID="CustomHeight" runat="server" Width="25px" MaxLength="4">&nbsp;</asp:TextBox>&nbsp;h
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="QualityLabel" runat="server" Text="Quality:" AssociatedControlID="Quality"></asp:Label>
                                    <asp:TextBox ID="Quality" runat="server" MaxLength="3" Width="40px" Text="100"></asp:TextBox>%
                                    <asp:RangeValidator ID="QualityValidator" runat="server" ControlToValidate="Quality"
                                        Text="*" ErrorMessage="Quality must be a number between 1 and 100." 
                                        MinimumValue="1" MaximumValue="100" Type="Integer"></asp:RangeValidator>
                                    <asp:Localize ID="QualityHelpText" runat="server" Text="(applies to JPG only)"></asp:Localize>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="MaintainAspectRatio" runat="server" Text="Maintain Aspect Ratio" Checked="true" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                        <br />
                        <asp:Button ID="UploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" />
                        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</asp:Content>