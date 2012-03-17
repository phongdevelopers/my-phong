<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Configure.aspx.cs" Inherits="Admin_Shipping_Providers_USPS_Configure" title="Configure USPS&reg;" %>
<%@ Register Src="../ProviderShipMethods.ascx" TagName="ShipMethods" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">

<div class="pageHeader">
	<div class="caption">
		<h1>Configure USPS&reg;</h1>
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
                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                    </td>
                                </tr>
                                
                                <tr>
                                    <td colspan="2" style="text-align:justify;padding:8px">
                                        <asp:Image ID="Logo" runat="server" ImageUrl="LOGO_S.gif" ImageAlign="Left" />
                                        <asp:Label ID="InstructionText" runat="server" Text="The United States Postal Service integration retrieves real-time estimates for shipping costs.  The estimated shipping costs are charged to an order when a U.S.P.S shipping method is used.  Shipping can be estimated for for all shipments of US origin, to both domestic and international destinations.  The integration also allows merchants to provide real-time tracking information to customers."></asp:Label>
                                    </td>
                                </tr>
                                <tr id="trInstanceName" runat="server" visible="false">
                                    <th class="rowHeader">Instance Name:<br/>
								        <span class="helpText">The name for this instance.</span>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="InstanceName" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th class="rowHeader" width="50%">User Id:<br/>
                                    </th>
                                    <td valign="top" width="50%">
                                        <asp:TextBox ID="UserId" runat="server" Text=""></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="EnablePackageBreakupLabel" runat="server" Text="Enable Package Breakup:"></asp:Label><br/>
                                        <asp:Label ID="EnablePackageBreakupHelpLabel" runat="server" CssClass="helpText" Text="If weight of a package exceeds maximum carrier weight, should it be broken into smaller packages?"></asp:Label>
                                    </th>
                                    <td>
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
				                        <asp:TextBox Width="60" ID="MaxPackageWeight" runat="server" Text="${MaxPackageWeight}"></asp:TextBox>lbs
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="MinPackageWeightLabel" runat="server" Text="Minimum Package Weight:"></asp:Label><br/>
                                        <asp:Label ID="MinPackageWeightHelpText" runat="server" CssClass="helpText" Text="The minimum weight of a package that the carrier allows."></asp:Label>
                                    </th>
                                    <td>
				                        <asp:TextBox Width="60" ID="MinPackageWeight" runat="server" Text="${MinPackageWeight}"></asp:TextBox>lbs
                                    </td>
                                </tr>

						        <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="LiveServerURLLabel" runat="server" Text="Live Mode URL:"></asp:Label><br/>
                                        <asp:Label ID="LiveServerURLHelpText" runat="server" CssClass="helpText" Text="The URL to which requests are posted in Live mode."></asp:Label>
                                    </th>
                                    <td>
				                        <asp:TextBox Width="300" ID="LiveServerURL" runat="server" Text="${LiveServerURL}"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="TestServerURLLabel" runat="server" Text="Test Mode URL:"></asp:Label><br/>
                                        <asp:Label ID="TestServerURLHelpText" runat="server" CssClass="helpText" Text="The URL to which requests are posted in Test Mode."></asp:Label>
                                    </th>
                                    <td>
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
                                    <th class="rowHeader" width="50%">Test Mode:<br/>
							        <span class="helpText">When Test Mode is enabled, rate requests are sent to test server.</span>	
                                    </th>
                                    <td>
                                        <asp:RadioButtonList ID="UseTestMode" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Disabled" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Enabled" Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">Enable Debug Mode:<br />
								        <span class="helpText">When debug mode is enabled, all messages sent to and received from USPS are logged. This should only be enabled at the direction of qualified support personnel.</span>
                                    </th>
                                    <td valign="top" width="50%">
                                        <asp:RadioButtonList ID="UseDebugMode" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Disabled" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Enabled" Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>                                
                                    </td>
                                </tr>                       
                                
                                <tr>
                                    <td colspan="2" align="center">
                                        <br />
                                        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" OnClientClick="javascript:return confirm('Are you sure you wish to delete your U.S. Postal Service registration information and disable the service?')" />
                                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
								        <asp:Button ID="CancelButton" runat="server" Text="Close" OnClick="CancelButton_Click" CausesValidation="false" />
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

</asp:Content>

