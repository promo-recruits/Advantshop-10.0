using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandAddTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Brands\\BrandAdd\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Brands\\BrandAdd\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Brands\\BrandAdd\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Brands\\BrandAdd\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Brands\\BrandAdd\\Catalog.ProductCategories.csv");
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
        public void AddNewBrand()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GetButton(EButtonType.Add).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Новый производитель", Driver.FindElement(By.TagName("h1")).Text);

            Driver.FindElement(By.Id("BrandName")).SendKeys("New_Brand_name");
            (new SelectElement(Driver.FindElement(By.Id("CountryId")))).SelectByText("Россия");

            Driver.FindElement(By.Id("SortOrder")).Clear();
            Driver.FindElement(By.Id("SortOrder")).SendKeys("1");
            Driver.FindElement(By.Id("BrandSiteUrl")).SendKeys("www.testsite.ru");

            Driver.SetCkText("New_Brand_Description_here", "Description");
            Driver.ScrollTo(By.Id("cke_BriefDescription"));
            Driver.SetCkText("New_Brand_Brief_Description_here", "BriefDescription");

            Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Изображения')]"));
            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("brand_logo.jpg"));
            Driver.SendKeysInput(By.Id("URL"), "new_brand_name");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //cheking details
            GoToAdmin("brands/edit/106");
            VerifyIsTrue(Driver.GetButton(EButtonType.Simple, "ViewBrand").GetAttribute("href")
                .Contains("/manufacturers/new_brand"));
            VerifyAreEqual("www.testsite.ru", Driver.FindElement(By.Id("BrandSiteUrl")).GetAttribute("value"));

            //cheking grid
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("New_Brand_name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text);
            VerifyAreEqual("Россия", Driver.GetGridCell(0, "CountryName").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));


            //cheking client grid
            GoToClient("manufacturers");
            var element = Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3"))[0];
            VerifyAreEqual("New_Brand_name",
                Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3"))[0]
                    .FindElement(By.ClassName("brand-name")).Text);

            GoToClient("manufacturers");
            Driver.FindElement(By.Name("SearchBrand")).SendKeys("New_Brand_name");
            Driver.FindElement(By.CssSelector(".btn-ghost.icon-search-before-abs")).Click();
            VerifyAreEqual(Driver.FindElement(By.CssSelector(".brand-name a")).Text, "New_Brand_name");
            //VerifyAreEqual(driver.FindElement(By.CssSelector(".brand-bDescr div p")).Text, "New_Brand_Brief_Description_here");
            VerifyIsTrue(Driver.PageSource.Contains("New_Brand_Brief_Description_here"));
            VerifyIsFalse(Driver.PageSource.Contains("New_Brand_Description_here"));

            GoToClient("manufacturers");
            (new SelectElement(Driver.FindElement(By.Id("country")))).SelectByText("Россия");
            Driver.DropFocus("h1");
            VerifyIsTrue(Driver.PageSource.Contains("New_Brand_name"));

            // checking client details
            GoToClient("manufacturers/New_Brand_name");
            VerifyIsTrue(Driver.PageSource.Contains("New_Brand_name"));
            VerifyAreEqual(Driver.FindElement(By.LinkText("Сайт производителя")).GetAttribute("href"),
                "http://www.testsite.ru/");
            VerifyIsTrue(Driver.PageSource.Contains("New_Brand_Description_here"));
            VerifyIsFalse(Driver.PageSource.Contains("New_Brand_Brief_Description_here"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".logo-container.center-aligner img")).GetAttribute("src")
                .Contains("nophoto"));
        }

        [Test]
        public void AddNewBrandCheckDisabled()
        {
            GoToAdmin("brands/add");
            Driver.FindElement(By.Id("BrandName")).SendKeys("New_Brand_Disabled");
            Driver.FindElement(By.CssSelector(".adv-checkbox-label.form-label-block span")).Click();
            Driver.SendKeysInput(By.Id("URL"), "New_Brand_Disabled");
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("New_Brand_Disabled");
            Driver.Blur();
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            //client 
            GoToClient("manufacturers");
            Driver.FindElement(By.Id("SearchBrand")).SendKeys("New_Brand_Disabled");
            Driver.FindElement(By.CssSelector(".btn-ghost.icon-search-before-abs")).Click();
            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3")).Count > 0);

            VerifyIsTrue(Is404Page("manufacturers/New_Brand_Disabled"));
        }

        [Test]
        public void AddNewBrandCheckSEO()
        {
            GoToAdmin("brands/add");
            Driver.FindElement(By.Id("BrandName")).SendKeys("new_brand_seo");
            Driver.ScrollTo(By.Name("DefaultMeta"));
            if (Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
            }

            Driver.WaitForElem(By.Id("SeoTitle"));
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("New_Brand_Title");
            Driver.FindElement(By.Id("SeoH1")).SendKeys("New_Brand_H1");
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("New_Brand_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("New_Brand_SeoDescription");
            Driver.SendKeysInput(By.Id("URL"), "new_brand_seo");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            // check admin
            VerifyAreEqual("New_Brand_Title", Driver.FindElement(By.Id("SeoTitle")).GetAttribute("value"));
            VerifyAreEqual("New_Brand_H1", Driver.FindElement(By.Id("SeoH1")).GetAttribute("value"));
            VerifyAreEqual("New_Brand_SeoKeywords", Driver.FindElement(By.Id("SeoKeywords")).GetAttribute("value"));
            VerifyAreEqual("New_Brand_SeoDescription",
                Driver.FindElement(By.Id("SeoDescription")).GetAttribute("value"));

            //check client 
            GoToClient("manufacturers/new_brand_seo");
            VerifyAreEqual("New_Brand_Title", Driver.Title);
            VerifyAreEqual("New_Brand_H1", Driver.FindElement(By.CssSelector(".site-body-main h1")).Text);
            VerifyAreEqual("New_Brand_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("New_Brand_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
        }


        [Test]
        public void AddNewBrandCheckAddImgByHref()
        {
            GoToAdmin("brands/add");
            Driver.FindElement(By.Id("BrandName")).SendKeys("New_Brand_Img_By_Href");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgByHref\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHref\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefLinkText\"]")).SendKeys(
                "https://upload.wikimedia.org/wikipedia/en/thumb/3/34/Mandaue_Cebu.png/80px-Mandaue_Cebu.png");
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();
            Driver.SendKeysInput(By.Id("URL"), "New_Brand_Img_By_Href");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("New_Brand_Img_By_Href");

            VerifyAreEqual("New_Brand_Img_By_Href", Driver.GetGridCell(0, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));

            //client
            GoToClient("manufacturers/New_Brand_Img_By_Href");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".logo-container.center-aligner img")).GetAttribute("src")
                .Contains("nophoto"));
        }
    }
}