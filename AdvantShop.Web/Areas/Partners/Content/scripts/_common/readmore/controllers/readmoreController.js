; (function (ng) {
    'use strict';

    var ReadmoreCtrl = function ($element, $timeout, readmoreConfig) {
        var ctrl = this;
        var timer;

        ctrl.$onInit = function () {
            ctrl.maxHeight = ctrl.maxHeight || readmoreConfig.maxHeight;
            ctrl.moreText = ctrl.moreText || readmoreConfig.moreText;
            ctrl.lessText = ctrl.lessText || readmoreConfig.lessText;
            ctrl.speed = ctrl.speed || readmoreConfig.speed;
        }; 

        ctrl.init = function () {
            ctrl.checkSizes($element).then(function (isActive) {
                ctrl.isActive = isActive;

                ctrl.expanded = ctrl.isActive === true ? readmoreConfig.expanded : true;

                ctrl.text = ctrl.expanded === true ? ctrl.lessText : ctrl.moreText;
            });
        };

        ctrl.$postLink = function () {
            ctrl.init();

            ctrl.$onChanges = function (changesObj) {
                ctrl.init();
            };
        };

        ctrl.switch = function (expanded) {
            if (expanded === true) {
                ctrl.expanded = false;
                ctrl.text = ctrl.moreText;
            } else {
                ctrl.expanded = true;
                ctrl.text = ctrl.lessText;
            }
        };

        ctrl.checkSizes = function ($el) {
            if (timer != null) {
                $timeout.cancel(timer);
            }

            timer = $timeout(function () {
                var content = $el.find('.js-readmore-inner-content'),
                    clone = content.clone(),
                    result = false;

                clone.addClass('readmore-unvisible').css('width', content.width());

                $el.after(clone);

                result = ctrl.maxHeight < clone[0].offsetHeight;

                clone.remove();

                return result;
            });

            return timer;
        };
    };

    ReadmoreCtrl.$inject = ['$element', '$timeout', 'readmoreConfig'];

    ng.module('readmore')
      .controller('ReadmoreCtrl', ReadmoreCtrl);

})(window.angular);