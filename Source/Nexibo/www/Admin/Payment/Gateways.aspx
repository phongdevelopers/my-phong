<%@ Page Language="C#" MasterPageFile="../Admin.master" CodeFile="Gateways.aspx.cs" Inherits="Admin_Payment_Gateways" Title="Payment Gateways"  %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Payment Gateways"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top" align="center">
                <asp:GridView ID="GatewayGrid" runat="server" DataKeyNames="PaymentGatewayId" DataSourceID="PaymentGatewayDs" AutoGenerateColumns="false"
                            Width="450px" SkinID="PagedList">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-HorizontalAlign="left" ReadOnly="true" ItemStyle-HorizontalAlign="left" />
                        <asp:TemplateField HeaderText="Payment Methods">
                            <HeaderStyle HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="PaymentMethods" runat="server" Text='<%#GetPaymentMethods(Container.DataItem)%>'></asp:Label><br />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemStyle HorizontalAlign="Center" Width="54px" Wrap="false" />
                            <ItemTemplate>
                                <asp:HyperLink ID="EditButton" runat="server" NavigateUrl='<%#Eval("PaymentGatewayId", "EditGateway.aspx?PaymentGatewayId={0}") %>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" /></asp:HyperLink>
                                <asp:ImageButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" SkinID="DeleteIcon" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' AlternateText="Delete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="EmptyMessage" runat="server" Text="No gateways are defined for your store."></asp:Label>
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                <asp:HyperLink ID="AddGateway" runat="server" Text="Add Gateway" NavigateUrl="AddGateway.aspx" SkinID="Button"></asp:HyperLink>
                <asp:HyperLink ID="MethodsLink" runat="server" Text="Edit Methods" NavigateUrl="Methods.aspx" SkinID="Button"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="PaymentGatewayDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForCriteria" TypeName="CommerceBuilder.Payments.PaymentGatewayDataSource" SelectCountMethod="CountForCriteria" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Payments.PaymentGateway" DeleteMethod="Delete">         
		  <SelectParameters>
			<asp:Parameter Name="sqlCriteria" DefaultValue="" />  
		  </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

