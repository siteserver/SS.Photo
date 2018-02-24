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
    }
}
