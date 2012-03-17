using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class holding basket summary data
    /// </summary>
    public class BasketSummary
    {
        private int _BasketId;
        private string _Customer;
        private int _ItemCount;
        private LSDecimal _BasketTotal;
        private DateTime _LastActivity;
        //private string _LastPage;
        //private string _IpAddress;

        /// <summary>
        /// Id of the basket
        /// </summary>
        public int BasketId
        {
            get { return _BasketId; }
            set { _BasketId = value; }
        }

        /// <summary>
        /// The customer name
        /// </summary>
        public string Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }

        /// <summary>
        /// Number of items in the basket
        /// </summary>
        public int ItemCount
        {
            get { return _ItemCount; }
            set { _ItemCount = value; }
        }

        /// <summary>
        /// Total value of the basket
        /// </summary>
        public LSDecimal BasketTotal
        {
            get { return _BasketTotal; }
            set { _BasketTotal = value; }
        }

        /// <summary>
        /// Last activity date 
        /// </summary>
        public DateTime LastActivity
        {
            get { return _LastActivity; }
            set { _LastActivity = value; }
        }

        /*public string LastPage
        {
            get { return _LastPage; }
            set { _LastPage = value; }
        }

        public string IpAddress
        {
            get { return _IpAddress; }
            set { _IpAddress = value; }
        }*/

    }
}
