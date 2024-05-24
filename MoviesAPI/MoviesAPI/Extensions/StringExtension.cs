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

        public static string GetVideoQuality(this string movieLinkTitle)
        {
            var qualities = new string[] { "480p", "720p", "1080p", "2160p", "DVDRIP", "WEBRIP", "HDTV" };

            foreach (var quality in qualities)
            {
                if (movieLinkTitle.Contains(quality))
                    return quality;
            }

            return "Unknown";
        }

        public static string RemoveSpecialCharacters(this string text, bool removeSpaces = false, bool toLower = false)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var result = text.Replace(":", "")
                            .Replace(",", "")
                            .Replace("-", "")
                            .Replace("/", "")
                            .Replace("'", "")
                            .Replace("é", "e")
                            .Replace("è", "e")
                            .Replace("ê", "e")
                            .Replace("à", "a")
                            .Replace("î", "i")
                            .Replace("ï", "i")
                            .Replace("ù", "u");

                if (removeSpaces)
                    result = result.Replace(" ", "");

                if (toLower)
                    result = result.ToLower();

                return result;
            }

            return string.Empty;
        }

        public static bool CustomStartsWith(this string text, string value)
        {
            return text.RemoveSpecialCharacters(removeSpaces: true, toLower: true).StartsWith(value.RemoveSpecialCharacters(removeSpaces: true, toLower: true));
        }

        public static bool CustomCompare(this string text, string value)
        {
            return text.RemoveSpecialCharacters(removeSpaces: true, toLower: true) == value.RemoveSpecialCharacters(removeSpaces: true, toLower: true);
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

        public static int? toInt(this string value)
        {
            int number;
            return int.TryParse(value, out number) ? number : null;
        }
    }
}
