<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Configure.aspx.cs" Inherits="Admin_Products_DigitalGoods_SerialKeyProviders_DefaultProvider_Configure" Title="Configure Serial Keys"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Import Namespace="System.IO" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Serial Keys For '{0}'"></asp:Localize></h1>
        </div>
    </div>
    <asp:GridView ID="SerialKeysGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="SerialKeyId"
        SkinID="PagedList" AllowPaging="False" AllowSorting="false" OnRowDeleting="SerialKeysGrid_RowDeleting">
        <Columns>
            <asp:BoundField DataField="SerialKeyData" HeaderText="Serial Key Data" />
            <asp:TemplateField>
                <ItemTemplate>                    
                    <asp:HyperLink ID="Edit" runat="server" NavigateUrl='<%# string.Format("EditSerialKey.aspx?CategoryId={0}&ProductId={1}&DigitalGoodId={2}&SerialKeyId={3}", _CategoryId, _ProductId, _DigitalGoodId, Eval("SerialKeyId")) %>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" ToolTip="Edit" /></asp:HyperLink>
                    <asp:ImageButton ID="Delete" runat="server" CommandName="Delete" AlternateText="Delete" ToolTip="Delete" SkinID="DeleteIcon" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="EmptyMessage" runat="server" Text="There are no serial keys attached to this digital good."></asp:Label>
        </EmptyDataTemplate>
    </asp:GridView><br />
    <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" />
    <asp:Button ID="BackButton" runat="server" Text="Close" OnClick="BackButton_Click" />
</asp:Content>
