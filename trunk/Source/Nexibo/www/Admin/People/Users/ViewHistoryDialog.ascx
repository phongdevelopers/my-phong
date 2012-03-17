<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewHistoryDialog.ascx.cs" Inherits="Admin_People_Users_ViewHistoryDialog" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<div class="section">
    <div class="header">
        <h2 class="currentlog"><asp:Localize ID="Localize1" runat="server" Text="Recent Page Views"></asp:Localize></h2>
    </div>
    <div class="content">        
          <asp:Repeater ID="ViewsList" runat="server">
            <HeaderTemplate>
                <table class="dataSheet" cellpadding="4" cellspacing="0" width="100%">
                    <tr>
                        <th align="left" width="150">
                            <asp:Label ID="DateHeader" runat="server" Text="Date"></asp:Label>
                        </th>
                        <th align="left">
                            <asp:Label ID="PageHeader" runat="server" Text="Page"></asp:Label>
                        </th>
                    </tr>                
            </HeaderTemplate>
            <ItemTemplate>                
                <tr>
                    <td>
                        <%#Eval("ActivityDate")%>
                    </td>
                    <td>
                        <%#GetUri(Container.DataItem)%>
                    </td>
                </tr>
            </ItemTemplate> 
            <FooterTemplate>
                    <tr>
                        <td colspan="2">
                            <asp:HyperLink ID="CompleteHistoryLink" NavigateUrl="<%#GetReportLink()%>" runat="server" Text="See All &raquo;" CssClass="showAll"></asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </FooterTemplate>                         
        </asp:Repeater>
        <asp:Panel ID="NoRecordedPageViewsPanel" runat="server" Visible="false" CssClass="emptyData">
            <asp:Label ID="NoRecordedPageViewsMessage" runat="server" Text="No recorded page views."></asp:Label>    
        </asp:Panel>
    </div>
</div>