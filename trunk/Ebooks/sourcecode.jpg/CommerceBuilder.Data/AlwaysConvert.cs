namespace CommerceBuilder.Data
{
    using System;

    /// <summary>
    /// Utility class for type conversion
    /// </summary>
    internal static class AlwaysConvert
    {
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
    }
}