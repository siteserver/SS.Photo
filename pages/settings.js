config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');

var $api = new utils.Api('/ss.photo/settings');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  configInfo: null
};

var methods = {
  submit: function () {
    var $this = this;

    utils.loading(true);
    $api.post({
      siteId: config.siteId,
      photoSmallWidth: this.configInfo.photoSmallWidth,
      photoMiddleWidth: this.configInfo.photoMiddleWidth
    }, function (err, res) {
      utils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      alert({
        toast: true,
        type: 'success',
        title: "设置保存成功",
        showConfirmButton: false,
        timer: 2000
      });
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;

    $api.get({
      siteId: config.siteId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.configInfo = res.value;

      $this.pageLoad = true;
    });
  }
});