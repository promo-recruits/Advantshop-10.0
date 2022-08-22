using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Modules.Tests.Modules
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class ModulesCommonTest : ModulesFunctions
    {
        static Dictionary<string, string> modulesData = 
            Functions.LoadCsvFile("Data\\Admin\\TestSettings\\ModulesData.csv",
                neededItems: Functions.GetModules());

        List<string> currentModules;

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Payment |
                                        ClearType.Shipping | ClearType.Settings);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\Catalog.Photo.csv",
                "Data\\Admin\\Catalog\\Catalog.Color.csv",
                "Data\\Admin\\Catalog\\Catalog.Size.csv",
                "Data\\Admin\\Catalog\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\Customers.Customer.csv",
                "Data\\Admin\\Catalog\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Catalog\\Customers.Contact.csv",
                "Data\\Admin\\Catalog\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Catalog\\Catalog.Offer.csv",
                "Data\\Admin\\Catalog\\Catalog.Property.csv",
                "Data\\Admin\\Catalog\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Catalog\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\Customers.Managers.csv");
            Init();

        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckModulesCommon([ValueSource(nameof(modulesData))] KeyValuePair<string, string> module)
        {
            ClearModulesBin();
            ClearModulesDirectory();

            //foreach (var module in modulesData)
            //{
                try
                {
                    InstallModule(BaseUrl, module.Key);
                    bool expectedPageStatus = GetPageStatus("adminv3/modules/details/" + module.Key, true) ==
                                              HttpStatusCode.OK;

                    VerifyIsTrue(expectedPageStatus,
                        "Module " + module.Key + ", module page, not expected status code",
                        false);
                    if (expectedPageStatus)
                    {
                        GetConsoleLog("Module " + module.Key + ", module page, message: ");

                        //прокликать вкладки модуля, если они есть
                        if (Driver.FindElements(By.CssSelector("[ng-init=\"module.stringId='" + module.Key + "';\"]"))
                            .Count > 0)
                        {
                            GoToModuleTab(module.Key, Driver.FindElements(By.ClassName("aside-menu-row")));
                        }
                        else
                        {
                            //old modules
                            Driver.SwitchTo().Frame("moduleIFrame");
                            if (Driver.FindElements(By.Id("tabs-headers")).Count > 0)
                            {
                                int tabid = 1;
                                while (tabid < Driver.FindElement(By.Id("tabs-headers")).FindElements(By.TagName("li"))
                                    .Count)
                                {
                                    GoToModuleTab(module.Key,
                                        Driver.FindElement(By.Id("tabs-headers")).FindElements(By.TagName("li")),
                                        tabid++);
                                }
                            }

                            Driver.SwitchTo().DefaultContent();
                        }

                        Thread.Sleep(1000);

                        ActivateModule(module.Key);

                        if (!string.IsNullOrEmpty(module.Value))
                        {
                            string[] modulePages = module.Value.Split(',');
                            foreach (string modulePage in modulePages)
                            {
                                GoToClient(modulePage);
                                Thread.Sleep(1000);
                                VerifyIsTrue(GetPageStatus(modulePage) == HttpStatusCode.OK,
                                    "Module " + module.Key + ", page " + modulePage + ", not expected status code",
                                    false);
                                GetConsoleLog("Module " + module.Key + ", page " + modulePage + ", message: ");
                            }
                        }

                        GoToAdmin("/modules/details/" + module.Key);
                        Thread.Sleep(1000);
                        ActivateModule(module.Key, "off");
                    }
                }
                catch (Exception ex)
                {
                    VerifyAddErrors("Module " + module.Key + ", ", ex.Message.ToString());
                }
            //}
        }
    }
}