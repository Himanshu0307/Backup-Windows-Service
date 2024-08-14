using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backup.Models
{
    public class FTPCredential
    {
        public FTPCredential(string username, string password, int port)
        {
            Username = username;
            Password = password;
            Port = port;
        }

        public String Username{get;set;}
        public String Password { get; set; }
        public int Port { get; set; }=21;


    }
}