<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="DigitalGoods.aspx.cs" Inherits="Admin_DigitalGoods_DigitalGoods" Title="Manage Digital Goods" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Manage Digital Goods" EnableViewState="false"></asp:Localize></h1>
            <div class="links">
                <asp:HyperLink ID="AddLink" runat="server" Text="Add Digital Good" NavigateUrl="#" EnableViewState="false"></asp:HyperLink>
                <asp:HyperLink ID="FileView" runat="server" Text="View Files" NavigateUrl="DigitalGoodFiles.aspx" EnableViewState="false"></asp:HyperLink>
            </div>
    	</div>
    </div>
    <asp:UpdatePanel ID="GridAjax" runat="server" UpdateMode="Conditional">
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
                        <asp:Localize ID="SearchDGNameLabel" runat="server" Text="Name:" EnableViewState="false"></asp:Localize>
                    </th>
                    <td>
                        <asp:TextBox ID="SearchDGName" runat="server" Width="200px" MaxLength="200"></asp:TextBox>
                    </td>
                    <th class="rowHeader">
                        <asp:Localize ID="SearchDGFileNameLabel" runat="server" Text="File:" EnableViewState="false"></asp:Localize>
                    </th>
                    <td>
                        <asp:TextBox ID="SearchDGFileName" runat="server" Width="200px" MaxLength="200"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
                    </td>
                </tr>
            </table>
            <cb:SortedGridView ID="DigitalGoodGrid" runat="server" AllowPaging="true" AllowSorting="true" PageSize="20"
                AutoGenerateColumns="False" DataKeyNames="DigitalGoodId" DataSourceID="DigitalGoodDs" 
                ShowFooter="False" DefaultSortExpression="Name" SkinID="PagedList" Width="100%" 
                EnableViewState="false" OnRowCommand="DigitalGoodGrid_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <a href="EditDigitalGood.aspx?DigitalGoodId=<%#Eval("DigitalGoodId")%>"><%#Eval("Name")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="File" SortExpression="ServerFileName">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:HyperLink ID="DownloadLink" runat="server" CausesValidation="False" NavigateUrl='<%# Eval("DigitalGoodId", "Download.ashx?DigitalGoodId={0}")%>' EnableViewState="false" Visible='<%#DGFileExists(Container.DataItem) %>'><asp:Image ID="DownloadIcon" runat="server" SkinID="DownloadIcon" AlternateText="Download" ToolTip="Download" EnableViewState="false" /></asp:HyperLink>
                            <asp:Localize ID="DownloadMissingLabel" runat="server" Text="[MISSING] " EnableViewState="false" Visible='<%#!DGFileExists(Container.DataItem) %>'></asp:Localize>
                            <%# Eval("ServerFileName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Products">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <a href="ViewProducts.aspx?DigitalGoodId=<%#Eval("DigitalGoodId")%>"><%#GetProductCount(Container.DataItem)%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Orders">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <a href="ViewOrders.aspx?DigitalGoodId=<%#Eval("DigitalGoodId")%>"><%#GetOrderCount(Container.DataItem)%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <ItemStyle HorizontalAlign="Right" Width="108px" Wrap="false" />
                        <ItemTemplate>
                            <a href="EditDigitalGood.aspx?DigitalGoodId=<%#Eval("DigitalGoodId")%>"><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" ToolTip="Edit" EnableViewState="false" /></a>
                            <asp:LinkButton ID="CopyButton" runat="server" CausesValidation="False" CommandName="Copy" CommandArgument='<%# Eval("DigitalGoodId")%>' EnableViewState="false"><asp:Image ID="CopyIcon" runat="server" SkinID="CopyIcon" AlternateText="Copy" EnableViewState="false" /></asp:LinkButton>
                            <asp:HyperLink ID="DeleteLink" runat="server" CausesValidation="False" NavigateUrl='<%# Eval("DigitalGoodId", "DeleteDigitalGood.aspx?DigitalGoodId={0}")%>' EnableViewState="false"><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" AlternateText="Delete" EnableViewState="false" /></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Localize ID="EmptyDataText" runat="server" Text="No digital goods are defined for your store." EnableViewState="false"></asp:Localize>
                </EmptyDataTemplate>
            </cb:SortedGridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="AddDialog" runat="server" Style="display:none;width:650px" CssClass="modalPopup">
        <script type="text/javascript" language="javascript">
            var lastName1="";var lastName2="";function setNames(){var a=document.getElementById("<%= UploadFile.ClientID %>").value;if(a.length>0){var b;var c=a.lastIndexOf("\\");if(c<0){c=a.lastIndexOf("/");if(c<0){b=a}else{b=a.substring(c+1)}}else{b=a.substring(c+1)}var d;var e=document.getElementById("<%= Name.ClientID %>");if((e.value.length==0)||(e.value==lastName1)){c=b.indexOf(".");if(c<0){d=b}else{d=b.substring(0,c)}e.value=d;lastName1=d}var f=document.getElementById("<%= UploadFileName.ClientID %>");if((f.value.length==0)||(f.value==lastName2)){f.value=b;lastName2=b}}}
        </script>
        <asp:Panel ID="AddDialogHeader" runat="server" CssClass="modalPopupHeader">
            Add Digital Good
        </asp:Panel>
        <div style="padding-top:5px;">
            <table class="inputForm" cellpadding="3">
                <tr>
                    <td colspan="2">
                        You can upload a file with a maximum size of <asp:Literal ID="UploadMaxSize" runat="server" Text="{0}KB" EnableViewState="false"></asp:Literal>.  Also configure the options for the digital good.  If you need to set additional options, click save and edit.
                        <asp:ValidationSummary ID="AddDigitalGoodValidationSummary" runat="server" ValidationGroup="Add" />
                    </td>
                </tr>   
                <tr>
                    <th class="rowHeader" nowrap>
                        <cb:ToolTipLabel ID="UploadFileTypesLabel" runat="server" Text="Valid Files:" AssociatedControlID="UploadFileTypes" ToolTip="A list of file extensions that are valid for uploaded files."></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:Literal ID="UploadFileTypes" runat="server" EnableViewState="false"></asp:Literal>
                        <asp:PlaceHolder ID="phUploadFileTypes" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" valign="top" nowrap>
                        <cb:ToolTipLabel ID="UploadFileLabel" runat="server" Text="Upload:" AssociatedControlID="UploadFile" ToolTip="Select the file to upload to the server."></cb:ToolTipLabel>
                    </th>
                    <td valign="top">
                        <asp:FileUpload ID="UploadFile" runat="server" Width="400px" OnBlur="setNames()" /> 
                        <asp:RequiredFieldValidator ID="UploadFileRequired" runat="server" ControlToValidate="UploadFile"
                            Display="Static" ErrorMessage="You must select a file to upload." Text="*" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" nowrap>
                        <cb:ToolTipLabel ID="UploadFileNameLabel" runat="server" Text="File Name:" AssociatedControlID="UploadFileName" ToolTip="Specify the name of the file to save your uploaded data to.  If you do not specify a name, the name of the uploaded file will be used."></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:TextBox ID="UploadFileName" runat="server" MaxLength="100" Width="200px" ValidationGroup="Upload"></asp:TextBox> (optional)
                        <asp:RegularExpressionValidator ID="FileNameValidator" runat="server" ControlToValidate="UploadFileName" ValidationGroup="Add"
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
                    <th class="rowHeader">
                        <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Display Name:" AssociatedControlID="Name" ToolTip="This is the name that will be displayed in the merchant admin and in the download link on customer invoices." />
                    </th>
                    <td>
                        <asp:TextBox ID="Name" runat="server" Text="" MaxLength="100" Width="300px" ValidationGroup="Add" />
                        <asp:RequiredFieldValidator ID="NameRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="Name" ValidationGroup="Add" Text="*" ErrorMessage="Display name is required." ></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="NameFormatValidator" runat="server" Display="Dynamic" ControlToValidate="Name" ValidationGroup="Add" Text="*" ErrorMessage="You may not use < or > characters in the name." ValidationExpression="[^<>]*"></asp:RegularExpressionValidator>
                        <asp:PlaceHolder ID="phUniqueName" runat="server"></asp:PlaceHolder>
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
                        <asp:TextBox ID="MaxDownloads" runat="Server" Width="25px" MaxLength="3"></asp:TextBox>
                        <asp:RangeValidator ID="MaxDownloadRangeValidator" runat="server" MinimumValue="1" MaximumValue="127" Text="*" ValidationGroup="Add"  ErrorMessage="Max downloads value must be from 1 to 127." ControlToValidate="MaxDownloads" Type="integer" ></asp:RangeValidator>
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
    <ajax:ModalPopupExtender ID="AddPopup" runat="server" 
        TargetControlID="AddLink"
        PopupControlID="AddDialog" 
        BackgroundCssClass="modalBackground"                         
        CancelControlID="CancelAddButton" 
        DropShadow="true"
        PopupDragHandleControlID="AddDialogHeader" />
    <asp:ObjectDataSource ID="DigitalGoodDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="Find" SelectCountMethod="FindCount" SortParameterName="sortExpression" TypeName="CommerceBuilder.DigitalDelivery.DigitalGoodDataSource">
        <SelectParameters>
            <asp:ControlParameter Name="nameToMatch" ControlID="SearchDGName" PropertyName="Text" Type="String" />
            <asp:ControlParameter Name="fileNameToMatch" ControlID="SearchDGFileName" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

