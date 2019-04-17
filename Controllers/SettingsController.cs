using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Photo.Core;

namespace SS.Photo.Controllers
{
    [RoutePrefix("settings")]
    public class SettingsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                var configInfo = Main.GetConfigInfo(siteId);

                return Ok(new
                {
                    Value = configInfo
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

                var siteId = request.GetPostInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                var configInfo = Main.GetConfigInfo(siteId);
                configInfo.PhotoSmallWidth = request.GetPostInt("photoSmallWidth");
                configInfo.PhotoMiddleWidth = request.GetPostInt("photoMiddleWidth");

                Context.ConfigApi.SetConfig(Utils.PluginId, siteId, configInfo);

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
