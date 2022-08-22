using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Core.Services.Catalog
{
    public class ProductHistoryService
    {
        #region Product

        public static void NewProduct(Product product, ChangedBy changedBy)
        {
            if(!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = product.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.ProductCreated")
            });
        }

        public static void DeleteProduct(int productId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.ProductDeleted")
            });
        }

        public static void TrackProductChanges(Product product, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var oldProduct = ProductService.GetProduct(product.ProductId);
            if (oldProduct == null)
                return;

            var history = ChangeHistoryService.GetChanges(product.ProductId, ChangeHistoryObjType.Product, oldProduct, product, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void TrackProductMainCategoryChanges(int productId, int categoryId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.ProductMainCategoryChanged") + " " + categoryId
            });
        }

        public static void ProductChanged(Product product, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = product.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.ProductChanged")
            });
        }

        public static void ProductAmountChangedByOrderStatus(int productId, string artno, bool isIncrement,
                                                             string orderNumber, string newStatus, string changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy != null ? new ChangedBy(changedBy) : null)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResourceFormat(
                    isIncrement
                        ? "Core.ProductHistory.IncrementProductAmountChangedByOrderStatus"
                        : "Core.ProductHistory.DecrementProductAmountChangedByOrderStatus",
                    artno, orderNumber, newStatus)
            });
        }

        public static void NewCategory(int productId, int categoryId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var category = CategoryService.GetCategory(categoryId);

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName =
                    LocalizationService.GetResource("Core.ProductHistory.CategoryAdded") +
                    (category != null ? category.Name : "")
            });
        }

        public static void DeleteCategory(int productId, int categoryId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var category = CategoryService.GetCategory(categoryId);

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName =
                    LocalizationService.GetResource("Core.ProductHistory.CategoryDeleted") +
                    (category != null ? category.Name : "")
            });
        }

        #endregion

        #region Offer

        public static void NewOffer(Offer offer, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = offer.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.OfferCreated") + " " + offer.ArtNo
            });
        }

        public static void DeleteOffer(int offerId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = offer.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.OfferDeleted")
            });
        }

        public static void TrackOfferChanges(Offer offer, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var oldOffer = OfferService.GetOffer(offer.OfferId);
            if (oldOffer == null)
                return;

            var history = ChangeHistoryService.GetChanges(offer.ProductId, ChangeHistoryObjType.Product, oldOffer, offer, changedBy, entityName:offer.ArtNo);
            
            ChangeHistoryService.Add(history);
        }

        #endregion
    }
}
