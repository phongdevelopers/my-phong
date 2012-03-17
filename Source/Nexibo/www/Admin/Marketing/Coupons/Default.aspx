<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Marketing_Coupons_Default" Title="Manage Coupons" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Manage Coupons"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <p align="justify">
                    <asp:Label ID="InstructionText" runat="server" Text="Manage the coupons that can be used in your store.  Coupons require the customer to enter a code during checkout to obtain a discount.  Coupons can provide for discounts toward the order amount, a specific product or line item, or shipping charges."></asp:Label><br /><br />
                </p>
            </td>
        </tr>
        <tr>
            <td class="itemList">
                <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <cb:SortedGridView ID="CouponGrid" runat="server" AutoGenerateColumns="False" DataSourceID="CouponDs" 
                            DataKeyNames="CouponId" AllowPaging="True" OnRowDataBound="CouponGrid_RowDataBound" OnRowCommand="CouponGrid_RowCommand" 
                            SkinId="PagedList" Width="100%" AllowSorting="true" DefaultSortExpression="Name">
                            <Columns>
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="Name" runat="server" NavigateUrl='<%# Eval("CouponId", "EditCoupon.aspx?CouponId={0}") %>' Text='<%# Eval("Name") %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Type">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="CouponType" runat="server" Text='<%# Eval("CouponType") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Discount">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Discount" runat="server" Text='<%# GetDiscount((Coupon)Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Restrictions">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Panel ID="MaxValuePanel" runat="server">
                                            <asp:Label ID="MaxValueLabel" runat="server" text="Max Value: " SkinID="FieldHeader"></asp:Label>
                                            <asp:Label ID="MaxValue" runat="server" Text='<%# Eval("MaxValue", "{0:lc}") %>'></asp:Label><br />
                                        </asp:Panel>
                                        <asp:Panel ID="MinPurchasePanel" runat="server">
                                            <asp:Label ID="MinPurchaseLabel" runat="server" text="Min Purchase: " SkinID="FieldHeader"></asp:Label>
                                            <asp:Label ID="MinPurchase" runat="server" Text='<%# Eval("MinPurchase", "{0:lc}") %>'></asp:Label><br />
                                        </asp:Panel>
                                        <asp:Panel ID="StartDatePanel" runat="server">
                                            <asp:Label ID="StartDateLabel" runat="server" text="Start Date: " SkinID="FieldHeader"></asp:Label>
                                            <asp:Label ID="StartDate" runat="server" Text='<%# Eval("StartDate", "{0:d}") %>'></asp:Label>
                                        </asp:Panel>
                                        <asp:Panel ID="EndDatePanel" runat="server">
                                            <asp:Label ID="EndDateLabel" runat="server" text="End Date: " SkinID="FieldHeader"></asp:Label>
                                            <asp:Label ID="EndDate" runat="server" Text='<%# Eval("EndDate", "{0:d}") %>'></asp:Label>
                                        </asp:Panel>
                                        <asp:Panel ID="MaximumUsesPanel" runat="server">
                                            <asp:Label ID="MaximumUsesLabel" runat="server" text="Max Uses: " SkinID="FieldHeader"></asp:Label>
                                            <asp:Label ID="MaximumUses" runat="server" Text='<%# Eval("MaxUses") %>'></asp:Label><br />
                                        </asp:Panel>
                                        <asp:Panel ID="MaximumUsesPerCustomerPanel" runat="server">
                                            <asp:Label ID="MaximumUsesPerCustomerLabel" runat="server" text="Uses Per Customer: " SkinID="FieldHeader"></asp:Label>
                                            <asp:Label ID="MaximumUsesPerCustomer" runat="server" Text='<%# Eval("MaxUsesPerCustomer") %>'></asp:Label><br />
                                        </asp:Panel>
                                        <asp:Panel ID="GroupsPanel" runat="server">
                                            <asp:Label ID="GroupsLabel" runat="server" text="Groups: " SkinID="FieldHeader"></asp:Label>
                                            <asp:Label ID="Groups" runat="server" Text='<%# GetNames((Coupon)Container.DataItem) %>'></asp:Label>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Uses">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="UsageLink" runat="server" Text='<%#GetUseCount((string)Eval("CouponCode"))%>' NavigateUrl='<%# string.Format("../../Reports/CouponUsage.aspx?CouponCode={0}", Server.UrlEncode(Eval("CouponCode").ToString())) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" Width="81px" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="EditLink" runat="server" NavigateUrl='<%# Eval("CouponId", "EditCoupon.aspx?CouponId={0}") %>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit"></asp:Image></asp:HyperLink>
                                        <asp:ImageButton ID="CopyButton" runat="server" AlternateText="Copy" SkinID="CopyIcon" CommandName="Copy" CommandArgument='<%#Eval("CouponId")%>' />
                                        <asp:ImageButton ID="DeleteButton" runat="server" AlternateText="Delete" SkinID="DeleteIcon" CommandName="Delete" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="NoCouponsText" runat="server" Text="<i>There are no coupons defined.</i>"></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>                
                    </ContentTemplate>
                </ajax:UpdatePanel>
                <br /><asp:HyperLink ID="AddLink" runat="server" Text="Add Coupon" NavigateUrl="AddCoupon.aspx" SkinID="Button"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="CouponDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForStore"
        TypeName="CommerceBuilder.Marketing.CouponDataSource" DataObjectTypeName="CommerceBuilder.Marketing.Coupon" DeleteMethod="Delete"
        SortParameterName="sortExpression">
    </asp:ObjectDataSource>
</asp:Content>



