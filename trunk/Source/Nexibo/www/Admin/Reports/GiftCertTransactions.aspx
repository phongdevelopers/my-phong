<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"  Title="View Gift Certificate Transactions"  CodeFile="GiftCertTransactions.aspx.cs" Inherits="Admin_Reports_GiftCertTransactions"%>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Gift Certificate Transactions for '{0}'"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top" align="center">
    
			<cb:SortedGridView ID="TransactionGrid" runat="server" DataKeyNames="GiftCertificateTransactionId"  Width="100%" SkinID="PagedList" AutoGenerateColumns="false">
				<Columns>
					<asp:TemplateField HeaderText="Transaction Date">
						<ItemStyle VerticalAlign="Top" HorizontalAlign="Center"/>
						<ItemTemplate>
							<asp:Label ID="TransactionDate" runat="server" Text='<%# Eval("TransactionDate", "{0:g}") %>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Amount">
						<ItemStyle VerticalAlign="Top" HorizontalAlign="Center"/>
						<ItemTemplate>
							<asp:Label ID="Amount" runat="server" Text='<%#Eval("Amount","{0:lc}")%>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Order #">
						<ItemStyle VerticalAlign="Top" HorizontalAlign="Center"/>
						<ItemTemplate>
							<asp:HyperLink ID="OrderLink" runat="server" Text='<%# GetOrderNumber(Container.DataItem)%>' NavigateUrl='<%# GetOrderLink(Container.DataItem)%>' Visible="<%# HasOrder(Container.DataItem) %>" >
							</asp:HyperLink>
							<asp:Label ID="OrderLinkNA" runat="server" Text='-' Visible="<%# !HasOrder(Container.DataItem) %>"></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Description">
						<ItemStyle VerticalAlign="Top" HorizontalAlign="Left"/>
						<ItemTemplate>
							<asp:Label ID="Description" runat="server" Text='<%#Eval("Description")%>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
				<EmptyDataTemplate>
					<asp:Label ID="EmptyMessage" runat="server" Text="No transactions are associated with gift certificate."></asp:Label>
				</EmptyDataTemplate>
			</cb:SortedGridView>  
                
            </td>
        </tr>
    </table>    

</asp:Content>
