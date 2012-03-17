using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

namespace CommerceBuilder.Messaging
{
    /// <summary>
    /// Class that holds SMTP settings information
    /// </summary>
    public class SmtpSettings
    {
        private string _Server;
        private int _Port = 25;
        private bool _RequiresAuthentication = false;
        private string _UserName;
        private string _Password;
        private bool _EnableSSL;

        /// <summary>
        /// Smtp server should use SSL
        /// </summary>
        public bool EnableSSL
        {
            get { return _EnableSSL; }
            set { _EnableSSL = value; }
        }

        /// <summary>
        /// The SMTP Server Name or IP
        /// </summary>
        public string Server
        {
            get { return _Server; }
            set { _Server = value; }
        }

        /// <summary>
        /// The SMTP server port. Default is 25.
        /// </summary>
        public int Port
        {
            get { return _Port; }
            set { _Port = value;}
        }

        /// <summary>
        /// Indicates whether this server requires authentication
        /// </summary>
        public bool RequiresAuthentication
        {
            get { return _RequiresAuthentication; }
            set { _RequiresAuthentication = value; }
        }

        /// <summary>
        /// SMTP user name for authentication
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// <summary>
        /// SMTP password for authentication
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }


        /// <summary>
        /// The instance that contains SMTP settings for the current store
        /// </summary>
        public static SmtpSettings DefaultSettings
        {
            get
            {
                SmtpSettings settings_ = new SmtpSettings();
                StoreSettingCollection settings = Token.Instance.Store.Settings;

                settings_.UserName = settings.SmtpUserName;
                settings_.Password = settings.SmtpPassword;
                settings_.RequiresAuthentication = settings.SmtpRequiresAuthentication;
                settings_.Server = settings.SmtpServer;
                settings_.Port = AlwaysConvert.ToInt(settings.SmtpPort, 25);
                settings_.EnableSSL = settings.SmtpEnableSSL;

                return settings_;
            }
        }
        
    }
}
