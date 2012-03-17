<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" CodeFile="ViewPart.aspx.cs" Inherits="Admin_Products_Kits_ViewPart" Title="View Kit Part"  %>
<%@ Register Src="../ProductMenu.ascx" TagName="ProductMenu" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<table class="contentPanel" style="width: 100%">
    <tr>        
        <td valign="top">
            <h1>View Kit Part</h1>
            <asp:Label ID="InstructionText" runat="Server" Text="The selected product is a part of the following kit components:"></asp:Label>
            <ajax:UpdatePanel ID="CompoentDetailPanel" runat="server">
                <ContentTemplate>
                    <asp:DataList ID="ComponentList" runat="server" OnSelectedIndexChanged="ComponentList_SelectedIndexChanged" DataKeyField="KitComponentId">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li><asp:LinkButton ID="Name" runat="server" Text='<%#Eval("Name")%>' CommandName="Select"></asp:LinkButton></li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:DataList>
                    <asp:Panel ID="ComponentDetail" runat="server" Visible="false">
                        <br />
                        <h2><asp:Label ID="SelectedComponentName" runat="server"></asp:Label></h2>
                        <asp:Label ID="ComponentHelpText" runat="Server" Text="The selected component is attached to the following products:"></asp:Label>
                        <asp:DataList ID="KitList" runat="server">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li><asp:HyperLink ID="Name" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%#Eval("ProductId", "EditKit.aspx?ProductId={0}")%>'></asp:HyperLink></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:DataList>
                    </asp:Panel>
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
    </tr>
</table>
</asp:Content>

