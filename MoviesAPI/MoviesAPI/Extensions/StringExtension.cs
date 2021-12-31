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
    }
}
