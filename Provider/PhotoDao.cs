using System.Collections;
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
        private readonly IDataApi _helper;

        public PhotoDao()
        {
            _connectionString = Main.Instance.ConnectionString;
            _helper = Main.Instance.DataApi;
        }

        private const string SqlUpdatePhotoContent = "UPDATE ss_Photo SET SmallUrl = @SmallUrl, MiddleUrl = @MiddleUrl, LargeUrl = @LargeUrl, Taxis = @Taxis, Description = @Description WHERE Id = @Id";
        private const string SqlDeletePhotoContent = "DELETE FROM ss_Photo WHERE Id = @Id";
        private const string SqlDeletePhotoContents = "DELETE FROM ss_Photo WHERE SiteId = @SiteId AND ContentId = @ContentId";

        private const string ParmId = "@Id";
        private const string ParmPublishmentsystemid = "@SiteId";
        private const string ParmContentid = "@ContentId";
        private const string ParmSmallUrl = "@SmallUrl";
        private const string ParmMiddleUrl = "@MiddleUrl";
        private const string ParmLargeUrl = "@LargeUrl";
        private const string ParmTaxis = "@Taxis";
        private const string ParmDescription = "@Description";

        public void Insert(PhotoInfo photoInfo)
        {
            var maxTaxis = GetMaxTaxis(photoInfo.SiteId, photoInfo.ContentId);
            photoInfo.Taxis = maxTaxis + 1;

            const string sqlString = "INSERT INTO ss_Photo (SiteId, ContentId, SmallUrl, MiddleUrl, LargeUrl, Taxis, Description) VALUES (@SiteId, @ContentId, @SmallUrl, @MiddleUrl, @LargeUrl, @Taxis, @Description)";

            var parms = new[]
            {
                _helper.GetParameter(ParmPublishmentsystemid, photoInfo.SiteId),
                _helper.GetParameter(ParmContentid, photoInfo.ContentId),
                _helper.GetParameter(ParmSmallUrl, photoInfo.SmallUrl),
                _helper.GetParameter(ParmMiddleUrl, photoInfo.MiddleUrl),
                _helper.GetParameter(ParmLargeUrl, photoInfo.LargeUrl),
                _helper.GetParameter(ParmTaxis, photoInfo.Taxis),
                _helper.GetParameter(ParmDescription, photoInfo.Description)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        public void Update(PhotoInfo photoInfo)
        {
            var updateParms = new[]
            {
                _helper.GetParameter(ParmSmallUrl, photoInfo.SmallUrl),
                _helper.GetParameter(ParmMiddleUrl, photoInfo.MiddleUrl),
                _helper.GetParameter(ParmLargeUrl, photoInfo.LargeUrl),
                _helper.GetParameter(ParmTaxis, photoInfo.Taxis),
                _helper.GetParameter(ParmDescription, photoInfo.Description),
                _helper.GetParameter(ParmId, photoInfo.Id)
            };

            _helper.ExecuteNonQuery(_connectionString, SqlUpdatePhotoContent, updateParms);
        }

        public void Delete(int id)
        {
            var parms = new[]
            {
                _helper.GetParameter(ParmId, id)
            };

            _helper.ExecuteNonQuery(_connectionString, SqlDeletePhotoContent, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            string sqlString =
                $"DELETE FROM ss_Photo WHERE Id IN ({string.Join(",", idList)})";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void Delete(int siteId, int contentId)
        {
            var parms = new[]
            {
                _helper.GetParameter(ParmPublishmentsystemid, siteId),
                _helper.GetParameter(ParmContentid, contentId)
            };

            _helper.ExecuteNonQuery(_connectionString, SqlDeletePhotoContents, parms);
        }

        public PhotoInfo GetPhotoInfo(int id)
        {
            PhotoInfo photoInfo = null;

            string sqlString =
                $"SELECT Id, SiteId, ContentId, SmallUrl, MiddleUrl, LargeUrl, Taxis, Description FROM ss_Photo WHERE Id = {id}";

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

        public PhotoInfo GetFirstPhotoInfo(int siteId, int contentId)
        {
            PhotoInfo photoInfo = null;

            //string sqlString =
            //    $"SELECT TOP 1 Id, SiteId, ContentId, SmallUrl, MiddleUrl, LargeUrl, Taxis, Description FROM ss_Photo WHERE SiteId = {siteId} AND ContentId = {contentId} ORDER BY Taxis";
            var sqlString = _helper.ToTopSqlString(TableName, "Id, SiteId, ContentId, SmallUrl, MiddleUrl, LargeUrl, Taxis, Description", $"WHERE SiteId = {siteId} AND ContentId = {contentId}", "ORDER BY Taxis", 1);

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

        public int GetCount(int siteId, int contentId)
        {
            var sqlString =
                $"SELECT Count(*) FROM ss_Photo WHERE SiteId = {siteId} AND ContentId = {contentId}";

            return _helper.ExecuteInt(_connectionString, sqlString);
        }

        public string GetSortFieldName()
        {
            return "Taxis";
        }

        public string GetSelectSqlString(int siteId, int contentId)
        {
            return
                $"SELECT Id, SiteId, ContentId, SmallUrl, MiddleUrl, LargeUrl, Taxis, Description FROM ss_Photo WHERE SiteId = {siteId} AND ContentId = {contentId} ORDER BY Taxis";
        }

        public IEnumerable GetStlDataSource(int siteId, int contentId, int startNum, int totalNum)
        {
            var sqlString = _helper.ToTopSqlString(_connectionString, TableName,
                "Id, SiteId, ContentId, SmallUrl, MiddleUrl, LargeUrl, Taxis, Description",
                $"WHERE (SiteId = {siteId} AND ContentId = {contentId})", "ORDER BY Taxis",
                startNum - 1, totalNum);

            //var sqlSelect = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, "*", $"WHERE (SiteId = {siteId} AND ContentId = {contentId})", "ORDER BY Taxis");

            return (IEnumerable)_helper.ExecuteReader(_connectionString, sqlString);
        }

        public List<int> GetPhotoContentIdList(int siteId, int contentId)
        {
            var list = new List<int>();

            string sqlString =
                $"SELECT Id FROM ss_Photo WHERE SiteId = {siteId} AND ContentId = {contentId} ORDER BY Taxis";

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

        public List<PhotoInfo> GetPhotoInfoList(int siteId, int contentId)
        {
            var list = new List<PhotoInfo>();

            string sqlString =
                $"SELECT Id, SiteId, ContentId, SmallUrl, MiddleUrl, LargeUrl, Taxis, Description FROM ss_Photo WHERE SiteId = {siteId} AND ContentId = {contentId} ORDER BY Taxis";

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

        private int GetTaxis(int id)
        {
            var sqlString = $"SELECT Taxis FROM ss_Photo WHERE (Id = {id})";

            return _helper.ExecuteInt(_connectionString, sqlString);
        }

        private void SetTaxis(int id, int taxis)
        {
            string sqlString = $"UPDATE ss_Photo SET Taxis = {taxis} WHERE (Id = {id})";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        private int GetMaxTaxis(int siteId, int contentId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM ss_Photo WHERE (SiteId = {siteId} AND ContentId = {contentId})";
            var maxTaxis = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    maxTaxis = rdr.GetInt32(0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public bool UpdateTaxisToUp(int siteId, int contentId, int id)
        {
            //Get Higher Taxis and Id
            //string sqlString =
            //    $"SELECT TOP 1 Id, Taxis FROM ss_Photo WHERE (Taxis > (SELECT Taxis FROM ss_Photo WHERE Id = {id}) AND (SiteId = {siteId} AND ContentId = {contentId})) ORDER BY Taxis";
            string sqlString = _helper.ToTopSqlString(TableName, "Id, Taxis", $"WHERE (Taxis > (SELECT Taxis FROM ss_Photo WHERE Id = {id}) AND (SiteId = {siteId} AND ContentId = {contentId}))", "ORDER BY Taxis", 1);

            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    higherId = _helper.GetInt(rdr, 0);
                    higherTaxis = _helper.GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (higherId > 0)
            {
                //Get Taxis Of Selected Id
                var selectedTaxis = GetTaxis(id);

                //Set The Selected Class Taxis To Higher Level
                SetTaxis(id, higherTaxis);
                //Set The Higher Class Taxis To Lower Level
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int siteId, int contentId, int id)
        {
            //Get Lower Taxis and Id
            //string sqlString =
            //    $"SELECT TOP 1 Id, Taxis FROM ss_Photo WHERE (Taxis < (SELECT Taxis FROM ss_Photo WHERE Id = {id}) AND (SiteId = {siteId} AND ContentId = {contentId})) ORDER BY Taxis DESC";
            var sqlString = _helper.ToTopSqlString(TableName, "Id, Taxis", $"WHERE (Taxis < (SELECT Taxis FROM ss_Photo WHERE Id = {id}) AND (SiteId = {siteId} AND ContentId = {contentId}))", "ORDER BY Taxis DESC", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = _helper.GetInt(rdr, 0);
                    lowerTaxis = _helper.GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (lowerId > 0)
            {
                //Get Taxis Of Selected Class
                var selectedTaxis = GetTaxis(id);

                //Set The Selected Class Taxis To Lower Level
                SetTaxis(id, lowerTaxis);
                //Set The Lower Class Taxis To Higher Level
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        private PhotoInfo GetPhotoInfo(IDataReader rdr)
        {
            if (rdr == null) return null;

            var i = 0;
            var photoInfo = new PhotoInfo
            {
                Id = _helper.GetInt(rdr, i++),
                SiteId = _helper.GetInt(rdr, i++),
                ContentId = _helper.GetInt(rdr, i++),
                SmallUrl = _helper.GetString(rdr, i++),
                MiddleUrl = _helper.GetString(rdr, i++),
                LargeUrl = _helper.GetString(rdr, i++),
                Taxis = _helper.GetInt(rdr, i++),
                Description = _helper.GetString(rdr, i)
            };

            return photoInfo;
        }

        public int GetSiblingContentId(string tableName, int nodeId, int taxis, bool isNextContent)
        {
            var contentId = 0;
            var sqlString = _helper.ToTopSqlString(tableName, "Id", $"WHERE (ChannelId = {nodeId} AND Taxis > {taxis} AND IsChecked = 'True')", "ORDER BY Taxis", 1);
            if (isNextContent)
            {
                sqlString = _helper.ToTopSqlString(tableName, "Id", $"WHERE (ChannelId = {nodeId} AND Taxis < {taxis} AND IsChecked = 'True')", "ORDER BY Taxis DESC", 1);
            }

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    contentId = _helper.GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }
    }
}