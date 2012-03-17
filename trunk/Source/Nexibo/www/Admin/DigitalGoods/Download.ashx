<%@ WebHandler Language="C#" Class="Download" %>

using System;
using System.IO;
using System.Web;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Utility;

public class Download : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Server.ScriptTimeout = 14400;

        // GET THE REQUEST OBJECT
        HttpRequest request = context.Request;

        // SEE IF WE HAVE A DIGITAL GOOD ID OR AN ENCODED FILE NAME
        int digitalGoodId = AlwaysConvert.ToInt(request.QueryString["DigitalGoodId"]);
        if (digitalGoodId != 0)
        {
            // REQUEST FOR A FILE LINKED TO A DIGITAL GOOD
            DigitalGood dg = DigitalGoodDataSource.Load(digitalGoodId);
            if (dg != null) DownloadHelper.SendFileDataToClient(context, dg);
            else context.Response.Write("The specified digital good cannot be found.");
        }
        else
        {
            // CHECK FOR AN ENCODED FILE NAME
            string encryptedFileName = request.QueryString["F"];
            if (!string.IsNullOrEmpty(encryptedFileName))
            {
                // THE NAME WILL BE AES ENCRYPTED, ATTEMPT DECRYPTION
                string requestedPath = EncryptionHelper.DecryptAES(encryptedFileName);
                FileInfo fi = new FileInfo(Path.Combine(FileHelper.BaseDigitalGoodsPath, requestedPath));
                if (fi.Exists)
                {
                    if (fi.FullName.StartsWith(FileHelper.BaseDigitalGoodsPath))
                    {
                        DownloadHelper.SendFileDataToClient(context, fi.FullName, fi.Name);
                    }
                    else
                    {
                        context.Response.Write("The requested file must be in your digital goods folder.");
                    }
                }
                else
                {
                    context.Response.Write("The specified file cannot be found.");
                }
            }
            else
            {
                context.Response.Write("The filename must be specified in the querystring, F=FileToDownload.");
            }
        }
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}