using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using SiteServer.Plugin;

namespace SS.Photo.Core
{
    public class StlPhoto
	{
        private StlPhoto() { }
        public const string ElementName = "stl:photo";

		public const string AttributeType = "type";
        public const string AttributeLeftText = "leftText";
        public const string AttributeRightText = "rightText";
        public const string AttributeFormatString = "formatString";
        public const string AttributeStartIndex = "startIndex";
        public const string AttributeLength = "length";
		public const string AttributeWordNum = "wordNum";
        public const string AttributeEllipsis = "ellipsis";
        public const string AttributeReplace = "replace";
        public const string AttributeTo = "to";
        public const string AttributeIsClearTags = "isClearTags";
        public const string AttributeIsReturnToBr = "isReturnToBr";
        public const string AttributeIsLower = "isLower";
        public const string AttributeIsUpper = "isUpper";

        public const string TypeItemIndex = "ItemIndex";
        public const string TypeId = "Id";
        public const string TypeSmallUrl = "SmallUrl";
        public const string TypeMiddleUrl = "MiddleUrl";
        public const string TypeLargeUrl = "LargeUrl";
        public const string TypeDescription = "Description";

        public static void SetContextItem(IParseContext context, PhotoInfo photoInfo, int itemIndex)
        {
            context.Set(ElementName, photoInfo);
            context.Set(ElementName + "ItemIndex", itemIndex);
        }

        private static bool TryGetContextItem(IParseContext context, out PhotoInfo photoInfo, out int itemIndex)
        {
            photoInfo = context.Get<PhotoInfo>(ElementName);
            itemIndex = context.Get<int>(ElementName + "ItemIndex");

            return photoInfo != null && itemIndex > 0;
        }

        public static string Parse(IParseContext context)
		{
		    var leftText = string.Empty;
            var rightText = string.Empty;
            var formatString = string.Empty;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = "...";
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isReturnToBr = false;
            var isLower = false;
            var isUpper = false;
            var type = string.Empty;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var attributeName = name.ToLower();
                var value = context.StlAttributes[name];

                if (attributeName.Equals(AttributeType))
                {
                    type = value.ToLower();
                }
                else if (attributeName.Equals(AttributeLeftText))
                {
                    leftText = value;
                }
                else if (attributeName.Equals(AttributeRightText))
                {
                    rightText = value;
                }
                else if (attributeName.Equals(AttributeFormatString))
                {
                    formatString = value;
                }
                else if (attributeName.Equals(AttributeStartIndex))
                {
                    startIndex = Utils.ToInt(value);
                }
                else if (attributeName.Equals(AttributeLength))
                {
                    length = Utils.ToInt(value);
                }
                else if (attributeName.Equals(AttributeWordNum))
                {
                    wordNum = Utils.ToInt(value);
                }
                else if (attributeName.Equals(AttributeEllipsis))
                {
                    ellipsis = value;
                }
                else if (attributeName.Equals(AttributeReplace))
                {
                    replace = value;
                }
                else if (attributeName.Equals(AttributeTo))
                {
                    to = value;
                }
                else if (attributeName.Equals(AttributeIsClearTags))
                {
                    isClearTags = Utils.ToBool(value, false);
                }
                else if (attributeName.Equals(AttributeIsReturnToBr))
                {
                    isReturnToBr = Utils.ToBool(value, false);
                }
                else if (attributeName.Equals(AttributeIsLower))
                {
                    isLower = Utils.ToBool(value, true);
                }
                else if (attributeName.Equals(AttributeIsUpper))
                {
                    isUpper = Utils.ToBool(value, true);
                }
            }

		    PhotoInfo photoInfo;
		    int itemIndex;
		    if (!TryGetContextItem(context, out photoInfo, out itemIndex))
		    {
		        return string.Empty;
		    }

            return ParseImpl(context, photoInfo, itemIndex, leftText, rightText, formatString, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, type);
		}

        private static string ParseImpl(IParseContext context, PhotoInfo photoInfo, int itemIndex, string leftText, string rightText, string formatString, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string type)
        {
            var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(type))
            {
                if (!string.IsNullOrEmpty(formatString))
                {
                    formatString = formatString.Trim();
                    if (!formatString.StartsWith("{0"))
                    {
                        formatString = "{0:" + formatString;
                    }
                    if (!formatString.EndsWith("}"))
                    {
                        formatString = formatString + "}";
                    }
                }
                else
                {
                    formatString = "{0}";
                }

                if (string.IsNullOrEmpty(type) || Utils.EqualsIgnoreCase(type, "imageUrl"))
                {
                    type = TypeLargeUrl;
                }

                if (Utils.StartsWithIgnoreCase(type, TypeItemIndex))
                {
                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
                }
                else if (Utils.StartsWithIgnoreCase(type, TypeId))
                {
                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, photoInfo.Id) : photoInfo.Id.ToString();
                }
                else if (Utils.StartsWithIgnoreCase(type, TypeSmallUrl))
                {
                    parsedContent = GetImageHtml(photoInfo.SmallUrl, context.StlAttributes, context.IsStlElement);
                }
                else if (Utils.StartsWithIgnoreCase(type, TypeMiddleUrl))
                {
                    parsedContent = GetImageHtml(photoInfo.MiddleUrl, context.StlAttributes, context.IsStlElement);
                }
                else if (Utils.StartsWithIgnoreCase(type, TypeLargeUrl))
                {
                    parsedContent = GetImageHtml(photoInfo.LargeUrl, context.StlAttributes, context.IsStlElement);
                }
                else if (Utils.StartsWithIgnoreCase(type, TypeDescription) || Utils.StartsWithIgnoreCase(type, "content"))
                {
                    parsedContent = Utils.ReplaceNewlineToBr(photoInfo.Description);
                }
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = Utils.ParseString(parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    parsedContent = leftText + parsedContent + rightText;
                }
            }

            return parsedContent;
        }

        public static string GetImageHtml(string imageUrl, NameValueCollection attributes, bool isStlElement)
        {
            if (string.IsNullOrEmpty(imageUrl)) return string.Empty;

            string retval;

            if (!isStlElement)
            {
                retval = imageUrl;
            }
            else
            {
                var htmlImage = new HtmlImage();
                AddAttributesIfNotExists(htmlImage, attributes);
                htmlImage.Src = imageUrl;
                retval = GetControlRenderHtml(htmlImage);
            }
            return retval;
        }

        public static void AddAttributesIfNotExists(IAttributeAccessor accessor, NameValueCollection attributes)
        {
            if (accessor == null || attributes == null) return;

            foreach (var key in attributes.AllKeys)
            {
                if (accessor.GetAttribute(key) == null)
                {
                    accessor.SetAttribute(key, attributes[key]);
                }
            }
        }

        public static string GetControlRenderHtml(Control control)
        {
            if (control == null) return string.Empty;

            var builder = new StringBuilder();
            var sw = new System.IO.StringWriter(builder);
            var htw = new HtmlTextWriter(sw);
            control.RenderControl(htw);
            return builder.ToString();
        }
    }
}
