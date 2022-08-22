using AdvantShop.Core.Common.Attributes;
using EShopMode = AdvantShop.Core.ModeConfigService.Modes;

namespace AdvantShop.Track
{
    public enum ETrackEvent
    {
        None,

        #region Dashboard

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_SkipDashboard,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_StoreInfoDone,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ProductDone,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_DesignDone,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_LogoDone,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ShippingPaymentDone,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_DomenDone,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_StaticPagesDone,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ModuleDone,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_SupportDone,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickProductButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDesignButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickLogoDownloadButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickLogoGenerateButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickShippingButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickPaymentButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickBuyDomainButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickBindDomainButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDomenButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickStaticPagesButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickModuleButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickSupportButton,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickCourseLink,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickProductButtonInVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDesignButtonInVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickStaticPagesButtonInVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickModuleButtonInVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDomenButtonInVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickSupportButtonInVideo,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickProductVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDesignVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickShippingVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickPaymentVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickStaticPagesVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickModuleVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDomenVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickSupportVideo,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickEcomhackingLink,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickAdvantshopHelp,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickAdvantshopSupport,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickEcomhackersRu,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDomainsPage,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickStaticpagesPage,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickSettingsPage,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickModuleShoppingCartSettings,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickShowTransformer,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickJoinTheCommunity,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickJoinTheCommunityButtonInVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickJoinTheCommunityVideo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickJoinTheCommunityEcomhackersRu,
        
        #endregion

        #region Landings

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFunnelShow,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateEmptyFunnel_Step_1,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateEmptyFunnel_Step_2,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateEmptyFunnel_Step_3,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step0,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step1,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step2,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step3,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step4,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step5,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_StepFinal,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateArticleFunnel_Step1,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateArticleFunnel_Step2,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateArticleFunnel_Step3,

        #endregion

        #region Trial

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_FillUserData,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_UserDataFormShown,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ClearData,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ChangeLogo,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ChangeAboutData,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_AddCategory,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_AddProduct,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_EditProduct,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_AddCarousel,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_AddOrder,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_AddCustomer,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_VisitCRM,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_AddLead,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_VisitTasks,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_AddTask,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ImportCSV,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ImportYML,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_VisitClientSide,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ChangeDesignTransformer,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ApplyDesignTemplate,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_PreviewDesignTemplate,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_VisitMobileVersion,
        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_AddOrderFromClientSide,
        // Зашел в n-ый день триала
        [TrackEvent(EShopMode.TrialMode, EventKey = "Trial.DailyVisit")]
        Trial_DailyVisit,

        #endregion

        #region Core

        #region Orders

        // Создан заказ из админки
        [TrackEvent("Product.Core.Orders.OrderCreated.AdminArea")]
        Core_Orders_OrderCreated_AdminArea,
        
        // Создан заказ из клиентки магазина (desktop)
        [TrackEvent("Product.Core.Orders.OrderCreated.Desktop")]
        Core_Orders_OrderCreated_Desktop,
        
        // Создан заказ из клиентки магазина (mobile version)
        [TrackEvent("Product.Core.Orders.OrderCreated.Mobile")]
        Core_Orders_OrderCreated_Mobile,
        
        // Создан заказ из воронки
        [TrackEvent("Product.Core.Orders.OrderCreated.SalesFunnel")]
        Core_Orders_OrderCreated_SalesFunnel,
        
        // Изменен статус заказа
        [TrackEvent("Product.Core.Orders.OrderStatusChanged")]
        Core_Orders_OrderStatusChanged,

        // Заказ изменен
        [TrackEvent("Product.Core.Orders.EditOrder")]
        Core_Orders_EditOrder,

        // Добавлен товар в заказ админом
        [TrackEvent("Product.Core.Orders.AddOrderItem")]
        Core_Orders_OrderItemAdded,

        // Выполнен экспорт заказов
        [TrackEvent("Product.Core.Orders.ExportOrders")]
        Core_Orders_ExportOrders,

        // Добавлен комментарий к заказу в обсуждения
        [TrackEvent("Product.Core.Orders.AddComment.Discussion")]
        Core_Orders_AddComment_Discussion,

