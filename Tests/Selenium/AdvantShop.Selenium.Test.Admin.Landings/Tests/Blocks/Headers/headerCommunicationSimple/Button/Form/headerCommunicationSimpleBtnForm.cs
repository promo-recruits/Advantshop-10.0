using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers.HeaderCommunicationSimple.Button.Form
{
    [TestFixture]
    public class headerCommunicationSimpleBtnForm : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\ManyBlocks\\Customers.CustomerField.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\ManyBlocks\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\ManyBlocks\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSubBlock.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingForm.csv"
            );

            Init();
        }

        private IWebElement agree;
        private string colorRGB;
        private string FormBtn = "HeadersBtn";

        [Ignore("Useless")]
        [Test]
        public void TestMethod1()
        {
            VerifyIsTrue(false);
        }
    }
}