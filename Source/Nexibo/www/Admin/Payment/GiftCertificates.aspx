<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="GiftCertificates.aspx.cs" Inherits="Admin_Payment_GiftCertificates" Title="Gift Certificates" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <script type="text/javascript">
        function toggleSelected(checkState)
        {
            // Toggles through all of the checkboxes defined in the CheckBoxIDs array
            // and updates their value to the checkState input parameter            
            for(i = 0; i< document.forms[0].elements.length; i++){
                var e = document.forms[0].elements[i];
                var name = e.name;
                if ((e.type == 'checkbox') && (name.indexOf('DeleteCheckbox') != -1) && !e.disabled)
                {
                    e.checked = checkState.checked;
                }
            }            
        }
    </script>
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Gift Certificates"></asp:Localize></h1>
    	</div>
    </div>
     <ajax:UpdatePanel ID="GiftCertificatesGridAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate> 
            <table cellpadding="2" cellspacing="0" class="innerLayout">
                <tr>
                    <td>
                        <table cellpadding="4">
                            <tr>
                                <th class="rowHeader" >
                                    <asp:Label ID="StatusListLabel" runat="server" Text="Status:" AssociatedControlID="StatusList" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:DropDownList ID="StatusList" runat="Server" AutoPostBack="true" OnSelectedIndexChanged="StatusList_SelectedIndexChanged">
                                        <asp:ListItem Text="Active Certificates" Value="0" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Inactive Certificates" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="All" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="PageSizeLabel" runat="server" Text="Page Size:" AssociatedControlID="PageSize" EnableViewState="false"></asp:Label>
                                </th>
                                <td>
                                    <asp:DropDownList ID="PageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PageSize_SelectedIndexChanged">
                                        <asp:ListItem Text="10 per page" Value="10"></asp:ListItem>
                                        <asp:ListItem Text="20 per page" Value="20" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="50 per page" Value="50"></asp:ListItem>
                                        <asp:ListItem Text="show all" Value=""></asp:ListItem>
                                    </asp:DropDownList>                    
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="GiftCertificatesGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="GiftCertificateId"
                            DataSourceID="GiftCertificateDs" OnRowCommand="GiftCertificatesGrid_RowCommand" AllowSorting="True" 
                            SkinID="Summary" AllowPaging="true" PageSize="20" Width="100%">
                            <Columns>    
                                <asp:TemplateField HeaderText="Select">
                                    <HeaderTemplate>
                                        <input type="checkbox" onclick="toggleSelected(this)" />
                                    </HeaderTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="40px" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="DeleteCheckbox" runat="server" Enabled ='<%#IsDeleteable((int)Eval("GiftCertificateId")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>                
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
								        <asp:HyperLink ID="GiftCertTransactionsLink" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%#Eval("GiftCertificateId", "../Reports/GiftCertTransactions.aspx?GiftCertificateId={0}")%>'>
								        </asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>                                                
                                <asp:TemplateField HeaderText="Order#">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                            	        <asp:HyperLink ID="GiftCertOrderIdLink" runat="server" Text='<%# Eval("OrderItem.Order.OrderNumber") %>' NavigateUrl='<%#string.Format("../Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderItem.Order.OrderNumber"), Eval("OrderItem.OrderId"))%>'>
								        </asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>                        
						        <asp:TemplateField HeaderText="Created" >                            
                                    <ItemTemplate>
                                        <asp:Label ID="CreateDate" runat="server" Text='<%#Eval("CreateDate", "{0:d}")%>' Visible='<%#(DateTime)Eval("CreateDate") != System.DateTime.MinValue%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
						        <asp:TemplateField HeaderText="Expires" SortExpression="ExpirationDate">                            
                                    <ItemTemplate>
                                        <asp:Label ID="ExpirationDate" runat="server" Text='<%#Eval("ExpirationDate", "{0:d}")%>' Visible='<%#(DateTime)Eval("ExpirationDate") != System.DateTime.MinValue%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
						        <asp:TemplateField HeaderText="Balance" SortExpression="Balance" >   
						            <ItemStyle HorizontalAlign="right" />                         
                                    <ItemTemplate>
                                        <asp:Label ID="Balance" runat="server" Text='<%#Eval("Balance", "{0:F2}")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Serial Number">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="SerialNumber" runat="server" Text='<%#Eval("SerialNumber")%>'></asp:Label>
								        <asp:Button ID="Generate" runat="Server" Text="Generate" CommandName="Generate" CommandArgument='<%#Container.DataItemIndex%>' Visible='<%#!HasSerialNumber(Container.DataItem)%>' OnClientClick='return confirm("Generating a Serial Number will Activate this Gift Certificate. Continue?")' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Right" Width="180px" />
                                    <ItemTemplate>
								        <asp:Button ID="DeactivateButton" runat="server" CausesValidation="False" CommandName="Deactivate"  CommandArgument='<%#Container.DataItemIndex%>' AlternateText="Deactivate" Text="Deactivate" Visible='<%#HasSerialNumber(Container.DataItem)%>' OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to deactivate {0}?\")") %>'/>
                                        <asp:HyperLink ID="EditButton" runat="server" NavigateUrl='<%#Eval("GiftCertificateId", "EditGiftCertificate.aspx?GiftCertificateId={0}") %>'><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" /></asp:HyperLink>
                                        <asp:ImageButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" SkinID="DeleteIcon" OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>' AlternateText="Delete" Visible='<%#IsDeleteable((int)Eval("GiftCertificateId")) %>' />								
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyMessage" runat="server" Text="No Gift Certificates found."></asp:Label>
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="GiftCertificateDs" runat="server" DeleteMethod="Delete"
                            OldValuesParameterFormatString="{0}" SelectMethod="LoadForStatus" 
                            TypeName="CommerceBuilder.Payments.GiftCertificateDataSource" SortParameterName="sortExpression"
                            EnablePaging="true" SelectCountMethod="CountForStatus">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="StatusList" Name="status" PropertyName="SelectedValue" Type="Object" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                        <asp:Literal ID="InstructionsText" runat="server" Text="<p />You can select and delete multiple inactive gift certificates at once." Visible='<%#StatusList.SelectedValue != "0" %>'></asp:Literal>
                        <br />
                        <asp:Button ID="MultipleRowDelete" runat="server"   Text="Delete Selected" OnClick="MultipleRowDelete_Click" OnClientClick="return confirm('Are you sure you want to delete the selected gift certificate(s)?')"  />
                        <asp:HyperLink ID="AddGiftCertificate" runat="server" Text="Add Gift Certificate" NavigateUrl="AddGiftCertificate.aspx" SkinID="Button"></asp:HyperLink>                
                    </td>
                </tr>         
            </table>
         </ContentTemplate>
    </ajax:UpdatePanel>
     <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <div class="section">                
                    <div class="header">
                        <h2 class="commonicon">
                            <asp:Localize ID="GiftCertLookUpCaption" runat="server" Text="Gift Certificate Lookup by Serial Number"></asp:Localize></h2>
                    </div>
                    <div class="content">
                        <ajax:UpdatePanel ID="GiftCertLookupPanel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>  
                                 <asp:Label ID="LookupLabel" runat="server" Text="Gift Certificate Serial Number:" SkinID="FieldHeader"></asp:Label>
                                 <asp:TextBox ID="SerialNumber" runat="server" ></asp:TextBox>
                                 <asp:Button ID="LookupButton" runat="server" Text="Find" OnClick="LookupButton_OnClick" />
                                 <br />
                                 <asp:Label ID="NotFoundMessage" runat="server" Visible="false" SkinID="errorCondition" EnableViewState="false" Text="No Gift Certificates found for the given serial number."></asp:Label>
                                 <table id="ViewGiftcertificatePanel" runat="server" cellpadding="3" cellspacing="0" width="350px" visible="false" >
                                    <tr>
                                        <td colspan="2">
                                            <hr />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="NameLabel" runat="server" Text="Name:" AssociatedControlID="Name" EnableViewState="false"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:Label ID="Name" runat="server" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="GiftCertOrderIdLinkLabel" runat="server" Text="Order #:" AssociatedControlID="GiftCertOrderIdLink" EnableViewState="false"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:HyperLink ID="GiftCertOrderIdLink" runat="server" ></asp:HyperLink>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="CreateDateLabel" runat="server" Text="Create Date:" AssociatedControlID="CreateDate" EnableViewState="false"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:Label ID="CreateDate" runat="server" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="ExpirationDateLabel" runat="server" Text="Expiration Date:" AssociatedControlID="ExpirationDate" EnableViewState="false"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:Label ID="ExpirationDate" runat="server" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="BalanceLabel" runat="server" Text="Balance:" AssociatedControlID="Balance" EnableViewState="false"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:Label ID="Balance" runat="server" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="rowHeader">
                                            <asp:Label ID="SerialNumberLabel" runat="server" Text="Serial Number:" AssociatedControlID="GCSerialNumber" EnableViewState="false"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:Label ID="GCSerialNumber" runat="server" ></asp:Label>
					                        <asp:Button ID="Generate" runat="Server" Text="Generate" OnClientClick='return confirm("Generating a Serial Number will Activate this Gift Certificate. Continue?")' OnClick="Generate_OnClick" EnableViewState="false" Visible="false" />
                                        </td>
                                    </tr>
                                    <tr>                                       
                                        <td colspan="2">
                                            <asp:Button ID="DeactivateButton" runat="Server"  Text="Deactivate"  OnClick="Deactivate_OnClick" EnableViewState="false" Visible="false" />
                                            <asp:Button ID="DeleteButton" runat="server" Text="Delete" CausesValidation="False" OnClick="Delete_OnClick"  OnClientClick='return confirm("Are you sure you want to delete this gift certificate?");' Visible="false" />
                                            <asp:HyperLink ID="ViewHistory" runat="Server" SkinID="button"   Text="View Transactions"   EnableViewState="false"  />
                                            <asp:HiddenField ID="HiddenGCID" runat="server" />
                                        </td>
                                    </tr>
                                 </table>
                            </ContentTemplate>
                        </ajax:UpdatePanel>                                 
                    </div>
                </div>
            </td>
        </tr>
    </table>        
</asp:Content>
