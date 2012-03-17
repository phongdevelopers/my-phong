<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DigitalClock.ascx.cs" Inherits="Admin_Dashboard_DigitalClock" EnableViewState="false" %>
<asp:Label ID="ServerTimeLabel" runat="server" Text="Store Time: " SkinID="FieldHeader"></asp:Label>
<asp:Label ID="ServerTime" runat="server" Text=""></asp:Label>&nbsp;&nbsp;&nbsp;
<asp:Label ID="ClientTimeLabel" runat="server" Text="Local Time: " SkinID="FieldHeader"></asp:Label>
<asp:Label ID="ClientTime" runat="server" Text=""></asp:Label>