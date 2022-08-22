//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;


namespace AdvantShop.Repository
{
    public class CityService
    {
        private const string CityCacheKey = "City_";

        #region Get /  Add / Update / Delete 

        public static City GetCity(int cityId)
        {
            if (cityId == 0)
                return null;

            return
                CacheManager.Get(CityCacheKey + cityId,
                    () =>
                        SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[City] WHERE [CityID] = @CityId",
                            CommandType.Text, GetCityFromReader, new SqlParameter("@CityId", cityId)));
        }

        public static string GetPhone(bool isPhoneLink = false)
        {
            var currentCity = GetCity(IpZoneContext.CurrentZone.CityId);

            if (SettingsDesign.IsMobileTemplate) //((MobileHelper.IsMobileBrowser() || MobileHelper.IsMobileForced()) && !MobileHelper.IsDesktopForced())
            {
                return currentCity != null && currentCity.MobilePhoneNumber.IsNotEmpty()
                               ? currentCity.MobilePhoneNumber
                               : SettingsMain.MobilePhone;
            }

            return currentCity != null && currentCity.PhoneNumber.IsNotEmpty()
                               ? isPhoneLink ? currentCity.MobilePhoneNumber : currentCity.PhoneNumber
                               : isPhoneLink ? SettingsMain.MobilePhone : SettingsMain.Phone;
        }

        public static City GetCityFromReader(SqlDataReader reader)
        {
            return new City
            {
                CityId = SQLDataHelper.GetInt(reader, "CityID"),
                Name = SQLDataHelper.GetString(reader, "CityName"),
                District = SQLDataHelper.GetString(reader, "District"),
                RegionId = SQLDataHelper.GetInt(reader, "RegionID"),
                CitySort = SQLDataHelper.GetInt(reader, "CitySort"),
                DisplayInPopup = SQLDataHelper.GetBoolean(reader, "DisplayInPopup"),
                PhoneNumber = SQLDataHelper.GetString(reader, "PhoneNumber"),
                MobilePhoneNumber = SQLDataHelper.GetString(reader, "MobilePhoneNumber"),
                Zip = SQLDataHelper.GetString(reader, "Zip")
            };
        }

        public static void Update(City city)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[City] set CityName=@name, CitySort=@CitySort, RegionId=@RegionId, DisplayInPopup=@DisplayInPopup, PhoneNumber=@PhoneNumber, MobilePhoneNumber=@MobilePhoneNumber, Zip=@Zip, District=@District Where CityID = @id",
                CommandType.Text,
                new SqlParameter("@id", city.CityId),
                new SqlParameter("@name", city.Name),
                new SqlParameter("@CitySort", city.CitySort),
                new SqlParameter("@RegionID", city.RegionId),
                new SqlParameter("@DisplayInPopup", city.DisplayInPopup),
                new SqlParameter("@PhoneNumber", city.PhoneNumber ?? (object)DBNull.Value),
                new SqlParameter("@MobilePhoneNumber", city.MobilePhoneNumber ?? (object)DBNull.Value),
                new SqlParameter("@Zip", city.Zip ?? (object)DBNull.Value),
                new SqlParameter("@District", city.District ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern(CityCacheKey);
        }

