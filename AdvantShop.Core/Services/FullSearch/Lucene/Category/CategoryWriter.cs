using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.FullSearch.Core;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.FullSearch
{
    public class CategoryWriter : BaseWriter<CategoryDocument>
    {
        private static bool isRun = false;
        static readonly object locker = new object();
        private static Task _reindexTask;
        public CategoryWriter()
            : base(string.Empty)
        {
        }
        public CategoryWriter(string path)
          : base(path)
        {
        }

        public void AddUpdateToIndex(Category model)
        {
            if (model.ID != 0)
                AddUpdateItemsToIndex(new List<CategoryDocument> { (CategoryDocument)model });
        }


        public void AddUpdateToIndex(List<Category> model)
        {
            AddUpdateItemsToIndex(model.Where(cat => cat.ID != 0).Select(p => (CategoryDocument)p).ToList());
        }


        public void DeleteFromIndex(Category model)
        {
            DeleteItemsFromIndex(new List<CategoryDocument> { (CategoryDocument)model });
        }


        public void DeleteFromIndex(int id)
        {
            DeleteItemsFromIndex(new List<CategoryDocument> { new CategoryDocument { Id = id } });
        }

        //static 
        public static void AddUpdate(Category model)
        {
            try
            {
                using (var writer = new CategoryWriter())
                {
                    writer.AddUpdateToIndex(model);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }


        public static void AddUpdate(List<Category> model)
        {
            using (var writer = new CategoryWriter())
            {
                writer.AddUpdateToIndex(model);
            }
        }


        public static void Delete(Category model)
        {
            using (var writer = new CategoryWriter())
            {
                writer.DeleteFromIndex(model);
            }
        }

        public static void Delete(int id)
        {
            try
            {
                using (var writer = new CategoryWriter())
                {
                    writer.DeleteFromIndex(id);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public static Task CreateIndexFromDbInTask()
        {
            return Task.Factory.StartNew(_createIndexFromDb, TaskCreationOptions.LongRunning);
        }

        public static void CreateIndexFromDb()
        {
            if (_reindexTask == null)
                _reindexTask = CreateIndexFromDbInTask();
            _reindexTask.Wait();
            _reindexTask = null;
        }

        private static void _createIndexFromDb()
        {
            if (isRun) return;
            isRun = true;
            
            lock (locker)
            {
                try
                {
                    var basePath = BasePath(nameof(CategoryDocument));
                    var tempPath = basePath + "_temp";
                    var mergePath = basePath + "_temp2";
                    var cats = CategoryService.GetCategories();

                    using (var writer = new CategoryWriter(tempPath))
                    {
                        foreach (var item in cats)
                            writer.AddUpdateToIndex(item);
                        
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
                    Debug.Log.Error("CategoryWriter _createIndexFromDb", ex);
                }
            }
            isRun = false;
        }
    }
}