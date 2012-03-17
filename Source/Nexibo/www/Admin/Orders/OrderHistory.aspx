<%@ Page Language="C#" MasterPageFile="Order.master" CodeFile="OrderHistory.aspx.cs" Inherits="Admin_Orders_OrderHistory" Title="Order History and Notes"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Order History and Notes"></asp:Localize></h1></div>
    </div>
    <div width="100%" style="margin:4px 0px;">
        <asp:GridView ID="OrderNotesGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderNoteId" 
            OnRowEditing="OrderNotesGrid_RowEditing" OnRowCancelingEdit="OrderNotesGrid_RowCancelingEdit" OnRowUpdating="OrderNotesGrid_RowUpdating" 
            OnRowDeleting="OrderNotesGrid_RowDeleting" CellSpacing="0" CellPadding="4" Width="100%" SkinID="Summary">
            <Columns>
                <asp:TemplateField HeaderText="Created">
                    <ItemStyle VerticalAlign="Top" HorizontalAlign="Left" BorderWidth="0" />
                    <headerstyle horizontalalign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="CreatedDate" runat="server" Text='<%# Eval("CreatedDate") %>'></asp:Label><br />
                        <asp:Label ID="Author" runat="server" Text='<%# Eval("User.UserName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comment">
                    <ItemStyle HorizontalAlign="Left" />
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="Comment" runat="server" Text='<%# Eval("Comment") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="EditComment" runat="server" Text='<%# Eval("Comment") %>' TextMode="MultiLine" Rows="4" Columns="40"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Private">
                    <ItemStyle HorizontalAlign="Center" BorderWidth="0" />
                    <ItemTemplate>
                        <asp:CheckBox ID="IsPrivate" runat="server" Enabled="false" Checked='<%# Eval("IsPrivate") %>'></asp:CheckBox>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="EditIsPrivate" runat="server" Checked='<%# Eval("IsPrivate") %>'></asp:CheckBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:ImageButton ID="EditLink" runat="Server" CommandName="Edit" Text="Edit" Visible='<%#!(bool)Eval("IsSystem")%>' SkinID="EditIcon" ToolTip="Edit"></asp:ImageButton>
                        <asp:ImageButton ID="DeleteLink" runat="Server" CommandName="Delete" Text="Delete" Visible='<%#!(bool)Eval("IsSystem")%>' OnClientClick="return confirm('Are you sure you want to delete this note?')" SkinID="DeleteIcon" ToolTip="Delete"></asp:ImageButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="SaveLink" runat="Server" CommandName="Update" Text="Save" ToolTip="Save" SkinID="SaveIcon"></asp:ImageButton>
                        <asp:ImageButton ID="CancelLink" runat="Server" CommandName="Cancel" Text="Cancel" ToolTip="Cancel" SkinID="CancelIcon"></asp:ImageButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <asp:Panel ID="AddPanel" runat="server">
        <table class="inputForm">
            <tr class="sectionHeader">
                <th style="text-align:left">
                    <asp:Localize ID="AddCommentCaption" runat="server" Text="Add Comment"></asp:Localize>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="AddComment" runat="server" TextMode="MultiLine" Rows="4" Columns="50"></asp:TextBox><br />
                    <asp:CheckBox ID="AddIsPrivate" runat="server" Text="Make comment private." />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="AddButton" runat="server" Text="Add Comment" OnClick="AddButton_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>