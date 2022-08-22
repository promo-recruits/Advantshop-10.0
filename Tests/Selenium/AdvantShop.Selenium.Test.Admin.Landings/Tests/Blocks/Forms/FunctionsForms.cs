using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Forms
{
    internal class FunctionsForms : LandingsFunctions
    {
        public string GetElAttribute(string selector, string atribute = "value", string selectorType = "CssSelector",
            int elNumber = 0)
        {
            switch (selectorType)
            {
                case "CssSelector":
                    return Driver.FindElements(By.CssSelector(selector))[elNumber].GetAttribute(atribute);
                case "ClassName":
                    return Driver.FindElements(By.ClassName(selector))[elNumber].GetAttribute(atribute);
                case "Id":
                    return Driver.FindElements(By.Id(selector))[elNumber].GetAttribute(atribute);
                default:
                    return Driver.FindElements(By.XPath(selector))[elNumber].GetAttribute(atribute);
            }
        }

        public string GetCssValue(string cssSelector, string atribute)
        {
            return Driver.FindElement(By.CssSelector(cssSelector)).GetCssValue(atribute);
        }

        public void FillLpFormField(string value, int elNumber = 0, string selector = "[data-e2e=\"FormField\"]")
        {
            Driver.FindElement(By.CssSelector(selector))
                .FindElement(By.CssSelector("[data-ng-model=\"lpForm.form.fields[" + elNumber + "].value\"]")).Clear();
            Driver.FindElement(By.CssSelector(selector))
                .FindElement(By.CssSelector("[data-ng-model=\"lpForm.form.fields[" + elNumber + "].value\"]"))
                .SendKeys(value);
        }

        public void SendForm(int elNumber = 0, string cssSelector = "[data-e2e=\"FormField\"]")
        {
            Driver.FindElements(By.CssSelector(cssSelector))[elNumber]
                .FindElement(By.CssSelector("button[type=\"submit\"]")).Click();
            Thread.Sleep(2000);
        }

        public void CheckBoxCheck(string cssSelector)
        {
            if (Driver.FindElement(By.CssSelector(cssSelector + " input")).GetAttribute("class").IndexOf("ng-empty") !=
                -1)
                try
                {
                    Driver.FindElement(By.CssSelector(cssSelector + " .blocks-constructor-checkbox")).Click();
                }
                catch (Exception)
                {
                    Driver.FindElement(By.CssSelector(cssSelector + " label.lp-label")).Click();
                }
        }

        public void CheckBoxUncheck(string cssSelector)
        {
            if (Driver.FindElement(By.CssSelector(cssSelector + " input")).GetAttribute("class")
                .IndexOf("ng-not-empty") != -1)
                try
                {
                    Driver.FindElement(By.CssSelector(cssSelector + " label.lp-label")).Click();
                }
                catch (Exception)
                {
                    Driver.FindElement(By.CssSelector(cssSelector + " .blocks-constructor-checkbox")).Click();
                }
        }

        public void SelectOption(string optionValue, string selectCssSelector)
        {
            new SelectElement(Driver.FindElement(By.CssSelector(selectCssSelector))).SelectByValue(optionValue);
            Thread.Sleep(500);
        }

        public void SetColor(string e2eSelector, string color)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + e2eSelector + "\"] input")).Clear();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + e2eSelector + "\"] input")).SendKeys(color);
            Thread.Sleep(500);
        }

        public void TabFormSettingsClick(string tabName)
        {
            TabSelect("tabForm");
            Thread.Sleep(500);
            TabSelect(tabName);
            Thread.Sleep(500);
        }

        public void SetInputText(string e2eSelector, string textValue)
        {
            Driver.FindElement(By.CssSelector("input[data-e2e=\"" + e2eSelector + "\"]")).Clear();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("input[data-e2e=\"" + e2eSelector + "\"]")).SendKeys(textValue);
            Thread.Sleep(500);
        }

        public void SetIframeText(string iframeText, string iframeSelector = "iframe.cke_reset")
        {
            var textFrame = Driver.FindElement(By.CssSelector(iframeSelector));
            Driver.SwitchTo().Frame(textFrame);
            Driver.FindElement(By.CssSelector("body.cke_editable")).Clear();
            Driver.FindElement(By.CssSelector("body.cke_editable")).SendKeys(iframeText);
            Driver.SwitchTo().DefaultContent();
        }

        public void SelectMultiProduct(List<string> productsName)
        {
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);
            foreach (var product in productsName)
            {
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys(product);
                Thread.Sleep(1000);
                Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            }

            Driver.FindElement(By.Id("modalSelectOffer")).FindElement(By.ClassName("blocks-constructor-btn-confirm"))
                .Click();
            Thread.Sleep(500);
        }

        public int LeadProductCount()
        {
            Driver.FindElement(By.CssSelector("[name=\"leadForm\"]")).Click();
            return Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;
        }

        public void SetCheckoutData(string name = "Name", string phone = "80000000000", string email = "email@mail.ru",
            string surname = "Surname")
        {
            Driver.FindElement(By.Id("Data_User_Email")).SendKeys(email);
            Driver.FindElement(By.Id("Data_User_FirstName")).SendKeys(name);
            Driver.FindElement(By.Id("Data_User_LastName")).SendKeys(surname);
            Driver.FindElement(By.Id("Data_User_Phone")).SendKeys(phone);

            SendCheckout();
        }

        public void SendCheckout()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success"));
        }

        public int RowItemsCount(string parentCssSelector, string childCssSelector)
        {
            return Driver.FindElement(By.CssSelector(parentCssSelector)).FindElements(By.CssSelector(childCssSelector))
                .Count;
        }

        public void GoToCustomerPage()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ClientData\"]")).Click();
            Thread.Sleep(1000);
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
        }

        public void CloseCustomerPage(bool removeLastOrder = true)
        {
            if (removeLastOrder)
            {
                Driver.GetGridCell(0, "Number", "CustomerOrders").FindElement(By.TagName("a")).Click();
                Thread.Sleep(1000);

                Driver.SwitchTo().Window(Driver.WindowHandles[2]);
                Driver.FindElement(By.PartialLinkText("Удалить заказ")).Click();
                Driver.FindElement(By.CssSelector(".swal2-confirm.btn-success")).Click();
                Thread.Sleep(500);
                Driver.Close();
                Driver.SwitchTo().Window(Driver.WindowHandles[1]);
                Thread.Sleep(500);
            }

            Driver.Close();
            Driver.SwitchTo().Window(Driver.WindowHandles[0]);
            Thread.Sleep(500);
        }

        public void DelLead()
        {
            Driver.FindElement(By.CssSelector(".sticky-page-name .link-danger")).Click();
            Driver.SwalConfirm();
        }
    }
}