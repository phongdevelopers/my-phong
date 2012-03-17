using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.DigitalDelivery
{
    /// <summary>
    /// Response to an acquire serial key request
    /// </summary>
    public class AcquireSerialKeyResponse
    {
        private string _SerialKey;
        private bool _Successful = false;
        private string _ErrorMessage = "";

        /// <summary>
        /// The serial key acquired
        /// </summary>
        public string SerialKey
        {
            get { return _SerialKey; }
            set { _SerialKey = value; }
        }

        /// <summary>
        /// Indicates whether the acquisition of serial key was successful or not
        /// </summary>
        public bool Successful
        {
            get { return _Successful; }
            set { _Successful = value; }
        }

        /// <summary>
        /// Error during serial key acquisition
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }
    }
}
