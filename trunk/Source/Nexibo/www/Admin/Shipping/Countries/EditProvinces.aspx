<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditProvinces.aspx.cs" Inherits="Admin_Shipping_Countries_EditProvinces" Title="Edit Provinces"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="AddProvinceDialog.ascx" TagName="AddProvinceDialog" TagPrefix="uc" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ContentAjax" runat="server" UpdateMode="conditional">
        <ContentTemplate>
            <div class="pageHeader">
            	<div class="caption">
            		<h1><asp:Localize ID="Caption" runat="server" Text="{0}: States and Provinces"></asp:Localize></h1>
            	</div>
            </div>
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td class="itemList">
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="EditProvince" />
                        <cb:SortedGridView ID="ProvinceGrid" runat="server" AutoGenerateColumns="False"
                            DataKeyNames="ProvinceId,CountryCode" DataSourceID="ProvinceDs" DefaultSortDirection="Ascending"
                            DefaultSortExpression="Name" AllowSorting="true" SkinID="PagedList" Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Code" SortExpression="ProvinceCode">
                                    <HeaderStyle HorizontalAlign="Center" Width="60px" />
                                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                                    <ItemTemplate>
                                        <asp:Label ID="ProvinceCode" runat="server" Text='<%# Eval("ProvinceCode") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="EditProvinceCode" runat="server" Width="45px" MaxLength="4" Text='<%# Bind("ProvinceCode") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <HeaderStyle HorizontalAlign="Left" Width="200px" />
                                    <ItemStyle HorizontalAlign="Left" Width="200px" />
                                    <ItemTemplate>
                                        <asp:Label ID="Name" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="EditName" runat="server" Text='<%# Bind("Name") %>' Width="185px" MaxLength="50"></asp:TextBox>
                                        <cb:RequiredRegularExpressionValidator ID="NameValidator" runat="server" ControlToValidate="EditName"
                                            Display="Static" ErrorMessage="Name must be between 1 and 50 characters in length." Text="*"
                                            ValidationGroup="EditProvince" ValidationExpression=".{1,50}" Required="true">
                                        </cb:RequiredRegularExpressionValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderStyle HorizontalAlign="Center" Width="60px" />
                                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="EditButton" runat="server" CommandName="Edit"><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" /></asp:LinkButton>
                                        <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" /></asp:LinkButton>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:ImageButton ID="EditSaveButton" runat="server" CausesValidation="True" ValidationGroup="EditProvince" CommandName="Update" SkinID="SaveIcon" ToolTip="Save"></asp:ImageButton>
                                        <asp:ImageButton ID="EditCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" SkinID="CancelIcon" ToolTip="Cancel"></asp:ImageButton>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyMessage" runat="server" Text="There are no states or provinces defined for this country."></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                        <div style="margin-top:10px;text-align:center">
                            <asp:HyperLink ID="ReturnLink" runat="server" Text="Return to Countries" SkinID="Link" NavigateUrl="Default.aspx"></asp:HyperLink>
                        </div>
                        <asp:ObjectDataSource ID="ProvinceDs" runat="server" DataObjectTypeName="CommerceBuilder.Shipping.Province"
                            DeleteMethod="Delete" InsertMethod="Insert" OldValuesParameterFormatString="original_{0}"
                            SelectMethod="LoadForCountry" TypeName="CommerceBuilder.Shipping.ProvinceDataSource"
                            UpdateMethod="Update" SortParameterName="sortExpression">
                            <SelectParameters>
                                <asp:QueryStringParameter Name="countryCode" QueryStringField="CountryCode" Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td class="detailPanel">
                        <uc:AddProvinceDialog ID="AddProvinceDialog1" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

