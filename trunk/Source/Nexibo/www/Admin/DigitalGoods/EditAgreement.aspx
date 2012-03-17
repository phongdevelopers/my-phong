<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="EditAgreement.aspx.cs" Inherits="Admin_DigitalGoods_EditAgreement" Title="Edit LicenseAgreement" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Edit {0}" EnableViewState="false"></asp:Localize>
            </h1>
    	</div>
    </div>
    <ajax:UpdatePanel ID="EditAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="inputForm" align="center">
                <tr>
                    <th class="rowHeader">
                        <asp:Label ID="DisplayNameLabel" runat="server" Text="Name:" AssociatedControlID="DisplayName" ToolTip="Name of agreement that is displayed in the link text" SkinID="FieldHeader"></asp:Label>&nbsp;
                    </th>
                    <td colspan="2">
                        <asp:TextBox ID="DisplayName" runat="server" MaxLength="100" Width="240px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="DisplayName"
                                Display="Static" ErrorMessage="LicenseAgreement name is required.">*</asp:RequiredFieldValidator>
                        &nbsp;&nbsp;&nbsp;
                        <asp:CheckBox ID="IsHtml" runat="server" Text="Is HTML" />
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader" valign="top">
                        <asp:ImageButton ID="HtmlButton" runat="server" SkinID="HtmlIcon" AlternateText="Edit HTML" />
                        <asp:Label ID="AgreementTextLabel" runat="server" Text="Content:" AssociatedControlID="AgreementText" ToolTip="Content of agreement displayed when it is viewed" SkinID="FieldHeader"></asp:Label>
                    </th>
                    <td>
                        <asp:TextBox ID="AgreementText" runat="server" Width="600px" Height="400px" TextMode="multiLine"  EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Label ID="SavedMessage" runat="server" Text="Saved at {0:t}" Visible="false" SkinID="GoodCondition" EnableViewState="False"></asp:Label>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                        <asp:Button ID="SaveAndCloseButton" runat="server" Text="Save and Close" OnClick="SaveAndCloseButton_Click" />
						<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <br />
    <table width="100%">
        <tr class="sectionHeader">
            <td>
                <h2>Associated Digital Goods</h2>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="DigitalGoodsGrid" runat="server" AutoGenerateColumns="false" ShowHeader="false"
                    ShowFooter="false" SkinID="PagedList" AllowSorting="false" AllowPaging="false" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Name">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <a href="EditDigitalGood.aspx?DigitalGoodId=<%#Eval("DigitalGoodId")%>"><%#Eval("Name")%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="EmptyMessage" runat="server" Text="There are no digital goods that are associated with this agreement."></asp:Label>
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
