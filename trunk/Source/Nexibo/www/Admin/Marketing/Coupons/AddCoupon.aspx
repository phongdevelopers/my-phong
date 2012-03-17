<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="AddCoupon.aspx.cs" Inherits="Admin_Marketing_Coupons_AddCoupon" Title="Add Coupon : Select Coupon Type" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Add Coupon (Select Coupon Type)"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2">
                <asp:Label ID="InstructionText" runat="server" Text="Select the type of the coupon to add:"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top" align="right" width="20%">
                <asp:RadioButton ID="OrderCoupon" runat="server" GroupName="CouponType" Checked="true" />
            </td>
            <td>
                <asp:Label ID="OrderCouponLabel" runat="server" Text="Order Coupon" SkinID="FieldHeader"></asp:Label><br />
                <asp:Label ID="OrderCouponHelpText" runat="server" Text="Provide a discount on the entire order.  For example, 10% off the entire order." SkinID="HelpText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top" align="right">
                <asp:RadioButton ID="ProductCoupon" runat="server" GroupName="CouponType" />
            </td>
            <td>
                <asp:Label ID="ProductCouponLabel" runat="server" Text="Product Coupon" SkinID="FieldHeader"></asp:Label><br />
                <asp:Label ID="ProductCouponHelpText" runat="server" Text="Provide a discount for a specific line item.  For example, buy one get one free." SkinID="HelpText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top" align="right">
                <asp:RadioButton ID="ShippingCoupon" runat="server" GroupName="CouponType" />
            </td>
            <td>
                <asp:Label ID="ShippingCouponLabel" runat="server" Text="Shipping Coupon" SkinID="FieldHeader"></asp:Label><br />
                <asp:Label ID="ShippingCouponHelpText" runat="server" Text="Provides a discount on shipping charges for an order.  For example, a free shipping coupon." SkinID="HelpText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="2">
                <br />
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                <asp:Button ID="NextButton" runat="server" Text="Next" OnClick="NextButton_Click" />
            </td>
        </tr>
    </table>
</asp:Content>

