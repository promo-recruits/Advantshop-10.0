using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditProperties : BaseSeleniumTest
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
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Photo.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductPropertyValue.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductCategories.csv"
            );

            Init();
            EnableInplaceOff();
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

        public void AddPropertyToProduct(string property, string propertyValue)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"selectProperty\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"selectProperty\"] input")).SendKeys(property);
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".ui-select-choices-row-inner")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"selectPropertyValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"selectPropertyValue\"] input")).SendKeys(propertyValue);
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".ui-select-choices-row-inner")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"propertyAddSelected\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.XPathContainsText("span", propertyValue);
        }

        public void RemoveProperty(string property)
        {
            GoToAdmin("settingscatalog");
            Driver.GridFilterSendKeys(property);
            Driver.FindElement(AdvBy.DataE2E("gridHeaderCheckboxWrapSelectAll")).Click();
            Driver.SetGridAction("Удалить выделенные");
            Driver.SwalConfirm();
        }

        [Test]
        public void ProductEditAddProperty()
        {
            GoToAdmin("product/edit/1");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(2000);//костыль, с товаром не придумала, как иначе

            AddPropertyToProduct("Property2", "PropertyValue12");
            AddPropertyToProduct("Property2", "PropertyValue13");
            AddPropertyToProduct("Property3", "PropertyValue22");
            AddPropertyToProduct("Property10", "PropertyValue55");
            AddPropertyToProduct("Property11", "PropertyValue60");
            AddPropertyToProduct("Property20", "PropertyValue80");
            
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-group-name.cs-t-5")).Text
                .Contains("PropertyGroup1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[1].Text
                .Contains("PropertyGroup2"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[2].Text
                .Contains("PropertyGroup3"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-name")).Text.Contains("Property1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[1].Text.Contains("Property2"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[2].Text.Contains("Property3"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-value")).Text.Contains("PropertyValue2"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text
                .Contains("PropertyValue12"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text
                .Contains("PropertyValue13"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[2].Text
                .Contains("PropertyValue22"));

            GoToClient("products/test-product1?tab=tabOptions");
            Driver.ScrollTo(By.Id("tabDescription"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[0].Text
                .Contains("PropertyGroup1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[1].Text
                .Contains("PropertyGroup2"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[2].Text
                .Contains("PropertyGroup3"));

            Driver.ScrollTo(By.Id("tabDescription"));
            VerifyAreEqual("Property1", Driver.FindElements(By.CssSelector(".properties-item-name"))[0].Text);
            VerifyAreEqual("Property2", Driver.FindElements(By.CssSelector(".properties-item-name"))[1].Text);
            VerifyAreEqual("Property3", Driver.FindElements(By.CssSelector(".properties-item-name"))[2].Text);
            VerifyAreEqual("Property10", Driver.FindElements(By.CssSelector(".properties-item-name"))[3].Text);
            VerifyAreEqual("Property11", Driver.FindElements(By.CssSelector(".properties-item-name"))[4].Text);
            VerifyAreEqual("Property20", Driver.FindElements(By.CssSelector(".properties-item-name"))[5].Text);

            VerifyAreEqual("PropertyValue2", Driver.FindElements(By.CssSelector(".properties-item-value"))[0].Text);
            VerifyAreEqual("PropertyValue12, PropertyValue13",
                Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text);
            VerifyAreEqual("PropertyValue22", Driver.FindElements(By.CssSelector(".properties-item-value"))[2].Text);
            VerifyAreEqual("PropertyValue55", Driver.FindElements(By.CssSelector(".properties-item-value"))[3].Text);
            VerifyAreEqual("PropertyValue60", Driver.FindElements(By.CssSelector(".properties-item-value"))[4].Text);
            VerifyAreEqual("PropertyValue80", Driver.FindElements(By.CssSelector(".properties-item-value"))[5].Text);
        }

        [Test]
        public void ProductEditAddNewProperty()
        {
            GoToAdmin("product/edit/5");

            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(1000);//костыль, с товаром не придумала, как иначе
            AddPropertyToProduct("Новое свойство", "Новое значение свойства");
            Thread.Sleep(1000);
            Driver.XPathContainsText("h2", "Свойства");
            Driver.FindElement(By.CssSelector("[data-e2e=\"propertyAddSelected\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.PageSource.Contains("Новое значение свойства"));
            VerifyIsTrue(Driver.PageSource.Contains("Новое свойство"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-name")).Text.Contains("Property1"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".properties-item-name"))[1].Text.Contains("Новое свойство"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-value")).Text.Contains("PropertyValue6"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text
                .Contains("Новое значение свойства"));

            GoToAdmin("settingscatalog#?catalogTab=properties");
            VerifyIsTrue(Driver.PageSource.Contains("Новое свойство"));

            GoToClient("products/test-product5?tab=tabOptions");
            Driver.ScrollTo(By.Id("tabDescription"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[0].Text
                .Contains("PropertyGroup1"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[1].Text.Contains("Прочее"));

            VerifyAreEqual("Property1", Driver.FindElements(By.CssSelector(".properties-item-name"))[0].Text);
            VerifyAreEqual("Новое свойство", Driver.FindElements(By.CssSelector(".properties-item-name"))[1].Text);

            VerifyAreEqual("PropertyValue6", Driver.FindElements(By.CssSelector(".properties-item-value"))[0].Text);
            VerifyAreEqual("Новое значение свойства",
                Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text);

            RemoveProperty("Новое свойство");
        }

        [Test]
        public void ProductEditAddNewPropertyValue()
        {
            GoToAdmin("product/edit/7");

            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(2000);//костыль, с товаром не придумала, как иначе
            AddPropertyToProduct("Property100", "Новое значение свойства2");
            Thread.Sleep(1000);
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"propertyAddSelected\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-group-name.cs-t-5")).Text
                .Contains("PropertyGroup1"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[1].Text.Contains("Прочее"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-name")).Text.Contains("Property1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[1].Text.Contains("Property100"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-value")).Text.Contains("PropertyValue8"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text
                .Contains("Новое значение свойства2"));

            GoToAdmin("propertyValues?propertyId=100");
            VerifyAreEqual("Новое значение свойства2", Driver.GetGridCell(0, "Value").Text);

            GoToClient("products/test-product7?tab=tabOptions");
            Driver.ScrollTo(By.Id("tabDescription"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[0].Text
                .Contains("PropertyGroup1"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[1].Text.Contains("Прочее"));

            Driver.ScrollTo(By.Id("tabDescription"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[0].GetAttribute("innerText")
                .Contains("Property1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[1].GetAttribute("innerText")
                .Contains("Property100"));

            VerifyAreEqual("PropertyValue8", Driver.FindElements(By.CssSelector(".properties-item-value"))[0].Text);
            VerifyAreEqual("Новое значение свойства2",
                Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text);
        }

        [Test]
        public void ProductEditAddDelProperty()
        {
            GoToAdmin("product/edit/6");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(1000);//костыль, с товаром не придумала, как иначе
            AddPropertyToProduct("Новое свойство0", "Новое значение свойства0");
            AddPropertyToProduct("Новое свойство01", "Новое значение свойства01");
            AddPropertyToProduct("Новое свойство02", "Новое значение свойства02");
            AddPropertyToProduct("Новое свойство03", "Новое значение свойства03");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-name")).Text.Contains("Property1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[1].Text
                .Contains("Новое свойство0"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[2].Text
                .Contains("Новое свойство01"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[3].Text
                .Contains("Новое свойство02"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[4].Text
                .Contains("Новое свойство03"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-value")).Text.Contains("PropertyValue7"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text
                .Contains("Новое значение свойства0"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[2].Text
                .Contains("Новое значение свойства01"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[3].Text
                .Contains("Новое значение свойства02"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[4].Text
                .Contains("Новое значение свойства03"));

            Driver.FindElements(By.CssSelector(".close.ui-select-match-close"))[3].Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[3].Text
                .Contains("Новое значение свойства03"));

            GoToAdmin("settingscatalog#?catalogTab=properties");
            VerifyAreEqual("Новое свойство0", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Новое свойство01", Driver.GetGridCell(1, "Name").Text);
            VerifyAreEqual("Новое свойство02", Driver.GetGridCell(2, "Name").Text);
            VerifyAreEqual("Новое свойство03", Driver.GetGridCell(3, "Name").Text);

            Driver.GetGridCell(0, "GroupName").Click();
            Thread.Sleep(1000);
            Driver.DropFocusCss("[data-e2e=\"PropertyValueSettingTitle\"]");
            VerifyAreEqual("Новое значение свойства0", Driver.GetGridCell(0, "Value", "PropertyValues").Text);

            GoToClient("products/test-product6?tab=tabOptions");
            Driver.ScrollTo(By.Id("tabDescription"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[0].Text
                .Contains("PropertyGroup1"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".properties-group-name.cs-t-5"))[1].Text.Contains("Прочее"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[0].GetAttribute("innerText")
                .Contains("Property1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[1].GetAttribute("innerText")
                .Contains("Новое свойство0"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[2].GetAttribute("innerText")
                .Contains("Новое свойство01"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[3].GetAttribute("innerText")
                .Contains("Новое свойство03"));

            VerifyAreEqual("PropertyValue7", Driver.FindElements(By.CssSelector(".properties-item-value"))[0].Text);
            VerifyAreEqual("Новое значение свойства0",
                Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text);
            VerifyAreEqual("Новое значение свойства01",
                Driver.FindElements(By.CssSelector(".properties-item-value"))[2].Text);
            VerifyAreEqual("Новое значение свойства03",
                Driver.FindElements(By.CssSelector(".properties-item-value"))[3].Text);

            RemoveProperty("Новое свойство0");

        }

        [Test]
        public void ProductEditDelAllProperty()
        {
            GoToAdmin("product/edit/4");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(1000);//костыль, с товаром не придумала, как иначе

            AddPropertyToProduct("Новое свойство1", "Новое значение свойства1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-name")).Text.Contains("Property1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-name"))[1].Text
                .Contains("Новое свойство1"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-value")).Text.Contains("PropertyValue5"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".properties-item-value"))[1].Text
                .Contains("Новое значение свойства1"));

            Driver.FindElements(By.CssSelector(".close.ui-select-match-close"))[0].Click();
            Thread.Sleep(1000);
            Driver.FindElements(By.CssSelector(".close.ui-select-match-close"))[0].Click();
            Thread.Sleep(1000);
            VerifyIsTrue(0 == Driver.FindElements(By.CssSelector(".properties-item-name")).Count);
            VerifyIsTrue(0 == Driver.FindElements(By.CssSelector(".properties-item-value")).Count);

            GoToClient("products/test-product4?tab=tabOptions");
            Driver.ScrollTo(By.Id("tabDescription"));
            VerifyIsFalse(Driver.PageSource.Contains("PropertyGroup1"));
            VerifyIsFalse(Driver.PageSource.Contains("Прочее"));
            VerifyIsFalse(Driver.PageSource.Contains("Новое значение свойства1"));
            VerifyIsFalse(Driver.PageSource.Contains("Новое свойство1"));
            VerifyIsFalse(Driver.PageSource.Contains("Property1"));
            VerifyIsFalse(Driver.PageSource.Contains("PropertyValue5"));

            RemoveProperty("Новое свойство1");
        }
    }
}