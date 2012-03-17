<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master" CodeFile="SortParts.aspx.cs" Inherits="Admin_Products_Kits_SortParts" Title="Sort Parts"  EnableViewState="false" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <script language="javascript">
	    function UP(nLevels) {
            var sel = document.forms[0].<%= KitParts.ClientID %>;
            if (sel.selectedIndex == null || sel.selectedIndex < 0)
			    //nothing to move
			    return false;
		    if (sel.length == 1)
			    //not long enough
			    return false
		    if (sel.selectedIndex == 0)
			    //already first
			    return false;			
		    if ((sel.selectedIndex - nLevels) < 0)
			    //can't move that high up
			    nLevels = sel.selectedIndex;

		    tempIndex = sel.selectedIndex
		    newIndex = tempIndex - nLevels
    		
		    tempVal = sel[tempIndex].value;
		    tempText = sel[tempIndex].text;
		    sel[tempIndex].text = sel[newIndex].text;
		    sel[tempIndex].value = sel[newIndex].value;
		    sel[newIndex].text = tempText;
		    sel[newIndex].value = tempVal;
		    sel.selectedIndex = newIndex
		    return false;
	    }

	    function DN(nLevels) {
            var sel = document.forms[0].<%= KitParts.ClientID %>;
		    if (sel.selectedIndex == null || sel.selectedIndex < 0)
			    //nothing to move
			    return false;
		    if (sel.length == 1)
			    //not long enough
			    return false
		    if (sel.selectedIndex == (sel.length - 1))
			    //already last
			    return false;			
		    if ((sel.selectedIndex + nLevels) > sel.length - 1)
			    //can't move that far
			    nLevels = sel.length - 1;

		    tempIndex = sel.selectedIndex
		    newIndex = tempIndex + nLevels
    		
		    tempVal= sel[tempIndex].value;
		    tempText = sel[tempIndex].text;
		    sel[tempIndex].text = sel[newIndex].text;
		    sel[tempIndex].value = sel[newIndex].value;
		    sel[newIndex].text = tempText;
		    sel[newIndex].value = tempVal;
		    sel.selectedIndex = newIndex
		    return false;
	    }
    	
	    function SubmitMe() {
            var sel = document.forms[0].<%= KitParts.ClientID %>;
            var sortOrder = document.forms[0].<%= SortOrder.ClientID %>;
		    for (i=0; i<sel.length; i++) {
			    sortOrder.value = sortOrder.value + sel[i].value;
			    if (i < (sel.length - 1)) sortOrder.value = sortOrder.value + ",";
		    }
		    return true;
	    }
</script>	    
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Kitting: Sort Parts in {0}" EnableViewState="false"></asp:Localize></h1>
        </div>
    </div>
    <div style="padding:6px">
        <asp:Label ID="InstructionText" runat="server" Text="To arrange the parts in the correct order, click an item in the list and use the buttons to move it up or down.  You can also use the quick sort to reorder the list by name or price.  When you have finished updating the order of the items, click Save."></asp:Label>
    </div>
    <div style="width:500px">
        <table>
            <tr>
                <td valign="top">
                    <asp:ListBox ID="KitParts" runat="server" DataTextField="DisplayName" DataValueField="KitProductId" Rows="10">
                    </asp:ListBox>
                </td>
                <td valign="middle" align="Center">
					<asp:Button ID="UP100" runat="server" Text="FIRST" Width="60px" OnClientClick="return UP(1000)" /><br />
					<asp:Button ID="UP1" runat="server" Text="/\" Width="60px" OnClientClick="return UP(1)" /><br />
					<asp:Button ID="DN1" runat="server" Text="\/" Width="60px" OnClientClick="return DN(1)" /><br />
					<asp:Button ID="DN100" runat="server" Text="LAST" Width="60px" OnClientClick="return DN(1000)" /><br />
                    <hr />
                    <asp:HiddenField ID="SortOrder" runat="server" />
                    <asp:Button ID="SaveButton" runat="server" Text="Save" Width="60px" OnClientClick="return SubmitMe()" OnClick="SaveButton_Click" /><br />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" Width="60px" OnClick="CancelButton_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="QuickSortLabel" runat="server" Text="Quick Sort:" SkinID="FieldHeader" AssociatedControlId="QuickSort">
                    </asp:Label>
                    <asp:DropDownList ID="QuickSort" runat="server" AutoPostBack="true" OnSelectedIndexChanged="QuickSort_SelectedIndexChanged">
                        <asp:ListItem Text=""></asp:ListItem>
                        <asp:ListItem Text="Name (A -> Z)"></asp:ListItem>
                        <asp:ListItem Text="Name (Z -> A)"></asp:ListItem>
                        <asp:ListItem Text="Price (Low -> High)"></asp:ListItem>
                        <asp:ListItem Text="Price (High -> Low)"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

