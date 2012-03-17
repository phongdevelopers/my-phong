<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditReview.aspx.cs" Inherits="Admin_Products_Reviews_EditReview" Title="Edit Review"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>



<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <div class="pageHeader">
        	<div class="caption">
        		<h1><asp:Localize ID="Caption" runat="server" Text="Edit Review"></asp:Localize></h1>
        	</div>
        </div>
        <table cellpadding="2" cellspacing="0" class="innerLayout">
            <tr>
                <td>
                     <table class="inputForm" cellpadding="3" cellspacing="3">
                        <tr>
                            <td align="center" colspan="2">
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                <asp:Label ID="SuccessMessage" runat="server" Text="Review updated at {0:t}" SkinID="GoodCondition" EnableViewState="false" Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader" style="vertical-align:top;">
                                <asp:Label ID="ProductNameLabel" runat="server" Text="Product:" AssociatedControlID="ProductLink" ToolTip="The product that was reviewed"></asp:Label>
                            </th>
                            <td colspan="2">
                                <asp:HyperLink ID="ProductLink" runat="server" NavigateUrl="../EditProduct.aspx?ProductId={0}"></asp:HyperLink>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="ReviewDateLabel" runat="server" Text="Date:" AssociatedControlID="ReviewDate" ToolTip="Date of the review"></asp:Label>
                            </th>
                            <td>
                                <asp:Label ID="ReviewDate" runat="Server" Text="{0:d}"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="ReviewerEmailLabel" runat="server" Text="Reviewer:" AssociatedControlID="ReviewerEmail" ToolTip="Email of the reviewer"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ReviewerEmail" runat="server" Width="200px" MaxLength="200"></asp:TextBox>
                                <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="ReviewerEmail" Required="true" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>
                             </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="ReviewerNameLabel" runat="server" Text="Name:" AssociatedControlID="ReviewerName" ToolTip="Name of the reviewer"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ReviewerName" runat="server" MaxLength="50"></asp:TextBox>
                             </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="ReviewerLocationLabel" runat="server" Text="Location:" AssociatedControlID="ReviewerLocation" ToolTip="Location of the reviewer"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ReviewerLocation" runat="server" MaxLength="50"></asp:TextBox>
                             </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="RatingLabel" runat="server" Text="Rating:" AssociatedControlID="Rating" ToolTip="Rating given for the review"></asp:Label>
                            </th>
                            <td>
                                <asp:Image ID="Rating" runat="server"></asp:Image>
                             </td>
                        </tr>
                        <tr>
                            <th class="rowHeader">
                                <asp:Label ID="ReviewTitleLabel" runat="server" Text="Title:" AssociatedControlID="ReviewTitle" ToolTip="Title of the review"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ReviewTitle" runat="server" MaxLength="100" Width="90%"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReviewTitleValidator" runat="server" ControlToValidate="ReviewTitle"
                                        ErrorMessage="Please provide a title for the review." Text="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader" valign="top">
                                <asp:Label ID="ReviewBodyLabel" runat="server" Text="Review:" AssociatedControlID="ReviewBody" ToolTip="Content of the review"></asp:Label>
                            </th>
                            <td>
                                <asp:TextBox ID="ReviewBody" runat="Server" Text="" Width="90%" Rows="8" TextMode="MultiLine" />
                                <asp:RequiredFieldValidator ID="ReviewBodyValidator" runat="server" ControlToValidate="ReviewBody"
                                        ErrorMessage="Review content is required." Text="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <br />                                
                                <asp:Button Id="SaveButon" runat="server" Text="Save" OnClick="SaveButton_Click" CssClass="button" />
                                <asp:Button Id="SaveAndCloseButton" runat="server" Text="Save and Close" OnClick="SaveAndCloseButton_Click" CssClass="button" />
								<asp:Button Id="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CssClass="button" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>
