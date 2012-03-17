<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddProvinceDialog.ascx.cs" Inherits="Admin_Shipping_Countries_AddProvinceDialog" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<div class="section">
    <div class="header">
        <h2><asp:Localize ID="AddCaption" runat="server" Text="Add State or Province" /></h2>
    </div>
    <div class="content">
        <table class="inputForm">
            <tr>
                <th class="rowHeader" nowrap>
                    <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="Enter the full name of the state or province as it should appear." />
                </th>
                <td>
                    <asp:TextBox ID="Name" runat="server" MaxLength="50" width="150px"></asp:TextBox>
                    <cb:RequiredRegularExpressionValidator ID="RequiredRegularExpressionValidator1" runat="server" ControlToValidate="Name"
                        Display="Static" ErrorMessage="Name must be between 1 and 50 characters in length." Text="*"
                        ValidationGroup="AddProvince" ValidationExpression=".{1,50}" Required="true">
                    </cb:RequiredRegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" nowrap>
                    <cb:ToolTipLabel ID="ProvinceCodeLabel" runat="server" Text="Code:" ToolTip="An abbreviation or code used to represent the state or province, such as CA for California." />
                </th>
                <td id="tdProvinceCode" runat="server">
                    <asp:TextBox ID="ProvinceCode" runat="server" Width="40px" MaxLength="4"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="AddProvince" />
                    <asp:Label ID="AddedMessage" runat="server" Text="{0} added." SkinID="GoodCondition" EnableViewState="false" Visible="false"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="submit">
                    <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ValidationGroup="AddProvince" />
                </td>
            </tr>
        </table>
    </div>
</div>

