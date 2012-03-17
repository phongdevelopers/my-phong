<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="EditManufacturer.aspx.cs" Inherits="Admin_People_Manufacturers_EditManufacturer" Title="Edit Manufacturer"  %>

<%@ Register Src="EditProducts.ascx" TagName="EditProducts" TagPrefix="uc1" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>



<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Edit {0}" EnableViewState="false"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout" width="100%">
        <tr>
            <td valign="top" >
                <ajax:UpdatePanel ID="EditAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="inputForm" width="100%">
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Name" ToolTip="Name of manufacturer"></asp:Label><br />
                                </th>
                                <td>
                                    <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                                            Display="Static" ErrorMessage="Manufacturer name is required.">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="validation" colspan="2">
                                    <asp:Label ID="SavedMessage" runat="server" Text="Saved at {0:t}" Visible="false" SkinID="GoodCondition" EnableViewState="False"></asp:Label>
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="submit" colspan="2">                                    
                                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
									<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td valign="top" style="width:100%;">
                <uc1:EditProducts ID="EditProducts1" runat="server" />                
            </td>
        </tr>
    </table>
</asp:Content>