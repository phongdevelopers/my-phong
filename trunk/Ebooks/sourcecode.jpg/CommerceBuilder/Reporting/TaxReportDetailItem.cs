//-----------------------------------------------------------------------
// <copyright file="TaxReportDetailItem.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Common;
    using CommerceBuilder.Orders;
    using CommerceBuilder.Taxes;

    /// <summary>
    /// Contains detail data for a tax report
    /// </summary>
    public class TaxReportDetailItem
    {
        private int _TaxRuleId;
        private string _TaxName;
        private int _OrderId;
        private int _OrderNumber;
        private DateTime _OrderDate;
        private LSDecimal _TaxAmount;

        /// <summary>
        /// Gets or sets the tax rule id for this summary record
        /// </summary>
        public int TaxRuleId
        {
            get { return this._TaxRuleId; }
            set { this._TaxRuleId = value; }
        }

        /// <summary>
        /// Gets the associated tax rule for this summary record
        /// </summary>
        public TaxRule TaxRule
        {
            get { return TaxRuleDataSource.Load(this._TaxRuleId); }
        }

        /// <summary>
        /// Gets or sets the tax name for this summary record
        /// </summary>
        public string TaxName
        {
            get { return this._TaxName; }
            set { this._TaxName = value; }
        }

        /// <summary>
        /// Gets or sets the order id for this detail record
        /// </summary>
        public int OrderId
        {
            get { return this._OrderId; }
            set { this._OrderId = value; }
        }

        /// <summary>
        /// Gets or sets the order number for this detail record
        /// </summary>
        public int OrderNumber
        {
            get { return this._OrderNumber; }
            set { this._OrderNumber = value; }
        }

        /// <summary>
        /// Gets or sets the order date for this detail record
        /// </summary>
        public DateTime OrderDate
        {
            get { return this._OrderDate; }
            set { this._OrderDate = value; }
        }

        /// <summary>
        /// Gets the order associated with this detail 
        /// </summary>
        public Order Order
        {
            get { return OrderDataSource.Load(this._OrderId); }
        }

        /// <summary>
        /// Gets or sets the tax amount for this summary record
        /// </summary>
        public LSDecimal TaxAmount
        {
            get { return this._TaxAmount; }
            set { this._TaxAmount = value; }
        }
    }
}