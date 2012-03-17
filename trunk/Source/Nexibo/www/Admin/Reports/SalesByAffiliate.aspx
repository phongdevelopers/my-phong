<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="SalesByAffiliate.aspx.cs" Inherits="Admin_Reports_SalesByAffiliate" Title="Sales by Affiliate" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

		<div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="Caption" runat="server" Text="Sales by Affiliate"></asp:Localize><asp:Localize ID="ReportDateCaption" runat="server" Text=" for {0:MMMM yyyy}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>



            <!--- <table cellpadding="2" cellspacing="0" class="innerLayout"> -->
			<table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr class="noPrint">
                    <td align="center">
                        <asp:Button ID="PreviousButton" runat="server" Text="&laquo; Prev" OnClick="PreviousButton_Click" />&nbsp;<asp:Label ID="MonthLabel" runat="server" Text="Month: " SkinID="FieldHeader"></asp:Label>
                        <asp:DropDownList ID="MonthList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DateFilter_SelectedIndexChanged">
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
                        <asp:DropDownList ID="YearList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DateFilter_SelectedIndexChanged">
                        </asp:DropDownList>
                        &nbsp;
                        <asp:Button ID="NextButton" runat="server" Text="Next &raquo;" OnClick="NextButton_Click" />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet" >
                        <cb:SortedGridView ID="AffiliateSalesGrid" runat="server" AutoGenerateColumns="False" DataSourceID="AffiliateSalesDs"
                            AllowPaging="false" AllowSorting="false" Width="100%" SkinID="Summary" Visible="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Affiliate">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="AffiliateLink" runat="server" Text='<%# Eval("AffiliateName") %>' NavigateUrl='<%#Eval("AffiliateId", "../Marketing/Affiliates/EditAffiliate.aspx?AffiliateId={0}")%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Referrals">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="ReferralCount" runat="server" Text='<%# Eval("ReferralCount") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Conversion">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="ConversionRate" runat="server" Text='<%# GetConversionRate(Container.DataItem) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Orders">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="OrderCount" runat="server" Text='<%# Eval("OrderCount") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Products">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="ProductSubtotal" runat="server" Text='<%# Eval("ProductSubtotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="OrderTotal" runat="server" Text='<%# Eval("OrderTotal", "{0:lc}")  %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Commission">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="CommissionRate" runat="server" Text='<%# GetCommissionRate(Container.DataItem) %>'></asp:Label>
                                        <asp:Label ID="Commission" runat="server" Text='<%# Eval("Commission", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="DetailLink" runat="server" Text="Detail" NavigateUrl='<%#GetDetailUrl(Container.DataItem)%>' SkinID="Link"></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyResult">
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                                </div>    
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                        <br /><i><asp:Label ID="ReportTimestamp" runat="server" Text="Report generated {0:MMM-dd-yyyy hh:mm tt}" EnableViewState="false"></asp:Label></i>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="HiddenStartDate" runat="server" Value="" />
            <asp:HiddenField ID="HiddenEndDate" runat="server" Value="" />
        </ContentTemplate>
    </ajax:UpdatePanel>
    <asp:ObjectDataSource ID="AffiliateSalesDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSalesByAffiliate" 
        TypeName="CommerceBuilder.Reporting.ReportDataSource">
        <SelectParameters>
            <asp:ControlParameter ControlID="HiddenStartDate" Name="startDate" PropertyName="Value" Type="DateTime" />
            <asp:ControlParameter ControlID="HiddenEndDate" Name="endDate" PropertyName="Value" Type="DateTime" />
            <asp:Parameter Name="affiliateId" DefaultValue="0" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

