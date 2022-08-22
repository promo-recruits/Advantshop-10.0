using System;
using System.Collections.Generic;
using System.Net;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Client.Templates.Tests.Pages
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class TemplatePages : TemplatesFunctions
    {
        static List<string> templates = Functions.GetTemplates();
        bool useTrial = Functions.GetCustomSiteUrl() != null;
        IEnumerable<string> pages;

        [OneTimeSetUp]
        public void SetupTest()
        {
            if (useTrial)
            {
                InitTrial();

                GoToAdmin("cards");
                if (Driver.FindElements(By.PartialLinkText("Включить канал")).Count > 0)
                {
                    Driver.FindElement(By.PartialLinkText("Включить канал")).Click();
                    Sleep(1000);
                }

                GoToAdmin("settingsbonus");
                Driver.CheckBoxCheck("IsEnabled");
                if (!(Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).GetAttribute("disabled") ==
                      "disabled"))
                {
                    Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
                }

                GoToAdmin("design");
                SetStandartTemplate();
                DeleteAllTemplates();
            }
            else
            {
                InitializeService.RollBackDatabase();
                InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Orders |
                                            ClearType.Payment | ClearType.Shipping | ClearType.Settings);
                InitializeService.LoadData(
                    "Data\\Client\\Catalog\\Catalog.Photo.csv",
                    "Data\\Client\\Catalog\\Catalog.Color.csv",
                    "Data\\Client\\Catalog\\Catalog.Size.csv",
                    "Data\\Client\\Catalog\\Catalog.Category.csv",
                    "Data\\Client\\Catalog\\Customers.Customer.csv",
                    "Data\\Client\\Catalog\\Customers.CustomerGroup.csv",
                    "Data\\Client\\Catalog\\Customers.Contact.csv",
                    "Data\\Client\\Catalog\\Catalog.Brand.csv",
                    "Data\\Client\\Catalog\\Catalog.Product.csv",
                    "Data\\Client\\Catalog\\Catalog.ProductCategories.csv",
                    "Data\\Client\\Catalog\\Catalog.Offer.csv",
                    "Data\\Client\\Catalog\\Catalog.Property.csv",
                    "Data\\Client\\Catalog\\Catalog.PropertyValue.csv",
                    "Data\\Client\\Catalog\\Catalog.ProductPropertyValue.csv",
                    "Data\\Client\\Catalog\\Catalog.PropertyGroup.csv",
                    "Data\\Client\\Catalog\\Customers.Managers.csv",
                    "Data\\Client\\Catalog\\[Order].OrderStatus.csv",
                    "Data\\Client\\Catalog\\[Order].ShippingMethod.csv",
                    "Data\\Client\\Catalog\\[Order].PaymentMethod.csv"
                );


                InitializeService.BonusSystemActive();
                Init();
                ClearTemplatesDirectory();
            }

            //templates = Functions.GetTemplates("Data\\Client\\TestSettings\\TemplateNamesDataPartial.csv");
            pages = Functions.LoadCsvFile("Data\\Client\\TestSettings\\TestingPagesData.csv", true);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("design");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckPagesCommon([ValueSource(nameof(templates))] string template)
        {
            try
            {
                SetTemplate(template);
                //clear log
                GoToClient();
                if (GetPageStatus("") == HttpStatusCode.OK)
                {
                    //прохожусь по тем страницам, где делать ничего совсем не надо и которые и так доступны
                    foreach (var page in pages)
                    {
                        GoToClient(page);
                        Sleep();
                        CheckPageErrors(template, page);
                    }
                }
                else
                {
                    CheckPageErrors(template, "", " Home page");
                }
            }
            catch (Exception ex)
            {
                VerifyIsTrue(false,
                    "Template " + template + ", message: " + ex.Message, false);
            }
            finally
            {
                ReInit(useTrial: useTrial);
            }
        }

        [Test]
        public void CheckPagesRest([ValueSource(nameof(templates))] string template)
        {
            try
            {
                SetTemplate(template);
                GoToClient();

                if (GetPageStatus("") == HttpStatusCode.OK)
                {
                    //включить автоматическое определение города
                    GoToAdmin("settingstemplate");
                    ChangeUibTab(0);
                    Driver.CheckBoxCheck("DisplayCityInTopPanel");
                    Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();

                    //одна / две колонки - если у шаблона есть такая настройка
                    GoToAdmin("settingstemplate");
                    ChangeUibTab(0);
                    if (Driver.FindElement(By.Name("MainPageMode")).FindElements(By.TagName("option")).Count > 1)
                    {
                        if (Driver.FindElement(By.Name("MainPageMode")).FindElements(By.TagName("option"))[0]
                            .GetAttribute("selected") == "true")
                        {
                            SelectOption("MainPageMode", "1", "Name", "Index");
                        }
                        else
                        {
                            SelectOption("MainPageMode", "0", "Name", "Index");
                        }

                        Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                        GoToClient();
                        Sleep();
                        CheckPageErrors(template, "", "home page mode change");
                    }

                    //добавить товар в сравнение и список желаний, проверить еще раз те страницы
                    GoToAdmin("settingstemplate");
                    ChangeUibTab(0);
                    Driver.CheckBoxCheck("WishListVisibility");
                    ChangeUibTab(2);
                    Driver.CheckBoxCheck("EnableCompareProducts");
                    Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();

                    //единственный товар с фото, нужно для некоторых шаблонов

                    //для шаблонов, где кнопки "Добавить в сравнение/список желаний" появляются при наведении на фото
                    //Для остальных - идем к товару, блок с инфой которого покороче
                    if (template.IndexOf("Elza") != -1)
                    {
                        GoToClient("products/test-product1");
                        Sleep();
                        Driver.MouseFocus(By.ClassName("gallery-picture"));
                    }
                    else
                    {
                        GoToClient("products/test-product110");
                        Sleep();
                        if (template.IndexOf("Mailo") == -1 && template.IndexOf("Nils") == -1)
                        {
                            Driver.ScrollTo(By.TagName("h1"));
                        }
                    }

                    Driver.FindElement(By.ClassName("compare-state-not-add")).Click();
                    Driver.FindElement(By.ClassName("wishlist-state-not-add")).Click();
                    GoToClient("compare");
                    CheckPageErrors(template, "compare", "compage page with product");
                    Driver.FindElement(By.ClassName("compareproduct-product-remove")).Click();
                    GoToClient("wishlist");
                    CheckPageErrors(template, "wishlist", "wishlist page with product");

                    Driver.FindElement(By.ClassName("wishlist-remove")).Click();

                    //товар под заказ
                    GoToClient("products/test-product17");
                    Sleep();
                    if (template.IndexOf("Moloko") == -1)
                    {
                        Driver.ScrollTo(By.CssSelector(".details-block"));
                    }
                    else
                    {
                        Driver.ScrollTo(By.ClassName("price"));
                    }

                    Driver.FindElement(By.CssSelector("[data-cart-preorder]")).Click();
                    Sleep();
                    CheckPageErrors(template, Driver.Url, "successful checkout");

                    //оформление заказа: карточка товара, корзина, оформление, оформленный заказ
                    GoToClient("products/test-product110");
                    Sleep();
                    if (template.IndexOf("Moloko") == -1)
                    {
                        Driver.ScrollTo(By.CssSelector(".details-block"));
                    }
                    else
                    {
                        Driver.ScrollTo(By.ClassName("price"));
                    }

                    Driver.FindElement(By.CssSelector("[data-cart-add]")).Click();
                    GoToClient("cart");
                    CheckPageErrors(template, "cart");

                    Driver.ScrollTo(By.ClassName("cart-full-buttons"));
                    Driver.FindElement(By.CssSelector("[data-ng-href=\"checkout\"]")).Click();
                    Sleep(1000);
                    CheckPageErrors(template, "checkout");

                    if (GetPageStatus("checkout") == HttpStatusCode.OK)
                    {
                        Driver.ScrollTo(By.CssSelector("[data-e2e=\"btnCheckout\"]"));
                        Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
                        Sleep();
                        CheckPageErrors(template, Driver.Url, "successful checkout");
                    }

                    //личный кабинет - пройтись по вкладкам + посмотреть карточку заказа
                    GoToClient("myaccount?tab=orderhistory");
                    CheckPageErrors(template, "myaccount?tab=orderhistory", "myaccount page");
                    if (GetPageStatus("myaccount") == HttpStatusCode.OK)
                    {
                        Driver.FindElement(By.ClassName("order-history-body-item-row")).Click();
                        GetConsoleLog(template);
                    }

                    //выйти, проверить страницы: регистрация, вход, забыли пароль
                    ReInitClient(useTrial: useTrial);
                    GoToClient("login");
                    CheckPageErrors(template, "login");
                    GoToClient("registration");
                    CheckPageErrors(template, "registration");
                    GoToClient("forgotpassword");
                    CheckPageErrors(template, "forgotpassword");
                }
                else
                {
                    CheckPageErrors(template, "", "Home page");
                }
            }
            catch (Exception ex)
            {
                VerifyIsTrue(false, "Template " + template + ", message: " + ex.Message, false);
            }
            finally
            {
                ReInit(useTrial: useTrial);
            }
        }

        [Test]
        public void MobileCheckPagesCommon([ValueSource(nameof(templates))] string template)
        {
            IEnumerable<string> pagesMobile =
                Functions.LoadCsvFile("Data\\Client\\TestSettings\\TestingPagesMobileData.csv", true);

            try
            {
                SetTemplate(template);

                GoToMobile();
                if (GetPageStatus("") == HttpStatusCode.OK)
                {
                    //прохожусь по тем страницам, где делать ничего совсем не надо и которые и так доступны
                    foreach (var page in pagesMobile)
                    {
                        GoToClient(page);
                        Sleep();
                        CheckPageErrors(template, page, "modern mobile., page" + page);
                    }
                }
                else
                {
                    CheckPageErrors(template, "", "modern mobile., Home page");
                }
            }
            catch (Exception ex)
            {
                VerifyIsTrue(false, "Template " + template + ", мобилка, message: " + ex.Message, false);
            }
            finally
            {
                ReInit(useTrial: useTrial);
            }
        }


        //добавить тест на pageSpeeder - для 37./dev

        //добавить тест, который бы проверял, работают ли настройки

        //добавить тест на триалках для проверки иконок при домене trial/3243241/templates...
    }
}