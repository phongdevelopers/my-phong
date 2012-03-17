<%@ Page Language="C#" MasterPageFile="Order.master" CodeFile="CancelOrder.aspx.cs" Inherits="Admin_Orders_CancelOrder" Title="Cancel Order" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Cancel Order"></asp:Localize></h1>
        </div>
    </div>
    <table width="90%" align="center">
    <tr>
        <td>
            <asp:Label ID="CommentLabel" runat="server" Text="Enter a comment or explanation for the order cancellation:"></asp:Label><br />
            <asp:TextBox ID="Comment" runat="server" TextMode="MultiLine" Rows="4" Columns="50"></asp:TextBox><br />
            <asp:CheckBox ID="IsPrivate" runat="server" Text="Make comment private." /><br /><br />
            <asp:HyperLink ID="BackButton" runat="server" Text="&laquo; Back" SkinID="Button" />
            <asp:Button ID="CancelButton" runat="server" Text="Cancel Order" OnClick="CancelButton_Click" />
        </td>
    </tr>
</table>
</asp:Content>

