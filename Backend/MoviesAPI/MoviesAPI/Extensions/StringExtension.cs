using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoviesAPI.Extensions
{
    public static class StringExtension
    {
        public static string GetVideoQuality(this string mediaTitle)
        {
            var qualities = new string[] { "480p", "720p", "1080p", "2160p", "DVDRIP", "WEBRIP", "HDTV" };

            foreach (var quality in qualities)
            {
                if (mediaTitle.Contains(quality, StringComparison.OrdinalIgnoreCase))
                    return quality;
            }

            return "Unknown";
        }

        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if(Char.IsPunctuation(c))
                    sb.Append(' ');
                else if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            var result = sb.ToString().Normalize(NormalizationForm.FormC);
            return Regex.Replace(result, " {2,}", " ");
        }

        public static bool StartsWithIgnoreDiactrics(this string text, string value)
        {
            if (text == null && value == null)
                return true;
            if (text == null || value == null)
                return false;

            string cleanText = RemoveDiacritics(text);
            string cleanValue = RemoveDiacritics(value);

            return cleanText.StartsWith(cleanValue, StringComparison.OrdinalIgnoreCase);
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
