using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments.Providers.PaymentechOrbital
{
    internal class CreditCardData : PaymentData
    {
        private string _CardNumber;
        private int _ExpirationMonth;
        private int _ExpirationYear;
        private string _SecurityCode;

        public CreditCardData() : base(CardBrand.CC) { }

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
            set {
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
    }
}
