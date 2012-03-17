using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// DataSource class for OrderItem objects
    /// </summary>
    [DataObject(true)]
    public partial class OrderItemDataSource
    {
        /// <summary>
        /// Gets the order items that would result from the addition of the given product to an order.
        /// </summary>
        /// <param name="productId">The id of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <returns>The order items that would result from the addition of the given product to an order</returns>
        public static List<OrderItem> CreateForProduct(int productId, short quantity)
        {
            return OrderItemDataSource.CreateForProduct(productId, quantity, string.Empty, null);
        }

        /// <summary>
        /// Gets the order items that would result from the addition of the given product to an order.
        /// </summary>
        /// <param name="productId">The id of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <param name="optionList">List of option choice ids if this is a variant</param>
        /// <param name="kitList">List of kit products if it is a Kit</param>
        /// <returns>The order items that would result from the addition of the given product to an order</returns>
        public static List<OrderItem> CreateForProduct(int productId, short quantity, string optionList, string kitList)
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            Product product = ProductDataSource.Load(productId);
            if (product != null)
            {
                //CREATE THE BASE ORDER ITEM
                OrderItem baseItem = new OrderItem();
                baseItem.Name = product.Name;
                baseItem.OrderItemType = OrderItemType.Product;
                baseItem.ProductId = productId;
                baseItem.TaxCodeId = product.TaxCodeId;
                baseItem.Quantity = quantity;
                baseItem.ShippableId = product.ShippableId;
                //CALCULATE THE PRICE OF THE PRODUCT
                ProductCalculator pcalc = ProductCalculator.LoadForProduct(productId, quantity, optionList, kitList);
                baseItem.Sku = pcalc.Sku;
                baseItem.Price = pcalc.Price;
                baseItem.Weight = pcalc.Weight;
                //CHECK PRODUCT VARIANT
                ProductVariant variant = ProductVariantDataSource.LoadForOptionList(productId, optionList);
                if (variant != null)
                {
                    baseItem.OptionList = optionList;
                    baseItem.VariantName = variant.VariantName;
                }
                //CHECK FOR DIGITAL GOODS
                foreach (ProductDigitalGood dg in product.DigitalGoods)
                {
                    if (dg.DigitalGood != null && (String.IsNullOrEmpty(baseItem.OptionList) || baseItem.OptionList == dg.OptionList))
                    {
                        OrderItemDigitalGood oidg = new OrderItemDigitalGood();
                        oidg.OrderItemId = baseItem.OrderItemId;
                        oidg.DigitalGoodId = dg.DigitalGoodId;
                        oidg.Name = dg.DigitalGood.Name;
                        baseItem.DigitalGoods.Add(oidg);
                    }
                }
                orderItems.Add(baseItem);

                //CHECK FOR KIT ITEMS
                int[] kitProductIds = AlwaysConvert.ToIntArray(kitList);
                if (kitProductIds != null && kitProductIds.Length > 0)
                {
                    LSDecimal baseItemPrice = baseItem.Price;
                    LSDecimal baseItemWeight = baseItem.Weight;
                    foreach (int kitProductId in kitProductIds)
                    {
                        KitProduct kitProduct = KitProductDataSource.Load(kitProductId);
                        if (kitProduct != null)
                        {
                            OrderItem kitItem = new OrderItem();
                            kitItem.Name = kitProduct.DisplayName;
                            kitItem.OrderItemType = OrderItemType.Product;
                            kitItem.ParentItemId = baseItem.OrderItemId;
                            kitItem.ProductId = kitProduct.ProductId;
                            kitItem.OptionList = kitProduct.OptionList;
                            kitItem.Quantity = (short)(kitProduct.Quantity * baseItem.Quantity);
                            kitItem.Sku = kitProduct.Product.Sku;
                            kitItem.TaxCodeId = kitProduct.Product.TaxCodeId;
                            kitItem.Price = kitProduct.CalculatedPrice / kitProduct.Quantity;
                            kitItem.Weight = kitProduct.CalculatedWeight / kitProduct.Quantity;
                            //CHECK FOR DIGITAL GOODS
                            foreach (DigitalGood dg in kitItem.Product.DigitalGoods)
                            {
                                OrderItemDigitalGood oidg = new OrderItemDigitalGood();
                                oidg.OrderItemId = kitItem.OrderItemId;
                                oidg.DigitalGoodId = dg.DigitalGoodId;
                                kitItem.DigitalGoods.Add(oidg);
                            }
                            baseItemPrice -= kitProduct.CalculatedPrice;
                            baseItemWeight -= kitProduct.CalculatedWeight;
                            orderItems.Add(kitItem);
                        }
                    }
                    baseItem.Price = baseItemPrice;
                    baseItem.Weight = baseItemWeight;
                }
            }
            return orderItems;
        }

        /// <summary>
        /// Gets a collection of order items that are associated with a subscription plan.
        /// </summary>
        /// <param name="orderId">The id of the order to get items for.</param>
        /// <returns>A collection of order items that are associated with a subscription plan.</returns>
        public static OrderItemCollection LoadSubscriptionItems(int orderId)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + OrderItem.GetColumnNames("OI"));
            selectQuery.Append(" FROM ac_OrderItems OI INNER JOIN ac_SubscriptionPlans SP ON OI.ProductId = SP.ProductId");
            selectQuery.Append(" WHERE OI.OrderId = @orderId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderId", System.Data.DbType.Int32, orderId);
            //EXECUTE THE COMMAND
            OrderItemCollection results = new OrderItemCollection();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    OrderItem orderItem = new OrderItem();
                    OrderItem.LoadDataReader(orderItem, dr);
                    results.Add(orderItem);
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Loads a collection of OrderItem objects for given order item type
        /// </summary>
        /// <param name="orderItemType">Type of OrderItem objects to load</param>
        /// <param name="startDate">Start date for the date range</param>
        /// <param name="endDate">End date for the date range</param>
        /// <returns>A collection of OrderItem objects for given order item type</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderItemCollection LoadForOrderItemType(OrderItemType orderItemType, DateTime startDate, DateTime endDate)
        {
            return LoadForOrderItemType(orderItemType, startDate, endDate, 0, 0);
        }
        
        /// <summary>
        /// Loads a collection of OrderItem objects for given order item type
        /// </summary>
        /// <param name="orderItemType">Type of OrderItem objects to load</param>
        /// <param name="startDate">Start date for the date range</param>
        /// <param name="endDate">End date for the date range</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of OrderItem objects for given order item type</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderItemCollection LoadForOrderItemType(OrderItemType orderItemType, DateTime startDate, DateTime endDate, int maximumRows, int startRowIndex)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + OrderItem.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_OrderItems");
            selectQuery.Append(" WHERE OrderItemTypeId = @orderItemType");
            selectQuery.Append(" AND OrderId IN ( SELECT OrderId From ac_Orders WHERE StoreId = @storeId");
            if (startDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                startDate = LocaleHelper.FromLocalTime(startDate);
                selectQuery.Append(" AND OrderDate >= @startDate");
            }
            if (endDate > DateTime.MinValue)
            {
                //CONVERT DATE TO UTC
                endDate = LocaleHelper.FromLocalTime(endDate);
                selectQuery.Append(" AND OrderDate <= @endDate");
            }
            selectQuery.Append(")");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderItemType", System.Data.DbType.Int32, orderItemType);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (startDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@startDate", System.Data.DbType.DateTime, startDate);
            if (endDate > DateTime.MinValue) database.AddInParameter(selectCommand, "@endDate", System.Data.DbType.DateTime, endDate);
            //EXECUTE THE COMMAND
            OrderItemCollection results = new OrderItemCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        OrderItem orderItem = new OrderItem();
                        OrderItem.LoadDataReader(orderItem, dr);
                        results.Add(orderItem);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }


    }

}
