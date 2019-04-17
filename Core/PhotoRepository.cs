using System.Collections.Generic;
using System.Linq;
using Datory;
using SiteServer.Plugin;

namespace SS.Photo.Core
{
    public class PhotoRepository : Repository<PhotoInfo>
    {
        public PhotoRepository() : base(Context.Environment.Database)
        {
        }

        private class Attr
        {
            public const string Id = nameof(PhotoInfo.Id);
            public const string SiteId = nameof(PhotoInfo.SiteId);
            public const string ChannelId = nameof(PhotoInfo.ChannelId);
            public const string ContentId = nameof(PhotoInfo.ContentId);
            public const string SmallUrl = nameof(PhotoInfo.SmallUrl);
            public const string MiddleUrl = nameof(PhotoInfo.MiddleUrl);
            public const string LargeUrl = nameof(PhotoInfo.LargeUrl);
            public const string Taxis = nameof(PhotoInfo.Taxis);
            public const string Description = nameof(PhotoInfo.Description);
        }

        public override int Insert(PhotoInfo photoInfo)
        {
            var maxTaxis = GetMaxTaxis(photoInfo.SiteId, photoInfo.ChannelId, photoInfo.ContentId);
            photoInfo.Taxis = maxTaxis + 1;

            photoInfo.Id =  base.Insert(photoInfo);

            return photoInfo.Id;
        }

        public void UpdateDescription(int photoId, string description)
        {
            base.Update(Q.Set(Attr.Description, description).Where(Attr.Id, photoId));
        }

        public void UpdateTaxis(List<int> photoIds)
        {
            var taxis = 1;
            foreach (var photoId in photoIds)
            {
                SetTaxis(photoId, taxis);
                taxis++;
            }
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            base.Delete(Q.WhereIn(Attr.Id, idList));
        }

        public void Delete(int siteId, int channelId, int contentId)
        {
            base.Delete(Q.Where(Attr.SiteId, siteId).Where(Attr.ChannelId, channelId).Where(Attr.ContentId, contentId));
        }

        public PhotoInfo GetFirstPhotoInfo(int siteId, int channelId, int contentId)
        {
            return base.Get(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.ContentId, contentId)
                .OrderBy(Attr.Taxis)
            );
        }

        public int GetCount(int siteId, int channelId, int contentId)
        {
            return Count(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.ContentId, contentId)
            );
        }

        public IList<int> GetPhotoContentIdList(int siteId, int channelId, int contentId)
        {
            return GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.ContentId, contentId)
                .OrderBy(Attr.Taxis)
            );
        }

        public List<PhotoInfo> GetPhotoInfoList(int siteId, int channelId, int contentId)
        {
            return GetAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.ContentId, contentId)
                .OrderBy(Attr.Taxis)
            ).ToList();
        }

        private void SetTaxis(int id, int taxis)
        {
            Update(Q.Set(Attr.Taxis, taxis).Where(Attr.Id, id));
        }

        private int GetMaxTaxis(int siteId, int channelId, int contentId)
        {
            return Max(Q
                       .Select(Attr.Taxis)
                       .Where(Attr.SiteId, siteId)
                       .Where(Attr.ChannelId, channelId)
                       .Where(Attr.ContentId, contentId)
                   ) ?? 0;
        }

        public int GetSiblingContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var contentRepository = new Repository(Context.Environment.Database, tableName);

            if (isNextContent)
            {
                return contentRepository.Get<int>(Q
                    .Select(nameof(IContentInfo.Id))
                    .Where(nameof(IContentInfo.ChannelId), channelId)
                    .Where(nameof(IContentInfo.Taxis), "<", taxis)
                    .Where("IsChecked", true.ToString())
                    .OrderByDesc(nameof(IContentInfo.Taxis))
                    );
            }
            else
            {
                return contentRepository.Get<int>(Q
                    .Select(nameof(IContentInfo.Id))
                    .Where(nameof(IContentInfo.ChannelId), channelId)
                    .Where(nameof(IContentInfo.Taxis), ">", taxis)
                    .Where("IsChecked", true.ToString())
                    .OrderBy(nameof(IContentInfo.Taxis))
                );
            }
        }

        //public int GetSiblingContentId(string tableName, int channelId, int taxis, bool isNextContent)
        //{
        //    var contentRepository = new Repository(Context.Environment.Database, tableName);

        //    var contentId = 0;
        //    var sqlString = Context.DatabaseApi.GetPageSqlString(tableName, nameof(IContentInfo.Id), $"WHERE ({nameof(IContentInfo.ChannelId)} = {channelId} AND {nameof(IContentInfo.Taxis)} > {taxis} AND {nameof(IContentInfo.IsChecked)} = '{true}')", $"ORDER BY {nameof(IContentInfo.Taxis)}", 0, 1);
        //    if (isNextContent)
        //    {
        //        sqlString = Context.DatabaseApi.GetPageSqlString(tableName, nameof(IContentInfo.Id), $"WHERE ({nameof(IContentInfo.ChannelId)} = {channelId} AND {nameof(IContentInfo.Taxis)} < {taxis} AND {nameof(IContentInfo.IsChecked)} = '{true}')", $"ORDER BY {nameof(IContentInfo.Taxis)} DESC", 0, 1);
        //    }

        //    using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
        //    {
        //        if (rdr.Read() && !rdr.IsDBNull(0))
        //        {
        //            contentId = Context.DatabaseApi.GetInt(rdr, 0);
        //        }
        //        rdr.Close();
        //    }
        //    return contentId;
        //}
    }
}