using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Brands\\BrandEdit\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Brands\\BrandEdit\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Brands\\BrandEdit\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Brands\\BrandEdit\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Brands\\BrandEdit\\Catalog.ProductCategories.csv");
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
        public void BrandEdit()
        {
            GoToAdmin("brands/edit/1");

            Driver.FindElement(By.CssSelector(".adv-checkbox-label.form-label-block span")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.Id("BrandName")).Click();
            Driver.FindElement(By.Id("BrandName")).Clear();
            Driver.FindElement(By.Id("BrandName")).SendKeys("Brand_Name_1_Edit_Changed");
            (new SelectElement(Driver.FindElement(By.Id("CountryId")))).SelectByText("Япония");
            Driver.FindElement(By.Id("BrandSiteUrl")).Click();
            Driver.FindElement(By.Id("BrandSiteUrl")).Clear();
            Driver.FindElement(By.Id("BrandSiteUrl")).SendKeys("www.testsite.ru");

            Driver.SetCkText("Brand_1_Changed_Description_here", "Description");
            Driver.SetCkText("Brand_1_Changed_Brief_Description_here", "BriefDescription");

            Driver.ScrollTo(By.Name("DefaultMeta"));
            Driver.FindElement(By.Name("UrlPath")).Click();
            Driver.FindElement(By.Name("UrlPath")).Clear();
            Driver.FindElement(By.Name("UrlPath")).SendKeys("brand_name_1_url_changed");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check details
            VerifyIsTrue(Driver.GetButton(EButtonType.Simple, "ViewBrand").GetAttribute("href")
                .Contains("/manufacturers/brand_name_1_url_changed"));
            VerifyAreEqual("www.testsite.ru", Driver.FindElement(By.Id("BrandSiteUrl")).GetAttribute("value"));

            //check grid
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("Brand_Name_1_Edit_Changed");
            VerifyAreEqual("Brand_Name_1_Edit_Changed", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("Япония", Driver.GetGridCell(0, "CountryName").Text);

            //check client grid
            GoToClient("manufacturers");
            Driver.FindElement(By.Id("SearchBrand")).SendKeys("Brand_Name_1_Edit_Changed");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn-ghost.icon-search-before-abs")).Click();
            VerifyAreEqual(Driver.FindElement(By.CssSelector(".brand-name")).Text, "Brand_Name_1_Edit_Changed");
            VerifyIsTrue(Driver.PageSource.Contains("Brand_1_Changed_Brief_Description_here"));

            GoToClient("manufacturers");
            (new SelectElement(Driver.FindElement(By.Id("country")))).SelectByText("Япония");
            Driver.DropFocus("h1");
            VerifyIsTrue(Driver.PageSource.Contains("Brand_Name_1_Edit_Changed"));

            //check client details
            GoToClient("manufacturers/brand_name_1_url_changed");
            VerifyIsTrue(Driver.PageSource.Contains("Brand_Name_1_Edit_Changed"));
            VerifyAreEqual(Driver.FindElement(By.LinkText("Сайт производителя")).GetAttribute("href"),
                "http://www.testsite.ru/");
            VerifyIsTrue(Driver.PageSource.Contains("Brand_1_Changed_Description_here"));
        }

        [Test]
        public void EditBrandCheckSEO()
        {
            GoToAdmin("brands/edit/6");
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

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            /* change seo */
            GoToAdmin("brands/edit/6");
            Driver.ScrollTo(By.Name("DefaultMeta"));

            Driver.FindElement(By.Id("SeoTitle")).Clear();
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("Brand_6_Changed_Title");
            Driver.FindElement(By.Id("SeoH1")).Clear();
            Driver.FindElement(By.Id("SeoH1")).SendKeys("Brand_6_Changed_H1");
            Driver.FindElement(By.Id("SeoKeywords")).Clear();
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("Brand_6_Changed_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).Clear();
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("Brand_6_Changed_SeoDescription");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin 
            VerifyAreEqual("Brand_6_Changed_Title", Driver.FindElement(By.Id("SeoTitle")).GetAttribute("value"));
            VerifyAreEqual("Brand_6_Changed_H1", Driver.FindElement(By.Id("SeoH1")).GetAttribute("value"));
            VerifyAreEqual("Brand_6_Changed_SeoKeywords",
                Driver.FindElement(By.Id("SeoKeywords")).GetAttribute("value"));
            VerifyAreEqual("Brand_6_Changed_SeoDescription",
                Driver.FindElement(By.Id("SeoDescription")).GetAttribute("value"));

            //check client 
            GoToClient("manufacturers/6");
            VerifyAreEqual("Brand_6_Changed_Title", Driver.Title);
            VerifyAreEqual("Brand_6_Changed_H1", Driver.FindElement(By.CssSelector(".site-body-main h1")).Text);
            VerifyAreEqual("Brand_6_Changed_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("Brand_6_Changed_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
        }

        [Test]
        public void EditABrandCheckDisabled()
        {
            GoToAdmin("brands/edit/10");

            Driver.FindElement(By.CssSelector(".adv-checkbox-label.form-label-block span")).Click();
            Thread.Sleep(1000);

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName10");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            //check client 
            GoToClient("manufacturers");
            Driver.FindElement(By.Id("SearchBrand")).SendKeys("BrandName10");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn-ghost.icon-search-before-abs")).Click();
            var element = Driver.FindElement(By.LinkText("BrandName100"));
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3")).Count == 6);
            VerifyIsTrue(Is404Page("manufacturers/10"));
        }

        [Test]
        public void AEditBrandCheckSort()
        {
            GoToAdmin("brands/edit/40");
            Driver.FindElement(By.Id("SortOrder")).Clear();
            Driver.FindElement(By.Id("SortOrder")).SendKeys("1");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName40");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text);

            //check client
            GoToClient("manufacturers");
            var element = Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3"))[0];
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            VerifyAreEqual("BrandName40",
                Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3"))[0]
                    .FindElement(By.ClassName("brand-name")).Text);
        }

        [Test]
        public void EditBrandCheckAddImg()
        {
            GoToAdmin("brands/edit/7");
            Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Изображения')]"));
            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("brand_logo.jpg"));
            Thread.Sleep(1000);

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName7");
            VerifyAreEqual("BrandName7", Driver.GetGridCell(0, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            string picFirst = Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src");
            Driver.GetGridCell(0, "BrandName").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);

            Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Изображения')]"));
            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("brandeditpic.jpg"));
            Thread.Sleep(1000);

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName7");
            VerifyAreEqual("BrandName7", Driver.GetGridCell(0, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            string picSecond = Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src");
            VerifyIsFalse(picFirst.Equals(picSecond));

            //check client
            GoToClient("manufacturers/7");
            string strClient = Driver.FindElement(By.CssSelector(".logo-container.center-aligner img"))
                .GetAttribute("src");
            VerifyIsFalse(strClient.Contains("nophoto"));
        }

        [Test]
        public void EditBrandCheckAddImgByHref()
        {
            GoToAdmin("brands/edit/8");
            Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Изображения')]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHref\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".modal-body input")).Click();
            Driver.FindElement(By.CssSelector(".modal-body input")).Clear();
            Driver.FindElement(By.CssSelector(".modal-body input")).SendKeys(
                "https://upload.wikimedia.org/wikipedia/en/thumb/3/34/Mandaue_Cebu.png/80px-Mandaue_Cebu.png");
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName8");
            VerifyAreEqual("BrandName8", Driver.GetGridCell(0, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            string picFirst = Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src");

            GoToAdmin("brands/edit/8");
            Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Изображения')]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHref\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".modal-body input")).Click();
            Driver.FindElement(By.CssSelector(".modal-body input")).Clear();
            Driver.FindElement(By.CssSelector(".modal-body input"))
                .SendKeys("https://pbs.twimg.com/profile_images/642876497267703808/QD_L-v4q_400x400.jpg");
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName8");

            VerifyAreEqual("BrandName8", Driver.GetGridCell(0, "BrandName").Text);
            string strAdmin = Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src");
            VerifyIsFalse(strAdmin.Contains("nophoto"));
            VerifyIsFalse(picFirst.Equals(strAdmin));

            //check client
            GoToClient("manufacturers/8");
            string strClient = Driver.FindElement(By.CssSelector(".logo-container.center-aligner img"))
                .GetAttribute("src");
            VerifyIsFalse(strClient.Contains("nophoto"));
        }


        [Test]
        public void EditBrandCheckDeleteImg()
        {
            GoToAdmin("brands/edit/9");
            Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Изображения')]"));
            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("brandeditpic.jpg"));
            Thread.Sleep(1000);

            //img added
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName9");
            VerifyAreEqual("BrandName9", Driver.GetGridCell(0, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));

            //check delete img
            Driver.GetGridCell(0, "BrandName").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Изображения')]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName9");
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyAreEqual("BrandName9", Driver.GetGridCell(0, "BrandName").Text);

            //check client
            GoToClient("manufacturers/9");
            string strClient = Driver.FindElement(By.CssSelector(".logo-container.center-aligner img"))
                .GetAttribute("src");
            VerifyIsTrue(strClient.Contains("nophoto"));
        }

        [Test]
        public void EditBrandDelete()
        {
            GoToAdmin("brands/edit/94");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Удалить')]")).Click();
            Driver.SwalConfirm();

            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName94");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check client grid 
            GoToClient("manufacturers");
            Driver.FindElement(By.Id("SearchBrand")).SendKeys("BrandName94");
            Driver.FindElement(By.CssSelector(".btn-ghost.icon-search-before-abs")).Click();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3")).Count == 0);
            VerifyIsTrue(Is404Page("manufacturers/94"));
        }
    }
}