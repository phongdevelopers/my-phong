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
using CommerceBuilder.Web.UI;
using CommerceBuilder.DataFeeds;
using CommerceBuilder.Common;
using System.Collections.Generic;

public partial class Admin_Marketing_Feeds_YahooShopping : AbleCommerceAdminPage
{
    YahooShoppingFeed feedProvider = null;
    FeedOperationStatus feedOperationStatus = null;
    FeedStatusContainer StatusContainer = FeedStatusContainer.Instance;

    // StatusContainer LEVEL KEYS
    String KeyFeedStatus = "CommerceBuilder_YahooFeed_Store" + Token.Instance.StoreId + "_FeedStatus";
    String KeyFeedStatusMessageList = "CommerceBuilder_YahooFeed_Store" + Token.Instance.StoreId + "_MessageList";
    String KeyFeedStatusMessage = "CommerceBuilder_YahooFeed_Store" + Token.Instance.StoreId + "_Message";
    String KeyFeedStatusPercent = "CommerceBuilder_YahooFeed_Store" + Token.Instance.StoreId + "_Percent";
    String KeyFeedStatusSuccess = "CommerceBuilder_YahooFeed_Store" + Token.Instance.StoreId + "_Success";

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        if (feedOperationStatus == null)
        {
            // feed operation finished or timer is running invalid
            Timer1.Enabled = false;
        }

