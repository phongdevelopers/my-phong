using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

public partial class Admin_Products_Variants_EditOption : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _OptionId = 0;
    Option _Option;

    protected Option Option
    {
        get
        {
            if (_Option == null)
            {
                _Option = OptionDataSource.Load(this._OptionId);
            }
            return _Option;
        }
    }

    protected bool HasChoices()
    {
        return _Option != null && _Option.Choices.Count > 0;
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        _OptionId = AlwaysConvert.ToInt(Request.QueryString["OptionId"]);
        _Option = OptionDataSource.Load(_OptionId);
        if (_Option == null)
        {
            Response.Redirect("Options.aspx?ProductId=" + PageHelper.GetProductId() + "&OptionId=" + _OptionId.ToString());
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            BindChoicesGrid();
        }

        PageHelper.SetDefaultButton(AddChoiceName, AddChoiceButton.ClientID);
        PageHelper.SetPickImageButton(AddChoiceThumbnail, BrowseThumbnail);
        PageHelper.SetPickImageButton(AddChoiceImage, BrowseImage);
    }

    protected void BindChoicesGrid()
    {
        OptionChoicesGrid.DataSource = _Option.Choices;
        OptionChoicesGrid.DataBind();
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Options.aspx?ProductId=" + PageHelper.GetProductId());
    }

    private string GetControlValue(GridViewRow row, string controlId)
    {
        TextBox tb = row.FindControl(controlId) as TextBox;
        if (tb != null) return tb.Text;
        return String.Empty;
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        // DETERMINE START INDEX
        OptionChoice opt;
        foreach (GridViewRow row in OptionChoicesGrid.Rows)
        {
            // LOCATE THE SKU, PRICE, WEIGHT, COST
            // GET VARIANT AT THIS INDEX
            opt = _Option.Choices[row.DataItemIndex];
            string name = GetControlValue(row, "Name").Trim();
            if (!string.IsNullOrEmpty(name)) opt.Name = name;
            opt.ThumbnailUrl = GetControlValue(row, "ThumbnailUrl").Trim();
            opt.ImageUrl = GetControlValue(row, "ImageUrl").Trim();
            opt.PriceModifier = AlwaysConvert.ToDecimal(GetControlValue(row, "PriceMod"));
            opt.WeightModifier = AlwaysConvert.ToDecimal(GetControlValue(row, "WeightMod"));
            opt.SkuModifier = GetControlValue(row, "SkuMod").Trim();
            opt.Save();
        }

        //Save the option itself
        Option.Name = OptionName.Text;
        Option.HeaderText = HeaderText.Text;
        Option.ShowThumbnails = ShowThumbnails.Checked;
        Option.ThumbnailColumns = AlwaysConvert.ToByte(ThumbnailColumns.Text);
        Option.ThumbnailHeight = AlwaysConvert.ToInt16(ThumbnailHeight.Text);
        Option.ThumbnailWidth = AlwaysConvert.ToInt16(ThumbnailWidth.Text);
        Option.Save();

        //Rebind choices
        BindChoicesGrid();
    }

    protected void SaveCloseButton_Click(object sender, System.EventArgs e)
    {
        SaveButton_Click(sender, e);
        Response.Redirect("Options.aspx?ProductId=" + PageHelper.GetProductId());
    }

    protected void OptionChoicesGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //HOOK UP BROWSE THUMBNAIL
            TextBox ThumbnailUrl = e.Row.FindControl("ThumbnailUrl") as TextBox;
            ImageButton BrowseButton = e.Row.FindControl("BrowseThumbnailUrl") as ImageButton;
            if ((ThumbnailUrl != null) && (BrowseButton != null))
            {
                PageHelper.SetPickImageButton(ThumbnailUrl, BrowseButton);
            }
            //HOOK UP BROWSE IMAGE
            TextBox ImageUrl = e.Row.FindControl("ImageUrl") as TextBox;
            ImageButton BrowseImageButton = e.Row.FindControl("BrowseImageUrl") as ImageButton;
            if ((ImageUrl != null) && (BrowseImageButton != null))
            {
                PageHelper.SetPickImageButton(ImageUrl, BrowseImageButton);
            }
        }
    }

    protected void OptionChoicesGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        OptionChoiceCollection options = this.Option.Choices;
        int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
        OptionChoice selectedOption;
        switch (e.CommandName)
        {
            case "MoveUp":
                if ((itemIndex < 1) || (itemIndex > (options.Count - 1))) return;
                selectedOption = options[itemIndex];
                options[itemIndex] = options[(itemIndex - 1)];
                options[(itemIndex - 1)] = selectedOption;
                break;
            case "MoveDown":
                if ((itemIndex > (options.Count - 2)) || (itemIndex < 0)) return;
                selectedOption = options[itemIndex];
                options[itemIndex] = options[(itemIndex + 1)];
                options[(itemIndex + 1)] = selectedOption;
                break;
            default:
                return;
        }
        ResetOrderBy();
        options.Save();
        BindChoicesGrid();
    }

    protected void OptionChoicesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int optionId = (int)OptionChoicesGrid.DataKeys[e.RowIndex].Value;
        //FIND THE OPTION
        OptionChoiceCollection options = this.Option.Choices;
        int index = -1;
        int i = 0;
        while ((i < options.Count) && (index < 0))
        {
            if (options[i].OptionChoiceId == optionId) index = i;
            i++;
        }
        if (index >= 0)
        {
            options.DeleteAt(index);
            //REBUILD VARIANT GRID FOR THIS PRODUCT
            int productId = PageHelper.GetProductId();
            if (productId > 0) ProductVariantManager.ScrubVariantGrid(productId, optionId);
            BindChoicesGrid();
        }
    }

    protected void ResetOrderBy()
    {
        OptionChoiceCollection options = this.Option.Choices;
        for (short i = 0; i < options.Count; i++)
        {
            options[i].OrderBy = i;
        }
    }

    protected string EmptyZero(object dataItem)
    {
        string result = dataItem.ToString();
        if (result == "0") return string.Empty;
        return result;
    }

    protected void AddChoiceButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            string optionName = AddChoiceName.Text;
            if (!string.IsNullOrEmpty(optionName))
            {
                OptionChoice opt = new OptionChoice();
                opt.Name = optionName;
                opt.ThumbnailUrl = AddChoiceThumbnail.Text;
                opt.ImageUrl = AddChoiceImage.Text;
                opt.PriceModifier = AlwaysConvert.ToDecimal(AddChoicePriceMod.Text);
                opt.WeightModifier = AlwaysConvert.ToDecimal(AddChoiceWeightMod.Text);
                opt.SkuModifier = AddChoiceSkuMod.Text.Trim();
                opt.OrderBy = -1;
                Option.Choices.Add(opt);
            }
            //UPDATE THUMBNAILS
            Option.Save();
            BindChoicesGrid();
            AddChoiceName.Text = string.Empty;
            AddChoiceThumbnail.Text = string.Empty;
            AddChoiceImage.Text = string.Empty;
            AddChoicePriceMod.Text = string.Empty;
            AddChoiceWeightMod.Text = string.Empty;
            AddChoiceSkuMod.Text = string.Empty;
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        /*        if (HasChoices())
                {
                    SaveButton.Visible = true;
                    SaveCloseButton.Visible = true;
                }
                else
                {
                    SaveButton.Visible = false;
                    SaveCloseButton.Visible = false;
                }
         */

        SaveButton.Visible = true;
        SaveCloseButton.Visible = true;

        OptionName.Text = Option.Name;
        HeaderText.Text = Option.HeaderText;    
        ShowThumbnails.Checked = Option.ShowThumbnails;
        ThumbnailWidth.Text = ZeroAsEmpty(Option.ThumbnailWidth.ToString());
        ThumbnailHeight.Text = ZeroAsEmpty(Option.ThumbnailHeight.ToString());
        ThumbnailColumns.Text = ZeroAsEmpty(Option.ThumbnailColumns.ToString());

        Caption.Text = string.Format(Caption.Text, Option.Name);
        EditChoicesLabel.Text = string.Format(EditChoicesLabel.Text, Option.Name);

        // REGISTER WARNING SCRIPTS IF THE PRODUCT HAS CUSTOMIZED VARIANTS
        string warnScript;
        int productId = PageHelper.GetProductId();
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            bool hasVariantData = ProductVariantManager.HasVariantData(productId);
            bool hasDigitalGoods = ProductVariantManager.HasDigitalGoodData(productId);
            if (hasVariantData || hasDigitalGoods || product.KitStatus == KitStatus.Member)
            {
                String delMesssage = String.Empty;
                if (hasVariantData)
                {
                    delMesssage += "WARNING: If you have made changes to the variant grid, such as adjusting the variant price or in-stock levels, deleting a choice will reset this data.\\n\\n";
                }
                if (hasDigitalGoods)
                {
                    delMesssage += "WARNING: If there are digital goods attached to one or more associated variants, deleting this choice will remove digital goods from associated variants.\\n\\n";                    
                }
                if (product.KitStatus == KitStatus.Member)
                {
                    delMesssage += "WARNING: If this product is part of one or more kit products, deleting this choice will remove associated product variants from those kit products.\\n\\n";
                }

                warnScript = "function confirmDel(){return confirm('" + delMesssage + "Are you sure you want to delete this option?');}";
            }
            else
            {
                warnScript = "function confirmDel(){return confirm('Are you sure you want to delete this option?');}";
            }
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "checkVariant", warnScript, true);
        }
    }

    protected string ZeroAsEmpty(string data)
    {
        if (data == "0") return string.Empty;
        return data;
    }
}
