<%@ Page Language="C#" MasterPageFile="Order.master" CodeFile="ViewDigitalGoods.aspx.cs" Inherits="Admin_Orders_ViewDigitalGoods" Title="Digital Goods" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Digital Goods"></asp:Localize></h1></div>
</div>
<div style="margin-top:4px">
    <ajax:UpdatePanel ID="DigitalGoodsAjax" runat="server">
        <ContentTemplate>
            <asp:GridView ID="DigitalGoodsGrid" runat="server" AutoGenerateColumns="false" AllowPaging="false" 
                AllowSorting="false" OnRowCommand="DigitalGoodsGrid_RowCommand" 
                CellPadding="4" SkinID="PagedList" Width="100%" EnableViewState="false" OnRowDeleting="DigitalGoodsGrid_RowDeleting">
                <Columns>
                    <asp:TemplateField HeaderText="Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <a href="../DigitalGoods/EditDigitalGood.aspx?DigitalGoodId=<%#Eval("DigitalGoodId")%>"><%#Eval("Name")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Activated" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Literal ID="ActivationDate" runat="server" Text='<%# Eval("ActivationDate", "{0:g}") %>' Visible='<%#(DateTime)Eval("ActivationDate") != System.DateTime.MinValue%>' EnableViewState="false"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Downloaded" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Literal ID="FirstDownload" runat="server" Text='<%# Eval("DownloadDate", "{0:g}") %>' Visible='<%#(DateTime)Eval("DownloadDate") != System.DateTime.MinValue%>' EnableViewState="False"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="# Downloads" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <a href="ViewDownloads.aspx?OrderNumber=<%#Eval("OrderItem.Order.OrderNumber")%>&OrderId=<%#Eval("OrderItem.OrderId")%>&OrderItemDigitalGoodId=<%# Eval("OrderItemDigitalGoodId")%>"><%#Eval("RelevantDownloads")%></a>
                            <asp:Literal ID="MaxDownloads" runat="server" Text='<%#GetMaxDownloads((OrderItemDigitalGood)Container.DataItem)%>' EnableViewState="false"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Literal ID="DownloadStatus" runat="server" Text='<%#Eval("DownloadStatus")%>' EnableViewState="false"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Serial Key" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>                            
                            <asp:Button ID="SetKey" runat="Server" Text="Set Key" CommandName="SetKey" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Visible='<%# ShowSetKey(Container.DataItem) %>' EnableViewState="false" />
                            <asp:Button ID="GetKey" runat="Server" Text="Get Key" CommandName="GetKey" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Visible='<%# ShowGetKey(Container.DataItem)%>' EnableViewState="false" />
                            <%--<asp:Button ID="ReturnKey" runat="Server" Text="Release" CommandName="ReturnKey" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Visible='<%#SerialKeyEnabled(Container.DataItem) && HasSerialKey((OrderItemDigitalGood)Container.DataItem)%>' EnableViewState="false" />--%>
                            <asp:Button ID="ViewKey" runat="Server" Text="View" CommandName="ViewKey" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Visible='<%#HasSerialKey((OrderItemDigitalGood)Container.DataItem)%>' EnableViewState="false" />
                        </ItemTemplate>                        
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="Activate" runat="Server" Text="Activate" CommandName="Activate" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Visible='<%#((OrderItemDigitalGood)Container.DataItem).DigitalGood != null && (DateTime)Eval("ActivationDate") == System.DateTime.MinValue%>' EnableViewState="false" />
                            <asp:Button ID="Deactivate" runat="Server" Text="Deactivate" CommandName="Deactivate" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Visible='<%#((OrderItemDigitalGood)Container.DataItem).DigitalGood != null && (DownloadStatus)Eval("DownloadStatus") == DownloadStatus.Valid%>' EnableViewState="false" />
                            <asp:Button ID="Reactivate" runat="Server" Text="Reactivate" CommandName="Activate" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Visible='<%#((OrderItemDigitalGood)Container.DataItem).DigitalGood != null && ((DownloadStatus)Eval("DownloadStatus") == DownloadStatus.Expired || (DownloadStatus)Eval("DownloadStatus") == DownloadStatus.Depleted)%>' EnableViewState="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:ImageButton ID="DeleteButton" runat="server" CommandName="Delete" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' ToolTip="Delete" SkinID="DeleteIcon" OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' EnableViewState="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Localize ID="EmptyDataMessage" runat="server" Text="There are no digital goods associated with this order." EnableViewState="false"></asp:Localize>
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:Panel ID="EditKeyDialog" runat="server" Style="display:none;width:450px" CssClass="modalPopup">
                <asp:Panel ID="EditKeyDialogHeader" runat="server" CssClass="modalPopupHeader" EnableViewState="false">
                    <asp:Localize ID="EditKeyDialogCaption" runat="server" Text="Serial Key" EnableViewState="false"></asp:Localize>
                </asp:Panel>
                <div style="padding-top:5px;">
                    <table class="inputForm">
                        <tr>
                            <td class="rowHeader" Width="30%" valign="top">
                                <asp:Label ID="SerialKeyDataLabel" runat="server" Text="Serial Key Data:" />
                            </td>
                            <td>
                                <asp:TextBox ID="SerialKeyData" runat="server" Rows="5" Columns="50" Wrap="true" TextMode="multiLine"></asp:TextBox>
                                <asp:HiddenField ID="OidgId" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>                                
                                <asp:Button ID="SaveKeyButton" runat="server" CommandName="SaveKey" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Text="Save" ValidationGroup="SerialKey" OnClick="SaveKeyButton_Click" />
                                <asp:Button ID="CancelButton" runat="server" CommandName="CancelKey" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Text="Close" ValidationGroup="SerialKey" />
                                <asp:Button ID="DeleteKeyButton" runat="server" CommandName="DeleteSerialKey" CommandArgument='<%#Eval("OrderItemDigitalGoodId")%>' Text="Delete" CausesValidation="false" OnClick="DeleteKeyButton_Click" OnClientClick="return confirm('Are you sure you want to delete the serial key?')" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>  
            <asp:HiddenField ID="EditKey" runat="server" />
            <ajax:ModalPopupExtender ID="EditKeyPopup" runat="server" 
                TargetControlID="EditKey"
                PopupControlID="EditKeyDialog" 
                BackgroundCssClass="modalBackground"                         
                CancelControlID="CancelButton" 
                DropShadow="false"
                PopupDragHandleControlID="EditKeyDialogHeader" />
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:HyperLink ID="AttachLink" runat="server" SkinID="Button" Text="Add" NavigateUrl="~/Admin/Orders/AddDigitalGoods.aspx?OrderNumber={0}&OrderId={1}"></asp:HyperLink>
</div>
</asp:Content>