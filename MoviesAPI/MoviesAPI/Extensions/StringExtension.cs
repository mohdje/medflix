using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

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
                return text.Replace(":", "")
                            .Replace("-", "")
                            .Replace("'", "")
                            .Replace("é", "e")
                            .Replace("è", "e")
                            .Replace("ê", "e")
                            .Replace("à", "a")
                            .Replace("î", "i")
                            .Replace("ï", "i")
                            .Replace("ù", "u");
            }

            return string.Empty;
        }

        public static string HtmlUnescape(this string text)
        {
            return HttpUtility.HtmlDecode(text);
        }

        public static int GetYear(this string date)
        {
            if (string.IsNullOrEmpty(date))
                return 0;

            int year;
            int.TryParse(date.Split('-')[0], out year);

            return year;
        }
    }
}
