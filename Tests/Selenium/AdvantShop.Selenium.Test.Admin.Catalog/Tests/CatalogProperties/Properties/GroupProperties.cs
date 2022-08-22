using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class GroupProperties : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Properties\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Properties\\Catalog.Category.csv",
                "Data\\Admin\\Properties\\Catalog.Brand.csv",
                "Data\\Admin\\Properties\\Catalog.Property.csv",
                "Data\\Admin\\Properties\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.Product.csv",
                "Data\\Admin\\Properties\\Catalog.Offer.csv",
                "Data\\Admin\\Properties\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.ProductCategories.csv"
            );
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
        [Order(0)]
        public void OpenWindowsAdd()
        {
            GoToAdmin("settingscatalog?catalogTab=properties");
            Driver.FindElement(By.CssSelector(".pull-right.header-alt-icons")).Click();

            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
        }

        [Test]
        [Order(1)]
        public void GroupAdd()
        {
            GoToAdmin("settingscatalog?catalogTab=properties");
            Driver.FindElement(By.CssSelector(".pull-right.header-alt-icons")).Click();

            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).SendKeys("New_group");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("New_group"));
        }

        [Test]
        [Order(2)]
        public void GroupAddCancel()
        {
            GoToAdmin("settingscatalog?catalogTab=properties");
            Driver.FindElement(By.CssSelector(".pull-right.header-alt-icons")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).SendKeys("New_new_group");
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();
            VerifyIsFalse(Driver.PageSource.Contains("New_new_group"));
        }

        [Test]
        [Order(3)]
        public void GroupEdit()
        {
            GoToAdmin("settingscatalog?catalogTab=properties");
            Driver.FindElement(By.XPath("//a[contains(text(), 'New_group')]")).Click();
            Driver.FindElement(By.CssSelector(".property-groups .selected .fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).Clear();
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).SendKeys("Newname_group");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Newname_group"));
        }

        [Test]
        [Order(4)]
        public void GroupEditCancel()
        {
            GoToAdmin("settingscatalog?catalogTab=properties");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Newname_group')]")).Click();
            Driver.FindElement(By.CssSelector(".property-groups .selected .fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).Clear();
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).SendKeys("Newname_group_edit");
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();
            VerifyIsFalse(Driver.PageSource.Contains("Newname_group_edit"));
        }

        [Test]
        [Order(5)]
        public void GroupDeleteCancel()
        {
            GoToAdmin("settingscatalog?groupId=11");
            string name = Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingTitle\"]")).Text;
            Driver.FindElement(By.CssSelector(".property-groups .selected .fa-times")).Click();
            Driver.SwalCancel();
            VerifyIsTrue(Driver.PageSource.Contains(name));
        }

        [Test]
        [Order(6)]
        public void GroupDeleteOk()
        {
            GoToAdmin("settingscatalog?groupId=11");
            string name = Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingTitle\"]")).Text;
            Driver.FindElement(By.CssSelector(".property-groups .selected .fa-times")).Click();
            Driver.SwalConfirm();
            VerifyIsFalse(Driver.PageSource.Contains(name));
        }
    }
}