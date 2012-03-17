using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// This class implements the Feed Provider for Shopping.com feed
    /// </summary>
    public class ShoppingComFeed : FeedProviderBase
    {

        //private static ShoppingComFeed _Instance = null;
        /// <summary>
        /// Default constructor for Shopping.com feed provider
        /// </summary>
        public ShoppingComFeed()
        {
            this.FeedOperationStatus = new FeedOperationStatus();
        }

        ///// <summary>
        ///// The singleton instance of the ShoppingComFeed proider
        ///// </summary>
        //public static ShoppingComFeed Instance
        //{ 
        //    get{
        //        if (_Instance == null)
        //        {
        //            _Instance = new ShoppingComFeed();
        //        }
        //        return _Instance;
        //    }
        //}

        /// <summary>
        /// Returns the header row for the feed
        /// </summary>
        /// <returns>The header row.</returns>
        protected override string GetHeaderRow()
        {
            StringBuilder feedLine = new StringBuilder();
            feedLine.Append("mpn,");
            feedLine.Append("upc,");
            feedLine.Append("manufacturer,");
            feedLine.Append("product name,");
            feedLine.Append("product description,");
            feedLine.Append("price,");
            feedLine.Append("stock,");
            feedLine.Append("stock description,");
            feedLine.Append("product url,");
            feedLine.Append("image url,");
            feedLine.Append("category,");
            feedLine.Append("shipping rate");
            feedLine.Append("\r\n");

            return feedLine.ToString();
        }

        /// <summary>
        /// Generates the feed data for given products
        /// </summary>
        /// <param name="products">The products to generate feed for</param>
        /// <returns>Feed data generated</returns>
        protected override string GetFeedData(ProductCollection products)
        {
            //write header row
            StringWriter feedWriter = new StringWriter();
            StringBuilder feedLine;

            string storeUrl = Token.Instance.Store.StoreUrl;
            storeUrl = storeUrl + (storeUrl.EndsWith("/") ? "" : "/");

            string url, name, desc, price, imgurl, category, mpn, upc, manufacturer, stock, stockdesc, shiprate;

            foreach (Product product in products)
            {
                url = product.NavigateUrl;
                if (url.StartsWith("~/"))
                {
                    url = url.Substring(2);
                }
                url = storeUrl + url;

                name = CleanupText(product.Name);
                desc = CleanupText(product.Summary);
                if (desc.Length > 1024)
                {
                    desc = desc.Substring(0, 1024);
                }

                price = string.Format("{0:F2}", product.Price);

                imgurl = product.ImageUrl;
                if (!string.IsNullOrEmpty(imgurl) && !IsAbsoluteURL(imgurl))
                {
                    if (imgurl.StartsWith("~/"))
                    {
                        imgurl = imgurl.Substring(2);
                    }
                    imgurl = storeUrl + imgurl;
                }

                if (product.Categories.Count > 0)
                {
                    List<CatalogPathNode> path = CatalogDataSource.GetPath(product.Categories[0], false);
                    category = "";
                    foreach (CatalogPathNode item in path)
                    {
                        if (item == null) continue;
                        if (category.Length > 0)
                        {
                            category = category + "->" + CleanupText(item.Name);
                        }
                        else
                        {
                            category = CleanupText(item.Name);
                        }
                    }
                }
                else
                {
                    category = "None";
                }

                manufacturer = "";
                if (product.Manufacturer != null)
                {
                    manufacturer = CleanupText(product.Manufacturer.Name);
                }
                
                mpn = product.ModelNumber;

                if (product.InventoryMode == InventoryMode.Product)
                {
                    stock = product.InStock > 0 ? "Y" : "N";
                }

                // IF INVENTORY IS DISABLED OR INVENTORY MODE IS TRACK VARIANTS, ASSUME INVENTORY IS AVAILABLE
                else stock = "Y";

                //TODO
                stockdesc = "New";

                upc = this.IsUpcCode(product.Sku) ? product.Sku : string.Empty;
                shiprate = "";

                feedLine = new StringBuilder();
                feedLine.Append(mpn + "," + upc + ",\"" + manufacturer
                    + "\",\"" + name + "\",\"" + desc + "\"," + price
                    + "," + stock + "," + stockdesc + "," + url
                    + "," + imgurl + ",\"" + category + "\"," + shiprate);
                feedWriter.WriteLine(feedLine.ToString());
            }

            string returnData = feedWriter.ToString();
            feedWriter.Close();

            return returnData;
        }

        private static string CleanupText(string data)
        {
            if (data == null) return "";
            string result = StringHelper.CleanupSpecialChars(data);
            result = result.Replace("\"", "\"\""); 
            return result;
        }
    }
}
