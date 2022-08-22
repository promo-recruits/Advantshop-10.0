using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.FullSearch.Core;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace AdvantShop.Core.Services.FullSearch
{
    public class ProductSeacher : BaseSearcher<ProductDocument>
    {
        public ProductSeacher(int hitsLimit, ESearchDeep deepLimit) : base(hitsLimit, deepLimit)
        {
        }

        public ProductSeacher(int hitsLimit, List<ESearchDeep> deepLimits) : base(hitsLimit, deepLimits)
        {
        }

        protected override BooleanQuery ProcessCondition(BooleanQuery bq)
        {
            var conditionParser = new QueryParser(CurrentVersion, Nameof<ProductDocument>.Property(e => e.Enabled), _analyzer);
            var conditionQuery = ParseQuery(true.ToString(), conditionParser);
            bq.Add(conditionQuery, Occur.MUST);

            conditionParser = new QueryParser(CurrentVersion, Nameof<ProductDocument>.Property(e => e.Hidden), _analyzer);
            conditionQuery = ParseQuery(false.ToString(), conditionParser);
            bq.Add(conditionQuery, Occur.MUST);

            return bq;
        }

        public static SearchResult Search(string searchTerm, string field = "", bool minimizeSearchResults = false)
        {
            var deep = searchTerm.Length == 1
                ? new List<ESearchDeep>() {ESearchDeep.WordsStartFrom, ESearchDeep.WordsBetween}
                : new List<ESearchDeep>() {SettingsCatalog.SearchDeep};

            using (var searcher = new ProductSeacher(SettingsCatalog.SearchMaxItems, deep))
            {
                var res = searcher.SearchItems(searchTerm, field, minimizeSearchResults: minimizeSearchResults);

                if (minimizeSearchResults && res.SearchResultItems.Count == 0)
                    res = searcher.SearchItems(searchTerm, field);

                return res;
            }
        }
    }
}
