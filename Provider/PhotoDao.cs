using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Photo.Model;

namespace SS.Photo.Provider
{
    public class PhotoDao
    {
        public const string TableName = "ss_Photo";

        public List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(PhotoInfo.Id),
                DataType = DataType.Integer
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

        private readonly string _connectionString;
        private readonly IDatabaseApi _helper;

        public PhotoDao()
        {
            _connectionString = Main.Instance.ConnectionString;
            _helper = Main.Instance.DatabaseApi;
        }

        private static readonly string ParmId = $"@{nameof(PhotoInfo.Id)}";
        private static readonly string ParmSiteId = $"@{nameof(PhotoInfo.SiteId)}";
        private static readonly string ParmChannelId = $"@{nameof(PhotoInfo.ChannelId)}";
        private static readonly string ParmContentId = $"@{nameof(PhotoInfo.ContentId)}";
        private static readonly string ParmSmallUrl = $"@{nameof(PhotoInfo.SmallUrl)}";
        private static readonly string ParmMiddleUrl = $"@{nameof(PhotoInfo.MiddleUrl)}";
        private static readonly string ParmLargeUrl = $"@{nameof(PhotoInfo.LargeUrl)}";
        private static readonly string ParmTaxis = $"@{nameof(PhotoInfo.Taxis)}";
        private static readonly string ParmDescription = $"@{nameof(PhotoInfo.Description)}";

        public int Insert(PhotoInfo photoInfo)
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
                _helper.GetParameter(ParmSiteId, photoInfo.SiteId),
                _helper.GetParameter(ParmChannelId, photoInfo.ChannelId),
                _helper.GetParameter(ParmContentId, photoInfo.ContentId),
                _helper.GetParameter(ParmSmallUrl, photoInfo.SmallUrl),
                _helper.GetParameter(ParmMiddleUrl, photoInfo.MiddleUrl),
                _helper.GetParameter(ParmLargeUrl, photoInfo.LargeUrl),
                _helper.GetParameter(ParmTaxis, photoInfo.Taxis),
                _helper.GetParameter(ParmDescription, photoInfo.Description)
            };

