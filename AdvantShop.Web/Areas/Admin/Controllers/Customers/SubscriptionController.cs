using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Customers.Subscription;
using AdvantShop.Web.Admin.Models.Customers.Subscription;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Controllers.Customers
{
    public partial class SubscriptionController : BaseAdminController
    {
        [Auth(RoleAction.Customers)]
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Subscription.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.SubscriptionCtrl);

            return View();
        }

        #region Get/Delete
        
        public JsonResult GetSubscribes(SubscriptionFilterModel model)
        {
            var handler = new GetSubscription(model);
            var result = handler.Execute();
            
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSubscription(SubscriptionFilterModel model)
        {
            Command(model, (id, c) =>
            {
                SubscriptionService.DeleteSubscription(id);
                return true;
            });

            return Json(true);
        }

        public JsonResult GetSubscriptionIds(SubscriptionFilterModel model)
        {
            var ids = new List<int>();
            Command(model, (id, c) =>
            {
                ids.Add(id);
                return true;
            });
            return Json(new { ids });
        }

        #endregion

        #region Inplace

        public JsonResult InplaceSubscription(SubscriptionFilterModel model)
        {
            int id = 0;
            Int32.TryParse(model.Id.ToString(), out id);
            if (id == 0)
            {
                return Json(new { result = false });
            }
            var subscribe = SubscriptionService.GetSubscription(id);

            subscribe.Subscribe = model.Enabled != null ? (bool)model.Enabled : false;

            SubscriptionService.UpdateSubscription(subscribe);

            return Json(new { result = true });
        }

        #endregion

        #region Export/Import

        public ActionResult Export(bool openInBrowser = false)
        {
            var filename = "subscribers" + DateTime.Now.ToShortDateString() + ".csv";
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp, filename);
            FileHelpers.DeleteFile(filePath);
            
            try
            {
                using (var csvWriter = new CsvHelper.CsvWriter(new StreamWriter(filePath, false, Encoding.UTF8), new CsvConfiguration() { Delimiter = ";", SkipEmptyRecords = false }))
                {
                    csvWriter.WriteField(LocalizationService.GetResource("Admin.Subscribe.Export.Email"));
                    csvWriter.WriteField(LocalizationService.GetResource("Admin.Subscribe.Export.Status"));
                    csvWriter.WriteField(LocalizationService.GetResource("Admin.Subscribe.Export.Date"));
                    csvWriter.WriteField(LocalizationService.GetResource("Admin.Subscribe.Export.UnsubscribeDate"));
                    csvWriter.WriteField(LocalizationService.GetResource("Admin.Subscribe.Export.UnsubscribeReason"));
                    csvWriter.NextRecord();

                    foreach (var subscriber in SubscriptionService.GetSubscriptions())
                    {
                        csvWriter.WriteField(subscriber.Email);
                        csvWriter.WriteField(subscriber.Subscribe ? "1" : "0");
                        csvWriter.WriteField(subscriber.SubscribeDate != DateTime.MinValue ? subscriber.SubscribeDate.ToString() : string.Empty);
                        csvWriter.WriteField(subscriber.UnsubscribeDate != DateTime.MinValue ? subscriber.UnsubscribeDate.ToString() : string.Empty);
                        csvWriter.WriteField(subscriber.UnsubscribeReason);

                        csvWriter.NextRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Json(false);
            }

            return openInBrowser ? File(filePath, "text/plain") : File(filePath, "application/octet-stream", filename);
        }

        public JsonResult Import(HttpPostedFileBase file)
        {
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(filePath);
            var outputFilePath = filePath + "importSubscription" + DateTime.Now.ToShortDateString() + ".csv";

            var result = new ImportSubscriptionHandlers(file, outputFilePath).Execute();
            return Json(result);
        }

        #endregion

        #region Commands

        private void Command(SubscriptionFilterModel command, Func<int, SubscriptionFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetSubscription(command);
                var ids = handler.GetItemsIds();

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }
        #endregion
    }
}
