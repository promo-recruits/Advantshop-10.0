; (function (ng) {
    'use strict';
    var replacer = {
        "q": "й", "w": "ц", "e": "у", "r": "к", "t": "е", "y": "н", "u": "г",
        "i": "ш", "o": "щ", "p": "з", "[": "х", "]": "ъ", "a": "ф", "s": "ы",
        "d": "в", "f": "а", "g": "п", "h": "р", "j": "о", "k": "л", "l": "д",
        ";": "ж", "'": "э", "z": "я", "x": "ч", "c": "с", "v": "м", "b": "и",
        "n": "т", "m": "ь", ",": "б", ".": "ю", "/": "."
    };
    /*убираем BFCache в FF*/
    window.addEventListener('unload', function () { });

    window.ajaxInProcess = [];
    var dependencyService = ng.injector(['dependency']).get('dependencyService');
    ng.module('app', dependencyService.get())
        .value('duScrollBottomSpy', true)
        .config(['$provide', '$compileProvider', '$cookiesProvider', '$localeProvider', '$httpProvider', '$translateProvider', 'urlHelperConfig', 'toasterConfig', 'ngFlatpickrDefaultOptions', '$locationProvider', 'uiMask.ConfigProvider',
            function ($provide, $compileProvider, $cookiesProvider, $localeProvider, $httpProvider, $translateProvider, urlHelperConfig, toasterConfig, ngFlatpickrDefaultOptions, $locationProvider, uiMaskConfigProvider) {
                var date = new Date(),
                    currentYear = date.getFullYear();

                //#region Disable comment and css class directives
                $compileProvider.commentDirectivesEnabled(false);
                //$compileProvider.cssClassDirectivesEnabled(false); не использовать так как падает chart.js
                //#endregion

                date.setFullYear(currentYear + 1);

                //#region compile debug
                $compileProvider.debugInfoEnabled(false);
                //#endregion

                // enable if need to use unsave protocols
                //$compileProvider.aHrefSanitizationWhitelist(/^\s*(http|https|ftp|mailto|callto|tel):/);

                //#region set cookie expires
                $cookiesProvider.defaults.expires = date;
                $cookiesProvider.defaults.path = '/';

                if (window.location.hostname !== 'localhost' && window.location.hostname !== 'server' &&
                    !/^(?!0)(?!.*\.$)((1?\d?\d|25[0-5]|2[0-4]\d)(\.|$)){4}$/.test(window.location.hostname)) {
                    $cookiesProvider.defaults.domain = '.' + window.location.hostname.replace('www.', '');
                }

                //#endregion

                //закоментировал, т.к. в FF не работает валидация input[type="number"]
                //#region ie10 bug validation

                //$provide.decorator('$sniffer', ['$delegate', function ($sniffer) {
                //    var msie = parseInt((/msie (\d+)/.exec(angular.lowercase(navigator.userAgent)) || [])[1], 10);
                //    var _hasEvent = $sniffer.hasEvent;
                //    $sniffer.hasEvent = function (event) {
                //        if (event === 'input' && msie === 10) {
                //            return false;
                //        }
                //        _hasEvent.call(this, event);
                //    }
                //    return $sniffer;
                //}]);

                //#endregion

                //#region prepera ajax url in absolute path

                var basePath = document.getElementsByTagName('base')[0].getAttribute('href'),
                    regex = new RegExp('^(?:[a-z]+:)?//', 'i');


                $httpProvider.useApplyAsync(true);

                var tokens = document.getElementsByName("__RequestVerificationToken");
                if (tokens.length > 0) {
                    $httpProvider.defaults.headers.post['__RequestVerificationToken'] = tokens[0].value;
                }
                $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

                $httpProvider.interceptors.push(['$q', 'urlHelper', function ($q, urlHelper) {
                    return {
                        'request': function (config) {

                            var urlOld = config.url,
                                template;

                            config.url = urlHelper.getAbsUrl(config.url);
                            
                            if (window.v != null && urlOld.indexOf('../') == 0 && config.url.indexOf('.html') != -1) {
                                config.url += '?v=' + (config.url.indexOf('localhost') != -1 ? Math.random() : window.v);
                            }
                            
                            //for templates
                            if (urlOld != config.url && ng.isObject(config.cache) && config.cache.get(urlOld) != null) {
                                template = config.cache.get(urlOld);
                                config.cache.remove(urlOld);
                                config.cache.put(config.url, template);
                                //config.cache.put(config.url.replace('adminv2', 'adminv3'), template);
                            }

                            if (window.location.href.indexOf('adminv3') != -1 && config.url.indexOf('adminv2') != -1 && config.url.indexOf('.html') === -1 && template == null) {
                                config.url = config.url.replace('adminv2', 'adminv3');
                            }

                            config.executingAjax = {
                                url: config.url,
                                params: config.params
                            };

                            window.ajaxInProcess.push(config.executingAjax);

                            return config;
                        },
                        // optional method
                        'response': function (response) {

                            var index = window.ajaxInProcess.indexOf(response.config.executingAjax);

                            window.ajaxInProcess.splice(index, 1);

                            return response;
                        },

                        // optional method
                        'responseError': function (rejection) {

                            var index = ajaxInProcess.indexOf(rejection.config.executingAjax);

                            ajaxInProcess.splice(index, 1);

                            return $q.reject(rejection);
                        }
                    };
                }]);

                //#endregion

                //#region localization

                var localeId = $localeProvider.$get().id;
                $translateProvider
                    .translations(localeId, ng.extend(window.AdvantshopResource || {}, window.AdvantshopAdminResource || {}))
                    .preferredLanguage(localeId)
                    .useSanitizeValueStrategy('sanitizeParameters');
                //#endregion

                urlHelperConfig.isAdmin = true;

                toasterConfig['icon-classes'].call = 'toast-call';
                toasterConfig['position-class'] = 'toast-bottom-right';
                toasterConfig['limit'] = 3;

                ngFlatpickrDefaultOptions.locale = localeId.split('-')[0];
                ngFlatpickrDefaultOptions.disableMobile = true;

                uiMaskConfigProvider.clearOnBlur(false);
            }])
        .run(['toaster', 'adminWebNotificationsService', '$templateCache', function (toaster, adminWebNotificationsService, $templateCache) {
            //replace ng-bind title on ng-bind-html
            var popeverHtml = $templateCache.get('uib/template/popover/popover-html.html')
            $templateCache.put('uib/template/popover/popover-html.html', popeverHtml.replace('ng-bind="uibTitle"', 'ng-bind-html="uibTitle"'));

            window.addEventListener('load', function load() {

                window.removeEventListener('load', load);

                var toasterContainer = document.querySelector('[data-toaster-container]'),
                    toasterItems,
                    linkWithAnchors = document.querySelectorAll('a[href*="#"]:not([target])');

                if (toasterContainer != null) {
                    toasterItems = document.querySelectorAll('[data-toaster-type]');
                    if (toasterItems != null) {
                        for (var i = 0, len = toasterItems.length; i < len; i++) {
                            toaster.pop({
                                type: toasterItems[i].getAttribute('data-toaster-type'),
                                body: toasterItems[i].innerHTML,
                                bodyOutputType: 'trustedHtml'
                            });
                        }
                    }
                }

                //old style using anchor
                if (linkWithAnchors.length > 0) {
                    for (var j = 0, lenJ = linkWithAnchors.length; j < lenJ; j += 1) {
                        linkWithAnchors[j].setAttribute('target', '_self');
                    }
                }                

                adminWebNotificationsService.onPageLoad();
            });

        }])
        .filter("sanitize", ['$sce', function ($sce) {
            return function (htmlCode) {
                return $sce.trustAsHtml(htmlCode);
            };
        }])
        .filter("sanitizeUrl", ['$sce', function ($sce) {
            return function (htmlCode) {
                return $sce.trustAsResourceUrl(htmlCode);
            };
        }])
        .filter('nl2br', ['$sanitize', function ($sanitize) {
            var tag = '<br />';
            return function (msg) {
                if (!msg) return '';
                msg = (msg + '').replace(/(\r\n|\n\r|\r|\n|&#10;&#13;|&#13;&#10;|&#10;|&#13;)/g, tag + '$1');
                return $sanitize(msg);
            };
        }])
        .filter("greedysearch", ['$filter', function ($filter) {
            return function (array, expression) {

                if (array == null || array.length === 0 || expression == null || expression === '') {
                    return array;
                }

                var altTextExpression,
                    keys,
                    arrayOriginalLang,
                    arrayAltLang,
                    result,
                    expressionAsObject = false,
                    arrayUniqueKey = [];

                if (ng.isString(expression) === true) {
                    altTextExpression = engToRus(expression);
                } else {

                    keys = Object.keys(expression);

                    altTextExpression = {};

                    for (var i = 0, len = keys.length; i < len; i++) {
                        if (keys[i] !== '$$hashKey') {
                            altTextExpression[keys[i]] = engToRus(expression[keys[i]]);
                        }
                    }

                    expressionAsObject = true;
                }

                arrayOriginalLang = $filter('filter')(array, expression) || [];
                arrayAltLang = $filter('filter')(array, altTextExpression) || [];

                result = arrayOriginalLang.concat(arrayAltLang);


                var d = result.filter(function (item) {
                    var result;

                    if (arrayUniqueKey.indexOf(JSON.stringify(item)) === -1) {
                        arrayUniqueKey.push(JSON.stringify(item));
                        result = true;
                    } else {
                        result = false;
                    }

                    return result;
                });

                return d;
            };
        }])
        .filter('numbergreedy', ['$locale', function ($locale) {
            return function (value) {
                var result;

                if (value != null && ((ng.isString(value) === true && value.length > 0) || ng.isNumber(value) === true)) {
                    result = value.toString().replace(/\s*/g, '').replace(/[,.]+/g, $locale.NUMBER_FORMATS.DECIMAL_SEP);
                } else {
                    result = value;
                }

                return result;
            };
        }])
        .controller('AppCtrl', function () { });

    window.ajaxIsComplete = function () {
        return window.ajaxInProcess.length == 0;
    };

    function engToRus(text) {
        return text.replace(/[A-z/,.;\'\]\[]/g, function (x) {
            return x == x.toLowerCase() ? replacer[x] : replacer[x.toLowerCase()].toUpperCase();
        });
    }
	
	sweetAlert.setDefaults({
                    cancelButtonText: 'Отмена',
                    confirmButtonText: 'ОK',
                    allowOutsideClick: false,
                    buttonsStyling: false,
                    confirmButtonClass: 'btn btn-sm btn-success',
                    cancelButtonClass: 'btn btn-sm btn-action',
                    useRejections: true
                });
})(window.angular);