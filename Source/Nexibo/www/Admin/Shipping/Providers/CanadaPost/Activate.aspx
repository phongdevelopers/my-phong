<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Activate.aspx.cs" Inherits="Admin_Shipping_Providers_CanadaPost_Activate" Title="CanadaPost Activation"
     %>

<%@ Register Assembly="CommerceBuilder.CanadaPost" Namespace="CommerceBuilder.Shipping.Providers.CanadaPost"
    TagPrefix="CanadaPostGW" %>



<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td  class="pageHeader">              
                <h1>
                    <asp:Label ID="Caption" runat="server" Text="CanadaPost&reg; Activation"></asp:Label><br />                    
                </h1>
            </td>
        </tr>
             
        <tr>
            <td >
                <table class="inputForm"  >
                    <tr>
                        <td colspan="2">
                            <img src="logo_sen.gif" align="Left" hspace="5" vspace="5" />
                            <p align="justify" style="margin-bottom: 10px;">
                                <asp:Label ID="InstructionText" runat="server" Text="In order to enable CanadaPost, you must activate your CanadaPost account."></asp:Label>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p style="margin-bottom: 10px;">
                                <asp:Label ID="Label1" runat="server" Text="To activate the CanadaPost&reg; integration, simply enter your Merchant CPC Id in the form below and click Finish."></asp:Label>
                            </p>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                            <asp:Label ID="CustomErrorMessage" runat="server" Text="" SkinID="ErrorCondition"></asp:Label>
                        </td>
                    </tr>                    
                   
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="CanadaPostMerchantCPCIDLabel" runat="server" Text="CanadaPost Merchant CPC ID:"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="CanadaPostMerchantCPCID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="UserIdActiveLabel" runat="server" Text="Activated:"></asp:Label>        
                    </th>
                    <td>
                        <asp:CheckBox ID="UserIdActive" runat="Server" />
                    </td>
                </tr>
                
                    
                    <tr>
                        <td colspan="2" align="center">                            
                            <asp:Button ID="NextButton" runat="server" Text="Next" OnClick="NextButton_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <br />
                            <asp:Label ID="CanadaPostCopyRight" runat="server" Text="CanadaPost and CanadaPost brandmark are trademarks of CanadaPost Inc. All Rights Reserved."
                                SkinID="Copyright"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
