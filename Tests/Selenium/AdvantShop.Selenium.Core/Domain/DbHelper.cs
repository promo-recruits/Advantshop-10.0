using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace AdvantShop.Selenium.Core.Domain;

public static class DbHelper
{
    public static bool ExistDatabase(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var connection = new ServerConnection(builder.DataSource, builder.UserID, builder.Password);
        var sqlServer = new Server(connection);
        var exist = sqlServer.Databases[builder.InitialCatalog] != null;
        connection.Disconnect();
        return exist;
    }

    public static void DropDatabase(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var connection = new ServerConnection(builder.DataSource, builder.UserID, builder.Password);
        var sqlServer = new Server(connection);

        var db = sqlServer.Databases[builder.InitialCatalog];
        if (db != null)
        {
            sqlServer.KillAllProcesses(builder.InitialCatalog);
            db.Drop();
        }

        connection.Disconnect();
    }


    public static void BackupDatabase(string connectionString, string databaseName, string destinationPath)
    {
        if (File.Exists(destinationPath))
        {
            var file = new FileInfo(destinationPath);
            if ((DateTime.Now - file.CreationTime).TotalHours < 3)
                return;

            File.Delete(destinationPath);
        }

        var builder = new SqlConnectionStringBuilder(connectionString);

        var serverConnection =
            new ServerConnection(builder.DataSource, builder.UserID, builder.Password);
        var sqlServer = new Server(serverConnection);

        Backup bkpDatabase = new() {Action = BackupActionType.Database, Database = databaseName};
        var bkpDevice = new BackupDeviceItem(destinationPath, DeviceType.File);
        bkpDatabase.Devices.Add(bkpDevice);
        bkpDatabase.SqlBackup(sqlServer);
        serverConnection.Disconnect();
    }

    public static void RestoreDatabase(string connectionString, string databaseName, string backUpFile,
        string databasePath)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);

        var serverConnection =
            new ServerConnection(builder.DataSource, builder.UserID, builder.Password);
        var sqlServer = new Server(serverConnection);
        var rstDatabase = new Restore
        {
            Action = RestoreActionType.Database,
            Database = databaseName,
            NoRecovery = false,
            ReplaceDatabase = true
        };

        var bkpDevice = new BackupDeviceItem(backUpFile, DeviceType.File);
        rstDatabase.Devices.Add(bkpDevice);

        var fl = rstDatabase.ReadFileList(sqlServer);
        foreach (DataRow fil in fl.Rows)
        {
            var relocatedFile = new RelocateFile
            {
                LogicalFileName = fil["LogicalName"].ToString(),
                PhysicalFileName = fil["Type"].ToString() == "D"
                    ? databasePath + databaseName + ".mdf"
                    : databasePath + databaseName + "_log" + ".ldf"
            };
            rstDatabase.RelocateFiles.Add(relocatedFile);
        }

        rstDatabase.SqlRestore(sqlServer);
    }
}