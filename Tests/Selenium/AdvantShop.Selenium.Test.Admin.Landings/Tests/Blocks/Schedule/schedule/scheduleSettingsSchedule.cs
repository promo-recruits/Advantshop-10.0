using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Schedule.schedule
{
    [TestFixture]
    public class scheduleSettingsSchedule : LandingsFunctions
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
        private readonly string blockImage = "schedulePic";
        private readonly string blockColTitle = "scheduleName";
        private readonly string blockColSubtitle = "schedulePosition";
        private readonly string blockColText = "scheduleText";
        private readonly string blockColTime = "scheduleTime";

        [Test]
        public void Schedule()
        {
            TestName = "Schedule";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-schedule__item")).Count == 1,
                "block Items initial");

            //delete
            BlockSettingsBtn(numberBlock);
            TabSelect("tabScheduleItems");
            DelAllColumns();
            VerifyAreEqual("Нет элементов", Driver.FindElement(By.CssSelector(".lp-table__cell")).Text,
                "no elements in grid");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-schedule__item")).Count == 0,
                "no Items in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-schedule__item")).Count == 0,
                "no Items in client");

            //add
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabScheduleItems");
            AddNewSchedule("NewColTitle1", "NewColSubtitle1", "NewColTime1", "NewColText1");
            AddNewSchedule("NewColTitle2", "NewColSubtitle2", "NewColTime2", "NewColText2");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-schedule__item")).Count == 2,
                "New Items in admin");
            VerifyAreEqual("NewColTitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in admin");
            VerifyAreEqual("NewColTitle2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in admin");
            VerifyAreEqual("NewColSubtitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[0].Text,
                "ColSubtitle1 in admin");
            VerifyAreEqual("NewColSubtitle2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Text,
                "ColSubtitle2 in admin");
            VerifyAreEqual("NewColTime1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[0].Text,
                "ColTime1 in admin");
            VerifyAreEqual("NewColTime2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1].Text,
                "ColTime2 in admin");
            VerifyAreEqual("NewColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in admin");
            VerifyAreEqual("NewColText2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in admin");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-schedule__item")).Count == 2,
                "New Items in client");
            VerifyAreEqual("NewColTitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in client");
            VerifyAreEqual("NewColTitle2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in client");
            VerifyAreEqual("NewColSubtitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[0].Text,
                "ColSubtitle1 in client");
            VerifyAreEqual("NewColSubtitle2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Text,
                "ColSubtitle2 in client");
            VerifyAreEqual("NewColTime1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[0].Text,
                "ColTime1 in client");
            VerifyAreEqual("NewColTime2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1].Text,
                "ColTime2 in client");
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
            TabSelect("tabScheduleItems");
            Driver.GetGridCell(0, "name").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "name").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "name").FindElement(By.TagName("input")).SendKeys("NewestColTitle1");
            Driver.GetGridCell(0, "position").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "position").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "position").FindElement(By.TagName("input")).SendKeys("NewestColSubtitle1");
            Driver.GetGridCell(0, "time").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "time").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "time").FindElement(By.TagName("input")).SendKeys("NewestColTime1");
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).SendKeys("NewestColText1");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyAreEqual("NewestColTitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "Newest ColTitle1 in admin");
            VerifyAreEqual("NewestColSubtitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[0].Text,
                "Newest ColSubtitle1 in admin");
            VerifyAreEqual("NewestColTime1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[0].Text,
                "Newest ColTime1 in admin");
            VerifyAreEqual("NewestColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "Newest ColText1 in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("NewestColTitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "Newest ColTitle1 in client");
            VerifyAreEqual("NewestColSubtitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[0].Text,
                "Newest ColSubtitle1 in client");
            VerifyAreEqual("NewestColTime1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[0].Text,
                "Newest ColTime1 in client");
            VerifyAreEqual("NewestColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "Newest ColText1 in client");

            //drag`n`drop
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabScheduleItems");
            DragDrop(0, 1, "ScheduleGrid");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyAreEqual("NewColTitle2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in admin after DragDrop");
            VerifyAreEqual("NewestColTitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitler2 in admin after DragDrop");
            VerifyAreEqual("NewColSubtitle2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[0].Text,
                "ColSubtitle1 in admin after DragDrop");
            VerifyAreEqual("NewestColSubtitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Text,
                "ColSubtitle2 in admin after DragDrop");
            VerifyAreEqual("NewColTime2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[0].Text,
                "ColTime1 in admin after DragDrop");
            VerifyAreEqual("NewestColTime1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1].Text,
                "ColTime2 in admin after DragDrop");
            VerifyAreEqual("NewColText2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in admin after DragDrop");
            VerifyAreEqual("NewestColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in admin after DragDrop");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("NewColTitle2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[0].Text,
                "ColTitle1 in client after DragDrop");
            VerifyAreEqual("NewestColTitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]"))[1].Text,
                "ColTitle2 in client after DragDrop");
            VerifyAreEqual("NewColSubtitle2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[0].Text,
                "ColSubtitle1 in client after DragDrop");
            VerifyAreEqual("NewestColSubtitle1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]"))[1].Text,
                "ColSubtitle2 in client after DragDrop");
            VerifyAreEqual("NewColTime2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[0].Text,
                "ColTime1 in client after DragDrop");
            VerifyAreEqual("NewestColTime1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]"))[1].Text,
                "ColTime2 in client after DragDrop");
            VerifyAreEqual("NewColText2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[0].Text,
                "ColText1 in client after DragDrop");
            VerifyAreEqual("NewestColText1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]"))[1].Text,
                "ColText2 in client after DragDrop");

            VerifyFinally(TestName);
        }

        [Test]
        public void ScheduleImages()
        {
            TestName = "ScheduleImages";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabScheduleItems");
            DelAllColumns();
            AddNewSchedule("ColTitle", "ColSubtitle", "ColTime", "ColText");
            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColImage initial");
            pathInit = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load initial");

            //from pc
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabScheduleItems");
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
            TabSelect("tabScheduleItems");
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
            TabSelect("tabScheduleItems");
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
            TabSelect("tabScheduleItems");
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
            TabSelect("tabScheduleItems");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            OnLazyLoad();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load on");

            //delete
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabScheduleItems");
            Driver.MouseFocus(By.CssSelector(".picture-loader-trigger-image-wrap.picture-loader-image-wrap"));
            Driver.FindElements(By.CssSelector(".subblock-inplace-image-trigger"))[1].Click();
            DelImageSave();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColImage in admin");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/nophoto.jpg"), "nophoto in ColImage after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).Count == 1,
                "nophoto in ColImage in client");

            VerifyFinally(TestName);
        }
    }
}