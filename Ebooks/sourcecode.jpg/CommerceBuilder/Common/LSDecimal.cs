using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Common
{
    /// <summary>
    /// LSDecimal type definition
    /// </summary>
    [Serializable]
    public struct LSDecimal : IFormattable, IComparable, IConvertible, IComparable<LSDecimal>, IEquatable<LSDecimal>
    {
        private Decimal InnerDecimal;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The decimal value</param>
        public LSDecimal(Decimal value)
        {
            InnerDecimal = value;
        }

        /// <summary>
        /// Formats the value using the default pattern
        /// </summary>
        /// <returns>String representation of the LSDecimal value</returns>
        public override string ToString()
        {
            return this.InnerDecimal.ToString();
        }

        /// <summary>
        /// Formats the value according to the specified pattern
        /// </summary>
        /// <param name="format">The pattern to use for formatting the value, use 
        /// lc to format in store base currency, ulc to format is user default currency.</param>
        /// <returns>String representation of the LSDecimal value</returns>
        public string ToString(string format)
        {
            if (string.IsNullOrEmpty(format)) return this.ToString();
            string tempFormat = format.Trim().ToLowerInvariant();
            if (tempFormat.StartsWith("ulc"))
            {
                //standardize the format code
                tempFormat = tempFormat.Replace("ulc", "lc");
                //substitute the user currency
                return string.Format(Token.Instance.User.UserCurrency, "{0:" + tempFormat + "}", this.InnerDecimal);
            }
            else if (tempFormat.StartsWith("lc"))
            {
                return string.Format(Token.Instance.Store.BaseCurrency, "{0:" + tempFormat + "}", this.InnerDecimal);
            }
            return this.InnerDecimal.ToString(format);
        }

        /// <summary>
        /// Formats the LSDecimal value using the specified format provider.
        /// </summary>
        /// <param name="provider">Format provider</param>
        /// <returns>String representation of the LSDecimal value</returns>
        public string ToString(IFormatProvider provider)
        {
            if (provider == null) return this.ToString();
            if (typeof(Currency).Equals(provider.GetType()))
                return string.Format(provider, "{0:lc}", this.InnerDecimal);
            return this.InnerDecimal.ToString(provider);
        }

        /// <summary>
        /// Formats the LSDecimal value using the specified format provider.
        /// </summary>
        /// <param name="format">The pattern to use for formatting the value.</param>
        /// <param name="provider">Format provider</param>
        /// <returns>String representation of the LSDecimal value</returns>
        public string ToString(string format, IFormatProvider provider)
        {
            if (provider == null) return this.ToString(format);
            if (typeof(Currency).Equals(provider.GetType()))
                return string.Format(provider, "{0:" + format + "}", this.InnerDecimal);
            return (this.InnerDecimal.ToString(format, provider));
        }

        /// <summary>
        /// Converts the value to LSDecimal equivalent
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The LSDecimal equivalent of the value</returns>
        public static implicit operator LSDecimal(Decimal value)
        {
            return new LSDecimal(value);
        }

        /// <summary>
        /// Converts the value to decimal equivalent
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The decimal equivalent of the value</returns>
        public static explicit operator Decimal(LSDecimal value)
        {
            return value.InnerDecimal;
        }

        /// <summary>
        /// Converts the value to Double equivalent
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The Double equivalent of the value</returns>
        public static explicit operator Double(LSDecimal value)
        {
            return (Double)value.InnerDecimal;
        }

        /// <summary>
        /// Converts the value to Int32 equivalent
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The Int32 equivalent of the value</returns>
        public static explicit operator Int32(LSDecimal value)
        {
            return (Int32)value.InnerDecimal;
        }

        /// <summary>
        /// Compares this LSDecimal value with another.
        /// </summary>
        /// <param name="value">The value to compare</param>
        /// <returns>A signed number indicating the relative values of this 
        /// instance and value</returns>
        public int CompareTo(LSDecimal value)
        {
            return Decimal.Compare((Decimal)this, (Decimal)value);
        }

        /// <summary>
        /// Compares this LSDecimal value with another.
        /// </summary>
        /// <param name="value">The value to compare</param>
        /// <returns>A signed number indicating the relative values of this 
        /// instance and value</returns>
        public int CompareTo(object value)
        {
            if (value == null) return 1;
            if (value is LSDecimal)
                return Decimal.Compare((Decimal)this, ((LSDecimal)value).InnerDecimal);
            else if ( value is Decimal)
                return Decimal.Compare((Decimal)this, (Decimal)value);

            throw new ArgumentException("Argument must be a Decimal");
        }

        /// <summary>
        /// Determines whether this value is equal to another
        /// </summary>
        /// <param name="value">The value to compare</param>
        /// <returns>True if the values are equal; false otherwise</returns>
        public override bool Equals(object value)
        {
            if ((value is LSDecimal) || (value is Decimal))
                return (Decimal.Compare((Decimal)this, (Decimal)value) == 0);
            return false;
        }

        /// <summary>
        /// Determines whether this value is equal to another
        /// </summary>
        /// <param name="other">The value to compare</param>
        /// <returns>True if the values are equal; false otherwise</returns>
        public bool Equals(LSDecimal other)
        {
            return (Decimal.Compare((Decimal)this, (Decimal)other) == 0);
        }

        /// <summary>
        /// Returns the hashcode for this instance
        /// </summary>
        /// <returns>The hashcode for this instance</returns>
        public override int GetHashCode()
        {
            return this.InnerDecimal.GetHashCode();
        }

        #region Operators

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static LSDecimal operator +(LSDecimal d)
        {
            return d;
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static LSDecimal operator -(LSDecimal d)
        {
            return (LSDecimal)Decimal.Negate(d.InnerDecimal);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static LSDecimal operator ++(LSDecimal d)
        {
            return (LSDecimal)Decimal.Add(d.InnerDecimal, 1M);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static LSDecimal operator --(LSDecimal d)
        {
            return (LSDecimal)Decimal.Subtract(d.InnerDecimal, 1M);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static LSDecimal operator +(LSDecimal d1, LSDecimal d2)
        {
            return (LSDecimal)Decimal.Add((Decimal)d1, (Decimal)d2);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static LSDecimal operator -(LSDecimal d1, LSDecimal d2)
        {
            return (LSDecimal)Decimal.Subtract((Decimal)d1, (Decimal)d2);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static LSDecimal operator *(LSDecimal d1, LSDecimal d2)
        {
            return (LSDecimal)Decimal.Multiply((Decimal)d1, (Decimal)d2);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static LSDecimal operator /(LSDecimal d1, LSDecimal d2)
        {
            return (LSDecimal)Decimal.Divide((Decimal)d1, (Decimal)d2);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static LSDecimal operator %(LSDecimal d1, LSDecimal d2)
        {
            return (LSDecimal)Decimal.Remainder((Decimal)d1, (Decimal)d2);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator ==(LSDecimal d1, LSDecimal d2)
        {
            return (Decimal.Compare((Decimal)d1, (Decimal)d2) == 0);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator !=(LSDecimal d1, LSDecimal d2)
        {
            return (Decimal.Compare((Decimal)d1, (Decimal)d2) != 0);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator <(LSDecimal d1, LSDecimal d2)
        {
            return (Decimal.Compare((Decimal)d1, (Decimal)d2) < 0);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator <=(LSDecimal d1, LSDecimal d2)
        {
            return (Decimal.Compare((Decimal)d1, (Decimal)d2) <= 0);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator >(LSDecimal d1, LSDecimal d2)
        {
            return (Decimal.Compare((Decimal)d1, (Decimal)d2) > 0);
        }

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator >=(LSDecimal d1, LSDecimal d2)
        {
            return (Decimal.Compare((Decimal)d1, (Decimal)d2) >= 0);
        }
        
        #endregion

        #region IConvertible Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            return this.InnerDecimal.GetTypeCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            return this.InnerDecimal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conversionType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.InnerDecimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.InnerDecimal);
        }

        #endregion
    }
}
