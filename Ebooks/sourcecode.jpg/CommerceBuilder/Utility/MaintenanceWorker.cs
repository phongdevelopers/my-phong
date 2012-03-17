using System;
using System.Text;
using System.Web;
using System.Data;
using System.Data.Common;
using System.IO;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Configuration;
using CommerceBuilder.Reporting;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using System.Collections.Generic;
using CommerceBuilder.Catalog;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Summary description for MaintenanceWorker
    /// </summary>
    public static class MaintenanceWorker
    {
        /// <summary>
        /// Runs all maintenance routines for the current store context
        /// </summary>
        public static void RunMaintenance()
        {
            Store store = Token.Instance.Store;
            if (store != null && store.StoreId > 0)
            {
                Logger.Info("Running maintenance routines");
                try { MaintainUsers(); }
                catch (Exception ex)
                {
                    Logger.Error("Error in user maintenance", ex);
                }
                try { MaintainAnonymousCheckouts(); }
                catch (Exception ex)
                {
                    Logger.Error("Error in anonymous checkout maintenance", ex);
                }
                try { ClearAccountData(); }
                catch (Exception ex)
                {
                    Logger.Error("Error in payment account data maintenance", ex);
                }
                try { MaintainPageViewLog(); }
                catch (Exception ex)
                {
                    Logger.Error("Error in page tracking log maintenance", ex);
                }
                try { MaintainEmailSubscriptions(); }
                catch (Exception ex)
                {
                    Logger.Error("Error in email list maintenance", ex);
                }
                try { ExpireSubscriptions(); }
                catch (Exception ex)
                {
                    Logger.Error("Error in subscription maintenance", ex);
                }
                try { UpdateExchangeRates(); }
                catch (Exception ex)
                {
                    Logger.Error("Error updating currency exchange rates", ex);
                }
                try { MaintainAuditLog(); }
                catch (Exception ex)
                {
                    Logger.Error("Error maintaining audit log", ex);
                }
                try { MaintainCatalog(); }
                catch (Exception ex)
                {
                    Logger.Error("Error maintaining catalog nodes", ex);
                }
                try { MaintainCustomUrls(); }
                catch (Exception ex)
                {
                    Logger.Error("Error maintaining custom urls", ex);
                }
                try { MaintainInstallFolder(); }
                catch (Exception ex)
                {
                    Logger.Error("Error maintaining 'Install' folder", ex);                    
                }
                Logger.Info("Maintenance routines complete");
            }
        }

        /// <summary>
        /// If UpdateKey.aspx and UpdateKey.aspx.cs are the only files in Install folder then we can
        /// safely delete the 'Install' folder.
        /// </summary>
        private static void MaintainInstallFolder()
        {
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Install\\");            
            if (Directory.Exists(directory) && 
                Directory.GetFiles(directory).Length == 2
                && File.Exists(directory + "UpdateKey.aspx")
                && File.Exists(directory + "UpdateKey.aspx.cs"))
            {
                //Remove the 'Install' folder
                Directory.Delete(directory,true);
            }
        }

        /// <summary>
        /// Ensures there are no catalog nodes without corresponding objects
        /// in the database.  This cleans the whole database, irrespective of store
        /// context.
        /// </summary>
        private static void MaintainCatalog()
        {
            Database database = Token.Instance.Database;
            int result=0;
            DbCommand deleteCommand;

            //DELETE NODES WITH INVALID PRODUCTS
            //string sql = "DELETE FROM ac_CatalogNodes WHERE CatalogNodeId IN (SELECT CatalogNodeId FROM ac_CatalogNodes CN LEFT JOIN ac_Products P ON CN.CatalogNodeId = P.ProductId WHERE P.ProductId IS NULL and CN.CatalogNodeTypeId = 1) AND CatalogNodeTypeId = 1";
            string sql = "SELECT CatalogNodeId FROM ac_CatalogNodes CN LEFT JOIN ac_Products P ON CN.CatalogNodeId = P.ProductId WHERE P.ProductId IS NULL and CN.CatalogNodeTypeId = 1";
            List<int> idList = GetIdList(sql);
            foreach (int id in idList)
            {
                sql = "DELETE FROM ac_CatalogNodes WHERE CatalogNodeId = @nodeId AND CatalogNodeTypeId = 1";
                deleteCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(deleteCommand, "@nodeId", System.Data.DbType.Int32, id);
                result += database.ExecuteNonQuery(deleteCommand);
            }
            
            //DELETE NODES WITH INVALID WEBPAGES
            //sql = "DELETE FROM ac_CatalogNodes WHERE CatalogNodeId IN (SELECT CatalogNodeId FROM ac_CatalogNodes CN LEFT JOIN ac_Webpages W ON CN.CatalogNodeId = W.WebpageId WHERE W.WebpageId IS NULL and CN.CatalogNodeTypeId = 2) AND CatalogNodeTypeId = 2";
            sql = "SELECT CatalogNodeId FROM ac_CatalogNodes CN LEFT JOIN ac_Webpages W ON CN.CatalogNodeId = W.WebpageId WHERE W.WebpageId IS NULL and CN.CatalogNodeTypeId = 2";
            idList = GetIdList(sql);
            foreach (int id in idList)
            {
                sql = "DELETE FROM ac_CatalogNodes WHERE CatalogNodeId = @nodeId AND CatalogNodeTypeId = 2";
                deleteCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(deleteCommand, "@nodeId", System.Data.DbType.Int32, id);
                result += database.ExecuteNonQuery(deleteCommand);
            }

            //DELETE NODES WITH INVALID LINKS
            //sql = "DELETE FROM ac_CatalogNodes WHERE CatalogNodeId IN (SELECT CatalogNodeId FROM ac_CatalogNodes CN LEFT JOIN ac_Links L ON CN.CatalogNodeId = L.LinkId WHERE L.LinkId IS NULL and CN.CatalogNodeTypeId = 3) AND CatalogNodeTypeId = 3";
            sql = "SELECT CatalogNodeId FROM ac_CatalogNodes CN LEFT JOIN ac_Links L ON CN.CatalogNodeId = L.LinkId WHERE L.LinkId IS NULL and CN.CatalogNodeTypeId = 3";
            idList = GetIdList(sql);
            foreach (int id in idList)
            {
                sql = "DELETE FROM ac_CatalogNodes WHERE CatalogNodeId = @nodeId AND CatalogNodeTypeId = 3";
                deleteCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(deleteCommand, "@nodeId", System.Data.DbType.Int32, id);
                result += database.ExecuteNonQuery(deleteCommand);
            }
            
            //DELETE NODES WITH INVALID Categories
            //sql = "DELETE FROM ac_CatalogNodes WHERE CatalogNodeId IN (SELECT CatalogNodeId FROM ac_CatalogNodes CN LEFT JOIN ac_Categories C ON CN.CatalogNodeId = C.CategoryId WHERE C.CategoryId IS NULL and CN.CatalogNodeTypeId = 0) AND CatalogNodeTypeId = 0";
            sql = "SELECT CatalogNodeId FROM ac_CatalogNodes CN LEFT JOIN ac_Categories C ON CN.CatalogNodeId = C.CategoryId WHERE C.CategoryId IS NULL and CN.CatalogNodeTypeId = 0";
            idList = GetIdList(sql);
            foreach (int id in idList)
            {
                sql = "DELETE FROM ac_CatalogNodes WHERE CatalogNodeId = @nodeId AND CatalogNodeTypeId = 0";
                deleteCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(deleteCommand, "@nodeId", System.Data.DbType.Int32, id);
                result += database.ExecuteNonQuery(deleteCommand);
            }

            //DELETE INVALID CATALOG NODES, CATEGORIES CAN ONLY HAVE ONE PARENT (BUG 6127)
            sql = "SELECT CN.CategoryId, CN.CatalogNodeId FROM ac_CatalogNodes CN INNER JOIN ac_Categories C ON CN.CatalogNodeId=C.CategoryId WHERE CN.CatalogNodeTypeId=0 AND CN.CategoryId <> C.ParentId";
            List<NodeRecord> nrList = GetInvalidCategoryNodes(sql);
            foreach (NodeRecord nr in nrList)
            {
                sql = "DELETE FROM ac_CatalogNodes WHERE CategoryId = @catId AND CatalogNodeId = @catNodeId AND CatalogNodeTypeId = @catNodeTypeId ";
                deleteCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(deleteCommand, "@catId", System.Data.DbType.Int32, nr.CategoryId);
                database.AddInParameter(deleteCommand, "@catNodeId", System.Data.DbType.Int32, nr.CatalogNodeId);
                database.AddInParameter(deleteCommand, "@catNodeTypeId", System.Data.DbType.Int32, nr.CatalogNodeTypeId);
                result += database.ExecuteNonQuery(deleteCommand);
            }

            if (result > 0)
                Logger.Info("Catalog maintenance removed " + result.ToString() + " invalid catalog nodes");
        }

        /// <summary>
        /// Maintains the custom URLs to remove orphaned custom URLs
        /// </summary>
        private static void MaintainCustomUrls()
        {
            List<int> customProductUrlIds = GetNonOrphanedCustomUrlIds(CatalogNodeType.Product);
            DeleteOrphanedCustomUrls(customProductUrlIds, CatalogNodeType.Product);
            List<int> customCategoryUrlIds = GetNonOrphanedCustomUrlIds(CatalogNodeType.Category);
            DeleteOrphanedCustomUrls(customCategoryUrlIds, CatalogNodeType.Category);
            List<int> customWebpageUrlIds = GetNonOrphanedCustomUrlIds(CatalogNodeType.Webpage);
            DeleteOrphanedCustomUrls(customWebpageUrlIds, CatalogNodeType.Webpage);
            List<int> customLinkUrlIds = GetNonOrphanedCustomUrlIds(CatalogNodeType.Link);
            DeleteOrphanedCustomUrls(customLinkUrlIds, CatalogNodeType.Link);
        }

        private static List<int> GetNonOrphanedCustomUrlIds(CatalogNodeType catalogNodeType) 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT CU.CustomUrlId ");
            sb.Append(" FROM ac_CustomUrls AS CU ");
            switch(catalogNodeType)
            {
                case CatalogNodeType.Product:
                    sb.Append(" JOIN ac_Products AS P ON CU.CatalogNodeId = P.ProductId ");
                    break;

                case CatalogNodeType.Category:
                    sb.Append(" JOIN ac_Categories AS C ON CU.CatalogNodeId = C.CategoryId ");
                    break;

                case CatalogNodeType.Webpage:
                    sb.Append(" JOIN ac_Webpages AS WP ON CU.CatalogNodeId = WP.WebpageId ");
                    break;

                case CatalogNodeType.Link:
                    sb.Append(" JOIN ac_Links AS L ON CU.CatalogNodeId = L.LinkId ");
                    break;
            }
            sb.Append(string.Format("WHERE CU.StoreId= {0} AND CatalogNodeTypeId = {1} ",Token.Instance.StoreId, (byte)catalogNodeType));
            return GetIdList(sb.ToString());
        }

        private static void DeleteOrphanedCustomUrls(List<int> nonOrphanedCustomUrlIds,CatalogNodeType catalogNodeType) 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" DELETE FROM ac_CustomUrls WHERE StoreId= @storeId ");
            sb.Append(" AND CatalogNodeTypeId=@catalogNodeType ");
            if(nonOrphanedCustomUrlIds.Count > 0)
                sb.Append(string.Format(" AND CustomUrlId NOT IN ({0}) ", string.Join(",", nonOrphanedCustomUrlIds.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray())));
            Database database = Token.Instance.Database;
            DbCommand dbCommand = database.GetSqlStringCommand(sb.ToString());
            database.AddInParameter(dbCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(dbCommand, "@catalogNodeType", DbType.Byte, (byte)catalogNodeType);
            database.ExecuteNonQuery(dbCommand);
        }


        /// <summary>
        /// Updates exchange rates for any currencies using autoupdate option
        /// </summary>
        private static void UpdateExchangeRates()
        {
            Store store = Token.Instance.Store;
            foreach (Currency currency in store.Currencies)
            {
                if (currency.AutoUpdate)
                {
                    TimeSpan elapsedTime = DateTime.Now.Subtract(currency.LastUpdate);
                    if (elapsedTime.Days > 0)
                    {
                        try
                        {
                            currency.UpdateExchangeRate(true);
                            //LOG RESULTS
                            Logger.Info("Exchange rate updated for " + currency.Name);
                        }
                        catch (Exception ex)
                        {
                            //LOG RESULTS
                            Logger.Warn("Could not update exchange rate for " + currency.Name, ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes users created by anonymous checkout process
        /// </summary>
        private static void MaintainAnonymousCheckouts()
        {
            Store store = Token.Instance.Store;
            StoreSettingCollection settings = store.Settings;
            if (settings.AnonymousUserLifespan > 0)
            {
                DateTime expireDate = DateTime.UtcNow.AddDays(-1 * settings.AnonymousUserLifespan);
                Database database = Token.Instance.Database;
                
                //GET ANONYMOUS CHECKOUT USER IDS
                string sql = "SELECT UserId FROM ac_Users WHERE StoreId = @storeId AND UserName LIKE 'zz_anonymous_%' AND (LastActivityDate IS NULL OR LastActivityDate < @expireDate)";
                DbCommand dbCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(dbCommand, "@storeId", System.Data.DbType.Int32, store.StoreId);
                database.AddInParameter(dbCommand, "@expireDate", System.Data.DbType.DateTime, expireDate);
                List<int> idList = GetIdList(dbCommand);
                
                //DELETE THE USERS WITH GIVEN IDS
                DeleteUsers(idList);
                
                int expiredCount = idList.Count;

                //LOG RESULTS
                if (expiredCount > 0)
                    Logger.Info("Anonymous checkout maintenance cleared " + expiredCount.ToString() + " expired anonymous users, last active before " + expireDate.ToString());
            }
        }

        /// <summary>
        /// Removes inactive users that have passed their lifespan for the current store
        /// </summary>
        private static void MaintainUsers()
        {
            Store store = Token.Instance.Store;
            DateTime expireDate;
            StoreSettingCollection settings = store.Settings;
            Database database = Token.Instance.Database;
            string sql;
            List<int> idList;

            if (settings.AnonymousUserLifespan > 0)
            {
                expireDate = DateTime.UtcNow.AddDays(-1 * settings.AnonymousUserLifespan);

                //GET USER IDS FOR ANONYMOUS USERS WITHOUT AFFILIATE ASSOCIATION
                sql = "SELECT UserId FROM ac_Users WHERE StoreId = @storeId AND IsAnonymous = 1 AND AffiliateId IS NULL AND (LastActivityDate IS NULL OR LastActivityDate < @expireDate)";
                DbCommand dbCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(dbCommand, "@storeId", System.Data.DbType.Int32, store.StoreId);
                database.AddInParameter(dbCommand, "@expireDate", System.Data.DbType.DateTime, expireDate);
                idList = GetIdList(dbCommand);

                //DELETE THE USERS FOUND
                DeleteUsers(idList);

                int expiredCount = idList.Count;

                //LOG RESULTS
                if (expiredCount > 0)
                    Logger.Info("User maintenance cleared " + expiredCount.ToString() + " expired anonymous users, last active before " + expireDate.ToString());
            }

            if (settings.AnonymousAffiliateUserLifespan > 0)
            {
                expireDate = DateTime.UtcNow.AddDays(-1 * settings.AnonymousAffiliateUserLifespan);

                //GET USER IDS FOR ANONYMOUS USERS WITH AFFILIATE ASSOCIATION
                sql = "SELECT UserId FROM ac_Users WHERE StoreId = @storeId AND IsAnonymous = 1 AND AffiliateId IS NOT NULL AND (LastActivityDate IS NULL OR LastActivityDate < @expireDate)";
                DbCommand dbCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(dbCommand, "@storeId", System.Data.DbType.Int32, store.StoreId);
                database.AddInParameter(dbCommand, "@expireDate", System.Data.DbType.DateTime, expireDate);
                idList = GetIdList(dbCommand);

                //DELETE THE USERS FOUND
                DeleteUsers(idList);

                int expiredCount = idList.Count;

                //LOG RESULTS
                if (expiredCount > 0)
                    Logger.Info("User maintenance cleared " + expiredCount.ToString() + " expired affiliate anonymous users, last active before " + expireDate.ToString());
            }

            MerchantPasswordPolicy policy = new MerchantPasswordPolicy();
            if (policy.InactivePeriod > 0)
            {
                //LOOK FOR INACTIVE USERS IN NON-CONSUMER GROUPS (I.E. HAVE ROLES ASSIGNED)
                sql = "SELECT UserId FROM ac_Users WHERE LastActivityDate < @expireDate AND IsApproved = 1 AND UserId IN (SELECT DISTINCT U.UserId FROM (ac_Users U INNER JOIN ac_UserGroups UG ON U.UserId = UG.UserId) INNER JOIN ac_GroupRoles GR ON UG.GroupId = GR.GroupId WHERE U.StoreId = @storeId)";
                DbCommand selCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(selCommand, "@storeId", System.Data.DbType.Int32, store.StoreId);
                database.AddInParameter(selCommand, "@expireDate", System.Data.DbType.DateTime, DateTime.UtcNow.AddMonths(-1 * policy.InactivePeriod));
                idList = GetIdList(selCommand);

                int expiredCount = 0;
                DbCommand updCommand;
                foreach (int id in idList)
                {
                    sql = "UPDATE ac_Users SET IsApproved = 0 WHERE UserId = @userId";
                    updCommand = database.GetSqlStringCommand(sql);
                    database.AddInParameter(updCommand, "@userId", System.Data.DbType.Int32, id);
                    expiredCount += database.ExecuteNonQuery(updCommand);
                }

                if (expiredCount > 0)
                    Logger.Info("User maintenance disabled " + expiredCount.ToString() + " expired merchant accounts.");
            }
        }

        /// <summary>
        /// Removes users from groups that were assigned based on the purchase of 
        /// a subscription, when the subscription has expired.
        /// </summary>
        private static void ExpireSubscriptions()
        {
            //DELETE ANY GROUP ASSIGNMENTS FOR EXPIRED SUBSCRIPTIONS
            string selectSql = "SELECT S.SubscriptionId FROM ac_Subscriptions S, ac_Products P WHERE S.ProductId = P.ProductId AND P.StoreId = @storeId AND S.ExpirationDate IS NOT NULL AND S.ExpirationDate < @currentDate";
            string sql = "DELETE FROM ac_UserGroups WHERE SubscriptionId IN (" + selectSql + ")";
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand(sql);
            database.AddInParameter(deleteCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(deleteCommand, "@currentDate", System.Data.DbType.DateTime, DateTime.UtcNow);
            database.ExecuteNonQuery(deleteCommand);
            //DELETE EXPIRED SUBSCRIPTIONS
            sql = "DELETE FROM ac_Subscriptions WHERE SubscriptionId IN (" + selectSql + ")";
            database = Token.Instance.Database;
            deleteCommand = database.GetSqlStringCommand(sql);
            database.AddInParameter(deleteCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(deleteCommand, "@currentDate", System.Data.DbType.DateTime, DateTime.UtcNow);
            int expiredCount = database.ExecuteNonQuery(deleteCommand);
            if (expiredCount > 0)
                Logger.Info("Subscription maintenance cleared " + expiredCount.ToString() + " expired subscriptions.");
        }

        /// <summary>
        /// Removes account data associated with completed payments that has past the lifespan
        /// </summary>
        private static void ClearAccountData()
        {
            Database database = Token.Instance.Database;
            Store store = Token.Instance.Store;
            //GET THE NUMBER OF DAYS ACCOUNT INFORMATION REMAINS AFTER COMPLETION
            int paymentLifespan = store.Settings.PaymentLifespan;
            //GET THE OLDEST DATE THAT ACCOUNT INFORMATION CAN REMAIN (CURRENT DATE LESS LIFESPAN)
            DateTime expiredDate = DateTime.UtcNow.Add(new TimeSpan(-1 * paymentLifespan, 0, 0, 0));
            //CLEAR PAYMENT ACCOUNT DATA

            //GET IDS OF PAYMENTS THAT NEED TO BE CLEARED OF ACCOUNT DATA
            StringBuilder selSql = new StringBuilder();
            selSql.Append("SELECT PaymentId ");
            selSql.Append(" FROM ac_Payments, ac_Orders");
            selSql.Append(" WHERE ac_Payments.OrderId = ac_Orders.OrderId");
            selSql.Append(" AND ac_Orders.StoreId = @storeId");
            selSql.Append(" AND ac_Payments.EncryptedAccountData IS NOT NULL");
            selSql.Append(" AND ac_Payments.CompletedDate <= @expiredDate");
            DbCommand selCommand = database.GetSqlStringCommand(selSql.ToString());
            database.AddInParameter(selCommand, "@storeId", System.Data.DbType.Int32, store.StoreId);
            database.AddInParameter(selCommand, "@expiredDate", System.Data.DbType.DateTime, expiredDate);
            List<int> idList = GetIdList(selCommand);

            string sql;
            DbCommand clearCommand;
            int clearCount = 0;
            foreach (int id in idList)
            {
                sql = "UPDATE ac_Payments SET EncryptedAccountData = NULL WHERE PaymentId = @paymentId";
                clearCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(clearCommand, "@paymentId", System.Data.DbType.Int32, id);
                clearCount += database.ExecuteNonQuery(clearCommand);
            }

            if (clearCount > 0)
                Logger.Info("Payment maintenance cleared " + clearCount.ToString() + " expired payments, completed before " + expiredDate.ToString());
        }

        /// <summary>
        /// Maintains the page view log, archiving and truncating expired records
        /// </summary>
        private static void MaintainPageViewLog()
        {
            Store store = Token.Instance.Store;
            DateTime utcDayStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
            int historyDays = (int)(Math.Abs(store.Settings.PageViewTrackingDays) * -1);
            DateTime oldestLogDate = utcDayStart.AddDays(historyDays);
            int records = CountPageViewLog(oldestLogDate);
            if (records > 0)
            {
                if (store.Settings.PageViewTrackingSaveArchive)
                {
                    //SAVE THE LOG
                    SavePageViewLog(oldestLogDate);
                }
                //CLEAR THE LOG
                TrimPageViewLog(oldestLogDate);
            }
        }

        /// <summary>
        /// Counts the number of records in the page view log that occurred
        /// prior to the specified date
        /// </summary>
        /// <param name="beforeDate">Records older than this date are counted</param>
        /// <returns>The number of records in the page view log that occured
        /// prior to the specified date</returns>
        private static int CountPageViewLog(DateTime beforeDate)
        {
            string sql = "SELECT COUNT(*) As NumRecords FROM ac_PageViews WHERE StoreId = @storeId AND ActivityDate < @beforeDate";
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sql);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@beforeDate", System.Data.DbType.DateTime, beforeDate);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Trims records from the page view log that occured before the specified date
        /// </summary>
        /// <param name="beforeDate">Records older than this date are truncated</param>
        private static void TrimPageViewLog(DateTime beforeDate)
        {
            string sql = "DELETE FROM ac_PageViews WHERE StoreId = @storeId AND ActivityDate < @beforeDate";
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand(sql);
            database.AddInParameter(deleteCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(deleteCommand, "@beforeDate", System.Data.DbType.DateTime, beforeDate);
            int expiredCount = database.ExecuteNonQuery(deleteCommand);
            if (expiredCount > 0)
                Logger.Info("Page view maintenance cleared " + expiredCount.ToString() + " expired records.");
        }

        /// <summary>
        /// Saves records to file from the page view log that occured before the specified date
        /// </summary>
        /// <param name="beforeDate">Records older than this date are saved</param>
        private static void SavePageViewLog(DateTime beforeDate)
        {
            try
            {
                DateTime archiveTime = beforeDate.AddSeconds(-1);
                string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Logs\\");
                string fileName = Path.Combine(directory, string.Format("web_{0:yyyy-MM-dd}.log", archiveTime));
                using (StreamWriter fs = File.CreateText(fileName))
                {
                    //WRITE THE HEADER
                    fs.Write("#Software: AbleCommerce 7.0\r\n");
                    fs.Write("#Version: 1.0\r\n");
                    fs.Write(string.Format("#Date: {0:yyyy-MM-dd}", archiveTime) + "\r\n");
                    fs.Write("#Fields: date time c-ip cs-username s-computername s-ip s-port cs-method cs-uri-stem cs-uri-query sc-" +
                        "status time-taken cs(User-Agent) cs(Referer)\r\n");

                    string sql = "SELECT " + PageView.GetColumnNames(string.Empty) + " FROM ac_PageViews WHERE StoreId = @storeId AND ActivityDate < @beforeDate";
                    Database database = Token.Instance.Database;
                    DbCommand selectCommand = database.GetSqlStringCommand(sql);
                    database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                    database.AddInParameter(selectCommand, "@beforeDate", System.Data.DbType.DateTime, beforeDate);
                    //EXECUTE THE COMMAND
                    using (IDataReader dr = database.ExecuteReader(selectCommand))
                    {
                        while (dr.Read())
                        {
                            PageView pageView = new PageView();
                            PageView.LoadDataReader(pageView, dr);
                            StringBuilder sb = new StringBuilder();
                            //WRITE LINE TO FILE
                            //DATE AND TIME
                            sb.Append(string.Format("{0:yyyy-MM-dd HH:mm:ss}", pageView.ActivityDate) + " ");
                            //CLIENT IP
                            sb.Append(pageView.RemoteIP + " ");
                            //USERNAME (-)
                            sb.Append("- ");
                            //SERVERNAME
                            sb.Append("127.0.0.1 ");
                            //SERVERIP
                            sb.Append("127.0.0.1 ");
                            //SERVERPORT
                            sb.Append("80 ");
                            //METHOD
                            sb.Append(pageView.RequestMethod + " ");
                            //STEM
                            sb.Append(pageView.UriStem + " ");
                            //QUERY
                            sb.Append(pageView.UriQuery + " ");
                            //STATUS
                            sb.Append("200 ");
                            //TIME
                            sb.Append(pageView.TimeTaken + " ");
                            //USERAGENT
                            sb.Append(pageView.UserAgent.Replace(" ", "+") + " ");
                            //REFERRER
                            sb.Append(pageView.Referrer + "\r\n");
                            string line = sb.ToString();
                            fs.Write(line.Replace("  ", " - "));
                        }
                        dr.Close();
                    }
                    fs.Flush();
                    fs.Close();
                }
            }
            catch
            {
                //ignore any error during logging
            }
        }

        /// <summary>
        /// Removes old email list signups that were never completed
        /// </summary>
        private static void MaintainEmailSubscriptions()
        {
            Store store = Token.Instance.Store;
            DateTime utcDayStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
            int historyDays = (int)(Math.Abs(store.Settings.SubscriptionRequestExpirationDays) * -1);
            DateTime beforeDate = utcDayStart.AddDays(historyDays);
            string sql = "DELETE FROM ac_EmailListSignups WHERE EmailListId IN (SELECT EmailListId FROM ac_EmailLists WHERE StoreId = @storeId) AND SignupDate < @beforeDate";
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand(sql);
            database.AddInParameter(deleteCommand, "@storeId", System.Data.DbType.Int32, store.StoreId);
            database.AddInParameter(deleteCommand, "@beforeDate", System.Data.DbType.DateTime, beforeDate);
            int expiredCount = database.ExecuteNonQuery(deleteCommand);
            if (expiredCount > 0)
                Logger.Info("Email subscription maintenance cleared " + expiredCount.ToString() + " expired list signup requests.");
        }

        /// <summary>
        /// Maintains the audit log to have at least three, at most four months online history
        /// </summary>
        private static void MaintainAuditLog()
        {
            DateTime utcDayStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
            DateTime fourMonthsAgo = utcDayStart.AddMonths(-4);
            //SEE IF THERE ARE ANY RECORDS OLDER THAN FOUR MONTHS
            int records = CountAuditLog(fourMonthsAgo);
            if (records > 0)
            {
                //ARCHIVE EVERYTHING OLDER THAN THREE MONTHS
                DateTime threeMonthsAgo = utcDayStart.AddMonths(-3);
                //SAVE THE LOG
                SaveAuditLog(threeMonthsAgo);
                //CLEAR THE LOG
                TrimAuditLog(threeMonthsAgo);
            }
        }

        /// <summary>
        /// Counts the number of entries in the audit log before the specified date
        /// </summary>
        /// <param name="beforeDate">Records before this date are counted</param>
        /// <returns>The number of entries in the audit log before the specified date</returns>
        private static int CountAuditLog(DateTime beforeDate)
        {
            string sql = "SELECT COUNT(*) As NumRecords FROM ac_AuditEvents WHERE StoreId = @storeId AND EventDate < @beforeDate";
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sql);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@beforeDate", System.Data.DbType.DateTime, beforeDate);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Saves records to file from the audit log that occured before the specified date
        /// </summary>
        /// <param name="beforeDate">Records older than this date are saved</param>
        private static void SaveAuditLog(DateTime beforeDate)
        {
            try
            {
                DateTime archiveTime = beforeDate.AddSeconds(-1);
                string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Logs\\");
                string fileName = Path.Combine(directory, string.Format("audit_{0:yyyy-MM-dd}.log", archiveTime));
                using (StreamWriter fs = File.CreateText(fileName))
                {
                    //WRITE HEADER TO FILE STREAM
                    fs.WriteLine("AuditEventId,StoreId,EventDate,EventTypeId,Successful,UserId,RelatedId,RemoteIP,Comment");
                    string sql = "SELECT " + AuditEvent.GetColumnNames(string.Empty) + " FROM ac_AuditEvents WHERE StoreId = @storeId AND EventDate < @beforeDate";
                    Database database = Token.Instance.Database;
                    DbCommand selectCommand = database.GetSqlStringCommand(sql);
                    database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                    database.AddInParameter(selectCommand, "@beforeDate", System.Data.DbType.DateTime, beforeDate);
                    //EXECUTE THE COMMAND
                    using (IDataReader dr = database.ExecuteReader(selectCommand))
                    {
                        while (dr.Read())
                        {
                            AuditEvent logEntry = new AuditEvent();
                            AuditEvent.LoadDataReader(logEntry, dr);
                            StringBuilder sb = new StringBuilder();
                            //SAVE RECORD TO FILE
                            sb.Append(logEntry.AuditEventId + ",");
                            sb.Append(logEntry.StoreId + ",");
                            sb.Append(string.Format("{0:yyyy-MM-dd HH:mm:ss},", logEntry.EventDate));
                            sb.Append(logEntry.EventTypeId + ",");
                            sb.Append((logEntry.Successful ? "1" : "0") + ",");
                            sb.Append(logEntry.UserId + ",");
                            sb.Append(logEntry.RelatedId + ",");
                            sb.Append(logEntry.RemoteIP + ",");
                            if (logEntry.Comment.Contains(","))
                            {
                                sb.Append("\"" + logEntry.Comment.Replace("\"", "\"\"") + "\"");
                            }
                            else sb.Append(logEntry.Comment);
                            fs.WriteLine(sb.ToString());
                        }
                        dr.Close();
                    }
                    fs.Flush();
                    fs.Close();
                }
            }
            catch
            {
                //ignore any error during logging
            }
        }

        /// <summary>
        /// Trims records from the audit log that occured before the specified date
        /// </summary>
        /// <param name="beforeDate">Records older than this date are truncated</param>
        private static void TrimAuditLog(DateTime beforeDate)
        {
            string sql = "DELETE FROM ac_AuditEvents WHERE StoreId = @storeId AND EventDate < @beforeDate";
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand(sql);
            database.AddInParameter(deleteCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(deleteCommand, "@beforeDate", System.Data.DbType.DateTime, beforeDate);
            int expiredCount = database.ExecuteNonQuery(deleteCommand);
            if (expiredCount > 0)
                Logger.Info("Audit log maintenance archived " + expiredCount.ToString() + " expired entries.");
        }

        /// <summary>
        /// Delete the users with given list of Ids
        /// </summary>
        /// <param name="idList">The list of user Ids</param>
        private static void DeleteUsers(List<int> idList)
        {
            Database database = Token.Instance.Database;
            int storeId = Token.Instance.Store.StoreId;
            string sql;
            DbCommand deleteCommand;

            foreach (int id in idList)
            {
                //DELETE BASKET FOR THE USER
                sql = "DELETE FROM ac_Baskets WHERE UserId = @userId";
                deleteCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(deleteCommand, "@userId", System.Data.DbType.Int32, id);
                database.ExecuteNonQuery(deleteCommand);

                //DELETE WISHLISTS FOR THE USER
                sql = "DELETE FROM ac_Wishlists WHERE UserId = @userId";
                deleteCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(deleteCommand, "@userId", System.Data.DbType.Int32, id);
                database.ExecuteNonQuery(deleteCommand);

                //DELETE THE USER ITSELF
                sql = "DELETE FROM ac_Users WHERE StoreId = @storeId AND UserId = @userId";
                deleteCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(deleteCommand, "@storeId", System.Data.DbType.Int32, storeId);
                database.AddInParameter(deleteCommand, "@userId", System.Data.DbType.Int32, id);
                database.ExecuteNonQuery(deleteCommand);
            }
        }

        /// <summary>
        /// Gets a list of ids from result of executing the given SQL string
        /// </summary>
        /// <param name="sql">The SQL to execute</param>
        /// <returns></returns>
        private static List<int> GetIdList(string sql)
        {
            Database database = Token.Instance.Database;
            DbCommand dbCmd = database.GetSqlStringCommand(sql);
            return GetIdList(dbCmd);
        }

        /// <summary>
        /// Gets a list of ids from executing the given SQL command
        /// </summary>
        /// <param name="dbCmd">The command to execute</param>
        /// <returns></returns>
        private static List<int> GetIdList(DbCommand dbCmd)
        {
            Database database = Token.Instance.Database;
            List<int> idList = new List<int>();
            //EXECUTE THE COMMAND TO OBTAIN IDS
            using (IDataReader dr = database.ExecuteReader(dbCmd))
            {
                while (dr.Read())
                {
                    idList.Add(dr.GetInt32(0));
                }
                dr.Close();
            }
            return idList;
        }

        /// <summary>
        /// Gets a list of NodeRecord objects by executing the given sql
        /// </summary>
        /// <param name="sql">The SQL to execute</param>
        /// <returns></returns>
        private static List<NodeRecord> GetInvalidCategoryNodes(string sql)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(sql);
            List<NodeRecord> InvalidNodes = new List<NodeRecord>();
            //EXECUTE THE COMMAND TO OBTAIN IDS
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    NodeRecord nr = new NodeRecord();
                    nr.CategoryId = dr.GetInt32(0);
                    nr.CatalogNodeId = dr.GetInt32(1);
                    nr.CatalogNodeTypeId = 0;
                    InvalidNodes.Add(nr);
                }
                dr.Close();
            }
            //BUILD THE INT ARRAY
            return InvalidNodes;
        }

        struct NodeRecord
        {
            public int CategoryId;
            public int CatalogNodeId;
            public int CatalogNodeTypeId;
        }
    }
}
