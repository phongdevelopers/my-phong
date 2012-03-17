<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="Admin_SEO_Settings" Title="SEO Settings" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel id="SettingsAjax" runat="server" >
    <ContentTemplate>
        <div class="pageHeader">
            <div class="caption">
                <h1><asp:Localize ID="SettingsCaption" runat="server" Text="SEO Settings"></asp:Localize></h1>
            </div>
        </div>
        <table cellpadding="2" cellspacing="2" class="innerLayout">
	        <tr>
		        <td align="left" valign="top" width="50%">
		            <div class="section" style="padding:0 0 2px 0;">
                        <div class="header">
                            <h2 class="commonicon"><asp:Localize ID="GeneralCaption" runat="server" Text="General"></asp:Localize></h2>
                        </div>
                        <div class="content">
                            <asp:Localize ID="CacheSizeHelpText" runat="server" Text="Using SEO features like redirects and custom URLs requires incoming requests to be checked and routed to the correct destination.  A second level cache helps this process run much faster than a database lookup.  Indicate the maximum size of your cache below.  A value of 1000 is suitable unless you have more than that number of custom urls and an extremely active site."></asp:Localize>
                            <br /><br />
                            <asp:Label ID="CacheSizeLabel" runat="Server" Text="Cache Size:" SkinID="FieldHeader"></asp:Label>
                            <asp:TextBox ID="CacheSize" runat="server" MaxLength="7" Columns="3" ValidationGroup="SEOSettings"></asp:TextBox>
                            <asp:RangeValidator ID="CacheRangeValidator" runat="server" Text="*" ErrorMessage="Cache size is should be at least 100." ControlToValidate="CacheSize" ValidationGroup="SEOSettings" Type="Integer" MinimumValue="100" MaximumValue="9999999"></asp:RangeValidator>
                            <asp:RequiredFieldValidator ID="CacheSizeRequired" runat="server" ErrorMessage="Cache size is required and should be at least 100." Text="*" ControlToValidate="CacheSize" ValidationGroup="SEOSettings"></asp:RequiredFieldValidator>
                            <br /><br />
                            <asp:Localize ID="TrackingHelpText" runat="server" Text="When a redirect is triggered, the date and time can be recorded and a hit counter can be updated. This can help you see what redirects are being used and which may be outdated and safe to remove. There is a small processing overhead due to a database update when a redirected URL is visited, so this feature is optional."></asp:Localize>
                            <br /><br />
                            <asp:CheckBox ID="EnableTracking" runat="server" />
                            <asp:Label ID="EnableTrackingLabel" runat="Server" Text="Enable Statistics Tracking" SkinID="FieldHeader" AssociatedControlID="EnableTracking"></asp:Label>
                        </div>
                    </div>
                </td>
                <td valign="top" width="50%">
		            <div class="section">
                        <div class="header">
                            <h2 class="commonicon"><asp:Localize ID="CustomExtensionsCaption" runat="server" Text="Custom Extensions"></asp:Localize></h2>
                        </div>
                        <div class="content">
                            <asp:PlaceHolder ID="phCustomExtensionsUnavailable" runat="server" Visible="false">
                                <asp:Literal ID="CustomExtensionsMessage" runat="server"></asp:Literal>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phCustomExtensionsUnconfigured" runat="server" Visible="false">
                                <asp:Localize ID="WebConfigChangeRequiredMessage" runat="server">
                                    For custom extensions to function, a change must be made to the web.config. 
                                    The runAllManagedModulesForAllRequests attribute must be set to true for the 
                                    system.webServer/modules element. You can make this change yourself, or we can 
                                    attempt to make the change for you automatically.<br /><br />                                
                                    Be aware that enabling this option will come at some performance cost.  Depending
                                    on the number of page requests you receive and your server hardware, the difference
                                    may be negligible.  Still you should enable it only if you need to use custom urls with
                                    exensions other than aspx.  Once the configuration change is made, you will be able
                                    to specify the additional extensions you wish to support.
                                </asp:Localize><br /><br />
                                <asp:Button ID="SetWebConfiguration" runat="server" Text="Enable Automatically" OnClick="SetWebConfiguration_Click" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phCustomExtensionsConfigured" runat="server" Visible="false">
                                <asp:Localize ID="CustomExtensionsHelpText" runat="server">
                                    When custom extensions are enabled, your redirects and custom urls can use extensions other than aspx.
                                </asp:Localize><br /><br />
                                <asp:CheckBox ID="AllowCustomExtensions" runat="server" /> 
                                <asp:Label ID="AllowCustomExtensionsLabel" runat="Server" Text="Allow Custom Extensions" AssociatedControlID="AllowCustomExtensions" SkinID="FieldHeader"></asp:Label>
                                <asp:Panel ID="CustomExtensionsPanel" runat="server" style="padding:6px;padding-left:20px">
                                    Enter the extension(s) you want to support.  Do not enter a leading period, and separate multiple the extensions with a comma.  You may also elect to enable support for URLs that do not have an extension.
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="AllowedExtensionsLabel" runat="Server" Text="Extensions:" AssociatedControlID="AllowedExtensions" SkinID="FieldHeader"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="AllowedExtensions" runat="server" MaxLength="200" Width="200px"></asp:TextBox> (e.g. htm,php)
                                                <asp:RegularExpressionValidator ID="AllowedExtensionsValidator" runat="server" ControlToValidate="AllowedExtensions" ValidationExpression="^.{1,6}(?:,\s*.{1,6})*$" Text="*" ErrorMessage="Allowed extensions value should be a comma delimited list of extensions." ValidationGroup="SEOSettings"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:CheckBox ID="AllowUrlWithoutExtensions" runat="server" Text="Allow URLs Without Extensions" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="RemoveWebConfigurationPanel" runat="server">
                                    <br />
                                    <asp:Localize ID="RemoveWebConfiguration" runat="server">
                                        You do not have custom extensions turned on, but in your web.config file the runAllManagedModulesForAllRequests
                                        attribute of system.webServer/modules is enabled.  If you have not enabled this 
                                        setting for some other reason, you will improve site performance by disabling the option.<br /><br />
                                        You can either make the change manually by editing your web.config and setting the attribute 
                                        value to false, or we can attempt to disable this setting automatically.
                                    </asp:Localize>
                                    <br /><br />
                                    <asp:Button ID="RemoveWebConfigurationButton" runat="server" Text="Disable Automatically" OnClick="RemoveWebConfigurationButton_Click" />
                                </asp:Panel>
                            </asp:PlaceHolder>
                        </div>
                    </div>
		        </td>
            </tr>
        </table>
        <div class="searchPanel" style="padding:6px;">
            <asp:Label ID="SavedMessage" runat="server" Text="The configuration has been saved at {0}." SkinID="GoodCondition" Visible="False" EnableViewState="false"></asp:Label>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="SEOSettings" />
            <div align="center">
                <asp:Button ID="SaveButton" runat="server" Text="Save Changes" OnClick="SaveButton_Click" ValidationGroup="SEOSettings"></asp:Button>
            </div>
        </div>
    </ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>