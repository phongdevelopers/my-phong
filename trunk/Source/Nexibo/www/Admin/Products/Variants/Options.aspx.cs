using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

public partial class Admin_Products_Variants_Options : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _ProductId = 0;
    Product _Product;
    OptionCollection _Options;

    protected string GetOptionNames(object dataItem)
    {
        Option option = (Option)dataItem;
        if (option.Choices.Count == 0)
        {
            return "<i>none</i>";
        }
        List<string> names = new List<string>();
        foreach (OptionChoice choice in option.Choices)
        {
            names.Add(choice.Name);
        }
        string retVal = string.Join(", ", names.ToArray());
        if ((retVal.Length > 100))
        {
            retVal = (retVal.Substring(0, 100) + "...");
        }
        return retVal;
    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId());
        _Options = _Product.GetOptions();
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        VariantLink.NavigateUrl += "?ProductId=" + _ProductId.ToString();
        BindOptionsGrid();
        PageHelper.ConvertEnterToTab(AddOptionName);
        PageHelper.SetDefaultButton(AddOptionChoices, AddButton.ClientID);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        string warnScript;
        bool hasVariantData = ProductVariantManager.HasVariantData(_ProductId);
        bool hasDigitalGoods = ProductVariantManager.HasDigitalGoodData(_ProductId);
        if (hasVariantData || hasDigitalGoods || _Product.KitStatus == KitStatus.Member)
        {
            String delMesssage = String.Empty;
            String addMessage = String.Empty;
            if (hasVariantData)
            {
                delMesssage += "WARNING: If you have made changes to the variant grid, such as adjusting the variant price or in-stock levels, deleting an option will reset this data.\\n\\n";
                addMessage += "WARNING: If you have made changes to the variant grid, such as adjusting the variant price or in-stock levels, adding an option will reset this data. \\n\\n";
            }
            if (hasDigitalGoods)
            {
                delMesssage += "WARNING: There are digital goods attached to one or more variants, deleting this option will remove all associated digital goods.\\n\\n";
                addMessage += "WARNING: There are digital goods attached to one or more existing variants, adding an option will remove all associated digital goods.\\n\\n";
            }
            if (_Product.KitStatus == KitStatus.Member)
            {
                addMessage += "WARNING: This product is part of one or more kit products, adding an option will remove this product from those kit products.\\n\\n";
                delMesssage += "WARNING: This product is part of one or more kit products, deleting an option will remove this product from those kit products.\\n\\n";
            }            

            warnScript = "function confirmAdd(){return confirm('" + addMessage + " Are you sure you want to continue?');}\r\n";
            warnScript += "function confirmDel(){return confirm('" + delMesssage + "Are you sure you want to delete this option?');}";
        }
        else
        {
            warnScript = "function confirmAdd(){return true;}";
            warnScript += "function confirmDel(){return confirm('Are you sure you want to delete this option?');}";
        }
        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "checkVariant", warnScript, true);
    }

    protected void BindOptionsGrid()
    {
        OptionsGrid.DataSource = _Options;
        OptionsGrid.DataBind();
    }

    protected void OptionsGrid_DataBound(object sender, System.EventArgs e)
    {
        navLinkPanel.Visible = (OptionsGrid.Rows.Count > 0);
    }

    protected void OptionsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MoveUp" || e.CommandName == "MoveDown" || e.CommandName == "DoDelete")
        {
            int optionId = AlwaysConvert.ToInt(e.CommandArgument);
            ProductOptionCollection prodOpts = _Product.ProductOptions;
            int itemIndex = prodOpts.IndexOf(_ProductId, optionId);
            ProductOption selectedItem;
            if (e.CommandName == "MoveUp")
            {
                if ((itemIndex < 1) || (itemIndex > (prodOpts.Count - 1))) return;
                selectedItem = prodOpts[itemIndex];
                prodOpts[itemIndex] = prodOpts[(itemIndex - 1)];
                prodOpts[(itemIndex - 1)] = selectedItem;
            }
            else if (e.CommandName == "MoveDown")
            {
                if ((itemIndex > (prodOpts.Count - 2)) || (itemIndex < 0)) return;
                selectedItem = prodOpts[itemIndex];
                prodOpts[itemIndex] = prodOpts[(itemIndex + 1)];
                prodOpts[(itemIndex + 1)] = selectedItem;
            }
            else if (e.CommandName == "DoDelete")
            {
                //DELETE THE PRODUCT - OPTION ASSOCIATION
                if (itemIndex >= 0) prodOpts.DeleteAt(itemIndex);
            }
            //UPDATE THE DISPLAY ORDER
            for (short i = 0; i < prodOpts.Count; i++)
            {
                prodOpts[i].OrderBy = i;
            }
            prodOpts.Save();
            _Options = _Product.GetOptions();
            _Product.UpdateInventoryMode();
            BindOptionsGrid();
        }
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Option option = new Option();
            option.Name = AddOptionName.Text;
            string[] choices = AddOptionChoices.Text.Split(",".ToCharArray());
            foreach (string item in choices)
            {
                string choiceName = item.Trim();
                if (choiceName != String.Empty)
                {
                    OptionChoice choice = new OptionChoice();
                    choice.Name = StringHelper.Truncate(choiceName, 50);
                    choice.OrderBy = -1;
                    option.Choices.Add(choice);
                }
            }
            option.ProductOptions.Add(new ProductOption(_ProductId, 0, -1));
            option.Save();
            _Options.Add(option);
            AddOptionName.Text = string.Empty;
            AddOptionChoices.Text = string.Empty;
            BindOptionsGrid();
        }
    }

    protected void OptionsGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int optionId = (int)OptionsGrid.DataKeys[e.RowIndex].Value;
        //FIND THE OPTION
        e.Cancel = true;
    }

    protected string GetEditOptionUrl(object dataItem)
    {
        Option o = (Option)dataItem;
        return Page.ResolveUrl("~/Admin/Products/Variants/EditOption.aspx?ProductId=" + _ProductId.ToString() + "&OptionId=" + o.OptionId.ToString());
    }
}