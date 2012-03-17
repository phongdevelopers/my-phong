<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="ViewGiftCertificate.aspx.cs" Inherits="Admin_Orders_ViewGiftCertificate" Title="View/Print Gift Certificate" %>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader noPrint">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="View/Print Gift Certificate - {0}"></asp:Localize></h1>
        </div>
        <div class="content">            
            <p><asp:Localize ID="PrintInstructions" runat="server" Text="This document includes a printable stylesheet.  If you are using a modern browser (such as IE7, FF2, or Opera 7) this page will print with appropriate styles and page breaks if needed.  Website headers, footers, and this message will not be printed."></asp:Localize></p>
        </div>
    </div>
    <div class="noPrint">
        <asp:Button ID="Print" runat="server" Text="Print" OnClientClick="window.print();return false;" />
        <asp:Button ID="Back" runat="server" Text="Back" OnClientClick="window.history.go(-1);return false;" />
    </div>
    
    <table align="center" class="form" cellpadding="0" cellspacing="0" style="border:solid 1px">
            <tr>
                <th colspan="2" style="border-bottom:solid 1px">
                    <asp:Localize ID="GiftCertificateSummayCaption" runat="server" Text="GIFT CERTIFICATE"></asp:Localize>
                </th>
            </tr>
            <tr>
                <th style="width:33%">Name:</th>
                <td><asp:Label runat="server" ID="Name" ></asp:Label></td>
            </tr>
            <tr>
                <th class="rowHeader">Status Description:</th>
                <td><asp:Label runat="server" ID="Description" ></asp:Label></td>
            </tr>
            <tr>
                <th class="rowHeader">Certificate Number:</th>
                <td><asp:Label runat="server" ID="Serial" ></asp:Label></td>
            </tr>
             <tr>
                <th class="rowHeader">Balance:</th>
                <td><asp:Label runat="server" ID="Balance" ></asp:Label></td>
            </tr>
            <tr>
                <th class="rowHeader">Expiration Date:</th>
                <td><asp:Label runat="server" ID="Expires" ></asp:Label></td>
            </tr>
    </table>    
</asp:Content>
