using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Products;
using CommerceBuilder.Catalog;
using System.IO;
using CommerceBuilder.Utility;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// Class that implements feed provider for Yahoo! Shopping
    /// </summary>
    public class YahooShoppingFeed : FeedProviderBase
    {

        //private static YahooShoppingFeed _Instance = null;

        /// <summary>
        /// Private constructor. This class can not be instantiated
        /// </summary>
        public YahooShoppingFeed()
        {
			this.FeedOperationStatus = new FeedOperationStatus();
        }

        ///// <summary>
        ///// Singleton instance of YahooShoppingFeed class
        ///// </summary>
        //public static YahooShoppingFeed Instance
        //{ 
        //    get{
        //        if (_Instance == null)
        //        {
        //            _Instance = new YahooShoppingFeed();
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
            feedLine.Append("code\t");
            feedLine.Append("name\t");
            feedLine.Append("description\t");
            feedLine.Append("price\t");
            feedLine.Append("product-url\t");
            feedLine.Append("merchant-site-category\t");
            feedLine.Append("image-url\t");

            feedLine.Append("\r\n");
            return feedLine.ToString();
        }


        /// <summary>
        /// Generates the feed data for given products in Yahoo! Shopping feed format
        /// </summary>
        /// <param name="products">The products to generate feed for</param>
        /// <returns>Feed data generated</returns>
        protected override string GetFeedData(ProductCollection products)
        {           
            //http://searchmarketing.yahoo.com/shopsb/shpsb_specs.php
            
            //write header row
            StringWriter feedWriter = new StringWriter();
            StringBuilder feedLine;

            string storeUrl = Token.Instance.Store.StoreUrl;
            storeUrl = storeUrl + (storeUrl.EndsWith("/") ? "" : "/");

            string url, name, desc, price, imgurl, category, code;
            foreach (Product product in products)
            {
                url = product.NavigateUrl;
                if (url.StartsWith("~/"))
                {
                    url = url.Substring(2);
                }
                url = storeUrl + url;

                name = StringHelper.CleanupSpecialChars(product.Name);
                desc = StringHelper.CleanupSpecialChars(product.Summary);
                price = string.Format("{0:F2}",product.Price);

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
                    category = CategoryDataSource.GetCategoryName(product.Categories[0]);
                    category = StringHelper.CleanupSpecialChars(category);
                }
                else
                {
                    category = "None";
                }

                code = product.ProductId.ToString();
                feedLine = new StringBuilder();
                feedLine.Append(code + "\t" + name + "\t" + desc
                    + "\t" + price + "\t" + url + "\t" + category
                    + "\t" + imgurl);
                feedWriter.WriteLine(feedLine.ToString());
            }

            string returnData = feedWriter.ToString();
            feedWriter.Close();

            return returnData;

        }

    }
}
