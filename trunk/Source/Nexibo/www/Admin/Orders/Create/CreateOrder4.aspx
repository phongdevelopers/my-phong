<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="CreateOrder4.aspx.cs" Inherits="Admin_Orders_Create_CreateOrder4" Title="Place Order" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/Orders/Create/MiniBasket.ascx" TagName="MiniBasket" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/CouponDialog.ascx" TagName="CouponDialog" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/CreditCardPaymentForm.ascx" TagName="CreditCardPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/CheckPaymentForm.ascx" TagName="CheckPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/PayPalPaymentForm.ascx" TagName="PayPalPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/MailPaymentForm.ascx" TagName="MailPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/PhoneCallPaymentForm.ascx" TagName="PhoneCallPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/PurchaseOrderPaymentForm.ascx" TagName="PurchaseOrderPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/GiftCertificatePaymentForm.ascx" TagName="GiftCertificatePaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/ZeroValuePaymentForm.ascx" TagName="ZeroValuePaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Admin/Orders/Create/DeferPaymentForm.ascx" TagName="DeferPaymentForm" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<ajax:UpdatePanel ID="BasketAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>  
        <div class="pageHeader">
            <div class="caption">
                <h1>
                    <asp:Localize ID="Caption" runat="server" Text="Create Order for {0} (Step 4 of 4)"></asp:Localize>
                </h1>
            </div>
        </div>
        <table width="100%">
            <tr>
                <td valign="top">
                    <table class="inputForm" cellpadding="3" width="100%">
                        <tr>
                            <td class="sectionHeader">
                                <asp:Localize ID="BillingAddressCaption" runat="server" Text="Customer Info"></asp:Localize>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td valign="top" width="200px">
                                            <asp:Label ID="BillToLabel" runat="server" Text="BILL TO:" SkinID="FieldHeader"></asp:Label><br />
                                            <asp:Literal ID="BillToAddress" runat="server"></asp:Literal>
                                        </td>
                                        <td id="tdShipTo" runat="server" valign="top" width="200px">
                                            <asp:Label ID="ShipToLabel" runat="server" Text="SHIP TO:" SkinID="FieldHeader"></asp:Label><br />
                                            <asp:Literal ID="ShipToAddress" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HyperLink ID="EditAddressesLink" runat="server" NavigateUrl="CreateOrder3.aspx" Text="Edit Info" SkinID="Button" EnableViewState="false"></asp:HyperLink>
                                <br /><br />
                            </td>
                        </tr>
                    </table>
                    <asp:PlaceHolder ID="RegisterPanel" runat="server" EnableViewState="false" Visible="false">
                        <table class="inputForm" cellpadding="3" width="100%">
                            <tr>
                                <td class="sectionHeader" colspan="2">
                                    <asp:Localize ID="RegisterCaption" runat="server" Text="Account Registration"></asp:Localize>
                                </td>
                            </tr>
                        </table>
                        <div style="padding:3px">
                            <asp:Localize ID="RegisteredUserHelpText" runat="server">
                                The email address specified for this order appears to match an existing user account.  Check the box below if you want this order to be linked with the specified account.
                            </asp:Localize>
                            <asp:Localize ID="UnregisteredUserHelpText" runat="server">
                                You are placing an order for a new or unregistered user.  Use the form below to create an account so that order history can be retrieved by the customer at a later date.
                            </asp:Localize><br /><br />
                            <asp:CheckBox ID="CreateAccount" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="CreateAccount_CheckedChanged" />
                            <asp:Label ID="CreateAccountLabel" runat="server" Text="Create Account" AssociatedControlID="CreateAccount" SkinID="fieldHeader"></asp:Label>
                            <asp:Label ID="LinkAccountLabel" runat="server" Text="Link Order to Account" AssociatedControlID="CreateAccount" SkinID="fieldHeader"></asp:Label>
                        </div>
                        <table class="inputForm">
                            <tr>
                                <th class="rowHeader">
                                    <asp:Localize ID="AccountUserNameLabel" runat="server" Text="Login name:"></asp:Localize>
                                </th>
                                <td>
                                    <asp:Literal ID="AccountUserName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Localize ID="AccountEmailLabel" runat="server" Text="Email:"></asp:Localize>
                                </th>
                                <td>
                                    <asp:Literal ID="AccountEmail" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr id="trAccountPassword" runat="server" enableviewstate="false">
                                <th class="rowHeader">
                                    <asp:Label ID="PasswordLabel" runat="server" Text="Initial Password:" AssociatedControlID="Password"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Password" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" Text="*"
                                        ErrorMessage="Password is required to create an account." Display="Static" ControlToValidate="Password"
                                        EnableViewState="False" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr id="trForceExpiration" runat="server" enableviewstate="false">
                                <th class="rowHeader">&nbsp;</th>
                                <td>
                                    <asp:CheckBox ID="ForceExpiration" runat="server" Text="User must change password at next login" Checked="true" />
                                </td>
                            </tr>
                        </table><br />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ShippingMethodPanel" runat="server">
                        <table class="inputForm" cellpadding="3" width="100%">
                            <tr>
                                <td class="sectionHeader" colspan="4">
                                    <asp:Localize ID="ShippingMethodCaption" runat="server" Text="Shipping Method"></asp:Localize>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:PlaceHolder ID="MultipleShipmentsMessage" runat="server" EnableViewState="false" Visible="false">
                                        <br />Your order contains items that must be sent in more than one shipment.  View the order details at the bottom of this page for the contents of each shipment.<br /><br />
                                    </asp:PlaceHolder>
                                    <asp:Repeater ID="ShipmentList" runat="server" OnItemDataBound="ShipmentList_ItemDataBound">
                                        <ItemTemplate>
                                            <asp:PlaceHolder ID="ShipmentCounter" runat="server" Visible='<%# (ShipmentCount > 1) %>'><b>Shipment <%# (Container.ItemIndex + 1) %>:&nbsp; </b></asp:PlaceHolder>
                                            <asp:Label ID="ShipMethodListLabel" runat="server" Visible='<%# (ShipmentCount == 1) %>' Text="Select Method:" SkinID="FieldHeader"></asp:Label>&nbsp;
                                            <asp:DropDownList ID="ShipMethodList" runat="server" DataTextField="Name" DataValueField="ShipMethodId" AutoPostBack="true" 
                                                AppendDataBoundItems="true" OnSelectedIndexChanged="ShipMethodList_SelectedIndexChanged">
                                                <asp:ListItem Text=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="ShipMethodRequired" runat="server" Text="*"
                                                ErrorMessage="You must select a shipping method." Display="Static" ControlToValidate="ShipMethodList"
                                                EnableViewState="False" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                            <br />
                                            <asp:PlaceHolder ID="ShipMessagePanel" runat="server">
                                                <asp:Label ID="ShipMessageLabel" runat="server" Text="Delivery Instructions?" SkinID="FieldHeader"></asp:Label>&nbsp;
                                                <asp:TextBox ID="ShipMessage" runat="server" Text="" MaxLength="200" Width="200px"></asp:TextBox>
                                            </asp:PlaceHolder>
                                            <br /><br />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Localize ID="ShipMethodErrorMessage" runat="server" Visible="false" EnableViewState="false" Text="There are no shipping methods available to the selected destination(s)."></asp:Localize>
                                </td>
                            </tr>
                        </table>
                    </asp:PlaceHolder>
                    <asp:ValidationSummary ID="PaymentValidationSummary" runat="server" EnableViewState="false" ValidationGroup="OPC" />
                    <table class="inputForm" cellpadding="3" width="100%">
                        <tr>
                            <td class="sectionHeader" colspan="2">
                                <asp:Localize ID="PaymentMethodCaption" runat="server" Text="Payment Method (Order Total: {0:lc})" EnableViewState="false"></asp:Localize> 
                            </td>
                        </tr>
                        <tr>
                            <td id="tdPaymentMethodList" runat="server" valign="top" width="150px">
                                <asp:RadioButtonList ID="PaymentMethodList" runat="server" DataTextField="Value" DataValueField="Key" AutoPostBack="true"></asp:RadioButtonList>
                            </td>
                            <td align="left" valign="top">
                                <asp:PlaceHolder ID="phPaymentForms" runat="server" EnableViewState="False"></asp:PlaceHolder>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top" width="300">
                    <uc:MiniBasket ID="MiniBasket1" runat="server" /><br />
                    <uc:CouponDialog ID="CouponDialog1" runat="server" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>