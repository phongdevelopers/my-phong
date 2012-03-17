using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments.Providers.PaymentechOrbital
{
    internal abstract class PaymentData
    {
        protected CardBrand _CardBrand;

        protected PaymentData(CardBrand cardBrand)
        {
            this._CardBrand = cardBrand;
        }

        public CardBrand CardBrand
        {
            get { return this._CardBrand; }
        }
    }
}
