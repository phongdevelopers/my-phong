using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;
using CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class OrderStateChangeHandler
    {
        private OrderStateChangeNotification N1;
        private GoogleCheckout _GatewayInstance = null;
        private GoogleCheckout GatewayInstance
        {
            get
            {
                if (_GatewayInstance != null)
                {
                    return _GatewayInstance;
                }
                else
                {
                    _GatewayInstance = GoogleCheckout.GetInstance();
                    return _GatewayInstance;
                }
            }
        }

        public OrderStateChangeHandler(OrderStateChangeNotification n1)
        {
            N1 = n1;
        }

        public void Process()
        {            
            string googleOrderNum = N1.googleordernumber;
            Order order = OrderDataSource.LoadForGoogleOrderNumber(googleOrderNum);
            if (order == null)
            {
                Logger.Warn("Unknown Google Order Number Order State Changed. GoogleOrderNumber=" + googleOrderNum );
            }
            else
            {
                //update financial order payment status
                Payment payment = AcHelper.GetGCPayment(order, GatewayInstance, true);                
                switch (N1.newfinancialorderstate)
                {
                    case FinancialOrderState.CHARGEABLE:
                        //authorized
                        payment.PaymentStatusReason = N1.reason;
                        if (payment.PaymentStatus == PaymentStatus.AuthorizationPending)
                        {
                            PaymentEngine.ProcessAuthorizePending(payment, GatewayInstance.PaymentGatewayId, true);
                        }
                        break;
                    case FinancialOrderState.CHARGING:
                        //capture pending
                        break;
                    case FinancialOrderState.CANCELLED:
                    case FinancialOrderState.CANCELLED_BY_GOOGLE:
                        //cancel the order
                        //if the order has been paid order cancellation will be preceeded by a refund notification
                        //so that aspect will be handled in refund handler
                        //TODO: Do we need to force void the remaining payments?
                        //payment.PaymentStatusReason = N1.reason;
                        //PaymentEngine.ForceVoid(payment, GatewayInstance.PaymentGatewayId, true);
                        if (order.OrderStatus.IsValid)
                        {
                            if (!string.IsNullOrEmpty(N1.reason))
                            {
                                OrderNote on = new OrderNote();
                                on.Comment = "Order Cancelled : " + N1.reason;
                                on.OrderId = order.OrderId;
                                order.Notes.Add(on);
                            }
                            order.Cancel();
                        }
                        break;
                    case FinancialOrderState.CHARGED:
                        //charge the payment
                        //we will handle this in charge amount notification handler only
                        //otherwise it can result in double charge transactions
                        /*payment.PaymentStatusReason = N1.reason;
                        if (payment.PaymentStatus == PaymentStatus.CapturePending)
                        {
                            PaymentEngine.ProcessCapturePending(payment, GatewayInstance.PaymentGatewayId, true);
                        }*/
                        break;
                    case FinancialOrderState.PAYMENT_DECLINED:
                        //payment declined
                        payment.PaymentStatusReason = N1.reason;
                        if (payment.PaymentStatus == PaymentStatus.CapturePending)
                        {
                            PaymentEngine.ProcessCapturePending(payment, GatewayInstance.PaymentGatewayId, false);
                        }
                        else if (payment.PaymentStatus == PaymentStatus.AuthorizationPending)
                        {
                            PaymentEngine.ProcessAuthorizePending(payment, GatewayInstance.PaymentGatewayId, false);
                        }
                        else
                        {
                            PaymentEngine.ForceCapture(payment, GatewayInstance.PaymentGatewayId, false);
                        }
                        break;

                    case FinancialOrderState.REVIEWING:
                        //No AC equivalent
                    default:
                        break;
                }
                
                //TODO : order statuses are custom defined in AC
                //update order shipment status
                switch (N1.newfulfillmentorderstate)
                {
                    case FulfillmentOrderState.NEW:
                        //do nothing                        
                        break;
                    case FulfillmentOrderState.DELIVERED:
                        //order has been shipped
                        foreach (OrderShipment os in order.Shipments)
                        {
                            if(!os.IsShipped) os.Ship(false);
                        }
                        break;
                    case FulfillmentOrderState.PROCESSING:
                        //no equivalent in AC
                        break;
                    case FulfillmentOrderState.WILL_NOT_DELIVER:
                        //this may happen if order is cancelled
                        //order cancelled event will take care of this
                        break;
                    default:
                        break;
                }

            }

        }

    }
}
