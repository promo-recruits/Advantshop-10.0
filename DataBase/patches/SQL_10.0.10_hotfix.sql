ALTER FUNCTION [Settings].[ProductColorsToString]  
(  
 @ProductId int,
 @OnlyAvailable bit
)  
RETURNS nvarchar(Max)  
AS  
BEGIN  
	DECLARE @result nvarchar(max)  
    ;with cte as (  
		Select Color.ColorID, ColorName, ColorCode, SortOrder, isnull(PhotoName, '') as PhotoName, Sum(1*Offer.Main) as Main  
		From Catalog.Offer 
		Left join catalog.color on Color.ColorID=Offer.ColorID   
		Inner Join catalog.product on Product.ProductID = Offer.ProductID  
		Left join catalog.photo on color.Colorid = photo.ObjId and photo.type = 'color'  
		WHERE Product.ProductID=@ProductId and (Product.AllowPreorder=1 or @OnlyAvailable = 0 or Offer.Amount >0) 
		Group by Color.ColorID, ColorName, ColorCode, SortOrder,PhotoName  
    )  
    SELECT  @result= coalesce(@result + ', ', '[') + '{ColorId:' + convert(nvarchar(max), ColorID) + ', ColorName:''' + Replace(ColorName, '''', '&rsquo;') + ''', ColorCode:''' + ColorCode +''', PhotoName:''' + PhotoName + ''', Main:' + convert(nvarchar(max), Main) + '}'  
	FROM cte order by SortOrder, ColorName
	set @result = @result + ']'  
	return @result  
END 

GO--

ALTER PROCEDURE [Catalog].[PreCalcProductParams] 
	@productId INT,  
    @ModerateReviews BIT,  
    @OnlyAvailable BIT,
	@ComplexFilter BIT
