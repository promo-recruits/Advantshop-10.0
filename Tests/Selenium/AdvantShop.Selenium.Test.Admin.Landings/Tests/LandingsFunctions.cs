using AdvantShop.Helpers;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests
{
    public class LandingsFunctions : BaseSeleniumTest
    {

        protected void CreateEmptyFunnel(string funnelName)
        {
            GoToAdmin("dashboard/createsite#?tabs=Presentations");
            Driver.FindElement(By.ClassName("last-item-empty-template-funnel")).Click();
            Driver.SendKeysInput(AdvBy.DataE2E("LandingSiteName"), funnelName);
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
            Thread.Sleep(1000);
        }

        protected void ClearLandingPicturesDirectory()
        {
            DirectoryInfo landings = new DirectoryInfo((GetSitePath() + "/" + "pictures/landing/"));
            if (landings.Exists)
            {
                foreach (var landing in landings.EnumerateDirectories())
                {
                    FileHelpers.DeleteDirectory(landing.FullName);
                }
            }
        }

        #region Main

        public void AddBlockByBtnBig(string category, string altblock)
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"AddBlockBtnBig\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBlockBtnBig\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".adv-modal-active:not(.ng-hide)"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"category_" + category + "\"]")).Click();
            Driver.FindElement(By.CssSelector("[alt=\"" + altblock + "\"]")).Click();
            Thread.Sleep(500);
        }

        public void AddBlockByBtnSml(int id_block, string category, string altblock)
        {
            Driver.FindElement(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"category_" + category + "\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[alt=\"" + altblock + "\"]")).Click();
            Thread.Sleep(2000);
        }

        public void MoveUpBlockByBtn(int id_block)
        {
            Driver.ScrollTo(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"));
            Driver.MouseFocus(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"));
            Driver.FindElement(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"MoveUpBlockBtn\"]")).Click();
            Thread.Sleep(2000);
        }

        public void MoveDownBlockByBtn(int id_block)
        {
            Driver.ScrollTo(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"));
            Driver.MouseFocus(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"));
            Driver.FindElement(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"MoveDownBlockBtn\"]")).Click();
            Thread.Sleep(2000);
        }

        public void DelBlockBtn(int id_block)
        {
            Driver.MouseFocus(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"));
            Driver.FindElement(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"))
                .FindElement(By.CssSelector(" [data-e2e=\"DelBlockBtn\"]")).Click();
            Thread.Sleep(500);
            Driver.WaitForElem(By.CssSelector(".swal2-cancel"));
            Driver.SwalConfirm();
            Thread.Sleep(2000);
        }

        public void DelBlockBtnCancel(int id_block)
        {
            Driver.MouseFocus(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"));
            Driver.FindElement(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"DelBlockBtn\"]")).Click();
            Thread.Sleep(500);
            Driver.WaitForElem(By.CssSelector(".swal2-cancel"));
            Driver.SwalCancel();
            Thread.Sleep(2000);
        }

        public void DelBlocksAll()
        {
            while (Driver.FindElements(By.CssSelector("[data-e2e=\"DelBlockBtn\"]")).Count > 0)
            {
                Driver.MouseFocus(By.CssSelector("blocks-constructor"));
                Driver.FindElement(By.CssSelector("[data-e2e=\"DelBlockBtn\"]")).Click();
                Thread.Sleep(500);
                Driver.SwalConfirm();
                Thread.Sleep(500);
            }
        }

        public void TabSelect(string idTabAction)
        {
            Driver.FindElement(By.Id(idTabAction)).Click();
            Thread.Sleep(500);
        }

        #endregion Main

        #region Settings

        public void BlockSettingsBtn(int id_block = 1)
        {
            Driver.MouseFocus(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"));
            Driver.FindElement(By.CssSelector("blocks-constructor[data-block-id=\"" + id_block + "\"]"))
                .FindElement(By.CssSelector(" [data-e2e=\"BlockSettingsBtn\"]")).Click();
            Thread.Sleep(2000);
        }

        public void BlockSettingsClose()
        {
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);
        }

        public void BlockSettingsSave()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveSettingsBtn\"]")).Click();
            Thread.Sleep(2000);
        }

        public void BlockSettingsCancel()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"CancelSettingsBtn\"]")).Click();
            Thread.Sleep(1000);
        }

        #endregion Settings

        #region SettingsFront

        // Светлая, Темная, Умеренная, Пользовательская
        public void SetColorScheme(string typeScheme)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"ColorScheme\"]")))
                .SelectByText(typeScheme);
            Thread.Sleep(500);
        }

        public void EditUserScheme()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"EditColorSchemeBtn\"]")).Click();
            Thread.Sleep(1000);
        }

        public void HiddenInMobile()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"mobile_hidden\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobile_hidden\"] span")).Click();
        }

        public void ShowInMobile()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"mobile_hidden\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobile_hidden\"] span")).Click();
        }

        public void HiddenInDesktop()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"desktop_hidden\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"desktop_hidden\"] span")).Click();
        }

        public void ShowInDesktop()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"desktop_hidden\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"desktop_hidden\"] span")).Click();
        }

        public void ShowTitle()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"show_title\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"show_title\"] span")).Click();
        }

        public void HiddenTitle()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"show_title\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"show_title\"] span")).Click();
        }

        public void ShowSubTitle()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"show_subtitle\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"show_subtitle\"] span")).Click();
        }

        public void HiddenSubTitle()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"show_subtitle\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"show_subtitle\"] span")).Click();
        }

        public void ShowOnAllPage()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"ShowOnAllPage\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowOnAllPage\"] span")).Click();
        }

        public void ShowOnThisPage()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"ShowOnAllPage\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowOnAllPage\"] span")).Click();
        }

        //Headers
        public void FixedOnScrollMobile()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"fixedOnScrollMobile\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"fixedOnScrollMobile\"] span")).Click();
        }

        public void NoFixedOnScrollMobile()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"fixedOnScrollMobile\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"fixedOnScrollMobile\"] span")).Click();
        }

        public void FixedOnScroll()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"fixedOnScroll\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"fixedOnScroll\"] span")).Click();
        }

        public void NoFixedOnScroll()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"fixedOnScroll\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"fixedOnScroll\"] span")).Click();
        }

        public void RunDown()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"runDown\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"runDown\"] span")).Click();
        }

        public void NoRunDown()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"runDown\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"runDown\"] span")).Click();
        }

        //Products
        public void ShowPrice()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"ShowPrice\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowPrice\"] span")).Click();
        }

        public void HiddenPrice()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"ShowPrice\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowPrice\"] span")).Click();
        }

        public void ShowBuyBtn()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"ShowBuyBtn\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowBuyBtn\"] span")).Click();
        }

        public void HiddenBuyBtn()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"ShowBuyBtn\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowBuyBtn\"] span")).Click();
        }

        public void HideShipping()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"HideShipping\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"HideShipping\"] span")).Click();
        }

        public void ShowShipping()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"HideShipping\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"HideShipping\"] span")).Click();
        }

        public void ShowDescription()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"ShowDescription\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowDescription\"] span")).Click();
        }

        public void HiddenDescription()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"ShowDescription\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowDescription\"] span")).Click();
        }

        public void HidePhoto()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"HidePhoto\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"HidePhoto\"] span")).Click();
        }

        public void ShowPhoto()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"HidePhoto\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"HidePhoto\"] span")).Click();
        }

        public void OnQuickView()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"OnQuickView\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"OnQuickView\"] span")).Click();
        }

        public void OffQuickView()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"OnQuickView\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"OnQuickView\"] span")).Click();
        }

        public void RoundPhotoOn()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"round_photo\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"round_photo\"] span")).Click();
        }

        public void RoundPhotoOff()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"round_photo\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"round_photo\"] span")).Click();
        }

        public void CountProductInRow(int countProd)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountProductInRow\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountProductInRow\"]")).SendKeys(countProd.ToString());
            Thread.Sleep(500);
        }

        //Covers
        public void FullHeightOn()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"FullHeight\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"FullHeight\"] span")).Click();
        }

        public void FullHeightOff()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"FullHeight\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"FullHeight\"] span")).Click();
        }

        public void ScrollToBlockOn()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"scrollToBlock\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"scrollToBlock\"] span")).Click();
        }

        public void ScrollToBlockOff()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"scrollToBlock\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"scrollToBlock\"] span")).Click();
        }

        //Columns
        public void AddNewColumns(string title, string text)
        {
            var count = Driver.FindElement(By.CssSelector("[data-e2e=\"gridColumn\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count;
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridColumn\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();

            Driver.GetGridCell(count, "title").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "title").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "title").FindElement(By.TagName("input")).SendKeys(title);

            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).SendKeys(text);
        }

        public void DelAllColumns()
        {
            Driver.FindElement(By.CssSelector("[[data-e2e=\"ItemDel\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            Thread.Sleep(500);
        }

        public void DelAllColumns(string gridName)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + gridName + "\"] [data-e2e=\"ItemDel\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".swal2-popup .swal2-confirm")).Click();
            Thread.Sleep(500);
        }

        //Team
        public void AddNewTeam(string title, string text)
        {
            var count = Driver.FindElement(By.CssSelector("[data-e2e=\"gridTeam\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count;
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridTeam\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();

            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).SendKeys(title);

            Driver.GetGridCell(count, "position").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "position").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "position").FindElement(By.TagName("input")).SendKeys(text);
        }

        //Schedule
        public void AddNewSchedule(string name, string position, string time, string text)
        {
            var count = Driver.FindElement(By.CssSelector("[data-e2e=\"ScheduleGrid\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count;
            Driver.FindElement(By.CssSelector("[data-e2e=\"ScheduleGrid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();

            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).SendKeys(name);

            Driver.GetGridCell(count, "position").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "position").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "position").FindElement(By.TagName("input")).SendKeys(position);

            Driver.GetGridCell(count, "time").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "time").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "time").FindElement(By.TagName("input")).SendKeys(time);

            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).SendKeys(text);
        }

        //Booking
        public void ShowServiceOn()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"show_ServiceLink\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"show_ServiceLink\"] span")).Click();
        }

        public void ShowServiceOff()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"show_ServiceLink\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"show_ServiceLink\"] span")).Click();
        }

        public void ChangeServiceLink(string text)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServiceLinkText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServiceLinkText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServiceLinkText\"]")).SendKeys(text);
            Thread.Sleep(500);
        }

        public void BookingBtnOn()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"EnabledBtn\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnabledBtn\"] span")).Click();
        }

        public void BookingBtnOff()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"EnabledBtn\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnabledBtn\"] span")).Click();
        }

        public void ChangeBookingBtnText(string text)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnTextBooking\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnTextBooking\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnTextBooking\"]")).SendKeys(text);
            Thread.Sleep(500);
        }

        public void BookingDaysOn()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"BookingByDays\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"BookingByDays\"] span")).Click();
        }

        public void BookingDaysOff()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"BookingByDays\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"BookingByDays\"] span")).Click();
        }

        public void ChangeReservation(int count, string name, string description)
        {
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).SendKeys(name);

            Driver.GetGridCell(count, "description").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(count, "description").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(count, "description").FindElement(By.TagName("textarea")).SendKeys(description);
        }

        public void ChangeReservation(int count, string name, string description, string TimeEnd, string TimeFrom)
        {
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("input")).SendKeys(name);

            Driver.GetGridCell(count, "description").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(count, "description").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(count, "description").FindElement(By.TagName("textarea")).SendKeys(description);

            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]")).Click();
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]")).Clear();
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]")).SendKeys(TimeEnd);

            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]")).Click();
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]")).Clear();
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]")).SendKeys(TimeFrom);
        }

        public void AddNewReservation(string resource)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();
            Thread.Sleep(500);
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BookingResource\"]")))
                .SelectByText(resource);
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingAdd\"]")).Click();
            Thread.Sleep(500);
        }

        public void AddNewReservation(string resource, string TimeEnd, string TimeFrom)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();
            Thread.Sleep(500);
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BookingResource\"]")))
                .SelectByText(resource);
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingAdd\"]")).Click();
            Thread.Sleep(500);

            var count = Driver.FindElement(By.CssSelector("[data-e2e=\"BookingGrid\"] .lp-table__body"))
                .FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count - 1;
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]")).Click();
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]")).Clear();
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeEnd\"]")).SendKeys(TimeEnd);

            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]")).Click();
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]")).Clear();
            Driver.GetGridCell(count, "time").FindElement(By.CssSelector("[data-e2e=\"BookingTimeFrom\"]")).SendKeys(TimeFrom);
        }

        //right general
        public void ConvertToHtmlSave()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"convertToHtml\"]")).Click();
            Thread.Sleep(500);
            Driver.SwalConfirm();
            Thread.Sleep(2000);
        }

        public void ConvertToHtmlCancel()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"convertToHtml\"]")).Click();
            Thread.Sleep(500);
            Driver.SwalCancel();
            Thread.Sleep(500);
        }

        public void CopyBlock()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"copyBlock\"]")).Click();
            Thread.Sleep(500);
        }

        public void UpdateBlockCancel()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"UpdateBlock\"]")).Click();
            Thread.Sleep(500);
            Driver.SwalCancel();
            Thread.Sleep(500);
        }

        public void UpdateBlockSave()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"UpdateBlock\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".swal2-confirm ")).Click();
            Thread.Sleep(500);
        }


        //1 колонка - 16 колонок
        public void WidthColumn(string Width)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"WidthColumn\"]"))).SelectByText(Width);
            Thread.Sleep(500);
        }

        #endregion SettingsFront

        #region Image

        public void OpenPopupUpdateImage()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"update_picture\"]")).Click();
            Thread.Sleep(500);
        }

        public void UpdateLoadImageDesktop(string picPath)
        {
            Thread.Sleep(500);
            AttachFile(By.CssSelector("input[data-e2e=\"LoadPicture\"]"), GetPicturePath(picPath));
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveCroperBtn\"]")).Click();
            Thread.Sleep(1000);
        }

        public void UpdateImageByUrl(string urlPath)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"uploadByUrl\"]")).Click();
            Thread.Sleep(500);

            Driver.FindElement(By.CssSelector("[data-e2e=\"UrlUpload\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"UrlUpload\"]")).SendKeys(urlPath);
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"uploadUrlBtn\"]")).Click();
            Thread.Sleep(1000);
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"SaveCroperBtn\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveCroperBtn\"]")).Click();
            Thread.Sleep(1000);
        }

        public void UpdateLoadImageGallery(int count = 5)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"GalleryCloud\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElements(By.CssSelector(".gallery-cloud__list-item"))[count].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveCroperBtn\"]")).Click();
            Thread.Sleep(1000);
        }

        public void UpdateLoadIconGallery(int count = 10)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"galleryIcons\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElements(By.CssSelector(".gallery-icons__list-item"))[count].Click();
            Thread.Sleep(1000);
        }

        public void OnLazyLoad()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
        }

        public void OffLazyLoad()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
        }

        public void DelImageSave()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"deletePicture\"]")).Click();
            Thread.Sleep(500);
        }

        public void BehaviorBackgroundImage(string behavior)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"Behavior_background\"]")))
                .SelectByText(behavior);
            Thread.Sleep(500);
        }

        public void ImagesAligment(string behavior)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"ImageAligment\"]")))
                .SelectByText(behavior);
            Thread.Sleep(500);
        }

        public void ImageBlockHeight(string count)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockHeight\"] input")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockHeight\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockHeight\"] input")).SendKeys(count);
        }

        public void ImageBlockCount(string count)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockCount\"] input")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockCount\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockCount\"] input")).SendKeys(count);
        }

        // shadow - int from 0 to 10
        public void moveSlider(int shadow)
        {
            var builder = new Actions(Driver);
            var slider = Driver.FindElement(By.CssSelector("[data-e2e=\"shading_background\"] .ngrs-handle-max"));
            /*  builder.ClickAndHold(slider).MoveByOffset(-(width / 2), 0).
                                 moveByOffset((int)((width / 100) * percent), 0).
                                 release().build();*/
            builder.ClickAndHold(slider).MoveByOffset(0, 0).MoveByOffset(50 * shadow, 0).Release().Build();
            Thread.Sleep(500);
            builder.Perform();
        }

        public void moveSliderPaddingTop(int pTop) //padding_top - int from (0px) to (375px)
        {
            var builder = new Actions(Driver);
            var slider = Driver.FindElement(By.CssSelector("[data-e2e=\"padding_top\"] .ngrs-handle-max"));
            var s = Driver.FindElement(By.CssSelector("[data-e2e=\"padding_top\"] .ngrs-value-max .ng-binding")).Text;
            var parts = s.Split('p');
            var equal = Convert.ToInt32(parts[0]);
            var step = Convert.ToInt32((pTop - equal) / 5);
            var stepvalue = 0;
            stepvalue = step > 0 ? 5 : -5;
            step = Math.Abs(step);
            //while(driver.FindElement(By.CssSelector("[data-e2e=\"padding_top\"] .ng-binding")).Text != "0 px")
            //{
            //    builder = new Actions(driver);
            //    slider = driver.FindElement(By.CssSelector("[data-e2e=\"padding_top\"] .ngrs-handle-max"));
            //    builder.ClickAndHold(slider).MoveByOffset(0, 0).MoveByOffset(-5, 0).Release().Build();
            //    Thread.Sleep(500);
            //    builder.Perform();
            //}
            //  builder.ClickAndHold(slider).MoveByOffset(0, 0).MoveByOffset(-1000, 0).Release().Build();
            // builder.Perform();
            for (var i = 0; i < step; i++)
            {
                builder = new Actions(Driver);
                slider = Driver.FindElement(By.CssSelector("[data-e2e=\"padding_top\"] .ngrs-handle-max"));
                builder.ClickAndHold(slider).MoveByOffset(stepvalue, 0).Release().Build();
                Thread.Sleep(500);
                builder.Perform();
            }
        }

        public void moveSliderPaddingBottom(int pBot) //padding_bottom - int from (0px) to (375px)
        {
            var builder = new Actions(Driver);
            var slider = Driver.FindElement(By.CssSelector("[data-e2e=\"padding_bottom\"] .ngrs-handle-max"));
            //builder.ClickAndHold(slider).MoveByOffset(0, 0).MoveByOffset(-1000, 0).Release().Build();
            //builder.Perform();
            //int step = Convert.ToInt32(pBot / 5);
            var s = Driver.FindElement(By.CssSelector("[data-e2e=\"padding_bottom\"] .ngrs-value-max .ng-binding"))
                .Text;
            var parts = s.Split('p');
            var equal = Convert.ToInt32(parts[0]);
            var step = Convert.ToInt32((pBot - equal) / 5);
            var stepvalue = 0;
            stepvalue = step > 0 ? 5 : -5;
            step = Math.Abs(step);
            for (var i = 0; i < step; i++)
            {
                builder = new Actions(Driver);
                slider = Driver.FindElement(By.CssSelector("[data-e2e=\"padding_bottom\"] .ngrs-handle-max"));
                builder.ClickAndHold(slider).MoveByOffset(stepvalue, 0).Release().Build();
                Thread.Sleep(500);
                builder.Perform();
            }
        }

        public void DragDrop(int startPosition, int endPosition, string gridName)
        {
            var dragElement = Driver.FindElement(By.CssSelector("[data-e2e=\"" + gridName +
                                                                "\"] [data-e2e-grid-drag=\"grid[" + startPosition +
                                                                "]\"]"));
            var dropElement = Driver.FindElement(By.CssSelector("[data-e2e=\"" + gridName +
                                                                "\"] [data-e2e-grid-drag=\"grid[" + endPosition +
                                                                "]\"]"));

            var builder = new Actions(Driver);
            builder.ClickAndHold(dragElement);
            Thread.Sleep(500);
            builder.MoveToElement(dropElement);
            Thread.Sleep(500);
            builder.Perform();
            builder.Release();
            Thread.Sleep(500);
            builder.Perform();
            Thread.Sleep(1000);
        }

        #endregion Image

        #region Button

        public void BtnEnabledButton(int count = 0)
        {
            if (!Driver.FindElements(By.CssSelector("[data-e2e=\"EnabledBtn\"] input"))[count].Selected)
                Driver.FindElements(By.CssSelector("[data-e2e=\"EnabledBtn\"] .blocks-constructor-checkbox"))[count]
                    .Click();
        }

        public void BtnDisableButton(int count = 0)
        {
            if (Driver.FindElements(By.CssSelector("[data-e2e=\"EnabledBtn\"] input"))[count].Selected)
                Driver.FindElements(By.CssSelector("[data-e2e=\"EnabledBtn\"] .blocks-constructor-checkbox"))[count]
                    .Click();
        }

        public void BtnSetTextButton(string TextBtn, int count = 0)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"TextBtn\"]"))[count].Clear();
            Driver.FindElements(By.CssSelector("[data-e2e=\"TextBtn\"]"))[count].SendKeys(TextBtn);
            Thread.Sleep(500);
        }

        public void BtnActionButtonSelect(string action, int count = 0)
        {
            new SelectElement(Driver.FindElements(By.CssSelector("[data-e2e=\"buttonActions\"]"))[count])
                .SelectByText(action);
            Thread.Sleep(500);
        }

        public void BtnAnaliticsEnabled(int count = 0)
        {
            if (!Driver.FindElements(By.CssSelector("[data-e2e=\"AnaliticsBtn\"] input"))[count].Selected)
                Driver.FindElements(By.CssSelector("[data-e2e=\"AnaliticsBtn\"] .blocks-constructor-checkbox"))[count]
                    .Click();
        }

        public void BtnAnaliticsDisabled(int count = 0)
        {
            if (Driver.FindElements(By.CssSelector("[data-e2e=\"AnaliticsBtn\"] input"))[count].Selected)
                Driver.FindElements(By.CssSelector("[data-e2e=\"AnaliticsBtn\"] .blocks-constructor-checkbox"))[count]
                    .Click();
        }

        #endregion Button

        #region Button Action

        #region Action url

        public void BtnActionUrlSelectUrl(string action, int count = 0)
        {
            new SelectElement(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlSelect\"]"))[count])
                .SelectByText(action);
            Thread.Sleep(500);
        }

        public void BtnActionUrlSetUrl(string Texturl, int count = 0)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"UrlInput\"]"))[count].Clear();
            Driver.FindElements(By.CssSelector("[data-e2e=\"UrlInput\"]"))[count].SendKeys(Texturl);
            Thread.Sleep(500);
        }

        public void BtnActionUrlTargetBlank(int count = 0)
        {
            if (!Driver.FindElements(By.CssSelector("[data-e2e=\"target_blank\"] input"))[count].Selected)
                Driver.FindElements(By.CssSelector("[data-e2e=\"target_blank\"] .blocks-constructor-checkbox"))[count]
                    .Click();
        }

        public void BtnActionUrlNoTargetBlank(int count = 0)
        {
            if (Driver.FindElements(By.CssSelector("[data-e2e=\"target_blank\"] input"))[count].Selected)
                Driver.FindElements(By.CssSelector("[data-e2e=\"target_blank\"] .blocks-constructor-checkbox"))[count]
                    .Click();
        }

        #endregion Action url

        #region Go to block

        public void BtnActionBlockSetBlock(string blocksId, int count = 0)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"BlockIdInput\"]"))[count].Clear();
            Driver.FindElements(By.CssSelector("[data-e2e=\"BlockIdInput\"]"))[count].SendKeys(blocksId);
            Thread.Sleep(500);
        }

        public void BtnActionBlockSelectBlock(int blockId, int count = 0)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"BlockSelect\"]"))[count].Click();
            Thread.Sleep(500);
            Driver.ScrollTo(By.Id("block_" + blockId));
            Driver.MouseFocus(By.CssSelector("#block_" + blockId));
            Driver.FindElements(By.CssSelector("[data-block-id=\"" + blockId + "\"]"))[1].Click();
            Thread.Sleep(500);
        }

        #endregion Go to block

        #region Form

        #region Form fields

        public void FormSetTitle(string titleTxt)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsFormTitle\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsFormTitle\"]")).SendKeys(titleTxt);
            Thread.Sleep(500);
        }

        public void FormSetSubTitle(string subTitleTxt)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsFormSubTitle\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsFormSubTitle\"]")).SendKeys(subTitleTxt);
            Thread.Sleep(500);
        }

        public void FormSetButtonText(string BtnTxt)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButtonText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButtonText\"]")).SendKeys(BtnTxt);
            Thread.Sleep(500);
        }

        public void FormEditFormName(int row, string fieldName)
        {
            Driver.GetGridCell(row, "Title").FindElement(By.CssSelector("input")).Clear();
            Driver.GetGridCell(row, "Title").FindElement(By.CssSelector("input")).SendKeys(fieldName);
            Thread.Sleep(500);
        }

        public void FormEditFormFieldCrm(int row, string fieldNameCrm)
        {
            new SelectElement(Driver.GetGridCell(row, "TitleCrm").FindElement(By.TagName("select")))
                .SelectByText(fieldNameCrm);
        }

        public void FormEditFormRequired(int row)
        {
            if (!Driver.GetGridCell(row, "Required").FindElement(By.TagName("input")).Selected)
                Driver.GetGridCell(row, "Required").FindElement(By.TagName("span")).Click();
        }

        public void FormEditFormNoRequired(int row)
        {
            if (Driver.GetGridCell(row, "Required").FindElement(By.TagName("input")).Selected)
                Driver.GetGridCell(row, "Required").FindElement(By.TagName("span")).Click();
        }

        public void FormDelFieldSave(int row)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"FormGrid\"] [data-e2e=\"ItemDel\"]"))[row].Click();
            Thread.Sleep(500);
            Driver.SwalConfirm();
            Thread.Sleep(500);
        }

        public void FormDelFieldCancel(int row)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"FormGrid\"] [data-e2e=\"ItemDel\"]"))[row].Click();
            Thread.Sleep(500);
            Driver.SwalCancel();
            Thread.Sleep(500);
        }

        public void FormAddField(string fieldName, string fieldNameCrm, bool reqired)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormGrid\"] [data-e2e=\"AddNewElem\"]")).Click();
            Thread.Sleep(500);
            var rowNew = Driver.FindElements(By.CssSelector("[data-e2e=\"FormGrid\"] [data-e2e=\"ItemDel\"]")).Count -
                         2;
            Driver.GetGridCell(rowNew, "Title").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(rowNew, "Title").FindElement(By.TagName("input")).SendKeys(fieldName);
            new SelectElement(Driver.GetGridCell(rowNew, "TitleCrm").FindElement(By.TagName("select"))).SelectByText(
                fieldNameCrm);
            if (reqired) Driver.GetGridCell(rowNew, "Required").FindElement(By.TagName("span")).Click();

            Thread.Sleep(500);
        }

        public void FormTitleLocationSelect(string location)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"TextPosition\"]"))).SelectByText(location);
            Thread.Sleep(500);
        }

        public void FormShowAgreement(string agreementTxt)
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"ShowAgreement\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowAgreement\"] span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AgreementText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AgreementText\"]")).SendKeys(agreementTxt);
        }

        public void FormHiddenAgreement()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"ShowAgreement\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowAgreement\"] span")).Click();
        }

        public void checkField(int row, string textField)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[row].Click();
            Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[row].Clear();
            Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[row]
                .SendKeys(textField);
            Thread.Sleep(1000);
        }

        #endregion Form fields

        #region ActionForm

        public void FormActionAfterSendSelect(string action)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PosFormAction\"]"))).SelectByText(action);
            Thread.Sleep(500);
        }

        public void FormActionAfterSendText(string text)
        {
            Driver.SetCkText(text, "editor1");
        }

        public void FormActionAfterSendSalesFunnelSelect(string action)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectSalesFunnels\"]")))
                .SelectByText(action);
            Thread.Sleep(500);
        }

        public void FormActionPostMessageRedirectSelect(string redirect)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")))
                .SelectByText(redirect);
            Thread.Sleep(500);
        }

        public void FormActionUrlRedirect(string UrlRedirect)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).SendKeys(UrlRedirect);
            Thread.Sleep(1000);
        }

        public void FormActionDontSendLead()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"DontSendLead\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"DontSendLead\"] span")).Click();
        }

        public void FormActionSendLead()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"DontSendLead\"] input")).Selected)
                Driver.FindElement(By.CssSelector("[data-e2e=\"DontSendLead\"] span")).Click();
        }

        public void FormActionEmailSubject(string EmailSubject)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"EmailSubject\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"EmailSubject\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"EmailSubject\"]")).SendKeys(EmailSubject);
            Thread.Sleep(1000);
        }

        #endregion ActionForm

        #region Goals

        public void FormGoalYaMetrika(string Text)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"YaMetrikaEvent\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"YaMetrikaEvent\"]")).SendKeys(Text);
            Thread.Sleep(500);
        }

        public void FormGoalGaCategory(string Text)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gaEventCategory\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gaEventCategory\"]")).SendKeys(Text);
            Thread.Sleep(500);
        }

        public void FormGoalGaAction(string Text)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gaEventAction\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gaEventAction\"]")).SendKeys(Text);
            Thread.Sleep(500);
        }

        #endregion Goals

        #region Product

        public void FormProductSelect(string testname)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProductBtn\"]")).Click();
            Thread.Sleep(500);
            Driver.GetGridFilter().SendKeys(testname);
            Thread.Sleep(500);
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr").Click();
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(500);
        }

        public void DelAllProduct()
        {
            while (Driver.FindElements(By.CssSelector("[data-e2e=\"DelProduct\"]")).Count > 0)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DelProduct\"]")).Click();
                Thread.Sleep(500);
            }
        }

        public void FormProdSelectUpsell(string upsellLp)
        {
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectUsell\"]"))).SelectByText(upsellLp);
            Thread.Sleep(500);
        }

        public void FormProdOfferPrice(string OfferPrice, int row = 0)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"NewPriceProduct\"]"))[row].Clear();
            Driver.FindElements(By.CssSelector("[data-e2e=\"NewPriceProduct\"]"))[row].SendKeys(OfferPrice);
            Thread.Sleep(500);
        }

        public void FormProdCount(string Count, int row = 0)
        {
            Driver.FindElements(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input"))[row].Clear();
            Driver.FindElements(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input"))[row].SendKeys(Count);
            Thread.Sleep(500);
        }

        public void FormProdShippingPrice(string ShippingPrice)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingPrice\"]")).SendKeys(ShippingPrice);
            Thread.Sleep(500);
        }

        /// <summary>
        ///     change price on product page admin
        /// </summary>
        /// <param name="id"></param>
        /// id product for change
        /// <param name="price"></param>
        /// new price product
        /// <param name="rowprice"></param>
        /// row price for modification
        public void ChangePriceAdmin(int id, string price, int rowprice = 0)
        {
            GoToAdmin("product/edit/" + id);
            Driver.GetGridCell(rowprice, "BasePrice", "Offers").FindElement(By.Name("inputForm")).Click();
            Driver.GetGridCell(rowprice, "BasePrice", "Offers").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(rowprice, "BasePrice", "Offers").FindElement(By.TagName("input")).SendKeys(price);
            Driver.FindElement(By.CssSelector("#price h2")).Click();
        }

        #endregion Product

        #endregion Form

        #endregion Button Action
    }
}