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
using CommerceBuilder.Orders;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using System.IO;

public partial class Admin_DigitalGoods_DigitalGoods : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private Dictionary<int, int> _ProductCounts = new Dictionary<int, int>();

    protected void Page_Init(object sender, EventArgs e)
    {
        AlphabetRepeater.DataSource = GetAlphabetDS();
        AlphabetRepeater.DataBind();

        // EXTRA MEASURE TO PROTECT AGAINST MISCONFIGURED SECURITY POLICY
        if (!Token.Instance.User.IsAdmin) NavigationHelper.Trigger403(Response, "Admin user rights required.");
        // INITIALIZE PAGE ELEMENTS
        UploadMaxSize.Text = String.Format(UploadMaxSize.Text, Store.GetCachedSettings().MaxRequestLength);
        ActivationMode.SelectedIndex = 2;
        UploadFileTypes.Text = Store.GetCachedSettings().FileExt_DigitalGoods;
        //string setNamesScript = @"var lastName="""";function setNames(){var a=document.getElementById(""" + UploadFile.ClientID + @""").value;if(a.length>0){var b;var c=a.lastIndexOf(""\\"");if(c<0){c=a.lastIndexOf(""/"");if(c<0){b=a}else{b=a.substring(c+1)}}else{b=a.substring(c+1)}var d=document.getElementById(""" + DownloadName.ClientID + @""");if((d.value.length==0)||(d.value==lastName)){b=b.replace(/ /g,""_"");d.value=b;lastName=b}}}";
        //ScriptManager.RegisterClientScriptBlock(UploadFile, this.GetType(), "SetNames", setNamesScript, true); 
    }

    protected int GetProductCount(object dataItem)
    {
        DigitalGood m = (DigitalGood)dataItem;
        if (_ProductCounts.ContainsKey(m.DigitalGoodId)) return _ProductCounts[m.DigitalGoodId];
        int count = ProductDigitalGoodDataSource.CountForDigitalGood(m.DigitalGoodId);
        _ProductCounts[m.DigitalGoodId] = count;
        return count;
    }

    protected int GetOrderCount(object dataItem)
    {
        DigitalGood m = (DigitalGood)dataItem;
        return OrderItemDigitalGoodDataSource.CountForDigitalGood(m.DigitalGoodId);
    }

    protected bool HasProducts(object dataItem)
    {
        return (GetProductCount(dataItem) > 0);
    }

    protected void DigitalGoodGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Copy")
        {
            int dgid = AlwaysConvert.ToInt(e.CommandArgument);
            DigitalGood copy = DigitalGood.Copy(dgid);
            if (copy != null)
            {
                String newName = "Copy of " + copy.Name;
                if (newName.Length > 100)
                {
                    newName = newName.Substring(0, 97) + "...";
                }
                copy.Name = newName;
                copy.Save();
            }
            DigitalGoodGrid.DataBind();
        }
    }

    protected bool DGFileExists(object dataItem)
    {
        DigitalGood dg = (DigitalGood)dataItem;
        return File.Exists(dg.AbsoluteFilePath);
    }

    private bool DigitalGoodNameExists(String name)
    {
        DigitalGoodCollection dgc = DigitalGoodDataSource.LoadForCriteria("Name = '" + StringHelper.SafeSqlString(name) + "'");
        if (dgc != null && dgc.Count > 0) return true;
        return false;
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (!DigitalGoodNameExists(Name.Text))
            {
                // SET THE DIGITAL GOOD PATH
                string digitalGoodPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\DigitalGoods");

                // SAVE THE BINARY FILE DATA
                HttpPostedFile file = null;
                if (Request.Files.Count > 0) file = Request.Files[0];
                bool fileUploaded = ((file != null) && (file.ContentLength > 0));
                if (fileUploaded)
                {
                    string fileName = string.IsNullOrEmpty(UploadFileName.Text) ? Path.GetFileName(file.FileName) : UploadFileName.Text;
                    string filePath = Path.Combine(digitalGoodPath, fileName);
                    if (FileHelper.IsExtensionValid(fileName, Store.GetCachedSettings().FileExt_DigitalGoods))
                    {
                        if (!File.Exists(filePath) || UploadOverwrite.Checked)
                        {
                            int fileLength = file.ContentLength;
                            Byte[] buffer = new byte[fileLength];
                            file.InputStream.Read(buffer, 0, fileLength);
                            File.WriteAllBytes(filePath, buffer);
                            // RECORD THE DIGITAL GOOD
                            DigitalGood dg = new DigitalGood();
                            dg.ServerFileName = fileName;
                            dg.Name = Name.Text;
                            dg.FileSize = fileLength;
                            dg.FileName = fileName;
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
                            else
                            {
                                // REDIRECT BACK TO THIS PAGE TO PREVENT REFRESH ISSUES WITH FILE UPLOAD
                                Response.Redirect(Request.ServerVariables["SCRIPT_NAME"]);
                            }
                        }
                        else
                        {
                            CustomValidator overwrite = new CustomValidator();
                            overwrite.IsValid = false;
                            overwrite.ControlToValidate = "UploadFileName";
                            overwrite.ErrorMessage = "The target file '" + fileName + "' already exists.  Either allow overwrite or use an alternate file name.";
                            overwrite.Text = "*";
                            overwrite.ValidationGroup = "Add";
                            phUploadOverwrite.Controls.Add(overwrite);
                            AddPopup.Show();
                        }
                    }
                    else
                    {
                        CustomValidator filetype = new CustomValidator();
                        filetype.IsValid = false;
                        filetype.ControlToValidate = "UploadFileName";
                        filetype.ErrorMessage = "The target file '" + fileName + "' does not have a valid file extension.";
                        filetype.Text = "*";
                        filetype.ValidationGroup = "Add";
                        phUploadFileTypes.Controls.Add(filetype);
                        AddPopup.Show();
                    }
                }
                UploadFileName.Text = string.Empty;
                UploadOverwrite.Checked = false;
            }
            else
            {
                CustomValidator uniqueName = new CustomValidator();
                uniqueName.IsValid = false;
                uniqueName.ControlToValidate = "Name";
                uniqueName.ErrorMessage = "The specified display name already exists.  You must use an alternate name.";
                uniqueName.Text = "*";
                uniqueName.ValidationGroup = "Add";
                phUniqueName.Controls.Add(uniqueName);
                AddPopup.Show();
            }
        }
        else AddPopup.Show();
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {

    }

    protected void AlphabetRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if ((e.CommandArgument.ToString().Length == 1))
        {
            SearchDGName.Text = (e.CommandArgument.ToString() + "*");
        }
        else
        {
            SearchDGName.Text = String.Empty;
        }
        // CLEAR OUT OTHER CRITERIA
        SearchDGFileName.Text = string.Empty;
        DigitalGoodGrid.DataBind();
    }

    protected string[] GetAlphabetDS()
    {
        string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "All" };
        return alphabet;
    }
}
