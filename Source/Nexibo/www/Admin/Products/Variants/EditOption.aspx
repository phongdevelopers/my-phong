<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="EditOption.aspx.cs" Inherits="Admin_Products_Variants_EditOption" Title="Edit Option" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption">
        <h1><asp:Localize ID="Caption" EnableViewState="false" runat="server" Text="Edit Option '{0}' and its choices"></asp:Localize></h1>
    </div>
</div>
<table cellpadding="2" cellspacing="0" width="100%">
    <tr>
        <td>
            <asp:Table ID="OptionFieldsTable" runat="server" CssClass="summary" CellSpacing="0" GridLines="none" Width="100%">
                <asp:TableHeaderRow ID="HeaderRow" runat="server" HorizontalAlign="Center" EnableViewState="false" >
                    <asp:TableHeaderCell ID="OptionNameHdr" runat="server" Text="Option Name" HorizontalAlign="Left"></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="HeaderTextHdr" runat="server" Text="Header Text" HorizontalAlign="Left"></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="ShowSwatchesHdr" runat="server" Text="Show Swatches"></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="SwatchWidthHdr" runat="server" Text="Swatch&nbsp;Width"></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="SwatchHeightHdr" runat="server" Text="Swatch&nbsp;Height"></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="SwatchColumnsHdr" runat="server" Text="Swatch&nbsp;Columns"></asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow ID="TableRow" runat="server">
                    <asp:TableCell ID="OptionNameRow" runat="server">
                        <asp:TextBox ID="OptionName" runat="server" Text='<%# Option.Name %>' Width="160px" MaxLength="80"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="HeaderTextRow" runat="server" HorizontalAlign="Left">
                        <asp:TextBox ID="HeaderText" runat="server" Text='<%# Option.HeaderText %>' Width="160px" MaxLength="100"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="ShowSwatchesRow" runat="server" HorizontalAlign="Center">
                        <asp:CheckBox ID="ShowThumbnails" runat="server" Checked='<%# Option.ShowThumbnails %>' />
                    </asp:TableCell>
                    <asp:TableCell ID="SwatchWidthRow" runat="server" HorizontalAlign="Center">
                        <asp:TextBox ID="ThumbnailWidth" runat="server" Text='<%# ZeroAsEmpty(Option.ThumbnailWidth.ToString()) %>' Width="40px" MaxLength="3"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="SwatchHeightRow" runat="server" HorizontalAlign="Center">
                        <asp:TextBox ID="ThumbnailHeight" runat="server" Text='<%# ZeroAsEmpty(Option.ThumbnailHeight.ToString()) %>' Width="40px" MaxLength="3"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="SwatchColumnsRow" runat="server" HorizontalAlign="Center">
                        <asp:TextBox ID="ThumbnailColumns" runat="server" Text='<%# ZeroAsEmpty(Option.ThumbnailColumns.ToString()) %>' Width="40px" MaxLength="3"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
    </tr>
    <tr>
        <td>
            <div class="sectionHeader">
                <h2><asp:Localize runat="server" ID="EditChoicesLabel" Text="Choices for '{0}':"></asp:Localize></h2>
            </div>
            <asp:GridView ID="OptionChoicesGrid" runat="server" AutoGenerateColumns="False"
                ShowFooter="False" SkinID="Summary" OnRowDataBound="OptionChoicesGrid_RowDataBound" DataKeyNames="OptionChoiceId"
                OnRowCommand="OptionChoicesGrid_RowCommand" OnRowDeleting="OptionChoicesGrid_RowDeleting"
                Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="Order">
                        <ItemStyle HorizontalAlign="Center" Width="60px" />
                        <ItemTemplate>
                            <asp:ImageButton ID="UpButton" runat="server" SkinID="UpIcon" CommandName="MoveUp" CommandArgument='<%#Container.DataItemIndex%>' CausesValidation="false" AlternateText="Move Up" />
                            <asp:ImageButton ID="DownButton" runat="server" SkinID="DownIcon" CommandName="MoveDown" CommandArgument='<%#Container.DataItemIndex%>' CausesValidation="false" AlternateText="Move Down" />
                        </ItemTemplate>
                    </asp:TemplateField>                
                    <asp:TemplateField HeaderText="Choice Name">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:TextBox ID="Name" runat="server" Text='<%# Eval("Name") %>' Width="95px"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:TextBox ID="PriceMod" runat="server" Text='<%# EmptyZero(Eval("PriceModifier")) %>' Width="30px" MaxLength="8"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Weight">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:TextBox ID="WeightMod" runat="server" Text='<%# EmptyZero(Eval("WeightModifier")) %>' Width="30px" MaxLength="8"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sku">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:TextBox ID="SkuMod" runat="server" Text='<%# Eval("SkuModifier") %>' Width="30px" MaxLength="8"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Swatch">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:TextBox ID="ThumbnailUrl" runat="server" Text='<%# Eval("ThumbnailUrl") %>' Width="150px"></asp:TextBox>
                            <asp:ImageButton ID="BrowseThumbnailUrl" runat="server" SkinID="FindIcon" AlternateText="Browse" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Image">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:TextBox ID="ImageUrl" runat="server" Text='<%# Eval("ImageUrl") %>' Width="150px"></asp:TextBox>
                            <asp:ImageButton ID="BrowseImageUrl" runat="server" SkinID="FindIcon" AlternateText="Browse" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="26px" ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="DeleteButton" runat="server" SkinID="DeleteIcon" CommandName="Delete" OnClientClick="javascript:return confirmDel()" CausesValidation="false" AlternateText="Delete" />
                        </ItemTemplate>
                    </asp:TemplateField>                    
                </Columns>
                <EmptyDataTemplate>
                    <asp:Localize ID="NoChoiceMessage" runat="server" Text="There are no choices for this option. Use 'Add Choice' dialog to add choices."></asp:Localize>
                </EmptyDataTemplate>
            </asp:GridView><br />
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" CausesValidation="false" Visible="false" />
            <asp:Button ID="SaveCloseButton" runat="server" Text="Save And Close" OnClick="SaveCloseButton_Click" CausesValidation="false" Visible="false" />
			<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
        </td>
    </tr>
