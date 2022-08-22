; (function (ng) {
    'use strict';

    ng.module('uiGridCustom')
        .directive('uiGridCustom', ['uiGridCustomService', 'urlHelper', function (uiGridCustomService, urlHelper) {
            return {
                restrict: 'E',
                templateUrl: function () {
                    return urlHelper.getAbsUrl('/areas/admin/content/src/_shared/ui-grid-custom/templates/ui-grid-custom.html', true);
                },
                controller: 'UiGridCustomCtrl',
                controllerAs: '$ctrl',
                bindToController: true,
                transclude: {
                    footer: '?uiGridCustomFooter',
                    overrideControl: '?uiGridCustomOverrideControl'
                },
                scope: {
                    gridOptions: '<',
                    gridUrl: '<?',
                    gridInplaceUrl: '<?',
                    gridParams: '<?',
                    gridFilterEnabled: '<?',
                    gridFilterHiddenTotalItemsCount: '<?',
                    gridSelectionEnabled: '<?',
                    gridPaginationEnabled: '<?',
                    gridTreeViewEnabled: '<?',
                    gridUniqueId: '@',
                    gridOnInplaceBeforeApply: '&',
                    gridOnInplaceApply: '&',
                    gridOnInit: '&',
                    gridSearchPlaceholder: '<?',
                    gridSearchVisible: '<?',
                    gridExtendCtrl: '<?',
                    gridEmptyText: '<?',
                    gridSelectionOnInit: '&',
                    gridSelectionOnChange: '&',
                    gridSelectionMassApply: '&',
                    gridOnFetch: '&',
                    gridOnDelete: '&',
                    gridOnPreinit: '&',
                    gridShowExport: '<?',
                    gridOnFilterInit: '&',
                    gridSelectionItemsSelectedFn: '&',
                    gridRowIdentificator: '<?',
                    gridPreventStateInHash: '<?',
                    gridFilterTemplateUrl: '<?'
                },
                compile: function (cElement, cAttrs, cTransclude) {
                    var uiGridElement = cElement[0].querySelector('[ui-grid]');

                    cAttrs.gridSelectionEnabled == null || cAttrs.gridSelectionEnabled === 'true' ? uiGridElement.setAttribute('ui-grid-selection', '') : uiGridElement.removeAttribute('ui-grid-selection');
                    cAttrs.gridTreeViewEnabled === 'true' ? uiGridElement.setAttribute('ui-grid-tree-view', '') : uiGridElement.removeAttribute('ui-grid-tree-view');

                    return function (scope, element, attrs, ctrl, transclude) {
                        scope.$on('modal.closing', function () {
                            ctrl.clearParams();
                            uiGridCustomService.removeFromStorage(ctrl.gridUniqueId);
                        });
                    }
                }
            }
        }])
        .component('uiGridCustomSwitch', {
            require: {
                uiGridCustom: '^uiGridCustom'
            },
            template: '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked"><switch-on-off checked="$ctrl.row.entity[$ctrl.fieldName || \'Enabled\']" on-change="$ctrl.uiGridCustom.setSwitchEnabled($ctrl.row.entity, checked, $ctrl.fieldName || \'Enabled\')" readonly="$ctrl.readonly" on-click="$ctrl.onClick()"></switch-on-off></div></div>',
            bindings: {
                row: '<',
                fieldName: '@',
                readonly: '<?',
                onClick: '&'
            }
        })
        .component('uiGridCustomDelete', {
            require: {
                uiGridCustom: '^^uiGridCustom'
            },
            transclude: true,
            template: '<a href="" ng-click="$ctrl.delete($ctrl.url, $ctrl.params, $ctrl.confirmText)" ng-class="$ctrl.classes" ng-transclude></a>',
            bindings: {
                url: '@',
                params: '<',
                confirmText: '@',
                onDelete: '&',
                classes: '@'
            },
            controller: ['$http', 'SweetAlert', 'toaster', 'lastStatisticsService', '$translate', '$scope', function ($http, SweetAlert, toaster, lastStatisticsService, $translate, $scope) {

                var ctrl = this;

                ctrl.$onInit = function () {
                    if (ctrl.classes == null) {
                        ctrl.classes = 'ui-grid-custom-service-icon fa fa-times link-invert';
                    }
                }

                ctrl.delete = function (url, params, confirmText) {
                    SweetAlert.confirm(confirmText != null ? confirmText : $translate.instant('Admin.Js.GridCustomComponent.AreYouSureDelete'), { title: $translate.instant('Admin.Js.GridCustomComponent.Deleting') })
                        .then(function (result) {
                            if (result === true) {
                                $http.post(url, params).then(function (response) {

                                    var data = response.data;

                                    if (data === true || (data.result != null && data.result === true)) {
                                        toaster.pop('success', '', $translate.instant('Admin.Js.GridCustom.ChangesSaved'));

                                        lastStatisticsService.getLastStatistics();

                                        var rowEntity = $scope.$parent.$parent.row != null ? $scope.$parent.$parent.row.entity : $scope.$parent.$parent.$parent.row.entity;

                                        ctrl.uiGridCustom.deleteItem(rowEntity)
                                            .then(function () {
                                                if (ctrl.onDelete != null) {
                                                    ctrl.onDelete();
                                                }
                                            });
                                    } else if (data.errors != null && data.errors.length > 0) {
                                        data.errors.forEach(function (error) {
                                            toaster.pop('error', '', error);
                                        });
                                    }

                                    return data;
                                }, function (response) {
                                    toaster.pop('success', '', $translate.instant('Admin.Js.GridCustomComponent.ErrorWhileDeletingWriting'));
                                });
                            }
                        });
                }
            }]
        })
        .directive('uiGridCustomOverrideControl', function () {
            return {
                require: '^uiGridCustom',
                compile: function compile(tElement, tAttrs, tTransclude) {
                    return function (scope, element, attrs, ctrl) {
                        scope.row = {
                            entity: scope.$parent.entity
                        };

                        scope.uiGridCustom = ctrl;
                    }
                }
            }
        })
})(window.angular);