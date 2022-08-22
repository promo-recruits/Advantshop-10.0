using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Images.columnsWithPicture
{
    [TestFixture]
    public class columnsWithPictureEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "columnsWithPicture";
        private string blockType = "Images";
        private int numberBlock = 1;
        private readonly string blockTitle = "TitleBlock";
        private readonly string blockSubTitle = "SubTitleBlock";
        private readonly string blockPicture = "columnsWithPicturePicture";
        private readonly string blockHeader = "columnsWithPictureHeader";
        private readonly string blockText = "columnsWithPictureText";

        [Test]
        public void InplacePicture()
        {
            TestName = "InplacePicture";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load initial");

            ReInit();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("/areas/landing/images/columns/columns_with_picture.jpg"), "block picture initial");
            pathInit = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".picture-upload-modal")).Displayed, "modal is dysplayed");
            VerifyAreEqual("Обновить изображение",
                Driver.FindElement(By.CssSelector(".picture-upload-modal .modal-header")).Text, "modal is correct");

            //from PC
            UpdateLoadImageDesktop("light.jpg");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src"); //pathPC
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathPC");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC in client");

            //from URL
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();

            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src"),
                "pathPC vs pathURL");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src"); //pathURL
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathURL");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL in client");

            //from Gallery
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();

            UpdateLoadImageGallery();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src"),
                "pathURL vs pathG");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src"); //pathG
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathG");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery in client");

            //lazy-load
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load off");

            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load on");

            VerifyFinally(TestName);
        }

        [Test]
        public void InplaceText()
        {
            TestName = "InplaceText";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 11,
                "all subblocks displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");
            VerifyAreEqual("header1",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[2].GetAttribute("data-name"),
                "header1 subblock");
            VerifyAreEqual("text1",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[3].GetAttribute("data-name"), "text1 subblock");
            VerifyAreEqual("header2",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[4].GetAttribute("data-name"),
                "header2 subblock");
            VerifyAreEqual("text2",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[5].GetAttribute("data-name"), "text2 subblock");
            VerifyAreEqual("picture",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[6].GetAttribute("data-name"),
                "picture subblock");
            VerifyAreEqual("header3",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[7].GetAttribute("data-name"),
                "header3 subblock");
            VerifyAreEqual("text3",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[8].GetAttribute("data-name"), "text3 subblock");
            VerifyAreEqual("header4",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[9].GetAttribute("data-name"),
                "header4 subblock");
            VerifyAreEqual("text4",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[10].GetAttribute("data-name"),
                "text4 subblock");

            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title initial");
            VerifyAreEqual("Реализуй все с Advantshop",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle initial");
            VerifyAreEqual("Трансформер дизайна",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[0].Text,
                "block header1 initial");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (расширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[0].Text,
                "block text1 initial");
            VerifyAreEqual("CRM для интернет-магазина",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[1].Text,
                "block header2 initial");
            VerifyAreEqual("Имманентный подход подразумевает отношение к тексту как к автономной реальности",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[1].Text,
                "block text2 initial");
            VerifyAreEqual("Регистрация SSL",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[2].Text,
                "block header3 initial");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (рас-ширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[2].Text,
                "block text3 initial");
            VerifyAreEqual("Регистрация SSL",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[3].Text,
                "block header4 initial");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (рас-ширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[3].Text,
                "block text4 initial");

            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized"))
                .SendKeys("New Subtitle");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"] .inplace-initialized"))[0].Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"] .inplace-initialized"))[0]
                .SendKeys("New Header1");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))[0].Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))[0]
                .SendKeys("New Text1");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"] .inplace-initialized"))[1].Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"] .inplace-initialized"))[1]
                .SendKeys("New Header2");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))[1].Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))[1]
                .SendKeys("New Text2");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"] .inplace-initialized"))[2].Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"] .inplace-initialized"))[2]
                .SendKeys("New Header3");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))[2].Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))[2]
                .SendKeys("New Text3");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"] .inplace-initialized"))[3].Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"] .inplace-initialized"))[3]
                .SendKeys("New Header4");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))[3].Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))[3]
                .SendKeys("New Text4");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]")).Click();

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle");
            VerifyAreEqual("New Header1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[0].Text, "block header1");
            VerifyAreEqual("New Text1", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[0].Text,
                "block text1");
            VerifyAreEqual("New Header2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[1].Text, "block header2");
            VerifyAreEqual("New Text2", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[1].Text,
                "block text2");
            VerifyAreEqual("New Header3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[2].Text, "block header3");
            VerifyAreEqual("New Text3", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[2].Text,
                "block text3");
            VerifyAreEqual("New Header4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[3].Text, "block header4");
            VerifyAreEqual("New Text4", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[3].Text,
                "block text4");
            Thread.Sleep(300);

            Refresh();

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title after refresh");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle after refresh");
            VerifyAreEqual("New Header1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[0].Text,
                "block header1 after refresh");
            VerifyAreEqual("New Text1", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[0].Text,
                "block text1 after refresh");
            VerifyAreEqual("New Header2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[1].Text,
                "block header2 after refresh");
            VerifyAreEqual("New Text2", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[1].Text,
                "block text2 after refresh");
            VerifyAreEqual("New Header3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[2].Text,
                "block header3 after refresh");
            VerifyAreEqual("New Text3", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[2].Text,
                "block text3 after refresh");
            VerifyAreEqual("New Header4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[3].Text,
                "block header4 after refresh");
            VerifyAreEqual("New Text4", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[3].Text,
                "block text4 after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title in client");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle in client");
            VerifyAreEqual("New Header1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[0].Text,
                "block header1 in client");
            VerifyAreEqual("New Text1", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[0].Text,
                "block text1 in client");
            VerifyAreEqual("New Header2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[1].Text,
                "block header2 in client");
            VerifyAreEqual("New Text2", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[1].Text,
                "block text2 in client");
            VerifyAreEqual("New Header3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[2].Text,
                "block header3 in client");
            VerifyAreEqual("New Text3", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[2].Text,
                "block text3 in client");
            VerifyAreEqual("New Header4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockHeader + "\"]"))[3].Text,
                "block header4 in client");
            VerifyAreEqual("New Text4", Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))[3].Text,
                "block text4 in client");

            VerifyFinally(TestName);
        }
    }
}