using System;
using System.Collections.Generic;
using System.IO;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Catalog;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Shipping;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Stores;
using CommerceBuilder.Orders;
using CommerceBuilder.Marketing;
using CommerceBuilder.DigitalDelivery;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// Class that represents a Product object in database
    /// </summary>
    public partial class Product : CatalogableBase
    {
        private string _IconUrl;

        /// <summary>
        /// Icon Url
        /// </summary>
        public string IconUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_IconUrl)) return _IconUrl;
                StoreSettingCollection settings = Token.Instance.Store.Settings;
                if (settings.ImageSkuLookupEnabled)
                {
                    string safeSku = FileHelper.GetSafeBaseImageName(this.Sku, false);
                    if (!string.IsNullOrEmpty(safeSku))
                    {
                        string physicalPath = FileHelper.BaseImagePath + safeSku + "_i.jpg";
                        if (File.Exists(physicalPath)) return FileHelper.BaseImageUrlPath + safeSku + "_i.jpg";
                    }
                }
                return string.Empty;
            }
            set
            {
                string originalValue = _IconUrl;
                _IconUrl = value;
                StoreSettingCollection settings = Token.Instance.Store.Settings;
                if (settings.ImageSkuLookupEnabled)
                {
                    string safeSku = FileHelper.GetSafeBaseImageName(this.Sku, false);
                    if (!string.IsNullOrEmpty(safeSku))
                    {
                        string testPath = FileHelper.BaseImageUrlPath + safeSku + "_i.jpg";
                        if (_IconUrl == testPath) _IconUrl = string.Empty;
                    }
                }
                if (originalValue != _IconUrl) this.IsDirty = true;
            }
        }

        private string _ThumbnailUrl;

        /// <summary>
        /// Thumbnail Url
        /// </summary>
        public override string ThumbnailUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_ThumbnailUrl)) return _ThumbnailUrl;
                StoreSettingCollection settings = Token.Instance.Store.Settings;
                if (settings.ImageSkuLookupEnabled)
                {
                    string safeSku = FileHelper.GetSafeBaseImageName(this.Sku, false);
                    if (!string.IsNullOrEmpty(safeSku))
                    {
                        string physicalPath = FileHelper.BaseImagePath + safeSku + "_t.jpg";
                        if (File.Exists(physicalPath)) return FileHelper.BaseImageUrlPath + safeSku + "_t.jpg";
                    }
                }
                return string.Empty;
            }
            set
            {
                string originalValue = _ThumbnailUrl;
                _ThumbnailUrl = value;
                StoreSettingCollection settings = Token.Instance.Store.Settings;
                if (settings.ImageSkuLookupEnabled)
                {
                    string safeSku = FileHelper.GetSafeBaseImageName(this.Sku, false);
                    if (!string.IsNullOrEmpty(safeSku))
                    {
                        string testPath = FileHelper.BaseImageUrlPath + safeSku + "_t.jpg";
                        if (_ThumbnailUrl == testPath) _ThumbnailUrl= string.Empty;
                    }
                }
                if (originalValue != _IconUrl) this.IsDirty = true;
            }
        }

        private string _ImageUrl;

        /// <summary>
        /// Image Url
        /// </summary>
        public string ImageUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_ImageUrl)) return _ImageUrl;
                StoreSettingCollection settings = Token.Instance.Store.Settings;
                if (settings.ImageSkuLookupEnabled)
                {
                    string safeSku = FileHelper.GetSafeBaseImageName(this.Sku, false);
                    if (!string.IsNullOrEmpty(safeSku))
                    {
                        string physicalPath = FileHelper.BaseImagePath + safeSku + ".jpg";
                        if (File.Exists(physicalPath)) return FileHelper.BaseImageUrlPath + safeSku + ".jpg";
                    }
                }
                return string.Empty;
            }
            set
            {
                string originalValue = _ImageUrl;
                _ImageUrl = value;
                StoreSettingCollection settings = Token.Instance.Store.Settings;
                if (settings.ImageSkuLookupEnabled)
                {
                    string safeSku = FileHelper.GetSafeBaseImageName(this.Sku, false);
                    if (!string.IsNullOrEmpty(safeSku))
                    {
                        string testPath = FileHelper.BaseImageUrlPath + safeSku + ".jpg";
                        if (_ImageUrl == testPath) _ImageUrl = string.Empty;
                    }
                }
                if (originalValue != _IconUrl) this.IsDirty = true;
            }
        }

        /// <summary>
        /// Inventory Mode of this product
        /// </summary>
        public InventoryMode InventoryMode
        {
            get
            {
                return (InventoryMode)this.InventoryModeId;
            }
            set
            {
                this.InventoryModeId = (byte)value;
            }
        }

        string _NavigateUrl = string.Empty;
        /// <summary>
        /// Navigation URL for this product in the store
        /// </summary>
        public override string NavigateUrl
        {
            get
            {
                if (_NavigateUrl.Length == 0)
                {
                    _NavigateUrl = UrlGenerator.GetBrowseUrl(this.ProductId, CatalogNodeType.Product, this.Name);
                }
                return _NavigateUrl;
            }
        }

        CustomUrl _CustomUrl;

        public string CustomUrl
        {
            get 
            {
                string url = string.Empty;
                if (_CustomUrl == null)
                    _CustomUrl = CustomUrlDataSource.LoadCustomUrl(this.ProductId, CatalogNodeType.Product);
                if (_CustomUrl != null)
                    url = _CustomUrl.Url;
                return url;
            }
            set 
            {
                string url = value.Trim();
                if (_CustomUrl == null)
                    _CustomUrl = CustomUrlDataSource.LoadCustomUrl(this.ProductId, CatalogNodeType.Product);

                if (!string.IsNullOrEmpty(url) && _CustomUrl == null)
                {
                    _CustomUrl = new CustomUrl();
                    _CustomUrl.CatalogNodeId = this.ProductId;
                    _CustomUrl.CatalogNodeTypeId = (byte)CatalogNodeType.Product;
                    _CustomUrl.Url = url;
                }
                else
                    if(_CustomUrl != null)
                    {
                        _CustomUrl.Url = url;
                    }
            }
        }

        /// <summary>
        /// Deletes this product from the database
        /// </summary>
        /// <returns><b>true</b> if delete is successful, <b>false</b> otherwise</returns>
        public bool Delete()
        {
            //DELETE ALL CHILD DATA THAT MAY NOT BE HANDLED BY CASCADE DELETES
            ProductDataSource.DeleteChildData(this);
            
            //CALL THE BASE DELETE METHOD
            return this.BaseDelete();
        }

        /// <summary>
        /// Saves this product to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public SaveResult Save()
        {
            if (this.IsDirty)
            {
                this.LastModifiedDate = LocaleHelper.LocalNow;
                if ((this.Shippable != Shippable.No) && (this.WarehouseId == 0))
                {
                    this.WarehouseId = Token.Instance.Store.DefaultWarehouse.WarehouseId;
                }              
            }

            SaveResult tempResult = this.BaseSave();
            if (tempResult != SaveResult.Failed)
            {
                UpdateInventoryMode();

                if (_Categories != null)
                {
                    _Categories.ProductId = this.ProductId;
                    _Categories.Save();
                }
                if (_RelatedProducts != null)
                {
                    foreach (RelatedProduct rp in _RelatedProducts)
                        rp.ProductId = this.ProductId;
                    _RelatedProducts.Save();
                }

                if (_CustomUrl != null)
                {
                    if (!string.IsNullOrEmpty(_CustomUrl.Url))
                    {
                        _CustomUrl.CatalogNodeId = this.ProductId;
                        _CustomUrl.CatalogNodeType = CatalogNodeType.Product;
                        _CustomUrl.Save();
                    }
                    else
                        _CustomUrl.Delete();
                }
            }
            return tempResult;
        }

        private ProductCategoryCollection _Categories;
        
        /// <summary>
        /// Collection of categories that this product is associated with
        /// </summary>
        public ProductCategoryCollection Categories
        {
            get
            {
                if (_Categories == null)
                {
                    _Categories = new ProductCategoryCollection();
                    _Categories.Load(this.ProductId);
                }
                return _Categories;
            }
        }

        private RelatedProductCollection _RelatedProducts;

        /// <summary>
        /// Collection of related products
        /// </summary>
        public RelatedProductCollection RelatedProducts
        {
            get
            {
                if (_RelatedProducts == null)
                {
                    _RelatedProducts = RelatedProductDataSource.LoadForProduct(this.ProductId);
                }
                return _RelatedProducts;
            }
        }

        /// <summary>
        /// Is this product shippable?
        /// </summary>
        public Shippable Shippable
        {
            get { return (Shippable)this.ShippableId; }
            set { this.ShippableId = (byte)value; }
        }

        /// <summary>
        /// Indicates the kit status of this product
        /// </summary>
        public KitStatus KitStatus
        {
            get
            {
                if (this.ProductKitComponents.Count > 0) return KitStatus.Master;
                int componentCount = KitComponentDataSource.CountForMemberProduct(this.ProductId);
                if (componentCount > 0) return KitStatus.Member;
                return KitStatus.None;
            }
        }

        private LSDecimal _Rating = -1;
        /// <summary>
        /// Rating of this product
        /// </summary>
        public LSDecimal Rating
        {
            get
            {
                if (_Rating < 0)
                {
                    _Rating = ProductReviewDataSource.GetProductRating(this.ProductId);
                }
                return _Rating;
            }
        }

        /// <summary>
        /// Does this product has options or choices to be selected?
        /// </summary>
        public bool HasChoices
        {
            get
            {
                if (this.ProductOptions.Count > 0) return true;
                if (this.KitStatus == KitStatus.Master) return true;
                foreach (ProductProductTemplate ppt in this.ProductProductTemplates)
                {
                    ProductTemplate template = ppt.ProductTemplate;
                    if (template != null)
                    {
                        foreach (InputField field in template.InputFields)
                        {
                            if (!field.IsMerchantField) return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of options associated with this product
        /// </summary>
        /// <returns>Collection of Option objects associated with this product</returns>
        public OptionCollection GetOptions()
        {
            OptionCollection options = new OptionCollection();
            foreach (ProductOption item in this.ProductOptions)
            {
                options.Add(item.Option);
            }
            return options;
        }

        /// <summary>
        /// Updates the inventory mode of this product
        /// </summary>
        public void UpdateInventoryMode()
        {
            if (this.InventoryMode == InventoryMode.Variant)
            {
                ProductVariantManager vm = new ProductVariantManager(this.ProductId);
                if (vm.Count == 0)
                {
                    ProductDataSource.UpdateInventoryMode(this.ProductId, InventoryMode.None);
                    _InventoryModeId = (byte)InventoryMode.None;
                }
            }
        }

        /// <summary>
        /// Copies the product to the specified category
        /// </summary>
        /// <param name="categoryId">if ZERO then will copy the product with the categories as the orignal product have</param>
        public void SaveCopy(int categoryId)
        {
            SaveCopy(categoryId, null);
        }

        /// <summary>
        /// Copies the product with the categories as the orignal product have
        /// </summary>
        public void SaveCopy()
        {
            SaveCopy(0, null);
        }

        /// <summary>
        /// Copies the product with the categories as the orignal product have
        /// </summary>
        /// <param name="copyName">Name to use for the copied product</param>
        public void SaveCopy(string copyName)
        {
            SaveCopy(0, copyName);
        }

        /// <summary>
        /// Copies the product to the specified category.
        /// </summary>
        /// <param name="categoryId">if ZERO then will copy the product with the categories as the orignal product have </param>
        /// <param name="copyName">Name to use for the copied product</param>
        public void SaveCopy(int categoryId, string copyName)
        {
            if (string.IsNullOrEmpty(copyName))
            {
                this.Name = "Copy of " + this.Name;
            }
            else
            {
                this.Name = copyName;
            }
            CopyChildren();
            //make sure related products and upsell products
            //are loaded before resetting product id to 0
            RelatedProductCollection relatedProds = this.RelatedProducts;
            UpsellProductCollection upsellProds = this.UpsellProducts;
            SubscriptionPlan subplan = this.SubscriptionPlan;
            ProductProductTemplateCollection productProductTemplates = this.ProductProductTemplates;
            
            // KEEP THE ORIGNAL PRODUCTS CATEGORIES IN A NEW LIST
            List<int> pcats = new List<int>();
            pcats.AddRange(this.Categories);

            this.ProductId = 0;
            this.Save();

            // RELOAD THE LIST, SO THAT IT CAN PROPERLY BE SAVED
            // THIS IS FOR PRODUCTS WE ARE COPYING WITH FULL CATEGORY DETAILS
            this.Categories.Load(this.ProductId);

            if (categoryId > 0)
            {                
                this.Categories.Add(categoryId);                
            }
            else
            {                
                this.Categories.AddRange(pcats);                
            }
            this.Categories.ProductId = this.ProductId;
            
            this.Categories.Save();

            foreach (RelatedProduct prod in relatedProds)
            {
                prod.ProductId = this.ProductId;
                prod.Save();
            }

            foreach (UpsellProduct upsell in upsellProds)
            {
                upsell.ProductId = this.ProductId;
                upsell.Save();
            }
            if (subplan != null)
            {
                subplan.ProductId = this.ProductId;
                subplan.Save();
            }
            foreach (ProductProductTemplate productProductTemplate in productProductTemplates)
            {
                productProductTemplate.ProductId = this.ProductId;
                productProductTemplate.Save();
            }
        }

        private void CopyChildren()
        {
            foreach (ProductAsset asset in Assets)
            {
                asset.ProductAssetId = 0;
            }

            foreach (ProductCustomField field in CustomFields)
            {
                field.ProductFieldId = 0;
            }

            foreach (ProductDigitalGood good in DigitalGoods)
            {
                good.ProductDigitalGoodId = 0;
            }

            foreach (ProductImage image in Images)
            {
                image.ProductImageId = 0;
            }

            foreach (ProductKitComponent comp in ProductKitComponents)
            {   
                KitComponent kcomp = comp.KitComponent;
                kcomp.KitComponentId = 0;
                kcomp.Save();
                comp.KitComponentId = comp.KitComponentId;
            }

            foreach (ProductTemplateField field in TemplateFields)
            {
                field.ProductTemplateFieldId = 0;
            }

            foreach (Special special in Specials)
            {
                List<int> groupIds = new List<int>();
                foreach (SpecialGroup sg in special.SpecialGroups)
                {
                    groupIds.Add(sg.GroupId);
                }
                
                special.SpecialId = 0;
                special.Save();

                // ADD THE SPECIAL GROUPS
                special.SpecialGroups.Clear();
                foreach (int groudId in groupIds)
                {
                    special.SpecialGroups.Add(new SpecialGroup(special.SpecialId, groudId));
                }
            }
                        
            List<int> oldChoiceIds = new List<int>();
            List<int> newChoiceIds = new List<int>();
            int oldChoiceId, newChoiceId;
            foreach (ProductOption option in ProductOptions)
            {                   
                Option opt = option.Option;
                foreach (OptionChoice choice in opt.Choices)
                {
                    oldChoiceId = choice.OptionChoiceId;
                    choice.OptionChoiceId = 0;
                    oldChoiceIds.Add(oldChoiceId);                    
                }

                opt.OptionId = 0;
                opt.Save();
                option.OptionId = opt.OptionId;

                foreach (OptionChoice choice in opt.Choices)
                {
                    newChoiceId = choice.OptionChoiceId;
                    newChoiceIds.Add(newChoiceId);
                }
            }

            Dictionary<int, int> optChoiceIdMap = new Dictionary<int, int>();
            if (oldChoiceIds.Count == newChoiceIds.Count)
            {
                for (int i = 0; i < oldChoiceIds.Count; i++)
                {
                    optChoiceIdMap.Add(oldChoiceIds[i], newChoiceIds[i]);
                }
            }

            foreach (ProductVariant pv in Variants)
            {
                pv.ProductVariantId = 0;
                pv.Option1 = GetNewChoiceId(pv.Option1, optChoiceIdMap);
                pv.Option2 = GetNewChoiceId(pv.Option2, optChoiceIdMap);
                pv.Option3 = GetNewChoiceId(pv.Option3, optChoiceIdMap);
                pv.Option4 = GetNewChoiceId(pv.Option4, optChoiceIdMap);
                pv.Option5 = GetNewChoiceId(pv.Option5, optChoiceIdMap);
                pv.Option6 = GetNewChoiceId(pv.Option6, optChoiceIdMap);
                pv.Option7 = GetNewChoiceId(pv.Option7, optChoiceIdMap);
                pv.Option8 = GetNewChoiceId(pv.Option8, optChoiceIdMap);                
            }
        }

        private int GetNewChoiceId(int oldId, Dictionary<int, int> optionChoiceIdMap)
        {
            if (optionChoiceIdMap.ContainsKey(oldId))
            {
                return optionChoiceIdMap[oldId];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a collection of Upsell Products associated with this 
        /// </summary>
        /// <param name="basketToExclude"></param>
        /// <returns></returns>
        public UpsellProductCollection GetUpsellProducts(Basket basketToExclude)
        {
            if (basketToExclude == null) return UpsellProducts;
            UpsellProductCollection upsellProds = new UpsellProductCollection();
            foreach (UpsellProduct upsell in UpsellProducts)
            {
                // ONLY CONSIDER PRODUTS WITH PUBLIC VISIBILITY
                if (!basketToExclude.ContainsProduct(upsell.ChildProductId) && upsell.ChildProduct.Visibility == CatalogVisibility.Public)
                {
                    upsellProds.Add(upsell);
                }
            }
            return upsellProds;
        }

        /// <summary>
        /// Gets a collection of Upsell Products associated with this
        /// </summary>
        /// <param name="excludeHidden">if true hidden and private associated Upsell Products will be excluded</param>
        /// <returns></returns> 
        public UpsellProductCollection GetUpsellProducts(bool excludeHidden)
        {
            if (!excludeHidden) return UpsellProducts;
            UpsellProductCollection upsellProds = new UpsellProductCollection();
            foreach (UpsellProduct upsell in UpsellProducts)
            {
                if (upsell.ChildProduct.Visibility == CatalogVisibility.Public)
                {
                    upsellProds.Add(upsell);
                }
            }
            return upsellProds;
        }

    }
}
