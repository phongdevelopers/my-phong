using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// Interface that all Feed Providers should implement.
    /// </summary>
    public interface IFeedProvider
    {
        /// <summary>
        /// Create a feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed creation is successful, false otherwise</returns>
        bool CreateFeed(FeedOptions options);

        /// <summary>
        /// Compress a feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed compression is successful, false otherwise</returns>
        bool CompressFeed(FeedOptions options);

        /// <summary>
        /// Upload un-compressed feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed upload is successful, false otherwise</returns>
        bool UploadUncompressedFeed(FeedOptions options);

        /// <summary>
        /// Upload compressed feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed upload is successful, false otherwise</returns>
        bool UploadCompressedFeed(FeedOptions options);


        /// <summary>
        /// Create and compress feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed creation and compression is successfull, false otherwise</returns>
        bool CreateCompressFeed(FeedOptions options);

        /// <summary>
        /// Create and upload uncompressed feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed creation and uploading is successfull, false otherwise</returns>
        bool CreateUploadFeed(FeedOptions options);

        /// <summary>
        /// Create, compress and upload feed
        /// </summary>
        /// <param name="options">Feed configuration options</param>        
        /// <returns>true if feed creation, compression and uploading is successful, false otherwise</returns>
        bool CreateCompressUploadFeed(FeedOptions options);
    }
}
