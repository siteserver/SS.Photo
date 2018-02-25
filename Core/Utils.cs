using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SS.Photo.Core
{
    public class Utils
    {
        public static string ToJsString(string value)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                retval = value.Replace("'", @"\'").Replace("\r", "\\r").Replace("\n", "\\n");
            }
            return retval;
        }

        public static string GetMessageHtml(string message, bool isSuccess)
        {
            return isSuccess
                ? $@"<div class=""alert alert-success"" role=""alert"">{message}</div>"
                : $@"<div class=""alert alert-danger"" role=""alert"">{message}</div>";
        }

        public static int ToInt(string intStr)
        {
            return ToInt(intStr, 0);
        }

        public static int ToInt(string intStr, int defaultValue)
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