        // Добавлен комментарий в ленту события из заказа
        [TrackEvent("Product.Core.Orders.AddComment.Events")]
        Core_Orders_AddComment_Events,

        // Заказ переведен в статус оплачен
        [TrackEvent("Product.Core.Orders.OrderPayed.AdminArea")]
        Core_Orders_OrderPayed_AdminArea,

        // Печать заказа
        [TrackEvent("Product.Core.Orders.PrintOrder.AdminArea")]
        Core_Orders_PrintOrder_AdminArea,

        // Экспорт заказа в Excel
        [TrackEvent("Product.Core.Orders.ExportExcel")]
        Core_Orders_ExportExcel,

        // Заказ подтвержден, разрешить оплату
        [TrackEvent("Product.Core.Orders.OrderConfirmedByManager")]
        Core_Orders_OrderConfirmedByManager,

        // Заказ передан в СДЕК (другие службы доставки) на каждую слубжу отдельный event
        [TrackEvent("Product.Core.Orders.OrderSentToDeliveryService")]
        Core_Orders_OrderSentToDeliveryService,

        // Добавлен новый статус заказа
        [TrackEvent("Product.Core.Orders.OrderStatusCreated")]
        Core_Orders_OrderStatusCreated,

        // Отправить письмо из заказа
        [TrackEvent("Product.Core.Orders.SendLetterToCustomer")]
        Core_Orders_SendLetterToCustomer,

        // Позвонить из заказа
        [TrackEvent("Product.Core.Orders.CallCustomer")]
        Core_Orders_CallCustomer,

        // Отправить СМС из заказа
        [TrackEvent("Product.Core.Orders.SendSmsToCustomer")]
        Core_Orders_SendSmsToCustomer,

        #endregion

        #region Customers

        // Создан покупатель
        [TrackEvent("Product.Core.Customers.CustomerCreated")]
        Core_Customers_CustomerCreated,

        // Добавлен комментарий в ленту события из покупателя
        [TrackEvent("Product.Core.Customers.AddComment.Events")]
        Core_Customers_AddComment_Events,

        // Отредактированны данные покупателя
        [TrackEvent("Product.Core.Customers.EditCustomer")]
        Core_Customers_EditCustomer,

        // Выставлен статус (VIP, Bad)
        [TrackEvent("Product.Core.Customers.StatusChanged")]
        Core_Customers_StatusChanged,

        // Отправить письмо из покупателя
        [TrackEvent("Product.Core.Customers.SendLetterToCustomer")]
        Core_Customers_SendLetterToCustomer,

        // Позвонить из покупателя
        [TrackEvent("Product.Core.Customers.CallCustomer")]
        Core_Customers_CallCustomer,

        // Отправить СМС из покупателя
        [TrackEvent("Product.Core.Customers.SendSmsToCustomer")]
        Core_Customers_SendSmsToCustomer,

        // Добавлена группа покупателей
        [TrackEvent("Product.Core.Customers.CustomerGroupCreated")]
        Core_Customers_CustomerGroupCreated,

        // Добавлен сегмент
        [TrackEvent("Product.Core.Customers.CustomerSegmentCreated")]
        Core_Customers_CustomerSegmentCreated,

        // Редактирование сегмента
        [TrackEvent("Product.Core.Customers.EditCustomerSegment")]
        Core_Customers_EditCustomerSegment,

        // Массовая email рассылка по сегменту
        [TrackEvent("Product.Core.Customers.BulkEmailSendingBySegment")]
        Core_Customers_BulkEmailSendingBySegment,

        // Массовая смс рассылка по сегменту
        [TrackEvent("Product.Core.Customers.BulkSmsSendingBySegment")]
        Core_Customers_BulkSmsSendingBySegment,

        // Экспорт покупателей входядих в сегмент в CSV
        [TrackEvent("Product.Core.Customers.ExportCustomersBySegment")]
        Core_Customers_ExportCustomersBySegment,

        // Импортированы покупатели
        [TrackEvent("Product.Core.Customers.ImportCustomers")]
        Core_Customers_ImportCustomers,

        // Покупателю добавлен тег
        [TrackEvent("Product.Core.Customers.AddTagToCustomer")]
        Core_Customers_AddTagToCustomer,

        // Покупателю добавлен тег
        [TrackEvent("Product.Core.Customers.TagCreated")]
        Core_Customers_TagCreated,

