<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="ErrorLog.aspx.cs" Inherits="Admin_Help_ErrorLog" Title="View Error Log"  %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
	
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <script type="text/javascript">
       function toggleCheckBoxState(id, checkState)
       {
          var cb = document.getElementById(id);
          if (cb != null)
             cb.checked = checkState;
       }

       function toggleSelected(checkState)
       {
          // Toggles through all of the checkboxes defined in the CheckBoxIDs array
          // and updates their value to the checkState input parameter
          if (CheckBoxIDs != null)
          {
             for (var i = 0; i < CheckBoxIDs.length; i++)
                toggleCheckBoxState(CheckBoxIDs[i], checkState.checked);
          }
       }
    </script>
    
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="View Error Log"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <ajax:UpdatePanel ID="ErrorMessageAjax" runat="server">
                    <ContentTemplate>
                        <cb:SortedGridView ID="ErrorMessageGrid" runat="server" AllowPaging="True" AutoGenerateColumns="False" 
                            DataKeyNames="ErrorMessageId" DataSourceID="ErrorMessageDs" PageSize="20" DefaultSortExpression="EntryDate" 
                            SkinID="PagedList" AllowSorting="True" DefaultSortDirection="Descending" ShowWhenEmpty="False" 
                            Width="100%" EnableViewState="false" OnDataBound="ErrorMessageGrid_DataBound">
                            <Columns>
                            <asp:TemplateField>
                                <HeaderStyle Width="40px" />
                                <HeaderTemplate>
                                    <input type="checkbox" onclick="toggleSelected(this)" />
                                </HeaderTemplate>
                                <ItemStyle HorizontalAlign="center" Width="40px" />
                                <ItemTemplate>
                                    <asp:CheckBox ID="Selected" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                                <asp:BoundField DataField="EntryDate" HeaderText="Date" SortExpression="EntryDate" >
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="MessageSeverity" HeaderText="Severity" SortExpression="MessageSeverityId" >
                                    <itemstyle horizontalalign="Left" Width="80px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Text" HeaderText="Message" SortExpression="Text" >
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DebugData" HeaderText="Debug Data" SortExpression="Text" >
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Localize ID="EmptyMessage" runat="server" Text="There are no entries in the error log."></asp:Localize>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                        <asp:ObjectDataSource ID="ErrorMessageDs" runat="server" 
                            OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForStore" 
                            TypeName="CommerceBuilder.Utility.ErrorMessageDataSource" SortParameterName="sortExpression"
                            EnablePaging="True" DataObjectTypeName="CommerceBuilder.Utility.ErrorMessage" DeleteMethod="Delete"></asp:ObjectDataSource>
                        <br />
						<asp:PlaceHolder ID="ButtonsPlaceHolder" runat="server">
							<asp:Button ID="DeleteSelectedButton" runat="server" Text="Delete Selected" OnClick="DeleteSelectedButton_Click" OnClientClick="return confirm('Are you sure you want to delete the selected log entries?')" />
							<asp:Button ID="DeleteAllButton" runat="server" Text="Delete All" OnClick="DeleteAllButton_Click" OnClientClick="return confirm('Are you sure you want to delete all log entries?')" />
							<asp:HyperLink ID="ExportErrorLogLink" runat="server" Text="Export" NavigateUrl="ExportErrorLog.ashx" SkinId="Button"></asp:HyperLink>
						</asp:PlaceHolder>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>
