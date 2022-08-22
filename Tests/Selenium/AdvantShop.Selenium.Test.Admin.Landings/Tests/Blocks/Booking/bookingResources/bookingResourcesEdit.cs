using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Booking.bookingResources
{
    [TestFixture]
    public class bookingResourcesEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing |
                                        ClearType.Booking);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.Customer.csv", 
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.Service.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.Affiliate.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.ReservationResource.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.ReservationResourceService.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.ReservationResourceTimeOfBooking.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.Category.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateCategory.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateReservationResource.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateService.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateTimeOfBooking.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "bookingResources";
        private string blockType = "Booking";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "BookingResourcesTitle";
        private readonly string blockSubTitle = "BookingResourcesSubtitle";
        private readonly string blockIcon = "BookingResourcesPicture";
        private readonly string blockColTitle = "BookingResourcesColTitle";
        private readonly string blockColText = "BookingResourcesColText";

        [Test]
        public void InplaceHeader()
        {
            TestName = "InplaceHeader";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2, "all subblocks displayed");

            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");

            VerifyAreEqual("Наша команда", Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text,
                "block title initial");
            VerifyAreEqual("Advantshop предлагает широкий кадровый резерв специалистов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "block subtitle initial");

            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized"))
                .SendKeys("New Subtitle");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("blocks-constructor-container")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle");

            Refresh();

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title after refresh");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title in client");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle in client");

            VerifyFinally(TestName);
        }

        [Test]
        public void InplaceItems()
        {
            TestName = "InplaceItems";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Count == 3,
                "all ColTitles displayed");
            VerifyAreEqual("Resource 1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 initial");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Count == 3,
                "all ColTexts displayed");
            VerifyAreEqual("Description 1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text, "ColText1 initial");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 3,
                "all Icons displayed");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            Driver.FindElements(By.CssSelector(".lp-table__body [data-e2e=\"ItemDel\"]"))[2].Click();
            Driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.CssSelector(".lp-table__body [data-e2e=\"ItemDel\"]"))[1].Click();
            Driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            Thread.Sleep(500);
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Count == 1,
                "1 ColTitle displayed");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Count == 1,
                "1 ColText displayed");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 1,
                "1 Icon displayed");

            VerifyAreEqual("Resource 1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Text,
                "ColTitle text initial");
            VerifyAreEqual("Description 1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Text, "ColText text initial");

            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                .SendKeys("New ColHeader 1");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                .SendKeys("New ColText 1");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Click();
            Thread.Sleep(500);

            VerifyAreEqual("New ColHeader 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                    .Text, "block ColHeader 1");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1");

            Refresh();

            VerifyAreEqual("New ColHeader 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                    .Text, "block ColHeader 1 after refresh");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1 after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New ColHeader 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                    .Text, "block ColHeader 1 in client");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1 in client");

            VerifyFinally(TestName);
        }
    }
}