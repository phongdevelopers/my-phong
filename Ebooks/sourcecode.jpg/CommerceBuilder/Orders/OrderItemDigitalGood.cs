using System;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Utility;
using CommerceBuilder.Messaging;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Class that represents an OrderItemDigitalGood object in database
    /// </summary>
    public partial class OrderItemDigitalGood
    {
        private DownloadStatus _DownloadStatus = DownloadStatus.Unknown;

        /// <summary>
        /// DownloadStatus
        /// </summary>
        public DownloadStatus DownloadStatus
        {
            get
            {
                if (_DownloadStatus == DownloadStatus.Unknown)
                {
                    _DownloadStatus = CalculateDownloadStatus();
                }
                return _DownloadStatus;
            }
        }

        /// <summary>
        /// Calculates the download status for this digital good
        /// </summary>
        /// <returns>DownloadStatus enumeration that represents the download status of this digital good</returns>
        private DownloadStatus CalculateDownloadStatus()
        {
            //CHECK IF THE DOWNLOAD IS STILL PENDING VALIDATION
            if (this.ActivationDate.Equals(System.DateTime.MinValue)) return DownloadStatus.Pending;
            //CHECK WHETHER THE DIGITAL GOOD IS EVEN AVAILABLE
            if (this.DigitalGood == null) return DownloadStatus.Expired;
            //CHECK IF THE DOWNLOAD IS EXPIRED
            if (this.Expiration <= LocaleHelper.LocalNow) return DownloadStatus.Expired;
            //CALCULATE MAXDOWNLOADS
            if (!this.DownloadDate.Equals(DateTime.MinValue) && (this.DigitalGood.MaxDownloads > 0))
            {
                //CHECK IF MAXIMUM DOWNLOADS IS MET OR EXCEEDED
                if (this.RelevantDownloads >= this.DigitalGood.MaxDownloads) return DownloadStatus.Depleted;
            }
            return DownloadStatus.Valid;
        }

        private DateTime Expiration
        {
            get
            {
                if (this.ActivationDate.Equals(DateTime.MinValue)) return DateTime.MaxValue;
                DateTime activationExpiration = DigitalGood.GetExpirationDate(this.ActivationDate, this.DigitalGood.ActivationTimeout);
                if (!this.DownloadDate.Equals(DateTime.MinValue)) {
                    DateTime downloadExpiration = DigitalGood.GetExpirationDate(this.DownloadDate, this.DigitalGood.DownloadTimeout);
                    if (downloadExpiration < activationExpiration) return downloadExpiration;
                }
                return activationExpiration;
            }
        }

        /// <summary>
        /// Gets the number of downloads relevant to this digital good
        /// </summary>
        public int RelevantDownloads
        {
            get
            {
                if (this.DownloadDate.Equals(System.DateTime.MinValue)) return 0;
                int count  = 0;
                foreach (Download download in this.Downloads)
                {
                    if (download.DownloadDate >= this.DownloadDate) count++;
                }
                return count;
            }
        }

        /// <summary>
        /// Records a new download for this digital good
        /// </summary>
        /// <param name="remoteIP">Remote IP of the user downloading</param>
        /// <param name="userAgent">HTTP User Agent (e.g; Browser) downloading</param>
        /// <param name="referrer">The HTTP Referrer</param>
        public void RecordDownload(string remoteIP, string userAgent, string referrer)
        {
            Download download = new Download();
            download.DownloadDate = LocaleHelper.LocalNow;
            download.RemoteAddr = StringHelper.Truncate(remoteIP, 39);
            download.UserAgent = StringHelper.Truncate(userAgent,255);
            download.Referrer = StringHelper.Truncate(referrer,255);
            download.OrderItemDigitalGoodId = this.OrderItemDigitalGoodId;
            if (this.DownloadDate.Equals(DateTime.MinValue)) this.DownloadDate = download.DownloadDate;
            this.Downloads.Add(download);
            this.Save();
        }

        /// <summary>
        /// Activates this digital good. 
        /// </summary>
        public void Activate()
        {
            _DownloadStatus = DownloadStatus.Unknown;
            ActivationDate = System.DateTime.UtcNow;
            DownloadDate = System.DateTime.MinValue;
            //initiate activation email
            EmailTemplate template = EmailTemplateDataSource.Load(DigitalGood.ActivationEmailId);
            if (template != null)
            {
                SendEmail(template);
            }
        }

        /// <summary>
        /// Deactivates this digital good
        /// </summary>
        public void Deactivate()
        {
            Deactivate(true);
        }

        /// <summary>
        /// Deactivates this digital good
        /// </summary>
        /// <param name="returnSerialKey">If <b>true</b> a serial key if acquired is returned back to the serial key pool of the provider</param>
        public void Deactivate(bool returnSerialKey)
        {
            _DownloadStatus = DownloadStatus.Unknown;
            ActivationDate = System.DateTime.MinValue;
            if (returnSerialKey)
            {
                ReturnSerialKey();
            }
        }

        /// <summary>
        /// Returns the serial key to the the serial key pool of the provider
        /// </summary>
        public void ReturnSerialKey()
        {
            if (IsSerialKeyAcquired() && HasSerialKeyProvider())
            {
                ISerialKeyProvider provider = DigitalGood.GetSerialKeyProviderInstance();
                provider.ReturnSerialKey(this.SerialKeyData, this);
                this.SerialKeyData = null;
            }
        }

        /// <summary>
        /// Acquires a serial key for this digital good
        /// </summary>
        public void AcquireSerialKey()
        {
            if (IsSerialKeyAcquired())
            {
                return;
            }

            if (!HasSerialKeyProvider())
            {
                return;
            }

            ISerialKeyProvider provider = DigitalGood.GetSerialKeyProviderInstance();
            AcquireSerialKeyResponse response = provider.AcquireSerialKey(this);

            if (response.Successful)
            {
                this.SerialKeyData = response.SerialKey;
                //initiate fulfillment email
                EmailTemplate template = EmailTemplateDataSource.Load(DigitalGood.FulfillmentEmailId);
                if (template != null)
                {
                    SendEmail(template);
                }
            }
            else
            {
                //serial key could not be acquired.
                Logger.Error(string.Format("Serial Key could not be acquired for {0} using Serial Key Provider {1}. Provider Error Message '{2}'", DigitalGood.Name, provider.Name, response.ErrorMessage));
            }
        }

        /// <summary>
        /// Sets the serial key for this digital good
        /// </summary>
        /// <param name="serialKeyData">Serial key to set</param>
        public void SetSerialKey(string serialKeyData)
        {
            SetSerialKey(serialKeyData, true);
        }

        /// <summary>
        /// Sets the serial key for this digital good
        /// </summary>
        /// <param name="serialKeyData">Serial key to set</param>
        /// <param name="sendFulfillmentEmail">If <b>true</b> fulfillment email is sent</param>
        public void SetSerialKey(string serialKeyData, bool sendFulfillmentEmail)
        {
            if (IsSerialKeyAcquired())
            {
                if(this.SerialKeyData == serialKeyData) return;
            }

            this.SerialKeyData = serialKeyData;

            if (sendFulfillmentEmail && !string.IsNullOrEmpty(serialKeyData))
            {
                //initiate fulfillment email
                EmailTemplate template = EmailTemplateDataSource.Load(DigitalGood.FulfillmentEmailId);
                if (template != null)
                {
                    SendEmail(template);
                }
            }
        }

        /// <summary>
        /// Whether serial key is acquired for this digital good or not
        /// </summary>
        /// <returns><b>true</b> if serial key is acquired, <b>false</b> otherwise</returns>
        public bool IsSerialKeyAcquired()
        {
            return !string.IsNullOrEmpty(this.SerialKeyData);
        }

        /// <summary>
        /// Whether this digital good has been activated for download or not
        /// </summary>
        /// <returns></returns>
        public bool IsActivated()
        {
            return this.ActivationDate != System.DateTime.MinValue;
        }

        /// <summary>
        /// Is any serial key provider associated with this digital good
        /// </summary>
        /// <returns><b>true</b> if a serial key provider is associated with this digital good, <b>false</b> otherwise</returns>
        public bool HasSerialKeyProvider()
        {
            return !string.IsNullOrEmpty(DigitalGood.SerialKeyProviderId);
        }

        private void SendEmail(EmailTemplate template)
        {
            string toAddress = this.OrderItem.Order.BillToEmail;
            if (string.IsNullOrEmpty(toAddress))
            {
                toAddress = this.OrderItem.Order.User.Email;
            }
            template.ToAddress = toAddress;
            template.Parameters["store"] = this.OrderItem.Order.Store;
            template.Parameters["digitalGood"] = this.DigitalGood;
            template.Parameters["customer"] = this.OrderItem.Order.User;
            template.Parameters["orderItem"] = this.OrderItem;
            template.Parameters["orderItemDigitalGood"] = this;
            template.Parameters["order"] = this.OrderItem.Order;
            template.Send();
        }
    }
}
