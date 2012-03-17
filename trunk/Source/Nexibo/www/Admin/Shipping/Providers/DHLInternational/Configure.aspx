<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Configure.aspx.cs" Inherits="Admin_Shipping_Providers_DHL_Configure" title="Configure DHL International" %>
<%@ Register Src="../ProviderShipMethods.ascx" TagName="ShipMethods" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">

<div class="pageHeader">
	<div class="caption">
		<h1>Configure DHL International</h1>
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
                            <table class="inputForm" >                               
                                <tr>
                                    <td colspan="2" align="center">                                
                                        <asp:Label id="InstructionText" runat="server" Text="Specify the DHL configuration options below."></asp:Label>
				                        <br /><br />
                                    </td>
                                </tr>
                                <tr id="trInstanceName" runat="server" visible="false">
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="NameLabel" runat="server" Text="Instance Name:"></asp:Label><br/>
                                        <asp:Label ID="NameHelpText" runat="server" CssClass="helpText" Text="The name for this instance."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="InstanceName" runat="server"></asp:TextBox></td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="UserIDLabel" runat="server" Text="User ID:"></asp:Label><br/>
                                        <asp:Label ID="UserIDHelpText" runat="server" CssClass="helpText" Text="This is your DHL User ID."></asp:Label>
                                    </th>
                                    <td>
				                        <asp:TextBox ID="UserIDDisplay" runat="server" Text="${UserID}"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="PasswordLabel" runat="server" Text="Password:"></asp:Label><br/>
                                        <asp:Label ID="PasswordHelpText" runat="server" CssClass="helpText" Text="This is your DHL Password."></asp:Label>
                                    </th>
                                    <td>
				                        <asp:TextBox ID="PasswordDisplay" runat="server" Text="${Password}"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="AccountNumberLabel" runat="server" Text="Account Number:"></asp:Label><br/>
                                        <asp:Label ID="AccountNumberHelpText" runat="server" CssClass="helpText" Text="DHL Account Number."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="AccountNumber" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="ShippingKeyLabel" runat="server" Text="Shipping Key:"></asp:Label><br/>
                                        <asp:Label ID="ShippingKeyHelpText" runat="server" CssClass="helpText" Text="DHL Shipping Key."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="ShippingKey" runat="server"></asp:TextBox></td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="DutiableFlagLabel" runat="server" Text="Shipments are Dutiable?"></asp:Label><br/>
                                        <asp:Label ID="DutiableFlagHelpLabel" runat="server" CssClass="helpText" Text="Indicates if the shipment sent via this DHL gateway are dutiable or non-dutiable."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:RadioButtonList ID="DutiableFlag" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem ID="DutiableFlagYes" Text="Yes" Value="1"></asp:ListItem>
                                            <asp:ListItem ID="DutiableFlagNo" Text="No" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>								

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="CustomsValueMultiplierLabel" runat="server" Text="Customs Value Multiplier:"></asp:Label><br/>
                                        <asp:Label ID="CustomsValueMultiplierHelpText" runat="server" CssClass="helpText" Text="If shipments are dutiable customs value is required by DHL. Enter a multiplier to use to determine the customs value of shipments. Customs value is calculated as Customs Value = (Retail Value of the Package) * (Customs Value Multiplier). The default value is 1 which effectively means Customs Value = Retail Value."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="CustomsValueMultiplier" runat="server" Width="50px"></asp:TextBox>
                                        (default 1)
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="CommerceLicensedLabel" runat="server" Text="Commerce Licensed?"></asp:Label><br/>
                                        <asp:Label ID="CommerceLicensedHelpLabel" runat="server" CssClass="helpText" Text="If Shipments are Dutiable, you must also indicate whether the shipments are commerce-licensed shipments. <br/>Note: An ITN number will be required if the shipments are commerce-licensed."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:RadioButtonList ID="CommerceLicensed" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="FilingTypeLabel" runat="server" Text="Filing Type:"></asp:Label><br/>
                                        <asp:Label runat="server" ID="FilingTypeHelpLabel" CssClass="helpText" Text="If Shipments are Dutiable, you must also indicate the filing type. <br/>FTR – if the shipments fulfils the requirements of an FTR (FTSR) exemption code. <br/>ITN – if the shipments have been filed for and an ITN number is available. <br/>AES4 – if the sender is an AES4 approved filer and will file for this shipment post-departure within the deadline."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:DropDownList ID="FilingType" runat="server">                         
                                        </asp:DropDownList>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="FTRExemptionCodeLabel" runat="server" Text="FTR Exemption Code:"></asp:Label><br/>
                                        <asp:Label ID="FTRExemptionCodeHelpText" runat="server" CssClass="helpText" Text="If the filing type is FTR, provide FTR exemption code to use. <br/>Required when filing type is FTR."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="FTRExemptionCode" runat="server" ></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="ITNNumberLabel" runat="server" Text="ITN Number:"></asp:Label><br/>
                                        <asp:Label ID="ITNNumberHelpText" runat="server" CssClass="helpText" Text="If the filing type is ITN, provide an ITN number. <br/>Required when filing type is ITN."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="ITNNumber" runat="server" ></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="EINCodeLabel" runat="server" Text="EIN Code:"></asp:Label><br/>
                                        <asp:Label ID="EINCodeHelpText" runat="server" CssClass="helpText" Text="If the filing type is AES4, provide an EIN code. <br/>Required when filing type is AES4."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="EINCode" runat="server" ></asp:TextBox> (default 0)
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="EnablePackageBreakupLabel" runat="server" Text="Enable Package Breakup:"></asp:Label><br/>
                                        <asp:Label ID="EnablePackageBreakupHelpLabel" runat="server" CssClass="helpText" Text="If weight of a package exceeds maximum carrier weight, should it be broken into smaller packages?. If yes what should be the maximum weight of a package in lbs?"></asp:Label>
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
                                        <asp:Label ID="DaysToShipLabel" runat="server" Text="Days To Ship:"></asp:Label><br/>
                                        <asp:Label ID="DaysToShipHelpText" runat="server" CssClass="helpText" Text="Number of days it takes to ship a package."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="DaysToShip" runat="server" Width="40px"></asp:TextBox>
                                        (min 1)
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
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="UseTestModeLabel" runat="server" Text="Test Mode:"></asp:Label><br/>
                                        <asp:Label ID="UseTestModeHelpLabel" runat="server" CssClass="helpText" Text="When Test Mode is enabled, rate requests are sent to test server."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:RadioButtonList ID="UseTestMode" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Disabled" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Enabled" Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>

                                <tr>
                                    <th class="rowHeader" width="50%">
                                        <asp:Label ID="UseDebugLabel" runat="server" Text="Debug Mode:"></asp:Label><br/>
                                        <asp:Label ID="UseDebugHelpText" runat="server" CssClass="helpText" Text="When debug mode is enabled, all messages sent to and received from DHL are logged. This should only be enabled at the direction of qualified support personnel."></asp:Label>
                                    </th>
                                    <td>
                                        <asp:RadioButtonList ID="UseDebugMode" runat="server" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Disabled" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Enabled" Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:Button ID="DeleteButton" runat="server" Text="Delete" CausesValidation="false" OnClick="DeleteButton_Click" OnClientClick="javascript:return confirm('Are you sure you wish to delete your registration and disable the features of DHLInternational?')" />                                
                                        <asp:Button ID="SaveButton" runat="server" Text="Update" OnClick="SaveButton_Click"/>
								        <asp:Button ID="CancelButton" runat="server" Text="Close" CausesValidation="false" OnClick="CancelButton_Click"/>
                                    </td>
                                </tr>
                            </table>
                          </ContentTemplate>
                    </ajaxToolkit:TabPanel>                   
                    <ajaxToolkit:TabPanel ID="ShipMethodsPanel" runat="server" HeaderText="Services (Shipping Methods)">
                        <ContentTemplate>                    
                            <table class="inputForm" >                        
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
	<asp:Label ID="DHLCopyRight" runat="server" Text="DHL and DHL brandmark are trademarks of DHL Inc. All Rights Reserved." SkinID="Copyright"></asp:Label>
	</div>

</asp:Content>
