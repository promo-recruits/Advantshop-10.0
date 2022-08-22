using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CommonSettingsTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\Default\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToFunnelSettingsTab(1, "Настройки", "Общие", By.Name("RequireAuth"));
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SetName()
        {
            VerifyAreEqual("TestFunnel", GetInputValue("SiteName"), "default funnel name");
            Driver.SendKeysInput(By.Name("SiteName"), "New Test Funnel Name");
            SaveFunnelSettings(By.ClassName("toast-success"));

            VerifyAreEqual("New Test Funnel Name", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text,
                "new funnel name in header");
            Refresh(); //GetAttribute(value) не работает в режиме реального времени, а форма сохраняется аяксом
            VerifyAreEqual("New Test Funnel Name", GetInputValue("SiteName"), "new funnel name");

            Driver.SendKeysInput(By.Name("SiteName"), "TestFunnel");
            SaveFunnelSettings(By.ClassName("toast-success"));
        }

        [Test]
        public void SetNameEmpty()
        {
            VerifyAreEqual("TestFunnel", GetInputValue("SiteName"), "default funnel name");
            Driver.SendKeysInput(By.Name("SiteName"), "");
            SaveFunnelSettings(By.ClassName("toast-error"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("div[validation-output]")).Text.Contains("Не заполнены поля"),
                "empty name");
            Thread.Sleep(100);
            VerifyAreEqual("TestFunnel", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text,
                "new funnel name in header");
            Refresh(); //GetAttribute(value) не работает в режиме реального времени, а форма сохраняется аяксом
            VerifyAreEqual("TestFunnel", GetInputValue("SiteName"), "old funnel name");
        }

        [Test]
        public void SetNameSame()
        {
            VerifyAreEqual("TestFunnel", GetInputValue("SiteName"), "default funnel name");
            Driver.SendKeysInput(By.Name("SiteName"), "TestFunnel");
            VerifyIsTrue(Driver.FindElement(AdvBy.DataE2E("funnelSettingSave")).Enabled == false,
                "not enabled save button");
            VerifyAreEqual("TestFunnel", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text,
                "new funnel name in header");
            Refresh(); //GetAttribute(value) не работает в режиме реального времени, а форма сохраняется аяксом
            VerifyAreEqual("TestFunnel", GetInputValue("SiteName"), "old funnel name");
        }

        [Test]
        public void SetNameSymbols()
        {
            VerifyAreEqual("TestFunnel", GetInputValue("SiteName"), "default funnel name");
            Driver.SendKeysInput(By.Name("SiteName"), Functions.SymbolsString);
            SaveFunnelSettings(By.ClassName("toast-success"));
            Thread.Sleep(100);
            VerifyAreEqual(Functions.SymbolsString, Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text,
                "new funnel name in header");
            Refresh(); //GetAttribute(value) не работает в режиме реального времени, а форма сохраняется аяксом
            VerifyAreEqual(Functions.SymbolsString, GetInputValue("SiteName"), "new funnel name");

            Driver.SendKeysInput(By.Name("SiteName"), "TestFunnel");
            SaveFunnelSettings(By.ClassName("toast-success"));
        }

        [Test]
        public void SetNameLong()
        {
            Driver.SendKeysInput(By.Name("SiteName"), Functions.SymbolsLong);
            SaveFunnelSettings(By.ClassName("toast-success"));

            VerifyAreEqual(Functions.SymbolsLong.Substring(0, 100),
                Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text, "new funnel name in header");
            Refresh(); //GetAttribute(value) не работает в режиме реального времени, а форма сохраняется аяксом
            VerifyAreEqual(Functions.SymbolsLong.Substring(0, 100), GetInputValue("SiteName"), "new funnel name");

            Driver.SendKeysInput(By.Name("SiteName"), "TestFunnel");
            SaveFunnelSettings(By.ClassName("toast-success"));
        }
    }
}