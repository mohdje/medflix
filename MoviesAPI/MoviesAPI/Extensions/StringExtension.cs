using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Extensions
{
    internal static class StringExtension
    {
        public static bool ContainsWords(this string str, string[] words)
        {
            foreach (var word in words)
            {
                if (!str.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        public static string GetMovieQuality(this string movieLinkTitle)
        {
            var qualities = new string[] { "720p", "1080p", "DVDRIP", "WEBRIP" };

            foreach (var quality in qualities)
            {
                if (movieLinkTitle.Contains(quality))
                    return quality;
            }

            return string.Empty;
        }

        public static string RemoveSpecialCharacters(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text.Replace(":", "").Replace("-", "").Replace("'", "");
            }

            return string.Empty;
        }
    }
}
