<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="OrphanedItems.aspx.cs" Inherits="Admin_Catalog_OrphanedItems" Title="Orphaned Items" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
    <script language="javascript" type="text/javascript">
        function toggleSelected(checkState)
        {
            // Toggles through all of the checkboxes defined in the CheckBoxIDs array
            // and updates their value to the checkState input parameter            
            for(i = 0; i< document.forms[0].elements.length; i++){
                var e = document.forms[0].elements[i];
                var name = e.name;
                if ((e.type == 'checkbox') && (name.indexOf('DeleteCheckbox') != -1))
                {
                    e.checked = checkState.checked;
                }
            }            
        }
        
        function FilesChecked()
        {
            var count = 0;
            for(i = 0; i< document.forms[0].elements.length; i++){
                var e = document.forms[0].elements[i];
                var name = e.name;
                if ((e.type == 'checkbox') && (e.checked))
                {
                    count ++;
                }
            }
            return (count > 0);
        }
    </script>
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Orphaned Items"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr> 
            <td align="left">
                <asp:Label ID="CatalogItemTypeLabel" runat="server" SkinID="FieldHeader" Text="Select Item Type:"></asp:Label>
                <ul>
                    <asp:RadioButtonList ID="CatalogItemTypeList" runat="server" OnSelectedIndexChanged="CatalogItemTypeList_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="1">Categories</asp:ListItem>
                        <asp:ListItem Selected="True" Value="2">Products</asp:ListItem>
                        <asp:ListItem Value="3">Webpages</asp:ListItem>
                        <asp:ListItem Value="4">Links</asp:ListItem>
                        <asp:ListItem Value="5">Images</asp:ListItem>
                    </asp:RadioButtonList>            
                </ul>
            </td>
        </tr> 
	    <tr id="CatalogItemsPanel" runat="server">
	        <td colspan="2">                
                <asp:GridView ID="CGrid" runat="server" AutoGenerateColumns="False" Width="100%" 
                    AllowSorting="True" AllowPaging="True" PageSize="40" 
                    SkinId="PagedList" DataSourceID="ProductsDs" OnRowCommand="CGrid_RowCommand">
                    <Columns>                                                                    
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:HyperLink ID="N" runat="server" Text='<%# Eval("Name") %>'  NavigateUrl='<%#GetEditUrl(Container.DataItem)%>' ></asp:HyperLink>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>                                
                                <asp:HyperLink ID="EditLink" runat="server" ToolTip="Edit" NavigateUrl='<%#GetEditUrl(Container.DataItem)%>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" /></asp:HyperLink>
                                <asp:ImageButton ID="DeleteButton" runat="server" ToolTip="Delete" CommandName="DeleteItem" CommandArgument='<%#Eval(GetIdFieldName(Container.DataItem)) %>' OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' SkinID="DeleteIcon" ></asp:ImageButton>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="right" Width="135px" />
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="even" />
                    <EmptyDataTemplate>
                        <div class="emptyResult">No orphaned items found.</div>
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:ObjectDataSource ID="CategoryDs" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="LoadOrphaned" TypeName="CommerceBuilder.Catalog.CategoryDataSource"
                    EnablePaging="true" EnableViewState="false"  SortParameterName="sortExpression"> 
                </asp:ObjectDataSource>
                <asp:ObjectDataSource ID="ProductsDs" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="LoadOrphaned" TypeName="CommerceBuilder.Products.ProductDataSource"
                    EnablePaging="true" EnableViewState="false"  SortParameterName="sortExpression"> 
                </asp:ObjectDataSource>
                <asp:ObjectDataSource ID="WebpagesDs" runat="server" OldValuesParameterFormatString="original_{0}" 
                    SelectMethod="LoadOrphaned" TypeName="CommerceBuilder.Catalog.WebpageDataSource"
                    EnablePaging="true" EnableViewState="false"  SortParameterName="sortExpression"> 
                </asp:ObjectDataSource>                
                <asp:ObjectDataSource ID="LinksDs" runat="server" OldValuesParameterFormatString="original_{0}" 
                    SelectMethod="LoadOrphaned" TypeName="CommerceBuilder.Catalog.LinkDataSource"
                    EnablePaging="true" EnableViewState="false"  SortParameterName="sortExpression">   
                </asp:ObjectDataSource>  
           
            </td>
        </tr>
         <tr id="ImagesPanel" runat="server" visible="false">
	        <td colspan="2">
	            <asp:Panel ID="MessagePanel" runat="server" EnableViewState="false" Visible="False">
                    <div >
                        <asp:Label ID="FailureMessagesHeader" runat="server" SkinID="ErrorCondition" Text="Access denied, following file(s) can not be deleted:" ></asp:Label>
                        <asp:BulletedList ID="Messages" runat="server">
                        </asp:BulletedList>
                        <br />
                    </div>    
                </asp:Panel>
                List of files that are present in the "~Assets/ProductImages" folder at your store and are not associated to any product, category, webpage or link.
                <table cellpadding="2" cellspacing="0" class="innerLayout">
                    <tr>
                        <td valign="top">
                            <cb:SortedGridView ID="ImageFilesGrid" runat="server" AutoGenerateColumns="false" 
                                ShowHeader="true" ShowFooter="false" SkinID="Summary"
                                Width="100%"  DataKeyNames="Name"
                                AllowPaging="true" PageSize="20" OnPageIndexChanging="ImageFilesGrid_PageIndexChanging">
                                <Columns>   
                                    <asp:TemplateField>
                                        <ItemStyle HorizontalAlign="center" />
                                        <HeaderTemplate>
                                            <input type="checkbox" onclick="toggleSelected(this)" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="DeleteCheckbox" runat="server" runat="server"  />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="File">
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:Label ID="Name" runat="server" Text='<%# Eval("Name")%>'></asp:Label>
                                            <a href="#" onclick="return false;" title="View image." onmouseover='<%# Eval("Name", "ImageLookupHoverPanel.startCallback(event,\"ImageName={0}\",null,null);")%>' onmouseout="ImageLookupHoverPanel.hide();"><asp:ImageButton ID="PreviewImageIcon" runat="server" SkinId="PreviewIcon"></asp:ImageButton></a>                                            
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="FileItemType" HeaderText="File Type" SortExpression="FileItemType">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>                    
                                    <asp:TemplateField HeaderText="Size">
                                        <ItemStyle HorizontalAlign="center" />
                                        <ItemTemplate>
                                            <asp:Label ID="Size" runat="server" Text='<%# GetFileSize((long)Eval("Size")) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:BoundField DataField="Dimensions" HeaderText="Dimensions" SortExpression="Dimensions">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>                   
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Label ID="EmptyMessage" runat="server" Text="There are no image files."></asp:Label>
                                </EmptyDataTemplate>
                            </cb:SortedGridView>     
	                    </td>
	                </tr>
	                <tr>
	                    <td align="left">
	                        <asp:Button ID="DeleteButton" runat="server" Text="Delete Selected Files" OnClick="DeleteButton_Click" OnClientClick="if(FilesChecked()){return confirm('Are you sure you want to delete all selected files?');} else {alert ('Please select a file first.'); return false;}"/>
	                    </td>
	                </tr>
                </table>
	        </td>
	    </tr>
    </table>
    <wwh:wwHoverPanel ID="ImageLookupHoverPanel"
        runat="server" 
        serverurl="~/Admin/Catalog/OrphanedItems.aspx"
        Navigatedelay="100"
        scriptlocation="WebResource"
        style="display: none; background: white;"
        panelopacity="0.89"
        shadowoffset="8"
        shadowopacity="0.18"
        PostBackMode="None"
        Closable="false"
        AdjustWindowPosition="true">
    </wwh:wwHoverPanel>
</asp:Content>


