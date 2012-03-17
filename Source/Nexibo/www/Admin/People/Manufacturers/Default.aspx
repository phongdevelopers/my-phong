<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_People_Manufacturers_Default" Title="Manage Manufacturers"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Manufacturers"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td valign="top"  class="mainPanel">
                <ajax:UpdatePanel ID="SearchAjax" runat="server" UpdateMode="Conditional" >                    
                    <ContentTemplate>
                        <div class="section">
                            <div class="header">
                                <h2 class="searchforuser"><asp:Localize ID="Localize1" runat="server" Text="Search for Manufacturers"></asp:Localize></h2>
                            </div>                            
                            <div class="content">
                                <asp:Label ID="SearchByLabel" runat="server" Text="Search by Name:" SkinID="FieldHeader"
                                    ToolTip="Specify search pattern"></asp:Label>
                                <asp:TextBox ID="SearchText" runat="server"></asp:TextBox>&nbsp;
                                <asp:Button ID="SearchButton" CausesValidation="false" runat="server" Text="Search"
                                    SkinID="AdminButton" OnClick="SearchButton_Click" /><br />
                                <asp:Label ID="InstructionText" runat="server" Text="Wildcard characters * and ? are accepted."></asp:Label><br />
                                <br />
                                <asp:Label ID="AlphabetRepeaterLabel" AssociatedControlID="AlphabetRepeater" runat="server"
                                    Text="Quick Search:" SkinID="FieldHeader"></asp:Label>
                                <asp:Repeater runat="server" ID="AlphabetRepeater" OnItemCommand="AlphabetRepeater_ItemCommand">
                                    <ItemTemplate>
                                        <asp:LinkButton CausesValidation="false" runat="server" ID="LinkButton1" CommandName="Display"
                                            CommandArgument="<%#Container.DataItem%>" Text="<%#Container.DataItem%>" />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>                            
                            <div class="section">
                                <div class="content">
                                <cb:SortedGridView ID="ManufacturerGrid" runat="server" AllowPaging="true" AllowSorting="true" PageSize="20"
                                    AutoGenerateColumns="False" DataKeyNames="ManufacturerId" DataSourceID="ManufacturerDs" 
                                    ShowFooter="False" DefaultSortExpression="Name" SkinID="PagedList" Width="100%">
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <ItemStyle HorizontalAlign="Center" Width="30px" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\");") %>' Visible='<%# !HasProducts(Container.DataItem) %>'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:LinkButton>
                                                <asp:HyperLink ID="DeleteLink" runat="server" CausesValidation="False" NavigateUrl='<%# Eval("ManufacturerId", "DeleteManufacturer.aspx?ManufacturerId={0}")%>' Visible='<%# HasProducts(Container.DataItem) %>'><asp:Image ID="DeleteIcon2" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="NameLink" runat="server" NavigateUrl='<%#Eval("ManufacturerId", "EditManufacturer.aspx?ManufacturerId={0}")%>' Text='<%# Eval("Name") %>'></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Products">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:HyperLink ID="ProductsLabel" runat="server" Text='<%#GetProductCount(Container.DataItem)%>' NavigateUrl='<%#Eval("ManufacturerId", "EditManufacturer.aspx?ManufacturerId={0}")%>'></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="EmptyDataText" runat="server" Text="No manufacturers are defined for your store."></asp:Label>
                                    </EmptyDataTemplate>
                                </cb:SortedGridView>
                             </div>
                        </div>
                    </div>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
            <td valign="top">
                <div class="section">
                    <div class="header">
                        <h2 class="addmanufacturer"><asp:Localize ID="AddCaption" runat="server" Text="Add Manufacturer" /></h2>
                    </div>
                    <div class="content">
                        <ajax:UpdatePanel ID="AddAjax" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                <asp:Label ID="AddedMessage" runat="server" Text="{0} added.<br />" SkinID="GoodCondition" Visible="false"></asp:Label>
                                <table class="inputForm">
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="AddManufacturerNameLabel" runat="server" Text="Name:" AssociatedControlID="AddManufacturerName" ToolTip="Name of manufacturer"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="AddManufacturerName" runat="server" MaxLength="100"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="AddManufacturerNameRequired" runat="server" ControlToValidate="AddManufacturerName"
                                                Display="Static" ErrorMessage="Manufacturer name is required." Text="*"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            <asp:Button ID="AddManufacturerButton" runat="server" Text="Add" OnClick="AddManufacturerButton_Click" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:ObjectDataSource ID="ManufacturerDs" runat="server" OldValuesParameterFormatString="original_{0}"
                                    SelectMethod="FindManufacturersByName" TypeName="CommerceBuilder.Products.ManufacturerDataSource"
                                    EnablePaging="True" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Products.Manufacturer"
                                    DeleteMethod="Delete">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="SearchText" Name="searchPattern" PropertyName="Text"
                                            Type="String" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </div>
                </div>
            </td>
        </tr>
    </table>   
</asp:Content>
