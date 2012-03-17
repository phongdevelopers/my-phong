using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Helper class for various string manipulation functions 
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Generates a random string of given length
        /// </summary>
        /// <param name="size">the length of the generated string</param>
        /// <returns>A random string of given length</returns>
        public static string RandomString(int size)
        {
            const string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            const int charSetLength = 62;
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                sb.Append(charSet.Substring(random.Next(0, charSetLength), 1));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates a random number with given number of digits
        /// </summary>
        /// <param name="size">Number of digits in the random number</param>
        /// <returns>A random number with given number of digits</returns>
        public static string RandomNumber(int size)
        {
            const string charSet = "1234567890";
            const int charSetLength = 10;
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                sb.Append(charSet.Substring(random.Next(0, charSetLength), 1));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Replaces old string with new string in a given text
        /// </summary>
        /// <param name="strText">The input text to be processed</param>
        /// <param name="strFind">The string to be replaced</param>
        /// <param name="strReplace">The string to replace with</param>
        /// <returns>The new text string that has old string replaced with new string</returns>
        public static String Replace(String strText, String strFind, String  strReplace)
        {
            int iPos = strText.IndexOf(strFind);
            String strReturn = "";
            while (iPos != -1)
            {
                strReturn += strText.Substring(0, iPos) + strReplace;
                strText = strText.Substring(iPos + strFind.Length);
                iPos = strText.IndexOf(strFind);
            }
            if (strText.Length > 0)
                strReturn += strText;
            return strReturn;
        }

        /// <summary>
        /// A fast, case-insensitive capable replacement for string.Replace
        /// </summary>
        /// <param name="input">The string to modify</param>
        /// <param name="oldValue">The old value to search for</param>
        /// <param name="newValue">The replacement value</param>
        /// <param name="comparisonType">The type of string comparision to use for matching</param>
        /// <returns>The modified string</returns>
        public static string Replace(string input, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (input == null) { return string.Empty; }
            if (String.IsNullOrEmpty(oldValue)) { return input; }
            int lenOldValue = oldValue.Length;
            int idxOldValue = -1;
            int idxLast = 0;
            StringBuilder result = new StringBuilder();
            while (true)
            {
                idxOldValue = input.IndexOf(oldValue, idxOldValue + 1, comparisonType);
                if (idxOldValue < 0)
                {
                    result.Append(input, idxLast, input.Length - idxLast);
                    break;
                }
                result.Append(input, idxLast, idxOldValue - idxLast);
                result.Append(newValue);
                idxLast = idxOldValue + lenOldValue;
            }
            return result.ToString();
        }


        /// <summary>
        /// Limits an input string to the specified length.
        /// </summary>
        /// <param name="str">The string to keep within the specified limit.</param>
        /// <param name="maxLength">The maximum length of the string.</param>
        /// <returns>If the input string is less than or equal to the maxLength, the original string is returned.  
        /// If the input string is greater than maxLength, the string is truncated to the correct length.</returns>
        public static String Truncate(String str, int maxLength)
        {
            if (maxLength < 1) throw new ArgumentException("maxLength must be greater than 0");
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (str.Length <= maxLength) return str;
            return str.Substring(0, maxLength);
        }

        
        /// <summary>
        /// Get string from the end (right)
        /// </summary>
        /// <param name="strParam">The input string</param>
        /// <param name="iLen">number of characters to extract from right</param>
        /// <returns>The extracted string from right</returns>
        public static String Right(String strParam, int iLen)
        {
            if (iLen > 0)
                return strParam.Substring(strParam.Length - iLen, iLen);
            else
                return strParam;
        }

        /// <summary>
        /// Converts camel cased text to spaced text. e.g; converts 
        /// "HelloThisIsCamelCaseString" to  "Hello This Is Camel Case String"
        /// </summary>
        /// <param name="camelCasedName">input camel-cased string</param>
        /// <returns></returns>
        public static string SpaceName(string camelCasedName)
        {
            return Regex.Replace(camelCasedName, "([A-Z])", " $1").Trim();
        }

        /// <summary>
        /// allows only the specified characters to be returned
        /// </summary>
        /// <param name="text">input string to filter</param>
        /// <param name="allowedChars">characters to allow</param>
        /// <returns>String that contains only the specified characters</returns>
        public static string ApplyFilter(string text, char[] allowedChars)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            StringBuilder result = new StringBuilder();
            foreach (char chr in text)
            {
                if (Array.Exists<char>(allowedChars, new Predicate<char>(delegate(char ch) { return chr == ch; })))
                {
                    result.Append(chr);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Capatilizes the first character of the words and lowers the remaining characters in the given input string.
        /// </summary>
        /// <param name="phrase">The input string to process</param>
        /// <returns>The new string with initial character capatilized for words and subsequent characters lower-cased</returns>
        public static string InitialCase(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;            
            StringBuilder sb = new StringBuilder();
            string[] words = phrase.Split(" ".ToCharArray());
            foreach (string word in words)
            {
                if (string.IsNullOrEmpty(word)) continue;
                if (sb.Length > 0) sb.Append(" ");
                sb.Append(word.Substring(0, 1).ToUpperInvariant() + word.Substring(1).ToLowerInvariant());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a string input into a SQL search pattern.
        /// </summary>
        /// <param name="searchPattern">The pattern to search for.</param>
        /// <returns>A string suitable for use with the SQL LIKE operator.</returns>
        public static string FixSearchPattern(string searchPattern)
        {
            return StringHelper.FixSearchPattern(searchPattern, true);
        }

        /// <summary>
        /// Converts a string input into a SQL search pattern.
        /// </summary>
        /// <param name="searchPattern">The pattern to search for.</param>
        /// <param name="substring">Only applicable if search pattern does not contain wildcards.  If true the method generates a substring match pattern; if false it generats a "starts with" match pattern.</param>
        /// <returns>A string suitable for use with the SQL LIKE operator.</returns>
        public static string FixSearchPattern(string searchPattern, bool substring)
        {
            if (string.IsNullOrEmpty(searchPattern)) return "%";
            if (searchPattern.IndexOfAny("*?%_".ToCharArray()) < 0)
                return (substring ? "%" : "") + searchPattern + "%";
            searchPattern = searchPattern.Replace("*", "%");
            searchPattern = searchPattern.Replace("?", "_");
            return searchPattern;
        }

        /// <summary>
        /// Calculates MD5 has for the given input string
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The calculated MD5 hash</returns>
        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Removes special characters for the input string. Non ASCII characters are considered special.
        /// </summary>
        /// <param name="text">The input string</param>
        /// <returns>Input string without special characters</returns>
        public static string RemoveSpecialChars(string text)
        {
            return RemoveSpecialChars(text, null);
        }

        /// <summary>
        /// Removes special characters and other undesired characters from the input string. Non ASCII characters are considered special.
        /// </summary>
        /// <param name="text">The input text</param>
        /// <param name="undesiredChars">Array of other undesired characters that are not special but should be removed as well</param>
        /// <returns>Input string without special and undesired characters</returns>
        public static string RemoveSpecialChars(string text, char[] undesiredChars)
        {
            if (string.IsNullOrEmpty(text)) return text;
            StringBuilder result = new StringBuilder();
            foreach (char chr in text)
            {
                if (chr < 127 && chr >= 32)
                {
                    if (undesiredChars != null)
                    {
                        if (!Array.Exists<char>(undesiredChars, new Predicate<char>(delegate(char ch) { return chr == ch; })))
                        {
                            result.Append(chr);
                        }
                    }
                    else
                    {
                        result.Append(chr);
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Removes the given characters from the input string
        /// </summary>
        /// <param name="text">The input string</param>
        /// <param name="undesiredChars">Array of characters to be removed from the string</param>
        /// <returns>Input string without the undesired characters</returns>
        public static string RemoveChars(string text, char[] undesiredChars)
        {
            if (string.IsNullOrEmpty(text)) return text;
            StringBuilder result = new StringBuilder();
            foreach (char chr in text)
            {
                Predicate<char> pchar = new Predicate<char>(delegate(char ch) { return ch == chr; });                 
                if (!System.Array.Exists<char>(undesiredChars, pchar))
                {
                    result.Append(chr);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Cleans up special characters from the given input string
        /// </summary>
        /// <param name="text">The input string</param>
        /// <returns>Input string without special characters</returns>
        public static string CleanupSpecialChars(string text)
        {
            return CleanupSpecialChars(text, null,' ');
        }

        /// <summary>
        /// Cleans up special characters and other undesired characters from the given input string
        /// </summary>
        /// <param name="text">The input string</param>
        /// <param name="undesiredChars">Array of other undesired characters that are not special but should be removed as well</param>
        /// <returns>Input string without special characters and undesired characters</returns>
        public static string CleanupSpecialChars(string text, char[] undesiredChars)
        {
            return CleanupSpecialChars(text, undesiredChars, ' ');
        }

        /// <summary>
        /// Cleans up special characters and other undesired characters from the given input string
        /// </summary>
        /// <param name="text">Input string</param>
        /// <param name="undesiredChars">Array of other undesired characters that are not special but should be removed as well</param>
        /// <param name="replacementChar">The character to user for replacement</param>
        /// <returns>Input string without special characters and undesired characters</returns>
        public static string CleanupSpecialChars(string text, char[] undesiredChars, char replacementChar)
        {
            if (string.IsNullOrEmpty(text)) return text;
            StringBuilder result = new StringBuilder();
            foreach (char chr in text)
            {
                if (chr < 127 && chr >= 32)
                {
                    if (undesiredChars != null)
                    {
                        if (!Array.Exists<char>(undesiredChars, new Predicate<char>(delegate(char ch) { return chr == ch; })))
                        {
                            result.Append(chr);
                        }
                        else
                        {
                            result.Append(replacementChar);
                        }
                    }
                    else
                    {
                        result.Append(chr);
                    }
                }
                else
                {
                    result.Append(replacementChar);
                }
            }
            return result.ToString();
        }


        /// <summary>
        /// Split a string on the given string separator
        /// </summary>
        /// <param name="value">String to be split</param>
        /// <param name="separator">String that delimits the substrings in this string.</param>
        /// <returns>An array of string items</returns>
        public static string[] Split(string value, string separator)
        {
            return Split(value, separator, StringSplitOptions.None);
        }

        /// <summary>
        /// Split a string on the given string separator
        /// </summary>
        /// <param name="value">String to be split</param>
        /// <param name="separator">String that delimits the substrings in this string.</param>
        /// <param name="options">Specify RemoveEmptyEntries to omit empty array elements from 
        /// the array returned, or None to include empty array elements in the array returned.</param>
        /// <returns></returns>
        public static string[] Split(string value, string separator, StringSplitOptions options)
        {
            string[] separatorArray = { separator };
            return value.Split(separatorArray, options);
        }

        /// <summary>
        /// Strips the HTML tags from the input string.
        /// </summary>
        /// <param name="value">String to remove HTML from</param>
        /// <returns>A string without any HTML tags</returns>
        public static string StripHtml(string value)
        {
            return StripHtml(value, false);
        }

        /// <summary>
        /// Strips the HTML tags from the input string.
        /// </summary>
        /// <param name="value">String to remove HTML from</param>
        /// <param name="encode">If true, the output is HTML encoded after tags are stripped.</param>
        /// <returns>A string without any HTML tags, optionally HTML encoded</returns>
        public static string StripHtml(string value, bool encode)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            string stripped = Regex.Replace(value, "<[^>]+>", string.Empty, RegexOptions.IgnoreCase); ;
            if (encode) return System.Web.HttpUtility.HtmlEncode(stripped);
            return stripped;
        }

        /// <summary>
        /// Convert an account number into a reference number.
        /// </summary>
        /// <param name="accountNumber">The account number to convert</param>
        /// <returns>The reference number in the form of x####.</returns>
        public static string MakeReferenceNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber)) return string.Empty;
            int length = accountNumber.Length;
            if (length < 5) return new string('x', length);
            return "x" + accountNumber.Substring(length - 4);
        }

        /// <summary>
        /// Returns a string that escapes single quotes in the input string
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>string with single quotes escaped</returns>
        public static string SafeSqlString(string str)
        {
            if (str == null)
            {
                return string.Empty;
            }

            return str.Replace("'", "''");
        }

    }

}
