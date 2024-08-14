using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backup.Models
{
    public class Server
    {
        public FTPCredential Credential;
        public String DatabaseServerName = "";
        public String DatabaseUserName = "";
        public List<String> ScheduleTime = new List<String>();

        public String DatabasePassword = "";

        public Server(FTPCredential credential, string databaseServerName, string databaseUserName, List<string> scheduleTime, string databasePassword, string targetServerIp, string baseDir, List<Database> databases)
        {
            Credential = credential;
            DatabaseServerName = databaseServerName;
            DatabaseUserName = databaseUserName;
            ScheduleTime = scheduleTime;
            DatabasePassword = databasePassword;
            TargetServerIp = targetServerIp;
            BaseDir = baseDir;
            Databases = databases;
        }

        public String TargetServerIp { get; set; }
        public String BaseDir { get; set; }
        public List<Database> Databases{get;set;}=new List<Database>();

       

        public override string ToString()
        {
            return $"DatabaseServerName:{DatabaseServerName} \n DatabaseUserName:{DatabaseUserName}";
        }
    }
}