        public static void UpdatePhone(int cityId, string phone)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[City] set PhoneNumber=@PhoneNumber Where CityID = @id",
                CommandType.Text,
                new SqlParameter("@id", cityId),
                new SqlParameter("@PhoneNumber", phone));
        }

        public static void Add(City city)
        {
            city.CityId =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert into [Customers].[City] (CityName, RegionID, CitySort, DisplayInPopup, PhoneNumber, MobilePhoneNumber, Zip, District) Values (@Name, @RegionID, @CitySort, @DisplayInPopup, @PhoneNumber, @MobilePhoneNumber, @Zip, @District);SELECT scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@Name", city.Name),
                    new SqlParameter("@CitySort", city.CitySort),
                    new SqlParameter("@RegionID", city.RegionId),
                    new SqlParameter("@DisplayInPopup", city.DisplayInPopup),
                    new SqlParameter("@PhoneNumber", city.PhoneNumber ?? (object)DBNull.Value),
                    new SqlParameter("@MobilePhoneNumber", city.MobilePhoneNumber ?? (object)DBNull.Value),
                    new SqlParameter("@Zip", city.Zip ?? (object)DBNull.Value),
                    new SqlParameter("@District", city.District ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern(CityCacheKey);
        }

        public static void Delete(int cityId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from Customers.City Where CityID=@CityID", CommandType.Text, new SqlParameter("@CityID", cityId));

            CacheManager.RemoveByPattern(CityCacheKey);
        }

        #endregion

        public static List<string> GetCitiesAutocomplete(string name)
        {
            string translitRu = StringHelper.TranslitToRus(name);
            string translitKeyboard = StringHelper.TranslitToRusKeyboard(name);

            return SQLDataAccess.ExecuteReadList("Select Distinct Top (10) CityName From Customers.City WHERE CityName like @name + '%' OR CityName like @translitRu + '%' OR CityName like @translitKeyboard + '%' order by CityName",
                                                    CommandType.Text, reader => SQLDataHelper.GetString(reader, "CityName"),
                                                    new SqlParameter("@name", name),
                                                    new SqlParameter("@translitRu", translitRu),
                                                    new SqlParameter("@translitKeyboard", translitKeyboard));
        }

        public static City GetCityByName(string name)
        {
            return SQLDataAccess.ExecuteReadOne("Select * from Customers.City where CityName=@CityName", 
                CommandType.Text, GetCityFromReader,
                new SqlParameter("@CityName", name));
        }

        public static City GetCityByName(string name, int regionId)
        {
            return SQLDataAccess.ExecuteReadOne("Select * from Customers.City where CityName=@CityName AND [RegionID] = @RegionID", 
                CommandType.Text, GetCityFromReader,
                new SqlParameter("@CityName", name),
                new SqlParameter("@RegionID", regionId));
        }

        public static List<City> GetCitiesByName(string name)
        {
            string translitRu = StringHelper.TranslitToRus(name);
            string translitKeyboard = StringHelper.TranslitToRusKeyboard(name);

            return SQLDataAccess.ExecuteReadList("Select * From Customers.City WHERE CityName = @CityName",
                CommandType.Text, GetCityFromReader,
                new SqlParameter("@CityName", name));
        }

        public static bool IsCityInCountry(int cityId, int countryId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(*) From Customers.City where RegionID in (SELECT RegionID FROM [Customers].[Region] WHERE [CountryID] = @countryID) and CityID=@CityID",
                CommandType.Text,
                new SqlParameter("@CityID", cityId),
                new SqlParameter("@countryID", countryId)) > 0;
        }

        public static int GetCountryIdByCity(int cityId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select CountryId From Customers.Region where RegionId in (SELECT RegionID FROM [Customers].[City] WHERE [CityId] = @CityId)",
                CommandType.Text,
                new SqlParameter("@CityId", cityId));
        }

        public static List<City> GetCitiesByCountryInPopup(int countryId)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select top 40 * From Customers.City Where RegionID in (SELECT RegionID FROM [Customers].[Region] WHERE [CountryID] = @CountryId) and DisplayInPopup=1",
                CommandType.Text,
                GetCityFromReader, new SqlParameter("@CountryId", countryId)).OrderByDescending(city=> city.CitySort).ThenBy(city=> city.Name).ToList();
        }

        public static List<City> GetAll()
        {
            return SQLDataAccess.ExecuteReadList("Select * From Customers.City Order by CitySort desc, CityName asc", CommandType.Text, GetCityFromReader);
        }

        public static List<string> GetUniqueCitiesNames()
        {
            return SQLDataAccess.ExecuteReadColumn<string>("Select DISTINCT CityName From Customers.City Order by CityName asc", CommandType.Text, "CityName");
        }

        public static List<City> GetCitiesByRegion(int regionId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM Customers.City WHERE RegionID = @RegionId Order by CitySort desc, CityName asc",
                CommandType.Text,
                GetCityFromReader,
                new SqlParameter("@RegionId", regionId));
        }

        public static City GetCityByZip(string zip)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM Customers.City WHERE Zip=@Zip", CommandType.Text,
                GetCityFromReader, new SqlParameter("@Zip", zip));
        }
    }
}