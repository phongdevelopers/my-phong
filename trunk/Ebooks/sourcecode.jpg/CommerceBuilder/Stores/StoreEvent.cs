using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// Enumeration that represents the events that occur in a store
    /// </summary>
    public enum StoreEvent : int
    {
        /// <summary>
        /// No event
        /// </summary>
        None = 0,
        
        //ORDER EVENTS START AT 100
        /// <summary>
        /// Order was placed
        /// </summary>
        OrderPlaced = 100,
        /// <summary>
        /// Payment was authorized
        /// </summary>
        PaymentAuthorized, //101
        /// <summary>
        /// Payment authorization was failed
        /// </summary>
        PaymentAuthorizationFailed, //102
        /// <summary>
        /// Payment was partially captured
        /// </summary>
        PaymentCapturedPartial, //103
        /// <summary>
        /// Payment was captured
        /// </summary>
        PaymentCaptured, //104
        /// <summary>
        /// Payment capture was failed
        /// </summary>
        PaymentCaptureFailed, //105
        /// <summary>
        /// Order was paid completely
        /// </summary>
        OrderPaid, //106
        /// <summary>
        /// Order was paid partially
        /// </summary>
        OrderPaidPartial, //107
        /// <summary>
        /// Order was overpaid
        /// </summary>
        OrderPaidCreditBalance, //108
        /// <summary>
        /// Order was shipped
        /// </summary>
        OrderShipped, //109
        /// <summary>
        /// Order was shipped partially
        /// </summary>
        OrderShippedPartial, //110
        /// <summary>
        /// A shipment was shipped
        /// </summary>
        ShipmentShipped, //111
        /// <summary>
        /// Order note was added by merchant
        /// </summary>
        OrderNoteAddedByMerchant, //112
        /// <summary>
        /// Order note was added by customer
        /// </summary>
        OrderNoteAddedByCustomer, //113
        /// <summary>
        /// Order status was updated
        /// </summary>
        OrderStatusUpdated, //114
        /// <summary>
        /// Order was cancelled
        /// </summary>
        OrderCancelled, //115
        /// <summary>
        /// Gift certificate was validated
        /// </summary>
        GiftCertificateValidated, //116
        /// <summary>
        /// An order with no shippments was fully paid
        /// </summary>
        OrderPaidNoShipments, //117
        

        //CUSTOMER EVENTS START AT 200
        /// <summary>
        /// Customer password request was initiated
        /// </summary>
        CustomerPasswordRequest = 200,


        //INVENTORY EVENTS START AT 300
        /// <summary>
        /// A low inventory item was purchased
        /// </summary>
        LowInventoryItemPurchased=300
    }
}
