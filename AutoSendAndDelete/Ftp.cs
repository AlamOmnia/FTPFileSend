using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoSendAndDelete
{
    public class FtpManager
    {
        public string FtpUrl { get; }
        public string FtpUser { get; }
        public string FtpPass { get; }
        public FtpManager(string ftpUrl, string ftpUser, string ftpPass)
        {
            this.FtpUrl = ftpUrl;
            this.FtpUser = ftpUser;
            this.FtpPass = ftpPass;
        }
        public void FTPSendSingleFile(string fullPathLocal, string fileToUpload)
        {
            string filename = FtpUrl + "/" + fileToUpload;
            FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(filename);
            ftpReq.UseBinary = true;
            ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
            ftpReq.Credentials = new NetworkCredential(FtpUser, FtpPass);
            byte[] b = File.ReadAllBytes(fullPathLocal);
            ftpReq.ContentLength = b.Length;
            using (Stream s = ftpReq.GetRequestStream())
            {
                s.Write(b, 0, b.Length);
            }
            FtpWebResponse ftpResp = (FtpWebResponse)ftpReq.GetResponse();
        }
        public bool FtpCheckSingleFile(string fileName)
        {
            // Get the object used to communicate with the server.
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FtpUrl + "/" + fileName);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(FtpUser, FtpPass);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string DirectoryList = reader.ReadToEnd();
                return DirectoryList.Contains(fileName);
            }
            catch(Exception e)
            {
                if (e.ToString().Contains("File unavailable")) return false;
                else throw;
            }
        }
    }
}
