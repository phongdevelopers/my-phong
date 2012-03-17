using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Search
{
    /// <summary>
    /// This class contains functions to parse SQL Server STOP/NOISE words.
    /// </summary>
    public class StopWordsParser
    {
        private static string[] _StopWords;

        /// <summary>
        /// static initializer
        /// </summary>
        static StopWordsParser()
        {
            // LOOK FOR THE STOP WORDS FILE
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\stopwords.txt");
            if (File.Exists(filePath))
            {
                // READ THE LIST OF STOP WORDS FROM THE FILE
                _StopWords = File.ReadAllLines(filePath);
            }
            else
            {
                // THE STOP WORDS FILE IS MISSING, THERE IS NO STOP WORDS FILTER
                _StopWords = null;
            }
        }

        /// <summary>
        /// Filter the stop words from keywords
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>filtered string</returns>
        public string Filter(string input)
        {
            // DO NOT PROCESS NULL INPUT
            if (input == null) return string.Empty;
            // NO PROCESSING IF THERE ARE NO STOPWORDS DEFINED
            if (_StopWords == null || _StopWords.Length == 0) return input;
            // TRIM WHITESPACE FROM THE STRING
            input = input.Trim();
            // DO NOT PROCESS EMPTY INPUTS
            if (string.IsNullOrEmpty(input)) return input;

            // BUILD A LIST OF WORDS THAT ARE NOT STOPWORDS
            List<string> outputKeywords = new List<string>();
            
            // TOKENIZE THE INPUT PHRASE AND LOOP EACH WORD
            string[] keyWords = input.Split(' ');
            foreach (string keyWord in keyWords)
            {
                // MAKE SURE THE KEYWORD IS NOT EMPTY
                if (!string.IsNullOrEmpty(keyWord))
                {
                    // LOOP IN STOPWORDS LIST FOR THIS WORD
                    if (Array.IndexOf(_StopWords, keyWord.ToLowerInvariant()) == -1)
                    {
                        // THE KEYWORD DOES NOT APPEAR IN STOPWORDS LIST
                        outputKeywords.Add(keyWord);
                    }
                }
            }

            // RETURN THE FILTERED PHRASE
            return string.Join(" ", outputKeywords.ToArray());
        }
    }
}
