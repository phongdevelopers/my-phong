<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="MonthlySales.aspx.cs" Inherits="Admin_Reports_MonthlySales" Title="Monthly Sales" %>
<%@ register tagprefix="web" namespace="WebChart" assembly="WebChart"%>
<%@ Import Namespace="WebChart" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <div class="pageHeader">
                <div class="caption">
                    <h1><asp:Localize ID="Caption" runat="server" Text="Monthly Sales Report"></asp:Localize><asp:Localize ID="ReportCaption" runat="server" Text=" for {0:MMMM yyyy}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>
            <br />
            <div class="noPrint" style="text-align:center;">
                <asp:Button ID="PreviousButton" runat="server" Text="&laquo; Previous" OnClick="PreviousButton_Click" />
                &nbsp;
                <asp:Label ID="MonthLabel" runat="server" Text="Month: " SkinID="FieldHeader"></asp:Label>
                <asp:DropDownList ID="MonthList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="MonthList_OnSelectedIndexChanged">
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
                <asp:DropDownList ID="YearList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="YearList_OnSelectedIndexChanged">
                </asp:DropDownList>
                &nbsp;
                <asp:Button ID="NextButton" runat="server" Text="Next &raquo;" OnClick="NextButton_Click" />
                <br />
                <asp:CheckBox ID="FilterResults" runat="server" Checked="true" Text="Do not display days having zero number of orders." AutoPostBack="true" OnCheckedChanged="FilterResults_CheckedChanged"/>
            </div><br />
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td align="center" class="dataSheet">
                        <web:chartcontrol runat="server" id="SalesChart" Width="700" Height="270" GridLines="Horizontal" HasChartLegend="false" YValuesFormat="{0:N0}" SkinID="CompactColumn"></web:chartcontrol>
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet">
                        <asp:GridView ID="MonthlySalesGrid" runat="server" AutoGenerateColumns="False" Width="100%" ShowFooter="True" 
                            SkinID="Summary" FooterStyle-CssClass="totalRow">
                            <Columns>
                                <asp:TemplateField HeaderText="Date" SortExpression="StartDate">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="DateLabel" runat="server" Text='<%# Eval("StartDate", "{0:d}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="FooterTotalsLabel" runat="server" Text="Totals:" SkinID="FieldHeader"></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Orders">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <FooterStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("OrderCount") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="OrderCountTotal" runat="server" Text='<%# GetTotal("Count") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Products">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("ProductTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="ProductTotal" runat="server" Text='<%# GetTotal("Product") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipping">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("ShippingTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="ShippingTotal" runat="server" Text='<%# GetTotal("Shipping") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Tax">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label4" runat="server" Text='<%# Eval("TaxTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="TaxTotal" runat="server" Text='<%# GetTotal("Tax") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Discount">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label5" runat="server" Text='<%# Eval("DiscountTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="DiscountTotal" runat="server" Text='<%# GetTotal("Discount") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Coupon">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label6" runat="server" Text='<%# Eval("CouponTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="CouponTotal" runat="server" Text='<%# GetTotal("Coupon") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Gift Wrap">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="GiftWrapLabel" runat="server" Text='<%# Eval("GiftWrapTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="GiftWrapTotal" runat="server" Text='<%# GetTotal("GiftWrap") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Other">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label7" runat="server" Text='<%# Eval("OtherTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="OtherTotal" runat="server" Text='<%# GetTotal("Other") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Profit">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="ProfitLabel" runat="server" Text='<%# Eval("ProfitTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="ProfitTotal" runat="server" Text='<%# GetTotal("Profit") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label8" runat="server" Text='<%# Eval("GrandTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="GrandTotal" runat="server" Text='<%# GetTotal("Grand") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" CssClass="noPrint" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="DetailsLink" runat="server" Text="Details" SkinID="Button" NavigateUrl='<%#Eval("StartDate", "DailySales.aspx?Date={0:yyyy-MMM-dd}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No orders to display.
                            </EmptyDataTemplate>              
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

