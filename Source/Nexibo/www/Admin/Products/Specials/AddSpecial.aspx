<%@ Page Language="C#" MasterPageFile="../Product.master" CodeFile="AddSpecial.aspx.cs" Inherits="Admin_Products_Specials_AddSpecial" Title="Add Pricing Rule"  %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Add Pricing Rule for {0}"></asp:Localize></h1>
        </div>
    </div>
    <p align="justify"><asp:Localize ID="InstructionText" runat="server" Text="Only price is required.  You can optionally restrict the price by date or group."></asp:Localize></p>
    <table class="inputForm" cellpadding="3" cellspacing="0">
        <tr>
            <th class="rowHeader">
                <asp:Label ID="PriceLabel" runat="server" Text="Special Price:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="Price" runat="server" Text="" Columns="10" MaxLength="11"></asp:TextBox>
                <asp:RangeValidator ID="PriceValidator" runat="server" Text="*" ErrorMessage="Special Price must be between 0 and 99999999.99" MinimumValue="0" MaximumValue="99999999.99" Type="currency" ControlToValidate="Price"></asp:RangeValidator>
                <asp:RequiredFieldValidator ID="PriceRequired" runat="server" Text="*" Display="Dynamic" ErrorMessage="Price is required." ControlToValidate="Price"></asp:RequiredFieldValidator>
                <asp:Label ID="BasePriceMessage" runat="server" Text="(must be less than base price of {0:lc})"></asp:Label>
            </td>
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="StartDateLabel" runat="server" Text="Start Date:"></asp:Label>
            </th>
            <td>
                <uc:PickerAndCalendar ID="StartDate" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="EndDateLabel" runat="server" Text="End Date:"></asp:Label><br />
            </th>
            <td>
                <uc:PickerAndCalendar ID="EndDate" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="rowHeader" valign="top">
                <asp:Label ID="GroupsLabel" runat="server" Text="Groups:"></asp:Label><br />
            </th>
            <td>
                <asp:RadioButton ID="AllGroups" GroupName="radGroups" runat="server" Text="All Groups" Checked="true" /><br />
                <asp:RadioButton ID="SelectedGroups" GroupName="radGroups" runat="server" Text="Selected Groups:" /><br />
                <div style="padding-left:20px">
                    <asp:ListBox ID="Groups" runat="server" SelectionMode="Multiple" Rows="6" DataSourceId="GroupsDs" DataTextField="Name" DataValueField="GroupId">
                    </asp:ListBox>
                    <asp:ObjectDataSource ID="GroupsDs" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Users.GroupDataSource">
                    </asp:ObjectDataSource>
                </div>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />                
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
				<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
            </td>
        </tr>
    </table>

</asp:Content>

