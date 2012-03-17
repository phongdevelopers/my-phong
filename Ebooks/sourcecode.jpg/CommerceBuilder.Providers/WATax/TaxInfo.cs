using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Taxes.Providers.WATax
{
    internal class TaxInfo
    {
        private decimal _TaxRate;
        private string _TaxSku;
        private string _TaxName;

        public TaxInfo(string name, string sku, decimal rate)
        {
            _TaxName = name;
            _TaxRate = rate;
            _TaxSku = sku;
        }

        public string TaxName
        {
            get { return _TaxName; }
        }

        public decimal TaxRate
        {
            get { return _TaxRate; }
        }

        public string TaxSku
        {
            get { return _TaxSku; }
        }

        /// <summary>
        /// Calculates the amount of tax for the given value
        /// </summary>
        /// <param name="value">The value to calculate tax for</param>
        /// <returns>The calculated tax</returns>
        public decimal Calculate(decimal value)
        {
            // THE RATE IS ALREADY IN PERCENTAGE, MULTIPLY IT TO COST TO GET TAX AMOUNT
            decimal tax = value * this.TaxRate;
            return TaxHelper.Round((decimal)tax, 2, RoundingRule.Common);
        }
    }
}
