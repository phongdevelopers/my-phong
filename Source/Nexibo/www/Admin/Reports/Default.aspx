<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Reports_Default" Title="Reports Menu"  %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Reports"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top" width="50%">
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="SalesCaption" runat="server" Text="Sales Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="SalesSummaryLink" runat="server" Text="Sales Summary" NavigateUrl="SalesSummary.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesSummaryDescription" runat="server" Text="Provides sales summary for a given period."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="DailySalesLink" runat="server" Text="Daily Sales" NavigateUrl="DailySales.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="DailySalesDescription" runat="server" Text="Provides sales totals for a single day."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="MonthlySalesLink" runat="server" Text="Monthly Sales" NavigateUrl="MonthlySales.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="MonthlySalesDescription" runat="server" Text="Provides sales totals by month."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="TaxReportLink" runat="server" Text="Taxes" NavigateUrl="Taxes.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="TaxesDescription" runat="server" Text="Provides a tax report for a given period."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="ProductCaption" runat="server" Text="Product Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="SalesByProductLink" runat="server" Text="Sales by Product" NavigateUrl="TopProducts.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesByProductDescription" runat="server" Text="Provides sales totals by product."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="PopularProductsLink" runat="server" Text="Product Popularity" NavigateUrl="PopularProducts.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="PopularProductsDescription" runat="server" Text="Ranks the products by popularity of views."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="PopularCategoriesLink" runat="server" Text="Category Popularity" NavigateUrl="PopularCategories.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="PopularCategoriesDescription" runat="server" Text="Ranks the categories by popularity of views."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="LowInventoryLink" runat="server" Text="Low Inventory" NavigateUrl="LowInventory.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="LowInventoryDescription" runat="server" Text="Provides a listing of products at or below their inventory warning level."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
	    </td>
        <td valign="top" width="50%">
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="Localize1" runat="server" Text="Customer Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="SalesByCustomerLink" runat="server" Text="Sales by Customer" NavigateUrl="TopCustomers.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesByCustomerCustomerDescription" runat="server" Text="Provides sales totals by customer."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="AbandonedBasketsLink" runat="server" Text="Abandoned Baskets" NavigateUrl="MonthlyAbandonedBaskets.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="AbandonedBasketsDescription" runat="server" Text="Provides a summary of abandoned shopping sessions for your store."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="PopularBrowsersLink" runat="server" Text="Browser Popularity" NavigateUrl="PopularBrowsers.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="PopularBrowsersDescription" runat="server" Text="Ranks the popularity of browsers used by your customers."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="MarketingReportsCaption" runat="server" Text="Marketing Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="AffiliateSalesLink" runat="server" Text="Sales by Affiliate" NavigateUrl="SalesByAffiliate.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="AffiliateSalesDescription" runat="server" Text="Provides sales totals by affiliate."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="AffiliateSalesDetailLink" runat="server" Text="Sales By Affiliate Details" NavigateUrl="SalesByAffiliateDetail.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="AffiliateSalesDetailsDescriptioin" runat="server" Text="Provides sales details for affiliates."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="SalesByReferrerLink" runat="server" Text="Sales By Referrer" NavigateUrl="SalesByReferrer.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesByReferrerDescription" runat="server" Text="Provides sales details for referrer."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="CouponUsageLink" runat="server" Text="Coupon Usage" NavigateUrl="CouponUsage.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="CouponUsageDescription" runat="server" Text="Provides usage statistics and sales totals for your coupons."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="OtherReprotsCaption" runat="server" Text="Misc Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="WhoIsOnlineLink" runat="server" Text="Who Is Online?" NavigateUrl="WhoIsOnline.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="WhoIsOnlineDescription" runat="server" Text="Lets the merchant see who is currently browsing the store. Anonymous users not include in the report."></asp:Label>
                        </li>                        
                        <li runat="server" id="liAuditLog">
	                        <asp:HyperLink ID="AuditLogLink" runat="server" Text="Audit Log" NavigateUrl="~/Admin/Store/Security/AuditLog.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="AuditLogLabel" runat="server" Text="It provides the merchant ability to view entire user activity across the application."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
	    </td>
	</tr>
</table>    
</asp:Content>
