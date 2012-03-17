<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Configure.aspx.cs" Inherits="Admin_Shipping_Providers_CanadaPost_Configure" title="Configure CanadaPost&reg;" %>
<%@ Register Src="../ProviderShipMethods.ascx" TagName="ShipMethods" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1>Configure CanadaPost&reg;</h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">

	<tr>
	<td>
    <table class="inputForm" >
        <tr>    
            <td>                
                <ajaxToolkit:TabContainer ID="ThemesTabContainer" runat="server" >
                   <ajaxToolkit:TabPanel ID="MyThemesTab" runat="server" HeaderText="Configuration Settings" CssClass="contentSection" >
                        <ContentTemplate>         
                            <table class="inputForm" width="100%">                       
                                <tr>
                                    <td colspan="2" align="center">                                
                                        <asp:Label id="InstructionText" runat="server" Text="Specify the CanadaPost configuration options below.">
								        </asp:Label>
				                        <br /><br />
                                    </td>
                                </tr>
                                <tr id="trInstanceName" runat="server" visible="false">
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="NameLabel" runat="server" Text="Instance Name:"></asp:Label><br/>
                                        <asp:Label ID="NameHelpText" runat="server" CssClass="helpText" Text="The name for this instance."></asp:Label>
                                    </th>
                                    <td width="50%">
                                        <asp:TextBox ID="InstanceName" runat="server"></asp:TextBox></td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="MerchantCPCIDLabel" runat="server" Text="Merchant CPC ID:"></asp:Label><br/>
                                        <asp:Label ID="MerchantCPCIDHelpText" runat="server" CssClass="helpText" Text="This is your CanadaPost Merchant CPC ID."></asp:Label>
                                    </th>
                                    <td width="50%">
				                        <asp:TextBox ID="MerchantCPCIDDisplay" runat="server" Text="${MerchantCPCID}"></asp:TextBox>
                                    </td>
                                </tr>
                                
						        <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="EnablePackageBreakupLabel" runat="server" Text="Enable Package Breakup:"></asp:Label><br/>
                                        <asp:Label ID="EnablePackageBreakupHelpLabel" runat="server" CssClass="helpText" Text="If weight of a package exceeds maximum carrier weight, should it be broken into smaller packages?"></asp:Label>
                                    </th>
                                    <td width="50%">
                                        <asp:RadioButtonList ID="EnablePackageBreakup" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="MaxPackageWeightLabel" runat="server" Text="Maximum Package Weight:"></asp:Label><br/>
                                        <asp:Label ID="MaxPackageWeightHelpText" runat="server" CssClass="helpText" Text="The maximum weight of a package that the carrier allows."></asp:Label>
                                    </th>
                                    <td>
				                        <asp:TextBox Width="60" ID="MaxPackageWeight" runat="server" Text="${MaxPackageWeight}"></asp:TextBox>kgs
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="MinPackageWeightLabel" runat="server" Text="Minimum Package Weight:"></asp:Label><br/>
                                        <asp:Label ID="MinPackageWeightHelpText" runat="server" CssClass="helpText" Text="The minimum weight of a package that the carrier allows."></asp:Label>
                                    </th>
                                    <td>
				                        <asp:TextBox Width="60" ID="MinPackageWeight" runat="server" Text="${MinPackageWeight}"></asp:TextBox>kgs
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="LiveServerURLLabel" runat="server" Text="Live Mode URL:"></asp:Label><br/>
                                        <asp:Label ID="LiveServerURLHelpText" runat="server" CssClass="helpText" Text="The URL to which requests are posted in Live mode."></asp:Label>
                                    </th>
                                    <td width="50%">
				                        <asp:TextBox Width="300" ID="LiveServerURL" runat="server" Text="${LiveServerURL}"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="TestServerURLLabel" runat="server" Text="Test Mode URL:"></asp:Label><br/>
                                        <asp:Label ID="TestServerURLHelpText" runat="server" CssClass="helpText" Text="The URL to which requests are posted in Test Mode."></asp:Label>
                                    </th>
                                    <td width="50%">
				                        <asp:TextBox Width="300" ID="TestServerURL" runat="server" Text="${TestServerURL}"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="TrackingURLLabel" runat="server" Text="Tracking URL:"></asp:Label><br/>
                                        <asp:Label ID="TrackingURLHelpText" runat="server" CssClass="helpText" Text="The URL to which requests are sent for package tracking. {0} is substituted with the tracking number at the time of request."></asp:Label>
                                    </th>
                                    <td width="50%">
				                        <asp:TextBox Width="300" ID="TrackingURL" runat="server" Text="${TrackingURL}"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="UseTestModeLabel" runat="server" Text="Test Mode:"></asp:Label><br/>
                                        <asp:Label ID="UseTestModeHelpLabel" runat="server" CssClass="helpText" Text="When Test Mode is enabled, rate requests are sent to test server."></asp:Label>
                                    </th>
                                    <td width="50%">
                                        <asp:RadioButtonList ID="UseTestMode" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Disabled" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Enabled" Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="UseDebugLabel" runat="server" Text="Debug Mode:"></asp:Label><br/>
                                        <asp:Label ID="UseDebugHelpText" runat="server" CssClass="helpText" Text="When debug mode is enabled, all messages sent to and received from CanadaPost are logged. This should only be enabled at the direction of qualified support personnel."></asp:Label>
                                    </th>
                                    <td width="50%">
                                        <asp:RadioButtonList ID="UseDebugMode" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Disabled" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Enabled" Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
        						
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:Button ID="DeleteButton" runat="server" Text="Delete" CausesValidation="false" OnClick="DeleteButton_Click" OnClientClick="javascript:return confirm('Are you sure you wish to delete your registration and disable the features of CanadaPost?')" />
                                        <asp:Button ID="SaveButton" runat="server" Text="Update" OnClick="SaveButton_Click"/>
								        <asp:Button ID="CancelButton" runat="server" Text="Close" CausesValidation="false" OnClick="CancelButton_Click" />
                                    </td>
                                </tr>         
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>                   
                    <ajaxToolkit:TabPanel ID="ShipMethodsPanel" runat="server" HeaderText="Services (Shipping Methods)">
                        <ContentTemplate>
                            <table class="inputForm" width="100%">
                                <tr>
                                    <td colspan="2">
                                        <uc1:ShipMethods id="ShipMethods1" runat="server">
                                        </uc1:ShipMethods>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer>
                </td>
            </tr>
        </table>
		
	</td>
	</tr>	
</table>

    <br />
	<div align="center">
	<asp:Label ID="CanadaPostCopyRight" runat="server" Text="CanadaPost and CanadaPost brandmark are trademarks of CanadaPost Inc. All Rights Reserved." SkinID="Copyright"></asp:Label>
	</div>

</asp:Content>
