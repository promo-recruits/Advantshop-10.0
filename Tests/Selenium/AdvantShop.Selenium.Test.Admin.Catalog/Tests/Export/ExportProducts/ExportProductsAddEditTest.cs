using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Export.ExportProducts
{
    [TestFixture]
    public class ExportProductsAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Export\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\Export\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Photo.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Property.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.PropertyValue.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.ProductPropertyValue.csv"
            );

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
        [Order(0)]
        public void ExportProductsAddNew()
        {
            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportProducts");

            VerifyIsFalse(Driver.PageSource.Contains("Новая выгрузка тест"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddExportFeed\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Новая выгрузка", Driver.FindElement(By.TagName("h2")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).SendKeys("Новая выгрузка тест");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddDesc\"]")).SendKeys("Описание тест");

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("Export"));

            //check 
            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportProducts");

            VerifyIsTrue(Driver.PageSource.Contains("Новая выгрузка тест"));

            Driver.FindElement(By.CssSelector(".export-feed-block-name"))
                .FindElement(By.XPath("//a[contains(text(), 'Новая выгрузка тест')]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("Export"));

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            VerifyAreEqual("Новая выгрузка тест", Driver.FindElement(By.Id("Name")).GetAttribute("value"));
            VerifyAreEqual("Описание тест", Driver.FindElement(By.Id("Description")).GetAttribute("value"));
        }

        [Test]
        [Order(1)]
        public void ExportProductsEdit()
        {
            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportProducts");

            VerifyIsTrue(Driver.PageSource.ToLower().Contains("выгрузка каталога в csv"));

            Driver.FindElements(By.CssSelector(".export-feed-block-name a"))[0].Click();
            Driver.WaitForElem(By.XPath("//a[contains(text(), 'Параметры выгрузки')]"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (!Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                VerifyIsFalse(Driver.FindElement(By.Id("ExportFeedSettings_Interval")).Enabled);
                VerifyIsFalse(Driver.FindElement(By.Id("ddlIntervalType")).Enabled);

                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
                VerifyIsTrue(Driver.FindElement(By.Id("ExportFeedSettings_Interval")).Enabled);
                VerifyIsTrue(Driver.FindElement(By.Id("ddlIntervalType")).Enabled);

                Driver.FindElement(By.Id("ExportFeedSettings_Interval")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_Interval")).Clear();

                Driver.FindElement(By.Id("ExportFeedSettings_Interval")).SendKeys("10");
                Driver.DropFocus("h1");

                (new SelectElement(Driver.FindElement(By.Id("ddlIntervalType")))).SelectByText("В часах");
            }

            else
            {
                Driver.FindElement(By.Id("ExportFeedSettings_Interval")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_Interval")).Clear();

                Driver.FindElement(By.Id("ExportFeedSettings_Interval")).SendKeys("10");
                Driver.DropFocus("h1");

                (new SelectElement(Driver.FindElement(By.Id("ddlIntervalType")))).SelectByText("В часах");
            }

            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("Измененное название выгрузки тест");

            Driver.FindElement(By.Id("Description")).Click();
            Driver.FindElement(By.Id("Description")).Clear();
            Driver.FindElement(By.Id("Description")).SendKeys("Измененное описание выгрузки тест");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check 
            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportProducts");

            VerifyIsFalse(Driver.PageSource.ToLower().Contains("выгрузка каталога в csv"));
            VerifyIsTrue(Driver.PageSource.Contains("Измененное название выгрузки тест"));

            Driver.FindElement(By.CssSelector(".export-feed-block-name"))
                .FindElement(By.XPath("//a[contains(text(), 'Измененное название выгрузки тест')]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("Export"));

            GoToAdmin("exportfeeds/exportfeed/2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();

            VerifyAreEqual("Измененное название выгрузки тест",
                Driver.FindElement(By.Id("Name")).GetAttribute("value"));
            VerifyAreEqual("Измененное описание выгрузки тест",
                Driver.FindElement(By.Id("Description")).GetAttribute("value"));

            IWebElement selectElem = Driver.FindElement(By.Id("ddlIntervalType"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("В часах"));
        }

        [Test]
        [Order(2)]
        public void ExportProductsDelete() //может перенести это в Маркетинг? 
        {
            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportProducts");

            VerifyIsTrue(Driver.PageSource.Contains("Новая выгрузка тест"));

            Driver.FindElement(By.CssSelector(".export-feed-block-name"))
                .FindElement(By.XPath("//a[contains(text(), 'Новая выгрузка тест')]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("Export"));

            //GoToAdmin("exportfeeds/exportfeed/3");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Удалить')]")).Click();
            Driver.SwalConfirm();

            //check 
            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportProducts");

            VerifyIsFalse(Driver.PageSource.Contains("Новая выгрузка тест"));
        }
    }
}