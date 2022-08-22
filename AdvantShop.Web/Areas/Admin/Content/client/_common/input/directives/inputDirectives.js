; (function (ng) {
    'use strict';

    ng.module('input')
      .directive('input', ['$parse', '$window', function ($parse, $window) {
          return {
              restrict: 'E',
              require: '?ngModel',
              link: function (scope, element, attrs, ctrl) {
                  var valueDirty = attrs.value,
                      el,
                      type,
                      value;

                  if (ctrl != null && ctrl.modelValue === undefined) {
                      el = element[0];
                      type = el.type;

                      switch (type) {
                          case 'radio':
                              if (attrs.checked) {
                                  value = attrs.ngValue ? $parse(attrs.ngValue)(scope) : valueDirty;
                              }
                              break;
                          case 'checkbox':
                              if (attrs.checked) {
                                  value = attrs.ngTrueValue ? $parse(attrs.ngTrueValue)(scope) : true;
                              } else if (attrs.ngFalseValue) {
                                  value = $parse(attrs.ngFalseValue)(scope);
                              }
                              break;
                          case 'number':
                              if (valueDirty != null && valueDirty.length > 0) {
								  var numberParsed = Number(valueDirty.replace(',', '.').replace(/\s*/,''));
                                  value = isNaN(numberParsed) === false ? numberParsed : null;
                              }
                              break;
                          default:
                              if (valueDirty != null && valueDirty.length > 0) {
                                  value = valueDirty;
                              }
                              break;
                      }

                      if (value != null) {
                          $parse(attrs.ngModel).assign(scope, value);
                      }

                      if (type === 'text') {
                          var callbackPaste = function (event) {

                              var content;

                              if ($window.clipboardData && $window.clipboardData.getData) { // IE
                                  content = $window.clipboardData.getData('Text');
                              }
                              else { // others
                                  content = event.clipboardData.getData('text/plain');
                              }

                              if (content != null) {
                                  ctrl.$setViewValue(content);
                              }

                          };

                          el.addEventListener('paste', callbackPaste);

                          scope.$on('$destroy', function () {
                              el.removeEventListener('paste', callbackPaste);
                          });
                      }
                  }
              }
          };
      }]);

    ng.module('input')
      .directive('textarea', ['$parse', function ($parse) {
          return {
              restrict: 'E',
              bindToController: true,
              require: {
                  ngModelCtrl: '?ngModel'
              },
              controller: ['$attrs', '$element', '$parse', '$scope', '$window', function ($attrs, $element, $parse, $scope, $window) {
                  var ctrl = this;

                  ctrl.$onInit = function () {

                      var val = $element.val();

                      if (ctrl.ngModelCtrl != null && val.length > 0) {
                          $parse($attrs.ngModel).assign($scope, val);
                      }

                      var callbackPaste = function (event) {

                          var content;

                          if ($window.clipboardData && $window.clipboardData.getData) { // IE
                              content = $window.clipboardData.getData('Text');
                          }
                          else { // others
                              content = event.clipboardData.getData('text/plain');
                          }

                          if (content != null && ctrl.ngModelCtrl != null) {
                              ctrl.ngModelCtrl.$setViewValue(content);
                          }

                      };

                      $element[0].addEventListener('paste', callbackPaste);

                      $scope.$on('$destroy', function () {
                          $element[0].removeEventListener('paste', callbackPaste);
                      });
                  };
              }],
              //link: function (scope, element, attrs, ctrl) {
              //    if (element[0].value && ctrl != null) {
              //        //ctrl.$setViewValue(element[0].value);
              //        $parse(attrs.ngModel).assign(scope, element[0].value);
              //    }
              //}
          };
      }]);

})(window.angular);




