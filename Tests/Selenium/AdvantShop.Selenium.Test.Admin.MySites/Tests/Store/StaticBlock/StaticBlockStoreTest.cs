using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.StaticBlock
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class StaticBlockStoreTest : MySitesFunctions
    {
        private string sbPageUrl = "staticpages#?staticPageTab=blocks";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();
            Init();

            EnableInplaceOn();
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
        public void BlockInplaceNOTCOMPLETE()
        {
            string sbKey = "bannerDetails";
            string sbContent = sbKey + " - text content";
            string sbLocation = "products/vash-tovar-1";
            By sbXPath = By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" + sbContent + "')]");
            //By sbInplaceXPath = By.XPath("//div[contains(@class, 'static-block inplace-initialized')][@data-static-block-key='bannerDetails'][contains(text(), '" + sbContent + "')]");
            By sbInplaceXPath =
                By.XPath(
                    "//div[contains(@class, 'static-block')][@data-static-block-key='bannerDetails'][contains(text(), '" +
                    sbContent + "')]");

            EnableInplaceOff();
            GoToAdmin(sbPageUrl);

            OpenStaticBlockEditor(sbKey);
            VerifyIsTrue(GetCheckboxState(Driver.GetByE2E("StaticBlockEnabled").FindElement(By.TagName("input"))),
                sbKey + " enabled default");
            Driver.SetCkText(sbContent, "editor1");
            SaveStaticBlock();

            GoToClient(sbLocation);
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "staticBlock at desktop without inplace");
            VerifyAreEqual(0, Driver.FindElements(sbInplaceXPath).Count,
                "staticBlocks inplace selector at desktop without inplace");
            GoToMobile(sbLocation);
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "staticBlock at mobile without inplace");
            VerifyAreEqual(0, Driver.FindElements(sbInplaceXPath).Count,
                "staticBlocks inplace selector at mobile without inplace");

            ReInitClient();
            GoToClient(sbLocation);
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "staticBlock at desktop without inplace - client");
            VerifyAreEqual(0, Driver.FindElements(sbInplaceXPath).Count,
                "staticBlocks inplace selector at desktop without inplace - client");
            GoToMobile(sbLocation);
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "staticBlock at mobile without inplace - client");
            VerifyAreEqual(0, Driver.FindElements(sbInplaceXPath).Count,
                "staticBlocks inplace selector at mobile without inplace - client");

            ReInit();
            EnableInplaceOn();

            GoToClient(sbLocation);
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "staticBlock at desktop with inplace");
            VerifyAreEqual(1, Driver.FindElements(sbInplaceXPath).Count,
                "staticBlocks inplace selector at desktop with inplace");
            GoToMobile(sbLocation);
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "staticBlock at mobile with inplace");
            VerifyAreEqual(1, Driver.FindElements(sbInplaceXPath).Count,
                "staticBlocks inplace selector at mobile with inplace");

            ReInitClient();
            GoToClient(sbLocation);
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "staticBlock at desktop with inplace - client");
            VerifyAreEqual(0, Driver.FindElements(sbInplaceXPath).Count,
                "staticBlocks inplace selector at desktop with inplace - client");
            GoToMobile(sbLocation);
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "staticBlock at mobile with inplace - client");
            VerifyAreEqual(0, Driver.FindElements(sbInplaceXPath).Count,
                "staticBlocks inplace selector at mobile with inplace - client");

            ReInit();

            //inplace-редактирование блока
            GoToClient(sbLocation);
            Driver.FindElement(sbInplaceXPath).Click();
            Thread.Sleep(500);
            Driver.FindElement(sbInplaceXPath).SendKeys(Keys.Control + "a" + Keys.Delete);
            Thread.Sleep(500);
            Driver.FindElement(
                    By.XPath("//div[contains(@class, 'static-block')][@data-static-block-key='bannerDetails']"))
                .SendKeys("this text was edited in inplace mode");
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("site-head-phone")).Click();
            Thread.Sleep(500);
            Refresh();
            VerifyAreEqual(0, Driver.FindElements(sbXPath).Count, "staticBlock with old text is not found");
            VerifyAreEqual(1,
                Driver.FindElements(By.XPath(
                        "//div[contains(@class, 'static-block')][@data-static-block-key='bannerDetails'][contains(text(), 'this text was edited in inplace mode')]"))
                    .Count, "staticBlock with new text is found");

            //https://task.advant.me/adminv3/tasks#?modal=22704 - после этой задачи 
            //либо поменять (staticBlocks inplace selector at mobile with inplace) строке на 0, 
            //либо дописать тест на редактирование из мобилки
            VerifyIsTrue(false, "test not work");
        }

        [Test]
        public void RenderActiveBlock()
        {
            string sbKey = "TextOnMain2";
            string sbContent = sbKey + " - text content";
            By sbXPath = By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" + sbContent + "')]");

            EnableInplaceOff();
            GoToAdmin(sbPageUrl);
            //включить блок, 
            //перейти на главную, найти блок.
            //перейти в мобилку, найти блок.
            //выключить блок, 
            //перейти на главную, не найти блок. 
            //перейти в мобилку, не найти блок. 
            //вернуть блок в исходное состояние.

            OpenStaticBlockEditor(sbKey);
            VerifyIsTrue(GetCheckboxState(Driver.GetByE2E("StaticBlockEnabled").FindElement(By.TagName("input"))),
                "staticBlock enabled default");
            Driver.SetCkText(sbContent, "editor1");
            SaveStaticBlock();

            GoToClient();
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "enable staticBlock at desktop");
            VerifyIsTrue(Driver.PageSource.IndexOf(sbContent) != -1, "enable staticBlock text at desktop");
            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "enable staticBlock at mobile");
            VerifyIsTrue(Driver.PageSource.IndexOf(sbContent) != -1, "enable staticBlock text at mobile");
            ReInitClient();
            GoToClient();
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "enable staticBlock at desktop - client");
            VerifyIsTrue(Driver.PageSource.IndexOf(sbContent) != -1, "enable staticBlock text at desktop - client");
            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(sbXPath).Count, "enable staticBlock at mobile - client");
            VerifyIsTrue(Driver.PageSource.IndexOf(sbContent) != -1, "enable staticBlock text at mobile - client");

            ReInit();
            GoToAdmin(sbPageUrl);
            OpenStaticBlockEditor(sbKey);
            Driver.CheckBoxUncheck("[data-e2e=\"StaticBlockEnabled\"]", "CssSelector");
            SaveStaticBlock();

            GoToClient();
            VerifyAreEqual(0, Driver.FindElements(sbXPath).Count, "disable staticBlock at desktop");
            VerifyIsFalse(Driver.PageSource.IndexOf(sbContent) != -1, "disable staticBlock text at desktop");
            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(sbXPath).Count, "disable staticBlock at mobile");
            VerifyIsFalse(Driver.PageSource.IndexOf(sbContent) != -1, "disable staticBlock text at mobile");
            ReInitClient();
            GoToClient();
            VerifyAreEqual(0, Driver.FindElements(sbXPath).Count, "disable staticBlock at desktop - client");
            VerifyIsFalse(Driver.PageSource.IndexOf(sbContent) != -1, "disable staticBlock text at desktop - client");
            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(sbXPath).Count, "disable staticBlock at mobile - client");
            VerifyIsFalse(Driver.PageSource.IndexOf(sbContent) != -1, "disable staticBlock text at mobile - client");

            ReInit();
            EnableInplaceOn();
        }

        [Test]
        public void RenderBlockByKey()
        {
            string sbKey = "bottom_menu_right";
            string sbContent = sbKey + " - text content";
            string sbXPath = "//div[contains(@class, 'static-block')]";
            By sbInplaceXPath =
                By.XPath("//div[contains(@class, 'static-block')][@data-static-block-key='" + sbKey + "']");

            GoToAdmin(sbPageUrl);
            OpenStaticBlockEditor(sbKey);
            Driver.SetCkText(sbContent, "editor1");
            SaveStaticBlock();

            GoToClient();
            VerifyAreEqual(1, Driver.FindElements(sbInplaceXPath).Count, "enable staticBlock key at desktop");
            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(sbInplaceXPath).Count, "enable staticBlock key at mobile");
            ReInitClient();
            GoToClient();
            VerifyAreEqual(1, Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + sbContent + "')]")).Count,
                "enable staticBlock key at desktop - client");
            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + sbContent + "')]")).Count,
                "enable staticBlock key at mobile - client");

            ReInit();
            GoToAdmin(sbPageUrl);
            OpenStaticBlockEditor(sbKey);
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "bottom_menu_right_edited");
            SaveStaticBlock();

            GoToClient();
            VerifyAreEqual(0, Driver.FindElements(sbInplaceXPath).Count, "not enable staticBlock key at desktop");
            VerifyAreEqual(0,
                Driver.FindElements(By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" + sbContent +
                                             "')]")).Count, "not enable staticBlock key at desktop(2)");
            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(sbInplaceXPath).Count, "not enable staticBlock key at mobile");
            VerifyAreEqual(0,
                Driver.FindElements(By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" + sbContent +
                                             "')]")).Count, "not enable staticBlock key at mobile(2)");
            ReInitClient();
            GoToClient();
            VerifyAreEqual(0, Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + sbContent + "')]")).Count,
                "not enable staticBlock key at desktop - client");
            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + sbContent + "')]")).Count,
                "not enable staticBlock key at mobile - client");

            ReInit();
            GoToAdmin(sbPageUrl);
            string newSbContent = "New static block with enable key content";
            Driver.GetByE2E("btnAdd").Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), sbKey);
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "New static block with enable key");
            Driver.CheckBoxCheck("[data-e2e=\"StaticBlockEnabled\"]", "CssSelector");
            Driver.SetCkText(newSbContent, "editor1");
            SaveStaticBlock();

            GoToClient();
            VerifyAreEqual(1, Driver.FindElements(sbInplaceXPath).Count, "new enable staticBlock key at desktop");
            VerifyAreEqual(0,
                Driver.FindElements(By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" + sbContent +
                                             "')]")).Count, "old staticBlock content at desktop is not shown");
            VerifyAreEqual(1,
                Driver.FindElements(By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" +
                                             newSbContent + "')]")).Count,
                "new staticBlock content at desktop is shown");
            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(sbInplaceXPath).Count, "not enable staticBlock key at mobile");
            VerifyAreEqual(0,
                Driver.FindElements(By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" + sbContent +
                                             "')]")).Count, "old staticBlock content at mobile is not shown");
            VerifyAreEqual(1,
                Driver.FindElements(By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" +
                                             newSbContent + "')]")).Count,
                "new staticBlock content at mobile is shown");
            ReInitClient();
            GoToClient();
            VerifyAreEqual(0, Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + sbContent + "')]")).Count,
                "old staticBlock content at desktop is not shown - client");
            VerifyAreEqual(1,
                Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + newSbContent + "')]")).Count,
                "new staticBlock content at desktop is shown - client");
            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + sbContent + "')]")).Count,
                "old staticBlock content at mobile is not shown - client");
            VerifyAreEqual(1,
                Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + newSbContent + "')]")).Count,
                "new staticBlock content at mobile is shown - client");

            ReInit();
            GoToAdmin(sbPageUrl);
            OpenStaticBlockEditor(sbKey);
            Driver.SetCkText(sbContent, "editor1");
            SaveStaticBlock();
            GoToClient();
            VerifyAreEqual(1, Driver.FindElements(sbInplaceXPath).Count, "new enable staticBlock key at desktop");
            VerifyAreEqual(1,
                Driver.FindElements(By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" + sbContent +
                                             "')]")).Count, "changed staticBlock content at desktop is shown");
            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(sbInplaceXPath).Count, "not enable staticBlock key at mobile");
            VerifyAreEqual(1,
                Driver.FindElements(By.XPath("//div[contains(@class, 'static-block')][contains(text(), '" + sbContent +
                                             "')]")).Count, "changed staticBlock content at mobile is shown");
            ReInitClient();
            GoToClient();
            VerifyAreEqual(1, Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + sbContent + "')]")).Count,
                "changed staticBlock content at desktop is shown - client");
            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(By.XPath(sbXPath + "[contains(text(), '" + sbContent + "')]")).Count,
                "changed staticBlock content at mobile is shown - client");

            ReInit();
        }

        [Test]
        public void RenderBlockContent()
        {
            string sbKey = "DescriptionDetails";
            string sbLocation = "products/vash-tovar-1";
            //string sbXPath = "//div[contains(@class, 'static-block')]";

            //текст, очень длинный текст, верстка, 
            //верстка со стилями в строке, стили в теге,
            //картинка, ссылка, скрипт,  

            string sbHtml = "Short text string";
            string sbText = sbHtml;
            SetStaticBlockHtml(sbKey, sbHtml);
            CheckBlockAllMode(sbKey, sbLocation, sbText, sbHtml, 1);

            sbHtml =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit " +
                "in voluptate velit esse cillum dolore eu fugiat nulla pariatur.";
            sbHtml = sbHtml + sbHtml + sbHtml;
            sbText = sbHtml;
            SetStaticBlockHtml(sbKey, sbHtml);
            CheckBlockAllMode(sbKey, sbLocation, sbText, sbHtml, 2);

            sbHtml =
                "<div><b>Lorem ipsum &amp; &quot;dolor&quot; sit amet,</b> consectetur adipiscing elit, <br><br><span>sed do eiusmod tempor incididunt ut <div>labore et <u>dolore magna</u> aliqua</div>.</span></div>";
            sbText =
                "Lorem ipsum & \"dolor\" sit amet, consectetur adipiscing elit,\r\n\r\nsed do eiusmod tempor incididunt ut\r\nlabore et dolore magna aliqua\r\n.";
            SetStaticBlockHtml(sbKey, sbHtml);
            CheckBlockAllMode(sbKey, sbLocation, sbText, sbHtml.Replace("&quot;", "\""), 3);

            sbHtml =
                "<div><b style=\"font-size: 28px; color: blue;\">Lorem ipsum &amp; dolor sit amet,</b> consectetur adipiscing elit, <br><br><span class=\"flex between-xs\">sed do eiusmod tempor incididunt ut <div style=\"display: grid\">labore et <u>dolore magna</u> aliqua</div>.</span></div>";
            sbText =
                "Lorem ipsum & dolor sit amet, consectetur adipiscing elit,\r\n\r\nsed do eiusmod tempor incididunt ut\r\nlabore et\r\ndolore magna\r\naliqua\r\n.";
            SetStaticBlockHtml(sbKey, sbHtml);
            CheckBlockAllMode(sbKey, sbLocation, sbText, sbHtml, 4);

            sbHtml =
                "<style>.my-sb-styles{background-color: pink; font-size: 20px;}.my-sb-styles b{text-decoration: underline;}.sb-display-grid{display: grid;}</style><div class=\"my-sb-styles\"><b>Lorem ipsum &amp; dolor sit amet,</b> consectetur adipiscing elit, <br><br><span>sed do eiusmod tempor incididunt ut <div class=\"sb-display-grid\">labore et <u>dolore magna</u> aliqua</div>.</span></div>";
            sbText =
                "Lorem ipsum & dolor sit amet, consectetur adipiscing elit,\r\n\r\nsed do eiusmod tempor incididunt ut\r\nlabore et\r\ndolore magna\r\naliqua\r\n.";
            SetStaticBlockHtml(sbKey, sbHtml);
            CheckBlockAllMode(sbKey, sbLocation, sbText, sbHtml, 5);

            sbHtml = "<img alt=\"owl\" src=\"http://bipbap.ru/wp-content/uploads/2017/04/72fqw2qq3kxh.jpg\">";
            sbText = "";
            SetStaticBlockHtml(sbKey, sbHtml);
            CheckBlockAllMode(sbKey, sbLocation, sbText, sbHtml, 6);

            sbHtml = "<a href=\"https://www.advantshop.net/\" target=\"_blank\">Create store with AdvantShop!</a>";
            sbText = "Create store with AdvantShop!";
            SetStaticBlockHtml(sbKey, sbHtml);
            CheckBlockAllMode(sbKey, sbLocation, sbText, sbHtml, 7);

            EnableInplaceOff();
            GoToClient(sbLocation);
            Driver.FindElement(By.LinkText("Create store with AdvantShop!")).Click();
            Thread.Sleep(500);
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
            Thread.Sleep(500);
            VerifyAreEqual("https://www.advantshop.net/", Driver.Url, "link url");
            Thread.Sleep(500);
            Driver.SwitchTo().Window(Driver.WindowHandles[0]);
            EnableInplaceOn();

            sbHtml =
                "<div class=\"my-sb-div-for-script\">My not edited text.</div><script>document.querySelector(\".my-sb-div-for-script\").innerText = \"My new text in my-sb-div-for-script block\";</script>";
            sbText = "My new text in my-sb-div-for-script block";
            SetStaticBlockHtml(sbKey, sbHtml);

            By sbInplaceXPath =
                By.XPath("//div[contains(@class, 'static-block')][@data-static-block-key='" + sbKey + "']");
            By sbSelectorClient = By.CssSelector(".details-payment + .static-block");

            GoToClient(sbLocation);
            VerifyAreEqual("My not edited text.", GetBlockText(sbInplaceXPath), $"staticBlock text at desktop({8})");
            VerifyAreEqual(sbHtml, GetBlockInnerHtml(sbInplaceXPath), $"staticBlock html at desktop({8})");
            GoToMobile(sbLocation);
            VerifyAreEqual("My not edited text.", GetBlockText(sbInplaceXPath), $"staticBlock text at mobile({8})");
            VerifyAreEqual(sbHtml.Replace("<script>", "<script type=\"inplace\">"), GetBlockInnerHtml(sbInplaceXPath),
                $"staticBlock html at mobile({8})");
            ReInitClient();
            GoToClient(sbLocation);
            VerifyAreEqual(sbHtml.Replace("My not edited text.", sbText),
                GetBlockInnerHtml(sbSelectorClient), $"staticBlock at client desktop({8})");
            GoToMobile(sbLocation);
            VerifyAreEqual(sbHtml.Replace("My not edited text.", sbText),
                GetBlockInnerHtml(sbSelectorClient), $"staticBlock at client mobile desktop({8})");

            ReInit();

            EnableInplaceOff();

            GoToClient(sbLocation);
            VerifyAreEqual(sbText, GetBlockText(By.CssSelector(".static-block .my-sb-div-for-script")),
                $"staticBlock text at desktop(123)");
            GoToMobile(sbLocation);
            VerifyAreEqual(sbText, GetBlockText(By.CssSelector(".static-block .my-sb-div-for-script")),
                $"staticBlock text at mobile(123)");
            ReInitClient();
            GoToClient(sbLocation);
            VerifyAreEqual(sbText, GetBlockText(By.CssSelector(".static-block .my-sb-div-for-script")),
                $"staticBlock text at client desktop(123)");
            GoToMobile(sbLocation);
            VerifyAreEqual(sbText, GetBlockText(By.CssSelector(".static-block .my-sb-div-for-script")),
                $"staticBlock text at client mobile(123)");

            ReInit();

            EnableInplaceOn();
        }

        protected void OpenStaticBlockEditor(string sbKey)
        {
            Driver.GridFilterSendKeys(sbKey, By.ClassName("ui-grid-custom-filter-total"));
            Driver.GetGridCell(0, "Key", "Blocks").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
        }

        protected void SetStaticBlockHtml(string blockKey, string htmlString)
        {
            GoToAdmin(sbPageUrl);
            OpenStaticBlockEditor(blockKey);
            Driver.SetCkHtml(htmlString);
            Thread.Sleep(500);
            SaveStaticBlock();
        }

        protected void SaveStaticBlock()
        {
            Driver.FindElement(By.ClassName("btn-save")).Click();
        }

        protected string GetBlockText(By cssSelector)
        {
            return Driver.FindElement(cssSelector).Text;
        }

        protected string GetBlockInnerHtml(By cssSelector)
        {
            return Driver.FindElement(cssSelector).GetAttribute("innerHTML").Trim();
        }

        protected void CheckBlockAllMode(string sbKey, string url, string text, string html, int checkId)
        {
            By sbInplaceXPath =
                By.XPath("//div[contains(@class, 'static-block')][@data-static-block-key='" + sbKey + "']");
            By sbSelectorClient = By.CssSelector(".details-payment + .static-block");

            GoToClient(url);
            VerifyAreEqual(text, GetBlockText(sbInplaceXPath), $"staticBlock text at desktop({checkId})");
            VerifyAreEqual(html, GetBlockInnerHtml(sbInplaceXPath), $"staticBlock html at desktop({checkId})");
            GoToMobile(url);
            VerifyAreEqual(text, GetBlockText(sbInplaceXPath), $"staticBlock text at mobile({checkId})");
            VerifyAreEqual(html, GetBlockInnerHtml(sbInplaceXPath), $"staticBlock html at mobile({checkId})");
            ReInitClient();
            GoToClient(url);
            VerifyAreEqual(html, GetBlockInnerHtml(sbSelectorClient), $"staticBlock at client desktop({checkId})");
            GoToMobile(url);
            VerifyAreEqual(html, GetBlockInnerHtml(sbSelectorClient), $"staticBlock at client mobile({checkId})");

            ReInit();
        }
    }
}