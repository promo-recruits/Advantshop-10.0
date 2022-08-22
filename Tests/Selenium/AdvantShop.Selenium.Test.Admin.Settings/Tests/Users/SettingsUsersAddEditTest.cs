using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users
{
    [TestFixture]
    public class SettingsUsersAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Users\\UserAddEdit\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\Users\\UserAddEdit\\Customers.Customer.csv",
                "data\\Admin\\Settings\\Users\\UserAddEdit\\Customers.Departments.csv",
                "data\\Admin\\Settings\\Users\\UserAddEdit\\Customers.Managers.csv",
                "data\\Admin\\Settings\\Users\\UserAddEdit\\Customers.ManagerRole.csv",
                "data\\Admin\\Settings\\Users\\UserAddEdit\\Customers.ManagerRolesMap.csv"
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
        public void SettingsUsersAdd()
        {
            GoToAdmin("settings/userssettings#?tab=Users");

            //  Driver.GetButton(eButtonType.Add).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnAdd\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Новый сотрудник", Driver.FindElement(By.TagName("h2")).Text, "h2 add new user");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userLastName\"]")).SendKeys("NewSurname");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userFirstName\"]")).SendKeys("NewName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userEmail\"]")).SendKeys("NewEmail@mail.ru");

            //add img from computer
            VerifyIsFalse(Driver.FindElements(By.XPath("//a[contains(text(),\"Удалить\")]")).Count > 0);
            Driver.FindElement(By.LinkText("Загрузить фото")).Click();
            Driver.FindElement(By.CssSelector("input[type=\"file\"]")).SendKeys(GetPicturePath("avatar.jpg"));
            VerifyAreEqual("Загрузка изображения", Driver.FindElement(By.TagName("h2")).Text, "h2 add new user img");
            Driver.FindElement(By.XPath("//button[contains(text(),\"Применить\")]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).SendKeys("PositionTest");

            //check all departments
            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"userDepartment\"]"));
            SelectElement select = new SelectElement(selectElem);

            IList<IWebElement> allOptionsDepart = select.Options;

            VerifyIsTrue(allOptionsDepart.Count == 10,
                "count departments"); //1 from 10 departments disabled + null select

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"userDepartment\"]")))).SelectByText(
                "Department9");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userPhone\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userPhone\"]")).SendKeys("+79278888888");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userPhone\"]"));

            //birthday by calendar
            //  driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).Click();
            //  Functions.DataTimePickerFilter(driver, baseURL, Monthfrom: "Январь", Yearfrom: "2013", Datafrom: "Январь 17, 2013", tagH: "h2", fieldFrom: "[data-e2e=\"userBirthDay\"]");
            //  Driver.WaitForAjax();

            Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).SendKeys("17.01.2013");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"userCity\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userCity\"]")).SendKeys("CityTest");

            //check all head users
            IWebElement selectElem2 = Driver.FindElement(By.CssSelector("[data-e2e=\"userHeadUser\"]"));
            SelectElement select2 = new SelectElement(selectElem2);

            IList<IWebElement> allOptionsHeadUser = select2.Options;

            VerifyIsTrue(allOptionsHeadUser.Count == 222, "count head users"); //all 221 users + null select

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"userHeadUser\"]")))).SelectByText(
                "testlastname200 testfirstname200");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"userPermissionsModerInput\"]")).Selected,
                "selected moderator");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userEnabled\"]"));
            //choose role from existing
            Driver.FindElement(By.XPath("//input[@type='search']")).Click();
            Driver.WaitForElem(By.CssSelector("span.ui-select-choices-row-inner"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userHeadUser\"]"));
            Driver.FindElement(By.CssSelector("span.ui-select-choices-row-inner")).Click();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "selected user enabled");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userButtonSave\"]")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            //check admin
            GoToAdmin("settings/userssettings#?tab=Users");

            VerifyAreEqual("Найдено записей: 222",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after adding");

            Driver.GetGridIdFilter("gridUsers", "NewName NewSurname");

            Driver.XPathContainsText("h1", "Сотрудники");


            VerifyIsTrue(
                !Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "avatar");

            VerifyAreEqual("NewName NewSurname", Driver.GetGridCell(0, "FullName", "Users").Text, "FullName");
            VerifyAreEqual("NewEmail@mail.ru", Driver.GetGridCell(0, "Email", "Users").Text, "Email");
            VerifyAreEqual("Department9", Driver.GetGridCell(0, "DepartmentName", "Users").Text, "DepartmentName");
            VerifyAreEqual("Модератор", Driver.GetGridCell(0, "Permissions", "Users").Text, "Permissions");
            VerifyAreEqual("Role1", Driver.GetGridCell(0, "Roles", "Users").Text, "Roles");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "Enabled");

            //check edit pop up
            Driver.GetGridCell(0, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("NewSurname",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userLastName\"]")).GetAttribute("value"),
                "edit pop up last name");
            VerifyAreEqual("NewName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userFirstName\"]")).GetAttribute("value"),
                "edit pop up first name");
            VerifyAreEqual("NewEmail@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEmail\"]")).GetAttribute("value"),
                "edit pop up email");
            VerifyIsTrue(Driver.FindElements(By.XPath("//a[contains(text(),\"Удалить\")]")).Count > 0,
                "edit pop up img saved");
            VerifyAreEqual("PositionTest",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).GetAttribute("value"),
                "edit pop up position");

            IWebElement selectElem3 = Driver.FindElement(By.CssSelector("[data-e2e=\"userDepartment\"]"));
            SelectElement select3 = new SelectElement(selectElem3);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Department9"), "edit pop up department");

            VerifyAreEqual("+7(927)888-88-88",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userPhone\"]")).GetAttribute("value"),
                "edit pop up phone");
            VerifyAreEqual("17.01.2013",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).GetAttribute("value"),
                "edit pop up birthday");
            VerifyAreEqual("CityTest",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userCity\"]")).GetAttribute("value"),
                "edit pop up city");

            IWebElement selectElem4 = Driver.FindElement(By.CssSelector("[data-e2e=\"userHeadUser\"]"));
            SelectElement select4 = new SelectElement(selectElem4);
            VerifyIsTrue(select4.AllSelectedOptions[0].Text.Contains("testlastname200 testfirstname200"),
                "edit pop up head user");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Role1"),
                "edit pop up role");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"userPermissionsModerInput\"]")).Selected,
                "edit pop up selected moderator");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "edit pop up selected user enabled");
        }

        [Test]
        public void SettingsUsersEdit()
        {
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname9 testlastname9");
            Driver.XPathContainsText("h1", "Сотрудники");


            //pre check edit pop up
            VerifyIsTrue(Driver.GetGridCell(0, "FullName", "Users").Text.Contains("testfirstname9"),
                "pre check user to edit");
            string imgFirstGrid = Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img"))
                .GetAttribute("src");
            Driver.GetGridCell(0, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование сотрудника", Driver.FindElement(By.TagName("h2")).Text, "h2 edit user");

            VerifyAreEqual("testlastname9",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userLastName\"]")).GetAttribute("value"),
                "pre check edit pop up last name");
            VerifyAreEqual("testfirstname9",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userFirstName\"]")).GetAttribute("value"),
                "pre check edit pop up first name");
            VerifyAreEqual("testmail@mail.ru9",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEmail\"]")).GetAttribute("value"),
                "pre check edit pop up email");
            VerifyIsTrue(Driver.FindElements(By.XPath("//a[contains(text(),\"Удалить\")]")).Count > 0,
                "pre check edit pop up img");
            string imgFirstPopUp = Driver.FindElement(By.TagName("img")).GetAttribute("src");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).GetAttribute("value"),
                "pre check edit pop up position");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"userDepartment\"]"));
            SelectElement select1 = new SelectElement(selectElem1);
            VerifyIsTrue(select1.AllSelectedOptions[0].Text.Contains("Не выбран"), "pre check department text");

            VerifyIsTrue(select1.AllSelectedOptions[0].GetAttribute("selected").Equals("true"),
                "pre check department value");

            VerifyAreEqual("+7(900)487-50-09",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userPhone\"]")).GetAttribute("value"),
                "pre check edit pop up phone");
            VerifyAreEqual("04.01.2003",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).GetAttribute("value"),
                "pre check edit pop up birthday");
            VerifyAreEqual("Москва",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userCity\"]")).GetAttribute("value"),
                "pre check edit pop up city");

            IWebElement selectElem2 = Driver.FindElement(By.CssSelector("[data-e2e=\"userHeadUser\"]"));
            SelectElement select2 = new SelectElement(selectElem2);
            VerifyIsTrue(select2.AllSelectedOptions[0].Text.Contains("testlastname221 admin"), "pre check head user");

            VerifyAreEqual("", Driver.FindElement(By.XPath("//input[@type='search']")).GetAttribute("value"),
                "pre check edit pop up role");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"userPermissionsAdminInput\"]")).Selected,
                "edit pop up selected admin");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "edit pop up selected user disabled");

            //check edit
            Driver.FindElement(By.CssSelector("[data-e2e=\"userLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userLastName\"]")).SendKeys("Edited Surname");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userFirstName\"]")).SendKeys("Edited Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userEmail\"]")).SendKeys("EditedEmail@mail.ru");

            //add img by link
            Driver.FindElement(By.LinkText("Загрузить фото")).Click();
            Driver.FindElement(By.LinkText("Загрузить по ссылке")).Click();
            Driver.FindElement(By.CssSelector("input[type=\"text\"]"))
                .SendKeys("http://www.bugaga.ru/uploads/posts/2016-02/1454695389_prikol-7.jpg");
            Driver.FindElement(By.LinkText("Загрузить")).Click();
            Driver.FindElement(By.XPath("//button[contains(text(),\"Применить\")]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).SendKeys("Edited PositionTest");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"userDepartment\"]")))).SelectByText(
                "Department5");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userPhone\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"userPhone\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"userPhone\"]")).SendKeys("+79279999999");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userPhone\"]"));

            //birthday by print
            Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).SendKeys("23.10.1990");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"userCity\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userCity\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userCity\"]")).SendKeys("Edited CityTest");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"userHeadUser\"]")))).SelectByText(
                "testlastname111 testfirstname111");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userPermissionsModer\"]")).Click();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userEnabled\"]"));

            //choose role from existing
            Driver.FindElement(By.XPath("//input[@type='search']")).Click();
            Driver.WaitForElem(By.CssSelector("span.ui-select-choices-row-inner"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userHeadUser\"]"));
            Driver.FindElement(By.CssSelector("span.ui-select-choices-row-inner")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"userEnabled\"]")).FindElement(By.TagName("span")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"userButtonSave\"]")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            //check admin
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname9 testlastname9");
            Driver.XPathContainsText("h1", "Сотрудники");


            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "edited user");

            Driver.GetGridIdFilter("gridUsers", "Edited Name Edited Surname");
            Driver.XPathContainsText("h1", "Сотрудники");


            VerifyIsTrue(
                !Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "avatar");

            VerifyAreEqual("Edited Name Edited Surname", Driver.GetGridCell(0, "FullName", "Users").Text, "FullName");
            VerifyAreEqual("EditedEmail@mail.ru", Driver.GetGridCell(0, "Email", "Users").Text, "Email");
            VerifyAreEqual("Department5", Driver.GetGridCell(0, "DepartmentName", "Users").Text, "DepartmentName");
            VerifyAreEqual("Role1", Driver.GetGridCell(0, "Roles", "Users").Text, "Roles");
            VerifyAreEqual("Модератор", Driver.GetGridCell(0, "Permissions", "Users").Text, "Permissions");

            string imgSecondGrid = Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img"))
                .GetAttribute("src");
            VerifyIsFalse(imgFirstGrid.Equals(imgSecondGrid), "grid another img saved");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "Enabled");

            //check edit pop up
            Driver.GetGridCell(0, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Edited Surname",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userLastName\"]")).GetAttribute("value"),
                "edit pop up last name");
            VerifyAreEqual("Edited Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userFirstName\"]")).GetAttribute("value"),
                "edit pop up first name");
            VerifyAreEqual("EditedEmail@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEmail\"]")).GetAttribute("value"),
                "edit pop up email");
            VerifyIsTrue(Driver.FindElements(By.XPath("//a[contains(text(),\"Удалить\")]")).Count > 0,
                "edit pop up img saved");
            string imgSecondPopUp = Driver.FindElement(By.TagName("img")).GetAttribute("src");
            VerifyIsFalse(imgFirstPopUp.Equals(imgSecondPopUp), "edit pop up another img saved");
            VerifyAreEqual("Edited PositionTest",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userPosition\"]")).GetAttribute("value"),
                "edit pop up position");

            IWebElement selectElem3 = Driver.FindElement(By.CssSelector("[data-e2e=\"userDepartment\"]"));
            SelectElement select3 = new SelectElement(selectElem3);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Department5"), "edit pop up department");

            VerifyAreEqual("+7(927)999-99-99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userPhone\"]")).GetAttribute("value"),
                "edit pop up phone");
            VerifyAreEqual("23.10.1990",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userBirthDay\"]")).GetAttribute("value"),
                "edit pop up birthday");
            VerifyAreEqual("Edited CityTest",
                Driver.FindElement(By.CssSelector("[data-e2e=\"userCity\"]")).GetAttribute("value"),
                "edit pop up city");

            IWebElement selectElem4 = Driver.FindElement(By.CssSelector("[data-e2e=\"userHeadUser\"]"));
            SelectElement select4 = new SelectElement(selectElem4);
            VerifyIsTrue(select4.AllSelectedOptions[0].Text.Contains("testlastname111 testfirstname111"),
                "edit pop up head user");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Role1"),
                "edit pop up role");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"userPermissionsModerInput\"]")).Selected,
                "edit pop up selected moderator");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "edit pop up selected user enabled");
        }

        [Test]
        public void SettingsUsersEditDelImg()
        {
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname26 testlastname26");
            Driver.XPathContainsText("h1", "Сотрудники");

            //pre check edit pop up
            VerifyAreEqual("Модератор", Driver.GetGridCell(0, "Permissions", "Users").Text,
                "pre check grid Permissions");
            VerifyIsTrue(
                !Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "pre check grid avatar");

            Driver.GetGridCell(0, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyIsTrue(Driver.FindElements(By.XPath("//a[contains(text(),\"Удалить\")]")).Count > 0,
                "pre check edit pop up img");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"userPermissionsModerInput\"]")).Selected,
                "pre check edit pop up selected moderator");
            VerifyAreEqual("", Driver.FindElement(By.XPath("//input[@type='search']")).GetAttribute("value"),
                "pre check edit pop up role");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "pre check edit pop up selected user enabled");

            //check edit
            Driver.FindElement(By.XPath("//a[contains(text(),\"Удалить\")]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userPermissionsAdmin\"]")).Click();
            Thread.Sleep(100);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userEnabled\"]"));

            //choose 2 roles from existing
            Driver.FindElement(By.XPath("//input[@type='search']")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".ui-select-choices-row-inner span")).Click();
            // Driver.XPathContainsText("span", "Role1");
            Thread.Sleep(100);
            Driver.FindElement(By.XPath("//input[@type='search']")).Click();
            Thread.Sleep(100);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userHeadUser\"]"));
            Driver.FindElement(By.CssSelector("#ui-select-choices-row-0-2")).Click();

            // Driver.XPathContainsText("span", "Role4");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"userEnabled\"]")).FindElement(By.TagName("span")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"userButtonSave\"]")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
            //check admin
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname26 testlastname26");
            Driver.XPathContainsText("h1", "Сотрудники");

            VerifyIsTrue(
                Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "avatar");

            VerifyAreEqual("testfirstname26 testlastname26", Driver.GetGridCell(0, "FullName", "Users").Text,
                "FullName");
            VerifyAreEqual("Role1, Role4", Driver.GetGridCell(0, "Roles", "Users").Text, "Roles");
            VerifyAreEqual("Администратор", Driver.GetGridCell(0, "Permissions", "Users").Text, "Permissions");

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "Enabled");

            //check edit pop up
            Driver.GetGridCell(0, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyIsFalse(Driver.FindElements(By.XPath("//a[contains(text(),\"Удалить\")]")).Count > 0,
                "edit pop up img delete");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Role1"),
                "edit pop up role 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Role4"),
                "edit pop up role 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"userPermissionsAdminInput\"]")).Selected,
                "edit pop up selected admin");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"userEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "edit pop up selected user disabled");
        }

        [Test]
        public void SettingsUsersEditDelRoles()
        {
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname218 testlastname218");
            Driver.XPathContainsText("h1", "Сотрудники");


            //pre check edit pop up
            VerifyAreEqual("testfirstname218 testlastname218", Driver.GetGridCell(0, "FullName", "Users").Text,
                "pre check grid FullName");
            VerifyAreEqual("Role3", Driver.GetGridCell(0, "Roles", "Users").Text, "pre check grid Roles");
            VerifyAreEqual("Модератор", Driver.GetGridCell(0, "Permissions", "Users").Text,
                "pre check grid Permissions");

            Driver.GetGridCell(0, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Role3"),
                "pre check edit pop up role");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userEnabled\"]"));
            //check edit delete role
            Driver.FindElement(By.CssSelector(".close.ui-select-match-close")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"userButtonSave\"]")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            //check admin
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname218 testlastname218");
            Driver.XPathContainsText("h1", "Сотрудники");


            VerifyAreEqual("testfirstname218 testlastname218", Driver.GetGridCell(0, "FullName", "Users").Text,
                "FullName");
            VerifyAreEqual("", Driver.GetGridCell(0, "Roles", "Users").Text, "Roles");
            VerifyAreEqual("Модератор", Driver.GetGridCell(0, "Permissions", "Users").Text, "Permissions");

            //check edit pop up
            Driver.GetGridCell(0, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("", Driver.FindElement(By.XPath("//input[@type='search']")).GetAttribute("value"),
                "edit pop up role 1");
        }
    }
}