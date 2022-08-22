using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.EditCategory
{
    [TestFixture]
    public class CategoryEditRecommendationProduct : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\EditCategory\\Catalog.Category.csv",
                "Data\\Admin\\EditCategory\\Catalog.Brand.csv",
                "Data\\Admin\\EditCategory\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\EditCategory\\Catalog.Property.csv",
                "Data\\Admin\\EditCategory\\Catalog.PropertyValue.csv",
                "Data\\Admin\\EditCategory\\Catalog.Product.csv",
                "Data\\Admin\\EditCategory\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\EditCategory\\Catalog.Offer.csv",
                "Data\\Admin\\EditCategory\\Catalog.ProductCategories.csv"
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
        public void GroupPropertyCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgDel\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));
            VerifyAreEqual("Добавление группы свойств", Driver.FindElement(By.TagName("h2")).Text);
            (new SelectElement(Driver.FindElement(By.CssSelector(".modal-body select")))).SelectByText("Group1");
            Driver.XPathContainsText("button", "Добавить группу");
            Driver.ScrollTo(By.Id("UrlPath"));
            VerifyAreEqual("Group1", Driver.GetGridCell(0, "Name", "PropertyGroups").Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));
            (new SelectElement(Driver.FindElement(By.CssSelector(".modal-body select")))).SelectByText("Group3");
            Driver.XPathContainsText("button", "Добавить группу");
            Driver.ScrollTo(By.Id("UrlPath"));
            VerifyAreEqual("Group1", Driver.GetGridCell(0, "Name", "PropertyGroups").Text);
            VerifyAreEqual("Group3", Driver.GetGridCell(1, "Name", "PropertyGroups").Text);
        }

        [Test]
        public void DelGroupPropertyCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgDel\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));
            (new SelectElement(Driver.FindElement(By.CssSelector(".modal-body select")))).SelectByText("Group1");
            Driver.XPathContainsText("button", "Добавить группу");
            Driver.ScrollTo(By.Id("UrlPath"));
            VerifyAreEqual("Group1", Driver.GetGridCell(0, "Name", "PropertyGroups").Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));
            (new SelectElement(Driver.FindElement(By.CssSelector(".modal-body select")))).SelectByText("Group3");
            Driver.XPathContainsText("button", "Добавить группу");
            Driver.ScrollTo(By.Id("UrlPath"));
            VerifyAreEqual("Group1", Driver.GetGridCell(0, "Name", "PropertyGroups").Text);
            VerifyAreEqual("Group3", Driver.GetGridCell(1, "Name", "PropertyGroups").Text);
            Driver.GetGridCell(0, "_serviceColumn", "PropertyGroups").FindElement(By.TagName("a")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Group3", Driver.GetGridCell(0, "Name", "PropertyGroups").Text);
        }

        [Test]
        public void AddRelatedCategory()
        {
            GoToAdmin("category/edit/7");
            Driver.ScrollTo(By.Id("UrlPath"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]"));

            Driver.FindElement(By.CssSelector("[data-type=\"Related\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("3")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("TestCategory3", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[1].Text);
            Driver.FindElement(By.CssSelector("[data-type=\"Related\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("TestCategory2", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[1].Text);
            VerifyAreEqual("TestCategory3", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[2].Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryDelete-id-3\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestCategory2", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[1].Text);

            GoToClient("/products/test-product32");
            var element9 = Driver.FindElement(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));
            IJavaScriptExecutor jse9 = (IJavaScriptExecutor) Driver;
            jse9.ExecuteScript("arguments[0].scrollIntoView(true)", element9);
            Thread.Sleep(1000);

            //С этим товаром покупают
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyAreEqual(4,
                Driver.FindElements(
                    By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item")).Count);

            VerifyAreEqual("TestProduct28",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct30",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct29",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct31",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //pre
            Driver.FindElement(By.CssSelector(".carousel-nav-prev.icon-left-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct28",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct30",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void AddRelatedProperty()
        {
            GoToAdmin("category/edit/8");
            Driver.ScrollTo(By.Id("UrlPath"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]"));
            Driver.FindElement(By.CssSelector("[data-type=\"Related\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-6 select")))).SelectByText("Property5");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-6 select"))[1])).SelectByText("5");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            Driver.FindElement(By.CssSelector("[data-type=\"Related\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-6 select")))).SelectByText("Property1");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-6 select"))[1])).SelectByText("1");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"RecommendationProperty\"]"))[0].Text
                .Contains("Property5 - 5"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"RecommendationProperty\"]"))[0].Text
                .Contains("Property1 - 1"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyWithValueDelete-value-id-12\"]"))
                .Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyWithValueDelete-value-id-1\"]"))
                .Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"RecommendationProperty\"]"))[0].Text
                .Contains("Property1 - 1"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"RecommendationProperty\"]"))[0].Text
                .Contains("Property5 - 5"));

            GoToClient("/products/test-product48");

            //С этим товаром покупают
            VerifyIsFalse(Driver.PageSource.Contains("С этим товаром покупают"));
            VerifyAreEqual(0,
                Driver.FindElements(
                    By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item")).Count);
        }

        [Test]
        public void AddRelatedCategoryAndProperty()
        {
            GoToAdmin("category/edit/9");
            Driver.ScrollTo(By.Id("UrlPath"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]"));

            Driver.FindElement(By.CssSelector("[data-type=\"Related\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("TestCategory2", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[1].Text);

            Driver.FindElement(By.CssSelector("[data-type=\"Related\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyAdd\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-6 select")))).SelectByText("Property1");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-6 select"))[1])).SelectByText("1");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("/products/test-product62");
            //С этим товаром покупают
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyAreEqual(2,
                Driver.FindElements(
                    By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item")).Count);

            VerifyAreEqual("TestProduct30",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct31",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void AddAlternativeCategory()
        {
            GoToAdmin("category/edit/10");
            Driver.ScrollTo(By.Id("UrlPath"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]"));

            Driver.FindElement(By.CssSelector("[data-type=\"Alternative\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("TestCategory2", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[5].Text);
            Driver.FindElement(By.CssSelector("[data-type=\"Alternative\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("1")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("TestCategory1", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[5].Text);
            VerifyAreEqual("TestCategory2", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[6].Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryDelete-id-1\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestCategory2", Driver.FindElements(By.CssSelector(".col-xs-4.col-wl-5 .m-b-sm"))[5].Text);

            GoToClient("/products/test-product79");
            var element7 = Driver.FindElement(By.CssSelector(".h2"));
            IJavaScriptExecutor jse7 = (IJavaScriptExecutor) Driver;
            jse7.ExecuteScript("arguments[0].scrollIntoView(true)", element7);

            //Похожие товары    
            VerifyAreEqual("TestProduct28",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct30",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct29",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct31",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //pre
            Driver.FindElement(By.CssSelector(".carousel-nav-prev.icon-left-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct28",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct30",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void AddAlternativeProperty()
        {
            GoToAdmin("category/edit/1");

            Driver.ScrollTo(By.Id("UrlPath"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]"));

            Driver.FindElement(By.CssSelector("[data-type=\"Alternative\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyAdd\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-6 select")))).SelectByText("Property5");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-6 select"))[1])).SelectByText("5");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            Driver.FindElement(By.CssSelector("[data-type=\"Alternative\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyAdd\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-6 select")))).SelectByText("Property1");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-6 select"))[1])).SelectByText("1");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"RecommendationProperty\"]"))[1].Text
                .Contains("Property5 - 5"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"RecommendationProperty\"]"))[1].Text
                .Contains("Property1 - 1"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyWithValueDelete-value-id-1\"]"))
                .Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"RecommendationProperty\"]"))[1].Text
                .Contains("Property1 - 1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"RecommendationProperty\"]"))[1].Text
                .Contains("Property5 - 5"));

            GoToClient("/products/test-product1");
            //Похожие товары
            VerifyIsFalse(Driver.PageSource.Contains("Похожие товары"));
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".carousel-inner")).Count);
        }

        [Test]
        public void AddAlternativeCategoryAndProperty()
        {
            GoToAdmin("category/edit/3");
            Driver.ScrollTo(By.Id("UrlPath"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]"));

            Driver.FindElement(By.CssSelector("[data-type=\"Alternative\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            Driver.FindElement(By.CssSelector("[data-type=\"Alternative\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyAdd\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-6 select")))).SelectByText("Property1");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-6 select"))[1])).SelectByText("1");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("/products/test-product25");
            //Похожие товары
            VerifyAreEqual("Похожие товары", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("Похожие товары"));
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))
                    .Count);
            VerifyAreEqual("TestProduct30",
                Driver.FindElements(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct31",
                Driver.FindElements(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void AddAlternativeRelatedCategoryAndProperty()
        {
            GoToAdmin("category/edit/6");
            Driver.ScrollTo(By.Id("UrlPath"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"categoryGroupAdd\"]"));

            //с этим товаром покупают
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-type=\"Related\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            Driver.FindElement(By.CssSelector("[data-type=\"Related\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyAdd\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-6 select")))).SelectByText("Property1");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-6 select"))[1])).SelectByText("1");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            //Похожие товары
            Driver.FindElement(By.CssSelector("[data-type=\"Alternative\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationCategoryAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector("[data-type=\"Alternative\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"RecommendationPropertyAdd\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-6 select")))).SelectByText("Property1");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-6 select"))[1])).SelectByText("2");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            GoToClient("/products/test-product26");

            //Похожие товары
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("Похожие товары"));
            VerifyAreEqual(4,
                Driver.FindElements(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))
                    .Count);
            VerifyAreEqual("TestProduct30",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct31",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("Похожие товары", Driver.FindElements(By.CssSelector(".h2"))[1].Text);
            VerifyAreEqual("TestProduct28",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct29",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }
    }
}