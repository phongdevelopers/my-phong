<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditShipMethodFixed.aspx.cs" Inherits="Admin_Shipping_Methods_EditShipMethodFixed" Title="Edit Shipping Method"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<ajax:UpdatePanel ID="PageAjax" runat="server">
<ContentTemplate>
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Edit Method '{0}'" EnableViewState="false"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td colspan="2">
            <asp:Panel ID="InputPanel" runat="server" DefaultButton="SaveButton">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                <table class="inputForm" cellpadding="4" width="700px">
                    <tr>
                        <th class="rowHeader" width="150px">
                            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="The name of this shipping method as it appears to the merchant and the customer." />
                        </th>
                        <td width="200px">
                            <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                                    Display="Static" ErrorMessage="Ship method name is required.">*</asp:RequiredFieldValidator>
                        </td>
                        <th class="rowHeader" width="200px">
                            <asp:Label ID="ShipMethodTypeLabel" runat="server" Text="Type:"></asp:Label>
                        </th>
                        <td width="200px">
                            <asp:Label ID="ShipMethodType" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="ShipRateLabel" runat="server" Text="Shipping Charge:" ToolTip="The amount charged for this shipping method." />
                        </th>
                        <td colspan="3">
                            <asp:TextBox ID="Rate" runat="server" MaxLength="8" Width="60px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RateRequired" runat="server" ControlToValidate="Rate"
                                    Display="Static" ErrorMessage="Ship rate is required.">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="TaxCodeLabel" runat="server" Text="Shipping Tax Code:" ToolTip="If you wish to create tax rules for this shipping method, choose the tax code that should be assigned to calculated charges.  You can then configure the tax rules for this code." AssociatedControlID="TaxCode" EnableViewState="false" />
                        </th>
                        <td>
                            <asp:DropDownList ID="TaxCode" runat="server" AppendDataBoundItems="true" DataTextField="Name" DataValueField="TaxCodeId" EnableViewState="false">
                                <asp:ListItem Text=""></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="SurchargeLabel" runat="server" Text="Handling Fee:" ToolTip="Specify a surcharge or handling fee that is associated with this method.  You can set surcharges as a fixed amount or a percentage of the shipping charge.  You can also choose to include the surcharge in the total shipping rate or display as a separate line item in the order." />
                        </th>
                        <td valign="top" colspan="3">
                            <asp:TextBox ID="Surcharge" runat="server" Columns="8"></asp:TextBox>
                            <asp:DropDownList ID="SurchargeIsPercent" runat="server">
                                <asp:ListItem Text="Fixed Amount"></asp:ListItem>
                                <asp:ListItem Text="Percent (%)"></asp:ListItem>
                            </asp:DropDownList>
                            <br />
                            <asp:RadioButtonList ID="SurchargeIsVisible" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SurchargeIsVisible_SelectedIndexChanged">
                                <asp:ListItem Text="Include handling fee in shipping cost."></asp:ListItem>
                                <asp:ListItem Text="Show handling fee separately from shipping cost."></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr id="trSurchargeTaxCode" runat="server" visible="false">
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="SurchargeTaxCodeLabel" runat="server" Text="Handling Tax Code:" ToolTip="If you wish to create tax rules for surcharge or handling fee of this shipping method, choose the tax code that should be assigned to calculated surcharge/handling fee charges.  You can then configure the tax rules for this code." AssociatedControlID="SurchargeTaxCode" EnableViewState="false" />
                        </th>
                        <td valign="top" colspan="3">
                            <asp:DropDownList ID="SurchargeTaxCode" runat="server" AppendDataBoundItems="true" DataTextField="Name" DataValueField="TaxCodeId" EnableViewState="false">
                                <asp:ListItem Text=""></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="WarehousesLabel" runat="server" Text="Warehouses:" ToolTip="Indicate whether this shipping method is available for products in all warehouses, or if it is limited to specific warehouses." />
                        </th>
                        <td valign="top">
                            <ajax:UpdatePanel ID="WarehouseRestrictionAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="UseWarehouseRestriction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UseWarehouseRestriction_SelectedIndexChanged">
                                        <asp:ListItem Text="All Warehouses" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="Selected Warehouses"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Panel ID="WarehouseListPanel" runat="server" Visible="false">
                                        <div style="padding-left:20px">
                                            <asp:ListBox ID="WarehouseList" runat="server" SelectionMode="multiple" Rows="4" DataTextField="Name" DataValueField="WarehouseId"></asp:ListBox>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="ZonesLabel" runat="server" Text="Zones:" ToolTip="Indicate whether this shipping method is available to all zones, or if it is limited to specific zones." />
                        </th>
                        <td valign="top">
                            <ajax:UpdatePanel ID="ZoneRestrictionAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="UseZoneRestriction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UseZoneRestriction_SelectedIndexChanged">
                                        <asp:ListItem Text="All Zones" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="Selected Zones"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Panel ID="ZoneListPanel" runat="server" Visible="false">
                                        <div style="padding-left:20px">
                                            <asp:ListBox ID="ZoneList" runat="server" SelectionMode="multiple" Rows="4" DataTextField="Name" DataValueField="ShipZoneId"></asp:ListBox>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="GroupsLabel" runat="server" Text="Groups:" ToolTip="Indicate whether only users that belong to specific groups can use this shipping method." />
                        </th>
                        <td>
                            <ajax:UpdatePanel ID="GroupRestrictionAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="UseGroupRestriction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UseGroupRestriction_SelectedIndexChanged">
                                        <asp:ListItem Text="All Groups" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="Selected Groups"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Panel ID="GroupListPanel" runat="server" Visible="false">
                                        <div style="padding-left:20px">
                                            <asp:CheckBoxList ID="GroupList" runat="server" DataTextField="Name" DataValueField="GroupId"></asp:CheckBoxList>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="MinPurchaseLabel" runat="server" Text="Minimum Purchase:" ToolTip="The minimum purchase value of a shipment required for this method to be valid." />
                        </th>
                        <td valign="top">
                            <asp:TextBox ID="MinPurchase" runat="server" MaxLength="8" Width="60px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" align="center">                            
                            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
							<asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false" OnClick="CancelButton_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
</ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>