        #endregion

        #region Categories

        // Добавленна категория
        [TrackEvent("Product.Core.Categories.CategoryCreated")]
        Core_Categories_CategoryCreated,

        // Изменена категория
        [TrackEvent("Product.Core.Categories.EditCategory")]
        Core_Categories_EditCategory,

        // Добавлена категория списком
        [TrackEvent("Product.Core.Categories.CategoriesListCreated")]
        Core_Categories_CategoriesListCreated,

        // Категории задан тег
        [TrackEvent("Product.Core.Categories.AddTagToCategory")]
        Core_Categories_AddTagToCategory,

        // Категории задана группа свойств
        [TrackEvent("Product.Core.Categories.AddPropertyGroupToCategory")]
        Core_Categories_AddPropertyGroupToCategory,

        // Категории задана автоподборка рекомендуемых товаров
        [TrackEvent("Product.Core.Categories.SetProductRecommendations")]
        Core_Categories_SetProductRecommendations,

        // Экспортированы категории
        [TrackEvent("Product.Core.Categories.ExportCategories")]
        Core_Categories_ExportCategories,

        // Импортированы категории
        [TrackEvent("Product.Core.Categories.ImportCategories")]
        Core_Categories_ImportCategories,

        #endregion

        #region Products

        // Добавлен товар
        [TrackEvent("Product.Core.Products.ProductCreated")]
        Core_Products_ProductCreated,

        // Товар измнен
        [TrackEvent("Product.Core.Products.EditProduct")]
        Core_Products_EditProduct,

        // Добавлен товар списком
        [TrackEvent("Product.Core.Products.ProductListCreated")]
        Core_Products_ProductListCreated,

        // Создана копия товара
        [TrackEvent("Product.Core.Products.ProductCopyCreated")]
        Core_Products_ProductCopyCreated,

        // Товару добавлен тег
        [TrackEvent("Product.Core.Products.AddTagToProduct")]
        Core_Products_AddTagToProduct,

        // Добавлен новый offer
        [TrackEvent("Product.Core.Products.AddOffer")]
        Core_Products_AddOffer,

        // Добавлена фотография по ссылке
        [TrackEvent("Product.Core.Products.AddPhoto.ByUrl")]
        Core_Products_AddPhoto_ByUrl,

        // Добавлена фотография с компа
        [TrackEvent("Product.Core.Products.AddPhoto.File")]
        Core_Products_AddPhoto_File,

        // Добавлена фотография через bing
        [TrackEvent("Product.Core.Products.AddPhoto.Bing")]
        Core_Products_AddPhoto_Bing,

        // Добавлено свойство товара
        [TrackEvent("Product.Core.Products.PropertyCreated")]
        Core_Products_PropertyCreated,

        // Добавлен производитель
        [TrackEvent("Product.Core.Products.BrandCreated")]
        Core_Products_BrandCreated,

        // Добавлен цвет
        [TrackEvent("Product.Core.Products.ColorCreated")]
        Core_Products_ColorCreated,

        // Добавлен размер
        [TrackEvent("Product.Core.Products.SizeCreated")]
        Core_Products_SizeCreated,

        // Добавлен тег
        [TrackEvent("Product.Core.Products.TagCreated")]
        Core_Products_TagCreated,

        // Добавлен новый отзыв
        [TrackEvent("Product.Core.Products.AddReview")]
        Core_Products_AddReview,

        // Изменены цены через ценорегулирование
        [TrackEvent("Product.Core.Products.ChangePricesByPriceRegulation")]
        Core_Products_ChangePricesByPriceRegulation,

        // Импортированы товары
        [TrackEvent("Product.Core.Products.ImportProducts.Csv")]
        Core_Products_ImportProducts_Csv,

        #endregion

        #region Leads

        // Создан лид из админки
        [TrackEvent("Product.Core.Leads.LeadCreated.AdminArea")]
        Core_Leads_LeadCreated_AdminArea,

        // Создан лид из клиентки магазина (desktop)
        [TrackEvent("Product.Core.Leads.LeadCreated.Desktop")]
        Core_Leads_LeadCreated_Desktop,

        // Создан лид из клиентки магазина (mobile version)
        [TrackEvent("Product.Core.Leads.LeadCreated.Mobile")]
        Core_Leads_LeadCreated_Mobile,

