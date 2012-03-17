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
using CommerceBuilder.Catalog;
using CommerceBuilder.Products;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Utility;
using System.IO;

public partial class Admin_Products_DigitalGoods_DigitalGoods : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private Category _Category;
    private int _CategoryId;
    private int _ProductId;
    private Product _Product;
    private ProductVariantManager _VariantManager;
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

    protected int CategoryId
    {
        get { return _CategoryId; }
    }

    protected int ProductId
    {
        get { return _ProductId; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + _CategoryId.ToString());
        _VariantManager = new ProductVariantManager(_ProductId);
        //BIND GOODS FOR PRODUCT
        BindDigitalGoodsGrid();
        //UPDATE ATTACH BUTTON
        AllVariants.NavigateUrl += _ProductId.ToString();
        //FIGURE OUT PAGING SETTINGS
        _VariantCount = _VariantManager.CountVariantGrid();
        if (_VariantCount > 0)
        {
            _WholePages = _VariantCount / _PageSize;
            _LeftOverItems = _VariantCount % _PageSize;
            if (_LeftOverItems > 0) _TotalPages = _WholePages + 1;
            else _TotalPages = _WholePages;
            if (_TotalPages == 1) VariantPager.Visible = false;
            //LOAD VIEW STATE
            LoadCustomViewState();
            //SAVE THIS TEXT FOR LATER
            _DisplayRangeLabelText = DisplayRangeLabel.Text;
            //INITIALIZE THE GRID
            BindVariantGrid();
        }
        else
        {
            VariantGoodsPanel.Visible = false;
        }
        //UPDATE THE CAPTION
        Caption.Text = string.Format(Caption.Text, _Product.Name);
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

    protected void BindDigitalGoodsGrid()
    {
        DigitalGoodsGrid.DataSource = ProductDigitalGoodDataSource.LoadForVariant(this.ProductId, string.Empty);
        DigitalGoodsGrid.DataBind();
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

    protected void VariantGrid_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            PlaceHolder phVariantRow = e.Item.FindControl("phVariantRow") as PlaceHolder;
            if (phVariantRow != null)
            {
                ProductVariant variant = (ProductVariant)e.Item.DataItem;
                int variantIndex = _VariantManager.IndexOf(variant);
                string optionList = variant.OptionList;
                StringBuilder rowBuilder = new StringBuilder();
                //DRAW THE DATA ROW
                string rowClass = (e.Item.ItemType == ListItemType.Item) ? "oddRow" : "evenRow";
                rowBuilder.Append("<tr class=\"" + rowClass + "\">\r\n");
                rowBuilder.Append("<td>" + (variantIndex + 1) + "</td>\r\n");
                rowBuilder.Append("<td>" + variant.VariantName + "</td>\r\n");
                rowBuilder.Append("<td>");
                //BUILD TABLE OF EXISTING DIGITAL GOODS
                ProductDigitalGoodCollection variantDigitalGoods = ProductDigitalGoodDataSource.LoadForVariant(this.ProductId, optionList);
                if (variantDigitalGoods.Count > 0)
                {
                    phVariantRow.Controls.Add(new LiteralControl(rowBuilder.ToString()));
                    foreach (ProductDigitalGood pdg in variantDigitalGoods)
                    {
                        string digitalGoodName = pdg.DigitalGood.Name;
                        string linkedName = string.Format("<a href=\"../../DigitalGoods/EditDigitalGood.aspx?CategoryId={0}&ProductId={1}&DigitalGoodId={2}\">" + digitalGoodName + "</a>",CategoryId,ProductId,pdg.DigitalGoodId);
                        phVariantRow.Controls.Add(new LiteralControl("<b>" + linkedName + "</b>&nbsp;&nbsp;" + GetFileSize(pdg.DigitalGood) + "&nbsp;&nbsp;"));
                        HyperLink downloadLink = new HyperLink();
                        downloadLink.NavigateUrl = "../../DigitalGoods/Download.ashx?DigitalGoodId=" + pdg.DigitalGoodId;
                        downloadLink.EnableViewState = false;
                        Image downloadIcon = new Image();
                        downloadIcon.AlternateText = "Download";
                        downloadIcon.ToolTip = "Download";
                        downloadIcon.SkinID = "DownloadIcon";
                        downloadIcon.EnableViewState = false;
                        downloadLink.Controls.Add(downloadIcon);
                        phVariantRow.Controls.Add(downloadLink);
                        ImageButton detachButton = new ImageButton();
                        detachButton.ID = "D" + pdg.ProductDigitalGoodId.ToString();
                        detachButton.AlternateText = "Delete";
                        detachButton.ToolTip = "Delete";
                        detachButton.SkinID = "DeleteIcon";
                        detachButton.Click += new ImageClickEventHandler(detachButton_Click);
                        detachButton.OnClientClick = string.Format("return confirm(\"Are you sure you want to remove {0} from this product?\")", digitalGoodName);
                        detachButton.EnableViewState = false;
                        phVariantRow.Controls.Add(detachButton);
                        phVariantRow.Controls.Add(new LiteralControl("<br />"));
                    }
                    rowBuilder = new StringBuilder();
                }
                rowBuilder.Append("<a href=\"AttachDigitalGood.aspx?ProductId=" + this.ProductId + "&Options=" + Server.UrlEncode(EncryptionHelper.EncryptAES(optionList)) + "\">Attach Digital Good</a>");
                rowBuilder.Append("</td>\r\n");
                /*
                rowBuilder.Append("<td>");
                rowBuilder.Append("<a href=\"AttachDigitalGood.aspx?ProductId=" + this.ProductId + "&Options=" + Server.UrlEncode(EncryptionHelper.EncryptAES(optionList)) + "\" Title=\"Attach Digital Good\">");
                phVariantRow.Controls.Add(new LiteralControl(rowBuilder.ToString()));
                Image addIcon = new Image();
                addIcon.SkinID = "AddIcon";
                phVariantRow.Controls.Add(addIcon);
                rowBuilder = new StringBuilder();
                rowBuilder.Append("</a></td>\r\n");
                rowBuilder.Append("</tr>\r\n");
                */
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
            LinkButton lb = (LinkButton)sender;
            _CurrentPage = AlwaysConvert.ToInt(lb.CommandArgument);
            BindVariantGrid();
        }
        else
        {
            DropDownList ddl = (DropDownList)sender;
            _CurrentPage = AlwaysConvert.ToInt(ddl.SelectedItem.Text);
            BindVariantGrid();
        }
    }

    protected string GetFileSize(object dataItem)
    {
        LSDecimal tempSize = 0;
        DigitalGood digitalGood = (DigitalGood)dataItem;
        if (digitalGood.FileSize < 1024) return digitalGood.FileSize.ToString() + " bytes";
        tempSize = digitalGood.FileSize / 1024;
        if (tempSize < 1024) return string.Format("{0:0.#}kb", tempSize);
        tempSize = tempSize / 1024;
        return string.Format("{0:F1}mb", tempSize);
    }

    protected void DigitalGoodsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Detach")
        {
            int pdgId = AlwaysConvert.ToInt(e.CommandArgument);
            int index = _Product.DigitalGoods.IndexOf(pdgId);
            if (index > -1) _Product.DigitalGoods.DeleteAt(index);
            BindDigitalGoodsGrid();
        }
    }

    void detachButton_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton detachButton = sender as ImageButton;
        if (detachButton != null)
        {
            int pdgId = AlwaysConvert.ToInt(detachButton.ID.Substring(1));
            int index = _Product.DigitalGoods.IndexOf(pdgId);
            if (index > -1) _Product.DigitalGoods.DeleteAt(index);
            BindVariantGrid();
        }
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

    protected bool DGFileExists(object dataItem)
    {
        DigitalGood dg = (DigitalGood)dataItem;
        return File.Exists(dg.AbsoluteFilePath);
    }
}
