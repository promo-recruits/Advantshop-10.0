using System.IO;
using System.Text;
using System.Web;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Customers.Subscription
{
    public class ImportSubscriptionHandlers
    {
        private readonly HttpPostedFileBase _file;
        private readonly string _outputFilePath;

        public class Results
        {
            public bool Result { get; set; }
            public string Error { get; set; }
        } 

        public ImportSubscriptionHandlers(HttpPostedFileBase file, string outputFilePath)
        {
            _file = file;
            _outputFilePath = outputFilePath;

            FileHelpers.DeleteFile(outputFilePath);
        }

        public Results Execute()
        {
            if (_file == null || string.IsNullOrEmpty(_file.FileName) 
                              || !FileHelpers.CheckFileExtension(_file.FileName, EAdvantShopFileTypes.Catalog))
                return new Results { Result = false, Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
            
            _file.SaveAs(_outputFilePath);
            if (!File.Exists(_outputFilePath))
                return new Results { Result = false, Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var result = Import(_outputFilePath);

            return result; // new Results { Result = true, Status = "Импорт выполнен успешно" };
        }

        public Results Import(string filePath)
        {
            try
            {
                using (var csvReader = new CsvHelper.CsvReader(new StreamReader(filePath, Encoding.UTF8), new CsvConfiguration() { Delimiter = ";" }))
                {
                    while (csvReader.Read())
                    {
                        if (csvReader.CurrentRecord.Length != 5)
                            return new Results { Result = false, Error = LocalizationService.GetResource("Admin.Subscribe.Import.WrongFile") };

                        var email = csvReader.CurrentRecord[0];
                        if (!ValidationHelper.IsValidEmail(email))
                            continue;

                        var subscription = SubscriptionService.GetSubscription(email);
                        if (subscription != null)
                        {
                            subscription.Email = email;
                            subscription.Subscribe = csvReader.CurrentRecord[1] == "1";
                            subscription.UnsubscribeReason = csvReader.CurrentRecord[4];
                            SubscriptionService.UpdateSubscription(subscription);
                        }
                        else
                        {
                            SubscriptionService.AddSubscription(new AdvantShop.Customers.Subscription
                            {
                                Email = email,
                                Subscribe = csvReader.CurrentRecord[1] == "1",
                                UnsubscribeReason = csvReader.CurrentRecord[4]
                            });
                        }
                    }
                }
                return new Results { Result = true };
            }
            catch
            {
                return new Results { Result = false, Error = LocalizationService.GetResource("Admin.Subscribe.Import.ImportError") };
            }
        }
    }
}
