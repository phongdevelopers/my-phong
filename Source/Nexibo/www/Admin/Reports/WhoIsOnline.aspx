<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="WhoIsOnline.aspx.cs" Inherits="Admin_Reports_WhoIsOnline" Title="Who Is Online?" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
	    <div class="caption">
		    <h1><asp:Localize ID="Localize1" runat="server" Text="Who Is Online?"></asp:Localize><asp:Localize ID="Localize2" runat="server" Text=" {0:d} to {1:d}" Visible="false" EnableViewState="false"></asp:Localize></h1>
	    </div>
    </div>
    <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">            
        <tr class="noPrint">
            <td>
				<asp:Label ID="ActivityThresholdLbl" runat="server" EnableViewState="false">Activity Threshold </asp:Label>
				<asp:TextBox ID="ActivityThreshold" runat="server" Text="30" Width="30px"></asp:TextBox>
				<asp:Label ID="LblMinutes" runat="server" EnableViewState="false">minutes</asp:Label>
				<asp:Button ID="ApplyButton" runat="server" OnClick="ApplyButton_Click" Text="Apply" />
            </td>
        </tr>
        <tr>
            <td class="dataSheet">                        
                <cb:SortedGridView ID="OnlineUserGrid" runat="server" AutoGenerateColumns="False" DataSourceID="OnlineUserDs"
                    DefaultSortExpression="LastActivityDate" DefaultSortDirection="Descending" AllowPaging="True" AllowSorting="true"
                    PageSize="10" OnSorting="OnlineUserGrid_Sorting" Width="100%" SkinID="Summary">
                    <Columns>
                        <asp:TemplateField HeaderText="User" SortExpression="UserName">
                            <ItemTemplate>
                                <asp:HyperLink ID="UserLink" runat="server" Text='<%# ((User)Container.DataItem).IsAnonymous?"anonymous":Eval("UserName") %>' NavigateUrl='<%#Eval("UserId", "../People/Users/EditUser.aspx?UserId={0}")%>'></asp:HyperLink>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email" SortExpression="Email">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="EmailLabel" runat="server" Text='<%# Eval("Email") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Activity" SortExpression="LastActivityDate">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="LastActivityDateLable" runat="server" Text='<%# Eval("LastActivityDate") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="IP Address" >
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="IpAddressLabel" runat="server" Text='<%# GetIpAddress(Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="emptyResult">
                            <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                        </div>    
                    </EmptyDataTemplate>
                </cb:SortedGridView>
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="OnlineUserDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetActiveUsers" 
        TypeName="CommerceBuilder.Reporting.ReportDataSource" SortParameterName="sortExpression" EnablePaging="true" 
        SelectCountMethod="GetActiveUsersCount">
        <SelectParameters>
            <asp:ControlParameter ControlID="ActivityThreshold" Name="activityThreshold" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

