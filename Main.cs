using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Photo.Model;
using SS.Photo.Pages;
using SS.Photo.Provider;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Photo
{
    public class Main : PluginBase
    {
        public static PhotoDao PhotoDao { get; private set; }

        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public ConfigInfo GetConfigInfo(int siteId)
        {
            if (!ConfigInfoDict.ContainsKey(siteId))
            {
                ConfigInfoDict[siteId] = ConfigApi.GetConfig<ConfigInfo>(siteId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[siteId];
        }

        internal static Main Instance { get; private set; }

        public override void Startup(IService service)
        {
            Instance = this;
            PhotoDao = new PhotoDao();

            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "多图内容",
                    IconClass = "ion-images",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "图片上传设置",
                            Href = $"{nameof(PageSettings)}.aspx"
                        }
                    }
                })
                .AddContentLink(new HyperLink
                {
                    Text = "内容图片",
                    NavigateUrl = $"{nameof(PageUpload)}.aspx"
                })
                .AddDatabaseTable(PhotoDao.TableName, PhotoDao.Columns)
                ;

            service.ContentTranslateCompleted += Service_ContentTranslateCompleted;
            service.ContentDeleteCompleted += Service_ContentDeleteCompleted;
        }

        private void Service_ContentDeleteCompleted(object sender, ContentEventArgs e)
        {
            PhotoDao.Delete(e.SiteId, e.ContentId);
        }

        private void Service_ContentTranslateCompleted(object sender, ContentTranslateEventArgs e)
        {
            var photoInfoList = PhotoDao.GetPhotoInfoList(e.SiteId, e.ContentId);
            if (photoInfoList.Count <= 0) return;

            foreach (var photoInfo in photoInfoList)
            {
                photoInfo.SiteId = e.TargetSiteId;
                photoInfo.ContentId = e.TargetContentId;

                FilesApi.MoveFiles(e.SiteId, e.TargetSiteId, new List<string>
                {
                    photoInfo.SmallUrl,
                    photoInfo.MiddleUrl,
                    photoInfo.LargeUrl
                });

                PhotoDao.Insert(photoInfo);
            }
        }
    }
}