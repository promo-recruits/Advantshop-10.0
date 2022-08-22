using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Export;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    public class VkMarketApiService
    {
        private const string ApiUrl = "https://api.vk.com/method/";
        private const string ApiVersion = "5.131";

        #region Auth

        public VkApi Auth()
        {
            if (string.IsNullOrEmpty(SettingsVk.TokenUser) || SettingsVk.UserId == 0)
                return null;

            try
            {
                var vk = new VkApi() {RequestsPerSecond = 2};
                vk.Authorize(new ApiAuthParams() {AccessToken = SettingsVk.TokenUser, UserId = SettingsVk.UserId});

                return vk;
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);

                var errors = VkMarketSettings.TokenErrorsCount;
                if (errors > 5)
                {
                    VkMarketSettings.TokenErrorsCount = 0;
                    SettingsVk.TokenUser = null;
                    SettingsVk.UserId = 0;
                }
                else
                {
                    VkMarketSettings.TokenErrorsCount = errors + 1;
                }

                throw new BlException("VkMarketApiService.Auth авторизация не прошла");
            }
        }

        #endregion

        public bool IsActive()
        {
            return !string.IsNullOrEmpty(SettingsVk.TokenUser) && SettingsVk.UserId != 0 && SettingsVk.Group != null;
        }

        #region Market Categories

        /// <summary>
        /// Список категорий из ВКонтакте
        /// </summary>
        public List<VkMarketCategory> GetMarketCategories()
        {
            return CacheManager.Get("VkMarketApiService.MarketCategories", () =>
            {
                try
                {
                    var vk = Auth();
                    return vk.Markets.GetCategories(1000, 0).Select(x => new VkMarketCategory(x)).ToList();
                }
                catch (Exception ex)
                {
                    Debug.Log.Warn(ex);
                }

                return null;
            });
        }

        #endregion

        #region Groups

        public List<VkGroup> GetGroups()
        {
            var vk = Auth();

            var groups = vk.Groups.Get(new GroupsGetParams()
            {
                Count = 1000,
                Extended = true,
                UserId = SettingsVk.UserId,
                Fields = GroupsFields.All,
                Filter = GroupsFilters.Moderator,
            }).Select(x => new VkGroup(x)).ToList();

            return groups;
        }

        public VkGroupSettings GetGroupSettings(long groupId)
        {
            var vk = Auth();

            try
            {
                // для настроек нужно stanalone приложение
                var settings = vk.Groups.GetSettings((ulong) groupId);
                if (settings == null)
                    return null;

                return new VkGroupSettings()
                {
                    IsMarketEnabled = settings.Market != null && settings.Market.Enabled == true,
                    MarketCurrency = settings.MarketCurrency.ToString()
                };
            }
            catch // Access denied: only group admin could access this method
            {
                // ignored
            }

            return null;
        }

        #endregion

        #region Categories

        public long AddAlbum(string name, VkApi vkApi = null)
        {
            var vk = vkApi ?? Auth();
            return vk.Markets.AddAlbum(-SettingsVk.Group.Id, name);
        }

        public bool UpdateAlbum(long albumId, string name, VkApi vkApi = null)
        {
            var vk = vkApi ?? Auth();
            try
            {
                return vk.Markets.EditAlbum(-SettingsVk.Group.Id, albumId, name);
            }
            catch (VkApiException ex)
            {
                Debug.Log.Warn(ex);
            }

            return false;
        }

        public bool DeleteAlbum(long albumId, VkApi vkApi = null)
        {
            var vk = vkApi ?? Auth();
            try
            {
                return vk.Markets.DeleteAlbum(-SettingsVk.Group.Id, albumId);
            }
            catch (VkApiException ex)
            {
                Debug.Log.Warn(ex);
            }

            return false;
        }

        public List<MarketAlbum> GetAllAlbums(VkApi vkApi = null)
        {
            var vk = vkApi ?? Auth();
            return vk.Markets.GetAlbums(-SettingsVk.Group.Id, 0, 100).ToList();
        }

        /// <summary>
        /// Изменяет положение подборки с товарами в списке.
        /// </summary>
        /// <param name="albumId">идентификатор подборки</param>
        /// <param name="before">идентификатор подборки, перед которой следует поместить текущую.</param>
        /// <param name="after">идентификатор подборки, после которой следует поместить текущую</param>
        /// <returns></returns>
        public bool ReorderAlbums(long albumId, long? before, long? after)
        {
            var vk = Auth();
            try
            {
                return vk.Markets.ReorderAlbums(-SettingsVk.Group.Id, albumId, before, after);
            }
            catch (VkApiException ex)
            {
                Debug.Log.Warn(ex);
            }

            return false;
        }

        #endregion

        #region Products

        public bool IsProductExistInVk(VkApi vk, long vkProductId, long groupId)
        {
            var id = string.Format("{0}_{1}", groupId, vkProductId);

            var product = vk.Markets.GetById(new[] {id}).FirstOrDefault();
            return product != null && product.Availability != ProductAvailability.Removed;
        }

        public long AddProduct(VkApi vk, VkProduct product)
        {
            var data = new VkApiProduct(product).ToString();

            var result = MakeRequest<VkApiProductAddResult>("market.add", ApiVersion, data, true);
            if (result == null || result.Response == null)
                return 0;

            product.Id = result.Response.market_item_id;

            Thread.Sleep(200);

            vk.Markets.AddToAlbum(product.OwnerId, product.Id, new[] {product.AlbumId});

            return product.Id;
        }

        public bool UpdateProduct(VkApi vk, VkProduct product, bool firstTry = true)
        {
            var data = new VkApiProductEdit(product).ToString();

            var result = MakeRequest<VkApiProductUpdateResult>("market.edit", ApiVersion, data, true);
            return result != null && result.Response != 0;
        }

        public bool DeleteProduct(VkApi vk, long groupId, long id)
        {
            try
            {
                return vk.Markets.Delete(groupId, id);
            }
            catch (VkApiException ex)
            {
                Debug.Log.Warn(ex);
            }

            return false;
        }

        public IEnumerable<Market> GetProducts(VkApi vk, long groupId, long? albumId)
        {
            var offset = 0;

            while (true)
            {
                var products = vk.Markets.Get(groupId, albumId, 200, offset, true);

                if (products == null || products.Count == 0)
                    break;

                foreach (var product in products)
                {
                    yield return product;
                }

                if (products.Count < 200)
                    break;

                offset += 200;
            }
        }

        public Market GetFirstProduct(VkApi vk, long groupId, long? albumId)
        {
            var product = vk.Markets.Get(groupId, albumId, 1, 0, true).FirstOrDefault();
            return product;
        }

        #endregion

        #region Photos

        public Photo AddPhoto(VkApi vk, long groupId, bool mainPhoto, string filePath)
        {
            // Получить адрес сервера для загрузки.
            var server = vk.Photo.GetMarketUploadServer(groupId, mainPhoto);
            if (server == null || server.UploadUrl == null)
                return null;

            // Загрузить фотографию.
            var wc = new WebClient();
            var responseImg = Encoding.ASCII.GetString(wc.UploadFile(server.UploadUrl, filePath));
            if (string.IsNullOrEmpty(responseImg))
                return null;

            // Сохранить загруженную фотографию
            var photos = vk.Photo.SaveMarketPhoto(groupId, responseImg);

            return photos?.FirstOrDefault();
        }

        public bool DeletePhoto(VkApi vk, long photoId, long groupId)
        {
            try
            {
                return vk.Photo.Delete((ulong) photoId, -groupId);
            }
            catch (Exception ex) // обложку удалить нельзя, будет падать
            {
                Debug.Log.Warn(ex);
            }

            return false;
        }

        #endregion

        #region Orders

        public List<VkOrder> GetOrders(int offset = 0)
        {
            var orders = new List<VkOrder>();
            var count = 50;
            try
            {
                var data = $"group_id={SettingsVk.Group.Id}&offset={offset}&count={count}";

                var result = MakeRequest<VkOrderResult>("market.getGroupOrders", "5.122", data);
                if (result != null && result.Response != null)
                {
                    orders.AddRange(result.Response.Items.Where(x => x.Status != VkOrderStatus.Canceled));

                    if (result.Response.Count > (offset + 1) * count)
                    {
                        var items = GetOrders(offset + 1);
                        orders.AddRange(items);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            Thread.Sleep(300);

            return orders;
        }

        public List<VkOrderItem> GetOrderItems(int orderId, long userId)
        {
            var data = $"user_id={userId}&order_id={orderId}&offset=0&count={1000}";

            var result = MakeRequest<VkOrderItemResult>("market.getOrderItems", ApiVersion, data);
            if (result != null && result.Response != null)
                return result.Response.Items;

            return null;
        }

        #endregion


        // docs: https://docs.google.com/document/d/1W6PqyDLfNwVJSR4QrEjQZbbbXKOLhRSIKUygRg468E0/edit
        // docs: https://docs.google.com/document/d/1Us8qWWiPCc238MTNa4Goib-b4RKNO-VlDGg1gJchgu0/edit

        #region Properties

        public long AddProperty(string title, string type = "text")
        {
            title = title.Reduce(50);

            var data = $"group_id={SettingsVk.Group.Id}&title={HttpUtility.UrlEncode(title)}&type={type}";  // color?

            var result = MakeRequest<VkResponse<VkResponseProperty>>("market.addProperty", ApiVersion, data);

            if (result != null && result.Response != null)
                return result.Response.PropertyId;
            
            return 0;
        }

        public bool EditProperty(long propertyId, string title)
        {
            title = title.Reduce(50);

            var data = $"group_id={SettingsVk.Group.Id}&property_id={propertyId}&title={HttpUtility.UrlEncode(title)}&type=text";  // color?

            var result = MakeRequest<VkResponse<int>>("market.editProperty", ApiVersion, data);

            return result != null && result.Response == 1;
        }
        
        public bool DeleteProperty(long propertyId)
        {
            var data = $"group_id={SettingsVk.Group.Id}&property_id={propertyId}";

            var result = MakeRequest<VkResponse<int>>("market.deleteProperty", ApiVersion, data);

            return result != null && result.Response == 1;
        }

        public List<VkResponsePropertyItem> GetProperties()
        {
            var data = $"group_id={SettingsVk.Group.Id}";

            var result = MakeRequest<VkResponse<VkResponseProperties>>("market.getProperties", ApiVersion, data);
            
            return
                (result != null && result.Response != null && result.Response.Items != null
                    ? result.Response.Items
                    : null) ?? new List<VkResponsePropertyItem>();
        }

        #endregion

        #region Property Variant

        public long AddPropertyVariant(long propertyId, string title, string value)
        {
            title = title.Reduce(60);
            value = value.Reduce(10);

            var data = $"group_id={SettingsVk.Group.Id}&property_id={propertyId}&title={HttpUtility.UrlEncode(title)}&value={HttpUtility.UrlEncode(value)}";

            var result = MakeRequest<VkResponse<VkResponsePropertyVariant>>("market.addPropertyVariant", ApiVersion, data);
            if (result != null && result.Response != null)
                return result.Response.Id;

            return 0;
        }

        public bool EditPropertyVariant(long variantId, string title, string value)
        {
            title = title.Reduce(60);
            value = value.Reduce(10);

            var data = $"group_id={SettingsVk.Group.Id}&variant_id={variantId}&name={HttpUtility.UrlEncode(title)}&value={HttpUtility.UrlEncode(value)}";

            var result = MakeRequest<VkResponse<int>>("market.editPropertyVariant", ApiVersion, data);

            return result != null && result.Response == 1;
        }

        public bool DeletePropertyVariant(long variantId)
        {
            var data = $"group_id={SettingsVk.Group.Id}&variant_id={variantId}";

            var result = MakeRequest<VkResponse<int>>("market.deletePropertyVariant", ApiVersion, data);

            return result != null && result.Response == 1;
        }

        #endregion

        #region Group Items

        /// <summary>
        /// Объединяет товары в группу товаров
        /// </summary>
        /// <returns>В случае успеха возвращается идентификатор группы товаров</returns>
        public long GroupItems(List<long> productIds, long? itemGroupId)
        {
            var data = $"group_id={SettingsVk.Group.Id}&item_ids={String.Join(",", productIds)}{(itemGroupId != null ? "&item_group_id=" + itemGroupId.Value : "")}";

            var result = MakeRequest<VkResponse<VkResponseGroup>>("market.groupItems", ApiVersion, data);

            if (result != null && result.Response != null)
                return result.Response.GroupId;

            return 0;
        }

        /// <summary>
        /// Разделяет группу товаров на несколько товаров
        /// </summary>
        public bool UnGroupItems(long itemGroupId)
        {
            var data = $"group_id={SettingsVk.Group.Id}&item_group_id={itemGroupId}";

            var result = MakeRequest<VkResponse<int>>("market.ungroupItems", ApiVersion, data);

            return result != null && result.Response == 1;
        }

        #endregion

        #region help methods

        private T MakeRequest<T>(string methodUrl, string version, string data, bool throwException = false) where T : IVkError
        {
            Thread.Sleep(200);

            var url = ApiUrl + methodUrl + $"?access_token={SettingsVk.TokenUser}&v={version}";

            var result = RequestHelper.MakeRequest<T>(url, data, method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);

            if (result != null)
            {
                var resError = (IVkError) result;
                if (resError.Error != null)
                {
                    var error = $"{methodUrl} {result.Error.ErrorCode} {result.Error.ErrorMsg} {data}";
                    
                    if (throwException)
                        throw new VkApiException(error);
                    
                    Debug.Log.Warn(error);
                    VkMarketExportState.WriteLog(error);
                }
            }

            return result;
        }

        #endregion
    }
}
