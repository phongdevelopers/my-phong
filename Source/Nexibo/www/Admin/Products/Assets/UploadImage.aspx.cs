using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

public partial class Admin_Products_Assets_UploadImage : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _ProductId;
    private Product _Product;

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) NavigationHelper.GetAdminUrl("Catalog/Browse.aspx");
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        ProductName.Text = _Product.Name;
        trSku.Visible = !string.IsNullOrEmpty(_Product.Sku);
        ValidFiles.Text = Store.GetCachedSettings().FileExt_Assets;
        if (string.IsNullOrEmpty(ValidFiles.Text)) ValidFiles.Text = "any";
        if (trSku.Visible)
        {
            Sku.Text = _Product.Sku;           
        }
        if (_Product.ImageUrl.Length > 0)
        {
            ImagePreview.ImageUrl = _Product.ImageUrl;
        }
        else
        {
            ImagePreview.Visible = false;
            ImagePreviewNoImage.Visible = true;
        }
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        UploadHelpText.Text = string.Format(UploadHelpText.Text, settings.StandardImageWidth, settings.StandardImageHeight);
        CancelButton.NavigateUrl += _ProductId;
        FileDataMaxSize.Text = String.Format(FileDataMaxSize.Text, Store.GetCachedSettings().MaxRequestLength);    
    }

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        if (UploadedFile.HasFile)
        {
            bool success = true;
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            if (!BaseFileName.Text.Contains(".")) BaseFileName.Text += System.IO.Path.GetExtension(UploadedFile.FileName);
            string safeFileName = string.Empty;
            safeFileName = FileHelper.GetSafeBaseImageName(BaseFileName.Text, true);
            if (!string.IsNullOrEmpty(safeFileName) && FileHelper.IsExtensionValid(safeFileName, Store.GetCachedSettings().FileExt_Assets))
            {
                try
                {
                    // TRY IF USER HAVE ACCESS RIGHTS ON THE DIRECTORY
                    string tempImagePath = FileHelper.BaseImagePath + Guid.NewGuid().ToString("N") + ".jpg";
                    UploadedFile.SaveAs(tempImagePath);
                    if (FileHelper.IsImageFile(tempImagePath))
                    {
                        using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(tempImagePath))
                        {
                            string extension = System.IO.Path.GetExtension(safeFileName);
                            int quality = AlwaysConvert.ToInt(Quality.Text);
                            //GENERATE ICON
                            FileHelper.WriteImageFile(originalImage, FileHelper.BaseImagePath + safeFileName.Replace(extension, "_i" + extension), settings.IconImageWidth, settings.IconImageHeight, true, 100);
                            if ((settings.ImageSkuLookupEnabled) && (BaseFileName.Text == _Product.Sku)) _Product.IconUrl = string.Empty;
                            else _Product.IconUrl = FileHelper.BaseImageUrlPath + safeFileName.Replace(extension, "_i" + extension);
                            //GENERATE THUMBNAIL
                            FileHelper.WriteImageFile(originalImage, FileHelper.BaseImagePath + safeFileName.Replace(extension, "_t" + extension), settings.ThumbnailImageWidth, settings.ThumbnailImageHeight, true, quality);
                            if ((settings.ImageSkuLookupEnabled) && (BaseFileName.Text == _Product.Sku)) _Product.ThumbnailUrl = string.Empty;
                            else _Product.ThumbnailUrl = FileHelper.BaseImageUrlPath + safeFileName.Replace(extension, "_t" + extension);
                            //GENERATE STANDARD IMAGE
                            FileHelper.WriteImageFile(originalImage, FileHelper.BaseImagePath + safeFileName, settings.StandardImageWidth, settings.StandardImageHeight, true, quality);
                            if ((settings.ImageSkuLookupEnabled) && (BaseFileName.Text == _Product.Sku)) _Product.ImageUrl = string.Empty;
                            else _Product.ImageUrl = FileHelper.BaseImageUrlPath + safeFileName;
                        }
                        _Product.Save();
                    }
                    else
                    {
                        success = false;
                        CustomValidator invalidFile = new CustomValidator();
                        invalidFile.Text = "*";
                        invalidFile.ErrorMessage = "You did not upload a valid file.";
                        invalidFile.IsValid = false;
                        phInvalidFile.Controls.Add(invalidFile);
                    }
                    try
                    {
                        if (System.IO.File.Exists(tempImagePath))
                        {
                            System.IO.File.Delete(tempImagePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("Could not delete temporary image file " + tempImagePath, ex);
                    }
                    if (success) Response.Redirect("Images.aspx?ProductId=" + _ProductId.ToString());
                }
                catch (UnauthorizedAccessException)
                {
                    CustomValidator UnauthorizedAccess = new CustomValidator();
                    UnauthorizedAccess.Text = "*";
                    UnauthorizedAccess.ErrorMessage = "Write access denied to path: \"Website\\Assets\\ProductImages\\\"";
                    UnauthorizedAccess.IsValid = false;
                    phInvalidFileName.Controls.Add(UnauthorizedAccess);

                }
            }
            else
            {
                CustomValidator filetype = new CustomValidator();
                filetype.IsValid = false;
                filetype.ControlToValidate = "BaseFileName";
                filetype.ErrorMessage = "The target file '" + safeFileName + "' does not have a valid file extension.";
                filetype.Text = "*";
                phValidFiles.Controls.Add(filetype);
            }
        }
    }

}