            return _helper.ExecuteNonQueryAndReturnId(TableName, nameof(PhotoInfo.Id), _connectionString, sqlString, parms);
        }

        public void UpdateDescription(int photoId, string description)
        {
            var parameters = new[]
            {
                _helper.GetParameter(nameof(PhotoInfo.Description), description),
                _helper.GetParameter(nameof(PhotoInfo.Id), photoId)
            };

            _helper.ExecuteNonQuery(_connectionString, $"UPDATE {TableName} SET {nameof(PhotoInfo.Description)} = @{nameof(PhotoInfo.Description)} WHERE {nameof(PhotoInfo.Id)} = @{nameof(PhotoInfo.Id)}", parameters);
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

        public void Delete(int id)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(PhotoInfo.Id)} = @{nameof(PhotoInfo.Id)}";

            var parms = new[]
            {
                _helper.GetParameter(ParmId, id)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            var sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(PhotoInfo.Id)} IN ({string.Join(",", idList)})";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void Delete(int siteId, int channelId, int contentId)
        {
            var sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = @{nameof(PhotoInfo.SiteId)} AND {nameof(PhotoInfo.ChannelId)} = @{nameof(PhotoInfo.ChannelId)} AND {nameof(PhotoInfo.ContentId)} = @{nameof(PhotoInfo.ContentId)}";

            var parms = new[]
            {
                _helper.GetParameter(ParmSiteId, siteId),
                _helper.GetParameter(ParmChannelId, channelId),
                _helper.GetParameter(ParmContentId, contentId)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        public PhotoInfo GetPhotoInfo(int id)
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

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    photoInfo = GetPhotoInfo(rdr);
                }
                rdr.Close();
            }

            return photoInfo;
        }

        public PhotoInfo GetFirstPhotoInfo(int siteId, int channelId, int contentId)
        {
            PhotoInfo photoInfo = null;

            var sqlString = _helper.GetPageSqlString(TableName, $"{nameof(PhotoInfo.Id)}, {nameof(PhotoInfo.SiteId)}, {nameof(PhotoInfo.ChannelId)}, {nameof(PhotoInfo.ContentId)}, {nameof(PhotoInfo.SmallUrl)}, {nameof(PhotoInfo.MiddleUrl)}, {nameof(PhotoInfo.LargeUrl)}, {nameof(PhotoInfo.Taxis)}, {nameof(PhotoInfo.Description)}", $"WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId}", $"ORDER BY {nameof(PhotoInfo.Taxis)}", 0, 1);

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    photoInfo = GetPhotoInfo(rdr);
                }
                rdr.Close();
            }

            return photoInfo;
        }

        public int GetCount(int siteId, int channelId, int contentId)
        {
            var sqlString =
                $"SELECT Count(*) FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId}";

            return Main.Dao.GetIntResult(sqlString);
        }

        public List<int> GetPhotoContentIdList(int siteId, int channelId, int contentId)
        {
            var list = new List<int>();

            string sqlString =
                $"SELECT {nameof(PhotoInfo.Id)} FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId} ORDER BY {nameof(PhotoInfo.Taxis)}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<PhotoInfo> GetPhotoInfoList(int siteId, int channelId, int contentId)
        {
            var list = new List<PhotoInfo>();

            string sqlString =
                $"SELECT {nameof(PhotoInfo.Id)}, {nameof(PhotoInfo.SiteId)}, {nameof(PhotoInfo.ChannelId)}, {nameof(PhotoInfo.ContentId)}, {nameof(PhotoInfo.SmallUrl)}, {nameof(PhotoInfo.MiddleUrl)}, {nameof(PhotoInfo.LargeUrl)}, {nameof(PhotoInfo.Taxis)}, {nameof(PhotoInfo.Description)} FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId} ORDER BY {nameof(PhotoInfo.Taxis)}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetPhotoInfo(rdr));
                }
                rdr.Close();
            }

            return list;
        }

        private void SetTaxis(int id, int taxis)
        {
            string sqlString = $"UPDATE {TableName} SET {nameof(PhotoInfo.Taxis)} = {taxis} WHERE {nameof(PhotoInfo.Id)} = {id}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        private int GetMaxTaxis(int siteId, int channelId, int contentId)
        {
            string sqlString =
                $"SELECT MAX({nameof(PhotoInfo.Taxis)}) FROM {TableName} WHERE {nameof(PhotoInfo.SiteId)} = {siteId} AND {nameof(PhotoInfo.ChannelId)} = {channelId} AND {nameof(PhotoInfo.ContentId)} = {contentId}";
            var maxTaxis = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    maxTaxis = rdr.GetInt32(0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        private PhotoInfo GetPhotoInfo(IDataReader rdr)
        {
            if (rdr == null) return null;

            var i = 0;
            var photoInfo = new PhotoInfo
            {
                Id = _helper.GetInt(rdr, i++),
                SiteId = _helper.GetInt(rdr, i++),
                ChannelId = _helper.GetInt(rdr, i++),
                ContentId = _helper.GetInt(rdr, i++),
                SmallUrl = _helper.GetString(rdr, i++),
                MiddleUrl = _helper.GetString(rdr, i++),
                LargeUrl = _helper.GetString(rdr, i++),
                Taxis = _helper.GetInt(rdr, i++),
                Description = _helper.GetString(rdr, i)
            };

            return photoInfo;
        }

        public int GetSiblingContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var contentId = 0;
            var sqlString = _helper.GetPageSqlString(tableName, nameof(IContentInfo.Id), $"WHERE ({nameof(IContentInfo.ChannelId)} = {channelId} AND {nameof(IContentInfo.Taxis)} > {taxis} AND {nameof(IContentInfo.IsChecked)} = '{true}')", $"ORDER BY {nameof(IContentInfo.Taxis)}", 0, 1);
            if (isNextContent)
            {
                sqlString = _helper.GetPageSqlString(tableName, nameof(IContentInfo.Id), $"WHERE ({nameof(IContentInfo.ChannelId)} = {channelId} AND {nameof(IContentInfo.Taxis)} < {taxis} AND {nameof(IContentInfo.IsChecked)} = '{true}')", $"ORDER BY {nameof(IContentInfo.Taxis)} DESC", 0, 1);
            }

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    contentId = _helper.GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }
    }
}