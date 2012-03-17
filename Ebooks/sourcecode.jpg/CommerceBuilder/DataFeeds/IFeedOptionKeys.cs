using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// Interface for feed option keys
    /// </summary>
    public interface IFeedOptionKeys
    {
        /// <summary>
        /// Name of the feed file
        /// </summary>
        string FeedFileName { get; }
        
        /// <summary>
        /// Whether to overwrite any existing feed file?
        /// </summary>
        string OverwriteFeedFile { get; }

        /// <summary>
        /// Whether to Include all products in feed or exclude the ones marked for feed exclusion?
        /// </summary>
        string IncludeAllProducts { get; }

        /// <summary>
        /// File name for the compressed feed file
        /// </summary>
        string CompressedFeedFileName { get; }

        /// <summary>
        /// Whether to overwrite any existing compressed feed file?
        /// </summary>
        string OverwriteCompressedFile { get; }

        /// <summary>
        /// The FTP host to upload the feed to
        /// </summary>
        string FtpHost { get; }

        /// <summary>
        /// The FTP user name
        /// </summary>
        string FtpUser { get; }

        /// <summary>
        /// The FTP password
        /// </summary>
        string FtpPassword { get; }

        /// <summary>
        /// Remote file name to use on the ftp server
        /// </summary>
        string RemoteFileName { get; }
    }
}
