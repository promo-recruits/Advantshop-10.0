using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Schedule.schedule
{
    [TestFixture]
    public class scheduleEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "schedule";
        private string blockType = "Schedule";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "TitleBlock";
        private readonly string blockSubTitle = "SubTitleBlock";
        private readonly string blockImage = "schedulePic";
        private readonly string blockColTitle = "scheduleName";
        private readonly string blockColSubtitle = "schedulePosition";
        private readonly string blockColText = "scheduleText";
        private readonly string blockColTime = "scheduleTime";

        [Test]
        public void InplaceHeader()
        {
            TestName = "InplaceHeader";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            ShowTitle();
            ShowSubTitle();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2, "all subblocks displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");

            VerifyAreEqual("Расписание", Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text,
                "block title initial");
            VerifyAreEqual("Advantshop предлагает широкий выбор специлистов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "block subtitle initial");

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
            Driver.FindElement(By.CssSelector("blocks-constructor-container")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle");

            Refresh();

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title after refresh");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title in client");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle in client");

            VerifyFinally(TestName);
        }

        [Test]
        public void InplaceImages()
        {
            TestName = "InplaceImages";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            ReInit();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).Count == 1,
                "all Image displayed");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/schedule/schedule1.jpg"), "Image initial");
            pathInit = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src");

            //from PC
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockImage + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".picture-upload-modal")).Displayed, "modal is dysplayed");
            VerifyAreEqual("Обновить изображение",
                Driver.FindElement(By.CssSelector(".picture-upload-modal .modal-header")).Text, "modal is correct");
            UpdateLoadImageDesktop("images2.jpg");
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img"))
                .GetAttribute("src"); //pathPC
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathPC");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load initial");

            //from URL
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockImage + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src"),
                "pathPC vs pathURL");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img"))
                .GetAttribute("src"); //pathURL
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathURL");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL in client");

            //from ImageGallery
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockImage + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            UpdateLoadImageGallery();
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src"),
                "pathURL vs pathG");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img"))
                .GetAttribute("src"); //pathG
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathG");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery in client");

            //lazy-load
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockImage + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            OffLazyLoad();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(500);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load off");

            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockImage + "\"]"));
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            OnLazyLoad();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(500);

            ReInitClient();
            GoToClient("lp/test1");
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load on");

            //delete
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockImage + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"deletePicture\"]")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("/areas/landing/frontend/images/nophoto.jpg"), "nophoto after delete in admin");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("/areas/landing/frontend/images/nophoto.jpg"), "nophoto after delete after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).Count == 1,
                "no img in client");

            VerifyFinally(TestName);
        }

        [Test]
        public void InplaceText()
        {
            TestName = "InplaceText";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            //inplace existing
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Count == 1,
                "all ColTitles displayed");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]")).Count == 1,
                "all ColSubTitles displayed");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Count == 1,
                "all ColTexts displayed");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]")).Count == 1,
                "all ColTimes displayed");

            VerifyAreEqual("Дмитрий Чкалов",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "1 ColTitle initial");
            VerifyAreEqual("Руководитель отдела маркетинга",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[0].Text,
                "1 ColSubTitle initial");
            VerifyAreEqual("В какой момент дизайн-команде нужно начинать делать свою дизайн-систему",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "1 ColText initial");
            VerifyAreEqual("09:30 - 10:15",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[0].Text,
                "1 ColTime initial");

            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                .SendKeys("New ColTitle 1");
            Thread.Sleep(500);

            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))
                .SendKeys("New ColSubTitle 1");
            Thread.Sleep(500);

            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                .SendKeys("New ColText 1");
            Thread.Sleep(500);

            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))
                .SendKeys("New ColTime 1");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"schedulePic\"]")).Click();
            Thread.Sleep(500);

            VerifyAreEqual("New ColTitle 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                    .Text, "block ColTitle 1");
            VerifyAreEqual("New ColSubTitle 1",
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]")).Text, "block ColText 1");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1");
            VerifyAreEqual("New ColTime 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))
                    .Text, "block ColText 1");

            Refresh();

            VerifyAreEqual("New ColTitle 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                    .Text, "block ColTitle 1 after refresh");
            VerifyAreEqual("New ColSubTitle 1",
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]")).Text,
                "block ColSubTitle 1 after refresh");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1 after refresh");
            VerifyAreEqual("New ColTime 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))
                    .Text, "block ColTime 1 after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New ColTitle 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))
                    .Text, "block ColTitle 1 in client");
            VerifyAreEqual("New ColSubTitle 1",
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]")).Text,
                "block ColSubTitle 1 in client");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1 in client");
            VerifyAreEqual("New ColTime 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))
                    .Text, "block ColTime 1 in client");

            //inplace new
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabScheduleItems");
            AddNewSchedule("New Name", "New Position", "New Time", "New text");
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Count == 2,
                "new element added");
            VerifyAreEqual("New Name",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "new element ColTitle correct");
            VerifyAreEqual("New Position",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Text,
                "new element ColSubTitle correct");
            VerifyAreEqual("New text",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "new element ColText correct");
            VerifyAreEqual("New Time",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1].Text,
                "new element ColTime correct");

            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1]
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1]
                .SendKeys("Newest ColTitle 4");
            Thread.Sleep(500);

            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Clear();
            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))
                [1].SendKeys("Newest ColSubTitle 4");
            Thread.Sleep(500);

            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1]
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1]
                .SendKeys("Newest ColText 4");
            Thread.Sleep(500);

            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1]
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1]
                .SendKeys("Newest ColTime 4");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"schedulePic\"]")).Click();
            Thread.Sleep(500);

            VerifyAreEqual("Newest ColTitle 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "block Newest ColTitle 4");
            VerifyAreEqual("Newest ColSubTitle 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Text,
                "block Newest ColSubTitle 4");
            VerifyAreEqual("Newest ColText 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "block Newest ColText 4");
            VerifyAreEqual("Newest ColTime 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1].Text,
                "block Newest ColTime 4");

            Refresh();

            VerifyAreEqual("Newest ColTitle 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "block Newest ColTitle 4 after refresh");
            VerifyAreEqual("Newest ColSubTitle 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Text,
                "block Newest ColSubTitle 4 after refresh");
            VerifyAreEqual("Newest ColText 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "block Newest ColText 4 after refresh");
            VerifyAreEqual("Newest ColTime 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1].Text,
                "block Newest ColTime 4 after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("Newest ColTitle 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "block Newest ColTitle 4 in client");
            VerifyAreEqual("Newest ColSubTitle 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Text,
                "block Newest ColSubTitle 4 in client");
            VerifyAreEqual("Newest ColText 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "block Newest ColText 4 in client");
            VerifyAreEqual("Newest ColTime 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1].Text,
                "block Newest ColTime 4 in client");

            VerifyFinally(TestName);
        }
    }
}