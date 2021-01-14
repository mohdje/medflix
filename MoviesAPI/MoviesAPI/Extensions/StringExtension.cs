using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Extensions
{
    internal static class StringExtension
    {
        public static bool ContainWords(this string str, string[] words)
        {
            foreach (var word in words)
            {
                if (!str.Contains(word))
                    return false;
            }
            return true;
        }
    }
}