AS  
 BEGIN  
	 SET NOCOUNT ON;  
	 DECLARE @CountPhoto INT;  
	 DECLARE @Type NVARCHAR(10);  
	 DECLARE @PhotoId INT;  
	 DECLARE @MaxAvailable FLOAT;  
	 DECLARE @VideosAvailable BIT;  
	 DECLARE @Colors NVARCHAR(MAX);  
	 DECLARE @NotSamePrices BIT;  
	 DECLARE @MinPrice FLOAT;  
	 DECLARE @PriceTemp FLOAT;  
	 DECLARE @AmountSort BIT;  
	 DECLARE @OfferId INT;  
	 DECLARE @Comments INT;  
	 DECLARE @CategoryId INT;  
	 DECLARE @Gifts BIT;  
	 IF NOT EXISTS  
	 (  
		 SELECT ProductID  
		 FROM [Catalog].Product  
		 WHERE ProductID = @productId  
	 )  
		 RETURN;  
	 SET @Type = 'Product';        
	 --@CountPhoto        
	 SET @CountPhoto =  
	 (  
		 SELECT TOP (1) CASE  
							WHEN  
		 (  
			 SELECT Offer.ColorID  
			 FROM [Catalog].[Offer]  
			 WHERE [ProductID] = @productId  
				   AND main = 1  
		 ) IS NOT NULL AND @ComplexFilter = 1
							THEN  
		 (  
			 SELECT COUNT(DISTINCT PhotoId)  
			 FROM [Catalog].[Photo]  
				  INNER JOIN [Catalog].[Offer] ON [Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL  
			 WHERE [Photo].[ObjId] = Offer.[ProductId]  
				   AND [Offer].Main = 1  
				   AND TYPE = @Type  
				   AND Offer.[ProductId] = @productId  
		 )  
							ELSE  
		 (  
			 SELECT COUNT(PhotoId)  
			 FROM [Catalog].[Photo]  
			 WHERE [Photo].[ObjId] = @productId  
				   AND TYPE = @Type  
		 )  
						END  
	 );        
	 --@PhotoId        
	 SET @PhotoId =  
	 (  
		 SELECT CASE  
					WHEN    



(  
			 SELECT Offer.ColorID  
			 FROM [Catalog].[Offer]  
			 WHERE [ProductID] = @productId  
				   AND main = 1  
		 ) IS NOT NULL  
					THEN  
		 (  
			 SELECT TOP (1) PhotoId  
			 FROM [Catalog].[Photo]  
 INNER JOIN [Catalog].[Offer] ON [Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL  
			 WHERE([Photo].ColorID = Offer.ColorID  
				   OR [Photo].ColorID IS NULL)  
				  AND [Photo].[ObjId] = @productId  
				  AND Type = @Type  
			 ORDER BY [Photo]. main DESC,  
					  [Photo].[PhotoSortOrder],  
					  [PhotoId]  
		 )  
					ELSE  
		 (  
			 SELECT TOP (1) PhotoId  
			 FROM [Catalog].[Photo]  
			 WHERE [Photo].[ObjId] = @productId  
				   AND Type = @Type  
			 ORDER BY main DESC,  
					  [Photo].[PhotoSortOrder],  
					  [PhotoId]  
		 )  
		 END  



	 );         
	 --VideosAvailable        
	 IF  
	 (  
		 SELECT COUNT(ProductVideoID)  
		 FROM [Catalog].[ProductVideo]  
		 WHERE ProductID = @productId  
	 ) > 0  
		 BEGIN  
			 SET @VideosAvailable = 1;  
		 END;  
	 ELSE  
		 BEGIN  
			 SET @VideosAvailable = 0;  
		 END;        
	
	 --@MaxAvailable        
	 SET @MaxAvailable =  
	 (  
		 SELECT MAX(Offer.Amount)  
		 FROM [Catalog].Offer  
		 WHERE ProductId = @productId  
	 );     

	 --AmountSort        
	 SET @AmountSort =  
	 (  
		 SELECT CASE  
					WHEN @MaxAvailable <= 0  
						 OR @MaxAvailable < ISNULL(Product.MinAmount, 0)  
					THEN 0  
					ELSE 1  
				END  
		 FROM [Catalog].Offer  
			  INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId  
		 WHERE Offer.ProductId = @productId  
			   AND main = 1  
	 );        
	 --Colors        
	 SET @Colors =  
	 (  
		 SELECT [Settings].[ProductColorsToString](@productId, @OnlyAvailable)  
	 );        
	
	 --@NotSamePrices        
	 IF  
	 (  
		 SELECT MAX(price) - MIN(price)  
		 FROM [Catalog].offer  
		 WHERE offer.productid = @productId  
			   AND price > 0 AND (@OnlyAvailable = 0  

					OR amount > 0)  
	 ) > 0  
		 BEGIN  
			 SET @NotSamePrices = 1;  
		 END;  
	 ELSE  
		 BEGIN  
			 SET @NotSamePrices = 0;  
		 END;        

	 --@MinPrice        
	 SET @MinPrice =  
	 (  
		 SELECT isNull(MIN(price), 0)  
		 FROM [Catalog].offer  
		 WHERE offer.productid = @productId  
			   AND price > 0 AND (@OnlyAvailable = 0  

					OR amount > 0)  
	 );        

	 --@OfferId      
	 SET @OfferId =  
	 (  
		 SELECT OfferID  
		 FROM [Catalog].offer  
		 WHERE offer.productid = @productId  
			   AND (offer.Main = 1  
					OR offer.Main IS NULL)  
	 );       


	 --@PriceTemp        
	 SET @PriceTemp =
         (
             SELECT CASE WHEN [Product].Discount > 0 THEN (@MinPrice - @MinPrice * [Product].Discount / 100) * CurrencyValue ELSE (@MinPrice - isnull([Product].DiscountAmount, 0)) * CurrencyValue END
             FROM Catalog.Product
                  INNER JOIN Catalog.Currency ON Product.Currencyid = Currency.Currencyid
             WHERE Product.Productid = @productId
         );         

	 --@Comments      
	 SET @Comments =  
	 (  
		 SELECT COUNT(ReviewId)  
		 FROM CMS.Review  
		 WHERE EntityId = @productId  
			   AND (Checked = 1  
					OR @ModerateReviews = 0)  
	 );       
	 
	 --@Gifts      
	 SET @Gifts =  
	 (  
		 SELECT TOP (1) CASE  
							WHEN COUNT([ProductGifts].ProductID) > 0  
							THEN 1  
							ELSE 0  
						END  
		 FROM [Catalog].[ProductGifts]
			INNER JOIN Catalog.Offer on ProductGifts.GiftOfferId = Offer.OfferId
			INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId
		 WHERE [ProductGifts].ProductID = @productId  and Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1
	 );      
   
	 --@CategoryId      
	 DECLARE @MainCategoryId INT;  
	 SET @MainCategoryId =  
	 (  
		 SELECT TOP 1 CategoryID  
		 FROM [Catalog].ProductCategories  
		 WHERE ProductID = @productId  
		 ORDER BY Main DESC  
	 );  
	 IF @MainCategoryId IS NOT NULL  
		 BEGIN  
			 SET @CategoryId =  
			 (  
				 SELECT TOP 1 id  
				 FROM [Settings].[GetParentsCategoryByChild](@MainCategoryId)  
				 ORDER BY sort DESC  
			 );  
		 END;  
	 IF  
	 (  
		 SELECT COUNT(productid)  
		 FROM [Catalog].ProductExt  
		 WHERE productid = @productId  
	 ) > 0  
		 BEGIN  
			 UPDATE [Catalog].[ProductExt]  
			   SET  
				   [CountPhoto] = @CountPhoto,  
				   [PhotoId] = @PhotoId,  
				   [VideosAvailable] = @VideosAvailable,  
				   [MaxAvailable] = @MaxAvailable,  
				   [NotSamePrices] = @NotSamePrices,  
				   [MinPrice] = @MinPrice,  
				   [Colors] = @Colors,  
				   [AmountSort] = @AmountSort,  
				   [OfferId] = @OfferId,  
				   [Comments] = @Comments,  
				   [CategoryId] = @CategoryId,  
				   [PriceTemp] = @PriceTemp,  
				   [Gifts] = @Gifts  
			 WHERE [ProductId] = @productId;  
		 END;  
	 ELSE  
		 BEGIN  
			 INSERT INTO [Catalog].[ProductExt]  
			 ([ProductId],  
			  [CountPhoto],  
			  [PhotoId],  
			  [VideosAvailable],  
			  [MaxAvailable],  
			  [NotSamePrices],  
			  [MinPrice],  
			  [Colors],  
			  [AmountSort],  
			  [OfferId],  
			  [Comments],  
			  [CategoryId],  
			  [PriceTemp],  
			  [Gifts]  
			 )  
			 VALUES  
			 (@productId,  
			  @CountPhoto,  
			  @PhotoId,  
			  @VideosAvailable,  
			  @MaxAvailable,  
			  @NotSamePrices,  
			  @MinPrice,  
			  @Colors,  
			  @AmountSort,  
			  @OfferId,  
			  @Comments,  
			  @CategoryId,  
			  @PriceTemp,  
			  @Gifts  
			 );  
		 END;  
 END;
 
GO--

ALTER PROCEDURE [Catalog].[PreCalcProductParamsMass] @ModerateReviews BIT, @OnlyAvailable bit,  @ComplexFilter BIT AS 
BEGIN
   
INSERT INTO
   [Catalog].[ProductExt] (ProductId, CountPhoto, PhotoId, VideosAvailable, MaxAvailable, NotSamePrices, MinPrice, Colors, AmountSort, OfferId, Comments, CategoryId) (
   SELECT
      ProductId, 0, NULL, 0, 0, 0, 0, NULL, 0, NULL, 0, NULL 
   FROM
      [Catalog].Product 
   WHERE
      Product.ProductId NOT IN 
      (
         SELECT
            ProductId 
         FROM
            [Catalog].[ProductExt]
      )
) 
      UPDATE
         catalog.ProductExt 
      SET
         ProductExt.[CountPhoto] = tempTable.CountPhoto,
         ProductExt.[PhotoId] = tempTable.[PhotoId],
         ProductExt.[VideosAvailable] = tempTable.[VideosAvailable],
         ProductExt.[MaxAvailable] = tempTable.[MaxAvailable],
         ProductExt.[NotSamePrices] = tempTable.[NotSamePrices],
         ProductExt.[MinPrice] = tempTable.[MinPrice],
         ProductExt.[OfferId] = tempTable.[OfferId],
         ProductExt.[Comments] = tempTable.[Comments],
         ProductExt.[Gifts] = tempTable.[Gifts],
		 ProductExt.[Colors] = tempTable.[Colors],
         ProductExt.[CategoryId] = tempTable.[CategoryId] ,
		 ProductExt.PriceTemp = tempTable.PriceTemp,
		 ProductExt.AmountSort=tempTable.AmountSort
      FROM
         catalog.ProductExt 
         INNER JOIN
            (               
           select 
		pe.ProductId,
		CountPhoto=case when offerId.ColorID is null OR @ComplexFilter = 0 then countNocolor.countNocolor else countColor.countColor end,				
		PhotoId=case when offerId.ColorID is null then PhotoIdNoColor.PhotoIdNoColor else PhotoIdColor.PhotoIdColor end,
		[VideosAvailable]=isnull(videosAvailable.videosAvailable,0),
		[MaxAvailable]=maxAvailable.maxAvailable,
		[NotSamePrices]=isnull(notSamePrices.notSamePrices,0),
		[MinPrice]=isnull(minPrice.minPrice,0),
		[OfferId]=offerId.OfferId,
		[Comments]=isnull(comments.comments,0),
		[Gifts]=isnull(gifts.gifts,0),
		[Colors]=(SELECT [Settings].[ProductColorsToString](pe.ProductId, @OnlyAvailable)),
		[CategoryId] = (SELECT TOP 1 id	FROM [Settings].[GetParentsCategoryByChild](( SELECT TOP 1 CategoryID FROM [Catalog].ProductCategories	WHERE ProductID = pe.ProductId ORDER BY Main DESC))ORDER BY sort DESC),
		PriceTemp = CASE WHEN p.Discount > 0 THEN (isnull(minPrice.minPrice,0) - isnull(minPrice.minPrice,0) * p.Discount/100)*c.CurrencyValue ELSE (isnull(minPrice.minPrice,0) - isnull(p.DiscountAmount,0))*c.CurrencyValue END,
		AmountSort=CASE when ISNULL(maxAvailable.maxAvailable, 0) <= 0 OR maxAvailable.maxAvailable < IsNull(p.MinAmount, 0) THEN 0 ELSE 1 end

from Catalog.[ProductExt] pe
left join (SELECT o.ProductId, COUNT(*) countColor FROM [Catalog].[Photo] ph INNER JOIN [Catalog].[Offer] o  ON ph.[ObjId] = o.ProductId 
            WHERE ( ph.ColorID = o.ColorID OR ph.ColorID IS NULL ) AND TYPE = 'Product' AND o.Main = 1 
			group by o.ProductId
) countColor on pe.ProductId=countColor.ProductId

left join (SELECT [ObjId], COUNT(*) countNocolor FROM [Catalog].[Photo]  
            WHERE TYPE = 'Product'
			group by [ObjId]
) countNocolor on pe.ProductId=countNocolor.[ObjId]

left join (
select ProductId, PhotoId PhotoIdColor from (
SELECT o.ProductId, ph.PhotoId, Row_Number() over (PARTITION  by o.ProductId ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn FROM [Catalog].[Photo] ph
							INNER JOIN [Catalog].[Offer] o ON ph.[ObjId] = o.ProductId
							WHERE (ph.ColorID = o.ColorID OR ph.ColorID IS NULL) AND TYPE = 'Product' ) ct where rn=1
) PhotoIdColor on pe.ProductId=PhotoIdColor.ProductId

left join (
select ProductId, PhotoId PhotoIdNoColor from (
SELECT ph.[ObjId] ProductId, ph.PhotoId, Row_Number() over (PARTITION  by ph.[ObjId] ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn FROM [Catalog].[Photo] ph	WHERE TYPE = 'Product' ) ct where rn=1
) PhotoIdNoColor on pe.ProductId=PhotoIdNoColor.ProductId

left join (select pv.ProductID, CASE WHEN COUNT(pv.ProductVideoID) > 0 THEN 1	ELSE 0 END videosAvailable FROM [Catalog].[ProductVideo] pv group by pv.ProductID) videosAvailable on pe.ProductId=videosAvailable.ProductId
left join (select o.ProductID,Max(o.Amount) maxAvailable  FROM [Catalog].Offer o group by o.ProductID) maxAvailable on pe.ProductId=maxAvailable.ProductId
left join (select o.ProductID, CASE WHEN MAX(o.price) - MIN(o.price) > 0 THEN 1 ELSE 0 END notSamePrices  FROM [Catalog].Offer o where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0) group by o.ProductID) notSamePrices on pe.ProductId=notSamePrices.ProductId
left join (select o.ProductID,MIN(o.price) minPrice FROM [Catalog].Offer o where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0)  group by o.ProductID) minPrice on pe.ProductId=minPrice.ProductId
left join ( 
select ProductId, OfferID, colorId from (
select o.ProductID,o.OfferID, o.colorId, Row_Number() over (PARTITION  by o.OfferID ORDER BY o.OfferID) rn  FROM [Catalog].Offer o where o.Main = 1 )ct where rn=1
) offerId on pe.ProductId=offerId.ProductId
left join (select EntityId ProductID,count(ReviewId) comments  FROM CMS.Review  where (Checked = 1 OR @ModerateReviews = 0) group by EntityId) comments on pe.ProductId=comments.ProductId
left join (select pg.ProductID, CASE WHEN COUNT(pg.ProductID) > 0 THEN 1 ELSE 0 END gifts FROM [Catalog].[ProductGifts] pg INNER JOIN Catalog.Offer on pg.GiftOfferId = Offer.OfferId INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId WHERE Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1 group by pg.ProductID) gifts on pe.ProductId=gifts.ProductId
inner join catalog.Product p on p.ProductID = pe.ProductID
INNER JOIN CATALOG.Currency c ON p.currencyid = c.currencyid
 )
AS tempTable 
ON tempTable.ProductId = ProductExt.ProductId
    
END

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE name = N'CurrentVkProduct' AND object_id = OBJECT_ID(N'[Vk].[VkProduct]'))
BEGIN
	ALTER TABLE Vk.VkProduct ADD
		CurrentVkProduct nvarchar(MAX) NULL,
		ModifiedDate date NULL
END

GO--

IF NOT EXISTS(SELECT 1
              FROM [Settings].[Localization]
              WHERE [ResourceKey] = 'Core.Orders.Order.ShippingTaxName')
BEGIN
    INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
    VALUES
        (1,'Core.Orders.Order.ShippingTaxName', 'Налог доставки'),
        (2,'Core.Orders.Order.ShippingTaxName', 'Shipping tax name'),
        (1,'Core.Orders.Order.ShippingTaxRate', 'Ставка налога доставки'),
        (2,'Core.Orders.Order.ShippingTaxRate', 'Shipping tax rate'),
        (1,'Core.Orders.Order.ShippingTaxSum', 'Сумма налога доставки'),
        (2,'Core.Orders.Order.ShippingTaxSum', 'Shipping tax sum'),
        (1,'Core.Orders.Order.ShippingTaxSumFormatted', 'Сумма налога доставки'),
        (2,'Core.Orders.Order.ShippingTaxSumFormatted', 'Shipping tax sum')
END

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Передавать налог в зависимости от способа расчета' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Settings.Checkout.TaxTypeByPaymentMethodType'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Для соблюдения статьи 164 п.4 НК РФ' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.TaxTypeByPaymentMethodTypeHint'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Увеличение количества товара "{0}" после смены статуса заказа {1} на "{2}"' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Core.ProductHistory.IncrementProductAmountChangedByOrderStatus'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Уменьшение количества товара "{0}" после смены статуса заказа {1} на "{2}"' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Core.ProductHistory.DecrementProductAmountChangedByOrderStatus'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Телефон отправителя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'GiftCertificate.Index.Phone'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Покупатели' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.Customers'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Получить покупателя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.GetCustomer'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Создать покупателя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.AddCustomer'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Изменить покупателя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.UpdateCustomer'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Послать смс-код по номеру телефона' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.UpdateCustomer'

GO--

UPDATE [Settings].[Settings] SET [Value] = 'False' WHERE [Name] = 'ShowImageSearchEnabled'
UPDATE [Settings].[Settings] SET [Value] = 'False' WHERE [Name] = 'ImageSearchEnabled'

GO--

update [Settings].[Settings]
set Value = REPLACE(Value, 'f003.backblazeb2.com', 'scr.advstatic.ru')
where value like '%f003.backblazeb2.com%'

update [CMS].[LandingSite]
set [ScreenShot] = REPLACE([ScreenShot], 'f003.backblazeb2.com', 'scr.advstatic.ru')
where [ScreenShot] like '%f003.backblazeb2.com%'
  
GO--

if (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TelegramMessage' AND COLUMN_NAME = 'FromId') = 'int'
begin
    ALTER TABLE Customers.TelegramMessage
    ALTER COLUMN FromId bigint NOT NULL
end

GO--

if (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TelegramMessage' AND COLUMN_NAME = 'ToId') = 'int'
begin
    ALTER TABLE Customers.TelegramMessage
    ALTER COLUMN ToId bigint NULL
end

GO--

IF NOT EXISTS(SELECT 1
              FROM [Settings].[Localization]
              WHERE [ResourceKey] = 'Admin.Files.Index.HtmlFileMoreThanAllowed')
BEGIN
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Files.Index.HtmlFileMoreThanAllowed', N'Файл превышает максимальный допустимый размер в {0}'),
    (2,'Admin.Files.Index.HtmlFileMoreThanAllowed', 'The file exceeds the maximum allowed size in {0}')
END

GO--
