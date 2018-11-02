using System;
using System.IO;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SS.Photo.Core
{
    public static class Utils
    {
        public const string PluginId = "SS.Photo";

        public static string ToJsString(string value)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                retval = value.Replace("'", @"\'").Replace("\r", "\\r").Replace("\n", "\\n");
            }
            return retval;
        }

        public static int ToInt(string intStr)
        {
            return ToInt(intStr, 0);
        }

        private static int ToInt(string intStr, int defaultValue)
        {
            int i;
            if (!int.TryParse(intStr?.Trim().TrimStart('0'), out i))
            {
                i = defaultValue;
            }
            if (i < 0)
            {
                i = defaultValue;
            }
            return i;
        }

        public static bool ToBool(string boolStr)
        {
            bool boolean;
            if (!bool.TryParse(boolStr?.Trim(), out boolean))
            {
                boolean = false;
            }
            return boolean;
        }

        public static bool ToBool(string boolStr, bool defaultValue)
        {
            bool boolean;
            if (!bool.TryParse(boolStr?.Trim(), out boolean))
            {
                boolean = defaultValue;
            }
            return boolean;
        }

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            return string.Equals(a.Trim().ToLower(), b.Trim().ToLower());
        }

        public static bool StartsWithIgnoreCase(string text, string startString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(startString)) return false;
            return text.Trim().ToLower().StartsWith(startString.Trim().ToLower()) || string.Equals(text.Trim(), startString.Trim(), StringComparison.CurrentCultureIgnoreCase);
        }

        public static string ReplaceNewlineToBr(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            foreach (var t in inputString)
            {
                switch (t)
                {
                    case '\n':
                        retVal.Append("<br />");
                        break;
                    case '\r':
                        break;
                    default:
                        retVal.Append(t);
                        break;
                }
            }
            return retVal.ToString();
        }

        private static string StripTags(string inputString)
        {
            var retval = RegexUtils.Replace("<script[^>]*>.*?<\\/script>", inputString, string.Empty);
            retval = RegexUtils.Replace("<[\\/]?[^>]*>|<[\\S]+", retval, string.Empty);
            return retval;
        }

        private static string Replace(string replace, string input, string to)
        {
            var retval = RegexUtils.Replace(replace, input, to);
            if (string.IsNullOrEmpty(replace)) return retval;
            if (replace.StartsWith("/") && replace.EndsWith("/"))
            {
                retval = RegexUtils.Replace(replace.Trim('/'), input, to);
            }
            else
            {
                retval = input.Replace(replace, to);
            }
            return retval;
        }

        public static string ParseString(string content, string replace, string to, int startIndex, int length, int wordNum, string ellipsis, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string formatString)
        {
            var parsedContent = content;

            if (!string.IsNullOrEmpty(replace))
            {
                parsedContent = Replace(replace, parsedContent, to);
            }

            if (isClearTags)
            {
                parsedContent = StripTags(parsedContent);
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                if (startIndex > 0 || length > 0)
                {
                    try
                    {
                        parsedContent = length > 0 ? parsedContent.Substring(startIndex, length) : parsedContent.Substring(startIndex);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (wordNum > 0)
                {
                    parsedContent = MaxLengthText(parsedContent, wordNum, ellipsis);
                }

                if (isReturnToBr)
                {
                    parsedContent = ReplaceNewlineToBr(parsedContent);
                }

                if (!string.IsNullOrEmpty(formatString))
                {
                    parsedContent = string.Format(formatString, parsedContent);
                }

                if (isLower)
                {
                    parsedContent = parsedContent.ToLower();
                }
                if (isUpper)
                {
                    parsedContent = parsedContent.ToUpper();
                }
            }

            return parsedContent;
        }

        private static string MaxLengthText(string inputString, int maxLength, string endString)
        {
            var retval = inputString;
            try
            {
                if (maxLength > 0)
                {
                    var decodedInputString = HttpUtility.HtmlDecode(retval);
                    retval = decodedInputString;

                    var totalLength = maxLength * 2;
                    var length = 0;
                    var builder = new StringBuilder();

                    var isOneBytesChar = false;
                    var lastChar = ' ';

                    if (!string.IsNullOrEmpty(retval))
                    {
                        foreach (var singleChar in retval.ToCharArray())
                        {
                            builder.Append(singleChar);

                            if (IsTwoBytesChar(singleChar))
                            {
                                length += 2;
                                if (length >= totalLength)
                                {
                                    lastChar = singleChar;
                                    break;
                                }
                            }
                            else
                            {
                                length += 1;
                                if (length == totalLength)
                                {
                                    isOneBytesChar = true;//已经截取到需要的字数，再多截取一位
                                }
                                else if (length > totalLength)
                                {
                                    lastChar = singleChar;
                                    break;
                                }
                                else
                                {
                                    isOneBytesChar = !isOneBytesChar;
                                }
                            }
                        }
                    }
                    if (isOneBytesChar && length > totalLength)
                    {
                        builder.Length--;
                        var theStr = builder.ToString();
                        retval = builder.ToString();
                        if (char.IsLetter(lastChar))
                        {
                            for (var i = theStr.Length - 1; i > 0; i--)
                            {
                                var theChar = theStr[i];
                                if (!IsTwoBytesChar(theChar) && char.IsLetter(theChar))
                                {
                                    retval = retval.Substring(0, i - 1);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            //int index = retval.LastIndexOfAny(new char[] { ' ', '\t', '\n', '\v', '\f', '\r', '\x0085' });
                            //if (index != -1)
                            //{
                            //    retval = retval.Substring(0, index);
                            //}
                        }
                    }
                    else
                    {
                        retval = builder.ToString();
                    }

                    var isCut = decodedInputString != retval;
                    retval = HttpUtility.HtmlEncode(retval);

                    if (isCut && endString != null)
                    {
                        retval += endString;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return retval;
        }

        private static bool IsTwoBytesChar(char chr)
        {
            // 使用中文支持编码
            return Encoding.GetEncoding("gb2312").GetByteCount(new[] { chr }) == 2;
        }

        public static string JsonSerialize(object obj)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var timeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                settings.Converters.Add(timeFormat);

                return JsonConvert.SerializeObject(obj, settings);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static JObject PostData
        {
            get
            {
                var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                var raw = bodyStream.ReadToEnd();
                return !string.IsNullOrEmpty(raw) ? JObject.Parse(raw) : new JObject();
            }
        }

        public static string GetPostString(JObject postData, string name)
        {
            return postData[name]?.ToString();
        }

        public static int GetPostInt(JObject postData, string name, int defaultValue = 0)
        {
            return ToInt(postData[name]?.ToString(), defaultValue);
        }

        public static T GetPostObject<T>(JObject postData, string name = "")
        {
            string json;
            if (string.IsNullOrEmpty(name))
            {
                var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                json = bodyStream.ReadToEnd();
            }
            else
            {
                json = GetPostString(postData, name);
            }
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var timeFormat = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
            settings.Converters.Add(timeFormat);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