        // Создан лид из воронки
        [TrackEvent("Product.Core.Leads.LeadCreated.SalesFunnel")]
        Core_Leads_LeadCreated_SalesFunnel,

        // Создан лид из соцсети (какой)
        [TrackEvent("Product.Core.Leads.LeadCreated.SocialNetwork")]
        Core_Leads_LeadCreated_SocialNetwork,

        // Создан лид из звонка
        [TrackEvent("Product.Core.Leads.LeadCreated.Call")]
        Core_Leads_LeadCreated_Call,

        // Создан лид из API
        [TrackEvent("Product.Core.Leads.LeadCreated.Api")]
        Core_Leads_LeadCreated_Api,

        // Изменен этап лида
        [TrackEvent("Product.Core.Leads.DealStatusChanged")]
        Core_Leads_DealStatusChanged,

        // Изменен лид
        [TrackEvent("Product.Core.Leads.EditLead")]
        Core_Leads_EditLead,

        // Добавлен комментарий в ленту события из лида
        [TrackEvent("Product.Core.Leads.AddComment.Events")]
        Core_Leads_AddComment_Events,

        // Массовая email рассылка по лидам
        [TrackEvent("Product.Core.Leads.BulkEmailSending")]
        Core_Leads_BulkEmailSending,

        // Массовая смс рассылка по лидам
        [TrackEvent("Product.Core.Leads.BulkSmsSending")]
        Core_Leads_BulkSmsSending,

        // Добавление списка лидов
        [TrackEvent("Product.Core.Leads.AddLeadsList")]
        Core_Leads_AddLeadsList,

        // Редактирование этапа сделки (добавление или удаление или редактирование)
        [TrackEvent("Product.Core.Leads.DealStatusCUD")]
        Core_Leads_DealStatusCUD,

        #endregion

        #region Tasks

        // Создана задача
        [TrackEvent("Product.Core.Tasks.TaskCreated")]
        Core_Tasks_TaskCreated,

        // Редактирование задачи
        [TrackEvent("Product.Core.Tasks.EditTask")]
        Core_Tasks_EditTask,

        // Изменен статус задачи
        [TrackEvent("Product.Core.Tasks.TaskStatusChanged")]
        Core_Tasks_TaskStatusChanged,

        // Добавление комментария к задаче
        [TrackEvent("Product.Core.Tasks.CommentAdded")]
        Core_Tasks_CommentAdded,

        // Добавление проекта
        [TrackEvent("Product.Core.Tasks.TaskProjectCreated")]
        Core_Tasks_TaskProjectCreated,

        #endregion

        #region Booking

        // Просмотр страницы списка броней
        [TrackEvent("Product.Core.Booking.ViewBookingList")]
        Core_Booking_ViewBookingList,

        // Просмотр брони
        [TrackEvent("Product.Core.Booking.ViewBooking")]
        Core_Booking_ViewBooking,

        // Редактирование брони
        [TrackEvent("Product.Core.Booking.EditBooking")]
        Core_Booking_EditBooking,

        // Изменение статуса брони
        [TrackEvent("Product.Core.Booking.BookingStatusChanged")]
        Core_Booking_BookingStatusChanged,

        // Добавление филиала
        [TrackEvent("Product.Core.Booking.AffiliateCreated")]
        Core_Booking_AffiliateCreated,

        // Добавление категории услуг
        [TrackEvent("Product.Core.Booking.CategoryCreated")]
        Core_Booking_CategoryCreated,

        // Добавление услуги
        [TrackEvent("Product.Core.Booking.ServiceCreated")]
        Core_Booking_ServiceCreated,

        // Добавление ресурса
        [TrackEvent("Product.Core.Booking.ResourceCreated")]
        Core_Booking_ResourceCreated,

        // Редактирование ресурса
        [TrackEvent("Product.Core.Booking.EditResource")]
        Core_Booking_EditResource,

        // Просмотр отчетов
        [TrackEvent("Product.Core.Booking.ViewReports")]
        Core_Booking_ViewReports,

        // Изменение настроек бронирования
        [TrackEvent("Product.Core.Booking.EditSettings")]
        Core_Booking_EditSettings,

        #endregion

        #region Discounts

        // Добавление скидки
        [TrackEvent("Product.Core.Discounts.DiscountPriceRangeCreated")]
        Core_Discounts_DiscountPriceRangeCreated,

