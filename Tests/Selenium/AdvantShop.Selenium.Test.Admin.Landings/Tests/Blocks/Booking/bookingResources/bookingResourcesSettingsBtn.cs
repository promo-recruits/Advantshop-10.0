using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Booking.bookingResources
{
    [TestFixture]
    public class bookingResourcesSettingsBtn : LandingsFunctions
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
        private readonly string blockColBtn = "BookingResourcesBtn";

        [Test]
        public void BookingBtn()
        {
            TestName = "BookingBtn";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 3,
                "BookingBtn initial count");
            VerifyAreEqual("Записаться", Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Text,
                "BookingBtn text initial");

            //off
            BlockSettingsBtn(numberBlock);
            TabSelect("tabBookingButton");
            BookingBtnOff();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 0,
                "no BookingBtn in admin");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 0,
                "no BookingBtn after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 0,
                "no BookingBtn in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 0,
                "no BookingBtn in mobile");

            //on
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBookingButton");
            BookingBtnOn();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 3,
                "BookingBtn count in admin");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 3,
                "BookingBtn count after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 3,
                "BookingBtn count in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 3,
                "BookingBtn count in mobile");

            //change
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBookingButton");
            ChangeBookingBtnText("BookingBtnText");
            BlockSettingsSave();
            VerifyAreEqual("BookingBtnText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Text, "BookingBtn text");

            Refresh();
            VerifyAreEqual("BookingBtnText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Text,
                "BookingBtn text after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("BookingBtnText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Text,
                "BookingBtn text in client");

            GoToMobile("lp/test1");
            VerifyAreEqual("BookingBtnText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Text,
                "BookingBtn text in mobile");

            VerifyFinally(TestName);
        }
    }
}