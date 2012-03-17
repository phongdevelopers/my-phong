using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// DataSource class for ShipRateQuote objects
    /// </summary>
    [DataObject(true)]
    public class ShipRateQuoteDataSource
    {
        /// <summary>
        /// Gets a collection of ship rate quotes for the given shipment
        /// </summary>
        /// <param name="shipment">The basket shipment for which to get the rate quotes</param>
        /// <returns>A collection of ship rate quotes</returns>
        public static Collection<ShipRateQuote> QuoteForShipment(IShipment shipment)
        {
            Collection<ShipRateQuote> rateQuotes = new Collection<ShipRateQuote>();
            //GET ALL OF THE POSSIBLE SHIPMETHODS
            ShipMethodCollection shipMethods = ShipMethodDataSource.LoadForShipment(shipment);
            foreach (ShipMethod method in shipMethods)
            {
                ShipRateQuote quote = method.GetShipRateQuote(shipment);
                if (quote != null) rateQuotes.Add(quote);
            }
            return rateQuotes;
        }

        /// <summary>
        /// Gets an array of ship rate quotes for the given basket
        /// </summary>
        /// <param name="basket">The basket for which to get the ship rate quotes</param>
        /// <returns>An array of ShipRateQuote objects</returns>
        public static ShipRateQuote[] QuoteForBasket(Basket basket)
        {
            //*******
            //SUMMARY: LOOP ALL SHIPMENTS IN BASKET AND GET QUOTES FOR EACH
            //ADD QUOTES TOGETHER FOR SERVICES IN COMMON TO ALL SHIPMENTS AND RETURN TOTALS
            //*******
            //CREATE A DICTIONARY TO HOLD QUOTED RATES
            Dictionary<int, ShipRateQuoteStack> rateQuotes = new Dictionary<int,ShipRateQuoteStack>();
            foreach (BasketShipment shipment in basket.Shipments)
            {
                //GET ALL OF THE POSSIBLE SHIPMETHODS
                ShipMethodCollection shipMethods = ShipMethodDataSource.LoadForShipment(shipment);
                foreach (ShipMethod method in shipMethods)
                {
                    ShipRateQuote quote = method.GetShipRateQuote(shipment);
                    if (quote != null)
                    {
                        if (rateQuotes.ContainsKey(method.ShipMethodId))
                        {
                            rateQuotes[method.ShipMethodId].Add(quote);
                        }
                        else
                        {
                            rateQuotes.Add(method.ShipMethodId, new ShipRateQuoteStack(quote));
                        }
                    }
                }
            }
            //NOW BUILD LIST OF QUOTES VALID FOR ALL SHIPMENTS
            List<ShipRateQuote> validQuotes = new List<ShipRateQuote>();
            foreach (ShipRateQuoteStack item in rateQuotes.Values)
            {
                if (item.ShipmentCount == basket.Shipments.Count)
                {
                    validQuotes.Add(item.ShipRateQuote);
                }
            }
            return validQuotes.ToArray();
        }

        /// <summary>
        /// Gets an array of ship rate quotes for the given basket
        /// </summary>
        /// <param name="basket">The basket for which to get the ship rate quotes</param>
        /// <param name="destination">The destination address for which to get the rates</param>        
        /// <returns>An array of ShipRateQuote objects</returns>
        public static ShipRateQuote[] QuoteForBasket(Basket basket, Address destination)
        {
            //*******
            //SUMMARY: LOOP ALL SHIPMENTS IN BASKET AND GET QUOTES FOR EACH
            //ADD QUOTES TOGETHER FOR SERVICES IN COMMON TO ALL SHIPMENTS AND RETURN TOTALS
            //*******
            //CREATE A DICTIONARY TO HOLD QUOTED RATES
            Dictionary<int, ShipRateQuoteStack> rateQuotes = new Dictionary<int, ShipRateQuoteStack>();
            foreach (BasketShipment shipment in basket.Shipments)
            {
                //GET ALL OF THE POSSIBLE SHIPMETHODS
                Address tempAddress = shipment.Address;
                shipment.SetAddress(destination);
                ShipMethodCollection shipMethods = ShipMethodDataSource.LoadForShipment(shipment);
                foreach (ShipMethod method in shipMethods)
                {
                    ShipRateQuote quote = method.GetShipRateQuote(shipment);
                    if (quote != null)
                    {
                        if (rateQuotes.ContainsKey(method.ShipMethodId))
                        {
                            rateQuotes[method.ShipMethodId].Add(quote);
                        }
                        else
                        {
                            rateQuotes.Add(method.ShipMethodId, new ShipRateQuoteStack(quote));
                        }
                    }
                }
                shipment.SetAddress(tempAddress);
            }
            //NOW BUILD LIST OF QUOTES VALID FOR ALL SHIPMENTS
            List<ShipRateQuote> validQuotes = new List<ShipRateQuote>();
            foreach (ShipRateQuoteStack item in rateQuotes.Values)
            {
                if (item.ShipmentCount == basket.Shipments.Count)
                {
                    validQuotes.Add(item.ShipRateQuote);
                }
            }
            return validQuotes.ToArray();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="basket">basket</param>
        /// <param name="destination">destination address</param>
        /// <returns><b>true</b> always</returns>
        public static bool QuoteForBasket2(Basket basket, Address destination)
        {
            return true;
        }

        private class ShipRateQuoteStack
        {
            public ShipRateQuote ShipRateQuote;
            public int ShipmentCount;
            public void Add(ShipRateQuote shipRateQuote)
            {
                this.ShipRateQuote.Rate += shipRateQuote.Rate;
                this.ShipRateQuote.Surcharge += shipRateQuote.Surcharge;
                this.ShipmentCount++;
            }
            public ShipRateQuoteStack(ShipRateQuote shipRateQuote)
            {
                this.ShipRateQuote = shipRateQuote;
                this.ShipmentCount = 1;
            }
        }
    }
}