        // Добавление купона
        [TrackEvent("Product.Core.Discounts.CouponCreated")]
        Core_Discounts_CouponCreated,

        // Добавление подарочного сертификата
        [TrackEvent("Product.Core.Discounts.CertificateCreated")]
        Core_Discounts_CertificateCreated,

        #endregion

        #region Reports

        // Просмотр страницы отчетов
        [TrackEvent("Product.Core.Reports.ViewReports")]
        Core_Reports_ViewReports,

        #endregion

        #region Common

        // Заход в админку первый раз в день
        [TrackEvent("Product.Core.Common.FirstVisitAdminAreaOfDay")]
        Core_Common_FirstVisitAdminAreaOfDay,

        // Кнопка быстрого добавления. Все события
        [TrackEvent("Product.Core.Common.LeftMenu.QuickAdd")]
        Core_Common_LeftMenu_QuickAdd,

        // Изменены настройки индикаторов на dashboard
        [TrackEvent("Product.Core.Common.StatisticsDashboard.IndicatorsChanged")]
        Core_Common_StatisticsDashboard_IndicatorsChanged,

        // Кликнули на статье Рекомендуем почитать
        [TrackEvent("Product.Core.Common.RecommendationsDashboard.ClickArticle")]
        Core_Common_RecommendationsDashboard_ClickArticle,

        // Кликнули на партнерский баннер
        [TrackEvent("Product.Core.Common.PartnersDashboard.ClickBanner")]
        Core_Common_PartnersDashboard_ClickBanner,

        // Кликнули на статусе заказа на главной
        [TrackEvent("Product.Core.Common.OrdersDashboard.ClickOrderStatus")]
        Core_Common_OrdersDashboard_ClickOrderStatus,

        // Кликнули на заказе в гриде заказов с dashboard
        [TrackEvent("Product.Core.Common.LastOrdersDashboard.ClickOrder")]
        Core_Common_LastOrdersDashboard_ClickOrder,

        // Кликнули по кнопке Витрина магазина
        [TrackEvent("Product.Core.Common.ClickStorefrontLink")]
        Core_Common_ClickStorefrontLink,

        // Изменили название проекта в шапке админки
        [TrackEvent("Product.Core.Common.Head.ShopNameChanged")]
        Core_Common_Head_ShopNameChanged,

        // Загрузили аватар через верний угол
        [TrackEvent("Product.Core.Common.Head.AddAvatar")]
        Core_Common_Head_AddAvatar,

        // Воспользовались поиском по магазину
        [TrackEvent("Product.Core.Common.Head.UsedSearch")]
        Core_Common_Head_UsedSearch,

        // Переход в нотифкации
        [TrackEvent("Product.Core.Common.ViewNotificationsPage")]
        Core_Common_ViewNotificationsPage,

        // Клик по Отметить все удовемления как прочитанные
        [TrackEvent("Product.Core.Common.SetAllNotificationsSeen")]
        Core_Common_SetAllNotificationsSeen,

        // Авторизация в админке (любой сотрудник)
        [TrackEvent("Product.Core.Common.Login.AdminArea")]
        Core_Common_Login_AdminArea,

        // Восстановление пароля к админке (любой сотрудник)
        [TrackEvent("Product.Core.Common.ForgotPassword.AdminArea")]
        Core_Common_ForgotPassword_AdminArea,

        #endregion

        #region Settings

        // Загружен логотип из настроек
        [TrackEvent("Product.Core.Settings.AddLogo.AdminArea")]
        Core_Settings_AddLogo_AdminArea,

        // Создан логотип с помощью конструтора из настроек
        [TrackEvent("Product.Core.Settings.GenerateLogo.AdminArea")]
        Core_Settings_GenerateLogo_AdminArea,

        // Загруженна favicon
        [TrackEvent("Product.Core.Settings.AddFavicon")]
        Core_Settings_AddFavicon,

        // Добавлен способ доставки (какой)
        [TrackEvent("Product.Core.Settings.ShippingMethodCreated")]
        Core_Settings_ShippingMethodCreated,

        // Добавлен спопоб оплаты (какой)
        [TrackEvent("Product.Core.Settings.PaymentMethodCreated")]
        Core_Settings_PaymentMethodCreated,

