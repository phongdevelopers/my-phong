using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CommerceBuilder.Shipping.Providers
{
    /// <summary>
    /// Class that represents a summary of shipment tracking information
    /// </summary>
    public class TrackingSummary
    {
        private string _OriginAddress1;
        private string _OriginCity;
        private string _OriginProvince;
        private string _OriginPostalCode;
        private string _OriginCountryCode;
        private string _DestinationAddress1;
        private string _DestinationCity;
        private string _DestinationProvince;
        private string _DestinationPostalCode;
        private string _DestinationCountryCode;

        private TrackingResultType _TrackingResultType = TrackingResultType.InlineDetails;
        private string _TrackingLink=string.Empty;

        /// <summary>
        /// Type of tracking result
        /// </summary>
        public TrackingResultType TrackingResultType
        {
            get { return _TrackingResultType; }
            set { _TrackingResultType = value; }
        }

        /// <summary>
        /// An external link for tracking details
        /// </summary>
        public string TrackingLink
        {
            get { return _TrackingLink; }
            set { _TrackingLink = value; }
        }

        /// <summary>
        /// The origin address
        /// </summary>
        public string OriginAddress1
        {
            get { return _OriginAddress1; }
            set { _OriginAddress1 = value; }
        }

        /// <summary>
        /// The origin city
        /// </summary>
        public string OriginCity
        {
            get { return _OriginCity; }
            set { _OriginCity = value; }
        }

        /// <summary>
        /// The origin province
        /// </summary>
        public string OriginProvince
        {
            get { return _OriginProvince; }
            set { _OriginProvince = value; }
        }

        /// <summary>
        /// The destination postal code
        /// </summary>
        public string OriginPostalCode
        {
            get { return _OriginPostalCode; }
            set { _OriginPostalCode = value; }
        }

        /// <summary>
        /// The destination country code
        /// </summary>
        public string OriginCountryCode
        {
            get { return _OriginCountryCode; }
            set { _OriginCountryCode = value; }
        }

        /// <summary>
        /// The destnation address
        /// </summary>
        public string DestinationAddress1
        {
            get { return _DestinationAddress1; }
            set { _DestinationAddress1 = value; }
        }

        /// <summary>
        /// The destination city
        /// </summary>
        public string DestinationCity
        {
            get { return _DestinationCity; }
            set { _DestinationCity = value; }
        }

        /// <summary>
        /// The destination province
        /// </summary>
        public string DestinationProvince
        {
            get { return _DestinationProvince; }
            set { _DestinationProvince = value; }
        }

        /// <summary>
        /// The destination postal code
        /// </summary>
        public string DestinationPostalCode
        {
            get { return _DestinationPostalCode; }
            set { _DestinationPostalCode = value; }
        }

        /// <summary>
        /// The destination country code
        /// </summary>
        public string DestinationCountryCode
        {
            get { return _DestinationCountryCode; }
            set { _DestinationCountryCode = value; }
        }
        
        //create a readonly collection
        private Collection<TrackingPackage> _PackageCollection;
        /// <summary>
        /// A collection of TrackingPackage elements
        /// </summary>
        public Collection<TrackingPackage> PackageCollection
        {
            get { return _PackageCollection; }
        }
                
        /// <summary>
        /// Default Constructor
        /// </summary>
        public TrackingSummary()
        {
            _PackageCollection = new Collection<TrackingPackage>();
        }
    }
}
