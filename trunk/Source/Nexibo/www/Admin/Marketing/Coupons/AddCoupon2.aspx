<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="AddCoupon2.aspx.cs" Inherits="Admin_Marketing_Coupons_AddCoupon2" Title="Add Coupon" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
		<div class="caption">
			<h1><asp:Localize ID="Caption" runat="server" Text="Add {0} Coupon"></asp:Localize></h1>
		</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <table class="inputForm" cellpadding="4" cellspacing="0">
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="The name of the coupon as it will appear in the merchant admin." />
                        </th>
                        <td>
                            <asp:TextBox ID="Name" runat="server" Text="" Columns="20" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Static" ErrorMessage="Name is required." ControlToValidate="Name"></asp:RequiredFieldValidator>
                        </td>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="CouponCodeLabel" runat="server" Text="Coupon Code:" ToolTip="The coupon code that will be entered by the customer." />
                        </th>
                        <td>
                            <asp:TextBox ID="CouponCode" runat="server" Text="" Columns="10" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="CouponCodeRequired" runat="server" Text="*" Display="Static" ErrorMessage="Coupon code is required." ControlToValidate="CouponCode"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="DiscountAmountLabel" runat="server" Text="Discount Amount:" ToolTip="The amount of the discount given by the coupon." />
                        </th>
                        <td colspan="3">
                            <asp:TextBox ID="DiscountAmount" runat="server" Text="" Columns="4" MaxLength="8"></asp:TextBox>
                            <asp:DropDownList ID="DiscountType" runat="server">
                                <asp:ListItem Text="Percent (%)"></asp:ListItem>
                                <asp:ListItem Text="Fixed Amount"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="DiscountRequired" runat="server" Text="*" Display="Static" ErrorMessage="Discount is required." ControlToValidate="DiscountAmount"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="DiscountAmountRangeValidator" runat="server" ControlToValidate="DiscountAmount"
                                ErrorMessage="Discount Amount is not valid." MaximumValue="99999999" MinimumValue="0"
                                Type="Currency">*</asp:RangeValidator><br />
                        </td>
                    </tr>
                    <tr id="trValue" runat="server">
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="MaxValueLabel" runat="server" Text="Max Value:" ToolTip="The maximum discount that can be provided by this coupon." />
                        </th>
                        <td>
                            <asp:TextBox ID="MaxValue" runat="server" Text="" Columns="4" MaxLength="8"></asp:TextBox>
                            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="MaxValue"
                                ErrorMessage="Maximum Value  is not valid." MaximumValue="99999999" MinimumValue="0"
                                Type="Currency">*</asp:RangeValidator></td>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="MinPurchaseLabel" runat="server" Text="Min Purchase:" ToolTip="The minimum purchase required for the coupon to be valid." />
                        </th>
                        <td>
                            <asp:TextBox ID="MinPurchase" runat="server" Text="" Columns="4" MaxLength="8"></asp:TextBox>
                            <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="MinPurchase"
                                ErrorMessage="Minimum Purchase is not valid." MaximumValue="99999999" MinimumValue="0"
                                Type="Currency">*</asp:RangeValidator></td>
                    </tr>
                    <tr id="trQuantity" runat="server"> 
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="QuantityLabel" runat="server" Text="Quantity:" ToolTip="The quantity of an item that must be purchased to qualify for this coupon." />
                        </th>
                        <td>
                            <asp:TextBox ID="Quantity" runat="server" Text="" Columns="3" MaxLength="4"></asp:TextBox>
                            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="Quantity"
                                ErrorMessage="Quantity is not valid." MaximumValue="9999" MinimumValue="0" Type="Integer">*</asp:RangeValidator></td>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="RepeatCouponLabel" runat="server" Text="Repeat:" ToolTip="If checked, the coupon will be applied on a line item for each multiple of the required quantity.  If unchecked, the coupon is applied only once on a line item when the minimum quantity is met or exceeded." />
                        </th>
                        <td>
                            <asp:CheckBox ID="RepeatCoupon" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="StartDateLabel" runat="server" Text="Start Date:" ToolTip="The earliest date the coupon can be used.  If not specified, the coupon is effective immediately." /><br />
                        </th>
                        <td valign="top">
                            <uc:PickerAndCalendar ID="StartDate" runat="server" />
                        </td>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="EndDateLabel" runat="server" Text="End Date:" ToolTip="The last date the coupon can be used.  If not specified, the coupon is effective until you delete or otherwise alter it." />
                        </th>
                        <td valign="top">
                            <uc:PickerAndCalendar ID="EndDate" runat="server" /><asp:PlaceHolder ID="phEndDateValidator" runat="server" EnableViewState="false"></asp:PlaceHolder>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="MaximumUsesPerCustomerLabel" runat="server" Text="Uses per Customer:" ToolTip="The maximum number of times this coupon can be used by a single customer." />
                        </th>
                        <td>
                            <asp:TextBox ID="MaximumUsesPerCustomer" runat="server" Text="" Columns="10" MaxLength="4"></asp:TextBox>
                            <asp:RangeValidator ID="RangeValidator4" runat="server" ControlToValidate="MaximumUsesPerCustomer"
                                ErrorMessage="Maximum uses per customer is not valid." MaximumValue="9999" MinimumValue="0"
                                Type="Integer">*</asp:RangeValidator>
                        </td>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="MaximumUsesLabel" runat="server" Text="Max Uses:" ToolTip="The maximum number of times this coupon can be used by all customers." />
                        </th>
                        <td>
                            <asp:TextBox ID="MaximumUses" runat="server" Text="" Columns="10" MaxLength="4"></asp:TextBox>
                            <asp:RangeValidator ID="RangeValidator5" runat="server" ControlToValidate="MaximumUses"
                                ErrorMessage="Maximum uses is not valid." MaximumValue="9999" MinimumValue="0"
                                Type="Integer">*</asp:RangeValidator></td>
                    </tr>
                    <%--
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="ComboRuleLabel" runat="server" Text="Combine Rule:" ToolTip="Indicate whether or not this coupon can be combined with other coupons." />
                        </th>
                        <td valign="top" colspan="3">
                            <ajax:UpdatePanel ID="ComboRuleAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ComboRule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ComboRule_SelectedIndexChanged">
                                        <asp:ListItem Value="0" Text="Combine with Any" Selected="true"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Combine with Selected"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Exclude Selected"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CheckBoxList ID="CouponList" runat="server" DataTextField="Name" DataValueField="CouponId" AppendDataBoundItems="false" Visible="false"></asp:CheckBoxList>
                                    <asp:Label ID="EmptyCouponList" runat="server" Text="<i>(there are no other coupons defined)</i>" Visible="false"></asp:Label>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                    </tr>
                    --%>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="AllowCombineLabel" runat="server" Text="Allow Combine:" ToolTip="Indicate whether or not this coupon can be combined with other coupons." />
                        </th>
                        <td valign="top" colspan="3">
                            <asp:CheckBox ID="AllowCombine" runat="server" />
                        </td>
                    </tr>
                    <tr id="trProductRule" runat="server">
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="ProuductRestrictionsLabel" runat="server" Text="Product Rule:" ToolTip="Indicate whether any product restrictions apply to this coupon." />
                        </th>
                        <td colspan="3">
                            <ajax:UpdatePanel ID="ProductRuleAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ProductRule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ProductRule_SelectedIndexChanged">
                                        <asp:ListItem Value="0" Text="Valid for any product." Selected="true"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Valid only with selected products."></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Not valid for selected products."></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Label ID="ProductRuleHelpText" runat="server" Text="<br />(You can select products in the next step.)" Visible="false"></asp:Label>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                    </tr>
                    <tr id="trShipMethodRule" runat="server">
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="ShipMethodsLabel" runat="server" Text="Shipping Methods:" ToolTip="Indicate the shipping methods this coupon applies to." />
                        </th>
                        <td colspan="3">
                            <ajax:UpdatePanel ID="ShipMethodRuleAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ShipMethodRule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ShipMethodRule_SelectedIndexChanged">
                                        <asp:ListItem Value="0" Text="Valid for all methods" Selected="true"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Valid only for selected methods"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Not valid for selected methods"></asp:ListItem>
                                    </asp:DropDownList>
                                    <div style="padding-left:20px">
                                        <asp:CheckBoxList ID="ShipMethodList" runat="server" DataTextField="Name" DataValueField="ShipMethodId" AppendDataBoundItems="false" Visible="false" RepeatColumns="3"></asp:CheckBoxList>
                                    </div>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="GroupsLabel" runat="server" Text="Groups:" ToolTip="If desired, select the user groups that are allowed to use the coupon. If none are selected, a group restriction is not applied to the coupon." />
                        </th>
                        <td colspan="3">
                            <ajax:UpdatePanel ID="GroupRestrictionAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="UseGroupRestriction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UseGroupRestriction_SelectedIndexChanged">
                                        <asp:ListItem Text="All Groups" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="Selected Groups"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Panel ID="GroupListPanel" runat="server" Visible="false">
                                        <div style="padding-left:20px">
                                            <asp:CheckBoxList ID="GroupList" runat="server" DataTextField="Name" DataValueField="GroupId" RepeatColumns="3"></asp:CheckBoxList>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="validation">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </td>
        </tr>
        <tr>            
            <td class="submit">
                <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" CausesValidation="false" />
                <asp:Button ID="SaveButton" runat="server" Text="Next" OnClick="SaveButton_Click" />
            </td>
        </tr>
    </table>
</asp:Content>


