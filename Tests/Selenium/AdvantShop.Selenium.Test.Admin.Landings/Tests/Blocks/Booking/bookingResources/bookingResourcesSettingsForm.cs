using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Booking.bookingResources
{
    [TestFixture]
    public class bookingResourcesSettingsForm : LandingsFunctions
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
        private int numberBlock = 1;
        private string blockIcon = "BookingResourcesPicture";
        private string blockColTitle = "BookingResourcesColTitle";
        private string blockColText = "BookingResourcesColText";
        private string blockColLink = "BookingResourcesColLink";
        private string blockColBtn = "BookingResourcesBtn";

        [Ignore("Useless")]
        [Test]
        public void TestMethod1()
        {
            TestName = "TestMethod1";
            VerifyBegin(TestName);


            VerifyFinally(TestName);
        }
    }
}