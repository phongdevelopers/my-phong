<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddWrapStyleDialog.ascx.cs" Inherits="Admin_Products_GiftWrap_AddWrapStyleDialog" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="AddWrapStyle" />
<asp:Label ID="AddedMessage" runat="server" Text="Wrap style {0} added." SkinID="GoodCondition" EnableViewState="false" Visible="false"></asp:Label>
<table class="inputForm" width="100%">
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Name" ToolTip="Name of the wrapping style"></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="Name" runat="server" MaxLength="50"></asp:TextBox>
            <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name" Text="*" ErrorMessage="Name is required." ValidationGroup="AddWrapStyle"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="ThumbnailUrlLabel" runat="server" Text="Thumbnail:" AssociatedControlID="ThumbnailUrl" ToolTip="Thumbnail image of the wrapping style"></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="ThumbnailUrl" runat="server" MaxLength="250" Width="200px"></asp:TextBox>&nbsp;
           <asp:ImageButton ID="BrowseThumbnailUrl" runat="server" SkinID="FindIcon" AlternateText="Browse" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="ImageUrlLabel" runat="server" Text="Image:" AssociatedControlID="ImageUrl" ToolTip="Enlarged image of the wrapping style"></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="ImageUrl" runat="server" MaxLength="250" Width="200px"></asp:TextBox>&nbsp;
           <asp:ImageButton ID="BrowseImageUrl" runat="server" SkinID="FindIcon" AlternateText="Browse" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="PriceLabel" runat="server" Text="Price:" AssociatedControlID="Price" ToolTip="Amount charged when the wrapping style is used"></cb:ToolTipLabel>
        </th>
        <td>
            <asp:TextBox ID="Price" runat="server" Columns="6" MaxLength="8"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <cb:ToolTipLabel ID="TaxCodeLabel" runat="server" Text="Tax Code:" AssociatedControlID="TaxCode" ToolTip="Tax code associated with the price of this wrapping style"></cb:ToolTipLabel>
        </th>
        <td>
            <asp:DropDownList ID="TaxCode" runat="server" DataSourceID="TaxCodeDs" DataTextField="Name"
                DataValueField="TaxCodeId" AppendDataBoundItems="True">
                <asp:ListItem Value="" Text=""></asp:ListItem>
            </asp:DropDownList>
            <asp:ObjectDataSource ID="TaxCodeDs" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="LoadForStore" TypeName="CommerceBuilder.Taxes.TaxCodeDataSource">
            </asp:ObjectDataSource>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ValidationGroup="AddWrapStyle" />
        </td>
    </tr>
</table>

