using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Reporting;
using System.Collections.Generic;

public partial class Admin_Products_Assets_UploadAdditionalImage : CommerceBuilder.Web.UI.AbleCommerceAdminPage
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
        ValidFiles.Text = Store.GetCachedSettings().FileExt_Assets;
        if (string.IsNullOrEmpty(ValidFiles.Text)) ValidFiles.Text = "any";
        trSku.Visible = !string.IsNullOrEmpty(_Product.Sku);
        if (trSku.Visible)
        {
            Sku.Text = _Product.Sku;            
        }
        if (!Page.IsPostBack)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            CustomWidth.Text = settings.StandardImageWidth.ToString();
            CustomHeight.Text = settings.StandardImageHeight.ToString();
        }

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
                //save the image to do the resizing work
                if (!Resize.Checked)
                {
                    //JUST SAVE THE IMAGE AND ASSOCIATE WITH PRODUCT
                    string tempImagePath = FileHelper.BaseImagePath + safeFileName;
                    UploadedFile.SaveAs(tempImagePath);
                    ProductImage newImage = new ProductImage();
                    newImage.ProductId = _ProductId;
                    newImage.ImageUrl = FileHelper.BaseImageUrlPath + safeFileName;
                    newImage.Save();
                }
                else
                {
                    string tempImagePath = FileHelper.BaseImagePath + Guid.NewGuid().ToString("N") + ".jpg";
                    UploadedFile.SaveAs(tempImagePath);
                    if (FileHelper.IsImageFile(tempImagePath))
                    {
                        using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(tempImagePath))
                        {
                            FileHelper.WriteImageFile(originalImage, FileHelper.BaseImagePath + safeFileName, AlwaysConvert.ToInt(CustomWidth.Text), AlwaysConvert.ToInt(CustomHeight.Text), MaintainAspectRatio.Checked, AlwaysConvert.ToInt(Quality.Text));
                            ProductImage newImage = new ProductImage();
                            newImage.ProductId = _ProductId;
                            newImage.ImageUrl = FileHelper.BaseImageUrlPath + safeFileName;
                            newImage.Save();
                        }
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
                }
                if (success) Response.Redirect("AdditionalImages.aspx?ProductId=" + _ProductId.ToString());
            }
            else
            {
                CustomValidator filetype = new CustomValidator();
                filetype.IsValid = false;
                filetype.ControlToValidate = "BaseFileName";
                filetype.ErrorMessage = "'" + safeFileName + "' is not a valid file name.";
                filetype.Text = "*";
                phValidFiles.Controls.Add(filetype);
            }
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AdditionalImages.aspx?ProductId=" + _ProductId.ToString());
    }



}
