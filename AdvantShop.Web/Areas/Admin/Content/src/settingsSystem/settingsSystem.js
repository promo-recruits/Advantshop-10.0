; (function (ng) {
    'use strict';

    var SettingsSystemCtrl = function (Upload, $http, toaster, $q, $window, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, SweetAlert, $translate, adminColorSchemeService) {

        var ctrl = this;

        ctrl.$onInit = function () {
        }

        ctrl.checkLicense = function () {
            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.CheckAndSpecifyKey'), { title: $translate.instant('Admin.Js.SettingsSystem.LicenseValidation') }).then(function (result) {
                return result === true
                    ? $http.post('settingsSystem/checkLicense', { 'licKey': ctrl.LicKey }).then(function (response) {
                        if (response.data.result === true) {
                            ctrl.ActiveLic = true;
                            toaster.pop('success', $translate.instant('Admin.Js.SettingsSystem.LicenseStatusActive'), "");
                        } else {
                            ctrl.ActiveLic = false;
                            toaster.pop({
                                type: 'error',
                                title: $translate.instant('Admin.Js.SettingsSystem.LicenseStatusNotActive'),
                                timeout: 0
                            });
                        }
                    })
                    : $q.reject('sweetAlertCancel');
            });
        };

        ctrl.updateSiteMaps = function () {
            $http.post('settingsSystem/updateSiteMaps').then(function (response) {
                if (response.data.result === true) {

                    ctrl.SiteMapFileHtmlDate = response.data.obj.htmlLastWriteTime;
                    ctrl.SiteMapFileXmlDate = response.data.obj.xmlLastWriteTime;

                    ctrl.SiteMapFileHtmlLinkText = response.data.obj.SiteMapFileHtmlLink;
                    ctrl.SiteMapFileHtmlLink = ctrl.SiteMapFileHtmlLinkText + '?rnd=' + (Math.random() * 100000);

                    ctrl.SiteMapFileXmlLinkText = response.data.obj.SiteMapFileXmlLink;
                    ctrl.SiteMapFileXmlLink = ctrl.SiteMapFileXmlLinkText + '?rnd=' + (Math.random() * 100000);

                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.SiteMapsRefreshed'));
                }
                else {
                    toaster.pop('error', $translate.instant('Admin.Js.SettingsSystem.ErrorUpdatingSiteMaps'), "");
                }
            });
        };

        ctrl.fileStorageRecalc = function () {
            $http.post('settingsSystem/fileStorageRecalc').then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.RecalculationIsStarted'));
                ctrl.showFileStorageRecalc = false;
            });
        };

        //#region Localization

        ctrl.AllLocalization = true;

        ctrl.changeSelectLanguage = function () {
            ctrl.gridLocalization.setParams({ 'Value': ctrl.langLocalization, 'ChangeAll': ctrl.AllLocalization });
            ctrl.gridLocalization.fetchData();
        }

        ctrl.startExportlocalization = function () {
            if (ctrl.langLocalization == null) {
                toaster.pop("error", $translate.instant('Admin.Js.SettingsSystem.Error'), $translate.instant('Admin.Js.SettingsSystem.SelectLanguageForExport'));
            }
            else {
                $window.location.assign('localization/export?lang=' + ctrl.langLocalization);
            }
        }

        var columnDefsLocalization = [
                {
                name: 'ResourceKey',
                displayName: $translate.instant('Admin.Js.SettingsSystem.Key'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.Key'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ResourceKey'
                    }
                },
                {
                    name: 'ResourceValue',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.Value'),
                    enableCellEdit: true,
                    uiGridCustomEdit: {
                        replaceNullable: false
                    },
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.Value'),
                        name: 'ResourceValue',
                        type: uiGridConstants.filter.INPUT
                    }
                }
        ];

        ctrl.gridOptionsLocalization = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsLocalization,
            uiGridCustom: {
                rowUrl: ''
            }
        });

        ctrl.gridLocalizationOnInit = function (gridLocalization) {
            ctrl.gridLocalization = gridLocalization;
        };

        ctrl.successLocalization = function () {
            toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.LocalizationImportSuccess'));
        }

        //#endregion


        ctrl.changeColorScheme = function () {
            adminColorSchemeService.change(ctrl.AdminAreaColorScheme);
        };
    };

    SettingsSystemCtrl.$inject = ['Upload', '$http', 'toaster', '$q', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', 'SweetAlert', '$translate', 'adminColorSchemeService'];

    ng.module('settingsSystem', ['ngFileUpload', 'toaster', 'as.sortable', 'paymentMethodsList'])
      .controller('SettingsSystemCtrl', SettingsSystemCtrl);

})(window.angular);