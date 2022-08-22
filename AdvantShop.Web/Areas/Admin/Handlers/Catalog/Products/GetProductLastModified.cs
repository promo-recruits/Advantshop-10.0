using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetProductLastModified
    {
        private readonly int _productId;

        public GetProductLastModified(int productId)
        {
            _productId = productId;
        }

        public ProductLastModifiedModel Execute()
        {
            var lastChanges = ChangeHistoryService.GetLast(_productId, ChangeHistoryObjType.Product);
            if (lastChanges == null)
                return null;

            var model = new ProductLastModifiedModel()
            {
                ModifiedDate = lastChanges.ModificationTime.ToString("dd.MM.yy HH:mm")
            };
            
            if (lastChanges.ChangedById != null)
            {
                var modifiedByCustomer = CustomerService.GetCustomer(lastChanges.ChangedById.Value);
                if (modifiedByCustomer != null)
                    model.ModifiedBy = modifiedByCustomer.GetShortName();
            }
            else
            {
                model.ModifiedBy = lastChanges.ChangedByName;
            }

            return model;
        }
    }

    public class ProductLastModifiedModel
    {
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}