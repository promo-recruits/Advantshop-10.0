using System;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.FullSearch.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.Services.FullSearch
{
    public class ProductWriter : BaseWriter<ProductDocument>
    {
        private static bool isRun = false;
        static readonly object locker = new object();
        private static Task _reindexTask;
        public ProductWriter()
            : base(string.Empty)
        {
        }
        public ProductWriter(string path)
            : base(path)
        {
        }

        public void AddUpdateToIndex(Product model)
        {
            AddUpdateItemsToIndex(new List<ProductDocument> { (ProductDocument)model });
        }

        public void AddUpdateToIndex(ProductLuceneDto model)
        {
            AddUpdateItemsToIndex(new List<ProductDocument> { (ProductDocument)model });
        }

        public void AddUpdateToIndex(List<Product> model)
        {
            AddUpdateItemsToIndex(model.Select(p => (ProductDocument)p).ToList());
        }

        public void DeleteFromIndex(Product model)
        {
            DeleteItemsFromIndex(new List<ProductDocument> { (ProductDocument)model });
        }

        public void DeleteFromIndex(int id)
        {
            DeleteItemsFromIndex(new List<ProductDocument> { new ProductDocument { Id = id } });
        }

        //static 
        public static void AddUpdate(Product model)
        {
            TryDo(() =>
            {
                using (var writer = new ProductWriter())
                    writer.AddUpdateToIndex(model);
            });
        }

        public static void AddUpdate(List<Product> model)
        {
            TryDo(() =>
            {
                using (var writer = new ProductWriter())
                    writer.AddUpdateToIndex(model);
            });
        }


        public static void Delete(Product model)
        {
            TryDo(() =>
            {
                using (var writer = new ProductWriter())
                    writer.DeleteFromIndex(model);
            });
        }

        public static void Delete(int id)
        {
            TryDo(() =>
            {
                using (var writer = new ProductWriter())
                    writer.DeleteFromIndex(id);
            });
        }

        public static Task CreateIndexFromDbInTask()
        {
            return Task.Factory.StartNew(() => CreateIndexFromDb(null), TaskCreationOptions.LongRunning);
        }

        public static void CreateIndexFromDb()
        {
            if (_reindexTask == null || _reindexTask.IsFaulted)
            {
                _reindexTask = CreateIndexFromDbInTask();
            }
            _reindexTask.Wait();
            _reindexTask = null;
        }

        public static void CreateIndexFromDb(int? categoryId)
        {
            if (isRun) return;

            isRun = true;
            lock (locker)
            {
                try
                {
                    var basePath = BasePath(nameof(ProductDocument));
                    var tempPath = basePath + "_temp";
                    var mergePath = basePath + "_temp2";

                    var offers = GetOffers();
                    var tags = GetTags();
                    using (var writer = new ProductWriter(tempPath))
                    {
                        foreach (var item in GetProducts(categoryId))
                        {
                            item.Offers = offers.ContainsKey(item.ProductId)
                                ? offers[item.ProductId]
                                : new List<ProductOfferLuceneDto>();
                            item.Tags = tags.ContainsKey(item.ProductId)
                                ? tags[item.ProductId]
                                : new List<ProductTagLuceneDto>();
                            writer.AddUpdateToIndex(item);
                        }

                        writer.Optimize();
                    }

                    FileHelpers.CreateDirectory(basePath);

                    if (Directory.Exists(mergePath))
                    {
                        Directory.Delete(mergePath, true);
                    }

                    Directory.Move(basePath, mergePath);
                    Directory.Move(tempPath, basePath);
                    Directory.Delete(mergePath, true);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error("ProductWriter CreateIndexFromDb", ex);
                }
            }
            isRun = false;
        }


        private static IEnumerable<ProductLuceneDto> GetProducts(int? categoryId)
        {
            //если кол-во категорий товара равно колву в которых он выключен то скрываем
            var sql = "SELECT p.ProductId, p.ArtNo, p.Name, p.Description, p.Enabled, p.CategoryEnabled, p.AllowPreOrder, case when pMap.HiddenCount=pMap.CatCount then 1 else 0 end [Hidden] " +
                     "FROM [Catalog].[Product] p " +
                     "INNER JOIN (select pc.productid, count(pc.productid) CatCount, sum(1 * c.Hidden) HiddenCount from [Catalog].[ProductCategories] pc " +
                     "INNER JOIN [Catalog].[Category] c ON pc.[CategoryId] = c.CategoryId " +
                     (categoryId.HasValue ? "inner join [Catalog].[ProductCategories] pcProduct on pc.productId = pcProduct.productId and pcProduct.CategoryId = " + categoryId.Value
                                          : string.Empty) +
                     " group by pc.productid) pMap on p.productid = pMap.productid";

            var products = SQLDataAccess.ExecuteReadIEnumerable(sql, CommandType.Text, reader => new ProductLuceneDto()
            {
                ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                CategoryEnabled = SQLDataHelper.GetBoolean(reader, "CategoryEnabled"),
                AllowPreOrder = SQLDataHelper.GetBoolean(reader, "AllowPreOrder"),
                Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
            });

            if (categoryId != null)
                return products;
            
            sql =
                "SELECT [Product].ProductId, [Product].ArtNo, [Product].Name, [Product].Description, [Product].Enabled, [Product].CategoryEnabled, [Product].AllowPreOrder, 1 as Hidden " +
                "FROM [Catalog].[Product] " +
                "WHERE not Exists (Select 1 From [Catalog].[ProductCategories] Where [ProductCategories].[ProductId] = [Product].[ProductId])";

            var productsWithoutCategory = SQLDataAccess.ExecuteReadIEnumerable(sql, CommandType.Text, reader => new ProductLuceneDto()
            {
                ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                CategoryEnabled = SQLDataHelper.GetBoolean(reader, "CategoryEnabled"),
                AllowPreOrder = SQLDataHelper.GetBoolean(reader, "AllowPreOrder"),
                Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
            });

            return products.Concat(productsWithoutCategory);
        }

        private static Dictionary<int, List<ProductOfferLuceneDto>> GetOffers()
        {
            var sqloffer = "SELECT Offer.ProductID, Offer.ArtNo, Offer.Amount FROM Catalog.Offer";
            var offers = SQLDataAccess.Query<ProductOfferLuceneDto>(sqloffer)
                                      .ToList()
                                      .GroupBy(x => new { x.ProductId })
                                      .ToDictionary(x => x.Key.ProductId, x => x.ToList());
            return offers;
        }

        private static Dictionary<int, List<ProductTagLuceneDto>> GetTags()
        {
            var sqlTags = "SELECT TagMap.ObjId as ProductId, Name from Catalog.Tag Inner Join Catalog.TagMap on Tag.Id = TagMap.TagId Where TagMap.Type = @Type AND Enabled=1";
            var tags = SQLDataAccess.Query<ProductTagLuceneDto>(sqlTags, new { Type = ETagType.Product.ToString() })
                                    .ToList().GroupBy(x => new { x.ProductId })
                                    .ToDictionary(x => x.Key.ProductId, x => x.ToList());
            return tags;
        }

        private static void TryDo(Action action, bool isSecond = false)
        {
            if (isRun) return;

            lock (locker)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (!isSecond && ex.Message != null &&
                        (ex.Message.Contains("Could not find file") || ex.Message.Contains("не найден")))
                    {
                        if (TryRecreateIndexes())
                        {
                            TryDo(action, true);
                            return;
                        }
                    }
                    Debug.Log.Warn(ex);
                }
            }
        }

        private static bool TryRecreateIndexes()
        {
            try
            {
                var basePath = BasePath(typeof(ProductDocument).Name);

                FileHelpers.DeleteDirectory(basePath, false);

                CreateIndexFromDb();
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
                return false;
            }

            return true;
        }
    }

    public class ProductLuceneDto
    {
        public int ProductId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool CategoryEnabled { get; set; }
        public bool AllowPreOrder { get; set; }
        public bool Hidden { get; set; }
        public List<ProductOfferLuceneDto> Offers { get; set; }
        public List<ProductTagLuceneDto> Tags { get; set; }
    }

    public class ProductOfferLuceneDto
    {
        public int ProductId { get; set; }
        public string ArtNo { get; set; }
        public float Amount { get; set; }
    }

    public class ProductTagLuceneDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
    }
}