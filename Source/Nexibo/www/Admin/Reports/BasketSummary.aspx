<%@ Page Language="C#" CodeFile="BasketSummary.aspx.cs" Inherits="Admin_Reports_BasketSummary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head2" runat="server">
    <title>Basket Summary</title>
</head>

<body>
<form runat="server">
<div class="section" style="width:400px">
    <div class="header">
        <h2><asp:Localize ID="Caption" runat="server" Text="Basket Contents"></asp:Localize></h2>
    </div>
    <div class="content">
            <asp:GridView ID="BasketItemsGrid" runat="server" AutoGenerateColumns="False" Width="100%" 
            DataKeyNames="BasketItemId" BorderColor="White" RowStyle-CssClass="odd" AlternatingRowStyle-CssClass="even" SkinID="PagedList" >
            <Columns>
                <asp:TemplateField HeaderText="Item Name" >
                    <ItemTemplate>
                        <asp:Label ID="Name" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Left" Width="80%" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Quantity" >
                    <ItemTemplate>
                        <asp:Label ID="Basis" runat="server" Text='<%#Eval("Quantity")%>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="20%" />
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div style="text-align:center;margin-top:10px;margin-bottom:10px;padding-left:10px;padding-right:10px">
                <asp:Label ID="NoBasketItemsText" runat="server" Text="<i>There are no items in this basket.</i>"></asp:Label>
                </div>
            </EmptyDataTemplate>
                <RowStyle CssClass="odd" />
                <AlternatingRowStyle CssClass="even" />
        </asp:GridView>
    </div>
</div>
</form>
</body>
</html>