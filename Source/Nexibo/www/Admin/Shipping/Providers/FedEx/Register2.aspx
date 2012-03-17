<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Register2.aspx.cs" Inherits="Admin_Shipping_FedEx_Register2" Title="FedEx&reg; Registration"
     %>

<%@ Register Assembly="CommerceBuilder.FedEx" Namespace="CommerceBuilder.Shipping.Providers.FedEx"
    TagPrefix="FedExGW" %>



<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" class="pageHeader">
                <h1>
                    <asp:Label ID="Caption" runat="server" Text="FedEx&reg; Registration"></asp:Label>
                    <br />
                </h1>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table class="inputForm" cellpadding="4" width="100%">
                    <tr>
                        <td colspan="2">
                            <img src="LOGO_S.gif" align="Left" hspace="20" vspace="20" />
                            <p align="center" style="margin-bottom: 10px;">
                                <asp:Label ID="InstructionText" runat="server" Text="In order to enable FedEx&reg;, you must register your FedEx&reg; account and obtain a meter number."></asp:Label>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p style="margin-bottom: 10px;">
                                <asp:Label ID="Label1" runat="server" Text="Please Provide FedEx Account Number and Meter Number information below."></asp:Label>
                            </p>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                            <asp:Label ID="CustomErrorMessage" runat="server" Text="" SkinID="ErrorCondition"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trInstanceName" runat="server" visible="false">
                        <th class="rowHeader">
                             <asp:Label ID="Label2" runat="server" Text="Gateway Name:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="InstanceName" runat="server" Text="FedEx"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter a name for this instance of FedEx."
                                ControlToValidate="InstanceName" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="FedExAccountNumberLbl" runat="server" Text="FedEx&reg; Account Number:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="FedExAccountNumber" runat="server"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="FedExAccountNumberValidator" runat="server" ErrorMessage="FedEx Account Number is required."
                                ControlToValidate="FedExAccountNumber" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="FedExMeterNumberLbl" runat="server" Text="FedEx&reg; Meter Number:"></asp:Label>
                        </th>
                        <td>
                            <asp:TextBox ID="FedExMeterNumber" runat="server"></asp:TextBox>&nbsp;R
                            <asp:RequiredFieldValidator ID="FedExMeterNumberValidator" runat="server" ErrorMessage="FedEx Meter Number is required."
                                ControlToValidate="FedExMeterNumber" Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">                            
                            <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
							<asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="false"
                                OnClick="CancelButton_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="justify">
                <br />
                <asp:Label ID="FedExCopyRight" runat="server" Text="FedEx and FedEx brandmark are trademarks of FedEx Inc. All Rights Reserved."
                    SkinID="Copyright"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
