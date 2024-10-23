using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Extensions
{
    public static class TimeSpanExtensions
    { 

        public static string ToTimeFormat(this TimeSpan timeSpan)
        {
            var result = new StringBuilder();

            if (timeSpan.Hours > 0)
                result.Append($"{timeSpan.Hours:D1}h");

            result.Append($"{timeSpan.Minutes:D2}min");

            return result.ToString();
        }
    }
}
