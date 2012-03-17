<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ucMethodGateways.ascx.cs" Inherits="Admin_Payment_ucMethodGateways" %>
<ajax:UpdatePanel ID="MethodGatewaysAjaxPanel" runat="server">
    <ContentTemplate>
        <asp:Label ID="SavedMessage" runat="server" Text="Gateway assignment saved." Visible="false"></asp:Label>
        <asp:GridView ID="GatewayGrid" runat="server" AutoGenerateColumns="False" ShowFooter="False" GridLines="Horizontal" OnRowDataBound="GatewayGrid_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Payment Method" ItemStyle-VerticalAlign="middle">
                    <ItemTemplate>
                        <asp:Label ID="Method" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Assigned Gateway" ItemStyle-VerticalAlign="middle">
                    <ItemTemplate>
                        <asp:DropDownList ID="Gateway" runat="server" AppendDataBoundItems="true" DataTextField="Name" DataValueField="PaymentGatewayId">
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView><br />
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
    </ContentTemplate>
</ajax:UpdatePanel>