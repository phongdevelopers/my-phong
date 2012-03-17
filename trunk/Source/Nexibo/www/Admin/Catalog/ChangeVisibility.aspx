<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/Catalog.master" CodeFile="ChangeVisibility.aspx.cs" Inherits="Admin_Catalog_ChangeVisibility" Title="Move Catalog Objects" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Change Visibility"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
	    <tr>
	        <td valign="top" style="width:35%">
	            <div class="section">
	                <div class="header">
	                    <h2 class="selecteditems">Selected items</h2>
	                </div>
	                <div class="content">
	                    <asp:GridView ID="CGrid" runat="server" AutoGenerateColumns="False" Width="100%" AllowSorting="False" AllowPaging="false" 
                        SkinId="PagedList" ShowHeader="true" EnableViewState="false">
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
                            <asp:TemplateField HeaderText="Visibility" >
                            <HeaderStyle HorizontalAlign="center" Width="50px" />
                            <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <img src="<%# GetVisibilityIconUrl(Container.DataItem) %>" border="0" alt="<%#Eval("Visibility")%>" />
                                </ItemTemplate>
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
	            <table class="inputForm" cellpadding="3">	                
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
                        <th class="rowHeader" valign="top">
                            <asp:Label ID="VisibilityLabel" runat="server" Text="Visibility Setting:" ToolTip="Visibility setting for this category."></asp:Label>
                        </th>
                        <td>
                            <asp:RadioButton ID="VisPublic" runat="server" GroupName="Visibility" />
                            <asp:Image ID="VisPublicIcon" runat="server" SkinID="CmsPublicIcon" />&nbsp;
                            <asp:Localize ID="VisPublicLabel" runat="server" Text="Public"></asp:Localize><br />
                            <asp:RadioButton ID="VisHidden" runat="server" GroupName="Visibility" />
                            <asp:Image ID="VisHiddenIcon" runat="server" SkinID="CmsHiddenIcon" />&nbsp;
                            <asp:Localize ID="VisHiddenLabel" runat="server" Text="Hidden"></asp:Localize>
                            <br />
                            <asp:RadioButton ID="VisPrivate" runat="server" GroupName="Visibility" Checked="true"/>
                            <asp:Image ID="VisPrivateIcon" runat="server" SkinID="CmsPrivateIcon" />&nbsp;
                            <asp:Localize ID="VisPrivateLabel" runat="server" Text="Private"></asp:Localize>
                        </td>
                    </tr>
                    <tr id="trIncludeContents" runat="server">
                        <th class="rowHeader" valign="top">
                            <asp:Label ID="ScopeLabel" runat="server" Text="Category Options:" AssociatedControlID="Scope" ToolTip="Indicate whether contents of category should be modified."></asp:Label>
                        </th>
                        <td>
                            <asp:RadioButtonList ID="Scope" runat="server" RepeatLayout="Flow">
                                <asp:ListItem Value="1" Text="Change visibility for selected categories and contents." Selected="true"></asp:ListItem>
                                <asp:ListItem Value="0" Text="Change visibility for selected categories only."></asp:ListItem>
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