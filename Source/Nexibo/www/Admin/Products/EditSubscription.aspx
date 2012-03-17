<%@ Page Language="C#" MasterPageFile="Product.master" CodeFile="EditSubscription.aspx.cs" Inherits="Admin_Products_EditSubscription" Title="Edit Subscription"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<script>
function addGroup()
{
    var name = prompt("New group name?");
    if ((name == null) || (name.length == 0)) return false;
    var c = document.getElementById('<%= NewName.ClientID %>');
    if (c == null) return false;
    c.value = name;
    return true;
}
</script>

    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Subscription Plan for '{0}'"></asp:Localize></h1>
        </div>
    </div>
    <ajax:UpdatePanel ID="PageAjax" runat="server" UpdateMode="conditional">
        <ContentTemplate>
            <asp:Panel ID="NoSubscriptionPlanPanel" runat="server">
                <p><asp:Localize ID="NoSubscriptionPlanDescription" runat="server" Text="This product does not have any associated subscription plan." EnableViewState="false"></asp:Localize></p>
                <asp:Button ID="ShowAddForm" runat="server" text="Add Subscription Plan" OnClick="ShowAddForm_Click"  EnableViewState="false" />
            </asp:Panel>
            <asp:Panel ID="SubscriptionPlanForm" runat="server" Visible="false">
                <table cellpadding="5" cellspacing="0" class="inputForm">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="SavedMessage" runat="server" Text="Subscription plan saved at {0}." EnableViewState="false" Visible="false" SkinID="GoodCondition"></asp:Label>
                            <asp:Label ID="ErrorMessageLabel" runat="server" Text="" EnableViewState="false" Visible="false" SkinID="ErrorCondition"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" width="165px">
                            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Subscription Name:" ToolTip="The name used for this subscription plan, as would show on your reports." EnableViewState="false"></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="Name" runat="server" MaxLength="100" EnableViewState="false" TabIndex="1" Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Static" 
                                ErrorMessage="You must enter a name for the subscription plan." ControlToValidate="Name" EnableViewState="false"></asp:RequiredFieldValidator><br />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" width="165px">
                            <cb:ToolTipLabel ID="SubscriptionGroupLabel" runat="server" Text="Subscriber Group:" ToolTip="If desired, select the group that users will be added to while their subscription is in effect." EnableViewState="false"></cb:ToolTipLabel>
                        </th>
                        <td>
                            <ajax:UpdatePanel ID="SubscriptionGroupAjax" runat="server" UpdateMode="conditional" EnableViewState="false">
                                <ContentTemplate>
                                    <asp:DropDownList ID="SubscriptionGroup" runat="server" AppendDataBoundItems="true"
                                        DataTextField="Name" DataValueField="GroupId" EnableViewState="false" TabIndex="2">
                                    </asp:DropDownList>
                                    <asp:LinkButton ID="NewGroupButton" runat="server" OnClientClick="return addGroup()" SkinID="Link" Text="new" CausesValidation="false" EnableViewState="false" OnClick="NewGroupButton_Click" TabIndex="3"></asp:LinkButton>
                                    <asp:HiddenField ID="NewName" runat="server" EnableViewState="false" />
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" width="165px">
                            <cb:ToolTipLabel ID="BillingOptionLabel" runat="server" Text="Billing Option:" ToolTip="Select the type of billing that will be used when this product is purchased." EnableViewState="false"></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:DropDownList ID="BillingOption" runat="server" EnableViewState="true" TabIndex="4" AutoPostBack="true" OnSelectedIndexChanged="BillingOption_SelectedIndexChanged">
                                <asp:ListItem Text="One-time charge of {0:lc}"></asp:ListItem>
                                <asp:ListItem Text="Recurring charge of {0:lc}"></asp:ListItem>
                                <asp:ListItem Text="Initial charge of {0:lc} with recurring charge"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" width="165px">
                            <cb:ToolTipLabel ID="SubscriptionExpirationLabel" runat="server" Text="Expiration:" ToolTip="Indicate how long after purchase this subscription will remain valid before it is expired.  If the subscription should not expire, leave the field blank." EnableViewState="true"></cb:ToolTipLabel>
                            <cb:ToolTipLabel ID="PaymentFrequencyLabel" runat="server" Text="Payment Frequency:" ToolTip="Indicate how often the recurring charge should be processed." EnableViewState="true"></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:Localize ID="PaymentFrequencyEveryLabel" runat="server" Text="every "></asp:Localize>
                            <asp:TextBox ID="PaymentFrequency" runat="server" MaxLength="3" EnableViewState="false" Width="40px" TabIndex="5"></asp:TextBox>
                            <asp:DropDownList ID="ddlPaymentFrequencyUnit" runat="server" EnableViewState="false" TabIndex="6">
                                <asp:ListItem Text="day(s)"></asp:ListItem>
                                <asp:ListItem Text="month(s)" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="trInitialCharge" runat="server">
                        <th class="rowHeader" width="165px">
                            <cb:ToolTipLabel ID="InitialChargeLabel" runat="server" Text="Initial Charge:" ToolTip="The amount that is charged for the first payment of the subscription plan." EnableViewState="false"></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:Label ID="InitialCharge" runat="server" Text="{0:lc}"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trRecurringCharge" runat="server">
                        <th class="rowHeader" width="165px">
                            <cb:ToolTipLabel ID="RecurringChargeLabel" runat="server" Text="Recurring Charge:" ToolTip="The amount that is charged for all recurring payments following the initial payment." EnableViewState="false"></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="RecurringCharge" runat="server" MaxLength="8" EnableViewState="true" Width="60px" TabIndex="8"></asp:TextBox>
                             (<cb:ToolTipLabel ID="TaxCodeLabel" runat="server" Text="Using Tax Code:" SkinID="FieldHeader" ToolTip="The initial charge uses the tax code assigned to the product.  The amount specified for the recurring charge will use this tax code." EnableViewState="false"></cb:ToolTipLabel>
                            <asp:DropDownList ID="TaxCode" runat="server" DataTextField="Name" DataValueField="TaxCodeId" 
                                AppendDataBoundItems="True" EnableViewState="false">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                            </asp:DropDownList>)
                        </td>
                    </tr>
                    <tr id="trNumberOfPayments" runat="server">
                        <th class="rowHeader" width="165px">
                            <cb:ToolTipLabel ID="NumberOfPaymentsLabel" runat="server" Text="Total Number of Payments:" ToolTip="Indicate the total number of payments (including the initial payment) that are made over the life of the subscription." EnableViewState="false"></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="NumberOfPayments" runat="server" MaxLength="3" EnableViewState="false" Width="40px" TabIndex="9"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NumberOfPaymentsRequired" runat="server" Text="*" Display="Dynamic" 
                                ErrorMessage="You must enter the total number of payments." ControlToValidate="NumberOfPayments" EnableViewState="false"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="NumberOfPaymentsRange" runat="server" Text="*" Display="Static" 
                                ErrorMessage="The total number of payments must be at least 1." ControlToValidate="NumberOfPayments" 
                                MinimumValue="1" MaximumValue="1000" Type="Integer" EnableViewState="false"></asp:RangeValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                            <asp:Panel ID="AddSubscriptionPlanButtons" runat="server" Visible="false">                               
                                <asp:Button ID="AddButton" runat="server" text="Save" OnClick="AddButton_Click" EnableViewState="false" TabIndex="11"/>
								<asp:Button ID="CancelButton" runat="server" text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" EnableViewState="false" TabIndex="10" />
                            </asp:Panel>
                            <asp:Panel ID="EditSubscriptionPlanButtons" runat="server" Visible="false">
                                <asp:Button ID="DeleteButton" runat="server" text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" EnableViewState="false" TabIndex="10" OnClientClick="return confirm('Are you sure you want to delete this subscription plan?')" />
                                <asp:Button ID="UpdateButton" runat="server" text="Save" OnClick="UpdateButton_Click" EnableViewState="false" TabIndex="11" />
                            </asp:Panel>            
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>