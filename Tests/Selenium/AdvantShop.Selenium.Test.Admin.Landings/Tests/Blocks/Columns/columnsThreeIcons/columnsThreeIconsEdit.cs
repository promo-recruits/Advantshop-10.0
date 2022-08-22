using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Columns.columnsThreeIcons
{
    [TestFixture]
    public class columnsThreeIconsEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "columnsThreeIcons";
        private string blockType = "Columns";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "columnsThreeIconsTitle";
        private readonly string blockSubTitle = "columnsThreeIconsSubtitle";
        private readonly string blockIcon = "columnsThreeIconsIcon";
        private readonly string blockColHeader = "columnsThreeIconsHeader";
        private readonly string blockColText = "columnsThreeIconsText";


        [Test]
        public void InplaceHeader()
        {
            TestName = "InplaceHeader";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2,
                "not all subblocks displayed");

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

            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text, "block title initial");
            VerifyAreEqual("Реализуй все с Advantshop",
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
        public void InplaceIcons()
        {
            TestName = "InplaceIcons";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            ReInit();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).Count == 3,
                "all Icons displayed");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img"))[0].GetAttribute("src")
                    .Contains("areas/landing/images/columns/good-review.png"), "Icon initial");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "no svg initial");
            pathInit = Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img"))[0]
                .GetAttribute("src");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            Driver.FindElements(By.CssSelector(".lp-table__body [data-e2e=\"ItemDel\"]"))[2].Click();
            Driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.CssSelector(".lp-table__body [data-e2e=\"ItemDel\"]"))[1].Click();
            Driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            Thread.Sleep(500);
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).Count == 1,
                "1 Icon displayed");

            //from PC
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".picture-upload-modal")).Displayed, "modal is dysplayed");
            VerifyAreEqual("Обновить изображение",
                Driver.FindElement(By.CssSelector(".picture-upload-modal .modal-header")).Text, "modal is correct");
            UpdateLoadImageDesktop("images2.jpg");
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img"))
                .GetAttribute("src"); //pathPC
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathPC");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load initial");

            //from URL
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src"),
                "pathPC vs pathURL");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img"))
                .GetAttribute("src"); //pathURL
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathURL");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL in client");

            //from ImageGallery
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            UpdateLoadImageGallery();
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src"),
                "pathURL vs pathG");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img"))
                .GetAttribute("src"); //pathG
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathG");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery in client");

            //lazy-load
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            OffLazyLoad();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(500);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load off");

            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]"));
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

            //from IconGallery
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            UpdateLoadIconGallery();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "svg on page");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "svg on page after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "svg on page in client");

            //delete
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"deletePicture\"]")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains("/areas/landing/frontend/images/nophoto.jpg"), "nophoto after delete in admin");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "no svg in admin");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains("/areas/landing/frontend/images/nophoto.jpg"), "nophoto after delete after refresh");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "no svg after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).Count == 1,
                "no img in client");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "no svg in client");

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
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]")).Count == 1,
                "all ColHeaders displayed");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Count == 1,
                "all ColTexts displayed");

            VerifyAreEqual("Трансформер дизайна",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[0].Text,
                "1 ColHeader initial");

            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (расширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "1 ColText initial");

            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))
                .SendKeys("New ColHeader 1");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                .SendKeys("New ColText 1");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("blocks-constructor-container")).Click();
            Thread.Sleep(500);

            VerifyAreEqual("New ColHeader 1",
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]")).Text, "block ColHeader 1");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1");

            Refresh();

            VerifyAreEqual("New ColHeader 1",
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]")).Text,
                "block ColHeader 1 after refresh");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1 after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New ColHeader 1",
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]")).Text,
                "block ColHeader 1 in client");
            VerifyAreEqual("New ColText 1",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))
                    .Text, "block ColText 1 in client");

            //inplace new
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            AddNewColumns("New title", "New text");
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]")).Count == 2,
                "new element added");
            VerifyAreEqual("New title",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Text,
                "new element ColHeader correct");
            VerifyAreEqual("New text",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "new element ColText correct");

            Driver.FindElement(By.Id("block_1"))
                .FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Clear();
            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))
                [1].SendKeys("Newest ColHeader 4");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1]
                .Clear();
            Driver.FindElement(By.Id("block_1")).FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1]
                .SendKeys("Newest ColText 4");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("blocks-constructor-container")).Click();
            Thread.Sleep(500);

            VerifyAreEqual("Newest ColHeader 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Text,
                "block Newest ColHeader 4");
            VerifyAreEqual("Newest ColText 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "block Newest ColText 4");

            Refresh();

            VerifyAreEqual("Newest ColHeader 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Text,
                "block Newest ColHeader 4 after refresh");
            VerifyAreEqual("Newest ColText 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "block Newest ColText 4 after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("Newest ColHeader 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Text,
                "block Newest ColHeader 4 in client");
            VerifyAreEqual("Newest ColText 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "block Newest ColText 4 in client");

            VerifyFinally(TestName);
        }
    }
}