<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_People_Vendors_Default" Title="Manage Vendors"  %>



<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
  	<div class="caption">
  		<h1><asp:Localize ID="Caption" runat="server" Text="Vendors"></asp:Localize></h1>
  	</div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">      
        <tr>
            <td align="left" valign="top" class="itemList">
                <asp:GridView ID="VendorGrid" runat="server" AllowPaging="True" PageSize="20" AllowSorting="False"
                AutoGenerateColumns="False" DataKeyNames="VendorId" DataSourceID="VendorDs" 
                 SkinID="PagedList" ShowFooter="False" Width="100%" BorderWidth="0">
                <Columns>
                    <asp:TemplateField HeaderText="Name" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:HyperLink ID="NameLabel" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%#Eval("VendorId", "EditVendor.aspx?VendorId={0}")%>'></asp:Hyperlink>
                        </ItemTemplate>
                        <HeaderStyle CssClass="Left" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Email" HeaderText="Email" ItemStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="Products">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="ProductsLabel" runat="server" Text='<%#GetProductCount(Container.DataItem)%>' NavigateUrl='<%#Eval("VendorId", "EditVendor.aspx?VendorId={0}")%>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False" >
                        <ItemTemplate>
                            <div align="center">
                                <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%#Eval("VendorId", "EditVendor.aspx?VendorId={0}")%>'><asp:Image ID="EditIcon" SkinID="Editicon" runat="server" /></asp:HyperLink>
                                <asp:ImageButton ID="DeleteButton" runat="server" CausesValidation="False" SkinID="DeleteIcon" CommandName="Delete" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' Visible='<%#ShowDeleteButton(Container.DataItem)%>' AlternateText="Delete" />
                                <asp:HyperLink ID="DeleteLink" runat="server" NavigateUrl='<%# Eval("VendorId", "DeleteVendor.aspx?VendorId={0}")%>' Visible='<%# ShowDeleteLink(Container.DataItem) %>'><asp:Image ID="DeleteIcon2" runat="server" SkinID="DeleteIcon" AlternateText="Delete" /></asp:HyperLink>
                            </div>
                        </ItemTemplate>
                        <ItemStyle Wrap="false" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>                   
                    <asp:Label ID="EmptyMessage" runat="server" Text="There are no vendors defined for your store."></asp:Label>              
                </EmptyDataTemplate>
            </asp:GridView>
            </td>
            <td align="left" valign="top" style="width:300px;">
            <div class="section">
            
                <div class="header">
                    <h2 class="addmanufacturer"><asp:Localize ID="AddShipZoneCaption" runat="server" Text="Add Vendor" /></h2>
                </div>
                
                <div class="content">    
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                <asp:Label ID="AddVendorNameLabel" runat="server" Text="Name: " SkinID="FieldHeader"></asp:Label>
                <asp:TextBox ID="AddVendorName" runat="server" MaxLength="100"></asp:TextBox>
                <asp:RegularExpressionValidator ID="NameValidator" runat="server" ErrorMessage="Maximum length for Name is 100 characters." Text="*" ControlToValidate="AddVendorName" ValidationExpression=".{0,100}"  ></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="AddVendorNameRequired" runat="server" ControlToValidate="AddVendorName"
                    Display="Dynamic" ErrorMessage="Vendor name is required.">*</asp:RequiredFieldValidator>
                <asp:Button ID="AddVendorButton" runat="server" Text="Add"  OnClick="AddVendorButton_Click" />
                <br />
                <asp:ObjectDataSource ID="VendorDs" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="LoadForStore" TypeName="CommerceBuilder.Products.VendorDataSource" SelectCountMethod="CountForStore" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Products.Vendor" DeleteMethod="Delete" InsertMethod="Insert" UpdateMethod="Update">
        </asp:ObjectDataSource>
                </div>
            </div>    
            </td>
        </tr>
    </table>
</asp:Content>

