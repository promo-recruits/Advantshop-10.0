using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Video.VideoBlock
{
    [TestFixture]
    public class VideoSettingsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Video\\Video\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.LandingSubBlock.csv"
            );

            Init();
        }
    }
}