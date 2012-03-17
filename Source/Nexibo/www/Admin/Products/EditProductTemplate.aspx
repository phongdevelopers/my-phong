<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" CodeFile="EditProductTemplate.aspx.cs" Inherits="Admin_Products_EditProductTemplate" Title="Edit Product Template" EnableViewState="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Product Templates for '{0}'"></asp:Localize></h1>
        </div>
    </div>
    <ajax:UpdatePanel ID="TemplateAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="inputForm">
                <tr>
                    <th class="rowHeader" valign="top" width="120px">
                        <asp:Label ID="TemplateListLabel" runat="server" Text="Assigned Templates: "></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="TemplateList" runat="server" Text=""></asp:Label>
                        <asp:HiddenField ID="TemplateListChanged" runat="server" />
                        <asp:HiddenField ID="HiddenSelectedTemplates" runat="server" />
                        <asp:LinkButton ID="ChangeTemplateListButton" runat="server" Text="Change" />
                        <asp:Panel ID="ChangeTemplateListDialog" runat="server" Style="display: none" CssClass="modalPopup" Width="600px">
                            <asp:Panel ID="ChangeTemplateListDialogHeader" runat="server" CssClass="modalPopupHeader">
                                Change Assigned Templates
                            </asp:Panel>
                            <div align="center">
                                <br />
                                Hold CTRL to select multiple Templates.  Double click to move a Template to the other list.
                                <br /><br />
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top" width="42%">
                                            <b>Available Templates</b><br />
                                            <asp:ListBox ID="AvailableTemplates" runat="server" Rows="12" SelectionMode="multiple" Width="220"></asp:ListBox>
                                        </td>
                                        <td valign="middle" width="6%">
                                            <asp:Button ID="SelectAllTemplates" runat="server" Text=" >> " /><br />
                                            <asp:Button ID="SelectTemplate" runat="server" Text=" > " /><br />
                                            <asp:Button ID="UnselectTemplate" runat="server" Text=" < " /><br />
                                            <asp:Button ID="UnselectAllTemplates" runat="server" Text=" << " /><br />
                                        </td>
                                        <td valign="top" width="42%">
                                            <b>Selected Templates</b><br />
                                            <asp:ListBox ID="SelectedTemplates" runat="server" Rows="12" SelectionMode="multiple" Width="220"></asp:ListBox>
                                        </td>
                                    </tr>
                                </table><br />
                                <asp:PlaceHolder ID="phMyTemplatesWarning" runat="server" Visible="false">
                                    <br />
                                    <asp:Label ID="MyTemplatesWarning" runat="server" Text="WARNING: You are modifying your own Templates.  Be careful not to lock yourself out!" SkinID="ErrorCondition"></asp:Label>
                                    <br /><br />
                                </asp:PlaceHolder>
                                <asp:Button ID="ChangeTemplateListOKButton" runat="server" Text="OK" OnClientClick="changeTemplateList()" />
                                <asp:Button ID="ChangeTemplateListCancelButton" runat="server" Text="Cancel" />
                                <br /><br />
                            </div>
                        </asp:Panel>
                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender" runat="server" 
                            TargetControlID="ChangeTemplateListButton"
                            PopupControlID="ChangeTemplateListDialog" 
                            BackgroundCssClass="modalBackground" 
                            CancelControlID="ChangeTemplateListCancelButton" 
                            DropShadow="true"
                            PopupDragHandleControlID="ChangeTemplateListDialogHeader" />
                    </td>
                </tr>
                <tr id="trUnsavedChanges" runat="server" enableviewstate="false" visible="false">
                    <td>&nbsp;</td>
                    <td style="font-style:italic;color:#960">
                        <asp:Localize ID="UnsavedChangesMessage" runat="server" Text="Assigned templates have changed.  Be sure to click the save button to make your changes permanent." EnableViewState="false" />
                    </td>
                </tr>
            </table>
            <asp:PlaceHolder ID="phCustomFields" runat="server"></asp:PlaceHolder>
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /><br />
            <asp:Label ID="SavedMessage" runat="server" Text="Saved at {0:t}." SkinID="GoodCondition" Visible="false" EnableViewState="false"></asp:Label>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>