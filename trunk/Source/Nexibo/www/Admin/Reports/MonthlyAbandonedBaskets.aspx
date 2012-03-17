<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="MonthlyAbandonedBaskets.aspx.cs" Inherits="Admin_Reports_MonthlyAbandonedBaskets" Title="Abandoned Baskets" %>
<%@ register tagprefix="web" namespace="WebChart" assembly="WebChart"%>
<%@ Import Namespace="WebChart" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1><asp:Localize ID="Caption" runat="server" Text="Abandoned Baskets"></asp:Localize><asp:Localize ID="ReportCaption" runat="server" Text=" for {0:MMMM yyyy}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>
            <br />
            <div class="noPrint" style="text-align:center;">
                <asp:Button ID="PreviousButton" runat="server" Text="&laquo; Previous" OnClick="PreviousButton_Click" />
                &nbsp;
                <asp:Label ID="MonthLabel" runat="server" Text="Month: " SkinID="FieldHeader"></asp:Label>
                <asp:DropDownList ID="MonthList" runat="server" AutoPostBack="true">
                    <asp:ListItem Value="1" Text="January"></asp:ListItem>
                    <asp:ListItem Value="2" Text="February"></asp:ListItem>
                    <asp:ListItem Value="3" Text="March"></asp:ListItem>
                    <asp:ListItem Value="4" Text="April"></asp:ListItem>
                    <asp:ListItem Value="5" Text="May"></asp:ListItem>
                    <asp:ListItem Value="6" Text="June"></asp:ListItem>
                    <asp:ListItem Value="7" Text="July"></asp:ListItem>
                    <asp:ListItem Value="8" Text="August"></asp:ListItem>
                    <asp:ListItem Value="9" Text="September"></asp:ListItem>
                    <asp:ListItem Value="10" Text="October"></asp:ListItem>
                    <asp:ListItem Value="11" Text="November"></asp:ListItem>
                    <asp:ListItem Value="12" Text="December"></asp:ListItem>
                </asp:DropDownList>&nbsp;
                <asp:Label ID="YearLabel" runat="server" Text="Year: " SkinID="FieldHeader"></asp:Label>
                <asp:DropDownList ID="YearList" runat="server" AutoPostBack="true">
                </asp:DropDownList>
                &nbsp;
                <asp:Button ID="NextButton" runat="server" Text="Next &raquo;" OnClick="NextButton_Click" />
                <br />
                <asp:CheckBox ID="FilterResults" runat="server" Checked="true" 
                    Text="Do not display days that don't have any abandoned baskets." 
                    oncheckedchanged="FilterResults_CheckedChanged" AutoPostBack="true" />
            </div>
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td align="center" class="dataSheet">
                        <web:chartcontrol runat="server" id="BasketChart" Width="700" Height="270" GridLines="Horizontal" HasChartLegend="false" YValuesFormat="{0:N0}" SkinID="Column"></web:chartcontrol>
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet">
                        <asp:GridView ID="AbandonedBasketGrid" runat="server" AutoGenerateColumns="False" Width="100%" ShowFooter="False"
                            SkinID="Summary">
                            <Columns>
                                <asp:TemplateField HeaderText="Date" SortExpression="StartDate">
                                    <ItemTemplate>
                                        <asp:Label ID="DateLabel" runat="server" Text='<%# Eval("StartDate", "{0:d}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No. of Baskets">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("BasketCount") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total Amount">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("Total", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>                                                                
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="DetailsLink" runat="server" Text="Details" SkinID="Button" NavigateUrl='<%#Eval("StartDate", "DailyAbandonedBaskets.aspx?Date={0:yyyy-MMM-dd}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

