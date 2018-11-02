using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Photo.Core.Model;

namespace SS.Photo.Core.Provider
{
    public static class PhotoDao
    {
        public const string TableName = "ss_Photo";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.ContentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.SmallUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.MiddleUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.LargeUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.Description),
                DataType = DataType.VarChar,
                DataLength = 255
            }
        };

        private static readonly string ParmId = $"@{nameof(PhotoInfo.Id)}";
        private static readonly string ParmSiteId = $"@{nameof(PhotoInfo.SiteId)}";
        private static readonly string ParmChannelId = $"@{nameof(PhotoInfo.ChannelId)}";
        private static readonly string ParmContentId = $"@{nameof(PhotoInfo.ContentId)}";
        private static readonly string ParmSmallUrl = $"@{nameof(PhotoInfo.SmallUrl)}";
        private static readonly string ParmMiddleUrl = $"@{nameof(PhotoInfo.MiddleUrl)}";
        private static readonly string ParmLargeUrl = $"@{nameof(PhotoInfo.LargeUrl)}";
        private static readonly string ParmTaxis = $"@{nameof(PhotoInfo.Taxis)}";
        private static readonly string ParmDescription = $"@{nameof(PhotoInfo.Description)}";

        public static int Insert(PhotoInfo photoInfo)
        {
            var maxTaxis = GetMaxTaxis(photoInfo.SiteId, photoInfo.ChannelId, photoInfo.ContentId);
            photoInfo.Taxis = maxTaxis + 1;

            var sqlString =
                $@"INSERT INTO {TableName} (
    {nameof(PhotoInfo.SiteId)}, 
    {nameof(PhotoInfo.ChannelId)}, 
    {nameof(PhotoInfo.ContentId)}, 
    {nameof(PhotoInfo.SmallUrl)}, 
    {nameof(PhotoInfo.MiddleUrl)}, 
    {nameof(PhotoInfo.LargeUrl)}, 
    {nameof(PhotoInfo.Taxis)}, 
    {nameof(PhotoInfo.Description)}
) VALUES (
    @{nameof(PhotoInfo.SiteId)}, 
    @{nameof(PhotoInfo.ChannelId)}, 
    @{nameof(PhotoInfo.ContentId)}, 
    @{nameof(PhotoInfo.SmallUrl)}, 
    @{nameof(PhotoInfo.MiddleUrl)}, 
    @{nameof(PhotoInfo.LargeUrl)}, 
    @{nameof(PhotoInfo.Taxis)}, 
    @{nameof(PhotoInfo.Description)}
)";

            var parms = new[]
            {
                Context.DatabaseApi.GetParameter(ParmSiteId, photoInfo.SiteId),
                Context.DatabaseApi.GetParameter(ParmChannelId, photoInfo.ChannelId),
                Context.DatabaseApi.GetParameter(ParmContentId, photoInfo.ContentId),
                Context.DatabaseApi.GetParameter(ParmSmallUrl, photoInfo.SmallUrl),
                Context.DatabaseApi.GetParameter(ParmMiddleUrl, photoInfo.MiddleUrl),
                Context.DatabaseApi.GetParameter(ParmLargeUrl, photoInfo.LargeUrl),
                Context.DatabaseApi.GetParameter(ParmTaxis, photoInfo.Taxis),
                Context.DatabaseApi.GetParameter(ParmDescription, photoInfo.Description)
            };

            return Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(PhotoInfo.Id), Context.ConnectionString, sqlString, parms);
        }

        public static void UpdateDescription(int photoId, string description)
        {
            var parameters = new[]
            {
                Context.DatabaseApi.GetParameter(nameof(PhotoInfo.Description), description),
                Context.DatabaseApi.GetParameter(nameof(PhotoInfo.Id), photoId)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, $"UPDATE {TableName} SET {nameof(PhotoInfo.Description)} = @{nameof(PhotoInfo.Description)} WHERE {nameof(PhotoInfo.Id)} = @{nameof(PhotoInfo.Id)}", parameters);
        }

        public static void UpdateTaxis(List<int> photoIds)
        {
            var taxis = 1;
            foreach (var photoId in photoIds)
            {
                SetTaxis(photoId, taxis);
                taxis++;
            }
        }

        public static void Delete(int id)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(PhotoInfo.Id)} = @{nameof(PhotoInfo.Id)}";

            var parms = new[]
            {
                Context.DatabaseApi.GetParameter(ParmId, id)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parms);
        }

        public static void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            var sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(PhotoInfo.Id)} IN ({string.Join(",", idList)})";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }

        public static void Delete(int siteId, int channelId, int contentId)
        {
            var sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = @{nameof(PhotoInfo.SiteId)} AND {nameof(PhotoInfo.ChannelId)} = @{nameof(PhotoInfo.ChannelId)} AND {nameof(PhotoInfo.ContentId)} = @{nameof(PhotoInfo.ContentId)}";

            var parms = new[]
            {
                Context.DatabaseApi.GetParameter(ParmSiteId, siteId),
                Context.DatabaseApi.GetParameter(ParmChannelId, channelId),
                Context.DatabaseApi.GetParameter(ParmContentId, contentId)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parms);
        }

        public static PhotoInfo GetPhotoInfo(int id)
        {
            PhotoInfo photoInfo = null;

            var sqlString = $@"SELECT 
    {nameof(PhotoInfo.Id)}, 
    {nameof(PhotoInfo.SiteId)}, 
    {nameof(PhotoInfo.ChannelId)}, 
    {nameof(PhotoInfo.ContentId)}, 
    {nameof(PhotoInfo.SmallUrl)}, 
    {nameof(PhotoInfo.MiddleUrl)}, 
    {nameof(PhotoInfo.LargeUrl)}, 
    {nameof(PhotoInfo.Taxis)}, 
    {nameof(PhotoInfo.Description)}
FROM {TableName} WHERE {nameof(PhotoInfo.Id)} = {id}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    photoInfo = GetPhotoInfo(rdr);
                }
                rdr.Close();
            }

            return photoInfo;
        }

        public static PhotoInfo GetFirstPhotoInfo(int siteId, int channelId, int contentId)
        {
            PhotoInfo photoInfo = null;

            var sqlString = Context.DatabaseApi.GetPageSqlString(TableName, $"{nameof(PhotoInfo.Id)}, {nameof(PhotoInfo.SiteId)}, {nameof(PhotoInfo.ChannelId)}, {nameof(PhotoInfo.ContentId)}, {nameof(PhotoInfo.SmallUrl)}, {nameof(PhotoInfo.MiddleUrl)}, {nameof(PhotoInfo.LargeUrl)}, {nameof(PhotoInfo.Taxis)}, {nameof(PhotoInfo.Description)}", $"WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId}", $"ORDER BY {nameof(PhotoInfo.Taxis)}", 0, 1);

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    photoInfo = GetPhotoInfo(rdr);
                }
                rdr.Close();
            }

            return photoInfo;
        }

        public static int GetCount(int siteId, int channelId, int contentId)
        {
            var sqlString =
                $"SELECT Count(*) FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId}";

            return Dao.GetIntResult(sqlString);
        }

        public static List<int> GetPhotoContentIdList(int siteId, int channelId, int contentId)
        {
            var list = new List<int>();

            string sqlString =
                $"SELECT {nameof(PhotoInfo.Id)} FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId} ORDER BY {nameof(PhotoInfo.Taxis)}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public static List<PhotoInfo> GetPhotoInfoList(int siteId, int channelId, int contentId)
        {
            var list = new List<PhotoInfo>();

            string sqlString =
                $"SELECT {nameof(PhotoInfo.Id)}, {nameof(PhotoInfo.SiteId)}, {nameof(PhotoInfo.ChannelId)}, {nameof(PhotoInfo.ContentId)}, {nameof(PhotoInfo.SmallUrl)}, {nameof(PhotoInfo.MiddleUrl)}, {nameof(PhotoInfo.LargeUrl)}, {nameof(PhotoInfo.Taxis)}, {nameof(PhotoInfo.Description)} FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId} ORDER BY {nameof(PhotoInfo.Taxis)}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetPhotoInfo(rdr));
                }
                rdr.Close();
            }

            return list;
        }

        private static void SetTaxis(int id, int taxis)
        {
            string sqlString = $"UPDATE {TableName} SET {nameof(PhotoInfo.Taxis)} = {taxis} WHERE {nameof(PhotoInfo.Id)} = {id}";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }

        private static int GetMaxTaxis(int siteId, int channelId, int contentId)
        {
            string sqlString =
                $"SELECT MAX({nameof(PhotoInfo.Taxis)}) FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId}";
            var maxTaxis = 0;

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    maxTaxis = rdr.GetInt32(0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        private static PhotoInfo GetPhotoInfo(IDataReader rdr)
        {
            if (rdr == null) return null;

            var i = 0;
            var photoInfo = new PhotoInfo
            {
                Id = Context.DatabaseApi.GetInt(rdr, i++),
                SiteId = Context.DatabaseApi.GetInt(rdr, i++),
                ChannelId = Context.DatabaseApi.GetInt(rdr, i++),
                ContentId = Context.DatabaseApi.GetInt(rdr, i++),
                SmallUrl = Context.DatabaseApi.GetString(rdr, i++),
                MiddleUrl = Context.DatabaseApi.GetString(rdr, i++),
                LargeUrl = Context.DatabaseApi.GetString(rdr, i++),
                Taxis = Context.DatabaseApi.GetInt(rdr, i++),
                Description = Context.DatabaseApi.GetString(rdr, i)
            };

            return photoInfo;
        }

        public static int GetSiblingContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var contentId = 0;
            var sqlString = Context.DatabaseApi.GetPageSqlString(tableName, nameof(IContentInfo.Id), $"WHERE ({nameof(IContentInfo.ChannelId)} = {channelId} AND {nameof(IContentInfo.Taxis)} > {taxis} AND {nameof(IContentInfo.IsChecked)} = '{true}')", $"ORDER BY {nameof(IContentInfo.Taxis)}", 0, 1);
            if (isNextContent)
            {
                sqlString = Context.DatabaseApi.GetPageSqlString(tableName, nameof(IContentInfo.Id), $"WHERE ({nameof(IContentInfo.ChannelId)} = {channelId} AND {nameof(IContentInfo.Taxis)} < {taxis} AND {nameof(IContentInfo.IsChecked)} = '{true}')", $"ORDER BY {nameof(IContentInfo.Taxis)} DESC", 0, 1);
            }

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    contentId = Context.DatabaseApi.GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }
    }
}