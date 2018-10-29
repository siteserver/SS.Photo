using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SS.Photo.Core;
using SS.Photo.Provider;

namespace SS.Photo.Pages
{
    public class PageUpload : Page
    {
        private int _siteId;
        private int _channelId;
        private int _contentId;
        private string _returnUrl;

        public string UploadUrl => HandlerUpload.GetRedirectUrl(_siteId, _channelId, _contentId);

        public string Photos => Utils.JsonSerialize(PhotoDao.GetPhotoInfoList(_siteId, _channelId, _contentId));

        public void Page_Load(object sender, EventArgs e)
        {
            var request = SiteServer.Plugin.Context.GetCurrentRequest();
            _siteId = request.GetQueryInt("siteId");
            _channelId = request.GetQueryInt("channelId");
            _contentId = request.GetQueryInt("contentId");
            _returnUrl = request.GetQueryString("returnUrl");

            if (!request.AdminPermissions.HasSitePermissions(_siteId, Main.PluginId))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
                return;
            }

            if (Utils.ToBool(Request.QueryString["isSaveDescription"]))
            {
                var postData = Utils.PostData;
                var photoId = Utils.GetPostInt(postData, "photoId");
                var description = Utils.GetPostString(postData, "description");
                PhotoDao.UpdateDescription(photoId, description);

                HttpContext.Current.Response.Write(Utils.JsonSerialize(new { }));
                HttpContext.Current.Response.End();
            }
            else if (Utils.ToBool(Request.QueryString["isSaveTaxis"]))
            {
                var postData = Utils.PostData;
                var photoIds = Utils.GetPostObject<List<int>>(postData, "photoIds");
                PhotoDao.UpdateTaxis(photoIds);

                HttpContext.Current.Response.Write(Utils.JsonSerialize(new { }));
                HttpContext.Current.Response.End();
            }
            else if (Utils.ToBool(Request.QueryString["isDelete"]))
            {
                var postData = Utils.PostData;
                var photoId = Utils.GetPostInt(postData, "photoId");

                PhotoDao.Delete(photoId);

                HttpContext.Current.Response.Write(Utils.JsonSerialize(new { }));
                HttpContext.Current.Response.End();
            }
        }
    }
}
