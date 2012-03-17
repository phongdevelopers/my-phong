<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Shipping_Warehouses_Default" Title="Manage Warehouses"  %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Warehouses"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td class="itemlist">
                <ajax:UpdatePanel ID="WarehouseAjax" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:GridView ID="WarehouseGrid" runat="server" AllowPaging="False" AllowSorting="False"
                            AutoGenerateColumns="False" DataKeyNames="WarehouseId" ShowFooter="False" OnRowDeleting="WarehouseGrid_RowDeleting" 
                            Width="100%" SkinID="PagedList">
                            <Columns>
                                <asp:TemplateField HeaderText="Primary">
                                    <HeaderStyle HorizontalAlign="center" />
                                    <ItemStyle HorizontalAlign="center" VerticalAlign="top" />
                                    <ItemTemplate>
										<asp:Image ID="IsPrimary" runat="server" SkinID="CodeGreen" ToolTip="Default Warehouse" Visible='<%# IsDefaultWarehouse(Container.DataItem) %>' ></asp:Image>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name">
                                    <HeaderStyle HorizontalAlign="left" />
                                    <ItemStyle HorizontalAlign="left" VerticalAlign="top" />
                                    <ItemTemplate>
                                        <asp:Label ID="NameLabel" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Address">
                                    <HeaderStyle HorizontalAlign="left" />
                                    <ItemStyle HorizontalAlign="left" VerticalAlign="top" />
                                    <ItemTemplate>
                                        <asp:Label ID="AddressLabel" runat="server" Text='<%# GetAddress(Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="#&nbsp;of&nbsp;Products">
                                    <HeaderStyle HorizontalAlign="center" />
                                    <ItemStyle HorizontalAlign="center" VerticalAlign="top" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ProductsLink" runat="server" Text='<%# CountProducts(Container.DataItem) %>' NavigateUrl='<%#Eval("WarehouseId", "ViewProducts.aspx?WarehouseId={0}")%>' SkinID="GridLink"></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False" >
                                    <ItemStyle HorizontalAlign="right" VerticalAlign="top" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("WarehouseId", "EditWarehouse.aspx?WarehouseId={0}")%>'><asp:Image ID="EditIcon" SkinID="Editicon" runat="server" /></asp:HyperLink>
                                        <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\");") %>' Visible='<%# ShowDeleteButton(Container.DataItem) %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:LinkButton>
                                        <asp:HyperLink ID="DeleteLink" runat="server" NavigateUrl='<%# Eval("WarehouseId", "DeleteWarehouse.aspx?WarehouseId={0}")%>' Visible='<%# ShowDeleteLink(Container.DataItem) %>'><asp:Image ID="DeleteIcon2" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyDataText" runat="server" Text="No warehouses are defined for your store."></asp:Label>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </ContentTemplate>
                </ajax:UpdatePanel>
                <asp:HyperLink ID="AddWarehouse" runat="server" Text="Add Warehouse" NavigateUrl="AddWarehouse.aspx" CssClass="button"></asp:HyperLink>
            </td>
            <td valign="top" width="40%">
                 <div class="section" style="padding:0 0 2px 0;">
                    <div class="header">
                        <h2 class="catalogmode"><asp:Localize ID="DefaultWarehouseCaption" runat="server" Text="Primary Warehouse (Store Address)"></asp:Localize></h2>
                    </div>
                    <div class="content">
                        <ajax:UpdatePanel ID="DefaultWarehouseAjax" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:Localize ID="DefaultWarehouseHelpText" runat="server" Text="Select the warehouse that represents the primary physical location of your store or business headquarters.  This is called the &quot;primary warehouse&quot; even if products do not ship from this location."></asp:Localize>
                                <table>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="DefaultWarehouseLabel" runat="server" Text="Store Address:" AssociatedControlID="DefaultWarehouse"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="DefaultWarehouse" runat="server" DataTextField="Name" DataValueField="WarehouseId" OnSelectedIndexChanged="DefaultWarehouse_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </div>
                </div>                
            </td>
        </tr>
    </table>
</asp:Content>

