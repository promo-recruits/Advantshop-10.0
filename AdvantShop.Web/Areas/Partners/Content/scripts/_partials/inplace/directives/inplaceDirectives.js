; (function (ng) {
    'use strict';

    var richIdIncrement = 0,
        autocompleteIdIncrement = 0,
        imageIdIncrement = 0,
        priceIdIncrement = 0;

    if (typeof (CKEDITOR) !== 'undefined' && CKEDITOR != null && CKEDITOR.env.isCompatible === true) {

        CKEDITOR.on('dialogDefinition', function (ev) {
            var dialogName = ev.data.name;
            var dialogDefinition = ev.data.definition;

            if (dialogName == 'table') {
                var info = dialogDefinition.getContents('info');

                info.get('txtWidth')['default'] = null;
                info.get('txtBorder')['default'] = null;
                info.get('txtCellPad')['default'] = null;
                info.get('txtCellSpace')['default'] = null;
            }
        });

        //#region inplaceRich
        ng.module('inplace')
          .directive('inplaceRich', ['$compile', '$document', '$locale', '$timeout', '$window', 'inplaceService', 'inplaceRichConfig', function ($compile, $document, $locale, $timeout, $window, inplaceService, inplaceRichConfig) {
              return {
                  restrict: 'A',
                  scope: {
                      inplaceRich: '&',
                      inplaceUrl: '@',
                      inplaceOnSave: '&'
                  },
                  controller: 'InplaceRichCtrl',
                  controllerAs: 'inplaceRich',
                  bindToController: true,
                  link: function (scope, element, attrs, ctrl) {

                      ctrl.getParams = function () {
                          return (new Function('return ' + element.attr('data-inplace-params'))()) || {};
                      };

                      //get time for ngBind
                      $timeout(function () {

                          var options = ng.extend(ng.copy(inplaceRichConfig), window._LandingCKeditorConfig || {}, ctrl.inplaceRich() || {}, {
                              language: $locale.id === 'ru-ru' ? 'ru' : ($locale.id === 'uk-ua' ? 'ua' : 'en'),
                          }),

                         timerPositionButtons,
                         timerPositionButtonsFunc,
                         setPosition,
                         saveFn,
                         scriptsInContent;

                          if (attrs.id == null) {
                              attrs.$set('id', 'inplaceRich_' + richIdIncrement);
                              richIdIncrement += 1;
                          }

                          if (options.editorSimple === true) {
                              options.removePlugins = 'showborders, magicline';
                              options.enterMode = CKEDITOR.ENTER_BR;
                              options.forcePasteAsPlainText = true;
                              element.addClass('inplace-rich-simple');
                          }

                          setPosition = function (buttons, rect) {
                              buttons.css({
                                  'top': ($window.pageYOffset + rect.bottom) + 'px',
                                  'right': ($document[0].body.clientWidth - rect.right) + 'px'
                              });
                          };

                          timerPositionButtonsFunc = function (buttons, element) {

                              if (timerPositionButtons != null) {
                                  clearTimeout(timerPositionButtons);
                              }

                              timerPositionButtons = setTimeout(function () {

                                  setPosition(buttons, element[0].getBoundingClientRect());

                                  timerPositionButtonsFunc(buttons, element);
                              }, 100);
                          };

                          element.find('script').removeAttr('type');

                          if (document.getElementById(attrs.id) == null) {
                              return;
                          }

                          element.attr('contenteditable', 'true');

                          ctrl.editor = CKEDITOR.inline(attrs.id, options);

                          if (ctrl.editor == null) {
                              return;
                          }

                          ctrl.editor.on('instanceReady', function (e) {
                              if (ctrl.editor.getData().trim().length === 0 && attrs.placeholder != null) {
                                  element.addClass('inplace-rich-empty');
                                  ctrl.editor.setData(attrs.placeholder);
                              }

                              if (options.editorSimple === true) {
                                  $document[0].getElementById(e.editor.id + '_top').style.display = 'none';
                              }
                          });

                          ctrl.editor.on('focus', function (e) {

                              if (ctrl.editor.getData().trim() === attrs.placeholder) {
                                  element.removeClass('inplace-rich-empty');
                                  ctrl.editor.setData('');
                              }

                              scope.$apply(function () {

                                  var buttons;

                                  ctrl.active();

                                  if (ctrl.buttonsRendered == null || ctrl.buttonsRendered === false) {

                                      buttons = ng.element('<div inplace-rich-buttons="' + attrs.id + '" is-show="inplaceRich.isShow"></div>')

                                      document.body.appendChild(buttons[0]);

                                      $compile(buttons)(scope);

                                      ctrl.buttonsRendered = true;
                                  } else {
                                      buttons = ctrl.buttons.element;
                                  }

                                  timerPositionButtonsFunc(buttons, element);
                              });
                          });

                          ctrl.editor.on('blur', function (e) {
                              clearTimeout(timerPositionButtons);

                              setTimeout(function () { //задержка чтобы узнать щелкнули ли на кнопки
                                  scope.$apply(function () {

                                      ctrl.isShow = false;

                                      if (ctrl.clickedButtons === false || ctrl.clickedButtons == null) {
                                          ctrl.save();
                                      }

                                      ctrl.clickedButtons = false;

                                      if (ctrl.editor.getData().trim().length === 0 && attrs.placeholder != null) {
                                          element.addClass('inplace-rich-empty');
                                          ctrl.editor.setData(attrs.placeholder);
                                      }
                                  });
                              }, 100);
                          });

                          ctrl.editor.on('key', function (event) {

                              var keyCode = event.data.keyCode;

                              switch (keyCode) {
                                  case 13://enter
                                      if (options.editorSimple === true) {
                                          var inputTemp = document.createElement('input'),
                                              pos = element[0].getBoundingClientRect();

                                          inputTemp.className = 'inplace-input-fake';
                                          inputTemp.style.top = pos.top + 'px';
                                          inputTemp.style.left = pos.left + 'px';
                                          document.body.appendChild(inputTemp);
                                          inputTemp.focus();
                                          setTimeout(function () { inputTemp.parentNode.removeChild(inputTemp); }, 100);

                                          //event.editor.focusManager.blur(false);

                                          event.stop();
                                          event.cancel();
                                      }
                                      break;
                                      //case 27://esc
                                      //    event.editor.focusManager.blur(false);
                                      //    event.stop();
                                      //    event.cancel();
                                      //    break;
                              }
                          });

                          inplaceService.addRich(attrs.id, ctrl);
                      });
                  }
              };
          }]);

        ng.module('inplace')
          .directive('inplaceRichButtons', function () {
              return {
                  restrict: 'A',
                  scope: {
                      inplaceRichButtons: '@',
                      isShow: '<?'
                  },
                  controller: 'InplaceRichButtonsCtrl',
                  controllerAs: 'inplaceRichButtons',
                  bindToController: true,
                  replace: true,
                  templateUrl: '../areas/partners/content/scripts/_partials/inplace/templates/richButtons.html'
              };
          });
        //#endregion

    }

})(window.angular);