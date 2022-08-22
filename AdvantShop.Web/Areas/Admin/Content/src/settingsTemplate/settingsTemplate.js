; (function (ng) {
    'use strict';

    var SettingsTemplateCtrl = function (SweetAlert, toaster, $translate, designService, cmStatService, $http, $location) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getData();
        };

        ctrl.getData = function () {
            return designService.getThemes().then(function(designData) {
                ctrl.designData = designData;

                ctrl.CurrentTheme = ctrl.designData.Themes.find(theme => theme.Name === ctrl.designData.CurrentTheme);
                ctrl.CurrentBackGround =
                    ctrl.designData.BackGrounds.find(
                        backGround => backGround.Name === ctrl.designData.CurrentBackGround);
                ctrl.CurrentColorScheme =
                    ctrl.designData.ColorSchemes.find(
                        colorScheme => colorScheme.Name === ctrl.designData.CurrentColorScheme);

                ctrl.form.$setPristine();
            });
        };


        ctrl.changeDesign = function (designType, name) {

            designService.saveDesign(designType, name).then(function (result) {
                if (result === true) {
                    ctrl.getData().then(function () {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.ChangesSaved'));
                    })
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileSavingDesign'));
                }
            });
        };

        ctrl.addDesign = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event, designType) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                designService.uploadDesign($file, designType)
                    .then(function (result) {

                        if (result === true) {
                            ctrl.getData().then(function () {
                                toaster.pop('success', '', $translate.instant('Admin.Js.Design.ArchiveSuccessfullyUploaded'));
                            });
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileLoading'));
                        }
                    })
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Design.ErrorWhileLoading'), $translate.instant('Admin.Js.Design.FileDoesNotMeet'));
            }
        };

        ctrl.deleteDesign = function (designType, designName) {
            SweetAlert.confirm($translate.instant('Admin.Js.Design.AreYouSureDelete'), { title: "" }).then(function (result) {
                if (result === true) {
                    designService.deleteDesign(designName, designType).then(function (result) {

                        if (result === true) {
                            ctrl.getData().then(function () {
                                toaster.pop('success', '', $translate.instant('Admin.Js.Design.SuccessfullyDeleted'));
                            });
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileDeleting'));
                        }

                    }, function () {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileDeleting'));
                    });
                }
            });
        };

        ctrl.resizePictures = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Design.DoYouWantSqueezePhotos'), { title: $translate.instant('Admin.Js.Design.SqueezePhotosOfProducts') }).then(function (result) {
                if (result === true) {
                    designService.resizePictures().finally(function() {
                        ctrl.startResizePictures = true;
                    });
                }
            });
        }

        ctrl.cmStatOnTick = function (data) {
            if (data.IsRun === false && data.ProcessedPercent === 100) {
                ctrl.startResizePictures = false;
                cmStatService.deleteObsevarable();
            }
        };

        ctrl.setAllProductsManualRatio = function () {
            if (ctrl.ManualRatio == null)
                return;
            if (ctrl.ManualRatio < 0 || ctrl.ManualRatio > 5) {
                toaster.pop('error', '', 'Значение рейтинга может быть от 0 до 5');
                return;
            }
            $http.post('product/setAllProductsManualRatio', { manualRatio: ctrl.ManualRatio })
                .then(function (response) {
                    if (response.data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.memoryForm = function (form) {
            ctrl.form = form;
        }

        ctrl.isHidden = function (setting) {
            return ctrl.HiddenSettings != null &&
                ctrl.HiddenSettings.length > 0 &&
                ctrl.HiddenSettings.indexOf(setting) > -1;
        }
    };

    SettingsTemplateCtrl.$inject = ['SweetAlert', 'toaster', '$translate', 'designService', 'cmStatService', '$http', '$location'];

    ng.module('settingsTemplate', [])
      .controller('SettingsTemplateCtrl', SettingsTemplateCtrl);

})(window.angular);