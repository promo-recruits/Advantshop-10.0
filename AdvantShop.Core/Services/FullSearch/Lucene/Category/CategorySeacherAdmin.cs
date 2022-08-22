using AdvantShop.Core.Services.FullSearch.Core;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.FullSearch
{
    public class CategorySeacherAdmin : BaseSearcher<CategoryDocument>
    {
        public CategorySeacherAdmin(int hitsLimit) : base(hitsLimit, ESearchDeep.WordsBetween)
        {
        }

        public static SearchResult Search(string searchTerm, int limit = 10000, string field = "")
        {
            using (var searcher = new CategorySeacherAdmin(limit))
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
