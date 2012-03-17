<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Shopping.com.aspx.cs" Inherits="Admin_Marketing_Feeds_Shopping_com" Title="Shopping.com Feed" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Namespace="CommerceBuilder.DataFeeds" TagPrefix="cbfeeds" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    
<div class="pageHeader">
	<div class="caption">
		<h1>Shopping.com Feed</h1>
	</div>
</div>
<ajax:UpdatePanel ID="UpdatePanel1" runat="server" >
    <ContentTemplate>
        <asp:Panel ID="FeedCreationProgressPanel" runat="server" Width="100%" Visible="False" >            
            <asp:Literal ID="WaitMessage" runat="server" Text="Please wait, this may take a few minutes. 0peration in progress..."></asp:Literal>
            <cb:ProgressBar ID="ProgressBar1" Width="500px" Value="0" runat="server"></cb:ProgressBar>
            <ajax:Timer ID="Timer1" runat="server" Enabled="False" Interval="1000" OnTick="Timer1_Tick">
            </ajax:Timer>
        </asp:Panel>
        <p>
        <asp:Panel ID="MessagePanel" runat="server" CssClass="contentPanel">
            <div class="contentPanelBody">
                <asp:Label ID="SuccessMessageHeader" runat="server" Text="SUCCESS"></asp:Label>
                <asp:Label ID="FailureMessageHeader" runat="server" Text="FAILED"></asp:Label>
                <asp:BulletedList ID="Messages" runat="server"></asp:BulletedList><br />
            </div>
            <asp:Button ID="ContinueButton" runat="server" Text="Click to Continue" OnClick="ContinueButton_Click" />
        </asp:Panel>
        </p>
   
        <br/>		  
        <table id="FeedInputPanel" runat="server" cellpadding="2" cellspacing="0" class="innerLayout">
	        <tr>
		        <td colspan="2" align="center">
		            <p class="InstructionText">Specify Shopping.com Feed Configuration Options Below.</p>		
		        </td>
	        </tr>
	        <tr>
	        <td>

                <table class="inputForm" >
                  <tr> 
                    <th class="rowHeader"  width="50%" > 
		                <asp:Label ID="FeedFileNameLabel" runat="server" Text="Feed File Name:"></asp:Label><br /> 
		                <span class="helpText">Name of the feed file</span>		
                    </th>
                    <td valign="Top" width="50%"> <asp:TextBox ID="FeedFileName" runat="server" Text="" Width="200px"></asp:TextBox> 
                    </td>
                  </tr>
                  <tr> 
                    <th class="rowHeader"  width="50%"> 
		                <asp:Label ID="AllowOverwriteLabel" runat="server" Text="Overwrite Existing Feed File"></asp:Label>
		                <br /> <span class="helpText">Should the existing feed file be overwritten when creating new feed?</span>
                    </th>
                    <td valign="Top" width="50%"> <asp:RadioButtonList ID="AllowOverwrite" runat="server" RepeatDirection="Vertical"> 
                        <asp:ListItem Text="No" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                      </asp:RadioButtonList> </td>
                  </tr>
                  <tr> 
                    <th class="rowHeader"  width="50%" > 
		                <asp:Label ID="Label1" runat="server" Text="Product Inclusion"></asp:Label>
		                <br /> <span class="helpText">Which Products to Include in the feed?</span>
                    </th>
                    <td valign="Top" width="50%"> 
	                  <asp:RadioButton ID="MarkedProducts" runat="Server" GroupName="ProductInclusion" /> 
                      <asp:Label ID="Label4" runat="server" Text="Do not include products marked for feed exclusion"></asp:Label>
                      <br />
	                  <asp:RadioButton ID="AllProducts" runat="Server" GroupName="ProductInclusion" /> 
                      <asp:Label ID="Label3" runat="server" Text="All Products (Ignore product feed setting)"></asp:Label>
                      <br /> 
	                 </td>
                  </tr>
                  <tr> 
                    <th class="rowHeader"  width="50%"> 
		                <asp:Label ID="Label9" runat="server" Text="Compressed Feed File Name"></asp:Label>
		                <br /> <span class="helpText">Name of the compressed feed file</span>
                    </th>
                    <td valign="Top" width="50%"> <asp:TextBox ID="CompressedFeedFileName" runat="server" Text="" Width="200px"></asp:TextBox> 
                    </td>
                  </tr>
                  <tr> 
                    <th class="rowHeader"  width="50%"> 
		                <asp:Label ID="AllowOverwriteCompressedLabel" runat="server" Text="Overwrite Existing Compressed Feed"></asp:Label>
		                <br /> <span class="helpText">Should the existing compressed feed file be overwritten when creating new compressed feed?</span>
                    </th>
                    <td valign="Top" width="50%"> <asp:RadioButtonList ID="AllowOverwriteCompressed" runat="server" RepeatDirection="Vertical"> 
                        <asp:ListItem Text="No" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                      </asp:RadioButtonList> </td>
                  </tr>
                  <tr> 
                    <th class="rowHeader"  width="50%"> 
		                <asp:Label ID="FtpHostLabel" runat="server" Text="FTP Host"></asp:Label>
		                <br /> <span class="helpText">FTP Host Name or IP address</span>
                    </th>
                    <td valign="Top" width="50%"> <asp:TextBox ID="FtpHost" runat="server" Text=""></asp:TextBox> 
                    </td>
                  </tr>
                  <tr> 
                    <th class="rowHeader"  width="50%"> 
		                <asp:Label ID="Label20" runat="server" Text="FTP User Name"></asp:Label>
                      <br /> <span class="helpText">FTP User Name</span>
                    </th>
                    <td valign="Top" width="50%"> <asp:TextBox ID="FtpUser" runat="server" Text=""></asp:TextBox> 
                    </td>
                  </tr>
                  <tr> 
                    <th class="rowHeader"  width="50%"> 
		                <asp:Label ID="Label22" runat="server" Text="FTP Password"></asp:Label>
                      <br /> <span class="helpText">FTP password</span>
                    </th>
                    <td valign="Top" width="50%"> <asp:TextBox ID="FtpPassword" runat="server" Text=""></asp:TextBox> 
                    </td>
                  </tr>
                  <tr> 
                    <th class="rowHeader"  width="50%"> 
		                <asp:Label ID="Label5" runat="server" Text="Remote File Name"></asp:Label>
		                <br /> <span class="helpText">Name of the file to set on remote server</span>
                    </th>
                    <td valign="Top" width="50%"> <asp:TextBox ID="RemoteFileName" runat="server" Width="200px" Text=""></asp:TextBox> 
                    </td>
                  </tr>  
                </table>
        	
		        <div align="center" style="margin-top:20px">
		          <asp:Label ID="FeedActionLabel" runat="server" Text="Tasks:" SkinID="FieldHeader"></asp:Label>
	              <asp:DropDownList ID="FeedAction" runat="server">
                  </asp:DropDownList>
                  <asp:ImageButton ID="FeedActionButton" runat="server" SkinID="GoIcon" OnClick="FeedActionButton_Click" />          
		          <br/>
		          <br/>  		  
		          <asp:Button ID="BtnSaveSettings" runat="server" Text="Save Settings" OnClick="BtnSaveSettings_Click" />
		          <asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" Text="Cancel" />
                </div>
	        </td>
	        </tr>	
        </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>


