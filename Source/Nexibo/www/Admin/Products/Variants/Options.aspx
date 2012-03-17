<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="Options.aspx.cs" Inherits="Admin_Products_Variants_Options" Title="Manage Options" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Manage Options for '{0}'"></asp:Localize></h1>
        </div>
    </div>
    <p align="justify">
        <asp:Localize ID="InstructionText" runat="server" Text="A product variant is a unique combination of options chosen when a customer purchases this product.  Use the controls below to define the options this product and the available choices for each.  Once you have defined all of the options and choices for a product, you can manage variants if you need to specify additional properties for each unique combination."></asp:Localize>
    </p>
    <ajax:UpdatePanel ID="PageAjax" runat="server">
        <ContentTemplate>
            <div align="center">
                <cb:SortedGridView ID="OptionsGrid" runat="server" AutoGenerateColumns="False"
                    ShowFooter="False" OnDataBound="OptionsGrid_DataBound" OnRowCommand="OptionsGrid_RowCommand" 
                    SkinID="PagedList" Width="500" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Order" ItemStyle-Width="56px">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:ImageButton ID="UpButton" runat="server" SkinID="UpIcon" CommandName="MoveUp" CommandArgument='<%#Eval("OptionId")%>' CausesValidation="false" />
                                <asp:ImageButton ID="DownButton" runat="server" SkinID="DownIcon" CommandName="MoveDown" CommandArgument='<%#Eval("OptionId")%>' CausesValidation="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Option">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>                                
                                <%# Eval("Name") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Header Text">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>                                
                                <%# Eval("HeaderText") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Option Choices">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>                                
                                <%# GetOptionNames(Container.DataItem) %> 
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Swatches">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="ShowSwatches" runat="server" Enabled="false" Checked='<%#Eval("ShowThumbnails")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemStyle VerticalAlign="Top" HorizontalAlign="Center" Wrap="false" Width="56px" />
                            <ItemTemplate>
                                <a href="<%#GetEditOptionUrl(Container.DataItem)%>"><asp:Image ID="EditOptionIcon" runat="server" SkinId="EditIcon" ToolTip="Edit Option" ></asp:Image></a>
                                <asp:ImageButton ID="DeleteButton" runat="server" ToolTip="Delete Option" SkinID="DeleteIcon" CommandName="DoDelete" CommandArgument='<%#Eval("OptionId")%>' OnClientClick="javascript:return confirmDel()" CausesValidation="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </cb:SortedGridView>
                <asp:Panel ID="navLinkPanel" runat="server">
                    <br />
                    <asp:HyperLink ID="VariantLink" runat="server" CssClass="button" Text="Manage Variants" NavigateUrl="Variants.aspx"></asp:HyperLink>
                    <br /><br />
                </asp:Panel>
            </div>
            <div class="sectionHeader">
                <h2><asp:Localize ID="AddOptionCaption" runat="server" Text="Add Option"></asp:Localize></h2>
            </div>
            <p><asp:Localize ID="AddOptionHelpText" runat="server" Text="Enter the name of the new option and the names of the available choices.  Separate choice names with a comma.  You can always change the choices later, but you must provide at least one choice name to add the option."></asp:Localize></p>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            <table cellspacing="4" cellpadding="2" border="0">
                <tr>
                    <td><asp:Label ID="AddOptionNameLabel" runat="server" SkinID="FieldHeader" Text="Option:"></asp:Label></td>
                    <td><asp:TextBox ID="AddOptionName" runat="server" MaxLength="50"></asp:TextBox><asp:RequiredFieldValidator ID="NameRequired" runat="server" ErrorMessage="Attribute name is required." Text="*" Display="Dynamic" ControlToValidate="AddOptionName"></asp:RequiredFieldValidator></td>
                    <td><asp:Label ID="AddOptionChoicesLabel" runat="server" SkinID="FieldHeader" Text="Choices:"></asp:Label></td>
                    <td><asp:TextBox ID="AddOptionChoices" runat="server" Columns="40"></asp:TextBox><asp:RequiredFieldValidator ID="OptionsRequired" runat="server" ErrorMessage="At least one choice must be specified." Text="*" Display="Dynamic" ControlToValidate="AddOptionChoices"></asp:RequiredFieldValidator></td>
                    <td><asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" OnClientClick="return confirmAdd();" /></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Localize ID="NameHelpText" runat="server" SkinID="HelpText" Text="(e.g. Color)"></asp:Localize>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Localize ID="OptionsHelpText" runat="server" SkinID="HelpText" Text="(e.g. Red, Blue, Green)"></asp:Localize>
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>