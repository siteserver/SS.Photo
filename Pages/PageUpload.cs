using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SS.Photo.Core;

namespace SS.Photo.Pages
{
    public class PageUpload : Page
    {
        private int _siteId;
        private int _channelId;
        private int _contentId;
        private string _returnUrl;

        public string UploadUrl => HandlerUpload.GetRedirectUrl(_siteId, _channelId, _contentId);

        public string Photos => Utils.JsonSerialize(Main.PhotoDao.GetPhotoInfoList(_siteId, _channelId, _contentId));

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Utils.ToInt(Request.QueryString["siteId"]);
            _channelId = Utils.ToInt(Request.QueryString["channelId"]);
            _contentId = Utils.ToInt(Request.QueryString["contentId"]);
            _returnUrl = Request.QueryString["returnUrl"];

            if (!Main.Request.AdminPermissions.HasSitePermissions(_siteId, Main.PluginId))
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
                Main.PhotoDao.UpdateDescription(photoId, description);

                HttpContext.Current.Response.Write(Utils.JsonSerialize(new { }));
                HttpContext.Current.Response.End();
            }
            else if (Utils.ToBool(Request.QueryString["isSaveTaxis"]))
            {
                var postData = Utils.PostData;
                var photoIds = Utils.GetPostObject<List<int>>(postData, "photoIds");
                Main.PhotoDao.UpdateTaxis(photoIds);

                HttpContext.Current.Response.Write(Utils.JsonSerialize(new { }));
                HttpContext.Current.Response.End();
            }
            else if (Utils.ToBool(Request.QueryString["isDelete"]))
            {
                var postData = Utils.PostData;
                var photoId = Utils.GetPostInt(postData, "photoId");

                Main.PhotoDao.Delete(photoId);

                HttpContext.Current.Response.Write(Utils.JsonSerialize(new { }));
                HttpContext.Current.Response.End();
            }
        }
    }
}
