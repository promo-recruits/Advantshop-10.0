using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Core.Services.TemplatesDocx.Templates;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GenerateTemplatesDocx
    {
        private readonly GenerateTemplatesDocxModel _model;
        public List<string> Errors { get; set; }

        public GenerateTemplatesDocx(GenerateTemplatesDocxModel model)
        {
            _model = model;
            Errors = new List<string>();
        }

        public object Execute()
        {
            if (_model.TemplatesDocx == null || _model.TemplatesDocx.Count <= 0)
            {
                Errors.Add("Укажите шаблоны");
                return null;
            }

            var order = OrderService.GetOrder(_model.OrderId);
            if (order == null)
            {
                Errors.Add("Заказ не найден");
                return null;
            }
            if (!OrderService.CheckAccess(order))
            {
                Errors.Add("Нет доступа");
                return null;
            }
            if (order.OrderCurrency.EnablePriceRounding)
            {
                // валидация из RecalculateOrderItemsToSum
                if (order.OrderItems.Any(x => !_ValidatePrice(x.Price, order.OrderCurrency.RoundNumbers)))
                {
                    Errors.Add("Цены некоторых позиций заказа указаны без применяемого валютой у данного заказа округления. Невозможно сформировать корректный документ.");
                    return null;
                }
            }

            OrderTemplate orderTemplate = (OrderTemplate)order;

            var directoryPath = string.Format("{0}{1}", FoldersHelper.GetPathAbsolut(FolderType.PriceTemp), Guid.NewGuid());
            FileHelpers.CreateDirectory(directoryPath);

            foreach (var id in _model.TemplatesDocx)
            {
                var template = TemplatesDocxServices.Get<OrderTemplateDocx>(id);
                var templateFile = TemplatesDocxServices.GetPathAbsolut(template);
                var generateFile = string.Format("{0}/{1}{2}", directoryPath, template.Name, Path.GetExtension(templateFile));
                File.Copy(templateFile, generateFile);

                TemplatesDocxServices.TemplateFillContent(generateFile, orderTemplate, isNeedToNoticeAboutErrors: template.DebugMode);
            }

            if (_model.TemplatesDocx.Count == 1)
            {
                return new Tuple<string, string>(Directory.GetFiles(directoryPath)[0], directoryPath);
            }
            else
            {
                var zipFilePath = string.Format("{0}/files.zip", directoryPath);
                if (!FileHelpers.ZipFiles(directoryPath, zipFilePath))
                {
                    Errors.Add("Не удалось заархивировать файлы");

                    FileHelpers.DeleteDirectory(directoryPath);
                    return null;
                }

                return new Tuple<string, string>(zipFilePath, directoryPath);
            }
        }
        
        // из RecalculateOrderItemsToSum
        private bool _ValidatePrice(float value, float? roundNumbers)
        {
            if (!roundNumbers.HasValue)
                return true;

            var invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
            var valueStr = value.ToString(invariantCulture);
            var roundNumbersStr = roundNumbers.Value.ToString(invariantCulture);
            var decimalSeparator = invariantCulture.NumberFormat.NumberDecimalSeparator;

		
            var indexValueStr = valueStr.IndexOf(decimalSeparator);
            var indexRoundNumbersStr = roundNumbersStr.IndexOf(decimalSeparator);

            return indexRoundNumbersStr < 0 
                ? value % roundNumbers.Value == 0
                : indexValueStr < 0 || valueStr.Substring(indexValueStr).Length <= roundNumbersStr.Substring(indexRoundNumbersStr).Length;
        }
    }
}
