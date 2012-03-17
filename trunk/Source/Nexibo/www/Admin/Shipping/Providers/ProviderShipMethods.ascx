<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProviderShipMethods.ascx.cs" Inherits="Admin_Shipping_Providers_ProviderShipMethods" %>
<table class="contentPanel" width="100%">
<tr class="sectionHeader" >
   <td colspan="2">
	   <asp:Label ID="ProviderShipMethodsConfig" runat="server" Text="Provider Services (Ship Methods) Configured" SkinID="FieldHeader">
	   </asp:Label>
  </td>
</tr>
</table>
<asp:GridView ID="ShipMethodGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="ShipMethodId" width="100%" OnRowDataBound="ShipMethodGrid_RowDataBound" OnRowDeleting="ShipMethodGrid_RowDeleting" SkinID="PagedList">
<Columns>
    <asp:TemplateField HeaderText="Delete">
        <ItemTemplate>
            <asp:CheckBox ID="DeleteCheckbox" runat="server" />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Name">
        <ItemTemplate>
            <asp:Label ID="NameLabel" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
        </ItemTemplate>        
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Type">
        <ItemTemplate>
            <asp:Label ID="ShipMethodTypeLabel" runat="server" Text='<%#Eval("ShipGateway.Name")%>'></asp:Label>
        </ItemTemplate>        
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Warehouses">
        <ItemTemplate>
            <asp:Label ID="WarehouseLabel" runat="server" Text='<%#GetWarehouseNames(Container.DataItem)%>'></asp:Label>
        </ItemTemplate>        
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Zones">
        <ItemTemplate>
            <asp:Label ID="ZoneLabel" runat="server" Text='<%#GetZoneNames(Container.DataItem)%>'></asp:Label>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField ShowHeader="False" >
        <ItemTemplate>
            <div align="center">
                <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#GetEditUrl(Container.DataItem)%>'><asp:Image ID="EditIcon" SkinID="Editicon" runat="server" /></asp:HyperLink>
                <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" /></asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:TemplateField>
</Columns>
<EmptyDataTemplate>
    No ship methods are configured for this gateway.
</EmptyDataTemplate>
<RowStyle CssClass="even" />
</asp:GridView>
<br/>
<asp:Button ID="MultipleRowDelete" runat="server"   Text="Delete Selected" OnClick="MultipleRowDelete_Click" OnClientClick="return confirm('Are you sure you want to delete the selected ship methods?')"  />
<br />
<table class="contentPanel" >	
	<tr class="sectionHeader">
	   <td colspan="2">
		   <asp:Label ID="ProviderShipMethodsNotConfig" runat="server" Text="Provider Services Not Configured" SkinID="FieldHeader">
		   </asp:Label>
	  </td>
	</tr>
  <tr>
    <th class="rowHeader" width="50%">Select Services (Shipping Methods):<br/>
		<span class="helpText">Please select the services which you want to configure as Shipping Methods.</span>            
    </th>
    <td width="50%">
        <asp:CheckBoxList ID="ServicesCheckList" runat="server">                    
        </asp:CheckBoxList>
        <asp:Label ID="ServicesCheckListPh" runat="server" Text="All Services Already Configured" ></asp:Label>
    </td>                    
 </tr>
 <tr>
    <td colspan="2" align="center">                                    
        <asp:Button ID="ServicesAddButton" runat="server" Text="Add Default Configuration" OnClick="ServicesAddButton_Click" />
    </td>
 </tr>
</table>
