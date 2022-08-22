using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class LocationController : BaseClientController
    {
        public JsonResult GetCities(int countryId)
        {
            var cities = new object();
            var country = new Country();
            var zone = IpZoneContext.CurrentZone;

            if (countryId != 0)
            {
                cities = CityService.GetCitiesByCountryInPopup(countryId).Select(item => new { item.CityId, item.Name, item.RegionId, item.Zip, item.District }).ToList();
                country = CountryService.GetCountry(countryId);
            }
            else
            {
                cities = CityService.GetCitiesByCountryInPopup(zone.CountryId).Select(item => new { item.CityId, item.Name, item.RegionId, item.Zip, item.District }).ToList();
                country = CountryService.GetCountry(zone.CountryId);
            }

            return Json(new
            {
                CountryID = country.CountryId,
                CountryName = country.Name,
                CountryCities = cities,
            });
        }

        public JsonResult GetDataForPopup()
        {
            var result = new List<object>();
            var countries = CountryService.GetCountriesByDisplayInPopup().Select(item => new { item.CountryId, item.Name, item.Iso2 });

            foreach (var country in countries)
            {
                result.Add(new
                {
                    country.CountryId,
                    country.Name,
                    country.Iso2,
                    Cities = CityService.GetCitiesByCountryInPopup(country.CountryId).Select(item => new { item.CityId, item.Name, item.RegionId, item.Zip, item.District })
                });
            }

            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        [CacheFilter(Duration = 604800 /*1 week*/, LastModifiedPeriod = 3600)]
        public JsonResult GetCurrentZone()
        {
            var zone = IpZoneContext.CurrentZone;
            var city = CityService.GetCity(zone.CityId);
            var phoneNumber = city != null && city.PhoneNumber.IsNotEmpty() ? city.PhoneNumber: SettingsMain.Phone;
            var mobilePhoneNumber = city != null && city.MobilePhoneNumber.IsNotEmpty() ? city.MobilePhoneNumber : SettingsMain.MobilePhone;

            return Json(new
            {
                current = new IpZoneModel()
                {
                    CountryId = zone.CountryId,
                    CountryName = zone.CountryName,
                    RegionId = zone.RegionId,
                    Region = zone.Region,
                    CityId = zone.CityId,
                    City = zone.City,
                    District = zone.District,
                    Zip = zone.Zip,
                    Phone = phoneNumber,
                    MobilePhone = mobilePhoneNumber
                }
            });
        }

        public JsonResult SetZone(string city, int? cityId, int? countryId, string regionName, string countryName, string zip, string district)
        {
            Country country = null;
            var zone = cityId.HasValue
                ? IpZoneService.GetZoneByCityId(cityId.Value)
                : IpZoneService.GetZoneByCity(city.Trim().ToLower(), countryId);

            if (zone == null)
            {
                var region = regionName.IsNotEmpty()
                    ? RegionService.GetRegionsListByName(regionName.Trim()).FirstOrDefault()
                    : null;
                country = (countryId.HasValue ? CountryService.GetCountry(countryId.Value) : null)
                    ?? (countryName.IsNotEmpty() ? CountryService.GetCountryByName(countryName) : null)
                    ?? (region != null ? CountryService.GetCountry(region.CountryId) : null)
                    ?? CountryService.GetCountry(SettingsMain.SellerCountryId);

                zone = new IpZone()
                {
                    CountryId = country != null ? country.CountryId : SettingsMain.SellerCountryId,
                    CountryName = country != null ? country.Name : countryName,
                    RegionId = region != null ? region.RegionId : 0,
                    Region = region != null ? region.Name : regionName,
                    City = HttpUtility.HtmlEncode(city.Trim()),
                    District = HttpUtility.HtmlEncode(district.DefaultOrEmpty().Trim()),
                    Zip = zip
                };
            }
            else if(!string.IsNullOrEmpty(zip))
            {
                zone.Zip = zip;
            }

            ModulesExecuter.OnSetZone(zone);

            IpZoneContext.SetCustomerCookie(zone);
            if (zone.Region.IsNotEmpty() && zone.City.IsNotEmpty() &&
                (country != null || (country = CountryService.GetCountry(zone.CountryId)) != null))
            {
                IpZoneContext.SendGeoIpData(new GeoIpData
                {
                    Country = country.Iso2,
                    State = zone.Region,
                    City = zone.City
                });
            }

            var cityObj = cityId.HasValue ? CityService.GetCity(cityId.Value) : CityService.GetCityByName(zone.City);
            var phoneNumber = cityObj != null && cityObj.PhoneNumber.IsNotEmpty() ? cityObj.PhoneNumber : SettingsMain.Phone;
            var mobilePhoneNumber = cityObj != null && cityObj.MobilePhoneNumber.IsNotEmpty() ? cityObj.MobilePhoneNumber : SettingsMain.MobilePhone;

            return Json(new IpZoneModel()
            {
                CountryId = zone.CountryId,
                CountryName = zone.CountryName,
                RegionId = zone.RegionId,
                Region = zone.Region,
                CityId = zone.CityId,
                City = zone.City,
                District = zone.District,
                Phone = phoneNumber,
                MobilePhone = mobilePhoneNumber,
                Zip = zone.Zip
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ApproveZone()
        {
            var zone = IpZoneContext.CurrentZone;
            var country = zone.CountryId != 0 ? CountryService.GetCountry(zone.CountryId) : null;

            IpZoneContext.SendGeoIpData(new GeoIpData
            {
                Country = country != null ? country.Iso2 : string.Empty,
                State = zone.Region,
                City = zone.City
            }, true);
            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetCitiesAutocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null, JsonRequestBehavior.AllowGet);

            var ipZones = IpZoneService.GetIpZonesByCity(q);
            var locations = ipZones.Select(x => (LocationModel)x).ToList();
            if (ipZones.Count < 5)
            {
                var zones = ModulesExecuter.GetIpZonesAutocomplete(q);
                locations.AddRange(
                    zones.Where(x => !ipZones.Any(zone =>
                        zone.Region.DefaultOrEmpty().Equals(x.Region.DefaultOrEmpty(), StringComparison.OrdinalIgnoreCase) &&
                        zone.City.DefaultOrEmpty().Equals(x.City.DefaultOrEmpty(), StringComparison.OrdinalIgnoreCase) &&
                        zone.District.DefaultOrEmpty().Equals(x.District.DefaultOrEmpty(), StringComparison.OrdinalIgnoreCase))
                    ).Select(x => (LocationModel)x));
            }

            return Json(locations, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRegionsAutocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json((from item in RegionService.GetRegionsByName(q)
                         select new
                         {
                             Name = item
                         }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCountriesAutocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json((from item in CountryService.GetCountriesByName(q)
                         select new
                         {
                             Name = item
                         }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCountries()
        {
            return Json(CountryService.GetAllCountries(), JsonRequestBehavior.AllowGet);
        }
    }
}