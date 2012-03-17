<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="ViewSubscriptions.aspx.cs" Inherits="Admin_Orders_ViewSubscriptions" Title="View Subscriptions" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar2" TagPrefix="uc1" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Subscriptions for Order #{0}"></asp:Localize></h1>
        </div>
    </div>
    <div style="margin:4px 0px;"
        <cb:SortedGridView ID="SubscriptionGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="SubscriptionId" DataSourceID="SubscriptionDs" 
            SkinID="PagedList" AllowSorting="False" ShowWhenEmpty="False" AllowPaging="false" EnableViewState="False" OnRowCommand="SubscriptionGrid_RowCommand" Width="100%" OnRowUpdating="SubscriptionGrid_RowUpdating" >
            <Columns>
                <asp:TemplateField HeaderText="Plan">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="SubscriptionPlan" runat="server" text='<%#Eval("SubscriptionPlan.Name")%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Group">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="Group" runat="server" text='<%#GetGroupName(Eval("GroupId"))%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Active">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:CheckBox ID="Active" runat="server" Checked='<%#Eval("IsActive")%>' Enabled="False" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Expiration">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Expiration" runat="server" text='<%#Eval("ExpirationDate", "{0:d}")%>' visible='<%# ((DateTime)Eval("ExpirationDate") != DateTime.MinValue) %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <uc1:PickerAndCalendar2 ID="EditExpiration" runat="server" SelectedDate='<%# Bind("ExpirationDate") %>' /> 
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>                    
                    <ItemStyle HorizontalAlign="right" Width="120px" />
                    <ItemTemplate>
                        <asp:LinkButton ID="ActivateLink" runat="server" visible='<%#(!(bool)Eval("IsActive"))%>' text="activate" SkinID="Button" CommandName="Activate" CommandArgument='<%#Eval("SubscriptionId")%>' />
                        <asp:LinkButton ID="EditButton" runat="server" CommandName="Edit"><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" /></asp:LinkButton>
                        <asp:LinkButton ID="CancelLink" runat="server"  SkinID="Link" CommandName="CancelSubscription" CommandArgument='<%#Eval("SubscriptionId")%>' OnClientClick='javascript:return confirm("Are you sure you want to cancel the subscription?")'><asp:Image ID="DeleteIcon" runat="server" SkinID="DeleteIcon" /></asp:LinkButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="EditSaveButton" runat="server" CommandName="Update" SkinID="SaveIcon" ToolTip="Save"></asp:ImageButton>
                        <asp:ImageButton ID="EditCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" SkinID="CancelIcon" ToolTip="Cancel Editing"></asp:ImageButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="EmptyMessage" runat="server" Text="There are no subscriptions associated with this order."></asp:Label> 
            </EmptyDataTemplate>
        </cb:SortedGridView>
    </div>
    <asp:ObjectDataSource ID="SubscriptionDs" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadForOrder" TypeName="CommerceBuilder.Orders.SubscriptionDataSource" DataObjectTypeName="CommerceBuilder.Orders.Subscription" >
        <SelectParameters>
            <asp:QueryStringParameter Name="OrderId" QueryStringField="OrderId" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>