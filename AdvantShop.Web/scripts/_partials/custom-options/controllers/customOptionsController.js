/* @ngInject */
function CustomOptionsCtrl(customOptionsService) {
        var ctrl = this,
            timeoutId;

        ctrl.$onInit = function() {
            customOptionsService.getData(ctrl.productId).then(function (customOptions) {
                ctrl.items = customOptions;

                var query = customOptionsService.convertToQuery(ctrl.items);

                customOptionsService.get(ctrl.productId, query).then(function (data) {
                    ctrl.xml = data.xml;
                    ctrl.jsonHash = data.jsonHash;

                    if (ctrl.initFn != null) {
                        ctrl.initFn({ customOptions: ctrl });
                    }
                });
            });
        }

        ctrl.eventDebounce = function (item) {
            if (timeoutId != null) {
                clearTimeout(timeoutId);
            }

            timeoutId = setTimeout(ctrl.change.bind(ctrl, item), 300);
        };

        ctrl.change = function (item) {

            var query = customOptionsService.convertToQuery(ctrl.items);

            customOptionsService.get(ctrl.productId, query).then(function (data) {
                ctrl.xml = data.xml;
                ctrl.jsonHash = data.jsonHash;
                ctrl.changeFn({ item: item });
            }); 
        };
};

export default CustomOptionsCtrl;