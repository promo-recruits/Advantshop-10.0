//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace AdvantShop.Repository
{
    public class CountryService
    {
        private const string CountryCacheKey = "Country_";

        public static List<Country> GetAllCountries()
        {
            const string cacheKey = CountryCacheKey + "All";

            return CacheManager.Get(cacheKey,
                () =>
                    SQLDataAccess.ExecuteReadList("SELECT * FROM [Customers].[Country] ORDER BY [CountryName] ASC",
                        CommandType.Text, GetCountryFromReader));
        }

        public static List<Country> GetAllCountryIdAndName()
        {
            return SQLDataAccess.ExecuteReadList("SELECT CountryID,CountryName FROM [Customers].[Country]",
                CommandType.Text,
                reader => new Country
                {
                    CountryId = SQLDataHelper.GetInt(reader, "CountryID"),
                    Name = SQLDataHelper.GetString(reader, "CountryName")
                });
        }

        public static List<Country> GetCountriesByDisplayInPopup()
        {
            return CacheManager.Get(CountryCacheKey + "DisplayInPopup",
                () =>
                    SQLDataAccess.ExecuteReadList(
                        "Select top 12 * From Customers.Country Where DisplayInPopup=1 Order By SortOrder desc, CountryName asc",
                        CommandType.Text, GetCountryFromReader));
        }

        #region Update / Add / Delete Country

        public static void Delete(int countryId)
        {
            if (countryId != SettingsMain.SellerCountryId)
            {
                SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[Country] where CountryID = @CountryId",
                    CommandType.Text, new SqlParameter("@CountryId", countryId));

                CacheManager.RemoveByPattern(CountryCacheKey);
            }
        }

        public static void Add(Country country)
        {
            country.CountryId =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Customers].[Country] (CountryName, CountryISO2, CountryISO3, DisplayInPopup,SortOrder,DialCode) VALUES (@Name, @ISO2, @ISO3, @DisplayInPopup,@SortOrder,@DialCode); SELECT scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@Name", country.Name),
                    new SqlParameter("@ISO2", country.Iso2),
                    new SqlParameter("@ISO3", country.Iso3),
                    new SqlParameter("@DisplayInPopup", country.DisplayInPopup),
                    new SqlParameter("@SortOrder", country.SortOrder),
                    new SqlParameter("@DialCode", country.DialCode ?? (object)DBNull.Value)
                    );
            CacheManager.RemoveByPattern(CountryCacheKey);
        }

        public static void Update(Country country)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[Country] set CountryName=@name, CountryISO2=@ISO2, CountryISO3=@ISO3, DisplayInPopup=@DisplayInPopup, SortOrder=@SortOrder, DialCode=@DialCode Where CountryID = @id",
                CommandType.Text,
                new SqlParameter("@id", country.CountryId),
                new SqlParameter("@name", country.Name),
                new SqlParameter("@ISO2", country.Iso2),
                new SqlParameter("@ISO3", country.Iso3),
                new SqlParameter("@DisplayInPopup", country.DisplayInPopup),
                new SqlParameter("@SortOrder", country.SortOrder),
                new SqlParameter("@DialCode", country.DialCode ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern(CountryCacheKey);
        }

        #endregion


        public static string GetIso2(string name)
        {
            return
                SQLDataAccess.ExecuteScalar<string>(
                    "SELECT [CountryISO2] FROM [Customers].[Country] Where CountryName = @CountryName",
                    CommandType.Text, new SqlParameter("@CountryName", name));
        }

        public static string GetIso3(string name)
        {
            return
                SQLDataAccess.ExecuteScalar<string>(
                    "SELECT [CountryISO3] FROM [Customers].[Country] Where CountryName = @CountryName",
                    CommandType.Text, new SqlParameter("@CountryName", name));
        }

        public static Country GetCountry(int id)
        {
            var cacheKey = CountryCacheKey + id;

            var country =
                CacheManager.Get<Country>(cacheKey,
                    () => SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[Country] Where CountryID = @id",
                        CommandType.Text, GetCountryFromReader, new SqlParameter("@id", id)));

            return country;
        }

        public static Country GetCountryByName(string countryName)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[Country] Where CountryName = @CountryName",
                CommandType.Text, GetCountryFromReader, new SqlParameter("@CountryName", countryName));
        }

        public static Country GetCountryByIso2(string iso2)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[Country] Where CountryISO2 = @iso2",
                CommandType.Text, GetCountryFromReader, new SqlParameter("@iso2", iso2));
        }


        public static Country GetCountryFromReader(SqlDataReader reader)
        {
            return new Country
            {
                CountryId = SQLDataHelper.GetInt(reader, "CountryID"),
                Iso2 = SQLDataHelper.GetString(reader, "CountryISO2"),
                Iso3 = SQLDataHelper.GetString(reader, "CountryISO3"),
                Name = SQLDataHelper.GetString(reader, "CountryName"),
                DisplayInPopup = SQLDataHelper.GetBoolean(reader, "DisplayInPopup"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                DialCode = SQLDataHelper.GetNullableInt(reader, "DialCode")
            };
        }


        //public static string GetCountryNameById(int countryId)
        //{
        //    return SQLDataAccess.ExecuteScalar<string>(
        //        "SELECT CountryName FROM Customers.Country Where CountryID = @id",
        //        CommandType.Text, new SqlParameter("@id", countryId));
        //}

        //public static string GetCountryIso2ById(int countryId)
        //{
        //    return SQLDataAccess.ExecuteScalar<string>(
        //        "SELECT CountryISO2 FROM Customers.Country Where CountryID = @id",
        //        CommandType.Text, new SqlParameter("@id", countryId));
        //}

        //public static List<int> GetCountryIdByIp(string Ip)
        //{
        //    long ipDec;
        //    try
        //    {
        //        if (Ip == "::1")
        //            ipDec = 127 * 16777216 + 1;
        //        else
        //        {
        //            string[] ip = Ip.Split('.');
        //            ipDec = (Int32.Parse(ip[0])) * 16777216 + (Int32.Parse(ip[1])) * 65536 + (Int32.Parse(ip[2])) * 256 + Int32.Parse(ip[3]);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ipDec = 127 * 16777216 + 1;
        //    }
        //    List<int> ids = SQLDataAccess.ExecuteReadList<int>("SELECT CountryID FROM Customers.Country Where CountryISO2 = (SELECT country_code FROM Customers.GeoIP Where begin_num <= @IP AND end_num >= @IP)",
        //                                                 CommandType.Text,
        //                                                 reader => SQLDataHelper.GetInt(reader, "CountryID"), new SqlParameter("@IP", ipDec));
        //    return ids;
        //}

        //public static List<string> GetCountryNameByIp(string Ip)
        //{
        //    long ipDec;
        //    try
        //    {
        //        if (Ip == "::1")
        //            ipDec = 127 * 16777216 + 1;
        //        else
        //        {
        //            string[] ip = Ip.Split('.');
        //            ipDec = (Int32.Parse(ip[0])) * 16777216 + (Int32.Parse(ip[1])) * 65536 + (Int32.Parse(ip[2])) * 256 + Int32.Parse(ip[3]);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ipDec = 127 * 16777216 + 1;
        //    }

        //    List<string> listNames = SQLDataAccess.ExecuteReadList<string>("SELECT * FROM Customers.Country WHERE CountryISO2 = (SELECT country_code FROM Customers.GeoIP WHERE begin_num <= @IP AND end_num >= @IP)",
        //                                                                   CommandType.Text, reader => SQLDataHelper.GetString(reader, "CountryName"),
        //                                                                   new SqlParameter("@IP", ipDec)) ?? new List<string> { { "local" } };

        //    if (listNames.Count == 0)
        //        listNames.Add("local");

        //    return listNames;
        //}

        public static int GetCountryIdByName(string countryName)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT CountryID FROM Customers.Country Where CountryName = @name",
                CommandType.Text, new SqlParameter("@name", countryName));
        }

        public static List<string> GetCountriesByName(string name)
        {
            var translit = StringHelper.TranslitToRusKeyboard(name);

            return
                SQLDataAccess.ExecuteReadList(
                    "Select CountryName From Customers.Country Where CountryName like @name + '%' or CountryName like @trname + '%'",
                    CommandType.Text,
                    reader => SQLDataHelper.GetString(reader, "CountryName"), 
                    new SqlParameter("@name", name),
                    new SqlParameter("@trname", translit));
        }

        #region Helps Methods

        public static string Iso2ToIso3(string iso2)
        {
            if (iso2.IsNullOrEmpty())
                return null;

            if (iso2.Equals("AF", StringComparison.InvariantCultureIgnoreCase)) return "AFG";
            if (iso2.Equals("AL", StringComparison.InvariantCultureIgnoreCase)) return "ALB";
            if (iso2.Equals("DZ", StringComparison.InvariantCultureIgnoreCase)) return "DZA";
            if (iso2.Equals("AS", StringComparison.InvariantCultureIgnoreCase)) return "ASM";
            if (iso2.Equals("AD", StringComparison.InvariantCultureIgnoreCase)) return "AND";
            if (iso2.Equals("AO", StringComparison.InvariantCultureIgnoreCase)) return "AGO";
            if (iso2.Equals("AI", StringComparison.InvariantCultureIgnoreCase)) return "AIA";
            if (iso2.Equals("AQ", StringComparison.InvariantCultureIgnoreCase)) return "ATA";
            if (iso2.Equals("AG", StringComparison.InvariantCultureIgnoreCase)) return "ATG";
            if (iso2.Equals("AR", StringComparison.InvariantCultureIgnoreCase)) return "ARG";
            if (iso2.Equals("AM", StringComparison.InvariantCultureIgnoreCase)) return "ARM";
            if (iso2.Equals("AW", StringComparison.InvariantCultureIgnoreCase)) return "ABW";
            if (iso2.Equals("AU", StringComparison.InvariantCultureIgnoreCase)) return "AUS";
            if (iso2.Equals("AT", StringComparison.InvariantCultureIgnoreCase)) return "AUT";
            if (iso2.Equals("AZ", StringComparison.InvariantCultureIgnoreCase)) return "AZE";
            if (iso2.Equals("BS", StringComparison.InvariantCultureIgnoreCase)) return "BHS";
            if (iso2.Equals("BH", StringComparison.InvariantCultureIgnoreCase)) return "BHR";
            if (iso2.Equals("BD", StringComparison.InvariantCultureIgnoreCase)) return "BGD";
            if (iso2.Equals("BB", StringComparison.InvariantCultureIgnoreCase)) return "BRB";
            if (iso2.Equals("BY", StringComparison.InvariantCultureIgnoreCase)) return "BLR";
            if (iso2.Equals("BE", StringComparison.InvariantCultureIgnoreCase)) return "BEL";
            if (iso2.Equals("BZ", StringComparison.InvariantCultureIgnoreCase)) return "BLZ";
            if (iso2.Equals("BJ", StringComparison.InvariantCultureIgnoreCase)) return "BEN";
            if (iso2.Equals("BM", StringComparison.InvariantCultureIgnoreCase)) return "BMU";
            if (iso2.Equals("BT", StringComparison.InvariantCultureIgnoreCase)) return "BTN";
            if (iso2.Equals("BO", StringComparison.InvariantCultureIgnoreCase)) return "BOL";
            if (iso2.Equals("BQ", StringComparison.InvariantCultureIgnoreCase)) return "BES";
            if (iso2.Equals("BA", StringComparison.InvariantCultureIgnoreCase)) return "BIH";
            if (iso2.Equals("BW", StringComparison.InvariantCultureIgnoreCase)) return "BWA";
            if (iso2.Equals("BV", StringComparison.InvariantCultureIgnoreCase)) return "BVT";
            if (iso2.Equals("BR", StringComparison.InvariantCultureIgnoreCase)) return "BRA";
            if (iso2.Equals("IO", StringComparison.InvariantCultureIgnoreCase)) return "IOT";
            if (iso2.Equals("BN", StringComparison.InvariantCultureIgnoreCase)) return "BRN";
            if (iso2.Equals("BG", StringComparison.InvariantCultureIgnoreCase)) return "BGR";
            if (iso2.Equals("BF", StringComparison.InvariantCultureIgnoreCase)) return "BFA";
            if (iso2.Equals("BI", StringComparison.InvariantCultureIgnoreCase)) return "BDI";
            if (iso2.Equals("CV", StringComparison.InvariantCultureIgnoreCase)) return "CPV";
            if (iso2.Equals("KH", StringComparison.InvariantCultureIgnoreCase)) return "KHM";
            if (iso2.Equals("CM", StringComparison.InvariantCultureIgnoreCase)) return "CMR";
            if (iso2.Equals("CA", StringComparison.InvariantCultureIgnoreCase)) return "CAN";
            if (iso2.Equals("KY", StringComparison.InvariantCultureIgnoreCase)) return "CYM";
            if (iso2.Equals("CF", StringComparison.InvariantCultureIgnoreCase)) return "CAF";
            if (iso2.Equals("TD", StringComparison.InvariantCultureIgnoreCase)) return "TCD";
            if (iso2.Equals("CL", StringComparison.InvariantCultureIgnoreCase)) return "CHL";
            if (iso2.Equals("CN", StringComparison.InvariantCultureIgnoreCase)) return "CHN";
            if (iso2.Equals("CX", StringComparison.InvariantCultureIgnoreCase)) return "CXR";
            if (iso2.Equals("CC", StringComparison.InvariantCultureIgnoreCase)) return "CCK";
            if (iso2.Equals("CO", StringComparison.InvariantCultureIgnoreCase)) return "COL";
            if (iso2.Equals("KM", StringComparison.InvariantCultureIgnoreCase)) return "COM";
            if (iso2.Equals("CD", StringComparison.InvariantCultureIgnoreCase)) return "COD";
            if (iso2.Equals("CG", StringComparison.InvariantCultureIgnoreCase)) return "COG";
            if (iso2.Equals("CK", StringComparison.InvariantCultureIgnoreCase)) return "COK";
            if (iso2.Equals("CR", StringComparison.InvariantCultureIgnoreCase)) return "CRI";
            if (iso2.Equals("HR", StringComparison.InvariantCultureIgnoreCase)) return "HRV";
            if (iso2.Equals("CU", StringComparison.InvariantCultureIgnoreCase)) return "CUB";
            if (iso2.Equals("CW", StringComparison.InvariantCultureIgnoreCase)) return "CUW";
            if (iso2.Equals("CY", StringComparison.InvariantCultureIgnoreCase)) return "CYP";
            if (iso2.Equals("CZ", StringComparison.InvariantCultureIgnoreCase)) return "CZE";
            if (iso2.Equals("CI", StringComparison.InvariantCultureIgnoreCase)) return "CIV";
            if (iso2.Equals("DK", StringComparison.InvariantCultureIgnoreCase)) return "DNK";
            if (iso2.Equals("DJ", StringComparison.InvariantCultureIgnoreCase)) return "DJI";
            if (iso2.Equals("DM", StringComparison.InvariantCultureIgnoreCase)) return "DMA";
            if (iso2.Equals("DO", StringComparison.InvariantCultureIgnoreCase)) return "DOM";
            if (iso2.Equals("EC", StringComparison.InvariantCultureIgnoreCase)) return "ECU";
            if (iso2.Equals("EG", StringComparison.InvariantCultureIgnoreCase)) return "EGY";
            if (iso2.Equals("SV", StringComparison.InvariantCultureIgnoreCase)) return "SLV";
            if (iso2.Equals("GQ", StringComparison.InvariantCultureIgnoreCase)) return "GNQ";
            if (iso2.Equals("ER", StringComparison.InvariantCultureIgnoreCase)) return "ERI";
            if (iso2.Equals("EE", StringComparison.InvariantCultureIgnoreCase)) return "EST";
            if (iso2.Equals("SZ", StringComparison.InvariantCultureIgnoreCase)) return "SWZ";
            if (iso2.Equals("ET", StringComparison.InvariantCultureIgnoreCase)) return "ETH";
            if (iso2.Equals("FK", StringComparison.InvariantCultureIgnoreCase)) return "FLK";
            if (iso2.Equals("FO", StringComparison.InvariantCultureIgnoreCase)) return "FRO";
            if (iso2.Equals("FJ", StringComparison.InvariantCultureIgnoreCase)) return "FJI";
            if (iso2.Equals("FI", StringComparison.InvariantCultureIgnoreCase)) return "FIN";
            if (iso2.Equals("FR", StringComparison.InvariantCultureIgnoreCase)) return "FRA";
            if (iso2.Equals("GF", StringComparison.InvariantCultureIgnoreCase)) return "GUF";
            if (iso2.Equals("PF", StringComparison.InvariantCultureIgnoreCase)) return "PYF";
            if (iso2.Equals("TF", StringComparison.InvariantCultureIgnoreCase)) return "ATF";
            if (iso2.Equals("GA", StringComparison.InvariantCultureIgnoreCase)) return "GAB";
            if (iso2.Equals("GM", StringComparison.InvariantCultureIgnoreCase)) return "GMB";
            if (iso2.Equals("GE", StringComparison.InvariantCultureIgnoreCase)) return "GEO";
            if (iso2.Equals("DE", StringComparison.InvariantCultureIgnoreCase)) return "DEU";
            if (iso2.Equals("GH", StringComparison.InvariantCultureIgnoreCase)) return "GHA";
            if (iso2.Equals("GI", StringComparison.InvariantCultureIgnoreCase)) return "GIB";
            if (iso2.Equals("GR", StringComparison.InvariantCultureIgnoreCase)) return "GRC";
            if (iso2.Equals("GL", StringComparison.InvariantCultureIgnoreCase)) return "GRL";
            if (iso2.Equals("GD", StringComparison.InvariantCultureIgnoreCase)) return "GRD";
            if (iso2.Equals("GP", StringComparison.InvariantCultureIgnoreCase)) return "GLP";
            if (iso2.Equals("GU", StringComparison.InvariantCultureIgnoreCase)) return "GUM";
            if (iso2.Equals("GT", StringComparison.InvariantCultureIgnoreCase)) return "GTM";
            if (iso2.Equals("GG", StringComparison.InvariantCultureIgnoreCase)) return "GGY";
            if (iso2.Equals("GN", StringComparison.InvariantCultureIgnoreCase)) return "GIN";
            if (iso2.Equals("GW", StringComparison.InvariantCultureIgnoreCase)) return "GNB";
            if (iso2.Equals("GY", StringComparison.InvariantCultureIgnoreCase)) return "GUY";
            if (iso2.Equals("HT", StringComparison.InvariantCultureIgnoreCase)) return "HTI";
            if (iso2.Equals("HM", StringComparison.InvariantCultureIgnoreCase)) return "HMD";
            if (iso2.Equals("VA", StringComparison.InvariantCultureIgnoreCase)) return "VAT";
            if (iso2.Equals("HN", StringComparison.InvariantCultureIgnoreCase)) return "HND";
            if (iso2.Equals("HK", StringComparison.InvariantCultureIgnoreCase)) return "HKG";
            if (iso2.Equals("HU", StringComparison.InvariantCultureIgnoreCase)) return "HUN";
            if (iso2.Equals("IS", StringComparison.InvariantCultureIgnoreCase)) return "ISL";
            if (iso2.Equals("IN", StringComparison.InvariantCultureIgnoreCase)) return "IND";
            if (iso2.Equals("ID", StringComparison.InvariantCultureIgnoreCase)) return "IDN";
            if (iso2.Equals("IR", StringComparison.InvariantCultureIgnoreCase)) return "IRN";
            if (iso2.Equals("IQ", StringComparison.InvariantCultureIgnoreCase)) return "IRQ";
            if (iso2.Equals("IE", StringComparison.InvariantCultureIgnoreCase)) return "IRL";
            if (iso2.Equals("IM", StringComparison.InvariantCultureIgnoreCase)) return "IMN";
            if (iso2.Equals("IL", StringComparison.InvariantCultureIgnoreCase)) return "ISR";
            if (iso2.Equals("IT", StringComparison.InvariantCultureIgnoreCase)) return "ITA";
            if (iso2.Equals("JM", StringComparison.InvariantCultureIgnoreCase)) return "JAM";
            if (iso2.Equals("JP", StringComparison.InvariantCultureIgnoreCase)) return "JPN";
            if (iso2.Equals("JE", StringComparison.InvariantCultureIgnoreCase)) return "JEY";
            if (iso2.Equals("JO", StringComparison.InvariantCultureIgnoreCase)) return "JOR";
            if (iso2.Equals("KZ", StringComparison.InvariantCultureIgnoreCase)) return "KAZ";
            if (iso2.Equals("KE", StringComparison.InvariantCultureIgnoreCase)) return "KEN";
            if (iso2.Equals("KI", StringComparison.InvariantCultureIgnoreCase)) return "KIR";
            if (iso2.Equals("KP", StringComparison.InvariantCultureIgnoreCase)) return "PRK";
            if (iso2.Equals("KR", StringComparison.InvariantCultureIgnoreCase)) return "KOR";
            if (iso2.Equals("KW", StringComparison.InvariantCultureIgnoreCase)) return "KWT";
            if (iso2.Equals("KG", StringComparison.InvariantCultureIgnoreCase)) return "KGZ";
            if (iso2.Equals("LA", StringComparison.InvariantCultureIgnoreCase)) return "LAO";
            if (iso2.Equals("LV", StringComparison.InvariantCultureIgnoreCase)) return "LVA";
            if (iso2.Equals("LB", StringComparison.InvariantCultureIgnoreCase)) return "LBN";
            if (iso2.Equals("LS", StringComparison.InvariantCultureIgnoreCase)) return "LSO";
            if (iso2.Equals("LR", StringComparison.InvariantCultureIgnoreCase)) return "LBR";
            if (iso2.Equals("LY", StringComparison.InvariantCultureIgnoreCase)) return "LBY";
            if (iso2.Equals("LI", StringComparison.InvariantCultureIgnoreCase)) return "LIE";
            if (iso2.Equals("LT", StringComparison.InvariantCultureIgnoreCase)) return "LTU";
            if (iso2.Equals("LU", StringComparison.InvariantCultureIgnoreCase)) return "LUX";
            if (iso2.Equals("MO", StringComparison.InvariantCultureIgnoreCase)) return "MAC";
            if (iso2.Equals("MG", StringComparison.InvariantCultureIgnoreCase)) return "MDG";
            if (iso2.Equals("MW", StringComparison.InvariantCultureIgnoreCase)) return "MWI";
            if (iso2.Equals("MY", StringComparison.InvariantCultureIgnoreCase)) return "MYS";
            if (iso2.Equals("MV", StringComparison.InvariantCultureIgnoreCase)) return "MDV";
            if (iso2.Equals("ML", StringComparison.InvariantCultureIgnoreCase)) return "MLI";
            if (iso2.Equals("MT", StringComparison.InvariantCultureIgnoreCase)) return "MLT";
            if (iso2.Equals("MH", StringComparison.InvariantCultureIgnoreCase)) return "MHL";
            if (iso2.Equals("MQ", StringComparison.InvariantCultureIgnoreCase)) return "MTQ";
            if (iso2.Equals("MR", StringComparison.InvariantCultureIgnoreCase)) return "MRT";
            if (iso2.Equals("MU", StringComparison.InvariantCultureIgnoreCase)) return "MUS";
            if (iso2.Equals("YT", StringComparison.InvariantCultureIgnoreCase)) return "MYT";
            if (iso2.Equals("MX", StringComparison.InvariantCultureIgnoreCase)) return "MEX";
            if (iso2.Equals("FM", StringComparison.InvariantCultureIgnoreCase)) return "FSM";
            if (iso2.Equals("MD", StringComparison.InvariantCultureIgnoreCase)) return "MDA";
            if (iso2.Equals("MC", StringComparison.InvariantCultureIgnoreCase)) return "MCO";
            if (iso2.Equals("MN", StringComparison.InvariantCultureIgnoreCase)) return "MNG";
            if (iso2.Equals("ME", StringComparison.InvariantCultureIgnoreCase)) return "MNE";
            if (iso2.Equals("MS", StringComparison.InvariantCultureIgnoreCase)) return "MSR";
            if (iso2.Equals("MA", StringComparison.InvariantCultureIgnoreCase)) return "MAR";
            if (iso2.Equals("MZ", StringComparison.InvariantCultureIgnoreCase)) return "MOZ";
            if (iso2.Equals("MM", StringComparison.InvariantCultureIgnoreCase)) return "MMR";
            if (iso2.Equals("NA", StringComparison.InvariantCultureIgnoreCase)) return "NAM";
            if (iso2.Equals("NR", StringComparison.InvariantCultureIgnoreCase)) return "NRU";
            if (iso2.Equals("NP", StringComparison.InvariantCultureIgnoreCase)) return "NPL";
            if (iso2.Equals("NL", StringComparison.InvariantCultureIgnoreCase)) return "NLD";
            if (iso2.Equals("NC", StringComparison.InvariantCultureIgnoreCase)) return "NCL";
            if (iso2.Equals("NZ", StringComparison.InvariantCultureIgnoreCase)) return "NZL";
            if (iso2.Equals("NI", StringComparison.InvariantCultureIgnoreCase)) return "NIC";
            if (iso2.Equals("NE", StringComparison.InvariantCultureIgnoreCase)) return "NER";
            if (iso2.Equals("NG", StringComparison.InvariantCultureIgnoreCase)) return "NGA";
            if (iso2.Equals("NU", StringComparison.InvariantCultureIgnoreCase)) return "NIU";
            if (iso2.Equals("NF", StringComparison.InvariantCultureIgnoreCase)) return "NFK";
            if (iso2.Equals("MK", StringComparison.InvariantCultureIgnoreCase)) return "MKD";
            if (iso2.Equals("MP", StringComparison.InvariantCultureIgnoreCase)) return "MNP";
            if (iso2.Equals("NO", StringComparison.InvariantCultureIgnoreCase)) return "NOR";
            if (iso2.Equals("OM", StringComparison.InvariantCultureIgnoreCase)) return "OMN";
            if (iso2.Equals("PK", StringComparison.InvariantCultureIgnoreCase)) return "PAK";
            if (iso2.Equals("PW", StringComparison.InvariantCultureIgnoreCase)) return "PLW";
            if (iso2.Equals("PS", StringComparison.InvariantCultureIgnoreCase)) return "PSE";
            if (iso2.Equals("PA", StringComparison.InvariantCultureIgnoreCase)) return "PAN";
            if (iso2.Equals("PG", StringComparison.InvariantCultureIgnoreCase)) return "PNG";
            if (iso2.Equals("PY", StringComparison.InvariantCultureIgnoreCase)) return "PRY";
            if (iso2.Equals("PE", StringComparison.InvariantCultureIgnoreCase)) return "PER";
            if (iso2.Equals("PH", StringComparison.InvariantCultureIgnoreCase)) return "PHL";
            if (iso2.Equals("PN", StringComparison.InvariantCultureIgnoreCase)) return "PCN";
            if (iso2.Equals("PL", StringComparison.InvariantCultureIgnoreCase)) return "POL";
            if (iso2.Equals("PT", StringComparison.InvariantCultureIgnoreCase)) return "PRT";
            if (iso2.Equals("PR", StringComparison.InvariantCultureIgnoreCase)) return "PRI";
            if (iso2.Equals("QA", StringComparison.InvariantCultureIgnoreCase)) return "QAT";
            if (iso2.Equals("RO", StringComparison.InvariantCultureIgnoreCase)) return "ROU";
            if (iso2.Equals("RU", StringComparison.InvariantCultureIgnoreCase)) return "RUS";
            if (iso2.Equals("RW", StringComparison.InvariantCultureIgnoreCase)) return "RWA";
            if (iso2.Equals("RE", StringComparison.InvariantCultureIgnoreCase)) return "REU";
            if (iso2.Equals("BL", StringComparison.InvariantCultureIgnoreCase)) return "BLM";
            if (iso2.Equals("SH", StringComparison.InvariantCultureIgnoreCase)) return "SHN";
            if (iso2.Equals("KN", StringComparison.InvariantCultureIgnoreCase)) return "KNA";
            if (iso2.Equals("LC", StringComparison.InvariantCultureIgnoreCase)) return "LCA";
            if (iso2.Equals("MF", StringComparison.InvariantCultureIgnoreCase)) return "MAF";
            if (iso2.Equals("PM", StringComparison.InvariantCultureIgnoreCase)) return "SPM";
            if (iso2.Equals("VC", StringComparison.InvariantCultureIgnoreCase)) return "VCT";
            if (iso2.Equals("WS", StringComparison.InvariantCultureIgnoreCase)) return "WSM";
            if (iso2.Equals("SM", StringComparison.InvariantCultureIgnoreCase)) return "SMR";
            if (iso2.Equals("ST", StringComparison.InvariantCultureIgnoreCase)) return "STP";
            if (iso2.Equals("SA", StringComparison.InvariantCultureIgnoreCase)) return "SAU";
            if (iso2.Equals("SN", StringComparison.InvariantCultureIgnoreCase)) return "SEN";
            if (iso2.Equals("RS", StringComparison.InvariantCultureIgnoreCase)) return "SRB";
            if (iso2.Equals("SC", StringComparison.InvariantCultureIgnoreCase)) return "SYC";
            if (iso2.Equals("SL", StringComparison.InvariantCultureIgnoreCase)) return "SLE";
            if (iso2.Equals("SG", StringComparison.InvariantCultureIgnoreCase)) return "SGP";
            if (iso2.Equals("SX", StringComparison.InvariantCultureIgnoreCase)) return "SXM";
            if (iso2.Equals("SK", StringComparison.InvariantCultureIgnoreCase)) return "SVK";
            if (iso2.Equals("SI", StringComparison.InvariantCultureIgnoreCase)) return "SVN";
            if (iso2.Equals("SB", StringComparison.InvariantCultureIgnoreCase)) return "SLB";
            if (iso2.Equals("SO", StringComparison.InvariantCultureIgnoreCase)) return "SOM";
            if (iso2.Equals("ZA", StringComparison.InvariantCultureIgnoreCase)) return "ZAF";
            if (iso2.Equals("GS", StringComparison.InvariantCultureIgnoreCase)) return "SGS";
            if (iso2.Equals("SS", StringComparison.InvariantCultureIgnoreCase)) return "SSD";
            if (iso2.Equals("ES", StringComparison.InvariantCultureIgnoreCase)) return "ESP";
            if (iso2.Equals("LK", StringComparison.InvariantCultureIgnoreCase)) return "LKA";
            if (iso2.Equals("SD", StringComparison.InvariantCultureIgnoreCase)) return "SDN";
            if (iso2.Equals("SR", StringComparison.InvariantCultureIgnoreCase)) return "SUR";
            if (iso2.Equals("SJ", StringComparison.InvariantCultureIgnoreCase)) return "SJM";
            if (iso2.Equals("SE", StringComparison.InvariantCultureIgnoreCase)) return "SWE";
            if (iso2.Equals("CH", StringComparison.InvariantCultureIgnoreCase)) return "CHE";
            if (iso2.Equals("SY", StringComparison.InvariantCultureIgnoreCase)) return "SYR";
            if (iso2.Equals("TW", StringComparison.InvariantCultureIgnoreCase)) return "TWN";
            if (iso2.Equals("TJ", StringComparison.InvariantCultureIgnoreCase)) return "TJK";
            if (iso2.Equals("TZ", StringComparison.InvariantCultureIgnoreCase)) return "TZA";
            if (iso2.Equals("TH", StringComparison.InvariantCultureIgnoreCase)) return "THA";
            if (iso2.Equals("TL", StringComparison.InvariantCultureIgnoreCase)) return "TLS";
            if (iso2.Equals("TG", StringComparison.InvariantCultureIgnoreCase)) return "TGO";
            if (iso2.Equals("TK", StringComparison.InvariantCultureIgnoreCase)) return "TKL";
            if (iso2.Equals("TO", StringComparison.InvariantCultureIgnoreCase)) return "TON";
            if (iso2.Equals("TT", StringComparison.InvariantCultureIgnoreCase)) return "TTO";
            if (iso2.Equals("TN", StringComparison.InvariantCultureIgnoreCase)) return "TUN";
            if (iso2.Equals("TR", StringComparison.InvariantCultureIgnoreCase)) return "TUR";
            if (iso2.Equals("TM", StringComparison.InvariantCultureIgnoreCase)) return "TKM";
            if (iso2.Equals("TC", StringComparison.InvariantCultureIgnoreCase)) return "TCA";
            if (iso2.Equals("TV", StringComparison.InvariantCultureIgnoreCase)) return "TUV";
            if (iso2.Equals("UG", StringComparison.InvariantCultureIgnoreCase)) return "UGA";
            if (iso2.Equals("UA", StringComparison.InvariantCultureIgnoreCase)) return "UKR";
            if (iso2.Equals("AE", StringComparison.InvariantCultureIgnoreCase)) return "ARE";
            if (iso2.Equals("GB", StringComparison.InvariantCultureIgnoreCase)) return "GBR";
            if (iso2.Equals("UM", StringComparison.InvariantCultureIgnoreCase)) return "UMI";
            if (iso2.Equals("US", StringComparison.InvariantCultureIgnoreCase)) return "USA";
            if (iso2.Equals("UY", StringComparison.InvariantCultureIgnoreCase)) return "URY";
            if (iso2.Equals("UZ", StringComparison.InvariantCultureIgnoreCase)) return "UZB";
            if (iso2.Equals("VU", StringComparison.InvariantCultureIgnoreCase)) return "VUT";
            if (iso2.Equals("VE", StringComparison.InvariantCultureIgnoreCase)) return "VEN";
            if (iso2.Equals("VN", StringComparison.InvariantCultureIgnoreCase)) return "VNM";
            if (iso2.Equals("VG", StringComparison.InvariantCultureIgnoreCase)) return "VGB";
            if (iso2.Equals("VI", StringComparison.InvariantCultureIgnoreCase)) return "VIR";
            if (iso2.Equals("WF", StringComparison.InvariantCultureIgnoreCase)) return "WLF";
            if (iso2.Equals("EH", StringComparison.InvariantCultureIgnoreCase)) return "ESH";
            if (iso2.Equals("YE", StringComparison.InvariantCultureIgnoreCase)) return "YEM";
            if (iso2.Equals("ZM", StringComparison.InvariantCultureIgnoreCase)) return "ZMB";
            if (iso2.Equals("ZW", StringComparison.InvariantCultureIgnoreCase)) return "ZWE";
            if (iso2.Equals("AX", StringComparison.InvariantCultureIgnoreCase)) return "ALA";

            return null;
        }

        public static string Iso3ToIso2(string iso3)
        {
            if (iso3.IsNullOrEmpty())
                return null;

            if (iso3.Equals("AFG", StringComparison.InvariantCultureIgnoreCase)) return "AF";
            if (iso3.Equals("ALB", StringComparison.InvariantCultureIgnoreCase)) return "AL";
            if (iso3.Equals("DZA", StringComparison.InvariantCultureIgnoreCase)) return "DZ";
            if (iso3.Equals("ASM", StringComparison.InvariantCultureIgnoreCase)) return "AS";
            if (iso3.Equals("AND", StringComparison.InvariantCultureIgnoreCase)) return "AD";
            if (iso3.Equals("AGO", StringComparison.InvariantCultureIgnoreCase)) return "AO";
            if (iso3.Equals("AIA", StringComparison.InvariantCultureIgnoreCase)) return "AI";
            if (iso3.Equals("ATA", StringComparison.InvariantCultureIgnoreCase)) return "AQ";
            if (iso3.Equals("ATG", StringComparison.InvariantCultureIgnoreCase)) return "AG";
            if (iso3.Equals("ARG", StringComparison.InvariantCultureIgnoreCase)) return "AR";
            if (iso3.Equals("ARM", StringComparison.InvariantCultureIgnoreCase)) return "AM";
            if (iso3.Equals("ABW", StringComparison.InvariantCultureIgnoreCase)) return "AW";
            if (iso3.Equals("AUS", StringComparison.InvariantCultureIgnoreCase)) return "AU";
            if (iso3.Equals("AUT", StringComparison.InvariantCultureIgnoreCase)) return "AT";
            if (iso3.Equals("AZE", StringComparison.InvariantCultureIgnoreCase)) return "AZ";
            if (iso3.Equals("BHS", StringComparison.InvariantCultureIgnoreCase)) return "BS";
            if (iso3.Equals("BHR", StringComparison.InvariantCultureIgnoreCase)) return "BH";
            if (iso3.Equals("BGD", StringComparison.InvariantCultureIgnoreCase)) return "BD";
            if (iso3.Equals("BRB", StringComparison.InvariantCultureIgnoreCase)) return "BB";
            if (iso3.Equals("BLR", StringComparison.InvariantCultureIgnoreCase)) return "BY";
            if (iso3.Equals("BEL", StringComparison.InvariantCultureIgnoreCase)) return "BE";
            if (iso3.Equals("BLZ", StringComparison.InvariantCultureIgnoreCase)) return "BZ";
            if (iso3.Equals("BEN", StringComparison.InvariantCultureIgnoreCase)) return "BJ";
            if (iso3.Equals("BMU", StringComparison.InvariantCultureIgnoreCase)) return "BM";
            if (iso3.Equals("BTN", StringComparison.InvariantCultureIgnoreCase)) return "BT";
            if (iso3.Equals("BOL", StringComparison.InvariantCultureIgnoreCase)) return "BO";
            if (iso3.Equals("BES", StringComparison.InvariantCultureIgnoreCase)) return "BQ";
            if (iso3.Equals("BIH", StringComparison.InvariantCultureIgnoreCase)) return "BA";
            if (iso3.Equals("BWA", StringComparison.InvariantCultureIgnoreCase)) return "BW";
            if (iso3.Equals("BVT", StringComparison.InvariantCultureIgnoreCase)) return "BV";
            if (iso3.Equals("BRA", StringComparison.InvariantCultureIgnoreCase)) return "BR";
            if (iso3.Equals("IOT", StringComparison.InvariantCultureIgnoreCase)) return "IO";
            if (iso3.Equals("BRN", StringComparison.InvariantCultureIgnoreCase)) return "BN";
            if (iso3.Equals("BGR", StringComparison.InvariantCultureIgnoreCase)) return "BG";
            if (iso3.Equals("BFA", StringComparison.InvariantCultureIgnoreCase)) return "BF";
            if (iso3.Equals("BDI", StringComparison.InvariantCultureIgnoreCase)) return "BI";
            if (iso3.Equals("CPV", StringComparison.InvariantCultureIgnoreCase)) return "CV";
            if (iso3.Equals("KHM", StringComparison.InvariantCultureIgnoreCase)) return "KH";
            if (iso3.Equals("CMR", StringComparison.InvariantCultureIgnoreCase)) return "CM";
            if (iso3.Equals("CAN", StringComparison.InvariantCultureIgnoreCase)) return "CA";
            if (iso3.Equals("CYM", StringComparison.InvariantCultureIgnoreCase)) return "KY";
            if (iso3.Equals("CAF", StringComparison.InvariantCultureIgnoreCase)) return "CF";
            if (iso3.Equals("TCD", StringComparison.InvariantCultureIgnoreCase)) return "TD";
            if (iso3.Equals("CHL", StringComparison.InvariantCultureIgnoreCase)) return "CL";
            if (iso3.Equals("CHN", StringComparison.InvariantCultureIgnoreCase)) return "CN";
            if (iso3.Equals("CXR", StringComparison.InvariantCultureIgnoreCase)) return "CX";
            if (iso3.Equals("CCK", StringComparison.InvariantCultureIgnoreCase)) return "CC";
            if (iso3.Equals("COL", StringComparison.InvariantCultureIgnoreCase)) return "CO";
            if (iso3.Equals("COM", StringComparison.InvariantCultureIgnoreCase)) return "KM";
            if (iso3.Equals("COD", StringComparison.InvariantCultureIgnoreCase)) return "CD";
            if (iso3.Equals("COG", StringComparison.InvariantCultureIgnoreCase)) return "CG";
            if (iso3.Equals("COK", StringComparison.InvariantCultureIgnoreCase)) return "CK";
            if (iso3.Equals("CRI", StringComparison.InvariantCultureIgnoreCase)) return "CR";
            if (iso3.Equals("HRV", StringComparison.InvariantCultureIgnoreCase)) return "HR";
            if (iso3.Equals("CUB", StringComparison.InvariantCultureIgnoreCase)) return "CU";
            if (iso3.Equals("CUW", StringComparison.InvariantCultureIgnoreCase)) return "CW";
            if (iso3.Equals("CYP", StringComparison.InvariantCultureIgnoreCase)) return "CY";
            if (iso3.Equals("CZE", StringComparison.InvariantCultureIgnoreCase)) return "CZ";
            if (iso3.Equals("CIV", StringComparison.InvariantCultureIgnoreCase)) return "CI";
            if (iso3.Equals("DNK", StringComparison.InvariantCultureIgnoreCase)) return "DK";
            if (iso3.Equals("DJI", StringComparison.InvariantCultureIgnoreCase)) return "DJ";
            if (iso3.Equals("DMA", StringComparison.InvariantCultureIgnoreCase)) return "DM";
            if (iso3.Equals("DOM", StringComparison.InvariantCultureIgnoreCase)) return "DO";
            if (iso3.Equals("ECU", StringComparison.InvariantCultureIgnoreCase)) return "EC";
            if (iso3.Equals("EGY", StringComparison.InvariantCultureIgnoreCase)) return "EG";
            if (iso3.Equals("SLV", StringComparison.InvariantCultureIgnoreCase)) return "SV";
            if (iso3.Equals("GNQ", StringComparison.InvariantCultureIgnoreCase)) return "GQ";
            if (iso3.Equals("ERI", StringComparison.InvariantCultureIgnoreCase)) return "ER";
            if (iso3.Equals("EST", StringComparison.InvariantCultureIgnoreCase)) return "EE";
            if (iso3.Equals("SWZ", StringComparison.InvariantCultureIgnoreCase)) return "SZ";
            if (iso3.Equals("ETH", StringComparison.InvariantCultureIgnoreCase)) return "ET";
            if (iso3.Equals("FLK", StringComparison.InvariantCultureIgnoreCase)) return "FK";
            if (iso3.Equals("FRO", StringComparison.InvariantCultureIgnoreCase)) return "FO";
            if (iso3.Equals("FJI", StringComparison.InvariantCultureIgnoreCase)) return "FJ";
            if (iso3.Equals("FIN", StringComparison.InvariantCultureIgnoreCase)) return "FI";
            if (iso3.Equals("FRA", StringComparison.InvariantCultureIgnoreCase)) return "FR";
            if (iso3.Equals("GUF", StringComparison.InvariantCultureIgnoreCase)) return "GF";
            if (iso3.Equals("PYF", StringComparison.InvariantCultureIgnoreCase)) return "PF";
            if (iso3.Equals("ATF", StringComparison.InvariantCultureIgnoreCase)) return "TF";
            if (iso3.Equals("GAB", StringComparison.InvariantCultureIgnoreCase)) return "GA";
            if (iso3.Equals("GMB", StringComparison.InvariantCultureIgnoreCase)) return "GM";
            if (iso3.Equals("GEO", StringComparison.InvariantCultureIgnoreCase)) return "GE";
            if (iso3.Equals("DEU", StringComparison.InvariantCultureIgnoreCase)) return "DE";
            if (iso3.Equals("GHA", StringComparison.InvariantCultureIgnoreCase)) return "GH";
            if (iso3.Equals("GIB", StringComparison.InvariantCultureIgnoreCase)) return "GI";
            if (iso3.Equals("GRC", StringComparison.InvariantCultureIgnoreCase)) return "GR";
            if (iso3.Equals("GRL", StringComparison.InvariantCultureIgnoreCase)) return "GL";
            if (iso3.Equals("GRD", StringComparison.InvariantCultureIgnoreCase)) return "GD";
            if (iso3.Equals("GLP", StringComparison.InvariantCultureIgnoreCase)) return "GP";
            if (iso3.Equals("GUM", StringComparison.InvariantCultureIgnoreCase)) return "GU";
            if (iso3.Equals("GTM", StringComparison.InvariantCultureIgnoreCase)) return "GT";
            if (iso3.Equals("GGY", StringComparison.InvariantCultureIgnoreCase)) return "GG";
            if (iso3.Equals("GIN", StringComparison.InvariantCultureIgnoreCase)) return "GN";
            if (iso3.Equals("GNB", StringComparison.InvariantCultureIgnoreCase)) return "GW";
            if (iso3.Equals("GUY", StringComparison.InvariantCultureIgnoreCase)) return "GY";
            if (iso3.Equals("HTI", StringComparison.InvariantCultureIgnoreCase)) return "HT";
            if (iso3.Equals("HMD", StringComparison.InvariantCultureIgnoreCase)) return "HM";
            if (iso3.Equals("VAT", StringComparison.InvariantCultureIgnoreCase)) return "VA";
            if (iso3.Equals("HND", StringComparison.InvariantCultureIgnoreCase)) return "HN";
            if (iso3.Equals("HKG", StringComparison.InvariantCultureIgnoreCase)) return "HK";
            if (iso3.Equals("HUN", StringComparison.InvariantCultureIgnoreCase)) return "HU";
            if (iso3.Equals("ISL", StringComparison.InvariantCultureIgnoreCase)) return "IS";
            if (iso3.Equals("IND", StringComparison.InvariantCultureIgnoreCase)) return "IN";
            if (iso3.Equals("IDN", StringComparison.InvariantCultureIgnoreCase)) return "ID";
            if (iso3.Equals("IRN", StringComparison.InvariantCultureIgnoreCase)) return "IR";
            if (iso3.Equals("IRQ", StringComparison.InvariantCultureIgnoreCase)) return "IQ";
            if (iso3.Equals("IRL", StringComparison.InvariantCultureIgnoreCase)) return "IE";
            if (iso3.Equals("IMN", StringComparison.InvariantCultureIgnoreCase)) return "IM";
            if (iso3.Equals("ISR", StringComparison.InvariantCultureIgnoreCase)) return "IL";
            if (iso3.Equals("ITA", StringComparison.InvariantCultureIgnoreCase)) return "IT";
            if (iso3.Equals("JAM", StringComparison.InvariantCultureIgnoreCase)) return "JM";
            if (iso3.Equals("JPN", StringComparison.InvariantCultureIgnoreCase)) return "JP";
            if (iso3.Equals("JEY", StringComparison.InvariantCultureIgnoreCase)) return "JE";
            if (iso3.Equals("JOR", StringComparison.InvariantCultureIgnoreCase)) return "JO";
            if (iso3.Equals("KAZ", StringComparison.InvariantCultureIgnoreCase)) return "KZ";
            if (iso3.Equals("KEN", StringComparison.InvariantCultureIgnoreCase)) return "KE";
            if (iso3.Equals("KIR", StringComparison.InvariantCultureIgnoreCase)) return "KI";
            if (iso3.Equals("PRK", StringComparison.InvariantCultureIgnoreCase)) return "KP";
            if (iso3.Equals("KOR", StringComparison.InvariantCultureIgnoreCase)) return "KR";
            if (iso3.Equals("KWT", StringComparison.InvariantCultureIgnoreCase)) return "KW";
            if (iso3.Equals("KGZ", StringComparison.InvariantCultureIgnoreCase)) return "KG";
            if (iso3.Equals("LAO", StringComparison.InvariantCultureIgnoreCase)) return "LA";
            if (iso3.Equals("LVA", StringComparison.InvariantCultureIgnoreCase)) return "LV";
            if (iso3.Equals("LBN", StringComparison.InvariantCultureIgnoreCase)) return "LB";
            if (iso3.Equals("LSO", StringComparison.InvariantCultureIgnoreCase)) return "LS";
            if (iso3.Equals("LBR", StringComparison.InvariantCultureIgnoreCase)) return "LR";
            if (iso3.Equals("LBY", StringComparison.InvariantCultureIgnoreCase)) return "LY";
            if (iso3.Equals("LIE", StringComparison.InvariantCultureIgnoreCase)) return "LI";
            if (iso3.Equals("LTU", StringComparison.InvariantCultureIgnoreCase)) return "LT";
            if (iso3.Equals("LUX", StringComparison.InvariantCultureIgnoreCase)) return "LU";
            if (iso3.Equals("MAC", StringComparison.InvariantCultureIgnoreCase)) return "MO";
            if (iso3.Equals("MDG", StringComparison.InvariantCultureIgnoreCase)) return "MG";
            if (iso3.Equals("MWI", StringComparison.InvariantCultureIgnoreCase)) return "MW";
            if (iso3.Equals("MYS", StringComparison.InvariantCultureIgnoreCase)) return "MY";
            if (iso3.Equals("MDV", StringComparison.InvariantCultureIgnoreCase)) return "MV";
            if (iso3.Equals("MLI", StringComparison.InvariantCultureIgnoreCase)) return "ML";
            if (iso3.Equals("MLT", StringComparison.InvariantCultureIgnoreCase)) return "MT";
            if (iso3.Equals("MHL", StringComparison.InvariantCultureIgnoreCase)) return "MH";
            if (iso3.Equals("MTQ", StringComparison.InvariantCultureIgnoreCase)) return "MQ";
            if (iso3.Equals("MRT", StringComparison.InvariantCultureIgnoreCase)) return "MR";
            if (iso3.Equals("MUS", StringComparison.InvariantCultureIgnoreCase)) return "MU";
            if (iso3.Equals("MYT", StringComparison.InvariantCultureIgnoreCase)) return "YT";
            if (iso3.Equals("MEX", StringComparison.InvariantCultureIgnoreCase)) return "MX";
            if (iso3.Equals("FSM", StringComparison.InvariantCultureIgnoreCase)) return "FM";
            if (iso3.Equals("MDA", StringComparison.InvariantCultureIgnoreCase)) return "MD";
            if (iso3.Equals("MCO", StringComparison.InvariantCultureIgnoreCase)) return "MC";
            if (iso3.Equals("MNG", StringComparison.InvariantCultureIgnoreCase)) return "MN";
            if (iso3.Equals("MNE", StringComparison.InvariantCultureIgnoreCase)) return "ME";
            if (iso3.Equals("MSR", StringComparison.InvariantCultureIgnoreCase)) return "MS";
            if (iso3.Equals("MAR", StringComparison.InvariantCultureIgnoreCase)) return "MA";
            if (iso3.Equals("MOZ", StringComparison.InvariantCultureIgnoreCase)) return "MZ";
            if (iso3.Equals("MMR", StringComparison.InvariantCultureIgnoreCase)) return "MM";
            if (iso3.Equals("NAM", StringComparison.InvariantCultureIgnoreCase)) return "NA";
            if (iso3.Equals("NRU", StringComparison.InvariantCultureIgnoreCase)) return "NR";
            if (iso3.Equals("NPL", StringComparison.InvariantCultureIgnoreCase)) return "NP";
            if (iso3.Equals("NLD", StringComparison.InvariantCultureIgnoreCase)) return "NL";
            if (iso3.Equals("NCL", StringComparison.InvariantCultureIgnoreCase)) return "NC";
            if (iso3.Equals("NZL", StringComparison.InvariantCultureIgnoreCase)) return "NZ";
            if (iso3.Equals("NIC", StringComparison.InvariantCultureIgnoreCase)) return "NI";
            if (iso3.Equals("NER", StringComparison.InvariantCultureIgnoreCase)) return "NE";
            if (iso3.Equals("NGA", StringComparison.InvariantCultureIgnoreCase)) return "NG";
            if (iso3.Equals("NIU", StringComparison.InvariantCultureIgnoreCase)) return "NU";
            if (iso3.Equals("NFK", StringComparison.InvariantCultureIgnoreCase)) return "NF";
            if (iso3.Equals("MKD", StringComparison.InvariantCultureIgnoreCase)) return "MK";
            if (iso3.Equals("MNP", StringComparison.InvariantCultureIgnoreCase)) return "MP";
            if (iso3.Equals("NOR", StringComparison.InvariantCultureIgnoreCase)) return "NO";
            if (iso3.Equals("OMN", StringComparison.InvariantCultureIgnoreCase)) return "OM";
            if (iso3.Equals("PAK", StringComparison.InvariantCultureIgnoreCase)) return "PK";
            if (iso3.Equals("PLW", StringComparison.InvariantCultureIgnoreCase)) return "PW";
            if (iso3.Equals("PSE", StringComparison.InvariantCultureIgnoreCase)) return "PS";
            if (iso3.Equals("PAN", StringComparison.InvariantCultureIgnoreCase)) return "PA";
            if (iso3.Equals("PNG", StringComparison.InvariantCultureIgnoreCase)) return "PG";
            if (iso3.Equals("PRY", StringComparison.InvariantCultureIgnoreCase)) return "PY";
            if (iso3.Equals("PER", StringComparison.InvariantCultureIgnoreCase)) return "PE";
            if (iso3.Equals("PHL", StringComparison.InvariantCultureIgnoreCase)) return "PH";
            if (iso3.Equals("PCN", StringComparison.InvariantCultureIgnoreCase)) return "PN";
            if (iso3.Equals("POL", StringComparison.InvariantCultureIgnoreCase)) return "PL";
            if (iso3.Equals("PRT", StringComparison.InvariantCultureIgnoreCase)) return "PT";
            if (iso3.Equals("PRI", StringComparison.InvariantCultureIgnoreCase)) return "PR";
            if (iso3.Equals("QAT", StringComparison.InvariantCultureIgnoreCase)) return "QA";
            if (iso3.Equals("ROU", StringComparison.InvariantCultureIgnoreCase)) return "RO";
            if (iso3.Equals("RUS", StringComparison.InvariantCultureIgnoreCase)) return "RU";
            if (iso3.Equals("RWA", StringComparison.InvariantCultureIgnoreCase)) return "RW";
            if (iso3.Equals("REU", StringComparison.InvariantCultureIgnoreCase)) return "RE";
            if (iso3.Equals("BLM", StringComparison.InvariantCultureIgnoreCase)) return "BL";
            if (iso3.Equals("SHN", StringComparison.InvariantCultureIgnoreCase)) return "SH";
            if (iso3.Equals("KNA", StringComparison.InvariantCultureIgnoreCase)) return "KN";
            if (iso3.Equals("LCA", StringComparison.InvariantCultureIgnoreCase)) return "LC";
            if (iso3.Equals("MAF", StringComparison.InvariantCultureIgnoreCase)) return "MF";
            if (iso3.Equals("SPM", StringComparison.InvariantCultureIgnoreCase)) return "PM";
            if (iso3.Equals("VCT", StringComparison.InvariantCultureIgnoreCase)) return "VC";
            if (iso3.Equals("WSM", StringComparison.InvariantCultureIgnoreCase)) return "WS";
            if (iso3.Equals("SMR", StringComparison.InvariantCultureIgnoreCase)) return "SM";
            if (iso3.Equals("STP", StringComparison.InvariantCultureIgnoreCase)) return "ST";
            if (iso3.Equals("SAU", StringComparison.InvariantCultureIgnoreCase)) return "SA";
            if (iso3.Equals("SEN", StringComparison.InvariantCultureIgnoreCase)) return "SN";
            if (iso3.Equals("SRB", StringComparison.InvariantCultureIgnoreCase)) return "RS";
            if (iso3.Equals("SYC", StringComparison.InvariantCultureIgnoreCase)) return "SC";
            if (iso3.Equals("SLE", StringComparison.InvariantCultureIgnoreCase)) return "SL";
            if (iso3.Equals("SGP", StringComparison.InvariantCultureIgnoreCase)) return "SG";
            if (iso3.Equals("SXM", StringComparison.InvariantCultureIgnoreCase)) return "SX";
            if (iso3.Equals("SVK", StringComparison.InvariantCultureIgnoreCase)) return "SK";
            if (iso3.Equals("SVN", StringComparison.InvariantCultureIgnoreCase)) return "SI";
            if (iso3.Equals("SLB", StringComparison.InvariantCultureIgnoreCase)) return "SB";
            if (iso3.Equals("SOM", StringComparison.InvariantCultureIgnoreCase)) return "SO";
            if (iso3.Equals("ZAF", StringComparison.InvariantCultureIgnoreCase)) return "ZA";
            if (iso3.Equals("SGS", StringComparison.InvariantCultureIgnoreCase)) return "GS";
            if (iso3.Equals("SSD", StringComparison.InvariantCultureIgnoreCase)) return "SS";
            if (iso3.Equals("ESP", StringComparison.InvariantCultureIgnoreCase)) return "ES";
            if (iso3.Equals("LKA", StringComparison.InvariantCultureIgnoreCase)) return "LK";
            if (iso3.Equals("SDN", StringComparison.InvariantCultureIgnoreCase)) return "SD";
            if (iso3.Equals("SUR", StringComparison.InvariantCultureIgnoreCase)) return "SR";
            if (iso3.Equals("SJM", StringComparison.InvariantCultureIgnoreCase)) return "SJ";
            if (iso3.Equals("SWE", StringComparison.InvariantCultureIgnoreCase)) return "SE";
            if (iso3.Equals("CHE", StringComparison.InvariantCultureIgnoreCase)) return "CH";
            if (iso3.Equals("SYR", StringComparison.InvariantCultureIgnoreCase)) return "SY";
            if (iso3.Equals("TWN", StringComparison.InvariantCultureIgnoreCase)) return "TW";
            if (iso3.Equals("TJK", StringComparison.InvariantCultureIgnoreCase)) return "TJ";
            if (iso3.Equals("TZA", StringComparison.InvariantCultureIgnoreCase)) return "TZ";
            if (iso3.Equals("THA", StringComparison.InvariantCultureIgnoreCase)) return "TH";
            if (iso3.Equals("TLS", StringComparison.InvariantCultureIgnoreCase)) return "TL";
            if (iso3.Equals("TGO", StringComparison.InvariantCultureIgnoreCase)) return "TG";
            if (iso3.Equals("TKL", StringComparison.InvariantCultureIgnoreCase)) return "TK";
            if (iso3.Equals("TON", StringComparison.InvariantCultureIgnoreCase)) return "TO";
            if (iso3.Equals("TTO", StringComparison.InvariantCultureIgnoreCase)) return "TT";
            if (iso3.Equals("TUN", StringComparison.InvariantCultureIgnoreCase)) return "TN";
            if (iso3.Equals("TUR", StringComparison.InvariantCultureIgnoreCase)) return "TR";
            if (iso3.Equals("TKM", StringComparison.InvariantCultureIgnoreCase)) return "TM";
            if (iso3.Equals("TCA", StringComparison.InvariantCultureIgnoreCase)) return "TC";
            if (iso3.Equals("TUV", StringComparison.InvariantCultureIgnoreCase)) return "TV";
            if (iso3.Equals("UGA", StringComparison.InvariantCultureIgnoreCase)) return "UG";
            if (iso3.Equals("UKR", StringComparison.InvariantCultureIgnoreCase)) return "UA";
            if (iso3.Equals("ARE", StringComparison.InvariantCultureIgnoreCase)) return "AE";
            if (iso3.Equals("GBR", StringComparison.InvariantCultureIgnoreCase)) return "GB";
            if (iso3.Equals("UMI", StringComparison.InvariantCultureIgnoreCase)) return "UM";
            if (iso3.Equals("USA", StringComparison.InvariantCultureIgnoreCase)) return "US";
            if (iso3.Equals("URY", StringComparison.InvariantCultureIgnoreCase)) return "UY";
            if (iso3.Equals("UZB", StringComparison.InvariantCultureIgnoreCase)) return "UZ";
            if (iso3.Equals("VUT", StringComparison.InvariantCultureIgnoreCase)) return "VU";
            if (iso3.Equals("VEN", StringComparison.InvariantCultureIgnoreCase)) return "VE";
            if (iso3.Equals("VNM", StringComparison.InvariantCultureIgnoreCase)) return "VN";
            if (iso3.Equals("VGB", StringComparison.InvariantCultureIgnoreCase)) return "VG";
            if (iso3.Equals("VIR", StringComparison.InvariantCultureIgnoreCase)) return "VI";
            if (iso3.Equals("WLF", StringComparison.InvariantCultureIgnoreCase)) return "WF";
            if (iso3.Equals("ESH", StringComparison.InvariantCultureIgnoreCase)) return "EH";
            if (iso3.Equals("YEM", StringComparison.InvariantCultureIgnoreCase)) return "YE";
            if (iso3.Equals("ZMB", StringComparison.InvariantCultureIgnoreCase)) return "ZM";
            if (iso3.Equals("ZWE", StringComparison.InvariantCultureIgnoreCase)) return "ZW";
            if (iso3.Equals("ALA", StringComparison.InvariantCultureIgnoreCase)) return "AX";

            return null;
        }

        public static string Iso2ToIso3Number(string iso2)
        {
            if (iso2.IsNullOrEmpty())
                return null;

            if (iso2.Equals("AF", StringComparison.InvariantCultureIgnoreCase)) return "004";
            if (iso2.Equals("AL", StringComparison.InvariantCultureIgnoreCase)) return "008";
            if (iso2.Equals("DZ", StringComparison.InvariantCultureIgnoreCase)) return "012";
            if (iso2.Equals("AS", StringComparison.InvariantCultureIgnoreCase)) return "016";
            if (iso2.Equals("AD", StringComparison.InvariantCultureIgnoreCase)) return "020";
            if (iso2.Equals("AO", StringComparison.InvariantCultureIgnoreCase)) return "024";
            if (iso2.Equals("AI", StringComparison.InvariantCultureIgnoreCase)) return "660";
            if (iso2.Equals("AQ", StringComparison.InvariantCultureIgnoreCase)) return "010";
            if (iso2.Equals("AG", StringComparison.InvariantCultureIgnoreCase)) return "028";
            if (iso2.Equals("AR", StringComparison.InvariantCultureIgnoreCase)) return "032";
            if (iso2.Equals("AM", StringComparison.InvariantCultureIgnoreCase)) return "051";
            if (iso2.Equals("AW", StringComparison.InvariantCultureIgnoreCase)) return "533";
            if (iso2.Equals("AU", StringComparison.InvariantCultureIgnoreCase)) return "036";
            if (iso2.Equals("AT", StringComparison.InvariantCultureIgnoreCase)) return "040";
            if (iso2.Equals("AZ", StringComparison.InvariantCultureIgnoreCase)) return "031";
            if (iso2.Equals("BS", StringComparison.InvariantCultureIgnoreCase)) return "044";
            if (iso2.Equals("BH", StringComparison.InvariantCultureIgnoreCase)) return "048";
            if (iso2.Equals("BD", StringComparison.InvariantCultureIgnoreCase)) return "050";
            if (iso2.Equals("BB", StringComparison.InvariantCultureIgnoreCase)) return "052";
            if (iso2.Equals("BY", StringComparison.InvariantCultureIgnoreCase)) return "112";
            if (iso2.Equals("BE", StringComparison.InvariantCultureIgnoreCase)) return "056";
            if (iso2.Equals("BZ", StringComparison.InvariantCultureIgnoreCase)) return "084";
            if (iso2.Equals("BJ", StringComparison.InvariantCultureIgnoreCase)) return "204";
            if (iso2.Equals("BM", StringComparison.InvariantCultureIgnoreCase)) return "060";
            if (iso2.Equals("BT", StringComparison.InvariantCultureIgnoreCase)) return "064";
            if (iso2.Equals("BO", StringComparison.InvariantCultureIgnoreCase)) return "068";
            if (iso2.Equals("BQ", StringComparison.InvariantCultureIgnoreCase)) return "535";
            if (iso2.Equals("BA", StringComparison.InvariantCultureIgnoreCase)) return "070";
            if (iso2.Equals("BW", StringComparison.InvariantCultureIgnoreCase)) return "072";
            if (iso2.Equals("BV", StringComparison.InvariantCultureIgnoreCase)) return "074";
            if (iso2.Equals("BR", StringComparison.InvariantCultureIgnoreCase)) return "076";
            if (iso2.Equals("IO", StringComparison.InvariantCultureIgnoreCase)) return "086";
            if (iso2.Equals("BN", StringComparison.InvariantCultureIgnoreCase)) return "096";
            if (iso2.Equals("BG", StringComparison.InvariantCultureIgnoreCase)) return "100";
            if (iso2.Equals("BF", StringComparison.InvariantCultureIgnoreCase)) return "854";
            if (iso2.Equals("BI", StringComparison.InvariantCultureIgnoreCase)) return "108";
            if (iso2.Equals("CV", StringComparison.InvariantCultureIgnoreCase)) return "132";
            if (iso2.Equals("KH", StringComparison.InvariantCultureIgnoreCase)) return "116";
            if (iso2.Equals("CM", StringComparison.InvariantCultureIgnoreCase)) return "120";
            if (iso2.Equals("CA", StringComparison.InvariantCultureIgnoreCase)) return "124";
            if (iso2.Equals("KY", StringComparison.InvariantCultureIgnoreCase)) return "136";
            if (iso2.Equals("CF", StringComparison.InvariantCultureIgnoreCase)) return "140";
            if (iso2.Equals("TD", StringComparison.InvariantCultureIgnoreCase)) return "148";
            if (iso2.Equals("CL", StringComparison.InvariantCultureIgnoreCase)) return "152";
            if (iso2.Equals("CN", StringComparison.InvariantCultureIgnoreCase)) return "156";
            if (iso2.Equals("CX", StringComparison.InvariantCultureIgnoreCase)) return "162";
            if (iso2.Equals("CC", StringComparison.InvariantCultureIgnoreCase)) return "166";
            if (iso2.Equals("CO", StringComparison.InvariantCultureIgnoreCase)) return "170";
            if (iso2.Equals("KM", StringComparison.InvariantCultureIgnoreCase)) return "174";
            if (iso2.Equals("CD", StringComparison.InvariantCultureIgnoreCase)) return "180";
            if (iso2.Equals("CG", StringComparison.InvariantCultureIgnoreCase)) return "178";
            if (iso2.Equals("CK", StringComparison.InvariantCultureIgnoreCase)) return "184";
            if (iso2.Equals("CR", StringComparison.InvariantCultureIgnoreCase)) return "188";
            if (iso2.Equals("HR", StringComparison.InvariantCultureIgnoreCase)) return "191";
            if (iso2.Equals("CU", StringComparison.InvariantCultureIgnoreCase)) return "192";
            if (iso2.Equals("CW", StringComparison.InvariantCultureIgnoreCase)) return "531";
            if (iso2.Equals("CY", StringComparison.InvariantCultureIgnoreCase)) return "196";
            if (iso2.Equals("CZ", StringComparison.InvariantCultureIgnoreCase)) return "203";
            if (iso2.Equals("CI", StringComparison.InvariantCultureIgnoreCase)) return "384";
            if (iso2.Equals("DK", StringComparison.InvariantCultureIgnoreCase)) return "208";
            if (iso2.Equals("DJ", StringComparison.InvariantCultureIgnoreCase)) return "262";
            if (iso2.Equals("DM", StringComparison.InvariantCultureIgnoreCase)) return "212";
            if (iso2.Equals("DO", StringComparison.InvariantCultureIgnoreCase)) return "214";
            if (iso2.Equals("EC", StringComparison.InvariantCultureIgnoreCase)) return "218";
            if (iso2.Equals("EG", StringComparison.InvariantCultureIgnoreCase)) return "818";
            if (iso2.Equals("SV", StringComparison.InvariantCultureIgnoreCase)) return "222";
            if (iso2.Equals("GQ", StringComparison.InvariantCultureIgnoreCase)) return "226";
            if (iso2.Equals("ER", StringComparison.InvariantCultureIgnoreCase)) return "232";
            if (iso2.Equals("EE", StringComparison.InvariantCultureIgnoreCase)) return "233";
            if (iso2.Equals("SZ", StringComparison.InvariantCultureIgnoreCase)) return "748";
            if (iso2.Equals("ET", StringComparison.InvariantCultureIgnoreCase)) return "231";
            if (iso2.Equals("FK", StringComparison.InvariantCultureIgnoreCase)) return "238";
            if (iso2.Equals("FO", StringComparison.InvariantCultureIgnoreCase)) return "234";
            if (iso2.Equals("FJ", StringComparison.InvariantCultureIgnoreCase)) return "242";
            if (iso2.Equals("FI", StringComparison.InvariantCultureIgnoreCase)) return "246";
            if (iso2.Equals("FR", StringComparison.InvariantCultureIgnoreCase)) return "250";
            if (iso2.Equals("GF", StringComparison.InvariantCultureIgnoreCase)) return "254";
            if (iso2.Equals("PF", StringComparison.InvariantCultureIgnoreCase)) return "258";
            if (iso2.Equals("TF", StringComparison.InvariantCultureIgnoreCase)) return "260";
            if (iso2.Equals("GA", StringComparison.InvariantCultureIgnoreCase)) return "266";
            if (iso2.Equals("GM", StringComparison.InvariantCultureIgnoreCase)) return "270";
            if (iso2.Equals("GE", StringComparison.InvariantCultureIgnoreCase)) return "268";
            if (iso2.Equals("DE", StringComparison.InvariantCultureIgnoreCase)) return "276";
            if (iso2.Equals("GH", StringComparison.InvariantCultureIgnoreCase)) return "288";
            if (iso2.Equals("GI", StringComparison.InvariantCultureIgnoreCase)) return "292";
            if (iso2.Equals("GR", StringComparison.InvariantCultureIgnoreCase)) return "300";
            if (iso2.Equals("GL", StringComparison.InvariantCultureIgnoreCase)) return "304";
            if (iso2.Equals("GD", StringComparison.InvariantCultureIgnoreCase)) return "308";
            if (iso2.Equals("GP", StringComparison.InvariantCultureIgnoreCase)) return "312";
            if (iso2.Equals("GU", StringComparison.InvariantCultureIgnoreCase)) return "316";
            if (iso2.Equals("GT", StringComparison.InvariantCultureIgnoreCase)) return "320";
            if (iso2.Equals("GG", StringComparison.InvariantCultureIgnoreCase)) return "831";
            if (iso2.Equals("GN", StringComparison.InvariantCultureIgnoreCase)) return "324";
            if (iso2.Equals("GW", StringComparison.InvariantCultureIgnoreCase)) return "624";
            if (iso2.Equals("GY", StringComparison.InvariantCultureIgnoreCase)) return "328";
            if (iso2.Equals("HT", StringComparison.InvariantCultureIgnoreCase)) return "332";
            if (iso2.Equals("HM", StringComparison.InvariantCultureIgnoreCase)) return "334";
            if (iso2.Equals("VA", StringComparison.InvariantCultureIgnoreCase)) return "336";
            if (iso2.Equals("HN", StringComparison.InvariantCultureIgnoreCase)) return "340";
            if (iso2.Equals("HK", StringComparison.InvariantCultureIgnoreCase)) return "344";
            if (iso2.Equals("HU", StringComparison.InvariantCultureIgnoreCase)) return "348";
            if (iso2.Equals("IS", StringComparison.InvariantCultureIgnoreCase)) return "352";
            if (iso2.Equals("IN", StringComparison.InvariantCultureIgnoreCase)) return "356";
            if (iso2.Equals("ID", StringComparison.InvariantCultureIgnoreCase)) return "360";
            if (iso2.Equals("IR", StringComparison.InvariantCultureIgnoreCase)) return "364";
            if (iso2.Equals("IQ", StringComparison.InvariantCultureIgnoreCase)) return "368";
            if (iso2.Equals("IE", StringComparison.InvariantCultureIgnoreCase)) return "372";
            if (iso2.Equals("IM", StringComparison.InvariantCultureIgnoreCase)) return "833";
            if (iso2.Equals("IL", StringComparison.InvariantCultureIgnoreCase)) return "376";
            if (iso2.Equals("IT", StringComparison.InvariantCultureIgnoreCase)) return "380";
            if (iso2.Equals("JM", StringComparison.InvariantCultureIgnoreCase)) return "388";
            if (iso2.Equals("JP", StringComparison.InvariantCultureIgnoreCase)) return "392";
            if (iso2.Equals("JE", StringComparison.InvariantCultureIgnoreCase)) return "832";
            if (iso2.Equals("JO", StringComparison.InvariantCultureIgnoreCase)) return "400";
            if (iso2.Equals("KZ", StringComparison.InvariantCultureIgnoreCase)) return "398";
            if (iso2.Equals("KE", StringComparison.InvariantCultureIgnoreCase)) return "404";
            if (iso2.Equals("KI", StringComparison.InvariantCultureIgnoreCase)) return "296";
            if (iso2.Equals("KP", StringComparison.InvariantCultureIgnoreCase)) return "408";
            if (iso2.Equals("KR", StringComparison.InvariantCultureIgnoreCase)) return "410";
            if (iso2.Equals("KW", StringComparison.InvariantCultureIgnoreCase)) return "414";
            if (iso2.Equals("KG", StringComparison.InvariantCultureIgnoreCase)) return "417";
            if (iso2.Equals("LA", StringComparison.InvariantCultureIgnoreCase)) return "418";
            if (iso2.Equals("LV", StringComparison.InvariantCultureIgnoreCase)) return "428";
            if (iso2.Equals("LB", StringComparison.InvariantCultureIgnoreCase)) return "422";
            if (iso2.Equals("LS", StringComparison.InvariantCultureIgnoreCase)) return "426";
            if (iso2.Equals("LR", StringComparison.InvariantCultureIgnoreCase)) return "430";
            if (iso2.Equals("LY", StringComparison.InvariantCultureIgnoreCase)) return "434";
            if (iso2.Equals("LI", StringComparison.InvariantCultureIgnoreCase)) return "438";
            if (iso2.Equals("LT", StringComparison.InvariantCultureIgnoreCase)) return "440";
            if (iso2.Equals("LU", StringComparison.InvariantCultureIgnoreCase)) return "442";
            if (iso2.Equals("MO", StringComparison.InvariantCultureIgnoreCase)) return "446";
            if (iso2.Equals("MG", StringComparison.InvariantCultureIgnoreCase)) return "450";
            if (iso2.Equals("MW", StringComparison.InvariantCultureIgnoreCase)) return "454";
            if (iso2.Equals("MY", StringComparison.InvariantCultureIgnoreCase)) return "458";
            if (iso2.Equals("MV", StringComparison.InvariantCultureIgnoreCase)) return "462";
            if (iso2.Equals("ML", StringComparison.InvariantCultureIgnoreCase)) return "466";
            if (iso2.Equals("MT", StringComparison.InvariantCultureIgnoreCase)) return "470";
            if (iso2.Equals("MH", StringComparison.InvariantCultureIgnoreCase)) return "584";
            if (iso2.Equals("MQ", StringComparison.InvariantCultureIgnoreCase)) return "474";
            if (iso2.Equals("MR", StringComparison.InvariantCultureIgnoreCase)) return "478";
            if (iso2.Equals("MU", StringComparison.InvariantCultureIgnoreCase)) return "480";
            if (iso2.Equals("YT", StringComparison.InvariantCultureIgnoreCase)) return "175";
            if (iso2.Equals("MX", StringComparison.InvariantCultureIgnoreCase)) return "484";
            if (iso2.Equals("FM", StringComparison.InvariantCultureIgnoreCase)) return "583";
            if (iso2.Equals("MD", StringComparison.InvariantCultureIgnoreCase)) return "498";
            if (iso2.Equals("MC", StringComparison.InvariantCultureIgnoreCase)) return "492";
            if (iso2.Equals("MN", StringComparison.InvariantCultureIgnoreCase)) return "496";
            if (iso2.Equals("ME", StringComparison.InvariantCultureIgnoreCase)) return "499";
            if (iso2.Equals("MS", StringComparison.InvariantCultureIgnoreCase)) return "500";
            if (iso2.Equals("MA", StringComparison.InvariantCultureIgnoreCase)) return "504";
            if (iso2.Equals("MZ", StringComparison.InvariantCultureIgnoreCase)) return "508";
            if (iso2.Equals("MM", StringComparison.InvariantCultureIgnoreCase)) return "104";
            if (iso2.Equals("NA", StringComparison.InvariantCultureIgnoreCase)) return "516";
            if (iso2.Equals("NR", StringComparison.InvariantCultureIgnoreCase)) return "520";
            if (iso2.Equals("NP", StringComparison.InvariantCultureIgnoreCase)) return "524";
            if (iso2.Equals("NL", StringComparison.InvariantCultureIgnoreCase)) return "528";
            if (iso2.Equals("NC", StringComparison.InvariantCultureIgnoreCase)) return "540";
            if (iso2.Equals("NZ", StringComparison.InvariantCultureIgnoreCase)) return "554";
            if (iso2.Equals("NI", StringComparison.InvariantCultureIgnoreCase)) return "558";
            if (iso2.Equals("NE", StringComparison.InvariantCultureIgnoreCase)) return "562";
            if (iso2.Equals("NG", StringComparison.InvariantCultureIgnoreCase)) return "566";
            if (iso2.Equals("NU", StringComparison.InvariantCultureIgnoreCase)) return "570";
            if (iso2.Equals("NF", StringComparison.InvariantCultureIgnoreCase)) return "574";
            if (iso2.Equals("MK", StringComparison.InvariantCultureIgnoreCase)) return "807";
            if (iso2.Equals("MP", StringComparison.InvariantCultureIgnoreCase)) return "580";
            if (iso2.Equals("NO", StringComparison.InvariantCultureIgnoreCase)) return "578";
            if (iso2.Equals("OM", StringComparison.InvariantCultureIgnoreCase)) return "512";
            if (iso2.Equals("PK", StringComparison.InvariantCultureIgnoreCase)) return "586";
            if (iso2.Equals("PW", StringComparison.InvariantCultureIgnoreCase)) return "585";
            if (iso2.Equals("PS", StringComparison.InvariantCultureIgnoreCase)) return "275";
            if (iso2.Equals("PA", StringComparison.InvariantCultureIgnoreCase)) return "591";
            if (iso2.Equals("PG", StringComparison.InvariantCultureIgnoreCase)) return "598";
            if (iso2.Equals("PY", StringComparison.InvariantCultureIgnoreCase)) return "600";
            if (iso2.Equals("PE", StringComparison.InvariantCultureIgnoreCase)) return "604";
            if (iso2.Equals("PH", StringComparison.InvariantCultureIgnoreCase)) return "608";
            if (iso2.Equals("PN", StringComparison.InvariantCultureIgnoreCase)) return "612";
            if (iso2.Equals("PL", StringComparison.InvariantCultureIgnoreCase)) return "616";
            if (iso2.Equals("PT", StringComparison.InvariantCultureIgnoreCase)) return "620";
            if (iso2.Equals("PR", StringComparison.InvariantCultureIgnoreCase)) return "630";
            if (iso2.Equals("QA", StringComparison.InvariantCultureIgnoreCase)) return "634";
            if (iso2.Equals("RO", StringComparison.InvariantCultureIgnoreCase)) return "642";
            if (iso2.Equals("RU", StringComparison.InvariantCultureIgnoreCase)) return "643";
            if (iso2.Equals("RW", StringComparison.InvariantCultureIgnoreCase)) return "646";
            if (iso2.Equals("RE", StringComparison.InvariantCultureIgnoreCase)) return "638";
            if (iso2.Equals("BL", StringComparison.InvariantCultureIgnoreCase)) return "652";
            if (iso2.Equals("SH", StringComparison.InvariantCultureIgnoreCase)) return "654";
            if (iso2.Equals("KN", StringComparison.InvariantCultureIgnoreCase)) return "659";
            if (iso2.Equals("LC", StringComparison.InvariantCultureIgnoreCase)) return "662";
            if (iso2.Equals("MF", StringComparison.InvariantCultureIgnoreCase)) return "663";
            if (iso2.Equals("PM", StringComparison.InvariantCultureIgnoreCase)) return "666";
            if (iso2.Equals("VC", StringComparison.InvariantCultureIgnoreCase)) return "670";
            if (iso2.Equals("WS", StringComparison.InvariantCultureIgnoreCase)) return "882";
            if (iso2.Equals("SM", StringComparison.InvariantCultureIgnoreCase)) return "674";
            if (iso2.Equals("ST", StringComparison.InvariantCultureIgnoreCase)) return "678";
            if (iso2.Equals("SA", StringComparison.InvariantCultureIgnoreCase)) return "682";
            if (iso2.Equals("SN", StringComparison.InvariantCultureIgnoreCase)) return "686";
            if (iso2.Equals("RS", StringComparison.InvariantCultureIgnoreCase)) return "688";
            if (iso2.Equals("SC", StringComparison.InvariantCultureIgnoreCase)) return "690";
            if (iso2.Equals("SL", StringComparison.InvariantCultureIgnoreCase)) return "694";
            if (iso2.Equals("SG", StringComparison.InvariantCultureIgnoreCase)) return "702";
            if (iso2.Equals("SX", StringComparison.InvariantCultureIgnoreCase)) return "534";
            if (iso2.Equals("SK", StringComparison.InvariantCultureIgnoreCase)) return "703";
            if (iso2.Equals("SI", StringComparison.InvariantCultureIgnoreCase)) return "705";
            if (iso2.Equals("SB", StringComparison.InvariantCultureIgnoreCase)) return "090";
            if (iso2.Equals("SO", StringComparison.InvariantCultureIgnoreCase)) return "706";
            if (iso2.Equals("ZA", StringComparison.InvariantCultureIgnoreCase)) return "710";
            if (iso2.Equals("GS", StringComparison.InvariantCultureIgnoreCase)) return "239";
            if (iso2.Equals("SS", StringComparison.InvariantCultureIgnoreCase)) return "728";
            if (iso2.Equals("ES", StringComparison.InvariantCultureIgnoreCase)) return "724";
            if (iso2.Equals("LK", StringComparison.InvariantCultureIgnoreCase)) return "144";
            if (iso2.Equals("SD", StringComparison.InvariantCultureIgnoreCase)) return "729";
            if (iso2.Equals("SR", StringComparison.InvariantCultureIgnoreCase)) return "740";
            if (iso2.Equals("SJ", StringComparison.InvariantCultureIgnoreCase)) return "744";
            if (iso2.Equals("SE", StringComparison.InvariantCultureIgnoreCase)) return "752";
            if (iso2.Equals("CH", StringComparison.InvariantCultureIgnoreCase)) return "756";
            if (iso2.Equals("SY", StringComparison.InvariantCultureIgnoreCase)) return "760";
            if (iso2.Equals("TW", StringComparison.InvariantCultureIgnoreCase)) return "158";
            if (iso2.Equals("TJ", StringComparison.InvariantCultureIgnoreCase)) return "762";
            if (iso2.Equals("TZ", StringComparison.InvariantCultureIgnoreCase)) return "834";
            if (iso2.Equals("TH", StringComparison.InvariantCultureIgnoreCase)) return "764";
            if (iso2.Equals("TL", StringComparison.InvariantCultureIgnoreCase)) return "626";
            if (iso2.Equals("TG", StringComparison.InvariantCultureIgnoreCase)) return "768";
            if (iso2.Equals("TK", StringComparison.InvariantCultureIgnoreCase)) return "772";
            if (iso2.Equals("TO", StringComparison.InvariantCultureIgnoreCase)) return "776";
            if (iso2.Equals("TT", StringComparison.InvariantCultureIgnoreCase)) return "780";
            if (iso2.Equals("TN", StringComparison.InvariantCultureIgnoreCase)) return "788";
            if (iso2.Equals("TR", StringComparison.InvariantCultureIgnoreCase)) return "792";
            if (iso2.Equals("TM", StringComparison.InvariantCultureIgnoreCase)) return "795";
            if (iso2.Equals("TC", StringComparison.InvariantCultureIgnoreCase)) return "796";
            if (iso2.Equals("TV", StringComparison.InvariantCultureIgnoreCase)) return "798";
            if (iso2.Equals("UG", StringComparison.InvariantCultureIgnoreCase)) return "800";
            if (iso2.Equals("UA", StringComparison.InvariantCultureIgnoreCase)) return "804";
            if (iso2.Equals("AE", StringComparison.InvariantCultureIgnoreCase)) return "784";
            if (iso2.Equals("GB", StringComparison.InvariantCultureIgnoreCase)) return "826";
            if (iso2.Equals("UM", StringComparison.InvariantCultureIgnoreCase)) return "581";
            if (iso2.Equals("US", StringComparison.InvariantCultureIgnoreCase)) return "840";
            if (iso2.Equals("UY", StringComparison.InvariantCultureIgnoreCase)) return "858";
            if (iso2.Equals("UZ", StringComparison.InvariantCultureIgnoreCase)) return "860";
            if (iso2.Equals("VU", StringComparison.InvariantCultureIgnoreCase)) return "548";
            if (iso2.Equals("VE", StringComparison.InvariantCultureIgnoreCase)) return "862";
            if (iso2.Equals("VN", StringComparison.InvariantCultureIgnoreCase)) return "704";
            if (iso2.Equals("VG", StringComparison.InvariantCultureIgnoreCase)) return "092";
            if (iso2.Equals("VI", StringComparison.InvariantCultureIgnoreCase)) return "850";
            if (iso2.Equals("WF", StringComparison.InvariantCultureIgnoreCase)) return "876";
            if (iso2.Equals("EH", StringComparison.InvariantCultureIgnoreCase)) return "732";
            if (iso2.Equals("YE", StringComparison.InvariantCultureIgnoreCase)) return "887";
            if (iso2.Equals("ZM", StringComparison.InvariantCultureIgnoreCase)) return "894";
            if (iso2.Equals("ZW", StringComparison.InvariantCultureIgnoreCase)) return "716";
            if (iso2.Equals("AX", StringComparison.InvariantCultureIgnoreCase)) return "248";

            return null;
        }

        public static string Iso3ToIso3Number(string iso3)
        {
            if (iso3.IsNullOrEmpty())
                return null;

            if (iso3.Equals("AFG", StringComparison.InvariantCultureIgnoreCase)) return "004";
            if (iso3.Equals("ALB", StringComparison.InvariantCultureIgnoreCase)) return "008";
            if (iso3.Equals("DZA", StringComparison.InvariantCultureIgnoreCase)) return "012";
            if (iso3.Equals("ASM", StringComparison.InvariantCultureIgnoreCase)) return "016";
            if (iso3.Equals("AND", StringComparison.InvariantCultureIgnoreCase)) return "020";
            if (iso3.Equals("AGO", StringComparison.InvariantCultureIgnoreCase)) return "024";
            if (iso3.Equals("AIA", StringComparison.InvariantCultureIgnoreCase)) return "660";
            if (iso3.Equals("ATA", StringComparison.InvariantCultureIgnoreCase)) return "010";
            if (iso3.Equals("ATG", StringComparison.InvariantCultureIgnoreCase)) return "028";
            if (iso3.Equals("ARG", StringComparison.InvariantCultureIgnoreCase)) return "032";
            if (iso3.Equals("ARM", StringComparison.InvariantCultureIgnoreCase)) return "051";
            if (iso3.Equals("ABW", StringComparison.InvariantCultureIgnoreCase)) return "533";
            if (iso3.Equals("AUS", StringComparison.InvariantCultureIgnoreCase)) return "036";
            if (iso3.Equals("AUT", StringComparison.InvariantCultureIgnoreCase)) return "040";
            if (iso3.Equals("AZE", StringComparison.InvariantCultureIgnoreCase)) return "031";
            if (iso3.Equals("BHS", StringComparison.InvariantCultureIgnoreCase)) return "044";
            if (iso3.Equals("BHR", StringComparison.InvariantCultureIgnoreCase)) return "048";
            if (iso3.Equals("BGD", StringComparison.InvariantCultureIgnoreCase)) return "050";
            if (iso3.Equals("BRB", StringComparison.InvariantCultureIgnoreCase)) return "052";
            if (iso3.Equals("BLR", StringComparison.InvariantCultureIgnoreCase)) return "112";
            if (iso3.Equals("BEL", StringComparison.InvariantCultureIgnoreCase)) return "056";
            if (iso3.Equals("BLZ", StringComparison.InvariantCultureIgnoreCase)) return "084";
            if (iso3.Equals("BEN", StringComparison.InvariantCultureIgnoreCase)) return "204";
            if (iso3.Equals("BMU", StringComparison.InvariantCultureIgnoreCase)) return "060";
            if (iso3.Equals("BTN", StringComparison.InvariantCultureIgnoreCase)) return "064";
            if (iso3.Equals("BOL", StringComparison.InvariantCultureIgnoreCase)) return "068";
            if (iso3.Equals("BES", StringComparison.InvariantCultureIgnoreCase)) return "535";
            if (iso3.Equals("BIH", StringComparison.InvariantCultureIgnoreCase)) return "070";
            if (iso3.Equals("BWA", StringComparison.InvariantCultureIgnoreCase)) return "072";
            if (iso3.Equals("BVT", StringComparison.InvariantCultureIgnoreCase)) return "074";
            if (iso3.Equals("BRA", StringComparison.InvariantCultureIgnoreCase)) return "076";
            if (iso3.Equals("IOT", StringComparison.InvariantCultureIgnoreCase)) return "086";
            if (iso3.Equals("BRN", StringComparison.InvariantCultureIgnoreCase)) return "096";
            if (iso3.Equals("BGR", StringComparison.InvariantCultureIgnoreCase)) return "100";
            if (iso3.Equals("BFA", StringComparison.InvariantCultureIgnoreCase)) return "854";
            if (iso3.Equals("BDI", StringComparison.InvariantCultureIgnoreCase)) return "108";
            if (iso3.Equals("CPV", StringComparison.InvariantCultureIgnoreCase)) return "132";
            if (iso3.Equals("KHM", StringComparison.InvariantCultureIgnoreCase)) return "116";
            if (iso3.Equals("CMR", StringComparison.InvariantCultureIgnoreCase)) return "120";
            if (iso3.Equals("CAN", StringComparison.InvariantCultureIgnoreCase)) return "124";
            if (iso3.Equals("CYM", StringComparison.InvariantCultureIgnoreCase)) return "136";
            if (iso3.Equals("CAF", StringComparison.InvariantCultureIgnoreCase)) return "140";
            if (iso3.Equals("TCD", StringComparison.InvariantCultureIgnoreCase)) return "148";
            if (iso3.Equals("CHL", StringComparison.InvariantCultureIgnoreCase)) return "152";
            if (iso3.Equals("CHN", StringComparison.InvariantCultureIgnoreCase)) return "156";
            if (iso3.Equals("CXR", StringComparison.InvariantCultureIgnoreCase)) return "162";
            if (iso3.Equals("CCK", StringComparison.InvariantCultureIgnoreCase)) return "166";
            if (iso3.Equals("COL", StringComparison.InvariantCultureIgnoreCase)) return "170";
            if (iso3.Equals("COM", StringComparison.InvariantCultureIgnoreCase)) return "174";
            if (iso3.Equals("COD", StringComparison.InvariantCultureIgnoreCase)) return "180";
            if (iso3.Equals("COG", StringComparison.InvariantCultureIgnoreCase)) return "178";
            if (iso3.Equals("COK", StringComparison.InvariantCultureIgnoreCase)) return "184";
            if (iso3.Equals("CRI", StringComparison.InvariantCultureIgnoreCase)) return "188";
            if (iso3.Equals("HRV", StringComparison.InvariantCultureIgnoreCase)) return "191";
            if (iso3.Equals("CUB", StringComparison.InvariantCultureIgnoreCase)) return "192";
            if (iso3.Equals("CUW", StringComparison.InvariantCultureIgnoreCase)) return "531";
            if (iso3.Equals("CYP", StringComparison.InvariantCultureIgnoreCase)) return "196";
            if (iso3.Equals("CZE", StringComparison.InvariantCultureIgnoreCase)) return "203";
            if (iso3.Equals("CIV", StringComparison.InvariantCultureIgnoreCase)) return "384";
            if (iso3.Equals("DNK", StringComparison.InvariantCultureIgnoreCase)) return "208";
            if (iso3.Equals("DJI", StringComparison.InvariantCultureIgnoreCase)) return "262";
            if (iso3.Equals("DMA", StringComparison.InvariantCultureIgnoreCase)) return "212";
            if (iso3.Equals("DOM", StringComparison.InvariantCultureIgnoreCase)) return "214";
            if (iso3.Equals("ECU", StringComparison.InvariantCultureIgnoreCase)) return "218";
            if (iso3.Equals("EGY", StringComparison.InvariantCultureIgnoreCase)) return "818";
            if (iso3.Equals("SLV", StringComparison.InvariantCultureIgnoreCase)) return "222";
            if (iso3.Equals("GNQ", StringComparison.InvariantCultureIgnoreCase)) return "226";
            if (iso3.Equals("ERI", StringComparison.InvariantCultureIgnoreCase)) return "232";
            if (iso3.Equals("EST", StringComparison.InvariantCultureIgnoreCase)) return "233";
            if (iso3.Equals("SWZ", StringComparison.InvariantCultureIgnoreCase)) return "748";
            if (iso3.Equals("ETH", StringComparison.InvariantCultureIgnoreCase)) return "231";
            if (iso3.Equals("FLK", StringComparison.InvariantCultureIgnoreCase)) return "238";
            if (iso3.Equals("FRO", StringComparison.InvariantCultureIgnoreCase)) return "234";
            if (iso3.Equals("FJI", StringComparison.InvariantCultureIgnoreCase)) return "242";
            if (iso3.Equals("FIN", StringComparison.InvariantCultureIgnoreCase)) return "246";
            if (iso3.Equals("FRA", StringComparison.InvariantCultureIgnoreCase)) return "250";
            if (iso3.Equals("GUF", StringComparison.InvariantCultureIgnoreCase)) return "254";
            if (iso3.Equals("PYF", StringComparison.InvariantCultureIgnoreCase)) return "258";
            if (iso3.Equals("ATF", StringComparison.InvariantCultureIgnoreCase)) return "260";
            if (iso3.Equals("GAB", StringComparison.InvariantCultureIgnoreCase)) return "266";
            if (iso3.Equals("GMB", StringComparison.InvariantCultureIgnoreCase)) return "270";
            if (iso3.Equals("GEO", StringComparison.InvariantCultureIgnoreCase)) return "268";
            if (iso3.Equals("DEU", StringComparison.InvariantCultureIgnoreCase)) return "276";
            if (iso3.Equals("GHA", StringComparison.InvariantCultureIgnoreCase)) return "288";
            if (iso3.Equals("GIB", StringComparison.InvariantCultureIgnoreCase)) return "292";
            if (iso3.Equals("GRC", StringComparison.InvariantCultureIgnoreCase)) return "300";
            if (iso3.Equals("GRL", StringComparison.InvariantCultureIgnoreCase)) return "304";
            if (iso3.Equals("GRD", StringComparison.InvariantCultureIgnoreCase)) return "308";
            if (iso3.Equals("GLP", StringComparison.InvariantCultureIgnoreCase)) return "312";
            if (iso3.Equals("GUM", StringComparison.InvariantCultureIgnoreCase)) return "316";
            if (iso3.Equals("GTM", StringComparison.InvariantCultureIgnoreCase)) return "320";
            if (iso3.Equals("GGY", StringComparison.InvariantCultureIgnoreCase)) return "831";
            if (iso3.Equals("GIN", StringComparison.InvariantCultureIgnoreCase)) return "324";
            if (iso3.Equals("GNB", StringComparison.InvariantCultureIgnoreCase)) return "624";
            if (iso3.Equals("GUY", StringComparison.InvariantCultureIgnoreCase)) return "328";
            if (iso3.Equals("HTI", StringComparison.InvariantCultureIgnoreCase)) return "332";
            if (iso3.Equals("HMD", StringComparison.InvariantCultureIgnoreCase)) return "334";
            if (iso3.Equals("VAT", StringComparison.InvariantCultureIgnoreCase)) return "336";
            if (iso3.Equals("HND", StringComparison.InvariantCultureIgnoreCase)) return "340";
            if (iso3.Equals("HKG", StringComparison.InvariantCultureIgnoreCase)) return "344";
            if (iso3.Equals("HUN", StringComparison.InvariantCultureIgnoreCase)) return "348";
            if (iso3.Equals("ISL", StringComparison.InvariantCultureIgnoreCase)) return "352";
            if (iso3.Equals("IND", StringComparison.InvariantCultureIgnoreCase)) return "356";
            if (iso3.Equals("IDN", StringComparison.InvariantCultureIgnoreCase)) return "360";
            if (iso3.Equals("IRN", StringComparison.InvariantCultureIgnoreCase)) return "364";
            if (iso3.Equals("IRQ", StringComparison.InvariantCultureIgnoreCase)) return "368";
            if (iso3.Equals("IRL", StringComparison.InvariantCultureIgnoreCase)) return "372";
            if (iso3.Equals("IMN", StringComparison.InvariantCultureIgnoreCase)) return "833";
            if (iso3.Equals("ISR", StringComparison.InvariantCultureIgnoreCase)) return "376";
            if (iso3.Equals("ITA", StringComparison.InvariantCultureIgnoreCase)) return "380";
            if (iso3.Equals("JAM", StringComparison.InvariantCultureIgnoreCase)) return "388";
            if (iso3.Equals("JPN", StringComparison.InvariantCultureIgnoreCase)) return "392";
            if (iso3.Equals("JEY", StringComparison.InvariantCultureIgnoreCase)) return "832";
            if (iso3.Equals("JOR", StringComparison.InvariantCultureIgnoreCase)) return "400";
            if (iso3.Equals("KAZ", StringComparison.InvariantCultureIgnoreCase)) return "398";
            if (iso3.Equals("KEN", StringComparison.InvariantCultureIgnoreCase)) return "404";
            if (iso3.Equals("KIR", StringComparison.InvariantCultureIgnoreCase)) return "296";
            if (iso3.Equals("PRK", StringComparison.InvariantCultureIgnoreCase)) return "408";
            if (iso3.Equals("KOR", StringComparison.InvariantCultureIgnoreCase)) return "410";
            if (iso3.Equals("KWT", StringComparison.InvariantCultureIgnoreCase)) return "414";
            if (iso3.Equals("KGZ", StringComparison.InvariantCultureIgnoreCase)) return "417";
            if (iso3.Equals("LAO", StringComparison.InvariantCultureIgnoreCase)) return "418";
            if (iso3.Equals("LVA", StringComparison.InvariantCultureIgnoreCase)) return "428";
            if (iso3.Equals("LBN", StringComparison.InvariantCultureIgnoreCase)) return "422";
            if (iso3.Equals("LSO", StringComparison.InvariantCultureIgnoreCase)) return "426";
            if (iso3.Equals("LBR", StringComparison.InvariantCultureIgnoreCase)) return "430";
            if (iso3.Equals("LBY", StringComparison.InvariantCultureIgnoreCase)) return "434";
            if (iso3.Equals("LIE", StringComparison.InvariantCultureIgnoreCase)) return "438";
            if (iso3.Equals("LTU", StringComparison.InvariantCultureIgnoreCase)) return "440";
            if (iso3.Equals("LUX", StringComparison.InvariantCultureIgnoreCase)) return "442";
            if (iso3.Equals("MAC", StringComparison.InvariantCultureIgnoreCase)) return "446";
            if (iso3.Equals("MDG", StringComparison.InvariantCultureIgnoreCase)) return "450";
            if (iso3.Equals("MWI", StringComparison.InvariantCultureIgnoreCase)) return "454";
            if (iso3.Equals("MYS", StringComparison.InvariantCultureIgnoreCase)) return "458";
            if (iso3.Equals("MDV", StringComparison.InvariantCultureIgnoreCase)) return "462";
            if (iso3.Equals("MLI", StringComparison.InvariantCultureIgnoreCase)) return "466";
            if (iso3.Equals("MLT", StringComparison.InvariantCultureIgnoreCase)) return "470";
            if (iso3.Equals("MHL", StringComparison.InvariantCultureIgnoreCase)) return "584";
            if (iso3.Equals("MTQ", StringComparison.InvariantCultureIgnoreCase)) return "474";
            if (iso3.Equals("MRT", StringComparison.InvariantCultureIgnoreCase)) return "478";
            if (iso3.Equals("MUS", StringComparison.InvariantCultureIgnoreCase)) return "480";
            if (iso3.Equals("MYT", StringComparison.InvariantCultureIgnoreCase)) return "175";
            if (iso3.Equals("MEX", StringComparison.InvariantCultureIgnoreCase)) return "484";
            if (iso3.Equals("FSM", StringComparison.InvariantCultureIgnoreCase)) return "583";
            if (iso3.Equals("MDA", StringComparison.InvariantCultureIgnoreCase)) return "498";
            if (iso3.Equals("MCO", StringComparison.InvariantCultureIgnoreCase)) return "492";
            if (iso3.Equals("MNG", StringComparison.InvariantCultureIgnoreCase)) return "496";
            if (iso3.Equals("MNE", StringComparison.InvariantCultureIgnoreCase)) return "499";
            if (iso3.Equals("MSR", StringComparison.InvariantCultureIgnoreCase)) return "500";
            if (iso3.Equals("MAR", StringComparison.InvariantCultureIgnoreCase)) return "504";
            if (iso3.Equals("MOZ", StringComparison.InvariantCultureIgnoreCase)) return "508";
            if (iso3.Equals("MMR", StringComparison.InvariantCultureIgnoreCase)) return "104";
            if (iso3.Equals("NAM", StringComparison.InvariantCultureIgnoreCase)) return "516";
            if (iso3.Equals("NRU", StringComparison.InvariantCultureIgnoreCase)) return "520";
            if (iso3.Equals("NPL", StringComparison.InvariantCultureIgnoreCase)) return "524";
            if (iso3.Equals("NLD", StringComparison.InvariantCultureIgnoreCase)) return "528";
            if (iso3.Equals("NCL", StringComparison.InvariantCultureIgnoreCase)) return "540";
            if (iso3.Equals("NZL", StringComparison.InvariantCultureIgnoreCase)) return "554";
            if (iso3.Equals("NIC", StringComparison.InvariantCultureIgnoreCase)) return "558";
            if (iso3.Equals("NER", StringComparison.InvariantCultureIgnoreCase)) return "562";
            if (iso3.Equals("NGA", StringComparison.InvariantCultureIgnoreCase)) return "566";
            if (iso3.Equals("NIU", StringComparison.InvariantCultureIgnoreCase)) return "570";
            if (iso3.Equals("NFK", StringComparison.InvariantCultureIgnoreCase)) return "574";
            if (iso3.Equals("MKD", StringComparison.InvariantCultureIgnoreCase)) return "807";
            if (iso3.Equals("MNP", StringComparison.InvariantCultureIgnoreCase)) return "580";
            if (iso3.Equals("NOR", StringComparison.InvariantCultureIgnoreCase)) return "578";
            if (iso3.Equals("OMN", StringComparison.InvariantCultureIgnoreCase)) return "512";
            if (iso3.Equals("PAK", StringComparison.InvariantCultureIgnoreCase)) return "586";
            if (iso3.Equals("PLW", StringComparison.InvariantCultureIgnoreCase)) return "585";
            if (iso3.Equals("PSE", StringComparison.InvariantCultureIgnoreCase)) return "275";
            if (iso3.Equals("PAN", StringComparison.InvariantCultureIgnoreCase)) return "591";
            if (iso3.Equals("PNG", StringComparison.InvariantCultureIgnoreCase)) return "598";
            if (iso3.Equals("PRY", StringComparison.InvariantCultureIgnoreCase)) return "600";
            if (iso3.Equals("PER", StringComparison.InvariantCultureIgnoreCase)) return "604";
            if (iso3.Equals("PHL", StringComparison.InvariantCultureIgnoreCase)) return "608";
            if (iso3.Equals("PCN", StringComparison.InvariantCultureIgnoreCase)) return "612";
            if (iso3.Equals("POL", StringComparison.InvariantCultureIgnoreCase)) return "616";
            if (iso3.Equals("PRT", StringComparison.InvariantCultureIgnoreCase)) return "620";
            if (iso3.Equals("PRI", StringComparison.InvariantCultureIgnoreCase)) return "630";
            if (iso3.Equals("QAT", StringComparison.InvariantCultureIgnoreCase)) return "634";
            if (iso3.Equals("ROU", StringComparison.InvariantCultureIgnoreCase)) return "642";
            if (iso3.Equals("RUS", StringComparison.InvariantCultureIgnoreCase)) return "643";
            if (iso3.Equals("RWA", StringComparison.InvariantCultureIgnoreCase)) return "646";
            if (iso3.Equals("REU", StringComparison.InvariantCultureIgnoreCase)) return "638";
            if (iso3.Equals("BLM", StringComparison.InvariantCultureIgnoreCase)) return "652";
            if (iso3.Equals("SHN", StringComparison.InvariantCultureIgnoreCase)) return "654";
            if (iso3.Equals("KNA", StringComparison.InvariantCultureIgnoreCase)) return "659";
            if (iso3.Equals("LCA", StringComparison.InvariantCultureIgnoreCase)) return "662";
            if (iso3.Equals("MAF", StringComparison.InvariantCultureIgnoreCase)) return "663";
            if (iso3.Equals("SPM", StringComparison.InvariantCultureIgnoreCase)) return "666";
            if (iso3.Equals("VCT", StringComparison.InvariantCultureIgnoreCase)) return "670";
            if (iso3.Equals("WSM", StringComparison.InvariantCultureIgnoreCase)) return "882";
            if (iso3.Equals("SMR", StringComparison.InvariantCultureIgnoreCase)) return "674";
            if (iso3.Equals("STP", StringComparison.InvariantCultureIgnoreCase)) return "678";
            if (iso3.Equals("SAU", StringComparison.InvariantCultureIgnoreCase)) return "682";
            if (iso3.Equals("SEN", StringComparison.InvariantCultureIgnoreCase)) return "686";
            if (iso3.Equals("SRB", StringComparison.InvariantCultureIgnoreCase)) return "688";
            if (iso3.Equals("SYC", StringComparison.InvariantCultureIgnoreCase)) return "690";
            if (iso3.Equals("SLE", StringComparison.InvariantCultureIgnoreCase)) return "694";
            if (iso3.Equals("SGP", StringComparison.InvariantCultureIgnoreCase)) return "702";
            if (iso3.Equals("SXM", StringComparison.InvariantCultureIgnoreCase)) return "534";
            if (iso3.Equals("SVK", StringComparison.InvariantCultureIgnoreCase)) return "703";
            if (iso3.Equals("SVN", StringComparison.InvariantCultureIgnoreCase)) return "705";
            if (iso3.Equals("SLB", StringComparison.InvariantCultureIgnoreCase)) return "090";
            if (iso3.Equals("SOM", StringComparison.InvariantCultureIgnoreCase)) return "706";
            if (iso3.Equals("ZAF", StringComparison.InvariantCultureIgnoreCase)) return "710";
            if (iso3.Equals("SGS", StringComparison.InvariantCultureIgnoreCase)) return "239";
            if (iso3.Equals("SSD", StringComparison.InvariantCultureIgnoreCase)) return "728";
            if (iso3.Equals("ESP", StringComparison.InvariantCultureIgnoreCase)) return "724";
            if (iso3.Equals("LKA", StringComparison.InvariantCultureIgnoreCase)) return "144";
            if (iso3.Equals("SDN", StringComparison.InvariantCultureIgnoreCase)) return "729";
            if (iso3.Equals("SUR", StringComparison.InvariantCultureIgnoreCase)) return "740";
            if (iso3.Equals("SJM", StringComparison.InvariantCultureIgnoreCase)) return "744";
            if (iso3.Equals("SWE", StringComparison.InvariantCultureIgnoreCase)) return "752";
            if (iso3.Equals("CHE", StringComparison.InvariantCultureIgnoreCase)) return "756";
            if (iso3.Equals("SYR", StringComparison.InvariantCultureIgnoreCase)) return "760";
            if (iso3.Equals("TWN", StringComparison.InvariantCultureIgnoreCase)) return "158";
            if (iso3.Equals("TJK", StringComparison.InvariantCultureIgnoreCase)) return "762";
            if (iso3.Equals("TZA", StringComparison.InvariantCultureIgnoreCase)) return "834";
            if (iso3.Equals("THA", StringComparison.InvariantCultureIgnoreCase)) return "764";
            if (iso3.Equals("TLS", StringComparison.InvariantCultureIgnoreCase)) return "626";
            if (iso3.Equals("TGO", StringComparison.InvariantCultureIgnoreCase)) return "768";
            if (iso3.Equals("TKL", StringComparison.InvariantCultureIgnoreCase)) return "772";
            if (iso3.Equals("TON", StringComparison.InvariantCultureIgnoreCase)) return "776";
            if (iso3.Equals("TTO", StringComparison.InvariantCultureIgnoreCase)) return "780";
            if (iso3.Equals("TUN", StringComparison.InvariantCultureIgnoreCase)) return "788";
            if (iso3.Equals("TUR", StringComparison.InvariantCultureIgnoreCase)) return "792";
            if (iso3.Equals("TKM", StringComparison.InvariantCultureIgnoreCase)) return "795";
            if (iso3.Equals("TCA", StringComparison.InvariantCultureIgnoreCase)) return "796";
            if (iso3.Equals("TUV", StringComparison.InvariantCultureIgnoreCase)) return "798";
            if (iso3.Equals("UGA", StringComparison.InvariantCultureIgnoreCase)) return "800";
            if (iso3.Equals("UKR", StringComparison.InvariantCultureIgnoreCase)) return "804";
            if (iso3.Equals("ARE", StringComparison.InvariantCultureIgnoreCase)) return "784";
            if (iso3.Equals("GBR", StringComparison.InvariantCultureIgnoreCase)) return "826";
            if (iso3.Equals("UMI", StringComparison.InvariantCultureIgnoreCase)) return "581";
            if (iso3.Equals("USA", StringComparison.InvariantCultureIgnoreCase)) return "840";
            if (iso3.Equals("URY", StringComparison.InvariantCultureIgnoreCase)) return "858";
            if (iso3.Equals("UZB", StringComparison.InvariantCultureIgnoreCase)) return "860";
            if (iso3.Equals("VUT", StringComparison.InvariantCultureIgnoreCase)) return "548";
            if (iso3.Equals("VEN", StringComparison.InvariantCultureIgnoreCase)) return "862";
            if (iso3.Equals("VNM", StringComparison.InvariantCultureIgnoreCase)) return "704";
            if (iso3.Equals("VGB", StringComparison.InvariantCultureIgnoreCase)) return "092";
            if (iso3.Equals("VIR", StringComparison.InvariantCultureIgnoreCase)) return "850";
            if (iso3.Equals("WLF", StringComparison.InvariantCultureIgnoreCase)) return "876";
            if (iso3.Equals("ESH", StringComparison.InvariantCultureIgnoreCase)) return "732";
            if (iso3.Equals("YEM", StringComparison.InvariantCultureIgnoreCase)) return "887";
            if (iso3.Equals("ZMB", StringComparison.InvariantCultureIgnoreCase)) return "894";
            if (iso3.Equals("ZWE", StringComparison.InvariantCultureIgnoreCase)) return "716";
            if (iso3.Equals("ALA", StringComparison.InvariantCultureIgnoreCase)) return "248";

            return null;
        }

        #endregion
    }
}