<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Shipping_Zones_Default" Title="Zones" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel runat="server" ID="ShipZonePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
            	<div class="caption">
            		<h1><asp:Localize ID="Caption" runat="server" Text="Configure Zones"></asp:Localize></h1>
            	</div>
            </div>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td style="padding:4px 4px 10px 4px; vertical-align:top; border: 1px solid #738AD0; width:60%;">
                        <cb:SortedGridView ID="ShipZoneGrid" runat="server" AllowPaging="False" AllowSorting="True"
                            AutoGenerateColumns="False" DataKeyNames="ShipZoneId" DataSourceID="ShipZoneDs" 
                            ShowFooter="False" Width="100%" SkinID="PagedList" DefaultSortExpression="Name" OnRowCommand="ShipZoneGrid_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <HeaderStyle HorizontalAlign="left" width="120px" />
                                    <ItemStyle VerticalAlign="top" width="120px" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="NameLink" runat="server" NavigateUrl='<%#Eval("ShipZoneId", "EditShipZone.aspx?ShipZoneId={0}")%>' Text='<%# Eval("Name") %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Countries">
                                    <HeaderStyle HorizontalAlign="left" width="100px" />
                                    <ItemStyle VerticalAlign="top" width="100px" />
                                    <ItemTemplate>
                                        <asp:Label ID="CountriesLabel" runat="server" Text='<%#GetCountryNames(Container.DataItem)%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Provinces">
                                    <HeaderStyle HorizontalAlign="left" Width="200px" />
                                    <ItemStyle Width="200px" VerticalAlign="top" />
                                    <ItemTemplate>
                                        <asp:Label ID="ProvincesLabel" runat="server" Text='<%#GetProvinceNames(Container.DataItem)%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Postal Codes">
                                    <HeaderStyle HorizontalAlign="left" Width="100px" />
                                    <ItemStyle VerticalAlign="top" Width="100px" />
                                    <ItemTemplate>
                                        <asp:Label ID="PostalCodesLabel" runat="server" Text='<%#GetPostalCodes(Container.DataItem)%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False" >
                                    <ItemStyle Width="90px" VerticalAlign="top" />
                                    <ItemTemplate>
                                        <div align="center">
                                            <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("ShipZoneId", "EditShipZone.aspx?ShipZoneId={0}")%>'><asp:Image ID="EditIcon" SkinID="Editicon" runat="server" AlternateText="Edit" /></asp:HyperLink>
                                            <asp:ImageButton ID="CopyButton" runat="server" AlternateText="Copy" SkinID="CopyIcon" CommandName="Copy" ToolTip="Copy" CommandArgument='<%#Container.DataItemIndex%>' />
                                            <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No shipping zones are defined for your store.
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </td>
                    <td class="detailPanel">
                        <div class="section">
                            <div class="header">
                                <h2 class="addshippingzone"><asp:Localize ID="AddShipZoneCaption" runat="server" Text="Add Zone" /></h2>
                            </div>
                            <div class="content">
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" />
                                <cb:ToolTipLabel ID="AddShipZoneNameLabel" runat="server" Text="Name: " SkinId="FieldHeader" ToolTip="Name of the zone for merchant reference.  This value is not displayed to customers." />
                                <asp:TextBox ID="AddShipZoneName" runat="server" MaxLength="100"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="AddShipZoneNameRequired" runat="server" ControlToValidate="AddShipZoneName"
                                    Display="Static" ErrorMessage="Zone name is required." Text="*" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                <asp:Button ID="AddShipZoneButton" runat="server" Text="Add" OnClick="AddShipZoneButton_Click" ValidationGroup="Add" />
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="ShipZoneDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Shipping.ShipZoneDataSource" SelectCountMethod="CountForStore" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Shipping.ShipZone" DeleteMethod="Delete" InsertMethod="Insert" UpdateMethod="Update">
    </asp:ObjectDataSource>
</asp:Content>