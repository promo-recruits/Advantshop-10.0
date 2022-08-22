using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Booking.Tests
{
    [TestFixture]
    public class BookingTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Booking);
            InitializeService.LoadData(
                "Data\\Admin\\AddBooking\\Booking.Affiliate.csv",
                "Data\\Admin\\AddBooking\\Booking.Category.csv",
                "Data\\Admin\\AddBooking\\Booking.Service.csv",
                "Data\\Admin\\AddBooking\\Booking.ReservationResource.csv",
                "Data\\Admin\\AddBooking\\Booking.AffiliateAdditionalTime.csv",
                "Data\\Admin\\AddBooking\\Booking.AffiliateCategory.csv",
                "Data\\Admin\\AddBooking\\Booking.AffiliateManager.csv",
                "Data\\Admin\\AddBooking\\Booking.AffiliateReservationResource.csv",
                "Data\\Admin\\AddBooking\\Booking.AffiliateService.csv",
                "Data\\Admin\\AddBooking\\Booking.AffiliateTimeOfBooking.csv",
                "Data\\Admin\\AddBooking\\Booking.ReservationResourceAdditionalTime.csv",
                "Data\\Admin\\AddBooking\\Booking.ReservationResourceService.csv",
                "Data\\Admin\\AddBooking\\Booking.ReservationResourceTag.csv",
                "Data\\Admin\\AddBooking\\Booking.ReservationResourceTagsMap.csv",
                "Data\\Admin\\AddBooking\\Booking.ReservationResourceTimeOfBooking.csv",
                "Data\\Admin\\AddBooking\\Booking.Booking.csv",
                "Data\\Admin\\AddBooking\\Booking.BookingCurrency.csv",
                "Data\\Admin\\AddBooking\\Booking.BookingItems.csv"
            );

            InitializeService.BookingActive();
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void OpenPageBooking()
        {
            GoToAdmin("booking");
            VerifyIsFalse(Is404Page("adminv3/booking"), "not 404");
        }
    }
}