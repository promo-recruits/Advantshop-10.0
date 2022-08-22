using System;
using System.Net;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.FromTemplate
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class SurveysCreateTest : MySitesFunctions
    {
        string landingId;
        string landingName;
        string landingTab = "Опросники";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

            Init(false);

            GoToAdmin("leads?salesFunnelId=1");
            Driver.FindElement(AdvBy.DataE2E("UseGrid")).Click();
            Thread.Sleep(1000);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin("dashboard/createsite");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        #region Surveys

        [Test]
        public void QuestionnaireCreate()
        {
            landingId = "Questionnaire";
            landingName = "Воронка \"Анкета\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "content page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel content page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel content page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons content page count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void QuizFunnelNewCreate()
        {
            landingId = "QuizFunnelNew";
            landingName = "Воронка \"Квиз\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "thanks page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");
                VerifyAreEqual(String.Empty,
                    Driver.FindElement(By.ClassName("lp-block-quiz")).GetAttribute("innerHTML").Trim(),
                    "booking not shown");
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }

            Driver.Navigate().GoToUrl(BaseUrl + "/adminv3/modules/details/Quizzes");
            Driver.FindElement(By.ClassName("form-label-block")).Click();
            Thread.Sleep(1000);
        }

        #endregion
    }

    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class SurveysCheckFunctionalityTest : MySitesFunctions
    {
        string landingId;
        string defaultDomain = "http://mydomain123.ru/";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\FromTemplate\\Surveys\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Surveys\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Surveys\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Surveys\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Surveys\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Surveys\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Surveys\\CMS.LandingSubBlock.csv"
                );

            Init(false);

            GoToAdmin("leads?salesFunnelId=1");
            Driver.FindElement(AdvBy.DataE2E("UseGrid")).Click();
            Thread.Sleep(1000);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            ReInit();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        #region Surveys

        [Test]
        public void QuestionnaireCheck()
        {
            landingId = "Questionnaire";
            By waitElem = By.CssSelector(".lp-h2");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button, questionary + shop link at thanks page WITHOUT scroll to top button
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                VerifyAreEqual("Анкета", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "questionary header");
                VerifyAreEqual("Пожалуйста, ответьте на несколько вопросов, для нас очень важно знать Ваше мнение.",
                    Driver.FindElement(By.ClassName("lp-block-text-single__content")).Text.Trim(),
                    "questionary subheader");
                FillQuestionary("Questionary Name");

                Driver.WaitForElem(waitElem);
                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid=") != -1, "funnel thanks page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console client");
                VerifyAreEqual("Спасибо!", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "thanks page header");
                VerifyAreEqual("Мы уже обрабатываем Ваши результаты.",
                    Driver.FindElement(By.ClassName("lp-block-text-single__content")).Text.Trim(),
                    "thanks page subheader");
                VerifyAreEqual("MYDOMAIN123.RU",
                    Driver.FindElement(By.CssSelector(".lp-block-text-coupon__block a")).Text.Trim(), "href to shop");
                Driver.FindElement(By.CssSelector(".lp-block-text-coupon__block a")).Click();
                Thread.Sleep(2000);
                VerifyAreEqual(defaultDomain, Driver.Url, "shop url");

                CheckLeadInAdmin(landingId, 1);
                CheckLeadQuestionaryDescription("Questionary Name");
                //repeat for mobile
            }

            //check all pages of funnel mobile - first button, questionary + button at thanks page WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                VerifyAreEqual("Анкета", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "questionary header mobile");
                VerifyAreEqual("Пожалуйста, ответьте на несколько вопросов, для нас очень важно знать Ваше мнение.",
                    Driver.FindElement(By.ClassName("lp-block-text-single__content")).Text.Trim(),
                    "questionary subheader mobile");
                FillQuestionary("Mobile Questionary Name");

                Driver.WaitForElem(waitElem);
                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid=") != -1,
                    "funnel thanks page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console client mobile");
                VerifyAreEqual("Спасибо!", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "thanks page header mobile mobile");
                VerifyAreEqual("Мы уже обрабатываем Ваши результаты.",
                    Driver.FindElement(By.ClassName("lp-block-text-single__content")).Text.Trim(),
                    "thanks page subheader mobile");
                VerifyAreEqual("Перейти в магазин",
                    Driver.FindElement(By.CssSelector(".lp-block-form-two-buttons a")).Text.Trim(),
                    "href to shop mobile");
                Driver.FindElement(By.CssSelector(".lp-block-form-two-buttons a")).Click();
                Thread.Sleep(2000);
                VerifyIsTrue(Driver.Url.IndexOf(defaultDomain + "?lid=") != -1, "shop url");

                CheckLeadInAdmin(landingId, 2);
                CheckLeadQuestionaryDescription("Mobile Questionary Name");
                //repeat for mobile
            }
        }

        [Test]
        public void QuizFunnelNewCheck()
        {
            landingId = "QuizFunnelNew";
            By waitElem = By.CssSelector(".lp-h2");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //quize enabled
            {
                InstallModule(BaseUrl, "quizzes", true);
                GoToClient("lp/" + landingId);
                GoToFunnelPageFromLp(1, waitElem);
                //driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                VerifyAreEqual("Ваш Квиз блок", Driver.FindElement(By.ClassName("lp-products-view__title")).Text.Trim(),
                    "quizze header");
                VerifyAreEqual("Выбрать Квиз", Driver.FindElement(By.ClassName("lp-btn--primary")).Text.Trim(),
                    "quizze button");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Driver.WaitForElem(By.Id("tabQuizOptions"));
                Driver.FindElement(By.Id("tabQuizOptions")).Click();
                Thread.Sleep(500);
                SelectItem(AdvBy.DataE2E("QuizId"), "Квиз - Exit Popup");
                Driver.FindElement(AdvBy.DataE2E("SaveSettingsBtn")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                VerifyAreEqual("Квиз - Exit Popup", Driver.FindElement(By.TagName("h1")).Text.Trim(),
                    "header of quizze block");
            }

            //check all pages of funnel client - button, link WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                Driver.WaitForElem(waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                VerifyAreEqual("Подбор охранного комплекса", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "main page header");
                VerifyAreEqual("Пройти опрос", Driver.FindElement(By.ClassName("lp-btn--primary")).Text.Trim(),
                    "main page button header subheader");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(500);

                Driver.WaitForElem(waitElem);
                VerifyIsTrue(Driver.Url.IndexOf("/kviz") != -1, "funnel quizze page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel quizze page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel quizze page console client");
                VerifyAreEqual("Подбор охранного комплекса", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "quizze page header");

                Driver.FindElements(By.ClassName("quizzes-answer-image-wrap"))[0].Click();
                Driver.FindElement(By.ClassName("btn-quiz-next")).Click();
                Thread.Sleep(1000);

                VerifyAreEqual("Отправить", Driver.FindElement(By.ClassName("btn-quiz-next")).Text.Trim(),
                    "btn send quizze text");
                Driver.SendKeysInput(By.CssSelector(".form-field-input input"), "test@test.ru");
                Driver.FindElement(By.ClassName("btn-quiz-next")).Click();
                Thread.Sleep(1000);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("quizzes")).Text.IndexOf("Благодарим за ваше время и доверие!") !=
                    -1, "success text");

                ReInit();

                CheckQuizzeLead(landingId, 1);
                CheckQuizzeLeadDescription("- Да");
            }

            //check all pages of funnel client - button, link WITHOUT scroll to top button
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                Driver.WaitForElem(waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                VerifyAreEqual("Подбор охранного комплекса", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "main page header");
                VerifyAreEqual("Пройти опрос", Driver.FindElement(By.ClassName("lp-btn--primary")).Text.Trim(),
                    "main page button header subheader");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(500);

                Driver.WaitForElem(waitElem);
                VerifyIsTrue(Driver.Url.IndexOf("/kviz") != -1, "funnel quizze page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel quizze page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel quizze page console client");
                VerifyAreEqual("Подбор охранного комплекса", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "quizze page header");

                Driver.FindElements(By.ClassName("quizzes-answer-image-wrap"))[1].Click();
                Driver.FindElement(By.ClassName("btn-quiz-next")).Click();
                Thread.Sleep(1000);

                VerifyAreEqual("Отправить", Driver.FindElement(By.ClassName("btn-quiz-next")).Text.Trim(),
                    "btn send quizze text");
                Driver.SendKeysInput(By.CssSelector(".form-field-input input"), "test@test.ru");
                Driver.FindElement(By.ClassName("btn-quiz-next")).Click();
                Thread.Sleep(1000);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("quizzes")).Text.IndexOf("Благодарим за ваше время и доверие!") !=
                    -1, "success text");

                ReInit();

                CheckQuizzeLead(landingId, 2);
                CheckQuizzeLeadDescription("- Нет");
            }

            Driver.Navigate().GoToUrl(BaseUrl + "/adminv3/modules/details/Quizzes");
            Driver.FindElement(By.ClassName("form-label-block")).Click();
            Thread.Sleep(1000);
        }

        #endregion

        protected void FillQuestionary(string name)
        {
            Driver.FindElements(By.CssSelector(".lp-form__field input"))[0].SendKeys(name);
            Driver.FindElements(By.CssSelector(".lp-form__field input"))[1].SendKeys("test@test.ru");
            Driver.FindElements(By.CssSelector(".lp-form__field textarea"))[0].SendKeys("Question 1 text");
            Driver.FindElements(By.CssSelector(".lp-form__field textarea"))[1].SendKeys("Question 2 text");
            Driver.FindElements(By.CssSelector(".lp-form__field input"))[2].SendKeys("Question 3 text");

            Driver.FindElement(By.CssSelector(".lp-form__agreement label")).Click();
            Driver.FindElement(AdvBy.DataE2E("FormBtn")).Click();
            Thread.Sleep(1000);
        }

        protected void CheckLeadQuestionaryDescription(string name)
        {
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(By.TagName("simple-edit"));
            VerifyIsTrue(Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Имя: " + name) != -1,
                "questionary name");
            VerifyIsTrue(Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Email: test@test.ru") != -1,
                "questionary phone");
            VerifyIsTrue(
                Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Комментарий: Question 1 text") != -1,
                "questionary comment");
            VerifyIsTrue(
                Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Многострочный текст: Question 2 text") !=
                -1, "questionary textarea");
            VerifyIsTrue(Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Город: Question 3 text") != -1,
                "questionary city");
            Driver.FindElement(By.ClassName("lead-info-close")).Click();
        }

        protected void CheckQuizzeLead(string landingId, int leadsCount)
        {
            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            Driver.SetGridFilter(BaseUrl, "_noopColumnSources", "Модуль \"Квизы\"", true);
            VerifyAreEqual("Найдено записей: " + leadsCount,
                Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text, "lead from funnel");
        }

        protected void CheckQuizzeLeadDescription(string answer)
        {
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(By.TagName("simple-edit"));
            IWebElement formSource = Driver.FindElement(By.Name("leadForm"));
            VerifyIsTrue(formSource.Text.IndexOf("Квиз \"Квиз - Exit Popup\"") != -1, "quizze header");
            VerifyIsTrue(formSource.Text.IndexOf("Вы к нам вернетесь?") != -1, "quizze question");
            VerifyIsTrue(formSource.Text.IndexOf(answer) != -1, "quizze answer");
            Driver.FindElement(By.ClassName("lead-info-close")).Click();
        }
    }
}