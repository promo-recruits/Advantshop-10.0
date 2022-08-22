using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Countdown.CountdownAlt
{
    [TestFixture]
    public class LandingsCountdownAltSettings : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Countdown\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Countdown\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Countdown\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Countdown\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Countdown\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Countdown\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Countdown\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Countdown\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Countdown\\CountdownAlt\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Countdown\\CountdownAlt\\CMS.LandingSubBlock.csv"
            );

            Init();
        }

        private string blockName = "countdownAlt";
        private string blockType = "Counters";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "TitleCountdownalt";
        private readonly string blockSubTitle = "SubTitleCountdownalt";
        private readonly string blockContent = "ContentCountdownalt";

        [Test]
        public void CountdownTypeMinutes()
        {
            TestName = "TypeCountdownMinutes";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);

            TabSelect("tabCountdown");

            var selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"TypeCountdown\"]"));
            var select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("До указанной даты"), "initial locate video");

            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"TypeCountdown\"]"))).SelectByText(
                "Выбранное кол-во минут");
            Thread.Sleep(500);
            VerifyAreEqual("30",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TimeCountDown\"]")).GetAttribute("value"),
                "Date Countdown");
            BlockSettingsSave();

            VerifyAreEqual("2", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[4].Text,
                "countdown elem: 4");
            VerifyAreEqual("9", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[5].Text,
                "countdown elem: 5");
            VerifyAreEqual("5", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[6].Text,
                "countdown elem: 6");
            var numb = Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[7].Text;
            for (var i = 0; i < 4; i++)
                VerifyAreEqual("0", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[i].Text,
                    "countdown elem: " + i);
            VerifyAreEqual("30",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).GetAttribute("is-loop"),
                "Date Countdown");
            Thread.Sleep(2000);
            VerifyAreNotEqual(numb, Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[7].Text,
                "countdown ");

            Refresh();

            VerifyAreEqual("2", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[4].Text,
                "countdown elem: 4 after refresh");
            VerifyAreEqual("9", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[5].Text,
                "countdown elem: 5 after refresh");
            VerifyAreEqual("5", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[6].Text,
                "countdown elem: 6 after refresh");
            for (var i = 0; i < 4; i++)
                VerifyAreEqual("0", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[i].Text,
                    "countdown elem: " + i + " after refresh");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabCountdown");

            VerifyAreEqual("30",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TimeCountDown\"]")).GetAttribute("value"),
                "Date Countdown after refresh");
            Driver.FindElement(By.CssSelector("[data-e2e=\"TimeCountDown\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TimeCountDown\"]")).SendKeys("5");
            BlockSettingsSave();

            VerifyAreEqual("4", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[5].Text,
                "countdown elem: 5");
            VerifyAreEqual("5", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[6].Text,
                "countdown elem: 6");
            for (var i = 0; i < 5; i++)
                VerifyAreEqual("0", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[i].Text,
                    "countdown elem: " + i);
            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).GetAttribute("is-loop"),
                "Date Countdown 2");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-countdown__item-part")).Count == 8, "count item 1");

            VerifyFinally(TestName);
        }

        [Test]
        public void CountdownTypesDays()
        {
            TestName = "TypeCountdownsDays";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            Thread.Sleep(2000);

            BlockSettingsBtn(numberBlock);
            TabSelect("tabCountdown");
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"TypeCountdown\"]"))).SelectByText(
                "До указанной даты");
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"DateCountdown\"]")).GetAttribute("value")
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "Date Countdown");

            var datatime = DateTime.Now.AddDays(3).AddHours(3).ToString("dd.MM.yyyy HH:mm");
            Driver.FindElement(By.CssSelector("[data-e2e=\"DateCountdown\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DateCountdown\"]")).SendKeys(datatime);
            TabSelect("tabCountdown");
            BlockSettingsSave();

            VerifyAreEqual("0", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[0].Text,
                "countdown elem: 0");
            VerifyAreEqual("3", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[1].Text,
                "countdown elem: 1");
            VerifyAreEqual("0", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[2].Text,
                "countdown elem: 2");
            VerifyAreEqual("2", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[3].Text,
                "countdown elem: 3");
            VerifyAreEqual("5", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[4].Text,
                "countdown elem: 4");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-countdown__item-part")).Count == 8, "count item 1");
            VerifyAreEqual("'" + datatime + "'",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).GetAttribute("end-time"),
                "Date Countdown 2");
            Refresh();
            VerifyAreEqual("'" + datatime + "'",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).GetAttribute("end-time"),
                "Date Countdown 2 after refresh");


            VerifyFinally(TestName);
        }

        [Test]
        public void InplaceHeaders()
        {
            TestName = "InplaceHeaders";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            ShowTitle();
            ShowSubTitle();
            BlockSettingsSave();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized"))
                .SendKeys("New SubTitle");

            Driver.FindElement(By.CssSelector(".lp-countdown__item-part")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");

            Thread.Sleep(1000);
            Driver.FindElement(By.Id("block_" + numberBlock)).FindElement(By.CssSelector(".lp-countdown__item-part"))
                .Click();
            Thread.Sleep(1000);

            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Text,
                "subtitle ");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Text,
                "title ");

            Refresh();
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Text,
                "subtitle after refresh");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Text,
                "title after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text, "subtitle client");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text, "title client");

            GoToMobile("lp/test1");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text, "subtitle mobile");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text, "title mobile");

            VerifyFinally(TestName);
        }
    }
}