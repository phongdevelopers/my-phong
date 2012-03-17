<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Activate.aspx.cs" Inherits="Admin_Shipping_DHLInternational_Activate" Title="DHLInternational Activation"
     %>

<%@ Register Assembly="CommerceBuilder.DHLInternational" Namespace="CommerceBuilder.Shipping.Providers.DHLInternational"
    TagPrefix="DHLGW" %>



<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" class="pageHeader">
                <h1>
                
                    <asp:Label ID="Caption" runat="server" Text="DHLInternational&reg; Activation"></asp:Label>
                </h1>
            </td>
        </tr>
        <tr>
            <th colspan="2" class="sectionHeader">
                Instance Name:
                <asp:Label ID="InstanceNameLabel" runat="server" Text="${InstanceName}" SkinID="FieldHeader"></asp:Label>
            </th>
        </tr>
        <tr>
            <td colspan="2">
                <table class="inputForm" cellpadding="4" width="100%">
                    <tr>
                        <td colspan="2">
                            <img src="Logo.jpg" align="Left" hspace="5" vspace="5"/>
                            <p style="margin-bottom: 10px; text-align: left;">
                                <asp:Label ID="InstructionText" runat="server" Text="In order to enable DHL, you must activate your DHL account."></asp:Label>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p style="margin-bottom: 10px;">
                                <asp:Label ID="Label1" runat="server" Text="Please Provide DHL User ID and Password information below."></asp:Label>
                            </p>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                            <asp:Label ID="CustomErrorMessage" runat="server" Text="" SkinID="ErrorCondition"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" >
                            <asp:Label ID="DHLUserIDLbl" runat="server" Text="DHL User ID:" SkinID="FieldHeader"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="DHLUserID" runat="server"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="DHLUserIDValidator" runat="server" ErrorMessage="DHL User ID is required."
                                ControlToValidate="DHLUserID" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" align="right">
                            <asp:Label ID="DHLPasswordLbl" runat="server" Text="DHL Password:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="DHLPassword" runat="server"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="DHLPasswordValidator" runat="server" ErrorMessage="DHL Password is required."
                                ControlToValidate="DHLPassword" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center" style="width: 100%; text-align: center;">                            
                            <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
							<asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false"
                                OnClick="CancelButton_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="DHLCopyRight" runat="server" Text="DHL and DHL brandmark are trademarks of DHL Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
