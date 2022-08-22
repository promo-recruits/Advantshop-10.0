using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditShippingTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Tag.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyGroup.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Property.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductPropertyValue.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductCategories.csv"
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
        public void ProductEditWeightEdit()
        {
            GoToClient("products/test-product1");
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector(".details-row.details-weight"))
                    .FindElement(By.CssSelector(".inplace-offset.details-param-value-weight")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-row.details-weight")).Count == 1);

            GoToAdmin("product/edit/1");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("1", Driver.FindElement(By.Id("Weight")).GetAttribute("value"));

            Driver.FindElement(By.Id("Weight")).Click();
            Driver.FindElement(By.Id("Weight")).Clear();
            Driver.FindElement(By.Id("Weight")).SendKeys("100");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/1");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("100", Driver.FindElement(By.Id("Weight")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product1");
            VerifyAreEqual("100",
                Driver.FindElement(By.CssSelector(".details-row.details-weight"))
                    .FindElement(By.CssSelector(".inplace-offset.details-param-value-weight")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-row.details-weight")).Count == 1);
        }


        [Test]
        public void ProductEditWeightAdd()
        {
            GoToClient("products/test-product4");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-row.details-weight")).Count == 0);

            GoToAdmin("product/edit/4");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("0", Driver.FindElement(By.Id("Weight")).GetAttribute("value"));

            Driver.FindElement(By.Id("Weight")).Click();
            Driver.FindElement(By.Id("Weight")).Clear();
            Driver.FindElement(By.Id("Weight")).SendKeys("400");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/4");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("400", Driver.FindElement(By.Id("Weight")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product4");
            VerifyAreEqual("400",
                Driver.FindElement(By.CssSelector(".details-row.details-weight"))
                    .FindElement(By.CssSelector(".inplace-offset.details-param-value-weight")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-row.details-weight")).Count == 1);
        }

        [Test]
        public void ProductEditLengthWidthHeightEdit()
        {
            GoToClient("products/test-product6");
            VerifyAreEqual("6 x 6 x 6",
                Driver.FindElement(By.CssSelector(".details-row.details-dimensions"))
                    .FindElement(By.CssSelector(".details-param-value")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-row.details-dimensions")).Count == 1);

            GoToAdmin("product/edit/6");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("6", Driver.FindElement(By.Id("Length")).GetAttribute("value"));
            VerifyAreEqual("6", Driver.FindElement(By.Id("Width")).GetAttribute("value"));
            VerifyAreEqual("6", Driver.FindElement(By.Id("Height")).GetAttribute("value"));

            Driver.FindElement(By.Id("Length")).Click();
            Driver.FindElement(By.Id("Length")).Clear();
            Driver.FindElement(By.Id("Length")).SendKeys("600");
            Driver.FindElement(By.Id("Width")).Click();
            Driver.FindElement(By.Id("Width")).Clear();
            Driver.FindElement(By.Id("Width")).SendKeys("600");
            Driver.FindElement(By.Id("Height")).Click();
            Driver.FindElement(By.Id("Height")).Clear();
            Driver.FindElement(By.Id("Height")).SendKeys("600");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/6");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("600", Driver.FindElement(By.Id("Length")).GetAttribute("value"));
            VerifyAreEqual("600", Driver.FindElement(By.Id("Width")).GetAttribute("value"));
            VerifyAreEqual("600", Driver.FindElement(By.Id("Height")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product6");
            VerifyAreEqual("600 x 600 x 600",
                Driver.FindElement(By.CssSelector(".details-row.details-dimensions"))
                    .FindElement(By.CssSelector(".details-param-value")).Text);
        }


        [Test]
        public void ProductEditLengthWidthHeightAdd()
        {
            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-row.details-dimensions")).Count == 0);

            GoToAdmin("product/edit/5");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("0", Driver.FindElement(By.Id("Length")).GetAttribute("value"));
            VerifyAreEqual("0", Driver.FindElement(By.Id("Width")).GetAttribute("value"));
            VerifyAreEqual("0", Driver.FindElement(By.Id("Height")).GetAttribute("value"));

            Driver.FindElement(By.Id("Length")).Click();
            Driver.FindElement(By.Id("Length")).Clear();
            Driver.FindElement(By.Id("Length")).SendKeys("500");
            Driver.FindElement(By.Id("Width")).Click();
            Driver.FindElement(By.Id("Width")).Clear();
            Driver.FindElement(By.Id("Width")).SendKeys("500");
            Driver.FindElement(By.Id("Height")).Click();
            Driver.FindElement(By.Id("Height")).Clear();
            Driver.FindElement(By.Id("Height")).SendKeys("500");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/5");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("500", Driver.FindElement(By.Id("Length")).GetAttribute("value"));
            VerifyAreEqual("500", Driver.FindElement(By.Id("Width")).GetAttribute("value"));
            VerifyAreEqual("500", Driver.FindElement(By.Id("Height")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product5");
            VerifyAreEqual("500 x 500 x 500",
                Driver.FindElement(By.CssSelector(".details-row.details-dimensions"))
                    .FindElement(By.CssSelector(".details-param-value")).Text);
        }


        [Test]
        public void ProductEditShippingPriceEdit()
        {
            GoToAdmin("product/edit/8");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("8", Driver.FindElement(By.Id("Weight")).GetAttribute("value"));

            Driver.FindElement(By.Id("ShippingPrice")).Click();
            Driver.FindElement(By.Id("ShippingPrice")).Clear();
            Driver.FindElement(By.Id("ShippingPrice")).SendKeys("800");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/8");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("800", Driver.FindElement(By.Id("ShippingPrice")).GetAttribute("value"));
        }


        [Test]
        public void ProductEditShippingPriceAdd()
        {
            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("0", Driver.FindElement(By.Id("ShippingPrice")).GetAttribute("value"));

            Driver.FindElement(By.Id("ShippingPrice")).Click();
            Driver.FindElement(By.Id("ShippingPrice")).Clear();
            Driver.FindElement(By.Id("ShippingPrice")).SendKeys("700");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("700", Driver.FindElement(By.Id("ShippingPrice")).GetAttribute("value"));
        }
    }
}