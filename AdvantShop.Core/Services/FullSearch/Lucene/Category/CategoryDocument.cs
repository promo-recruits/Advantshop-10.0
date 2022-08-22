using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Saas;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.FullSearch
{
    public class CategoryDocument : BaseDocument
    {
        private string _name;
        [SearchField]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                AddParameterToDocumentNoStoreAnalyzed(_name);
            }
        }

        private IEnumerable<string> _tags;
        [SearchField]
        public IEnumerable<string> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                foreach (var item in _tags)
                {
                    AddParameterToDocumentNoStoreAnalyzed(item);
                }
            }
        }

        private bool _enabled;
        [SearchField]
        public bool Enabled
        {

            get { return _enabled; }
            set
            {
                _enabled = value;
                AddParameterToDocumentNoStoreAnalyzed(_enabled);
            }
        }

        private bool _hidden;
        [SearchField]
        public bool Hidden
        {

            get { return _hidden; }
            set
            {
                _hidden = value;
                AddParameterToDocumentNoStoreAnalyzed(_hidden);
            }
        }

        //[SearchField("OfferArtNo", "Artno", "RegSuffix")]
        public static explicit operator CategoryDocument(Category model)
        {
            return new CategoryDocument()
            {
                Id = model.CategoryId,
                Name = StringHelper.ReplaceCirilikSymbol(model.Name),
                Tags = (model.Tags != null && (!SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTags))
                    ? model.Tags
                    : new List<Tag>()).Select(l => StringHelper.ReplaceCirilikSymbol(l.Name)),
                Enabled = model.Enabled,
                Hidden = model.Hidden
            };
        }
    }
}
