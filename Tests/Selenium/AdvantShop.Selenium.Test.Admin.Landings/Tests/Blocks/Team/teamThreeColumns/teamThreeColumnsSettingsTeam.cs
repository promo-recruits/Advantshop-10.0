using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Team.teamThreeColumns
{
    [TestFixture]
    public class teamThreeColumnsSettingsTeam : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "teamThreeColumns";
        private string blockType = "Team";
        private readonly int numberBlock = 1;
        private readonly string blockImage = "teamThreeColumnsPic";
        private readonly string blockColHeader = "teamThreeColumnsHeader";
        private readonly string blockColText = "teamThreeColumnsText";

        [Test]
        public void Team()
        {
            TestName = "Team";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-team-three-columns__row")).Count == 3,
                "block Columns initial");

            //delete
            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            DelAllColumns();
            VerifyAreEqual("Нет элементов", Driver.FindElement(By.CssSelector(".lp-table__cell")).Text,
                "no elements in grid");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-team-three-columns__row")).Count == 0,
                "no Columns in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-team-three-columns__row")).Count == 0,
                "no Columns in client");

            //add
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            AddNewTeam("NewColHeader1", "NewColText1");
            AddNewTeam("NewColHeader2", "NewColText2");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-team-three-columns__row")).Count == 2,
                "New Columns in admin");
            VerifyAreEqual("NewColHeader1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[0].Text,
                "ColHeader1 in admin");
            VerifyAreEqual("NewColHeader2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Text,
                "ColHeader2 in admin");
            VerifyAreEqual("NewColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in admin");
            VerifyAreEqual("NewColText2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in admin");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-team-three-columns__row")).Count == 2,
                "New Columns in client");
            VerifyAreEqual("NewColHeader1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[0].Text,
                "ColHeader1 in client");
            VerifyAreEqual("NewColHeader2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Text,
                "ColHeader2 in client");
            VerifyAreEqual("NewColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in client");
            VerifyAreEqual("NewColText2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in client");

            //change
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            Driver.GetGridCell(0, "name").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "name").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "name").FindElement(By.TagName("input")).SendKeys("NewestColHeader1");
            Driver.GetGridCell(0, "position").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "position").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "position").FindElement(By.TagName("input")).SendKeys("NewestColText1");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyAreEqual("NewestColHeader1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[0].Text,
                "Newest ColHeader1 in admin");
            VerifyAreEqual("NewestColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "Newest ColText1 in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("NewestColHeader1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[0].Text,
                "Newest ColHeader1 in client");
            VerifyAreEqual("NewestColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "Newest ColText1 in client");

            //drag`n`drop
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            DragDrop(0, 1, "gridTeam");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyAreEqual("NewColHeader2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[0].Text,
                "ColHeader1 in admin after DragDrop");
            VerifyAreEqual("NewestColHeader1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Text,
                "ColHeader2 in admin after DragDrop");
            VerifyAreEqual("NewColText2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in admin after DragDrop");
            VerifyAreEqual("NewestColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in admin after DragDrop");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("NewColHeader2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[0].Text,
                "ColHeader1 in client after DragDrop");
            VerifyAreEqual("NewestColHeader1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColHeader + "\"]"))[1].Text,
                "ColHeader2 in client after DragDrop");
            VerifyAreEqual("NewColText2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in client after DragDrop");
            VerifyAreEqual("NewestColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in client after DragDrop");

            VerifyFinally(TestName);
        }

        [Test]
        public void TeamIcons()
        {
            TestName = "TeamIcons";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            DelAllColumns();
            AddNewTeam("ColTitle", "ColText");
            BlockSettingsSave();

            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] svg")).Count == 1,
                "no svg on page initial");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColIcon initial");
            pathInit = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load initial");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] svg")).Count == 1,
                "svg on page initial in client");

            //from pc
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            Driver.GetGridCell(0, "picture").FindElement(By.TagName("picture-loader-trigger")).Click();
            UpdateLoadImageDesktop("images2.jpg");
            BlockSettingsSave();

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

            //from url
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            Thread.Sleep(500);
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            UpdateImageByUrl(
                "https://api.icons8.com/download/22be815d0465a849eb05744984759928828d1ec3/windows10/PNG/512/Very_Basic/plus-512.png");
            BlockSettingsSave();

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

            //from image gallery
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            UpdateLoadImageGallery();
            BlockSettingsSave();

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

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            OffLazyLoad();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load off");

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            OnLazyLoad();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load on"); //-----

            //delete
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTeam");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            DelImageSave();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] svg")).Count == 1,
                "no svg on page in admin");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColIcon in admin");

            Refresh();
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] svg")).Count == 1,
                "no svg on page after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColIcon after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] svg")).Count == 1,
                "no svg on page in client");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).Count == 1,
                "nophoto in ColIcon in client");

            VerifyFinally(TestName);
        }
    }
}