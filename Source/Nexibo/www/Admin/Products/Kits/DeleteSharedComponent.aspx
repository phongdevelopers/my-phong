<%@ Page Language="C#" MasterPageFile="../Product.master" CodeFile="DeleteSharedComponent.aspx.cs" Inherits="Admin_Products_Kits_DeleteSharedComponent" Title="Delete Shared Component"  %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Delete Component"></asp:Localize></h1>
        </div>
    </div>
    <asp:Label ID="InstructionText" runat="server" Text="The component you want to delete is attached to more than one kit.  Please select the appropriate action below, or click Cancel if you change your mind."></asp:Label><br /><br />
    <asp:RadioButton ID="Detach" runat="server" GroupName="DeleteAction" Text="Remove the component from the current kit only.  Leave the other kits alone." Checked="true" /><br /><br />
    <asp:RadioButton ID="Delete" runat="server" GroupName="DeleteAction" Text="Delete this component completely from all kits:" /><br />
    <asp:Repeater ID="KitList" runat="server">
        <HeaderTemplate><ul></HeaderTemplate>
        <ItemTemplate>
            <li><asp:Literal ID="Name" runat="server" Text='<%#Eval("Name")%>'></asp:Literal></li>
        </ItemTemplate>
        <FooterTemplate></ul></FooterTemplate>
    </asp:Repeater>
    <br /><asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
    <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" OnClientClick="javascript: return confirm('Are you sure you want to delete the component?');" />
</asp:Content>

