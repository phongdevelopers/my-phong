<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="Admin_Store_EmailTemplates_Help" Title="NVelocity Syntax Help" EnableViewState="false" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Email Templates: NVelocity Syntax Help"></asp:Localize></h1>
    	</div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <p align="justify">
                Both subject and message content support NVelocity syntax.  This allows you to substitute dynamic information in your email messages
                when they are generated.</p>
                
                <h2>Variable Substitution</h2>
                
                <b>Standard Syntax:</b> ${variable_name}<br />
                <b>Example:</b> Hello ${customer.FirstName}!<br /><br />

                <b>Shortcut Syntax:</b> $variable_name<br />
                <b>Example:</b> Your order for $order.TotalCharges was received.<br /><br />
                
                <p>Shortcut syntax cannot be used if your variable is part of a larger string where the variable name
                would not be clear.  Below shows an example of incorrect use of the shortcut syntax, where standard syntax 
                must be used instead:</p>
                
                <b>Incorrect (Shortcut):</b> some_text$variable_namesome_more_text<br />
                <b>Correct (Standard):</b> some_text${variable_name}some_more_text<br />
                
                <h2>Standard Variables</h2>
                
                <p>All email templates provide you access to the "store" variable.  From here you can access all the properties 
                of the CommerceBuilder.Stores.Store object, initialized for your store.</p>
                
                <p>Most email templates will also provide you access to the "customer" variable.  This gives you access to
                all of the properties of the CommerceBuilder.Users.User object, initialized for the applicable customer.</p>
                
                <p>Email templates that are triggered by order events will also provide access to the "order" variable.  This gives you 
                access to all the properties of the CommerceBuilder.Orders.Order object, initialized for the applicable order.</p>

                <h2>.NET Syntax and Variable Formatting</h2>
                
                <p>When you reference variables you are accessing the object through the .NET framework.  You can use traditional
                .NET syntax to access properties and methods.  A common example might be to provide string formats:</p>
                
                $order.OrderDate.ToString("mm-ddd-yyyy")<br />
                $order.TotalCharges.ToString("lc")
                
                <h2>Conditional Statements</h2>
                
                You can employ conditional logic in your email templates with the if-else-end statement.  For example:
                
<pre>
#if($order.TotalCharges > 100)
    Because your order was more than $100, we will include a free hat as a thank you.
#else
    Spend $100 or more on your next order to receive a free gift!
#end
</pre>

                <h2>Looping</h2>

                You can loop over collections of items with the foreach-end statement.  For example:
                
<pre>
&lt;table&gt;
#foreach($item in $order.Items)
&lt;tr&gt;
&lt;td&gt;$item.Name&lt;/td&gt;
&lt;td&gt;$item.Quantity&lt;/td&gt;
&lt;/tr&gt;
#end
&lt;/table&gt;
</pre>


                <h2>Fancy Looping</h2>

                The foreach statement supports additional features to enable things like alternating rows, headers, and footers.
<pre>
#foreach($i in $items)
#each (this is optional since its the default section)
       text which appears for each item
#before
       text which appears before each item
#after
       text which appears after each item
#between
       text which appears between each two items
#odd
       text which appears for every other item, including the first
#even
       text which appears for every other item, starting with the second
#nodata
       Content rendered if $items evaluated to null or empty
#beforeall
       text which appears before the loop, only if there are items
       matching condition
#afterall
       text which appears after the loop, only of there are items
       matching condition
#end
</pre>                
            </td>
        </tr>
    </table>
</asp:Content>