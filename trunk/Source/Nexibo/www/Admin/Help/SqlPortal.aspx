<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="SqlPortal.aspx.cs" Inherits="Admin_Help_SqlPortal" Title="SQL Portal" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="SQL Portal"></asp:Localize></h1>
    	</div>
    </div>
    <asp:UpdatePanel ID="PageAjax" runat="server">
        <ContentTemplate>
            <div style="padding:4px;">
                <asp:Localize ID="SqlPortalHelpText" runat="server" Text="WARNING: The sql portal is an extremely powerful tool.  If you choose to use this tool, be warned that AbleCommerce is not responsible for any negative consequences such as loss of data.  Always maintain current backups of your database and website."></asp:Localize><br />
                <asp:Label ID="SqlQueryLabel" runat="server" Text="Query Text:" SkinID="FieldHeader"></asp:Label><br />
                <asp:TextBox ID="SqlQuery" runat="server" TextMode="MultiLine" Width="600px" Height="100px"></asp:TextBox><br />
                <asp:Button ID="ExecuteButton" runat="server" Text="Execute" OnClick="ExecuteButton_Click"  OnClientClick="this.value='Executing...';this.enabled= false;" /><br /><br />
                <asp:PlaceHolder ID="phQueryResult" runat="server"></asp:PlaceHolder>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>