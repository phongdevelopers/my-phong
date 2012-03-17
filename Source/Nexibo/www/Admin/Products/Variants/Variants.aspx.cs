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
using CommerceBuilder.Common;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

public partial class Admin_Products_Variants_Variants : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _ProductId;
    private Product _Product;
    private ProductVariantManager _VariantManager;
    private bool _ShowInventory;
    private string _DisplayRangeLabelText;

    //stores data from form post
    Dictionary<string, string> _FormValues = new Dictionary<string, string>();

    //hold the last displayed datasource
    PersistentCollection<ProductVariant> _DisplayedVariants;

    //stores the page we are currently displaying
    private const int _PageSize = 40;
    private int _CurrentPage;
    private int _VariantCount;
    private int _TotalPages;
    private int _WholePages;
    private int _LeftOverItems;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _Product = ProductDataSource.Load(_ProductId);
        _ShowInventory = (_Product.InventoryMode == InventoryMode.Variant && Token.Instance.Store.EnableInventory);
        if (_Product == null) Response.Redirect("../../Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId());
        _VariantManager = new ProductVariantManager(_ProductId);
        if (_VariantManager.Count == 0) Response.Redirect("Options.aspx?ProductId=" + _ProductId.ToString());
        if (_Product.ProductOptions.Count > ProductVariant.MAXIMUM_ATTRIBUTES)
        {
            TooManyVariantsPanel.Visible = true;
            TooManyVariantsMessage.Text = string.Format(TooManyVariantsMessage.Text, _Product.Name, ProductVariant.MAXIMUM_ATTRIBUTES);
        }
        
        //FIGURE OUT PAGING SETTINGS
        _VariantCount = _VariantManager.CountVariantGrid();
        _WholePages = _VariantCount / _PageSize;
        _LeftOverItems = _VariantCount % _PageSize;
        if (_LeftOverItems > 0) _TotalPages = _WholePages + 1;
        else _TotalPages = _WholePages;
        PagerPanel.Visible = (_TotalPages > 1);
        //LOAD VIEW STATE
        LoadCustomViewState();
        //SAVE THIS TEXT FOR LATER
        _DisplayRangeLabelText = DisplayRangeLabel.Text;
        //CONVERT VARIANT FORM DATA TO STRING DICTIONARY
        ParseFormData();
        //INITIALIZE THE GRID
        BindVariantGrid();
        //UPDATE THE CAPTION
        Caption.Text = string.Format(Caption.Text, _Product.Name);
    }

    /// <summary>
    /// This is a helper function to parse the variant form data into a string dictionary
    /// so we can support the ContainsKey method
    /// </summary>
    private void ParseFormData()
    {
        _FormValues.Clear();
        foreach (string key in Request.Form.Keys)
        {
            if (key != null && key.StartsWith("V_"))
            {
                _FormValues.Add(key, Request.Form[key]);
            }
        }
    }

    private void LoadCustomViewState()
    {
        if (Page.IsPostBack)
        {
            UrlEncodedDictionary customViewState = new UrlEncodedDictionary(EncryptionHelper.DecryptAES(Request.Form[VS_CustomState.UniqueID]));
            _CurrentPage = AlwaysConvert.ToInt(customViewState.TryGetValue("CurrentPage"));
            if (_CurrentPage < 1) _CurrentPage = 1;
        }
    }

    private string SafeTryGetValue(Dictionary<string, string> dic, string key)
    {
        if (dic.ContainsKey(key)) return dic[key];
        return string.Empty;
    }

    protected void VariantGrid_ItemCreated(object sender , RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Header)
        {
            if (_ShowInventory)
            {
                PlaceHolder phInventoryColumns = e.Item.FindControl("phInventoryColumns") as PlaceHolder;
                if (phInventoryColumns != null)
                {
                    phInventoryColumns.Controls.Add(new LiteralControl("<th>In Stock</th><th>Low Stock</th>"));
                }
            }
        }
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
		    PlaceHolder phVariantRow = e.Item.FindControl("phVariantRow") as PlaceHolder;
            if (phVariantRow != null)
            {
                ProductVariant variant = (ProductVariant)e.Item.DataItem;
                int variantIndex = _VariantManager.IndexOf(variant);
                StringBuilder rowBuilder = new StringBuilder();
                string skuKey = "V_" + variantIndex.ToString() + "S";
                string priceKey = "V_" + variantIndex.ToString() + "P";
                string priceModKey = "V_" + variantIndex.ToString() + "PM";
                string weightKey = "V_" + variantIndex.ToString() + "W";
                string weightModKey = "V_" + variantIndex.ToString() + "WM";
                string cogsKey = "V_" + variantIndex.ToString() + "C";
                string availKey = "V_" + variantIndex.ToString() + "A";
                string instockKey = "V_" + variantIndex.ToString() + "I";
                string restockKey = "V_" + variantIndex.ToString() + "R";
                //CHECK IF THIS VARIANT IS IN FORM POST
                if (_FormValues.ContainsKey(skuKey))
                {
                    //UPDATE THE VALUES BASED ON THE FORM POST
                    variant.Sku = SafeTryGetValue(_FormValues, skuKey);
                    variant.Price = AlwaysConvert.ToDecimal(SafeTryGetValue(_FormValues, priceKey));
                    variant.PriceModeId = AlwaysConvert.ToByte(SafeTryGetValue(_FormValues, priceModKey));
                    variant.Weight = AlwaysConvert.ToDecimal(SafeTryGetValue(_FormValues, weightKey));
                    variant.WeightModeId = AlwaysConvert.ToByte(SafeTryGetValue(_FormValues, weightModKey));
                    variant.CostOfGoods = AlwaysConvert.ToDecimal(SafeTryGetValue(_FormValues, cogsKey));
                    variant.Available = (SafeTryGetValue(_FormValues, availKey) == "1");
                    if (_ShowInventory)
                    {
                        variant.InStock = AlwaysConvert.ToInt(SafeTryGetValue(_FormValues, instockKey));
                        variant.InStockWarningLevel = AlwaysConvert.ToInt(SafeTryGetValue(_FormValues, restockKey));
                    }
                }
                //DRAW THE DATA ROW
                string rowClass = (e.Item.ItemType == ListItemType.Item) ? "oddRow" : "evenRow";
                rowBuilder.Append("<tr class=\"" + rowClass + "\">\r\n");
                rowBuilder.Append("<td>" + (variantIndex + 1) + "</td>\r\n");
                rowBuilder.Append("<td>" + variant.VariantName + "</td>\r\n");
                rowBuilder.Append("<td align=\"center\"><input name=\"" + skuKey + "\" type=\"text\" value=\"" + variant.Sku + "\" style=\"width:100px;\" /></td>\r\n");
                rowBuilder.Append("<td align=\"center\"><input name=\"" + priceKey + "\" type=\"text\" value=\"" + variant.Price.ToString("F2") + "\" style=\"width:50px;\" />");
                string overrideSelected = (variant.PriceMode == ModifierMode.Modify) ? string.Empty : " selected";
                rowBuilder.Append("<select name=\"" + priceModKey + "\"><option value=\"0\">Modify</option><option value=\"1\"" + overrideSelected + ">Override</option></select></td>\r\n");
                rowBuilder.Append("<td align=\"center\"><input name=\"" + weightKey + "\" type=\"text\" value=\"" + variant.Weight.ToString("F2") + "\" style=\"width:50px;\" />");
                overrideSelected = (variant.WeightMode == ModifierMode.Modify) ? string.Empty : " selected";
                rowBuilder.Append("<select name=\"" + weightModKey + "\"><option value=\"0\">Modify</option><option value=\"1\"" + overrideSelected + ">Override</option></select></td>\r\n");
                rowBuilder.Append("<td align=\"center\"><input name=\"" + cogsKey + "\" type=\"text\" value=\"" + variant.CostOfGoods.ToString("F2") + "\" style=\"width:50px;\" /></td>\r\n");
                string checkedAttribute = string.Empty;
                if (variant.Available) checkedAttribute = " checked=\"true\"";
                rowBuilder.Append("<td align=\"center\"><input name=\"" + availKey + "\" type=\"checkbox\"" + checkedAttribute + " value=\"1\" /></td>");
                if (_ShowInventory)
                {
                    rowBuilder.Append("<td align=\"center\"><input name=\"" + instockKey + "\" type=\"text\" value=\"" + variant.InStock.ToString("F0") + "\" style=\"width:50px;\" /></td>\r\n");
                    rowBuilder.Append("<td align=\"center\"><input name=\"" + restockKey + "\" type=\"text\" value=\"" + variant.InStockWarningLevel.ToString("F0") + "\" style=\"width:50px;\" /></td>\r\n");
                }
                rowBuilder.Append("</tr>\r\n");
                phVariantRow.Controls.Add(new LiteralControl(rowBuilder.ToString()));
            }
        }
    }

    protected void ChangePage(object sender, EventArgs e)
    {
        string WhichSender = sender.ToString();
        // If display criteria has changed or user has clicked "Update Display" button, display first page with selected criteria 
        // If user has clicked navigation link (first,previous,next,last) or selected page to jump to, display selected page 
        if (WhichSender == "System.Web.UI.WebControls.LinkButton")
        {
            SaveGrid();
            LinkButton lb = (LinkButton)sender;
            _CurrentPage = AlwaysConvert.ToInt(lb.CommandArgument);
            BindVariantGrid();
        }
        else
        {
            SaveGrid();
            DropDownList ddl = (DropDownList)sender;
            _CurrentPage = AlwaysConvert.ToInt(ddl.SelectedItem.Text);
            BindVariantGrid();
        }
    }

    private void BindVariantGrid()
    {
        //VALIDATE CURRENT PAGE IS IN RANGE
        if (_CurrentPage > _TotalPages | _CurrentPage < 1)
        {
            //RESET TO THE FIRST PAGE
            _CurrentPage = 1;
        }

        //CHECK IF LAST PAGE
        if (_CurrentPage == _TotalPages)
        {
            //HIDE NEXT AND LAST LINK FOR LAST PAGE
            NextLink.Visible = false;
            LastLink.Visible = false;
        }
        else
        {
            //SHOW NEXT AND LAST LINK FOR THIS PAGE
            NextLink.Visible = true;
            LastLink.Visible = true;
            NextLink.CommandArgument = ((int)(_CurrentPage + 1)).ToString();
            LastLink.CommandArgument = _TotalPages.ToString();
        }

        if (_CurrentPage == 1)
        {
            //HIDE PREVIOUS AND FIRST LINK FOR FIRST PAGE
            PreviousLink.Visible = false;
            FirstLink.Visible = false;
        }
        else
        {
            //SHOW PREVIOUS AND NEXT LINK FOR THIS PAGE
            PreviousLink.Visible = true;
            FirstLink.Visible = true;
            PreviousLink.CommandArgument = ((int)(_CurrentPage - 1)).ToString();
            FirstLink.CommandArgument = "1";
        }

        //BUILD PAGE LIST
        JumpPage.Items.Clear();
        if (_TotalPages <= 100)
        {
            //ADD ALL PAGES TO LIST
            for (int x = 1; x <= _TotalPages; x++)
            {
                JumpPage.Items.Add(x.ToString());
            }
            JumpPage.SelectedIndex = (_CurrentPage - 1);
        }
        else
        {
            //DISPLAY ONLY ONE HUNDRED PAGES
            int startPage, endPage;
            if (_CurrentPage < 51)
            {
                //SHOW FIRST HUNDRED PAGES
                startPage = 1;
                endPage = 100;
            }
            else if (_CurrentPage > _TotalPages - 100)
            {
                //SHOW LAST HUNDRED PAGES
                startPage = _TotalPages - 100;
                endPage = _TotalPages;
            }
            else
            {
                //SHOW RANGE OF HUNDRED
                startPage = _CurrentPage - 50;
                endPage = _CurrentPage + 50;
            }
            for (int i = startPage; i <= endPage; i++)
            {
                JumpPage.Items.Add(i.ToString());
            }
            JumpPage.SelectedIndex = (_CurrentPage - startPage);
        }

        // Set the record count and page count text 
        PageCountLabel.Text = _TotalPages.ToString();

        // Determine the starting and ending index in the IDList ArrayList given the current page 
        int startIndex = _PageSize * (_CurrentPage - 1);
        int endIndex = Math.Min((_PageSize * (_CurrentPage - 1)) + (_PageSize - 1), ((_WholePages * _PageSize) + _LeftOverItems - 1));

        DisplayRangeLabel.Text = string.Format(_DisplayRangeLabelText, (startIndex + 1), (endIndex + 1), _VariantCount);

        //BIND THE REPEATER HERE
        _DisplayedVariants = _VariantManager.LoadVariantGrid(_PageSize, startIndex);
        VariantGrid.DataSource = _DisplayedVariants;
        VariantGrid.DataBind();
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        SaveGrid();
        SavedMessage.Text = string.Format(SavedMessage.Text, DateTime.Now);
        SavedMessage.Visible = true;
    }

    protected void EditOptionsButton_Click(object sender, EventArgs e)
    {
        SaveGrid();
        Response.Redirect("Options.aspx?ProductId=" + _ProductId.ToString());
    }

    private void SaveGrid()
    {
        //SAVES THE LAST VARIANT GRID DISPLAYED
        //THIS GRID WOULD HAVE BEEN UPDATED WITH FORM POST VALUES DURING THE DATABIND
        _DisplayedVariants.Save();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        SaveCustomViewState();
    }

    private void SaveCustomViewState()
    {
        UrlEncodedDictionary customViewState = new UrlEncodedDictionary();
        customViewState.Add("CurrentPage", _CurrentPage.ToString());
        VS_CustomState.Value = EncryptionHelper.EncryptAES(customViewState.ToString());
    }


}