<%@ Page Language="C#" MasterPageFile="../../Admin.master" CodeFile="SubscriptionPlans.aspx.cs" Inherits="Admin_People_Subscriptions_SubscriptionPlans" Title="Subscription Plans"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Subscription Plans"></asp:Localize></h1>
            <div class="links">
                <asp:HyperLink ID="DetailsLink" runat="server" Text="Subscription List" NavigateUrl="Default.aspx" EnableViewState="false"></asp:HyperLink>
            </div>
    	</div>
    </div>
    <ajax:UpdatePanel ID="SubscriptionAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cb:SortedGridView ID="SubscriptionPlanGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="ProductId" DataSourceID="SubscriptionPlanDs" 
                SkinID="PagedList" AllowSorting="True" DefaultSortExpression="SP.Name" DefaultSortDirection="Ascending" ShowWhenEmpty="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="Subscription Plan" SortExpression="SP.Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:HyperLink ID="SubscriptionPlanLink" runat="server" NavigateUrl='<%# Eval("ProductId", "../../Products/EditSubscription.aspx?ProductId={0}") %>' Text='<%# Eval("Name") %>' SkinID="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="ActiveLink" runat="server" NavigateUrl='<%# Eval("ProductId", "Default.aspx?PlanId={0}&ActiveState=1") %>' Text='<%# CountSubscriptions(Container.DataItem, BitFieldState.True) %>' SkinId="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Inactive">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="InactiveLink" runat="server" NavigateUrl='<%# Eval("ProductId", "Default.aspx?PlanId={0}&ActiveState=0") %>' Text='<%# CountSubscriptions(Container.DataItem, BitFieldState.False) %>' SkinId="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="TotalLink" runat="server" NavigateUrl='<%# Eval("ProductId", "Default.aspx?PlanId={0}&ActiveState=-1") %>' Text='<%# CountSubscriptions(Container.DataItem, BitFieldState.Any) %>' SkinId="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Group">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Eval("Group.Name")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Expire Today">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="ExpireToday" runat="server" NavigateUrl='<%# Eval("ProductId", "Default.aspx?PlanId={0}&Exp=0") %>' Text='<%# CountExpiringSubscriptions(Container.DataItem, 0) %>' SkinId="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Expire in 7">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="ExpireIn7Link" runat="server" NavigateUrl='<%# Eval("ProductId", "Default.aspx?PlanId={0}&Exp=7") %>' Text='<%# CountExpiringSubscriptions(Container.DataItem, 7) %>' SkinId="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Expire in 30">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:HyperLink ID="ExpireIn30Link" runat="server" NavigateUrl='<%# Eval("ProductId", "Default.aspx?PlanId={0}&Exp=30") %>' Text='<%# CountExpiringSubscriptions(Container.DataItem, 30) %>' SkinId="Link"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate> 
                    <asp:Label ID="EmptyMessage" runat="server" Text="There are no active subscription plans in your store."></asp:Label>
                </EmptyDataTemplate>
            </cb:SortedGridView>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="SubscriptionPlanDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForStore" TypeName="CommerceBuilder.Products.SubscriptionPlanDataSource" DataObjectTypeName="CommerceBuilder.Products.SubscriptionPlan"
        SortParameterName="sortExpression">
    </asp:ObjectDataSource>
</asp:Content>