        // Привязанная почта на advantshop сервисе
        [TrackEvent("Product.Core.Settings.BindAdvantshopMailService", SendOnce = true)]
        Core_Settings_BindAdvantshopMailService,

        // Изменены настройки отправки почты (любые)
        [TrackEvent("Product.Core.Settings.EditMailSettings")]
        Core_Settings_EditMailSettings,

        // Добавлен сотрудник
        [TrackEvent("Product.Core.Settings.EmployeeCreated")]
        Core_Settings_EmployeeCreated,

        // Изменение параметров соц сетей
        [TrackEvent("Product.Core.Settings.EditSocialSettings")]
        Core_Settings_EditSocialSettings,

        #endregion

        #endregion

        #region Shop

        #region Design

        // Применен шаблон дизайна (какой)
        [TrackEvent("Product.Shop.Design.TemplateApplied")]
        Shop_Design_TemplateApplied,

        // Установлен шаблон из магазина шаблонов (какой)
        [TrackEvent("Product.Shop.Design.TemplateInstalled")]
        Shop_Design_TemplateInstalled,

        // Изменены настройки шаблона
        [TrackEvent("Product.Shop.Design.EditTemplateSettings")]
        Shop_Design_EditTemplateSettings,

        // Запущено пережатие фотографий
        [TrackEvent("Product.Shop.Design.ResizePictures")]
        Shop_Design_ResizePictures,

        #endregion

        #region Modules

        // Установлен модуль (какой)
        [TrackEvent("Product.Shop.Modules.ModuleInstalled")]
        Shop_Modules_ModuleInstalled,

        #endregion

        #region StaticPages

        // Добавлена страница
        [TrackEvent("Product.Shop.StaticPages.StaticPageCreated")]
        Shop_StaticPages_StaticPageCreated,

        // Изменена страница
        [TrackEvent("Product.Shop.StaticPages.EditStaticPage")]
        Shop_StaticPages_EditStaticPage,

        #endregion

        #region News

        // Добавлена новость
        [TrackEvent("Product.Shop.News.NewsCreated")]
        Shop_News_NewsCreated,

        #endregion

        #region Carousel

        // Загружен слайд
        [TrackEvent("Product.Shop.Carousel.AddSlide")]
        Shop_Carousel_AddSlide,

        // Удален слайд
        [TrackEvent("Product.Shop.Carousel.DeleteSlide")]
        Shop_Carousel_DeleteSlide,

        #endregion

        #region Menu

        // Добавлен пункт меню
        [TrackEvent("Product.Shop.Menu.MenuItemCreated")]
        Shop_Menu_MenuItemCreated,

        #endregion

        #region Files

        // Добавлен файл
        [TrackEvent("Product.Shop.Files.AddFile")]
        Shop_Files_AddFile,

        #endregion

        #region Voting

        // Создано голосование
        [TrackEvent("Product.Shop.Voting.VotingCreated")]
        Shop_Voting_VotingCreated,

        #endregion

        #region Funnels

        // Создана новая воронка продаж (по каждому типу)
        [TrackEvent("Product.Shop.Funnels.FunnelCreated")]
        Shop_Funnels_FunnelCreated,

        // Добавлена страница в воронку
        [TrackEvent("Product.Shop.Funnels.PageCreated")]
        Shop_Funnels_PageCreated,

        // Открыта страница в редакторе
        [TrackEvent("Product.Shop.Funnels.ViewPageEditor")]
        Shop_Funnels_ViewPageEditor,

        // Добавлен новый блок на страницу
        [TrackEvent("Product.Shop.Funnels.AddBlock")]
        Shop_Funnels_AddBlock,

        // Изменены настройки воронки
        [TrackEvent("Product.Shop.Funnels.EditFunnelSettings")]
        Shop_Funnels_EditFunnelSettings,

        // Привязан домен к воронке
        [TrackEvent("Product.Shop.Funnels.BindDomain")]
        Shop_Funnels_BindDomain,

        // Включены воронки в приложениях
        [TrackEvent("Product.Shop.Funnels.FunnelsEnabled")]
        Shop_Funnels_FunnelsEnabled,

        // Клик по приложению "Воронки"
        [TrackEvent("Product.Shop.Funnels.LeftMenuClickFunnels")]
        Shop_Funnels_LeftMenuClickFunnels,

