using System;
using System.Text;
using CommerceBuilder.Products;
using System.ComponentModel;
using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// DataSource class for BasketItem objects
    /// </summary>
    [DataObject(true)]
    public partial class BasketItemDataSource
    {
        /// <summary>
        /// Creates a basket item for a product
        /// </summary>
        /// <param name="productId">The product for which to create the basket item</param>
        /// <param name="quantity">The quantity of the basket item</param>
        /// <returns>The BasketItem created</returns>
        public static BasketItem CreateForProduct(int productId, short quantity)
        {
            return BasketItemDataSource.CreateForProduct(productId, quantity, string.Empty, String.Empty, Token.Instance.UserId);
        }

        /// <summary>
        /// Creates a basket item for a product
        /// </summary>
        /// <param name="productId">Id of the product for which to create the basket item</param>
        /// <param name="quantity">The quantity of basket item</param>
        /// <param name="optionList">List of option ids for the variant</param>
        /// <param name="kitList">List of product Ids of the kit items</param>
        /// <returns>The BasketItem created</returns>
        public static BasketItem CreateForProduct(int productId, short quantity, string optionList, string kitList)
        {
            return BasketItemDataSource.CreateForProduct(productId, quantity, optionList, kitList, Token.Instance.UserId);
        }

        /// <summary>
        /// Creates a basket item for a product
        /// </summary>
        /// <param name="productId">Id of the product for which to create the basket item</param>
        /// <param name="quantity">The quantity of basket item</param>
        /// <param name="optionList">List of option ids for the variant</param>
        /// <param name="kitList">List of product Ids of the kit items</param>
        /// <param name="userId">The user Id</param>
        /// <returns>The BasketItem created</returns>
        public static BasketItem CreateForProduct(int productId, short quantity, string optionList, string kitList, int userId)
        {
            // vALIDATE PARAMETERS
            Product product = ProductDataSource.Load(productId);
            if (product == null) throw new ArgumentException("invalid product specified", "product");
            if (quantity < 1) throw new ArgumentException("quantity must be greater than 0", "quantity");
            if (!string.IsNullOrEmpty(optionList))
            {
                ProductVariant v = ProductVariantDataSource.LoadForOptionList(productId, optionList);
                if (v == null) throw new ArgumentException("specified product options are invalid", "optionList");
            }
            if (product.KitStatus == KitStatus.Master)
            {
                Kit kit = product.Kit;
                if (!kit.ValidateChoices(kitList)) throw new ArgumentException("specified kit configuration is invalid", "kitProductIds");
            }

            //GET THE QUANTITY
            short tempQuantity = quantity;
            short basketQuantity = GetExistingBasketCount(product);

            if ((product.MinQuantity > 0) && ((tempQuantity + basketQuantity) < product.MinQuantity))
                tempQuantity = (short)(product.MinQuantity - basketQuantity);


            if ((product.MaxQuantity > 0) && ((tempQuantity + basketQuantity) > product.MaxQuantity))
                tempQuantity = (short)(product.MaxQuantity - basketQuantity);


            if (tempQuantity < 1) return null;
            quantity = tempQuantity;

            BasketItem item = new BasketItem();
            //CALCULATE THE PRICE OF THE PRODUCT
            ProductCalculator pcalc = ProductCalculator.LoadForProduct(productId, quantity, optionList, kitList, userId);
            item.Sku = pcalc.Sku;
            item.Price = pcalc.Price;
            item.Weight = pcalc.Weight;
            //SET VALUES COMMON TO ALL BASKET ITEMS
            item.TaxCodeId = product.TaxCodeId;
            item.Name = product.Name;
            item.Quantity = quantity;
            item.OrderItemType = CommerceBuilder.Orders.OrderItemType.Product;
            item.Shippable = product.Shippable;
            item.ProductId = productId;
            item.OptionList = optionList;
            item.KitList = kitList;

            // COPY MERCHANT INPUT FIELDS, PRODUCT TEMPLATE FIELDS           
            foreach (ProductTemplateField tf in product.TemplateFields)
            {
                if (!string.IsNullOrEmpty(tf.InputValue))
                {
                    InputField field = tf.InputField;
                    if (field.IsMerchantField && field.PersistWithOrder)
                    {
                        BasketItemInput itemInput = new BasketItemInput();
                        itemInput.BasketItemId = item.BasketItemId;
                        itemInput.InputFieldId = tf.InputFieldId;
                        itemInput.InputValue = tf.InputValue;
                        item.Inputs.Add(itemInput);
                    }
                }                
            }
            

            return item;
        }

        private static short GetExistingBasketCount(Product product)
        {
            BasketItemCollection items = Token.Instance.User.Basket.Items;
            foreach (BasketItem item in items)
            {
                if (item.ProductId == product.ProductId)
                {
                    return item.Quantity;
                }
            }
            // no item found
            return 0;
        }

    }
}
