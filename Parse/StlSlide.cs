using System.Text;
using SiteServer.Plugin;
using SS.Photo.Core;

namespace SS.Photo.Parse
{
    public class StlSlide
    {
        private StlSlide() { }
        public const string ElementName = "stl:slide";

        public static string Parse(IParseContext context)
        {
            var sGifUrl = Main.Instance.PluginApi.GetPluginUrl("assets/slide/s.gif");
            var jqueryUrl = Main.Instance.PluginApi.GetPluginUrl("assets/slide/js/jquery-1.9.1.min.js");
            var swfobjectUrl = Main.Instance.PluginApi.GetPluginUrl("assets/slide/js/swfobject.js");
            var fullScreenSwf = Main.Instance.PluginApi.GetPluginUrl("assets/slide/fullscreen.swf");
            var js = Main.Instance.PluginApi.GetPluginUrl("assets/slide/script.js");
            var css = Main.Instance.PluginApi.GetPluginUrl("assets/slide/style.css");

            var contentInfo = context.ContentInfo ?? Main.Instance.ContentApi.GetContentInfo(context.SiteId, context.ChannelId, context.ContentId);

            var photoInfoList = Main.PhotoDao.GetPhotoInfoList(context.SiteId, context.ChannelId, context.ContentId);

            var builder = new StringBuilder();

            builder.Append($@"
<link rel=""stylesheet"" type=""text/css"" href="""" />
<script type=""text/javascript"" src=""{jqueryUrl}""></script>
<script type=""text/javascript"" src=""{swfobjectUrl}""></script>
");

            builder.Append($@"
<script type=""text/javascript"">
var slideFullScreenUrl = ""{fullScreenSwf}"";
");

            builder.Append(@"
var slide_data = {
");

            builder.Append($@"
    ""slide"":{{""title"":""{Utils.ToJsString(contentInfo.Title)}""}},
    ""images"":[
");


            foreach (var photoInfo in photoInfoList)
            {
                var smallUrl = photoInfo.SmallUrl;
                var largeUrl = photoInfo.LargeUrl;

                builder.Append(
                    $@"{{""title"":""{Utils.ToJsString(contentInfo.Title)}"",""intro"":""{Utils.ToJsString(
                        photoInfo.Description)}"",""previewUrl"":""{Utils.ToJsString(smallUrl)}"",""imageUrl"":""{Utils
                        .ToJsString(largeUrl)}"",""id"":""{photoInfo.Id}""}},");
            }

            if (photoInfoList.Count > 0)
            {
                builder.Length -= 1;
            }

            builder.Append(@"
    ],
");

            var contentTableName = Main.Instance.ContentApi.GetTableName(context.SiteId, contentInfo.ChannelId);
            var siblingContentId = Main.PhotoDao.GetSiblingContentId(contentTableName, contentInfo.ChannelId,
                contentInfo.Taxis, true);

            if (siblingContentId > 0)
            {
                var title = Main.Instance.ContentApi.GetContentValue(context.SiteId, contentInfo.ChannelId,
                    siblingContentId, nameof(IContentInfo.Title));
                var url = Main.Instance.ContentApi.GetContentUrl(context.SiteId, contentInfo.ChannelId, siblingContentId);
                var photoInfo = Main.PhotoDao.GetFirstPhotoInfo(context.SiteId, contentInfo.ChannelId, siblingContentId);
                var previewUrl = photoInfo != null ? photoInfo.SmallUrl : sGifUrl;
                builder.Append($@"""next_album"":{{""title"":""{Utils.ToJsString(title)}"",""url"":""{Utils.ToJsString(url)}"",""previewUrl"":""{Utils
                        .ToJsString(previewUrl)}""}},");
            }
            else
            {
                builder.Append($@"""next_album"":{{""title"":"""",""url"":""javascript:void(0);"",""previewUrl"":""{sGifUrl}""}},");
            }

            //siblingContentId = BaiRongDataProvider.ContentDao.GetContentId(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentInfo.ChannelId, contentInfo.Taxis, false);
            siblingContentId = Main.PhotoDao.GetSiblingContentId(contentTableName, contentInfo.ChannelId,
                contentInfo.Taxis, false);

            if (siblingContentId > 0)
            {
                var title = Main.Instance.ContentApi.GetContentValue(context.SiteId, contentInfo.ChannelId,
                    siblingContentId, nameof(IContentInfo.Title));
                var url = Main.Instance.ContentApi.GetContentUrl(context.SiteId, contentInfo.ChannelId, siblingContentId);
                var photoInfo = Main.PhotoDao.GetFirstPhotoInfo(context.SiteId, contentInfo.ChannelId, siblingContentId);
                var previewUrl = photoInfo != null ? photoInfo.SmallUrl : sGifUrl;
                builder.Append($@"""prev_album"":{{""title"":""{Utils.ToJsString(title)}"",""url"":""{Utils.ToJsString(url)}"",""previewUrl"":""{Utils.ToJsString(previewUrl)}""}}");
            }
            else
            {
                builder.Append($@"""prev_album"":{{""title"":"""",""url"":""javascript:void(0);"",""previewUrl"":""{sGifUrl}""}}");
            }

            builder.Append(@"
}
</script>
");

            builder.Append($@"
<link href=""{css}"" rel=""stylesheet"" />
<script src=""{js}"" type=""text/javascript""></script>
");

            builder.Append(@"
<div id=""wrap"">
 
<div class=""eTitle"">
	<div class=""h1title""><span id=""d_picTit""></span><span id=""total"">(<span class=""cC00"">0</span>/0)</span></div>
</div>
 
<div class=""clearfix"">
<div class=""eControl"">
 
	<div class=""ecCont"">
		<div id=""ecbFullScreen"" title=""点击全屏获得更好观看效果""><div class=""buttonCont"" id=""fullScreenFlash""></div></div>
		<div id=""ecbSpeed""><div id=""ecbSpeedInfo"" class=""buttonCont"">5秒</div></div>
		<div id=""ecbPre"" title=""上一张""><div class=""buttonCont""></div></div>
		<div id=""ecbPlay"">
			<div id=""ecpPlayStatus"" class=""play""></div>
		</div>
		<div id=""ecbNext"" title=""下一张""><div class=""buttonCont""></div></div>
		<div id=""ecbLine""><div class=""buttonCont""></div></div>
 
		<div id=""ecbMode"" title=""列表模式(tab)""><div class=""buttonCont""></div></div>
		<div id=""ecbModeReturn"" title=""返回幻灯模式(tab)""><div class=""buttonCont""></div></div>
		
		<!-- 速度条 begin -->
		<div id=""SpeedBox"">
			<div id=""SpeedCont"">
				<div id=""SpeedSlide""></div>
				<div id=""SpeedNonius""></div>
 
			</div>
		</div>
		<!-- 速度条 end -->
	</div>
</div>
</div>
<!-- 2010/12/2 end -->
 
<div id=""eFramePic"">
	<div id=""efpBigPic"">
		<div id=""efpClew""></div>
 
		<div id=""d_BigPic""></div>
		<div id=""efpLeftArea"" class=""arrLeft"" title=""上一张""></div>
		<div id=""efpRightArea"" class=""arrRight"" title=""下一张""></div>
		<!-- endSelect begin -->
		<div id=""endSelect"">
			<div id=""endSelClose""></div>
			<div class=""bg""></div>
			<div class=""E_Cont"">
	
				<p>您已经浏览完所有图片</p>
 
				<p><a href=""javascript:void(0)"" id=""rePlayBut""></a><a href=""javascript:void(0)"" id=""nextPicsBut""></a></p>
			</div>
		</div>
		<!-- endSelect end -->
	</div>
 
	<div class=""others""	>
		<div class=""tblog_bg"">
        		<span><a href=""javascript:void(0)"" onclick=""downloadPic()"">下载</a> | </span><a href=""javascript:window.print()"">打印</a>
		</div>
		
	</div>
	
	<div id=""efpTxt"">
		<div id=""d_picTime""></div>
		<div id=""d_picIntro""></div>
	</div>
    
    
    <div id=""efpPicList"">
      <div id=""efpPreGroup"">
        <div id=""efpPrePic"" onmouseover=""this.className='selected'"" onmouseout=""this.className=''"">
 
          <table cellspacing=""0"">
            <tr>
              <td><a href=""""><img src=""{sGifUrl}"" width=""50"" height=""33"" alt="""" title="""" /></a></td>
            </tr>
          </table>
        </div>
        <div id=""efpPreTxt""><a href="""" title="""">&lt;&lt;上一图集</a></div>
      </div>
 
      <div id=""efpListLeftArr"" onmouseover=""this.className='selected'"" onmouseout=""this.className=''""></div>
      <div id=""efpPicListCont""></div>
      <div id=""efpListRightArr"" onmouseover=""this.className='selected'"" onmouseout=""this.className=''""></div>
      <div id=""efpNextGroup"">
        <div id=""efpNextPic"" onmouseover=""this.className='selected'"" onmouseout=""this.className=''"">
          <table cellspacing=""0"">
            <tr>
              <td><a href=""""><img src=""{sGifUrl}"" width=""50"" height=""33"" alt="""" title="""" /></a></td>
            </tr>
 
          </table>
        </div>
        <div id=""efpNextTxt""><a href="""" title="""">下一图集&gt;&gt;</a></div>
      </div>
    </div>
</div>
<div id=""ePicList""></div>
<!--v3-->
</div>
<script language=""javascript"" type=""text/javascript""> 
<!--//--><![CDATA[//><!--
 
function echoFocus(){
	getData.fillData();
	window.scrollTo(0,40);
};
 
function fullFlash(txt,pic){	
	var fullScreen = new SWFObject(slideFullScreenUrl, ""fullScreenObj"", ""100%"", ""100%"", ""8"", ""#000000"");
	fullScreen.addParam(""quality"", ""best"");
	fullScreen.addParam(""wmode"", ""transparent"");
	fullScreen.addParam(""allowFullScreen"", ""true"");
	fullScreen.addParam(""allowScriptAccess"",""always"");
	fullScreen.addVariable(""mylinkpic"", pic);		//此处添加组图标题
	//此处添加图片文字标题，导语，分别用“|”，“#”分割
	fullScreen.addVariable(""mytxt"",txt);
	fullScreen.addVariable(""fulls_btnx"",""0"");
	fullScreen.addVariable(""fulls_btny"",""0"");
	fullScreen.addVariable(""fulls_btnalpha"",""0"")
	fullScreen.write(""fullScreenFlash"");

};
function getFullScreenFlash(){
	if(slide.isIE){
		return slide.$('fullScreenFlash').getElementsByTagName('object')[0];
	}else{
		return slide.$('fullScreenFlash').getElementsByTagName('embed')[0];
	};
};
function flash_to_js(name){
	name = new String(name);
	var status = name.split(""|"");
	epidiascope.speedBar.setGrade(status[1]);
	epidiascope.select(parseInt(status[0]));
};
function js_to_flash(){
	epidiascope.stop();
	return epidiascope.selectedIndex + ""|"" + epidiascope.speedBar.grade;
};
function errorFocus(str){//错误
	document.getElementById(""d_picTit"").innerHTML = str;
	document.getElementById(""d_BigPic"").innerHTML = str;
};
//下载图片
function downloadPic(){
	try{
		var src = epidiascope.filmstrips[epidiascope.selectedIndex].src;
		window.open(src, 'download');
	}catch(e){};
};
function next_jstoflash(){	
	return;
}
function pre_jstoflash(){
	return;
}

jQuery(document).ready(function(){
	echoFocus();
});
//--><!]]>
</script>
");
            builder.Replace("{sGifUrl}", sGifUrl);
            return builder.ToString();
        }
    }
}
