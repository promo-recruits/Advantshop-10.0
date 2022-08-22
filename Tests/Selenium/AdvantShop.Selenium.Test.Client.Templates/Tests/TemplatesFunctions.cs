using System;
using System.IO;
using System.Net;
using System.Threading;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Client.Templates.Tests
{
    public class TemplatesFunctions : BaseSeleniumTest
    {
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

        public void ChangeUibTab(int tabIndex)
        {
            Driver.FindElements(By.ClassName("uib-tab"))[tabIndex].Click();
        }

        public void CheckPageErrors(string template, string page = "", string message = "")
        {
            if (message.IsNullOrEmpty())
            {
                VerifyIsTrue(GetPageStatus(page) == HttpStatusCode.OK,
                    "Template " + template + ", page " + page + " status code: " + GetPageStatus(page),
                    false);
            }
            else
            {
                VerifyIsTrue(GetPageStatus(page) == HttpStatusCode.OK,
                    "Template " + template + ", " + message + " status code: " + GetPageStatus(page),
                    false);
            }

            GetConsoleLog("Template " + template + ", " + (message.IsNullOrEmpty() ? "page " + page : message) + ": ");
        }

        public void ClearTemplatesDirectory()
        {
            DirectoryInfo templates = new DirectoryInfo((GetSitePath() + "/" + "templates/"));
            if (templates.Exists)
            {
                foreach (var template in templates.EnumerateDirectories())
                {
                    FileHelpers.DeleteDirectory(template.FullName);
                }
            }
        }

        public void Sleep(int seconds = 1000)
        {
            Thread.Sleep(seconds);
        }

        public void SetStandartTemplate()
        {
            if (Driver.FindElement(By.ClassName("design-first")).FindElement(By.ClassName("info-item-content")).Text
                .IndexOf("Стандартный") == -1)
            {
                Driver.FindElement(
                        By.CssSelector("[ng-class=\"{'progress-overlay': design.templatesProgress['_default']}\"]"))
                    .FindElement(By.ClassName("btn-outline")).Click();
            }
        }

        public void SetTemplate(string template)
        {
            try
            {
                GoToAdmin("design/templateshop");
                Driver.FindElement(By.CssSelector("[ng-class=\"{'progress-overlay': design.templatesProgress['" +
                                                  template + "']}\"]")).FindElement(By.ClassName("btn-success"))
                    .Click();
            }
            catch (Exception)
            {
                GoToAdmin("design");
                Driver.FindElement(By.CssSelector("[ng-class=\"{'progress-overlay': design.templatesProgress['" +
                                                  template + "']}\"]")).FindElement(By.ClassName("btn-outline"))
                    .Click();
            }

            Driver.WaitForElem(By.ClassName("design-title"), TimeSpan.FromMinutes(5));
        }

        public void DeleteAllTemplates()
        {
            while (Driver.FindElements(By.CssSelector(".design-last td")).Count > 0)
            {
                Driver.FindElements(By.CssSelector(".design-last td .btn-link[type=\"button\"]"))[0].Click();
                Driver.SwalConfirm();
                Driver.WaitForElem(By.ClassName("design-title"), TimeSpan.FromMinutes(5));
            }
        }
    }
}