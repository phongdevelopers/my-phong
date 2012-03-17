<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="PullSheet.aspx.cs" Inherits="Admin_Orders_Print_PullSheet" Title="Order Pull Sheet"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Order Pull Sheet"></asp:Localize></h1>
        </div>
        <div class="content">
            <h2><asp:Localize ID="OrderListLabel" runat="server" Text="Includes Order Numbers:"></asp:Localize></h2>
            <asp:Label ID="OrderList" runat="server" Text=""></asp:Label>
        </div>
    </div>
    <div class="noPrint">
        <p><asp:Localize ID="PrintInstructions" runat="server" Text="This document includes a printable stylesheet. The latest versions of IE and Firefox browsers will print with appropriate styles and page breaks if needed. Website headers, footers, and this message will not be printed."></asp:Localize></p>
        <asp:Button ID="Print" runat="server" Text="Print" OnClientClick="window.print();return false;" />
        <asp:Button ID="Back" runat="server" Text="Back" OnClientClick="window.history.go(-1);return false;" />
    </div>
    <div align="center">
        <asp:GridView ID="ItemGrid" runat="server" AutoGenerateColumns="false" CssClass="dataSheet"
            CellPadding="0" CellSpacing="0" GridLines="None" Width="600">
            <Columns>
                <asp:BoundField DataField="Quantity" HeaderText="Qty" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="Sku" HeaderText="Sku" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <asp:TemplateField HeaderText="Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <%#Eval("Name")%><asp:Label ID="VariantName" runat="server" Text='<%#Eval("VariantName", " ({0})")%>' Visible='<%#(Eval("VariantName").ToString().Length > 0)%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Pulled" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        [&nbsp;&nbsp;&nbsp;]
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>