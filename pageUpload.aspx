<%@ Page Language="C#" Inherits="SS.Photo.Pages.PageUpload" %>
	<!DOCTYPE html>
	<html>

	<head>
		<meta charset="utf-8">
		<link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
		<link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
		<link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
		<link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
	</head>


	<body style="padding: 20px;">
		<form runat="server">

			<div class="row">
				<div class="card-box">
					<div class="row">
						<div class="col-lg-12">
							<h4 class="m-t-0 header-title">
								<b>上传图片</b>
							</h4>
							<p class="text-muted font-13 m-b-30">
								在此设置多图内容图片上传尺寸
							</p>
						</div>
					</div>

					<div class="row">
						<div class="col-sm-12">
							<div class="card-box">
								<h4>
									<i class="fa fa-paperclip m-r-10 m-b-10"></i> Attachments
									<span>(3)</span>
								</h4>

								<div class="row">
									<div class="col-sm-2">
										<a href="#">
											<img src="assets/images/small/img1.jpg" alt="attachment" class="img-thumbnail img-responsive"> </a>
									</div>
									<div class="col-sm-2">
										<a href="#">
											<img src="assets/images/small/img2.jpg" alt="attachment" class="img-thumbnail img-responsive"> </a>
									</div>
									<div class="col-sm-2">
										<a href="#">
											<img src="assets/images/small/img3.jpg" alt="attachment" class="img-thumbnail img-responsive"> </a>
									</div>
									<div class="col-sm-6">
										<p>
											<b>Hi Bro...</b>
										</p>
										<p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis
											natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque
											eu, pretium quis, sem.</p>
										<p>Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo,
											rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt.
											Cras dapibus. Vivamus elementum semper nisi.</p>
										<p>Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam
											lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque
											rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui.
											Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem
											neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar,</p>
									</div>
								</div>
							</div>

						</div>
					</div>

				</div>
			</div>

			<script type="text/javascript" src="assets/js/ajaxupload.js"></script>
			<script type="text/javascript" src="assets/swfUpload/swfupload.js"></script>
			<script type="text/javascript" src="assets/swfUpload/handlers.js"></script>

			<script type="text/javascript">
				function uploadSuccess(file, response) {
					try {
						if (response) {
							response = eval("(" + response + ")");

							if (response.success == 'true') {
								add_form();
								var $count = $('#Photo_Count');
								var index = parseInt($count.val());
								$("#imgPhoto_" + index).attr('src', response.url);
								$("#SmallUrl_" + index).val(response.smallUrl);
								$("#MiddleUrl_" + index).val(response.middleUrl);
								$("#LargeUrl_" + index).val(response.largeUrl);
							} else {
								alert(response.message);
							}
						}
					} catch (ex) {
						this.debug(ex);
					}
				}

				var swfu;
				$(document).ready(function () {
					swfu = new SWFUpload({
						// Backend Settings
						upload_url: "<%=GetContentPhotoUploadMultipleUrl()%>",

						// File Upload Settings
						file_size_limit: "10 MB",
						file_types: "*.jpg;*.jpeg;*.gif;*.png",
						file_types_description: "Images",
						file_upload_limit: 0, // Zero means unlimited

						// Event Handler Settings - these functions as defined in Handlers.js
						//  The handlers are not part of SWFUpload but are part of my website and control how
						//  my website reacts to the SWFUpload events.
						swfupload_preload_handler: preLoad,
						swfupload_load_failed_handler: loadFailed,
						file_queue_error_handler: fileQueueError,
						file_dialog_complete_handler: fileDialogComplete,
						upload_error_handler: uploadError,
						upload_success_handler: uploadSuccess,
						upload_complete_handler: uploadComplete,

						// Button settings
						button_image_url: "assets/swfUpload/button.png",
						button_placeholder_id: "swfUploadPlaceholder",
						button_width: 114,
						button_height: 22,
						button_text: '» 批量添加图片',
						button_text_top_padding: 1,
						button_text_left_padding: 10,

						// Flash Settings
						flash_url: "assets/swfUpload/swfupload.swf", // Relative to this file
						flash9_url: "assets/swfUpload/swfupload_FP9.swf", // Relative to this file

						// Debug Settings
						debug: false
					});
				});
			</script>

			<div class="popover popover-static">
				<h3 class="popover-title">上传图片</h3>
				<div class="popover-content">

					<div id="contents">
						<table border=0 cellspacing=5 cellpadding=5 width="95%">
							<tr>
								<td colspan="2">
									<input id="Photo_Count" type="hidden" name="Photo_Count" value="0" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<table width="240" border="0" cellspacing="0" cellpadding="0">
										<tr>
											<td>
												<span id="swfUploadPlaceholder"></span>
											</td>
										</tr>
									</table>
								</td>
								<td align="right">&nbsp;</td>
							</tr>
						</table>
					</div>

					<hr />
					<table class="table noborder">
						<tr>
							<td class="center">
								<asp:Button class="btn btn-primary" id="Submit" OnClick="Submit_OnClick" Text="确 定" runat="server" />
								<asp:Button class="btn" id="Return" CausesValidation="false" OnClick="Return_OnClick" Text="返 回" runat="server" />
							</td>
						</tr>
					</table>

				</div>
			</div>

			<div id="Photo_0" style="display:none">
				<table class="table table-noborder">
					<tr>
						<td>
							<img id="imgPhoto_0" style="border: #ccc 1px solid; padding:1px;" src="assets/preview.gif" <%=GetPreviewImageSize() %> />
							<div>
								<a id="uploadFile_0" href="javascript:void(0);">» 上传</a>
							</div>
							<span id="img_upload_txt_0" style="clear:both; font-size:12px; color:#FF3737;"></span>
							<input type="hidden" id="ID_0" name="ID_0" value="" />
							<input type="hidden" id="SmallUrl_0" name="SmallUrl_0" value="" />
							<input type="hidden" id="MiddleUrl_0" name="MiddleUrl_0" value="" />
							<input type="hidden" id="LargeUrl_0" name="LargeUrl_0" value="" />
						</td>
						<td>
							<table cellpadding=5>
								<tr>
									<td>图片说明:</td>
									<td>
										<textarea id="Description_0" name="Description_0" style="width: 350px; height:66px;"></textarea>
									</td>
								</tr>
							</table>
						</td>
						<td style="vertical-align:bottom">
							<a href="javascript:;" onClick="remove_form('#Photo_0');">删除图片</a>
						</td>
					</tr>
				</table>

				<hr />
			</div>

			<script type="text/javascript">
				var ajaxUploadUrl = '<%=GetContentPhotoUploadSingleUrl()%>';

				function add_form(id, url, smallUrl, middleUrl, largeUrl, description) {
					var $count = $('#Photo_Count');
					var count = parseInt($count.val());
					count = count + 1;
					var $el = $("<div id='Photo_" + count + "'>" + $('#Photo_0').html().replace(/_0/g, '_' + count) + "</div>");
					$el.insertBefore($count);
					$('#Photo_Count').val(count);
					add_ajaxUpload(count);

					if (id && id > 0) {
						$('#ID_' + count).val(id);
						$('#imgPhoto_' + count).attr("src", url);
						$('#SmallUrl_' + count).val(smallUrl);
						$('#MiddleUrl_' + count).val(middleUrl);
						$('#LargeUrl_' + count).val(largeUrl);
						$('#Description_' + count).val(description);
					}
				}

				function remove_form(divID) {
					$(divID).remove();
				}

				function add_ajaxUpload(index) {
					new AjaxUpload('uploadFile_' + index, {
						action: ajaxUploadUrl,
						name: "ImageUrl",
						data: {},
						onSubmit: function (file, ext) {
							var reg = /^(jpg|jpeg|png|gif)$/i;
							if (ext && reg.test(ext)) {
								$('#img_upload_txt_' + index).text('上传中... ');
							} else {
								$('#img_upload_txt_' + index).text('只允许上传JPG,PNG,GIF图片');
								return false;
							}
						},
						onComplete: function (file, response) {
							$('#img_upload_txt_' + index).text(' ');
							if (response) {
								response = eval("(" + response + ")");
								if (response.success == 'true') {
									$("#imgPhoto_" + index).attr('src', response.url);
									$("#SmallUrl_" + index).val(response.smallUrl);
									$("#MiddleUrl_" + index).val(response.middleUrl);
									$("#LargeUrl_" + index).val(response.largeUrl);
								} else {
									$('#img_upload_txt_' + index).text(response.message);
								}
							}
						}
					});
				}
			</script>

			<asp:Literal ID="LtlScript" runat="server"></asp:Literal>

		</form>
	</body>

	</html>