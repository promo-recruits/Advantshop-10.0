; (function (ng) {
    'use strict';

    var LandingSiteAdminCtrl = function (uiGridConstants, uiGridCustomConfig, $translate, landingsService, toaster, $http, SweetAlert, $window, $location, $document) {

        var TAB_SEARCH_NAME = 'landingAdminTab';
        var SETTINGSTAB_SEARCH_NAME = 'landingSettingsTab';

        var ctrl = this;

        ctrl.$onInit = function () {
            var search = $location.search();
            ctrl.tab = (search != null && search[TAB_SEARCH_NAME]) || 'pages';
            ctrl.settingsTab = (search != null && search[SETTINGSTAB_SEARCH_NAME]) || 'common';
        };

        ctrl.changeTab = function (tab) {
            if (tab != null) {
                ctrl.tab = tab;
            }
            $location.search(TAB_SEARCH_NAME, ctrl.tab);
        };

        ctrl.changeSettingsTab = function (tab) {
            ctrl.settingsTab = tab;
            $location.search(SETTINGSTAB_SEARCH_NAME, tab);
        };

        ctrl.initSite = function (id, siteUrl, orderSourceId) {
            ctrl.id = id;
            ctrl.actualSiteUrl = siteUrl;
            ctrl.orderSourceId = orderSourceId;
            ctrl.settings = { id: id, };

            ctrl.getDomain();
            ctrl.getSiteMapInfo();
            ctrl.getStats();
            ctrl.getFunnelDomains();
        };

        ctrl.updateTitle = function (id, value) {
            landingsService.updateTitle(id, value).then(function (data) {
                if (data.result == true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    ctrl.title = value;
                    ctrl.settings.SiteName = value;
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.setEnabled = function (enabled) {
            $http.post("funnels/setSiteEnabled", { id: ctrl.id, enabled: enabled }).then(function (response) {
                if (response.data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    ctrl.siteEnabled = enabled;
                }
            });
        };

        ctrl.copyLandingPage = function (pageId) {
            SweetAlert.confirm($translate.instant('Admin.Js.GridCustomComponent.AreYouSureCopy'), { title: $translate.instant('Admin.Js.GridCustomComponent.Copying') }).then(function (result) {
                if (result === true) {
                    landingsService.copyLandingPage(pageId).then(function (data) {
                        if (data.obj.Lp != null) {
                            toaster.success('', $translate.instant('Admin.Js.GridCustom.ChangesSaved'));
                            $window.location.reload();
                        } else if (data.errors != null && data.errors.length > 0) {
                            toaster.error('', error);
                        }

                    }).catch(function (err) {
                        console.error(err.message);
                    });
                }
            });
        };

        ctrl.copyLandingSite = function (siteId) {
            SweetAlert.confirm($translate.instant('Admin.Js.GridCustomComponent.AreYouSureCopy'), { title: $translate.instant('Admin.Js.GridCustomComponent.Copying') }).then(function (result) {
                if (result === true) {
                    landingsService.copyLandingSite(siteId).then(function (data) {
                        if (data.obj != null && data.obj != 0) {
                            toaster.success('', $translate.instant('Admin.Js.GridCustom.ChangesSaved'));
                            $window.location.assign('funnels/site/' + data.obj);

                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    }).catch(function (err) {
                        console.error(err.message);
                    });
                }
            });
        };

        // #region grid pages

        var columnDefs = [
                {
                    name: 'Name',
                    displayName: 'Название',
                    cellTemplate: '<div class="ui-grid-cell-contents"><div><a ng-href="{{row.entity.TechUrl}}?inplace=true" onclick="return advTrack(\'Shop_Funnels_ViewPageEditor\');">{{row.entity.Name}}</a></div></div>'
                },
                {
                    name: 'CreatedDateFormatted',
                    displayName: $translate.instant('Admin.Js.Landing.DateAndTime'),
                    width: 150,
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Landing.Activ'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled"></ui-grid-custom-switch>',
                    width: 80,
                },
                {
                    name: 'IsMain',
                    displayName: 'Главная',
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="IsMain"></ui-grid-custom-switch>',
                    width: 80,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="" class="link-invert" title="Создать копию" ng-click="grid.appScope.$ctrl.gridExtendCtrl.copyLandingPage(row.entity.Id)"><i class="fa fa-clone"></i></a>' +
                        '<ui-grid-custom-delete url="funnels/deleteLandingPage" ng-if="!row.entity.IsMain" params="{\'Id\': row.entity.Id }"></ui-grid-custom-delete></div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'funnels/deleteLandingPages',
                        field: 'Id'
                    },
                    {
                        text: $translate.instant('Admin.Js.Landing.MakeActive'),
                        url: 'funnels/activateLandingPages',
                        field: 'Id'
                    },
                    {
                        text: $translate.instant('Admin.Js.Landing.MakeInactive'),
                        url: 'funnels/disableLandingPages',
                        field: 'Id'
                    }
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.updateGrid = function () {
            ctrl.grid.fetchData();
        };

        // #endregion

        // #region Settings

        ctrl.getSettings = function () {
            $http.get('funnels/getSiteSettings', { params: { id: ctrl.id } }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.settings = data.obj;
                    ctrl.title = ctrl.settings.SiteName;
                    ctrl.actualSiteUrl = ctrl.settings.SiteUrl;
                    ctrl.settingsForm.$setPristine();
                }
            });
        };

        ctrl.saveSettings = function () {
            $http.post('funnels/saveSiteSettings', ctrl.settings).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    // перезагрузка настроек, значения при сохранении могут измениться (напр., url)
                    ctrl.getSettings();
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        // #region Domain

        ctrl.getDomain = function () {
            $http.get('funnels/getSiteDomain', { params: { id: ctrl.id } }).then(function (response) {
                ctrl.domain = response.data.domain;
                ctrl.techdomain = response.data.techdomain;
                ctrl.getSiteDomains();
            });
        }

        ctrl.getSiteDomains = function () {
            $http.get('funnels/getSiteDomains', { params: { siteId: ctrl.id } }).then(function (response) {
                ctrl.domains = response.data;
            });
        }

        ctrl.getFunnelDomains = function () {
            $http.get('funnels/getFunnelDomains', { params: { siteId: ctrl.id } }).then(function (response) {
                ctrl.funnelDomains = response.data;
                if (ctrl.funnelDomains != null && ctrl.funnelDomains.length > 0) {
                    ctrl.reuseDomain = ctrl.funnelDomains[0];
                }
            });
        }

        ctrl.addDomain = function (isAdditional) {
            $http.post('funnels/saveSiteDomain', { id: ctrl.id, domain: ctrl.newdomain, isAdditional: isAdditional }).then(function (response) {
                var data = response.data;
                if (data.result) {
                    toaster.success('', 'Домен добавлен. Чтобы он начал откликаться, необходимо привязать его на стороне хостинга.');
                    ctrl.newdomain = '';
                    ctrl.getDomain();
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        }

        ctrl.removeDomain = function (domain) {
            SweetAlert.confirm('Вы уверены что хотите удалить домен?', { title: '' }).then(function (result) {
                if (result === true) {
                    $http.post('funnels/removeSiteDomain', { id: ctrl.id, domain: domain }).then(function (response) {
                        var data = response.data;
                        if (data.result) {
                            toaster.success('', 'Домен успешно удален');
                            ctrl.getDomain();
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        }

        ctrl.addReuseDomain = function (reuseDomain) {
            $http.post('funnels/saveReuseSiteDomain', { id: ctrl.id, reuseDomain: reuseDomain }).then(function (response) {
                var data = response.data;
                if (data.result) {
                    toaster.success('', 'Домен добавлен. Чтобы он начал откликаться, необходимо привязать его на стороне хостинга.');
                    ctrl.newdomain = '';
                    ctrl.getDomain();

                    ctrl.addDomainMode = 'new';
                    ctrl.getFunnelDomains();
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        }

        // #endregion

        // #region Sitemap

        ctrl.generateSitemapXml = function () {
            $http.post('funnels/generateSitemapXml', { siteId: ctrl.id, useHttps: ctrl.settings.UseHttpsForSitemap }).then(function (response) {
                if (response.data.result) {
                    toaster.success('', 'Карта сайта сгенерирована');
                }
                ctrl.getSiteMapInfo();
            });
        }

        ctrl.generateSitemapHtml = function () {
            $http.post('funnels/generateSitemapHtml', { siteId: ctrl.id, useHttps: ctrl.settings.UseHttpsForSitemap }).then(function (response) {
                if (response.data.result) {
                    toaster.success('', 'Карта сайта сгенерирована');
                }
                ctrl.getSiteMapInfo();
            });
        }

        ctrl.getSiteMapInfo = function () {
            $http.get('funnels/getSiteMapInfo', { params: { siteId: ctrl.id } }).then(function (response) {
                ctrl.sitemapData = response.data;
            });
        }

        // #endregion

        // #region additional sales

        ctrl.gridFunnelProductsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'ArtNo',
                    displayName: 'Артикул',
                    cellTemplate: '<div class="ui-grid-cell-contents"><a class="link-invert" ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                    width: 100
                },
                {
                    name: 'Name',
                    displayName: 'Название',
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.unsetProductFunnel(row.entity.ProductId)" class="ui-grid-custom-service-icon fa fa-times link-invert"></a> ' +
                        '</div></div>'
                }
            ],
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: 'Отвязать воронку от выделенных',
                        url: 'funnels/unsetProductsFunnel',
                        field: 'ProductId',
                        before: function () {
                            return SweetAlert.confirm('Вы уверены, что хотите отвязать воронку от товаров?', { title: '' }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                ]
            }
        });

        ctrl.onGridFunnelProductsInit = function (grid) {
            ctrl.gridFunnelProducts = grid;
        };

        ctrl.unsetProductFunnel = function (productId) {
            SweetAlert.confirm('Вы уверены, что хотите отвязать воронку от товара?', { title: '' }).then(function (result) {
                if (result === true) {
                    $http.post('funnels/unsetProductsFunnel', { siteId: ctrl.id, ids: [productId] }).then(function (response) {
                        ctrl.gridFunnelProducts.fetchData();
                    });
                }
            });
        };

        ctrl.addSiteProducts = function (result) {

            if (result == null || result.ids == null || result.ids.length == 0)
                return;

            $http.post('funnels/addSiteProducts', { siteId: ctrl.id, ids: result.ids }).then(function (response) {
                ctrl.gridFunnelProducts.fetchData();
            });
        };

        // #endregion

        // #region Auth

        ctrl.getDealStatuses = function (salesFunnelId) {
            if (salesFunnelId == null || salesFunnelId == '')
                return;

            $http.get('salesFunnels/getDealStatuses', { params: { salesFunnelId: salesFunnelId } }).then(function (response) {
                ctrl.DealStatuses = response.data;
            });
        };

        ctrl.getAuthOrderProducts = function () {
            $http.get('funnels/getAuthOrderProducts', { params: { landingSiteId: ctrl.id } }).then(function (response) {
                ctrl.AuthOrderProducts = response.data.obj;
            });
        };

        ctrl.selectAuthOrderProducts = function (result) {
            if (result == null || result.ids == null || result.ids.length === 0)
                return;
            $http.post('funnels/addAuthOrderProducts', { landingSiteId: ctrl.id, ids: result.ids }).then(function (response) {
                ctrl.getAuthOrderProducts();
                toaster.success($translate.instant('Admin.Js.Common.ChangesSaved'));
            });
        };

        ctrl.deleteAuthOrderProduct = function (productId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Common.Deleting'), { title: $translate.instant('Admin.Js.Common.AreYouSureDelete') }).then(function (result) {
                if (result === true) {
                    $http.post('funnels/deleteAuthOrderProduct', { landingSiteId: ctrl.id, productId: productId }).then(function (response) {
                        ctrl.getAuthOrderProducts();
                        toaster.success($translate.instant('Admin.Js.Common.ChangesSaved'));
                    });
                }
            });
        };

        // #endregion

        // #region MobileApp

        ctrl.onChangeMobileAppActiveStateOffOn = function (checked) {
            ctrl.settings.MobileAppActive = checked;
        };

        // #endregion

        // #endregion

        // #region Funnel Stats

        var timerStats,
            documentIsVisible = $document[0].visibilityState === 'visible';

        ctrl.getStats = function () {
            if (documentIsVisible !== true) {
                return;
            }
            $http.get('funnels/getFunnelStats', { params: { orderSourceId: ctrl.orderSourceId } }).then(function (response) {
                ctrl.stats = response.data;
                if (timerStats != null) {
                    clearTimeout(timerStats);
                }
                timerStats = setTimeout(function () {
                    ctrl.getStats();
                }, 30 * 1000);
            });
        };

        $document.on("visibilitychange", function () {
            documentIsVisible = $document[0].visibilityState === 'visible';
            if (documentIsVisible === true) {
                ctrl.getStats();
            }
        });

        // #endregion
    };

    LandingSiteAdminCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', '$translate', 'landingsService', 'toaster', '$http', 'SweetAlert', '$window', '$location', '$document'];


    ng.module('landingSite', [])
        .controller('LandingSiteAdminCtrl', LandingSiteAdminCtrl);

})(window.angular);