using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// Feed Option Keys for Yahoo Feed
    /// </summary>
    public class YahooFeedOptionKeys : IFeedOptionKeys
    {
        /// <summary>
        /// Name of the feed file
        /// </summary>
        public string FeedFileName { get{ return "YahooShopping_FeedFileName";}}
        
        /// <summary>
        /// Whether to overwrite any existing feed file?
        /// </summary>
        public string OverwriteFeedFile { get{ return "YahooShopping_OverwriteFeedFile";}}

        /// <summary>
        /// Whether to Include all products in feed or exclude the ones marked for feed exclusion?
        /// </summary>
        public string IncludeAllProducts { get { return "YahooShopping_IncludeAllProducts"; } }

        /// <summary>
        /// File name for the compressed feed file
        /// </summary>
        public string CompressedFeedFileName { get { return "YahooShopping_CompressedFeedFileName"; } }

        /// <summary>
        /// Whether to overwrite any existing compressed feed file?
        /// </summary>
        public string OverwriteCompressedFile { get { return "YahooShopping_OverwriteCompressedFile"; } }

        /// <summary>
        /// The FTP host to upload the feed to
        /// </summary>
        public string FtpHost { get { return "YahooShopping_FtpHost"; } }

        /// <summary>
        /// The FTP user name
        /// </summary>
        public string FtpUser { get { return "YahooShopping_FtpUser"; } }

        /// <summary>
        /// The FTP password
        /// </summary>
        public string FtpPassword { get { return "YahooShopping_FtpPassword"; } }

        /// <summary>
        /// Remote file name to use on the ftp server
        /// </summary>
        public string RemoteFileName { get { return "YahooShopping_RemoteFileName"; } }
    }
}
