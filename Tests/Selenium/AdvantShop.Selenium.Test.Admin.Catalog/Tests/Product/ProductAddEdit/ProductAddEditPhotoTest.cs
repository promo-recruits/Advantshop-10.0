using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditPhotoTest : BaseSeleniumTest
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
        public void ProductEditPhotoAddByHref()
        {
            GoToClient("products/test-product15");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));

            GoToAdmin("product/edit/15");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("ng-src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".italic.hover-padding-left")).Count > 0);

            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHref\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefLinkText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefLinkText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefLinkText\"]"))
                .SendKeys("http://www.porjati.ru/uploads/posts/2016-03/thumbs/1457852671_1.jpg");
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/15");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"MainPhotoInput\"]"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("ng-src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.PageSource.Contains("Главное фото"));

            //check client
            GoToClient("products/test-product15");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));
        }

        [Test]
        public void ProductEditPhotoAdd()
        {
            GoToClient("products/test-product4");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));

            GoToAdmin("product/edit/4");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("ng-src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".italic.hover-padding-left")).Count > 0);

            AttachFile(By.XPath("(//input[@type='file'])"), GetPicturePath("big.png"));
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/4");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"MainPhotoInput\"]"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("ng-src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoImg\"]")).Text.Contains("Главное фото"));

            //check client
            GoToClient("products/test-product4");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));
        }

        [Test]
        public void ProductEditPhotoAddByPlus()
        {
            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));

            GoToAdmin("product/edit/5");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("ng-src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".italic.hover-padding-left")).Count > 0);

            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("big.png"));
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/5");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"MainPhotoInput\"]"));
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("src").Contains("nophoto"));
            VerifyIsTrue(Driver.PageSource.Contains("Главное фото"));

            //check client
            GoToClient("products/test-product5");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));
        }


        [Test]
        public void ProductEditPhotoAdd360()
        {
            GoToClient("products/test-product6");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-modal-open=\"modalProductRotate\"]")).Count > 0);

            GoToAdmin("product/edit/6");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count > 0);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360CheckBox\"]")).Click();
            Driver.WaitForToastSuccess();
            AttachFile(By.XPath("(//input[@type='file'])[3]"),
                GetPicturePath("pics3d\\1.jpg")); //selenium can't upload miltiple files
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/6");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.Id("productPhotosSortable"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count == 1);

            //check client
            GoToClient("products/test-product6");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-modal-open=\"modalProductRotate\"]")).Displayed);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-modal-open=\"modalProductRotate\"]")).Count == 1);
        }

        [Test]
        public void ProductEditPhotoAddColorFilter()
        {
            GoToClient("products/test-product7");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));

            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться
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

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.Id("productPhotosSortable"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("ng-src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".italic.hover-padding-left")).Count > 0);

            AttachFile(By.XPath("(//input[@type='file'])"), GetPicturePath("big.png"));
            Driver.WaitForToastSuccess();
            Refresh();
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.Id("productPhotosSortable"));
            Thread.Sleep(500);

            AttachFile(By.XPath("(//input[@type='file'])"), GetPicturePath("pictures-1.jpg"));
            Driver.WaitForToastSuccess();
            Refresh();
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.Id("productPhotosSortable"));
            AttachFile(By.XPath("(//input[@type='file'])"), GetPicturePath("brand_logo.jpg"));
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"PhotoImg\"]")).Count == 3);

            IWebElement selectColor1 = Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.TagName("select"))[0];
            SelectElement select1 = new SelectElement(selectColor1);
            VerifyIsTrue(select1.AllSelectedOptions[0].Text.Contains("Нет цвета"));

            IWebElement selectColor2 = Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.TagName("select"))[1];
            SelectElement select2 = new SelectElement(selectColor2);
            VerifyIsTrue(select2.AllSelectedOptions[0].Text.Contains("Нет цвета"));

            IWebElement selectColor3 = Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.TagName("select"))[2];
            SelectElement select3 = new SelectElement(selectColor3);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Нет цвета"));

            (new SelectElement(Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.TagName("select"))[0])).SelectByText("Color7");
            Driver.WaitForToastSuccess();
            (new SelectElement(Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.TagName("select"))[1])).SelectByText("Color1");
            Driver.WaitForToastSuccess();
            //check admin
            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"MainPhotoInput\"]"));
            IWebElement selectColor4 = Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.TagName("select"))[0];
            SelectElement select4 = new SelectElement(selectColor4);
            VerifyIsTrue(select4.AllSelectedOptions[0].Text.Contains("Color7"));

            IWebElement selectColor5 = Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.TagName("select"))[1];
            SelectElement select5 = new SelectElement(selectColor5);
            VerifyIsTrue(select5.AllSelectedOptions[0].Text.Contains("Color1"));

            IWebElement selectColor6 = Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.TagName("select"))[2];
            SelectElement select6 = new SelectElement(selectColor6);
            VerifyIsTrue(select6.AllSelectedOptions[0].Text.Contains("Нет цвета"));

            //check admin photo filter color
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoFilterColor\"]"))))
                .SelectByText("Color7");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"PhotoImg\"]")).Count == 1);

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoFilterColor\"]"))))
                .SelectByText("Color1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"PhotoImg\"]")).Count == 1);

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoFilterColor\"]"))))
                .SelectByText("Нет цвета");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"PhotoImg\"]")).Count == 1);

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoFilterColor\"]"))))
                .SelectByText("Все");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"PhotoImg\"]")).Count == 3);

            //check client
            GoToClient("products/test-product7");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));
        }

        [Test]
        public void ProductEditPhoto360DeleteByCheckBox()
        {
            GoToClient("products/test-product10");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-modal-open=\"modalProductRotate\"]")).Count > 0);

            GoToAdmin("product/edit/10");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count > 0);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgByHref\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360CheckBox\"]")).Click();
            Driver.WaitForToastSuccess();
            AttachFile(By.XPath("(//input[@type='file'])[3]"),
                GetPicturePath("pics3d\\2.jpg")); //selenium can't upload miltiple files
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count == 1);

            GoToAdmin("product/edit/10");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.Id("productPhotosSortable"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgByHref\"]"));
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"Photo360CheckBox\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360CheckBox\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/10");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count > 0);

            //check client
            GoToClient("products/test-product10");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-modal-open=\"modalProductRotate\"]")).Count > 0);
        }

        [Test]
        public void ProductEditPhotoChangeMain()
        {
            GoToAdmin("product/edit/12");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1500);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("ng-src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".italic.hover-padding-left")).Count > 0);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgByHref\"]"));
            AttachFile(By.XPath("(//input[@type='file'])"), GetPicturePath("big.png"));
            Driver.WaitForToastSuccess();
            AttachFile(By.XPath("(//input[@type='file'])"), GetPicturePath("pictures-1.jpg"));
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/12");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"MainPhotoInput\"]"));
            string Img1 = Driver.FindElement(By.Id("leftAsideProduct")).FindElement(By.TagName("img"))
                .GetAttribute("src");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgByHref\"]"));
            Thread.Sleep(500); //костыль, ангуляр в админке не сразу подгружается
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.CssSelector("[data-e2e=\"MainPhotoInput\"]"))[1].Selected);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.CssSelector("[data-e2e=\"MainPhotoInput\"]"))[0].Selected);

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.CssSelector("[data-e2e=\"PhotoImg\"]"))[1]);
            a.Perform();
            Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElements(By.CssSelector("[data-e2e=\"PhotoImg\"]"))[1]
                .FindElement(By.CssSelector("[data-e2e=\"MainPhoto\"]")).Click();

            //check admin
            GoToAdmin("product/edit/12");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"MainPhotoInput\"]"));
            string Img2 = Driver.FindElement(By.Id("leftAsideProduct")).FindElement(By.TagName("img"))
                .GetAttribute("src");
            VerifyIsFalse(Img1.Equals(Img2));
        }

        [Test]
        public void ProductEditPhotoAltAdd()
        {
            GoToAdmin("product/edit/13");

            Driver.ScrollTo(By.Id("Weight"));
            Thread.Sleep(500); //костыль, ангуляр в админке не сразу подгружается
            AttachFile(By.XPath("(//input[@type='file'])"), GetPicturePath("big.png"));
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/13");
            Driver.ScrollTo(By.Id("Weight"));

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElement(By.CssSelector("[data-e2e=\"PhotoImg\"]")));
            a.Perform();
            Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElement(By.CssSelector("[data-e2e=\"PhotoItemEdit\"] a")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"PhotoAltEdit\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoAltEdit\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoAltEdit\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoAltEdit\"]")).SendKeys("TestAltPhoto");
            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check client
            GoToClient("products/test-product13");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("alt")
                .Contains("TestAltPhoto"));
        }

        [Test]
        public void ProductEditPhotoAltEdit()
        {
            //pre check client
            GoToClient("products/test-product14");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("alt")
                .Contains("TestAltPhotoFirst"));

            GoToAdmin("product/edit/14");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElement(By.CssSelector("[data-e2e=\"PhotoImg\"]")));
            a.Perform();
            Driver.FindElement(By.CssSelector(".product-pic-list"))
                .FindElement(By.CssSelector("[data-e2e=\"PhotoItemEdit\"] a")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"PhotoAltEdit\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoAltEdit\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoAltEdit\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoAltEdit\"]")).SendKeys("Edited Alt Text");
            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check client
            GoToClient("products/test-product14");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("alt")
                .Contains("Edited Alt Text"));
        }
    }
}