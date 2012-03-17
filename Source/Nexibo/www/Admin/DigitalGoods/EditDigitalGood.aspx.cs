using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Messaging;
using CommerceBuilder.Utility;
using CommerceBuilder.Stores;

public partial class Admin_DigitalGoods_EditDigitalGood : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private DigitalGood _DigitalGood;
    private int _DigitalGoodId;
    private int _CategoryId = 0;    
    private int _ProductId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        // EXTRA MEASURE TO PROTECT AGAINST MISCONFIGURED SECURITY POLICY
        if (!Token.Instance.User.IsAdmin) NavigationHelper.Trigger403(Response, "Admin user rights required.");
        // INITIALIZE PAGE ELEMENTS
        InitializeForm(false);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Caption.Text = string.Format(Caption.Text, _DigitalGood.Name);
        ServerFileName.Text = _DigitalGood.ServerFileName;
        RenameFileName.Text = _DigitalGood.ServerFileName;
        CurrentFileSize.Text = _DigitalGood.FormattedFileSize;
        CurrentFileDownloadLink.NavigateUrl = string.Format(CurrentFileDownloadLink.NavigateUrl, _DigitalGoodId);
        bool dgFileExists = DGFileExists(_DigitalGood);
        CurrentFileDownloadLink.Visible = dgFileExists;
        MissingDownloadText.Visible = !dgFileExists;
    }

    private void InitializeForm(bool forceRefresh)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _DigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["DigitalGoodId"]);
        _DigitalGood = DigitalGoodDataSource.Load(_DigitalGoodId);
        if (_DigitalGood == null) Response.Redirect(CancelButton.NavigateUrl);
        RenameFileExtensions.Text = Store.GetCachedSettings().FileExt_DigitalGoods;
        if (!Page.IsPostBack || forceRefresh)
        {
            Name.Text = _DigitalGood.Name;
            FileName.Text = _DigitalGood.FileName;
            MediaKey.Text = _DigitalGood.MediaKey;
            ListItem item = ActivationMode.Items.FindByValue(_DigitalGood.ActivationModeId.ToString());
            if (item != null) item.Selected = true;
            if (_DigitalGood.MaxDownloads > 0) MaxDownloads.Text = _DigitalGood.MaxDownloads.ToString();
            int days, hours, minutes;
            DigitalGood.ParseTimeout(_DigitalGood.ActivationTimeout, out days, out hours, out minutes);
            if (days > 0) ActivationTimeoutDays.Text = days.ToString();
            if (hours > 0) ActivationTimeoutHours.Text = hours.ToString();
            if (minutes > 0) ActivationTimeoutMinutes.Text = minutes.ToString();
            DigitalGood.ParseTimeout(_DigitalGood.DownloadTimeout, out days, out hours, out minutes);
            if (days > 0) DownloadTimeoutDays.Text = days.ToString();
            if (hours > 0) DownloadTimeoutHours.Text = hours.ToString();
            if (minutes > 0) DownloadTimeoutMinutes.Text = minutes.ToString();
            //bind license agreements
            LicenseAgreement.DataSource = LicenseAgreementDataSource.LoadForStore();
            LicenseAgreement.DataBind();
            item = LicenseAgreement.Items.FindByValue(_DigitalGood.LicenseAgreementId.ToString());
            if (item != null) LicenseAgreement.SelectedIndex = LicenseAgreement.Items.IndexOf(item);
            item = LicenseAgreementMode.Items.FindByValue(_DigitalGood.LicenseAgreementModeId.ToString());
            if (item != null) LicenseAgreementMode.SelectedIndex = LicenseAgreementMode.Items.IndexOf(item);
            //bind readmes
            Readme.DataSource = ReadmeDataSource.LoadForStore();
            Readme.DataBind();
            item = Readme.Items.FindByValue(_DigitalGood.ReadmeId.ToString());
            if (item != null) Readme.SelectedIndex = Readme.Items.IndexOf(item);
            EnableSerialKeys.Checked = _DigitalGood.EnableSerialKeys;
            //INITIALIZE SERIAL KEY PROVIDERS
            KeySource.Items.Clear();
            KeySource.Items.Add(new ListItem("Manual Entry", "0"));
            //ADD PROVIDERS
            List<ISerialKeyProvider> providers = SerialKeyProviderDataSource.GetSerialKeyProviders();
            foreach (ISerialKeyProvider provider in providers)
            {
                string classId = Misc.GetClassId(provider.GetType());
                ListItem providerItem;
                // (BUG # 8347) ABLECOMMERCE DEFAULT KEY PROVIDER SHOULD BE NAMED AS "Managed List of Keys"
                if (provider.GetType() == typeof(DefaultSerialKeyProvider))
                {
                    providerItem = new ListItem("Managed List of Keys", classId);
                }
                else
                {                    
                    providerItem = new ListItem(provider.Name, classId);
                }
                KeySource.Items.Add(providerItem);
            }
            //SELECT CORRECT PROVIDER
            if (_DigitalGood.SerialKeyProviderId != null)
            {
                ListItem providerItem = KeySource.Items.FindByValue(_DigitalGood.SerialKeyProviderId);
                if (providerItem != null)
                    KeySource.SelectedIndex = KeySource.Items.IndexOf(providerItem);
            }

            EmailTemplateCollection templates = EmailTemplateDataSource.LoadForStore();
            foreach (EmailTemplate template in templates)
            {
                ListItem activationItem = new ListItem(template.Name, template.EmailTemplateId.ToString());
                ListItem fulfillItem = new ListItem(template.Name, template.EmailTemplateId.ToString());

                if (_DigitalGood.ActivationEmailId == template.EmailTemplateId)
                {
                    activationItem.Selected = true;
                }

                if (_DigitalGood.FulfillmentEmailId == template.EmailTemplateId)
                {
                    fulfillItem.Selected = true;
                }

                ActivationEmailTemplateList.Items.Add(activationItem);
                FulfillmentEmailTemplateList.Items.Add(fulfillItem);
            }

            if (_DigitalGood.FulfillmentMode == CommerceBuilder.DigitalDelivery.FulfillmentMode.Manual)
            {
                FulfillmentMode.SelectedIndex = 0;
            }
            else if (_DigitalGood.FulfillmentMode == CommerceBuilder.DigitalDelivery.FulfillmentMode.OnOrder)
            {
                FulfillmentMode.SelectedIndex = 1;
            }
            else
            {
                FulfillmentMode.SelectedIndex = 2;
            }

            ToggleConfigureProvider();

            if (_ProductId > 0)
            {
                CancelButton.NavigateUrl = string.Format("~/Admin/Products/DigitalGoods/DigitalGoods.aspx?CategoryId={0}&ProductId={1}", _CategoryId, _ProductId);
            }
        }
    }

    protected void ToggleConfigureProvider()
    {
        if (string.IsNullOrEmpty(KeySource.SelectedValue) ||
            KeySource.SelectedValue.Equals("0"))
        {
            ProviderConfigLink.Visible = false;
            ProviderConfigLink.NavigateUrl = "";
        }
        else
        {
            //show the configure link and fulfillment mode
            ProviderConfigLink.Visible = true;
			if(_ProductId > 0) {
				ProviderConfigLink.NavigateUrl = "~/Admin/Products/DigitalGoods/SerialKeyProviders/" + GetConfigUrl(KeySource.SelectedValue) 
                + "?CategoryId=" + _CategoryId + "&ProductId=" + _ProductId 
                + "&DigitalGoodId=" + _DigitalGoodId.ToString();
			}else {
				ProviderConfigLink.NavigateUrl = "~/Admin/Products/DigitalGoods/SerialKeyProviders/" + GetConfigUrl(KeySource.SelectedValue)
                + "?DigitalGoodId=" + _DigitalGoodId.ToString();
			}
        }
    }

    protected void ToggleConfigureProvider(object sender, EventArgs e)
    {
        ToggleConfigureProvider();
    }

    protected string GetConfigUrl(string classId)
    {
        ISerialKeyProvider instance = Activator.CreateInstance(Type.GetType(classId)) as ISerialKeyProvider;
        return instance.GetConfigUrl(Page.ClientScript);
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        if (Save())
        {
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
            SavedMessage.Visible = true;
        }
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        if (Save())
        {
            Response.Redirect(CancelButton.NavigateUrl);
        }
    }

    private bool Save()
    {
        if (!Page.IsValid) return false;
        _DigitalGood.Name = Name.Text;
        _DigitalGood.FileName = FileName.Text;
        _DigitalGood.MediaKey = MediaKey.Text;
        _DigitalGood.ActivationModeId = AlwaysConvert.ToByte(ActivationMode.SelectedValue);
        _DigitalGood.MaxDownloads = AlwaysConvert.ToByte(MaxDownloads.Text);
        int tempDays = AlwaysConvert.ToInt(ActivationTimeoutDays.Text);
        int tempHours = AlwaysConvert.ToInt(ActivationTimeoutHours.Text);
        int tempMinutes = AlwaysConvert.ToInt(ActivationTimeoutMinutes.Text);
        if ((tempDays > 0) || (tempHours > 0) || (tempMinutes > 0)) _DigitalGood.ActivationTimeout = string.Format("{0},{1},{2}", tempDays, tempHours, tempMinutes);
        else _DigitalGood.ActivationTimeout = string.Empty;
        tempDays = AlwaysConvert.ToInt(DownloadTimeoutDays.Text);
        tempHours = AlwaysConvert.ToInt(DownloadTimeoutHours.Text);
        tempMinutes = AlwaysConvert.ToInt(DownloadTimeoutMinutes.Text);
        if ((tempDays > 0) || (tempHours > 0) || (tempMinutes > 0)) _DigitalGood.DownloadTimeout = string.Format("{0},{1},{2}", tempDays, tempHours, tempMinutes);
        else _DigitalGood.DownloadTimeout = string.Empty;

        //VALIDATE THE FILE SIE
        if (System.IO.File.Exists(_DigitalGood.AbsoluteFilePath))
        {
            //READ THE EXISTING FILE SIZE
            System.IO.FileInfo fi = new System.IO.FileInfo(_DigitalGood.AbsoluteFilePath);
            _DigitalGood.FileSize = fi.Length;
        }

        //CHECK THE README TEXT
        _DigitalGood.LicenseAgreementId = AlwaysConvert.ToInt(LicenseAgreement.SelectedValue);
        _DigitalGood.LicenseAgreementModeId = AlwaysConvert.ToByte(LicenseAgreementMode.SelectedValue);
        _DigitalGood.ReadmeId = AlwaysConvert.ToInt(Readme.SelectedValue);
        _DigitalGood.EnableSerialKeys = EnableSerialKeys.Checked;
        //Check serial key provider
        if (string.IsNullOrEmpty(KeySource.SelectedValue) ||
            KeySource.SelectedValue.Equals("0"))
        {
            _DigitalGood.SerialKeyProviderId = null;
        }
        else
        {
            _DigitalGood.SerialKeyProviderId = KeySource.SelectedValue;
        }

        //Check fulfillment mode
        if (FulfillmentMode.SelectedValue.Equals("0"))
        {
            _DigitalGood.FulfillmentMode = CommerceBuilder.DigitalDelivery.FulfillmentMode.Manual;
        }
        else if (FulfillmentMode.SelectedValue.Equals("1"))
        {
            _DigitalGood.FulfillmentMode = CommerceBuilder.DigitalDelivery.FulfillmentMode.OnOrder;
        }
        else
        {
            _DigitalGood.FulfillmentMode = CommerceBuilder.DigitalDelivery.FulfillmentMode.OnPaidOrder;
        }

        //Check Activation Email
        if (string.IsNullOrEmpty(ActivationEmailTemplateList.SelectedValue) ||
            ActivationEmailTemplateList.SelectedValue.Equals("0"))
        {
            _DigitalGood.ActivationEmailId = 0;
        }
        else
        {
            _DigitalGood.ActivationEmailId = AlwaysConvert.ToInt(ActivationEmailTemplateList.SelectedValue);
        }

        //Check Fulfillment Email
        if (string.IsNullOrEmpty(FulfillmentEmailTemplateList.SelectedValue) ||
            FulfillmentEmailTemplateList.SelectedValue.Equals("0"))
        {
            _DigitalGood.FulfillmentEmailId = 0;
        }
        else
        {
            _DigitalGood.FulfillmentEmailId = AlwaysConvert.ToInt(FulfillmentEmailTemplateList.SelectedValue);
        }

        _DigitalGood.Save();
        ToggleConfigureProvider();
        return true;
    }

    protected bool DGFileExists(object dataItem)
    {
        DigitalGood dg = (DigitalGood)dataItem;
        return System.IO.File.Exists(dg.AbsoluteFilePath);
    }

    protected void RenameButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            string fileName = RenameFileName.Text;
            if (FileHelper.IsExtensionValid(fileName, Store.GetCachedSettings().FileExt_DigitalGoods))
            {
                _DigitalGood.ServerFileName = fileName;
                //VALIDATE THE FILE SIE
                if (System.IO.File.Exists(_DigitalGood.AbsoluteFilePath))
                {
                    //READ THE EXISTING FILE SIZE
                    System.IO.FileInfo fi = new System.IO.FileInfo(_DigitalGood.AbsoluteFilePath);
                    _DigitalGood.FileSize = fi.Length;
                }
                _DigitalGood.Save();
                InitializeForm(true);
            }
            else
            {
                CustomValidator filetype = new CustomValidator();
                filetype.IsValid = false;
                filetype.ControlToValidate = "RenameFileName";
                filetype.ErrorMessage = "The target file '" + fileName + "' does not have a valid file extension.";
                filetype.Text = "*";
                filetype.ValidationGroup = "Rename";
                phRenameFileExtensions.Controls.Add(filetype);
                RenamePopup.Show();
            }
        }
    }
}
