using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Backup.Models;
using System.Data.SqlClient;
using System.IO;


namespace Backup
{
    public class BackupService
    {
        private Database DatabaseInfo;
        private Server ServerInfo;

        private LogService logService;

        private String ConnectionString;
        private String BackupPath;

        public BackupService(Database database, ref Server server, ref LogService logService)
        {
            this.DatabaseInfo = database;
            this.ServerInfo = server;
            this.logService = logService;

            //Create a Backup Directory
            this.BackupPath = this.ServerInfo.BaseDir + "/Backup/" + DatabaseInfo.CompanyName + "/";
            // if (!Directory.Exists(BackupPath))
            // {
            //     Directory.CreateDirectory(BackupPath);
            // }
            this.CreateBackupDirectory();
            ConnectionString = $"Data Source={ServerInfo.DatabaseServerName}; UID={ServerInfo.DatabaseUserName}; Password={ServerInfo.DatabasePassword};Database={DatabaseInfo.DatabaseName};";
        }

        public void CreateBackupDirectory()
        {
            try
            {
                if (!Directory.Exists(BackupPath))
                {
                    Directory.CreateDirectory(BackupPath);
                }
            }
            catch (System.Exception e)
            {

                logService.LogError("Failed to create backup directory in local system" + e.ToString());
            }
        }

        public void DeleteOldDirectory()
        {
            try
            {
                string path = this.ServerInfo.BaseDir + "/Backup/" + DatabaseInfo.CompanyName + "/";
                DateTime firstDeleteDate = DateTime.Now.AddDays(-1 * DatabaseInfo.DaysBackKeep);
                logService.LogInformation("Deleting old directory if any....");

                while (Directory.Exists(path + firstDeleteDate.ToString("yyyy-MM-dd") + "/"))
                {
                    logService.LogInformation("Deleting Directory: " + path + firstDeleteDate.ToString("yyyy-MM-dd") + "/");

                    Directory.Delete(path + firstDeleteDate.ToString("yyyy-MM-dd") + "/", true);
                    firstDeleteDate = firstDeleteDate.AddDays(-1);
                }

            }
            catch (Exception e)
            {
                logService.LogError("Failed to delete old directories" + e.ToString());
            }


        }



        public string Backup()
        {
            try
            {
                //Create Directory
                if (!Directory.Exists(BackupPath + DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    Directory.CreateDirectory(BackupPath + DateTime.Now.ToString("yyyy-MM-dd"));
                }
                String finalPath = BackupPath + DateTime.Now.ToString("yyyy-MM-dd") + "/Backup_" + DateTime.Now.ToString("yyyy-MM-dd-ss") + ".bak";
                // finalPath = Directory.GetCurrentDirectory().Replace("\\", "/") + "/" + finalPath;
                logService.LogInformation("Path for Backup in local system:" + finalPath);
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"BACKUP DATABASE {DatabaseInfo.DatabaseName} TO DISK ='{finalPath}'";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    command.Dispose();
                    logService.LogInformation("Successfully Created Backup");

                    // logService.LogInformation("finalPath"+finalPath);
                }
                return finalPath;
            }
            catch (Exception e)
            {
                logService.LogError("Error Creating Backup in local system:" + e.ToString());
                return null;
            }
        }

        public void BackupNow()
        {
            this.Backup();
        }

        public void BackupAndTransfer()
        {
            try
            {
                FTPService service = new FTPService($"{DateTime.Now.ToString("yyyy-MM-dd")}/{DatabaseInfo.CompanyName}", ref ServerInfo, ref logService);
                string path = this.Backup();
                if (!String.IsNullOrEmpty(path))
                    service.TransferBackup(path);
                this.DeleteOldDirectory();
            }
            catch (Exception e)
            {
                logService.LogError("Failed to Create and Transfer backup");
            }
        }
        public void BackupAndTransfer(object state)
        {
            FTPService service = new FTPService($"{DateTime.Now.ToString("yyyy-MM-dd")}/{DatabaseInfo.CompanyName}", ref ServerInfo, ref logService);
            string path = this.Backup();
            if (!String.IsNullOrEmpty(path))
                service.TransferBackup(path);

        }


        public void ChangeDatabase(Database db)
        {
            this.DatabaseInfo = db;
            this.BackupPath = this.ServerInfo.BaseDir + "/Backup/" + DatabaseInfo.CompanyName + "/";
            if (!Directory.Exists(BackupPath))
            {
                Directory.CreateDirectory(BackupPath);
            }
            ConnectionString = $"Data Source={ServerInfo.DatabaseServerName}; UID={ServerInfo.DatabaseUserName}; Password={ServerInfo.DatabasePassword};Database={DatabaseInfo.DatabaseName};";

        }


    }
}
