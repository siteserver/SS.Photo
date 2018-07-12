<%@ Page Language="C#" Inherits="SS.Photo.Pages.PageUpload" %>
	<!DOCTYPE html>
	<html>

	<head>
		<meta charset="utf-8">
		<link href="assets/bootstrap/bootstrap.min.css" rel="stylesheet" type="text/css" />
		<link href="assets/siteserver/siteserver.min.css" rel="stylesheet" type="text/css" />
	</head>


	<body style="padding: 20px;">
		<form runat="server">

			<div class="card-box">
				<div class="row">
					<div class="col-lg-12">
						<h4 class="m-t-0 header-title">
							<b>上传内容图片</b>
						</h4>
						<p class="text-muted font-13 m-b-10">
							可选择多幅图片一次上传
						</p>
					</div>
				</div>

				<div id="drop-area" style="height: 200px; line-height: 200px; text-align: center; font-size: 18px; color: #777; border: 2px dashed #0000004d;
						background: #fff;	border-radius: 6px; cursor: pointer; margin-bottom: 20px">
					点击选择上传图片或者将图片拖拽到此区域
				</div>

				<div id="main" class="row">

					<div class="col-sm-4 col-lg-3 col-xs-12" v-for="(photo, index) in photos">

						<div class="card m-b-20">

							<a v-bind:href="photo.largeUrl" target="_blank">
								<img class="card-img-top img-fluid" v-bind:src="photo.middleUrl">
							</a>

							<div class="card-body">
								<p class="card-text" v-bind:style="{ display: photo.description ? '' : 'none' }" style="display: none">
									{{ photo.description }}
								</p>
								<a @click="describe(photo)" href="javascript:;" class="card-link text-success">
									{{ photo.description ? '修改说明' : '添加说明' }}
								</a>
								<a @click="order(photo, index)" href="javascript:;" class="card-link text-success">排 序</a>
								<a @click="del(photo)" href="javascript:;" class="card-link text-danger">删 除</a>
							</div>

						</div>

					</div>

					<div class="modal" tabindex="-1" role="dialog" v-bind:style="{ display: op === 'describe' && photo ? 'block' : 'none' }"
					  style="display: none;top: 100px;">
						<div class="modal-dialog" role="document">
							<div class="modal-content">
								<div class="modal-header">
									<h4 class="modal-title">图片说明</h4>
								</div>
								<div class="modal-body">
									<div class="input-group">
										<textarea class="form-control" style="height: 150px" v-model="description"></textarea>
									</div>
								</div>
								<div class="modal-footer">
									<button type="button" @click="saveDescription" class="btn btn-primary">保 存</button>
									<button type="button" @click="close" class="btn btn-secondary" data-dismiss="modal">取 消</button>
								</div>
							</div>
						</div>
					</div>
					<div class="modal" tabindex="-1" role="dialog" v-bind:style="{ display: op === 'order' && photo ? 'block' : 'none' }" style="display: none;top: 100px;">
						<div class="modal-dialog" role="document">
							<div class="modal-content">
								<div class="modal-header">
									<h4 class="modal-title">图片排序</h4>
								</div>
								<div class="modal-body">
									<div class="input-group">
										<select class="form-control" v-model="indexNew">
											<option disabled value="">请选择图片排序</option>
											<option v-for="(photo, index) in photos" v-bind:value="index">{{index + 1}}</option>
										</select>
									</div>
								</div>
								<div class="modal-footer">
									<button type="button" @click="saveTaxis" class="btn btn-primary">保 存</button>
									<button type="button" @click="close" class="btn btn-secondary" data-dismiss="modal">取 消</button>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-backdrop show fade" v-bind:style="{ display: op && photo ? 'block' : 'none' }"></div>

				</div>

				<hr />

				<div class="text-right">
					<input type="button" value="关 闭" onClick="window.parent.layer.closeAll()" class="btn" />
				</div>
			</div>

		</form>
	</body>

	</html>

	<script type="text/javascript" src="assets/axios/axios.min.js"></script>
	<script type="text/javascript" src="assets/vuejs/vue.min.js"></script>
	<script type="text/javascript" src="assets/sweetalert/sweetalert.min.js"></script>
	<script type="text/javascript" src="assets/web-uploader/js/Q.js"></script>
	<script type="text/javascript" src="assets/web-uploader/js/Q.Uploader.js"></script>
	<script type="text/javascript">
		var data = {
			op: '',
			photo: null,
			description: '',
			indexOld: 0,
			indexNew: 0,
			photos: <%=Photos%>
		};

		var $vue = new Vue({
			el: '#main',
			data: data,
			methods: {
				upload: function (photo) {
					this.photos.push(photo);
				},
				describe: function (photo) {
					this.op = 'describe';
					this.description = photo.description;
					this.photo = photo;
				},
				order: function (photo, index) {
					this.op = 'order';
					this.indexOld = this.indexNew = index;
					this.photo = photo;
				},
				close: function () {
					this.photo = null;
				},
				saveDescription: function () {
					var $this = this;
					var description = this.description;
					axios.post(location.href + '&isSaveDescription=True', {
							photoId: $this.photo.id,
							description: description
						})
						.then(function (response) {
							$this.photo.description = description;
							$this.op = '';
							$this.photo = null;
						})
						.catch(function (error) {
							console.log(error);
						});
				},
				saveTaxis: function () {
					var $this = this;
					var indexOld = this.indexOld;
					var indexNew = this.indexNew;

					$this.op = '';

					if (indexOld === indexNew) return;

					$this.photos.splice(indexOld, 1);
					$this.photos.splice(indexNew, 0, $this.photo);
					$this.photo = null;

					var photoIds = [];
					for (var i = 0; i < $this.photos.length; i++) {
						photoIds.push($this.photos[i].id);
					}

					axios.post(location.href + '&isSaveTaxis=True', {
							photoIds: photoIds
						})
						.then(function (response) {})
						.catch(function (error) {
							console.log(error);
						});
				},
				del: function (photo) {
					var $this = this;

					swal({
							title: '删除图片',
							text: '此操作将删除图片，确认吗？',
							icon: 'warning',
							buttons: {
								cancel: {
									text: '取 消',
									visible: true,
									className: 'btn'
								},
								confirm: {
									text: '删 除',
									visible: true,
									className: 'btn btn-danger'
								}
							}
						})
						.then(function (isConfirm) {
							if (isConfirm) {
								axios.post(location.href + '&isDelete=True', {
										photoId: photo.id
									})
									.then(function (response) {
										$this.photos.splice($this.photos.indexOf(photo), 1);
									})
									.catch(function (error) {
										console.log(error);
									});
							}
						});
				}
			}
		});

		var E = Q.event,
			Uploader = Q.Uploader;

		var boxDropArea = document.getElementById("drop-area");

		var uploader = new Uploader({
			url: '<%=UploadUrl%>',
			target: document.getElementById("drop-area"),
			allows: ".jpg,.jpeg,.png,.gif,.bmp",
			on: {
				add: function (task) {
					if (task.disabled) return alert("允许上传的文件格式为：" + this.ops.allows);
				},
				complete: function (task) {
					var json = task.json;
					if (!json || json.ret != 1) return alert("上传失败！");

					$vue.upload(json);
				}
			}
		});

		function set_drag_drop() {
			//若浏览器不支持html5上传，则禁止拖拽上传
			if (!Uploader.support.html5 || !uploader.html5) {
				boxDropArea.innerHTML = "点击选择上传图片";
				return;
			}

			//阻止浏览器默认拖放行为
			E.add(boxDropArea, "dragleave", E.stop);
			E.add(boxDropArea, "dragenter", E.stop);
			E.add(boxDropArea, "dragover", E.stop);

			E.add(boxDropArea, "drop", function (e) {
				E.stop(e);

				//获取文件对象
				var files = e.dataTransfer.files;

				uploader.addList(files);
			});
		}

		set_drag_drop();
	</script>