using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Api
{
    public class ApiPagination
    {
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("totalPageCount")]
        public int TotalPageCount { get; set; }

        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class ApiPaginationResponse
    {
        [JsonProperty("pagination")]
        public ApiPagination Pagination { get; set; }
    }
}
