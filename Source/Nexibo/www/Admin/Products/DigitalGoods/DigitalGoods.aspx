<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="DigitalGoods.aspx.cs" Inherits="Admin_Products_DigitalGoods_DigitalGoods" Title="Digital Goods" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<ajax:UpdatePanel runat="server" ID="VariantPanel" UpdateMode="Always">
    <ContentTemplate>
        <div class="pageHeader">
            <div class="caption">
                <h1><asp:Localize ID="Caption" runat="server" Text="Digital Goods: {0}" EnableViewState="false"></asp:Localize></h1>
            </div>
        </div>
	    <br/>
        <asp:GridView ID="DigitalGoodsGrid" runat="server" AutoGenerateColumns="False" 
            SkinID="PagedList" AllowPaging="False" AllowSorting="false" EnableViewState="false"
            OnRowCommand="DigitalGoodsGrid_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="Name">
                    <HeaderStyle HorizontalAlign="left" />
                    <ItemTemplate>
					    <a href="<%# string.Format("../../DigitalGoods/EditDigitalGood.aspx?CategoryId={0}&ProductId={1}&DigitalGoodId={2}", CategoryId, ProductId, Eval("DigitalGoodId")) %>"><asp:Literal ID="Name" runat="server" Text='<%# Eval("DigitalGood.Name") %>' EnableViewState="False"></asp:Literal></a>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="Size">
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                    <ItemTemplate>
                        <asp:Literal ID="Size" runat="server" Text='<%# GetFileSize(Eval("DigitalGood")) %>' EnableViewState="False"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>                    
                <asp:TemplateField>
                    <ItemStyle HorizontalAlign="Center" Width="80px" Wrap="false" />
                    <ItemTemplate>                            
                        <asp:HyperLink ID="Download" runat="server" NavigateUrl='<%# Eval("DigitalGoodId", "~/Admin/DigitalGoods/Download.ashx?DigitalGoodId={0}") %>' Visible='<%#DGFileExists(Eval("DigitalGood")) %>'><asp:Image ID="DownloadIcon" runat="server" SkinID="DownloadIcon" AlternateText="Download" ToolTip="Download" EnableViewState="false" /></asp:HyperLink>
                        <asp:Literal ID="MissingDownloadText" runat="server" Text="[file missing or inaccessible]" EnableViewState="false" Visible='<%#!DGFileExists(Eval("DigitalGood")) %>' />
                        <asp:ImageButton ID="Detach" runat="server" CommandName="Detach" CommandArgument='<%#Eval("ProductDigitalGoodId")%>' AlternateText="Delete" ToolTip="Delete" SkinID="DeleteIcon" OnClientClick='<%# Eval("DigitalGood.Name", "return confirm(\"Are you sure you want to remove {0} from this product?\")") %>' EnableViewState="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>			
                <asp:Localize ID="EmptyMessage" runat="server" Text="There are no digital goods attached to this product." EnableViewState="false"></asp:Localize>
            </EmptyDataTemplate>
        </asp:GridView><br />
        <asp:HyperLink ID="AllVariants" runat="server" SkinID="Button" Text="Attach Digital Good to Product" NavigateUrl="AttachDigitalGood.aspx?ProductId=" EnableViewState="false"></asp:HyperLink>
        <asp:PlaceHolder ID="VariantGoodsPanel" runat="server" EnableViewState="false">
	        <br /><br />
	        <div class="contentPanelBody" style="text-align:justify">
                <asp:Label ID="InstructionText" runat="server" Text="This product has variants.  Digital goods attached above apply to ANY variant.  You can also attach digital goods to specific variants using the grid below." EnableViewState="false"></asp:Label><br /><br />
            </div>
            <asp:Localize runat="server" ID="DisplayRangeLabel" EnableViewState="false" Text="Displaying {0:#,###} through {1:#,###} out of {2:#,###} unique variants" /><br />
            <asp:PlaceHolder ID="VariantPager" runat="server">
                <table cellspacing="0" cellpadding="4">
                    <tr>
                        <td>
                            <b>Page:</b>
                        </td>
                        <td>
                            <asp:LinkButton runat="server" ID="FirstLink" Text="&laquo; First" onclick="ChangePage" />
                            &nbsp;&nbsp;
                            <asp:LinkButton runat="server" ID="PreviousLink" Text="&laquo; Previous" onclick="ChangePage" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="JumpPage" AutoPostBack="true" OnSelectedIndexChanged="ChangePage"  />
                             / <asp:Label runat="server" ID="PageCountLabel" Style="font-size:10pt" EnableViewState="true" />&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:LinkButton runat="server" ID="NextLink" Text="Next &raquo;" onclick="ChangePage" />
                            &nbsp;&nbsp;
                            <asp:LinkButton runat="server" ID="LastLink" Text="Last &raquo;" onclick="ChangePage" />
                        </td>
                    </tr>
                </table>
            </asp:PlaceHolder>
            <asp:Repeater ID="VariantGrid" Runat="server" OnItemCreated="VariantGrid_ItemCreated">
                <HeaderTemplate>
	            <table class="summary" cellspacing="0" border="0" style="width:100%;border-collapse:collapse;">
	                <tr>
	                    <th><asp:Localize ID="RowHeader" runat="server" Text="Row" EnableViewState="False"></asp:Localize></th>
	                    <th><asp:Localize ID="VariantHeader" runat="server" Text="Variant" EnableViewState="False"></asp:Localize></th>
	                    <th><asp:Localize ID="DigitalGoodsHeader" runat="server" Text="Digital Good(s)" EnableViewState="False"></asp:Localize></th>
	                </tr>
	            </HeaderTemplate>
	            <ItemTemplate>
	                <asp:PlaceHolder ID="phVariantRow" runat="server"></asp:PlaceHolder>
	            </ItemTemplate>
	            <FooterTemplate>
	            </table>
	            </FooterTemplate>
            </asp:Repeater>
        </asp:PlaceHolder>
        <asp:HiddenField ID="VS_CustomState" runat="server" EnableViewState="false" />
    </ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>