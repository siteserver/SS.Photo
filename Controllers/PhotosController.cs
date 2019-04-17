using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Photo.Core;

namespace SS.Photo.Controllers
{
    [RoutePrefix("photos")]
    public class PhotosController : ApiController
    {
        private const string Route = "";
        private const string RoutePhoto = "{photoId:int}";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                var channelId = request.GetQueryInt("channelId");
                var contentId = request.GetQueryInt("contentId");

                var repository = new PhotoRepository();
                var photos = repository.GetPhotoInfoList(siteId, channelId, contentId);

                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

                return Ok(new
                {
                    Value = photos,
                    AdminToken = adminToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();
                
                var channelId = request.GetQueryInt("channelId");
                var contentId = request.GetQueryInt("contentId");
                var fileName = request.GetQueryString("fileName");

                PhotoInfo photoInfo = null;

                var httpRequest = HttpContext.Current.Request;

                var fileCount = httpRequest.Files.Count;

                if (fileCount > 0)
                {
                    var file = httpRequest.Files[0];

                    //var fileName = Path.GetFileName(file.FileName);
                    //var path = context.Server.MapPath("~/upload/" + fileName);

                    if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                    var filePath = Context.SiteApi.GetUploadFilePath(siteId, fileName);
                    file.SaveAs(filePath);

                    photoInfo = InsertPhoto(filePath, siteId, channelId, contentId);
                }

                return Ok(photoInfo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static PhotoInfo InsertPhoto(string filePath, int siteId, int channelId, int contentId)
        {
            var configInfo = Main.GetConfigInfo(siteId);
            var largeUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);
            var smallUrl = largeUrl;
            var middleUrl = largeUrl;

            var srcImage = Image.FromFile(filePath);
            if (srcImage.Width > configInfo.PhotoSmallWidth)
            {
                smallUrl = Resize(siteId, filePath, srcImage, configInfo.PhotoSmallWidth);
            }
            if (srcImage.Width > configInfo.PhotoMiddleWidth)
            {
                middleUrl = Resize(siteId, filePath, srcImage, configInfo.PhotoMiddleWidth);
            }

            var repository = new PhotoRepository();

            var photoInfo = new PhotoInfo
            {
                SiteId = siteId,
                ChannelId = channelId,
                ContentId = contentId,
                SmallUrl = smallUrl,
                MiddleUrl = middleUrl,
                LargeUrl = largeUrl
            };
            photoInfo.Id = repository.Insert(photoInfo);
            return photoInfo;
        }

        private static string Resize(int siteId, string filePath, Image srcImage, int maxWidth)
        {
            var ratio = srcImage.Height / (double)srcImage.Width;
            var height = (int)(maxWidth * ratio);

            var newImage = new Bitmap(maxWidth, height);
            using (var gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gr.DrawImage(srcImage, new Rectangle(0, 0, maxWidth, height));
            }

            var resizeFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_{maxWidth}.png";
            var resizeFilePath = Context.SiteApi.GetUploadFilePath(siteId, resizeFileName);
            newImage.Save(resizeFilePath, ImageFormat.Png);

            return Context.SiteApi.GetSiteUrlByFilePath(resizeFilePath);
        }

        [HttpPut, Route(RoutePhoto)]
        public IHttpActionResult Update(int photoId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var repository = new PhotoRepository();

                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                var type = request.GetPostString("type");

                if (type == "description")
                {
                    var description = request.GetPostString("description");
                    repository.UpdateDescription(photoId, description);
                }
                else if (type == "taxis")
                {
                    var photoIds = request.GetPostObject<List<int>>("photoIds");
                    repository.UpdateTaxis(photoIds);
                }

                return Ok(new {});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RoutePhoto)]
        public IHttpActionResult Delete(int photoId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var repository = new PhotoRepository();

                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                repository.Delete(photoId);

                return Ok(new {});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
