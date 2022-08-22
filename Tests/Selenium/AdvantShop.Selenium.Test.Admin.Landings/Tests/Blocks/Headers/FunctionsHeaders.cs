using AdvantShop.Selenium.Core.Domain;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers
{
    public class FunctionsHeaders : BaseSeleniumTest
    {
        #region Menu

        public static void MenuFontSize(IWebDriver driver, int fontsize)
        {
            driver.FindElement(By.Id("modalSettingsMenuFontSize")).Click();
            driver.FindElement(By.Id("modalSettingsMenuFontSize")).Clear();
            driver.FindElement(By.Id("modalSettingsMenuFontSize")).SendKeys(fontsize.ToString());
            Thread.Sleep(500);
        }

        public static void ShowMenuInDesktop(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"ShowMenuOnDesktop\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"ShowMenuOnDesktop\"] span")).Click();
        }

        public static void HiddenMenuInDesktop(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"ShowMenuOnDesktop\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"ShowMenuOnDesktop\"] span")).Click();
        }

        public static void
            moveSliderDistanceBtwnPoints(IWebDriver driver, int dis) //DistanceBetweenPoints - int from (0px) to (50px)
        {
            var builder = new Actions(driver);
            var slider = driver.FindElement(By.CssSelector("[data-e2e=\"MenuSpacing\"] .ngrs-handle-max"));
            builder.ClickAndHold(slider).MoveByOffset(0, 0).MoveByOffset(-1000, 0).Release().Build();
            builder.Perform();
            for (var i = 0; i < dis; i++)
            {
                builder = new Actions(driver);
                slider = driver.FindElement(By.CssSelector("[data-e2e=\"MenuSpacing\"] .ngrs-handle-max"));
                builder.ClickAndHold(slider).MoveByOffset(10, 0).Release().Build();
                Thread.Sleep(500);
                builder.Perform();
            }
        }

        public static void DelMenuItem(IWebDriver driver, int ind)
        {
            driver.FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]"))[ind].Click();
            Thread.Sleep(2000);
            driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            Thread.Sleep(1000);
        }

        public static void DelAllMenuItem(IWebDriver driver)
        {
            while (driver.FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count > 0)
            {
                driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
                Thread.Sleep(500);
                driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
                Thread.Sleep(500);
            }
        }

        public static void AddMenuItem(IWebDriver driver, string name, string href)
        {
            var count = driver.FindElements(By.CssSelector(".block-constructor-menu-list-body [data-e2e=\"ItemDel\"]"))
                .Count;
            driver.FindElement(By.CssSelector("[data-e2e=\"MenuGrid\"] [data-e2e=\"AddNewElem\"]")).Click();
            Thread.Sleep(500);

            driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[" + count + "][\'text\']\"]"))
                .FindElement(By.TagName("input")).Click();
            driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[" + count + "][\'text\']\"]"))
                .FindElement(By.TagName("input")).Clear();
            driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[" + count + "][\'text\']\"]"))
                .FindElement(By.TagName("input")).SendKeys(name);

            driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[" + count + "][\'href\']\"]"))
                .FindElement(By.TagName("input")).Click();
            driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[" + count + "][\'href\']\"]"))
                .FindElement(By.TagName("input")).Clear();
            driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[" + count + "][\'href\']\"]"))
                .FindElement(By.TagName("input")).SendKeys(href);
            Thread.Sleep(500);
        }

        public static void AddSubMenuItem(IWebDriver driver, int row, string name, string href)
        {
            driver.FindElement(By.CssSelector("[data-e2e-row-index=\"" + row + "\"] [data-e2e=\"closedGroups\"]"))
                .Click();
            var count = driver
                .FindElement(By.CssSelector("[data-e2e-grid-group=\"" + row + "\"] [data-e2e=\"SubMenuGrid\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count;
            driver.FindElement(By.CssSelector("[data-e2e=\"SubMenuGrid\"] [data-e2e=\"AddNewElem\"]")).Click();
            Thread.Sleep(500);

            driver.FindElement(By.CssSelector("[data-e2e-grid-group=\"" + row +
                                              "\"] [data-e2e=\"SubMenuGrid\"] [data-e2e-grid-cell=\"grid[" + count +
                                              "]['text']\"] input")).Click();
            driver.FindElement(By.CssSelector("[data-e2e-grid-group=\"" + row +
                                              "\"] [data-e2e=\"SubMenuGrid\"] [data-e2e-grid-cell=\"grid[" + count +
                                              "]['text']\"] input")).Clear();
            driver.FindElement(By.CssSelector("[data-e2e-grid-group=\"" + row +
                                              "\"] [data-e2e=\"SubMenuGrid\"] [data-e2e-grid-cell=\"grid[" + count +
                                              "]['text']\"] input")).SendKeys(name);

            driver.FindElement(By.CssSelector("[data-e2e-grid-group=\"" + row +
                                              "\"] [data-e2e=\"SubMenuGrid\"] [data-e2e-grid-cell=\"grid[" + count +
                                              "]['href']\"] input")).Click();
            driver.FindElement(By.CssSelector("[data-e2e-grid-group=\"" + row +
                                              "\"] [data-e2e=\"SubMenuGrid\"] [data-e2e-grid-cell=\"grid[" + count +
                                              "]['href']\"] input")).Clear();
            driver.FindElement(By.CssSelector("[data-e2e-grid-group=\"" + row +
                                              "\"] [data-e2e=\"SubMenuGrid\"] [data-e2e-grid-cell=\"grid[" + count +
                                              "]['href']\"] input")).SendKeys(href);
            driver.FindElement(By.CssSelector("[data-e2e-row-index=\"" + row + "\"] [data-e2e=\"openedGroups\"]"))
                .Click();
            Thread.Sleep(500);
        }

        #endregion Menu

        #region Contacts

        public static void ShowPhone(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"ShowPhone\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"ShowPhone\"] span")).Click();
        }

        public static void HiddenPhone(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"ShowPhone\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"ShowPhone\"] span")).Click();
        }

        public static void ShowEmail(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"ShowEmail\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"ShowEmail\"] span")).Click();
        }

        public static void HiddenEmail(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"ShowEmail\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"ShowEmail\"] span")).Click();
        }

        public static void ShowVK(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"vk_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"vk_enabled\"] span")).Click();
        }

        public static void HiddenVK(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"vk_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"vk_enabled\"] span")).Click();
        }

        public static void ShowFB(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"fb_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"fb_enabled\"] span")).Click();
        }

        public static void HiddenFB(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"fb_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"fb_enabled\"] span")).Click();
        }

        public static void ShowInstagram(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"instagram_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"instagram_enabled\"] span")).Click();
        }

        public static void HiddenInstagram(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"instagram_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"instagram_enabled\"] span")).Click();
        }

        public static void ShowYouTube(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"youtube_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"youtube_enabled\"] span")).Click();
        }

        public static void HiddenYouTube(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"youtube_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"youtube_enabled\"] span")).Click();
        }

        public static void ShowTwitter(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"twitter_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"twitter_enabled\"] span")).Click();
        }

        public static void HiddenTwitter(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"twitter_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"twitter_enabled\"] span")).Click();
        }

        public static void ShowTelegram(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"telegram_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"telegram_enabled\"] span")).Click();
        }

        public static void HiddenTelegram(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"telegram_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"telegram_enabled\"] span")).Click();
        }

        public static void ShowOdnoklassniki(IWebDriver driver)
        {
            if (!driver.FindElement(By.CssSelector("[data-e2e=\"odnoklassniki_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"odnoklassniki_enabled\"] span")).Click();
        }

        public static void HiddenOdnoklassniki(IWebDriver driver)
        {
            if (driver.FindElement(By.CssSelector("[data-e2e=\"odnoklassniki_enabled\"] input")).Selected)
                driver.FindElement(By.CssSelector("[data-e2e=\"odnoklassniki_enabled\"] span")).Click();
        }

        public static void ShowAllContacts(IWebDriver driver)
        {
            ShowPhone(driver);
            ShowEmail(driver);
            ShowVK(driver);
            ShowFB(driver);
            ShowInstagram(driver);
            ShowYouTube(driver);
            ShowTwitter(driver);
            ShowTelegram(driver);
            ShowOdnoklassniki(driver);
            Thread.Sleep(1000);
        }

        public static void HiddenAllContacts(IWebDriver driver)
        {
            HiddenPhone(driver);
            HiddenEmail(driver);
            HiddenVK(driver);
            HiddenFB(driver);
            HiddenInstagram(driver);
            HiddenYouTube(driver);
            HiddenTwitter(driver);
            HiddenTelegram(driver);
            HiddenOdnoklassniki(driver);
            Thread.Sleep(1000);
        }

        #endregion Contacts
    }
}