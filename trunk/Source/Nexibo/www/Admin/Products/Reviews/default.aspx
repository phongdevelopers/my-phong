<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Products_Reviews_Default" Title="Manage Reviews"  EnableViewState="false" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript">
    function ShowHoverPanel(event,Id){OrderHoverLookupPanel.startCallback(event,"OrderId="+Id.toString(),null,OnError);}
    function HideHoverPanel(){OrderHoverLookupPanel.hide();}
    function OnError(e){alert("***Error:\r\n\r\n"+e.message);}
    function toggleCheckBoxState(id,checkState){var cb=document.getElementById(id);if(cb!=null)cb.checked=checkState;}
    function toggleSelected(checkState){if(CheckBoxIDs!=null){for(var i=0;i<CheckBoxIDs.length;i++)toggleCheckBoxState(CheckBoxIDs[i],checkState.checked);}}


</script>
<div class="pageHeader">
	<div class="caption">
		<h1>
                <asp:Localize ID="Caption" runat="server" Text="Product Reviews"></asp:Localize>
                <asp:Localize ID="ProductCaption" runat="server" Text=" for {0}"></asp:Localize>
            </h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td style="padding:10px 0 10px 0 ;">
            <ajax:UpdatePanel ID="ReviewAjax" runat="server">
                <ContentTemplate>
                    <div class="section">
                        <div class="header">
                            <h2 class="findproductreview"><asp:Localize ID="FindProductsCaption" runat="server" Text="Find Reviews"></asp:Localize></h2>
                        </div>
                        <div class="content">
                            <table class="inputForm">
                                <tr>
                                    <th class="rowHeader">
                                        <cb:ToolTipLabel ID="ShowApprovedLabel" runat="server" Text="Approval Status:" AssociatedControlId="ShowApproved" ToolTip="Indicate whether the result should include reviews that are approved, unapproved or both." />
                                    </th>
                                    <td>
                                        <asp:DropDownList ID="ShowApproved" runat="server">
                                            <asp:ListItem Value="Any" Text="Any"></asp:ListItem>
                                            <asp:ListItem Value="True" Text="Approved"></asp:ListItem>
                                            <asp:ListItem Value="False" Text="Unapproved" Selected="true"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Button ID="SearchButton" runat="server" Text="Search" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <cb:SortedGridView ID="ReviewGrid" runat="server" AllowPaging="true" AllowSorting="true" PageSize="20"
                        AutoGenerateColumns="False" DataKeyNames="ProductReviewId" DataSourceID="ProductReviewDs" 
                        ShowFooter="False" DefaultSortExpression="Name" SkinID="PagedList" Width="100%" OnDataBound="ReviewGrid_DataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Select">
                                <HeaderStyle Width="40px" />
                                <HeaderTemplate>
                                    <input type="checkbox" onclick="toggleSelected(this)" />
                                </HeaderTemplate>
                                <ItemStyle HorizontalAlign="center" VerticalAlign="Top" Width="40px" />
                                <ItemTemplate>
                                    <asp:CheckBox ID="Selected" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Approved" SortExpression="IsApproved">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="center" VerticalAlign="Top" />
                                <ItemTemplate>
                                    <asp:Label ID="Approved" runat="server" Text='<%# ((bool)Eval("IsApproved") ? "X" : "") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date" SortExpression="ReviewDate">
                                <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                                <ItemStyle VerticalAlign="Top" />
                                <ItemTemplate>
                                    <asp:Label ID="ReviewDate" runat="server" Text='<%# Eval("ReviewDate", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product" SortExpression="P.Name">
                                <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                                <ItemStyle VerticalAlign="Top" />
                                <ItemTemplate>
                                    <asp:HyperLink ID="Product" runat="server" NavigateUrl='<%# Eval("ProductId", "../EditProduct.aspx?ProductId={0}") %>' Text='<%#Eval("Product.Name")%>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Reviewer" SortExpression="DisplayName">
                                <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                                <ItemStyle VerticalAlign="Top" />
                                <ItemTemplate>
                                    <asp:Label ID="ReviewerEmail" runat="server" Text='<%# Eval("ReviewerProfile.Email") %>'></asp:Label>
                                    <asp:Label ID="ReviewerEmailVerified" runat="server" Visible='<%# Eval("ReviewerProfile.EmailVerified") %>' Text=" (verified)"></asp:Label><br />
                                    <asp:Label ID="ReviewerName" runat="server" Text='<%# Eval("ReviewerProfile.DisplayName", "{0}") %>'></asp:Label>
                                    <asp:Label ID="ReviewerLocation" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("ReviewerProfile.Location").ToString()) %>' Text='<%# Eval("ReviewerProfile.Location", " from {0}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Rating" SortExpression="Rating">
                                <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                                <ItemStyle VerticalAlign="Top" />
                                <ItemTemplate>
                                    <asp:Image ID="Rating" runat="server" ImageUrl='<%# NavigationHelper.GetRatingImage(AlwaysConvert.ToDecimal(Eval("Rating")))%>' AlternateText='<%# Eval("Rating") %>'></asp:Image>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Review" SortExpression="ReviewTitle">
                                <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                                <ItemStyle VerticalAlign="Top" width="300px" />
                                <ItemTemplate>
                                    <asp:Label ID="ReviewTitleLabel" runat="server" Text="Title:" SkinID="FieldHeader"></asp:Label>
                                    <asp:Label ID="ReviewTitle" runat="server" Text='<%# Eval("ReviewTitle") %>'></asp:Label><br /><br />
                                    <asp:Label ID="ReviewBody" runat="server" Text='<%#"<pre class=Reviews>" +  Eval("ReviewBody")+ "</pre>" %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemStyle HorizontalAlign="center" VerticalAlign="Top" width="60px" />
                                <ItemTemplate>
                                    <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%# Eval("ProductReviewId", "EditReview.aspx?ReviewId={0}") %>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit"></asp:Image></asp:HyperLink>
                                    <asp:ImageButton ID="DeleteButton" runat="server" SkinID="DeleteIcon" AlternateText="Delete" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this review?')" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Label ID="EmptyDataText" runat="server" Text="No matching reviews found."></asp:Label>
                        </EmptyDataTemplate>
                    </cb:SortedGridView>
                    <asp:HiddenField ID="HiddenProductId" runat="server" />
                    <asp:ObjectDataSource ID="ProductReviewDs" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="Search" TypeName="CommerceBuilder.Products.ProductReviewDataSource" 
                        SelectCountMethod="SearchCount" SortParameterName="sortExpression" DataObjectTypeName="CommerceBuilder.Products.ProductReview" 
                        DeleteMethod="Delete" EnablePaging="true">
                        <SelectParameters>
                            <asp:ControlParameter Name="productId" ControlID="HiddenProductId" PropertyName="Value" Type="Int32" />
                            <asp:ControlParameter Name="approved" ControlID="ShowApproved" PropertyName="SelectedValue" Type="Object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:Panel ID="ReviewActionPanel" runat="server">
                        <br />
                        <table class="inputForm">
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="ReviewActionLabel" runat="server" Text="Selected Reviews:" AssociatedControlId="ReviewAction" ToolTip="Select an action to take with the selected reviews." />
                                </th>
                                <td>
                                    <asp:DropDownList ID="ReviewAction" runat="server">
                                        <asp:ListItem Text=""></asp:ListItem>
                                        <asp:ListItem Text="Approve"></asp:ListItem>
                                        <asp:ListItem Text="Disapprove"></asp:ListItem>
                                        <asp:ListItem Text="Delete"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:Button ID="GoButton" runat="server" Text="Go" OnClick="GoButton_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </ajax:UpdatePanel>
        </td>
    </tr>
</table>
</asp:Content>
