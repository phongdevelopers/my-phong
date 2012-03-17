<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="AddressFormat.aspx.cs" Inherits="Admin_Shipping_Countries_AddressFormat" Title="Address Format Help"  %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Address Format Help"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <p align="justify">When adding or updating a country, you may need to adjust the rules for address formatting for that locale.
                AbleCommerce provides you the ability to specify variables in the address format so that you can control
                how an address will be displayed or printed for that country.  This will allow you to accomodate addressing
                regulations for the countries that you ship to.</p>
                
                <table cellpadding="4" cellspacing="0" border="1">
                    <tr>
                        <th>
                            [Name]
                        </th>
                        <td>
                            The full name of the addressee.
                        </td>
                    </tr>
                    <tr>
                        <th>
                            [Address1]
                        </th>
                        <td>
                            The first line of the street address.
                        </td>
                    </tr>
                    <tr>
                        <th>
                            [Address2]
                        </th>
                        <td>
                            The second line of the street address.
                        </td>
                    </tr>
                    <tr>
                        <th>
                            [City]
                        </th>
                        <td>
                            The city for the address.
                        </td>
                    </tr>
                    <tr>
                        <th>
                            [Province]
                        </th>
                        <td>
                            The state or province for the address.  If a province abbreviation is defined, this is used instead of the full name.
                        </td>
                    </tr>
                    <tr>
                        <th>
                            [PostalCode]
                        </th>
                        <td>
                            The postal code for the address.
                        </td>
                    </tr>
                    <tr>
                        <th>
                            [Country]
                        </th>
                        <td>
                            The full name of the country for the address.
                        </td>
                    </tr>
                </table>
                
                <p align="justify">For any of the variable, you can add "_U" to the end to force the value to upper case.  For example, "[Country_U]" will 
                display the name of the country in uppercase.</p>
                
                <p>Example formats, for mail sent from United States:</p>
                
                <pre>
                <b>United States</b>
                [Name_U]
                [Address1_U]
                [Address2_U]
                [City_U] [Province_U] [PostalCode_U]

                <b>Canada</b>
                [Name_U]
                [Address1_U]
                [Address2_U]
                [City_U] [Province_U] &nbsp;[PostalCode_U]
                [Country_U]

                <b>United Kingdom</b>
                [Name]
                [Address1]
                [Address2]
                [City_U]
                [PostalCode_U]
                [Country_U]
                </pre>                
                
                <p>If an address format is not provided for a country, a US style format will be applied.</p>
            </td>
        </tr>
    </table>
</asp:Content>

