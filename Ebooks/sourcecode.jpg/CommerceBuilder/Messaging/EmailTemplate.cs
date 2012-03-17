using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using System.Text;
using System;
using System.IO;

namespace CommerceBuilder.Messaging
{
    /// <summary>
    /// Class representing an EmailTemplate object in database
    /// </summary>
    public partial class EmailTemplate
    {

        private string _Body = null;        
        internal bool BodyLoaded { get { return this._Body != null; } }
        private bool _BodyDirty = false;

        /// <summary>
        /// Body
        /// </summary>
        public String Body
        {
            get 
            {
                if (BodyLoaded) return _Body;

                string fileName = this.ContentFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = this.Name + ".html";
                }
                string filePath = "App_Data\\EmailTemplates\\" + this.StoreId + "\\" + fileName;
                filePath = Path.Combine(FileHelper.ApplicationBasePath, filePath);
                
                if (File.Exists(filePath))
                {
                    try
                    {
                        _Body = File.ReadAllText(filePath);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("Error reading content file for email template '" + this.Name + "'. " + filePath, ex);
                        _Body = string.Empty;
                    }
                }
                else
                {
                    Logger.Warn("Content file for email template '" + this.Name + "' does not exist. " + filePath);
                    _Body = string.Empty;
                }

                _BodyDirty = false;
                return _Body;
            }
            
            set
            {
                if (value == null) value = string.Empty;
                if (this._Body != value)
                {
                    this._Body = value;
                    this._BodyDirty = true;
                }
            }
             
        }

        /// <summary>
        /// Reloads the message body from the message content file
        /// </summary>
        /// <returns>Returns the message body</returns>
        public string ReloadBody()
        {
            _Body = null; //this will force body to reload
            return Body;
        }

        private Hashtable _Parameters = new Hashtable();

        /// <summary>
        /// Parameters
        /// </summary>
        public Hashtable Parameters { get { return _Parameters; } }

        /// <summary>
        /// Parses a given list of coma separated email addresses and creates a list of MailAddress objects.
        /// 'vendor' and 'customer' are special keywords that are replaced by appropriate email addresses of
        /// vendor or customer.
        /// </summary>
        /// <param name="addresses">A coman separated list of email addresses</param>
        /// <returns>A List of MailAddress objects parsed from the given coma separated list</returns>
        public virtual List<MailAddress> ParseAddresses(string addresses)
        {
            bool ignoreVendor;
            return this.ParseAddresses(addresses, out ignoreVendor);
        }

