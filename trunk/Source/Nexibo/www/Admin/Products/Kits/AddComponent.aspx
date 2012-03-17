<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" AutoEventWireup="true" CodeFile="AddComponent.aspx.cs" Inherits="Admin_Products_Kits_AddComponent" Title="Add Component" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Create Component"></asp:Localize></h1>
        </div>
    </div>
    <asp:Label ID="InstructionText" runat="server" Text="Define a new component to this kitted product using the form below.  If you wish to attach an existing component to this kit, "></asp:Label><asp:HyperLink
        ID="AttachComponent" runat="server" NavigateUrl="AttachComponent.aspx" Text="click here."></asp:HyperLink><br />
    <br />
    
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />    
    <ajax:UpdatePanel ID="ComponentPanel" runat="server">
        <ContentTemplate>
            <table class="inputForm">
                <tr>
                    <th class="rowHeader">
                        <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="Name of the component." AssociatedControlID="Name"></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:TextBox ID="Name" runat="server" Text="New Component" width="150px" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                            ErrorMessage="Component Name Required." ToolTip="Component Name Required." Display="Static"
                            Text="*">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <cb:ToolTipLabel ID="InputTypeLabel" runat="server" Text="Input Type:" ToolTip="Determines the type of input control that will be used for the products in this component." AssociatedControlID="InputTypeId"></cb:ToolTipLabel>
                    </th>
                    <td>
                        <asp:DropDownList ID="InputTypeId" runat="server" AutoPostBack="true" OnSelectedIndexChanged="InputTypeChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="trHeaderOption" runat="server">
                    <th class="rowHeader" valign="top">
                        <cb:ToolTipLabel ID="HeaderOptionLabel" runat="server" Text="Header Option:" ToolTip="If you wish to have an option that indicates 'no selection', enter the text of the option here." AssociatedControlID="HeaderOption"></cb:ToolTipLabel>
                    </th>
                    <td valign="top">
                        <asp:TextBox ID="HeaderOption" runat="server" Width="200px" MaxLength="100"></asp:TextBox><br />
                        
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                        <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false" OnClick="CancelButton_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>