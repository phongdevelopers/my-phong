using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class for type conversion
    /// </summary>
    public static class AlwaysConvert
    {
        /// <summary>
        /// Parses the given input value and converts it to a short. Default return value is 0.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>The converted short value</returns>
        public static short ToInt16(object parseValue) { return ToInt16(parseValue, 0); }

        /// <summary>
        /// Parses the given input value and converts it to a short.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted short value</returns>
        public static short ToInt16(object parseValue, short defaultValue)
        {
            short returnValue;
            try
            {
                returnValue = short.Parse(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to a byte. Default return value is 0.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>The converted byte value</returns>
        public static byte ToByte(object parseValue) { return AlwaysConvert.ToByte(parseValue, 0); }
        
        /// <summary>
        /// Parses the given input value and converts it to a byte.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted byte value</returns>
        public static byte ToByte(object parseValue, byte defaultValue)
        {
            byte returnValue;
            try
            {
                returnValue = byte.Parse(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to a Guid. Default return value is Guid.Empty.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>The converted Guid</returns>
        public static Guid ToGuid(object parseValue) { return ToGuid(parseValue, Guid.Empty); }

        /// <summary>
        /// Parses the given input value and converts it to a Guid.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted Guid</returns>
        public static Guid ToGuid(object parseValue, Guid defaultValue)
        {
            Guid returnValue;
            try
            {
                returnValue = new Guid(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to a Decimal. Default return value is 0.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>The converted Decimal value</returns>
        public static Decimal ToDecimal(object parseValue) { return ToDecimal(parseValue, 0); }

        /// <summary>
        /// Parses the given input value and converts it to a Decimal.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted Decimal value</returns>
        public static Decimal ToDecimal(object parseValue, Decimal defaultValue)
        {
            Decimal returnValue;
            try
            {
                returnValue = Decimal.Parse(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to a long. Default return value is 0.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>The converted long value</returns>
        public static long ToLong(object parseValue) { return ToLong(parseValue, 0); }

        /// <summary>
        /// Parses the given input value and converts it to a long.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted long value</returns>
        public static long ToLong(object parseValue, long defaultValue)
        {
            long returnValue;
            try
            {
                returnValue = long.Parse(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to a double. Default return value is 0.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>The converted double value</returns>
        public static double ToDouble(object parseValue) { return ToDouble(parseValue, 0); }

        /// <summary>
        /// Parses the given input value and converts it to a double.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted double value</returns>
        public static double ToDouble(object parseValue, double defaultValue)
        {
            double returnValue;
            try
            {
                returnValue = double.Parse(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to an int. Default return value is 0.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>The converted int value</returns>
        public static int ToInt(object parseValue) { return ToInt(parseValue, 0); }

        /// <summary>
        /// Parses the given input value and converts it to an int.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted int value</returns>
        public static int ToInt(object parseValue, int defaultValue)
        {
            int returnValue;
            try
            {
                returnValue = int.Parse(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to a bool.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted boolean value</returns>
        public static bool ToBool(object parseValue, bool defaultValue)
        {
            bool returnValue;
            try
            {
                returnValue = Boolean.Parse(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to an enum of given type.
        /// </summary>
        /// <param name="enumType">The enumeration type to convert to</param>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted enumeration value</returns>
        public static object ToEnum(Type enumType, string parseValue, object defaultValue){
            return ToEnum(enumType,parseValue,defaultValue,false);
        }

        /// <summary>
        /// Parses the given input value and converts it to an enum of given type.
        /// </summary>
        /// <param name="enumType">The enumeration type to convert to</param>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <param name="ignoreCase">If <b>true</b> case is ignored when parsing.</param>
        /// <returns>The converted enumeration value</returns>
        public static object ToEnum(Type enumType, string parseValue, object defaultValue, bool ignoreCase)
        {
            try
            {
                return Enum.Parse(enumType, parseValue, ignoreCase);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Parses the given input value and converts it to a string. Default return value is string.Empty.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>The converted string value</returns>
        public static string ToString(object parseValue) { return ToString(parseValue, string.Empty); }

        /// <summary>
        /// Parses the given input value and converts it to a string.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted string value</returns>
        public static string ToString(object parseValue, string defaultValue)
        {
            if (parseValue == null || parseValue == DBNull.Value) return defaultValue;
            string returnValue;
            try
            {
                returnValue = (string)parseValue;
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to a DateTime value.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <param name="defaultValue">The default value to return in case of failure to convert the given input</param>
        /// <returns>The converted DateTime value</returns>
        public static DateTime ToDateTime(object parseValue, DateTime defaultValue)
        {
            DateTime returnValue;
            try
            {
                returnValue = DateTime.Parse(parseValue.ToString());
            }
            catch
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the given input value and converts it to an array of <b>int</b>s assuming 
        /// the input is a coma separated list of integers.
        /// </summary>
        /// <param name="parseValue">The input value to parse</param>
        /// <returns>An array of integers</returns>
        public static int[] ToIntArray(string parseValue)
        {
            if (string.IsNullOrEmpty(parseValue)) return null;
            List<int> returnValues = new List<int>();
            string[] tokens = parseValue.Split(",".ToCharArray());
            foreach (string token in tokens)
            {
                string trimmedToken = token.Trim();
                if (trimmedToken.Length > 0)
                {
                    int checkValue = AlwaysConvert.ToInt(trimmedToken);
                    if (checkValue.ToString() == trimmedToken)
                    {
                        returnValues.Add(checkValue);
                    }
                }
            }
            if (returnValues.Count == 0) return null;
            return returnValues.ToArray();
        }

        /// <summary>
        /// Converts an int array into a delimited string list
        /// </summary>
        /// <param name="delimiter">The delimiter character for the list, e.g. comma.</param>
        /// <param name="intArray">The array of int to convert to list</param>
        /// <returns>A delimited string containing the values of the array</returns>
        public static string ToList(string delimiter, IEnumerable<int> intArray)
        {
            if (intArray == null) return string.Empty;
            List<string> result = new List<string>();
            foreach (int item in intArray)
            {
                result.Add(item.ToString());
            }
            if (result.Count == 0) return string.Empty;
            return string.Join(delimiter, result.ToArray());
        }

        /// <summary>
        /// Makes sure an IP address is in valid IPV4 decimal dot notation
        /// </summary>
        /// <param name="remoteIP">The IP to verify</param>
        /// <returns>An decimal dot #.#.#.# formatted address.</returns>
        public static string ToIPV4(string remoteIP)
        {
            if (!Regex.IsMatch(remoteIP, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
            {
                if (remoteIP == "::1") return "127.0.0.1";
                return "0.0.0.0";
            }
            return remoteIP;
        }

    }
}
