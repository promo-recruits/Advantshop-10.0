using System;
using NUnit.Framework;
using OpenQA.Selenium;
using AdvantShop.SeleniumTest.Core;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace AdvantShop.Web.Site.Selenium.Test.Admin.CRM.SocialWidget
{
    [TestFixture]
    public class CRMSocialWidgetTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SocialWidget);
            InitializeService.LoadData(
            "data\\Admin\\CRM\\SocialWidget\\Settings.Settings.csv"

          );

            Init();
        }

        [Test]
        public void SocialWidgetOn()
        {
            testname = "CRMSocialWidgetOn";
            VerifyBegin(testname);

            GoToAdmin("settingssocial");
            TabClick(".nav.nav-collapse-tab.nav-tabs", "socialwidget");

            checkSelected("SocialWidgetIsActive", "SettingsCrmSave");

            VerifyIsTrue(driver.FindElement(By.Id("SocialWidgetIsActive")).Selected, "widget on");

            GoToAdmin("settingscrm");
            TabClick(".nav.nav-collapse-tab.nav-tabs", "socialwidget", "Еще");
            VerifyIsTrue(driver.FindElement(By.Id("SocialWidgetIsActive")).Selected, "widget on after refresh");

            VerifyFinally(testname);
        }
    }
}