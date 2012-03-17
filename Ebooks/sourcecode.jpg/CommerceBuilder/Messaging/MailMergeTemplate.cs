//-----------------------------------------------------------------------
// <copyright file="MailMergeTemplate.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Messaging
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Text;
    using System.Net.Mail;
    using CommerceBuilder.Common;
    using CommerceBuilder.Stores;
    using CommerceBuilder.Utility;
    using CommerceBuilder.Users;
    using CommerceBuilder.Marketing;

    public class MailMergeTemplate
    {
        private Collection<MailAddress> _CCList = new Collection<MailAddress>();
        private Collection<MailAddress> _BCCList = new Collection<MailAddress>();
        private MailAddress _FromAddress;
        private string _Subject;
        private string _Body;
        private bool _IsHTML;

        /// <summary>
        /// Private storage for NVelocity parameters associated with this message
        /// </summary>
        private Hashtable _Parameters = new Hashtable();

        /// <summary>
        /// Gets or sets a list of addresses that should be included in the CC
        /// for every message sent
        /// </summary>
        public string CCList
        {
            get
            {
                return MailAddressHelper.ConvertToList(_CCList);
            }
            set
            {
                _CCList = MailAddressHelper.ParseList(value);
            }
        }

        /// <summary>
        /// Gets or sets a list of addresses that should be included in the BCC
        /// for every message sent
        /// </summary>
        public string BCCList
        {
            get
            {
                return MailAddressHelper.ConvertToList(_BCCList);
            }
            set
            {
                _BCCList = MailAddressHelper.ParseList(value);
            }
        }

        /// <summary>
        /// Gets or sets the address that the message is sent from
        /// </summary>
        public string FromAddress
        {
            get
            {
                if (_FromAddress == null) return string.Empty;
                return _FromAddress.Address;
            }
            set
            {
                _FromAddress = new MailAddress(value);
            }
        }

        /// <summary>
        /// Gets or sets the subject of the merge template
        /// </summary>
        public string Subject
        {
            get { return _Subject; }
            set { _Subject = value; }
        }

        /// <summary>
        /// Gets or sets the body content of the merge template
        /// </summary>
        public string Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        /// <summary>
        /// Gets or sets a flag that indicates whether the merge template is HTML mail
        /// </summary>
        public bool IsHTML
        {
            get { return _IsHTML; }
            set { _IsHTML = value; }
        }

        /// <summary>
        /// Gets the parameters for NVelocity processing that are shared for all recipients.
        /// </summary>
        /// <remarks>If a MailRecipient class specifies a parameter using the same name, the
        /// more specific MailRecipient parameter is used.</remarks>
        public Hashtable Parameters
        {
            get { return _Parameters; }
        }

        /// <summary>
        /// Generates a mail message for the given target
        /// </summary>
        /// <param name="recipient">The message target</param>
        /// <returns>A mail message generated for the given target</returns>
        public MailMessage GenerateMessage(MailMergeRecipient recipient)
        {
            // BUILD MESSAGE GOING TO SPECIFIED ADDRESSES
            MailMessage mailMessage = new MailMessage();
            foreach (MailAddress item in recipient.RecipientAddresses) { mailMessage.To.Add(item); }
            foreach (MailAddress item in _CCList) { mailMessage.CC.Add(item); }
            foreach (MailAddress item in _BCCList) { mailMessage.Bcc.Add(item); }

            // SET MESSAGE DATA
            mailMessage.From = _FromAddress;
            NVelocityEngine velocityEngine = NVelocityEngine.Instance;
            MergeSharedParameters(this.Parameters, recipient.Parameters);
            mailMessage.Subject = velocityEngine.Process(recipient.Parameters, this.Subject);
            mailMessage.Body = velocityEngine.Process(recipient.Parameters, this.Body);
            mailMessage.IsBodyHtml = this.IsHTML;

            // RETURN THE GENERATED MESSAGE
            return mailMessage;
        }

        private static void MergeSharedParameters(Hashtable sharedParameters, Hashtable recipientParameters)
        {
            foreach (object key in sharedParameters.Keys)
            {
                if (!recipientParameters.ContainsKey(key))
                {
                    recipientParameters.Add(key, sharedParameters[key]);
                }
            }
        }

        /// <summary>
        /// Sends the email message to the recipient list
        /// </summary>
        public void Send(MailMergeRecipientCollection recipients)
        {
            Send(recipients, true);
        }

        /// <summary>
        /// Sends the email messages generated for this merge template
        /// <param name="recipients">Recipients for the message</param>
        /// <param name="async">If true email messages are sent asynchronously</param>
        /// </summary>
        public void Send(MailMergeRecipientCollection recipients, bool async)
        {
            if (recipients != null && recipients.Count > 0)
            {
                if (async)
                {
                    SendEmailsDelegate del = new SendEmailsDelegate(AsycnSendEmails);
                    AsyncCallback cb = new AsyncCallback(AsyncSendEmailsCallback);
                    IAsyncResult ar = del.BeginInvoke(Token.Instance.StoreId, Token.Instance.UserId, recipients, cb, null);
                }
                else
                {
                    SendEmails(recipients);
                }
            }
        }

        /// <summary>
        /// Sends the email messages geneated for this email template
        /// </summary>
        private void SendEmails(MailMergeRecipientCollection recipients)
        {
            if (recipients != null && recipients.Count > 0)
            {
                foreach (MailMergeRecipient recipient in recipients)
                {
                    MailMessage message = null;
                    try
                    {
                        try
                        {
                            message = this.GenerateMessage(recipient);
                        }
                        catch 
                        { 
                            // SKIP THIS 
                            continue;
                        }
                        if (message != null)
                        {
                            EmailClient.Send(message);

                            //UPDATE LAST SEND DATE FOR EmailListUser
                            if (recipient.Parameters.ContainsKey("emailListUser"))
                            {
                                EmailListUser elu = (EmailListUser)recipient.Parameters["emailListUser"];
                                elu.LastSendDate = LocaleHelper.LocalNow;
                                elu.Save();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error sending email with subject '" + message.Subject + "'.", ex);
                    }
                }
            }
        }

        private delegate void SendEmailsDelegate(int storeId, int userId, MailMergeRecipientCollection recipients);
        private void AsycnSendEmails(int storeId, int userId, MailMergeRecipientCollection recipients)
        {
            //REINITIALIZE THE TOKEN WITH SAVED STORE/USER CONTEXT
            Store store = StoreDataSource.Load(storeId);
            if (store != null)
            {
                Token.Instance.InitStoreContext(store);
                User user = UserDataSource.Load(userId);
                Token.Instance.InitUserContext(user);
                SendEmails(recipients);
            }
        }

        private static void AsyncSendEmailsCallback(IAsyncResult ar)
        {
            SendEmailsDelegate del = (SendEmailsDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
            try
            {
                del.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                Logger.Write("Error", Logger.LogMessageType.Error, ex);
            }
        }

        /*
        /// <summary>
        /// Generates a message for the given target
        /// </summary>
        /// <param name="target">The message target</param>
        /// <returns>A message generated for the target</returns>
        public MailMessage GenerateMessage(MessageTargetCollection target)
        {
            //BUILD AN ARRAY OF MESSAGES TO BE SENT
            List<MailMessage> messages = new List<MailMessage>();
            NVelocityEngine velocityEngine = NVelocityEngine.Instance;

            //PARSE THE ADDRESSES
            List<MailAddress> addressList = this.ParseAddresses(this.ToAddress, out hasVendor);
            List<MailAddress> ccList = this.ParseAddresses(this.CCList);
            List<MailAddress> bccList = this.ParseAddresses(this.BCCList);

            //CHECK FOR TO ADDRESSES
            if (addressList.Count > 0)
            {
                //BUILD MESSAGE GOING TO SPECIFIC ADDRESSES
                MailMessage mailMessage = new MailMessage();
                //ADD TO ADDRESSES
                foreach (MailAddress item in addressList) { mailMessage.To.Add(item); }
                //ADD CC ADDRESSES
                foreach (MailAddress item in ccList) { mailMessage.CC.Add(item); }
                //ADD BCC ADDRESSES
                foreach (MailAddress item in bccList) { mailMessage.Bcc.Add(item); }
                //SET FROM ADDRESS
                mailMessage.From = new MailAddress(this.FromAddress);
                //PROCESS THE SUBJECT AND BODY
                mailMessage.Subject = velocityEngine.Process(this.Parameters, this.Subject);
                mailMessage.Body = velocityEngine.Process(this.Parameters, this.Body);
                mailMessage.IsBodyHtml = this.IsHTML;
                messages.Add(mailMessage);
            }

            //PROCESS VENDOR MESSAGES IF WE HAVE AN ORDER CONTEXT
            if (hasVendor && this.Parameters.ContainsKey("order"))
            {
                Order order = (Order)this.Parameters["order"];
                //DECIDE WHICH VENDORS NEED TO HAVE MESSAGES COMPILED
                VendorCollection vendors = VendorDataSource.LoadForOrder(order.OrderId);
                if (vendors.Count > 0)
                {
                    //VENDORS ARE FOUND, WE MUST COMPILE ONE MESSAGE FOR EACH VENDOR
                    foreach (Vendor vendor in vendors)
                    {
                        List<string> emailList = GetValidEmailList(vendor.Email);
                        if (emailList != null && emailList.Count > 0)
                        {
                            //BUILD VENDOR SPECIFIC ITEM COLLECTIONS
                            OrderItemCollection vendorItems = new OrderItemCollection();
                            OrderItemCollection vendorNonShippingItems = new OrderItemCollection();
                            foreach (OrderItem item in order.Items)
                            {
                                if (item.Product != null)
                                {
                                    //SEE IF THIS IS A PRODUCT THAT BELONGS TO THE VENDOR
                                    if (item.Product.VendorId == vendor.VendorId)
                                    {
                                        vendorItems.Add(item);
                                        if (item.OrderShipmentId == 0) vendorNonShippingItems.Add(item);
                                    }
                                }
                                else
                                {
                                    //SEE IF THIS IS A NON-PRODUCT ITEM THAT APPLIES
                                    //E.G. A DISCOUNT ON A VENDOR ITEM
                                    if (vendorItems.IndexOf(item.ParentItemId) > -1)
                                    {
                                        vendorItems.Add(item);
                                        if (item.OrderShipmentId == 0) vendorNonShippingItems.Add(item);
                                    }
                                }
                            }
                            //BUILD VENDOR SPECIFIC SHIPMENT COLLECTION
                            OrderShipmentCollection vendorShipments = new OrderShipmentCollection();
                            foreach (OrderItem item in vendorItems)
                            {
                                if ((item.OrderShipment != null) && (vendorShipments.IndexOf(item.OrderShipmentId) < 0))
                                {
                                    vendorShipments.Add(item.OrderShipment);
                                }
                            }
                            //CREATE VENDOR SPECIFIC PARAMETERS
                            Hashtable vendorParameters = new Hashtable(this.Parameters);
                            vendorParameters["Vendor"] = vendor;
                            vendorParameters["VendorShipments"] = vendorShipments;
                            vendorParameters["VendorItems"] = vendorItems;
                            vendorParameters["VendorNonShippingItems"] = vendorNonShippingItems;
                            //COMPILE VENDOR SPECIFIC MAIL MESSAGE
                            MailMessage mailMessage = new MailMessage();
                            //ADD TO ADDRESSES
                            foreach (string email in emailList) { mailMessage.To.Add(email); }
                            //ADD CC ADDRESSES
                            foreach (MailAddress item in ccList) { mailMessage.CC.Add(item); }
                            //ADD BCC ADDRESSES
                            foreach (MailAddress item in bccList) { mailMessage.Bcc.Add(item); }
                            //SET FROM ADDRESS
                            mailMessage.From = new MailAddress(this.FromAddress);
                            //PROCESS THE SUBJECT AND BODY
                            mailMessage.Subject = velocityEngine.Process(vendorParameters, this.Subject);
                            mailMessage.Body = velocityEngine.Process(vendorParameters, this.Body);
                            mailMessage.IsBodyHtml = this.IsHTML;
                            messages.Add(mailMessage);
                        }
                        else
                        {
                            Logger.Warn("Could not process vendor notification for '" + vendor.Name + "'; email address is invalid.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates mail messages for this email template
        /// </summary>
        /// <returns>An array of MailMessage objects generated</returns>
        private MailMessage[] GenerateMailMessages()
        {
            //BUILD AN ARRAY OF MESSAGES TO BE SENT
            List<MailMessage> messages = new List<MailMessage>();
            NVelocityEngine velocityEngine = NVelocityEngine.Instance;

            //PARSE THE ADDRESSES
            bool hasVendor;
            List<MailAddress> addressList = this.ParseAddresses(this.ToAddress, out hasVendor);
            List<MailAddress> ccList = this.ParseAddresses(this.CCList);
            List<MailAddress> bccList = this.ParseAddresses(this.BCCList);

            //CHECK FOR TO ADDRESSES
            if (addressList.Count > 0)
            {
                //BUILD MESSAGE GOING TO SPECIFIC ADDRESSES
                MailMessage mailMessage = new MailMessage();
                //ADD TO ADDRESSES
                foreach (MailAddress item in addressList) { mailMessage.To.Add(item); }
                //ADD CC ADDRESSES
                foreach (MailAddress item in ccList) { mailMessage.CC.Add(item); }
                //ADD BCC ADDRESSES
                foreach (MailAddress item in bccList) { mailMessage.Bcc.Add(item); }
                //SET FROM ADDRESS
                mailMessage.From = new MailAddress(this.FromAddress);
                //PROCESS THE SUBJECT AND BODY
                mailMessage.Subject = velocityEngine.Process(this.Parameters, this.Subject);
                mailMessage.Body = velocityEngine.Process(this.Parameters, this.Body);
                mailMessage.IsBodyHtml = this.IsHTML;
                messages.Add(mailMessage);
            }

            //PROCESS VENDOR MESSAGES IF WE HAVE AN ORDER CONTEXT
            if (hasVendor && this.Parameters.ContainsKey("order"))
            {
                Order order = (Order)this.Parameters["order"];
                //DECIDE WHICH VENDORS NEED TO HAVE MESSAGES COMPILED
                VendorCollection vendors = VendorDataSource.LoadForOrder(order.OrderId);
                if (vendors.Count > 0)
                {
                    //VENDORS ARE FOUND, WE MUST COMPILE ONE MESSAGE FOR EACH VENDOR
                    foreach (Vendor vendor in vendors)
                    {
                        List<string> emailList = GetValidEmailList(vendor.Email);
                        if (emailList != null && emailList.Count > 0)
                        {
                            //BUILD VENDOR SPECIFIC ITEM COLLECTIONS
                            OrderItemCollection vendorItems = new OrderItemCollection();
                            OrderItemCollection vendorNonShippingItems = new OrderItemCollection();
                            foreach (OrderItem item in order.Items)
                            {
                                if (item.Product != null)
                                {
                                    //SEE IF THIS IS A PRODUCT THAT BELONGS TO THE VENDOR
                                    if (item.Product.VendorId == vendor.VendorId)
                                    {
                                        vendorItems.Add(item);
                                        if (item.OrderShipmentId == 0) vendorNonShippingItems.Add(item);
                                    }
                                }
                                else
                                {
                                    //SEE IF THIS IS A NON-PRODUCT ITEM THAT APPLIES
                                    //E.G. A DISCOUNT ON A VENDOR ITEM
                                    if (vendorItems.IndexOf(item.ParentItemId) > -1)
                                    {
                                        vendorItems.Add(item);
                                        if (item.OrderShipmentId == 0) vendorNonShippingItems.Add(item);
                                    }
                                }
                            }
                            //BUILD VENDOR SPECIFIC SHIPMENT COLLECTION
                            OrderShipmentCollection vendorShipments = new OrderShipmentCollection();
                            foreach (OrderItem item in vendorItems)
                            {
                                if ((item.OrderShipment != null) && (vendorShipments.IndexOf(item.OrderShipmentId) < 0))
                                {
                                    vendorShipments.Add(item.OrderShipment);
                                }
                            }
                            //CREATE VENDOR SPECIFIC PARAMETERS
                            Hashtable vendorParameters = new Hashtable(this.Parameters);
                            vendorParameters["Vendor"] = vendor;
                            vendorParameters["VendorShipments"] = vendorShipments;
                            vendorParameters["VendorItems"] = vendorItems;
                            vendorParameters["VendorNonShippingItems"] = vendorNonShippingItems;
                            //COMPILE VENDOR SPECIFIC MAIL MESSAGE
                            MailMessage mailMessage = new MailMessage();
                            //ADD TO ADDRESSES
                            foreach (string email in emailList) { mailMessage.To.Add(email); }
                            //ADD CC ADDRESSES
                            foreach (MailAddress item in ccList) { mailMessage.CC.Add(item); }
                            //ADD BCC ADDRESSES
                            foreach (MailAddress item in bccList) { mailMessage.Bcc.Add(item); }
                            //SET FROM ADDRESS
                            mailMessage.From = new MailAddress(this.FromAddress);
                            //PROCESS THE SUBJECT AND BODY
                            mailMessage.Subject = velocityEngine.Process(vendorParameters, this.Subject);
                            mailMessage.Body = velocityEngine.Process(vendorParameters, this.Body);
                            mailMessage.IsBodyHtml = this.IsHTML;
                            messages.Add(mailMessage);
                        }
                        else
                        {
                            Logger.Warn("Could not process vendor notification for '" + vendor.Name + "'; email address is invalid.");
                        }
                    }
                }
            }

            //RETURN THE GENERATED MAIL MESSAGE
            if (messages.Count == 0) return null;
            return messages.ToArray();
        }


        /// <summary>
        /// Parses a given list of coma separated email addresses and creates a list of MailAddress objects.
        /// 'vendor' and 'customer' are special keywords that are replaced by appropriate email addresses of
        /// vendor or customer.
        /// </summary>
        /// <param name="addresses">A coman separated list of email addresses</param>
        /// <param name="hasVendor">If 'vendor' keyword is found as an email address this variable returns <b>true</b></param>
        /// <returns>A List of MailAddress objects parsed from the given coma separated list</returns>
        internal static List<MailAddress> ParseAddresses(string addresses, out bool hasVendor)
        {
            hasVendor = false;
            List<MailAddress> addressList = new List<MailAddress>();
            if (!string.IsNullOrEmpty(addresses))
            {
                string[] addressArray = addresses.Split(",".ToCharArray());
                foreach (string thisAddress in addressArray)
                {
                    string checkAddress = thisAddress.ToLowerInvariant().Trim();
                    if (checkAddress != "vendor")
                    {
                        bool emailIsValid = false;
                        if (checkAddress == "customer")
                        {
                            checkAddress = GetCustomerEmail();
                            emailIsValid = (!string.IsNullOrEmpty(checkAddress));
                        }
                        else emailIsValid = ValidationHelper.IsValidEmail(checkAddress);
                        if (emailIsValid) addressList.Add(new MailAddress(checkAddress));
                    }
                    else hasVendor = true;
                }
            }
            return addressList;
        }
        */
    }
}
