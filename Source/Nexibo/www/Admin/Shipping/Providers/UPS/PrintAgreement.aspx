<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="PrintAgreement.aspx.cs" Inherits="Admin_Shipping_UPS_PrintAgreement" Title="Print Agreement"  EnableViewState="False" %>
<%@ Import Namespace="CommerceBuilder.Shipping.Providers.UPS" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="noPrint">
    <asp:Button ID="PrintButton" runat="server" Text="Print" OnClientClick="window.print();return false;" />
    <asp:Button ID="CloseButton" runat="server" Text="Close" OnClientClick="window.close();return false;" />
    <br />
</div>
<pre style="word-wrap:break-word" wrap="hard">
    <asp:Literal ID="LicenseAgreement" runat="server"></asp:Literal>
</pre>
</asp:Content>