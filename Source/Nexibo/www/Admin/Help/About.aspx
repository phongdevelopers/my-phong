<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="Admin_Help_About" Title="About AbleCommerce" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="About AbleCommerce {0}"></asp:Localize></h1>
    	</div>
    </div>
    <div style="padding:4px">
        <br />
        <asp:TextBox ID="DllVersions" runat="server" Width="400px" Height="200px" TextMode="multiLine"></asp:TextBox><br />
        <p>For more information and help resources, see our website:</p>
        <p><a href="http://www.ablecommerce.com">http://www.ablecommerce.com</a></p>
        <p>&copy; 2009 Able Solutions Corporation</p>
        <p><i>This product includes software developed by the Apache Software Foundation (http://www.apache.org/).  It also includes some third party components that are licensed under the terms of LGPL, among others.  Please review the App_Data/Licenses folder of your installation for complete details.</i></p>
    </div>
</asp:Content>