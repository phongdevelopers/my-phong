
namespace CommerceBuilder.Payments.Providers.PayPal
{
    public class ExpressCheckoutResult
    {
        private com.paypal.soap.api.ErrorType[] _Errors;

        public com.paypal.soap.api.ErrorType[] Errors
        {
            get
            {
                return _Errors;
            }
        }

        public ExpressCheckoutResult(com.paypal.soap.api.ErrorType[] errors)
        {
            this._Errors = errors;
        }
    }
}
