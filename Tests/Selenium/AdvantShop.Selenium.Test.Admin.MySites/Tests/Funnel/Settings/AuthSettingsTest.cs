using System;
using System.Net;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class AuthSettingsTest : MySitesFunctions
    {
        string siteUrl = "lp/testfunnel";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing | ClearType.Customers | ClearType.CRM | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\Default\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSubBlock.csv",
                "data\\Admin\\Funnel\\Settings\\Customers.CustomerGroup.csv",
                "data\\Admin\\Funnel\\Settings\\Customers.TaskGroup.csv",
                "data\\Admin\\Funnel\\Settings\\Customers.Customer.csv",
                "data\\Admin\\Funnel\\Settings\\Customers.Departments.csv",
                "data\\Admin\\Funnel\\Settings\\Customers.Managers.csv",
                "data\\Admin\\Funnel\\Settings\\Customers.Contact.csv",
                "data\\Admin\\Funnel\\Settings\\CRM.DealStatus.csv",
                "data\\Admin\\Funnel\\Settings\\CRM.SalesFunnel.csv",
                "data\\Admin\\Funnel\\Settings\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].Lead.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].LeadCurrency.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].LeadItem.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].OrderSource.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].OrderStatus.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].[Order].csv",
                "data\\Admin\\Funnel\\Settings\\[Order].OrderContact.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].OrderCustomer.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].OrderItems.csv",
                "data\\Admin\\Funnel\\Settings\\[Order].OrderCurrency.csv"
            );

            Init(false);
            Functions.EnableCapcha(Driver, BaseUrl);

            //один зарегистрированный человек без лидов и заказов- Customer1
            //один зарегистрированный человек с лидом и заказом, но не тем - Customer2
            //один зарегистрированный человек с нужными лидами и заказами - Customer3
            //salesFunnel
            //customers
            //orders
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToFunnelSettingsTab(1, "Настройки", "Доступ", By.Name("RequireAuth"));
        }

        [TearDown]
        public void TearDownTest()
        {
            GoToAdmin("funnels/site/1#?landingAdminTab=settings&landingSettingsTab=auth");
            SetDefaultAuthSettings();
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckWithoutAuth()
        {
            VerifyIsTrue(Driver.FindElement(By.Name("RequireAuth")).GetAttribute("class").IndexOf("ng-not-empty") == -1,
                "require auth is not active");

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "page without auth is enabled");
                FillFunnelForm(); //check that action is enabled
                Driver.WaitForElem(AdvBy.DataE2E("FormSuccessText"));
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckWithoutAuth: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthWithRegUrl()
        {
            Driver.CheckBoxCheck("RequireAuth");
            Driver.SendKeysInput(By.Name("AuthRegUrl"), "https://advantshop.net");
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                VerifyAreEqual(BaseUrl + "lp/user/auth/1", Driver.Url, "auth login url page");
                Driver.FindElement(By.ClassName("link-registartion")).Click();
                Driver.WaitForElem(By.ClassName("site-header"));
                VerifyAreEqual("https://www.advantshop.net/", Driver.Url, "reg reg url page");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthWithRegUrl: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthWithoutRegUrl()
        {
            Driver.CheckBoxCheck("RequireAuth");
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                VerifyAreEqual(BaseUrl + "lp/user/auth/1", Driver.Url, "default auth login url page");
                Driver.FindElement(By.ClassName("link-registartion")).Click();
                Driver.WaitForElem(By.ClassName("registration-block"));
                VerifyAreEqual(BaseUrl + "registration?lpid=1", Driver.Url, "default reg reg url page");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthWithoutRegUrl: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthFilterRuleRegisteredRegistrate()
        {
            //registrate
            Driver.CheckBoxCheck("RequireAuth");
            SelectItem(By.Id("AuthFilterRule"), "0", false);
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                Driver.FindElement(By.ClassName("link-registartion")).Click();
                Driver.WaitForElem(By.ClassName("registration-block"));

                Driver.SendKeysInput(By.Name("FirstName"), "testname");
                Driver.SendKeysInput(By.Name("LastName"), "testlastname");
                Driver.SendKeysInput(By.Name("Email"), "test@test.ru");
                Driver.SendKeysInput(By.Name("Phone"), "+79999999999");
                Driver.SendKeysInput(By.Name("Password"), "123123");
                Driver.SendKeysInput(By.Name("PasswordConfirm"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("lp-block-form"));
                VerifyAreEqual(BaseUrl + siteUrl, Driver.Url, "after successful registration");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthFilterRuleNotRegistred: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthFilterRuleRegisteredLogin()
        {
            //login
            Driver.CheckBoxCheck("RequireAuth");
            SelectItem(By.Id("AuthFilterRule"), "0", false);
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "test@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "111111");
                Driver.FindElement(By.ClassName("btn-submit")).Click();
                Driver.WaitForElem(By.ClassName("toast-error"));
                VerifyAreEqual("Неверный логин или пароль", Driver.FindElement(By.ClassName("toast-title")).Text,
                    "not existing customer");

                Driver.SendKeysInput(By.Name("email"), "emailFirst@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("lp-block-form"));
                VerifyAreEqual(BaseUrl + siteUrl, Driver.Url, "after successful login");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthFilterRuleRegistred: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthFilterRuleRegisteredValidate()
        {
            string notValidColor = "rgb(241, 89, 89)";

            //кол-во полей, все поля обязательные.
            Driver.CheckBoxCheck("RequireAuth");
            SelectItem(By.Id("AuthFilterRule"), "0", false);
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient("registration?lpid=1");
                VerifyIsTrue(IsFieldRequired("FirstName"), "required firstname");
                VerifyIsTrue(IsFieldRequired("LastName"), "required LastName");
                VerifyIsTrue(IsFieldRequired("Email"), "required Email");
                VerifyIsTrue(IsFieldRequired("Phone"), "required Phone");
                VerifyIsTrue(IsFieldRequired("Password"), "required Password");
                VerifyIsTrue(IsFieldRequired("PasswordConfirm"), "required PasswordConfirm");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                VerifyAreEqual(notValidColor, GetFieldBorderColor("FirstName"), "border for firstname");
                VerifyAreEqual(notValidColor, GetFieldBorderColor("LastName"), "border for LastName");
                VerifyAreEqual(notValidColor, GetFieldBorderColor("Email"), "border for Email");
                VerifyAreEqual(notValidColor, GetFieldBorderColor("Phone"), "border for Phone");
                VerifyAreEqual(notValidColor, GetFieldBorderColor("Password"), "border for Password");
                VerifyAreEqual(notValidColor, GetFieldBorderColor("PasswordConfirm"), "border for PasswordConfirm");

                Driver.SendKeysInput(By.Name("FirstName"), "FirstName");
                Driver.SendKeysInput(By.Name("LastName"), "LastName");
                Driver.SendKeysInput(By.Name("Email"), "Email");
                Driver.SendKeysInput(By.Name("Phone"), "Phone");
                Driver.SendKeysInput(By.Name("Password"), "Password");
                Driver.SendKeysInput(By.Name("PasswordConfirm"), "PasswordConfirm");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                VerifyAreEqual(notValidColor, GetFieldBorderColor("Email"), "border for not valid Email");
                VerifyAreEqual(notValidColor, GetFieldBorderColor("Phone"), "border for not valid Phone");

                Driver.SendKeysInput(By.Name("Email"), "Email@email.ru");
                Driver.SendKeysInput(By.Name("Phone"), "79009009090");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("toast-error"));
                VerifyAreEqual("Введенные пароли не совпадают", Driver.FindElement(By.ClassName("toast-title")).Text,
                    "not same pass");

                Driver.SendKeysInput(By.Name("Password"), "Password");
                Driver.SendKeysInput(By.Name("PasswordConfirm"), "Password");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("lp-block-form"));
                VerifyAreEqual(BaseUrl + siteUrl, Driver.Url, "after fill registration");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthFilterRuleRegisterValidate: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthFilterRuleHasProduct()
        {
            //проверяет покупателя, у которого есть неподходящий, и покупателя с подходящим заказом
            Driver.CheckBoxCheck("RequireAuth");
            SelectItem(By.Id("AuthFilterRule"), "1", false);
            Driver.FindElement(By.CssSelector(".tab-pane.active a")).Click();
            Driver.WaitForModal();
            Driver.GridFilterSendKeys("Платье");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.ClassName("adv-checkbox-emul")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailSecond@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("block-user"));
                VerifyAreEqual("У Вас нет доступа к данной странице",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "user without any order");

                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailThird@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("lp-block-form"));
                VerifyAreEqual(BaseUrl + siteUrl, Driver.Url, "user with any order");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthFilterRuleHasProduct: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthFilterRuleHasProductWithoutSelectedProduct()
        {
            //проверяет покупателя, у которого нет заказа, и покупателя с подходящим заказом
            //в случае, если конкретный товар в насртойках не выбран
            Driver.CheckBoxCheck("RequireAuth");
            SelectItem(By.Id("AuthFilterRule"), "1", false);
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailFirst@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("block-user"));
                VerifyAreEqual("У Вас нет доступа к данной странице",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "user without any order");

                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailSecond@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("block-user"));
                VerifyAreEqual("У Вас нет доступа к данной странице",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "user without any order");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthFilterRuleHasProductWithoutSelectedProduct: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthFilterRuleHasLead()
        {
            //проверяет покупателя, у которого есть неподходящий, и покупателя с подходящим лидом
            Driver.CheckBoxCheck("RequireAuth");
            SelectItem(By.Id("AuthFilterRule"), "2", false);
            SelectItem(By.Id("AuthLeadSalesFunnelId"), "3", false);
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailSecond@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("block-user"));
                VerifyAreEqual("У Вас нет доступа к данной странице",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "user without any order");

                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailThird@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("lp-block-form"));
                VerifyAreEqual(BaseUrl + siteUrl, Driver.Url, "user with any order");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthFilterRuleHasLead: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthFilterRuleHasLeadByStatus()
        {
            //проверяет покупателя, у которого есть неподходящий, и покупателя с подходящим лидом
            Driver.CheckBoxCheck("RequireAuth");
            SelectItem(By.Id("AuthFilterRule"), "2", false);
            SelectItem(By.Id("AuthLeadSalesFunnelId"), "3", false);
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailSecond@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("block-user"));
                VerifyAreEqual("У Вас нет доступа к данной странице",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "user without any order");

                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailThird@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("lp-block-form"));
                VerifyAreEqual(BaseUrl + siteUrl, Driver.Url, "user with any order");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthFilterRuleHasLead: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void CheckAuthFilterRuleHasLeadWithoutSelectedSalesFunnel()
        {
            //проверяет покупателя, у которого нет лида, и покупателя с подходящим лидом
            //в случае, если конкретный список лидов в настройках не выбран
            Driver.CheckBoxCheck("RequireAuth");
            SelectItem(By.Id("AuthFilterRule"), "2", false);
            SaveFunnelSettings();

            try
            {
                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailFirst@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("block-user"));
                VerifyAreEqual("У Вас нет доступа к данной странице",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "user without any order");

                ReInitClient();
                GoToClient(siteUrl);
                Driver.SendKeysInput(By.Name("email"), "emailSecond@mail.ru");
                Driver.SendKeysInput(By.Name("password"), "123123");
                Driver.FindElement(By.ClassName("btn-submit")).Click();

                Driver.WaitForElem(By.ClassName("block-user"));
                VerifyAreEqual("У Вас нет доступа к данной странице",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "user without any order");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("CheckAuthFilterRuleHasLeadWithoutSelectedSalesFunnel: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        public void SetDefaultAuthSettings()
        {
            Driver.CheckBoxUncheck("RequireAuth");
            Driver.ClearInput(By.Name("AuthRegUrl"));
            SelectItem(By.Name("AuthFilterRule"), "0", false);
            if (Driver.FindElement(AdvBy.DataE2E("funnelSettingSave")).Enabled)
            {
                SaveFunnelSettings();
            }
        }

        public bool IsFieldRequired(string inputName)
        {
            return Driver.FindElement(By.Name(inputName)).GetAttribute("required") != null;
        }

        public string GetFieldBorderColor(string inputName)
        {
            return Driver.FindElement(By.Name(inputName)).GetCssValue("borderColor");
        }
    }
}