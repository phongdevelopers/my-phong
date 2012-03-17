using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// A class used as information holder for an applied tax
    /// </summary>
    public class TaxInfo
    {
        private LSDecimal _Price;
        private LSDecimal _Tax;
        private LSDecimal _TaxRate;

        /// <summary>
        /// The price on which the tax is applied
        /// </summary>
        public LSDecimal Price
        {
            get { return _Price; }
            set { _Price = value; }
        }

        /// <summary>
        /// The amount of tax
        /// </summary>
        public LSDecimal Tax
        {
            get { return _Tax; }
            set { _Tax = value; }
        }

        /// <summary>
        /// Total price with tax
        /// </summary>
        public LSDecimal PriceWithTax
        {
            get { return _Price + _Tax; }
        }

        /// <summary>
        /// Tax rate applied
        /// </summary>
        public LSDecimal TaxRate
        {
            get { return _TaxRate; }
            set { _TaxRate = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="tax">Applied tax</param>
        /// <param name="taxRate">Tax rate</param>
        public TaxInfo(LSDecimal price, LSDecimal tax, LSDecimal taxRate)
        {
            _Price = price;
            _Tax = tax;
            _TaxRate = taxRate;
        }
    }
}