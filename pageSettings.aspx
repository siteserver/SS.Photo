<%@ Page Language="C#" Inherits="SS.Photo.Pages.PageSettings" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/bootstrap/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/siteserver/siteserver.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body style="padding: 20px;">
    <form id="form" runat="server">

      <div class="card-box">
        <div class="row">
          <div class="col-lg-10">
            <h4 class="m-t-0 header-title">
              <b>多图内容设置</b>
            </h4>
            <p class="text-muted font-13 m-b-30">
              在此设置多图内容图片上传尺寸
            </p>
          </div>
        </div>

        <asp:Literal id="LtlMessage" runat="server" />

        <div class="form-group">
          <label class="col-sm-3 control-label">缩略图（小）最大宽度</label>
          <div class="col-sm-3">
            <div class="input-group">
              <asp:TextBox id="TbPhotoSmallWidth" class="form-control" runat="server"></asp:TextBox>
              <span class="input-group-addon">像素</span>
            </div>
          </div>
          <div class="col-sm-6">
            <asp:RequiredFieldValidator ControlToValidate="TbPhotoSmallWidth" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
            <asp:RegularExpressionValidator ControlToValidate="TbPhotoSmallWidth" runat="server" ValidationExpression="^[1-9]\d*(\.\d+)?$"
              ErrorMessage="请输入正确的数字" foreColor="red"></asp:RegularExpressionValidator>
          </div>
        </div>

        <div class="form-group">
          <label class="col-sm-3 control-label">缩略图（中）最大宽度</label>
          <div class="col-sm-3">
            <div class="input-group">
              <asp:TextBox id="TbPhotoMiddleWidth" class="form-control" runat="server"></asp:TextBox>
              <span class="input-group-addon">像素</span>
            </div>
          </div>
          <div class="col-sm-6">
            <asp:RequiredFieldValidator ControlToValidate="TbPhotoMiddleWidth" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
            <asp:RegularExpressionValidator ControlToValidate="TbPhotoMiddleWidth" runat="server" ValidationExpression="^[1-9]\d*(\.\d+)?$"
              ErrorMessage="请输入正确的数字" foreColor="red"></asp:RegularExpressionValidator>
          </div>
        </div>

        <hr />
        <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
      </div>

    </form>
  </body>

  </html>