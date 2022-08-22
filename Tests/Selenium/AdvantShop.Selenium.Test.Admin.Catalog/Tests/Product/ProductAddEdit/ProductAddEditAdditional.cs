using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditAdditional : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductAddEdit\\Additional\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Additional\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Additional\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Additional\\Catalog.ProductCategories.csv"
            );

            Init();

            GoToAdmin("product/edit/101");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(500);//костыль, с товаром не придумала, как иначе

            Driver.FindElement(By.Id("customoptions")).FindElement(By.TagName("ui-modal-trigger")).Click();

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);
            Driver.WaitForElem(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"), TimeSpan.FromSeconds(45));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtTitle"))
                .SendKeys("New custom options 6");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder"))
                .SendKeys("6");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_ddlInputType"))))
                .SelectByText("Выпадающий список");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtTitle"))
                .SendKeys("select button1");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtBasePrice"))
                .SendKeys("300");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_ddlPriceType"))))
                .SelectByText("Фиксированная");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtSortOrder"))
                .SendKeys("1");

            Driver.SwitchTo().DefaultContent();
            Driver.ScrollTo(By.CssSelector(".btn.btn-save.btn-primary"));
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            //VerifyFinally(TestName);
        }

        [Test]
        public void ProductEditAdditional()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("product/edit/2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(500);//костыль, с товаром не придумала, как иначе

            Driver.FindElement(By.Id("customoptions")).FindElement(By.TagName("ui-modal-trigger")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 1,
                "view custom options modal dialog");

            VerifyIsTrue(Driver.FindElements(By.TagName("iframe")).Count > 0, "iframe on page");
            VerifyIsTrue(
                Driver.FindElement(By.TagName("iframe")).GetAttribute("src").ToString()
                    .Contains("productCustomOptions.aspx"), "src page");

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);
            Driver.WaitForElem(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".option-box")).Count == 1, "view custom options box");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtTitle"))
                .SendKeys("New custom options 11");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_cbIsRequired")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder"))
                .SendKeys("10");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_ddlInputType"))))
                .SelectByText("Однострочное текстовое поле");

            Driver.SwitchTo().DefaultContent();
            Driver.ScrollTo(By.CssSelector(".btn.btn-save.btn-primary"));
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            //  Driver.ScrollToTop();
            //  Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector(".modal-footer button")).Click();
        }

        [Test]
        public void ProductEditAdditionalTextField()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("product/edit/9");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(1500);//костыль, с товаром не придумала, как иначе

            Driver.FindElement(By.Id("customoptions")).FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(Driver.FindElements(By.TagName("iframe")).Count > 0, "iframe on page");
            VerifyIsTrue(
                Driver.FindElement(By.TagName("iframe")).GetAttribute("src").ToString()
                    .Contains("productCustomOptions.aspx"), "src page");

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);
            Driver.WaitForElem(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"), TimeSpan.FromSeconds(45));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Driver.WaitForElem(By.ClassName("option-box"));
            VerifyIsTrue(Driver.FindElements(By.ClassName("option-box")).Count == 1, "view custom options box");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtTitle"))
                .SendKeys("New custom options 1");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_cbIsRequired")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder"))
                .SendKeys("10");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_ddlInputType"))))
                .SelectByText("Однострочное текстовое поле");

            Driver.SwitchTo().DefaultContent();
            Driver.ScrollTo(By.CssSelector(".btn.btn-save.btn-primary"));
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("products/test-product9");
            Driver.ScrollTo(By.ClassName("rating"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-list")).Count == 1,
                "view custom options list");
            VerifyAreEqual("New custom options 1", Driver.FindElement(By.CssSelector(".custom-options-name")).Text,
                " custom options name");
            VerifyIsTrue(
                Driver.FindElement(By.Name("customOptionsForm")).FindElements(By.CssSelector(".custom-options-value"))
                    .Count == 1, "product view custom options field");

            Driver.FindElement(By.Name("customOptionsForm")).FindElement(By.CssSelector(".custom-options-value input"))
                .Click();
            Driver.FindElement(By.Name("customOptionsForm")).FindElement(By.CssSelector(".custom-options-value input"))
                .SendKeys("test custom options1");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            Thread.Sleep(500);
            VerifyAreEqual("New custom options 1:",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " cart custom options name");
            VerifyAreEqual("test custom options1",
                Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text, " cart custom options value");

            GoToClient("checkout");
            Thread.Sleep(500);
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            VerifyAreEqual("New custom options 1:",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " checkout custom options name");
            VerifyAreEqual("test custom options1",
                Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text,
                " checkout custom options value");
        }

        [Test]
        public void ProductEditAdditionalBigTextField()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("product/edit/4");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(1500);//костыль, с товаром не придумала, как иначе

            Driver.FindElement(By.Id("customoptions")).FindElement(By.TagName("ui-modal-trigger")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 1,
                "view custom options modal dialog");

            VerifyIsTrue(Driver.FindElements(By.TagName("iframe")).Count > 0, "iframe on page");
            VerifyIsTrue(
                Driver.FindElement(By.TagName("iframe")).GetAttribute("src").ToString()
                    .Contains("productCustomOptions.aspx"), "src page");

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);
            Driver.WaitForElem(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".option-box")).Count == 1, "view custom options box");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtTitle"))
                .SendKeys("New custom options 2");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder"))
                .SendKeys("9");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_ddlInputType"))))
                .SelectByText("Многострочное поле ввода");

            Driver.SwitchTo().DefaultContent();
            Driver.ScrollTo(By.CssSelector(".btn.btn-save.btn-primary"));
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("products/test-product4");
            Driver.ScrollTo(By.ClassName("rating"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-list")).Count == 1,
                "view custom options list");
            VerifyAreEqual("New custom options 2", Driver.FindElement(By.CssSelector(".custom-options-name")).Text,
                " custom options name");
            VerifyIsTrue(
                Driver.FindElement(By.Name("customOptionsForm")).FindElements(By.CssSelector(".custom-options-value"))
                    .Count == 1, "product view custom options field");

            Driver.FindElement(By.Name("customOptionsForm"))
                .FindElement(By.CssSelector(".custom-options-value textarea")).Click();
            Driver.FindElement(By.Name("customOptionsForm"))
                .FindElement(By.CssSelector(".custom-options-value textarea")).SendKeys("test custom options2");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            VerifyAreEqual("New custom options 2:",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " cart custom options name");
            VerifyAreEqual("test custom options2",
                Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text, " cart custom options value");

            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            VerifyAreEqual("New custom options 2:",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " checkout custom options name");
            VerifyAreEqual("test custom options2",
                Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text,
                " checkout custom options value");
        }

        [Test]
        public void ProductEditAdditionalCheckBox()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("product/edit/5");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(500);//костыль, с товаром не придумала, как иначе

            Driver.FindElement(By.Id("customoptions")).FindElement(By.TagName("ui-modal-trigger")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 1,
                "view custom options modal dialog");

            VerifyIsTrue(Driver.FindElements(By.TagName("iframe")).Count > 0, "iframe on page");
            VerifyIsTrue(
                Driver.FindElement(By.TagName("iframe")).GetAttribute("src").ToString()
                    .Contains("productCustomOptions.aspx"), "src page");

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);
            Driver.WaitForElem(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".option-box")).Count == 1, "view custom options box");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtTitle"))
                .SendKeys("New custom options 3");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder"))
                .SendKeys("8");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_ddlInputType"))))
                .SelectByText("Галочка");

            Driver.FindElement(By.Name("ctl00$cphMain$productCustomOption$rCustomOptions$ctl00$txtBasePrice")).Clear();
            Driver.FindElement(By.Name("ctl00$cphMain$productCustomOption$rCustomOptions$ctl00$txtBasePrice"))
                .SendKeys("200");
            (new SelectElement(
                    Driver.FindElement(By.Name("ctl00$cphMain$productCustomOption$rCustomOptions$ctl00$ddlPriceType"))))
                .SelectByText("Фиксированная");

            Driver.SwitchTo().DefaultContent();
            Driver.ScrollTo(By.CssSelector(".btn.btn-save.btn-primary"));
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            //Driver.ScrollToTop();
            //Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("products/test-product5");
            Driver.ScrollTo(By.ClassName("rating"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-list")).Count == 1,
                "view custom options list");
            VerifyAreEqual("New custom options 3", Driver.FindElement(By.CssSelector(".custom-options-name")).Text,
                " custom options name");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-input-checkbox")).Count == 1,
                "view custom options checkbox");
            VerifyAreEqual("+200 руб.", Driver.FindElement(By.CssSelector(".custom-input-text")).Text,
                " custom options text price");
            Driver.FindElement(By.CssSelector(".custom-input-checkbox")).Click();

            VerifyAreEqual("205", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            VerifyAreEqual("New custom options 3",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " cart custom options name");
            VerifyAreEqual("+ 200 руб.", Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text,
                " cart custom options value");

            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            VerifyAreEqual("New custom options 3",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " checkout custom options name");
            VerifyAreEqual("+ 200 руб.", Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text,
                " checkout custom options value");
        }

        [Test]
        public void ProductEditAdditionalRadioButton()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("product/edit/6");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(500);//костыль, с товаром не придумала, как иначе

            Driver.FindElement(By.Id("customoptions")).FindElement(By.TagName("ui-modal-trigger")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 1,
                "view custom options modal dialog");

            VerifyIsTrue(Driver.FindElements(By.TagName("iframe")).Count > 0, "iframe on page");
            VerifyIsTrue(
                Driver.FindElement(By.TagName("iframe")).GetAttribute("src").ToString()
                    .Contains("productCustomOptions.aspx"), "src page");

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".option-box")).Count == 1, "view custom options box");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtTitle"))
                .SendKeys("New custom options 4");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder"))
                .SendKeys("7");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_ddlInputType"))))
                .SelectByText("Радио кнопки");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtTitle"))
                .SendKeys("radio button1");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtBasePrice"))
                .SendKeys("300");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_ddlPriceType"))))
                .SelectByText("Фиксированная");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtSortOrder"))
                .SendKeys("1");

            //Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtTitle"))
                .SendKeys("radio button2");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtBasePrice"))
                .SendKeys("100");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_ddlPriceType"))))
                .SelectByText("Процент");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtSortOrder"))
                .SendKeys("2");

            Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtTitle"))
                .SendKeys("radio button5");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtBasePrice"))
                .SendKeys("50000");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_ddlPriceType"))))
                .SelectByText("Фиксированная");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtSortOrder"))
                .SendKeys("3");

            Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl05_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl05_txtTitle"))
                .SendKeys("radio button3");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl05_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl05_txtBasePrice"))
                .SendKeys("500");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl05_ddlPriceType"))))
                .SelectByText("Фиксированная");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl05_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl05_txtSortOrder"))
                .SendKeys("3");

            Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_lbDelete"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_lbDelete"))
                .Click();

            Driver.SwitchTo().DefaultContent();
            Driver.ScrollTo(By.CssSelector(".btn.btn-save.btn-primary"));
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("products/test-product6");
            Driver.ScrollTo(By.ClassName("rating"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-list")).Count == 1,
                "view custom options list");
            VerifyAreEqual("New custom options 4", Driver.FindElement(By.CssSelector(".custom-options-name")).Text,
                " custom options name");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-value .custom-input-radio")).Count == 4,
                "view custom options field");

            VerifyAreEqual("6", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price  with 0 option");
            Driver.FindElements(By.CssSelector(".custom-options-value  .custom-input-radio"))[1].Click();
            VerifyAreEqual("306", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price with 1 option");

            Driver.FindElements(By.CssSelector(".custom-options-value  .custom-input-radio"))[2].Click();
            VerifyAreEqual("12", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price  with 2 option");

            Driver.FindElements(By.CssSelector(".custom-options-value  .custom-input-radio"))[3].Click();
            VerifyAreEqual("506", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price with 3 option");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            VerifyAreEqual("New custom options 4:",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " cart custom options name");
            VerifyAreEqual("radio button3 + 500 руб.",
                Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text, " cart custom options value");

            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            VerifyAreEqual("New custom options 4:",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " checkout custom options name");
            VerifyAreEqual("radio button3 + 500 руб.",
                Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text,
                " checkout custom options value");
        }

        [Test]
        public void ProductEditAdditionalSelect()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("product/edit/7");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(500);//костыль, с товаром не придумала, как иначе

            Driver.FindElement(By.Id("customoptions")).FindElement(By.TagName("ui-modal-trigger")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 1,
                "view custom options modal dialog");

            VerifyIsTrue(Driver.FindElements(By.TagName("iframe")).Count > 0, "iframe on page");
            VerifyIsTrue(
                Driver.FindElement(By.TagName("iframe")).GetAttribute("src").ToString()
                    .Contains("productCustomOptions.aspx"), "src page");

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);
            Driver.WaitForElem(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".option-box")).Count == 1, "view custom options box");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtTitle"))
                .SendKeys("New custom options 5");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder"))
                .SendKeys("6");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_ddlInputType"))))
                .SelectByText("Выпадающий список");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtTitle"))
                .SendKeys("select button1");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtBasePrice"))
                .SendKeys("300");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_ddlPriceType"))))
                .SelectByText("Фиксированная");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtSortOrder"))
                .SendKeys("1");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtTitle"))
                .SendKeys("select button2");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtBasePrice"))
                .SendKeys("100");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_ddlPriceType"))))
                .SelectByText("Процент");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtSortOrder"))
                .SendKeys("2");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtTitle"))
                .SendKeys("select button3");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtBasePrice"))
                .SendKeys("500");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_ddlPriceType"))))
                .SelectByText("Фиксированная");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl04_txtSortOrder"))
                .SendKeys("3");

            Driver.SwitchTo().DefaultContent();
            Driver.ScrollTo(By.CssSelector(".btn.btn-save.btn-primary"));
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("products/test-product7");
            Driver.ScrollTo(By.ClassName("rating"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-list")).Count == 1,
                "view custom options list");
            VerifyAreEqual("New custom options 5", Driver.FindElement(By.CssSelector(".custom-options-name")).Text,
                " custom options name");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-value select")).Count == 1,
                "view custom options field");

            VerifyAreEqual("7", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price  with 0 option");
            (new SelectElement(Driver.FindElement(By.CssSelector(".custom-options-value select")))).SelectByText(
                "Не выбрано");
            VerifyAreEqual("7", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price  with 0 option");
            (new SelectElement(Driver.FindElement(By.CssSelector(".custom-options-value select")))).SelectByText(
                "select button1 +300 руб.");
            //   VerifyAreEqual("307", driver.FindElement(By.CssSelector(".price-number")).Text, " cart custom options rezult price with 1 option");

            (new SelectElement(Driver.FindElement(By.CssSelector(".custom-options-value select")))).SelectByText(
                "select button2 +100%");
            VerifyAreEqual("14", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price  with 2 option");

            (new SelectElement(Driver.FindElement(By.CssSelector(".custom-options-value select")))).SelectByText(
                "select button3 +500 руб.");
            VerifyAreEqual("507", Driver.FindElement(By.CssSelector(".price-number")).Text,
                " cart custom options rezult price with 3 option");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            VerifyAreEqual("New custom options 5:",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " cart custom options name");
            VerifyAreEqual("select button3 + 500 руб.",
                Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text, " cart custom options value");

            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            VerifyAreEqual("New custom options 5:",
                Driver.FindElement(By.CssSelector(".cart-full-properties-name")).Text, " checkout custom options name");
            VerifyAreEqual("select button3 + 500 руб.",
                Driver.FindElement(By.CssSelector(".cart-full-properties-value")).Text,
                " checkout custom options value");
        }

        [Test]
        public void ProductEditAdditionalTextBigFieldAndSelect()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("product/edit/8");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(1500);//костыль, с товаром не придумала, как иначе

            Driver.FindElement(By.Id("customoptions")).FindElement(By.TagName("ui-modal-trigger")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 1,
                "view custom options modal dialog");

            VerifyIsTrue(Driver.FindElements(By.TagName("iframe")).Count > 0, "iframe on page");
            VerifyIsTrue(
                Driver.FindElement(By.TagName("iframe")).GetAttribute("src").ToString()
                    .Contains("productCustomOptions.aspx"), "src page");

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);
            Driver.WaitForElem(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"), TimeSpan.FromSeconds(45));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".option-box")).Count == 1, "view custom options box");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtTitle"))
                .SendKeys("New custom options 6");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_txtSortOrder"))
                .SendKeys("6");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_ddlInputType"))))
                .SelectByText("Выпадающий список");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtTitle"))
                .SendKeys("select button1");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtBasePrice"))
                .SendKeys("300");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_ddlPriceType"))))
                .SelectByText("Фиксированная");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl02_txtSortOrder"))
                .SendKeys("1");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_btnAdd")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtTitle"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtTitle"))
                .SendKeys("select button2");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtBasePrice"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtBasePrice"))
                .SendKeys("100");
            (new SelectElement(
                    Driver.FindElement(
                        By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_ddlPriceType"))))
                .SelectByText("Процент");

            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtSortOrder"))
                .Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtSortOrder"))
                .SendKeys("2");

            Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Thread.Sleep(1000);
            Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtSortOrder"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl01_txtTitle"))
                .SendKeys("New custom options 10");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl01_cbIsRequired")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl01_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl01_txtSortOrder"))
                .SendKeys("1");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl01_ddlInputType"))))
                .SelectByText("Многострочное поле ввода");

            Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_btnAddCustomOption")).Click();
            Thread.Sleep(1000);
            Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl01_txtSortOrder"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl02_txtTitle"))
                .SendKeys("New custom options 7");
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl02_cbIsRequired")).Click();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl02_txtSortOrder")).Clear();
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl02_txtSortOrder"))
                .SendKeys("10");
            (new SelectElement(
                    Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl02_ddlInputType"))))
                .SelectByText("Многострочное поле ввода");

            Driver.ScrollTo(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl00_grid_ctl03_txtSortOrder"));
            Driver.FindElement(By.Id("ctl00_cphMain_productCustomOption_rCustomOptions_ctl01_Button1")).Click();

            Driver.SwitchTo().DefaultContent();
            Driver.ScrollTo(By.CssSelector(".btn.btn-save.btn-primary"));
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("products/test-product8");
            Driver.ScrollTo(By.ClassName("rating"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-list")).Count == 1,
                "view custom options list");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".custom-options-name")).Count == 2,
                "custom options count");
            VerifyAreEqual("New custom options 6", Driver.FindElements(By.CssSelector(".custom-options-name"))[0].Text,
                " 1 custom options name");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".custom-options-value"))[0].FindElements(By.TagName("select"))
                    .Count == 1, "1 view custom options field");

            VerifyAreEqual("New custom options 7", Driver.FindElements(By.CssSelector(".custom-options-name"))[1].Text,
                "2 custom options name");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".custom-options-value"))[1].FindElements(By.TagName("textarea"))
                    .Count == 1, " 2 view custom options field");

            (new SelectElement(Driver.FindElement(By.CssSelector(".custom-options-value select")))).SelectByText(
                "select button1 +300 руб.");
            Driver.FindElements(By.CssSelector(".custom-options-value"))[1].FindElement(By.TagName("textarea"))
                .SendKeys("test custom options1");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            VerifyAreEqual("New custom options 6:",
                Driver.FindElements(By.CssSelector(".cart-full-properties-name"))[0].Text,
                " cart custom options name 1");
            VerifyAreEqual("select button1 + 300 руб.",
                Driver.FindElements(By.CssSelector(".cart-full-properties-value"))[0].Text,
                " cart custom options value 1");

            VerifyAreEqual("New custom options 7:",
                Driver.FindElements(By.CssSelector(".cart-full-properties-name"))[1].Text,
                " cart custom options name 2");
            VerifyAreEqual("test custom options1",
                Driver.FindElements(By.CssSelector(".cart-full-properties-value"))[1].Text,
                " cart custom options value 2");

            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));
            VerifyAreEqual("New custom options 6:",
                Driver.FindElements(By.CssSelector(".cart-full-properties-name"))[0].Text,
                " checkout custom options name 1");
            VerifyAreEqual("select button1 + 300 руб.",
                Driver.FindElements(By.CssSelector(".cart-full-properties-value"))[0].Text,
                " cart custom options value 1");

            VerifyAreEqual("New custom options 7:",
                Driver.FindElements(By.CssSelector(".cart-full-properties-name"))[1].Text,
                " cart custom options name 2");
            VerifyAreEqual("test custom options1",
                Driver.FindElements(By.CssSelector(".cart-full-properties-value"))[1].Text,
                " checkout custom options value 2");
        }
    }
}