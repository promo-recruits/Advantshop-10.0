
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog.Properties;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetAllProperties
    {
        private readonly PropertiesFilterModel _filterModel;
        private SqlPaging _paging;

        public GetAllProperties(PropertiesFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<PropertyShortModel> Execute()
        {
            var model = new FilterResult<PropertyShortModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<PropertyShortModel>();

            return model;
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "PropertyId",
                "Name"
            );
            _paging.From("[Catalog].[Property]");
            _paging.OrderBy("Name");

            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
            {
                _paging.Where("Name like '%'+{0}+'%'", _filterModel.Search);
            }
        }
    }
}
