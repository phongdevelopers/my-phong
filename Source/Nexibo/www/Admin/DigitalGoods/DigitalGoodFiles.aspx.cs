using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

public partial class Admin_DigitalGoods_DigitalGoodFiles : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private string _BaseDigitalGoodPath;
    private string _RelativeDigitalGoodPath = string.Empty;
    private string _DigitalGoodPath;
    List<FileDigitalGood> _FileList;

    private const string folderNameExpression = @"(?:[A-Za-z0-9]+)(?:[_\- \.][A-Za-z0-9]+)*";
    Regex relativePathRegex = new Regex(string.Format(@"^{0}\\(?:{0}\\)*$", folderNameExpression));
    Regex folderNameRegex = new Regex(string.Format("^{0}$", folderNameExpression));

    protected void Page_Init(object sender, EventArgs e)
    {
        // EXTRA MEASURE TO PROTECT AGAINST MISCONFIGURED SECURITY POLICY
        if (!Token.Instance.User.IsAdmin) NavigationHelper.Trigger403(Response, "Admin user rights required.");
        // INITIALIZE THE DIGITAL GOOD PATHS
        _BaseDigitalGoodPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\DigitalGoods");
        string testPath = Request.QueryString["Path"];
        if (!string.IsNullOrEmpty(testPath) && relativePathRegex.IsMatch(testPath))
        {
            _RelativeDigitalGoodPath = testPath;
        }
        _DigitalGoodPath = Path.Combine(_BaseDigitalGoodPath, _RelativeDigitalGoodPath);
        BuildPathBreadCrumbs();
        // INITIALIZE FILES
        UploadMaxSize.Text = String.Format(UploadMaxSize.Text, Store.GetCachedSettings().MaxRequestLength);
        UploadFileTypes.Text = Store.GetCachedSettings().FileExt_DigitalGoods;
        if (string.IsNullOrEmpty(UploadFileTypes.Text)) UploadFileTypes.Text = "<i>any file</i>";
        RenameFileExtensions.Text = UploadFileTypes.Text;
        BuildFileList();
        BindFileGrid();
    }

    private void BuildPathBreadCrumbs()
    {
        StringBuilder pathBreadCrumbs = new StringBuilder();
        pathBreadCrumbs.Append("<a href=\"DigitalGoodFiles.aspx\">Digital Goods</a>");
        if (!string.IsNullOrEmpty(_RelativeDigitalGoodPath))
        {
            string[] pathTokens = _RelativeDigitalGoodPath.Split("\\".ToCharArray());
            string thisPath = string.Empty;
            for (int i = 0; i < pathTokens.Length; i++)
            {
                if (!string.IsNullOrEmpty(pathTokens[i]))
                {
                    thisPath = thisPath + pathTokens[i] + "\\";
                    pathBreadCrumbs.Append(" &gt; <a href=\"DigitalGoodFiles.aspx?Path=" + HttpUtility.UrlEncode(thisPath) + "\">" + pathTokens[i] + "</a>");
                }
            }
        }
        FilePath.Text = pathBreadCrumbs.ToString();
    }

    private void BuildFileList()
    {
        // BUILD THE LIST OF FILES AVAILABLE
        _FileList = new List<FileDigitalGood>();
        if (!string.IsNullOrEmpty(_RelativeDigitalGoodPath))
        {
            string parentPath = _RelativeDigitalGoodPath.TrimEnd("\\".ToCharArray());
            int index = parentPath.LastIndexOf("\\");
            if (index > -1)
            {
                parentPath = "?Path=" + HttpUtility.UrlEncode(parentPath.Substring(0, index + 1));
            }
            else
            {
                parentPath = string.Empty;
            }
            _FileList.Add(new FileDigitalGood("<< Parent Folder", parentPath));
        }
        //IF CURRENT PATH DOESN'T EXIST THEN GO BACK TO ROOT
        if (!Directory.Exists(_DigitalGoodPath))
            Response.Redirect("DigitalGoodFiles.aspx");
        string[] dirs = Directory.GetDirectories(_DigitalGoodPath);
        foreach (string dirpath in dirs)
        {
            DirectoryInfo di = new DirectoryInfo(dirpath);
            if (!di.Name.StartsWith("."))
            {
                _FileList.Add(new FileDigitalGood(di.Name, "?Path=" + HttpUtility.UrlEncode(_RelativeDigitalGoodPath + di.Name + "\\")));
            }
        }
        string[] files = Directory.GetFiles(_DigitalGoodPath);
        foreach (string filepath in files)
        {
            FileInfo fi = new FileInfo(filepath);
            string fileSize = FileHelper.FormatFileSize(fi.Length).ToLowerInvariant();
            DigitalGoodCollection goods = GetFileDigitalGoods(fi.Name);
            _FileList.Add(new FileDigitalGood(fi.Name, fileSize, goods));
        }
    }

    private void BindFileGrid()
    {
        // BIND FILE LIST TO GRID
        FileGrid.DataSource = _FileList;
        FileGrid.DataBind();
    }

    private void RebindFileGrid()
    {
        BuildFileList();
        BindFileGrid();
    }

    private DigitalGoodCollection GetFileDigitalGoods(string fileName)
    {
        return DigitalGoodDataSource.LoadForCriteria("StoreId=" + Token.Instance.StoreId + " AND ServerFileName='" + StringHelper.SafeSqlString(_RelativeDigitalGoodPath + fileName) + "'");
    }

    protected string GetDownloadLink(string filename)
    {
        return this.Page.ResolveUrl("~/Admin/DigitalGoods/Download.ashx") + "?F=" + Server.UrlEncode(EncryptionHelper.EncryptAES(_RelativeDigitalGoodPath + filename));
    }

    protected void FileGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FileDigitalGood dataItem = e.Row.DataItem as FileDigitalGood;
            Repeater goodsList = (Repeater)e.Row.FindControl("DigitalGoodsList");
            if (dataItem != null && dataItem.DigitalGoods != null && goodsList != null)
            {
                goodsList.DataSource = dataItem.DigitalGoods;
                goodsList.DataBind();
            }
        }
    }

    protected void FileGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "AddDigitalGood":
                ShowAddForm(e.CommandArgument.ToString());
                break;
            case "DeleteFile":
                ShowDeleteForm(e.CommandArgument.ToString());
                break;
            case "RenameFile":
                ShowRenameForm(e.CommandArgument.ToString());
                break;
            case "DeleteDir":
                DeleteDir(e.CommandArgument.ToString());
                break;
        }
    }

    private void DeleteDir(string dirName)
    {
        string deletePath = Path.Combine(_DigitalGoodPath, dirName);
        if (Directory.Exists(deletePath))
        {
            try
            {
                Directory.Delete(deletePath, true);
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not delete digital goods folder " + deletePath, ex);
            }
        }
        RebindFileGrid();
    }

    private void ShowAddForm(string fileName)
    {
        FileInfo fi = new FileInfo(Path.Combine(_DigitalGoodPath, fileName));
        ServerFileName.Text = fileName;
        ServerFileSize.Text = FileHelper.FormatFileSize(fi.Length);
        Name.Text = Path.GetFileNameWithoutExtension(fileName);
        DownloadName.Text = fileName;
        ActivationMode.SelectedIndex = 2;
        AddPopup.Show();
    }

    private void ShowDeleteForm(string fileName)
    {
        FileInfo fi = new FileInfo(Path.Combine(_DigitalGoodPath, fileName));
        DeleteFileName.Text = fileName;
        DeleteFileSize.Text = FileHelper.FormatFileSize(fi.Length);
        DigitalGoodCollection goods = GetFileDigitalGoods(fi.Name);
        if (goods.Count > 0)
        {
            DeleteDigitalGoodsList.DataSource = goods;
            DeleteDigitalGoodsList.DataBind();
            // NEVER DEFAULT TO DELETE THE GOODS
            DeleteDigitalGoodsWithFile.Checked = false;
            DeleteGoodsMessage.Visible = true;
            trDeleteGoods.Visible = true;
        }
        else
        {
            DeleteGoodsMessage.Visible = false;
            trDeleteGoods.Visible = false;
        }
        DeletePopup.Show();
    }

    private void ShowRenameForm(string fileName)
    {
        FileInfo fi = new FileInfo(Path.Combine(_DigitalGoodPath, fileName));
        RenameDialogCaption.Text = string.Format(RenameDialogCaption.Text, fileName);
        RenameFileName.Text = fileName;
        RenameFileSize.Text = FileHelper.FormatFileSize(fi.Length);
        RenameNewFilename.Text = fileName;
        RenameOverwrite.Checked = false;
        DigitalGoodCollection goods = GetFileDigitalGoods(fi.Name);
        if (goods.Count > 0)
        {
            RenameDigitalGoodsList.DataSource = goods;
            RenameDigitalGoodsList.DataBind();
            // ALWAYS DEFAULT TO UPDATE THE GOODS
            UpdateDigitalGoodsOnRename.Checked = true;
            RenameGoodsMessage.Visible = true;
            trRenameGoods.Visible = true;
        }
        else
        {
            RenameGoodsMessage.Visible = false;
            trRenameGoods.Visible = false;
        }
        List<ListItem> folderList = GetFolderListItems(_BaseDigitalGoodPath, string.Empty, 0);
        folderList.Insert(0, new ListItem("(root)", string.Empty));
        RenameFileNewFolder.Items.Clear();
        RenameFileNewFolder.Items.AddRange(folderList.ToArray());
        ListItem selected = RenameFileNewFolder.Items.FindByValue(_RelativeDigitalGoodPath);
        if (selected != null) RenameFileNewFolder.SelectedIndex = RenameFileNewFolder.Items.IndexOf(selected);
        RenamePopup.Show();
    }

    protected bool DigitalGoodNameExists(String name)
    {
        DigitalGoodCollection dgc = DigitalGoodDataSource.LoadForCriteria("Name = '" + StringHelper.SafeSqlString(name) +  "'");
        if (dgc != null && dgc.Count > 0) return true;
        return false;
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (!DigitalGoodNameExists(Name.Text))
            {
                string fileName = Path.GetFileName(ServerFileName.Text);
                DigitalGood dg = new DigitalGood();
                dg.ServerFileName = _RelativeDigitalGoodPath + fileName;
                dg.Name = Name.Text;
                dg.FileSize = new FileInfo(Path.Combine(_DigitalGoodPath, fileName)).Length;
                dg.FileName = string.IsNullOrEmpty(DownloadName.Text) ? fileName : DownloadName.Text;
                dg.ActivationModeId = AlwaysConvert.ToByte(ActivationMode.SelectedValue);
                dg.MaxDownloads = AlwaysConvert.ToByte(MaxDownloads.Text);
                int tempDays = AlwaysConvert.ToInt(ActivationTimeoutDays.Text);
                int tempHours = AlwaysConvert.ToInt(ActivationTimeoutHours.Text);
                int tempMinutes = AlwaysConvert.ToInt(ActivationTimeoutMinutes.Text);
                if ((tempDays > 0) || (tempHours > 0) || (tempMinutes > 0)) dg.ActivationTimeout = string.Format("{0},{1},{2}", tempDays, tempHours, tempMinutes);
                else dg.ActivationTimeout = string.Empty;
                tempDays = AlwaysConvert.ToInt(DownloadTimeoutDays.Text);
                tempHours = AlwaysConvert.ToInt(DownloadTimeoutHours.Text);
                tempMinutes = AlwaysConvert.ToInt(DownloadTimeoutMinutes.Text);
                if ((tempDays > 0) || (tempHours > 0) || (tempMinutes > 0)) dg.DownloadTimeout = string.Format("{0},{1},{2}", tempDays, tempHours, tempMinutes);
                else dg.DownloadTimeout = string.Empty;
                dg.Save();

                // SEE WHETHER WE ARE SUPPOSED TO EDIT AFTER SAVING
                if (((WebControl)sender).ID == "AddAndEditButton")
                {
                    // REDIRECT TO THE EDIT PAGE
                    Response.Redirect("~/Admin/DigitalGoods/EditDigitalGood.aspx?DigitalGoodId=" + dg.DigitalGoodId);
                }

                // UPDATE THE FILE LIST
                int index = IndexOfFile(fileName);
                if (index > -1)
                {
                    FileDigitalGood fdg = _FileList[index];
                    if (fdg.DigitalGoods == null)
                    {
                        fdg.DigitalGoods = new DigitalGoodCollection();
                    }
                    fdg.DigitalGoods.Add(dg);
                }

                // REBIND THE FILE GRID
                BindFileGrid();
            }
            else
            {
                CustomValidator uniqueName = new CustomValidator();
                uniqueName.IsValid = false;
                uniqueName.ControlToValidate = "Name";
                uniqueName.ErrorMessage = "The specified name already exists.  You must use an alternate name.";
                uniqueName.Text = "*";
                uniqueName.ValidationGroup = "Add";
                phUniqueName.Controls.Add(uniqueName);
                AddPopup.Show();
            }
        }
        else AddPopup.Show();
    }

    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        string fileName = Path.GetFileName(DeleteFileName.Text);
        string filePath = Path.Combine(_DigitalGoodPath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            if (DeleteDigitalGoodsWithFile.Checked)
            {
                DigitalGoodCollection goods = GetFileDigitalGoods(fileName);
                goods.DeleteAll();
            }
        }
        // REMOVE THE FILE FROM THE LIST
        int index = IndexOfFile(fileName);
        if (index > -1)
        {
            _FileList.RemoveAt(index);
            // BIND FILE LIST TO THE GRID
            FileGrid.DataSource = _FileList;
            FileGrid.DataBind();
        }
    }

    private int IndexOfFile(string fileName)
    {
        int index = 0;
        foreach (FileDigitalGood fdg in _FileList)
        {
            if (fdg.FileName == fileName) return index;
            index++;
        }
        return -1;
    }

    private void SortFileList(SortDirection order)
    {
        if (order == SortDirection.Ascending)
        {
            _FileList.Sort(delegate(FileDigitalGood x, FileDigitalGood y)
            {
                return x.FileName.CompareTo(y.FileName);
            });
        }
        else
        {
            _FileList.Sort(delegate(FileDigitalGood x, FileDigitalGood y)
            {
                return y.FileName.CompareTo(x.FileName);
            });
        }
    }

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            //ADD IN THE BINARY FILE DATA
            HttpPostedFile file = null;
            if (Request.Files.Count > 0) file = Request.Files[0];
            bool fileUploaded = ((file != null) && (file.ContentLength > 0));
            if (fileUploaded)
            {
                string fileName = string.IsNullOrEmpty(UploadFileName.Text) ? Path.GetFileName(file.FileName) : UploadFileName.Text;
                string filePath = Path.Combine(_DigitalGoodPath, fileName);
                if (FileHelper.IsExtensionValid(fileName, Store.GetCachedSettings().FileExt_DigitalGoods))
                {
                    if (!File.Exists(filePath) || UploadOverwrite.Checked)
                    {
                        int fileLength = file.ContentLength;
                        Byte[] buffer = new byte[fileLength];
                        file.InputStream.Read(buffer, 0, fileLength);
                        File.WriteAllBytes(filePath, buffer);
                        //REDIRECT TO PREVENT REFRESH WARNING
                        string queryString = string.Empty;
                        if (!string.IsNullOrEmpty(_RelativeDigitalGoodPath))
                        {
                            queryString = "?Path=" + _RelativeDigitalGoodPath;
                        }
                        Response.Redirect("DigitalGoodFiles.aspx" + queryString);
                    }
                    else
                    {
                        CustomValidator overwrite = new CustomValidator();
                        overwrite.IsValid = false;
                        overwrite.ControlToValidate = "UploadFileName";
                        overwrite.ErrorMessage = "The target file '" + fileName + "' already exists.  Either allow overwrite or use an alternate file name.";
                        overwrite.Text = "*";
                        overwrite.ValidationGroup = "Upload";
                        phUploadOverwrite.Controls.Add(overwrite);
                    }
                }
                else
                {
                    CustomValidator filetype = new CustomValidator();
                    filetype.IsValid = false;
                    filetype.ControlToValidate = "UploadFileName";
                    filetype.ErrorMessage = "The target file '" + fileName + "' does not have a valid file extension.";
                    filetype.Text = "*";
                    filetype.ValidationGroup = "Upload";
                    phUploadFileTypes.Controls.Add(filetype);
                    UploadDialog.Attributes.Add("style", "display:block");
                }
            }
            UploadFileName.Text = string.Empty;
            UploadOverwrite.Checked = false;
        }
    }

    protected void RenameButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            string sourceFileName = Path.GetFileName(RenameFileName.Text);
            string sourcePath = Path.Combine(_DigitalGoodPath, sourceFileName);
            if (File.Exists(sourcePath))
            {
                // FIGURE OUT THE TARGET FILENAME
                string targetFileName = Path.GetFileName(RenameNewFilename.Text);
                string targetFolder = Path.Combine(_BaseDigitalGoodPath, RenameFileNewFolder.SelectedValue);
                string targetPath = Path.Combine(targetFolder, targetFileName);

                if (FileHelper.IsExtensionValid(targetFileName, Store.GetCachedSettings().FileExt_DigitalGoods))
                {
                    if (!File.Exists(targetPath) || RenameOverwrite.Checked)
                    {
                        // IF NECESSARY, REMOVE THE EXISTING TARGET FILE FROM DISK AND IN-MEMORY LIST
                        if (File.Exists(targetPath))
                        {
                            int targetIndex = IndexOfFile(targetFileName);
                            if (targetIndex > -1) _FileList.RemoveAt(targetIndex);
                            File.Delete(targetPath);
                        }
                        // MOVE THE FILE TO THE NEW NAME
                        File.Move(sourcePath, targetPath);
                        // UPDATE THE FILE IN THE LIST
                        int index = IndexOfFile(sourceFileName);
                        if (index > -1)
                        {
                            FileDigitalGood fdg = _FileList[index];
                            // SEE IF WE NEED TO UPDATE THE ASSOCIATED DIGITAL GOODS
                            if (UpdateDigitalGoodsOnRename.Checked)
                            {
                                DigitalGoodCollection goods = GetFileDigitalGoods(sourceFileName);
                                foreach (DigitalGood dg in goods)
                                {
                                    dg.ServerFileName = RenameFileNewFolder.SelectedValue + targetFileName;
                                    if (dg.FileName == sourceFileName) dg.FileName = targetFileName;
                                    dg.Save();
                                }
                            }
                            else fdg.DigitalGoods = null;
                            fdg.FileName = targetFileName;
                            RebindFileGrid();
                        }
                    }
                    else
                    {
                        CustomValidator overwrite = new CustomValidator();
                        overwrite.IsValid = false;
                        overwrite.ControlToValidate = "RenameNewFileName";
                        overwrite.ErrorMessage = "The target file '" + targetFileName + "' already exists.  Either allow overwrite or use an alternate file name.";
                        overwrite.Text = "*";
                        overwrite.ValidationGroup = "Rename";
                        phRenameOverwrite.Controls.Add(overwrite);
                        RenamePopup.Show();
                    }
                }
                else
                {
                    CustomValidator filetype = new CustomValidator();
                    filetype.IsValid = false;
                    filetype.ControlToValidate = "RenameNewFileName";
                    filetype.ErrorMessage = "The target file '" + targetFileName + "' does not have a valid file extension.";
                    filetype.Text = "*";
                    filetype.ValidationGroup = "Rename";
                    phRenameFileExtensions.Controls.Add(filetype);
                    RenamePopup.Show();
                }
            }
        }
        else RenamePopup.Show();
    }

    protected void NewFolderOkButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            string newFolderName = NewFolderName.Text.Trim();
            if (Regex.IsMatch(newFolderName, "^([A-Za-z0-9]+)([_\\- \\.][A-Za-z0-9]+)*$"))
            {
                if (newFolderName.Length <= 240)
                {
                    string newPath = Path.Combine(_DigitalGoodPath, newFolderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                        RebindFileGrid();
                        NewFolderName.Text = string.Empty;
                    }
                }
            }
        }else NewFolderPopupExtender.Show();
    }

    public List<ListItem> GetFolderListItems(string absolutePath, string relativePath, int level)
    {
        List<ListItem> items = new List<ListItem>();
        string[] dirs = Directory.GetDirectories(absolutePath);
        foreach (string dirpath in dirs)
        {
            DirectoryInfo di = new DirectoryInfo(dirpath);
            if (!di.Name.StartsWith("."))
            {
                string thisRelativePath = relativePath + di.Name + "\\";
                string padding = "-" + ((new string('-', level)).Replace("-", " -")) + " ";
                items.Add(new ListItem(padding + di.Name, thisRelativePath));
                List<ListItem> childItems = GetFolderListItems(dirpath, thisRelativePath, level + 1);
                if (childItems.Count > 0) items.AddRange(childItems);
            }
        }
        return items;
    }

    protected void ValidateFolderLength(object source, ServerValidateEventArgs args)
    {
        args.IsValid = args.Value.Length <= 240;
    }

    protected void ValidateDuplicateName(object source, ServerValidateEventArgs args)
    {

        string newPath = Path.Combine(_DigitalGoodPath, args.Value.Trim());
        args.IsValid = !Directory.Exists(newPath);
    }

    public class FileDigitalGood
    {
        private string _FileName;
        private bool _IsDirectory;
        private string _FileSize;
        private string _PathQueryString;
        private DigitalGoodCollection _DigitalGoods;

        public FileDigitalGood(string dirName, string pathQueryString)
        {
            _FileName = dirName;
            _PathQueryString = pathQueryString;
            _IsDirectory = true;
            _FileSize = string.Empty;
            _DigitalGoods = null;
        }

        public FileDigitalGood(string fileName, string fileSize, DigitalGoodCollection digitalGoods)
        {
            _FileName = fileName;
            _PathQueryString = string.Empty;
            _IsDirectory = false;
            _FileSize = fileSize;
            _DigitalGoods = digitalGoods;
        }

        public bool IsDirectory { get { return _IsDirectory; } }
        public bool IsChildDirectory { get { return _IsDirectory && _FileName != "<< Parent Folder"; } }
        public bool IsRootDirectory { get { return _IsDirectory && _FileName == "<< Parent Folder"; } }
        public string PathQueryString { get { return _PathQueryString; } }
        public bool IsFile { get { return !_IsDirectory; } }

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        public string FileSize
        {
            get { return _FileSize; }
            set { _FileSize = value; }
        }

        public DigitalGoodCollection DigitalGoods
        {
            get { return _DigitalGoods; }
            set { _DigitalGoods = value; }
        }
    }
}