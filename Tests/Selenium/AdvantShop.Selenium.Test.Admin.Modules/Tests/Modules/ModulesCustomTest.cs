using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Modules.Tests.Modules
{
    [TestFixture]
    class ModulesCustomTest : ModulesFunctions
    {
        Dictionary<string, string> modulesData;
        List<string> currentModules;

        [OneTimeSetUp]
        [Author("irina.ba@advantshop.net")]
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

            modulesData = Functions.LoadCsvFile("Data\\Admin\\TestSettings\\ModulesData.csv",
                neededItems: Functions.GetModules());
            ClearModulesBin();
            ClearModulesDirectory();
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
        public void CheckMoySklad()
        {
            string moduleKey = "MoySklad";
            try
            {
                InstallModule(BaseUrl, moduleKey);
                bool expectedPageStatus =
                    GetPageStatus("adminv3/modules/details/" + moduleKey, true) == HttpStatusCode.OK;
                VerifyIsTrue(expectedPageStatus,
                    "Module " + moduleKey + ", module page, not expected status code",
                    false);
                if (expectedPageStatus)
                {
                    GetConsoleLog("Module " + moduleKey + ", module page, message: ");
                    GoToClient("modules/moysklad/1c_exchange.ashx");

                    VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "1c_exchange.ashx 500");

                    GoToAdmin("/modules/details/" + moduleKey);
                    Thread.Sleep(1000);
                    ActivateModule(moduleKey, "off");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Module " + moduleKey + ", " + ex.Message.ToString());
                Debug("Module " + moduleKey + ", " + ex.Message.ToString());
            }
        }
    }
}