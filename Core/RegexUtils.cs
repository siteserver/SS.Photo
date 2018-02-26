using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SS.Photo.Core
{
    public class RegexUtils
    {

        /*
         * 通用：.*?
         * 所有链接：<a\s*.*?href=(?:"(?<url>[^"]*)"|'(?<url>[^']*)'|(?<url>\S+)).*?>
         * */

        public static RegexOptions Options = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

        public static List<string> GetOriginalScriptSrcs(string html)
        {
            const string regex = "script\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetContents("url", regex, html);
        }

        public static List<string> GetTagInnerContents(string tagName, string html)
        {
            string regex = $"<{tagName}\\s+[^><]*>\\s*(?<content>[\\s\\S]+?)\\s*</{tagName}>";
            return GetContents("content", regex, html);
        }

        public static List<string> GetTagContents(string tagName, string html)
        {
            var list = new List<string>();

            string regex = $@"<({tagName})[^>]*>(.*?)</\1>|<{tagName}[^><]*/>";

            var matches = Regex.Matches(html, regex, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    list.Add(match.Result("$0"));
                }
            }

            return list;
        }

        public static string GetTagName(string html)
        {
            var match = Regex.Match(html, "<([^>\\s]+)[\\s\\SS]*>", RegexOptions.IgnoreCase);
            return match.Success ? match.Result("$1") : string.Empty;
        }

        public static string GetInnerContent(string tagName, string html)
        {
            string regex = $"<{tagName}[^><]*>(?<content>[\\s\\S]+?)</{tagName}>";
            return GetContent("content", regex, html);
        }

        public static string GetAttributeContent(string attributeName, string html)
        {
            string regex =
                $"<[^><]+\\s*{attributeName}\\s*=\\s*(?:\"(?<value>[^\"]*)\"|'(?<value>[^']*)'|(?<value>[^>\\s]*)).*?>";
            return GetContent("value", regex, html);
        }

        public static string GetContent(string groupName, string regex, string html)
        {
            var content = string.Empty;
            if (string.IsNullOrEmpty(regex)) return content;
            if (regex.IndexOf("<" + groupName + ">", StringComparison.Ordinal) == -1)
            {
                return regex;
            }

            var reg = new Regex(regex, Options);
            var match = reg.Match(html);
            if (match.Success)
            {
                content = match.Groups[groupName].Value;
            }

            return content;
        }

        public static string Replace(string regex, string input, string replacement)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var reg = new Regex(regex, Options);
            return reg.Replace(input, replacement);
        }

        public static string Replace(string regex, string input, string replacement, int count)
        {
            if (count == 0)
            {
                return Replace(regex, input, replacement);
            }
            if (string.IsNullOrEmpty(input)) return input;
            var reg = new Regex(regex, Options);
            return reg.Replace(input, replacement, count);
        }

        public static bool IsMatch(string regex, string input)
        {
            var reg = new Regex(regex, Options);
            return reg.IsMatch(input);
        }

        public static List<string> GetContents(string groupName, string regex, string html)
        {
            if (string.IsNullOrEmpty(regex)) return new List<string>();

            var list = new List<string>();
            var reg = new Regex(regex, Options);

            for (var match = reg.Match(html); match.Success; match = match.NextMatch())
            {
                var theValue = match.Groups[groupName].Value;
                if (!list.Contains(theValue))
                {
                    list.Add(theValue);
                }
            }
            return list;
        }

        public static string RemoveScripts(string html)
        {
            const string regex = "<script[^><]*>.*?<\\/script>";
            return Replace(regex, html, string.Empty);
        }

        public static string RemoveImages(string html)
        {
            const string regex = "<img[^><]*>";
            return Replace(regex, html, string.Empty);
        }
    }
}
