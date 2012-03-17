using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Products;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;

public partial class Admin_Products_Kits_EditKit : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected int _CategoryId = 0;
    protected int _ProductId = 0;
    private Product _Product;
    private Kit _Kit;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        _CategoryId = PageHelper.GetCategoryId();
        if (_Product == null) Response.Redirect("../../Catalog/Browse.aspx?CategoryId=" + _CategoryId);

        // PREVENT CREATION OF RECURSIVE KITS (MEMBER PRODUCTS CANNOT BE MASTER PRODUCTS)
        if (_Product.KitStatus == KitStatus.Member) Response.Redirect("ViewPart.aspx?ProductId=" + _ProductId.ToString());
        _Kit = _Product.Kit;

        // INITIALIZE PAGE ELEMENTS
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        SortComponents.NavigateUrl += "?ProductId=" + _ProductId;
        if (!Page.IsPostBack)
        {
            ItemizedDisplayOption.SelectedIndex = _Kit.ItemizeDisplay ? 1 : 0;
        }

        // POPULATE THE ADD COMPONENT INPUT TYPE LIST
        foreach (string inputName in Enum.GetNames(typeof(KitInputType)))
        {
            AddComponentInputTypeId.Items.Add(new ListItem(FixInputTypeName(inputName), Enum.Parse(typeof(KitInputType), inputName).ToString()));
        }
        AddComponentInputTypeId.SelectedIndex = 1;

        // INITIALIZE THE KIT DETAILS
        BindComponentList();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        ItemizedDisplay.Text = _Kit.ItemizeDisplay ? "Itemized" : "Bundle";
    }

    protected string FixInputTypeName(string name)
    {
        switch (name.ToUpperInvariant())
        {
            case "INCLUDEDHIDDEN":
                return "Included - Hidden";
            case "INCLUDEDSHOWN":
                return "Included - Shown";
            default:
                return Regex.Replace(name, "([A-Z])", " $1").Trim();
        }
    }

    protected void BindComponentList()
    {
        List<KitComponent> components = new List<KitComponent>();
        KitComponent kc;
        foreach (ProductKitComponent pkc in _Product.ProductKitComponents)
        {
            kc = pkc.KitComponent;
            if (kc != null)
            {
                kc.RemoveInvalidKitProducts();
                components.Add(kc);
            }
        }
        if (components.Count > 0)
        {
            ComponentList.DataSource = components;
            ComponentList.DataBind();
            SortComponents.Visible = (components.Count > 1);
            PriceRange.Text = string.Format("{0:lc} - {1:lc}", _Kit.MinPrice, _Kit.MaxPrice);
            DefaultPrice.Text = string.Format("{0:lc}", _Kit.DefaultPrice);
            WeightRange.Text = string.Format("{0:F2} - {1:F2}", _Kit.MinWeight, _Kit.MaxWeight);
            DefaultWeight.Text = string.Format("{0:F2}", _Kit.DefaultWeight);
            ExistingKitPanel.Visible = true;
            NewKitPanel.Visible = false;
        }
        else
        {
            ExistingKitPanel.Visible = false;
            NewKitPanel.Visible = true;
        }
    }

    protected void MoveKitProduct(KitProduct kitProduct, int direction)
    {
        //MAKE SURE ALL KIT PRODUCTS ARE IN CORRECT ORDER
        short orderBy = -1;
        int index = -1;
        KitComponent kitComponent = kitProduct.KitComponent;
        foreach (KitProduct kp in kitComponent.KitProducts)
        {
            orderBy += 1;
            kp.OrderBy = orderBy;
            if (kp.KitProductId.Equals(kitProduct.KitProductId)) index = orderBy;
        }
        //LOCATE THE DESIRED ITEM
        if (index > -1)
        {
            KitProduct temp = null;
            if (direction < 0 && index > 0)
            {
                //MOVE UP
                temp = kitComponent.KitProducts[index];
                temp.OrderBy -= 1;
                kitComponent.KitProducts[index] = kitComponent.KitProducts[index - 1];
                kitComponent.KitProducts[index].OrderBy += 1;
                kitComponent.KitProducts[index - 1] = temp;
                kitComponent.KitProducts.Save();
                BindComponentList();
            }
            else if (direction > 0 && index < kitComponent.KitProducts.Count - 1)
            {
                //MOVEDOWN
                temp = kitComponent.KitProducts[index];
                temp.OrderBy += 1;
                kitComponent.KitProducts[index] = kitComponent.KitProducts[index + 1];
                kitComponent.KitProducts[index].OrderBy -= 1;
                kitComponent.KitProducts[index + 1] = temp;
                kitComponent.KitProducts.Save();
                BindComponentList();
            }
        }
    }

    protected void KitProductList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DoDelete")
        {
            int kitProductId = AlwaysConvert.ToInt(e.CommandArgument);
            KitProduct kitProduct = KitProductDataSource.Load(kitProductId);
            if (kitProduct != null)
            {
                int index = kitProduct.KitComponent.KitProducts.IndexOf(kitProductId);
                if (index >= 0) kitProduct.KitComponent.KitProducts.RemoveAt(index);

                kitProduct.Delete();
                BindComponentList();
            }
        }
    }

    protected void ComponentList_ItemCommand(object source, DataListCommandEventArgs e)
    {
        int dataItemIndex = AlwaysConvert.ToInt(e.CommandArgument);
        int kitComponentId = (int)ComponentList.DataKeys[dataItemIndex];
        KitComponent kitComponent = KitComponentDataSource.Load(kitComponentId);
        switch (e.CommandName)
        {
            case "Branch":
                if (kitComponent != null)
                {
                    kitComponent.Branch(_ProductId);
                    BindComponentList();
                }
                break;
            case "Delete":
                if (kitComponent != null)
                {
                    int index = _Product.ProductKitComponents.IndexOf(_ProductId, kitComponentId);
                    if (index > -1)
                        _Product.ProductKitComponents.DeleteAt(index);
                    kitComponent.Delete();
                    BindComponentList();
                }
                break;
            case "DeleteShared":
                Response.Redirect(string.Format("DeleteSharedComponent.aspx?CategoryId={0}&ProductId={1}&KitComponentId={2}", _CategoryId, _ProductId, kitComponentId));
                break;
        }
    }

    protected string GetEditPartLink(object dataItem)
    {
        KitProduct kp = (KitProduct)dataItem;
        return string.Format("EditPart.aspx?CategoryId={0}&ProductId={1}&KitComponentId={2}&PartId={3}", _CategoryId, _ProductId, kp.KitComponentId, kp.KitProductId);
    }


    #region Add Component
    protected void AddComponentButton_Click(object sender, EventArgs e)
    {
        AddComponentPopup.Show();
    }

    protected void AddComponentCancelButton_Click(object sender, EventArgs e)
    {
        // RESET THE ADD COMPONENT DIALOG
        AddComponentName.Text = string.Empty;
        AddComponentInputTypeId.SelectedIndex = 1;
        AddComponentPopup.Hide();
    }

    protected void AddComponentSaveButton_Click(object sender, EventArgs e)
    {
        // CREATE THE KIT COMPONENT
        KitComponent component = new KitComponent();
        component.Name = AddComponentName.Text;
        component.InputTypeId = (short)(AddComponentInputTypeId.SelectedIndex);
        //component.HeaderOption = HeaderOption.Text;
        component.Save();

        // ASSOCIATE KIT COMPONENT TO THE PRODUCT
        _Product.ProductKitComponents.Add(new ProductKitComponent(_ProductId, component.KitComponentId));
        _Product.Save();

        // RESET THE ADD COMPONENT DIALOG
        AddComponentName.Text = string.Empty;
        AddComponentInputTypeId.SelectedIndex = 1;

        // REBIND THE KIT DETAILS
        BindComponentList();
    }
    #endregion

    protected void ItemizedDisplayOkButton_Click(object sender, EventArgs e)
    {
        _Kit.ItemizeDisplay = (AlwaysConvert.ToInt(ItemizedDisplayOption.SelectedValue) != 0);
        _Kit.Save();
    }

}
