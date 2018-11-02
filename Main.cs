using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Photo.Core;
using SS.Photo.Core.Model;
using SS.Photo.Core.Parse;
using SS.Photo.Core.Provider;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Photo
{
    public class Main : PluginBase
    {
        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public static ConfigInfo GetConfigInfo(int siteId)
        {
            if (!ConfigInfoDict.ContainsKey(siteId))
            {
                ConfigInfoDict[siteId] = Context.ConfigApi.GetConfig<ConfigInfo>(Utils.PluginId, siteId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[siteId];
        }

        public override void Startup(IService service)
        {
            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "内容相册",
                    IconClass = "ion-images",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "图片上传设置",
                            Href = "pages/settings.html"
                        }
                    }
                })
                .AddContentMenu(contentInfo => new Menu
                {
                    Text = "内容相册",
                    Href = "pages/photos.html"
                })
                .AddDatabaseTable(PhotoDao.TableName, PhotoDao.Columns)
                .AddStlElementParser(StlPhotos.ElementName, StlPhotos.Parse)
                .AddStlElementParser(StlPhoto.ElementName, StlPhoto.Parse)
                .AddStlElementParser(StlSlide.ElementName, StlSlide.Parse)
                ;

            service.ContentTranslateCompleted += Service_ContentTranslateCompleted;
            service.ContentDeleteCompleted += Service_ContentDeleteCompleted;
        }

        private static void Service_ContentDeleteCompleted(object sender, ContentEventArgs e)
        {
            PhotoDao.Delete(e.SiteId, e.ChannelId, e.ContentId);
        }

        private void Service_ContentTranslateCompleted(object sender, ContentTranslateEventArgs e)
        {
            var photoInfoList = PhotoDao.GetPhotoInfoList(e.SiteId, e.ChannelId, e.ContentId);
            if (photoInfoList.Count <= 0) return;

            foreach (var photoInfo in photoInfoList)
            {
                photoInfo.SiteId = e.TargetSiteId;
                photoInfo.ContentId = e.TargetContentId;

                Context.UtilsApi.MoveFiles(e.SiteId, e.TargetSiteId, new List<string>
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