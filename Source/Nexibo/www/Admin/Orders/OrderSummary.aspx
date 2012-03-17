<%@ Page Language="C#" CodeFile="OrderSummary.aspx.cs" Inherits="Admin_Orders_OrderSummary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head2" runat="server">
    <title>Order Summary</title>
</head>
<body>
<form runat="server">
<div class="section" style="width:300px">
    <div class="header">
        <h2><asp:Localize ID="Caption" runat="server" Text="Order Contents"></asp:Localize></h2>
    </div>
    <div class="contentSection">
            <asp:GridView ID="OrderItemsGrid" runat="server" AutoGenerateColumns="False" Width="100%" GridLines="both" 
            DataKeyNames="OrderItemId" AllowPaging="False" BorderColor="white" RowStyle-CssClass="odd" AlternatingRowStyle-CssClass="even">
            <Columns>
                <asp:TemplateField HeaderText="Order Item Name" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:Label ID="Name" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="center">
                    <ItemTemplate>
                        <asp:Label ID="Basis" runat="server" Text='<%#Eval("Quantity")%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div style="text-align:center;margin-top:10px;margin-bottom:10px;padding-left:10px;padding-right:10px">
                <asp:Label ID="NoBasketItemsText" runat="server" Text="<i>There are no order items for this order.</i>"></asp:Label>
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
</form>
</body>
</html>