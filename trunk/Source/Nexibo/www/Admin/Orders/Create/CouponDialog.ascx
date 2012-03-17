<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CouponDialog.ascx.cs" Inherits="Admin_Orders_Create_CouponDialog " EnableViewState="false" %>
<div class="section">
    <div class="header">
        <h2 class="commonicon"><asp:Localize ID="Caption" runat="server" Text="Coupon or Promotional Code" EnableViewState="false"></asp:Localize></h2>
    </div>
    <asp:Panel ID="ContentPanel" runat="server" CssClass="content" DefaultButton="ApplyCouponButton">
        <asp:Label ID="InvalidCouponMessage" runat="server" Text="Invalid coupon code.<br /><br />" Visible="false" SkinID="ErrorCondition" EnableViewState="false"></asp:Label>
        <asp:Label ID="NotCombineCouponRemoveMessage" runat="server" Text="The coupon {0} can not be combined with other coupons. Coupons {1} have been removed.<br /><br />" Visible="false" SkinID="ErrorCondition" EnableViewState="false"></asp:Label>
        <asp:Label ID="CombineCouponRemoveMessage" runat="server" Text="The coupon {0} can not be combined with coupon {1}. Coupon {2} has been removed.<br /><br />" Visible="false" SkinID="ErrorCondition" EnableViewState="false"></asp:Label>
        <asp:Label ID="ValidCouponMessage" runat="server" Text="Coupon accepted.<br /><br />" Visible="false" SkinID="GoodCondition" EnableViewState="false"></asp:Label>
        <asp:TextBox ID="CouponCode" runat="server" Width="110px" MaxLength="100" EnableViewState="false"></asp:TextBox>
        <asp:Button ID="ApplyCouponButton" runat="server" Text="Apply" OnClick="ApplyCouponButton_Click" CausesValidation="false" EnableViewState="false"  />
    </asp:Panel>
</div>