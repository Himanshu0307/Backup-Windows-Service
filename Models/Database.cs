using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backup.Models
{
    public class Database
    {
       public String DatabaseName="";

        public Database(string databaseName, string companyName)
        {
            DatabaseName = databaseName;
            CompanyName = companyName;
        }

        public String CompanyName{get;set;}="";
        public int DaysBackKeep { get; set; } = 1;



    }
}