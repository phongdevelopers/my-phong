<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="TestUrl.aspx.cs" Inherits="Admin_SEO_TestUrl" Title="Test Url" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Test Url"></asp:Localize></h1>
        </div>
    </div>
    <div class="bodyText">
        Enter a URL below to see how it is impacted by redirection and rewrites and to find the actual target script.<br /><br />
    </div>
    <ajax:UpdatePanel ID="RedirectGridAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            
            <asp:ValidationSummary ID="AddValidationSummary" runat="server" ValidationGroup="TestUrl" />
            <table class="inputForm">
                <tr>
                    <th class="rowHeader" nowrap>
                        <cb:ToolTipLabel ID="RequestUrlLabel" runat="server" Text="Request Url:" AssociatedControlID="RequestUrl" ToolTip="The URL to check for redirection and rewrites."></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:TextBox ID="RequestUrl" runat="server" MaxLength="250" Width="250px" AutoComplete="off"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequestUrlRequired" runat="server" ControlToValidate="RequestUrl" ErrorMessage="Request URL is required." Text="*" ValidationGroup="TestUrl" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <asp:Button ID="TestButton" runat="server" Text="Submit" ValidationGroup="TestUrl" OnClick="TestButton_Click" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="ResultsPanel" runat="server" Visible="false" EnableViewState="false">
                <div class="pageHeader">
                     <div class="caption">
                        <h1><asp:Localize ID="ResultsCaption" runat="server" Text="You are looking for '{0}':" EnableViewState="false"></asp:Localize></h1>
                    </div>
                </div>  
                <div class="bodyText">
                    <asp:Label ID="ResultsLabel" runat="Server" Text="{0}<br/>{1}" Visible="false" EnableViewState="false"></asp:Label>
                    <asp:Label ID="NoResultsLabel" runat="Server" Text="<br/>No redirects or rewrites rules are found for the specified URL.<br/>" Visible="false" EnableViewState="false"></asp:Label>
                    <asp:Label ID="CircularRedirectsLabel" runat="Server" Text="<br/>Limit Exceeded, Circular Redirection?<br/>" SkinID="WarnCondition" Visible="false" EnableViewState="false"></asp:Label>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