        // Клик по кнопке и блоку "Создать воронку"
        [TrackEvent("Product.Shop.Funnels.ClickCreateFunnel")]
        Shop_Funnels_ClickCreateFunnel,

        // Выбран любой из типов воронки на странице создания воронки (по каждому типу)
        [TrackEvent("Product.Shop.Funnels.ClickFunnelType")]
        Shop_Funnels_ClickFunnelType,

        // Просмотр видео внутри типа воронки
        [TrackEvent("Product.Shop.Funnels.WatchVideo")]
        Shop_Funnels_WatchVideo,

        // Просмотр схемы воронки внутри типа
        [TrackEvent("Product.Shop.Funnels.WatchScheme")]
        Shop_Funnels_WatchScheme,

        // Выбран шаблон внутри типа воронки
        [TrackEvent("Product.Shop.Funnels.ClickTemplate")]
        Shop_Funnels_ClickTemplate,

        // Редактирование блока
        [TrackEvent("Product.Shop.Funnels.EditBlock")]
        Shop_Funnels_EditBlock,

        // Редактирование настроек страницы (на странице воронки)
        [TrackEvent("Product.Shop.Funnels.EditPageSettings")]
        Shop_Funnels_EditPageSettings,

        // Клик по настройкам воронки (в админке)
        [TrackEvent("Product.Shop.Funnels.ClickFunnelSettings")]
        Shop_Funnels_ClickFunnelSettings,

        // Клик по кнопке "купить домен" в настройках воронки
        [TrackEvent("Product.Shop.Funnels.ClickBuyDomain")]
        Shop_Funnels_ClickBuyDomain,

        // Копирование воронки
        [TrackEvent("Product.Shop.Funnels.CopyFunnel")]
        Shop_Funnels_CopyFunnel,

        // Копирование страницы
        [TrackEvent("Product.Shop.Funnels.CopyPage")]
        Shop_Funnels_CopyPage,

        #endregion

        #region Vk

        // Подключена группа
        [TrackEvent("Product.Shop.Vk.GroupConnected")]
        Shop_Vk_GroupConnected,

        // Осуществленна выгрузка товаров первый раз
        [TrackEvent("Product.Shop.Vk.FirstExportProducts", SendOnce = true)]
        Shop_Vk_FirstExportProducts,

        // Осуществленна выгрузка товаров последующие разы
        [TrackEvent("Product.Shop.Vk.ExportProducts")]
        Shop_Vk_ExportProducts,

        #endregion

        #region Instagram

        // Подключена группа
        [TrackEvent("Product.Shop.Instagram.AccountConnected")]
        Shop_Instagram_AccountConnected,

        #endregion

        #region Facebook

        // Подключена группа
        [TrackEvent("Product.Shop.Facebook.GroupConnected")]
        Shop_Facebook_GroupConnected,

        #endregion

        #region Telegram

        // Подключена группа
        [TrackEvent("Product.Shop.Telegram.BotConnected")]
        Shop_Telegram_BotConnected,

        #endregion

        #region OK

        [TrackEvent("Product.Shop.Ok.BotConnected")]
        Shop_Ok_BotConnected,
        [TrackEvent("Product.Shop.Ok.ExportProducts")]
        Shop_Ok_ExportProducts,

        #endregion

        #region ExportFeeds

        // Добавлен новая выгрузка/новый файл (тип)
        [TrackEvent("Product.Shop.ExportFeeds.ExportFeedCreated")]
        Shop_ExportFeeds_ExportFeedCreated,

        // Изменены параметры выгрузки (тип)
        [TrackEvent("Product.Shop.ExportFeeds.EditExportFeed")]
        Shop_ExportFeeds_EditExportFeed,

        // Включена настройка выгружать по расписанию (тип)
        [TrackEvent("Product.Shop.ExportFeeds.EnableJob")]
        Shop_ExportFeeds_EnableJob,

        // Осуществлена выгрузка руками (тип)
        [TrackEvent("Product.Shop.ExportFeeds.ExportManual")]
        Shop_ExportFeeds_ExportManual,

        // Осуществлена выгрузка автоматически (тип)
        [TrackEvent("Product.Shop.ExportFeeds.ExportAuto")]
        Shop_ExportFeeds_ExportAuto,

