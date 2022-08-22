//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Catalog
{
    public class CouponService
    {

        #region Get, Add, Update, Delete

        public static Coupon GetCoupon(int couponId)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Catalog].[Coupon] WHERE CouponID = @CouponID", CommandType.Text, GetFromReader, new SqlParameter("@CouponID", couponId));
        }

        public static Coupon GetCouponByCode(string code)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Catalog].[Coupon] WHERE Code = @Code", CommandType.Text, GetFromReader, new SqlParameter("@Code", code));
        }

        public static List<Coupon> GetAllCoupons()
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Catalog].[Coupon]", CommandType.Text, GetFromReader);
        }

        private static Coupon GetFromReader(SqlDataReader reader)
        {
            return new Coupon
            {
                CouponID = SQLDataHelper.GetInt(reader, "CouponID"),
                Code = SQLDataHelper.GetString(reader, "Code"),
                Type = (CouponType) SQLDataHelper.GetInt(reader, "Type"),
                Value = SQLDataHelper.GetFloat(reader, "Value"),
                AddingDate = SQLDataHelper.GetDateTime(reader, "AddingDate"),
                ExpirationDate = SQLDataHelper.GetNullableDateTime(reader, "ExpirationDate"),
                PossibleUses = SQLDataHelper.GetInt(reader, "PossibleUses"),
                ActualUses = SQLDataHelper.GetInt(reader, "ActualUses"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                MinimalOrderPrice = SQLDataHelper.GetFloat(reader, "MinimalOrderPrice"),
                IsMinimalOrderPriceFromAllCart = SQLDataHelper.GetBoolean(reader, "IsMinimalOrderPriceFromAllCart"),
                CurrencyIso3 = SQLDataHelper.GetString(reader, "CurrencyIso3"),
                TriggerActionId = SQLDataHelper.GetNullableInt(reader, "TriggerActionId"),
                TriggerId = SQLDataHelper.GetNullableInt(reader, "TriggerId"),
                Mode = (CouponMode)SQLDataHelper.GetInt(reader, "Mode"),
                Days = SQLDataHelper.GetNullableInt(reader, "Days"),
                CustomerId = SQLDataHelper.GetNullableGuid(reader, "CustomerId"),
                StartDate = SQLDataHelper.GetNullableDateTime(reader, "StartDate"),
                ForFirstOrder = SQLDataHelper.GetBoolean(reader, "ForFirstOrder"),
            };
        }

        public static void AddCoupon(Coupon coupon)
        {
            coupon.CouponID = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Catalog].[Coupon] " +
                "([Code], [Type], [Value], [AddingDate], [ExpirationDate], [PossibleUses], [ActualUses], [Enabled], [MinimalOrderPrice], IsMinimalOrderPriceFromAllCart, CurrencyIso3, TriggerActionId, Mode, Days, TriggerId, CustomerId, StartDate, ForFirstOrder, EntityId) " +
                "VALUES (@Code, @Type, @Value, @AddingDate, @ExpirationDate, @PossibleUses, @ActualUses, @Enabled, @MinimalOrderPrice, @IsMinimalOrderPriceFromAllCart, @CurrencyIso3, @TriggerActionId, @Mode, @Days, @TriggerId, @CustomerId, @StartDate, @ForFirstOrder, @EntityId); " +
                "SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Code", coupon.Code),
                new SqlParameter("@Type", coupon.Type),
                new SqlParameter("@Value", coupon.Value),
                new SqlParameter("@AddingDate", coupon.AddingDate),
                new SqlParameter("@ExpirationDate", coupon.ExpirationDate ?? (object) DBNull.Value),
                new SqlParameter("@PossibleUses", coupon.PossibleUses),
                new SqlParameter("@ActualUses", coupon.ActualUses),
                new SqlParameter("@Enabled", coupon.Enabled),
                new SqlParameter("@MinimalOrderPrice", coupon.MinimalOrderPrice),
                new SqlParameter("@IsMinimalOrderPriceFromAllCart", coupon.IsMinimalOrderPriceFromAllCart),
                new SqlParameter("@CurrencyIso3", coupon.CurrencyIso3),
                new SqlParameter("@TriggerActionId", coupon.TriggerActionId ?? (object) DBNull.Value),
                new SqlParameter("@TriggerId", coupon.TriggerId ?? (object) DBNull.Value),
                new SqlParameter("@Mode", coupon.Mode),
                new SqlParameter("@Days", coupon.Days ?? (object) DBNull.Value),
                new SqlParameter("@CustomerId", coupon.CustomerId ?? (object) DBNull.Value),
                new SqlParameter("@StartDate", coupon.StartDate ?? (object) DBNull.Value),
                new SqlParameter("@ForFirstOrder", coupon.ForFirstOrder),
                new SqlParameter("@EntityId", coupon.EntityId ?? (object) DBNull.Value)
                );
        }

        public static void UpdateCoupon(Coupon coupon)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[Coupon] " +
                "SET [Code] = @Code, [Type] = @Type, [Value] = @Value, [AddingDate]=@AddingDate, [ExpirationDate] = @ExpirationDate, " +
                "[PossibleUses] = @PossibleUses, [ActualUses] = @ActualUses, [Enabled] = @Enabled, [MinimalOrderPrice] = @MinimalOrderPrice, " +
                "IsMinimalOrderPriceFromAllCart = @IsMinimalOrderPriceFromAllCart, CurrencyIso3 = @CurrencyIso3, " +
                "TriggerActionId = @TriggerActionId, Mode=@Mode, Days=@Days, TriggerId=@TriggerId, CustomerId=@CustomerId, " +
                "StartDate = @StartDate, ForFirstOrder = @ForFirstOrder " +
                "WHERE CouponID = @CouponID", CommandType.Text,
                new SqlParameter("@CouponID", coupon.CouponID),
                new SqlParameter("@Code", coupon.Code),
                new SqlParameter("@Type", coupon.Type),
                new SqlParameter("@Value", coupon.Value),
                new SqlParameter("@AddingDate", coupon.AddingDate),
                new SqlParameter("@ExpirationDate", coupon.ExpirationDate ?? (object) DBNull.Value),
                new SqlParameter("@PossibleUses", coupon.PossibleUses),
                new SqlParameter("@ActualUses", coupon.ActualUses),
                new SqlParameter("@Enabled", coupon.Enabled),
                new SqlParameter("@MinimalOrderPrice", coupon.MinimalOrderPrice),
                new SqlParameter("@IsMinimalOrderPriceFromAllCart", coupon.IsMinimalOrderPriceFromAllCart),
                new SqlParameter("@CurrencyIso3", coupon.CurrencyIso3),
                new SqlParameter("@TriggerActionId", coupon.TriggerActionId ?? (object) DBNull.Value),
                new SqlParameter("@TriggerId", coupon.TriggerId ?? (object)DBNull.Value),
                new SqlParameter("@Mode", coupon.Mode),
                new SqlParameter("@Days", coupon.Days ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", coupon.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@StartDate", coupon.StartDate ?? (object)DBNull.Value),
                new SqlParameter("@ForFirstOrder", coupon.ForFirstOrder)
                );
        }

        public static void DeleteCoupon(int couponId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Coupon] WHERE CouponID = @CouponID", CommandType.Text, new SqlParameter("@CouponID", couponId));
        }

        public static void DeleteExpiredGeneratedCoupons()
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[Coupon] WHERE Mode = @Mode and ExpirationDate is not null and dateadd(dd,90,ExpirationDate) < getdate() and ActualUses = 0",
                CommandType.Text,
                new SqlParameter("@Mode", (int) CouponMode.Generated));
        }

        #endregion

        #region Product links

        public static void AddProductToCoupon(int couponID, int productID)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [Catalog].[CouponProducts] (couponID,  productID) values (@couponID, @productID)",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponID),
                                            new SqlParameter("@productID", productID));
        }

        public static List<int> GetProductsIDsByCoupon(int couponID)
        {
            List<int> list = SQLDataAccess.ExecuteReadList<int>("Select ProductID from [Catalog].[CouponProducts] where couponID=@couponID",
                                                           CommandType.Text,
                                                           reader => SQLDataHelper.GetInt(reader, "ProductID"),
                                                           new SqlParameter("@CouponID", couponID));
            return list;
        }

        public static void DeleteProductFromCoupon(int couponID, int productID)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponProducts] Where couponID=@couponID and productID=@productID",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponID),
                                            new SqlParameter("@productID", productID));
        }

        public static void DeleteAllProductsFromCoupon(int couponID)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponProducts] Where couponID=@couponID",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponID));
        }

        #endregion

        #region Categories link

        public static void AddCategoryToCoupon(int couponID, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [Catalog].[CouponCategories] (couponID,  categoryId) values (@couponID, @categoryId)",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponID),
                                            new SqlParameter("@categoryId", categoryId));
        }

        public static List<int> GetCategoriesIDsByCoupon(int couponID)
        {
            List<int> list = SQLDataAccess.ExecuteReadList<int>("Select CategoryID from [Catalog].[CouponCategories] where couponID=@couponID",
                                                           CommandType.Text,
                                                           reader => SQLDataHelper.GetInt(reader, "CategoryID"),
                                                           new SqlParameter("@CouponID", couponID));
            return list;
        }

        public static void DeletecategoriesFromCoupon(int couponID, int categoryID)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponCategories] Where couponID=@couponID and categoryID=@categoryID",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponID),
                                            new SqlParameter("@categoryID", categoryID));
        }

        public static void DeleteAllCategoriesFromCoupon(int couponID)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CouponCategories] Where couponID=@couponID",
                                            CommandType.Text,
                                            new SqlParameter("@CouponID", couponID));
        }

        #endregion

        #region CustomerCoupon

        public static bool CanApplyCustomerCoupon(Coupon coupon)
        {
            return CanApplyCustomerCoupon(coupon, CustomerContext.CustomerId);
        }

        public static bool CanApplyCustomerCoupon(Coupon coupon, Guid customerId)
        {
            return (coupon.StartDate == null || coupon.StartDate < DateTime.Now) &&
                (coupon.ExpirationDate == null || coupon.ExpirationDate > DateTime.Now) &&
                (coupon.PossibleUses == 0 || coupon.PossibleUses > coupon.ActualUses) &&
                !(coupon.ForFirstOrder && OrderService.IsCustomerHasConfirmedOrders(customerId)) &&
                coupon.Enabled;
        }

        public static Coupon GetCustomerCoupon()
        {
            return GetCustomerCoupon(CustomerContext.CustomerId);
        }

        public static Coupon GetCustomerCoupon(Guid customerId)
        {
            var coupon = SQLDataAccess.ExecuteReadOne(
                "Select * From Catalog.Coupon Where CouponID = (Select Top(1) CouponID From Customers.CustomerCoupon Where CustomerID = @CustomerID)",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@CustomerID", customerId));

            if (coupon == null)
                return null;

            if (CanApplyCustomerCoupon(coupon, customerId))
                return coupon;

            DeleteCustomerCoupon(coupon.CouponID, customerId);
            return null;
        }

        public static void DeleteCustomerCoupon(int couponId)
        {
            DeleteCustomerCoupon(couponId, CustomerContext.CustomerId);
        }

        public static void DeleteCustomerCoupon(int couponId, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Customers.CustomerCoupon Where CouponID = @CouponID and CustomerID = @CustomerID",
                CommandType.Text, 
                new SqlParameter("@CustomerID", customerId), 
                new SqlParameter("@CouponID", couponId));
        }

        public static void AddCustomerCoupon(int couponId)
        {
            AddCustomerCoupon(couponId, true);
        }

        public static void AddCustomerCoupon(int couponId, bool customerApplied)
        {
            // покупатель может применить только один купон, иначе падало при выборке, также добавлено  Select Top(1) в GetCustomerCoupon
            if (!IsCustomerHaveThisCupon(couponId, CustomerContext.CustomerId) && !IsCustomerApplyAnyCupon(CustomerContext.CustomerId))
                AddCustomerCoupon(couponId, CustomerContext.CustomerId);

            if (customerApplied)
                PartnerService.SetReferralCookie(couponId);
        }

        public static void AddCustomerCoupon(int couponId, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (NOT EXISTS(Select 1 from Customers.CustomerCoupon where CustomerID=@CustomerID and CouponID=@CouponID))" +
                "BEGIN" +
                "   INSERT INTO Customers.CustomerCoupon (CustomerID, CouponID) VALUES (@CustomerID, @CouponID)" +
                "END",
                 CommandType.Text,
                 new SqlParameter("@CustomerID", customerId),
                 new SqlParameter("@CouponID", couponId));
        }

        private static bool IsCustomerApplyAnyCupon(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>
                ("Select Count(*) from Customers.CustomerCoupon where CustomerID=@CustomerID",
                 CommandType.Text,
                 new SqlParameter("@CustomerID", customerId)                 
                ) > 0;
        }

        private static bool IsCustomerHaveThisCupon(int couponId, Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>
                ("Select Count(*) from Customers.CustomerCoupon where CustomerID=@CustomerID and CouponID=@CouponID",
                 CommandType.Text,
                 new SqlParameter("@CustomerID", customerId),
                 new SqlParameter("@CouponID", couponId)
                ) > 0;
        }

        #endregion

        public static string GenerateCouponCode()
        {
            var code = "";
            while (string.IsNullOrEmpty(code) || IsExistCouponCode(code) || GiftCertificateService.IsExistCertificateCode(code))
            {
                code = Strings.GetRandomString(8);
            }
            return code;
        }

        public static bool IsExistCouponCode(string code)
        {
            return
                SQLDataHelper.GetInt(
                    SQLDataAccess.ExecuteScalar("Select COUNT(CouponID) From Catalog.Coupon Where Code = @Code",
                        CommandType.Text,
                        new SqlParameter("@Code", code))) > 0;
        }

        public static bool IsCouponAppliedToProduct(int couponId, int productId)
        {
            return
                SQLDataHelper.GetInt(
                    SQLDataAccess.ExecuteScalar("Catalog.sp_IsCouponAppliedToProduct", 
                        CommandType.StoredProcedure,
                        new SqlParameter("@CouponID", couponId),
                        new SqlParameter("@productId", productId))) > 0;
        }

        public static void SetCouponActivity(int couponId, bool active)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalog.Coupon Set Enabled = @Enabled Where CouponID = @CouponID",
                                         CommandType.Text,
                                          new SqlParameter("@CouponID", couponId),
                                          new SqlParameter("@Enabled", active));
        }

        public static List<Coupon> GetCouponsByTriggerAction(int triggerActionId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[Coupon] WHERE TriggerActionId = @TriggerActionId and Mode=@Mode",
                CommandType.Text, GetFromReader, 
                new SqlParameter("@TriggerActionId", triggerActionId),
                new SqlParameter("@Mode", (int)CouponMode.Template));
        }

        public static List<Coupon> GetAllCouponsByTriggerAction(int triggerActionId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[Coupon] WHERE TriggerActionId = @TriggerActionId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@TriggerActionId", triggerActionId));
        }

        public static Coupon GetCouponByTrigger(int triggerId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Catalog].[Coupon] WHERE TriggerId = @TriggerId and Mode=@Mode",
                CommandType.Text, GetFromReader,
                new SqlParameter("@TriggerId", triggerId),
                new SqlParameter("@Mode", (int)CouponMode.TriggerTemplate));
        }

        public static List<Coupon> GetAllCouponsByTrigger(int triggerId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[Coupon] WHERE TriggerId = @TriggerId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@TriggerId", triggerId));
        }

        public static Coupon GenerateCoupon(Coupon couponTemplate, Guid? customerId = null, int? entityId = null)
        {
            var coupon = couponTemplate.DeepClone();
            coupon.Code = GenerateCouponCode();
            coupon.Mode = CouponMode.Generated;
            coupon.CustomerId = customerId;
            coupon.AddingDate = DateTime.Now;
            coupon.EntityId = entityId;

            if (coupon.Days != null)
                coupon.ExpirationDate = DateTime.Now.AddDays(coupon.Days.Value);

            AddCoupon(coupon);

            foreach (var categoryId in couponTemplate.CategoryIds)
                AddCategoryToCoupon(coupon.CouponID, categoryId);

            foreach (var productId in couponTemplate.ProductsIds)
                AddProductToCoupon(coupon.CouponID, productId);
            
            return coupon;
        }

        public static Coupon GeneratePartnerCoupon(Coupon couponTemplate, string code)
        {
            var coupon = couponTemplate.DeepClone();
            coupon.Code = code;
            coupon.Mode = CouponMode.Partner;
            coupon.AddingDate = DateTime.Now;

            if (coupon.Days != null)
                coupon.ExpirationDate = DateTime.Now.AddDays(coupon.Days.Value);

            AddCoupon(coupon);

            foreach (var categoryId in couponTemplate.CategoryIds)
                AddCategoryToCoupon(coupon.CouponID, categoryId);

            foreach (var productId in couponTemplate.ProductsIds)
                AddProductToCoupon(coupon.CouponID, productId);

            return coupon;
        }

        public static Coupon GetGeneratedTriggerCouponByCustomerId(int triggerId, Guid customerId, int entityId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Catalog].[Coupon] WHERE TriggerId=@TriggerId and CustomerId=@CustomerId and Mode=@Mode and EntityId=@EntityId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@TriggerId", triggerId),
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@EntityId", entityId),
                new SqlParameter("@Mode", (int)CouponMode.Generated));
        }

        #region Partner Coupon

        public static Coupon GetPartnersCouponTemplate()
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Catalog].[Coupon] WHERE Mode=@Mode",
                CommandType.Text, GetFromReader,
                new SqlParameter("@Mode", (int)CouponMode.PartnersTemplate));
        }

        #endregion
    }
}