//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core
{
    public enum PingDbState
    {
        NoError = 0,
        FailConnectionSqlDb = 1,
        WrongDbStructure = 2,
        WrongDbVersion = 3,
        Unknown = 4
    }

    public class DataBaseService
    {
        public static string GetDbVersionFromConfig()
        {
            try
            {
                return SettingProvider.GetConfigSettingValue("DB_Version");
            }
            catch
            {
                // nothing here
            }
            return "";

        }

        public static string GetkDBVersionFomDatabase()
        {
            try
            {
                string strDbVersion = "";

                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandText = "[Settings].[sp_GetInternalSetting]";
                    db.cmd.CommandType = CommandType.StoredProcedure;
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.AddWithValue("@settingKey", "db_version");

                    db.cnOpen();
                    strDbVersion = (string)db.cmd.ExecuteScalar();
                    db.cnClose();
                }

                return strDbVersion;
            }
            catch
            {
                // nothing here
            }

            return "";
        }

        public static bool PingDateBase()
        {
            bool boolRes = false;
            try
            {
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandText = "SELECT GETDATE() AS NOWDATE";
                    db.cmd.CommandType = CommandType.Text;
                    db.cmd.CommandTimeout = 10;

                    object obj = null;

                    db.cnOpen();

                    if (db.cnStatus() == ConnectionState.Open)
                    {
                        obj = db.cmd.ExecuteScalar();
                    }

                    db.cnClose();

                    if ((obj != null) && (!(obj is DBNull)))
                    {
                        boolRes = true;
                    }
                }
            }
            catch
            {
                boolRes = false;
            }

            return boolRes;
        }


        public static bool PingDateBase(string strConnectionString)
        {
            var boolRes = false;

            try
            {
                using (var db = new SQLDataAccess(strConnectionString))
                {
                    db.cmd.CommandText = "SELECT GETDATE() AS NOWDATE";
                    db.cmd.CommandType = CommandType.Text;
                    db.cmd.CommandTimeout = 3;

                    object obj = null;

                    db.cnOpen();

                    if (db.cnStatus() == ConnectionState.Open)
                    {
                        obj = db.cmd.ExecuteScalar();
                    }

                    db.cnClose();

                    if ((obj != null) && (!(obj is DBNull)))
                    {
                        boolRes = true;
                    }
                }
            }
            catch
            {
                boolRes = false;
            }

            return boolRes;
        }



        public static bool CheckDBStructure()
        {

            bool boolResult = false;

            try
            {
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandText =
                        @"declare @res bit
                        set @res = 0 
                        IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[Settings]') AND type in (N'U')) AND
	                        EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[InternalSettings]') AND type in (N'U')) AND
	                        EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[sp_GetInternalSetting]') AND type in (N'P', N'PC'))
                        BEGIN
	                        set @res = 1 
                        END 
                        SELECT @res AS result";

                    db.cmd.CommandType = CommandType.Text;
                    db.cmd.Parameters.Clear();

                    db.cnOpen();
                    boolResult = (bool)db.cmd.ExecuteScalar();
                    db.cnClose();
                }
            }
            catch
            {
                boolResult = false;
            }

            return boolResult;
        }

        public static PingDbState CheckDbStates()
        {
            var status = PingDbState.NoError;
            var dbversion = GetkDBVersionFomDatabase();
            var configVersion = GetDbVersionFromConfig();

            if (string.IsNullOrEmpty(dbversion))
            {
                status = PingDbState.FailConnectionSqlDb;
            }
            else if (string.IsNullOrEmpty(configVersion))
            {
                status = PingDbState.Unknown;
            }
            else if (dbversion != configVersion)
            { 
                status = PingDbState.WrongDbVersion;
            }

            return status;

            //if (!DataBaseService.PingDateBase())
            //{
            //    return PingDbState.FailConnectionSqlDb;
            //}

            //if (!DataBaseService.CheckDBStructure())
            //{
            //    return PingDbState.WrongDbStructure;
            //}

            //if (!DataBaseService.CheckDBVersion())
            //{
            //    return PingDbState.WrongDbVersion;
            //}

            //return PingDbState.NoError;
        }

    } 

}