<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdminWebpartsPanel.ascx.cs" Inherits="Admin_Dashboard_AdminWebpartsPanel" %>
<%@ Import Namespace="CommerceBuilder.Personalization" %>
<%@ Register Assembly="CommerceBuilder" Namespace="CommerceBuilder.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<%@ Register Src="DigitalClock.ascx" TagName="DigitalClock" TagPrefix="uc" %>
<table class="WebPartEditor">
    <tr>
        <th class="editorHeader">
            <asp:Label ID="CurrentModeLabel" runat="server" Text="Customize Dashboard:" SkinID="FieldHeader"></asp:Label>
            <asp:DropDownList ID="CurrentMode" runat="server" AutoPostBack="true"
                OnSelectedIndexChanged="CurrentMode_SelectedIndexChanged">
                <asp:ListItem Text="View" Value="Browse"></asp:ListItem>
                <asp:ListItem Text="Edit Layout" Value="Catalog"></asp:ListItem>
                <asp:ListItem Text="Edit Content" Value="Edit"></asp:ListItem>
            </asp:DropDownList>
        </th>
    </tr>
    <tr id="trLayoutPanel" runat="server">
        <td>
            <table width="100%">
                <tr>
                    <td>
                        <asp:CatalogZone ID="CatalogZone1" runat="server" BackColor="#F7F6F3" BorderColor="#CCCCCC" BorderWidth="1px" Font-Names="Verdana" Padding="6" Width="100%">
                            <ZoneTemplate>
                                <asp:DeclarativeCatalogPart ID="DeclarativeCatalogPart1" runat="server" Title="Built-in Controls">
                                    <WebPartsTemplate>
                                        <uc:DigitalClock ID="DigitalClock1" runat="server" Title="Digital Clock" />
                                    </WebPartsTemplate>
                                </asp:DeclarativeCatalogPart>
                                <asp:ImportCatalogPart ID="ImportCatalogPart1" runat="server" Title="Your Controls" />
                                <asp:PageCatalogPart ID="PageCatalogPart1" runat="server" Title="Page Controls" />
                            </ZoneTemplate>
                            <HeaderVerbStyle Font-Bold="False" Font-Size="0.8em" Font-Underline="False" ForeColor="#333333" />
                            <PartTitleStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="0.8em" ForeColor="White" />
                            <PartChromeStyle BorderColor="#E2DED6" BorderStyle="Solid" BorderWidth="1px" />
                            <InstructionTextStyle Font-Size="0.8em" ForeColor="#333333" />
                            <PartLinkStyle Font-Size="0.8em" />
                            <EmptyZoneTextStyle Font-Size="0.8em" ForeColor="#333333" />
                            <LabelStyle Font-Size="0.8em" ForeColor="#333333" />
                            <VerbStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
                            <PartStyle BorderColor="#F7F6F3" BorderWidth="5px" />
                            <SelectedPartLinkStyle Font-Size="0.8em" />
                            <FooterStyle BackColor="#E2DED6" HorizontalAlign="Right" />
                            <HeaderStyle BackColor="#E2DED6" Font-Bold="True" Font-Size="0.8em" ForeColor="#333333" />
                            <EditUIStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
                        </asp:CatalogZone><br />
                    </td>
                    <td valign="top">
                        <asp:Label ID="Label1" runat="server">
                            You can reposition page elements by dragging them to the desired location.  To remove an element, access its action menu and choose close or delete.  To add an element, locate the desired element in the catalog below and add it to the desired zone.
                        </asp:Label>
                        <asp:Panel ID="ResetPagePanel" runat="server">
                            <br /><br />
                            <asp:Label ID="ResetPageHelpText" runat="server">
                                Your dashboard has been customized.  You can click "Reset Page" below to remove all customizations and reset the dashboard.
                            </asp:Label>
                            <br /><br />
                            <asp:Button ID="ResetPage" runat="server" Text="Reset Page" OnClick="ResetPage_Click" OnClientClick="return confirm('Are you sure you want to reset the page? This will remove any customizations made using this website editor.')" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr id="trEditPanel" runat="server">
        <td align="center">
            <asp:Label ID="EditPanelHelpText" runat="server">
                To edit an element, access its action menu and choose Edit.  The window below will display the editable properies for the element.
            </asp:Label>
            <asp:EditorZone ID="EditorZone1" runat="server" BackColor="#F7F6F3" BorderColor="#CCCCCC"
                BorderWidth="1px" Font-Names="Verdana" Padding="6" Width=100%>
                <HeaderStyle BackColor="#E2DED6" Font-Bold="True" Font-Size="0.8em" ForeColor="#333333" />
                <LabelStyle Font-Size="0.8em" ForeColor="#333333" />
                <HeaderVerbStyle Font-Bold="False" Font-Size="0.8em" Font-Underline="False" ForeColor="#333333" />
                <PartChromeStyle BorderColor="#E2DED6" BorderStyle="Solid" BorderWidth="1px" />
                <ZoneTemplate>
                    <asp:PropertyGridEditorPart ID="PropertyGridEditorPart1" runat="server" />
                </ZoneTemplate>
                <PartStyle BorderColor="#F7F6F3" BorderWidth="5px" />
                <FooterStyle BackColor="#E2DED6" HorizontalAlign="Right" />
                <EditUIStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
                <InstructionTextStyle Font-Size="0.8em" ForeColor="#333333" />
                <ErrorStyle Font-Size="0.8em" />
                <VerbStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
                <EmptyZoneTextStyle Font-Size="0.8em" ForeColor="#333333" />
                <PartTitleStyle Font-Bold="True" Font-Size="0.8em" ForeColor="#333333" />
            </asp:EditorZone>    
        </td>
    </tr>
</table>
