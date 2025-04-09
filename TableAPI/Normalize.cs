﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableAPI
{
   
    /// <summary>
    /// 
    /// </summary>
    public enum NormalizationStrategy
    {
        /// <summary>
        /// Convert to lower case
        /// </summary>
        LowerCase,
        /// <summary>
        /// Convert to Upper case 
        /// </summary>
        UpperCase,
        /// <summary>
        /// Convert to Sentence Case
        /// </summary>
        SentenceCase,
        /// <summary>
        /// Replace with the most frequent one
        /// </summary>
        MostFrequentOne,
        /// <summary>
        /// Replace with the least frequent one
        /// </summary>
        LeastFrequentOne,
        /// <summary>
        /// Capture till we encounter the space character
        /// </summary>
        TerminateAtSpace,
        /// <summary>
        /// 
        /// </summary>
        TerminateAtNumber,
        /// <summary>
        /// 
        /// </summary>
        TerminateAtFirstNonAlpha,
        /// <summary>
        /// 
        /// </summary>
        TerminateAtFirstNonAlphaNumeric

        

    };
    /// <summary>
    /// 
    /// </summary>
    public static class Normalize
    {
        static char[] NUMBERS = new char[]{'0','1','2','3','4','5','6','7','8','9'};
        /// <summary>
        /// Default Special Chars list
        /// </summary>
        public static string SPECIAL_CHARS = "~!@#$%^&*()_+`-=/*-:\"'<>?,./";
        /// <summary>
        /// Default Alphabet list 
        /// </summary>
        public static string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private static string SentenceCase(this string text)
        {
            return text[0].ToString().ToUpper() + text.Substring(1).ToLower();
        }
        private static string TillSpace(this string text )
        {
            if (text.IndexOf(' ') == -1) return text;
            return text.Substring(0, text.IndexOf(' '));
        }

        private static string TillNumber(this string text)
        {
            if (text.IndexOf(' ') == -1) return text;
          
            return text.Substring(0, text.IndexOfAny(NUMBERS));

        }
        private static string MostFrequentOne(this List<string> values)
                     =>               values.ToLookup(t => t)
                                             .ToDictionary(t => t.Key, t => t.Count())
                                             .OrderByDescending(t => t.Value)
                                             .First()
                                             .Key;

        private static string LeastFrequentOne(this List<string> values)
                     => values.ToLookup(t => t)
                                             .ToDictionary(t => t.Key, t => t.Count())
                                             .OrderByDescending(t => t.Value)
                                             .Last()
                                             .Key;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnValues"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public static List<string> NormalizeAsPerStrategy(this List<string> columnValues, NormalizationStrategy strategy)
        {
            List<string> normalizedColumnValues = new List<string>();
            switch (strategy)
            {
                case NormalizationStrategy.LowerCase:
                    normalizedColumnValues = columnValues.Select(t => t.ToLower()).ToList();
                    break;
                case NormalizationStrategy.UpperCase:
                    normalizedColumnValues = columnValues.Select(t => t.ToUpper()).ToList();
                    break;
                case NormalizationStrategy.SentenceCase:
                    normalizedColumnValues = columnValues.Select(t => t.SentenceCase()).ToList();
                    break;
                case NormalizationStrategy.MostFrequentOne:
                    var mfo = columnValues.MostFrequentOne();
                    normalizedColumnValues = columnValues.Select(t => mfo).ToList();
                    break;
                case NormalizationStrategy.LeastFrequentOne:
                    var lfo = columnValues.LeastFrequentOne();
                    normalizedColumnValues = columnValues.Select(t => lfo).ToList();
                    break;
                case NormalizationStrategy.TerminateAtSpace:
                    normalizedColumnValues = columnValues.Select(t => t.TillSpace()).ToList();
                    break;
                case NormalizationStrategy.TerminateAtNumber:
                    normalizedColumnValues = columnValues.Select(t => t.TillNumber()).ToList();
                    break;
                case NormalizationStrategy.TerminateAtFirstNonAlpha:
                    
                    break;
                case NormalizationStrategy.TerminateAtFirstNonAlphaNumeric:
                    break;
                
            }
            return normalizedColumnValues;
        }
    }
}
