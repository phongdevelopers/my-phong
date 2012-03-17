using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments.Providers.PaymentechOrbital
{
    internal class DebitCardData : PaymentData
    {
        private string _CardNumber;
        private int _ExpirationMonth;
        private int _ExpirationYear;
        private string _SecurityCode;
        private string _IssueNumber;
        private int _StartMonth;
        private int _StartYear;

        public DebitCardData() : base(CardBrand.SW) { }

        public string CardNumber
        {
            get { return _CardNumber; }
            set { _CardNumber = value; }
        }

        public int ExpirationMonth
        {
            get { return _ExpirationMonth; }
            set
            {
                if (value < 1 || value > 12)
                    throw new ArgumentException("Expiration month must be between 1 and 12.", "ExpirationMonth");
                _ExpirationMonth = value;
            }
        }

        public int ExpirationYear
        {
            get { return _ExpirationYear; }
            set
            {
                if (value < 2000) value += 2000;
                if (value < 2009 || value > 2100)
                    throw new ArgumentException("Expiration year must be between 2009 and 2100.", "ExpirationYear");
                _ExpirationYear = value;
            }
        }

        /// <summary>
        /// Returns the expiration date in MMYY format
        /// </summary>
        public string ExpirationDate
        {
            get { return this.ExpirationMonth.ToString("00") + this.ExpirationYear.ToString("0000").Substring(2); }
        }

        public string SecurityCode
        {
            get { return _SecurityCode; }
            set { _SecurityCode = value; }
        }

        public string IssueNumber
        {
            get { return _IssueNumber; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length > 2)
                    throw new ArgumentException("Issue number must be a one or two digit number.", "IssueNumber");
                _IssueNumber = value;
            }
        }

        public int StartMonth
        {
            get { return _StartMonth; }
            set
            {
                if (value < 1 || value > 12)
                    throw new ArgumentException("Start month must be between 1 and 12.", "StartMonth");
                _StartMonth = value;
            }
        }

        public int StartYear
        {
            get { return _StartYear; }
            set
            {
                if (value < 50) value += 2000;
                else if (value > 50 && value < 2000) value += 1900;
                if (value < 1980 || value > 2100)
                    throw new ArgumentException("Start year must be between 1980 and 2100.", "StartYear");
                _StartYear = value;
            }
        }

        /// <summary>
        /// Returns the start date in MMYY format, or empty string if start date is not set or invalid
        /// </summary>
        public string StartDate
        {
            get
            {
                if (_StartMonth == 0 || _StartYear == 0) return string.Empty;
                return this.StartMonth.ToString("00") + this.StartYear.ToString("0000").Substring(2);
            }
        }
    }
}