using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Photo.Core;
using SS.Photo.Model;

namespace SS.Photo.Pages
{
    public class PageSettings : Page
    {
        public Literal LtlMessage;
        public TextBox TbPhotoSmallWidth;
        public TextBox TbPhotoMiddleWidth;

        private int _siteId;
        private ConfigInfo _configInfo;

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);

            if (!Main.Request.AdminPermissions.HasSitePermissions(_siteId, Main.PluginId))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
                return;
            }

            _configInfo = Main.GetConfigInfo(_siteId);

            if (IsPostBack) return;

            TbPhotoSmallWidth.Text = _configInfo.PhotoSmallWidth.ToString();
            TbPhotoMiddleWidth.Text = _configInfo.PhotoMiddleWidth.ToString();
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            _configInfo.PhotoSmallWidth = Convert.ToInt32(TbPhotoSmallWidth.Text);
            _configInfo.PhotoMiddleWidth = Convert.ToInt32(TbPhotoMiddleWidth.Text);

            SiteServer.Plugin.Context.ConfigApi.SetConfig(Main.PluginId, _siteId, _configInfo);
            LtlMessage.Text = Utils.GetMessageHtml("多图内容设置修改成功！", true);
        }
    }
}