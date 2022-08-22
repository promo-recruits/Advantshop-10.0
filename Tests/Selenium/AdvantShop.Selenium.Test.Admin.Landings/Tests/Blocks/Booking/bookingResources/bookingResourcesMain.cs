using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Booking.bookingResources
{
    [TestFixture]
    public class bookingResourcesMain : LandingsFunctions
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
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateTimeOfBooking.csv"
            );
            Init();
        }

        private readonly string blockName = "bookingResources";
        private readonly string blockType = "Booking";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "BookingResourcesTitle";
        private readonly string blockSubTitle = "BookingResourcesSubtitle";
        private readonly string blockAddBtn = "BookingResourcesAddBtn";

        [Test]
        [Order(1)]
        public void AddBlock()
        {
            TestName = "AddBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page");
            AddBlockByBtnBig(blockType, blockName);

            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 2,
                "new block on page");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor")).Count == 3, "block settings on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Count == 1,
                "add block btn up");
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "display add block");
            VerifyIsTrue(Driver.PageSource.Contains("bookingResources, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2, "subblock displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockAddBtn + "\"]")).Displayed,
                "btn displayed in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .lp-block-booking-resources__item")).Count == 0,
                "no items in admin");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingResourcesAddBtn\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".adv-modal-inner.blocks-constructor-modal")).Displayed,
                "block`s settings open");
            //VerifyIsTrue(driver.FindElement(By.Id("tabBooking")).GetAttribute("class").Contains("tabs-header-active"), "tabBooking open");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).Displayed,
                "displayed block in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Displayed,
                "displayed title in client");
            VerifyAreEqual("Наша команда",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Displayed,
                "displayed subtitle in client");
            VerifyAreEqual("Advantshop предлагает широкий кадровый резерв специалистов",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "subtitle in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"" + blockAddBtn + "\"]")).Count == 0,
                "btn not displayed in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Displayed,
                "displayed title in mobile");
            VerifyAreEqual("Наша команда",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Displayed,
                "displayed subtitle in mobile");
            VerifyAreEqual("Advantshop предлагает широкий кадровый резерв специалистов",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "subtitle in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"" + blockAddBtn + "\"]")).Count == 0,
                "btn not displayed in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        [Order(1)]
        public void MoveBlock()
        {
            TestName = "MoveBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            AddBlockByBtnBig("Headers", "headerCommunicationSimple");
            AddBlockByBtnBig("Headers", "headerCommunicationSimple");

            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "initial position of block 1");
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "initial position of block 2");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "initial position of block 3");

            MoveDownBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Down block 1st");

            MoveDownBlockByBtn(numberBlock);
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "Move Down block 2nd /1");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Down block 2nd /2");
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "Move Down block 2nd /3");

            MoveUpBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Up block 1st");

            MoveUpBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "Move Up block 2nd /1");
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Up block 2nd /2");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "Move Up block 2nd /3");

            VerifyFinally(TestName);
        }

        [Test]
        [Order(10)]
        public void DeleteBlock()
        {
            TestName = "DeleteBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));

            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block displayed");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "block is only one");

            DelBlockBtnCancel(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "before delete block");

            DelBlockBtn(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "after delete block");

            DelBlocksAll();
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page anymore");

            VerifyFinally(TestName);
        }
    }
}