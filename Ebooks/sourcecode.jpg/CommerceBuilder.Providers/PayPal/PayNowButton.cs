using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//IMPORT COMMERCEBUILDER NAMESPACES
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments.Providers.PayPal
{
    public class PayNowButton : ImageButton
    {
        private Order _Order;
        private int _PaymentId;
        private string _PayPalAccount;
        private bool _UseAuthCapture;
        private bool _UseSandbox;

        public Order Order { get { return _Order; } }
        public int PaymentId { get { return _PaymentId; } }
        public string PayPalAccount { get { return _PayPalAccount; } }
        public bool UseAuthCapture { get { return _UseAuthCapture; } }
        public bool UseSandbox { get { return _UseSandbox; } }

        private PayNowButton() { }

        public PayNowButton(Order order, int paymentId, string paypalAccount, bool useAuthCapture, bool useSandbox)
        {
            _Order = order;
            _PaymentId = paymentId;
            _PayPalAccount = paypalAccount;
            _UseAuthCapture = useAuthCapture;
            _UseSandbox = useSandbox;
            this.ImageUrl = "https://www.paypal.com/images/x-click-but6.gif";
            this.Click += new ImageClickEventHandler(PayNowButton_Click);
        }

        /// <summary>
        /// Use to simulate with code a click of the pay button by the user
        /// </summary>
        public void AutoClick()
        {
            PayNowButton_Click(this, null);
        }

        void PayNowButton_Click(object sender, ImageClickEventArgs e)
        {
            PayNowButton button = (PayNowButton)sender;
            //GET THE ASSOCIATED PAYMENT (IF ANY)
            Payment payment = null;
            int index = button.Order.Payments.IndexOf(button.PaymentId);
            if (index > -1) payment = button.Order.Payments[index];
            //GET THE ORDER BALANCE
            LSDecimal orderBalance = button.Order.GetBalance(false);
            //GET THE PAYMENT AMOUNT IF AVAILABLE, OTHERWISE USE ORDER BALANCE
            LSDecimal paymentAmount = (payment != null) ? payment.Amount : orderBalance;
            //DO NOT DISPLAY THE BUTTON IF THE PAYMENT AMOUNT IS INVALID
            if (paymentAmount <= 0) return;
            LSDecimal totalOfAllOrderItems = button.Order.TotalCharges;
            LSDecimal totalOfSupportedItems = button.Order.Items.TotalPrice(new OrderItemType[]{OrderItemType.Product, OrderItemType.Tax, OrderItemType.Shipping, OrderItemType.Handling, OrderItemType.Discount});
            bool orderHasUnsupportedItems = totalOfAllOrderItems != totalOfSupportedItems;
            //BUILD TEH PAYPAL BUTTON
            string storeUrl = this.GetStoreUrl();
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><meta HTTP-EQUIV=\"Pragma\" CONTENT=\"no-cache\"></head>");
            sb.Append("<body onload=\"document.PayPal_Button.submit();\">");
            string paypalUrl = (button.UseSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr/" : "https://www.paypal.com/cgi-bin/webscr/");
            sb.Append("<form name=\"PayPal_Button\" action=\"" + paypalUrl + "\" method=\"post\">");
            if (paymentAmount == totalOfAllOrderItems && !orderHasUnsupportedItems)
            {
                //SHOW THE BREAKDOWN OF CHARGES
                LSDecimal shippingAmount = button.Order.Items.TotalPrice(OrderItemType.Shipping);
                LSDecimal handlingAmount = button.Order.Items.TotalPrice(OrderItemType.Handling);
                LSDecimal taxAmount = button.Order.Items.TotalPrice(OrderItemType.Tax);
                LSDecimal itemAmount = orderBalance - (shippingAmount + handlingAmount + taxAmount);
                sb.Append("<input type=\"hidden\" name=\"amount\" value=\"" + itemAmount.ToString("F2") + "\">");
                sb.Append("<input type=\"hidden\" name=\"handling\" value=\"" + handlingAmount.ToString("F2") + "\">");
                sb.Append("<input type=\"hidden\" name=\"shipping\" value=\"" + shippingAmount.ToString("F2") + "\">");
                sb.Append("<input type=\"hidden\" name=\"tax\" value=\"" + taxAmount.ToString("F2") + "\">");
            }
            else
            {
                sb.Append("<input type=\"hidden\" name=\"amount\" value=\"" + paymentAmount.ToString("F2") + "\">");
                sb.Append("<input type=\"hidden\" name=\"handling\" value=\"0\">");
                sb.Append("<input type=\"hidden\" name=\"shipping\" value=\"0\">");
                sb.Append("<input type=\"hidden\" name=\"tax\" value=\"0\">");
            }
            sb.Append("<input type=\"hidden\" name=\"shipping2\" value=\"0\">");
            sb.Append("<input type=\"hidden\" name=\"cmd\" value=\"_xclick\">");
            sb.Append("<input type=\"hidden\" name=\"business\" value=\"" + button.PayPalAccount + "\">");
            sb.Append("<input type=\"hidden\" name=\"bn\" value=\"AbleCommerce 7.0\">");
            sb.Append("<input type=\"hidden\" name=\"item_name\" value=\"Order #: " + button.Order.OrderNumber.ToString() + "\">");
            sb.Append("<input type=\"hidden\" name=\"quantity\" value=\"1\">");
            sb.Append("<input type=\"hidden\" name=\"return\" value=\"" + storeUrl + "/ProcessPayPal.ashx?OrderId=" + button.Order.OrderId.ToString() + "\">");
            sb.Append("<input type=\"hidden\" name=\"rm\" value=\"2\">");
            sb.Append("<input type=\"hidden\" name=\"cancel_return\" value=\"" + storeUrl + "/Members/MyOrder.aspx?OrderId=" + button.Order.OrderId.ToString() + "\">");
            sb.Append("<input type=\"hidden\" name=\"no_shipping\" value=\"1\">");
            sb.Append("<input type=\"hidden\" name=\"no_note\" value=\"1\">");
            sb.Append("<input type=\"hidden\" name=\"cs\" value=\"0\">");
            sb.Append("<input type=\"hidden\" name=\"custom\" value=\"" + _Order.OrderId.ToString() + "\">");
            sb.Append("<input type=\"hidden\" name=\"currency_code\" value=\"" + GetPayPalCurrencyCode(Token.Instance.Store.BaseCurrency.ISOCode) + "\">");
            sb.Append("<input type=\"hidden\" name=\"handling_cart\" value=\"0\">");
            sb.Append("<input type=\"hidden\" name=\"bn\" value=\"ablecommerce\">");
            sb.Append("<input type=\"hidden\" name=\"mrb\" value=\"R-4SH04028LF295822X\">");
            sb.Append("<input type=\"hidden\" name=\"pal\" value=\"393Q5LFLKJUCU\">");
            sb.Append("<input type=\"hidden\" name=\"notify_url\" value=\"" + storeUrl + "/ProcessPayPal.ashx?OrderId=" + button.Order.OrderId.ToString() + "\">");
            sb.Append("<input type=\"hidden\" name=\"paymentaction\" value=\"" + (button.UseAuthCapture ? "sale" : "authorization") + "\">");
            sb.Append("<input type=\"hidden\" name=\"upload\" value=\"1\">");
            sb.Append("</form>");
            sb.Append("</body></html>");
            HttpResponse response = this.Context.Response;
            response.Clear();
            response.Write(sb.ToString());
            response.End();
        }

        private static string GetPayPalCurrencyCode(string currencyCode)
        {
            string[] codes = { "USD", "EUR", "GBP", "CAD", "JPY", "AUD", "NZD", "CHF", "HKD", "SGD", "SEK", "DKK", "PLN", "NOK", "HUF", "CZK" };
            if (Array.IndexOf(codes, currencyCode) < 0) return "USD";
            return currencyCode;
        }

        private string GetStoreUrl()
        {
            HttpContext context = HttpContext.Current;
            if (context == null) return string.Empty;
            Uri currentUrl = context.Request.Url;
            string scheme = currentUrl.Scheme + "://";
            string port = (currentUrl.Port == 80 || currentUrl.Port == 443) ? string.Empty : ":" + currentUrl.Port.ToString();
            string host = currentUrl.Host;
            return scheme + host + port + context.Request.ApplicationPath;
        }
    }
}