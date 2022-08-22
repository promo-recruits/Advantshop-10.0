using System.Collections.Generic;
using AdvantShop.Core.Services.FullSearch.Core;

namespace AdvantShop.Core.Services.FullSearch
{
    public class ProductSeacherAdmin : BaseSearcher<ProductDocument>
    {
        public ProductSeacherAdmin(int hitsLimit)
            : base(hitsLimit, new List<ESearchDeep> { ESearchDeep.StrongPhase, ESearchDeep.SepareteWords, ESearchDeep.WordsStartFrom, ESearchDeep.WordsBetween })
        {
        }

        public static SearchResult Search(string searchTerm, int limit = 10000, string field = "")
        {
            using (var searcher = new ProductSeacherAdmin(limit))
            {
                var res = searcher.SearchItems(searchTerm, field);
                return res;
            }
        }

        protected override List<string> GetIgnoredFields()
        {
            return new List<string>() { "Tags", "Desc" };
        }
    }
}
