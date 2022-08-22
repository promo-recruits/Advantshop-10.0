using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Tag
{
    [TestFixture]
    public class TagAdd : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\Tag\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.Tag.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.TagMap.csv"
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
        public void DelTagCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTag\"]")).Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_new_Tag");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector(".link-danger.m-r-xs")).Click();
            Driver.SwalCancel();

            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridFilterSendKeys("New_new_Tag");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
        }

        [Test]
        public void DelTagOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTag\"]")).Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_new_Tagdel");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector(".link-danger.m-r-xs")).Click();
            Driver.SwalConfirm();
            Driver.GridFilterSendKeys("New_new_Tagdel");
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
        }

        [Test]
        public void OpenAllTags()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTag\"]")).Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            VerifyAreEqual("Новый тег", Driver.FindElement(By.CssSelector("h1")).Text);
            Driver.FindElement(By.CssSelector(".sticky-page-name .link-invert")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains("settingscatalog#?catalogTab=tags"));
        }

        [Test]
        public void OpenPage()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTag\"]")).Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            VerifyAreEqual("Новый тег", Driver.FindElement(By.TagName("h1")).Text);
        }

        [Test]
        public void SaveNameTag()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTag\"]")).Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Name_Tag1");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector(".adv-checkbox-emul")).Click();
            Driver.SetCkText("Edit_Tag_Description_here", "Description");
            Driver.SetCkText("Edit_Tag_Brief_Description_here", "BriefDescription");
            Driver.FindElement(By.Name("UrlPath")).Clear();
            Driver.FindElement(By.Name("UrlPath")).SendKeys("newtesttag1");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
            Driver.WaitForToastSuccess();
            //в админке 
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridFilterSendKeys("New_Name_Tag1");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("newtesttag1", Driver.GetGridCell(0, "UrlPath", "Tags").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            Driver.GetGridCell(0, "Name", "Tags").Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.AssertCkText("Edit_Tag_Description_here", "Description");
            Driver.AssertCkText("Edit_Tag_Brief_Description_here", "BriefDescription");
        }

        [Test]
        public void TagCheckMeta1()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTag\"]")).Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Meta");
            Driver.ScrollTo(By.Name("DefaultMeta"));
            Driver.FindElements(By.CssSelector(".adv-checkbox-emul"))[2].Click();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.Id("SeoH1")).SendKeys("New_Tag_H1");
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("New_Tag_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("New_Tag_SeoDescription");
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("New_Category_Title");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridFilterSendKeys("New_Meta");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            Driver.GetGridCell(0, "Name", "Tags").Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            VerifyAreEqual("New_Tag_H1", Driver.FindElement(By.Id("SeoH1")).GetAttribute("value"));
            VerifyAreEqual("New_Tag_SeoKeywords", Driver.FindElement(By.Id("SeoKeywords")).GetAttribute("value"));
            VerifyAreEqual("New_Tag_SeoDescription", Driver.FindElement(By.Id("SeoDescription")).GetAttribute("value"));
            VerifyAreEqual("New_Category_Title", Driver.FindElement(By.Id("SeoTitle")).GetAttribute("value"));
        }
    }
}