using System.Globalization;
using System.Text;
using System.Xml;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Core.Services.Helpers
{
    #region CSV

    public class CsvComparer
    {
        private readonly string _firstFileFullPath;
        private readonly string _secondFileFullPath;

        private readonly string _separators;
        private readonly string _columnSeparator;
        private readonly string _propertySeparator;
        private readonly string _encoding;
        private readonly bool _hasHeadrs;

        private CsvComparer(string firstFileFullPath, string secondFileFullPath, string separators,
            string columnSeparator, string propertySeparator, EncodingsEnum encoding, bool hasHeadrs)
        {
            _firstFileFullPath = firstFileFullPath;
            _secondFileFullPath = secondFileFullPath;
            _separators = separators;
            _columnSeparator = columnSeparator;
            _propertySeparator = propertySeparator;
            _encoding = encoding.StrName();
            _hasHeadrs = hasHeadrs;
        }

        public static CsvComparer Factory(string firstFileFullPath, string secondFileFullPath, string separators,
            string columnSeparator, string propertySeparator, EncodingsEnum encoding, bool hasHeadrs = false)
        {
            return new CsvComparer(firstFileFullPath, secondFileFullPath, separators, columnSeparator,
                propertySeparator, encoding, hasHeadrs);
        }

        //public FileComparerResult Process()
        //{
        //    var result = new FileComparerResult();
        //    if (!File.Exists(_firstFileFullPath) || !File.Exists(_secondFileFullPath))
        //    {
        //        result.Errors.Add("Files does not exists");
        //        return result;
        //    }

        //    using (var firstFile = InitReader(_firstFileFullPath))
        //    using (var secondFile = InitReader(_secondFileFullPath))
        //    {
        //        #region Checking field headers
        //        if (_hasHeadrs)
        //        {
        //            firstFile.ReadHeader();
        //            secondFile.ReadHeader();
        //            if (firstFile.FieldHeaders.Length != secondFile.FieldHeaders.Length)
        //            {
        //                result.Errors.Add(string.Format("First file field headers length: {0}, Second file field headers length: {1}",
        //                    firstFile.FieldHeaders.Length, secondFile.FieldHeaders.Length));
        //                return result;
        //            }
        //            else
        //            {
        //                for (int i = 0; i < firstFile.FieldHeaders.Length - 1; i++)
        //                {
        //                    if (firstFile.FieldHeaders[i] != secondFile.FieldHeaders[i])
        //                    {
        //                        result.Errors.Add(string.Format("Different headers! Header number {0}. First file field header: {1}, second file field header: {2}",
        //                            i + 1, firstFile.FieldHeaders[i], secondFile.FieldHeaders[i]));
        //                    }
        //                }
        //                if (result.Errors.Count > 0)
        //                {
        //                    return result;
        //                }
        //            }
        //        }
        //        #endregion
        //        var lineNumber = 0;
        //        while (firstFile.Read())
        //        {
        //            if (!secondFile.Read())
        //            {
        //                result.Errors.Add("Differing number of lines - second file has less.");
        //                break;
        //            }
        //            lineNumber++;
        //            var firstFileLine = firstFile.CurrentRecord;
        //            var secondFileLine = secondFile.CurrentRecord;
        //            if (firstFileLine.Length != secondFileLine.Length)
        //            {
        //                result.Errors.Add(string.Format("Line {0} differs. First file length: {1}, Second file length: {2}", lineNumber, firstFileLine.Length, secondFileLine.Length));
        //            }
        //            else
        //            {
        //                for (int i = 0; i < firstFileLine.Length - 1; i++)
        //                {
        //                    if (firstFileLine[i] != secondFileLine[i])
        //                    {
        //                        if (_hasHeadrs)
        //                            result.Errors.Add(string.Format("Line {0} differs. Column {1}. Column header {2}. First file: {3}, Second file: {4}", lineNumber, i, firstFile.FieldHeaders[i], firstFileLine[i], secondFileLine[i]));
        //                        else
        //                            result.Errors.Add(string.Format("Line {0} differs. Column {1}. First file: {2}, Second file: {3}", lineNumber, i, firstFileLine[i], secondFileLine[i]));
        //                    }
        //                }
        //            }
        //        }
        //        if (secondFile.Read())
        //        {
        //            result.Errors.Add("Differing number of lines - first file has less.");
        //            result.FilesIdentical = false;
        //        }
        //    }
        //    if (result.Errors.Count == 0)
        //        result.FilesIdentical = true;
        //    return result;
        //}

        private CsvReader InitReader(string filePath)
        {
            var reader = new CsvReader(
                new StreamReader(filePath, Encoding.GetEncoding(_encoding ?? EncodingsEnum.Utf8.StrName())),
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = _separators ?? SeparatorsEnum.SemicolonSeparated.StrName(),
                    HasHeaderRecord = _hasHeadrs
                });
            return reader;
        }
    }

    public class FileComparerResult
    {
        public bool FilesIdentical { get; set; }

        public List<string> Errors { get; set; }

        public FileComparerResult()
        {
            this.FilesIdentical = false;
            this.Errors = new List<string>();
        }
    }

    #endregion


    #region XML

    public static class XmlFeedReader
    {
        public static string YamarketFeedToString(string filePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                ValidationType = ValidationType.Schema
            };

            using (var lStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var xr = XmlReader.Create(lStream, settings))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(xr);
                return xml.InnerXml;
            }
        }

        public static string YamarketStrToXml(string initString)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(string.Format("<root>{0}</root>", initString.Replace("\"\"", "\"")));
            return xml.InnerXml;
        }

        public static YamarketFeedObject ReadYamarketFeed(string filePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore
            };

            var feedObj = new YamarketFeedObject();

            using (var lStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var xr = XmlReader.Create(lStream, settings))
            {
                while (xr.Read())
                {
                    if (xr.IsStartElement())
                    {
                        switch (xr.Name)
                        {
                            case "name":
                                if (xr.Read())
                                {
                                    feedObj.Name = xr.Value;
                                }

                                break;
                            case "company":
                                if (xr.Read())
                                {
                                    feedObj.Company = xr.Value;
                                }

                                break;
                            case "url":
                                if (xr.Read())
                                {
                                    feedObj.Url = xr.Value;
                                }

                                break;
                            case "currencies":
                                using (var subreader = xr.ReadSubtree())
                                {
                                    while (subreader.Read())
                                    {
                                        if (subreader.Name == "currency")
                                        {
                                            var obj = new YamarketFeedCurrency();
                                            string attribute = subreader["id"];
                                            if (attribute != null)
                                            {
                                                obj.Id = attribute;
                                            }

                                            attribute = subreader["rate"];
                                            if (attribute != null)
                                            {
                                                obj.Rate = attribute.TryParseFloat();
                                            }

                                            feedObj.Currencies.Add(obj);
                                        }
                                    }
                                }

                                break;
                            case "categories":
                                using (var subreader = xr.ReadSubtree())
                                {
                                    while (subreader.Read())
                                    {
                                        if (subreader.Name == "category")
                                        {
                                            var obj = new YamarketFeedCategory();
                                            string attribute = subreader["id"];
                                            if (attribute != null)
                                            {
                                                obj.Id = attribute.TryParseInt();
                                                attribute = subreader["parentId"];
                                                if (attribute != null)
                                                {
                                                    obj.ParentId = attribute.TryParseInt();
                                                }

                                                if (subreader.Read())
                                                {
                                                    obj.Name = subreader.Value;
                                                }

                                                feedObj.Categories.Add(obj);
                                            }
                                        }
                                    }
                                }

                                break;
                            case "delivery-options":
                                using (var subreader = xr.ReadSubtree())
                                {
                                    while (subreader.Read())
                                    {
                                        if (subreader.Name == "option")
                                        {
                                            var obj = new YamarketFeedDeliveryOtion();
                                            string attribute = subreader["cost"];
                                            if (attribute != null)
                                            {
                                                obj.Cost = attribute.TryParseInt();
                                            }

                                            attribute = subreader["days"];
                                            if (attribute != null)
                                            {
                                                obj.Days = attribute.TryParseInt();
                                            }

                                            attribute = subreader["order-before"];
                                            if (attribute != null)
                                            {
                                                obj.OrderBefore = attribute;
                                            }

                                            feedObj.DeliveryOptions.Add(obj);
                                        }
                                    }
                                }

                                break;
                            case "offers":
                                using (var subreader = xr.ReadSubtree())
                                {
                                    while (subreader.Read())
                                    {
                                        if (subreader.Name == "offer")
                                        {
                                            var obj = new YamarketFeedOffer();
                                            string attribute = subreader["id"];
                                            if (attribute != null)
                                            {
                                                obj.Id = attribute.TryParseInt();
                                            }

                                            attribute = subreader["available"];
                                            if (attribute != null)
                                            {
                                                obj.Available = attribute.TryParseBool();
                                            }

                                            using (var elemreader = subreader.ReadSubtree())
                                            {
                                                while (elemreader.Read())
                                                {
                                                    switch (elemreader.Name)
                                                    {
                                                        case "url":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Url = elemreader.Value;
                                                            }

                                                            break;
                                                        case "price":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Price = elemreader.Value.TryParseFloat();
                                                            }

                                                            break;
                                                        case "purchase_price":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.PurchasePrice =
                                                                        elemreader.Value.TryParseFloat();
                                                            }

                                                            break;
                                                        case "currencyId":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.CurrencyId = elemreader.Value;
                                                            }

                                                            break;
                                                        case "categoryId":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.CategoryId = elemreader.Value.TryParseInt();
                                                            }

                                                            break;
                                                        case "picture":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Picture = elemreader.Value;
                                                            }

                                                            break;
                                                        case "store":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Store = elemreader.Value.TryParseBool();
                                                            }

                                                            break;
                                                        case "pickup":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Pickup = elemreader.Value.TryParseBool();
                                                            }

                                                            break;
                                                        case "delivery":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Delivery = elemreader.Value.TryParseBool();
                                                            }

                                                            break;
                                                        case "name":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Name = elemreader.Value;
                                                            }

                                                            break;
                                                        case "description":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Description = elemreader.Value;
                                                            }

                                                            break;
                                                        case "sales_notes":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.SalesNotes = elemreader.Value;
                                                            }

                                                            break;
                                                        case "delivery-options":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    using (var optionsreader = elemreader.ReadSubtree())
                                                                    {
                                                                        while (optionsreader.Read())
                                                                        {
                                                                            if (optionsreader.Name == "option")
                                                                            {
                                                                                var option =
                                                                                    new YamarketFeedDeliveryOtion();
                                                                                string optAttribute =
                                                                                    optionsreader["cost"];
                                                                                if (optAttribute != null)
                                                                                {
                                                                                    option.Cost =
                                                                                        optAttribute.TryParseInt();
                                                                                }

                                                                                optAttribute = optionsreader["days"];
                                                                                if (optAttribute != null)
                                                                                {
                                                                                    option.Days =
                                                                                        optAttribute.TryParseInt();
                                                                                }

                                                                                optAttribute =
                                                                                    optionsreader["order-before"];
                                                                                if (optAttribute != null)
                                                                                {
                                                                                    option.OrderBefore = optAttribute;
                                                                                }

                                                                                obj.DeliveryOptions.Add(option);
                                                                            }
                                                                        }
                                                                    }
                                                            }

                                                            break;
                                                        case "weight":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Weight = elemreader.Value.TryParseFloat();
                                                            }

                                                            break;
                                                        case "param":
                                                            if (elemreader.Read())
                                                            {
                                                                var param = new YamarketFeedOfferPrameter();
                                                                string prmAttribute = elemreader["name"];
                                                                if (prmAttribute != null)
                                                                {
                                                                    param.Name = prmAttribute;
                                                                }

                                                                prmAttribute = elemreader["unit"];
                                                                if (prmAttribute != null)
                                                                {
                                                                    param.Unit = prmAttribute;
                                                                }

                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                {
                                                                    param.Value = elemreader.Value;
                                                                    obj.Parameters.Add(param);
                                                                }
                                                            }

                                                            break;
                                                    }
                                                }
                                            }

                                            feedObj.Offers.Add(obj);
                                        }
                                    }
                                }

                                break;
                            case "gifts":
                                using (var subreader = xr.ReadSubtree())
                                {
                                    while (subreader.Read())
                                    {
                                        if (subreader.Name == "gift")
                                        {
                                            var obj = new YamarketFeedGift();
                                            string attribute = subreader["id"];
                                            if (attribute != null)
                                            {
                                                obj.Id = attribute.TryParseInt();
                                            }

                                            using (var elemreader = subreader.ReadSubtree())
                                            {
                                                while (elemreader.Read())
                                                {
                                                    switch (elemreader.Name)
                                                    {
                                                        case "name":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Name = elemreader.Value;
                                                            }

                                                            break;
                                                        case "picture":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Picture = elemreader.Value;
                                                            }

                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                break;
                            case "promos":
                                using (var subreader = xr.ReadSubtree())
                                {
                                    while (subreader.Read())
                                    {
                                        if (subreader.Name == "promo")
                                        {
                                            var obj = new YamarketFeedPromo();
                                            string attribute = subreader["id"];
                                            if (attribute != null)
                                            {
                                                obj.Id = attribute;
                                            }

                                            attribute = subreader["type"];
                                            if (attribute != null)
                                            {
                                                obj.Type = attribute;
                                            }

                                            using (var elemreader = subreader.ReadSubtree())
                                            {
                                                while (elemreader.Read())
                                                {
                                                    switch (elemreader.Name)
                                                    {
                                                        case "start-date":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.StartDate = elemreader.Value.TryParseDateTime();
                                                            }

                                                            break;
                                                        case "end-date":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.EndDate = elemreader.Value.TryParseDateTime();
                                                            }

                                                            break;
                                                        case "description":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Description = elemreader.Value;
                                                            }

                                                            break;
                                                        case "url":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Url = elemreader.Value;
                                                            }

                                                            break;
                                                        case "promo-code":
                                                            if (elemreader.Read())
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.PromoCode = elemreader.Value;
                                                            }

                                                            break;
                                                        case "discount":
                                                            if (elemreader.Read())
                                                            {
                                                                string discAttribute = elemreader["unit"];
                                                                if (discAttribute != null)
                                                                {
                                                                    obj.DiscountType = discAttribute;
                                                                }

                                                                if (!string.IsNullOrWhiteSpace(elemreader.Value))
                                                                    obj.Discount = elemreader.Value;
                                                            }

                                                            break;
                                                        case "purchase":
                                                            var purchase = new YamarketFeedPromoPurchase();
                                                            using (var purchaseReader = elemreader.ReadSubtree())
                                                            {
                                                                while (purchaseReader.Read())
                                                                {
                                                                    if (purchaseReader.Name == "required-quantity")
                                                                    {
                                                                        if (purchaseReader.Read())
                                                                        {
                                                                            if (!string.IsNullOrWhiteSpace(
                                                                                purchaseReader.Value))
                                                                                purchase.RequeredQuantity =
                                                                                    purchaseReader.Value.TryParseInt();
                                                                        }
                                                                    }

                                                                    if (purchaseReader.Name == "free-quantity")
                                                                    {
                                                                        if (purchaseReader.Read())
                                                                        {
                                                                            if (!string.IsNullOrWhiteSpace(
                                                                                purchaseReader.Value))
                                                                                purchase.FreeQuantity =
                                                                                    purchaseReader.Value.TryParseInt();
                                                                        }
                                                                    }

                                                                    if (purchaseReader.Name == "product")
                                                                    {
                                                                        var product = new YamarketFeedPromoProduct();
                                                                        string prodAttribute =
                                                                            purchaseReader["category-id"];
                                                                        if (prodAttribute != null)
                                                                        {
                                                                            product.CategoryId =
                                                                                prodAttribute.TryParseInt();
                                                                        }

                                                                        prodAttribute = purchaseReader["offer-id"];
                                                                        if (prodAttribute != null)
                                                                        {
                                                                            product.OfferId =
                                                                                prodAttribute.TryParseInt();
                                                                        }

                                                                        purchase.Products.Add(product);
                                                                    }
                                                                }
                                                            }

                                                            obj.Purchase = purchase;
                                                            break;
                                                        case "promo-gifts":
                                                            using (var purchaseReader = elemreader.ReadSubtree())
                                                            {
                                                                while (purchaseReader.Read())
                                                                {
                                                                    if (purchaseReader.Name == "product")
                                                                    {
                                                                        var gift = new YamarketFeedPromoGift();
                                                                        string giftAttribute =
                                                                            purchaseReader["offer-id"];
                                                                        if (giftAttribute != null)
                                                                        {
                                                                            gift.OfferId = giftAttribute.TryParseInt();
                                                                        }

                                                                        obj.Gifts.Add(gift);
                                                                    }
                                                                }
                                                            }

                                                            break;
                                                    }
                                                }
                                            }

                                            feedObj.Promos.Add(obj);
                                        }
                                    }
                                }

                                break;
                        }
                    }
                }
            }

            return feedObj;
        }

        public static GoogleFeedObject ReadGoogleFeed(string filePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore
            };

            var feedObj = new GoogleFeedObject();

            using (var lStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var xr = XmlReader.Create(lStream, settings))
            {
                while (xr.Read())
                {
                    if (xr.IsStartElement())
                    {
                        switch (xr.Name)
                        {
                            case "title":
                                if (xr.Read())
                                {
                                    feedObj.Title = xr.Value;
                                }

                                break;
                            case "link":
                                if (xr.Read())
                                {
                                    feedObj.Link = xr.Value;
                                }

                                break;
                            case "description":
                                if (xr.Read())
                                {
                                    feedObj.Description = xr.Value;
                                }

                                break;
                            case "item":
                                var obj = new GoogleFeedItem();
                                using (var subreader = xr.ReadSubtree())
                                {
                                    while (subreader.Read())
                                    {
                                        switch (subreader.Name)
                                        {
                                            case "g:id":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.Id = subreader.Value.TryParseInt();
                                                }

                                                break;
                                            case "title":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.Tile = subreader.Value;
                                                }

                                                break;
                                            case "g:description":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.Description = subreader.Value;
                                                }

                                                break;
                                            case "g:google_product_category":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.GoogleProductCategory = subreader.Value;
                                                }

                                                break;
                                            case "g:product_type":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.ProductType = subreader.Value;
                                                }

                                                break;
                                            case "link":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.Link = subreader.Value;
                                                }

                                                break;
                                            case "g:image_link":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.ImageLink = subreader.Value;
                                                }

                                                break;
                                            case "g:condition":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.Condition = subreader.Value;
                                                }

                                                break;
                                            case "g:availability":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.Availability = subreader.Value;
                                                }

                                                break;
                                            case "g:price":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.Price = subreader.Value;
                                                }

                                                break;
                                            case "g:mpn":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.MPN = subreader.Value;
                                                }

                                                break;
                                            case "g:expiration_date":
                                                if (subreader.Read())
                                                {
                                                    if (!string.IsNullOrWhiteSpace(subreader.Value))
                                                        obj.ExpirationTime = subreader.Value.TryParseDateTime();
                                                }

                                                break;
                                        }
                                    }
                                }

                                feedObj.Items.Add(obj);
                                break;
                        }
                    }
                }
            }

            return feedObj;
        }
    }

    #region additional classes

    public class YamarketFeedObject
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Url { get; set; }
        public List<YamarketFeedCurrency> Currencies { get; set; }
        public List<YamarketFeedCategory> Categories { get; set; }
        public List<YamarketFeedDeliveryOtion> DeliveryOptions { get; set; }
        public List<YamarketFeedOffer> Offers { get; set; }
        public List<YamarketFeedGift> Gifts { get; set; }
        public List<YamarketFeedPromo> Promos { get; set; }

        public YamarketFeedObject()
        {
            Currencies = new List<YamarketFeedCurrency>();
            Categories = new List<YamarketFeedCategory>();
            DeliveryOptions = new List<YamarketFeedDeliveryOtion>();
            Offers = new List<YamarketFeedOffer>();
            Gifts = new List<YamarketFeedGift>();
            Promos = new List<YamarketFeedPromo>();
        }
    }

    public class YamarketFeedCurrency
    {
        public string Id { get; set; }
        public float Rate { get; set; }
    }

    public class YamarketFeedCategory
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
    }

    public class YamarketFeedDeliveryOtion
    {
        public int Cost { get; set; }
        public int Days { get; set; }
        public string OrderBefore { get; set; }
    }

    public class YamarketFeedOffer
    {
        public int Id { get; set; }
        public bool Available { get; set; }
        public string Url { get; set; }
        public float Price { get; set; }
        public string CurrencyId { get; set; }
        public int CategoryId { get; set; }
        public string Picture { get; set; }
        public bool Store { get; set; }
        public bool Delivery { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Weight { get; set; }
        public float PurchasePrice { get; set; }
        public bool Pickup { get; set; }
        public string SalesNotes { get; set; }
        public List<YamarketFeedDeliveryOtion> DeliveryOptions { get; set; }
        public List<YamarketFeedOfferPrameter> Parameters { get; set; }

        public YamarketFeedOffer()
        {
            DeliveryOptions = new List<YamarketFeedDeliveryOtion>();
            Parameters = new List<YamarketFeedOfferPrameter>();
        }
    }

    public class YamarketFeedOfferPrameter
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Value { get; set; }
    }

    public class YamarketFeedGift
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
    }

    public class YamarketFeedPromo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string PromoCode { get; set; }
        public string Description { get; set; }
        public string Discount { get; set; }
        public string DiscountType { get; set; }
        public string Url { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public YamarketFeedPromoPurchase Purchase { get; set; }
        public List<YamarketFeedPromoGift> Gifts { get; set; }

        public YamarketFeedPromo()
        {
            Gifts = new List<YamarketFeedPromoGift>();
        }
    }

    public class YamarketFeedPromoPurchase
    {
        public int RequeredQuantity { get; set; }
        public int FreeQuantity { get; set; }
        public List<YamarketFeedPromoProduct> Products { get; set; }

        public YamarketFeedPromoPurchase()
        {
            Products = new List<YamarketFeedPromoProduct>();
        }
    }

    public class YamarketFeedPromoProduct
    {
        public int CategoryId { get; set; }
        public int OfferId { get; set; }
    }

    public class YamarketFeedPromoGift
    {
        public int OfferId { get; set; }
    }

    public class GoogleFeedObject
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public List<GoogleFeedItem> Items { get; set; }

        public GoogleFeedObject()
        {
            Items = new List<GoogleFeedItem>();
        }
    }

    public class GoogleFeedItem
    {
        public int Id { get; set; }
        public string Tile { get; set; }
        public string Description { get; set; }
        public string GoogleProductCategory { get; set; }
        public string ProductType { get; set; }
        public string Link { get; set; }
        public string ImageLink { get; set; }
        public string Condition { get; set; }
        public string Availability { get; set; }
        public string Price { get; set; }
        public string MPN { get; set; }
        public DateTime ExpirationTime { get; set; }
    }

    #endregion

    #endregion
}