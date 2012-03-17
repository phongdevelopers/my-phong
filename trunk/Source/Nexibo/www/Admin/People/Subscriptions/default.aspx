<%@ Page Language="C#" MasterPageFile="../../Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_People_Subscriptions_Default" Title="Manage Subscriptions" EnableViewState="false" %>
<%@ Register Src="../../UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar2" TagPrefix="uc1" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
<script language="javascript" type="text/javascript">
    function toggleState(id, checkState)
    {
        var cb = $get(id);
        if (cb != null) cb.checked = checkState;
    }

    function toggleSelected(checkState)
    {
        // Toggles through all of the checkboxes defined in the CheckBoxIDs array
        // and updates their value to the checkState input parameter
        if (CheckBoxIDs != null)
        {
            for (var i = 0; i < CheckBoxIDs.length; i++)
                toggleState(CheckBoxIDs[i], checkState.checked);
        }
    }
    
    function hasSelected()
    {
        if (CheckBoxIDs != null)
        {
            for (var i = 0; i < CheckBoxIDs.length; i++)
            {
                var cb = $get(CheckBoxIDs[i]);
                if (cb != null && cb.checked) return true;
            }
        }
        return false;
    }
</script>
<ajax:UpdatePanel ID="SubscriptionsAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="pageHeader">
    	    <div class="caption">
    		    <h1><asp:Localize ID="Caption" runat="server" Text="Manage Subscriptions"></asp:Localize></h1>
                <div class="links">
                    <asp:HyperLink ID="SummaryLink" runat="server" Text="Plan Summary" NavigateUrl="SubscriptionPlans.aspx" EnableViewState="false"></asp:HyperLink>
                </div>
    	    </div>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        <table class="inputForm" cellpadding="3">
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="SubscriptionPlanLabel" runat="server" Text="Plan:" AssociatedControlID="SubscriptionPlan" 
                        ToolTip="Filter the subscription list by plan name." />
                </th>
                <td>
                    <asp:DropDownList ID="SubscriptionPlan" runat="server"
                        DataTextField="Name" DataValueField="ProductId" AppendDataBoundItems="true">
                        <asp:ListItem Text="- any -" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ExpirationStartLabel" runat="server" Text="Expiration:" AssociatedControlID="ExpirationStart"
                        ToolTip="Find subscriptions that will expire within a given date range." />
                </th>
                <td colspan="3">
                    <uc1:PickerAndCalendar2 ID="ExpirationStart" runat="server" /> 
                    &nbsp;<asp:Label ID="ExpirationEndLabel" runat="server" Text="to" SkinID="fieldHeader"></asp:Label>&nbsp; 
                    <uc1:PickerAndCalendar2 ID="ExpirationEnd" runat="server" />
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="EmailLabel" runat="server" Text="Email:" AssociatedControlID="Email"
                        ToolTip="You can enter email to filter the list." />
                </th>
                <td>
                    <asp:TextBox ID="Email" runat="server" Text="" Width="220px"></asp:TextBox>
                </td>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="LastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="LastName"
                        ToolTip="Filter the list by part of a customer last name." />
                </th>
                <td>
                    <asp:TextBox ID="LastName" runat="server" Text=""></asp:TextBox>
                </td>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="FirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="FirstName"
                        ToolTip="Filter the list by part of a customer first name." />
                </th>
                <td>
                    <asp:TextBox ID="FirstName" runat="server" Text=""></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="OrderNumberLabel" runat="server" Text="Order Number(s):" AssociatedControlID="OrderNumber"
                        ToolTip="You can enter order number(s) to filter the list.  Separate multiple orders with a comma.  You can also enter ranges like 4-10 for all orders numbered 4 through 10." />
                </th>
                <td>
                    <asp:TextBox ID="OrderNumber" runat="server" Text=""></asp:TextBox>
                    <cb:IdRangeValidator ID="OrderNumberValidator" runat="server" Required="false"
                        ControlToValidate="OrderNumber" Text="*" 
                        ErrorMessage="The range is invalid.  Enter a specific order number or a range of numbers like 4-10.  You can also include mulitple values separated by a comma."></cb:IdRangeValidator>                
                </td>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ActiveOnlyLabel" runat="server" AssociatedControlID="ActiveOnly"
                        Text="Status:" ToolTip="Filter subscriptions by active/inactive status." />
                </th>
                <td>
                    <asp:DropDownList ID="ActiveOnly" runat="server">
                        <asp:ListItem Text="Active Only" Value="True"></asp:ListItem>
                        <asp:ListItem Text="Inactive Only" Value="False"></asp:ListItem>
                        <asp:ListItem Text="All" Value="Any"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                
            </tr>
            <tr>
                <td></td>
                <td align="left"><asp:Button ID="SearchButton" runat="server" Text="Search" /></td>
            </tr>
        </table>
        <cb:SortedGridView ID="SubscriptionGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="SubscriptionId,UserId" DataSourceID="SubscriptionDs" 
            SkinID="PagedList" AllowSorting="true" ShowWhenEmpty="False" Width="100%" AllowPaging="true" PageSize="20" EnableViewState="False" 
            DefaultSortExpression="S.ExpirationDate" DefaultSortDirection="Ascending" OnDataBound="SubscriptionGrid_DataBound">
            <Columns>
                <asp:TemplateField HeaderText="Select">
                    <HeaderTemplate>
                        <input type="checkbox" onclick="toggleSelected(this)" />
                    </HeaderTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="40px" />
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:CheckBox ID="Selected" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Plan" SortExpression="SP.Name">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <a href="../../Products/EditSubscription.aspx?ProductId=<%#Eval("ProductId")%>"><%#Eval("SubscriptionPlan.Name")%></a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Order" SortExpression="O.OrderNumber">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:HyperLink ID="orderLink" runat="server" NavigateUrl='<%#String.Format("../../Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderItem.Order.OrderNumber"), Eval("OrderItem.OrderId"))%>' Text='<%# Eval("OrderItem.Order.OrderNumber") %>' SkinID="Link"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" SortExpression="A.LastName">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <%# Eval("User.PrimaryAddress.FullName") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email" SortExpression="U.LoweredEmail">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:HyperLink ID="UserLink" runat="server" NavigateUrl='<%# Eval("UserId", "../Users/EditUser.aspx?UserId={0}") %>' Text='<%# Eval("User.Email") %>' SkinID="Link"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Expiration" SortExpression="S.ExpirationDate">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Expiration" runat="server" text='<%#Eval("ExpirationDate", "{0:d}")%>' visible='<%# ((DateTime)Eval("ExpirationDate") != DateTime.MinValue) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Group">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <%#GetGroupName(Container.DataItem)%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Active">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%#GetIsActiveCheckbox(Container.DataItem)%>
                    </ItemTemplate>
                </asp:TemplateField>            
            </Columns>
            <EmptyDataTemplate> 
                <asp:Label ID="EmptyMessage" runat="server" Text="There are no active subscriptions for this plan."></asp:Label>
            </EmptyDataTemplate>
        </cb:SortedGridView>
        <br />
        
        <table class="inputForm" cellpadding="3" cellspacing="0" ID="TasksPanel" runat="server">
            <tr>
                <th class="rowHeader">
                    <asp:Localize ID="TasksLabel" runat="server" Text="Tasks:"></asp:Localize>
                </th>
                <td>
                    <asp:Button ID="EmailButton" runat="server" Text="Email" />
                    <asp:Button ID="ExportButton" runat="server" Text="Export" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClientClick="if(hasSelected()){return confirm('Are you sure you want to cancel selected subscriptions?');}else{alert('You must select at least one subscription to cancel.');}" OnClick="CancelButton_Click" />
                </td>
            </tr>
        </table>
        <asp:Panel ID="EmailDialog" runat="server" Style="display:none;width:300px" CssClass="modalPopup">
            <asp:Panel ID="EmailDialogHeader" runat="server" CssClass="modalPopupHeader" EnableViewState="false">
                <asp:Localize ID="EmailDialogCaption" runat="server" Text="Send Email" EnableViewState="false"></asp:Localize>
            </asp:Panel>
            <div style="padding-top:5px;">
                <table class="inputForm" cellspacing="0" cellpadding="3">
                    <tr>
                        <th valign="top">
                            <asp:Localize ID="EmailSelectedLabel" runat="server" Text="Send To:"></asp:Localize>
                        </th>
                        <td>
                            <asp:DropDownList ID="EmailSelected" runat="server">
                                <asp:ListItem Text="Selected" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="All"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th valign="top">
                            <asp:Localize ID="EmailDistinctLabel" runat="server" Text="Filter:"></asp:Localize>
                        </th>
                        <td>
                            <asp:DropDownList ID="EmailDistinct" runat="server">
                                <asp:ListItem Text="One email per subscription" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="One email per user"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="EmailOkButton" runat="server" Text="Next" OnClick="EmailOkButton_Click" CausesValidation="false" />
                            <asp:Button ID="EmailCancelButton" runat="server" Text="Cancel" CausesValidation="false" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <ajax:ModalPopupExtender ID="EmailPopup" runat="server" 
            TargetControlID="EmaiLButton"
            PopupControlID="EmailDialog" 
            BackgroundCssClass="modalBackground"                         
            CancelControlID="EmailCancelButton" 
            DropShadow="false"
            PopupDragHandleControlID="EmailDialogHeader" />
        <asp:Panel ID="ExportDialog" runat="server" Style="display:none;width:300px" CssClass="modalPopup">
            <asp:Panel ID="ExportDialogHeader" runat="server" CssClass="modalPopupHeader" EnableViewState="false">
                <asp:Localize ID="ExportDialogCaption" runat="server" Text="Export Subscriptions/Users" EnableViewState="false"></asp:Localize>
            </asp:Panel>
            <div style="padding-top:5px;">
                <table class="inputForm" cellspacing="0" cellpadding="3">
                    <tr>
                        <th valign="top">
                            <asp:Localize ID="ExportSelectedLabel" runat="server" Text="Export:"></asp:Localize>
                        </th>
                        <td>
                            <asp:DropDownList ID="ExportSelected" runat="server">
                                <asp:ListItem Text="Selected" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="All"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th valign="top">
                            <asp:Localize ID="ExportDistinctLabel" runat="server" Text="Filter:"></asp:Localize>
                        </th>
                        <td>
                            <asp:DropDownList ID="ExportDistinct" runat="server">
                                <asp:ListItem Text="Export subscriptions" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Export unique users"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="ExportOkButton" runat="server" Text="Next" OnClick="ExportOkButton_Click" CausesValidation="false" />
                            <asp:Button ID="ExportCancelButton" runat="server" Text="Cancel" CausesValidation="false" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <ajax:ModalPopupExtender ID="ExportPopup" runat="server" 
            TargetControlID="ExportButton"
            PopupControlID="ExportDialog" 
            BackgroundCssClass="modalBackground"                         
            CancelControlID="ExportCancelButton" 
            DropShadow="false"
            PopupDragHandleControlID="ExportDialogHeader" />
        
        <asp:ObjectDataSource ID="SubscriptionDs" runat="server" SelectMethod="Search" SelectCountMethod="SearchCount"
            TypeName="CommerceBuilder.Orders.SubscriptionDataSource" DataObjectTypeName="CommerceBuilder.Orders.Subscription"
            SortParameterName="sortExpression" EnablePaging="true">
            <SelectParameters>
                <asp:ControlParameter Name="subscriptionPlanId" ControlID="SubscriptionPlan" PropertyName="SelectedValue" />
                <asp:ControlParameter Name="orderRange" ControlID="OrderNumber" PropertyName="Text" />
                <asp:Parameter Name="userIdRange" DefaultValue="" />
                <asp:ControlParameter Name="firstName" ControlID="FirstName" PropertyName="Text" />
                <asp:ControlParameter Name="lastName" ControlID="LastName" PropertyName="Text" />
                <asp:ControlParameter Name="email" ControlID="Email" PropertyName="Text" />
                <asp:ControlParameter Name="expirationStart" ControlID="ExpirationStart" PropertyName="SelectedStartDate" />
                <asp:ControlParameter Name="expirationEnd" ControlID="ExpirationEnd" PropertyName="SelectedEndDate" />
                <asp:ControlParameter Name="active" ControlId="ActiveOnly" PropertyName="SelectedValue" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>


