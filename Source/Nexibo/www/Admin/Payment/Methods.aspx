<%@ Page Language="C#" MasterPageFile="../Admin.master" CodeFile="Methods.aspx.cs" Inherits="Admin_Payment_Methods" Title="Payment Methods" %>
<%@ Register Src="AddPaymentMethodDialog.ascx" TagName="AddPaymentMethodDialog" TagPrefix="uc" %>
<%@ Register Src="EditPaymentMethodDialog.ascx" TagName="EditPaymentMethodDialog" TagPrefix="uc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
            	<div class="caption">
            		<h1><asp:Localize ID="Caption" runat="server" Text="Payment Methods"></asp:Localize></h1>
            	</div>
            </div>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td width="50%" valign="top" class="itemList">
                        <div>
                            <asp:GridView ID="PaymentMethodGrid" runat="server" DataKeyNames="PaymentMethodId" DataSourceID="PaymentMethodDs" AutoGenerateColumns="false"
                                 width="100%" SkinID="PagedList" OnRowEditing="PaymentMethodGrid_RowEditing" OnRowCancelingEdit="PaymentMethodGrid_RowCancelingEdit"
                                OnRowCommand="PaymentMethodGrid_RowCommand" >
                                <Columns>
                                    <asp:TemplateField HeaderText="Order">
                                        <ItemStyle HorizontalAlign="center" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="UpButton" runat="server" SkinID="UpIcon" CommandName="MoveUp" CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Up" />
                                            <asp:ImageButton ID="DownButton" runat="server" SkinID="DownIcon" CommandName="MoveDown" CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Down" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-HorizontalAlign="left" ReadOnly="true" />
                                    <asp:TemplateField HeaderText="Gateway">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Label ID="Gateway" runat="server" Text='<%#Eval("PaymentGateway.Name")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit" SkinID="EditIcon" AlternateText="Edit" />
                                            <asp:ImageButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" SkinID="DeleteIcon" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' AlternateText="Delete" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:ImageButton ID="CancelButton" runat="server" CausesValidation="False" CommandName="Cancel" SkinID="CancelIcon" AlternateText="Cancel" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div style="margin-top:4px;text-align:center">
                                <asp:HyperLink ID="GatewaysLink" runat="server" Text="Edit Gateways" SkinID="Link" NavigateUrl="Gateways.aspx"></asp:HyperLink>
                            </div>
                        </div>
                    </td>
                    <td width="50%" class="detailPanel" style="padding-top:-10px;">
                        <asp:Panel ID="AddPanel" runat="server" CssClass="section">
                            <div class="header">
                                <h2 class="addpaymentmethod"><asp:Localize ID="AddCaption" runat="server" Text="Add Payment Method" /></h2>
                            </div>
                            <div class="content">
                                <uc:AddPaymentMethodDialog ID="AddPaymentMethodDialog1" runat="server" />
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="EditPanel" runat="server" CssClass="section" Visible="false">
                            <div class="header">
                                <h2><asp:Localize ID="EditCaption" runat="server" Text="Edit '{0}'" EnableViewState="false" /></h2>
                            </div>
                            <div class="content">
                                <uc:EditPaymentMethodDialog ID="EditPaymentMethodDialog1" runat="server" OnItemUpdated="EditPaymentMethodDialog1_ItemUpdated" OnCancelled="EditPaymentMethodDialog1_Cancelled" />
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="PaymentMethodDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForCriteria" TypeName="CommerceBuilder.Payments.PaymentMethodDataSource" 
        SelectCountMethod="CountForCriteria" SortParameterName="sortExpression" 
        DataObjectTypeName="CommerceBuilder.Payments.PaymentMethod" DeleteMethod="Delete">
        <SelectParameters>
			<asp:Parameter Name="sqlCriteria" DefaultValue="PaymentInstrumentId<>12 AND PaymentInstrumentId<>11" />
		</SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
