using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditVideoTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Tag.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Property.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyValue.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductPropertyValue.csv"
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
        public void ProductEditVideoYouTubeAdd()
        {
            GoToClient("products/test-product1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count > 0);

            GoToAdmin("product/edit/1");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            VerifyAreEqual("Видео", Driver.FindElement(By.TagName("h2")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("VideoNameYouTube");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLink\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Displayed);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Count == 0);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]"))
                .SendKeys("https://vimeo.com/293033666");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/1");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();

            VerifyAreEqual("VideoNameYouTube", Driver.GetGridCell(0, "Name", "Videos").Text);
            VerifyAreEqual("10", Driver.GetGridCell(0, "VideoSortOrder", "Videos").Text);

            //check client
            GoToClient("products/test-product1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count == 1);
            VerifyIsTrue(Driver.PageSource.Contains("VideoDescriptoin"));
        }

        [Test]
        public void ProductEditVideoVimeoAdd()
        {
            GoToClient("products/test-product10");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count > 0);

            GoToAdmin("product/edit/10");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            VerifyAreEqual("Видео", Driver.FindElement(By.TagName("h2")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("VideoNameVimeo");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLink\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Displayed);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Count == 0);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]"))
                .SendKeys("https://vimeo.com/293033666");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/10");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться

            VerifyAreEqual("VideoNameVimeo", Driver.GetGridCell(0, "Name", "Videos").Text);
            VerifyAreEqual("10", Driver.GetGridCell(0, "VideoSortOrder", "Videos").Text);

            //check client
            GoToClient("products/test-product10");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count == 1);
            VerifyIsTrue(Driver.PageSource.Contains("VideoDescriptoin"));
        }

        [Test]
        public void ProductEditVideoCodeAdd()
        {
            GoToClient("products/test-product4");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count > 0);

            GoToAdmin("product/edit/4");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("VideoNameCode");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCode\"]")).Click();
            VerifyIsFalse(Driver.PageSource.Contains("Cсылка на видео"));
            VerifyIsTrue(Driver.PageSource.Contains("Код плеера"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).SendKeys("LetItBeCode");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/4");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться

            VerifyAreEqual("VideoNameCode", Driver.GetGridCell(0, "Name", "Videos").Text);
            VerifyAreEqual("10", Driver.GetGridCell(0, "VideoSortOrder", "Videos").Text);

            //check client
            GoToClient("products/test-product4");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count == 1);
            VerifyIsTrue(Driver.PageSource.Contains("VideoDescriptoin"));
        }

        [Test]
        public void ProductEditVideoEdit()
        {
            GoToAdmin("product/edit/5");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("VideoNameYouTube");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLink\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]"))
                .SendKeys("https://www.youtube.com/watch?v=N7sGh4KKDg8");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/5");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.GetGridCell(0, "_serviceColumn", "Videos")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("EditName");

            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)",
                Driver.FindElement(By.CssSelector(".modal-content"))
                    .FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")));
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescEdited");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("50");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/5");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            VerifyAreEqual("EditName", Driver.GetGridCell(0, "Name", "Videos").Text);
            VerifyAreEqual("50", Driver.GetGridCell(0, "VideoSortOrder", "Videos").Text);

            //check client
            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count == 1);
            VerifyIsTrue(Driver.PageSource.Contains("VideoDescEdited"));
        }

        [Test]
        public void ProductEditVideoEditInplace()
        {
            GoToAdmin("product/edit/6");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("VideoNameCheckInplace");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLink\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]"))
                .SendKeys("https://vimeo.com/293033666");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/6");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(1000);//костыль, иначе видео не успевает подгружаться
            Thread.Sleep(1000);//костыль, иначе видео не успевает подгружаться 
            //Да, специально 2 слипа, иначе падало.
            Driver.SendKeysGridCell("EditNameInplace", 0, "Name", "Videos");
            Driver.SendKeysGridCell("999", 0, "VideoSortOrder", "Videos");

            Driver.FindElement(By.XPath("//h2[contains(text(), 'Видео')]")).Click();

            //check admin
            GoToAdmin("product/edit/6");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            VerifyAreEqual("EditNameInplace", Driver.GetGridCell(0, "Name", "Videos").Text);
            VerifyAreEqual("999", Driver.GetGridCell(0, "VideoSortOrder", "Videos").Text);

            //check client
            GoToClient("products/test-product6");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count == 1);
            VerifyIsTrue(Driver.PageSource.Contains("VideoDescriptoin"));
        }

        [Test]
        public void ProductEditVideoAddSeveral()
        {
            GoToClient("products/test-product7");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count > 0);

            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(1500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("Video1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCode\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).SendKeys("LetItBeCode");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("Video2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLink\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]"))
                .SendKeys("https://www.youtube.com/watch?v=N7sGh4KKDg8");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("20");
            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/7");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться

            VerifyAreEqual("Video1", Driver.GetGridCell(0, "Name", "Videos").Text);
            VerifyAreEqual("10", Driver.GetGridCell(0, "VideoSortOrder", "Videos").Text);
            VerifyAreEqual("Video2", Driver.GetGridCell(1, "Name", "Videos").Text);
            VerifyAreEqual("20", Driver.GetGridCell(1, "VideoSortOrder", "Videos").Text);

            //check client
            GoToClient("products/test-product7");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count == 1);
            VerifyIsTrue(Driver.PageSource.Contains("VideoDescriptoin1"));
            VerifyIsTrue(Driver.PageSource.Contains("VideoDescriptoin2"));
        }

        [Test]
        public void ProductEditVideoDeleteFromSeveral()
        {
            GoToClient("products/test-product8");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count > 0);

            GoToAdmin("product/edit/8");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(2000);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("Video1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCode\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).SendKeys("LetItBeCode");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("Video2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLink\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoLinkAdd\"]"))
                .SendKeys("https://www.youtube.com/watch?v=N7sGh4KKDg8");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("20");
            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/8");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.GetGridCell(1, "_serviceColumn", "Videos").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/8");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться

            VerifyAreEqual("Video1", Driver.GetGridCell(0, "Name", "Videos").Text);
            VerifyAreEqual("10", Driver.GetGridCell(0, "VideoSortOrder", "Videos").Text);
            VerifyIsFalse(Driver.PageSource.Contains("Video2"));

            //check client
            GoToClient("products/test-product8");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count == 1);
            VerifyIsTrue(Driver.PageSource.Contains("VideoDescriptoin1"));
            VerifyIsFalse(Driver.PageSource.Contains("VideoDescriptoin2"));
        }


        [Test]
        public void ProductEditVideoDelete()
        {
            GoToClient("products/test-product9");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count > 0);

            GoToAdmin("product/edit/9");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoName\"]")).SendKeys("Video1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCode\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoCodeAdd\"]")).SendKeys("LetItBeCode");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoDesc\"]")).SendKeys("VideoDescriptoin1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductVideoSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Video1", Driver.GetGridCell(0, "Name", "Videos").Text);

            GoToAdmin("product/edit/9");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться
            Driver.GetGridCell(0, "_serviceColumn", "Videos").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/9");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Видео')]")).Click();
            Thread.Sleep(500);//костыль, иначе видео не успевает подгружаться

            VerifyIsFalse(Driver.PageSource.Contains("Video1"));

            //check client
            GoToClient("products/test-product9");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-photos__trigger-video")).Count == 1);
            VerifyIsFalse(Driver.PageSource.Contains("VideoDescriptoin1"));
        }
    }
}