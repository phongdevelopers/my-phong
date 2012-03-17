using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Shipping.Providers
{
    /// <summary>
    /// Class holding information about a particular tracking activity 
    /// </summary>
    public class TrackingActivity
    {
        private string _City;
        private string _Province;
        private string _CountryCode;
        private DateTime _ActivityDate;
        private string _Status;
        private string _SignedBy;
        private string _Comment;

        /// <summary>
        /// City
        /// </summary>
        public string City
        {
            get { return _City; }
            set { _City = value; }
        }

        /// <summary>
        /// Province
        /// </summary>
        public string Province
        {
            get { return _Province; }
            set { _Province = value; }
        }

        /// <summary>
        /// CountryCode
        /// </summary>
        public string CountryCode
        {
            get { return _CountryCode; }
            set { _CountryCode = value; }
        }

        /// <summary>
        /// ActivityDate
        /// </summary>
        public DateTime ActivityDate
        {
            get { return _ActivityDate; }
            set { _ActivityDate = value; }
        }

        /// <summary>
        /// Status
        /// </summary>
        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>
        /// SignedBy
        /// </summary>
        public string SignedBy
        {
            get { return _SignedBy; }
            set { _SignedBy = value; }
        }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment
        {
            get { return _Comment; }
            set { _Comment = value; }
        }
    }
}
