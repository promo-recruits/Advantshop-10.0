﻿; (function(ng) {
    'use strict';

    var TriggerActionEditFieldCtrl = function ($http, $filter, $q) {
        var ctrl = this;

        ctrl.$onInit = function() {
            ctrl.mode = ctrl.action.IsNew === true ? "add" : "edit";
            ctrl.checkboxOptions = [{ label: 'Да', value: 'true' }, { label: 'Нет', value: 'false' }];

            if (ctrl.mode == "add") {
                ctrl.resetComparers();
            } else {
                var field = $filter('filter')(ctrl.fields, function(item) {
                    return item.type == ctrl.action.EditField.type && (ctrl.action.EditField.objId == null || item.objId == ctrl.action.EditField.objId);
                })[0];

                if (field != null) {
                    $q.when(ctrl.setParamValues(field))
                        .then(function() {
                            if (ctrl.isLeadEvent() && ctrl.action.EditField.type == 15) {
                                ctrl.changeSalesFunnel(ctrl.action.EditField.EditFieldValue);
                            }
                        });
                }
            }
        };

        ctrl.resetComparers = function() {
            ctrl.compareValues = null;
        };

        ctrl.paramValuesEmpty = function() {
            return ctrl.compareValues == null || ctrl.compareValues.length == 0;
        };

        ctrl.setParamValues = function (field) {
            ctrl.resetComparers();

            var promise = null;

            promise = ctrl.getParamValues(field).then(function(data) {
                ctrl.compareValues = data != null ? data.values : null;
                ctrl.field = field;
                if (ctrl.paramValuesEmpty() && ctrl.field.fieldType == 'checkbox' && (ctrl.action.EditField.EditFieldValue == null || ctrl.action.EditField.EditFieldValue == ''))
                    ctrl.action.EditField.EditFieldValue = 'true';
                return data;
            });

            return promise;
        };

        ctrl.getParamValues = function (field) {
            return $http.get('triggers/getActionEditFieldValues',
            {
                params: {
                    eventType: ctrl.eventType,
                    fieldType: field.type,
                    fieldObjId: field.objId
                }
            }).then(function(response) { return response.data; });
        };

        ctrl.changeSalesFunnel = function(salesFunnelId) {
            return $http.get('salesFunnels/getDealStatuses', { params: { salesFunnelId: salesFunnelId } }).then(function(response) {
                ctrl.dealStatuses = response.data;
            });
        };

        ctrl.isLeadEvent = function() {
            return ctrl.eventType == 3 || ctrl.eventType == 4;
        };
    };

    TriggerActionEditFieldCtrl.$inject = ['$http', '$filter', '$q'];

    ng.module('triggers')
        .controller('TriggerActionEditFieldCtrl', TriggerActionEditFieldCtrl);

})(window.angular);