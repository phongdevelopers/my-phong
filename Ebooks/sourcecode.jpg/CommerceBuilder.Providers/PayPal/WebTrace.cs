using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CommerceBuilder.Payments.Providers.PayPal
{
    internal static class WebTrace
    {
        public static void Write(string message)
        {
            Write(String.Empty, message);
        }

        public static void Write(string category, string message)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Trace.Write(category, message);
            }

        }
    }
}
