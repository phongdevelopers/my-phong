<%@ Page Language="C#" MasterPageFile="../Admin.master" CodeFile="AddGiftCertificate.aspx.cs" Inherits="Admin_Payment_AddGiftCertificate" Title="Add Gift Certificate"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc" %>
<%@ Import Namespace="CommerceBuilder.Payments" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
		<div class="caption">
			<h1><asp:Localize ID="Caption" runat="server" Text="Add Gift Certificate"></asp:Localize></h1>
		</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <table class="inputForm" cellpadding="4" cellspacing="0">
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="The name of the Gift Certificate as it will appear in the merchant admin." />
                        </th>
                        <td>
                            <asp:TextBox ID="Name" runat="server" Text="" Columns="20" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Static" ErrorMessage="Name is required." ControlToValidate="Name"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="HtmlValidator" runat="server" ControlToValidate="Name" ValidationExpression="[^<>]*" Text="*" ErrorMessage="You may not use < or > characters in the name."></asp:RegularExpressionValidator>
                        </td>    
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="BalanceLabel" runat="server" Text="Balance:" ToolTip="The Balance of the Gift Certificate." />
                        </th>
                        <td>
                            <asp:TextBox ID="Balance" runat="server" Text="" Columns="4" MaxLength="10"></asp:TextBox>                            
                            <asp:RequiredFieldValidator ID="BalanceRequired" runat="server" Text="*" Display="Static" ErrorMessage="Balance is required." ControlToValidate="Balance"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="PriceValidator" runat="server" ControlToValidate="Balance"
                            ErrorMessage="Balance should fall between 0.01 and 99999999.99" MaximumValue="99999999.99"
                            MinimumValue="0.01" Type="Currency">*</asp:RangeValidator>
                        </td>
                    </tr>
					<th class="rowHeader" nowrap>
						<cb:ToolTipLabel ID="ActivatedLable" runat="server" Text="Activated" ToolTip="When this is checked, serial number is generated for the gift certificate."></cb:ToolTipLabel>
					</th>
					<td >
						<asp:CheckBox ID="Activated" runat="server" />
					</td>
                    <tr>
                        <th class="rowHeader">
                            <cb:ToolTipLabel ID="ExpireDateLabel" runat="server" Text="Expire Date:" ToolTip="The date after which the the Gift Certificate will expire. If not specified, the Gift Certificate does not expire." />
                        </th>
                        <td valign="top">
                            <table cellpadding="0" cellspacing="0">
                                <tr><td><uc:PickerAndCalendar ID="ExpireDate" runat="server" /></td>
                                <td>&nbsp;<asp:PlaceHolder ID="phExpirationValidator" runat="server" EnableViewState="false"></asp:PlaceHolder></td></tr>
                            </table>
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
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
				<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
            </td>
        </tr>
    </table>
</asp:Content>
