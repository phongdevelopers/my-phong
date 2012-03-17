<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ViewOrders.aspx.cs" Inherits="Admin_DigitalGoods_ViewOrders" Title="Digital Good Orders" EnableViewState="false" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Orders associated with '{0}'"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <br />
                <asp:GridView ID="OrderGrid" runat="server" AutoGenerateColumns="False"
                    SkinID="PagedList" AllowPaging="False" AllowSorting="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Order #">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <a href="../Orders/ViewDigitalGoods.aspx?OrderNumber=<%#Eval("OrderNumber")%>&OrderId=<%#Eval("OrderId")%>"><%# Eval("OrderNumber") %></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="OrderDate" HeaderText="Order Date" />
                        <asp:TemplateField HeaderText="Customer">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <%# Eval("BillToLastName") %>, <%# Eval("BillToFirstName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="EmptyDataMessage" runat="server" Text="There are no orders associated with this digital good."></asp:Label>
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <asp:HyperLink ID="BackLink" runat="server" Text="Back" SkinID="Button" NavigateUrl="DigitalGoods.aspx"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="ProductDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
        SelectCountMethod="CountForDigitalGood" SelectMethod="LoadForDigitalGood" SortParameterName="sortExpression"
        TypeName="">
        <SelectParameters>
            <asp:QueryStringParameter Name="digitalGoodId" QueryStringField="DigitalGoodId" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>