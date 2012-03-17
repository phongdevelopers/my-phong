<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Shipping_Countries_Default" Title="Manage Countries"  %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="AddCountryDialog.ascx" TagName="AddCountryDialog" TagPrefix="uc" %>
<%@ Register Src="EditCountryDialog.ascx" TagName="EditCountryDialog" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ContentAjax" runat="server" UpdateMode="conditional">
        <ContentTemplate>
            <div class="pageHeader">
            	<div class="caption">
            		<h1><asp:Localize ID="Caption" runat="server" Text="Countries"></asp:Localize></h1>
            	</div>
            </div>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td class="itemList">
                        <br />
                        <asp:Label ID="AlphabetRepeaterLabel" AssociatedControlID="AlphabetRepeater" runat="server" Text="Quick Search:" SkinID="FieldHeader"></asp:Label>
                        <asp:Repeater runat="server" id="AlphabetRepeater" OnItemCommand="AlphabetRepeater_ItemCommand">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="LinkButton1" CommandName="Display" CommandArgument="<%#Container.DataItem%>" Text="<%#Container.DataItem%>" ValidationGroup="Search" />
                            </ItemTemplate>                                    
                        </asp:Repeater>
                        <br />
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="CountryList" />
                        <cb:SortedGridView ID="CountryGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="CountryCode" DataSourceId="CountryDs" 
                            SkinID="PagedList" AllowPaging="true" PageSize="25" AllowSorting="true" DefaultSortDirection="ascending" 
                            DefaultSortExpression="Name" Width="100%" OnRowEditing="CountryGrid_RowEditing" OnRowCancelingEdit="CountryGrid_RowCancelingEdit">
                            <Columns>
                                <asp:TemplateField HeaderText="Code" SortExpression="CountryCode">
                                    <HeaderStyle Width="50px" HorizontalAlign="left" />
                                    <ItemStyle Width="50px" HorizontalAlign="left" />
                                    <ItemTemplate>
                                        <asp:Label ID="CountryCode" runat="server" Text='<%#Eval("CountryCode")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <HeaderStyle Width="125px" HorizontalAlign="left" />
                                    <ItemStyle Width="125px" HorizontalAlign="left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Name" runat="server" Text='<%#Eval("Name")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Provinces" ItemStyle-HorizontalAlign="center">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ProvincesLink" runat="server" Text='<%#Eval("Provinces.Count")%>' NavigateUrl='<%# Eval("CountryCode", "EditProvinces.aspx?CountryCode={0}")%>' SkinID="GridLink"></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" width="60px" />
                                    <EditItemTemplate>
                                        <asp:LinkButton ID="CancelLinkButton" runat="server" CausesValidation="False" CommandName="Cancel"><asp:Image ID="CancelIcon" runat="server" SkinID="CancelIcon" AlternateText="Cancel" /></asp:LinkButton>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="EditLinkButton" runat="server" CausesValidation="False" CommandName="Edit"><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" /></asp:LinkButton>
                                        <asp:LinkButton ID="DeleteLinkButton" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyData">
                                    <asp:Label ID="CountriesInstructionText" runat="server" Text="Define the country list for your store."></asp:Label><br /><br />
                                </div>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                    </td>
                    <td class="detailPanel" style="width:40%;">
                        <uc:AddCountryDialog ID="AddCountryDialog1" runat="server" />
                        <uc:EditCountryDialog ID="EditCountryDialog1" runat="server" OnItemUpdated="EditCountryDialog1_ItemUpdated" OnCancelled="EditCountryDialog1_Cancelled"  Visible="false" />
                   </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="CountryDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForCriteria" TypeName="CommerceBuilder.Shipping.CountryDataSource" SelectCountMethod="CountForCriteria"
        SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Shipping.Country"
        DeleteMethod="Delete">
            <SelectParameters>
                <asp:Parameter Name="sqlCriteria" Type="string" DefaultValue="" />
            </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

