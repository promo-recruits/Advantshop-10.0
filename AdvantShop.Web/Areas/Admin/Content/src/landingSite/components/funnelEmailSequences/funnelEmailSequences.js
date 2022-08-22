; (function (ng) {
    'use strict';

    var FunnelEmailSequencesCtrl = function ($http, $uibModal, $window, SweetAlert) {
        var ctrl = this;

        ctrl.init = function (model) {
            ctrl.fetch(model.dateFrom, model.dateTo);
            ctrl.triggersActive = model.triggersActive;
            ctrl.canAddSalesChannel = model.canAddSalesChannel;
            ctrl.triggerObjectTypes = model.triggerObjectTypes;
        };

        ctrl.fetch = function (dateFrom, dateTo) {
            if (dateFrom) { ctrl.emailsDateFrom = dateFrom; }
            if (dateTo) { ctrl.emailsDateTo = dateTo; }
            if (!ctrl.emailsDateFrom || !ctrl.emailsDateTo) { return; }

            $http.get('funnels/getSiteTriggerEmails', { params: { orderSourceId: ctrl.orderSourceId, dateFrom: ctrl.emailsDateFrom, dateTo: ctrl.emailsDateTo } }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.triggerEmails = data.obj;
                }
            });
        };

        ctrl.addTrigger = function () {
            if (ctrl.triggersActive) {
                ctrl.addTriggerModal();
                return;
            }
            if (!ctrl.canAddSalesChannel) {
                SweetAlert.alert(null, { title: '', text: 'Канал продаж "Триггерный маркетинг" не подключен' });
                return;
            }

            SweetAlert.confirm(null, {
                title: '',
                text: 'Канал продаж "Триггерный маркетинг" не подключен',
                showCancelButton: true,
                confirmButtonText: 'Подключить канал',
                type: 'warning',
                confirmButtonText: 'Подключить'
            }).then(function (result) {
                if (result === true) {
                    ctrl.addTriggersChannelModal();
                }
            });
        };

        ctrl.addTriggerModal = function () {
            $uibModal.open({
                animation: false,
                bindToController: true,
                controller: 'ModalAddTriggerCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/triggers/modal/addTrigger/addTrigger.html',
                resolve: { params: { 'orderSourceId': ctrl.orderSourceId, 'objectTypes': ctrl.triggerObjectTypes } }
            }).result.then(function (result) {
                return result;
            }, function (result) {
                return result;
            });
        };

        ctrl.addTriggersChannelModal = function () {
            $uibModal.open({
                animation: false,
                bindToController: true,
                controller: 'ModalSalesChannelsCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/_shared/modal/salesChannels/salesChannels.html',
                resolve: { data: { 'selectedChannelTypeStr': 'triggers', 'closeOnComplete': true } },
                size: 'sidebar-unit-modal-trigger',
                backdrop: 'static',
                windowClass: 'simple-modal modal-sales-channels',
            }).result.then(function (result) {
                if (result === true) {
                    ctrl.triggersActive = true;
                    ctrl.addTriggerModal();
                }
                return result;
            }, function (result) {
                return result;
            });
        };

        ctrl.setMailSettings = function () {
            SweetAlert.confirm('', {
                title: '',
                html: '<div>Для сбора и отображения статистики открываемости писем необходимо подключить почтовый сервис ADVANTSHOP.</div>' +
                    '<a href="https://www.advantshop.net/help/pages/email-google-yandex#200" target="_blank">Подробнее...</a>',
                type: 'info',
                confirmButtonText: 'Подключить'
            }).then(function (result) {
                if (result === true) {
                    $window.location.assign('settingsmail#?notifyTab=emailsettings');
                }
            });
        };
    };

    FunnelEmailSequencesCtrl.$inject = ['$http', '$uibModal', '$window', 'SweetAlert'];

    ng.module('landingSite')
        .controller('FunnelEmailSequencesCtrl', FunnelEmailSequencesCtrl)
        .component('funnelEmailSequences', {
            templateUrl: 'funnels/_emailSequences',
            controller: 'FunnelEmailSequencesCtrl',
            transclude: true,
            bindings: {
                orderSourceId: '<'
            }
        });

})(window.angular);