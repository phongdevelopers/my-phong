<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="EditDiscount.aspx.cs" Inherits="Admin_Marketing_Discounts_EditDiscount" Title="Add/Edit Discount" %>
<%@ Register Assembly="CommerceBuilder.Web" Namespace="CommerceBuilder.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1>
                <asp:Localize ID="AddCaption" runat="server" Text="Add Volume Discount"></asp:Localize>
                <asp:Localize ID="EditCaption" runat="server" Text="Edit '{0}'"></asp:Localize>
            </h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td align="center">
                <asp:ValidationSummary ID="DetailValidationSummary" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <table cellpadding="4" cellspacing="0" class="inputForm">
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Name:" ToolTip="The name of the discount as it will appear in the merchant admin."></cb:ToolTipLabel>
                        </th>
                        <td valign="top">
                            <asp:TextBox ID="Name" runat="server" Text="" Columns="20" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NameRequired" runat="server" Text="*" Display="Static" ErrorMessage="Name is required." ControlToValidate="Name"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="NameValidator" runat="server" ErrorMessage="Maximum length for Name is 100 characters." Text="*" ControlToValidate="Name" ValidationExpression=".{0,100}"  ></asp:RegularExpressionValidator>
                        </td>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="DiscountBasisLabel" runat="server" Text="Base on:" ToolTip="Indicate whether the discount is based on the quantity or the total value of items purchased."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:DropDownList ID="IsValueBased" runat="server">
                                <asp:ListItem Text="Quantity of Line Item"></asp:ListItem>
                                <asp:ListItem Text="Total Price of Line Item"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="DiscountLevelsLabel" runat="server" Text="Discount Levels:" ToolTip="Configure the amount of discount based on the minimum and maximum values.  The value ranges are based on either quantity or value purchased as configured above."></cb:ToolTipLabel><br />
                        </th>
                        <td colspan="3">
                            <ajax:UpdatePanel ID="DiscountGridAjax" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="DiscountLevelGrid" runat="server" AutoGenerateColumns="false" Width="100%" SkinID="Summary" OnPreRender="DiscountLevelGrid_PreRender" OnRowDataBound="DiscountLevelGrid_RowDataBound" OnRowDeleting="DiscountLevelGrid_OnRowDeleting">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Minimum">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="MinValue" runat="server" Text='<%#GetLevelValue(((VolumeDiscountLevel)Container.DataItem).MinValue)%>' Columns="2" MaxLength="10"></asp:TextBox>                                                    
                                                    <asp:RangeValidator ID="MinValueRangeValidator" runat="server" Text="*" Type="currency" ErrorMessage="Minimum value should be between 0 and 99999999.99" ControlToValidate="MinValue" MinimumValue="0" MaximumValue="99999999.99" >
                                                    </asp:RangeValidator>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Maximum">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="MaxValue" runat="server" Text='<%#GetLevelValue(((VolumeDiscountLevel)Container.DataItem).MaxValue)%>' Columns="2" MaxLength="10"></asp:TextBox>
                                                    <asp:RangeValidator ID="MaxValueRangeValidator1" runat="server" Text="*" Type="currency" ErrorMessage="Maximum value should be between 0 and 99999999.99" ControlToValidate="MaxValue" MinimumValue="1" MaximumValue="99999999.99"/>
                                                    <asp:CompareValidator ID="MinMaxCompareValidator" runat="server" Text="*"  ControlToValidate="MaxValue" ControlToCompare="MinValue" 
                                                        ErrorMessage="Maximum value can not be less then minimum value" Operator="GreaterThanEqual" Type="currency"></asp:CompareValidator>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Discount" ItemStyle-Wrap="false" HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="DiscountAmount" runat="server" Text='<%#Eval("DiscountAmount", "{0:0.##}")%>' Columns="2" MaxLength="10"></asp:TextBox>
                                                    <asp:RangeValidator ID="DiscountAmountValidator" runat="server" Text="*" Type="currency" ErrorMessage="Discount amount should be between 0 and 99999999.99" ControlToValidate="DiscountAmount" MinimumValue="0" MaximumValue="99999999.99" />
                                                    <asp:DropDownList ID="IsPercent" runat="server" >
                                                        <asp:ListItem Text="Percent (%)" ></asp:ListItem>
                                                        <asp:ListItem Text="Fixed Amount" ></asp:ListItem>
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="DeleteRowButton" runat="server" AlternateText="Delete Row" SkinID="DeleteIcon" CommandName="Delete"  OnClientClick="return confirm('Are you sure you want to delete this row?')"/>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView><br />
                                    <asp:Button id="AddRowButton" runat="server" Text="Add Row" OnClick="AddRowButton_Click" CausesValidation="true"></asp:Button>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                       </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="ScopeLabel" runat="server" Text="Discount Scope:" ToolTip="Manage the categories and products that the discount applies to."></cb:ToolTipLabel>
                        </th>
                        <td colspan="3">
                            <ajax:UpdatePanel ID="ScopeAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="UseGlobalScope" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UseGlobalScope_SelectedIndexChanged">
                                        <asp:ListItem Text="Global"></asp:ListItem>
                                        <asp:ListItem Text="Specific" Selected="true"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Panel ID="ScopePanel" runat="server">
                                        <div style="padding-left:20px">
                                            <cb:ToolTipLabel ID="Scope" runat="server" Text="{0} Categories and {1} Products"></cb:ToolTipLabel>
                                            <asp:LinkButton ID="EditDiscountScope" runat="server" Text="[change]" SkinID="Link" OnClick="EditDiscountScope_Click"></asp:LinkButton>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <cb:ToolTipLabel ID="GroupsLabel" runat="server" Text="Groups:" ToolTip="Indicate whether only users that belong to specific groups can have this discount applied." />
                        </th>
                        <td colspan="3">
                            <ajax:UpdatePanel ID="GroupRestrictionAjax" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="UseGroupRestriction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UseGroupRestriction_SelectedIndexChanged">
                                        <asp:ListItem Text="All Groups" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="Selected Groups"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Panel ID="GroupListPanel" runat="server" Visible="false">
                                        <div style="padding-left:20px">
                                            <asp:CheckBoxList ID="GroupList" runat="server" DataTextField="Name" DataValueField="GroupId" RepeatColumns="3"></asp:CheckBoxList>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </ajax:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="submit">                            
                            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
							<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>


