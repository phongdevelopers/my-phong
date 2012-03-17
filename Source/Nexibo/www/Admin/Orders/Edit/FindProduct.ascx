<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FindProduct.ascx.cs" Inherits="Admin_Orders_Edit_FindProductControl" %>
<%@ Register Src="~/Admin/UserControls/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<ajax:UpdatePanel ID="SearchAjax" runat="server">
        <ContentTemplate>
            <table class="inputForm">
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="FindProductSearchLabel" runat="server" text="Find Product:"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="FindProductSearchText" runat="server"></asp:TextBox>
                        <asp:Label ID="FindProductSearchInLabel" runat="server" text="in"></asp:Label>
                        <asp:DropDownList ID="FindProductSearchField" runat="server">                                            
                            <asp:ListItem Text="Name"></asp:ListItem>
                            <asp:ListItem Text="SKU"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="FindProductSearchButton" runat="server" Text="Search" OnClick="FindProductSearchButton_Click" />
                    </td>
                </tr>
            </table>
            <asp:GridView ID="FindProductSearchResults" runat="server" AutoGenerateColumns="false" 
                DataSourceID="AddProductDs" AllowPaging="true" PageSize="20" AllowSorting="true" 
                Visible="false" SkinID="PagedList" Width="460px">
                <Columns>
                    <asp:BoundField DataField="Sku" HeaderText="SKU" SortExpression="Sku" ItemStyle-Width="150px" />
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-Width="200px" />
                    <asp:TemplateField HeaderText="Price" SortExpression="Price" ItemStyle-Width="80px">
                        <ItemTemplate>
                            <uc:ProductPrice ID="ProductPrice1" runat="server" Product='<%#Container.DataItem%>' />                            
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="30px" HorizontalAlign="center" Wrap="false" />
                        <ItemTemplate>
                            <asp:HyperLink ID="AddLink" runat="server" NavigateUrl='<%#GetAddLinkURL((int)Eval("ProductId"))%>'><asp:Image ID="AddIcon" runat="server" SkinID="AddIcon" /></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label ID="FindProductEmptyResultsMessage" runat="server" Text="No products match the search criteria."></asp:Label>
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:HiddenField ID="HiddenName" runat="server" />
            <asp:HiddenField ID="HiddenSku" runat="server" />
            <asp:ObjectDataSource ID="AddProductDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="FindProducts" TypeName="CommerceBuilder.Products.ProductDataSource" SortParameterName="sortExpression">
                <SelectParameters>
                    <asp:ControlParameter ControlID="HiddenName" Name="name" PropertyName="Value" Type="String" />
                    <asp:ControlParameter ControlID="HiddenSku" Name="sku" PropertyName="Value" Type="String" />
                    <asp:Parameter DefaultValue="0" Name="categoryId" Type="Object" />
                    <asp:Parameter DefaultValue="0" Name="manufacturerId" Type="Object" />
                    <asp:Parameter DefaultValue="0" Name="vendorId" Type="Object" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </ContentTemplate>
    </ajax:UpdatePanel>

