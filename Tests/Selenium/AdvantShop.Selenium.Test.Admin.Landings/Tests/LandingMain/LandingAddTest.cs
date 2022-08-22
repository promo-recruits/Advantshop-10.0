using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.LandingMain
{
    [TestFixture]
    public class LandingAddTest : LandingsFunctions
    {
        static List<string> categoriesItems = new List<string>()
        {
            "Headers", "Covers", "Images", "Text", 
            "Columns", "Buttons", "Forms", "Products", 
            "Characteristics", "Video", "Counters", "Services", 
            "Quizzes", "Line", "Reviews", "Partners",
            "Team", "Schedule", "Contacts", "Footers",
            "ExitPopup", "Html"
        };

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

            Init(false);
            ClearLandingPicturesDirectory();
        }


        [Test]
        public void AddNewLandingTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            //add new landing method 1 - only for first site
            GoToAdmin("dashboard");
            Driver.FindElement(By.ClassName("add-new-site-wrap")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Создать сайт", Driver.FindElement(By.TagName("h1")).Text, "default h1");
            Driver.FindElement(By.LinkText("Презентационные воронки")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("last-item-empty-template-funnel")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Новая воронка", Driver.FindElement(By.ClassName("modal-header-title")).Text, "default modal header");
            Driver.SendKeysInput(AdvBy.DataE2E("LandingSiteName"), "NewLanding");
            Driver.FindElement(By.CssSelector("[data-e2e=\"LandingSiteNext\"]")).Click();
            Driver.WaitForAjax();
            
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("NewLanding"), "with grid - 1");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count rows - 1");
            VerifyAreEqual("NewLanding", Driver.GetGridCell(0, "Name").Text, "grid text - 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .GetAttribute("value") == "true", "grid activity - 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsMain").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .GetAttribute("value") == "true", "grid main page - 1");

            GoToAdmin("funnels");
            VerifyIsTrue(Driver.PageSource.Contains("NewLanding"), "successful creating - 1");

            //add new landing method 2
            GoToAdmin("dashboard");
            Driver.FindElement(By.ClassName("btn-minimalist")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Создать сайт", Driver.FindElement(By.TagName("h1")).Text, "default h1");
            Driver.FindElement(By.LinkText("Презентационные воронки")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("last-item-empty-template-funnel")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Новая воронка", Driver.FindElement(By.ClassName("modal-header-title")).Text, "default modal header");
            Driver.SendKeysInput(AdvBy.DataE2E("LandingSiteName"), "NewNewLanding");
            Driver.FindElement(By.CssSelector("[data-e2e=\"LandingSiteNext\"]")).Click();
            Driver.WaitForAjax();

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("NewNewLanding"), "with grid - 2");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count rows - 2");
            VerifyAreEqual("NewNewLanding", Driver.GetGridCell(0, "Name").Text, "grid text - 2");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .GetAttribute("value") == "true", "grid activity - 2");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsMain").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .GetAttribute("value") == "true", "grid main page - 2");

            GoToAdmin("funnels");
            VerifyIsTrue(Driver.PageSource.Contains("NewNewLanding"), "successful creating - 2");

            VerifyFinally(TestName);
        }


        [Test] 
        public void AddNewLandingTestBlock([ValueSource(nameof(categoriesItems))] string categoryItem)
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            CreateEmptyFunnel("NewLandingTestBlock" + categoryItem);
            
            GoToClient("lp/NewLandingTestBlock" + categoryItem + "?inplace=true");
            var elemCategory = 0;
            var j = 0;
            var n = 1;
            var blockName = "";
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBlockBtnBig\"]")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(AdvBy.DataE2E("category_" + categoryItem)).Click();
            Thread.Sleep(1000);
            elemCategory = Driver.FindElements(By.CssSelector(".blocks-constructor-modal-block")).Count;
            j = 0;
            while (j < elemCategory)
            {
                try
                {
                    blockName = Driver.FindElements(By.CssSelector(".blocks-constructor-modal-block"))[j]
                        .FindElement(By.CssSelector("img")).GetAttribute("alt");
                    Driver.FindElements(By.CssSelector(".blocks-constructor-modal-block"))[j].Click();
                    Thread.Sleep(2000);
                    VerifyAreEqual(blockName,
                        Driver.FindElement(By.CssSelector("blocks-constructor[data-block-id=\"" + n + "\"]")).GetAttribute("data-name"),
                        "block name " + blockName);
                    VerifyIsNull(CheckConsoleLog(), $"inplace not empty console log {blockName}");

                    GoToClient("lp/NewLandingTestBlock" + categoryItem + "?inplace=false");
                    VerifyIsNull(CheckConsoleLog(), $"client not empty console log {blockName}");

                    //add check for content and exist context
                    //   VerifyIsTrue(driver.FindElement(By.Id("block_"+n)).FindElements(By.CssSelector(".")).Count==1, "Count content block: " + n);
                }
                catch (Exception ex)
                {
                    VerifyAddErrors($"category{j}, {blockName}: " + ex.Message);
                }
                finally
                {
                    GoToClient("lp/NewLandingTestBlock" + categoryItem + "?inplace=true");

                    j++;
                    n++;
                    Driver.FindElement(By.CssSelector("[data-e2e=\"AddBlockBtnBig\"]")).Click();
                    Thread.Sleep(1000);
                    Driver.FindElement(AdvBy.DataE2E("category_" + categoryItem)).Click();
                    Thread.Sleep(1000);
                }
            }

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".blocks-constructor-container")).Count == n - 1,
                "Count all block: " + (n - 1));
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckRedirectToDashboard()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("funnels");
            VerifyAreEqual(BaseUrl + "adminv3/dashboard", Driver.Url, "redirect from funnels page");
            GoToAdmin("funnels/createfunnel");
            VerifyAreEqual(BaseUrl + "adminv3/dashboard/createsite", Driver.Url, "redirect from createfunnel page");
            GoToAdmin("funnels/funneldetails");
            VerifyAreEqual(BaseUrl + "adminv3/dashboard/createsite", Driver.Url, "redirect from funneldetails page");
            GoToAdmin("funnels/funneldetails?category=SaledOfGoods");
            VerifyAreEqual(BaseUrl + "adminv3/dashboard/createsite", Driver.Url, "redirect from funneldetails and params page");
            
            VerifyFinally(TestName);
        }
    }
}