using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments.Providers.PaymentechOrbital
{
    internal class ECheckData : PaymentData
    {
        private string _RoutingNumber;
        private string _AccountNumber;
        private BankAccountType _BankAccountType = BankAccountType.C;

        public ECheckData() : base(CardBrand.EC) { }

        public string RoutingNumber
        {
            get { return _RoutingNumber; }
            set { _RoutingNumber = value; }
        }

        public string AccountNumber
        {
            get { return _AccountNumber; }
            set { _AccountNumber = value; }
        }

        public BankAccountType BankAccountType
        {
            get { return _BankAccountType; }
            set { _BankAccountType = value;}
        }
    }
}
