<%@ Page Language="C#" MasterPageFile="../../Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Shipping_Providers_Default" Title="Shipping Gateways"  %>
<%@ Import Namespace="CommerceBuilder.Shipping.Providers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Integrated Carriers"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="2" class="innerLayout">
        <tr>
            <td valign="top" align="center" class="itemlist">
                <asp:GridView ID="GatewayGrid" runat="server" DataKeyNames="ShipGatewayId" DataSourceID="ShipGatewayDs" 
                    AutoGenerateColumns="false" Width="100%" SkinID="PagedList">
                    <Columns>                        
						<asp:TemplateField HeaderText="Enabled">
						   <ItemStyle HorizontalAlign="Center" Width="10%" />
						   <ItemTemplate>
						      <asp:CheckBox ID="Enabled" runat="server" Text='<%#Eval("ShipGatewayId")%>' CssClass="hiddenText" Checked='<%#(bool)Eval("Enabled")%>' OnCheckedChanged="Enabled_CheckedChanged" AutoPostBack="true" />
						   </ItemTemplate>
						</asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Shipping Carrier" HeaderStyle-HorizontalAlign="left" ReadOnly="true"  />
                        <asp:TemplateField HeaderText="Shipping Methods">
                            <ItemStyle HorizontalAlign="Left" Width="65%" />
                            <ItemTemplate>
                                <asp:BulletedList ID="ShipMethodList" runat="server" OnDataBinding="ShipMethodList_DataBinding" DataSource='<%#GetShipMethodList(Container.DataItem)%>'> </asp:BulletedList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                            <ItemTemplate>
                                <asp:HyperLink ID="EditButton" runat="server" NavigateUrl='<%#GetConfigUrl(Container.DataItem) %>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" /></asp:HyperLink>
                                <asp:ImageButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" SkinID="DeleteIcon" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' AlternateText="Delete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="EmptyMessage" runat="server" Text="No integrated shipping carriers have been configured for your store.  To get started, click the 'Add Carrier' button."></asp:Label>
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                <asp:HyperLink ID="AddGateway" runat="server" Text="Add Carrier" NavigateUrl="AddGateway.aspx" SkinID="Button"></asp:HyperLink>
                <asp:HyperLink ID="MethodsLink" runat="server" Text="Shipping Methods >" NavigateUrl="../Methods/Default.aspx" SkinID="Button"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="ShipGatewayDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Shipping.ShipGatewayDataSource" SelectCountMethod="CountForStore" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Shipping.ShipGateway" DeleteMethod="Delete">
    </asp:ObjectDataSource>
</asp:Content>

