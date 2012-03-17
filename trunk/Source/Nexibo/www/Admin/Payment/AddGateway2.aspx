<%@ Page Language="C#" MasterPageFile="../Admin.master" CodeFile="AddGateway2.aspx.cs" Inherits="Admin_Payment_AddGateway2" Title="Add Gateway"  %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Add Gateway"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td>
            <asp:PlaceHolder ID="phInputForm" runat="server"></asp:PlaceHolder>
            <asp:Panel ID="PaymentMethodPanel" runat="server">
                <table class="inputForm" width="100%">
                    <tr class="sectionHeader">
                        <td>
                            <asp:Label ID="PaymentMethodCaption" runat="server" Text="Payment Methods" SkinID="FieldHeader"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p style="margin:4px;">Select the methods that should use this gateway.</p>
                            <div align="center">
                                <asp:DataList ID="PaymentMethodList" runat="server" DataKeyField="PaymentMethodId" RepeatDirection="Horizontal" RepeatColumns="2" CellPadding="3" Width="300px">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="Method" runat="server" Text='<%#Eval("Name")%>' Checked='<%#IsMethodAssigned(Container.DataItem)%>' />
                                    </ItemTemplate>
                                </asp:DataList>
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div align="center" style="margin-top:20px">            
            <asp:Button ID="BackButton" runat="server" Text="< Back" OnClick="BackButton_Click" />
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
			<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
            </div>
        </td>
    </tr>
</table>
</asp:Content>

