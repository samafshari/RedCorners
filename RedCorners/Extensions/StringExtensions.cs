using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RedCorners
{
    public static class StringExtensions
    {
        public static string Head(this string s, int take = 20)
        {
            if (s == null) return "";
            s = s.Trim().Replace("\r", " ").Replace("\n", " ");
            if (s.Length <= take) return s;
            return s.Substring(0, take) + "...";
        }

        public static HashSet<string> Hashtags(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return new HashSet<string>();
            var pattern = @"#(\w*[0-9a-zA-Z]+\w*[0-9a-zA-Z])";
            var results = new HashSet<string>();
            foreach (Match m in Regex.Matches(s, pattern))
                results.Add(m.Value.ToLowerInvariant());
            return results;
        }

        public static string RemovePrefix(this string s, string prefix)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (!s.StartsWith(prefix)) return s;
            var prefixLength = prefix.Length;
            return s.Substring(prefixLength, s.Length - prefixLength);
        }

        public static DateTime DateFromEpochMs(this long epochMs)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(epochMs);
        }

        public static long ToEpochMs(this DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static bool IsNW(this string s) => string.IsNullOrWhiteSpace(s);
        public static bool HasValue(this string s) => !string.IsNullOrWhiteSpace(s);

        public static string CapitalizeFirstLetter(this string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }
    }
}