        // YandexMarket: Изменений параметров промо акций (все виды)
        [TrackEvent("Product.Shop.ExportFeeds.YandexMarket.EditPromo")]
        Shop_ExportFeeds_YandexMarket_EditPromo,

        #endregion

        #endregion

        #region Triggers

        // Создание триггера (по типам)
        [TrackEvent("Product.Triggers.TriggerCreated")]
        Triggers_TriggerCreated,

        // Редактирование триггера
        [TrackEvent("Product.Triggers.EditTrigger")]
        Triggers_EditTrigger,

        // Просмотр аналитики триггерной email рассылки
        [TrackEvent("Product.Triggers.ViewTriggerEmailingAnalitycs")]
        Triggers_ViewTriggerEmailingAnalitycs,

        // Просмотр аналитики ручной email рассылки
        [TrackEvent("Product.Triggers.ViewManualEmailingAnalitycs")]
        Triggers_ViewManualEmailingAnalitycs,

        #endregion

       #region SalesChannels

        // Добавлен канал продаж (по типам)
        [TrackEvent("Product.SalesChannels.SalesChannelAdded")]
        SalesChannels_SalesChannelAdded,

        // Проявлен интерес к функционалу (по каналам)
        [TrackEvent("interest", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_Interest,
        
        // Попытка подключения Покупок на Маркете
        [TrackEvent("connection_attempt_ym", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_YM,

        // Попытка подключения vk
        [TrackEvent("connection_attempt_vk", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_Vk,

        #region Ozon

        // Попытка подключения Ozon
        [TrackEvent("connection_attempt_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_Ozon,

        // Синхронизация товаров
        [TrackEvent("products_sync_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsSync_Ozon,

        // Выгрузка товаров
        [TrackEvent("products_import_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsImport_Ozon,

        // Обновление цен
        [TrackEvent("prices_update_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_PricesUpdate_Ozon,

        // Обновление остатков
        [TrackEvent("stocks_update_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_StocksUpdate_Ozon,

        // Загрузка заказов/статусов
        [TrackEvent("orders_get_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersGet_Ozon,

        // Вырузка заказов/статусов
        [TrackEvent("orders_put_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersPut_Ozon,

        // Загрузка заказов/статусов
        [TrackEvent("firstorder_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_FirstOrder_Ozon,

        #endregion

        #region Wildberries

        // Попытка подключения wildberries
        [TrackEvent("connection_attempt_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_Wb,

        // Синхронизация товаров
        [TrackEvent("products_sync_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsSync_Wb,

        // Выгрузка товаров
        [TrackEvent("products_import_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsImport_Wb,

        // Обновление цен
        [TrackEvent("prices_update_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_PricesUpdate_Wb,

        // Обновление остатков
        [TrackEvent("stocks_update_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_StocksUpdate_Wb,

        // Загрузка заказов/статусов
        [TrackEvent("orders_get_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersGet_Wb,

        // Вырузка заказов/статусов
        [TrackEvent("orders_put_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersPut_Wb,

        // Загрузка заказов/статусов
        [TrackEvent("firstorder_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_FirstOrder_Wb,

        #endregion

        #region AliExpress

        // Попытка подключения wildberries
        [TrackEvent("connection_attempt_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_Ali,

        // Синхронизация товаров
        [TrackEvent("products_sync_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsSync_Ali,

        // Выгрузка товаров
        [TrackEvent("products_import_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsImport_Ali,

        // Обновление цен
        [TrackEvent("prices_update_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_PricesUpdate_Ali,

        // Обновление остатков
        [TrackEvent("stocks_update_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_StocksUpdate_Ali,

        // Загрузка заказов/статусов
        [TrackEvent("orders_get_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersGet_Ali,

        // Вырузка заказов/статусов
        [TrackEvent("orders_put_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersPut_Ali,

        // Загрузка заказов/статусов
        [TrackEvent("firstorder_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_FirstOrder_Ali,

        #endregion

        #endregion

        #region Заглушка клиентки в триале

        // страница открылась
        [TrackEvent(EShopMode.TrialMode, EventKey = "ClientBlocker_visited")]
        ClientBlocker_Visited,

        // авторизовался успешно как гость или перешел по ссылке войти как администратор
        [TrackEvent(EShopMode.TrialMode, EventKey = "ClientBlocker_authorized")]
        ClientBlocker_Authorized,

        #endregion
    }
}