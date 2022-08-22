using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Columns.columnsThreeIcons
{
    [TestFixture]
    public class columnsThreeIconsSettingsColumns : LandingsFunctions
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
        private readonly string blockIcon = "columnsThreeIconsIcon";
        private readonly string blockColHeader = "columnsThreeIconsHeader";
        private readonly string blockColText = "columnsThreeIconsText";

        [Test]
        public void Columns()
        {
            TestName = "Columns";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-columns-three__item")).Count == 3,
                "block Columns initial");

            //delete
            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            DelAllColumns();
            VerifyAreEqual("Нет элементов", Driver.FindElement(By.CssSelector(".lp-table__cell")).Text,
                "no elements in grid");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-columns-three__item")).Count == 0,
                "no Columns in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-columns-three__item")).Count == 0,
                "no Columns in client");

            //add
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            AddNewColumns("NewColHeader1", "NewColText1");
            AddNewColumns("NewColHeader2", "NewColText2");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-columns-three__item")).Count == 2,
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

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-columns-three__item")).Count == 2,
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
            TabSelect("tabTextColumns");
            Driver.GetGridCell(0, "title").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "title").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "title").FindElement(By.TagName("input")).SendKeys("NewestColHeader1");
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).SendKeys("NewestColText1");
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
            TabSelect("tabTextColumns");
            DragDrop(0, 1, "gridColumn");
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
        public void ColumnsIcons()
        {
            TestName = "ColumnsIcons";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            DelAllColumns();
            AddNewColumns("ColTitle", "ColText");
            BlockSettingsSave();

            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "no svg on page initial");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColIcon initial");
            pathInit = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load initial");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "svg on page initial in client");

            //from pc
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            Driver.GetGridCell(0, "picture").FindElement(By.TagName("picture-loader-trigger")).Click();
            UpdateLoadImageDesktop("images2.jpg");
            BlockSettingsSave();

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

            //from url
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            Thread.Sleep(500);
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            UpdateImageByUrl(
                "https://api.icons8.com/download/22be815d0465a849eb05744984759928828d1ec3/windows10/PNG/512/Very_Basic/plus-512.png");
            BlockSettingsSave();

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

            //from image gallery
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            UpdateLoadImageGallery();
            BlockSettingsSave();

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

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
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
            TabSelect("tabTextColumns");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            OnLazyLoad();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load on"); //-----

            //from icon gallery
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            UpdateLoadIconGallery();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "upload via IconGallery");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).GetAttribute("src"),
                "pathG vs pathIG");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg"))
                .GetAttribute("src"); //pathIG
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathIG");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "upload via IconGallery after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "upload via IconGallery in client");

            //delete
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabTextColumns");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            DelImageSave();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "no svg on page in admin");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColIcon in admin");

            Refresh();
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "no svg on page after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColIcon after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] svg")).Count == 1,
                "no svg on page in client");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockIcon + "\"] img")).Count == 1,
                "nophoto in ColIcon in client");

            VerifyFinally(TestName);
        }
    }
}