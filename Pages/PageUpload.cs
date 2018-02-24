using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Photo.Core;
using SS.Photo.Model;

namespace SS.Photo.Pages
{
    public class PageUpload : Page
    {
        public Literal LtlScript;

        private int _siteId;
        private int _contentId;
        private string _returnUrl;
        private ConfigInfo _configInfo;

        public string GetContentPhotoUploadMultipleUrl()
        {
            return string.Empty;
            //return AjaxUploadService.GetContentPhotoUploadMultipleUrl(SiteId);
        }

        public string GetContentPhotoUploadSingleUrl()
        {
            return string.Empty;
            //return AjaxUploadService.GetContentPhotoUploadSingleUrl(SiteId);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Utils.ToInt(Request.QueryString["siteId"]);
            _contentId = Utils.ToInt(Request.QueryString["contentId"]);
            _returnUrl = Request.QueryString["returnUrl"];
            _configInfo = Main.Instance.GetConfigInfo(_siteId);

            if (IsPostBack) return;

            var photoInfoList = new List<PhotoInfo>();
            if (_contentId > 0)
            {
                photoInfoList = Main.PhotoDao.GetPhotoInfoList(_siteId, _contentId);
            }

            var scriptBuilder = new StringBuilder();

            foreach (var photoInfo in photoInfoList)
            {
                //var smallUrl = PageUtility.ParseNavigationUrl(PublishmentSystemInfo, photoInfo.SmallUrl, true);
                var smallUrl = string.Empty;
                scriptBuilder.Append($@"
add_form({photoInfo.Id}, '{Utils.ToJsString(smallUrl)}', '{Utils.ToJsString(photoInfo.SmallUrl)}', '{Utils.ToJsString(photoInfo.MiddleUrl)}', '{Utils.ToJsString(photoInfo.LargeUrl)}', '{Utils.ToJsString(photoInfo.Description)}');
");
            }

            LtlScript.Text = $@"
<script type=""text/javascript"">
$(document).ready(function(){{
	{scriptBuilder}
}});
</script>
";
        }

        public string GetPreviewImageSize()
        {
            return
                $@"max-width:{_configInfo.PhotoSmallWidth}; max-height={_configInfo.PhotoSmallWidth}";
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var contentIdList = new List<int>();
            if (_contentId > 0)
            {
                contentIdList = Main.PhotoDao.GetPhotoContentIdList(_siteId, _contentId);
            }
            var photos = Utils.ToInt(Request.Form["Photo_Count"]);
            if (photos > 0)
            {
                for (var index = 1; index <= photos; index++)
                {
                    var id = Utils.ToInt(Request.Form["ID_" + index]);
                    var smallUrl = Request.Form["SmallUrl_" + index];
                    var middleUrl = Request.Form["MiddleUrl_" + index];
                    var largeUrl = Request.Form["LargeUrl_" + index];
                    var description = Request.Form["Description_" + index];

                    if (!string.IsNullOrEmpty(smallUrl) && !string.IsNullOrEmpty(middleUrl) && !string.IsNullOrEmpty(largeUrl))
                    {
                        if (id > 0)
                        {
                            var photoInfo = Main.PhotoDao.GetPhotoInfo(id);
                            if (photoInfo != null)
                            {
                                photoInfo.SmallUrl = smallUrl;
                                photoInfo.MiddleUrl = middleUrl;
                                photoInfo.LargeUrl = largeUrl;
                                photoInfo.Description = description;

                                Main.PhotoDao.Update(photoInfo);
                            }
                            contentIdList.Remove(id);
                        }
                        else
                        {
                            var photoInfo = new PhotoInfo(0, _siteId, _contentId, smallUrl, middleUrl, largeUrl, 0, description);

                            Main.PhotoDao.Insert(photoInfo);
                        }
                    }
                }
            }

            if (contentIdList.Count > 0)
            {
                Main.PhotoDao.Delete(contentIdList);
            }

            HttpContext.Current.Response.Redirect(_returnUrl);
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect(_returnUrl);
        }
    }
}
