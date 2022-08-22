//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Catalog
{

    public class CustomOptionsService
    {
        #region  Public CustomOption Methods
        public static int AddCustomOption(CustomOption copt)
        {
            var id = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                                                        "[Catalog].[sp_AddCustomOption]",
                                                        CommandType.StoredProcedure,
                                                        new[]
                                                             {
                                                                 new SqlParameter("@Title", copt.Title), 
                                                                 new SqlParameter("@IsRequired", copt.IsRequired), 
                                                                 new SqlParameter("@InputType", copt.InputType), 
                                                                 new SqlParameter("@SortOrder", copt.SortOrder), 
                                                                 new SqlParameter("@ProductID", copt.ProductId)
                                                             }
                                                          ));
            if (id != 0)
            {
                foreach (var optionItem in copt.Options)
                {
                    if (optionItem.Title != null)
                    {
                        AddOption(optionItem, id);
                    }
                }
            }
            return id;
        }

        public static void UpdateCustomOption(CustomOption copt)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateCustomOption]", CommandType.StoredProcedure,
                                            new SqlParameter("@CustomOptionsId", copt.CustomOptionsId),
                                            new SqlParameter("@Title", copt.Title),
                                            new SqlParameter("@IsRequired", copt.IsRequired),
                                            new SqlParameter("@InputType", copt.InputType),
                                            new SqlParameter("@SortOrder", copt.SortOrder),
                                            new SqlParameter("@ProductID", copt.ProductId));
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Options] WHERE [CustomOptionsId] = @CustomOptionsId",
                                            CommandType.Text, new SqlParameter("@CustomOptionsId", copt.CustomOptionsId));
            foreach (var optionItem in copt.Options)
            {
                AddOption(optionItem, copt.CustomOptionsId);
            }
        }

        public static void DeleteCustomOption(int customOptionId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteCustomOption]", CommandType.StoredProcedure, new SqlParameter("@CustomOptionsId", customOptionId));
        }

        public static void DeleteCustomOptionByProduct(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CustomOptions] where [ProductID] = @productId", CommandType.Text, new SqlParameter("@productId", productId));
        }

        public static List<CustomOption> GetCustomOptionsByProductId(int productId)
        {
            var options =
                SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetCustomOptionsByProductId]", CommandType.StoredProcedure,
                    GetCustomOptionFromReader, new SqlParameter("@ProductId", productId));

            foreach (var item in options)
            {
                if (item.IsRequired && item.Options.Count > 0)
                {
                    item.SelectedOptions = item.Options.ElementAt(0);
                }
            }

            return options;
        }

        public static bool DoesProductHaveRequiredCustomOptions(string productId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCustomOptionsIsRequiredByProductId]", CommandType.StoredProcedure, new SqlParameter("@ProductId", productId)) > 0;
        }

        public static bool DoesProductHaveRequiredCustomOptions(int productId)
        {
            return DoesProductHaveRequiredCustomOptions(productId.ToString(CultureInfo.InvariantCulture));
        }
        

        // Получаем сумму кастом опций по цене продукта, и сериализованным опциям
        public static float GetCustomOptionPrice(float price, string attributeXml, float baseCurrencyValue)
        {
            if (string.IsNullOrEmpty(attributeXml))
                return 0;

            return GetCustomOptionPrice(price, DeserializeFromXml(attributeXml, baseCurrencyValue));
        }

        // Получаем сумму каcтом опций по цене продукта, и списку десериализованных кастом опций
        public static float GetCustomOptionPrice(float price, IEnumerable<EvaluatedCustomOptions> customOptions)
        {
            float fixedPrice = 0;
            float percentPrice = 0;

            if (customOptions != null)
            {
                foreach (var item in customOptions)
                {
                    switch (item.OptionPriceType)
                    {
                        case OptionPriceType.Fixed:
                            fixedPrice += item.OptionPriceBc;
                            break;

                        case OptionPriceType.Percent:
                            percentPrice += price * item.OptionPriceBc * 0.01F;
                            break;
                    }
                }
            }

            return (fixedPrice + percentPrice); // CurrencyService.CurrentCurrency.Rate 
        }

        #endregion

        #region  Public OptionItem Methods
        public static void AddOption(OptionItem opt, int customOptionsId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_AddOption]", CommandType.StoredProcedure,
                                            new SqlParameter("@CustomOptionsId", customOptionsId),
                                            new SqlParameter("@Title", opt.Title),
                                            new SqlParameter("@PriceBC", opt.BasePrice),
                                            new SqlParameter("@PriceType", opt.PriceType),
                                            new SqlParameter("@SortOrder", opt.SortOrder));
        }

        public static void DeleteOption(int optionId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteOption]", CommandType.StoredProcedure, new SqlParameter("@OptionID", optionId));
        }

        public static void UpdateOption(OptionItem opt, int customOptionsId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateOption]", CommandType.StoredProcedure,
                                            new SqlParameter("@OptionID", opt.OptionId),
                                            new SqlParameter("@CustomOptionsId", customOptionsId),
                                            new SqlParameter("@Title", opt.Title),
                                            new SqlParameter("@PriceBC", opt.BasePrice),
                                            new SqlParameter("@PriceType", opt.PriceType),
                                            new SqlParameter("@SortOrder", opt.SortOrder));
        }

        public static List<OptionItem> GetCustomOptionItems(int customOptionId)
        {
            return SQLDataAccess.ExecuteReadList<OptionItem>("[Catalog].[sp_GetOptionsByCustomOptionId]", CommandType.StoredProcedure, GetOptionItemFromReader, new SqlParameter("@CustomOptionId", customOptionId));
        }

        public static void SubmitCustomOptionsWithSameProductId(int productId, List<CustomOption> list)
        {
            var oldlist = GetCustomOptionsByProductId(productId);
            //Deleting
            foreach (CustomOption copt in oldlist.Where(opt => list.WithId(opt.CustomOptionsId) == null))
            {
                DeleteCustomOption(copt.CustomOptionsId);
            }
            //Updating
            foreach (CustomOption copt in list.Where(copt => oldlist.WithId(copt.CustomOptionsId) != null))
            {
                UpdateCustomOption(copt);
            }
            //Adding
            foreach (CustomOption copt in list.AllWithId(-1))
            {
                AddCustomOption(copt);
            }
        }

        public static List<EvaluatedCustomOptions> GetEvaluatedCustomOptions(List<CustomOption> options, List<OptionItem> values)
        {
            var result = new List<EvaluatedCustomOptions>();

            for (int i = 0; i <= options.Count - 1; i++)
            {
                if (i < values.Count && values[i] != null)
                {
                    result.Add(new EvaluatedCustomOptions()
                    {
                        CustomOptionId = options[i].CustomOptionsId,
                        CustomOptionTitle = options[i].Title,
                        OptionId = values[i].OptionId,
                        OptionTitle = values[i].Title,
                        OptionPriceBc = values[i].BasePrice,
                        OptionPriceType = values[i].PriceType
                    });
                }
            }
            return result;
        }

        public static string SerializeToXml(List<EvaluatedCustomOptions> evcoList)
        {
            if (evcoList == null || !evcoList.Any())
                return string.Empty;


            var doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Options");
            foreach (var evco in evcoList)
            {
                XmlElement xopt = doc.CreateElement("Option");
                XmlElement xel = doc.CreateElement("CustomOptionId");
                xel.InnerText = evco.CustomOptionId.ToString(CultureInfo.InvariantCulture);
                xopt.AppendChild(xel);
                xel = doc.CreateElement("CustomOptionTitle");
                xel.InnerText = evco.CustomOptionTitle;
                xopt.AppendChild(xel);

                xel = doc.CreateElement("OptionId");
                xel.InnerText = evco.OptionId.ToString(CultureInfo.InvariantCulture);
                xopt.AppendChild(xel);
                xel = doc.CreateElement("OptionTitle");
                xel.InnerText = evco.OptionTitle;
                xopt.AppendChild(xel);
                xel = doc.CreateElement("OptionPriceBC");
                xel.InnerText = evco.OptionPriceBc.ToString(CultureInfo.InvariantCulture);
                xopt.AppendChild(xel);
                xel = doc.CreateElement("OptionPriceType");
                xel.InnerText = evco.OptionPriceType.ToString();
                xopt.AppendChild(xel);
                root.AppendChild(xopt);
            }
            doc.AppendChild(root);

            string str;
            using (var memstream = new MemoryStream())
            {

                var wrtr = new XmlTextWriter(memstream, null);
                doc.WriteTo(wrtr);
                wrtr.Close();
                byte[] buff = memstream.GetBuffer();
                int eidx = buff.Length - 1;
                for (int i = buff.Length - 1; i >= 0; i--)
                {
                    if (buff[i] == 0)
                        continue;
                    eidx = i;
                    break;
                }

                str = Encoding.UTF8.GetString(buff, 0, eidx + 1);
                memstream.Close();
            }

            return str;
        }

        /// <summary>
        /// Get custom options from string
        /// </summary>
        /// <param name="xml">Custom option xml</param>
        /// <param name="baseCurrencyValue">Product currency rate</param>
        /// <returns></returns>
        public static List<EvaluatedCustomOptions> DeserializeFromXml(string xml, float baseCurrencyValue)
        {
            if (String.IsNullOrWhiteSpace(xml))
                return null;

            try
            {
                var res = new List<EvaluatedCustomOptions>();

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                foreach (XmlElement xel in doc.GetElementsByTagName("Option"))
                {
                    var xelm = xel.GetElementsByTagName("OptionId")[0];
                    if (int.Parse(xelm.InnerText) < 0)
                    {
                        continue;
                    }
                    var evco = new EvaluatedCustomOptions();
                    xelm = xel.GetElementsByTagName("CustomOptionId")[0];
                    evco.CustomOptionId = int.Parse(xelm.InnerText);

                    xelm = xel.GetElementsByTagName("OptionId")[0];
                    evco.OptionId = int.Parse(xelm.InnerText);

                    xelm = xel.GetElementsByTagName("OptionTitle")[0];
                    evco.OptionTitle = xelm.InnerText;

                    xelm = xel.GetElementsByTagName("CustomOptionTitle")[0];
                    evco.CustomOptionTitle = xelm.InnerText;

                    xelm = xel.GetElementsByTagName("OptionPriceType")[0];
                    evco.OptionPriceType = (OptionPriceType)Enum.Parse(typeof(OptionPriceType), xelm.InnerText);

                    xelm = xel.GetElementsByTagName("OptionPriceBC")[0];
                    var price = float.Parse(xelm.InnerText, CultureInfo.InvariantCulture);
                    evco.OptionPriceBc = evco.OptionPriceType == OptionPriceType.Percent
                        ? price
                        : price.RoundPrice(baseCurrencyValue);

                    res.Add(evco);
                }

                return res;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        public static string GetJsonHash(List<EvaluatedCustomOptions> options)
        {
            return options.Any()
                ? Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
                    options.Select(x => new
                    {
                        x.CustomOptionId,
                        x.CustomOptionTitle,
                        x.OptionId,
                        x.OptionTitle,
                        x.OptionPriceBc,
                        x.OptionPriceType
                    }).ToList())))
                : string.Empty;
        }

        public static List<EvaluatedCustomOptions> GetFromJsonHash(string hash, float baseCurrencyValue)
        {
            if (hash.IsNullOrEmpty())
                return null;
            try
            {
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(hash));
                return DeserializeFromJson(json, baseCurrencyValue);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }
        }

        public static string SerializeToJson(List<EvaluatedCustomOptions> evcoList)
        {
            if (evcoList == null || !evcoList.Any())
                return string.Empty;
            return JsonConvert.SerializeObject(evcoList);
        }

        public static List<EvaluatedCustomOptions> DeserializeFromJson(string json, float baseCurrencyValue)
        {
            try
            {
                var result = json.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<List<EvaluatedCustomOptions>>(json);
                if (result == null || !result.Any())
                    return null;

                foreach (var evco in result)
                {
                    if (evco.OptionPriceType == OptionPriceType.Fixed)
                        evco.OptionPriceBc = evco.OptionPriceBc.RoundPrice(baseCurrencyValue);
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }
        }

        public static bool IsValidCustomOptions(ShoppingCartItem item)
        {
            if (string.IsNullOrWhiteSpace(item.AttributesXml))
                return true;

            var selectedOptions =
                CustomOptionsService.DeserializeFromXml(item.AttributesXml, item.Offer.Product.Currency.Rate);

            if (selectedOptions == null)
                return true;

            var productOptions = CustomOptionsService.GetCustomOptionsByProductId(item.Offer.ProductId);

            foreach (var option in selectedOptions)
            {
                if (productOptions.Find(x => x.CustomOptionsId == option.CustomOptionId && x.Options != null &&
                                             x.Options.Any(o => o.OptionId == option.OptionId)) == null)
                    return false;
            }

            return true;
        }

        #endregion

        #region  Private Methods
        private static CustomOption GetCustomOptionFromReader(SqlDataReader reader)
        {
            return new CustomOption
            {
                CustomOptionsId = SQLDataHelper.GetInt(reader, "CustomOptionsID"),
                Title = SQLDataHelper.GetString(reader, "Title"),
                InputType = (CustomOptionInputType)SQLDataHelper.GetInt(reader, "InputType"),
                IsRequired = SQLDataHelper.GetBoolean(reader, "IsRequired"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID")
            };
        }

        private static OptionItem GetOptionItemFromReader(SqlDataReader reader)
        {
            return new OptionItem()
            {
                OptionId = SQLDataHelper.GetInt(reader, "OptionID"),
                //CustomOptionsId = SQLDataHelper.GetInt(reader, "CustomOptionsId"),
                Title = SQLDataHelper.GetString(reader, "Title"),
                BasePrice = SQLDataHelper.GetFloat(reader, "PriceBC"),
                CurrencyRate = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                PriceType = (OptionPriceType)SQLDataHelper.GetInt(reader, "PriceType"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }
        #endregion

        public static string OptionsToString(List<OptionItem> options)
        {
            var res = new StringBuilder();
            for (var i = 0; i < options.Count; i++)
            {
                res.Append("{");
                res.Append(options[i].Title);
                res.Append("|");
                res.Append(options[i].BasePrice.ToString("F2"));
                res.Append("|");
                res.Append(options[i].PriceType);
                res.Append("|");
                res.Append(options[i].SortOrder);
                res.Append("}");
            }
            return res.ToString();
        }

        public static string CustomOptionsToString(List<CustomOption> customOptions)
        {   //[title|type|IsRequired|sortOrder:{value|price|typePrice|sort}...] 
            //[Цвет1|drop|1|0:{Синий|100|f|10}{Красный|10|p|20}],....
            var res = new StringBuilder();
            for (var i = 0; i < customOptions.Count; i++)
            {
                res.Append("[");
                res.Append(customOptions[i].Title);
                res.Append("|");
                res.Append(customOptions[i].InputType);
                res.Append("|");
                res.Append(customOptions[i].IsRequired ? 1 : 0);
                res.Append("|");
                res.Append(customOptions[i].SortOrder);
                res.Append(":");
                res.Append(OptionsToString(customOptions[i].Options));
                res.Append("]");
            }
            return res.ToString();
        }

        public static void CustomOptionsFromString(int productId, string customOptionString)
        {
            try
            {
                DeleteCustomOptionByProduct(productId);
                if (string.IsNullOrEmpty(customOptionString))
                    return;
                var items = customOptionString.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in items)
                {
                    var ind = item.LastIndexOf(":{"); // со значениями
                    var customOptionStr = ind != -1 ? item.Substring(0, ind) : item.TrimEnd(':');
                    var customOption = ParseCustomOption(customOptionStr, productId);
                    if (customOption == null)
                        continue;

                    var optionsStr = ind != -1 ? item.Substring(ind + 1) : null;
                    if (optionsStr.IsNotEmpty())
                        customOption.Options = ParseOption(optionsStr);
                    
                    AddCustomOption(customOption);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private static CustomOption ParseCustomOption(string source, int productId)
        {
            var parts = source.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4) return null;
            return new CustomOption
            {
                Title = parts[0],
                InputType = parts[1].TryParseEnum<CustomOptionInputType>(),
                IsRequired = parts[2] == "1",
                SortOrder = parts[3].TryParseInt(),
                ProductId = productId
            };
        }
        private static List<OptionItem> ParseOption(string source)
        {
            var res = new List<OptionItem>();
            var items = source.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var parts in items.Select(item => item.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (parts.Length == 4) //return null;
                res.Add(new OptionItem
                    {
                        Title = parts[0],
                        BasePrice = parts[1].TryParseFloat(),
                        PriceType = parts[2].TryParseEnum<OptionPriceType>(),
                        SortOrder = parts[3].TryParseInt()
                    });
                if (parts.Length == 3) //return null;
                    res.Add(new OptionItem
                    {
                        Title = string.Empty,//parts[0],
                        BasePrice = parts[0].TryParseFloat(),
                        PriceType = parts[1].TryParseEnum<OptionPriceType>(),
                        SortOrder = parts[2].TryParseInt()
                    });
            }
            return res;
        }
        
        public static string RenderSelectedOptions(string xml, float baseCurrencyRate)
        {
            if (String.IsNullOrEmpty(xml))
                return String.Empty;

            var result = new StringBuilder("<div class=\"customoptions\">");

            foreach (var item in DeserializeFromXml(xml, baseCurrencyRate))
            {

                result.AppendFormat("<div class=\"customoption-item\">{0}: {1}", item.CustomOptionTitle, item.OptionTitle);
                if (item.OptionPriceBc != 0)
                {
                    result.Append(" ");
                    if (item.OptionPriceBc > 0)
                        result.Append("+" +
                                      (item.OptionPriceType == OptionPriceType.Fixed
                                          ? PriceFormatService.FormatPrice(item.OptionPriceBc)
                                          : item.OptionPriceBc + " %"));
                }
                result.Append("</div>");
            }

            result.Append("</div>");

            return result.ToString();
        }

        public static string RenderSelectedOptions(List<EvaluatedCustomOptions> customOptions)
        {
            if (customOptions == null)
                return String.Empty;

            var result = new StringBuilder("<div class=\"customoptions\">");

            foreach (var option in customOptions)
            {
                result.AppendFormat("<div class=\"customoption-item\">{0}: {1}", option.CustomOptionTitle, option.OptionTitle);
                if (option.OptionPriceBc != 0)
                {
                    result.Append(" ");
                    if (option.OptionPriceBc > 0)
                        result.Append("+" +
                                      (option.OptionPriceType == OptionPriceType.Fixed
                                          ? PriceFormatService.FormatPrice(option.OptionPriceBc)
                                          : option.OptionPriceBc + " %"));
                }
                result.Append("</div>");
            }

            result.Append("</div>");

            return result.ToString();
        }

    }
}