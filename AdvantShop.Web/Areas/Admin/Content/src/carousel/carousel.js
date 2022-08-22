; (function (ng) {
    'use strict';

    var CarouselCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $uibModal, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'ImageSrc',
                    displayName: $translate.instant('Admin.Js.Carousel.Image'),
                    enableSorting: false,
                    width: 80,
                    cellTemplate: '<div class="ui-grid-cell-contents"><a class="ui-grid-custom-flex-center ui-grid-custom-link-for-img"><img class="ui-grid-custom-col-img" ng-src="{{row.entity.ImageSrc}}"></a></div>',
                    enableCellEdit: false,
                },
                {
                    name: 'CaruselUrl',
                    displayName: $translate.instant('Admin.Js.Carousel.SynonymForURL'),
                    enableCellEdit: true,
                    uiGridCustomEdit: {
                        replaceNullable: false,
                    },
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Carousel.SynonymForURL'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'CaruselUrl'
                    }
                },
                {
                    name: 'Description',
                    displayName: $translate.instant('Admin.Js.Carousel.AltTagImage'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Carousel.AltTagImage'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Description'
                    }
                },

                {
                    name: 'DisplayInOneColumn',
                    displayName: $translate.instant('Admin.Js.Carousel.OneColumn'),
                    enableCellEdit: true,
                    type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Carousel.OneColumn'),
                        name: 'DisplayInOneColumn',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Carousel.Yes'), value: true }, { label: $translate.instant('Admin.Js.Carousel.No'), value: false }]
                    }
                },

                {
                    name: 'DisplayInTwoColumns',
                    displayName: $translate.instant('Admin.Js.Carousel.TwoColumns'),
                    enableCellEdit: true,
                    type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Carousel.TwoColumns'),
                        name: 'DisplayInTwoColumns',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Carousel.Yes'), value: true }, { label: $translate.instant('Admin.Js.Carousel.No'), value: false }]
                    }
                },
                {
                    name: 'DisplayInMobile',
                    displayName: $translate.instant('Admin.Js.Carousel.MobVersion'),
                    enableCellEdit: true,
                    type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Carousel.MobileVersion'),
                        name: 'DisplayInMobile',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Carousel.Yes'), value: true }, { label: $translate.instant('Admin.Js.Carousel.No'), value: false }]
                    }
                },
                {
                    name: 'Blank',
                    displayName: $translate.instant('Admin.Js.Carousel.InANewWindow'),
                    enableCellEdit: true,
                    type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Carousel.InANewWindow'),
                        name: 'Blank',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Carousel.Yes'), value: true }, { label: $translate.instant('Admin.Js.Carousel.No'), value: false }]
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Carousel.Sorting'),
                    width: 120,
                    enableCellEdit: true
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Carousel.Activ'),
                    enableCellEdit: true,
                    type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Carousel.Activ'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Carousel.Yes'), value: true }, { label: $translate.instant('Admin.Js.Carousel.No'), value: false }]
                    }
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-grid-custom-delete url="Carousel/DeleteCarousel" params="{\'Ids\': row.entity.CarouselId}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Carousel.DeleteSelected'),
                        url: 'Carousel/DeleteCarousel',
                        field: 'CarouselId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Carousel.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Carousel.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
                
        ctrl.upload = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: '/Carousel/upload',
                    data: {
                        file: $file,
                        rnd: Math.random(),
                    }
                }).then(function (response) {
                    var data = response.data;
                    if (data.Result == true) {
                        ctrl.ImageSrc = data.Picture;
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Carousel.ErrorLoadingImage'), data.error);
                    }
                })
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Carousel.ErrorLoadingImage'), $translate.instant('Admin.Js.Carousel.FileDoesNotMeet'));
            }
        };
    };

    CarouselCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$uibModal', '$translate'];


    ng.module('carouselPage', ['uiGridCustom', 'urlHelper'])
      .controller('CarouselPageCtrl', CarouselCtrl);

})(window.angular);