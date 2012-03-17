<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="EditShipment.aspx.cs" Inherits="Admin_Orders_Shipments_EditShipment" Title="Edit Shipment" EnableViewState="false" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register Src="EditShipmentItems.ascx" TagName="EditShipmentItems" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">   

<script language="javascript" type="text/javascript">
var val=0;
function QuantityCheck(qty)
{
	if(qty.value<1)
	{
		if( confirm('Changing quantity to 0 will result in deletion of this item from the order. Are you sure you want to delete this item?') )
		{
			qty.value = 0;
		}
		else
		{
			qty.value = val;
			
		}
	}
	val = 0;

}
</script>
 
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Edit Shipment #{0}"></asp:Localize></h1>
        </div>
    </div>
    
    <table class="inputForm" cellpadding="4" cellspacing="0" style="margin-top:8px;" width="100%">
        <tr>
            <th colspan="4">                
                <asp:Literal ID="EditInstructions" runat="server" Text="If you modify the items in a shipment, you may need to manually adjust any taxes, shipping charges, or payments already made. When adding Gift Certificates, Digital Goods, or Subscriptions to an order, you must take steps to manually activate these items. When editing the value of a Gift Certificate type order item, after it has been added to an order, you must manually adjust the respective Gift Certificate value."></asp:Literal>
            </th>
        </tr>
        <%--<tr>--%>       
        <tr>
            <td colspan="4">
                 <uc1:EditShipmentItems ID="EditShipmentItems1" runat="server" CaptionText="Edit Shipment Items" />
                <br /><br />                
            </td>
        </tr>
        <tr class="sectionHeader">
            <th colspan="4">
                <asp:Label ID="AddressLabel" runat="server" Text="Shipping Address"></asp:Label>
            </th>
        </tr>      
        <tr>
            <th class="rowHeader">
                <asp:Label ID="ShipToFirstNameLabel" runat="server" Text="First Name:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToFirstName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ShipToFirstNameValidator" runat="server" ControlToValidate="ShipToFirstName" ErrorMessage="First name is required." Text="*"></asp:RequiredFieldValidator>
            </td>
            <th class="rowHeader">
                <asp:Label ID="ShipToLastNameLabel" runat="server" Text="Last Name:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToLastName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ShipToLastNameValidator" runat="server" ControlToValidate="ShipToLastName" ErrorMessage="Last name is required." Text="*"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="ShipToCompanyLabel" runat="server" Text="Company:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToCompany" runat="server"></asp:TextBox>
            </td>
            
            <th class="rowHeader" valign="top">
                <asp:Label ID="ShipToPhoneLabel" runat="server" Text="Phone:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToPhone" runat="server"></asp:TextBox>
            </td>
            
            
        </tr>
        <tr>
            <th class="rowHeader">
                <asp:Label ID="ShipToAddress1Label" runat="server" Text="Street Address 1:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToAddress1" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ShipToAddress1Validator" runat="server" ControlToValidate="ShipToAddress1" ErrorMessage="First line of street address is required." Text="*"></asp:RequiredFieldValidator>
            </td>
            <th class="rowHeader">
                <asp:Label ID="ShipToAddress2Label" runat="server" Text="Street Address 2:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToAddress2" runat="server"></asp:TextBox>
            </td>
            
        </tr>
        <tr>
        <th class="rowHeader">
                <asp:Label ID="ShipToCityLabel" runat="server" Text="City:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToCity" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ShipToCityValidator" runat="server" ControlToValidate="ShipToCity" ErrorMessage="City is required." Text="*"></asp:RequiredFieldValidator>
            </td>
            <th class="rowHeader">
                <asp:Label ID="ShipToProvinceLabel" runat="server" Text="State / Province:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToProvince" runat="server"></asp:TextBox>
            </td>
            
            
            
        </tr>
        <tr>
        <th class="rowHeader">
                <asp:Label ID="ShipToPostalCodeLabel" runat="server" Text="ZIP / Postal Code:"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="ShipToPostalCode" runat="server"></asp:TextBox>
            </td>
            
            <th class="rowHeader">
                <asp:Label ID="ShipToCountryCodeLabel" runat="server" Text="Country:"></asp:Label>
            </th>
            <td>
                <asp:DropDownList ID="ShipToCountryCode" runat="server" DataTextField="Name" DataValueField="CountryCode"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <th class="rowHeader" valign="top" width="120">
                <asp:Label ID="ShipToFaxLabel" runat="server" Text="Fax:" AssociatedControlID="ShipToFax" EnableViewState="false"></asp:Label>
            </th>
            <td valign="top">
                <asp:TextBox ID="ShipToFax" runat="server"  MaxLength="30"></asp:TextBox> 
            </td>
            <th class="rowHeader" valign="top">
                    <asp:Label ID="ShipToResidenceLabel" runat="server" Text="Type:" AssociatedControlID="ShipToResidence" EnableViewState="false"></asp:Label>
            </th>
            <td valign="top">
                <asp:DropDownList ID="ShipToResidence" runat="server" >
                    <asp:ListItem Text="This is a residence" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="This is a business" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr class="sectionHeader">
            <th colspan="4">
                <asp:Label ID="Details" runat="server" Text="Other Details"></asp:Label>
            </th>
        </tr>
        <tr>
            <th class="rowHeader"><asp:Label ID="ShipMessageLabel" runat="server" Text="Delivery Instructions:"></asp:Label></th>
            <td colspan="3"><asp:TextBox ID="ShipMessage" runat="server" Height="74px" Rows="3" Width="257px" TextMode="multiLine" Wrap="true"></asp:TextBox></td>
        </tr>
        <tr>
            <th class="rowHeader"><asp:Label ID="TrackingNumbersLabel" runat="server" Text="Tracking Number:"></asp:Label></th>
            <td colspan="3">
                <asp:GridView ID="TrackingGrid" DataSourceID="TrackingNumberDs"  runat="server" AutoGenerateColumns="false" DataKeyNames="TrackingNumberId"  ShowHeader="false" ShowFooter="false" BorderWidth="0">
                <Columns>
                <asp:TemplateField>
                <ItemTemplate>
                <asp:TextBox ID="TrackingNumberData" runat="server" Text='<%#Eval("TrackingNumberData")%>'></asp:TextBox>
                </ItemTemplate>
                </asp:TemplateField>
                </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr id="trAddTrackingNumber" runat="server" visible="false">
        <th class="rowHeader"><asp:Label ID="AddTrackingNumberLabel" runat="server" Text="Tracking Number: " ></asp:Label></th>
        <td colspan="3">
        
                    <asp:DropDownList ID="ShipGateway" runat="server" DataTextField="Name" DataValueField="ShipGatewayId" AppendDataBoundItems="true">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="TrackingNumber" runat="server"></asp:TextBox>
                <asp:Button ID="AddTrackingNumber" runat="server" Text="Add Tracking Number" OnClick="AddTrackingNumber_Click"  />
        </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="3">
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                <asp:HyperLink ID="CancelLink" runat="server" Text="Cancel" NavigateUrl="Default.aspx" SkinID="Button" />
            </td>
        </tr>
    </table>
    <asp:ObjectDataSource ID="TrackingNumberDs" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="LoadForOrderShipment" 
        TypeName="CommerceBuilder.Orders.TrackingNumberDataSource" SortParameterName="sortExpression" EnablePaging="true" 
        SelectCountMethod="CountForOrderShipment">
        <SelectParameters>
        <asp:QueryStringParameter Name="orderShipmentId" QueryStringField="OrderShipmentId" DefaultValue="0" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>