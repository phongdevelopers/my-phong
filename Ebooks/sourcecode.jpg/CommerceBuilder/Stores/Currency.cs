using System;
using System.Globalization;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores
{
    public partial class Currency : IFormatProvider, ICustomFormatter
    {
        private NumberFormatInfo _NumberFormat;

        /// <summary>
        /// Returns a NumberFormat instance configured with the format rules defined
        /// by this currency.
        /// </summary>
        protected NumberFormatInfo NumberFormat
        {
            get
            {
                if (_NumberFormat == null || this.IsDirty)
                {
                    _NumberFormat = new NumberFormatInfo();
                    _NumberFormat.CurrencyDecimalDigits = this.DecimalDigits;
                    _NumberFormat.CurrencyGroupSeparator = this.GroupSeparator;
                    _NumberFormat.CurrencyNegativePattern = this.NegativePattern;
                    _NumberFormat.CurrencyPositivePattern = this.PositivePattern;
                    _NumberFormat.CurrencySymbol = this.CurrencySymbol;
                    _NumberFormat.NegativeSign = this.NegativeSign;
                    if (!string.IsNullOrEmpty(this.DecimalSeparator))
                    {
                        _NumberFormat.CurrencyDecimalSeparator = this.DecimalSeparator;
                    }
                    int[] groupSizes = AlwaysConvert.ToIntArray(this.GroupSizes);
                    if (groupSizes != null)
                    {
                        _NumberFormat.CurrencyGroupSizes = groupSizes;
                    }
                }
                return _NumberFormat;
            }
        }

        /// <summary>
        /// Indicates whether this is the base currency for the store.
        /// </summary>
        public bool IsBaseCurrency
        {
            get
            {
                return (Token.Instance.Store.BaseCurrency.CurrencyId == this.CurrencyId);
            }
        }

        #region IFormatProvider Members

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to 
        /// return.</param>
        /// <returns>An instance of the object specified by formatType, if the IFormatProvider 
        /// implementation can supply that type of object; otherwise, null reference</returns>
        public object GetFormat(Type formatType)
        {
            if (typeof(ICustomFormatter).Equals(formatType)) return this;
            if (typeof(NumberFormatInfo).Equals(formatType)) return this.NumberFormat;
            return null;
        }

        #endregion

        #region ICustomFormatter Members

        /// <summary>
        /// Converts the value of a specified object to an equivalent string 
        /// representation using specified format and culture-specific formatting 
        /// information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">An IFormatProvider object that supplies format 
        /// information about the current instance.</param>
        /// <returns>The string representation of the value of arg, formatted as 
        /// specified by format and formatProvider.</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null) throw new ArgumentNullException("arg");
            if (format != null)
            {
                string s = format.Trim().ToLower();
                if (s == "lc")
                {
                    if (arg is LSDecimal) return FormatCurrency((LSDecimal)arg, true);
                    else if (arg is decimal) return FormatCurrency(new LSDecimal((decimal)arg), true);
                } else if (s == "lcf")
                {
                    if (arg is LSDecimal) return FormatCurrency((LSDecimal)arg, false);
                    else if (arg is decimal) return FormatCurrency(new LSDecimal((decimal)arg), false);
                }
            }

            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, formatProvider);
            else return arg.ToString();
        }

        /// <summary>
        /// Formats
        /// </summary>
        /// <param name="value">the LSDecimal value to format as a currency</param>
        /// <param name="convert">If true, the value is converted from the 
        /// base currency using the configured exhange rate.  If false, the value
        /// is not converted before formatting.</param>
        /// <returns>The string representation of the value of arg, formatted as 
        /// specified by format and formatProvider.</returns>
        public string FormatCurrency(LSDecimal value, bool convert)
        {
            if (convert) value = (value * this.ExchangeRate);
            string formattedCurrency = value.ToString("c", this.NumberFormat);
            switch (this.ISOCodePattern)
            {
                case 1: return formattedCurrency + " " + this.ISOCode;
                case 2: return this.ISOCode + " " + formattedCurrency;
                case 3: return formattedCurrency + this.ISOCode;
                case 4: return this.ISOCode + formattedCurrency;
                default: return formattedCurrency;
            }
        }

        #endregion

        /// <summary>
        /// Converts an amount from the base currency into the currency specified by 
        /// this instance.
        /// </summary>
        /// <param name="value">The amount in the base currency to convert.</param>
        /// <returns>The converted amount.</returns>
        public LSDecimal ConvertFromBase(LSDecimal value)
        {
            Currency baseCurrency = Token.Instance.Store.BaseCurrency;
            if (baseCurrency.CurrencyId == this.CurrencyId) return value;
            LSDecimal convertedValue = Math.Round((decimal)(value * this.ExchangeRate), this.DecimalDigits);
            return convertedValue;
        }

        /// <summary>
        /// Converts an amount in the currency specified by this instance into the
        /// base currency.
        /// </summary>
        /// <param name="value">The amount in this currency to convert</param>
        /// <returns>The converted amount</returns>
        public LSDecimal ConvertToBase(LSDecimal value)
        {
            Currency baseCurrency = Token.Instance.Store.BaseCurrency;
            if (baseCurrency.CurrencyId == this.CurrencyId) return value;
            LSDecimal convertedValue = Math.Round((decimal)(value / this.ExchangeRate), baseCurrency.DecimalDigits);
            return convertedValue;
        }

        /// <summary>
        /// If an exchange rate provider is configured, this method will attempt to update
        /// the exchange rate based from the provider data.
        /// </summary>
        /// <param name="throwOnError">When false, if an error occurs during the update process
        /// the error is logged and ignored.  When true, the error is logged and then rethrown
        /// for the calling process</param>
        public void UpdateExchangeRate(bool throwOnError)
        {
            try
            {
                Currency baseCurrency = Token.Instance.Store.BaseCurrency;
                if (baseCurrency.CurrencyId == this.CurrencyId)
                {
                    this.ExchangeRate = 1;
                }
                else
                {
                    //GET THE FOREX PROVIDER
                    IForexProvider provider = ForexProviderDataSource.GetCurrentProvider();
                    if (provider != null)
                    {
                        this.ExchangeRate = provider.GetExchangeRate(baseCurrency.ISOCode, this.ISOCode);
                    }
                    else
                    {
                        Logger.Warn("Exchange rate update for " + this.Name + " failed because the rate provider could not be loaded.");
                    }
                }
                this.Save();
            }
            catch
            {
                if (throwOnError) throw;
            }
        }

        /// <summary>
        /// Saves this Currency object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public SaveResult Save()
        {
            if (this.CurrencyId != 0)
            {
                Decimal oldRate = CurrencyDataSource.GetExchangeRate(this.CurrencyId);
                if (this.ExchangeRate != oldRate)
                    this.LastUpdate = LocaleHelper.LocalNow;
            }
            else this.LastUpdate = LocaleHelper.LocalNow;
            return this.BaseSave();
        }

    }
}
