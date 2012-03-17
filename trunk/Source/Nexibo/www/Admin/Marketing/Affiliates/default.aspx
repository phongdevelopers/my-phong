<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Marketing_Affiliates_Default" Title="Manage Affiliates" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Namespace="Westwind.Web.Controls" assembly="wwhoverpanel" TagPrefix="wwh" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">

    <script type="text/javascript">
        function ShowHoverPanel(event)
        { 
            TrackerHelpHover.startCallback(event,"",null,OnError);    
        }
        function HideHoverPanel()
        {
            TrackerHelpHover.hide();
        }
        function OnError(Result)
        {
            alert("*** Error:\r\n\r\n" + Result.message);    
        }

        function toggleDialog()
        {
            var toggleButton = document.getElementById("<%= this.ToggleSettingsButton.ClientID %>");
            var settingsPanel = document.getElementById("<%= this.ToggleSettingsPanel.ClientID %>");
            if (toggleButton != null && settingsPanel != null)
            {
                if (settingsPanel.style.display=='block')
                {
                    settingsPanel.style.display='none';
                    toggleButton.src = "<%=this.ExpandIconUrl%>";
                }
                else
                {
                    settingsPanel.style.display='block';
                    toggleButton.src = "<%=this.CollapseIconUrl%>";
                }
            }
        }
    </script>	
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Affiliates"></asp:Localize></h1>
    	</div>
    </div>
    <div class="section" style="padding:0 0 2px 0;">
        <div class="searchPanel" style="padding:6px;">
            <span onclick="toggleDialog()" style="cursor:pointer">
                <asp:Label ID="AffiliateSettingsCaption" runat="server" Text="Affiliate Settings" SkinID="FieldHeader"></asp:Label>
                <asp:Image ID="ToggleSettingsButton" runat="server" AlternateText="Show/Hide Affiliate Settings" />
            </span>
            <asp:Panel ID="ToggleSettingsPanel" runat="server">
                <ajax:UpdatePanel id="SettingsAjax" runat="server" >                    
                    <contenttemplate>
                        <hr />
                        <asp:ValidationSummary ID="SettingsValidationSummary" runat="server" ValidationGroup="Settings" />
                        <table cellpadding="2" cellspacing="0">
                            <tr>
                                <td width="50%" align="left" valign="top">
                                    <div class="header">
                                        <h2 class="commonicon"><asp:Localize ID="GeneralSettingsCaption" runat="server" Text="General" /></h2>
                                    </div>
                                    <div class="content">
                                        <table class="inputForm">
                                            <tr>
                                                <th class="rowHeader" align="right" style="width:120px;" valign="Top">
                                                    <cb:ToolTipLabel ID="AffiliateParameterLabel" runat="server" Text="Parameter Name:" ToolTip="Specify that parameter name that should be used in the query string to identify the referring affiliate.  For instance if you use afid then external links will need to have ?afid=# (where # is the affiliate ID) appended to them to identify the affiliate." />
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="AffiliateParameter" runat="server" ValidationGroup="Settings"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="AffiliateParameterRequired" runat="server" ControlToValidate="AffiliateParameter" ValidationGroup="Settings" Text="*" ErrorMessage="The affiliate parameter name is required."></asp:RequiredFieldValidator>                                                    
                                                    <br />
                                                    <span class="helpText">WARNING: This setting is used for all affiliates.  If you have already created affiliates and they have posted links to your site, those link will need to be updated if you change this value.</span>

                                                </td>
                                            </tr>
                                        </table>
                                    </div><br />
                                    <div class="header">
                                        <h2 class="thirdpartytracker"><asp:Localize ID="TrackerCaption" runat="server" Text="Third Party Tracker" /></h2>
                                    </div>
                                    <div class="content">
                                        <asp:Localize id="TrackerHelpText" runat="server" Text="If you are using a third party tracker such as AffiliateWiz, you can provide the tracking URL here."></asp:Localize>
                                        <a onmouseover="ShowHoverPanel(event)" class="link" onmouseout="HideHoverPanel();" href="#">More Help</a><br />
                                        <div style="padding-left:20px;padding-top:10px;">
                                            <asp:Label ID="TrackerUrlCaption" runat="server" Text="Tracking Url: " SkinID="FieldHeader"></asp:Label>
                                            <asp:TextBox id="TrackerUrl" runat="server" Width="250px" MaxLength="200"></asp:TextBox>
                                        </div>
                                    </div>
                                </td>
                                <td width="50%" align="left" valign="top">
                                    <div class="header">
                                        <h2 class="commonicon"><asp:Localize ID="Localize1" runat="server" Text="Self Signup" /></h2>
                                    </div>
                                    <div class="content">
                                        <asp:Localize ID="SelfSignupHelpText" runat="server">
                                            You can enable your customers to sign themselves up as affiliates to your store.  Enable this option and then configure the commission rates you will pay to your new affiliates.  This information will be displayed for the customer when they sign up.
                                        </asp:Localize><br />
                                        <table class="inputForm">
                                            <tr>
                                                <th class="rowHeader" align="right">
                                                    <cb:ToolTipLabel ID="SelfSignupLabel" runat="server" Text="Enable Self Signup:" AssociatedControlID="SelfSignup" ToolTip="Enable or disable affiliate self signup." />
                                                </th>
                                                <td>
                                                    <asp:CheckBox ID="SelfSignup" runat="server" Checked="false" 
                                                        oncheckedchanged="SelfSignup_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                            </tr>
                                            <tr id="trPersistence" runat="server" visible="false">
                                                <th class="rowHeader" align="right">
                                                    <cb:ToolTipLabel ID="PersistenceLabel" runat="server" Text="Persistence:"
                                                        ToolTip="Indicate how long orders will count for commissions after an affiliate refers a customer." />
                                                </th>
                                                <td>
                                                    <asp:DropDownList ID="AffiliatePersistence" runat="server" 
                                                        onselectedindexchanged="AffiliatePersistence_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Text="Persistent" Value="3"></asp:ListItem>
                                                        <asp:ListItem Text="First Order" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="First X Days" Value="0"></asp:ListItem>
                                                        <asp:ListItem Text="First Order Within X Days" Value="2"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr id="trReferralPeriod" runat="server" visible="false">
                                                <th class="rowHeader" align="right">
                                                    <cb:ToolTipLabel ID="AffiliateReferralPeriodLabel" runat="server" Text="Referral Period:" ToolTip="The number days for referrals sent by an affiliate."></cb:ToolTipLabel>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="ReferralPeriod" runat="server" Columns="4" MaxLength="4"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="ReferralPeriodRequiredValidator" runat="server" ControlToValidate="ReferralPeriod" Text="*" ErrorMessage="You must specify number of days for referral." ValidationGroup="Settings"></asp:RequiredFieldValidator>
                                                    <asp:RangeValidator ID="ReferralPeriodValidator" runat="server" Type="Integer" MinimumValue="1"  MaximumValue="9999" ControlToValidate="ReferralPeriod" ErrorMessage="Referral period must be a numeric value." Text="*" ValidationGroup="Settings"></asp:RangeValidator>
                                                </td>
                                            </tr>
                                            <tr id="trCommissionRate" runat="server" visible="false">
                                                <th class="rowHeader" align="right">
                                                    <cb:ToolTipLabel ID="AffiliateCommissionRateLabel" runat="server" Text="Commission Rate" ToolTip="Indicate the amount paid for affiliate orders.  A fixed amount will be calculated per order and a percentage will be calculated on the total value of products in referred orders."></cb:ToolTipLabel><br />                                        </th>
                                                <td>
                                                    <asp:TextBox ID="CommissionRate" runat="server" Columns="4" MaxLength="8"></asp:TextBox>
                                                    <asp:DropDownList ID="CommissionType" runat="server" >
                                                        <asp:ListItem Text="Flat rate"></asp:ListItem>
                                                        <asp:ListItem Text="% of product subtotal"></asp:ListItem>
                                                        <asp:ListItem Text="% of order total"></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:RangeValidator ID="CommissionRateValidator" runat="server" Type="Double" MinimumValue="0" MaximumValue="999999999" ControlToValidate="CommissionRate" ErrorMessage="Commission rate must be a numeric value greater than 0." Text="*" ValidationGroup="Settings"></asp:RangeValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <asp:Button id="SaveButton" onclick="SaveButton_Click" 
                            runat="server" Text="Save" ValidationGroup="Settings"></asp:Button>&nbsp;
                        <asp:Label id="AffiliateSettingsMessage" runat="server" SkinID="GoodCondition" EnableViewState="false"></asp:Label>
                    </contenttemplate>
                </ajax:UpdatePanel>
            </asp:Panel>
        </div>
    </div>
    <ajax:UpdatePanel ID="GridAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cb:SortedGridView ID="AffiliateGrid" runat="server" AllowPaging="true" AllowSorting="true" PageSize="20"
                AutoGenerateColumns="False" DataKeyNames="AffiliateId" DataSourceID="AffiliateDs" 
                ShowFooter="False" DefaultSortExpression="Name" SkinID="PagedList" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="ID" SortExpression="AffiliateId">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="AffiliateIdLabel" runat="server" Text='<%# Eval("AffiliateId") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:HyperLink ID="NameLink" runat="server" NavigateUrl='<%#Eval("AffiliateId", "EditAffiliate.aspx?AffiliateId={0}")%>' Text='<%#Eval("Name")%>' SkinId="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Commission">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# GetCommissionRate(Container.DataItem) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Persistence">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# GetPersistenceLabel(Container.DataItem) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Orders" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="OrdersLink" runat="server" text='<%#GetOrderCount(Container.DataItem)%>' NavigateUrl='<%#Eval("AffiliateId", "../../Reports/SalesByAffiliateDetail.aspx?AffiliateId={0}") %>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Customers" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#GetUserCount(Container.DataItem)%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Link">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#GetHomeUrl(Container.DataItem)%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("AffiliateId", "EditAffiliate.aspx?AffiliateId={0}")%>'><asp:Image ID="EditIcon" SkinID="EditIcon" runat="server" AlternateText="Edit" /></asp:HyperLink>
                            <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\");") %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon"  AlternateText="Delete" /></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Wrap="false" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label ID="EmptyDataText" runat="server" Text="No Affiliates are defined for your store."></asp:Label>
                </EmptyDataTemplate>
            </cb:SortedGridView>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <div class="section">
        <div class="header">
            <h2 class="addaffiliate"><asp:Localize ID="AddCaption" runat="server" Text="Add Affiliate" /></h2>
        </div>
        <div class="content">
            <ajax:UpdatePanel ID="AddAjax" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="AddAffiliateNameLabel" runat="server" Text="Name:" SkinID="FieldHeader"></asp:Label>&nbsp;
                    <asp:TextBox ID="AddAffiliateName" runat="server" Width="150px" MaxLength="100"></asp:TextBox>&nbsp;
                    <asp:Button ID="AddAffiliateButton" runat="server" Text="Add" OnClick="AddAffiliateButton_Click" ValidationGroup="Add" />
                </ContentTemplate>
            </ajax:UpdatePanel>
            <asp:RequiredFieldValidator ID="AddAffiliateNameRequired" runat="server" ControlToValidate="AddAffiliateName"
                Display="Dynamic" Text="Name is required." ValidationGroup="Add"></asp:RequiredFieldValidator>
        </div>
    </div>
    <asp:ObjectDataSource ID="AffiliateDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Marketing.AffiliateDataSource" 
        SelectCountMethod="CountForStore" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Marketing.Affiliate" 
        DeleteMethod="Delete" UpdateMethod="Update">
    </asp:ObjectDataSource>
    <wwh:wwHoverPanel ID="TrackerHelpHover"
        runat="server" 
        serverurl="~/Admin/Marketing/Affiliates/TrackerHelp.htm"
        Navigatedelay="250"              
        scriptlocation="WebResource"
        style="display: none; background: white;" 
        panelopacity="0.89" 
        shadowoffset="8"
        shadowopacity="0.18"
        PostBackMode="None"
        AdjustWindowPosition="true">
    </wwh:wwHoverPanel>
</asp:Content>

