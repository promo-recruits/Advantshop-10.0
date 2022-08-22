namespace AdvantShop.Repository
{
    public class IpZoneModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int RegionId { get; set; }
        public string Region { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Zip { get; set; }
    }
}