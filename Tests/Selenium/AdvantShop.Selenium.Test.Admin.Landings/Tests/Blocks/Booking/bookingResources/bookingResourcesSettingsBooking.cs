using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Booking.bookingResources
{
    [TestFixture]
    public class bookingResourcesSettingsBooking : LandingsFunctions
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
        private readonly string blockIcon = "BookingResourcesPicture";
        private readonly string blockColTitle = "BookingResourcesColTitle";
        private readonly string blockColText = "BookingResourcesColText";
        private readonly string blockColLink = "BookingResourcesColLink";
        private readonly string blockColBtn = "BookingResourcesBtn";

        [Test]
        public void BookingAltGrid()
        {
            TestName = "BookingAltGrid";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 3,
                "block Items initial");

            //Delete
            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
            Driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            VerifyAreEqual("Нет элементов", Driver.FindElement(By.CssSelector(".lp-table__cell")).Text,
                "no elements in grid");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 0,
                "no Items in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 0,
                "no Items in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 0,
                "no Items in mobile");

            //Add
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            BookingDaysOn();
            AddNewReservation("Resource 3", "18:00", "13:00");
            AddNewReservation("Resource 4", "17:00", "11:00");
            Thread.Sleep(1000);
            BlockSettingsSave();

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            VerifyAreEqual("18:00",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]"))[0].GetAttribute("value"),
                "TimeEnd1 after add");
            VerifyAreEqual("13:00",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]"))[0].GetAttribute("value"),
                "TimeFrom1 after add");
            VerifyAreEqual("17:00",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]"))[1].GetAttribute("value"),
                "TimeEnd2 after add");
            VerifyAreEqual("11:00",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]"))[1].GetAttribute("value"),
                "TimeFrom2 after add");
            BlockSettingsCancel();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "Items in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "Images in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "Links in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "Btns in admin");
            VerifyAreEqual("Resource 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in admin");
            VerifyAreEqual("Resource 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in admin");
            VerifyAreEqual("Description 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in admin");
            VerifyAreEqual("Description 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "Items in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "Images in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "Links in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "Btns in client");
            VerifyAreEqual("Resource 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in client");
            VerifyAreEqual("Resource 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in client");
            VerifyAreEqual("Description 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in client");
            VerifyAreEqual("Description 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "Items in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "Images in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "Links in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "Btns in mobile");
            VerifyAreEqual("Resource 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in mobile");
            VerifyAreEqual("Resource 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in mobile");
            VerifyAreEqual("Description 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in mobile");
            VerifyAreEqual("Description 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in mobile");

            //Change
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            ChangeReservation(0, "ResourceName1", "ResourceDescription1", "20:00", "09:00");
            ChangeReservation(1, "ResourceName2", "ResourceDescription2", "15:00", "10:00");
            Thread.Sleep(1000);
            BlockSettingsSave();

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            VerifyAreEqual("20:00",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]"))[0].GetAttribute("value"),
                "TimeEnd1 after change");
            VerifyAreEqual("09:00",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]"))[0].GetAttribute("value"),
                "TimeFrom1 after change");
            VerifyAreEqual("15:00",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]"))[1].GetAttribute("value"),
                "TimeEnd2 after change");
            VerifyAreEqual("10:00",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]"))[1].GetAttribute("value"),
                "TimeFrom1 after change");
            BlockSettingsCancel();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "new Items in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "new Images in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "new Links in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "new Btns in admin");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "new ColTitle1 in admin");
            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "new ColTitle2 in admin");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "new ColText1 in admin");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "new ColText2 in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "new Items in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "new Images in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "new Links in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "new Btns in client");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "new ColTitle1 in client");
            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "new ColTitle2 in client");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "new ColText1 in client");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "new ColText2 in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "new Items in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "new Images in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "new Links in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "new Btns in mobile");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "new ColTitle1 in mobile");
            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "new ColTitle2 in mobile");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "new ColText1 in mobile");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "new ColText2 in mobile");

            //Drag`n`Drop
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            DragDrop(0, 1, "BookingGrid");
            Thread.Sleep(1000);
            BlockSettingsSave();

            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in admin after DragDrop");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitler2 in admin after DragDrop");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in admin after DragDrop");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in admin after DragDrop");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in client after DragDrop");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in client after DragDrop");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in client after DragDrop");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in client after DragDrop");

            VerifyFinally(TestName);
        }

        [Test]
        public void BookingOriginalGrid()
        {
            TestName = "BookingOriginalGrid";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "block Items initial");

            //Delete
            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
            Driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            VerifyAreEqual("Нет элементов", Driver.FindElement(By.CssSelector(".lp-table__cell")).Text,
                "no elements in grid");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 0,
                "no Items in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 0,
                "no Items in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 0,
                "no Items in mobile");

            //Add
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            BookingDaysOff();
            AddNewReservation("Resource 3");
            AddNewReservation("Resource 4");
            Thread.Sleep(1000);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "Items in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "Images in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "Links in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "Btns in admin");
            VerifyAreEqual("Resource 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in admin");
            VerifyAreEqual("Resource 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in admin");
            VerifyAreEqual("Description 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in admin");
            VerifyAreEqual("Description 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "Items in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "Images in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "Links in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "Btns in client");
            VerifyAreEqual("Resource 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in client");
            VerifyAreEqual("Resource 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in client");
            VerifyAreEqual("Description 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in client");
            VerifyAreEqual("Description 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "Items in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "Images in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "Links in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "Btns in mobile");
            VerifyAreEqual("Resource 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in mobile");
            VerifyAreEqual("Resource 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in mobile");
            VerifyAreEqual("Description 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in mobile");
            VerifyAreEqual("Description 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in mobile");

            //Change
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            ChangeReservation(0, "ResourceName1", "ResourceDescription1");
            ChangeReservation(1, "ResourceName2", "ResourceDescription2");
            Thread.Sleep(1000);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "new Items in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "new Images in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "new Links in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "new Btns in admin");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "new ColTitle1 in admin");
            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "new ColTitle2 in admin");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "new ColText1 in admin");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "new ColText2 in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "new Items in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "new Images in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "new Links in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "new Btns in client");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "new ColTitle1 in client");
            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "new ColTitle2 in client");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "new ColText1 in client");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "new ColText2 in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-booking-resources__item")).Count == 2,
                "new Items in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).Count == 2,
                "new Images in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 2,
                "new Links in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColBtn + "\"]")).Count == 2,
                "new Btns in mobile");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "new ColTitle1 in mobile");
            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "new ColTitle2 in mobile");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "new ColText1 in mobile");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "new ColText2 in mobile");

            //Drag`n`Drop
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabBooking");
            DragDrop(0, 1, "BookingGrid");
            Thread.Sleep(1000);
            BlockSettingsSave();

            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in admin after DragDrop");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitler2 in admin after DragDrop");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in admin after DragDrop");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in admin after DragDrop");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("ResourceName2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in client after DragDrop");
            VerifyAreEqual("ResourceName1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in client after DragDrop");
            VerifyAreEqual("ResourceDescription2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in client after DragDrop");
            VerifyAreEqual("ResourceDescription1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in client after DragDrop");

            VerifyFinally(TestName);
        }
    }
}