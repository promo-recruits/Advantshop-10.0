using System.Collections.ObjectModel;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditPriceTest : BaseSeleniumTest
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
        public void ProductEditChangeCurrency()
        {
            GoToAdmin("product/edit/1");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            //pre check
            IWebElement selectElemBegin = Driver.FindElement(By.Id("CurrencyId"));
            SelectElement select = new SelectElement(selectElemBegin);
            VerifyIsTrue(select.AllSelectedOptions[0].Text.Contains("Рубли"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAmount\"]"))
                .FindElement(By.TagName("span")).Text.Contains("руб."));

            (new SelectElement(Driver.FindElement(By.Id("CurrencyId")))).SelectByText("Евро");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/1");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            IWebElement selectElemEnd = Driver.FindElement(By.Id("CurrencyId"));
            SelectElement select1 = new SelectElement(selectElemEnd);
            VerifyIsTrue(select1.AllSelectedOptions[0].Text.Contains("Евро"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAmount\"]"))
                .FindElement(By.TagName("span")).Text.Contains("€"));

            //check client
            GoToClient("products/test-product1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price .price-number"))
                .Text.Contains("75"));
        }

        [Test]
        public void ProductDiscountAddAmount()
        {
            GoToAdmin("mainpageproducts?type=sale");
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(1, "Name").Text);
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            GoToAdmin("product/edit/4");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAmount\"]")).GetAttribute("class")
                .Contains("active"))
            {
                VerifyAreEqual("0", Driver.FindElement(By.Id("DiscountAmount")).GetAttribute("value"));
            }

            else if (Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).GetAttribute("class")
                .Contains("active"))
            {
                VerifyAreEqual("0", Driver.FindElement(By.Id("DiscountPercent")).GetAttribute("value"));
            }

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAmount\"]")).GetAttribute("class")
                .Contains("active"))
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAmount\"]")).Click();
            }

            Driver.FindElement(By.Id("DiscountAmount")).Click();
            Driver.FindElement(By.Id("DiscountAmount")).Clear();
            Driver.FindElement(By.Id("DiscountAmount")).SendKeys("3");
            Driver.XPathContainsText("h2", "Цена и наличие");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/4");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("3", Driver.FindElement(By.Id("DiscountAmount")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAmount\"]")).GetAttribute("class")
                .Contains("active"));

            GoToAdmin("mainpageproducts?type=sale");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct4"));
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check client
            GoToClient("products/test-product4");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price"))
                .FindElements(By.CssSelector(".price-number"))[1].Text.Contains("1"));
            VerifyIsTrue(Driver.PageSource.Contains("Скидка 3 руб."));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-block .price-discount")).Text
                .Contains("выгода 3 руб."));
        }

        [Test]
        public void ProductDiscountAddPercent()
        {
            GoToAdmin("mainpageproducts?type=sale");
            var gridCountBegin = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;
            VerifyIsTrue(gridCountBegin >= 2);

            GoToAdmin("product/edit/26");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAmount\"]")).GetAttribute("class")
                .Contains("active"))
            {
                VerifyAreEqual("0", Driver.FindElement(By.Id("DiscountAmount")).GetAttribute("value"));
            }

            else if (Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).GetAttribute("class")
                .Contains("active"))
            {
                VerifyAreEqual("0", Driver.FindElement(By.Id("DiscountPercent")).GetAttribute("value"));
            }

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).GetAttribute("class")
                .Contains("active"))
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).Click();
            }

            Driver.FindElement(By.Id("DiscountPercent")).Click();
            Driver.FindElement(By.Id("DiscountPercent")).Clear();
            Driver.FindElement(By.Id("DiscountPercent")).SendKeys("50");
            Driver.XPathContainsText("h2", "Цена и наличие");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/26");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("50", Driver.FindElement(By.Id("DiscountPercent")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).GetAttribute("class")
                .Contains("active"));

            GoToAdmin("mainpageproducts?type=sale");
            var gridCountEnd = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;
            VerifyIsTrue(gridCountBegin < gridCountEnd);

            //check client
            GoToClient("products/test-product26");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price"))
                .FindElements(By.CssSelector(".price-number"))[1].Text.Contains("13"));
            VerifyIsTrue(Driver.PageSource.Contains("Скидка 50%"));
            VerifyAreEqual("выгода 13 руб. или 50%",
                Driver.FindElement(By.CssSelector(".details-payment-block .price-discount")).Text);
        }

        [Test]
        public void ProductDiscountPercentEdit()
        {
            //pre check
            GoToAdmin("mainpageproducts?type=sale");
            var gridCountBegin = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;

            GoToClient("products/test-product18");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price"))
                .FindElements(By.CssSelector(".price-number"))[1].Text.Contains("15"));
            VerifyIsTrue(Driver.PageSource.Contains("Скидка 18%"));
            VerifyAreEqual("выгода 3 руб. или 18%",
                Driver.FindElement(By.CssSelector(".details-payment-block .price-discount")).Text);

            GoToAdmin("product/edit/18");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("18", Driver.FindElement(By.Id("DiscountPercent")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).GetAttribute("class")
                .Contains("active"));

            Driver.FindElement(By.Id("DiscountPercent")).Click();
            Driver.FindElement(By.Id("DiscountPercent")).Clear();
            Driver.FindElement(By.Id("DiscountPercent")).SendKeys("50");
            Driver.XPathContainsText("h2", "Цена и наличие");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/18");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("50", Driver.FindElement(By.Id("DiscountPercent")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).GetAttribute("class")
                .Contains("active"));

            GoToAdmin("mainpageproducts?type=sale");
            var gridCountEnd = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;
            VerifyIsTrue(gridCountBegin.Equals(gridCountEnd));

            //check client
            GoToClient("products/test-product18");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price"))
                .FindElements(By.CssSelector(".price-number"))[1].Text.Contains("9"));
            VerifyIsTrue(Driver.PageSource.Contains("Скидка 50%"));
            VerifyAreEqual("выгода 9 руб. или 50%",
                Driver.FindElement(By.CssSelector(".details-payment-block .price-discount")).Text);
        }

        [Test]
        public void ProductDiscountEdit()
        {
            //pre check
            GoToAdmin("mainpageproducts?type=sale");
            var gridCountBegin = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;

            GoToClient("products/test-product20");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price"))
                .FindElements(By.CssSelector(".price-number"))[1].Text.Contains("15"));
            VerifyIsTrue(Driver.PageSource.Contains("Скидка 5 руб."));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-block .price-discount")).Text
                .Contains("выгода 5 руб."));

            GoToAdmin("product/edit/20");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("5", Driver.FindElement(By.Id("DiscountAmount")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAmount\"]")).GetAttribute("class")
                .Contains("active"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).Click();

            Driver.FindElement(By.Id("DiscountPercent")).Click();
            Driver.FindElement(By.Id("DiscountPercent")).Clear();
            Driver.FindElement(By.Id("DiscountPercent")).SendKeys("50");
            Driver.XPathContainsText("h2", "Цена и наличие");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/20");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("50", Driver.FindElement(By.Id("DiscountPercent")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountPercent\"]")).GetAttribute("class")
                .Contains("active"));

            GoToAdmin("mainpageproducts?type=sale");
            var gridCountEnd = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;
            VerifyIsTrue(gridCountBegin.Equals(gridCountEnd));

            //check client
            GoToClient("products/test-product20");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price"))
                .FindElements(By.CssSelector(".price-number"))[1].Text.Contains("10"));
            VerifyIsTrue(Driver.PageSource.Contains("Скидка 50%"));
            VerifyAreEqual("выгода 10 руб. или 50%",
                Driver.FindElement(By.CssSelector(".details-payment-block .price-discount")).Text);
        }

        [Test]
        public void ProductEditAllowPreOrder()
        {
            GoToClient("products/test-product101");
            VerifyIsTrue(Driver.PageSource.Contains("Нет в наличии"));
            VerifyIsFalse(Driver.PageSource.Contains("Под заказ"));

            GoToAdmin("product/edit/101");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            if (!Driver.FindElement(By.Id("AllowPreOrder")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AllowPreOrderClick\"]")).Click();

                Driver.ScrollToTop();
                Driver.GetButton(EButtonType.Save).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("product/edit/101");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            //check admin
            VerifyIsTrue(Driver.FindElement(By.Id("AllowPreOrder")).Selected);

            //check client
            GoToClient("products/test-product101");
            VerifyIsTrue(Driver.PageSource.Contains("Нет в наличии"));
            VerifyIsTrue(Driver.PageSource.Contains("Под заказ"));
        }

        [Test]
        public void ProductEditDisallowPreOrder()
        {
            GoToClient("products/test-product17");
            VerifyIsTrue(Driver.PageSource.Contains("Нет в наличии"));
            VerifyIsTrue(Driver.PageSource.Contains("Под заказ"));

            GoToAdmin("product/edit/17");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            if (Driver.FindElement(By.Id("AllowPreOrder")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AllowPreOrderClick\"]")).Click();

                Driver.ScrollToTop();
                Driver.GetButton(EButtonType.Save).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("product/edit/17");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            //check  admin
            VerifyIsFalse(Driver.FindElement(By.Id("AllowPreOrder")).Selected);

            //check client
            GoToClient("products/test-product17");
            VerifyIsTrue(Driver.PageSource.Contains("Нет в наличии"));
            VerifyIsFalse(Driver.PageSource.Contains("Под заказ"));
        }

        [Test]
        public void ProductEditNewPriceAddMain()
        {
            GoToAdmin("product/edit/6");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName6"));
            VerifyIsTrue(Driver.GetGridCell(0, "ColorId", "Offers").Text.Contains("Color6"));
            VerifyIsTrue(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddSize\"]")))).SelectByText("SizeName1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddColor\"]")))).SelectByText("Color1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddArtNo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddArtNo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddArtNo\"]")).SendKeys("666");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).SendKeys("666");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).SendKeys("666");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddSupplyPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddSupplyPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddSupplyPrice\"]")).SendKeys("666");
            Driver.DropFocus("h2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"MainClick\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/6");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(1, "SizeId", "Offers").Text.Contains("SizeName1"));
            VerifyIsTrue(Driver.GetGridCell(1, "ColorId", "Offers").Text.Contains("Color1"));
            VerifyIsTrue(Driver.GetGridCell(1, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyAreEqual("666", Driver.GetGridCell(1, "ArtNo", "Offers").Text);
            VerifyAreEqual("666", Driver.GetGridCell(1, "BasePrice", "Offers").Text);
            VerifyAreEqual("666", Driver.GetGridCell(1, "SupplyPrice", "Offers").Text);
            VerifyAreEqual("666", Driver.GetGridCell(1, "Amount", "Offers").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            //check client
            GoToClient("products/test-product6");
            VerifyIsTrue(Driver.PageSource.Contains("SizeName1"));
            VerifyIsTrue(Driver.PageSource.Contains("SizeName6"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price .price-number"))
                .Text.Contains("666"));
        }

        [Test]
        public void ProductEditNewPriceAddNotMain()
        {
            GoToAdmin("product/edit/5");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName5"));

            //scroll
            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddSize\"]")))).SelectByText("SizeName1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddColor\"]")))).SelectByText("Color1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).SendKeys("555");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).SendKeys("555");
            Driver.DropFocus("h2");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/5");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(1, "SizeId", "Offers").Text.Contains("SizeName1"));
            VerifyIsFalse(Driver.GetGridCell(1, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            //check client
            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.PageSource.Contains("SizeName1"));
            VerifyIsTrue(Driver.PageSource.Contains("SizeName5"));
        }

        [Test]
        public void ProductEditNewPriceAdd()
        {
            GoToAdmin("product/edit/102");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("––––"));
            VerifyIsTrue(Driver.GetGridCell(0, "ColorId", "Offers").Text.Contains("––––"));

            VerifyIsTrue(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddSize\"]")))).SelectByText("SizeName1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddColor\"]")))).SelectByText("Color1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).SendKeys("102102");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).SendKeys("102102");
            Driver.DropFocus("h2");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/102");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.PageSource.Contains("Укажите размер у всех цен!"));

            VerifyIsTrue(Driver.GetGridCell(1, "SizeId", "Offers").Text.Contains("SizeName1"));
            VerifyIsTrue(Driver.GetGridCell(1, "ColorId", "Offers").Text.Contains("Color1"));
            VerifyIsFalse(Driver.GetGridCell(1, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            Driver.MouseFocus(Driver.GetGridCell(0, "SizeId", "Offers"));
            Driver.GetGridCell(0, "SizeId", "Offers").FindElement(By.CssSelector("[data-e2e=\"select\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'SizeName2')]")).Click();

            Driver.MouseFocus(Driver.GetGridCell(0, "ColorId", "Offers"));
            Driver.GetGridCell(0, "ColorId", "Offers").FindElement(By.CssSelector("[data-e2e=\"select\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'Color2')]")).Click();
            //check admin with first price modified
            GoToAdmin("product/edit/102");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName2"));
            VerifyIsTrue(Driver.GetGridCell(0, "ColorId", "Offers").Text.Contains("Color2"));
            VerifyIsTrue(Driver.GetGridCell(1, "SizeId", "Offers").Text.Contains("SizeName1"));
            VerifyIsTrue(Driver.GetGridCell(1, "ColorId", "Offers").Text.Contains("Color1"));

            //check client
            GoToClient("products/test-product102");
            VerifyIsTrue(Driver.PageSource.Contains("SizeName1"));
            VerifyIsTrue(Driver.PageSource.Contains("SizeName2"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".color-viewer-header")).Displayed);
        }

        [Test]
        public void ProductEditPriceDelete()
        {
            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName7"));

            //scroll
            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddSize\"]")))).SelectByText("SizeName1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddColor\"]")))).SelectByText("Color1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).SendKeys("777");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).SendKeys("777");
            Driver.DropFocus("h2");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check delete price
            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(1, "SizeId", "Offers").Text.Contains("SizeName1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count == 2);

            Driver.GetGridCell(1, "_serviceColumn", "Offers").Click();
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName7"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count == 1);

            //check client
            GoToClient("products/test-product7");
            VerifyIsFalse(Driver.PageSource.Contains("SizeName1"));
            VerifyIsTrue(Driver.PageSource.Contains("SizeName7"));
        }

        [Test]
        public void ProductEditPriceDeleteMain()
        {
            GoToAdmin("product/edit/8");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).FindElement(By.TagName("input"))
                .Selected);
            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName8"));

            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddSize\"]")))).SelectByText("SizeName1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddColor\"]")))).SelectByText("Color1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).SendKeys("888");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).SendKeys("888");
            Driver.DropFocus("h2");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName8"));
            VerifyIsFalse(Driver.GetGridCell(1, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            //check delete price
            GoToAdmin("product/edit/8");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Driver.GetGridCell(0, "_serviceColumn", "Offers").Click();
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/8");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count == 1);

            //check client
            GoToClient("products/test-product8");
            VerifyIsTrue(Driver.PageSource.Contains("SizeName1"));
            VerifyIsFalse(Driver.PageSource.Contains("SizeName8"));
        }

        [Test]
        public void ProductEditPriceDeleteAll()
        {
            GoToAdmin("product/edit/9");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddSize\"]")))).SelectByText("SizeName1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddColor\"]")))).SelectByText("Color1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).SendKeys("999");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).SendKeys("999");
            Driver.DropFocus("h2");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/9");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(1, "SizeId", "Offers").Text.Contains("SizeName1"));
            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName9"));

            //check delete price
            GoToAdmin("product/edit/9");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Driver.GetGridCell(1, "_serviceColumn", "Offers").Click();
            Driver.SwalConfirm();
            Driver.GetGridCell(0, "_serviceColumn", "Offers").Click();
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/9");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.PageSource.Contains("У товара нет цен"));
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Enabled);
            VerifyIsTrue(0 == Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count);

            //check client
            GoToClient("products/test-product9");
            VerifyIsFalse(Driver.PageSource.Contains("SizeName1"));
            VerifyIsFalse(Driver.PageSource.Contains("SizeName9"));
        }

        [Test]
        public void ProductEditPriceGoToSizesColors()
        {
            GoToAdmin("product/edit/10");

            Driver.ScrollTo(By.Id("CurrencyId"));
            Driver.FindElement(By.LinkText("размеров")).Click();

            //focus to second browser tab
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            String AdminProduct = (String)windowHandles[0];
            String AdminSizes = windowHandles[windowHandles.Count - 1];
            Driver.SwitchTo().Window(AdminSizes);
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Размеры"));
            VerifyIsTrue(Driver.Url.Contains("sizes"));

            //close tab and focus to first tab
            Driver.Close();
            Driver.SwitchTo().Window(AdminProduct);

            // Driver.ScrollTo(By.Id("CurrencyId"));
            Driver.FindElement(By.LinkText("цветов")).Click();

            //focus to next browser tab
            windowHandles = Driver.WindowHandles;
            String AdminColors = windowHandles[windowHandles.Count - 1];
            Driver.SwitchTo().Window(AdminColors);
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Цвета"));
            VerifyIsTrue(Driver.Url.Contains("colors"));

            //close tab
            Driver.Close();
            Driver.SwitchTo().Window(AdminProduct);
        }

        [Test]
        public void ProductEditPriceChangeMain()
        {
            GoToAdmin("product/edit/11");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName11"));

            //scroll
            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddSize\"]")))).SelectByText("SizeName1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddColor\"]")))).SelectByText("Color1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).SendKeys("1111");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).SendKeys("1111");
            Driver.DropFocus("h2");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/11");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName11"));
            VerifyIsFalse(Driver.GetGridCell(1, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "SizeId", "Offers").Text.Contains("SizeName1"));

            //check change main price 
            Driver.GetGridCellInputForm(1, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/11");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            //check admin
            VerifyIsFalse(Driver.GetGridCell(0, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "Main", "Offers")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            //check client
            GoToClient("products/test-product11");
            VerifyIsTrue(Driver.PageSource.Contains("SizeName1"));
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[title=\"SizeName11\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("disabled"));
        }


        [Test]
        public void ProductEditPriceInplace()
        {
            GoToAdmin("product/edit/12");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName12"));
            VerifyIsTrue(Driver.GetGridCell(0, "ColorId", "Offers").Text.Contains("Color12"));
            VerifyAreEqual("12", Driver.GetGridCell(0, "ArtNo", "Offers").Text);
            VerifyAreEqual("12", Driver.GetGridCell(0, "BasePrice", "Offers").Text);
            VerifyAreEqual("12", Driver.GetGridCell(0, "SupplyPrice", "Offers").Text);
            VerifyAreEqual("12", Driver.GetGridCell(0, "Amount", "Offers").Text);

            //check inplace edit
            Driver.SendKeysGridCell("121212", 0, "ArtNo", "Offers");
            Driver.MouseFocus(Driver.GetGridCell(0, "SizeId", "Offers"));
            Driver.GetGridCell(0, "SizeId", "Offers").FindElement(By.CssSelector("[data-e2e=\"select\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'SizeName5')]")).Click();
            Driver.WaitForToastSuccess();
            Driver.MouseFocus(Driver.GetGridCell(0, "ColorId", "Offers"));
            Driver.GetGridCell(0, "ColorId", "Offers").FindElement(By.CssSelector("[data-e2e=\"select\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'Color5')]")).Click();
            Driver.WaitForToastSuccess();
            Driver.SendKeysGridCell("121212", 0, "BasePrice", "Offers");
            Driver.SendKeysGridCell("", 0, "BasePrice", "Offers");
            Driver.SendKeysGridCell("121212", 0, "SupplyPrice", "Offers");
            Driver.SendKeysGridCell("121212", 0, "Amount", "Offers");

            //check admin
            GoToAdmin("product/edit/12");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName5"));
            VerifyIsTrue(Driver.GetGridCell(0, "ColorId", "Offers").Text.Contains("Color5"));
            VerifyAreEqual("121212", Driver.GetGridCell(0, "ArtNo", "Offers").Text);
            VerifyAreEqual("121212", Driver.GetGridCell(0, "BasePrice", "Offers").Text);
            VerifyAreEqual("121212", Driver.GetGridCell(0, "SupplyPrice", "Offers").Text);
            VerifyAreEqual("121212", Driver.GetGridCell(0, "Amount", "Offers").Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count == 1);

            //check client
            GoToClient("products/test-product12");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-sku")).Text.Contains("121212"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".price")).Text.Contains("121 212"));
            VerifyIsTrue(Driver.PageSource.Contains("SizeName5"));
            VerifyIsTrue(Driver.PageSource.Contains("Color5"));
            VerifyIsFalse(Driver.PageSource.Contains("SizeName12"));
            VerifyIsFalse(Driver.PageSource.Contains("Color12"));
        }

        [Test]
        public void ProductEditPriceAddUnit()
        {
            GoToAdmin("product/edit/13");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);
            
            Driver.FindElement(By.Id("Unit")).Click();
            Driver.FindElement(By.Id("Unit")).Clear();
            Driver.FindElement(By.Id("Unit")).SendKeys("км");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/13");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("км", Driver.FindElement(By.Id("Unit")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product13");
            VerifyIsTrue(Driver.PageSource.Contains("км"));
        }

        [Test]
        public void ProductEditPriceEditUnit()
        {
            GoToAdmin("product/edit/14");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("unit", Driver.FindElement(By.Id("Unit")).GetAttribute("value"));

            Driver.FindElement(By.Id("Unit")).Click();
            Driver.FindElement(By.Id("Unit")).Clear();
            Driver.FindElement(By.Id("Unit")).SendKeys("км");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/14");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("км", Driver.FindElement(By.Id("Unit")).GetAttribute("value"));

            //check client
            VerifyIsTrue(Driver.PageSource.Contains("км"));
        }

        [Test]
        public void ProductEditPriceAddMiMaxAmount()
        {
            GoToAdmin("product/edit/15");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Driver.FindElement(By.Id("MinAmount")).Click();
            Driver.FindElement(By.Id("MinAmount")).Clear();
            Driver.FindElement(By.Id("MinAmount")).SendKeys("10");
            Driver.FindElement(By.Id("MaxAmount")).Click();
            Driver.FindElement(By.Id("MaxAmount")).Clear();
            Driver.FindElement(By.Id("MaxAmount")).SendKeys("15");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/15");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("10", Driver.FindElement(By.Id("MinAmount")).GetAttribute("value"));
            VerifyAreEqual("15", Driver.FindElement(By.Id("MaxAmount")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product15");
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector(".details-spinbox-block input")).GetAttribute("min"));
            VerifyAreEqual("15",
                Driver.FindElement(By.CssSelector(".details-spinbox-block input")).GetAttribute("max"));
        }

        [Test]
        public void ProductEditPriceEditMultiplicity()
        {
            GoToAdmin("product/edit/16");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("1", Driver.FindElement(By.Id("Multiplicity")).GetAttribute("value"));

            Driver.FindElement(By.Id("Multiplicity")).Click();
            Driver.FindElement(By.Id("Multiplicity")).Clear();
            Driver.FindElement(By.Id("Multiplicity")).SendKeys("100");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/16");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();

            VerifyAreEqual("100", Driver.FindElement(By.Id("Multiplicity")).GetAttribute("value"));
        }
    }
}