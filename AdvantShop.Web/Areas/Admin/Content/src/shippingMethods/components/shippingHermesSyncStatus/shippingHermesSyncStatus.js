; (function (ng) {
    'use strict';

    var ShippingHermesSyncStatusCtrl = function ($http, toaster) {
        var ctrl = this;
        ctrl.statuses = {
            "NEW": { "Name": "Новая", "Comment": "Информация корректно загружена. " },
            "ARRIVED_AT_TERMINAL_PICKUP": { "Name": "Доставлена на терминал (для доставки)", "Comment": "Склад Hermes подтвердил получение посылки." },
            "OUT_FOR_DELIVERY": { "Name": "Отправлена в пункт выдачи", "Comment": "Посылка прошла сортировку и покинула склад перевозчика. " },
            "DELIVERED_TO_DESTINATION": { "Name": "Доставлена в пункт выдачи", "Comment": "Посылка доставлена в пункт выдачи. " },
            "ARRIVED_AT_PARCEL_SHOP": { "Name": "Принята в пункте выдачи", "Comment": "Посылка принята оператором пункта выдачи. " },
            "RECEIVED": { "Name": "Выдана", "Comment": "Посылка выдана получателю. " },
            "NON_PICKED": { "Name": "Просрочена", "Comment": "Истек срок хранения в пункте выдачи. Посылка находится в ПВЗ и ждет отправки. " },
            "REFUSED_BY_CUSTOMER": { "Name": "Отказ", "Comment": "Отказ получателя от посылки. " },
            "UNDELIVERED": { "Name": "Отправлена на терминал (возврат)", "Comment": "Посылка покинула пункт выдачи по истечению срока хранения. " },
            "ARRIVED_AT_TERMINAL_PICKUP_RETURN_WAY": { "Name": "Доставлена на терминал (возврат)", "Comment": "Возврат. Посылка на доставке в промежуточном терминале. " },
            "OUT_FOR_DELIVERY_RETURN_WAY": { "Name": "Отправлена поставщику", "Comment": "Возврат. Посылка в течение дня будет возвращена на складе Клиента. " },
            "DELIVERED_TO_DESTINATION_RETURN_WAY": { "Name": "Получена поставщиком", "Comment": "Возврат. Посылка принята на складе Клиента. " },
            "MISROUTED": { "Name": "Ошибочно доставлена в пункт выдачи", "Comment": "Посылка доставлена не по тому адресу пункта выдачи. " },
            "MISDIRECTED": { "Name": "Перенаправлена в другой пункт выдачи", "Comment": "Посылка перенаправляется в ближайший пункт выдачи. " },
            "ON_ROAD_BETWEEN_TERMINALS": { "Name": "В пути между терминалами", "Comment": "Посылка находится на доставке в пункт выдачи между региональными терминалами. Промежуточный статус доставки. " },
            "ARRIVED_AT_TERMINAL_TRANSIT": { "Name": "Доставлена на транзитный терминал", "Comment": "Посылка находится в транзитном терминале. Промежуточный статус доставки. " },
            "ARRIVED_AT_TERMINAL_DELIVERY": { "Name": "Доставлена на терминал доставки", "Comment": "Отправление находится в терминале доставки. Промежуточный статус доставки. " },
            "DELIVERY_NOT_DONE": { "Name": "Доставка не состоялась", "Comment": "Доставка в пункт выдачи не состоялась. Причину нужно смотреть индивидуально по каждой посылке. " },
            "LOST_DELIVERY": { "Name": "Потеряна при доставке", "Comment": "Потеря посылки в момент доставки. " },
            "PROBLEM_DELIVERY": { "Name": "Проблемы при доставке", "Comment": "Проблемы при доставке. Причину необходимо уточнить. " },
            "RETURNED_FROM_DELIVERY": { "Name": "Возвращена с маршрута", "Comment": "Доставка в пункт выдачи не удалась, посылка вернулась на терминал. " },
            "ON_ROAD_BETWEEN_TERMINALS_RETURN_WAY": { "Name": "В пути между терминалами (возврат)", "Comment": "Посылка возвращается на склад Клиента и находится на доставке между региональными терминалами. " },
            "ARRIVED_AT_TERMINAL_TRANSIT_RETURN_WAY": { "Name": "Доставлена на транзитный терминал (возврат)", "Comment": "Возвращаемая посылка находится в транзитном терминале. " },
            "ARRIVED_AT_TERMINAL_DELIVERY_RETURN_WAY": { "Name": "Доставлена на терминал доставки (возврат)", "Comment": "Возвращаемая посылка находится на терминале города Клиента. " },
            "DELIVERY_NOT_DONE_RETURN_WAY": { "Name": "Доставка не состоялась (возврат)", "Comment": "Доставка посылок на склад Клиента не состоялась.  " },
            "LOST_DELIVERY_RETURN_WAY": { "Name": "Потеряна при доставке (возврат)", "Comment": "Возвращаемая посылка потеряна в момент доставки.  " },
            "PROBLEM_DELIVERY_RETURN_WAY": { "Name": "Проблемы при доставке (возврат)", "Comment": "Проблемы при доставке возвращаемых посылок. " },
            "RETURNED_FROM_DELIVERY_RETURN_WAY": { "Name": "Возвращена с маршрута (возврат)", "Comment": "По не были переданы на складе Клиента и были возвращены на склад Hermes. " },
            "RETURN_CONFIRMED": { "Name": "Возврат поставщику подтвержден", "Comment": "Возвращаемые посылки переданы отправителю. " },
            "CUSTOMER_RETURN_RECEIVED": { "Name": "Принята в пункте выдачи (клиентский возврат)", "Comment": "C2B – Пункт выдачи принял посылку у получателя. " },
            "PICKED_UP_BY_COURIER_CUSTOMER_RETURN": { "Name": "Отправлена на терминал (клиентский возврат)", "Comment": "C2B – Посылка отправлена из пункта выдачи.  " },
            "ARRIVED_AT_TERMINAL_PICKUP_CUSTOMER_RETURN": { "Name": "Доставлена на терминал (клиентский возврат)", "Comment": "C2B – Посылка проходит сортировку на складе перевозчика. " },
            "ON_ROAD_BETWEEN_TERMINALS_CUSTOMER_RETURN": { "Name": "В пути между терминалами (клиентский возврат)", "Comment": "C2B – Посылка покинула склад перевозчика и находится в между терминалами перевозчика. " },
            "ARRIVED_AT_TERMINAL_TRANSIT_CUSTOMER_RETURN": { "Name": "Доставлена на транзитный терминал (клиентский возврат)", "Comment": "C2B - Промежуточный статус доставки между терминалами перевозчика. " },
            "ARRIVED_AT_TERMINAL_DELIVERY_CUSTOMER_RETURN": { "Name": "Доставлена на терминал доставки (клиентский возврат)", "Comment": "C2B – Посылка находится в терминале города Клиента " },
            "OUT_FOR_DELIVERY_CUSTOMER_RETURN": { "Name": "Отправлена поставщику (клиентский возврат)", "Comment": "C2B – Посылка будет возвращена сегодня на склад Клиента " },
            "DELIVERY_NOT_DONE_CUSTOMER_RETURN": { "Name": "Доставка не состоялась (клиентский возврат)", "Comment": "C2B – Возврат посылки на складе Клиента не состоялся. " },
            "LOST_DELIVERY_CUSTOMER_RETURN": { "Name": "Потеряна при доставке (клиентский возврат)", "Comment": "C2B – Посылка потеряна в момент доставки на склад Клиента " },
            "PROBLEM_DELIVERY_CUSTOMER_RETURN": { "Name": "Проблемы при доставке (клиентский возврат)", "Comment": "C2B – Проблемы при доставке посылки на склад Клиента " },
            "RETURNED_FROM_DELIVERY_CUSTOMER_RETURN": { "Name": "Возвращена с маршрута (клиентский возврат)", "Comment": "C2B – Посылка возвращена на склад перевозчика из-за проблем в момент доставки на склад Клиента " },
            "DELIVERED_TO_DESTINATION_CUSTOMER_RETURN": { "Name": "Получена поставщиком (клиентский возврат)", "Comment": "C2B – Посылка возвращена на склад Клиенту " },
            "CUSTOMER_RETURN_CONFIRMED": { "Name": "Возврат поставщику подтвержден (клиентский возврат)", "Comment": "C2B - Склад Клиента подтвердил прием посылки от Hermes " },
            "INVENTORIED": { "Name": "Инвентаризирована", "Comment": "В пункте выдачи проходит инвентаризация, посылка успешно прошла инвентаризацию и находится в ПВЗ. " },
            "MISSING": { "Name": "Потеряна", "Comment": "Потеря посылки в пункте выдачи после инвентаризации. Статус не окончательный и требует дальнейшего разбирательства. " },
            "LOST": { "Name": "Окончательно потеряна", "Comment": "По посылке было разбирательство, ее искали, но не нашли. Это означает, что она утеряна, по ней можно выставить претензию. " },
        };

        ctrl.$onInit = function () {

            ctrl.listStatuses = [];
            for (var value in ctrl.statuses) {
                if (ctrl.statuses.hasOwnProperty(value)) {
                    ctrl.listStatuses.push({ value: value, label: ctrl.statuses[value].Name });
                }
            }

            $http.get('orders/getorderstatuses').then(function (response) {
                ctrl.advStatuses = response.data;
            }).then(function () {
                ctrl.syncStatuses = [];

                if (ctrl.statusesReference != null && ctrl.statusesReference !== '') {
                    ctrl.syncStatuses = ctrl.statusesReference.split(';')
                        .filter(function (x) { return x; })
                        .map(function (x) {
                            var arr = x.split(',');
                            return { hermesStatus: arr[0], advStatus: arr[1] };
                        })
                        // фильтруем существующие статусы
                        .filter(function (x) {
                            return ctrl.getStatusNameByObj(x) && ctrl.getAdvStatusName(x.advStatus);
                        });

                    ctrl.syncStatuses.sort(compare);
                    ctrl.updateStatusesReference();
                }
            });
        };

        ctrl.addSyncStatus = function () {
            if (ctrl.syncStatuses.some(function (item) { return item.hermesStatus === ctrl.Status })) {
                toaster.error('Данный статус уже указан. Чтобы обновить необходимо удалить.');
                return;
            }

            ctrl.syncStatuses.push({ hermesStatus: ctrl.Status, advStatus: ctrl.advStatus });
            ctrl.syncStatuses.sort(compare);

            ctrl.updateStatusesReference();
        }

        ctrl.deleteSyncStatus = function (index) {

            ctrl.syncStatuses.splice(index, 1);
            ctrl.updateStatusesReference();
        }

        ctrl.updateStatusesReference = function () {
            ctrl.statusesReference = ctrl.syncStatuses.map(function (x) { return x.hermesStatus + "," + x.advStatus }).join(';');
            ctrl.update = true;
        }

        ctrl.getStatusNameByObj = function (obj) {
            return ctrl.getStatusName(obj.hermesStatus);
        }

        ctrl.getStatusName = function (id) {
            return ctrl.statuses[id].Name;
        }

        ctrl.getStatusComment = function (id) {
            return ctrl.statuses[id].Comment;
        }

        ctrl.getAdvStatusName = function (id) {
            var status = ctrl.advStatuses.find(function (item) { return item.value === id; });
            return status ? status.label : undefined;
        }

        function compare(a, b) {
            if (a.hermesStatus < b.hermesStatus)
                return -1;
            if (a.hermesStatus > b.hermesStatus)
                return 1;

            return 0;
        }
    }

    ShippingHermesSyncStatusCtrl.$inject = ['$http', 'toaster'];

    ng.module('shippingMethod')
        .controller('ShippingHermesSyncStatusCtrl', ShippingHermesSyncStatusCtrl)
        .component('shippingHermesSyncStatus', {
            templateUrl: '../areas/admin/content/src/shippingMethods/components/shippingHermesSyncStatus/templates/shippingHermesSyncStatus.html',
            controller: 'ShippingHermesSyncStatusCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                statusesReference: '@'
            }
        });

})(window.angular);