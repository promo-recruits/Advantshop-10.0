using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsShop
{
    [TestFixture]
    public class SettingsShopTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            Init();
            GoToAdmin("settingstemplate");
            if (!Driver.FindElement(By.Id("EnableInplace")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableInplace\"]")).Click();
                Thread.Sleep(100);
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForToastSuccess();
            }
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
        public void ShopInfo()
        {
            GoToAdmin("settings/common");

            (new SelectElement(Driver.FindElement(By.Id("CountryId")))).SelectByText("Беларусь");
            Thread.Sleep(1500);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSelect\"]")))).SelectByText(
                "Брестская область");
            Driver.FindElement(By.Id("City")).Clear();
            Driver.FindElement(By.Id("City")).SendKeys("Брест");
            Driver.FindElement(By.Id("Phone")).Clear();
            Driver.FindElement(By.Id("Phone")).SendKeys("79999999999");

            Driver.FindElement(By.Id("StoreName")).Clear();
            Driver.FindElement(By.Id("StoreName")).SendKeys("Test Shop");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();
            Thread.Sleep(2000);

            GoToAdmin("settings/common");
            IWebElement selectElem1 = Driver.FindElement(By.Id("CountryId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Беларусь"), "country");

            selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSelect\"]"));
            select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Брестская область"), "region");

            VerifyAreEqual("Брест", Driver.FindElement(By.Id("City")).GetAttribute("value"), "city");
            VerifyAreEqual("79999999999", Driver.FindElement(By.Id("Phone")).GetAttribute("value"), "phone");

            GoToClient();
            VerifyAreEqual("79999999999", Driver.FindElement(By.CssSelector(".site-head-phone")).Text, "phone client");
            VerifyAreEqual("Test Shop", Driver.FindElement(By.TagName("title")).GetAttribute("innerText"),
                "title shop");
        }

        [Test]
        public void ShopFavicon()
        {
            GoToAdmin("settings/common");

            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("big.png"));
            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".picture-uploader-img"))[1].GetAttribute("src").Contains("nophoto"),
                "add img admin");
            Refresh();
            GoToAdmin("settings/common");

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".picture-uploader-img"))[1].GetAttribute("src").Contains("nophoto"),
                "add img admin 2");

            Driver.FindElements(By.CssSelector("[data-e2e=\"imgDel\"]"))[1].Click();
            Driver.SwalConfirm();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".picture-uploader-img"))[1].GetAttribute("src").Contains("nophoto"),
                "del img admin");

            Driver.FindElements(By.CssSelector("[data-e2e=\"imgByHref\"]"))[1].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefLinkText\"]"))
                .SendKeys("https://hsto.org/storage/habraeffect/8e/de/8ede5c77f2055b9374613f69b39c8e1c.png");
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".picture-uploader-img"))[1].GetAttribute("src").Contains("nophoto"),
                "add img  by href admin");

            Driver.FindElements(By.CssSelector("[data-e2e=\"imgDel\"]"))[1].Click();
            Driver.SwalConfirm();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".picture-uploader-img"))[1].GetAttribute("src").Contains("nophoto"),
                "del img admin");
        }

        [Test]
        public void ShopLogo()
        {
            GoToAdmin("settings/common");

            AttachFile(By.XPath("(//input[@type='file'])[1]"), GetPicturePath("brandpic.png"));
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".picture-uploader-img")).GetAttribute("src").Contains("nophoto"),
                "add img admin");

            GoToClient();
            VerifyIsTrue(Driver.FindElements(By.Id("logo")).Count == 1, "add img client");

            GoToAdmin("settings/common");
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".picture-uploader-img")).GetAttribute("src").Contains("nophoto"),
                "del img admin");

            GoToClient();
            VerifyIsTrue(Driver.FindElement(By.Id("logo")).GetAttribute("src").Contains("nophoto"), "del img client");
            //VerifyIsTrue(driver.FindElement(By.ClassName("logo-generator-wrap")).GetAttribute("src").Contains("nophoto"), "del img client");

            GoToAdmin("settings/common");

            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHref\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefLinkText\"]")).SendKeys(
                "http://paporotnik.com.ua/images/works/logotip-dlya-reklamnogo-internet-servisa-kartinka_1_mobile_2015-04-22-00-19-13.jpg");
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".picture-uploader-img")).GetAttribute("src").Contains("nophoto"),
                "add img admin 2");

            GoToClient();
            VerifyIsTrue(Driver.FindElements(By.Id("logo")).Count == 1, "add img client 2");
            VerifyIsFalse(Driver.FindElement(By.Id("logo")).GetAttribute("src").Contains("nophoto"), "add img client");
        }
    }
}