<%@ Page Language="C#" MasterPageFile="Product.master" CodeFile="ProductDiscounts.aspx.cs" Inherits="Admin_Products_ProductDiscounts" Title="Manage Product Discounts"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Discounts for {0}"></asp:Localize></h1>
        </div>
    </div>
    <asp:Label ID="InstructionText" runat="server" Text="Check the discounts that should be attached to this product. If you need to manage the available discounts, <a href='../Marketing/Discounts/Default.aspx'>click here</a>."></asp:Label><br /><br />
    <asp:GridView ID="DiscountGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="VolumeDiscountId" DataSourceID="DiscountDs" SkinID="PagedList" AllowPaging="False"
        AllowSorting="false" EnableViewState="false" RowStyle-CssClass="odd" AlternatingRowStyle-CssClass="even">    
        <Columns>
            <asp:TemplateField HeaderText="Attached">
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:CheckBox ID="Attached" runat="server" Checked='<%# IsAttached((int)Eval("VolumeDiscountId")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="IsGlobal" HeaderText="Global" ReadOnly="True" SortExpression="IsGlobal" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" HeaderStyle-HorizontalAlign="Left" />
            <asp:TemplateField HeaderText="Discount" SortExpression="IsValueBased" HeaderStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:Label ID="QuantityLabel" runat="server" Visible='<%# !((bool)Eval("IsValueBased")) %>' Text="Quantity" SkinID="fieldheader"></asp:Label>
                    <asp:Label ID="ValueLabel" runat="server" Visible='<%# (bool)Eval("IsValueBased") %>' Text="Value" SkinID="fieldheader"></asp:Label><br />
                    <asp:Label ID="Levels" runat="server" Text='<%# GetLevels((VolumeDiscount)Container.DataItem) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Groups">
                <ItemTemplate>
                    <asp:Label ID="Groups" runat="server" Text='<%# GetNames((VolumeDiscount)Container.DataItem) %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="EmptyMessage" runat="server" Text="There are no discounts defined for your store."></asp:Label>
        </EmptyDataTemplate>
    </asp:GridView><br />    
    <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
	<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
    <asp:ObjectDataSource ID="DiscountDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Marketing.VolumeDiscountDataSource"
        SortParameterName="sortExpression">
    </asp:ObjectDataSource>
</asp:Content>