        /// <summary>
        /// Retrieve the best email address to use to contact the customer related to the triggered event(s).
        /// </summary>
        /// <returns>The best email address to use to contact the customer.  If the return value is not an 
        /// empty string, it is guaranteed to be a valid email format.</returns>
        private string GetCustomerEmail()
        {
            //FIND AN EMAIL ADDRESS
            //PRECENDENCE ORDER EMAIL, USERNAME, EMAIL, BILLING EMAIL
            if (this.Parameters.ContainsKey("order"))
            {
                Order order = (Order)this.Parameters["order"];
                string checkAddress = order.BillToEmail;
                if (ValidationHelper.IsValidEmail(checkAddress)) return checkAddress;
            }
            if (this.Parameters.ContainsKey("customer"))
            {
                User customer = this.Parameters["customer"] as User;
                if (customer != null)
                {
                    string checkAddress = customer.Email;
                    if (ValidationHelper.IsValidEmail(checkAddress)) return checkAddress;
                    checkAddress = customer.UserName;
                    if (ValidationHelper.IsValidEmail(checkAddress)) return checkAddress;
                    checkAddress = customer.PrimaryAddress.Email;
                    if (ValidationHelper.IsValidEmail(checkAddress)) return checkAddress;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Parses a given list of coma separated email addresses and creates a list of MailAddress objects.
        /// 'vendor' and 'customer' are special keywords that are replaced by appropriate email addresses of
        /// vendor or customer.
        /// </summary>
        /// <param name="addresses">A coman separated list of email addresses</param>
        /// <param name="hasVendor">If 'vendor' keyword is found as an email address this variable returns <b>true</b></param>
        /// <returns>A List of MailAddress objects parsed from the given coma separated list</returns>
        public virtual List<MailAddress> ParseAddresses(string addresses, out bool hasVendor)
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

        /// <summary>
        /// Add the given email address to 'TO' address list
        /// </summary>
        /// <param name="toAddress">The email address to add</param>
        public void AddToAddress(string toAddress)
        {
            if (!string.IsNullOrEmpty(toAddress))
            {
                if (string.IsNullOrEmpty(this.ToAddress))
                {
                    this.ToAddress = toAddress;
                }
                else if (ToAddress.EndsWith(","))
                {
                    this.ToAddress = this.ToAddress + toAddress;
                }
                else
                {
                    this.ToAddress = this.ToAddress + "," + toAddress;
                }
            }
        }

        /// <summary>
        /// Add the given email address to 'CC' address list
        /// </summary>
        /// <param name="ccAddress">The email address to add</param>
        public void AddCcAddress(string ccAddress)
        {
            if (!string.IsNullOrEmpty(ccAddress))
            {
                if (string.IsNullOrEmpty(this.CCList))
                {
                    this.CCList = ccAddress;
                }
                else if (CCList.EndsWith(","))
                {
                    this.CCList = this.CCList + ccAddress;
                }
                else
                {
                    this.CCList = this.CCList + "," + ccAddress;
                }
            }
        }

        /// <summary>
        /// Add the given email address to 'BCC' address list
        /// </summary>
        /// <param name="bccAddress">The email address to add</param>
        public void AddBccAddress(string bccAddress)
        {
            if (!string.IsNullOrEmpty(bccAddress))
            {
                if (string.IsNullOrEmpty(this.BCCList))
                {
                    this.BCCList = bccAddress;
                }
                else if (BCCList.EndsWith(","))
                {
                    this.BCCList = this.BCCList + bccAddress;
                }
                else
                {
                    this.BCCList = this.BCCList + "," + bccAddress;
                }
            }
        }

        /// <summary>
        /// Generates mail messages for this email template
        /// </summary>
        /// <returns>An array of MailMessage objects generated</returns>
        public MailMessage[] GenerateMailMessages()
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
            else
                if (hasVendor && this.Parameters.ContainsKey("vendors"))
                {
                    Vendor[] vendors = (Vendor[])this.Parameters["vendors"];
                    Product[] products = (Product[])this.Parameters["products"];
                    ProductVariant[] variants = (ProductVariant[])this.Parameters["variants"];
                    
                    if (vendors != null && vendors.Length > 0)
                    {
                        foreach (Vendor vendor in vendors)
                        {
                            List<string> emailList = GetValidEmailList(vendor.Email);
                            if (emailList != null && emailList.Count > 0)
                            {
                                List<Product> vendorProducts = new List<Product>();
                                List<ProductVariant> vendorVariants = new List<ProductVariant>();

                                if (products != null)
                                {
                                    foreach (Product product in products)
                                    {
                                        if (product.Vendor != null && product.VendorId == vendor.VendorId)
                                            vendorProducts.Add(product);
                                    }
                                }

                                if (variants != null)
                                {
                                    foreach (ProductVariant variant in variants)
                                    {
                                        if (variant.Product.Vendor != null && variant.Product.Vendor.VendorId == vendor.VendorId)
                                            vendorVariants.Add(variant);
                                    }
                                }
                                
                                //CREATE VENDOR SPECIFIC PARAMETERS
                                Hashtable vendorParameters = new Hashtable(this.Parameters);
                                vendorParameters["vendor"] = vendor;
                                vendorParameters["products"] = vendorProducts;
                                vendorParameters["variants"] = vendorVariants;
                                
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

        private static List<string> GetValidEmailList(string emailString)
        {
            if (string.IsNullOrEmpty(emailString)) return new List<string>();
            char[] splitter = { ',' };
            string[] emails = emailString.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);

            List<string> emailList = new List<string>();
            foreach (string email in emails)
            {
                if (ValidationHelper.IsValidEmail(email.Trim()))
                {
                    emailList.Add(email);
                }
            }

            return emailList;
        }

        /// <summary>
        /// Sends the email messages geneated for this email template
        /// </summary>
        public void Send()
        {
            Send(true);
        }

        /// <summary>
        /// Sends the email messages geneated for this email template
        /// <param name="async">If true email messages are sent asynchronously</param>
        /// </summary>
        public void Send(bool async)
        {
            MailMessage[] messages = null;
            try
            {
                messages = this.GenerateMailMessages();
            }
            catch (Exception ex)
            {
                Logger.Error("Error generating email messages for template '" + this.Name + "' having subject '" + this.Subject + "'.", ex);
            }

            if (messages != null && messages.Length > 0)
            {
                if (async)
                {
                    SendEmailsDelegate del = new SendEmailsDelegate(AsycnSendEmails);
                    AsyncCallback cb = new AsyncCallback(AsyncSendEmailsCallback);
                    IAsyncResult ar = del.BeginInvoke(Token.Instance.StoreId, Token.Instance.UserId, messages, cb, null);
                }
                else
                {
                    SendEmails(messages);
                }
            }
        }

        /// <summary>
        /// Sends the email messages geneated for this email template
        /// </summary>
        private void SendEmails(MailMessage[] messages)
        {
            if (messages != null && messages.Length > 0)
            {
                foreach (MailMessage message in messages)
                {
                    try
                    {
                        EmailClient.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error sending email with subject '" + message.Subject + "'.", ex);
                    }
                }
            }
        }

        private delegate void SendEmailsDelegate(int storeId, int userId, MailMessage[] messages);
        private void AsycnSendEmails(int storeId, int userId, MailMessage[] messages)
        {
            //REINITIALIZE THE TOKEN WITH SAVED STORE/USER CONTEXT
            Store store = StoreDataSource.Load(storeId);
            if (store != null)
            {
                Token.Instance.InitStoreContext(store);
                User user = UserDataSource.Load(userId);
                Token.Instance.InitUserContext(user);
                SendEmails(messages);
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

        /// <summary>
        /// Generates a copy of the indicated item.
        /// </summary>
        /// <param name="emailTemplateId">The id of the item to copy.</param>
        /// <param name="deepCopy">If true, all related child data for this item is copied.  If false, only the base item is copied.</param>
        /// <returns>A copy of the item</returns>
        /// <remarks>The returned copy has not been persisted to the database, you must call Save method to persist the new item.</remarks>
        public static EmailTemplate Copy(int emailTemplateId, bool deepCopy)
        {
            EmailTemplate copy = EmailTemplateDataSource.Load(emailTemplateId);
            if (copy != null)
            {
                if (deepCopy)
                {
                    //LOAD THE CHILD COLLECTIONS AND RESET
                    foreach (EmailTemplateTrigger trigger in copy.Triggers)
                    {
                        trigger.EmailTemplateId = 0;
                    }
                }
                copy.EmailTemplateId = 0;
                return copy;
            }
            return null;
        }

        /// <summary>
        /// Deletes this EmailTemplate object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            // RELOAD THE OBJECT DATA
            this.Load(this.EmailTemplateId);

            // DELETE THE CONTENT FILE
            string templatesPath = "App_Data\\EmailTemplates\\" + Token.Instance.StoreId + "\\" + this.ContentFileName;
            templatesPath = Path.Combine(FileHelper.ApplicationBasePath, templatesPath);
            try
            {
                if (File.Exists(templatesPath))
                {
                    File.Delete(templatesPath);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Error deleting email template content file '" + templatesPath + "'.", ex);
            }

            return BaseDelete();
        }

        /// <summary>
        /// Saves this EmailTemplate object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (string.IsNullOrEmpty(this.ContentFileName))
            {
                this.ContentFileName = this.Name + " ID" + this.EmailTemplateId + ".html";
            }
            
            SaveResult result = BaseSave();
            
            if (result != SaveResult.Failed)
            {
                if (this._BodyDirty)
                {
                    //try to save the template body now
                    string templatesPath = "App_Data\\EmailTemplates\\" + Token.Instance.StoreId + "\\" + this.ContentFileName;
                    templatesPath = Path.Combine(FileHelper.ApplicationBasePath, templatesPath);
                    try
                    {
                        File.WriteAllText(templatesPath, this.Body);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("Error saving email template content file '" + templatesPath + "'.", ex);
                    }
                }
            }

            return result;
        }

    }
}
