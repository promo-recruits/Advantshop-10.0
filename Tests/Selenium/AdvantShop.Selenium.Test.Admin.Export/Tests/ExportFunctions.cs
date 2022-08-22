using System.Text.RegularExpressions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Helpers;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Export
{
    public class ExportServices : BaseSeleniumTest
    {
        //В МЕТОДАХ VERIFY ПЕРЕПУТАНЫ МЕСТАМИ "EXPECTED" И "WAS"!!!
        //НА ПЕРВОМ МЕСТЕ СТОИТ WAS, НА ВТОРОМ EXPECTED (КОСЯК)
        //КОГДА-НИБУДЬ ПОПРАВИТЬ.

        #region CommonMethods

        //TODO: add to csv and google this method
        public string GetExportName(string testname)
        {
            DateTime date = DateTime.Now;
            return testname + date.ToShortDateString() + "_" + date.Hour + date.Minute + date.Second;
        }

        //создает новую выгрузку с заданным именем
        public void AddExportFeed(string exportName, string description = "")
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddExportFeed\"]")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), exportName);
            if (!String.IsNullOrEmpty(description))
            {
                Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddDesc\"]"), description);
            }

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("Export"));
        }

        public bool CheckboxChecked(string elName)
        {
            try
            {
                return Driver.FindElement(By.Name(elName)).GetAttribute("class").IndexOf("ng-not-empty") != -1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ElementName: " + elName + e.Message.ToString());
                return Driver.FindElement(By.Name(elName)).GetAttribute("class").IndexOf("ng-not-empty") != -1;
            }
        }

        public bool CompareTextValue(string elName, string expectedValue = "")
        {
            return Driver.FindElement(By.Name(elName)).GetAttribute("value") == expectedValue;
        }

        public bool SelectOptionSelected(string selectSelector, string expectedValue, string findSelectType = "Name",
            string findOptionType = "Value")
        {
            IWebElement element;
            switch (findSelectType)
            {
                case "Name":
                    element = Driver.FindElement(By.Name(selectSelector))
                        .FindElement(By.CssSelector("option[selected=\"selected\"]"));
                    break;
                case "CssSelector":
                    element = Driver.FindElement(By.CssSelector(selectSelector))
                        .FindElement(By.CssSelector("option[selected=\"selected\"]"));
                    break;
                default:
                    element = Driver.FindElement(By.ClassName(selectSelector))
                        .FindElement(By.CssSelector("option[selected=\"selected\"]"));
                    break;
            }

            switch (findOptionType)
            {
                case "Value":
                    return element.GetAttribute("value") == expectedValue;
                case "Label":
                    return element.GetAttribute("label") == expectedValue;
                default:
                    return element.Text == expectedValue;
            }
        }

        public void SelectOption(string selectSelector, string optionSelector, string findSelectType = "Name",
            string findOptionType = "Value")
        {
            SelectElement selectEl;
            switch (findSelectType)
            {
                case "Name":
                    selectEl = new SelectElement(Driver.FindElement(By.Name(selectSelector)));
                    break;
                case "CssSelector":
                    selectEl = new SelectElement(Driver.FindElement(By.CssSelector(selectSelector)));
                    break;
                default:
                    selectEl = new SelectElement(Driver.FindElement(By.ClassName(selectSelector)));
                    break;
            }

            switch (findOptionType)
            {
                case "Value":
                    selectEl.SelectByValue(optionSelector);
                    break;
                case "Text":
                    selectEl.SelectByText(optionSelector);
                    break;
                default:
                    selectEl.SelectByIndex(Convert.ToInt32(optionSelector));
                    break;
            }
        }

        public bool IsElementExists(string value, string type)
        {
            try
            {
                switch (type)
                {
                    case "LinkText":
                        Driver.FindElement(By.LinkText(value));
                        break;
                    case "CssSelector":
                        Driver.FindElement(By.CssSelector(value));
                        break;
                    case "ClassName":
                        Driver.FindElement(By.ClassName(value));
                        break;
                    case "XPath":
                        Driver.FindElement(By.XPath(value));
                        break;
                    default:
                        return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //возвращает count of вхождений xml-подстроки в xml-строку файла
        public int CountOfStrInXml(string mainStr, string subStr)
        {
            return new Regex(ConvertCsvString(subStr)).Matches(mainStr).Count;
        }

        //убирает из строки ненужные слеши
        public string ConvertCsvString(string csvStr)
        {
            return csvStr.ToLower().Replace("\\\"", "\"");
        }

        public string DateToYmlFormat(DateTime date, string time = "12:00:00")
        {
            return date.ToString("yyyy-MM-dd") + " " + time;
        }

        public string GetTegContent(string mainStr, string tagName)
        {
            string result = mainStr.Substring(mainStr.IndexOf("<" + tagName + ">"));
            return result.Substring(tagName.Length + 2, result.IndexOf("</" + tagName + ">") - tagName.Length - 2);
        }

        public void ReturnToExport()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReturnToExport\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("Export"));
        }

        #endregion

        #region Yandex

        //переходит на вкладку настроек, задает имя файла и выставляет настройку "выгружать в архив" в истину
        public string SetCommonYExportSettings(string testname, string exportPath, string exportType = "xml")
        {
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            string exportName = GetExportName(testname);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), exportPath + exportName);
            Driver.FindElement(By.CssSelector("[name=\"NeedZip\"]~.adv-checkbox-emul")).Click();
            return exportName + "." + exportType;
        }


        //скачивает архив, преобразовывает архив в xml-строку и возвращает её
        //в т.ч. авторизация в сервисах яндекса
        public string GetXmlFromYZip(string exportName)
        {
            Driver.WaitForElem(AdvBy.DataE2E("DownloadFile"));

            string localExportName = GetDownloadPath() + exportName;
            Driver.FindElement(By.CssSelector("[ng-if=\"exportFeeds.IsZip\"] a")).Click();
            Thread.Sleep(500);
            FileHelpers.UnZipFile(localExportName + ".zip");
            File.Delete(localExportName + ".zip");

            try
            {
                CheckXmlValidator(localExportName);
            }
            catch(Exception ex)
            {
                VerifyAddErrors("Error in webmaster xml-validator (mb capcha)", ex.Message);
            }

            string xmlString = "";
            try
            {
                if (Driver.FindElement(By.CssSelector(".XmlResult-Content span")).Text.IndexOf("XML соответствует схеме XSD") != -1)
                {
                    IJavaScriptExecutor javaScript = (IJavaScriptExecutor)Driver;
                    javaScript.ExecuteScript("window.close()");
                    Driver.SwitchTo().Window(Driver.WindowHandles[0]);
                    Console.Error.WriteLine("begin XmlFeedReader.YamarketFeedToString");
                    xmlString = XmlFeedReader.YamarketFeedToString(localExportName).ToLower();
                    Console.Error.WriteLine("end XmlFeedReader.YamarketFeedToString");

                    File.Delete(localExportName);
                }
                else throw new Exception();
            }
            catch (Exception ex)
            {
                VerifyIsTrue(false, "yandex validation failed:" +
                                    Driver.FindElement(By.CssSelector(".slot-error-box .error-message.b-static-text"))
                                        .Text +
                                    "; filename: " + localExportName);
            }

            return xmlString;
        }

        [Ignore("only for local run")]
        public void CheckXmlValidator(string pathToFile)
        {
            //отправлять файл в яндекс-вебмастер
            IJavaScriptExecutor javaScript = (IJavaScriptExecutor)Driver;
            javaScript.ExecuteScript("window.open()");
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);

            Driver.Navigate().GoToUrl("https://webmaster.yandex.ru/welcome/");
            if (Driver.Manage().Cookies.GetCookieNamed("yandex_login") == null)
            {
                Driver.Manage().Cookies.AddCookie(new Cookie("yandex_login", "testmailimap", ".yandex.ru", "/", null));
                Driver.Manage().Cookies.AddCookie(new Cookie("yandexuid", "175826191634113453", ".yandex.ru", "/", null));
                Driver.Manage().Cookies.AddCookie(new Cookie("yuidss", "175826191634113453", ".yandex.ru", "/", null));
                Driver.Manage().Cookies.AddCookie(new Cookie("Session_id", "3:1634114726.5.0.1634114726330:QhDvWw:22.1|472614925.0.2|3:242063.274119.-aElYUoIxsHdRTl1HVVVgyttz9c", ".yandex.ru", "/", null));
            }
            
            //именно переход через страницу вебмастера, чтоб обойти проверку на робота
            Driver.Navigate().GoToUrl("https://webmaster.yandex.ru/tools/xml-validator/");

            Driver.FindElement(By.ClassName("TabsMenu-Tab_type_SHOPS")).FindElement(By.TagName("button")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".XmlFeeds .TabsMenu-Tab_type_FILE button")).Click();
            var input = Driver.FindElement(By.CssSelector(".XmlFeeds-Input input[type=\"file\"]"));
            UnHide(input);
            input.SendKeys(pathToFile);
            Driver.FindElement(By.ClassName("XmlForm-Submit")).Click();
            Thread.Sleep(100);
        }

        //возвращает подстроку с продуктом из xml-строки начиная с заданного вхождения и заканчивая tegом </offer>
        public string GetYProductFromXml(string mainStr, string productStart)
        {
            string product = mainStr.Substring(mainStr.IndexOf(ConvertCsvString(productStart)));
            return product.Substring(0, product.IndexOf("</offer>"));
        }

        //возвращает подстроку с promos из xml-строки начиная с заданного вхождения и заканчивая tegом </promos>
        public string GetPromosFromXml(string mainStr)
        {
            string promos = mainStr.Substring(mainStr.IndexOf("<promos>"));
            return promos.Substring(0, promos.IndexOf("</promos>"));
        }

        //возвращает подстроку с promo из xml-строки начиная с заданного вхождения и заканчивая tegом </promo>
        public static string GetPromoFromXml(string mainStr, int promoNumber)
        {
            int currentIndex = 0;
            int currentPromo = 0;

            while (currentPromo++ != promoNumber && currentIndex != -1)
            {
                currentIndex = mainStr.IndexOf("<promo ", currentIndex + 1);
            }

            return mainStr.Substring(currentIndex, mainStr.IndexOf("</promo>", currentIndex) - currentIndex);
        }

        public void AddPromocode(string promocodeName, string couponName, string couponValue = "100",
            string couponCur = "RUB", List<int> couponCategories = null, List<string> couponProducts = null,
            bool isAllProducts = false)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), promocodeName, false);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCoupon\"]")).Click();

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), couponName);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponValue\"]"), couponValue);
            SelectOption("[data-e2e=\"couponCurrency\"]", couponCur, "CssSelector");
            if (couponCategories != null)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
                Driver.WaitForElem(By.ClassName("ng-tree-search"));

                var hasChilds =
                    Driver.FindElements(By.CssSelector(".modal-body .jstree-closed .jstree-icon.jstree-ocl"));
                foreach (var el in hasChilds)
                {
                    el.Click();
                }

                foreach (int idAncor in couponCategories)
                {
                    Driver.FindElement(By.CssSelector("[id=\"" + idAncor + "_anchor\"]")).Click();
                }

                Driver.FindElement(By.ClassName("btn-save")).Click();
            }

            if (couponProducts != null)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"couponProducts\"]")).Click();

                if (isAllProducts)
                {
                    var hasChilds =
                        Driver.FindElements(By.CssSelector(
                            ".modal-body li.jstree-node.category-item-active.jstree-closed:not(.jstree-leaf)"));
                    Driver.WaitForElem(AdvBy.DataE2E("gridFilterSearch"));
                    //string selector = "li.jstree-node.category-item-active.jstree-closed:not(.jstree-leaf)";
                    foreach (var el in hasChilds)
                    {
                        el.Click();
                        Thread.Sleep(100);
                    }

                    foreach (string product in couponProducts)
                    {
                        Driver.FindElement(By.CssSelector("[id=\"" + product + "_anchor\"]")).Click();
                        Thread.Sleep(100);
                        Driver.FindElement(By.CssSelector(
                                "[data-e2e-grid-cell=\"gridProductsSelectvizr[-1][\'selectionRowHeaderCol\']\"]"))
                            .FindElement(
                                By.CssSelector("[data-e2e=\"gridHeaderCheckboxSelectAll\"]~.adv-checkbox-emul"))
                            .Click();
                    }
                }
                else
                {
                    foreach (string product in couponProducts)
                    {
                        Driver.GridFilterModalSendKeys(product);
                        Driver.FindElement(By.CssSelector(
                                "[data-e2e-grid-cell=\"gridProductsSelectvizr[0][\'selectionRowHeaderCol\']\"]"))
                            .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
                    }
                }

                Driver.FindElement(By.ClassName("btn-save")).Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", couponName, "CssSelector", "Text");
            Driver.FindElement(By.ClassName("modal-dialog")).FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]"))
                .Click();
            Driver.WaitForToastSuccess();
        }

        public void AddCoupon(string couponName, string couponValue = "100", string couponCur = "RUB")
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCoupon\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), couponName);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponValue\"]"), couponValue);
            SelectOption("[data-e2e=\"couponCurrency\"]", couponCur, "CssSelector");
        }

        public void AddDiscount(string name, List<string> products = null, string dateStart = "",
            string dateExpiration = "", string description = "", string url = "")
        {
            if (string.IsNullOrWhiteSpace(dateStart))
            {
                dateStart = DateTime.Now.ToShortDateString();
            }

            if (string.IsNullOrWhiteSpace(dateExpiration))
            {
                dateExpiration = DateTime.Now.AddDays(5).ToShortDateString();
            }

            Driver.FindElement(
                By.CssSelector(
                    "[data-controller=\"'ModalAddEditYandexPromoFlashCtrl'\"] [data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), name);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]"), description);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.PromoUrl\"]"), url);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            if (products != null)
            {
                foreach (string product in products)
                {
                    AddProductToPromo(product);
                }
            }
            else
            {
                Driver.FindElement(
                        By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[0][\'selectionRowHeaderCol\']\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            }

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                dateExpiration + Keys.Enter, byToDropFocus: By.ClassName("text-required"));
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                dateStart + Keys.Enter, byToDropFocus: By.ClassName("text-required"));
            Driver.FindElement(By.ClassName("modal-dialog")).FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]"))
                .Click();
            Driver.WaitForToastSuccess();
        }

        public void AddGiftWithPurchase(string giftName, List<string> products, string giftId, string quantity = "1",
            string dateStart = "", string dateExpiration = "", string description = "", string url = "")
        {
            Driver.FindElement(
                    By.CssSelector(
                        "[data-controller=\"'ModalAddEditYandexPromoGiftCtrl'\"] [data-e2e=\"AddYandexPromo\"]"))
                .Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), giftName);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]"), description);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.PromoUrl\"]"), url);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            foreach (string product in products)
            {
                AddProductToPromo(product);
            }

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddGift\"]")).Click();
            Driver.GridFilterModalSendKeys("TestProduct" + giftId);
            Driver.FindElement(
                    By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[0][\'selectionRowHeaderCol\']\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();

            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                quantity);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                dateExpiration + Keys.Enter, byToDropFocus: By.ClassName("text-required"));

            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                dateStart + Keys.Enter, byToDropFocus: By.ClassName("text-required"));
            Driver.FindElement(By.ClassName("modal-dialog")).FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]"))
                .Click();
            Driver.WaitForToastSuccess();
        }

        public void AddNplusM(string name, List<string> products = null, List<string> categories = null,
            string quantity = "2", string freeQuantity = "1", string dateStart = "", string dateExpiration = "",
            string description = "", string url = "")
        {
            Driver.FindElement(By.CssSelector(
                "[data-controller=\"'ModalAddEditYandexPromoNPlusMCtrl'\"] [data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), name);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]"), description);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.PromoUrl\"]"), url);

            if (products != null)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
                foreach (string product in products)
                {
                    AddProductToPromo(product);
                }

                Driver.FindElement(By.ClassName("btn-save")).Click();
            }

            if (categories != null)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategories\"]")).Click();
                Driver.WaitForElem(By.ClassName("jstree-search__btn-wrap"));
                string selector = "li.jstree-node.category-item-active.jstree-closed:not(.jstree-leaf)";
                while (Driver.FindElements(By.CssSelector(selector)).Count != 0)
                {
                    Driver.FindElement(By.CssSelector(selector + " i.jstree-icon.jstree-ocl")).Click();
                }

                //while (driver.FindElements(By.CssSelector("li.jstree-node.jstree-closed")).Count != 0)
                //{
                //    driver.FindElement(By.CssSelector("li.jstree-node.jstree-closed i.jstree-icon.jstree-ocl")).Click();
                //    Thread.Sleep(500);               
                //}
                foreach (string category in categories)
                {
                    Driver.FindElement(By.CssSelector("a[id=\"" + category + "_anchor\"")).Click();
                }

                foreach (var el in Driver.FindElements(By.CssSelector(".modal-body a.jstree-anchor.jstree-clicked")))
                {
                    if (!categories.Contains(el.GetAttribute("id").Substring(0, 1)))
                    {
                        el.Click();
                    }
                }

                Driver.FindElement(By.ClassName("btn-save")).Click();
            }

            if (quantity != "2")
            {
                Driver.SendKeysInput(
                    By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                    Keys.Backspace + quantity, false);
            }
            if (freeQuantity != "1")
            {
                Driver.SendKeysInput(
                    By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"),
                    Keys.Backspace + freeQuantity, false);
            }
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                dateExpiration + Keys.Enter, byToDropFocus: By.ClassName("text-required"));
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                dateStart + Keys.Enter, byToDropFocus: By.ClassName("text-required"));
            Driver.FindElement(By.ClassName("modal-dialog")).FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]"))
                .Click();
            Driver.WaitForToastSuccess();
        }

        public void AddProductToPromo(string product)
        {
            Driver.GridFilterModalSendKeys("TestProduct" + product);
            Driver.FindElement(
                    By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[0][\'selectionRowHeaderCol\']\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
        }

        #endregion

        #region Google

        //переходит на вкладку настроек, задает имя файла и выставляет настройку "выгружать в архив" в истину
        public string SetCommonGExportSettings(string testname, string exportPath, string exportType = "xml")
        {
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            string exportName = GetExportName(testname);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), exportPath + exportName);
            return exportName + "." + exportType;
        }

        public string GetXmlFromGFile(IWebDriver driver, string exportPath, string exportName)
        {
            Driver.WaitForElem(AdvBy.DataE2E("DownloadFile"));
            GoToClient(exportPath + exportName);
            Thread.Sleep(100);
            string xmlResult = driver.PageSource.ToLower();
            int indexOfStart = xmlResult.IndexOf("<rss xmlns");

            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(
                        xmlResult.Substring(indexOfStart, xmlResult.IndexOf("</rss>") - indexOfStart + 6), @"[\r\n]",
                        ""),
                    @"  ", ""),
                @"> <", "><");
        }

        public string GetGProductFromXml(string mainStr, int promoNumber)
        {
            int currentIndex = 0;
            int currentPromo = 0;

            while (currentPromo++ != promoNumber && currentIndex != -1)
            {
                currentIndex = mainStr.IndexOf("<item>", currentIndex + 1);
            }

            return mainStr.Substring(currentIndex, mainStr.IndexOf("</item>", currentIndex) - currentIndex);
        }

        public string GetGProductFromXml(string mainStr, string productId)
        {
            string product = mainStr.Substring(mainStr.IndexOf("<g:id>" + (productId) + "</g:id>"));
            return product.Substring(0, product.IndexOf("</item>"));
        }

        public void RemoveGoogleExport(string name)
        {
            Driver.FindElement(By.LinkText(name)).Click();
            Driver.WaitForElem(By.ClassName("link-danger"));
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        //убирает из строки ненужные слеши
        public string ConvertGCsvString(string csvStr)
        {
            return csvStr.ToLower().Replace("<![cdata[ ", "<![cdata[").Replace(" ]]>", "]]>");
        }

        #endregion
    }
}