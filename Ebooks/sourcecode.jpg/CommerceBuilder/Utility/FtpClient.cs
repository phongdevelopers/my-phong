using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class for basic FTP operations
    /// </summary>
    public class FtpClient
    {
        #region variables

        private string _RemoteHost;
        //private int _RemotePort;
        private FtpConnectionMode _ConnectMode;
        private FtpTransferType _TransferType;
        private bool _KeepAlive;
        private string _UserName = string.Empty;
        private string _Password = string.Empty;

        #endregion variables

        /// <summary>
        /// Constructor
        /// </summary>
        public FtpClient()
        {
            _ConnectMode = FtpConnectionMode.ACTIVE;
            _TransferType = FtpTransferType.BINARY;            
            _KeepAlive = true;
        }

        #region properties

        /// <summary>
        /// Remote host to connect to
        /// </summary>
        public string RemoteHost
        {
            get { return _RemoteHost; }
            set { _RemoteHost = value;}
        }
/*        public int RemotePort
        {
            get { return _RemotePort; }
            set { _RemotePort = value; }
        }
*/ 
        /// <summary>
        /// 
        /// </summary>
        public bool KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }

        /// <summary>
        /// FTP transfer type. Binary or ASCII
        /// </summary>
        public FtpTransferType TransferType
        {
            get { return _TransferType; }
            set { _TransferType = value; }
        }

        /// <summary>
        /// FTP connection mode. Active or Passive
        /// </summary>
        public FtpConnectionMode ConnectionMode
        {
            get { return _ConnectMode; }
            set { _ConnectMode = value; }
        }

        /// <summary>
        /// User name to access the FTP server
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// <summary>
        /// Password to access the FTP server
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        #endregion properties

        /// <summary>
        /// Uploads a file from to the FTP server
        /// </summary>
        /// <param name="localFilePath">The path of the file to upload</param>
        /// <param name="remoteFileName">The name of the file on remote server</param>
        public virtual void PutFile(string localFilePath, string remoteFileName)
        {
            if(string.IsNullOrEmpty(this.RemoteHost)) throw new IOException("Remote host is undefined.");

            ManualResetEvent waitObject;
            FtpState state = new FtpState();
            string remoteFileUri;
            string remoteHost = _RemoteHost.Trim();
            if (!remoteHost.StartsWith("ftp://"))
            {
                remoteHost = "ftp://" + remoteHost;
            }

            if (remoteHost.EndsWith("/"))
            {
                remoteFileUri = remoteHost + remoteFileName;
            }
            else
            {
                remoteFileUri = remoteHost + "/" + remoteFileName;
            }
            Uri target = new Uri(remoteFileUri);
            FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(target);
            ftpReq.UseBinary = this.TransferType == FtpTransferType.BINARY;
            ftpReq.Credentials = new NetworkCredential(this.UserName, this.Password);
            ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
            
            // Store the request in the object that we pass into the
            // asynchronous operations.
            state.Request = ftpReq;
            state.FileName = localFilePath;

            // Get the event to wait on.
            waitObject = state.OperationComplete;

            // Asynchronously get the stream for the file contents.
            ftpReq.BeginGetRequestStream(
                new AsyncCallback(EndGetStreamCallback),
                state
            );

            // Block the current thread until all operations are complete.
            waitObject.WaitOne();

            // The operations either completed or threw an exception.
            if (state.OperationException != null)
            {
                throw state.OperationException;
            }
        }

        private static void EndGetStreamCallback(IAsyncResult ar)
        {
            FtpState state = (FtpState)ar.AsyncState;

            Stream requestStream = null;
            // End the asynchronous call to get the request stream.
            try
            {
                requestStream = state.Request.EndGetRequestStream(ar);
                // Copy the file contents to the request stream.
                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                int readBytes = 0;
                FileStream stream = File.OpenRead(state.FileName);
                do
                {
                    readBytes = stream.Read(buffer, 0, bufferLength);
                    requestStream.Write(buffer, 0, readBytes);
                    count += readBytes;
                }
                while (readBytes != 0);
                
                // IMPORTANT: Close the request stream before sending the request.
                requestStream.Close();
                // Asynchronously get the response to the upload request.
                state.Request.BeginGetResponse(
                    new AsyncCallback(EndGetResponseCallback),
                    state
                );
            }
            // Return exceptions to the main application thread.
            catch (Exception e)
            {
                Console.WriteLine("Could not get the request stream.");
                state.OperationException = e;
                state.OperationComplete.Set();
                return;
            }

        }

        // The EndGetResponseCallback method  
        // completes a call to BeginGetResponse.
        private static void EndGetResponseCallback(IAsyncResult ar)
        {
            FtpState state = (FtpState)ar.AsyncState;
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)state.Request.EndGetResponse(ar);
                response.Close();
                state.StatusDescription = response.StatusDescription;
                // Signal the main application thread that 
                // the operation is complete.
                state.OperationComplete.Set();
            }
            // Return exceptions to the main application thread.
            catch (Exception e)
            {                
                state.OperationException = e;
                state.OperationComplete.Set();
            }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="remoteFileName"></param>
        public virtual void GetFile(string localFilePath, string remoteFileName)
        {
            throw new NotImplementedException("Not implemented");
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <returns></returns>
        public virtual byte[] GetFile(string remoteFileName)
        {
            throw new NotImplementedException("Not implemented");
        } 
    }

    /// <summary>
    /// The ftp transfer type. Binary or ASCII
    /// </summary>
    public enum FtpTransferType
    {
        /// <summary>
        /// Transfer type binary
        /// </summary>
        BINARY,

        /// <summary>
        /// Transfer type ASCII
        /// </summary>
        ASCII
    }

    /// <summary>
    /// Connection mode. Active or Passive.
    /// </summary>
    public enum FtpConnectionMode
    {
        /// <summary>
        /// Active connection mode
        /// </summary>
        ACTIVE,
 
        /// <summary>
        /// Passive connection mode
        /// </summary>
        PASV
    }

    class FtpState
    {
        private ManualResetEvent wait;
        private FtpWebRequest request;
        private string fileName;
        private Exception operationException = null;
        string status;

        public FtpState()
        {
            wait = new ManualResetEvent(false);
        }

        public ManualResetEvent OperationComplete
        {
            get { return wait; }
        }

        public FtpWebRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public Exception OperationException
        {
            get { return operationException; }
            set { operationException = value; }
        }
        public string StatusDescription
        {
            get { return status; }
            set { status = value; }
        }
    }

}
