<%@ Page Language="C#" MasterPageFile="../../Admin.master" CodeFile="AddGateway.aspx.cs" Inherits="Admin_Shipping_Providers_AddGateway" Title="Add Gateway"  %>
<%@ Import Namespace="CommerceBuilder.Shipping.Providers" %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Add Integrated Carrier"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top">
                <asp:GridView ID="ProviderGrid" runat="server" AutoGenerateColumns="False" 
                    GridLines="None" CellPadding="4">
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="top">
                            <ItemTemplate>
                                <asp:HyperLink ID="ProviderLink" runat="server" NavigateUrl="<%#GetConfigUrl(Container.DataItem)%>">
                                    <asp:Image ID="ProviderLogo" runat="server" ImageUrl='<%#GetLogoUrl(Container.DataItem)%>' />
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:HyperLink ID="ProviderNameLink" runat="server" NavigateUrl="<%#GetConfigUrl(Container.DataItem)%>">
                                    <%#Eval("Name")%>
                                </asp:HyperLink><br />                                    
                                <asp:Label ID="ProviderDescription" runat="server" Text='<%#Eval("Description")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
	        </td>
	    </tr>
    </table>
</asp:Content>