</table>
<br />
<div class="sectionHeader">
    <h2><asp:Localize ID="AddChoiceCaption" runat="server" Text="Add Choice"></asp:Localize></h2>
</div>
<asp:Label ID="AddAttributeHelpText" runat="server" Text="Enter the name of the choice to add in the box below.  You can also include a swatch and a product image."></asp:Label>
<asp:ValidationSummary ID="ValidationSummary1" runat="server" />
<table>
    <tr>
        <td align="right">
            <asp:Label ID="AddChoiceNameLabel" runat="server" SkinID="FieldHeader" Text="Name:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="AddChoiceName" runat="server" Width="160px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="OptionsRequired" runat="server" ErrorMessage="At least one option name must be specified." Text="*" Display="Dynamic" ControlToValidate="AddChoiceName"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:Label ID="AddChoiceThumbnailLabel" runat="server" SkinID="FieldHeader" Text="Swatch:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="AddChoiceThumbnail" runat="server" width="200px"></asp:TextBox>
            <asp:ImageButton ID="BrowseThumbnail" runat="server" SkinID="FindIcon" />
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:Label ID="AddChoiceImageLabel" runat="server" SkinID="FieldHeader" Text="Image:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="AddChoiceImage" runat="server" width="200px"></asp:TextBox>
            <asp:ImageButton ID="BrowseImage" runat="server" SkinID="FindIcon" />
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:Label ID="AddChoicePriceModLabel" runat="server" SkinID="FieldHeader" Text="Price Mod:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="AddChoicePriceMod" runat="server" width="40px" MaxLength="8"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:Label ID="AddChoiceWeightModLabel" runat="server" SkinID="FieldHeader" Text="Weight Mod:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="AddChoiceWeightMod" runat="server" width="40px" MaxLength="8"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:Label ID="AddChoiceSkuModLabel" runat="server" SkinID="FieldHeader" Text="Sku Mod:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="AddChoiceSkuMod" runat="server" width="40px" MaxLength="8"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td><asp:Button ID="AddChoiceButton" runat="server" CssClass="button" Text="add choice" OnClick="AddChoiceButton_Click" /></td>
    </tr>
</table>
</asp:Content>