//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using AdvantShop.Core.Services.FullSearch;
using System;

namespace AdvantShop.FullSearch
{
    public class LuceneSearch 
    {
        public static void CreateNewIndex<T>() where T : BaseDocument
        {
            var type = typeof(T);
            if (type == typeof(ProductDocument))
                ProductWriter.CreateIndexFromDb();
            else if (type == typeof(CategoryDocument))
                CategoryWriter.CreateIndexFromDb();
            else
                throw new Exception(type + " is unknown type");
        }

        public static void CreateAllIndex()
        {
            ProductWriter.CreateIndexFromDb();
            CategoryWriter.CreateIndexFromDb();
        }

        public static void CreateAllIndex(int categoryId)
        {
            ProductWriter.CreateIndexFromDb(categoryId);
        }

        public static void CreateAllIndexInBackground()
        {
            ProductWriter.CreateIndexFromDbInTask();
            CategoryWriter.CreateIndexFromDbInTask();
        }
    }
}