; (function (ng) {
    'use strict';

    var DealStatusesCtrl = function ($http, $filter, $timeout, $q, toaster, SweetAlert, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {

            ctrl.fetch();

            if (ctrl.onInit != null) {
                ctrl.onInit({ items: ctrl });
            }

            // #region colorPicker
            ctrl.colorPickerOptions = {
                swatchBootstrap: false,
                format: 'hex',
                alpha: false,
                swatchOnly: false,
                'case': 'lower',
                allowEmpty: true,
                required: false,
                preserveInputFormat: false,
                restrictToFormat: false,
                inputClass: 'form-control'
            };

            ctrl.colorPickerEventApi = {
                onBlur: function () {
                    ctrl.colorPickerApi.getScope().AngularColorPickerController.update();
                }
            };

            // #endregion
        };

        ctrl.fetch = function () {
            var url = ctrl.salesFunnelId != null ? 'leads/getDealStatuses' : 'leads/getDefaultDealStatuses';
            $http.get(url, { params: { salesFunnelId: ctrl.salesFunnelId } }).then(function (response) {
                var data = response.data;
                ctrl.systemItems = data.systemItems || [];
                ctrl.items = data.items || [];
                ctrl.addEmpty();
            });
        };

        ctrl.colorsSet = [
            '8bc34a',
            'ffc73e',
            '1ec5b8',
            '78909c',
            'd48fc1'
        ];

        ctrl.getNextColor = function () {
            var result;
            for (var i = 0, len = ctrl.colorsSet.length; i < len; i++) {
                var items = $filter('filter')(ctrl.items, function (item) { return item.Color == ctrl.colorsSet[i]; });
                if (!items.length) {
                    result = ctrl.colorsSet[i];
                    break;
                }
            }
            return result || ctrl.colorsSet[Math.floor(Math.random() * ctrl.colorsSet.length)];
        }

        ctrl.addEmpty = function () {
            ctrl.newColor = ctrl.getNextColor();
            ctrl.items.push({ Id: null, Name: '', SortOrder: 100 });
            //$timeout(function () {
            if (ctrl.colorPickerApi)
                ctrl.colorPickerApi.getScope().AngularColorPickerController.setNgModel(ctrl.newColor);
            //})
        };

        ctrl.sortableOptions = {
            orderChanged: function(event) {
                if (ctrl.salesFunnelId == null) {
                    return;
                }

                var id = event.source.itemScope.item.Id,
                    prev = ctrl.items[event.dest.index - 1],
                    next = ctrl.items[event.dest.index + 1];
                
                $http.post('leads/changeDealStatusSorting', {
                    salesFunnelId: ctrl.salesFunnelId,
                    id: id,
                    prevId: prev != null ? prev.Id : null,
                    nextId: next != null ? next.Id : null
                }).then(function(response) {
                    if (response.data.result === true) {
                        toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
                    }
                });
            }
        };
                
        ctrl.deleteItem = function (item) {
            if (ctrl.salesFunnelId == null) {
                var index = ctrl.items.indexOf(item);
                if (index !== -1) {
                    ctrl.items.splice(index, 1);
                }
                return;
            }
            
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsCrm.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsCrm.Delete') }).then(function (result) {
                if (result === true) {
                    $http.post('leads/deleteDealStatus', { id: item.Id }).then(function (response) {
                        if (response.data.result === true) {
                            ctrl.fetch();
                            toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
                        }
                    });
                }
            });
        }

        ctrl.addItem = function () {
            if (!ctrl.newName)
                return;

            if (ctrl.salesFunnelId == null) {
                ctrl.items[ctrl.items.length - 1] = { Id: 0, Name: ctrl.newName, Color: ctrl.newColor, Status: 0 };
                ctrl.addEmpty();
                ctrl.newName = '';
                return;
            }

            $http.post('leads/addDealStatus', { name: ctrl.newName, color: ctrl.newColor, salesFunnelId: ctrl.salesFunnelId }).then(function (response) {
                if (response.data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
                    ctrl.fetch();
                    ctrl.newName = '';
                }
            });
        };

        ctrl.onEditDealStatus = function (prev, edited, isSystemStatus) {
            if (ctrl.salesFunnelId == null) {
                var index = (isSystemStatus ? ctrl.systemItems : ctrl.items).indexOf(prev);
                if (index !== -1) {
                    (isSystemStatus ? ctrl.systemItems : ctrl.items)[index] = edited;
                }
                return;
            }
            ctrl.fetch();
        }
    };

    DealStatusesCtrl.$inject = ['$http', '$filter', '$timeout', '$q', 'toaster', 'SweetAlert', '$translate'];

    ng.module('dealStatuses', ['as.sortable'])
        .controller('DealStatusesCtrl', DealStatusesCtrl)
        .component('dealStatuses', {
            templateUrl: '../areas/admin/content/src/settingsCrm/components/dealStatuses/dealStatuses.html',
            controller: 'DealStatusesCtrl',
            transclude: true,
            bindings: {
                onInit: '&',
                salesFunnelId: '=',
                items: '=',
                systemItems: '='
            }
        });

})(window.angular);