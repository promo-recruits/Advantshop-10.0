using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Tag
{
    [TestFixture]
    public class TagEdit : BaseSeleniumTest
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
        public void AOpenPage()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GetGridCell(0, "Name", "Tags").Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            VerifyAreEqual("Тег New_Tag1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("New_Tag1", Driver.FindElement(By.Id("Name")).GetAttribute("value"));

            Driver.AssertCkText("new new new1", "Description");
            Driver.AssertCkText("new tag1", "BriefDescription");

            VerifyAreEqual("teg1", Driver.FindElement(By.Id("URL")).GetAttribute("value"));
        }

        [Test]
        public void AOpenPageByPencil()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            Driver.GetGridCell(0, "_serviceColumn", "Tags").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            VerifyAreEqual("Тег New_Tag1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("New_Tag1", Driver.FindElement(By.Id("Name")).GetAttribute("value"));

            Driver.AssertCkText("new new new1", "Description");
            Driver.AssertCkText("new tag1", "BriefDescription");

            VerifyAreEqual("teg1", Driver.FindElement(By.Id("URL")).GetAttribute("value"));
        }

        [Test]
        public void AOpenAllTags()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GetGridCell(0, "Name", "Tags").Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            VerifyAreEqual("Тег New_Tag1", Driver.FindElement(By.TagName("h1")).Text);
            Driver.FindElement(By.CssSelector(".sticky-page-name .link-invert")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains("settingscatalog#?catalogTab=tags"));
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
        }

        [Test]
        public void DelTag()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GetGridCell(0, "Name", "Tags").Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New_new_Tag");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
            Driver.WaitForToastSuccess();

            Driver.FindElement(By.CssSelector(".link-danger.m-r-xs")).Click();
            Driver.SwalCancel();
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridFilterSendKeys("New_new_Tag");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1);
        }

        public void DelTagOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(By.CssSelector(".btn.btn-sm.btn-success.btn--margin-left")).Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New_new_Tagdel");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector(".link-danger.m-r-xs")).Click();
            Driver.SwalConfirm();
            Driver.GridFilterSendKeys("New_new_Tagdel");
            Driver.DropFocusCss("[data-e2e=\"TagSettingTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 0);
        }

        [Test]
        public void SaveNameTag()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GetGridCell(0, "Name", "Tags").Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.FindElement(By.Id("Name")).Clear();
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
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1);
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            Driver.GetGridCell(0, "Name", "Tags").Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.AssertCkText("Edit_Tag_Description_here", "Description");
            Driver.AssertCkText("Edit_Tag_Brief_Description_here", "BriefDescription");
        }

        [Test]
        public void TagCheckaSEOaH1()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GetGridCell(0, "Name", "Tags").Click();
            Driver.WaitForElem(By.CssSelector("#cke_Description iframe"));
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New_Tag_H1");
            Thread.Sleep(1000);
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
            Driver.GridFilterSendKeys("New_Tag_H1");
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