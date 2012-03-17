using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Web.UI;
using CommerceBuilder.Common;
using CommerceBuilder.Marketing;
using CommerceBuilder.Orders;
using CommerceBuilder.Shipping.Providers;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes.Providers.AvaTax.TaxService;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes.Providers.AvaTax
{
    public class AvaTaxProvider : CommerceBuilder.Taxes.Providers.TaxProviderBase, IAddressValidatorService
    {
        private const string _EncKey = "3tQ9ZxEw";
        private string _AccountNumber;
        private string _CompanyCode;
        private string _License;
        private bool _RecordTaxBreakdown = true;
        private string _TaxServiceUrl = "https://development.avalara.net/Tax/TaxSvc.asmx";
        private string _AddressServiceUrl = "https://development.avalara.net/Address/AddressSvc.asmx";
        private string _SummaryTaxName = "Tax";
        private bool _UseDebugMode = false;
        
        public override bool Activated
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the unique account number provided by Avalara for verification against the service. 
        /// </summary>
        public string AccountNumber
        {
            get { return _AccountNumber; }
            set { _AccountNumber = value; }
        }

        /// <summary>
        /// Calculates tax for the given basket
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <returns>Tax items are added to the basket and total calculated tax is returned</returns>
        public override LSDecimal Calculate(Basket basket)
        {
            return Calculate(basket, true);
        }

        /// <summary>
        /// Calculates tax for the given basket
        /// </summary>
        /// <param name="basket">The basket</param>
        /// <param name="save">If true, items added to the basket are saved</param>
        /// <returns>Tax items are added to the basket and total calculated tax is returned</returns>
        public LSDecimal Calculate(Basket basket, bool save)
        {
            // DO NOT CALCULATE IF THE BASKET IS EMPTY
            if (basket.Items.Count == 0) return 0;
            // DO NOT CALCULATE IF BILLING ADDRESS IS INCOMPLETE
            User user = basket.User;
            if (!user.PrimaryAddress.IsValid) return 0;
            Store store = Token.Instance.Store;

            // CREATE THE TAX REQUEST OBJECT
            GetTaxRequest getTaxRequest = new GetTaxRequest();
            getTaxRequest.CompanyCode = this.CompanyCode;
            getTaxRequest.CurrencyCode = store.BaseCurrency.ISOCode;
            getTaxRequest.CustomerCode = user.UserId.ToString();

            // OBTAIN THE TAX LEVEL DETAIL IN THE RESPONSE
            getTaxRequest.DetailLevel = DetailLevel.Line;
            getTaxRequest.DocCode = basket.BasketId.ToString();
            getTaxRequest.DocDate = DateTime.Now;

            getTaxRequest.Commit = false;
            getTaxRequest.HashCode = 0;
            getTaxRequest.ServiceMode = ServiceMode.Automatic;
            getTaxRequest.PaymentDate = new DateTime(1900, 1, 1);
            getTaxRequest.ExchangeRate = 1;
            getTaxRequest.ExchangeRateEffDate = new DateTime(1900, 1, 1);

            // DURING THE CALCULATE STAGE, WE SHOULD ASSUME THAT THE BASKET IS TRANSIENT
            // WE WANT TO USE DocumentType.SalesOrder SO THAT THE DATA IS NOT SAVED TO AVATAX HISTORY
            getTaxRequest.DocType = DocumentType.SalesOrder;

            // BUILD A DICTIONARY OF AVALARA ADDRESS OBJECTS KEYED TO ABLECOMMERCE SHIPMENT ID
            Dictionary<int, BaseAddress> shipmentAddresses = new Dictionary<int, BaseAddress>();

            // POPULATE WITH THE BILLING ADDRESS AS DESTINATION FOR ANY NON-SHIPPABLE OBJECTS
            shipmentAddresses[0] = AvaTaxHelper.GetBaseAddressForTaxService(user.PrimaryAddress);

            // SET THE DEFAULT SHIPPING ADDRESS
            BasketShipment defaultShipment = null;
            int defaultShipmentId = 0;
            if (basket.Shipments.Count > 0)
            {
                // USE FIRST SHIPMENT AS DEFAULT SHIPPING ADDRESS
                defaultShipment = basket.Shipments[0];
                defaultShipmentId = defaultShipment.BasketShipmentId;
                shipmentAddresses[defaultShipmentId] = AvaTaxHelper.GetBaseAddressForTaxService(defaultShipment.Address);
            }
            getTaxRequest.DestinationCode = shipmentAddresses[defaultShipmentId].AddressCode;

            // BUILD A DICTIONARY OF AVALARA ADDRESS OBJECTS KEYED TO WAREHOUSE ID
            Dictionary<int, BaseAddress> warehouseAddresses = new Dictionary<int, BaseAddress>();

            // CONVERT THE DEFAULT WAREHOUSE TO AN AVALARA ADDRESS OBJECT
            CommerceBuilder.Shipping.Warehouse defaultWarehouse = null;
            if (defaultShipment != null) defaultWarehouse = defaultShipment.Warehouse;
            if (defaultWarehouse == null) defaultWarehouse = store.DefaultWarehouse;
            int defaultWarehouseId = defaultWarehouse.WarehouseId;
            warehouseAddresses[defaultWarehouseId] = AvaTaxHelper.GetBaseAddressForTaxService(defaultWarehouse);
            getTaxRequest.OriginCode = warehouseAddresses[defaultWarehouseId].AddressCode;

            // LIST THE ITEMS IN THE ORDER FOR THE TAX REQUEST
            List<Line> linesList = new List<Line>();
            foreach (BasketItem item in basket.Items)
            {
                Line line;
                switch (item.OrderItemType)
                {
                    case OrderItemType.Product:
                        line = GetTaxLine(basket, item, defaultShipmentId, shipmentAddresses, defaultWarehouseId, warehouseAddresses);
                        break;
                    case OrderItemType.Shipping:
                        line = GetTaxLine(basket, item, defaultShipmentId, shipmentAddresses, defaultWarehouseId, warehouseAddresses);
                        line.ItemCode = "SHIPPING";
                        if (string.IsNullOrEmpty(line.TaxCode)) line.TaxCode = "FR020100";
                        break;
                    case OrderItemType.Handling:
                        line = GetTaxLine(basket, item, defaultShipmentId, shipmentAddresses, defaultWarehouseId, warehouseAddresses);
                        line.ItemCode = "HANDLING";
                        line.TaxCode = "OH010000";
                        break;
                    case OrderItemType.GiftWrap:
                        line = GetTaxLine(basket, item, defaultShipmentId, shipmentAddresses, defaultWarehouseId, warehouseAddresses);
                        line.ItemCode = "GIFTWRAP";
                        break;
                    default:
                        // DO NOT INCLUDE THIS LINE
                        line = null;
                        break;
                }

                // SEE IF WE HAVE A VALID LINE ITEM TO ADD
                if (line != null)
                {
                    linesList.Add(line);
                }
            }

            // ADD LINES TO THE TAX REQUEST
            if (linesList.Count > 0)
            {
                getTaxRequest.Lines = linesList.ToArray();
            }

            // ADD IN ALL ADDRESSES FOR THIS REQUEST
            int addressCount = warehouseAddresses.Count + shipmentAddresses.Count;
            getTaxRequest.Addresses = new BaseAddress[addressCount];
            int addressIndex = 0;
            foreach (BaseAddress address in warehouseAddresses.Values)
            {
                getTaxRequest.Addresses[addressIndex] = address;
                addressIndex++;
            }
            foreach (BaseAddress address in shipmentAddresses.Values)
            {
                getTaxRequest.Addresses[addressIndex] = address;
                addressIndex++;
            }

            GetTaxResult getTaxResult;
            try
            {
                // SEND REQUEST TO AVALARA GATEWAY
                if (this.UseDebugMode) { this.RecordSoapCommunication(getTaxRequest, CommunicationDirection.Send); }
                getTaxResult = GetTax(getTaxRequest);
                if (this.UseDebugMode) { this.RecordSoapCommunication(getTaxResult, CommunicationDirection.Receive); }
            }
            catch (Exception ex)
            {
                // AN EXCEPTION OCCURRED DURING CALCULATION REQUEST, LOG AND EXIT
                Logger.Error("AvaTax threw an exception while calculating tax for basket " + basket.BasketId + ": " + ex.Message, ex);
                return 0;
            }

            // HANDLE WARNINGS OR ERRORS
            if (getTaxResult.ResultCode != SeverityLevel.Success)
            {
                // COMPILE THE MESSAGES FROM THE GATEWAY
                StringBuilder warningMessage = new StringBuilder();
                if (getTaxResult.Messages != null && getTaxResult.Messages.Length > 0)
                {
                    warningMessage.Append(" Message(s) from Gateway:");
                    foreach (Message message in getTaxResult.Messages)
                    {
                        warningMessage.Append(" " + message.Summary);
                    }
                }

                // HANDLE RESULT
                if (getTaxResult.ResultCode == SeverityLevel.Warning)
                {
                    // WARNING ONLY, LOG AND CONTINUE PROCESSING
                    Logger.Warn("AvaTax returned a warning while calculating tax for basket " + basket.BasketId + "." + warningMessage.ToString());
                }
                else
                {
                    // A FATAL ERROR OCCURRED, LOG ERROR AND EXIT METHOD
                    Logger.Error("AvaTax returned an error while calculating tax for basket " + basket.BasketId + "." + warningMessage.ToString());
                    return 0;
                }
            }

            // IF WE REACH THIS STAGE, THE RESULT IS EITHER SUCCESS OR WARNING AND TAX WAS CALCULATED
            if (getTaxResult.TotalTax > 0)
            {
                // THERE IS TAX, DETERMINE WHETHER WE NEED BREAKDOWN OR SUMMARY
                if (this.RecordTaxBreakdown)
                {
                    // RECORDING THE TAX BREAKDOWN
                    // LOOP THE TAX DETAILS OF THE RESPONSE TO CREATE ABLECOMMERCE BASKET ITEMS
                    foreach (TaxDetail taxDetail in getTaxResult.TaxSummary)
                    {
                        if (taxDetail.Tax > 0)
                        {
                            BasketItem basketItem = new BasketItem();
                            basketItem.BasketId = basket.BasketId;
                            basketItem.OrderItemType = OrderItemType.Tax;
                            basketItem.BasketShipmentId = 0;
                            basketItem.Name = taxDetail.TaxName;
                            basketItem.Sku = "AV";
                            basketItem.Price = taxDetail.Tax;
                            basketItem.Quantity = 1;
                            basket.Items.Add(basketItem);
                        }
                    }
                }
                else
                {
                    // RECORDING THE TAX SUMMARY ONLY
                    BasketItem basketItem = new BasketItem();
                    basketItem.BasketId = basket.BasketId;
                    basketItem.OrderItemType = OrderItemType.Tax;
                    basketItem.BasketShipmentId = 0;
                    basketItem.Name = this.SummaryTaxName;
                    basketItem.Sku = "AV";
                    basketItem.Price = getTaxResult.TotalTax;
                    basketItem.Quantity = 1;
                    basket.Items.Add(basketItem);
                }

                // SAVE BASKET IF INDICATED
                if (save) basket.Items.Save();
            }

            // RETURN THE CALCULATED TAX TOTAL
            return getTaxResult.TotalTax;
        }

        public override void Cancel(Basket basket)
        {
            // NO ACTION IS NEEDED BECAUSE PENDING SALESORDERS ARE NOT SAVED TO AVATAX HISTORY
        }

        /// <summary>
        /// Cancels taxes that have been commited for an order
        /// </summary>
        /// <param name="order">The order to cancel taxes for</param>
        public override void Cancel(Order order)
        {
            // NEED TO DETERMINE THE AVATAX DOCUMENT TO BE CANCELLED
            string avaTaxDocId = string.Empty;

            // LOOP THE ORDER NOTES TO FIND AVATAX DOCUMENT IDS
            for (int i = 0; i < order.Notes.Count; i++)
            {
                OrderNote note = order.Notes[i];
                if (note.Comment.StartsWith("AvaTax Document Id: ") && note.Comment.Length > 20)
                {
                    avaTaxDocId = note.Comment.Substring(20);
                    break;
                }
            }

            // IF WE CANNOT DETERMINE AVATAX DOCUMENT ID, NO NEED TO CONTINUE
            if (string.IsNullOrEmpty(avaTaxDocId)) return;

            // ATTEMPT TO CANCEL TAX
            CancelTaxRequest cancelTaxRequest = new CancelTaxRequest();
            cancelTaxRequest.CancelCode = CancelCode.DocDeleted;
            cancelTaxRequest.CompanyCode = this.CompanyCode;
            cancelTaxRequest.DocCode = order.OrderNumber.ToString();
            cancelTaxRequest.DocId = avaTaxDocId;
            cancelTaxRequest.DocType = DocumentType.SalesInvoice;

            CancelTaxResult cancelTaxResult;
            try
            {
                if (this.UseDebugMode) this.RecordSoapCommunication(cancelTaxRequest, CommunicationDirection.Send);
                cancelTaxResult = CancelTaxRequest(cancelTaxRequest);
                if (this.UseDebugMode) this.RecordSoapCommunication(cancelTaxResult, CommunicationDirection.Receive);
            }
            catch (Exception ex)
            {
                cancelTaxResult = null;
                Logger.Error("Error cancelling AvaTax transaction.  AbleCommerce Order Number: " + order.OrderNumber + "; AvaTax DocId: " + avaTaxDocId + "; Exception: " + ex.Message);
            }

            // CHECK FOR A VALID RESULT OBJECT
            if (cancelTaxResult != null)
            {
                // CHECK FOR ERRORS OR WARNINGS
                if (cancelTaxResult.ResultCode != SeverityLevel.Success)
                {
                    StringBuilder warningMessage = new StringBuilder();
                    warningMessage.Append("Error cancelling AvaTax transaction.  AbleCommerce Order Number: " + order.OrderNumber + "; AvaTax DocId: " + avaTaxDocId);
                    if (cancelTaxResult != null && cancelTaxResult.Messages.Length > 0)
                    {
                        warningMessage.Append("; Message(s) from Gateway:");
                        foreach (Message message in cancelTaxResult.Messages)
                        {
                            warningMessage.Append(" " + message.Summary);
                        }
                    }
                    Logger.Error(warningMessage.ToString());
                }

                // IF TAX WAS 
                if (cancelTaxResult.ResultCode == SeverityLevel.Success
                    || cancelTaxResult.ResultCode == SeverityLevel.Warning)
                {
                    // LOOP THE TAX ITEMS TO REMOVE CORRESONDING AVATAX DOCIDS
                    for (int i = order.Items.Count - 1; i >= 0; i--)
                    {
                        OrderItem item = order.Items[i];
                        string avSku = "AV" + avaTaxDocId;
                        if (item.OrderItemType == OrderItemType.Tax && item.Sku == avSku)
                        {
                            // THIS TAX HAS BEEN CANCELLED
                            order.Items.DeleteAt(i);
                        }
                    }
                }
            }
        }

        
        /// <summary>
        /// Commits any pending tax transactions for this order
        /// </summary>
        /// <param name="order">Order to commit tax transactions for</param>
        public override void Commit(Order order)
        {
            // FIRST REMOVE ANY PRE-CALCULATED AVATAX ITEMS FROM THE ORDER
            for (int i = order.Items.Count - 1; i >= 0; i--)
            {
                OrderItem item = order.Items[i];
                if (item.OrderItemType == OrderItemType.Tax && item.Sku == "AV")
                {
                    order.Items.DeleteAt(i);
                }
            }

            Store store = Token.Instance.Store;

            // CREATE THE TAX REQUEST OBJECT
            GetTaxRequest getTaxRequest = new GetTaxRequest();
            getTaxRequest.CompanyCode = this.CompanyCode;
            getTaxRequest.CurrencyCode = store.BaseCurrency.ISOCode;
            getTaxRequest.CustomerCode = order.UserId.ToString();

            // OBTAIN THE TAX LEVEL DETAIL IN THE RESPONSE
            getTaxRequest.DetailLevel = DetailLevel.Line;
            getTaxRequest.DocCode = order.OrderNumber.ToString();
            getTaxRequest.DocDate = LocaleHelper.FromLocalTime(order.OrderDate);

            getTaxRequest.Commit = false;
            getTaxRequest.HashCode = 0;
            getTaxRequest.ServiceMode = ServiceMode.Automatic;
            getTaxRequest.PaymentDate = new DateTime(1900, 1, 1);
            getTaxRequest.ExchangeRate = 1;
            getTaxRequest.ExchangeRateEffDate = new DateTime(1900, 1, 1);

            // DURING THE COMMIT STAGE, WE SHOULD ASSUME THAT THE ORDER IS COMPLETE
            // WE WANT TO USE DocumentType.SalesInvoice SO THAT THE DATA IS SAVED TO AVATAX HISTORY
            getTaxRequest.DocType = DocumentType.SalesInvoice;

            // BUILD A DICTIONARY OF AVALARA ADDRESS OBJECTS KEYED TO ABLECOMMERCE SHIPMENT ID
            Dictionary<int, BaseAddress> shipmentAddresses = new Dictionary<int, BaseAddress>();

            // POPULATE WITH THE BILLING ADDRESS AS DESTINATION FOR ANY NON-SHIPPABLE OBJECTS
            shipmentAddresses[0] = AvaTaxHelper.GetBaseAddressForTaxService(order);

            // SET THE DEFAULT SHIPPING ADDRESS
            OrderShipment defaultShipment = null;
            int defaultShipmentId = 0;
            if (order.Shipments.Count > 0)
            {
                // USE FIRST SHIPMENT AS DEFAULT SHIPPING ADDRESS
                defaultShipment = order.Shipments[0];
                defaultShipmentId = defaultShipment.OrderShipmentId;
                shipmentAddresses[defaultShipmentId] = AvaTaxHelper.GetBaseAddressForTaxService(defaultShipment.Address);
            }
            getTaxRequest.DestinationCode = shipmentAddresses[defaultShipmentId].AddressCode;

            // BUILD A DICTIONARY OF AVALARA ADDRESS OBJECTS KEYED TO WAREHOUSE ID
            Dictionary<int, BaseAddress> warehouseAddresses = new Dictionary<int, BaseAddress>();

            // CONVERT THE DEFAULT WAREHOUSE TO AN AVALARA ADDRESS OBJECT
            CommerceBuilder.Shipping.Warehouse defaultWarehouse = null;
            if (defaultShipment != null) defaultWarehouse = defaultShipment.Warehouse;
            if (defaultWarehouse == null) defaultWarehouse = store.DefaultWarehouse;
            int defaultWarehouseId = defaultWarehouse.WarehouseId;
            warehouseAddresses[defaultWarehouseId] = AvaTaxHelper.GetBaseAddressForTaxService(defaultWarehouse);
            getTaxRequest.OriginCode = warehouseAddresses[defaultWarehouseId].AddressCode;

            // LIST THE ITEMS IN THE ORDER FOR THE TAX REQUEST
            List<Line> linesList = new List<Line>();
            foreach (OrderItem item in order.Items)
            {
                Line line;
                switch (item.OrderItemType)
                {
                    case OrderItemType.Product:
                        line = GetTaxLine(order, item, defaultShipmentId, shipmentAddresses, defaultWarehouseId, warehouseAddresses);
                        break;
                    case OrderItemType.Shipping:
                        line = GetTaxLine(order, item, defaultShipmentId, shipmentAddresses, defaultWarehouseId, warehouseAddresses);
                        line.ItemCode = "SHIPPING";
                        if (string.IsNullOrEmpty(line.TaxCode)) line.TaxCode = "FR020100";
                        break;
                    case OrderItemType.Handling:
                        line = GetTaxLine(order, item, defaultShipmentId, shipmentAddresses, defaultWarehouseId, warehouseAddresses);
                        line.ItemCode = "HANDLING";
                        line.TaxCode = "OH010000";
                        break;
                    case OrderItemType.GiftWrap:
                        line = GetTaxLine(order, item, defaultShipmentId, shipmentAddresses, defaultWarehouseId, warehouseAddresses);
                        line.ItemCode = "GIFTWRAP";
                        break;
                    default:
                        // DO NOT INCLUDE THIS LINE
                        line = null;
                        break;
                }

                // SEE IF WE HAVE A VALID LINE ITEM TO ADD
                if (line != null)
                {
                    linesList.Add(line);
                }
            }

            // ADD LINES TO THE TAX REQUEST
            if (linesList.Count > 0)
            {
                getTaxRequest.Lines = linesList.ToArray();
            }

            // ADD IN ALL ADDRESSES FOR THIS REQUEST
            int addressCount = warehouseAddresses.Count + shipmentAddresses.Count;
            getTaxRequest.Addresses = new BaseAddress[addressCount];
            int addressIndex = 0;
            foreach (BaseAddress address in warehouseAddresses.Values)
            {
                getTaxRequest.Addresses[addressIndex] = address;
                addressIndex++;
            }
            foreach (BaseAddress address in shipmentAddresses.Values)
            {
                getTaxRequest.Addresses[addressIndex] = address;
                addressIndex++;
            }

            GetTaxResult getTaxResult;
            try
            {
                // SEND REQUEST TO AVALARA GATEWAY
                if (this.UseDebugMode) { this.RecordSoapCommunication(getTaxRequest, CommunicationDirection.Send); }
                getTaxResult = GetTax(getTaxRequest);
                if (this.UseDebugMode) { this.RecordSoapCommunication(getTaxResult, CommunicationDirection.Receive); }
            }
            catch (Exception ex)
            {
                // AN EXCEPTION OCCURRED DURING CALCULATION REQUEST, LOG AND EXIT
                Logger.Error("AvaTax threw an exception while committing tax for order " + order.OrderNumber + ": " + ex.Message, ex);
                return;
            }

            // HANDLE WARNINGS OR ERRORS
            if (getTaxResult.ResultCode != SeverityLevel.Success)
            {
                // COMPILE THE MESSAGES FROM THE GATEWAY
                StringBuilder warningMessage = new StringBuilder();
                if (getTaxResult.Messages != null && getTaxResult.Messages.Length > 0)
                {
                    warningMessage.Append(" Message(s) from Gateway:");
                    foreach (Message message in getTaxResult.Messages)
                    {
                        warningMessage.Append(" " + message.Summary);
                    }
                }

                // HANDLE RESULT
                if (getTaxResult.ResultCode == SeverityLevel.Warning)
                {
                    // WARNING ONLY, LOG AND CONTINUE PROCESSING
                    Logger.Warn("AvaTax returned a warning while committing tax for order " + order.OrderNumber + "." + warningMessage.ToString());
                }
                else
                {
                    // A FATAL ERROR OCCURRED, LOG ERROR AND EXIT METHOD
                    Logger.Error("AvaTax returned an error while committing tax for order " + order.OrderNumber + "." + warningMessage.ToString());
                    return;
                }
            }

            // IF WE REACH THIS STAGE, THE RESULT IS EITHER SUCCESS OR WARNING AND TAX WAS CALCULATED
            if (getTaxResult.TotalTax > 0)
            {
                // THERE IS TAX, DETERMINE WHETHER WE NEED BREAKDOWN OR SUMMARY
                if (this.RecordTaxBreakdown)
                {
                    // RECORDING THE TAX BREAKDOWN
                    // LOOP THE TAX DETAILS OF THE RESPONSE TO CREATE ABLECOMMERCE BASKET ITEMS
                    foreach (TaxDetail taxDetail in getTaxResult.TaxSummary)
                    {
                        if (taxDetail.Tax > 0)
                        {
                            OrderItem orderItem = new OrderItem();
                            orderItem.OrderId = order.OrderId;
                            orderItem.OrderItemType = OrderItemType.Tax;
                            orderItem.OrderShipmentId = 0;
                            orderItem.Name = taxDetail.TaxName;
                            orderItem.Sku = "AV" + getTaxResult.DocId;
                            orderItem.Price = taxDetail.Tax;
                            orderItem.Quantity = 1;
                            orderItem.Save();
                            order.Items.Add(orderItem);
                        }
                    }
                }
                else
                {
                    // RECORDING THE TAX SUMMARY ONLY
                    OrderItem orderItem = new OrderItem();
                    orderItem.OrderId = order.OrderId;
                    orderItem.OrderItemType = OrderItemType.Tax;
                    orderItem.OrderShipmentId = 0;
                    orderItem.Name = this.SummaryTaxName;
                    orderItem.Sku = "AV" + getTaxResult.DocId;
                    orderItem.Price = getTaxResult.TotalTax;
                    orderItem.Quantity = 1;
                    orderItem.Save();
                    order.Items.Add(orderItem);
                }
            }

            // ADD AN ORDER NOTE WITH THE AVATAX DOC ID
            string taxNoteText = "AvaTax Document Id: " + getTaxResult.DocId;
            OrderNote taxNote = new OrderNote(order.OrderId, order.UserId, LocaleHelper.LocalNow, taxNoteText, NoteType.SystemPrivate);
            order.Notes.Add(taxNote);
            taxNote.Save();

            // CREATE THE POST/COMMIT REQUEST
            PostTaxRequest postTaxRequest = new PostTaxRequest();
            postTaxRequest.Commit = true;
            postTaxRequest.CompanyCode = this.CompanyCode;
            postTaxRequest.DocCode = order.OrderNumber.ToString();
            postTaxRequest.DocDate = getTaxResult.DocDate;
            postTaxRequest.DocId = getTaxResult.DocId;
            postTaxRequest.DocType = DocumentType.SalesInvoice;
            postTaxRequest.TotalAmount = getTaxResult.TotalAmount;
            postTaxRequest.TotalTax = getTaxResult.TotalTax;

            // SEND THE REQUEST TO THE GATEWAY
            PostTaxResult postTaxResult;
            try
            {
                if (this.UseDebugMode) this.RecordSoapCommunication(postTaxRequest, CommunicationDirection.Send);
                postTaxResult = PostTax(postTaxRequest);
                if (this.UseDebugMode) this.RecordSoapCommunication(postTaxResult, CommunicationDirection.Receive);
            }
            catch (Exception ex)
            {
                // HANDLE ANY EXCEPTION DURING COMMIT STAGE
                Logger.Error("AvaTax threw an exception while committing tax for order " + order.OrderNumber + " / AvaTax DocId " + getTaxResult.DocId + ": " + ex.Message, ex);
                return;
            }

            // HANDLE WARNINGS OR ERRORS
            if (getTaxResult.ResultCode != SeverityLevel.Success)
            {
                // COMPILE THE MESSAGES FROM THE GATEWAY
                StringBuilder warningMessage = new StringBuilder();
                if (getTaxResult.Messages != null && getTaxResult.Messages.Length > 0)
                {
                    warningMessage.Append(" Message(s) from Gateway:");
                    foreach (Message message in getTaxResult.Messages)
                    {
                        warningMessage.Append(" " + message.Summary);
                    }
                }

                // HANDLE RESULT
                if (getTaxResult.ResultCode == SeverityLevel.Warning)
                {
                    // WARNING ONLY
                    Logger.Warn("AvaTax returned a warning while committing tax for order " + order.OrderNumber + " / AvaTax DocId " + getTaxResult.DocId + "." + warningMessage.ToString());
                }
                else
                {
                    // A FATAL ERROR OCCURRED
                    Logger.Error("AvaTax returned an error while committing tax for order " + order.OrderNumber + " / AvaTax DocId " + getTaxResult.DocId + "." + warningMessage.ToString());
                }
            }
        }

        /// <summary>
        /// Code that uniquely identifies the company, as set in the AvaTax dashboard.
        /// </summary>
        public string CompanyCode
        {
            get { return _CompanyCode; }
            set { _CompanyCode = value; }
        }

        /// <summary>
        /// A short description of the tax provider
        /// </summary>
        public override string Description
        {
            get { return "Whether you’re a large, multichannel retailer, a boutique vendor, or a home based business with a specialty shopping website, you need to accurately calculate sales tax instantly, avoiding any delay to the customer at the time of purchase. Avalara provides robust e-commerce sales tax software solutions that integrate seamlessly with your e-commerce shopping systems and provide instant, accurate sales tax calculations at checkout. In addition, Avalara can provide all the back end compliance functions such as reporting, returns and remittance for your business."; }
        }

        /// <summary>
        /// Generates a configuration data dictionary for this provider instance
        /// </summary>
        /// <returns>A string dictionary of configuration parameters</returns>
        public override Dictionary<string, string> GetConfigData()
        {
            Dictionary<string, string> configData = base.GetConfigData();
            configData["AccountNumber"] = EncryptionHelper.EncryptAES(this.AccountNumber, _EncKey);
            configData["CompanyCode"] = this.CompanyCode;
            configData["License"] = EncryptionHelper.EncryptAES(this.License, _EncKey);
            configData["RecordTaxBreakdown"] = this.RecordTaxBreakdown.ToString();
            configData["TaxServiceUrl"] = this.TaxServiceUrl;
            configData["AddressServiceUrl"] = this.AddressServiceUrl;
            configData["SummaryTaxName"] = this.SummaryTaxName;
            configData["UseDebugMode"] = this.UseDebugMode.ToString();
            return configData;
        }

        /// <summary>
        /// Gets the configuration URL
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public override string GetConfigUrl(ClientScriptManager cs)
        {
            return "AvaTax/Default.aspx";
        }

        /// <summary>
        /// Gets the Logo URL
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Taxes.Providers.AvaTax.Logo.gif");
            return string.Empty;
        }

        private Line GetTaxLine(Basket basket, BasketItem item, int defaultShipmentId, Dictionary<int, BaseAddress> shipmentAddresses, int defaultWarehouseId, Dictionary<int, BaseAddress> warehouseAddresses)
        {
            // GET THE SHIPMENT FOR THIS ITEM
            BasketShipment shipment = null;
            if (item.BasketShipmentId > 0)
            {
                int index = basket.Shipments.IndexOf(item.BasketShipmentId);
                if (index > -1)
                {
                    shipment = basket.Shipments[index];
                }
            }

            // CREATE THE AVATAX LINE
            Line line = new Line();
            line.Amount = (decimal)GetDiscountedPrice(basket, item);
            if (line.Amount != item.ExtendedPrice)
            {
                line.Ref1 = "Reg. Price: " + item.ExtendedPrice.ToString("LC");
            }
            line.Description = item.Name;
            // SEE IF WE HAVE TO SET AN ALTERNATE DESTINATION ADDRESS
            if (item.BasketShipmentId != defaultShipmentId)
            {
                // SEE IF THE DESTINATION ADDRESS IS AVAILABLE
                if (!shipmentAddresses.ContainsKey(item.BasketShipmentId))
                {
                    // CREATE THE DESTINATION ADDRESS INSTANCE
                    shipmentAddresses[item.BasketShipmentId] = AvaTaxHelper.GetBaseAddressForTaxService(shipment.Address);
                }
            }
            // SET THE DESTINATION FOR THIS ITEM, IT WILL BE THE ADDRESS CODE
            line.DestinationCode = shipmentAddresses[item.BasketShipmentId].AddressCode;
            line.ItemCode = item.Sku;
            line.No = item.BasketItemId.ToString();
            // SEE IF WE HAVE TO SET AN ALTERNATE ORIGIN ADDRESS
            if (shipment != null && shipment.WarehouseId != defaultWarehouseId)
            {
                // SEE IF THE ORIGIN ADDRESS IS AVAILABLE
                if (!warehouseAddresses.ContainsKey(shipment.WarehouseId))
                {
                    // CREATE THE ORIGIN ADDRESS INSTANCE
                    warehouseAddresses[shipment.WarehouseId] = AvaTaxHelper.GetBaseAddressForTaxService(shipment.Warehouse);
                }
                // SET THE ORIGIN ADDRESS FOR THIS INSTANCE, IT WILL BE THE ADDRESS CODE OF THE ALTERNATE ADDRESS
                line.OriginCode = warehouseAddresses[shipment.WarehouseId].AddressCode;
            }
            else
            {
                // COVERS NON-SHIPPABLE ITEMS AND ITEMS COMING FROM DEFAULT LOCATION
                line.OriginCode = warehouseAddresses[defaultWarehouseId].AddressCode;
            }
            line.Qty = item.Quantity;
            // SET TAX CODE FROM AbleCommerce LINE DATA
            TaxCode taxCode = item.TaxCode;
            if (taxCode != null) line.TaxCode = taxCode.Name;
            return line;
        }

        private LSDecimal GetDiscountedPrice(Basket basket, BasketItem parentItem)
        {
            LSDecimal discountedPrice = parentItem.ExtendedPrice;
            foreach (BasketItem item in basket.Items)
            {
                if (item.ParentItemId == parentItem.BasketItemId && item.IsChildItem)
                {
                    if (item.OrderItemType == OrderItemType.Discount
                        || item.OrderItemType == OrderItemType.Credit
                        || item.OrderItemType == OrderItemType.Charge
                        || item.OrderItemType == OrderItemType.Coupon)
                    {
                        discountedPrice += item.ExtendedPrice;
                    }
                }
            }
            return discountedPrice;
        }

        private Line GetTaxLine(Order order, OrderItem item, int defaultShipmentId, Dictionary<int, BaseAddress> shipmentAddresses, int defaultWarehouseId, Dictionary<int, BaseAddress> warehouseAddresses)
        {
            // GET THE SHIPMENT FOR THIS ITEM
            OrderShipment shipment = null;
            if (item.OrderShipmentId > 0)
            {
                int index = order.Shipments.IndexOf(item.OrderShipmentId);
                if (index > -1)
                {
                    shipment = order.Shipments[index];
                }
            }

            // CREATE THE AVATAX LINE
            Line line = new Line();
            line.Amount = (decimal)GetDiscountedPrice(order, item);
            if (line.Amount != item.ExtendedPrice)
            {
                line.Ref1 = "Reg. Price: " + item.ExtendedPrice.ToString("LC");
            }
            line.Description = item.Name;
            // SEE IF WE HAVE TO SET AN ALTERNATE DESTINATION ADDRESS
            if (item.OrderShipmentId != defaultShipmentId)
            {
                // SEE IF THE DESTINATION ADDRESS IS AVAILABLE
                if (!shipmentAddresses.ContainsKey(item.OrderShipmentId))
                {
                    // CREATE THE DESTINATION ADDRESS INSTANCE
                    shipmentAddresses[item.OrderShipmentId] = AvaTaxHelper.GetBaseAddressForTaxService(shipment.Address);
                }
            }
            line.DestinationCode = shipmentAddresses[item.OrderShipmentId].AddressCode;
            line.ItemCode = item.Sku;
            line.No = item.OrderItemId.ToString();
            // SEE IF WE HAVE TO SET AN ALTERNATE ORIGIN ADDRESS
            if (shipment.WarehouseId != defaultWarehouseId)
            {
                // SEE IF THE ORIGIN ADDRESS IS AVAILABLE
                if (!warehouseAddresses.ContainsKey(shipment.WarehouseId))
                {
                    // CREATE THE ORIGIN ADDRESS INSTANCE
                    warehouseAddresses[shipment.WarehouseId] = AvaTaxHelper.GetBaseAddressForTaxService(shipment.Warehouse);
                }
                // SET THE ORIGIN ADDRESS FOR THIS INSTANCE, IT WILL BE THE INDEX OF ADDRESS IN ADDRESS ARAY
                line.OriginCode = warehouseAddresses[shipment.WarehouseId].AddressCode;
            }
            else
            {
                line.OriginCode = warehouseAddresses[defaultWarehouseId].AddressCode;
            }
            line.Qty = item.Quantity;
            // SET TAX CODE FROM AbleCommerce LINE DATA
            TaxCode taxCode = item.TaxCode;
            if (taxCode != null) line.TaxCode = taxCode.Name;
            return line;
        }

        private LSDecimal GetDiscountedPrice(Order order, OrderItem parentItem)
        {
            LSDecimal discountedPrice = parentItem.ExtendedPrice;
            foreach (OrderItem item in order.Items)
            {
                if (item.ParentItemId == parentItem.OrderItemId && item.IsChildItem)
                {
                    if (item.OrderItemType == OrderItemType.Discount
                        || item.OrderItemType == OrderItemType.Credit
                        || item.OrderItemType == OrderItemType.Charge
                        || item.OrderItemType == OrderItemType.Coupon)
                    {
                        discountedPrice += item.ExtendedPrice;
                    }
                }
            }
            return discountedPrice;
        }

        /// <summary>
        /// Initializes this tax gateway with the given configuration data
        /// </summary>
        /// <param name="taxGatewayId">ID of this tax gateway</param>
        /// <param name="configurationData">configuration dictionary</param>
        public override void Initialize(int taxGatewayId, Dictionary<string, string> configurationData)
        {
            base.Initialize(taxGatewayId, configurationData);
            if (configurationData.ContainsKey("AccountNumber")) this.AccountNumber = EncryptionHelper.DecryptAES(configurationData["AccountNumber"], _EncKey);
            if (configurationData.ContainsKey("CompanyCode")) this.CompanyCode = configurationData["CompanyCode"];
            if (configurationData.ContainsKey("License")) this.License = EncryptionHelper.DecryptAES(configurationData["License"], _EncKey);
            if (configurationData.ContainsKey("RecordTaxBreakdown")) this.RecordTaxBreakdown = AlwaysConvert.ToBool(configurationData["RecordTaxBreakdown"], true);
            if (configurationData.ContainsKey("TaxServiceUrl")) this.TaxServiceUrl = configurationData["TaxServiceUrl"];
            if (configurationData.ContainsKey("AddressServiceUrl")) this.AddressServiceUrl = configurationData["AddressServiceUrl"];
            if (configurationData.ContainsKey("SummaryTaxName")) this.SummaryTaxName = configurationData["SummaryTaxName"];
            if (configurationData.ContainsKey("UseDebugMode")) this.UseDebugMode = AlwaysConvert.ToBool(configurationData["UseDebugMode"], false);
        }

        /// <summary>
        /// Gets or sets the unique alpha-numeric key for the Account provided by Avalara for verification against the service. 
        /// </summary>
        public string License
        {
            get { return _License; }
            set { _License = value; }
        }

        public override string Name
        {
            get { return "Avalara AvaTax"; }
        }

        private void RecordSoapCommunication(object soapData, CommunicationDirection direction)
        {
            try
            {
                string debugData = AvaTaxHelper.SerializeAvaTaxObject(soapData);
                base.RecordCommunication("AvaTax", direction, debugData, null);
            }
            catch { }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether tax breakdown should be recorded.  If true,
        /// order line items are created for each tax jurisdiction.  If false, the total tax amount
        /// is recorded using the tax label as the item name.
        /// </summary>
        public bool RecordTaxBreakdown
        {
            get { return _RecordTaxBreakdown; }
            set { _RecordTaxBreakdown = value; }
        }

        /// <summary>
        /// Gets or sets the location of the AvaTax  tax service.
        /// </summary>
        public string TaxServiceUrl
        {
            get { return _TaxServiceUrl; }
            set { _TaxServiceUrl = value; }
        }

        /// <summary>
        /// Gets or sets the location of the AvaTax address service.
        /// </summary>
        public string AddressServiceUrl
        {
            get { return _AddressServiceUrl; }
            set { _AddressServiceUrl = value; }
        }

        /// <summary>
        /// Gets or sets the name used for the tax line item when recording the tax breakdown 
        /// is not enabled.
        /// </summary>
        public string SummaryTaxName
        {
            get
            {
                if (string.IsNullOrEmpty(_SummaryTaxName)) return "Tax";
                return _SummaryTaxName;
            }
            set { _SummaryTaxName = value; }
        }

        /// <summary>
        /// Indicates whether debug mode is enabled
        /// </summary>
        public bool UseDebugMode
        {
            get { return _UseDebugMode; }
            set { _UseDebugMode = value; }
        }

        public override string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary>
        /// Gets the provider as configured for the store, or null if not configured
        /// </summary>
        /// <returns>The provider as configured for the store, or null if not configured</returns>
        public static AvaTaxProvider GetProvider()
        {
            int gatewayId = TaxGatewayDataSource.GetTaxGatewayIdByClassId(Misc.GetClassId(typeof(AvaTaxProvider)));
            if (gatewayId > 0)
            {
                TaxGateway gateway = TaxGatewayDataSource.Load(gatewayId);
                if (gateway != null)
                {
                    return (AvaTaxProvider)gateway.GetProviderInstance();
                }
            }
            return null;
        }

        public void ValidateAddress(CommerceBuilder.Users.Address address)
        {
            // CONFIGURE THE VALIDATION REQUEST
            AddressService.AddressSvcSoapClient svc = GetAddressService();
            AddressService.ValidateRequest validateRequest = new AddressService.ValidateRequest();
            validateRequest.Address = AvaTaxHelper.GetBaseAddressForAddressService(address);
            validateRequest.TextCase = AddressService.TextCase.Upper;

            // SEND THE REQUEST TO THE GATEWAY
            AddressService.ValidateResult validateResult;
            try
            {
                if (this.UseDebugMode) this.RecordSoapCommunication(validateRequest, CommunicationDirection.Send);
                validateResult = Validate(validateRequest);
                if (this.UseDebugMode) this.RecordSoapCommunication(validateResult, CommunicationDirection.Receive);
            }
            catch (Exception ex)
            {
                // HANDLE ANY EXCEPTION DURING COMMIT STAGE
                Logger.Error("AvaTax threw an exception while validating address " + address.AddressId + ": " + ex.Message, ex);
                return;
            }

            // IF THE VALIDATION WAS SUCCESSFUL UPDATE THE ABLECOMMERCE ADDRESS OBJECT
            if (validateResult.ResultCode == AddressService.SeverityLevel.Success
                || validateResult.ResultCode == AddressService.SeverityLevel.Warning)
            {
                AddressService.ValidAddress validAddress = validateResult.ValidAddresses[0];
                address.Address1 = validAddress.Line1;
                address.Address2 = validAddress.Line2;
                address.City = validAddress.City;
                address.Province = validAddress.Region;
                address.PostalCode = validAddress.PostalCode;
            }
        }

        public void ValidateAddress(CommerceBuilder.Shipping.Warehouse warehouse)
        {
            // CONFIGURE THE VALIDATION REQUEST
            AddressService.ValidateRequest validateRequest = new AddressService.ValidateRequest();
            validateRequest.Address = AvaTaxHelper.GetBaseAddressForAddressService(warehouse);
            validateRequest.TextCase = AddressService.TextCase.Upper;

            // SEND THE REQUEST TO THE GATEWAY
            AddressService.ValidateResult validateResult;
            try
            {
                if (this.UseDebugMode) this.RecordSoapCommunication(validateRequest, CommunicationDirection.Send);
                validateResult = Validate(validateRequest);
                if (this.UseDebugMode) this.RecordSoapCommunication(validateResult, CommunicationDirection.Receive);
            }
            catch (Exception ex)
            {
                // HANDLE ANY EXCEPTION DURING COMMIT STAGE
                Logger.Error("AvaTax threw an exception while validating warehouse " + warehouse.WarehouseId + ": " + ex.Message, ex);
                return;
            }

            // IF THE VALIDATION WAS SUCCESSFUL UPDATE THE ABLECOMMERCE ADDRESS OBJECT
            if (validateResult.ResultCode == AddressService.SeverityLevel.Success
                || validateResult.ResultCode == AddressService.SeverityLevel.Warning)
            {
                AddressService.ValidAddress validAddress = validateResult.ValidAddresses[0];
                warehouse.Address1 = validAddress.Line1;
                warehouse.Address2 = validAddress.Line2;
                warehouse.City = validAddress.City;
                warehouse.Province = validAddress.Region;
                warehouse.PostalCode = validAddress.PostalCode;
            }
        }


        /// <summary>
        /// Gets a tax service instance configured according to the merchant rules
        /// </summary>
        /// <returns>A configured tax service</returns>
        private TaxSvcSoapClient GetTaxService()
        {
            TaxSvcSoapClient svc = new TaxSvcSoapClient(CreateBasicHttpBinding(), new EndpointAddress(this.TaxServiceUrl));
            svc.Endpoint.Behaviors.Add(new AvalaraSecurityHeaderBehavior(this.AccountNumber, this.License));
            // RETURN CONFIGURED SERVICE
            return svc;
        }

        /// <summary>
        /// Gets an address service instance configured according to the merchant rules
        /// </summary>
        /// <returns>A configured address service</returns>
        private AddressService.AddressSvcSoapClient GetAddressService()
        {
            // CONFIGURE CLIENT AND SERVICE URL
            AddressService.AddressSvcSoapClient svc = new AddressService.AddressSvcSoapClient(CreateBasicHttpBinding(), new EndpointAddress(this.AddressServiceUrl));
            svc.Endpoint.Behaviors.Add(new AvalaraSecurityHeaderBehavior(this.AccountNumber, this.License));
            //RETURN CONFIGURED SERVICE
            return svc;
        }

        private Profile GetTaxProfile()
        {
            Profile profile = new Profile();
            //TODO: DO WE NEED TO MAKE IT DYNAMIC
            profile.Name = "5.7.0";
            profile.Client = "AbleCommerce " + AvaTaxHelper.GetAbleCommerceVersion() + "/" + this.Version;

            return profile;
        }

        private AddressService.Profile GetAddressProfile()
        {
            AddressService.Profile profile = new AddressService.Profile();
            //TODO: DO WE NEED TO MAKE IT DYNAMIC
            profile.Name = "5.7.0";
            profile.Client = "AbleCommerce " + AvaTaxHelper.GetAbleCommerceVersion()  + "/" + this.Version;

            return profile;
        }


        private GetTaxResult GetTax(GetTaxRequest request) 
        {
            TaxSvcSoapClient svc = GetTaxService();
            Profile profile = GetTaxProfile();
            return svc.GetTax(profile, request);
        }

        private CancelTaxResult CancelTaxRequest(CancelTaxRequest request)
        {
            TaxSvcSoapClient svc = GetTaxService();
            Profile profile = GetTaxProfile();
            
            //TODO: HOW TO UTILIZE THIS AUDIT MESSAGE
            AuditMessage auditMessage = new AuditMessage();
            return svc.CancelTax(auditMessage, profile, request);
        }

        private PostTaxResult PostTax(PostTaxRequest request)
        {
            TaxSvcSoapClient svc = GetTaxService();
            Profile profile = GetTaxProfile();
            return svc.PostTax(profile, request);
        }

        private AddressService.ValidateResult Validate(AddressService.ValidateRequest request)
        {
            AddressService.AddressSvcSoapClient svc = GetAddressService();
            AddressService.Profile profile = GetAddressProfile();
            return svc.Validate(profile, request);
        }

        private static BasicHttpBinding CreateBasicHttpBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.AllowCookies = false;
            binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            binding.OpenTimeout = new TimeSpan(0, 1, 0);
            binding.SendTimeout = new TimeSpan(0, 1, 0);
            binding.AllowCookies = false;
            
            // add more based on config file ...
            //buffer size
            binding.MaxBufferSize = 65536;
            binding.MaxBufferPoolSize = 534288;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = Encoding.UTF8;
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;
            
            //quotas
            binding.ReaderQuotas.MaxDepth = 32;
            binding.ReaderQuotas.MaxStringContentLength = 8192;

            binding.ReaderQuotas.MaxBytesPerRead = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;

            // add more based on config file ...
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            binding.Security.Transport.Realm = string.Empty;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;
            
            return binding;
        }
    }
}
