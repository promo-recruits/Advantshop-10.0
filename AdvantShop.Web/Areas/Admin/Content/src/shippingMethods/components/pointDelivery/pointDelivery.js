﻿; (function (ng) {
    'use strict';

    var PointDeliveryMethodCtrl = function ($http, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {

        };

        ctrl.addPoint = function (point) {
            if (!point) {
                return;
            }
            ctrl.points.push(point);

            ctrl.updatePoints();
        };

        ctrl.editPoint = function (newPoint, oldPoint) {
            if (!newPoint || !oldPoint) {
                return;
            }
            var index = ctrl.points.indexOf(oldPoint);
            if (index !== -1) {
                ctrl.points[index] = newPoint;
                ctrl.updatePoints();
            }
        };

        ctrl.deletePoint = function (item) {

            var index = ctrl.points.indexOf(item);

            if (index !== -1) {
                ctrl.points.splice(index, 1);
                ctrl.updatePoints();
            }
        };

        ctrl.updatePoints = function () {
            ctrl.update = true;
        };

        ctrl.sortableOptions = {
            containment: "#pointDeliverySortingContainer",
            scrollableContainer: '#pointDeliverySortingContainer',
            containerPositioning: 'relative',
            accept: function (sourceItemHandleScope, destSortableScope) {
                return sourceItemHandleScope.itemScope.sortableScope.$id === destSortableScope.$id;
            },
            orderChanged: function (event) {
                ctrl.updatePoints();
            }
        };
    };

    PointDeliveryMethodCtrl.$inject = ['$http', 'toaster'];

    ng.module('shippingMethod')
        .controller('PointDeliveryMethodCtrl', PointDeliveryMethodCtrl)
        .component('pointDelivery', {
            templateUrl: '../areas/admin/content/src/shippingMethods/components/pointDelivery/templates/pointDelivery.html',
            controller: 'PointDeliveryMethodCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                points: '<?'
            }
        });

})(window.angular);