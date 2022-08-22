using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests
{
    public class MySitesFunctions : BaseSeleniumTest
    {
        //в CMS оч много тестов
        public void SetAdminStartPage(string optionSelector)
        {
            Driver.Navigate().GoToUrl(BaseUrl + "adminv3/settingssystem");
            (new SelectElement(Driver.FindElement(By.Id("AdminStartPage")))).SelectByValue(optionSelector);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
            Thread.Sleep(1000);
        }

        public void GoToStoreSettings(string page = "", string subPage = "")
        {
            GoToAdmin("dashboard");
            Driver.FindElements(By.ClassName("dashboard-site__block"))[0].FindElement(By.ClassName("btn-submit"))
                .Click();
            Driver.WaitForAjax();
            if (!String.IsNullOrEmpty(page))
            {
                Functions.TabSubPageClick(Driver, page);
                VerifyAreEqual(1,
                    Driver.FindElements(By.CssSelector(".nav-tabs--adaptive-special-store .uib-tab.active")).Count,
                    "active tabs count");
                VerifyIsTrue(
                    Driver.FindElement(By.CssSelector(".nav-tabs--adaptive-special-store .uib-tab.active")).Text
                        .IndexOf(page) != -1, "active tab text");

                if (!String.IsNullOrEmpty(subPage))
                {
                    Driver.FindElement(By.LinkText(subPage)).Click();
                }
            }
        }

        public void SaveTemplateSettings()
        {
            Driver.ScrollToTop();
            if (Driver.FindElements(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"][disabled]")).Count == 0)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForAjax();
            }
        }

        public void SaveMobileSettings()
        {
            Driver.ScrollToTop();
            if (Driver.FindElements(By.CssSelector("[data-e2e=\"mobileSave\"][disabled]")).Count == 0)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
                Driver.WaitForAjax();
            }
        }

        public void SaveStaticPageSettings()
        {
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForAjax();
        }

        public void SaveCommonSettings()
        {
            Driver.ScrollToTop();
            Driver.GetByE2E("btnSave").FindElement(By.ClassName("btn-success")).Click();
            Driver.WaitForAjax();
        }

        public void SaveFunnelSettings(By waitElemSelector = null)
        {
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("funnelSettingSave")).Click();
            Driver.WaitForElem(waitElemSelector != null ? waitElemSelector : By.ClassName("toast-success"));
        }

        public void SaveModalSettings()
        {
            Driver.FindElement(By.CssSelector(".adv-modal-active .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(100);
        }

        public void SetTemplate(string templateName)
        {
            while (Driver.FindElements(By.CssSelector("[data-e2e=\"template." + templateName + "\"] .btn-default")).Count < 1)
                Driver.ScrollTo(By.ClassName("site-footer"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"template.Fly\"]"));

            Driver.GetByE2E("template." + templateName + "").FindElement(By.LinkText("Подробнее")).Click();
            Driver.WaitForElem(By.ClassName("page-name-block-text"));

        }

        public bool DeleteSite(int sitesCount = 1, int deletedIndex = 0)
        {
            GoToAdmin("dashboard");
            Driver.WaitForElem(By.ClassName("dashboard-current-site__name"));
            Driver.FindElement(By.ClassName("btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("funnel-page__name"));
            VerifyAreEqual("Дизайн", Driver.FindElement(By.CssSelector("h2.sticky-page-name-text")).Text,
                "design page subheader");
            VerifyAreEqual("Мой магазин", Driver.FindElement(By.ClassName("funnel-page__name")).Text,
                "design page header");
            Driver.FindElement(By.ClassName("breadcrumb__link--admin")).Click();
            Driver.WaitForElem(By.ClassName("dashboard-current-site__name"));

            Driver.FindElements(By.LinkText("Удалить"))[deletedIndex].Click();
            Driver.SwalConfirm();
            if (sitesCount == 1)
            {
                return BaseUrl + "adminv3/dashboard" == Driver.Url;
            }
            else
            {
                return Driver.FindElements(By.ClassName("dashboard-site__block")).Count == sitesCount - 1;
            }
        }

        public bool DeleteFunnel(string funnelName, string funnelUrl, int sitesCount = 1, int deletedIndex = 0,
            bool newDashboard = false)
        {
            GoToAdmin("dashboard");
            Driver.WaitForElem(By.ClassName("dashboard-current-site__name"));
            VerifyIsNull(CheckConsoleLog(), "dashboard page log");

            VerifyAreEqual(funnelName, Driver.FindElement(By.CssSelector(".dashboard-site-name a")).Text,
                "funnel name in dashboard");
            VerifyAreEqual("Опубликован", Driver.FindElement(By.CssSelector(".dashboard-site-name button")).Text,
                "funnel enabled in dashboard");
            VerifyAreEqual("Тип сайта:  \r\nВоронка продаж",
                Driver.FindElement(By.ClassName("dashboard-site--secondary-text")).Text, "funnel type in dashboard");
            VerifyAreEqual(BaseUrl + "lp/" + funnelUrl,
                Driver.FindElement(By.CssSelector(".dashboard-site--secondary-text + a")).Text,
                "funnel url in dashboard");

            Driver.FindElement(By.ClassName("btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("funnel-page__name"));
            VerifyIsNull(CheckConsoleLog(), "funnel page log");

            if (newDashboard)
            {
                VerifyAreEqual("Воронки: \r\n" + funnelName, Driver.FindElement(By.TagName("h1")).Text,
                    "funnle page header");
                Driver.FindElement(By.ClassName("breadcrumb__link--admin")).Click();
            }
            else
            {
                GoToAdmin("dashboard");
            }

            Driver.WaitForElem(By.ClassName("dashboard-current-site__name"));

            Driver.FindElements(By.LinkText("Удалить"))[deletedIndex].Click();
            Driver.SwalConfirm();
            if (sitesCount == 1)
            {
                return BaseUrl + "adminv3/dashboard" == Driver.Url;
            }
            else
            {
                return Driver.FindElements(By.ClassName("dashboard-site__block")).Count == sitesCount - 1;
            }
        }

        public void SelectItem(string selectId, string optionSelector, bool selectByText = true)
        {
            if (selectByText)
            {
                GetSelect(By.Id(selectId)).SelectByText(optionSelector);
            }
            else
            {
                GetSelect(By.Id(selectId)).SelectByValue(optionSelector);
            }

            Thread.Sleep(100);
        }

        public void SelectItem(By selector, string optionSelector, bool selectByText = true)
        {
            if (selectByText)
            {
                GetSelect(selector).SelectByText(optionSelector);
            }
            else
            {
                GetSelect(selector).SelectByValue(optionSelector);
            }

            Thread.Sleep(100);
        }

        public SelectElement GetSelect(By cssSelector)
        {
            return new SelectElement(Driver.FindElement(cssSelector));
        }

        public string GetSelectedOptionText(string selectId)
        {
            return GetSelect(By.Id(selectId)).SelectedOption.Text.Trim();
        }

        public string GetInputValue(string inputName)
        {
            return Driver.FindElement(By.Name(inputName)).GetAttribute("value");
        }

        public bool GetCheckboxState(string checkboxName)
        {
            return Driver.FindElement(By.Name(checkboxName)).GetAttribute("class").IndexOf("ng-not-empty") != -1;
        }

        public bool GetCheckboxState(IWebElement checkbox)
        {
            return checkbox.GetAttribute("class").IndexOf("ng-not-empty") != -1;
        }

        public void AddCarouselItem(string picturePath)
        {
            Driver.GetByE2E("btnAdd").Click();
            Thread.Sleep(1000);
            if (picturePath.IndexOf("http://") != -1 || picturePath.IndexOf("https://") != -1)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHref\"]")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefLinkText\"]")).SendKeys(picturePath);
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();
                Thread.Sleep(500);
            }
            else
            {
                AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath(picturePath));
            }

            Driver.GetByE2E("carouselAdd").Click();
            Thread.Sleep(500);
        }

        //удалить?
        public void ClearCarouselItems()
        {
            GoToStoreSettings("Карусель");
            while (Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count > 0)
            {
                Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("a")).Click();
                Driver.SwalConfirm();
            }
        }

        public void ClearGridItems()
        {
            Driver.FindElement(AdvBy.DataE2E("gridHeaderCheckboxWrapSelectAll")).Click();
            Driver.FindElement(AdvBy.DataE2E("gridSelectionDropdownButton")).Click();
            Driver.FindElements(AdvBy.DataE2E("gridSelectionDropdownItem"))[0].Click();
            Driver.SwalConfirm();
        }

        public void SetImg(string imgName, string inputSelector, bool expectedSuccess = true)
        {
            try
            {
                AttachFile(By.XPath($"(//input[@type='file'][@data-e2e='imgAdd{inputSelector}'])"),
                    GetPicturePath(imgName));

                Thread.Sleep(500);
                if (expectedSuccess)
                {
                    Driver.FindElement(By.CssSelector(".toast-success .toast-close-button")).Click();
                }
                else
                {
                    VerifyIsTrue(
                        Driver.FindElement(By.ClassName("toast-message")).Text.IndexOf("Неверный формат") != -1,
                        "error message");
                    Driver.FindElement(By.CssSelector(".toast-error .toast-close-button")).Click();
                }

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                VerifyIsTrue(false, ex.Message);
                Refresh();
            }
        }

        public void SetImgByHref(string parentSelector, string urlPath, bool expectedSuccess = true)
        {
            try
            {
                Driver.FindElement(By.CssSelector(parentSelector + " [data-e2e=\"imgByHref\"]")).Click();
                Thread.Sleep(500);
                Driver.GetByE2E("imgByHrefLinkText").SendKeys(urlPath);
                Thread.Sleep(500);
                Driver.GetByE2E("imgByHrefBtnSave").Click();
                Thread.Sleep(500);
                if (expectedSuccess)
                {
                    Driver.FindElement(By.CssSelector(".toast-success .toast-close-button")).Click();
                }
                else
                {
                    VerifyIsTrue(
                        Driver.FindElement(By.ClassName("toast-message")).Text
                            .IndexOf("Некорректный формат изображения") != -1, "error message");
                    Driver.FindElement(By.CssSelector(".toast-error .toast-close-button")).Click();
                }

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                VerifyIsTrue(false, ex.Message);
                Refresh();
            }
        }

        public string GetIpzoneCookie(int index)
        {
            var cookieValues = HttpUtility.UrlDecode(Driver.Manage().Cookies.GetCookieNamed("ipzone").Value)
                .Split(new[] {';'}, StringSplitOptions.None);
            return cookieValues[index];
        }

        public void CheckStoreUrlInGiftCertificate(string expectedUrl)
        {
            string certificateCode = DateTime.Now.ToString();
            GoToAdmin("settingscoupons#?couponsTab=certificates");
            Driver.GetByE2E("AddCertificates").Click();
            Driver.WaitForModal();
            Driver.GetByE2E("CertCode").SendKeys(certificateCode);
            Driver.GetByE2E("CertSum").SendKeys("1000");
            Driver.GetByE2E("CertMailTo").SendKeys("testmailimap@yandex.ru");
            Driver.GetByE2E("CertMailFrom").SendKeys("testmailimap@yandex.ru");
            Driver.GetByE2E("CertSave").Click();
            Driver.WaitForAjax();
            Driver.GetGridCell(0, "_serviceColumn", "Certificates").FindElement(By.ClassName("fa-pencil-alt")).Click();
            Driver.WaitForModal();
            Driver.GetByE2E("CertPaid").FindElement(By.ClassName("adv-checkbox-emul")).Click();
            Driver.GetByE2E("CertSave").Click();
            Driver.WaitForAjax();

            Driver.Navigate().GoToUrl("https://mail.yandex.ru/");
            VerifyAreEqual("Подарочный сертификат",
                Driver.FindElement(By.ClassName("mail-MessageSnippet-Item_subject")).Text,
                $"last letter subject ({certificateCode})");
            try
            {
                Functions.CloseYandexMailPopups(Driver);
                Driver.FindElements(By.ClassName("mail-MessageSnippet-Item_subjectWrapper"))[0]
                    .FindElement(By.ClassName("mail-MessageSnippet-Item_threadExpand")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".mail-MessageSnippet-Thread .mail-MessageSnippet-Item_body"))
                    .Click();
            }
            catch (Exception ex)
            {
                Driver.FindElement(By.ClassName("mail-MessageSnippet-Item_subjectWrapper")).Click();
            }

            Driver.WaitForElem(By.CssSelector(".mail-Message-Body-Content tbody"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".mail-Message-Body-Content tbody")).Text.IndexOf(certificateCode) !=
                -1, $"letter is expected ({certificateCode})");
            VerifyAreEqual(expectedUrl + "/", Driver.FindElement(By.CssSelector(".mail-Message-Body-Content a")).Text,
                $"shop url in letter ({certificateCode})");
        }

        public string GetCurrentCity(bool inMobile = false)
        {
            if (inMobile)
            {
                Driver.FindElement(By.ClassName("mobile-header__menu-triger")).Click();
                Thread.Sleep(500);
            }

            return Driver.FindElement(By.CssSelector("[data-zone-dialog-trigger] span")).Text;
        }

        public void CheckPhoneMobile(string[] cities, string mobilePhone, string subMessage)
        {
            GoToMobile();
            foreach (string city in cities)
            {
                Functions.SetCity(Driver, city, true);
                VerifyAreEqual("tel:" + mobilePhone,
                    Driver.FindElement(By.ClassName("mobile-header__phone-link")).GetAttribute("href"),
                    $"{subMessage} phone mobile phone - {city}");
            }

            ReInit();
        }

        public void CheckSamePhones(string[] cities, string phone, string phoneHtml, string mobilePhone,
            string subMessage)
        {
            GoToClient();
            foreach (string city in cities)
            {
                Functions.SetCity(Driver, city);
                VerifyAreEqual(phone, Driver.FindElement(By.ClassName("site-head-phone")).Text,
                    $"{subMessage} phone text - {city}");
                VerifyAreEqual(phoneHtml, Driver.FindElement(By.ClassName("site-head-phone")).GetAttribute("innerHTML"),
                    $"{subMessage} phone html - {city}");
            }

            CheckPhoneMobile(cities, mobilePhone, subMessage);
        }

        public void CheckDifferentPhones(string[,] date, string subMessage)
        {
            GoToClient();
            for (int i = 0; i < date.Length / 3; i++)
            {
                Functions.SetCity(Driver, date[i, 0]);
                VerifyAreEqual(date[i, 1], Driver.FindElement(By.ClassName("site-head-phone")).Text,
                    $"{subMessage} phone text - {date[i, 0]}");
                //VerifyAreEqual(date[i, 1], driver.FindElement(By.ClassName("site-head-phone")).GetAttribute("innerHTML"), $"{subMessage} phone html - {date[i, 0]}");
            }

            GoToMobile();
            for (int i = 0; i < date.Length / 3; i++)
            {
                Functions.SetCity(Driver, date[i, 0], true);
                VerifyAreEqual("tel:" + date[i, 2],
                    Driver.FindElement(By.ClassName("mobile-header__phone-link")).GetAttribute("href"),
                    $"{subMessage} phone mobile phone - {date[i, 0]}");
            }

            ReInit();
        }

        public void CheckToastErrors(List<String> error_messages, string additional_message = "")
        {
            Driver.WaitForElem(By.ClassName("toast-error"));
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count,
                additional_message + " - toast errors count");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Не заполнены поля"),
                additional_message + " - toast error contain default message");
            VerifyAreEqual(2, Driver.FindElements(By.TagName("validation-list-item")).Count,
                additional_message + " - count of not valid fields");
            for (int i = 0; i < error_messages.Count; i++)
            {
                VerifyAreEqual(error_messages[i], Driver.FindElements(By.TagName("validation-list-item"))[i].Text,
                    additional_message + "not valid item (" + i + ")");
            }

            Driver.FindElement(By.ClassName("toast-close-button")).Click();
        }

        public void AddMenuItem(string btnAddItemE2e, string name, string linkTypeE2e, string url,
            string root = "Корень", bool openNewTab = false, bool enabled = true, bool noFollow = false,
            int showMode = 0)
        {
            Driver.FindElement(AdvBy.DataE2E(btnAddItemE2e)).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(AdvBy.DataE2E("MenuItemName"), name);
            Driver.FindElement(AdvBy.DataE2E(linkTypeE2e)).Click();
            Driver.SendKeysInput(AdvBy.DataE2E("MenuItemURL"), url);
            if (root != "Корень")
            {
                Driver.FindElement(By.ClassName("edit")).Click();
                Driver.WaitForAjax();
                Driver.GridFilterSendKeys(root);
                Driver.WaitForAjax();
                Driver.FindElements(AdvBy.DataE2E("gridRow"))[0].FindElement(By.TagName("a")).Click();
            }

            if (openNewTab)
            {
                Driver.FindElement(AdvBy.DataE2E("MenuItemBlank")).Click();
            }

            if (!enabled)
            {
                Driver.FindElement(AdvBy.DataE2E("MenuItemEnabled")).Click();
            }

            if (noFollow)
            {
                Driver.FindElement(AdvBy.DataE2E("MenuItemNoFollow")).Click();
            }

            if (showMode != 0)
            {
                SelectItem(AdvBy.DataE2E("MenuItemShowMode"), showMode.ToString(), false);
            }

            Driver.FindElement(By.CssSelector(".modal-footer .btn-primary")).Click();
            Driver.WaitForAjax();
        }


        #region Funnels

        protected void SetFunnelProducts(string productMain, string productUpsell, string productDownsell)
        {
            Driver.FindElement(By.ClassName("product-block-content")).Click();
            Driver.WaitForElem(By.ClassName("ui-grid-custom-wrapper"));
            Driver.GridFilterSendKeys(productMain, By.ClassName("ui-grid-custom-filter-total"));
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(AdvBy.DataE2E("gridCheckboxWrapSelect")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();

            Driver.FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Driver.WaitForAjax();

            Driver.FindElements(By.ClassName("product-block-content"))[2].Click();
            Driver.WaitForElem(By.ClassName("ui-grid-custom-wrapper"));
            Driver.GridFilterSendKeys(productUpsell, By.ClassName("ui-grid-custom-filter-total"));
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(AdvBy.DataE2E("gridCheckboxWrapSelect")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.WaitForAjax();

            Driver.FindElements(By.ClassName("product-block-content"))[4].Click();
            Driver.WaitForElem(By.ClassName("ui-grid-custom-wrapper"));
            Driver.GridFilterSendKeys(productDownsell, By.ClassName("ui-grid-custom-filter-total"));
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(AdvBy.DataE2E("gridCheckboxWrapSelect")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.WaitForAjax();

            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
        }

        protected void SetFunnelProducts(List<string> products)
        {
            Driver.FindElement(By.ClassName("product-block-content")).Click();
            Driver.WaitForElem(By.ClassName("ui-grid-custom-wrapper"));
            foreach (string product in products)
            {
                Driver.GridFilterSendKeys(product, By.ClassName("ui-grid-custom-filter-total"));
                Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                    .FindElement(AdvBy.DataE2E("gridCheckboxWrapSelect")).Click();
            }

            Driver.FindElement(By.ClassName("btn-save")).Click();

            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
        }

        protected void SetFunnelCategories(List<string> categories)
        {
            Driver.FindElement(By.ClassName("product-block-content")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            foreach (string category in categories)
            {
                Driver.SendKeysInput(By.ClassName("ng-tree-search"), category);
                Driver.WaitForElem(By.XPath("//li[contains(@class, 'category-item-active')][not(contains(@class, 'jstree-hidden'))]" +
                    "/a/span[contains(@class, 'jstree-advantshop-name')][contains(text(), '" + category + "')]"), new TimeSpan(0, 1, 0));
                Driver.FindElement(By.XPath("//li[contains(@class, 'category-item-active')][not(contains(@class, 'jstree-hidden'))]" +
                    "/a/span[contains(@class, 'jstree-advantshop-name')][contains(text(), '"+ category + "')]")).Click();
                Driver.WaitForAjax();
            }

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
        }

        protected void SetFunnelName(string funnelName)
        {
            Driver.WaitForElem(AdvBy.DataE2E("LandingSiteName"));
            Driver.SendKeysInput(AdvBy.DataE2E("LandingSiteName"), funnelName);
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();

            Driver.WaitForElem(By.CssSelector("h1.funnel-page__name"));
        }

        protected void FillCheckoutForm()
        {
            Driver.SendKeysInput(By.Id("Data_User_Email"), "test@email.ru");
            Driver.SendKeysInput(By.Id("Data_User_FirstName"), "FirstName");
            Driver.SendKeysInput(By.Id("Data_User_LastName"), "TestLastName");
            Driver.SendKeysInput(By.Id("Data_User_Phone"), "79012345678");
            Driver.ScrollTo(AdvBy.DataE2E("btnCheckout"));
            Driver.FindElement(AdvBy.DataE2E("btnCheckout")).Click();
            Thread.Sleep(1000);
        }

        protected void FillFunnelFormModal(bool name = true, bool email = true, bool phone = true,
            bool textarea = false)
        {
            if (name)
            {
                Driver.SendKeysInput(By.CssSelector(".adv-modal:not(.ng-hide) input[type=\"text\"]"), "FirstName");
            }

            if (email)
            {
                Driver.SendKeysInput(By.CssSelector(".adv-modal:not(.ng-hide) input[type=\"email\"]"), "test@email.ru");
            }

            if (phone)
            {
                Driver.SendKeysInput(By.CssSelector(".adv-modal:not(.ng-hide) input[type=\"tel\"]"), "79012345678");
            }

            Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .lp-form__agreement label")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .lp-btn")).Click();
            Thread.Sleep(1000);
        }

        protected void FillFunnelForm(bool name = true, bool email = true, bool phone = true, bool textarea = false,
            bool agreement = true)
        {
            if (name)
            {
                Driver.SendKeysInput(By.CssSelector(".lp-form__field input[type=\"text\"]"), "FirstName");
            }

            if (email)
            {
                Driver.SendKeysInput(By.CssSelector(".lp-form__field input[type=\"email\"]"), "test@email.ru");
            }

            if (phone)
            {
                Driver.SendKeysInput(By.CssSelector(".lp-form__field input[type=\"tel\"]"), "79012345678");
            }

            if (textarea)
            {
                Driver.FindElement(By.ClassName("lp-textarea")).SendKeys("My textarea text");
            }

            if (agreement)
            {
                Driver.FindElement(By.CssSelector(".lp-form__agreement label")).Click();
            }

            Driver.FindElement(By.CssSelector(".lp-form__submit-block .lp-btn")).Click();

            Thread.Sleep(1000);
        }

        protected void GoToCreateFunnel(string funnelTab, string funnelPage)
        {
            Driver.FindElement(By.LinkText(funnelTab)).Click();
            Driver.WaitForAjax();
            Driver.FindElement(By.LinkText(funnelPage)).Click();
            Driver.WaitForAjax();
        }

        protected void CreateEmptyFunnel(string funnelName)
        {
            GoToAdmin("dashboard/createsite#?tabs=Presentations");
            Driver.FindElement(By.ClassName("last-item-empty-template-funnel")).Click();
            Driver.SendKeysInput(AdvBy.DataE2E("LandingSiteName"), funnelName);
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
            Driver.WaitForElem(By.ClassName("funnel-page__name"));
        }

        protected void GoToFunnelPageFromLp(int pageIndex, By waitElement)
        {
            Driver.WaitForElem(By.ClassName("lp-admin-menu__item"));
            Driver.FindElement(By.ClassName("lp-admin-menu__item")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.ClassName("lp-admin-menu__link"))[pageIndex].Click();
            Driver.WaitForElem(waitElement);
        }

        protected string GetOrderNum(bool isMobile = false)
        {
            if (isMobile)
            {
                return Driver.FindElement(By.ClassName("checkout-confirm-number")).Text.Substring(2);
            }
            else
            {
                string congratText = Driver.FindElement(By.ClassName("congrat-num")).Text;
                return congratText.Substring(congratText.IndexOf("номером") + 8);
            }
        }

        protected void CheckOrderInAdmin(string orderNum, List<string> productsInOrder)
        {
            ReInit();
            GoToAdmin("orders/edit/" + orderNum);
            VerifyAreEqual(productsInOrder.Count, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count,
                "expected products count");
            foreach (string product in productsInOrder)
            {
                VerifyAreEqual(1,
                    Driver.FindElement(By.ClassName("order-grid")).FindElements(By.LinkText(product)).Count,
                    "product " + product + " in order");
            }
        }

        protected void CheckLeadInAdmin(string landingId, int leadsCount)
        {
            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            Driver.GridFilterSendKeys(landingId, By.ClassName("ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: " + leadsCount,
                Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text, "lead from funnel");
        }

        protected void CheckBurgerMenuLink(int index)
        {
            Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.ClassName("lp-menu-header__item"))[index].Click();
            Thread.Sleep(500);
            Driver.ScrollToTop();
        }

        protected int ScrollPosition()
        {
            return Convert.ToInt32(((IJavaScriptExecutor) Driver).ExecuteScript("return window.pageYOffset"));
        }

        protected void CheckScrollToTop(By selector)
        {
            Driver.ScrollTo(selector);
            Driver.FindElement(By.ClassName("scroll-to-top-active")).Click();
            VerifyAreEqual(0, ScrollPosition(), "position after click by scroll-to-top");
        }

        protected void CheckFunnelPageInplaceEnabled(int pageNum, By waitElem, string landingId, string funnelPageName)
        {
            GoToFunnelPageFromLp(pageNum, waitElem);
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, $"funnel {funnelPageName} status inplace");
            VerifyIsNull(CheckConsoleLog(true), $"funnel {funnelPageName} console log inplace");
            CheckFunnelBlocksConsole(landingId, pageNum); //settings blocks console
        }

        protected void CheckFunnelBlocksConsole(string landingId, int pageNum)
        {
            for (int i = 0; i < Driver.FindElements(By.ClassName("blocks-constructor-container")).Count; i++)
            {
                Driver.ScrollTo(By.ClassName("blocks-constructor-container"), i);
                Driver.WaitForElems(By.ClassName("lp-block-constructor-hint"), i);
                Driver.MouseFocus(Driver.FindElements(By.ClassName("lp-block-constructor-hint"))[i]);
                Thread.Sleep(100);
                Driver.FindElements(By.ClassName("lp-blocks-constructor"))[i]
                    .FindElement(AdvBy.DataE2E("BlockSettingsBtn")).Click();
                Driver.WaitForElem(By.ClassName("blocks-constructor-modal--settings"));
                foreach (IWebElement tabLink in Driver.FindElements(
                    By.CssSelector(".tabs-horizontal>ul>.tabs-header-item")))
                {
                    tabLink.FindElement(By.ClassName("tabs-header-item-link")).Click();
                    Thread.Sleep(500);
                    VerifyIsNull(CheckConsoleLog(true),
                        $"funnel {landingId}, page {pageNum}, block {i}, tab {tabLink.GetAttribute("id")}");
                }

                Driver.FindElement(By.CssSelector(".blocks-constructor-modal--settings .adv-modal-close")).Click();
            }
        }

        public void GoToFunnelTab(int funnelId, string tabName, By waitElemSelector)
        {
            GoToAdmin("funnels/site/" + funnelId);
            Driver.FindElement(By.XPath("//span[contains(@class, 'lead-events__item__label')][contains(text(), '" +
                                        tabName + "')]")).Click();
            Driver.WaitForElem(waitElemSelector);
        }

        public void GoToFunnelSettingsTab(int funnelId, string tabName, string subtabName, By waitElemSelector)
        {
            GoToFunnelTab(funnelId, tabName, waitElemSelector);
            Driver.FindElement(By.LinkText(subtabName)).Click();
            Driver.WaitForAjax();
        }

        #endregion

        #region Grid

        protected void SelectGridRow(int rowIndex, string gridUniqueId)
        {
            IWebElement gridCheckbox = Driver.GetGridCell(rowIndex, "selectionRowHeaderCol", gridUniqueId);
            if (gridCheckbox.FindElement(By.TagName("input")).GetAttribute("class").Contains("ng-empty"))
            {
                gridCheckbox.FindElement(By.ClassName("adv-checkbox-emul")).Click();
            }
        }

        protected void UnselectGridRow(int rowIndex, string gridUniqueId)
        {
            IWebElement gridCheckbox = Driver.GetGridCell(rowIndex, "selectionRowHeaderCol", gridUniqueId);
            if (gridCheckbox.FindElement(By.TagName("input")).GetAttribute("class").Contains("ng-not-empty"))
            {
                gridCheckbox.FindElement(By.ClassName("adv-checkbox-emul")).Click();
            }
        }

        protected void RemoveSelectedGridRows()
        {
            Driver.SetGridAction("Удалить выделенные");
            Driver.SwalConfirm();
        }

        #endregion

        #region AdminPanelActions

        public void APItemBackClick()
        {
            Driver.FindElement(AdvBy.DataE2E("LpAdminMenuItemBack")).Click();
            Driver.WaitForElem(By.ClassName("funnel-page__name"));
        }

        public void APItemSettingsClick()
        {
            Driver.FindElement(AdvBy.DataE2E("LpAdminMenuItemSettings")).Click();
            Driver.WaitForElem(By.CssSelector("#lpSettings.adv-modal-active"));
        }

        public void APItemPagesClick()
        {
            Driver.FindElement(AdvBy.DataE2E("LpAdminMenuItemPages")).Click();
            Thread.Sleep(100);
        }

        public void APItemPagesClick(int pageIndex, By waitElemSelector)
        {
            Driver.FindElement(AdvBy.DataE2E("LpAdminMenuItemPages")).Click();
            Thread.Sleep(100);
            Driver.FindElements(By.ClassName("lp-admin-menu__link"))[pageIndex].Click();
            Driver.WaitForElem(waitElemSelector);
        }

        public void APItemInplaceModeClick(bool inplaceDisable)
        {
            Driver.FindElement(AdvBy.DataE2E("LpAdminMenuItemInplaceMode")).Click();
            if (inplaceDisable)
            {
                Driver.WaitForElem(By.CssSelector("html:not(.edit-mode)"));
            }
            else
            {
                Driver.WaitForElem(By.CssSelector("html.edit-mode"));
            }
        }

        #endregion
    }
}