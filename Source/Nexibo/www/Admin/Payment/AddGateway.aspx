<%@ Page Language="C#" MasterPageFile="../Admin.master" CodeFile="AddGateway.aspx.cs" Inherits="Admin_Payment_AddGateway" Title="Add Gateway"  %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Add Gateway"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top">
	            <asp:DataList ID="ProviderList" runat="server" OnItemCommand="ProviderList_ItemCommand" RepeatDirection="Horizontal" RepeatColumns="2" CellPadding="10">
	                <ItemStyle HorizontalAlign="Left" VerticalAlign="top" />
	                <ItemTemplate>
	                    <div style="margin-bottom:8px;">
                            <asp:LinkButton ID="ProviderLink" runat="server" CommandName="Add" CommandArgument='<%# GetClassId(Container.DataItem) %>' SkinID="Button">
                                <asp:Label ID="ProviderName" runat="server" Text='<%#Eval("Name")%>'></asp:Label>
                            </asp:LinkButton>
                        </div>
                        <asp:Label ID="SupportedTransactionsLabel" runat="server" Text="Supported Transactions: " SkinID="FieldHeader"></asp:Label>
                        <asp:Label ID="SupportedTransactions" runat="server" Text='<%#GetSupportedTransactions(Container.DataItem)%>'></asp:Label>
                    </ItemTemplate>                    
	            </asp:DataList>	            
	            <asp:Panel ID="EmptyDataSection" runat="server" Visible="false" CssClass="modalPopup" >
	                <asp:Label ID="EmptyDataMessage" runat="server" Text="All Payment Gateways have already been added. There are no more gateways to add."></asp:Label>
	            </asp:Panel>
	        </td>
	    </tr>
	    <tr>
	        <td>
	            <asp:HyperLink ID="CancelLink" runat="server" NavigateUrl="Gateways.aspx" Text="Cancel" SkinID="Button"></asp:HyperLink>
	        </td>
	    </tr>
    </table>
</asp:Content>

