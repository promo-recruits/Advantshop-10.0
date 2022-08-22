using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests
{
    [TestFixture]
    public class Example : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\CMS.LandingSiteSettings.csv"
            );

            Init();
        }

        [Test]
        public void Add()
        {
            TestName = "Add";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            AddBlockByBtnBig("Forms", "formSubscribe");
            AddBlockByBtnBig("Forms", "formSubscribe");
            AddBlockByBtnBig("Forms", "formSubscribe");
            Thread.Sleep(5000);
            DelBlocksAll();
            /*  AddBlockByBtnSml(1, "Team", "teamThreeColumns");
            BlockSettingsBtn(2);
            SetColorScheme("Темная");
            HiddenInDesktop();
            HiddenSubTitle();
            RoundPhotoOn();
            ShowOnAllPage();
            OpenPopupUpdateImage();
            UpdateLoadImageDesktop("images.jpg");
            DelImageSave();
            UpdateImageByUrl("https://avatanplus.com/files/resources/mid/5abd3d8455c4c16273384cff.jpg");
            BehaviorBackgroundImage("Фиксированный фон");
            moveSlider(5);
  
  
            TabSelect("tabTeamButton");
            BtnEnabledButton();
            BtnSetTextButton("Button1");
            BtnActionButtonSelect("Переход на оплату");
            FormProductSelect("testProduct13");
            HideShipping();
  
            BlockSettingsSave();
            MoveDownBlockByBtn(2);
            MoveUpBlockByBtn(2);
            DelBlockBtnCancel(2);
            DelBlockBtn(2);
  
            BlockSettingsBtn(1);*/

            VerifyFinally(TestName);
        }
    }
}