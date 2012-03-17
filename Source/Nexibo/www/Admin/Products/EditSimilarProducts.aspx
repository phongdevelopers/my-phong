<%@ Page Language="C#" MasterPageFile="Product.master" CodeFile="EditSimilarProducts.aspx.cs" Inherits="Admin_Products_EditSimilarProducts" Title="Similar Products"  %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">

<script language="javascript" type="text/javascript">

function changeState(target)
{
    var eleIcon = target.firstChild;
    var eleState = target.lastChild;
    value = parseInt(eleState.value);
    value = (value==3)?0:value+1;
    eleIcon.src = iconImages[value];
    eleState.value = value;
    return false;
}


</script>


<div class="pageHeader">
    <div class="caption">
        <h1><asp:Localize ID="Caption" runat="server" Text="Similar Products for '{0}'"></asp:Localize></h1>
    </div>
</div>
    <table cellpadding="4" cellspacing="0" class="inputForm" width="100%">
        <tr>
            <td width="100%">
                <p align="justify">
                    <asp:Localize ID="InstructionText" runat="server" Text="Select the products that are related, that should be highlighted in cross-selling scenarios." />
                </p>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <ajax:UpdatePanel ID="MainContentAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="section">
                            <div class="header">
                                <h2><asp:Localize ID="FindProductsCaption" runat="server" Text="Find Products"></asp:Localize></h2>
                            </div>
                            <asp:Panel ID="SearchFormPanel" runat="server" CssClass="content" DefaultButton="SearchButton">
                                <table class="inputForm">
                                    <tr>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="SearchNameLabel" runat="server" Text="Product Name:" ToolTip="Enter all or part of a product name.  Wildcard characters * and ? are accepted." />
                                        </th>
                                        <td>
                                            <asp:TextBox ID="SearchName" runat="server" Text=""></asp:TextBox>
                                        </td>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="CategoryLabel" runat="server" Text="Category:" ToolTip="You can select a category to list only products meeting the critera from that category." />
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="CategoryList" runat="server" AppendDataBoundItems="True" 
                                                DataTextField="Name" DataValueField="CategoryId">
                                                <asp:ListItem Text="- Any Category -" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                        <cb:ToolTipLabel ID="CrossSellFilterLabel" runat="server" Text="Filter:" ToolTip="Helps you filter cross sell products" />
                                        </th>
                                        <td>
                                         <asp:DropDownList ID="Filter" runat="server">
                                                <asp:ListItem Text="" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Linked" Selected="True" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Cross Linked" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Links To" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Linked From" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Unlinked" Value="4"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <th class="rowHeader">
                                            <cb:ToolTipLabel ID="ShowImagesLabel" runat="server" Text="Show Thumbnails:" ToolTip="When checked, product images will be displayed in the search results." />
                                        </th>
                                        <td>
                                            <asp:CheckBox ID="ShowImages" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                    <td></td>
                                        <td>
                                            <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />                                        
                                        </td>
                                        <td colspan="2"></td>
                                    </tr>
                                </table>
                                <cb:SortedGridView ID="SearchResultsGrid" runat="server" 
                                    AutoGenerateColumns="False" DataKeyNames="ProductId" GridLines="None" 
                                    SkinId="PagedList" DataSourceId="ProductSearchDs"
                                    Width="100%" Visible="False" AllowPaging="True" PageSize="20" 
                                    AllowSorting="True" DefaultSortExpression="Name" 
                                    DefaultSortDirection="Ascending" ShowWhenEmpty="False" 
                                    onrowcreated="SearchResultsGrid_RowCreated">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemStyle Width="32"/>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="CrossSellLink" runat="server" OnClientClick="return changeState(this);" style="background:none !important;text-decoration:none!important;">
                                                    <asp:ImageButton ID="CrossSellIcon" runat="server" />
                                                    <asp:HiddenField ID="CrossSellState" runat="server" />
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item" SortExpression="Name">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="ProductName" runat="server" Text='<%#Eval("Name")%>' SkinID="FieldHeader" /><br />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Thumbnail">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="NodeImageLink" runat="server" NavigateUrl='<%# UrlGenerator.GetBrowseUrl((int)Eval("ProductId"), CatalogNodeType.Product, (string)Eval("Name")) %>'>
                                                    <asp:Image ID="NodeImage" runat="server" ImageUrl='<%# Eval("ThumbnailUrl") %>' Visible='<%# !string.IsNullOrEmpty((string)Eval("ThumbnailUrl")) %>' AlternateText='<%# Eval("Name") %>' />
                                                </asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:HyperLink ID="EmptyMessage" runat="server" Text="There are no products that match the search criteria."></asp:HyperLink>
                                    </EmptyDataTemplate>
                                </cb:SortedGridView>
                                <asp:ObjectDataSource ID="ProductSearchDs" runat="server"
                                    OldValuesParameterFormatString="original_{0}" SelectMethod="SearchRelatedProducts" SortParameterName="sortExpression"
                                    TypeName="CommerceBuilder.Products.ProductDataSource" OnSelecting="ProductSearchDs_Selecting" >
                                    <SelectParameters>
                                        <asp:ControlParameter Name="productName" ControlID="SearchName" DefaultValue="" DbType="String" />
                                        <asp:ControlParameter Name="categoryId" ControlID="CategoryList" DefaultValue="0" DbType="Int32" />
                                        <asp:QueryStringParameter Name="productId" QueryStringField="ProductId" Type="Int32" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </asp:Panel>
                        </div>
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" OnClientClick="this.value = 'Saving...'; this.enabled= false;" EnableViewState="false" />
                    <table>
                        <tr>
                        <td><asp:Label ID="LegendCaption" runat="server" Text="Legend:&nbsp;&nbsp;" SkinID="FieldHeader"></asp:Label></td>
                        <td><asp:Image ID="UnlinkedIconLegend" runat="server" ToolTip="Unlinked" /></td>
                        <td>Unlinked&nbsp;&nbsp;</td>
                        <td><asp:Image ID="LinksToIconLegend" runat="server" ToolTip="Links To"/></td>
                        <td>Links To&nbsp;&nbsp;</td>
                        <td><asp:Image ID="LinksFromIconLegend" runat="server" ToolTip="Links From"/></td>
                        <td>Links From&nbsp;&nbsp;</td>
                        <td><asp:Image ID="CrossLinkedIconLegend" runat="server" ToolTip="Cross Linked"/></td>
                        <td>Cross Linked</td>
                        </tr>
                    </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>

