using System;
using System.Web;
using System.IO;
using SS.Photo.Core;
using SS.Photo.Model;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SS.Photo.Pages
{
    public class HandlerUpload : IHttpHandler
    {
        public static string GetRedirectUrl(int siteId, int channelId, int contentId)
        {
            return
                Main.Instance.PluginApi.GetPluginUrl(
                    $"{nameof(HandlerUpload)}.ashx?siteId={siteId}&channelId={channelId}&contentId={contentId}");
        }

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;

            var siteId = Utils.ToInt(request["siteId"]);
            var channelId = Utils.ToInt(request["channelId"]);
            var contentId = Utils.ToInt(request["contentId"]);
            var action = request["action"];
            var hash = request["hash"];
            var fileName = request["fileName"];

            PhotoInfo photoInfo = null;

            var fileCount = request.Files.Count;

            if (string.IsNullOrEmpty(hash))
            {
                //普通上传
                if (fileCount > 0)
                {
                    var file = request.Files[0];

                    //var fileName = Path.GetFileName(file.FileName);
                    //var path = context.Server.MapPath("~/upload/" + fileName);

                    if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                    var filePath = Main.Instance.FilesApi.GetUploadFilePath(siteId, fileName);
                    file.SaveAs(filePath);

                    photoInfo = InsertPhoto(filePath, siteId, channelId, contentId);
                }
            }
            else
            {
                //秒传或断点续传
                //var path = context.Server.MapPath("~/upload/" + hash);
                var path = Main.Instance.FilesApi.GetUploadFilePath(siteId, hash);
                var pathOk = path + Path.GetExtension(fileName);

                //状态查询
                if (action == "query")
                {
                    if (File.Exists(pathOk))
                    {
                        photoInfo = InsertPhoto(pathOk, siteId, channelId, contentId);

                        Finish(GetResponseJson(photoInfo));
                    }
                    else if (File.Exists(path))
                    {
                        Finish(new FileInfo(path).Length.ToString());
                    }
                    else
                    {
                        Finish("0");
                    }
                }
                else
                {
                    if (fileCount > 0)
                    {
                        var file = request.Files[0];
                        using (var fs = File.Open(path, FileMode.Append))
                        {
                            byte[] buffer = new byte[file.ContentLength];
                            file.InputStream.Read(buffer, 0, file.ContentLength);

                            fs.Write(buffer, 0, buffer.Length);
                        }
                    }

                    var isOk = request["ok"] == "1";
                    if (!isOk) Finish("1");

                    if (File.Exists(path)) File.Move(path, pathOk);
                }
            }

            Finish(GetResponseJson(photoInfo));
        }

        private static PhotoInfo InsertPhoto(string filePath, int siteId, int channelId, int contentId)
        {
            var configInfo = Main.Instance.GetConfigInfo(siteId);
            var largeUrl = Main.Instance.FilesApi.GetSiteUrlByFilePath(filePath);
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

            var photoInfo = new PhotoInfo(0, siteId, channelId, contentId, smallUrl, middleUrl, largeUrl, 0, string.Empty);
            photoInfo.Id = Main.PhotoDao.Insert(photoInfo);
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
            var resizeFilePath = Main.Instance.FilesApi.GetUploadFilePath(siteId, resizeFileName);
            newImage.Save(resizeFilePath, ImageFormat.Png);

            return Main.Instance.FilesApi.GetSiteUrlByFilePath(resizeFilePath);
        }

        /// <summary>
        /// 获取返回的json字符串
        /// </summary>
        /// <returns></returns>
        private static string GetResponseJson(PhotoInfo photoInfo)
        {
            if (photoInfo != null)
            {
                return Utils.JsonSerialize(new
                {
                    photoInfo.Id,
                    photoInfo.SiteId,
                    photoInfo.ContentId,
                    photoInfo.SmallUrl,
                    photoInfo.MiddleUrl,
                    photoInfo.LargeUrl,
                    photoInfo.Taxis,
                    photoInfo.Description,
                    ret = 1
                });
            }

            return Utils.JsonSerialize(new
            {
                time = DateTime.Now,
                ret = 0
            });
        }

        /// <summary>
        /// 完成上传
        /// </summary>
        /// <param name="json">回调函数参数</param>
        private static void Finish(string json)
        {
            var response = HttpContext.Current.Response;

            response.Write(json);
            response.End();
        }

        public bool IsReusable => false;
    }
}
