using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Cms.Files
{
    public class FilesFilterModel : BaseFilterModel<string>
    {
        public string FileName { get; set; }
    }

}
