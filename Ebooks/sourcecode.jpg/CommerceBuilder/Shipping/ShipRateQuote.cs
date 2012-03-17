using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// Class that holds ship rate quote data
    /// </summary>
    public class ShipRateQuote
    {
        private int _ShipMethodId;
        private LSDecimal _Rate;
        private LSDecimal _Surcharge;
        private List<string> _Warnings;

        /// <summary>
        /// Warning messages associated with this rate quote
        /// </summary>
        public List<string> Warnings
        {
            get { return this._Warnings; }
            set { this._Warnings = value; }
        }

        /// <summary>
        /// Adds a warning message to this ship rate quote
        /// </summary>
        /// <param name="message">Warning message to add</param>
        public void AddWarning(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            if (this._Warnings == null)
            {
                this._Warnings = new List<string>();
            }
            if (!this._Warnings.Contains(message))
            {
                this._Warnings.Add(message);
            }
        }

        /// <summary>
        /// Adds a list of warning messages to this ship rate quote
        /// </summary>
        /// <param name="messages">List of warning messages</param>
        public void AddWarnings(List<string> messages)
        {
            if (messages == null || messages.Count == 0) return;

            if (this._Warnings == null)
            {
                this._Warnings = messages;
            }
            else
            {
                foreach (string message in messages)
                {
                    if (!this._Warnings.Contains(message))
                    {
                        this._Warnings.Add(message);
                    }
                }                
            }
        }

        /// <summary>
        /// Ship method name
        /// </summary>
        public string Name
        {
            get { return this.ShipMethod.Name; }
        }

        /// <summary>
        /// Base rate
        /// </summary>
        public LSDecimal Rate
        {
            get { return this._Rate; }
            set { this._Rate = value; }
        }

        /// <summary>
        /// Additional surcharge
        /// </summary>
        public LSDecimal Surcharge
        {
            get { return this._Surcharge; }
            set { this._Surcharge = value; }
        }

        /// <summary>
        /// Total rate including surcharge
        /// </summary>
        public LSDecimal TotalRate
        {
            get { return this.Rate + this.Surcharge; }
        }

        /// <summary>
        /// Id of the associated ship method
        /// </summary>
        public int ShipMethodId
        {
            get { return this._ShipMethodId; }
            set
            {
                this._ShipMethodId = value;
                if (this._ShipMethod != null && this._ShipMethod.ShipMethodId != value)
                    this._ShipMethod = null;
            }
        }

        private ShipMethod _ShipMethod;

        /// <summary>
        /// Associated ship method
        /// </summary>
        public ShipMethod ShipMethod
        {
            get
            {
                if (!this.ShipMethodLoaded)
                {
                    this._ShipMethod = new ShipMethod();
                    if (!this._ShipMethod.Load(this.ShipMethodId)) this._ShipMethod = null;
                }
                return this._ShipMethod;
            }
            set
            {
                if (value != null) this.ShipMethodId = value.ShipMethodId;
                else this.ShipMethodId = 0;
                this._ShipMethod = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ShipMethodLoaded { get { return (this._ShipMethod != null); } }

        /// <summary>
        /// Creates a copy of the current instance
        /// </summary>
        /// <returns>A copy of the current instance</returns>
        public ShipRateQuote Clone()
        {
            ShipRateQuote clonedQuote = new ShipRateQuote();
            clonedQuote.ShipMethodId = this.ShipMethodId;
            clonedQuote.Rate = this.Rate;
            clonedQuote.Surcharge = this.Surcharge;
            if (_Warnings != null && _Warnings.Count > 0) clonedQuote.AddWarnings(_Warnings);
            if (this.ShipMethodLoaded) clonedQuote.ShipMethod = this.ShipMethod;
            return clonedQuote;
        }
    }
}
