<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="ViewProducts.aspx.cs" Inherits="Admin_Shipping_Warehouses_ViewProducts" Title="Warehouse Products"  Debug="true"%>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<script type="text/javascript">
   function toggleCheckBoxState(id, checkState)
   {
      var cb = document.getElementById(id);
      if (cb != null)
         cb.checked = checkState;
   }

   function toggleSelected(checkState)
   {
      // Toggles through all of the checkboxes defined in the CheckBoxIDs array
      // and updates their value to the checkState input parameter
      if (CheckBoxIDs != null)
      {
         for (var i = 0; i < CheckBoxIDs.length; i++)
            toggleCheckBoxState(CheckBoxIDs[i], checkState.checked);
      }
   }    
</script>
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Products in {0}" EnableViewState="False"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2">
                <cb:SortedGridView ID="ProductGrid" runat="server" AllowPaging="True" AllowSorting="true" AutoGenerateColumns="False"
                    DataKeyNames="ProductId" DataSourceID="ProductDs" Width="100%" SkinID="PagedList" PageSize="20" 
                    DefaultSortExpression="Name" OnDataBound="ProductGrid_DataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Name" SortExpression="Name">
                            <HeaderStyle HorizontalAlign="left" />
                            <ItemTemplate>
                                <asp:HyperLink ID="NameLink" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%#Eval("ProductId", "../../Products/EditProduct.aspx?ProductId={0}")%>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <input type="checkbox" onclick="toggleSelected(this)" />
                            </HeaderTemplate>
                            <ItemStyle HorizontalAlign="Center" width="30px" />
                            <ItemTemplate>
                                <asp:CheckBox ID="Selected" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerSettings FirstPageText="|<" LastPageText=">|" NextPageText=">" PreviousPageText="<" Mode="NumericFirstLast" />
                </cb:SortedGridView>
            </td>
        </tr>
        <tr><td colspan="2"><hr /></td></tr>
        <tr>
            <td>
                <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />
            </td>
            <td align="right">
                <asp:Panel ID="NewWarehousePanel" runat="server">
                    <asp:Label ID="NewWarehouseLabel" runat="server" Text="Move selected to:" SkinID="FieldHeader"></asp:Label>
                    <asp:DropDownList ID="NewWarehouse" runat="server" AppendDataBoundItems="true" DataTextField="Name" DataValueField="WarehouseId">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button ID="NewWarehouseUpdateButton" runat="server" Text="Go" OnClick="NewWarehouseUpdateButton_Click" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="ProductDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
        SelectCountMethod="CountForWarehouse" SelectMethod="LoadForWarehouse" SortParameterName="sortExpression"
        TypeName="CommerceBuilder.Products.ProductDataSource">
        <SelectParameters>
            <asp:QueryStringParameter Name="warehouseId" QueryStringField="WarehouseId" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

