using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket
{
    #region Responses
    public class OkMarketApiBaseResponse
    {
        public bool Success
        {
            get
            {
                return ErrorCode == 0 && ErrorMsg == null;
            }
        }
        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }
        [DefaultValue(0)]
        [JsonProperty("error_code", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int ErrorCode { get; set; }
        [JsonProperty("wait_millis")]
        public int? Millis { get; set; }
    }

    public class OkMarketApiGetGroupResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("groups")]
        public List<OkMarketGroup> Groups { get; set; }
    }

    public class OkMarketApiAddCatalogResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("catalog_id")]
        public long CatalogId { get; set; }
    }

    public class OkMarketApiGetCatalogResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("catalogs")]
        public List<OkMarketCatalog> Catalogs { get; set; }
    }

    public class OkMarketApiAddProductResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("product_id")]
        public long ProductId { get; set; }
    }

    public class OkMarketApiGetProductsResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("anchor")]
        public string Anchor { get; set; }
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
        [JsonProperty("products")]
        public List<OkMarketProduct> Products { get; set; }
        [JsonProperty("short_products")]
        public List<OkMarketProduct> ShortProducts { get; set; }
    }

    public class OkMarketApiPhotoUploadResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("photos")]
        public Dictionary<string, OkMarketPhotoToken> Photos { get; set; }

        [JsonProperty("photo_ids")]
        public List<string> PhotoIds { get; set; }
        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }
    }

    public class OkMarketApiGetPhotoResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("photo")]
        public OkMarketPhoto Photo { get; set; }
    }

    public class OKMarketApiGetIdFromUrlResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("objectIdStr")]
        public string IdString { get; set; }
        [JsonProperty("objectId")]
        public long Id { get; set; }
    }
    #endregion
    #region Request
    public class OkMarketApiRequest
    {
        public string access_token { get; set; }
        public string application_key { get; set; }
        public string attachment { get; set; }
        public string anchor { get; set; }
        public string catalog_ids { get; set; }
        public string catalog_id { get; set; }
        public int? count { get; set; }
        public bool? delete_products { get; set; }
        public string fields { get; set; }
        public string format { get; set; }
        public string gid { get; set; }
        public string method { get; set; }
        public string name { get; set; }
        public string photos { get; set; }
        public bool? placeholders { get; set; }
        public string product_id { get; set; }
        public string product_ids { get; set; }
        public string sig { get; set; }
        public string tab { get; set; }
        public string type { get; set; }
        public string uids { get; set; }
        public string photo_id { get; set; }
        public string product_status { get; set; }
        public string url { get; set; }

        public void UpdateSinature(string secretKey)
        {
            var sortedParams = this.ToString("", forSignature: true);
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(sortedParams + secretKey));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            sig = hash.ToString().ToLower();
        }

        public string ToString(string separator, bool codeActtachmet = false, bool forSignature = false)
        {
            return string.Join(separator, ToDictionary().Where(x => x.Value != null && (forSignature ? (x.Key != "access_token" && x.Key != "sig") : true)).
                OrderBy(x => x.Key).Select(x => x.Key + "=" + (codeActtachmet ? x.Value.ToString().Replace("%", "%25").Replace("+", "%2B").Replace("&", "%26") : x.Value.ToString())));
        }

        private IDictionary<string, object> ToDictionary (BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return this.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(this, null)
            );
        }
    }
    #endregion
}