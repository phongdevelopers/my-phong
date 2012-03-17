using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// Feed option keys for GoogleBase 
    /// </summary>
    public class GoogleBaseOptionKeys : IFeedOptionKeys
    {        
        /// <summary>
        /// Name of the feed file
        /// </summary>
        public string FeedFileName { get{ return "GoogleBase_FeedFileName";}}

        /// <summary>
        /// Whether to overwrite any existing feed file?
        /// </summary>
        public string OverwriteFeedFile { get { return "GoogleBase_OverwriteFeedFile"; } }

        /// <summary>
        /// Whether to Include all products in feed or exclude the ones marked for feed exclusion?
        /// </summary>
        public string IncludeAllProducts { get { return "GoogleBase_IncludeAllProducts"; } }

        /// <summary>
        /// File name for the compressed feed file
        /// </summary>
        public string CompressedFeedFileName { get { return "GoogleBase_CompressedFeedFileName"; } }

        /// <summary>
        /// Whether to overwrite any existing compressed feed file?
        /// </summary>
        public string OverwriteCompressedFile { get { return "GoogleBase_OverwriteCompressedFile"; } }

        /// <summary>
        /// The FTP host to upload the feed to
        /// </summary>
        public string FtpHost { get { return "GoogleBase_FtpHost"; } }

        /// <summary>
        /// The FTP user name
        /// </summary>
        public string FtpUser { get { return "GoogleBase_FtpUser"; } }

        /// <summary>
        /// The FTP password
        /// </summary>
        public string FtpPassword { get { return "GoogleBase_FtpPassword"; } }

        /// <summary>
        /// Remote file name to use on the ftp server
        /// </summary>
        public string RemoteFileName { get { return "GoogleBase_RemoteFileName"; } }
    }
}