        try
        {
            UpdateStatusDisplay();
        }
        catch (Exception exp)
        {
            Timer1.Enabled = false;
            StatusContainer.Lock();
            StatusContainer.Remove(KeyFeedStatus);
            StatusContainer.UnLock();

            FeedCreationProgressPanel.Visible = false;
            List<string> errorMessages = new List<string>();
            if (StatusContainer[KeyFeedStatusMessageList] != null)
            {
                errorMessages = StatusContainer[KeyFeedStatusMessageList] as List<string>;
            }
            errorMessages.Add("An unexpected error occurred. " + exp.Message);
            UpdateMessagePanel(false, errorMessages);
        }
    }

    protected void SetApplicationVariable(String key, object value)
    {
        StatusContainer.Lock();
        StatusContainer.Add(key, value);
        StatusContainer.UnLock();
    }


    protected void UpdateStatusDisplay()
    {
        if (feedOperationStatus != null)
        {
            ProgressBar1.Value = feedOperationStatus.Percent;
            ProgressBar1.ProgressText = feedOperationStatus.StatusMessage;
        }
        else
        {
            ProgressBar1.Value = (int)StatusContainer[KeyFeedStatusPercent];
            ProgressBar1.ProgressText = (string)StatusContainer[KeyFeedStatusMessage];
            bool success = (bool)StatusContainer[KeyFeedStatusSuccess];

            UpdateMessagePanel(success, (List<string>)StatusContainer[KeyFeedStatusMessageList]);

            FeedCreationProgressPanel.Visible = false;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            FeedOptions options = new FeedOptions();
            options.Load(new YahooFeedOptionKeys());

            if (!string.IsNullOrEmpty(options.FeedFileName))
            {
                FeedFileName.Text = options.FeedFileName;
            }
            else
            {
                FeedFileName.Text = "YahooFeedData.txt";
            }

            if (options.OverwriteFeedFile)
            {
                AllowOverwrite.SelectedIndex = 1;
            }
            else
            {
                AllowOverwrite.SelectedIndex = 0;
            }
            AllProducts.Checked = options.IncludeAllProducts;
            MarkedProducts.Checked = !options.IncludeAllProducts;

            if (!string.IsNullOrEmpty(options.CompressedFeedFileName))
            {
                CompressedFeedFileName.Text = options.CompressedFeedFileName;
            }
            else
            {
                CompressedFeedFileName.Text = "YahooFeedData.txt.zip";
            }

            if (options.OverwriteCompressedFile)
            {
                AllowOverwriteCompressed.SelectedIndex = 1;
            }
            else
            {
                AllowOverwriteCompressed.SelectedIndex = 0;
            }

            FtpHost.Text = options.FtpHost;
            FtpPassword.Text = options.FtpPassword;
            FtpUser.Text = options.FtpUser;

            if (string.IsNullOrEmpty(options.RemoteFileName))
            {
                RemoteFileName.Text = "data.txt";
            }
            else
            {
                RemoteFileName.Text = options.RemoteFileName;
            }

        }

        MessagePanel.Visible = false;

        // initialize the feedOperationStatus from StatusContainer cache
        if (feedOperationStatus == null)
        {
            feedOperationStatus = StatusContainer[KeyFeedStatus] as FeedOperationStatus;

            // If new page is opened and feed operation in progress
            // then show only the progress panel
            if (!Page.IsPostBack && (feedOperationStatus != null || StatusContainer[KeyFeedStatusSuccess] != null))
            {
                FeedCreationProgressPanel.Visible = true;
                FeedInputPanel.Visible = false;
                if (feedOperationStatus != null) Timer1.Enabled = true;
                UpdateStatusDisplay();
            }
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        BindFeedActions();
    }

    protected void BindFeedActions()
    {
        FeedAction.Items.Clear();
        //ADD BLANK ITEM TO START
        FeedAction.Items.Add(new ListItem("", "NONE"));

        //ADD ACTIONS
        FeedAction.Items.Add(new ListItem("Create Feed", "CREATE"));
        FeedAction.Items.Add(new ListItem("Compress Feed", "COMPRESS"));
        FeedAction.Items.Add(new ListItem("Upload Uncompressed Feed", "UPLOAD_UNCOMPRESSED"));
        FeedAction.Items.Add(new ListItem("Upload Compressed Feed", "UPLOAD_COMPRESSED"));
        FeedAction.Items.Add(new ListItem("Create and Compress Feed", "CREATE_COMPRESS"));
        FeedAction.Items.Add(new ListItem("Create and Upload Feed", "CREATE_UPLOAD"));
        FeedAction.Items.Add(new ListItem("Create, Compress and Upload Feed", "CREATE_COMPRESS_UPLOAD"));
    }

    protected FeedOptions GetPostedOptions()
    {
        FeedOptions options = new FeedOptions();
        options.Load(new YahooFeedOptionKeys());
        options.CompressedFeedFileName = CompressedFeedFileName.Text;
        options.FeedFileName = FeedFileName.Text;
        options.FtpHost = FtpHost.Text;
        options.FtpPassword = FtpPassword.Text;
        options.FtpUser = FtpUser.Text;
        options.IncludeAllProducts = AllProducts.Checked;
        options.OverwriteCompressedFile = AllowOverwriteCompressed.SelectedIndex == 1;
        options.OverwriteFeedFile = AllowOverwrite.SelectedIndex == 1;
        options.RemoteFileName = RemoteFileName.Text;
        options.StoreId = Token.Instance.StoreId;
        return options;
    }

    protected void Create()
    {
        try
        {
            feedProvider = new YahooShoppingFeed();
            FeedOptions options = GetPostedOptions();

            // handle using asynchronous way
            if (feedOperationStatus == null)
            {
                feedOperationStatus = feedProvider.FeedOperationStatus;
                SetApplicationVariable(KeyFeedStatus, feedOperationStatus);
            }

            feedProvider.BeginCreateFeed(options, new AsyncCallback(this.EndAsynFeedOperationCallBack), "");
            feedOperationStatus.Messages.Add("Creating  feed File.");
            feedOperationStatus.StatusMessage = "Creating feed File.";
            feedOperationStatus.Percent = 0;
            ProgressBar1.ProgressText = "Creating feed File.";
            ProgressBar1.Value = 0;
            FeedCreationProgressPanel.Visible = true;
            FeedInputPanel.Visible = false;
            Timer1.Enabled = true;
        }
        catch (Exception exp)
        {
            ProgressBar1.ProgressText = "Error creating feed file.";
            feedOperationStatus.Messages.Add("Error occured : " + exp.ToString() + " " + exp.Message);
            ProgressBar1.Value = 100;
            Timer1.Enabled = false;
        }
    }
    private void EndAsynFeedOperationCallBack(IAsyncResult result)
    {
        feedProvider.EndFeedOperation(result);

        StatusContainer.Lock();
        StatusContainer.Remove(KeyFeedStatus);
        StatusContainer.UnLock();

        // just add this for next timer update
        SetApplicationVariable(KeyFeedStatusPercent, feedOperationStatus.Percent);
        SetApplicationVariable(KeyFeedStatusMessage, feedOperationStatus.StatusMessage);
        SetApplicationVariable(KeyFeedStatusMessageList, feedOperationStatus.Messages);
        SetApplicationVariable(KeyFeedStatusSuccess, feedOperationStatus.Success);

        UpdateMessagePanel(feedProvider.FeedOperationStatus.Success, feedOperationStatus.Messages);
        ProgressBar1.Value = 100;
        //ProgressBar1.ProgressText = "Operation Completed successfully.";

    }

    protected void Compress()
    {
        feedProvider = new YahooShoppingFeed();
        FeedOptions options = GetPostedOptions();

        bool success = feedProvider.CompressFeed(options);
        UpdateMessagePanel(feedProvider.FeedOperationStatus.Success, feedProvider.FeedOperationStatus.Messages);
    }

    protected void UploadUncompressed()
    {
        FeedOptions options = GetPostedOptions();
        feedProvider = new YahooShoppingFeed();
        bool success = feedProvider.UploadUncompressedFeed(options);
        UpdateMessagePanel(feedProvider.FeedOperationStatus.Success, feedProvider.FeedOperationStatus.Messages);
    }

    protected void UploadCompressed()
    {
        FeedOptions options = GetPostedOptions();
        feedProvider = new YahooShoppingFeed();
        bool success = feedProvider.UploadCompressedFeed(options);
        UpdateMessagePanel(feedProvider.FeedOperationStatus.Success, feedProvider.FeedOperationStatus.Messages);
    }

    protected void CreateCompress()
    {
        try
        {
            FeedOptions options = GetPostedOptions();
            feedProvider = new YahooShoppingFeed();

            // handle using asynchronous way
            if (feedOperationStatus == null)
            {
                feedOperationStatus = feedProvider.FeedOperationStatus;
                SetApplicationVariable(KeyFeedStatus, feedOperationStatus);
            }

            feedProvider.BeginCreateCompressFeed(options, new AsyncCallback(this.EndAsynFeedOperationCallBack), "");
            feedOperationStatus.Messages.Add("Creating feed File.");
            feedOperationStatus.StatusMessage = "Creating feed File.";
            feedOperationStatus.Percent = 0;
            ProgressBar1.ProgressText = "Creating feed File.";
            ProgressBar1.Value = 0;
            FeedCreationProgressPanel.Visible = true;
            FeedInputPanel.Visible = false;
            Timer1.Enabled = true;
        }
        catch (Exception exp)
        {
            ProgressBar1.ProgressText = "Error creating/compressing feed file.";
            feedOperationStatus.Messages.Add("Error occured : " + exp.ToString() + " " + exp.Message);
            ProgressBar1.Value = 100;
            Timer1.Enabled = false;
        }
    }

    protected void CreateUpload()
    {
        try
        {
            FeedOptions options = GetPostedOptions();
            feedProvider = new YahooShoppingFeed();

            // handle using asynchronous way
            if (feedOperationStatus == null)
            {
                feedOperationStatus = feedProvider.FeedOperationStatus;
                SetApplicationVariable(KeyFeedStatus, feedOperationStatus);
            }
            feedProvider.BeginCreateUploadFeed(options, new AsyncCallback(this.EndAsynFeedOperationCallBack), "");
            feedOperationStatus.Messages.Add("Creating  feed File.");
            feedOperationStatus.StatusMessage = "Creating feed File.";
            feedOperationStatus.Percent = 0;
            ProgressBar1.ProgressText = "Creating feed File.";
            ProgressBar1.Value = 0;
            FeedCreationProgressPanel.Visible = true;
            FeedInputPanel.Visible = false;
            Timer1.Enabled = true;
        }
        catch (Exception exp)
        {
            ProgressBar1.ProgressText = "Error creating and uploading feed file.";
            feedOperationStatus.Messages.Add("Error occured : " + exp.ToString() + " " + exp.Message);
            ProgressBar1.Value = 100;
            Timer1.Enabled = false;
        }
    }

    protected void CreateCompressUpload()
    {
        try
        {
            FeedOptions options = GetPostedOptions();
            List<string> messages = new List<string>();
            feedProvider = new YahooShoppingFeed();
            // handle using asynchronous way
            if (feedOperationStatus == null)
            {
                feedOperationStatus = feedProvider.FeedOperationStatus;
                SetApplicationVariable(KeyFeedStatus, feedOperationStatus);
            }

            feedProvider.BeginCreateCompressUploadFeed(options, new AsyncCallback(this.EndAsynFeedOperationCallBack), "");
            feedOperationStatus.Messages.Add("Creating  feed File.");
            feedOperationStatus.StatusMessage = "Creating feed File.";
            feedOperationStatus.Percent = 0;
            ProgressBar1.ProgressText = "Creating feed File.";
            ProgressBar1.Value = 0;
            FeedCreationProgressPanel.Visible = true;
            FeedInputPanel.Visible = false;
            Timer1.Enabled = true;
        }
        catch (Exception exp)
        {
            ProgressBar1.ProgressText = "Error creating, compressing uploading feed file.";
            feedOperationStatus.Messages.Add("Error occured : " + exp.ToString() + " " + exp.Message);
            ProgressBar1.Value = 100;
            Timer1.Enabled = false;
        }
    }

    protected void BtnSaveSettings_Click(object sender, EventArgs e)
    {
        FeedOptions options = GetPostedOptions();
        options.Save(new YahooFeedOptionKeys());
        List<string> messages = new List<string>();
        messages.Add("Settings Have Been Saved.");
        UpdateMessagePanel(true, messages);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        //redirect to dashboard?
        Response.Redirect("~/Admin/Default.aspx");
    }

    protected void UpdateMessagePanel(bool success, List<string> messages)
    {
        MessagePanel.Visible = true;
        SuccessMessageHeader.Visible = success;
        FailureMessageHeader.Visible = !success;
        PopulateMessages(messages);
    }

    protected void PopulateMessages(List<string> messages)
    {
        Messages.Items.Clear();
        if (messages == null) return;
        foreach (string message in messages)
        {
            Messages.Items.Add(message);
        }
    }

    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        FeedCreationProgressPanel.Visible = false;
        FeedInputPanel.Visible = true;
        MessagePanel.Visible = false;

        // REMOVE THE VALUES FROM StatusContainer VARIABLES
        StatusContainer.Lock();
        StatusContainer.Remove(KeyFeedStatusPercent);
        StatusContainer.Remove(KeyFeedStatusMessage);
        StatusContainer.Remove(KeyFeedStatusMessageList);
        StatusContainer.Remove(KeyFeedStatusSuccess);
        StatusContainer.UnLock();
    }

    protected void FeedActionButton_Click(object sender, ImageClickEventArgs e)
    {
        string action = FeedAction.SelectedValue;

        if (!string.IsNullOrEmpty(action))
        {
            switch (action)
            {
                case "CREATE":
                    Create();
                    break;
                case "COMPRESS":
                    Compress();
                    break;
                case "UPLOAD_UNCOMPRESSED":
                    UploadUncompressed();
                    break;
                case "UPLOAD_COMPRESSED":
                    UploadCompressed();
                    break;
                case "CREATE_COMPRESS":
                    CreateCompress();
                    break;
                case "CREATE_UPLOAD":
                    CreateUpload();
                    break;
                case "CREATE_COMPRESS_UPLOAD":
                    CreateCompressUpload();
                    break;
                default:
                    break;
            }
        }
    }
}
