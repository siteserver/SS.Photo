using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Photo.Core;
using SS.Photo.Model;

namespace SS.Photo.Parse
{
    public class StlPhotos
    {
        public const string ElementName = "stl:photos";

        public const string AttributeTotalNum = "totalNum";
        public const string AttributeStartNum = "startNum";
        public const string AttributeScope = "scope";
        public const string AttributeOrder = "order";
        public const string AttributeWhere = "where";

        public static string Parse(IParseContext context)
        {
            var totalNum = 0;
            var startNum = 1;
            var scope = string.Empty;
            var order = string.Empty;
            var where = string.Empty;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];

                if (Utils.EqualsIgnoreCase(name, AttributeTotalNum))
                {
                    totalNum = Utils.ToInt(value);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeStartNum))
                {
                    startNum = Utils.ToInt(value);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeScope))
                {
                    scope = value;
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeOrder))
                {
                    order = value;
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeWhere))
                {
                    where = Context.ParseApi.ParseAttributeValue(value, context);
                }
            }

            var photoInfoList = Main.PhotoDao.GetPhotoInfoList(context.SiteId, context.ChannelId, context.ContentId);
            if (photoInfoList.Count == 0) return string.Empty;

            if (startNum > 1 || totalNum > 0)
            {
                if (startNum > 1)
                {
                    var count = startNum - 1;
                    if (count > photoInfoList.Count)
                    {
                        count = photoInfoList.Count;
                    }
                    photoInfoList.RemoveRange(0, count);
                }

                if (totalNum > 0)
                {
                    if (totalNum < photoInfoList.Count)
                    {
                        photoInfoList.RemoveRange(totalNum, photoInfoList.Count - totalNum);
                    }
                }
            }

            return ParseImpl(photoInfoList, context);
        }

        private static string ParseImpl(IEnumerable<PhotoInfo> photoInfoList, IParseContext context)
        {
            var parsedContent = string.Empty;

            var itemIndex = 1;
            foreach (var photoInfo in photoInfoList)
            {
                StlPhoto.SetContextItem(context, photoInfo, itemIndex++);
                parsedContent += Context.ParseApi.Parse(context.StlInnerHtml, context);
            }

            return parsedContent;
        }
    }
}
