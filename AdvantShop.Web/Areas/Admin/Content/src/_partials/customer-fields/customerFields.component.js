; (function (ng) {
    'use strict';

    var CustomerFieldsCtrl = function ($http, $compile, $element, $scope, $timeout, $q) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.customerfieldsJs = [];
            if (ctrl.onInit) {
                ctrl.onInit({
                    reloadFn: ctrl.loadForm
                });
            }
            ctrl.loadForm();
        };

        ctrl.loadForm = function () {
            var promise = $q.defer();

            $timeout(function () {
                $http.get('customers/customerFieldsForm', {
                    params: { customerId: ctrl.customerId, rnd: Math.random() }
                }).then(function (response) {
                    var el = ng.element(response.data);
                    $element.empty();
                    $element.append(el);
                    $compile(el)($scope);

                    promise.resolve();
                })
            }, 0);

            return promise;
        };
    };

    CustomerFieldsCtrl.$inject = ['$http', '$compile', '$element', '$scope', '$timeout', '$q'];

    ng.module('customerFields', [])
        .controller('CustomerFieldsCtrl', CustomerFieldsCtrl)
        .component('customerFields', {
            //template: '<div ng-include="\'customers/customerFieldsForm?customerId=\'+ctrl.customerId" class="custom-fields-2-cols"></div>',
            //templateUrl: ['$element', '$attrs', '$parse', function (element, $attrs, $parse) {
            //    //console.log($attrs.customerId);
            //    return 'customers/customerFieldsForm?customerId=';// + ($attrs.customerId || '')
            //}],
            controller: CustomerFieldsCtrl,
            transclude: true,
            bindings: {
                customerfieldsJs: '=',
                customerId: '<?',
                onInit: '&'
            }
        });

})(window.angular);