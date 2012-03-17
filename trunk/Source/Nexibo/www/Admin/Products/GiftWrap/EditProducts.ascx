<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditProducts.ascx.cs" Inherits="Admin_Products_GiftWrap_EditProducts" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls"
    TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls"
    TagPrefix="cb" %>
<script language="javascript">
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
<table cellpadding="4" cellspacing="0" class="inputForm" width="100%" >
    <tr>
        <td valign="top">
            <ajax:UpdatePanel ID="MainContentAjax" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table border="0" cellpadding="5" cellspacing="0" style="width:100%;">
                        <tr>
                            <td valign="top" style="width:50%;">
                                <div class="section">
                                    <div class="header">
                                        <h2>
                                            Assigned Products</h2>
                                    </div>
                                    <div class="content">
                                        <asp:GridView ID="RelatedProductGrid" runat="server" AutoGenerateColumns="False"
                                            DataSourceID="RelatedProductsDs" DataKeyNames="ProductId" ShowHeader="False"
                                            Width="100%" GridLines="None" SkinID="PagedList" OnRowDeleting="RelatedProductGrid_RowDeleting" OnDataBound="RelatedProductGrid_DataBound">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <HeaderTemplate>
                                                        <input type="checkbox" onclick="toggleSelected(this)" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="Selected" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Name">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="ProductName2" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%#Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="RemoveButton2" runat="server" SkinID="DeleteIcon" AlternateText="Remove"
                                                            ToolTip="Remove" CommandArgument='<%#Eval("ProductId")%>' OnClick="RemoveButton2_Click" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <asp:HyperLink ID="EmptyMessage" runat="server" Text="There are no products for this WrapGroup."></asp:HyperLink>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                        <br />
                                        <asp:Panel ID="NewWrapGroupPanel" runat="server">
                                            <asp:Label ID="NewWrapGroupLabel" runat="server" Text="Move selected to:" SkinID="FieldHeader"></asp:Label>
                                            <asp:DropDownList ID="NewWrapGroup" runat="server" AppendDataBoundItems="true" DataTextField="Name" DataValueField="WrapGroupId">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Button ID="NewWrapGroupUpdateButton" runat="server" Text="Go" OnClick="NewWrapGroupUpdateButton_Click" />
                                        </asp:Panel>
                                        <br />
                                        <asp:HyperLink ID="BackButton" runat="server" Text="Back" SkinID="Button" NavigateUrl="Default.aspx"  />    
                                        <asp:ObjectDataSource ID="RelatedProductsDs" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="LoadForWrapGroup" TypeName="CommerceBuilder.Products.ProductDataSource">
                                            <SelectParameters>
                                                <asp:QueryStringParameter Name="wrapGroupId" QueryStringField="WrapGroupId"
                                                    Type="Object" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </div>
                                </div>
                            </td>
                            <td valign="top">
                                <div class="section">
                                    <div class="header">
                                        <h2>
                                            <asp:Localize ID="FindProductsCaption" runat="server" Text="Find and Assign Products"></asp:Localize></h2>
                                    </div>
                                    <asp:Panel ID="SearchFormPanel" runat="server" CssClass="content" DefaultButton="SearchButton">
                                        <table class="inputForm">
                                            <tr>
                                                <th class="rowHeader" style="text-align: left;">
                                                    <cb:ToolTipLabel ID="SearchNameLabel" runat="server" Text="Product Name:" ToolTip="Enter all or part of a product name.  Wildcard characters * and ? are accepted." />
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="SearchName" runat="server" Text=""></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader" style="text-align: left;">
                                                    <cb:ToolTipLabel ID="ShowImagesLabel" runat="server" Text="Show Thumbnails:" ToolTip="When checked, product images will be displayed in the search results." />
                                                </th>
                                                <td>
                                                    <asp:CheckBox ID="ShowImages" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader" style="text-align: left;">
                                                    <cb:ToolTipLabel ID="NoWrapGroupLabel" runat="server" Text="Products without WrapGroups only:"
                                                        ToolTip="Show products with that have no WrapGroup assigned." />
                                                </th>
                                                <td>
                                                    <asp:CheckBox ID="NoWrapGroup" runat="server" Checked="true" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="right">
                                                    <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" /><br />
                                                </td>
                                            </tr>
                                        </table>
                                        <cb:SortedGridView ID="SearchResultsGrid" runat="server" AutoGenerateColumns="False"
                                            DataKeyNames="ProductId" GridLines="None" SkinID="PagedList" DataSourceID="ProductSearchDs"
                                            Width="100%" Visible="false" AllowPaging="true" PageSize="20" AllowSorting="true"
                                            DefaultSortExpression="Name">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Thumbnail">
                                                    <itemstyle horizontalalign="Center" />
                                                    <itemtemplate>
                                                <asp:HyperLink ID="NodeImageLink" runat="server" NavigateUrl='<%#Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}")%>'>
                                                    <asp:Image ID="NodeImage" runat="server" ImageUrl='<%# Eval("ThumbnailUrl") %>' Visible='<%# !string.IsNullOrEmpty((string)Eval("ThumbnailUrl")) %>' AlternateText='<%# Eval("Name") %>' />
                                                </asp:HyperLink>
                                            </itemtemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Product" SortExpression="Name">
                                                    <headerstyle horizontalalign="Left" />
                                                    <itemtemplate>
                                                <asp:HyperLink ID="ProductName" runat="server" Text='<%#Eval("Name")%>' SkinID="FieldHeader" NavigateUrl='<%#Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}")%>' /><br />
                                            </itemtemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="WrapGroup">
                                                    <headerstyle horizontalalign="Center" />
                                                    <itemstyle horizontalalign="Center" />
                                                    <itemtemplate>
                                                <asp:Label ID="WrapGroupName" runat="server" Text='<%#((Product)Container.DataItem).WrapGroup != null? ((Product)Container.DataItem).WrapGroup.Name:""%>' SkinID="FieldHeader"  /><br />
                                            </itemtemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Related">
                                                    <itemstyle width="50px" horizontalalign="Center" />
                                                    <itemtemplate>
                                                <asp:ImageButton ID="AttachButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Add" ToolTip="Assign to this WrapGroup" SkinId="AddIcon" OnClientClick="this.visible=false" OnClick="AttachButton_Click" Visible='<%#!IsProductLinked((Product)Container.DataItem)%>' />
                                                <asp:ImageButton ID="RemoveButton" runat="server" CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Remove" ToolTip="Remove from this WrapGroup" SkinId="DeleteIcon" OnClientClick="this.visible=false" OnClick="RemoveButton_Click" Visible='<%#IsProductLinked((Product)Container.DataItem)%>' />
                                            </itemtemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <asp:HyperLink ID="EmptyMessage" runat="server" Text="There are no products that match the search text."></asp:HyperLink>
                                            </EmptyDataTemplate>
                                        </cb:SortedGridView>
                                        <asp:ObjectDataSource ID="ProductSearchDs" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="LoadForCriteria" SortParameterName="sortExpression" TypeName="CommerceBuilder.Products.ProductDataSource"
                                            OnSelecting="ProductSearchDs_Selecting" SelectCountMethod="CountForCriteria">
                                            <SelectParameters>                                                
                                                <asp:Parameter Name="sqlCriteria" Type="String" DefaultValue="WrapGroupId IS NULL"/>                                                
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </asp:Panel>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
    </tr>
</table>
