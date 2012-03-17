using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Shipping.Providers
{
    /// <summary>
    /// Class representing the tracking status of a package
    /// </summary>
    public class TrackingPackage
    {
        private string _StatusCode;
        private string _StatusName;
        private string _StatusMessage;
        private string _ServiceCode;
        private string _ServiceName;
        private string _ServiceMessage;
        private string _TrackingNumber;
        private LSDecimal _Weight;
        private WeightUnit _WeightUnit;
        private DateTime _PickupDate;
        private DateTime _DeliveryDate;

        /// <summary>
        /// Status code
        /// </summary>
        public string StatusCode
        {
            get { return _StatusCode; }
            set { _StatusCode = value; }
        }

        /// <summary>
        /// Status name
        /// </summary>
        public string StatusName
        {
            get { return _StatusName; }
            set { _StatusName = value; }
        }

        /// <summary>
        /// Status message
        /// </summary>
        public string StatusMessage
        {
            get { return _StatusMessage; }
            set { _StatusMessage = value; }
        }

        /// <summary>
        /// Service code
        /// </summary>
        public string ServiceCode
        {
            get { return _ServiceCode; }
            set { _ServiceCode = value; }
        }

        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName
        {
            get { return _ServiceName; }
            set { _ServiceName = value; }
        }

        /// <summary>
        /// Service message
        /// </summary>
        public string ServiceMessage
        {
            get { return _ServiceMessage; }
            set { _ServiceMessage = value; }
        }
        /// <summary>
        /// Tracking number
        /// </summary>
        public string TrackingNumber
        {
            get { return _TrackingNumber; }
            set { _TrackingNumber = value; }
        }

        /// <summary>
        /// Weight of the package
        /// </summary>
        public LSDecimal Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }

        /// <summary>
        /// Unit of weight measurement
        /// </summary>
        public WeightUnit WeightUnit
        {
            get { return _WeightUnit; }
            set { _WeightUnit = value; }
        }

        /// <summary>
        /// Date of pickup
        /// </summary>
        public DateTime PickupDate
        {
            get { return _PickupDate; }
            set { _PickupDate = value; }
        }

        /// <summary>
        /// Date of delivery
        /// </summary>
        public DateTime DeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; }
        }

        //create a readonly collection
        private Collection<TrackingActivity> _ActivityCollection;
        /// <summary>
        /// Collection of TrackingActivity objects associated with this TrakingPackage object
        /// </summary>
        public Collection<TrackingActivity> ActivityCollection
        {
            get { return _ActivityCollection; }
        }

        //DEFAULT CONSTROCTOR
        /// <summary>
        /// Default constructor
        /// </summary>
        public TrackingPackage()
        {
            _ActivityCollection = new Collection<TrackingActivity>();
        }
    }
}
