<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/Catalog.master" CodeFile="MoveCatalogObjects.aspx.cs" Inherits="Admin_Catalog_MoveCatalogObjects" Title="Move Catalog Objects" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Move Catalog Objects"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
	    <tr >
	        <td valign="top" style="width:30%">
	            <div class="section">
	                <div class="header">
	                    <h2 class="selecteditems">Selected items</h2>
	                </div>
	                <div class="content">
	                    <asp:GridView ID="CGrid" runat="server" AutoGenerateColumns="False" Width="100%" AllowSorting="False" AllowPaging="false" 
                        SkinId="PagedList" ShowHeader="false" EnableViewState="false">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderStyle HorizontalAlign="center" Width="30px" />
                                <ItemStyle Width="30px" HorizontalAlign="Center" />
                                <ItemTemplate>                                            
                                    <img src="<%# GetCatalogIconUrl(Container.DataItem) %>" border="0" alt="" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name">
                                <ItemTemplate>
                                    <asp:HyperLink ID="N" runat="server" Text='<%# Eval("Name") %>' Target="_blank" NavigateUrl='<%# GetNavigateUrl(Container.DataItem) %>'></asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>                                    
                        </Columns>
                        <AlternatingRowStyle CssClass="even" />
                        <EmptyDataTemplate>
                            <div class="emptyResult">No items to display.</div>
                        </EmptyDataTemplate>
                        <PagerSettings Position="TopAndBottom" />
                    </asp:GridView>
	                </div>
	            </div>
                
                
	        </td>
	        <td valign="top">
	            <table class="inputForm" width="100%">	                
		            <tr>
		                <th valign="top" class="rowHeader">
		                    <asp:Label ID="CurrentPathLabel" runat="server" Text="Current Category:"></asp:Label><br />
		                </th>
		                <td class="middle">
		                    <asp:Label ID="TopLevelLabel" runat="server" Text="Top Level"></asp:Label>
                            <asp:Repeater ID="CurrentPath" runat="server">
                                <ItemTemplate> &gt; <%#Eval("Name")%></ItemTemplate>
                            </asp:Repeater>
		                </td>
		            </tr>
		            <tr>
		                <th valign="top" class="rowHeader">
		                    <asp:Label ID="NewPathLabel" runat="server" Text="New Category:" ></asp:Label><br />
		                </th>
		                <td>
		                    <asp:DropDownList ID="NewPath" runat="server" AppendDataBoundItems="true">
		                    </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
		                <th valign="top" class="rowHeader">
		                    <asp:Label ID="MoveOptionLabel" runat="server" Text="Move Option:" ></asp:Label><br />
		                </th>
		                <td>
		                    <asp:RadioButtonList ID="MoveOptions" runat="server" AutoPostBack="false">
		                        <asp:ListItem Text="Move item(s) to new category." Value="MoveSingle" Selected="true" />
		                        <asp:ListItem Text="Move item(s) to new category, remove from any other linked categories." Value="MoveAll" />
		                        <asp:ListItem Text="Link item(s) to new category, keeping existing category." Value="Add" />
		                    </asp:RadioButtonList>
		                </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>                            
                            <asp:Button ID="SaveButton" runat="server" Text="Save" CausesValidation="True" OnClick="SaveButton_Click" />
							<asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="CancelButton_Click" />
                        </td>
                    </tr>
	            </table>
	        </td>
	    </tr>
	</table>
</asp:Content>