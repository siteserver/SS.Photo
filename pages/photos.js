config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');
config.channelId = utils.getQueryString('channelId');
config.contentId = utils.getQueryString('contentId');
config.returnUrl = utils.getQueryString('returnUrl');

var $api = new utils.Api('/ss.photo/photos');

var data = {
  pageAlert: null,
  pageLoad: false,
  pageType: 'list',
  photos: null,
  uploadUrl: null,
  op: '',
  photo: null,
  description: '',
  indexOld: 0,
  indexNew: 0,
  isUploadLoaded: false
};

var methods = {
  upload: function (photo) {
    this.photos.push(photo);
    this.pageType = 'list';
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

    utils.loading(true);
    $api.put({
      siteId: config.siteId,
      type: 'description',
      description: this.description
    }, function (err, res) {
      utils.loading(false);
      if (err || !res) return;

      $this.photo.description = description;
      $this.op = '';
      $this.photo = null;
    }, this.photo.id);
  },

  saveTaxis: function () {
    var $this = this;
    var indexOld = this.indexOld;
    var indexNew = this.indexNew;

    $this.op = '';

    if (indexOld === indexNew) return;

    $this.photos.splice(indexOld, 1);
    $this.photos.splice(indexNew, 0, $this.photo);

    var photoIds = [];
    for (var i = 0; i < $this.photos.length; i++) {
      photoIds.push($this.photos[i].id);
    }

    utils.loading(true);
    $api.put({
      siteId: config.siteId,
      type: 'taxis',
      photoIds: photoIds
    }, function (err, res) {
      utils.loading(false);
      if (err || !res) return;

      $this.op = '';
      $this.photo = null;
    }, this.photo.id);
  },

  del: function (photo) {
    var $this = this;

    utils.alertDelete({
      title: '删除图片',
      text: '此操作将删除图片，确认吗？',
      callback: function () {
        utils.loading(true);
        $api.delete({
          siteId: config.siteId
        }, function (err, res) {
          utils.loading(false);
          if (err || !res) return;

          $this.photos.splice($this.photos.indexOf(photo), 1);
        }, photo.id);
      }
    });
  },

  loadUpload: function () {
    var E = Q.event,
      Uploader = Q.Uploader;

    var boxDropArea = document.getElementById("drop-area");

    var uploader = new Uploader({
      url: this.uploadUrl,
      target: document.getElementById("drop-area"),
      allows: ".jpg,.jpeg,.png,.gif,.bmp,.webp",
      on: {
        add: function (task) {
          if (task.disabled) return alert("允许上传的文件格式为：" + this.ops.allows);
          utils.loading(true);
        },
        complete: function (task) {
          utils.loading(false);
          $vue.upload(task.json);
        }
      }
    });

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
  },

  btnPageTypeClick: function (pageType) {
    this.pageType = pageType;
    if (this.pageType == 'upload' && !this.isUploadLoaded) {
      this.isUploadLoaded = true;
      setTimeout(this.loadUpload, 200);
    }
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;

    $api.get({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.photos = res.value;
      $this.uploadUrl = config.apiUrl + '/ss.photo/photos?adminToken=' + res.adminToken + '&siteId=' + config.siteId + '&channelId=' + config.channelId + '&contentId=' + config.contentId;
      $this.pageLoad = true;
    });
  }
});