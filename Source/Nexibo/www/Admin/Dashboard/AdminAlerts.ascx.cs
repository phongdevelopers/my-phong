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
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Shipping;
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using System.Web.Caching;
using System.Text;
using CommerceBuilder.Catalog;

public partial class Admin_Dashboard_AdminAlerts : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (Token.Instance.User.IsInRole(CommerceBuilder.Users.Role.AdminRoles))
        {
            BindAlerts();
        }
        else this.Controls.Clear();
    }

    private void BindAlerts()
    {
        List<string> alertList;
        DateTime cacheDate;
        CacheWrapper alertWrapper = Cache.Get("AdminAlerts") as CacheWrapper;
        if (alertWrapper == null)
        {
            alertList = new List<string>();

            //Check if installation directory still exists
            if (System.IO.Directory.Exists(Request.MapPath("~/Install")))
            {
                string alertText = "The 'Install' directory still exists in your store. It should be removed immediately after the Installation is complete.";
                alertList.Add(alertText);
            }

            // CHECK IF EMAIL TEMPLATES ARE CONFIGURED, WITHOUT EMAIL SERVER SETTINGS
            Store store = Token.Instance.Store;
            if ((store.EmailTemplates.Count > 0))
            {
                if (string.IsNullOrEmpty(store.Settings.SmtpServer))
                {
                    string alertText = "You have email templates configured, but you have not provided an SMTP (mail) server.  Without a serv" +
                    "er, email notifications cannot be sent.  <a href=\'Store/EmailTemplates/Settings.aspx\'>Click here</a> to configure " +
                    "email now.";
                    alertList.Add(alertText);
                }
            }
            //VALIDATE ORDER STATUSES
            //CHECK FOR A STATUS ATTACHED TO THE ORDER PLACED EVENT
            OrderStatus status = OrderStatusTriggerDataSource.LoadForStoreEvent(StoreEvent.OrderPlaced);
            if (status == null)
            {
                status = new OrderStatus();
                status.Name = "Payment Pending";
                status.DisplayName = "Payment Pending";
                status.IsActive = false;
                status.IsValid = true;
                status.Triggers.Add(new OrderStatusTrigger(StoreEvent.OrderPlaced));
                status.Save();
                alertList.Add("You did not have an order status assigned to the 'Order Placed' event, so one was created for you.  <a href=\"Store/OrderStatuses/Default.aspx\">Click here</a> to check the order status configuration for your store.");
            }
            //CHECK FOR A STATUS ATTACHED TO THE ORDER CANCELLED EVENT
            status = OrderStatusTriggerDataSource.LoadForStoreEvent(StoreEvent.OrderCancelled);
            if (status == null)
            {
                status = new OrderStatus();
                status.Name = "Cancelled";
                status.DisplayName = "Cancelled";
                status.IsActive = false;
                status.IsValid = false;
                status.Triggers.Add(new OrderStatusTrigger(StoreEvent.OrderCancelled));
                status.Save();
                alertList.Add("You did not have an order status assigned to the 'Order Cancelled' event, so one was created for you.  <a href=\"Store/OrderStatuses/Default.aspx\">Click here</a> to check the order status configuration for your store.");
            }
            //CHECK FOR ANY EVENTS THAT HAVE MORE THAN ONE STATUS ASSIGNED
            List<StoreEvent> overloadedEvents = OrderStatusTriggerDataSource.GetOverloadedEvents();
            if (overloadedEvents.Count > 0)
            {
                alertList.Add("You have an event that is linked to more than one order status.  It is strongly recommended that you only have one order status per event.  <a href=\"Store/OrderStatuses/Default.aspx\">Click here</a> to check the order status configuration for your store.");
            }

            //MAKE SURE AT LEAST ONE PRODUCT EXISTS
            int productCount = ProductDataSource.CountForStore();
            if (productCount == 0)
            {
                alertList.Add("You have not yet added any products in your store.  <a href=\"Catalog/Browse.aspx\">Click here</a> to manage your catalog now.");
            }

            //MAKE SURE AT LEAST ONE SHIPPING METHOD EXISTS
            int shipMethodCount = ShipMethodDataSource.CountForStore();
            if (shipMethodCount == 0)
            {
                alertList.Add("You do not have any shipping methods configured.  Your customers will not be able to complete checkout if the order contains any shippable products.  <a href=\"Shipping/Methods/Default.aspx\">Click here</a> to manage shipping methods now.");
            }

            //CHECK FOR LOW INVENTORY PRODUCTS
            int lowInventoryProducts = ProductInventoryDataSource.GetLowProductInventoryCount();
            if (lowInventoryProducts > 0)
            {
                alertList.Add("One or more products are at or below their low inventory warning level.  You can view these products <a href=\"Reports/LowInventory.aspx\">here</a>.");
            }

            //CHECK FOR PRESENCE OF ERRORS
            int errorCount = ErrorMessageDataSource.CountForStore();
            if (errorCount > 0)
            {
                string errorAlert = string.Format("There are {0} messages in your <a href=\"Help/ErrorLog.aspx\">error log</a>.  You should review these messages and take corrective action if necessary.", errorCount);
                alertList.Add(errorAlert);
            }

            //Check of SSL is not enabled
            StoreSettingCollection storeSettings = Store.GetCachedSettings();
            if (!storeSettings.SSLEnabled)
            {
                string alertText = "SSL is not enabled. Your store is currently being accessed over an insecure connection. <a href=\"Store/Security/Default.aspx\">Click Here</a> to change SSL settings.";
                alertList.Add(alertText);
            }

            //MAKE SURE ORDER NUMBER INCREMENT IS VALID
            if (store.OrderIdIncrement < 1)
            {
                string alertText = "The order number increment for your store was " + store.OrderIdIncrement + " (invalid).  The increment has been updated to 1.  <a href=\"Store/StoreSettings.aspx\">Click Here</a> to review this setting.";
                alertList.Add(alertText);
                store.OrderIdIncrement = 1;
                store.Save();
            }

            //ALERT FOR ORDER NUMBER PROBLEM
            int maxOrderNumber = StoreDataSource.GetMaxOrderNumber();
            int nextOrderNumber = StoreDataSource.GetNextOrderNumber(false);
            if (maxOrderNumber >= nextOrderNumber)
            {
                int newOrderNumber = maxOrderNumber + store.OrderIdIncrement;
                StoreDataSource.SetNextOrderNumber(newOrderNumber);
                string alertText = "The next order number of {0} is less than the highest assigned order number of {1}.  We have automatically increased your next order number to {2} to prevent errors.  <a href=\"Store/StoreSettings.aspx\">Click Here</a> to review this setting.";
                alertList.Add(string.Format(alertText, nextOrderNumber, maxOrderNumber, newOrderNumber));
            }

            //MAKE SURE A VALID ENCRYPTION KEY IS PRESENT
            bool encryptionKeyValid;
            try
            {
                encryptionKeyValid = EncryptionHelper.IsKeyValid(EncryptionHelper.GetEncryptionKey());
            }
            catch
            {
                encryptionKeyValid = false;
            }
            if (!encryptionKeyValid)
            {
                //ENCRYPTION KEY IS MISSING OR INVALID, SEE WHETHER WE ARE STORING CARD DATA
                if (storeSettings.EnableCreditCardStorage)
                {
                    string alertText = "Your store encryption key is missing or invalid, and you have not disabled storage of card data.  You should either <a href=\"Store/Security/EncryptionKey.aspx\">set the encryption key</a> or <a href=\"Store/Security/Default.aspx\">disable credit card storage</a>.";
                    alertList.Add(alertText);
                }
            }

            // ALERT FOR PRODUCT IMAGE LOOKUP BY SKU
            if(storeSettings.ImageSkuLookupEnabled)
            {
                // SEARCH FOR PRODUCTS MISSING SKU AND MISSING IMAGE URLs               
                ProductCollection products = ProductDataSource.LoadForCriteria("(Sku IS NULL OR Sku = '') AND (ImageUrl IS NULL OR IconUrl IS NULL OR ThumbnailUrl IS NULL) AND VisibilityId = " + (byte)CatalogVisibility.Public);
                if (products != null && products.Count > 0)
                {
                    StringBuilder textBuilder = new StringBuilder();
                    textBuilder.Append("Following product(s) are missing SKU, and also do not have image paths provided:<br>");
                    textBuilder.Append("<ul>");

                    int counter = 0; // product counter, show only first five products
                    foreach (Product product in products)
                    {
                        counter++;                        
                        textBuilder.Append("<li><a href=\"products/EditProduct.aspx?ProductId=" + product.ProductId + "\">" + product.Name + "</a>.</li>");
                        if (counter >= 5) break;
                    }                    
                    textBuilder.Append("<ul>");
                    alertList.Add(textBuilder.ToString());
                }
            }
            //UPDATE CACHE
            alertWrapper = new CacheWrapper(alertList);
            Cache.Remove("AdminAlerts");
            Cache.Add("AdminAlerts", alertWrapper, null, DateTime.UtcNow.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }
        else
        {
            alertList = (List<string>)alertWrapper.CacheValue;
        }
        cacheDate = alertWrapper.CacheDate;

        if (alertList.Count == 0) alertList.Add("no alerts available");
        AlertList.DataSource = alertList;
        AlertList.DataBind();
        CachedAt.Text = string.Format("{0:g}", cacheDate);
    }

    protected void RefreshButton_Click(object sender, EventArgs e)
    {
        Cache.Remove("AdminAlerts");
        BindAlerts();
    }
}