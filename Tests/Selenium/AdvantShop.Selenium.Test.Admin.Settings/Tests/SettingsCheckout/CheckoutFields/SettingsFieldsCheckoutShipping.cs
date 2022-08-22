using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.CheckoutFields
{
    [TestFixture]
    public class SettingsCheckoutShipping : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv"
            );

            Init();
            GoToAdmin("settingstemplate#?settingsTemplateTab=common");
            Functions.CheckSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(1000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-page"));
            Refresh();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);
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
        public void ShippingNoVisibleCountry()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowCountry", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Страна"),
                "country");
            VerifyIsTrue(Driver.FindElements(By.Id("addressCountry")).Count == 0, "country field");
        }

        [Test]
        public void ShippingNoVisibleState()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowState", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Регион"),
                "State");
            VerifyIsTrue(Driver.FindElements(By.Id("addressState")).Count == 0, "State field");
        }

        [Test]
        public void ShippingNoVisibleCity()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowCity", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Город"),
                "City");
            VerifyIsTrue(Driver.FindElements(By.Id("addressCity")).Count == 0, "City field");
        }

        [Test]
        public void ShippingNoVisibleZip()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowZip", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Индекс"),
                "Zip");
            VerifyIsTrue(Driver.FindElements(By.Id("addressZip")).Count == 0, "Zip field");
        }

        [Test]
        public void ShippingNoVisibleAddress()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckNotSelected("IsShowAddress", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Адрес"),
                "Address");
            VerifyIsTrue(Driver.FindElements(By.Id("addressDetails")).Count == 0, "Address field");
        }

        [Test]
        public void ShippingNoVisibleAddField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.FindElement(By.Id("CustomShippingField1")).Clear();
            Driver.FindElement(By.Id("CustomShippingField1")).SendKeys("Custom field9");
            Functions.CheckNotSelected("IsShowCustomShippingField1", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Refresh();
            Driver.WaitForElem(By.ClassName("checkout-page"));
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            VerifyIsFalse(Driver.PageSource.Contains("Custom field9"), "Custom1");
            VerifyIsTrue(Driver.FindElements(By.Id("customField9")).Count == 0, "Custom1 field");
        }

        [Test]
        public void ShippingNoVisibleTwoField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.FindElement(By.Id("CustomShippingField2")).Clear();
            Driver.FindElement(By.Id("CustomShippingField2")).SendKeys("Custom field8");
            Functions.CheckNotSelected("IsShowCustomShippingField2", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            VerifyIsFalse(Driver.PageSource.Contains("Custom field8"), "Custom2");
            VerifyIsTrue(Driver.FindElements(By.Id("customField8")).Count == 0, "Custom2 field");
        }

        [Test]
        public void ShippingNoVisibleComment()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowUserComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsTrue(Driver.FindElements(By.Id("CustomerComment")).Count == 0, "CustomerComment field");
        }

        [Test]
        public void ShippingNoVisibleFullAdress()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowFullAddress", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Улица"),
                "Street");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Дом"),
                "House");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Квартира"),
                "Apartment");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Подьезд"),
                "Entrance");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Этаж"),
                "floor");
        }

        [Test]
        public void ShippinzVisibleNewCustomerNo()
        {
            ReInit();
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowCountry", Driver);
            Functions.CheckNotSelected("IsShowState", Driver);
            Functions.CheckNotSelected("IsShowCity", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToAdmin("settingstemplate#?settingsTemplateTab=common");

            Functions.CheckNotSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            Thread.Sleep(2000);
            GoToClient("products/test-product5");
            Refresh();
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsFalse(Driver.PageSource.Contains("Страна"), "country");
            VerifyIsFalse(Driver.PageSource.Contains("Область"), "state");
            VerifyIsFalse(Driver.PageSource.Contains("Город"), "city");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_Contact_Country")).Count == 0, "country field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_Contact_Region")).Count == 0, "state field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_Contact_City")).Count == 0, "city field");

            ReInit();
        }

        [Test]
        public void ShippingVisibleCountry()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCountry", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Страна"),
                "country");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCountry")).Displayed, "country field");
        }

        [Test]
        public void ShippingVisibleState()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowState", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Регион"),
                "State");
            VerifyIsTrue(Driver.FindElement(By.Id("addressState")).Displayed, "State field");
        }

        [Test]
        public void ShippingVisibleCity()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCity", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Город"),
                "City");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCity")).Displayed, "City field");
        }

        [Test]
        public void ShippingVisibleZip()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowZip", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Индекс"),
                "Zip");
            VerifyIsTrue(Driver.FindElement(By.Id("addressZip")).Displayed, "Zip field");
        }

        [Test]
        public void ShippingVisibleAddress()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckSelected("IsShowAddress", Driver);
            Functions.CheckNotSelected("IsShowFullAddress", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Адрес"),
                "Address");
            VerifyIsTrue(Driver.FindElement(By.Id("addressDetails")).Displayed, "Address field");
        }

        [Test]
        public void ShippingVisibleAddField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.FindElement(By.Id("CustomShippingField1")).Clear();
            Driver.FindElement(By.Id("CustomShippingField1")).SendKeys("Custom field1");
            Functions.CheckSelected("IsShowCustomShippingField1", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-page"));

            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");

            VerifyIsTrue(Driver.PageSource.Contains("Custom field1"), "Custom1");
            VerifyIsTrue(Driver.FindElement(By.Id("customField1")).Displayed, "Custom1 field");
        }

        [Test]
        public void ShippingVisibleTwoField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCity\"]"));
            Driver.FindElement(By.Id("CustomShippingField2")).Clear();
            Driver.FindElement(By.Id("CustomShippingField2")).SendKeys("Custom field2");
            Functions.CheckSelected("IsShowCustomShippingField2", Driver);

            Driver.ScrollToTop();
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            VerifyIsTrue(Driver.PageSource.Contains("Custom field2"), "Custom2");
            VerifyIsTrue(Driver.FindElement(By.Id("customField2")).Displayed, "Custom2 field");
        }

        [Test]
        public void ShippingVisibleComment()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowUserComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsTrue(Driver.FindElement(By.Id("CustomerComment")).Displayed, "CustomerComment field");
        }

        [Test]
        public void ShippingVisibleFullAdress()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckSelected("IsShowFullAddress", Driver);
            Functions.CheckSelected("IsShowAddress", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Улица"),
                "Street");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Дом"),
                "House");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Квартира"),
                "Apartment");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Подъезд"),
                "Entrance");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Этаж"),
                "floor");
        }

        [Test]
        public void ShippingVisibleZipPlace()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("ZipDisplayPlace", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            Thread.Sleep(2000);
            GoToClient("products/test-product5");
            Refresh();
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsTrue(Driver.PageSource.Contains("Почтовый индекс"), "zip");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_Zip")).Displayed, "zip field");
            ReInit();
        }

        [Test]
        public void ShippingVisibleZipPlaceNo()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("ZipDisplayPlace", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("products/test-product5");
            Refresh();
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsFalse(Driver.PageSource.Contains("Почтовый индекс"), "zip");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_Contact_Zip")).Count == 0, "zip field");
            ReInit();
        }

        [Test]
        public void ShippinzVisibleAll()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCountry", Driver);
            Functions.CheckSelected("IsShowState", Driver);
            Functions.CheckSelected("IsShowCity", Driver);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckSelected("IsShowZip", Driver);
            Functions.CheckSelected("IsShowAddress", Driver);
            Functions.CheckSelected("IsShowUserComment", Driver);
            Functions.CheckSelected("IsShowCustomShippingField1", Driver);
            Functions.CheckSelected("IsShowCustomShippingField2", Driver);
            Functions.CheckSelected("IsShowCustomShippingField3", Driver);
            Functions.CheckSelected("ZipDisplayPlace", Driver);
            Functions.CheckSelected("IsShowFullAddress", Driver);

            Driver.FindElement(By.Id("CustomShippingField1")).Clear();
            Driver.FindElement(By.Id("CustomShippingField1")).SendKeys("Custom field22");
            Driver.FindElement(By.Id("CustomShippingField2")).Clear();
            Driver.FindElement(By.Id("CustomShippingField2")).SendKeys("Custom field23");
            Driver.FindElement(By.Id("CustomShippingField3")).Clear();
            Driver.FindElement(By.Id("CustomShippingField3")).SendKeys("Custom field24");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));

            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Страна"),
                "country");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCountry")).Displayed, "country field");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Регион"),
                "State");
            VerifyIsTrue(Driver.FindElement(By.Id("addressState")).Displayed, "State field");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Город"),
                "City");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCity")).Displayed, "City field");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Индекс"),
                "Zip");
            VerifyIsTrue(Driver.FindElement(By.Id("addressZip")).Displayed, "Zip field");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Адрес"),
                "Address");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Улица"),
                "Street");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Дом"),
                "House");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Квартира"),
                "Apartment");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Подъезд"),
                "Entrance");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Этаж"),
                "floor");

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-action")).Click();
            Thread.Sleep(2000);
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            VerifyIsTrue(Driver.PageSource.Contains("Custom field22"), "Custom1");
            VerifyIsTrue(Driver.FindElement(By.Id("customField1")).Displayed, "Custom1 field1");
            VerifyIsTrue(Driver.PageSource.Contains("Custom field23"), "Custom1");
            VerifyIsTrue(Driver.FindElement(By.Id("customField2")).Displayed, "Custom1 field2");
            VerifyIsTrue(Driver.PageSource.Contains("Custom field24"), "Custom1");
            VerifyIsTrue(Driver.FindElement(By.Id("customField3")).Displayed, "Custom1 field3");

            VerifyIsTrue(Driver.FindElement(By.Id("CustomerComment")).Displayed, "CustomerComment field");
        }

        [Test]
        public void ShippinxVisibleNoOne()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowCountry", Driver);
            Functions.CheckNotSelected("IsShowState", Driver);
            Functions.CheckNotSelected("IsShowCity", Driver);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckNotSelected("IsShowZip", Driver);
            Functions.CheckNotSelected("IsShowAddress", Driver);
            Functions.CheckNotSelected("IsShowUserComment", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField1", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField2", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField3", Driver);
            Functions.CheckNotSelected("IsShowFullAddress", Driver);
            Functions.CheckNotSelected("ZipDisplayPlace", Driver);

            Driver.FindElement(By.Id("CustomShippingField1")).Clear();
            Driver.FindElement(By.Id("CustomShippingField1")).SendKeys("Custom field22");
            Driver.FindElement(By.Id("CustomShippingField2")).Clear();
            Driver.FindElement(By.Id("CustomShippingField2")).SendKeys("Custom field23");
            Driver.FindElement(By.Id("CustomShippingField3")).Clear();
            Driver.FindElement(By.Id("CustomShippingField3")).SendKeys("Custom field24");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));

            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Страна"),
                "country");
            VerifyIsTrue(Driver.FindElements(By.Id("addressCountry")).Count == 0, "country field");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Регион"),
                "State");
            VerifyIsTrue(Driver.FindElements(By.Id("addressState")).Count == 0, "State field");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Город"),
                "City");
            VerifyIsTrue(Driver.FindElements(By.Id("addressCity")).Count == 0, "City field");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Индекс"),
                "Zip");
            VerifyIsTrue(Driver.FindElements(By.Id("addressZip")).Count == 0, "Zip field");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Адрес"),
                "Address");
            VerifyIsTrue(Driver.FindElements(By.Id("addressDetails")).Count == 0, "Address field");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Улица"),
                "Street");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Дом"),
                "House");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Квартира"),
                "Apartment");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Подьезд"),
                "Entrance");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Этаж"),
                "floor");

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-action")).Click();
            Thread.Sleep(2000);
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            VerifyIsFalse(Driver.PageSource.Contains("Custom field22"), "Custom1");
            VerifyIsTrue(Driver.FindElements(By.Id("customField1")).Count == 0, "Custom1 field");
            VerifyIsFalse(Driver.PageSource.Contains("Custom field23"), "Custom2");
            VerifyIsTrue(Driver.FindElements(By.Id("customField1")).Count == 0, "Custom2 field");
            VerifyIsFalse(Driver.PageSource.Contains("Custom field24"), "Custom3");
            VerifyIsTrue(Driver.FindElements(By.Id("customField1")).Count == 0, "Custom3 field");

            VerifyIsTrue(Driver.FindElements(By.Id("CustomerComment")).Count == 0, "CustomerComment field");
        }

        [Test]
        public void ShippinzVisibleNewCustomer()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCountry", Driver);
            Functions.CheckSelected("IsShowState", Driver);
            Functions.CheckSelected("IsShowCity", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToAdmin("settingstemplate#?settingsTemplateTab=common");
            Functions.CheckNotSelected("DisplayCityInTopPanel", Driver);

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            Thread.Sleep(2000);
            GoToClient("products/test-product5");
            Refresh();
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsTrue(Driver.PageSource.Contains("Страна"), "country");
            VerifyIsTrue(Driver.PageSource.Contains("Область"), "state");
            VerifyIsTrue(Driver.PageSource.Contains("Город"), "city");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_Country")).Displayed, "country field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_Region")).Displayed, "state field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_City")).Displayed, "city field");
        }
    }

    [TestFixture]
    public class SettingsCheckoutShippingNoRequired : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowCountry", Driver);
            Functions.CheckNotSelected("IsShowState", Driver);
            Functions.CheckNotSelected("IsShowCity", Driver);
            Functions.CheckNotSelected("IsRequiredCountry", Driver);
            Functions.CheckNotSelected("IsRequiredState", Driver);
            Functions.CheckNotSelected("IsRequiredCity", Driver);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            Functions.CheckNotSelected("IsRequiredAddress", Driver);
            Functions.CheckNotSelected("IsShowZip", Driver);
            Functions.CheckNotSelected("IsShowAddress", Driver);
            Functions.CheckNotSelected("IsShowFullAddress", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField1", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField2", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField3", Driver);
            Functions.CheckNotSelected("ZipDisplayPlace", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToAdmin("settingstemplate#?settingsTemplateTab=common");

            Functions.CheckSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(1000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Refresh();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
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
        public void ShippingNoRequiredCountry()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCountry", Driver);
            Functions.CheckNotSelected("IsRequiredCountry", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Страна"),
                "country");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCountry")).Displayed, "country field");

            (new SelectElement(Driver.FindElement(By.Id("addressCountry")))).SelectByText("Чили");

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void ShippingNoRequiredState()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowState", Driver);
            Functions.CheckNotSelected("IsRequiredState", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Регион"),
                "State");
            VerifyIsTrue(Driver.FindElement(By.Id("addressState")).Displayed, "State field");
            Driver.FindElement(By.Id("addressState")).Clear();

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void ShippingNoRequiredCity()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCity", Driver);
            Functions.CheckNotSelected("IsRequiredCity", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Город"),
                "City");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCity")).Displayed, "country field");

            Driver.FindElement(By.Id("addressCity")).Clear();

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void ShippingNoRequiredZip()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowZip", Driver);
            Functions.CheckNotSelected("IsRequiredZip", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Индекс"),
                "Zip");
            VerifyIsTrue(Driver.FindElement(By.Id("addressZip")).Displayed, "country field");

            Driver.FindElement(By.Id("addressZip")).Clear();

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void ShippingNoRequiredAddress()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckSelected("IsShowAddress", Driver);
            Functions.CheckNotSelected("IsRequiredAddress", Driver);
            Functions.CheckNotSelected("IsShowFullAddress", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Адрес"),
                "Address");
            VerifyIsTrue(Driver.FindElement(By.Id("addressDetails")).Displayed, "country field");

            Driver.FindElement(By.Id("addressDetails")).Clear();

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.ScrollTo(By.TagName("footer"));
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void ShippingNoRequiredAddField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.FindElement(By.Id("CustomShippingField1")).Clear();
            Driver.FindElement(By.Id("CustomShippingField1")).SendKeys("Custom field1");
            Functions.CheckSelected("IsShowCustomShippingField1", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField1", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-page"));
            Refresh();
            Driver.WaitForElem(By.ClassName("checkout-page"));
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");

            VerifyIsTrue(Driver.PageSource.Contains("Custom field1"), "Custom1");
            VerifyIsTrue(Driver.FindElements(By.Id("customField1")).Count == 1, "Custom1 field");
            Driver.FindElement(By.Id("customField1")).Clear();

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".checkout-success-content"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void ShippingNoRequiredTwoField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Driver.FindElement(By.Id("CustomShippingField2")).Clear();
            Driver.FindElement(By.Id("CustomShippingField2")).SendKeys("Custom field2");
            Functions.CheckSelected("IsShowCustomShippingField2", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField2", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            VerifyIsTrue(Driver.PageSource.Contains("Custom field2"), "Custom2");
            VerifyIsTrue(Driver.FindElements(By.Id("customField2")).Count == 1, "Custom2 field");
            Driver.FindElement(By.Id("customField2")).Clear();

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".checkout-success-content"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }
    }

    [TestFixture]
    public class SettingsCheckoutShippingRequired : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsRequiredCountry", Driver);
            Functions.CheckNotSelected("IsRequiredState", Driver);
            Functions.CheckNotSelected("IsRequiredCity", Driver);
            Functions.CheckNotSelected("IsShowCountry", Driver);
            Functions.CheckNotSelected("IsShowState", Driver);
            Functions.CheckNotSelected("IsShowCity", Driver);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            Functions.CheckNotSelected("IsRequiredAddress", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField1", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField2", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField3", Driver);

            Functions.CheckNotSelected("IsShowZip", Driver);
            Functions.CheckNotSelected("IsShowAddress", Driver);
            Functions.CheckNotSelected("IsShowFullAddress", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField1", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField2", Driver);
            Functions.CheckNotSelected("IsShowCustomShippingField3", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(1000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Refresh();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
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
        public void ShippingRequiredCountry()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCountry", Driver);
            Functions.CheckSelected("IsRequiredCountry", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Страна"),
                "country");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCountry")).Displayed, "country field");

            (new SelectElement(Driver.FindElement(By.Id("addressCountry")))).SelectByText("Чили");

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Functions.CheckNotSelected("IsRequiredCountry", Driver);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void ShippingRequiredState()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowState", Driver);
            Functions.CheckSelected("IsRequiredState", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Регион"),
                "State");
            VerifyIsTrue(Driver.FindElement(By.Id("addressState")).Displayed, "State field");
            Driver.FindElement(By.Id("addressState")).Clear();

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed, "modal window");

            Driver.FindElement(By.Id("addressState")).SendKeys("Московская область");
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "no modal window");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Functions.CheckNotSelected("IsRequiredState", Driver);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void ShippingRequiredCity()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCity", Driver);
            Functions.CheckSelected("IsRequiredCity", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Город"),
                "City");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCity")).Displayed, "country field");

            Driver.FindElement(By.Id("addressCity")).Clear();

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed, "modal window");

            Driver.FindElement(By.Id("addressCity")).SendKeys("Москва");
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "no modal window");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Functions.CheckNotSelected("IsRequiredCity", Driver);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void ShippingRequiredZip()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowZip", Driver);
            Functions.CheckSelected("IsRequiredZip", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Индекс"),
                "Zip");
            VerifyIsTrue(Driver.FindElement(By.Id("addressZip")).Displayed, "country field");

            Driver.FindElement(By.Id("addressZip")).Clear();
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed, "modal window");

            Driver.FindElement(By.Id("addressZip")).SendKeys("999999");
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "no modal window");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckNotSelected("IsRequiredZip", Driver);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void ShippingRequiredAddress()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckSelected("IsShowAddress", Driver);
            Functions.CheckSelected("IsRequiredAddress", Driver);
            Functions.CheckNotSelected("IsShowFullAddress", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Адрес"),
                "Address");
            VerifyIsTrue(Driver.FindElement(By.Id("addressDetails")).Displayed, "country field");

            Driver.FindElement(By.Id("addressDetails")).Clear();

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed, "modal window");

            Driver.FindElement(By.Id("addressDetails")).SendKeys("Test street, 8");
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "no modal window");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckNotSelected("IsRequiredAddress", Driver);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void ShippingRequiredAddField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Driver.FindElement(By.Id("CustomShippingField1")).Clear();
            Driver.FindElement(By.Id("CustomShippingField1")).SendKeys("Custom field1");
            Functions.CheckSelected("IsShowCustomShippingField1", Driver);
            Functions.CheckSelected("IsReqCustomShippingField1", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-page"));
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            VerifyIsTrue(Driver.PageSource.Contains("Custom field1"), "Custom1");
            VerifyIsTrue(Driver.FindElements(By.Id("customField1")).Count == 1, "Custom1 field");
            Driver.FindElement(By.Id("customField1")).Clear();

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail");
            Driver.FindElement(By.Id("customField1")).SendKeys("Custom field1");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".checkout-success-content"));

            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");

            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckNotSelected("IsReqCustomShippingField1", Driver);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void ShippingRequiredTwoField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Driver.FindElement(By.Id("CustomShippingField2")).Clear();
            Driver.FindElement(By.Id("CustomShippingField2")).SendKeys("Custom field2");
            Functions.CheckSelected("IsShowCustomShippingField2", Driver);
            Functions.CheckSelected("IsReqCustomShippingField2", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            //Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            VerifyIsTrue(Driver.PageSource.Contains("Custom field2"), "Custom2");
            VerifyIsTrue(Driver.FindElements(By.Id("customField2")).Count == 1, "Custom2 field");
            Driver.FindElement(By.Id("customField2")).Clear();

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail");
            Driver.FindElement(By.Id("customField2")).SendKeys("Custom field2");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".checkout-success-content"));

            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckNotSelected("IsReqCustomShippingField2", Driver);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void ShippinzRequiredAll()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCountry", Driver);
            Functions.CheckSelected("IsShowState", Driver);
            Functions.CheckSelected("IsShowCity", Driver);
            Functions.CheckSelected("IsRequiredCountry", Driver);
            Functions.CheckSelected("IsRequiredState", Driver);
            Functions.CheckSelected("IsRequiredCity", Driver);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"IsShowCountry\"]"));
            Functions.CheckSelected("IsShowZip", Driver);
            Functions.CheckSelected("IsShowAddress", Driver);
            Functions.CheckSelected("IsShowCustomShippingField1", Driver);
            Functions.CheckSelected("IsShowCustomShippingField2", Driver);
            Functions.CheckSelected("IsShowCustomShippingField3", Driver);
            Functions.CheckNotSelected("IsShowFullAddress", Driver);

            Functions.CheckSelected("IsRequiredZip", Driver);
            Functions.CheckSelected("IsRequiredAddress", Driver);
            Functions.CheckSelected("IsReqCustomShippingField1", Driver);
            Functions.CheckSelected("IsReqCustomShippingField2", Driver);
            Functions.CheckSelected("IsReqCustomShippingField3", Driver);

            Driver.FindElement(By.Id("CustomShippingField1")).Clear();
            Driver.FindElement(By.Id("CustomShippingField1")).SendKeys("Custom field22");
            Driver.FindElement(By.Id("CustomShippingField2")).Clear();
            Driver.FindElement(By.Id("CustomShippingField2")).SendKeys("Custom field23");
            Driver.FindElement(By.Id("CustomShippingField3")).Clear();
            Driver.FindElement(By.Id("CustomShippingField3")).SendKeys("Custom field24");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));

            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            VerifyAreEqual("Адрес", Driver.FindElement(By.CssSelector(".modal-header")).Text, "modal window h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Страна"),
                "country");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCountry")).Displayed, "country field");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Регион"),
                "State");
            VerifyIsTrue(Driver.FindElement(By.Id("addressState")).Displayed, "State field");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Город"),
                "City");
            VerifyIsTrue(Driver.FindElement(By.Id("addressCity")).Displayed, "City field");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Индекс"),
                "Zip");
            VerifyIsTrue(Driver.FindElement(By.Id("addressZip")).Displayed, "Zip field");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Text.Contains("Адрес"),
                "Address");
            VerifyIsTrue(Driver.FindElement(By.Id("addressDetails")).Displayed, "Address field");

            Driver.FindElement(By.Id("addressCity")).Clear();
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "modal window City");
            Driver.FindElement(By.Id("addressCity")).SendKeys("Москва");

            Thread.Sleep(1000);
            Driver.FindElement(By.ClassName("autocompleter-list-item")).Click();
            VerifyAreEqual("101000", Driver.FindElement(By.Id("addressZip")).GetAttribute("value"),
                "modal window Zip autoset");
            Driver.FindElement(By.Id("addressZip")).Clear();

            Driver.FindElement(By.Id("addressState")).Click();
            Driver.FindElement(By.Id("addressState")).Clear();
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "modal window State");
            Driver.FindElement(By.Id("addressState")).SendKeys("Московская область");
            Thread.Sleep(1000);
            Driver.FindElements(By.ClassName("autocompleter-list-item"))[1].Click();

            (new SelectElement(Driver.FindElement(By.Id("addressCountry")))).SelectByText("Аргентина");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();

            Driver.FindElement(By.Id("addressDetails")).Clear();
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "modal window Details");
            Driver.FindElement(By.Id("addressDetails")).SendKeys("Test street, 88");
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();

            Driver.FindElement(By.Id("addressZip")).Clear();
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "modal window Zip");
            Driver.FindElement(By.Id("addressZip")).SendKeys("999999");

            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "no modal window");

            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            Driver.FindElement(By.Id("customField1")).Clear();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail 1");
            Driver.FindElement(By.Id("customField1")).SendKeys("Custom field2");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail 2");
            Driver.FindElement(By.Id("customField2")).Clear();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail 3");
            Driver.FindElement(By.Id("customField2")).SendKeys("Custom field2");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail 4");
            Driver.FindElement(By.Id("customField3")).Clear();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail 5");
            Driver.FindElement(By.Id("customField3")).SendKeys("Custom field2");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();

            Driver.WaitForElem(By.CssSelector(".checkout-success-content"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
            GoToAdmin("orders");
            Driver.GetGridCell(0, "StatusName").Click();
            Thread.Sleep(2000);
            //VerifyAreEqual("Аргентина", driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"), "country in order");
            //VerifyAreEqual("Московская область", driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "state in order");
            //VerifyAreEqual("Москва", driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"), "city in order");
            //VerifyAreEqual("Test street, 88", driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"), "street in order");
            //VerifyAreEqual("999999", driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"), "zip in order");
            VerifyAreEqual("Аргентина, Московская область, Москва, 999999, улица Test street, 88",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerAdress\"]")).Text, "adress in order");
        }

        [Test]
        public void ShippinxRequiredNoOne()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowCountry", Driver);
            Functions.CheckSelected("IsShowState", Driver);
            Functions.CheckSelected("IsShowCity", Driver);
            Functions.CheckSelected("IsShowZip", Driver);
            Functions.CheckSelected("IsShowAddress", Driver);
            Functions.CheckSelected("IsShowCustomShippingField1", Driver);
            Functions.CheckSelected("IsShowCustomShippingField2", Driver);
            Functions.CheckSelected("IsShowCustomShippingField3", Driver);
            Functions.CheckNotSelected("IsShowFullAddress", Driver);

            Functions.CheckNotSelected("IsRequiredCountry", Driver);
            Functions.CheckNotSelected("IsRequiredState", Driver);
            Functions.CheckNotSelected("IsRequiredCity", Driver);
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            Functions.CheckNotSelected("IsRequiredAddress", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField1", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField2", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField3", Driver);

            Driver.FindElement(By.Id("CustomShippingField1")).Clear();
            Driver.FindElement(By.Id("CustomShippingField1")).SendKeys("Custom field22");
            Driver.FindElement(By.Id("CustomShippingField2")).Clear();
            Driver.FindElement(By.Id("CustomShippingField2")).SendKeys("Custom field23");
            Driver.FindElement(By.Id("CustomShippingField3")).Clear();
            Driver.FindElement(By.Id("CustomShippingField3")).SendKeys("Custom field24");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.FindElement(By.CssSelector(".address-controls-item")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("form")).Displayed, "modal window");
            (new SelectElement(Driver.FindElement(By.Id("addressCountry")))).SelectByText("Чили");

            Driver.FindElement(By.Id("addressState")).Clear();
            Driver.FindElement(By.Id("addressCity")).Clear();
            Driver.FindElement(By.Id("addressZip")).Clear();
            Driver.FindElement(By.Id("addressDetails")).Clear();
            Driver.FindElement(By.CssSelector(".modal-footer"))
                .FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#modalAddress .modal-content")).Displayed,
                "no modal window");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"));
            Driver.XPathContainsText("span", "Курьером");
            Driver.ScrollTo(By.CssSelector(".custom-input-radio"),
                Driver.FindElements(By.CssSelector(".shipping-item")).Count - 1);
            Driver.FindElement(By.Id("customField1")).Clear();
            Driver.FindElement(By.Id("customField2")).Clear();
            Driver.FindElement(By.Id("customField3")).Clear();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();

            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");

            GoToAdmin("orders");
            Driver.GetGridCell(0, "StatusName").Click();
            Thread.Sleep(2000);
            //VerifyAreEqual("Чили", driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"), "country in order");
            //VerifyAreEqual("", driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "state in order");
            //VerifyAreEqual("Москва", driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"), "city in order");
            //VerifyAreEqual("", driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"), "street in order");
            //VerifyAreEqual("", driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"), "zip in order");

            VerifyAreEqual("Чили, Москва,", Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerAdress\"]")).Text,
                "adress in order");
        }

        [Test]
        public void ShippinyRequiredNewCustomer()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowLastName", Driver);
            Functions.CheckNotSelected("IsShowPatronymic", Driver);
            Functions.CheckNotSelected("IsShowPhone", Driver);
            Functions.CheckNotSelected("IsShowZip", Driver);
            Functions.CheckSelected("IsShowCountry", Driver);
            Functions.CheckSelected("IsShowState", Driver);
            Functions.CheckSelected("IsShowCity", Driver);

            Functions.CheckSelected("IsRequiredCountry", Driver);
            Functions.CheckSelected("IsRequiredState", Driver);
            Functions.CheckSelected("IsRequiredCity", Driver);

            Functions.CheckNotSelected("IsRequiredZip", Driver);
            Functions.CheckNotSelected("IsRequiredAddress", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField1", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField2", Driver);
            Functions.CheckNotSelected("IsReqCustomShippingField3", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToAdmin("settingstemplate#?settingsTemplateTab=common");

            Functions.CheckNotSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            Thread.Sleep(2000);
            GoToClient("products/test-product5");
            Refresh();
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsTrue(Driver.PageSource.Contains("Страна"), "country");
            VerifyIsTrue(Driver.PageSource.Contains("Область"), "state");
            VerifyIsTrue(Driver.PageSource.Contains("Город"), "city");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_Country")).Displayed, "country field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_Region")).Displayed, "state field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_City")).Displayed, "city field");

            Driver.FindElement(By.Id("Data_User_Email")).SendKeys("TestName@test.test");
            Driver.FindElement(By.Id("Data_User_FirstName")).SendKeys("TestName");
            Driver.FindElement(By.Id("Data_Contact_Country")).Clear();
            Driver.FindElement(By.Id("Data_Contact_Region")).Clear();
            Driver.FindElement(By.Id("Data_Contact_City")).Clear();

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail 1");
            Driver.FindElement(By.Id("Data_Contact_Country")).SendKeys("Россия");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail 2");
            Driver.FindElement(By.Id("Data_Contact_Region")).SendKeys("Ульяновская область");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "fail 3");
            Driver.FindElement(By.Id("Data_Contact_City")).SendKeys("Ульяновск");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();

            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            ReInit();
            GoToAdmin("orders");
            Driver.GetGridCell(0, "StatusName").Click();
            Thread.Sleep(2000);
            //VerifyAreEqual("Россия", driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"), "country in order");
            //VerifyAreEqual("Ульяновская область", driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "state in order");
            //VerifyAreEqual("Ульяновск", driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"), "city in order");

            VerifyAreEqual("Россия, Ульяновская область, Ульяновск,",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerAdress\"]")).Text, "adress in order");
        }

        [Test]
        public void ShippinyRequiredNewCustomerNo()
        {
            ReInit();
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowLastName", Driver);
            Functions.CheckNotSelected("IsShowPatronymic", Driver);
            Functions.CheckNotSelected("IsShowPhone", Driver);
            Functions.CheckSelected("IsShowCountry", Driver);
            Functions.CheckSelected("IsShowState", Driver);
            Functions.CheckSelected("IsShowCity", Driver);

            Functions.CheckNotSelected("IsRequiredCountry", Driver);
            Functions.CheckNotSelected("IsRequiredState", Driver);
            Functions.CheckNotSelected("IsRequiredCity", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToAdmin("settingstemplate#?settingsTemplateTab=common");

            Functions.CheckNotSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            Thread.Sleep(2000);
            GoToClient("products/test-product5");
            Refresh();
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsTrue(Driver.PageSource.Contains("Страна"), "country");
            VerifyIsTrue(Driver.PageSource.Contains("Область"), "state");
            VerifyIsTrue(Driver.PageSource.Contains("Город"), "city");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_Country")).Displayed, "country field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_Region")).Displayed, "state field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_Contact_City")).Displayed, "city field");

            Driver.FindElement(By.Id("Data_User_Email")).SendKeys("TestName@test.test");
            Driver.FindElement(By.Id("Data_User_FirstName")).SendKeys("TestName");
            Driver.FindElement(By.Id("Data_Contact_Country")).Clear();
            Driver.FindElement(By.Id("Data_Contact_Region")).Clear();
            Driver.FindElement(By.Id("Data_Contact_City")).Clear();

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();

            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            ReInit();
            GoToAdmin("orders");
            Driver.GetGridCell(0, "StatusName").Click();
            Thread.Sleep(2000);
            //VerifyAreEqual("", driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"), "country in order");
            //VerifyAreEqual("", driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "state in order");
            //VerifyAreEqual("", driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"), "city in order");
            //VerifyAreEqual("", driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"), "street in order");
            //VerifyAreEqual("", driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"), "zip in order");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CustomerAdress\"]")).Count == 0,
                "no adress in order");
        }
    }
}