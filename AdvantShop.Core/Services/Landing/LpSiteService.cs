using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Domains;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Landing
{
    public class LpSiteService
    {
        public int Add(LpSite site)
        {
            if (SaasDataService.IsSaasEnabled)
            {
                if (GetLandingSiteCount() >= SaasDataService.CurrentSaasData.LandingFunnelCount)
                    throw new BlException("Количество воронок по тарифному плану не может превышать " + SaasDataService.CurrentSaasData.LandingFunnelCount);
            }

            site.Id =
                Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                    "INSERT INTO [CMS].[LandingSite] ([Name],[Enabled],[Template],[Url],[DomainUrl],[CreatedDate],[ProductId],ModifiedDate) " +
                    "VALUES (@Name,@Enabled,@Template,@Url,@DomainUrl,getdate(),@ProductId,getdate()); Select scope_identity(); ",
                    CommandType.Text,
                    new SqlParameter("@Name", site.Name ?? ""),
                    new SqlParameter("@Enabled", site.Enabled),
                    new SqlParameter("@Template", site.Template ?? ""),
                    new SqlParameter("@Url", site.Url ?? ""),
                    new SqlParameter("@DomainUrl", site.DomainUrl ?? ""),
                    new SqlParameter("@ProductId", site.ProductId ?? (object)DBNull.Value)
                    ));

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);

            OrderSourceService.AddOrderSource(OrderType.LandingPage, objId: site.Id, objName: site.Name);

            return site.Id;
        }

        public void Update(LpSite site)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CMS].[LandingSite] " +
                "Set [Name] = @Name, [Enabled] = @Enabled, [Template] = @Template, [Url] = @Url, [DomainUrl] = @DomainUrl, [ProductId] = @ProductId, [ScreenShot] = @ScreenShot, ModifiedDate = getdate() " +
                "Where Id = @Id",
                CommandType.Text,
                new SqlParameter("@Name", site.Name ?? ""),
                new SqlParameter("@Enabled", site.Enabled),
                new SqlParameter("@Template", site.Template ?? ""),
                new SqlParameter("@Url", site.Url ?? ""),
                new SqlParameter("@DomainUrl", site.DomainUrl ?? ""),
                new SqlParameter("@ProductId", site.ProductId ?? (object)DBNull.Value),
                new SqlParameter("@ScreenShot", site.ScreenShot ?? (object)DBNull.Value),
                new SqlParameter("@Id", site.Id)
                );

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);
        }

        public static void UpdateScreenShot(int siteId, string screenShot)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CMS].[LandingSite] Set [ScreenShot] = @ScreenShot, ModifiedDate = getdate(), ScreenShotDate = getdate() Where Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", siteId),
                new SqlParameter("@ScreenShot", screenShot));
        }

        public static void UpdateModifiedDate(int siteId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CMS].[LandingSite] Set ModifiedDate = getdate() Where Id = @Id",
                CommandType.Text, 
                new SqlParameter("@Id", siteId));
        }

        public static void UpdateModifiedDateByLandingId(int landingId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CMS].[LandingSite] " +
                "Set ModifiedDate = getdate() " +
                "Where Id = (Select LandingSiteId From [CMS].[Landing] Where Id=@LandingId)",
                CommandType.Text, 
                new SqlParameter("@LandingId", landingId));
        }

        public void Delete(int id)
        {
            var site = Get(id);
            if (site == null)
                return;

            var lpService = new LpService();
            
            foreach (var page in lpService.GetList(id))
            {
                lpService.Delete(page.Id);
            }

            SQLDataAccess.ExecuteNonQuery("Delete From [CMS].[LandingSite] Where Id = @Id", CommandType.Text, new SqlParameter("@Id", id));

            if (!string.IsNullOrEmpty(site.DomainUrl))
                Task.Run(() =>
                {
                    try
                    {
                        new DomainService().Remove(site.DomainUrl);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                });

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);

            OrderSourceService.DeleteOrderSource(OrderType.LandingPage, id);
        }

        public List<LpSite> GetList()
        {
            return SQLDataAccess.Query<LpSite>("Select * From [CMS].[LandingSite]").ToList();
        }

        public int GetLandingSiteCount()
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(Id) From [CMS].[LandingSite]", CommandType.Text);
        }

        public LpSite Get(int id)
        {
            return CacheManager.Get(LpConstants.LpCachePrefix + "_site_" + id, LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<LpSite>("Select * From [CMS].[LandingSite] Where Id = @id", new {id})
                        .FirstOrDefault());
        }

        public LpSite GetByLandingBlockId(int lpBlockId)
        {
            return
                SQLDataAccess.Query<LpSite>(
                    "Select * From [CMS].[LandingSite] " +
                    "Where Id = (Select top(1) LandingSiteId from [CMS].[Landing] Where [Landing].[Id] = (Select top(1) LandingId from [CMS].[LandingBlock] Where Id=@lpBlockId))",
                    new { lpBlockId }).FirstOrDefault();
        }

        public LpSite GetByDomainUrl(string domainUrl)
        {
            return CacheManager.Get(LpConstants.LpCachePrefix + "_site_" + domainUrl, LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<LpSite>("Select * From [CMS].[LandingSite] Where DomainUrl = @domainUrl", new { domainUrl })
                        .FirstOrDefault());
        }

        public LpSite GetByUrl(string url)
        {
            return CacheManager.Get(LpConstants.LpCachePrefix + "_site_url_" + url, LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<LpSite>("Select * From [CMS].[LandingSite] Where Url = @url", new { url })
                        .FirstOrDefault());
        }

        public LpSite GetByName(string name)
        {
            return SQLDataAccess.Query<LpSite>("Select * From [CMS].[LandingSite] Where LOWER(Name) = @name", new { name = name.ToLower() })
                        .FirstOrDefault();
        }

        public string GetAvailableUrl(string name)
        {
            var translit = StringHelper.TransformUrl(StringHelper.Translit(name));
            var url = translit;
            var i = 1;

            while (GetByUrl(url) != null)
            {
                url = translit + "-" + (i++);
                if (i == 15)
                    url = Guid.NewGuid().ToString();
            }

            return url;
        }


        #region Link product and landing site

        public LpSite GetByAdditionalSalesProductId(int productId)
        {
            return
                SQLDataAccess.Query<LpSite>(
                    "Select top(1) * From [CMS].[LandingSite] " +
                    "Inner Join [CMS].[LandingSite_Product] On LandingSite.Id = LandingSite_Product.LandingSiteId " +
                    "Where [LandingSite_Product].ProductId=@productId",
                    new { productId }).FirstOrDefault();
        }

        public void AddAdditionalSalesProduct(int productId, int siteId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [CMS].[LandingSite_Product] (ProductId, LandingSiteId) Values (@ProductId, @LandingSiteId)",
                CommandType.Text,
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@LandingSiteId", siteId));
        }

        public void AddUpdateAdditionalSalesProduct(int productId, int siteId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT(ProductId) FROM CMS.LandingSite_Product WHERE ProductId = @ProductId) = 0 " +
                "BEGIN " +
                    "INSERT INTO CMS.LandingSite_Product (ProductId, LandingSiteId) VALUES (@ProductId, @LandingSiteId) " +
                "END ELSE BEGIN " +
                    "UPDATE CMS.LandingSite_Product SET LandingSiteId = @LandingSiteId WHERE ProductId = @ProductId " +
                "END",
                CommandType.Text,
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@LandingSiteId", siteId));
        }

        public void DeleteAdditionalSalesProduct(int productId, int siteId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [CMS].[LandingSite_Product] Where ProductId=@ProductId and LandingSiteId=@LandingSiteId",
                CommandType.Text,
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@LandingSiteId", siteId));
        }

        #endregion
    }
}
