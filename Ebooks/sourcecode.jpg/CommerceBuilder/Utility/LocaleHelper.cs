using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CommerceBuilder.Shipping;
using CommerceBuilder.Common;
using System.Globalization;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class to support localization 
    /// </summary>
    public static class LocaleHelper
    {
        /// <summary>
        /// Returns the local time adjusted for the correct timezone offset as specified in the store settings.
        /// </summary>
        public static DateTime LocalNow
        {
            get
            {
                return ToLocalTime(DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Converts a utc time to local time, using the offset configured in the store settings.
        /// </summary>
        /// <param name="utc">The time in utc.  The datetime value is assumed to be in utc, regardless of the value of the "Kind" property.</param>
        /// <returns>A datetime instance with the time adjusted to local time.</returns>
        /// <remarks>If local is DateTime.MinValue, or if local.Kind is DateTimeKind.Local, local is returned unaltered.</remarks>
        public static DateTime ToLocalTime(DateTime utc)
        {
            if ((utc == DateTime.MinValue) || (utc == DateTime.MaxValue) || (utc.Kind == DateTimeKind.Local)) return utc;
            if (Token.Instance.Store != null) return ToLocalTime(utc, Token.Instance.Store.TimeZoneOffset);
            return ToLocalTime(utc, 0);
        }

        /// <summary>
        /// Converts a utc time to local time, based on the provided offset.
        /// </summary>
        /// <param name="utc">The time in utc.  The datetime value is assumed to be in utc, regardless of the value of the "Kind" property.</param>
        /// <param name="offset">The offset to use in calculating the local date.</param>
        /// <returns>A datetime instance with the time adjusted to local time for the offset specified.</returns>
        /// <remarks>If local is DateTime.MinValue, or if local.Kind is DateTimeKind.Local, local is returned unaltered.</remarks>
        public static DateTime ToLocalTime(DateTime utc, double offset)
        {
            if ((utc == DateTime.MinValue) || (utc == DateTime.MaxValue) || (utc.Kind == DateTimeKind.Local)) return utc;
            return DateTime.SpecifyKind(utc.AddHours(offset), DateTimeKind.Local);
        }

        /// <summary>
        /// Converts a local time to utc, using the offset configured in the store settings.
        /// </summary>
        /// <param name="local">The local time to convert.</param>
        /// <returns>A datetime instance with the time adjusted to utc.</returns>
        /// <remarks>If local is DateTime.MinValue, or if local.Kind is DateTimeKind.Utc, local is returned unaltered.</remarks>
        public static DateTime FromLocalTime(DateTime local)
        {
            if (Token.Instance.Store != null) return FromLocalTime(local, Token.Instance.Store.TimeZoneOffset);
            return FromLocalTime(local, 0);
        }
    
        /// <summary>
        /// Converts a local time to utc, based on the provided offset.
        /// </summary>
        /// <param name="local">The local time to convert.</param>
        /// <param name="offset">The offset to use in calculating the utc date.</param>
        /// <returns>A datetime instance with the time adjusted to utc for the offset specified.</returns>
        /// <remarks>If local is DateTime.MinValue, or if local.Kind is DateTimeKind.Utc, local is returned unaltered.</remarks>
        public static DateTime FromLocalTime(DateTime local, double offset)
        {
            if ((local == DateTime.MinValue) || (local == DateTime.MaxValue) || (local.Kind == DateTimeKind.Utc)) return local;
            return DateTime.SpecifyKind(local.AddHours(-1 * offset), DateTimeKind.Utc);
        }

        /// <summary>
        /// Convert a measurement from source unit to target unit
        /// </summary>
        /// <param name="sourceUnit">The source unit</param>
        /// <param name="sourceValue">The value to convert</param>
        /// <param name="targetUnit">The target unit to convert to</param>
        /// <returns>A LSDecimal with the value converted into the target unit</returns>
        public static LSDecimal ConvertMeasurement(MeasurementUnit sourceUnit, LSDecimal sourceValue, MeasurementUnit targetUnit)
        {
            if (sourceUnit == targetUnit) return sourceValue;
            if (sourceUnit == MeasurementUnit.Centimeters)
            {
                //WE MUST BE CONVERTING TO INCHES
                return Math.Round(((Decimal)sourceValue * 0.3937M), 2);
            }
            //WE MUST BE CONVERTING TO CENTIMETERS
            return Math.Round(((Decimal)sourceValue * 2.54M), 2);
        }

        /// <summary>
        /// Converts a weight from source unit to target unit
        /// </summary>
        /// <param name="sourceUnit">The source unit</param>
        /// <param name="sourceValue">The value to convert</param>
        /// <param name="targetUnit">The target unit to convert to</param>
        /// <returns>A LSDecimal with the value converted into the target unit</returns>
        public static LSDecimal ConvertWeight(WeightUnit sourceUnit, LSDecimal sourceValue, WeightUnit targetUnit)
        {
            if (sourceUnit == targetUnit) return sourceValue;
            Decimal factor = 1;
            switch (sourceUnit)
            {
                case WeightUnit.Grams:
                    switch (targetUnit)
                    {
                        case WeightUnit.Kilograms:
                            factor = 0.001M;
                            break;
                        case WeightUnit.Ounces:
                            factor = 0.0352739596M;
                            break;
                        case WeightUnit.Pounds:
                            factor = 0.0022046225M;
                            break;
                    }
                    break;
                case WeightUnit.Kilograms:
                    switch (targetUnit)
                    {
                        case WeightUnit.Grams:
                            factor = 1000M;
                            break;
                        case WeightUnit.Ounces:
                            factor = 35.2739596M;
                            break;
                        case WeightUnit.Pounds:
                            factor = 2.2046225M;
                            break;
                    }
                    break;
                case WeightUnit.Ounces:
                    switch (targetUnit)
                    {
                        case WeightUnit.Grams:
                            factor = 28.349523M;
                            break;
                        case WeightUnit.Kilograms:
                            factor = 0.028349523M;
                            break;
                        case WeightUnit.Pounds:
                            factor = 0.0625M;
                            break;
                    }
                    break;
                case WeightUnit.Pounds:
                    switch (targetUnit)
                    {
                        case WeightUnit.Grams:
                            factor = 453.5924M;
                            break;
                        case WeightUnit.Kilograms:
                            factor = 0.4535924M;
                            break;
                        case WeightUnit.Ounces:
                            factor = 16M;
                            break;
                    }
                    break;
            }
			return Math.Round(((Decimal)sourceValue * factor), 2);
        }

        /// <summary>
        /// Formats the currency
        /// </summary>
        /// <param name="currency">The currency value to format</param>
        /// <returns>Formatted currency</returns>
        public static string FormatCurrency(object currency)
        {
            string pattern = "$#,##0.00;-$#,##0.00;Zero";
            if (currency is LSDecimal) return ((LSDecimal)currency).ToString(pattern);
            if (currency is double) return((double)currency).ToString(pattern);
            throw new ArgumentException("Input must be LSDecimal or double.", "currency");
        }
    }
}
