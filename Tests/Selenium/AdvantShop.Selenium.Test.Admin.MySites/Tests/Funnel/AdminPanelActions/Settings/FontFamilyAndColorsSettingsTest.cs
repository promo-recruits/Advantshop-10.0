using System;
using System.Collections.Generic;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.AdminPanelActions.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class FontFamilyAndColorsSettingsTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToClient("lp/testfunnel_2");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckAllFonts()
        {
            APItemSettingsClick();
            Driver.FindElement(By.Id("tabHeaderFonts")).Click();
            Driver.WaitForElem(By.CssSelector(".tab-content-active [data-e2e=\"FontMain\"]"));

            List<string> selectValues = new List<string>();
            SelectElement select = GetSelect(AdvBy.DataE2E("FontMain"));
            foreach (IWebElement option in select.Options)
            {
                selectValues.Add(option.GetAttribute("value"));
            }

            Refresh();
            foreach (string optionValue in selectValues)
            {
                try
                {
                    APItemSettingsClick();
                    Driver.FindElement(By.Id("tabHeaderFonts")).Click();
                    Driver.WaitForElem(By.CssSelector(".tab-content-active [data-e2e=\"FontMain\"]"));
                    ((SelectElement) GetSelect(AdvBy.DataE2E("FontMain"))).SelectByValue(optionValue);
                    SaveModalSettings();
                    VerifyIsTrue(CheckConsoleLog() == null, "font " + optionValue);
                    GoToClient("lp/testfunnel_2?inplace=false");
                    VerifyIsTrue(CheckConsoleLog() == null, "font " + optionValue);
                }
                catch (Exception ex)
                {
                    VerifyAddErrors(ex.Message);
                }
                finally
                {
                    GoToClient("lp/testfunnel_2?inplace=true");
                }
            }
        }

        [Test]
        public void CheckLineHeightNOTCOMPLETE()
        {
            //TODO https://task.advant.me/adminv3/tasks#?modal=23706
            //обычные значения, отрицательные, пограничные, введение невалидной строки и html

            //SetLineHeight("1.5");
            //VerifyIsTrue(false, "need to check it's ok");

            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void CheckLightThemeNOTCOMPLETE()
        {
            //?
            //Для каждой, учитывая Прозрачность и проверяя Предпросмотр и предпросмотр на альтернативном фоне(на всех трех блоках предпросмотра):
            //I.Фон из списка, 
            //II.Указать свой цвет фона
            //III.Альтернативный фон из списка
            //IV.Указать свой цвет альтернативного фона
            //V.Цвет и жирность заголовка
            //VI.Цвет и жирность подзагловка
            //VII.Цвет и жирность текста
            //VIII.Альтернативный цвет текста
            //IX.Цвет ссылки, цвет ссылки при нажатии, цвет ссылки при наведении
            //X.Цвет разделителя. 
            //XI. -- -
            //XII.Кнопка 1: заливка + текст, заливка при наведении + текст при наведеении, заливка при нажатии +цвет при нажатии, цвет границы, жирность текста, толщина границы, радиус скругления.
            //XIII.Кнопка 2: заливка + текст, заливка при наведении + текст при наведеении, заливка при нажатии +цвет при нажатии, цвет границы, жирность текста, толщина границы, радиус скругления.
            //XIV.    -- -
            //XV. + для полей валидацию???

            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void CheckGreyThemeNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void CheckDarkThemeNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        public void SetLineHeight(string lineHeight)
        {
            APItemSettingsClick();
            Driver.FindElement(By.Id("tabHeaderFonts")).Click();
            Driver.WaitForElem(By.CssSelector(".tab-content-active [data-e2e=\"lineHeight\"]"));
            Driver.SendKeysInput(AdvBy.DataE2E("lineHeight"), lineHeight);
            SaveModalSettings();
        }
    }
}