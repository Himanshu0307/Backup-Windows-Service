# Backup Windows Service
This repository contains a Windows Service application designed to automate SQL Server backups and transfer them to a remote server via FTP. This service is particularly useful for database administrators and IT professionals looking to streamline their backup and transfer processes, ensuring that backups are both secure and readily accessible.  
## Features
* **Automated SQL Backup**: Schedule and manage SQL Server backups automatically.
* **FTP Transfer**: Upload backups to a remote server via FTP.
* **Configuration**: Easily configure backup schedules, FTP server details, and other settings.
* **Error Logging**: Comprehensive error handling and logging for troubleshooting.
* **Service Management**: Install, start, stop, and manage the service through Windows Service Manager.


## How to approach
* Install .NET SDK v4.8
* Clone the repository in local File System.
* Run `dotnet restore` command to generate object files and download dependencies for the project.
* Now run `dotnet build` command to generate build files in `bin/` folder.
* Install the exe as Service using Windows Service Manager. [Link](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/sc-create)
* Edit the Configuration file `BaseDirectory/BackupConfig.json`
* Start the service using Service Manager.

And all set for the day : thumbsup:

 *If you encounter any issues or have any questions, please feel free to reach out.*
 ## \#FeelFreetoConnect
   
