using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

namespace CommerceBuilder.DataFeeds
{
    /// <summary>
    /// Class that holds various feed options
    /// </summary>
    public class FeedOptions
    {
        private string _FeedFileName = "";
        private bool _OverwriteFeedFile = true;
        private bool _IncludeAllProducts = false;
        private string _CompressedFeedFileName = "";
        private bool _OverwriteCompressedFile = true;
        private string _FtpHost = "";
        private string _FtpUser = "";
        private string _FtpPassword = "";
        private string _RemoteFileName = "";
        private string _FeedDataPath = "";

        private int _StoreId;

        

        private bool IsDirty = false;
        
        /// <summary>
        /// Name of the feed file
        /// </summary>
        public string FeedFileName
        {
            get { return _FeedFileName; }
            set
            {
                if (!_FeedFileName.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    _FeedFileName = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Id for the store for which we need to create feed
        /// </summary>
        public int StoreId
        {
            get { return _StoreId; }
            set { _StoreId = value; }
        }

        /// <summary>
        /// Whether to overwrite any existing feed file?
        /// </summary>
        public bool OverwriteFeedFile
        {
            get { return _OverwriteFeedFile; }
            set
            {
                if (!_OverwriteFeedFile.Equals(value))
                {
                    _OverwriteFeedFile = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Whether to Include all products in feed or exclude the ones marked for feed exclusion?
        /// </summary>
        public bool IncludeAllProducts
        {
            get { return _IncludeAllProducts; }
            set
            {
                if (!_IncludeAllProducts.Equals(value))
                {
                    _IncludeAllProducts = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// File name for the compressed feed file
        /// </summary>
        public string CompressedFeedFileName
        {
            get { return _CompressedFeedFileName; }
            set
            {
                if (!_CompressedFeedFileName.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    _CompressedFeedFileName = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Whether to overwrite any existing compressed feed file?
        /// </summary>
        public bool OverwriteCompressedFile
        {
            get { return _OverwriteCompressedFile; }
            set
            {
                if (!_OverwriteCompressedFile.Equals(value))
                {
                    _OverwriteCompressedFile = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// The FTP host to upload the feed to
        /// </summary>
        public string FtpHost
        {
            get { return _FtpHost; }
            set
            {
                if (!_FtpHost.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    _FtpHost = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// The FTP user name
        /// </summary>
        public string FtpUser
        {
            get { return _FtpUser; }
            set
            {
                if (!_FtpUser.Equals(value))
                {
                    _FtpUser = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// The FTP password
        /// </summary>
        public string FtpPassword
        {
            get { return _FtpPassword; }
            set
            {
                if (!_FtpPassword.Equals(value))
                {
                    _FtpPassword = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Remote file name to use on the ftp server
        /// </summary>
        public string RemoteFileName
        {
            get { return _RemoteFileName; }
            set
            {
                if (!_RemoteFileName.Equals(value))
                {
                    _RemoteFileName = value;
                    IsDirty = true;
                }
            }
        }


        /// <summary>
        /// Gets or sets thje path of the feed data folder
        /// </summary>
        /// <remarks>This value is not saved to persistent storage.</remarks>
        public string FeedDataPath
        {
            get { return _FeedDataPath; }
            set { _FeedDataPath = value; }
        }

        /// <summary>
        /// Load feed options using the given option keys
        /// </summary>
        /// <param name="settingKeys">Feed Option Setting Keys</param>
        public void Load(IFeedOptionKeys settingKeys)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            _CompressedFeedFileName = settings.GetValueByKey(settingKeys.CompressedFeedFileName);
            _FeedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Feeds");
            _FeedFileName = settings.GetValueByKey(settingKeys.FeedFileName);
            _FtpHost = settings.GetValueByKey(settingKeys.FtpHost);
            _FtpPassword = settings.GetValueByKey(settingKeys.FtpPassword);
            _FtpUser = settings.GetValueByKey(settingKeys.FtpUser);
            _IncludeAllProducts = AlwaysConvert.ToBool(settings.GetValueByKey(settingKeys.IncludeAllProducts), false);
            _OverwriteCompressedFile = AlwaysConvert.ToBool(settings.GetValueByKey(settingKeys.OverwriteCompressedFile), true);
            _OverwriteFeedFile = AlwaysConvert.ToBool(settings.GetValueByKey(settingKeys.OverwriteFeedFile), true);
            _RemoteFileName = settings.GetValueByKey(settingKeys.RemoteFileName);
            IsDirty = false;
        }

        /// <summary>
        /// Save the feed options using the given feed option keys
        /// </summary>
        /// <param name="settingKeys">Feed Option Setting Keys</param>
        public void Save(IFeedOptionKeys settingKeys)
        {
            if (IsDirty)
            {
                StoreSettingCollection settings = Token.Instance.Store.Settings;
                settings.SetValueByKey(settingKeys.CompressedFeedFileName, CompressedFeedFileName);
                settings.SetValueByKey(settingKeys.FeedFileName, FeedFileName);
                settings.SetValueByKey(settingKeys.FtpHost, FtpHost);
                settings.SetValueByKey(settingKeys.FtpPassword, FtpPassword);
                settings.SetValueByKey(settingKeys.FtpUser, FtpUser);
                settings.SetValueByKey(settingKeys.IncludeAllProducts, IncludeAllProducts.ToString());
                settings.SetValueByKey(settingKeys.OverwriteCompressedFile, OverwriteCompressedFile.ToString());
                settings.SetValueByKey(settingKeys.OverwriteFeedFile, OverwriteFeedFile.ToString());
                settings.SetValueByKey(settingKeys.RemoteFileName, RemoteFileName);
                settings.Save();
            }
        }
    }
}
