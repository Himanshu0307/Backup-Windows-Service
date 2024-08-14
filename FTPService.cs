using System;
using Backup.Models;
using System.IO;
using System.Net;
using System.Net.FtpClient;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


namespace Backup
{
    public class FTPService
    {
        public string RemotePath = "";

        private Server server;
        private LogService logService;


        public FTPService(string remotePath, ref Server server, ref LogService logService)
        {
            RemotePath = remotePath;
            this.server = server;
            this.logService = logService;
            logService.LogInformation("Remote Directory Path:" + remotePath);



        }

        public void disableCertificateCheck()
        {
            ServicePointManager.ServerCertificateValidationCallback =
           new RemoteCertificateValidationCallback((object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors) => true);
        }


        public bool CheckDirectory(string filepath)
        {
            try
            {
                this.disableCertificateCheck();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filepath);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(server.Credential.Username, server.Credential.Password);
                request.KeepAlive = false;
                request.EnableSsl = true;
                logService.LogInformation("Checking Directory on server...");

                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    logService.LogInformation("Directory Found.");

                    return true;
                }
            }
            catch (Exception e)
            {
                logService.LogError("Directory Not Found. ");
                return false;
            }
        }

        public bool CheckandCreateDirectoryRecursively(string initialpath, string recursivePath)
        {
            try
            {
                string[] paths = recursivePath.Split('/');
                string currentPath = initialpath;
                foreach (var x in paths)
                {
                    currentPath = currentPath + "/" + x;
                    if (!CheckDirectory(currentPath))
                    {
                        CreateDirectory(currentPath);
                    }
                }
                return true;


            }
            catch (Exception e)
            {
                logService.LogError("Directory Not Found: " + e.ToString());
                return false;
            }
        }

        public void CreateDirectory(string filepath)
        {
            try
            {
              
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filepath);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(server.Credential.Username, server.Credential.Password);
                request.KeepAlive = false;
                request.EnableSsl = true;
                logService.LogInformation("Creating Directory on server...");
                using (var status = (FtpWebResponse)request.GetResponse())
                {
                    if (status.StatusCode == FtpStatusCode.PathnameCreated)
                        logService.LogInformation("SuccessFully Created Directory on Server.");
                }
            }
            catch (Exception e)
            {

                logService.LogError($"Failed to create directory({filepath})" + e.ToString());
            }
        }

        public void TransferBackup(string localBackupFilePath)
        {
            try
            {

                CheckandCreateDirectoryRecursively(server.TargetServerIp, RemotePath);
                string uploadUrl = $"{server.TargetServerIp}/{RemotePath}/backup_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.bak";
                logService.LogInformation("Uploading Data online.... ");
                this.disableCertificateCheck();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.EnableSsl = true;
                // Set FTP transfer mode to binary
                request.UseBinary = true;

                // Read the local backup file and upload it to the FTP server
                byte[] fileContents;
                using (FileStream fileStream = new FileStream(localBackupFilePath, FileMode.Open))
                {
                    fileContents = new byte[fileStream.Length];
                    fileStream.Read(fileContents, 0, (int)fileStream.Length);
                }

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                logService.LogInformation($"Upload File Complete, status: {response.StatusDescription}");
                response.Close();
            }
            catch (Exception e)
            {
                logService.LogError("Failed to upload file on server:" + e.ToString());
            }
        }

    }
}