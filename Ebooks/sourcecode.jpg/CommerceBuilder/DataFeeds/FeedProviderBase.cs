using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using CommerceBuilder.Catalog;
using CommerceBuilder.Products;
using System.Web.Util;
using System.Web;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Security;
using System.Security.Permissions;
using CommerceBuilder.Utility;
using System.Threading;
using CommerceBuilder.Stores;
using CommerceBuilder.Common;
using System.Net;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// Base class that can be used by Feed Provider implementations
    /// </summary>
    public abstract class FeedProviderBase : IFeedProvider
    {

        // are used for asynchronious feed operations
        /// <summary>
        /// Used for asynchronious operations
        /// </summary>
        /// <param name="options">FeedOptions</param>
        /// <returns>true if operation is successfull otherwise false</returns>
        protected delegate bool FeedOperationDelegate(FeedOptions options);

        /// <summary>
        /// Delegate used for asynchronious operations
        /// </summary>
        protected FeedOperationDelegate feedOpDelegate;
               

        /// <summary>
        /// Holds the status of import progress
        /// </summary> 
        FeedOperationStatus feedOperationStatus;

        /// <summary>
        /// Holds the status of import progress
        /// </summary>
        public FeedOperationStatus FeedOperationStatus
        {
            get { return feedOperationStatus; }
            set { feedOperationStatus = value; }
        }


        /// <summary>
        /// Begins an asynchronous call to our long running create method.
        /// </summary>
        /// <param name="options">FeedOptions</param>
        /// <param name="cb">System.AsyncCallback</param>
        /// <param name="state"></param>
        /// <returns>System.IAsyncResult</returns> 
        public System.IAsyncResult BeginCreateFeed(FeedOptions options, System.AsyncCallback cb, object state)
        {            
            this.feedOpDelegate = new FeedOperationDelegate(this.CreateFeed);
            return this.feedOpDelegate.BeginInvoke(options, cb, state);
        }
       
        /// <summary>
        /// Begins an asynchronous call to our long running create and compress feed method.
        /// </summary>
        /// <param name="options">FeedOptions</param>
        /// <param name="cb">System.AsyncCallback</param>
        /// <param name="state"></param>
        /// <returns>System.IAsyncResult</returns> 
        public System.IAsyncResult BeginCreateCompressFeed(FeedOptions options, System.AsyncCallback cb, object state)
        {            
            this.feedOpDelegate = new FeedOperationDelegate(this.CreateCompressFeed);
            return this.feedOpDelegate.BeginInvoke(options, cb, state);
        }


        /// <summary>
        /// Begins an asynchronous call to our long running create and upload feed method.
        /// </summary>
        /// <param name="options">FeedOptions</param>
        /// <param name="cb">System.AsyncCallback</param>
        /// <param name="state"></param>
        /// <returns>System.IAsyncResult</returns> 
        public System.IAsyncResult BeginCreateUploadFeed(FeedOptions options, System.AsyncCallback cb, object state)
        {            
            this.feedOpDelegate = new FeedOperationDelegate(this.CreateUploadFeed);
            return this.feedOpDelegate.BeginInvoke(options, cb, state);
        }

        /// <summary>
        /// Begins an asynchronous call to our long running create, compress and upload feed method.
        /// </summary>
        /// <param name="options">FeedOptions</param>
        /// <param name="cb">System.AsyncCallback</param>
        /// <param name="state"></param>
        /// <returns>System.IAsyncResult</returns> 
        public System.IAsyncResult BeginCreateCompressUploadFeed(FeedOptions options, System.AsyncCallback cb, object state)
        {            
            this.feedOpDelegate = new FeedOperationDelegate(this.CreateCompressUploadFeed);
            return this.feedOpDelegate.BeginInvoke(options, cb, state);
        }

        /// <summary>
        /// Waits for the pending asynchronous request to complete.
        /// </summary>
        /// <param name="result"></param> 
        public void EndFeedOperation(System.IAsyncResult result)
        {
            this.feedOpDelegate.EndInvoke(result);
        }

        private void UpdateStatus(int percent, string message, bool success)
        {
            UpdateStatus(percent, message);
            this.feedOperationStatus.Success = success;           
        }

        private void UpdateStatus(int percent, string message)
        {
            this.feedOperationStatus.Percent = percent;
            this.feedOperationStatus.StatusMessage = message;
            this.feedOperationStatus.Messages.Add(message);
        }


        /// <summary>
        /// Create a feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed creation is successful, false otherwise</returns>
        public bool CreateFeed(FeedOptions options)
        {            
            string feedFile = Path.Combine(options.FeedDataPath, options.FeedFileName);
            if (File.Exists(feedFile) && !options.OverwriteFeedFile)
            {
                UpdateStatus(100, "Feed File Already Exists. You should either chose to overwrite the feed file or provide a different name.",false);
                return false;
            }

            string criteria = "VisibilityId<>" + (short)CatalogVisibility.Private;
            if (!options.IncludeAllProducts)
            {
                criteria = criteria + " AND ExcludeFromFeed=0";
            }

            string headerRow = GetHeaderRow();
            if (!headerRow.EndsWith("\r\n")) headerRow += "\r\n";

            try
            {
                using (StreamWriter feedWriter = File.CreateText(feedFile))
                {
                    feedWriter.Write(headerRow);
                    feedWriter.Close();
                }
            }
            catch (System.UnauthorizedAccessException accessException)
            {
                UpdateStatus(100, "Access restricted on feed data folder. In order to create feeds, the current user (" + Misc.GetProcessIdentity() + ") needs write access to feeds data folder.", false);
                UpdateStatus(100, "Access exception : " + accessException.Message, false);
                return false;
            }
            
            bool needTokenReset = false;
            if (Token.Instance.Store == null)
            {
                Store store = StoreDataSource.Load(options.StoreId);
                Token.Instance.InitStoreContext(store);
                needTokenReset = true;
            }

            try
            {
                using (StreamWriter feedWriter = File.AppendText(feedFile))
                {
                    ProductCollection products;
                    int count = ProductDataSource.CountForCriteria(criteria);
                    int startIndex = 0;
                    while (startIndex < count)
                    {
                        // DETERMINE HOW MANY ROWS LEFT TO INCLUDE IN FEED
                        int rowsRemaining = count - startIndex;

                        // ONLY PROCESS 1000 ROWS AT A TIME
                        int maxRows = (rowsRemaining > 1000) ? 1000 : rowsRemaining;

                        // CALCULATE PROGRESS PERCENTAGE AND DISPLAY PROGRESS
                        int percentDone = startIndex / count * 100;
                        UpdateStatus(percentDone, "Generating feed for products from " + startIndex + " to " + (startIndex + maxRows) + " out of " + count + " products.");

                        // GET THE ROWS TO BE OUTPUT
                        products = ProductDataSource.LoadForCriteria(criteria, maxRows, startIndex);

                        // GENERATE THE FEED DATA
                        string feedData = GetFeedData(products);

                        // WRITE DATA TO THE FEED FILE
                        if (!feedData.EndsWith("\r\n")) feedData += "\r\n";
                        feedWriter.Write(feedData);
                        feedWriter.Flush();

                        // LOOP TO THE NEXT BLOCK OF DATA
                        startIndex += 1000;
                    }

                    // CLOSE THE FEED FILE
                    feedWriter.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Warn("Error Feed Creator Thread : " + e.Message, e);
                UpdateStatus(100, "Error while creating feed file." + e.Message, false);
                return false;
            }
            finally
            {
                if (needTokenReset) Token.ResetInstance();
            }
            UpdateStatus(100, string.Format("Feed file generated at {0}.", options.FeedDataPath), true);
            return true;
        }

        /// <summary>
        /// Compress a feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed compression is successful, false otherwise</returns>
        public bool CompressFeed(FeedOptions options)
        {            
            string compressedFile = Path.Combine(options.FeedDataPath, options.CompressedFeedFileName);

            if (File.Exists(compressedFile) && !options.OverwriteCompressedFile)
            {
                UpdateStatus(100, "Compressed Feed File Already Exists. You should either chose to overwrite the compressed feed file or provide a different name.", false);
                return false;
            }

            string feedFile = Path.Combine(options.FeedDataPath, options.FeedFileName);
            if (!File.Exists(feedFile))
            {
                UpdateStatus(100, "Feed File Does not exist. Create a feed file first.", false);
                return false;
            }

            if (File.Exists(compressedFile))
            {
                File.Delete(compressedFile);
            }

            using (ZipOutputStream zipOutStream = new ZipOutputStream(new FileStream(compressedFile, FileMode.Create, FileAccess.Write)))
            {
                using (FileStream fileStreamIn = new FileStream(feedFile, FileMode.Open, FileAccess.Read))
                {
                    zipOutStream.UseZip64 = UseZip64.Off;

                    byte[] buffer = new byte[4096];
                    ZipEntry zipEntry = new ZipEntry(Path.GetFileName(options.FeedFileName));
                    zipOutStream.PutNextEntry(zipEntry);
                    int size;
                    do
                    {
                        size = fileStreamIn.Read(buffer, 0, buffer.Length);
                        zipOutStream.Write(buffer, 0, size);
                    } while (size > 0);

                    zipOutStream.Flush();
                    zipOutStream.Finish();
                    fileStreamIn.Close();
                }
                zipOutStream.Close();
            }
            UpdateStatus(100, string.Format("Feed File Compressed at {0}.", options.FeedDataPath), true);
            return true;
        }

        /// <summary>
        /// Upload un-compressed feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed upload is successful, false otherwise</returns>
        public bool UploadUncompressedFeed(FeedOptions options)
        {
            // NEED TO INITIALIZE STORE CONTEXT ( AS THIS CAN BE CALLED IN ASYN WAY AS WELL), IN CASE AN ERROR OCCURS THEN WE NEED TO LOG FOR STORE
            bool needTokenReset = false;
            if (Token.Instance.Store == null)
            {
                Store store = StoreDataSource.Load(options.StoreId);
                Token.Instance.InitStoreContext(store);
                needTokenReset = true;
            }

            string feedFile = Path.Combine(options.FeedDataPath, options.FeedFileName);

            if (!File.Exists(feedFile))
            {
                UpdateStatus(100, "Can not upload. Feed file does not exist. Please Create the Feed file first.", false);
                return false;
            }
            try
            {
                string remoteFileName = options.RemoteFileName;
                if(string.IsNullOrEmpty(remoteFileName)){
                    remoteFileName = options.FeedFileName;
                }
                UploadFile(options, feedFile, remoteFileName);
            }
            catch (Exception e)
            {
                UpdateStatus(100, "An error occured while uploading: " + e.Message, false);
                Utility.Logger.Warn("FTP upload of " + options.FeedFileName + " could not be completed.", e);
                if (needTokenReset) Token.ResetInstance();
                return false;
            }

            if (needTokenReset) Token.ResetInstance();

            UpdateStatus(100, "Uncompressed Feed File Uploaded.", true);
            return true;
        }

        /// <summary>
        /// Upload compressed feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed upload is successful, false otherwise</returns>
        public bool UploadCompressedFeed(FeedOptions options)
        {
            bool needTokenReset = false;
            if (Token.Instance.Store == null)
            {
                Store store = StoreDataSource.Load(options.StoreId);
                Token.Instance.InitStoreContext(store);
                needTokenReset = true;
            }

            string compressedFile = Path.Combine(options.FeedDataPath, options.CompressedFeedFileName);

            if (!File.Exists(compressedFile))
            {
                UpdateStatus(100,"Can not upload. Compressed feed file does not exist. Please create the compressed feed file first.", false);
                return false;
            }
            try
            {
                string remoteFileName = options.RemoteFileName;
                if (string.IsNullOrEmpty(remoteFileName))
                {
                    remoteFileName = options.CompressedFeedFileName;
                }

                UploadFile(options, compressedFile, options.CompressedFeedFileName);
            }
            catch (Exception e)
            {
                UpdateStatus(100, "An error occured while uploading: " + e.Message, false);
                Utility.Logger.Warn("FTP upload of " + options.CompressedFeedFileName + " could not be completed.", e);
                if (needTokenReset) Token.ResetInstance();

                return false;
            }
            if (needTokenReset) Token.ResetInstance();

            UpdateStatus(100, "Compressed Feed File Uploaded.", true);
            return true;
        }

        private static void UploadFile(FeedOptions options, string localFilePath, string remoteFileName)
        {
            AspNetHostingPermissionLevel currentLevel = GetCurrentTrustLevel();
            if (currentLevel == AspNetHostingPermissionLevel.Unrestricted ||
                currentLevel == AspNetHostingPermissionLevel.High ||
                (currentLevel == AspNetHostingPermissionLevel.Medium && CheckWebPermission()))
            {
                FtpClient ftpClient = new FtpClient();
                ftpClient.RemoteHost = options.FtpHost;
                ftpClient.UserName = options.FtpUser;
                ftpClient.Password = options.FtpPassword;
                ftpClient.TransferType = FtpTransferType.BINARY;
                ftpClient.PutFile(localFilePath, remoteFileName);
            }
            else throw new CommerceBuilder.Exceptions.SecurityException("The application requires at least Medium trust and WebPermission credentials to perform FTP operations.");
        }

        private static bool CheckWebPermission()
        {
            try
            {
                WebPermission unrestricted = new WebPermission(PermissionState.Unrestricted);
                unrestricted.Demand();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            foreach (AspNetHostingPermissionLevel trustLevel in
                    new AspNetHostingPermissionLevel[] {
                AspNetHostingPermissionLevel.Unrestricted,
                AspNetHostingPermissionLevel.High,
                AspNetHostingPermissionLevel.Medium,
                AspNetHostingPermissionLevel.Low,
                AspNetHostingPermissionLevel.Minimal 
            })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch (System.Security.SecurityException)
                {
                    continue;
                }

                return trustLevel;
            }
            return AspNetHostingPermissionLevel.None;
        }

        /// <summary>
        /// Checks if a Url is absolute or not
        /// </summary>
        /// <param name="url">The Url to check</param>
        /// <returns>true if Url is absolute, false otherwise</returns>
        protected static bool IsAbsoluteURL(string url)
        {
            if (url == null)
            {
                return false;
            }

            int colonPos = url.IndexOf(":");
            if (colonPos != -1)
            {
                string sub = url.Substring(colonPos + 1);
                if (sub.StartsWith("//"))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// To be implemented by sub-classes to generate the header row for the feed
        /// </summary>
        /// <returns>The header row.</returns>
        protected abstract string GetHeaderRow();

        /// <summary>
        /// To be implemented by sub-classes to generate the feed data for given products
        /// </summary>
        /// <param name="products">The products to generate feed for</param>
        /// <returns>Feed data generated</returns>
        protected abstract string GetFeedData(ProductCollection products);

        /// <summary>
        /// Create a feed and then compress the feed file
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed creation and compression is successful, false otherwise</returns>
        public bool CreateCompressFeed(FeedOptions options)
        {
            if (CreateFeed(options)) return CompressFeed(options);
            else return false;
        }


        /// <summary>
        /// Creates a feed file and then upload the un-compressed feed file
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed creation and upload is successful, false otherwise</returns>
        public bool CreateUploadFeed(FeedOptions options)
        {
            if (CreateFeed(options)) return UploadUncompressedFeed(options);
            else return false;
        }

        /// <summary>
        /// Create a feed file, compresses and then uploads the compressed file
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed creation, compression and cupload is successful, false otherwise</returns>
        public bool CreateCompressUploadFeed(FeedOptions options)
        {
            if (CreateCompressFeed(options)) return UploadCompressedFeed(options);
            else return false;
        }

        private static Regex reUPC = new Regex("^\\d{12}$");

        /// <summary>
        /// Determines whether value is a 12 digit UPC code
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <returns>True if the value is a UPC code; false otherwise.</returns>
        protected bool IsUpcCode(string value)
        {
            return reUPC.IsMatch(value);
        }
    }
}
