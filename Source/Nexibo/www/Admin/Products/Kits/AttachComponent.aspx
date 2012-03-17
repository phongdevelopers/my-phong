<%@ Page Language="C#" MasterPageFile="../Product.master" CodeFile="AttachComponent.aspx.cs" Inherits="Admin_Products_Kits_AttachComponent" Title="Select KitComponent"  %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Attach/Copy Component"></asp:Localize></h1>
        </div>
    </div>
    <asp:Label ID="InstructionText" runat="server" Text="Search for the existing component to attach or copy for this kitted product.  If you wish to create a new component for this kit, "></asp:Label><asp:HyperLink ID="AddComponent" runat="server" NavigateUrl="AddComponent.aspx" Text="click here."></asp:HyperLink><br /><br />
    <asp:Label ID="SearchNameHelpText" runat="server" Text="Enter all or part of the name(s) to locate the desired component:"></asp:Label><br />
    <table class="inputForm">
        <tr>
            <th class="rowHeader">
                <asp:Label ID="ProductNameLabel" runat="server" Text="Product:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ProductName" runat="server" Text=""></asp:TextBox>
            </td>
            <th class="rowHeader">
                <asp:Label ID="KitComponentNameLabel" runat="server" Text="Component:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="KitComponentName" runat="server" Text=""></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
            </td>
        </tr>
    </table>
    <asp:GridView ID="SearchResultsGrid" runat="server" AutoGenerateColumns="false" DataSourceID="ComponentDs" ShowHeader="true" ShowFooter="false"
        DataKeyNames="KitComponentId" OnRowCommand="SearchResultsGrid_RowCommand" SkinID="PagedList" AllowSorting="true"
        AllowPaging="true" PageSize="20">
        <Columns>
            <asp:TemplateField HeaderText="Name" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:Label ID="Name" runat="server" Text='<%#Eval("Name")%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Type" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:Label ID="InputType" runat="server" Text='<%#FixInputTypeName(Eval("InputType").ToString())%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Contains" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:DataList ID="ProductList" runat="server" DataSource='<%#Eval("KitProducts")%>'>
                        <ItemTemplate>
                            <asp:Label ID="Quantity" runat="server" Text='<%#Eval("Quantity", "[{0}]")%>' /> 
                            <asp:Label ID="Name" runat="server" Text='<%#Eval("Name")%>' /> @ 
                            <asp:Label ID="CalculatedPrice" runat="server" Text='<%#Eval("CalculatedPrice", "{0:lc}")%>' />
                            <asp:Label ID="EachLabel" runat="server" Text="ea." />
                        </ItemTemplate>
                    </asp:DataList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:LinkButton ID="AttachComponent" runat="server" CommandName="Attach" CommandArgument='<%#Container.DataItemIndex%>' Text="Attach" Visible='<%#!((KitComponent)Container.DataItem).IsAttached(_ProductId)%>'></asp:LinkButton>
                    <asp:Label ID="AttachedLabel" runat="server" Text="Attached" Visible='<%#((KitComponent)Container.DataItem).IsAttached(_ProductId)%>' />
                    <asp:LinkButton ID="CopyComponent" runat="server" CommandName="Copy" CommandArgument='<%#Container.DataItemIndex%>' Text="Copy"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="EmptyMessage" runat="server" Text="There are no components that match the search parameters."></asp:Label>
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:ObjectDataSource ID="ComponentDs" runat="server" DataObjectTypeName="CommerceBuilder.Products.KitComponent"
        OldValuesParameterFormatString="original_{0}" SelectMethod="Search" SortParameterName="sortExpression"
        TypeName="CommerceBuilder.Products.KitComponentDataSource" SelectCountMethod="SearchCount" EnablePaging="true">
        <SelectParameters>
            <asp:ControlParameter Name="productName" Type="String" ControlID="ProductName" PropertyName="Text" />
            <asp:ControlParameter Name="componentName" Type="String" ControlID="KitComponentName" PropertyName="Text" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>