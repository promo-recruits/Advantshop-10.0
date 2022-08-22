(function (angular) {
    var eventReady = 'shippingTemplateReady';
    var dataSet = [];
    var shippingService = function () {
        var service = this;


        service.whenTemplateReady = function(scope, fn){
            scope.$on(eventReady, event => {
                fn(event);
            });
        }

        service.fireTemplateReady = function(scope, value){
            scope.$emit(eventReady, value);
        }

        service.saveTemplateState = function(templateUrl){
            dataSet.push(templateUrl);
        }

        service.isTemplateReady = function (templateUrl) {
            return dataSet.includes(templateUrl);
        }
    };

    angular.module('shipping')
        .service('shippingService', shippingService);

})(window.angular);