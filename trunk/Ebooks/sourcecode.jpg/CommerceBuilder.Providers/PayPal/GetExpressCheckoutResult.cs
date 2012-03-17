using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments.Providers.PayPal
{
    public class GetExpressCheckoutResult : ExpressCheckoutResult
    {
        private CommerceBuilder.Users.User _PayPalUser;
        public CommerceBuilder.Users.User PayPalUser
        {
            get { return _PayPalUser; }
        }

        public GetExpressCheckoutResult(CommerceBuilder.Users.User paypalUser, com.paypal.soap.api.ErrorType[] errors) : base(errors)
        {
            _PayPalUser = paypalUser;
        }
    }
}
