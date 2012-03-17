using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Users;
using CommerceBuilder.Web.UI.WebControls;
using CommerceBuilder.UI.Styles;
using CommerceBuilder.Utility;

public partial class Admin_Products_Batching_BatchEdit : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private bool _ShowEditGrid = false;
    private bool _EnableScrolling = true;
    private List<Product> _SelectedProducts = new List<Product>();
    private List<int> _SelectedProductIds = new List<int>();
    private List<string> _SelectedFields = new List<string>();
    private Dictionary<string, string> _ProductFields = new Dictionary<string, string>();
    private Dictionary<string, string> _ProductFieldDescriptions = new Dictionary<string, string>();

    protected void Page_Init(object sender, EventArgs e)
    {
        // INITIALIZE FORM FIELDS
        InitializeCategoryTree();
        ManufacturerFilter.DataSource = ManufacturerDataSource.LoadForStore("Name");
        ManufacturerFilter.DataBind();
        VendorFilter.DataSource = VendorDataSource.LoadForStore("Name");
        VendorFilter.DataBind();
        InitJs();
        LoadSearchCriteria();
        InitializeFieldSelections();

        // LOAD VIEWSTATE DATA
        LoadCustomViewState();

        // INITIALIZE GRID ACCORDING TO VIEWSTATE
        SearchPanel.Visible = !_ShowEditGrid;
        EditPanel.Visible = _ShowEditGrid;
        if (EditPanel.Visible)
        {
            BindEditGrid();
        }

         // INITIALIZE HTML EDITOR
        HtmlEditorPageInit();
    }

    private void InitializeFieldSelections()
    {
        // BUILD THE LIST OF AVAILABLE FIELDS
        _ProductFields["allowbackorder"] = "Allow Backorder";
        if(Token.Instance.Store.Settings.ProductReviewEnabled != UserAuthFilter.None)
            _ProductFields["allowreviews"] = "Allow Reviews";
        _ProductFields["costofgoods"] = "Cost of Goods";
        _ProductFields["customurl"] = "Custom Url";
        _ProductFields["description"] = "Description";
        _ProductFields["disablepurchase"] = "Disable Purchase";
        _ProductFields["displaypage"] = "Display Page";
        _ProductFields["excludefromfeed"] = "Exclude From Feed";
        _ProductFields["isfeatured"] = "Featured";
        _ProductFields["isgiftcertificate"] = "Gift Certificate";
        _ProductFields["wrapgroupid"] = "Gift Wrap";
        _ProductFields["height"] = "Height";
        _ProductFields["hideprice"] = "Hide Price";
        _ProductFields["htmlhead"] = "Html Header";
        _ProductFields["iconalttext"] = "Icon Alt Text";
        _ProductFields["iconurl"] = "Icon Url";
        _ProductFields["imagealttext"] = "Image Alt Text";
        _ProductFields["imageurl"] = "Image Url";
        _ProductFields["instock"] = "In Stock";
        _ProductFields["inventorymodeid"] = "Inventory Tracking";
        _ProductFields["length"] = "Length";
        _ProductFields["instockwarninglevel"] = "Low Stock";
        _ProductFields["manufacturerid"] = "Manufacturer";
        _ProductFields["modelnumber"] = "Manuf. Part No.";
        _ProductFields["maximumprice"] = "Max Price";
        _ProductFields["maxquantity"] = "Max Quantity";
        _ProductFields["minimumprice"] = "Min Price";
        _ProductFields["minquantity"] = "Min Quantity";
        _ProductFields["extendeddescription"] = "More Details";
        _ProductFields["msrp"] = "MSRP";
        _ProductFields["name"] = "Name";
        _ProductFields["isprohibited"] = "Prohibited";
        _ProductFields["price"] = "Price";
        _ProductFields["searchkeywords"] = "Search Keywords";
        _ProductFields["shippableid"] = "Shippable";
        _ProductFields["sku"] = "SKU";
        _ProductFields["summary"] = "Summary";
        _ProductFields["taxcodeid"] = "Tax Code";
        _ProductFields["theme"] = "Theme";
        _ProductFields["thumbnailalttext"] = "Thumbnail Alt Text";
        _ProductFields["thumbnailurl"] = "Thumbnail Url";
        _ProductFields["usevariableprice"] = "Variable Price";
        _ProductFields["vendorid"] = "Vendor";
        _ProductFields["visibilityid"] = "Visibility";
        _ProductFields["warehouseid"] = "Warehouse";
        _ProductFields["weight"] = "Weight";
        _ProductFields["width"] = "Width";

        // SET FIELD DESCRIPTIONS
        _ProductFieldDescriptions["allowbackorder"] = "If inventory control is enabled, check this field to enable backorder for the product.";
        if (Token.Instance.Store.Settings.ProductReviewEnabled != UserAuthFilter.None)
            _ProductFieldDescriptions["allowreviews"] = "If product reviews are enabled on store then you can enable/disable reviews on product level through this Allow Review property.";
        _ProductFieldDescriptions["costofgoods"] = "The cost of goods for this product - what you pay to acquire your inventory.";
        _ProductFieldDescriptions["customurl"] = "You can provide a custom URL to access your product. This URL will override the default one generated by system.";
        _ProductFieldDescriptions["description"] = "The main description of the product that is usually shown on the product detail page.";
        _ProductFieldDescriptions["disablepurchase"] = "When checked, this product cannot be purchased by a customer.";
        _ProductFieldDescriptions["displaypage"] = "Indicates the display page used when the product is viewed.";
        _ProductFieldDescriptions["excludefromfeed"] = "When checked, this product is not included in automated product feeds such as Google Base.";
        _ProductFieldDescriptions["extendeddescription"] = "An extended description of the product that can be viewed by the customer.";
        _ProductFieldDescriptions["height"] = "The shipping height of the product.";
        _ProductFieldDescriptions["hideprice"] = "When checked, the price of the product is not shown until the customer requests to see it or adds the item to the basket.";
        _ProductFieldDescriptions["htmlhead"] = "Any content here will be put into the HEAD portion of the HTML document generated for the product display page.";
        _ProductFieldDescriptions["iconalttext"] = "Alternate text for the icon image.";
        _ProductFieldDescriptions["iconurl"] = "The product icon image.";
        _ProductFieldDescriptions["imagealttext"] = "Alternate text for the main product image.";
        _ProductFieldDescriptions["imageurl"] = "The main product image.";
        _ProductFieldDescriptions["instock"] = "Indicates the count of the product currenly in stock.  This value is only effective if inventory tracking is enabled for the store and the product is set to track inventory at the product level.";
        _ProductFieldDescriptions["instockwarninglevel"] = "Indicates the inventory level at which a warning notice will be sent to the merchant.  This value is only effective if inventory tracking is enabled for the store and the product is set to track inventory.";
        _ProductFieldDescriptions["inventorymodeid"] = "Indicates the inventory mode for the product.  This value is only effective if inventory tracking is enabled for the store.";
        _ProductFieldDescriptions["isfeatured"] = "When checked, the product is marked as featured.";
        _ProductFieldDescriptions["isgiftcertificate"] = "When checked, the product generates a gift certificate when it is purchased.";
        _ProductFieldDescriptions["isprohibited"] = "When checked, this product cannot be purchased with Google Checkout.";
        _ProductFieldDescriptions["length"] = "The shipping length of the product.";
        _ProductFieldDescriptions["manufacturerid"] = "The manufacturer of the product.";
        _ProductFieldDescriptions["maximumprice"] = "The maximum price that can be set for the product.  This value is only effective if the product uses variable pricing.";
        _ProductFieldDescriptions["maxquantity"] = "The maximum quantity of the product that can be purchased by a customer in a single order.";
        _ProductFieldDescriptions["minimumprice"] = "The minimum price that can be set for the product.  This value is only effective if the product uses variable pricing.";
        _ProductFieldDescriptions["minquantity"] = "The minimum quantity of the product that must be purchased by a customer in a single order.";
        _ProductFieldDescriptions["modelnumber"] = "The model number of the product.";
        _ProductFieldDescriptions["msrp"] = "The manufacturer suggested retail price of the product.";
        _ProductFieldDescriptions["name"] = "The name of the product.";
        _ProductFieldDescriptions["price"] = "The base price of the product.";
        _ProductFieldDescriptions["searchkeywords"] = "Search keywords that are associated with the product.";
        _ProductFieldDescriptions["shippableid"] = "Indicates whether or not the product is shippable.";
        _ProductFieldDescriptions["sku"] = "The SKU of the product.";
        _ProductFieldDescriptions["summary"] = "A summary description of the product.  This generally appears on category or search display pages where the product is listed with other products.";
        _ProductFieldDescriptions["taxcodeid"] = "The tax code of the product.";
        _ProductFieldDescriptions["theme"] = "The theme used to display the product.";
        _ProductFieldDescriptions["thumbnailalttext"] = "Alternate text for the thumbnail image.";
        _ProductFieldDescriptions["thumbnailurl"] = "Thumbnail image for the prdocut.";
        _ProductFieldDescriptions["usevariableprice"] = "When checked, the product is set to use a variable price that can be adjusted by the customer at checkout.  Useful for sales of gift certificates or donation items.";
        _ProductFieldDescriptions["vendorid"] = "The vendor of the product.";
        _ProductFieldDescriptions["visibilityid"] = "Indicates whether the product is visible on the retail store website.";
        _ProductFieldDescriptions["warehouseid"] = "The warehouse of the product.";
        _ProductFieldDescriptions["weight"] = "The shipping weight of the product.";
        _ProductFieldDescriptions["width"] = "The shipping width of the prodouct.";
        _ProductFieldDescriptions["wrapgroupid"] = "The gift wrapping styles associated with the product.";

        // DRAW SELECTED FIELDS
        SelectedFields.Items.Clear();
        List<string> defaultFields = GetDefaultFields();
        foreach (string field in defaultFields)
        {
            SelectedFields.Items.Add(new ListItem(_ProductFields[field], field));
        }

        // DRAW AVAILABLE FIELDS
        AvailableFields.Items.Clear();
        foreach (string field in _ProductFields.Keys)
        {
            if (!defaultFields.Contains(field))
            {
                AvailableFields.Items.Add(new ListItem(_ProductFields[field], field));
            }
        }

    }

    private List<string> GetDefaultFields()
    {
        List<string> defaultFields = new List<string>();
        string defaultFieldList = Token.Instance.User.Settings.GetValueByKey("BatchEditFieldList");
        if (!string.IsNullOrEmpty(defaultFieldList))
        {
            string[] defaultFieldArray = defaultFieldList.Split(",".ToCharArray());
            foreach (string field in defaultFieldArray)
            {
                string cleanField = field.Trim().ToLowerInvariant();
                if (IsValidField(cleanField))
                {
                    defaultFields.Add(cleanField);
                }
            }
        }
        if (defaultFields.Count == 0)
        {
            defaultFields.Add("instock");
            defaultFields.Add("name");
            defaultFields.Add("price");
            defaultFields.Add("sku");
        }
        return defaultFields;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        bool rebind = false;

        // DETERMINE IF EVENTS NEED TO BE HANDLED
        string eventTarget = Request.Form["__EVENTTARGET"];
        if (eventTarget == SearchButton.UniqueID)
        {
            SearchButton_Click();
            rebind = true;
        }
        else if (eventTarget == NewSearchButton.UniqueID)
        {
            NewSearchButton_Click();
            rebind = true;
        }
        else if (eventTarget == SaveButton.UniqueID)
        {
            SaveGrid();
            rebind = true;
        }

        // REBIND GRID IF NEEDED IN RESPONSE TO ACTION
        if (rebind)
        {
            SearchPanel.Visible = !_ShowEditGrid;
            EditPanel.Visible = _ShowEditGrid;
            if (EditPanel.Visible)
            {
                BindEditGrid();
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        SaveCustomViewState();
    }

    protected void InitializeCategoryTree()
    {
        CategoryLevelNodeCollection categories = CategoryParentDataSource.GetCategoryLevels(0);
        foreach (CategoryLevelNode node in categories)
        {
            string prefix = string.Empty;
            for (int i = 0; i <= node.CategoryLevel; i++) prefix += " . . ";
            CategoryFilter.Items.Add(new ListItem(prefix + node.Name, node.CategoryId.ToString()));
        }
    }

    private void BindEditGrid()
    {
        // START TABLE
        BatchEditGrid.Controls.Clear();
        if (_EnableScrolling) BatchEditGrid.Controls.Add(new LiteralControl("<div style=\"width:100%;overflow:scroll;\">"));
        BatchEditGrid.Controls.Add(new LiteralControl("<table cellpadding=\"3\" cellspacing=\"0\" class=\"pagedList\">\n"));

        // DRAW HEADERS
        BatchEditGrid.Controls.Add(new LiteralControl("<tr>\n"));
        BatchEditGrid.Controls.Add(new LiteralControl("<th align=\"left\">Product</th>"));
        foreach (string field in _SelectedFields)
        {
            BatchEditGrid.Controls.Add(new LiteralControl(string.Format("<th align=\"left\" nowrap>")));
            ToolTipLabel colHeader = new ToolTipLabel();
            colHeader.Text = _ProductFields[field];
            colHeader.ToolTip = _ProductFieldDescriptions[field];
            colHeader.SkinID = "ColHeader";
            BatchEditGrid.Controls.Add(colHeader);
            BatchEditGrid.Controls.Add(new LiteralControl("</th>"));
        }
        BatchEditGrid.Controls.Add(new LiteralControl("</tr>\n"));

        // DRAW ROWS
        bool isOddRow = true;
        foreach (Product product in _SelectedProducts)
        {
            if (isOddRow) BatchEditGrid.Controls.Add(new LiteralControl("<tr class=\"oddRow\">\n"));
            else BatchEditGrid.Controls.Add(new LiteralControl("<tr class=\"evenRow\">\n"));
            isOddRow = !isOddRow;
            string productUrl = "../EditProduct.aspx?ProductId=" + product.ProductId;
            BatchEditGrid.Controls.Add(new LiteralControl(string.Format("<td>\n<input type=\"hidden\" name=\"P{0}\" value=\"1\">\n<a href=\"{2}\">{1}</a>\n</td>\n", product.ProductId, Server.HtmlEncode(product.Name), productUrl)));
            foreach (string field in _SelectedFields)
            {
                string cellAlignment;
                List<Control> controls = DrawField(product, field, out cellAlignment);
                if (controls != null && controls.Count > 0)
                {
                    if (!string.IsNullOrEmpty(cellAlignment)) BatchEditGrid.Controls.Add(new LiteralControl("<td align=\"" + cellAlignment + "\" nowrap>\n"));
                    else BatchEditGrid.Controls.Add(new LiteralControl("<td nowrap>\n"));
                    foreach (Control wc in controls) BatchEditGrid.Controls.Add(wc);
                    BatchEditGrid.Controls.Add(new LiteralControl("</td>\n"));
                }
                else BatchEditGrid.Controls.Add(new LiteralControl("<td>&nbsp</td>"));
            }
            BatchEditGrid.Controls.Add(new LiteralControl("</tr>\n"));
        }

        // CLOSE TABLE
        BatchEditGrid.Controls.Add(new LiteralControl("</table>\n"));
        if (_EnableScrolling) BatchEditGrid.Controls.Add(new LiteralControl("</div>\n"));
    }

    private void SearchButton_Click()
    {
        _SelectedProducts.Clear();
        _SelectedProductIds.Clear();
        List<Product> products = ProductDataSource.FindProducts(NameFilter.Text, SkuFilter.Text, AlwaysConvert.ToInt(CategoryFilter.SelectedValue),
            AlwaysConvert.ToInt(ManufacturerFilter.SelectedValue), AlwaysConvert.ToInt(VendorFilter.SelectedValue), BitFieldState.Any, 0,
            AlwaysConvert.ToInt(MaximumRows.SelectedValue), 0, string.Empty);
        if (products.Count > 0)
        {
            foreach (Product product in products) AddProductToList(product);
            BuildFieldList(HiddenSelectedFields.Value);
            SaveSearchCriteria();
            _ShowEditGrid = true;
            _EnableScrolling = EnableScrolling.Checked;
        }
        else
        {
            NoResultsMessage.Visible = true;
            _ShowEditGrid = false;
        }
    }

    private void LoadSearchCriteria()
    {
        string searchCriteriaList = Token.Instance.User.Settings.GetValueByKey("BatchEditSearchCriteria");
        if (!string.IsNullOrEmpty(searchCriteriaList))
        {
            string[] searchCriteriaArray = searchCriteriaList.Split("|".ToCharArray());
            if (searchCriteriaArray.Length == 7)
            {
                NameFilter.Text = searchCriteriaArray[0];
                SkuFilter.Text = searchCriteriaArray[1];
                ListItem selItem = CategoryFilter.Items.FindByValue(searchCriteriaArray[2]);
                if (selItem != null) CategoryFilter.SelectedIndex = CategoryFilter.Items.IndexOf(selItem);
                selItem = ManufacturerFilter.Items.FindByValue(searchCriteriaArray[3]);
                if (selItem != null) ManufacturerFilter.SelectedIndex = ManufacturerFilter.Items.IndexOf(selItem);
                selItem = VendorFilter.Items.FindByValue(searchCriteriaArray[4]);
                if (selItem != null) VendorFilter.SelectedIndex = VendorFilter.Items.IndexOf(selItem);
                selItem = MaximumRows.Items.FindByValue(searchCriteriaArray[5]);
                if (selItem != null) MaximumRows.SelectedIndex = MaximumRows.Items.IndexOf(selItem);
                EnableScrolling.Checked = AlwaysConvert.ToBool(searchCriteriaArray[6], true);
            }
        }
    }

    private void SaveSearchCriteria()
    {
        User user = Token.Instance.User;
        // SAVE CRITERIA
        List<string> searchCriteria = new List<string>();
        searchCriteria.Add(NameFilter.Text);
        searchCriteria.Add(SkuFilter.Text);
        searchCriteria.Add(CategoryFilter.SelectedValue);
        searchCriteria.Add(ManufacturerFilter.SelectedValue);
        searchCriteria.Add(VendorFilter.SelectedValue);
        searchCriteria.Add(MaximumRows.SelectedValue);
        searchCriteria.Add(EnableScrolling.Checked.ToString());
        user.Settings.SetValueByKey("BatchEditSearchCriteria", string.Join("|", searchCriteria.ToArray()));
        // SAVE FIELDS
        string defaultFields = string.Empty;
        if (_SelectedFields.Count > 0) defaultFields = string.Join(",", _SelectedFields.ToArray());
        user.Settings.SetValueByKey("BatchEditFieldList", defaultFields);
        // UPDATE USER
        user.Save();
    }

    private void NewSearchButton_Click()
    {
        _ShowEditGrid = false;
    }

    private void InitJs()
    {
        this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "selectbox", this.ResolveUrl("~/js/selectbox.js"));
        string unselectedBoxName = AvailableFields.ClientID;
        string selectedBoxName = SelectedFields.ClientID;
        AvailableFields.Attributes.Add("onDblClick", "moveSelectedOptions(this.form['" + unselectedBoxName + "'], this.form['" + selectedBoxName + "'], true, '')");
        SelectedFields.Attributes.Add("onDblClick", "moveSelectedOptions(this.form['" + selectedBoxName + "'], this.form['" + unselectedBoxName + "'], true, '');");
        SelectAllFields.Attributes.Add("onclick", "moveAllOptions(this.form['" + unselectedBoxName + "'], this.form['" + selectedBoxName + "'], true, ''); return false;");
        SelectField.Attributes.Add("onclick", "moveSelectedOptions(this.form['" + unselectedBoxName + "'], this.form['" + selectedBoxName + "'], true, ''); return false;");
        UnselectField.Attributes.Add("onclick", "moveSelectedOptions(this.form['" + selectedBoxName + "'], this.form['" + unselectedBoxName + "'], true, ''); return false;");
        UnselectAllFields.Attributes.Add("onclick", "moveAllOptions(this.form['" + selectedBoxName + "'], this.form['" + unselectedBoxName + "'], true, ''); return false;");
        SearchButton.OnClientClick = "var ele=document.getElementById('" + HiddenSelectedFields.ClientID + "');ele.value=getOptions(document.getElementById('" + selectedBoxName + "'));";

        // CUSTOM JAVASCRIPT TO WORK WITH EDITOR BOX
        StringBuilder script = new StringBuilder();
        script.Append("var _EditHtmlFieldRef = null;\n");
        script.Append("var _EditHtmlInstanceRef = null;\n");
        script.Append("function ShowEditHtmlDialog(fieldId)\n");
        script.Append("{\n");
        script.Append("\tvar callerField = document.getElementById(fieldId);\n");
        script.Append("\tif (callerField && _EditHtmlInstanceRef)\n");
        script.Append("\t{\n");
        script.Append("\t\t_EditHtmlFieldRef = fieldId;\n");
        script.Append("\t\t_EditHtmlInstanceRef.SetHTML(callerField.value);\n");
        script.Append("\t\t$find('" + EditHtmlPopup.ClientID + "').show();\n");
        script.Append("\t}\n");
        script.Append("}\n\n");
        script.Append("function SaveEditHtmlDialog()\n");
        script.Append("{\n");
        script.Append("\tvar callerField = document.getElementById(_EditHtmlFieldRef);\n");
        script.Append("\tif (callerField)\n");
        script.Append("\t{\n");
        script.Append("\t\tcallerField.value = _EditHtmlInstanceRef.GetHTML();\n");
        script.Append("\t}\n");
        script.Append("\treturn HideEditHtmlDialog();\n");
        script.Append("}\n");
        script.Append("function HideEditHtmlDialog()\n");
        script.Append("{\n");
        script.Append("\tvar callerField = document.getElementById(_EditHtmlFieldRef);\n");
        script.Append("\tif (callerField) callerField.focus();\n");
        script.Append("\t_EditHtmlFieldRef = null;\n");
        script.Append("\t_EditHtmlInstanceRef.SwitchEditMode();\n");
        script.Append("\tif (_EditHtmlInstanceRef.EditMode == FCK_EDITMODE_SOURCE) _EditHtmlInstanceRef.SwitchEditMode();\n");
        script.Append("\t$find('" + EditHtmlPopup.ClientID + "').hide();\n");
        script.Append("\treturn false;\n");
        script.Append("}\n\n");
        script.Append("function FCKeditor_OnComplete(editorInstance)\n");
        script.Append("{\n");
        script.Append("\t_EditHtmlInstanceRef = editorInstance;\n");
        script.Append("\teditorInstance.LinkedField.form.onsubmit = SaveEditHtmlDialog;\n");
        script.Append("}\n");
        script.Append("var _EditLongTextFieldRef = null;\n");
        script.Append("function ShowEditLongTextDialog(fieldId)\n");
        script.Append("{\n");
        script.Append("\tvar callerField = document.getElementById(fieldId);\n");
        script.Append("\tvar textRef = document.getElementById('" + EditLongText.ClientID + "');\n");
        script.Append("\tif (callerField && textRef)\n");
        script.Append("\t{\n");
        script.Append("\t\t_EditLongTextFieldRef = fieldId;\n");
        script.Append("\t\ttextRef.value = callerField.value;\n");
        script.Append("\t\t$find('" + EditLongTextPopup.ClientID + "').show();\n");
        script.Append("\t\ttextRef.focus();\n");
        script.Append("\t}\n");
        script.Append("}\n");
        script.Append("function SaveEditLongTextDialog()\n");
        script.Append("{\n");
        script.Append("\tvar callerField = document.getElementById(_EditLongTextFieldRef);\n");
        script.Append("\tvar textRef = document.getElementById('" + EditLongText.ClientID + "');\n");
        script.Append("\tif (callerField && textRef) callerField.value = textRef.value;\n");
        script.Append("\treturn HideEditLongTextDialog();\n");
        script.Append("}\n");
        script.Append("function HideEditLongTextDialog()\n");
        script.Append("{\n");
        script.Append("\tvar callerField = document.getElementById(_EditLongTextFieldRef);\n");
        script.Append("\tif (callerField) callerField.focus();\n");
        script.Append("\t_EditLongTextFieldRef = null;\n");
        script.Append("\t$find('" + EditLongTextPopup.ClientID + "').hide();\n");
        script.Append("\treturn false;\n");
        script.Append("}\n\n");
        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "editHtmlDialog", script.ToString(), true);
    }

    private void AddProductToList(int productId)
    {
        AddProductToList(ProductDataSource.Load(productId));
    }

    private void AddProductToList(Product product)
    {
        if (product != null && !_SelectedProductIds.Contains(product.ProductId))
        {
            _SelectedProducts.Add(product);
            _SelectedProductIds.Add(product.ProductId);
        }
    }

    private void BuildFieldList(string fieldList)
    {
        _SelectedFields.Clear();
        if (!string.IsNullOrEmpty(fieldList))
        {
            string[] tempFields = fieldList.Split(",".ToCharArray());
            if (tempFields != null && tempFields.Length > 0)
            {
                for (int i = 0; i < tempFields.Length; i++)
                {
                    AddFieldToList(tempFields[i]);
                }
            }
        }
    }

    private void AddFieldToList(string field)
    {
        if (IsValidField(field) && !_SelectedFields.Contains(field))
            _SelectedFields.Add(field);
    }

    private bool IsValidField(string field)
    {
        if (field != null)
        {
            string cleanField = field.Trim().ToLowerInvariant();
            foreach (string key in _ProductFields.Keys)
            {
                if (key.ToLowerInvariant() == cleanField) return true;
            }
        }
        return false;
    }

    private List<Control> DrawCheckField(int productId, string field, bool initialChecked)
    {
        List<Control> controls = new List<Control>();
        string checkedValue = initialChecked ? " checked" : string.Empty;
        controls.Add(new LiteralControl(string.Format("<input type=\"checkbox\" name=\"P{0}_{1}\" value=\"1\"{2}>\n", productId, field, checkedValue)));
        return controls;
    }

    private List<Control> DrawTextField(int productId, string field, string initialValue, int maxlength, int width, bool required)
    {
        List<Control> controls = new List<Control>();
        controls.Add(new LiteralControl(string.Format("<input type=\"text\" name=\"P{0}_{1}\" value=\"{2}\" maxlength=\"{3}\" style=\"width:{4}px\">\n", productId, field, Server.HtmlEncode(initialValue), maxlength, width)));
        return controls;
    }

    private List<Control> DrawLongTextField(int productId, string field, string initialValue, int width)
    {
        List<Control> controls = new List<Control>();
        string fieldId = string.Format("P{0}_{1}", productId, field);
        controls.Add(new LiteralControl(string.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" style=\"width:{2}px\"><input type=\"button\" value=\"  ..  \" onclick=\"ShowEditLongTextDialog('{0}');return false;\">\n", fieldId, Server.HtmlEncode(initialValue), width)));
        return controls;
    }

    private List<Control> DrawHtmlField(int productId, string field, string initialValue, int width)
    {
        List<Control> controls = new List<Control>();
        string fieldId = string.Format("P{0}_{1}", productId, field);
        controls.Add(new LiteralControl(string.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" style=\"width:{2}px\"><input type=\"button\" value=\"  ..  \" onclick=\"ShowEditHtmlDialog('{0}');return false;\">\n", fieldId, Server.HtmlEncode(initialValue), width)));
        return controls;
    }

    private List<Control> DrawNumberField(int productId, string field, string initialValue, int maxlength, int width, bool required)
    {
        List<Control> controls = new List<Control>();
        controls.Add(new LiteralControl(string.Format("<input type=\"text\" name=\"P{0}_{1}\" value=\"{2}\" maxlength=\"{3}\" style=\"width:{4}px\">\n", productId, field, Server.HtmlEncode(initialValue), maxlength, width)));
        return controls;
    }

    private List<Control> DrawDropDownField(int productId, string field, List<ListItem> items, string selectedValue, bool showHeader)
    {
        return DrawDropDownField(productId, field, items, selectedValue, showHeader, string.Empty);
    }

    private List<Control> DrawDropDownField(int productId, string field, List<ListItem> items, string selectedValue, bool showHeader, string headerText)
    {
        // BUILD THE SELECT BOX
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format("<select name=\"P{0}_{1}\">\n", productId, field));
        if (showHeader) sb.Append("<option value=\"\">" + headerText + "</option>\n");
        foreach (ListItem item in items)
        {
            sb.Append("<option value=\"" + Server.HtmlEncode(item.Value) + "\"");
            if (item.Value == selectedValue) sb.Append(" selected");
            sb.Append(">" + Server.HtmlEncode(item.Text) + "</option>\n");
        }
        sb.Append("</select>\n");

        // CREATE AND RETURN THE LITERAL CONTROL
        List<Control> controls = new List<Control>();
        controls.Add(new LiteralControl(sb.ToString()));
        return controls;
    }

    private List<Control> DrawField(Product product, string field, out string cellAlignment)
    {
        cellAlignment = string.Empty;
        if (field != null)
        {
            string cleanField = field.Trim().ToLowerInvariant();
            switch (cleanField)
            {
                case "allowbackorder":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.AllowBackorder);
                case "allowreviews":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.AllowReviews);
                case "costofgoods":
                    return DrawNumberField(product.ProductId, cleanField, product.CostOfGoods.ToString("F2"), 10, 50, false);
                case "customurl":
                    return DrawTextField(product.ProductId, cleanField, product.CustomUrl, 150, 150, false);
                case "description":
                    return DrawHtmlField(product.ProductId, cleanField, product.Description, 200);
                case "disablepurchase":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.DisablePurchase);
                case "displaypage":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.DisplayPage, true, "Inherit");
                case "excludefromfeed":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.ExcludeFromFeed);
                case "extendeddescription":
                    return DrawHtmlField(product.ProductId, cleanField, product.ExtendedDescription, 200);
                case "height":
                    return DrawNumberField(product.ProductId, cleanField, product.Height.ToString("F2"), 10, 50, false);
                case "hideprice":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.HidePrice);
                case "htmlhead":
                    return DrawLongTextField(product.ProductId, cleanField, product.HtmlHead, 200);
                case "iconalttext":
                    return DrawTextField(product.ProductId, cleanField, product.IconAltText, 255, 200, false);
                case "iconurl":
                    return DrawTextField(product.ProductId, cleanField, product.IconUrl, 255, 200, false);
                case "imagealttext":
                    return DrawTextField(product.ProductId, cleanField, product.ImageAltText, 255, 200, false);
                case "imageurl":
                    return DrawTextField(product.ProductId, cleanField, product.ImageUrl, 255, 200, false);
                case "instock":
                    return DrawNumberField(product.ProductId, cleanField, product.InStock.ToString(), 10, 50, false);
                case "instockwarninglevel":
                    return DrawNumberField(product.ProductId, cleanField, product.InStockWarningLevel.ToString(), 10, 50, false);
                case "inventorymodeid":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.InventoryModeId.ToString(), false);
                case "isfeatured":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.IsFeatured);
                case "isgiftcertificate":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.IsGiftCertificate);
                case "isprohibited":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.IsProhibited);
                case "length":
                    return DrawNumberField(product.ProductId, cleanField, product.Length.ToString("F2"), 10, 50, false);
                case "manufacturerid":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.ManufacturerId.ToString(), true);
                case "maximumprice":
                    return DrawNumberField(product.ProductId, cleanField, product.MaximumPrice.ToString("F2"), 10, 50, false);
                case "maxquantity":
                    return DrawNumberField(product.ProductId, cleanField, product.MaxQuantity.ToString(), 10, 50, false);
                case "minimumprice":
                    return DrawNumberField(product.ProductId, cleanField, product.MinimumPrice.ToString("F2"), 10, 50, false);
                case "minquantity":
                    return DrawNumberField(product.ProductId, cleanField, product.MinQuantity.ToString(), 10, 50, false);
                case "modelnumber":
                    return DrawTextField(product.ProductId, cleanField, product.ModelNumber, 40, 80, false);
                case "msrp":
                    return DrawNumberField(product.ProductId, cleanField, product.MSRP.ToString("F2"), 10, 50, false);
                case "name":
                    return DrawTextField(product.ProductId, cleanField, product.Name, 255, 200, true);
                case "price":
                    return DrawNumberField(product.ProductId, cleanField, product.Price.ToString("F2"), 10, 50, false);
                case "searchkeywords":
                    return DrawLongTextField(product.ProductId, cleanField, product.SearchKeywords, 200);
                case "shippableid":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.ShippableId.ToString(), false);
                case "sku":
                    return DrawTextField(product.ProductId, cleanField, product.Sku, 40, 80, false);
                case "summary":
                    return DrawLongTextField(product.ProductId, cleanField, product.Summary, 200);
                case "taxcodeid":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.TaxCodeId.ToString(), true);
                case "theme":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.Theme, true, "Store Default");
                case "thumbnailalttext":
                    return DrawTextField(product.ProductId, cleanField, product.ThumbnailAltText, 255, 200, false);
                case "thumbnailurl":
                    return DrawTextField(product.ProductId, cleanField, product.ThumbnailUrl, 255, 200, false);
                case "usevariableprice":
                    cellAlignment = "center";
                    return DrawCheckField(product.ProductId, cleanField, product.UseVariablePrice);
                case "vendorid":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.VendorId.ToString(), true);
                case "visibilityid":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.VisibilityId.ToString(), false);
                case "warehouseid":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.WarehouseId.ToString(), false);
                case "weight":
                    return DrawNumberField(product.ProductId, cleanField, product.Weight.ToString("F2"), 10, 50, false);
                case "width":
                    return DrawNumberField(product.ProductId, cleanField, product.Width.ToString("F2"), 10, 50, false);
                case "wrapgroupid":
                    return DrawDropDownField(product.ProductId, cleanField, GetListItems(product, cleanField), product.WrapGroupId.ToString(), true);
            }
        }
        return null;
    }

    private Dictionary<string, List<ListItem>> _ListItemCache = new Dictionary<string, List<ListItem>>();
    private List<ListItem> GetListItems(Product product, string field)
    {
        if (!_ListItemCache.ContainsKey(field))
        {
            // LIST ITEMS NOT CACHED, CONSTRUCT THEM
            // SET THE CACHE FLAG TO FALSE IF THE OPTIONS DEPEND ON THE PRODUCT
            bool enableCache = true;
            List<ListItem> listItems = new List<ListItem>();
            switch (field)
            {
                case "displaypage":
                    List<DisplayPage> displayPages = DisplayPageDataSource.Load();
                    foreach (DisplayPage displayPage in displayPages)
                    {
                        string displayName = string.Format("{0} ({1})", displayPage.Name, displayPage.DisplayPageFile);
                        if (displayPage.NodeType == CatalogNodeType.Product)
                            listItems.Add(new ListItem(displayName, displayPage.DisplayPageFile));
                    }
                    break;
                case "inventorymodeid":
                    enableCache = false;
                    listItems.Add(new ListItem("Disabled", "0"));
                    if (Token.Instance.Store.EnableInventory)
                    {
                        listItems.Add(new ListItem("Track Product", "1"));
                        if (product.Variants.Count > 0)
                        {
                            listItems.Add(new ListItem("Track Variants", "2"));
                        }
                    }
                    break;
                case "manufacturerid":
                    ManufacturerCollection manufacturers = ManufacturerDataSource.LoadForStore("Name");
                    foreach (Manufacturer manufacturer in manufacturers)
                    {
                        listItems.Add(new ListItem(manufacturer.Name, manufacturer.ManufacturerId.ToString()));
                    }
                    break;
                case "shippableid":
                    listItems.Add(new ListItem("No", "0"));
                    listItems.Add(new ListItem("Yes", "1"));
                    listItems.Add(new ListItem("Separately", "2"));
                    break;
                case "taxcodeid":
                    TaxCodeCollection taxCodes = TaxCodeDataSource.LoadForStore("Name");
                    foreach (TaxCode taxCode in taxCodes)
                    {
                        listItems.Add(new ListItem(taxCode.Name, taxCode.TaxCodeId.ToString()));
                    }
                    break;
                case "theme":
                    List<Theme> themes = ThemeDataSource.Load();
                    foreach (Theme theme in themes)
                    {
                        if (!theme.IsAdminTheme)
                        {
                            listItems.Add(new ListItem(theme.DisplayName, theme.Name));
                        }
                    }
                    break;
                case "vendorid":
                    VendorCollection vendors = VendorDataSource.LoadForStore("Name");
                    foreach (Vendor vendor in vendors)
                    {
                        listItems.Add(new ListItem(vendor.Name, vendor.VendorId.ToString()));
                    }
                    break;
                case "visibilityid":
                    listItems.Add(new ListItem("Public", "0"));
                    listItems.Add(new ListItem("Hidden", "1"));
                    listItems.Add(new ListItem("Private", "2"));
                    break;
                case "warehouseid":
                    WarehouseCollection warehouses = WarehouseDataSource.LoadForStore("Name");
                    foreach (Warehouse warehouse in warehouses)
                    {
                        listItems.Add(new ListItem(warehouse.Name, warehouse.WarehouseId.ToString()));
                    }
                    break;
                case "wrapgroupid":
                    WrapGroupCollection wrapGroups = WrapGroupDataSource.LoadForStore("Name");
                    foreach (WrapGroup wrapGroup in wrapGroups)
                    {
                        listItems.Add(new ListItem(wrapGroup.Name, wrapGroup.WrapGroupId.ToString()));
                    }
                    break;
            }
            if (!enableCache) return listItems;
            _ListItemCache[field] = listItems;
        }
        return _ListItemCache[field];
    }

    private bool IsValidListItemValue(Product product, string field, string value, bool emptyIsValid)
    {
        if (string.IsNullOrEmpty(value)) return emptyIsValid;
        List<ListItem> validItems = GetListItems(product, field);
        foreach (ListItem item in validItems)
        {
            if (item.Value == value) return true;
        }
        return false;
    }

    private void SaveGrid()
    {
        // SAVE THE GRID
        foreach (Product product in _SelectedProducts)
        {
            // SEE IF WE CAN LOCATE THIS PRODUCT
            if (AlwaysConvert.ToInt(Request.Form["P" + product.ProductId]) == 1)
            {
                // THE PRODUCT WAS PRESENT IN THE GRID
                foreach (string field in _SelectedFields)
                {
                    UpdateField(product, field);
                }
            }
            product.Save();
        }
        SavedMessagePanel.Visible = true;
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
    }

    private void UpdateField(Product product, string field)
    {
        if (field != null)
        {
            string cleanField = field.Trim().ToLowerInvariant();
            string fieldValue = GetValueFromFormPost(product.ProductId, cleanField);
            switch (cleanField)
            {
                case "allowbackorder":
                    product.AllowBackorder = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "allowreviews":
                    product.AllowReviews = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "costofgoods":
                    product.CostOfGoods = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "customurl":
                    bool isValid = false;
                    string oldCustomUrl = product.CustomUrl;
                    string newCustomUrl = fieldValue.Trim();
                    if (!string.IsNullOrEmpty(newCustomUrl) && string.IsNullOrEmpty(oldCustomUrl) || oldCustomUrl != newCustomUrl)
                        isValid = !CustomUrlDataSource.IsAlreadyUsed(newCustomUrl);
                    else
                        isValid = true;
                    if (isValid)
                        product.CustomUrl = fieldValue;
                    break;
                case "description":
                    product.Description = fieldValue;
                    break;
                case "disablepurchase":
                    product.DisablePurchase = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "displaypage":
                    if (IsValidListItemValue(product, field, fieldValue, true)) product.DisplayPage = fieldValue;
                    break;
                case "excludefromfeed":
                    product.ExcludeFromFeed = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "extendeddescription":
                    product.ExtendedDescription = fieldValue;
                    break;
                case "height":
                    product.Height = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "hideprice":
                    product.HidePrice = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "htmlhead":
                    product.HtmlHead = fieldValue;
                    break;
                case "iconalttext":
                    product.IconAltText = fieldValue;
                    break;
                case "iconurl":
                    product.IconUrl = fieldValue;
                    break;
                case "imagealttext":
                    product.ImageAltText = fieldValue;
                    break;
                case "imageurl":
                    product.ImageUrl = fieldValue;
                    break;
                case "instock":
                    product.InStock = AlwaysConvert.ToInt(fieldValue);
                    break;
                case "instockwarninglevel":
                    product.InStockWarningLevel = AlwaysConvert.ToInt(fieldValue);
                    break;
                case "inventorymodeid":
                    if (IsValidListItemValue(product, field, fieldValue, false)) product.InventoryModeId = AlwaysConvert.ToByte(fieldValue);
                    break;
                case "isfeatured":
                    product.IsFeatured = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "isgiftcertificate":
                    product.IsGiftCertificate = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "isprohibited":
                    product.IsProhibited = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "length":
                    product.Length = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "manufacturerid":
                    if (IsValidListItemValue(product, field, fieldValue, true)) product.ManufacturerId = AlwaysConvert.ToInt(fieldValue);
                    break;
                case "maximumprice":
                    product.MaximumPrice = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "maxquantity":
                    product.MaxQuantity = AlwaysConvert.ToInt16(fieldValue);
                    break;
                case "minimumprice":
                    product.MinimumPrice = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "minquantity":
                    product.MinQuantity = AlwaysConvert.ToInt16(fieldValue);
                    break;
                case "modelnumber":
                    product.ModelNumber = fieldValue;
                    break;
                case "msrp":
                    product.MSRP = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "name":
                    if (!string.IsNullOrEmpty(fieldValue)) product.Name = fieldValue;
                    break;
                case "price":
                    product.Price = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "searchkeywords":
                    product.SearchKeywords = fieldValue;
                    break;
                case "shippableid":
                    if (IsValidListItemValue(product, field, fieldValue, false)) product.ShippableId = AlwaysConvert.ToByte(fieldValue);
                    break;
                case "sku":
                    product.Sku = fieldValue;
                    break;
                case "summary":
                    product.Summary = fieldValue;
                    break;
                case "taxcodeid":
                    if (IsValidListItemValue(product, field, fieldValue, true)) product.TaxCodeId = AlwaysConvert.ToInt(fieldValue);
                    break;
                case "theme":
                    if (IsValidListItemValue(product, field, fieldValue, true)) product.Theme = fieldValue;
                    break;
                case "thumbnailalttext":
                    product.ThumbnailAltText = fieldValue;
                    break;
                case "thumbnailurl":
                    product.ThumbnailUrl = fieldValue;
                    break;
                case "usevariableprice":
                    product.UseVariablePrice = (AlwaysConvert.ToInt(fieldValue) == 1);
                    break;
                case "vendorid":
                    if (IsValidListItemValue(product, field, fieldValue, true)) product.VendorId = AlwaysConvert.ToInt(fieldValue);
                    break;
                case "visibilityid":
                    if (IsValidListItemValue(product, field, fieldValue, false)) product.VisibilityId = AlwaysConvert.ToByte(fieldValue);
                    break;
                case "warehouseid":
                    if (IsValidListItemValue(product, field, fieldValue, false)) product.WarehouseId = AlwaysConvert.ToInt(fieldValue);
                    break;
                case "weight":
                    product.Weight = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "width":
                    product.Width = AlwaysConvert.ToDecimal(fieldValue);
                    break;
                case "wrapgroupid":
                    if (IsValidListItemValue(product, field, fieldValue, true)) product.WrapGroupId = AlwaysConvert.ToInt(fieldValue);
                    break;
            }
        }
    }

    private string GetValueFromFormPost(int productId, string field)
    {
        string tempValue = Request.Form["P" + productId + "_" + field];
        if (tempValue == null) return string.Empty;
        return tempValue.Trim();
    }


    #region CustomViewState
    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(EncryptionHelper.DecryptAES(Request.Form[VS.UniqueID]));
            _ShowEditGrid = AlwaysConvert.ToBool(customViewState.TryGetValue("ShowEditGrid"), false);
            _EnableScrolling = AlwaysConvert.ToBool(customViewState.TryGetValue("EnableScrolling"), true);
            string selectedProducts = customViewState.TryGetValue("SP");
            if (!string.IsNullOrEmpty(selectedProducts))
            {
                int[] tempProductIds = AlwaysConvert.ToIntArray(selectedProducts);
                if (tempProductIds != null && tempProductIds.Length > 0)
                {
                    for (int i = 0; i < tempProductIds.Length; i++)
                    {
                        AddProductToList(tempProductIds[i]);
                    }
                }
            }
            BuildFieldList(customViewState.TryGetValue("SF"));
        }
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState["ShowEditGrid"] = _ShowEditGrid.ToString();
        customViewState["EnableScrolling"] = _EnableScrolling.ToString();
        if (_SelectedProductIds.Count > 0)
            customViewState["SP"] = AlwaysConvert.ToList(",", _SelectedProductIds.ToArray());
        else customViewState["SP"] = string.Empty;
        if (_SelectedFields.Count > 0)
            customViewState["SF"] = string.Join(",", _SelectedFields.ToArray());
        else customViewState["SF"] = string.Empty;
        VS.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }

    #endregion

    #region HtmlEditor
    protected void HtmlEditorPageInit()
    {
        //SET THEME FOR EDITOR
        Editor.BasePath = this.Page.ResolveUrl("~/FCKeditor/");
        Editor.EditorAreaCSS = this.EditorAreaCSS;
        Editor.ToolbarSet = "Modal";
        Session["FCKEditor:UserFilesPath"] = this.Page.ResolveUrl("~/Assets");
    }

    private string GetStoreTheme()
    {
        string theme = Token.Instance.Store.Settings.StoreTheme;
        if (!string.IsNullOrEmpty(theme)) return theme;
        return "AbleCommerce";
    }

    protected string EditorAreaCSS
    {
        get
        {
            string theme = GetStoreTheme();
            return this.Page.ResolveUrl("~/App_Themes/" + theme + "/style.css");
        }
    }
    #endregion
}