using System.Collections.Generic;
using System.IO;
using System.Threading;
using AdvantShop.Helpers;
using AdvantShop.Selenium.Core.Domain;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Modules.Tests
{
    class ModulesFunctions : BaseSeleniumTest
    {
        public void ClearModulesBin()
        {
            DirectoryInfo modulesBin = new DirectoryInfo((GetSitePath() + "/" + "bin/"));
            if (modulesBin.Exists)
            {
                foreach (var moduleBin in modulesBin.EnumerateFiles())
                {
                    if (moduleBin.Name.IndexOf("AdvantShop.Module") != -1)
                    {
                        FileHelpers.DeleteFile(moduleBin.FullName);
                    }
                }
            }
        }

        public void ClearModulesDirectory()
        {
            DirectoryInfo modules = new DirectoryInfo((GetSitePath() + "/" + "Modules/"));
            if (modules.Exists)
            {
                foreach (var module in modules.EnumerateDirectories())
                {
                    FileHelpers.DeleteDirectory(module.FullName);
                }
            }
        }

        public void GoToModuleTab(string moduleKey, IReadOnlyCollection<IWebElement> moduleSettingsTabs)
        {
            if (moduleSettingsTabs.Count > 1)
            {
                foreach (IWebElement tab in moduleSettingsTabs)
                {
                    tab.Click();
                    Thread.Sleep(500);
                }

                GetConsoleLog("Module " + moduleKey + ", module page, tabs errors, message: ");
            }
        }

        public void GoToModuleTab(string moduleKey, IReadOnlyCollection<IWebElement> moduleSettingsTabs, int tabId)
        {
            if (moduleSettingsTabs.Count > 1)
            {
                int i = 0;
                foreach (IWebElement tab in moduleSettingsTabs)
                {
                    if (tabId == i++)
                    {
                        tab.Click();
                        break;
                    }
                }

                Thread.Sleep(500);
                GetConsoleLog("Module " + moduleKey + ", module page, tabs errors, message: ");
            }
        }
    }
}