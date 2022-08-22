; (function (ng, _clientDependency, _adminDependency) {
    'use strict';

    ng.module('app', _clientDependency.concat(_adminDependency))
        .config(['$localeProvider', '$translateProvider', '$locationProvider', '$httpProvider', 'ngFlatpickrDefaultOptions', '$anchorScrollProvider', 'modalDefaultOptions',
            function ($localeProvider, $translateProvider, $locationProvider, $httpProvider, ngFlatpickrDefaultOptions, $anchorScrollProvider, modalDefaultOptions) {

                var localeId = $localeProvider.$get().id;

                ngFlatpickrDefaultOptions.locale = localeId.split('-')[0];

               $anchorScrollProvider.disableAutoScrolling();

                $translateProvider
                    .translations(localeId, ng.extend(window.AdvantshopResource || {}, window.AdvantshopAdminResource || {}))
                    .preferredLanguage(localeId)
                    .useSanitizeValueStrategy('sanitizeParameters');

                $httpProvider.useApplyAsync(true);


                //#region prepera ajax url in absolute path

                $httpProvider.useApplyAsync(true);

                $httpProvider.interceptors.push(['urlHelper', function (urlHelper) {
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
                            }

                            //config.headers['Pragma'] = 'no-cache';
                            //config.headers['Expires'] = '-1';
                            //config.headers['Cache-Control'] = 'no-cache, no-store';

                            return config;
                        }
                    };
                }]);

                //#endregion

                var tokens = document.getElementsByName("__RequestVerificationToken");
                if (tokens.length > 0) {
                    $httpProvider.defaults.headers.post['__RequestVerificationToken'] = tokens[0].value;
                }
                $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';


                /* Прописано для # в URL вместо /#/ */
                $locationProvider.html5Mode({
                    enabled: true,
                    requireBase: true,
                    rewriteLinks: false
                });
                $locationProvider.hashPrefix('#');

                modalDefaultOptions.appendModalClass = 'color-scheme--light';
            }])
        .filter('sanitize', ['$sce', function ($sce) {
            return function (htmlCode) {
                return $sce.trustAsHtml(htmlCode);
            }
        }])
        .run(['scrollToBlockService', function (scrollToBlockService) {
             //переход к якорям с учетом включенной фиксированной шапки

            document.addEventListener('click', function (event) {//блокировать когда быстро нажимать кнопку
                var anchorIndex, anchorId, anchor;
        
                if (event.target.tagName.toLowerCase() === 'a' && event.target.getAttribute('href').indexOf('#') !== -1) {
                    anchorIndex = event.target.href.lastIndexOf('#');
                    if (anchorIndex !== -1 && event.target.href.indexOf(window.location.pathname + '/') === -1) {
                        anchorId = event.target.href.substring(anchorIndex + 1);
                        anchor = document.getElementById(anchorId);
        
                        if (anchor != null) {
                            event.preventDefault();
                            window.location.hash = anchorId;
                            scrollToBlockService.scrollToBlock(anchor);
                        }
                    }
                }
            });

            sweetAlert.setDefaults({
                cancelButtonText: 'Отмена',
                confirmButtonText: 'ОK',
                allowOutsideClick: false,
                useRejections: true
            });
        }])
        .directive('anchorImplementation', ['scrollToBlockService', '$location', '$window', function(scrollToBlockService, $location, $window) {
            return {
                link: function() {
                    if ($window.location.hash) {
                        var hash = $location.hash();
                        if (hash != null) {
                            var splitedHash = hash.split('?');
                            hash = splitedHash != null ? splitedHash[0] : hash;
                        }
                        var block = document.querySelector(hash);
                        if (block) {
                            scrollToBlockService.scrollToBlock(block);
                        }
                     }
                }
            }
        }]);

})(window.angular, window.clientDependency || [], window.adminDependency || []);

window.addEventListener('load', function () {

    $(document.querySelectorAll('.parallax')).enllax({
        type: 'background'
    });


    //отслеживание изменения URL для меню
    menuToggleClass(menuIsOpen());

    window.addEventListener('popstate', function (event) {
        menuToggleClass(menuIsOpen());
    });

    function menuIsOpen() {
        return window.location.hash.indexOf('menu_') === 1; //1 - с учётом '#'
    };

    function menuToggleClass(state) {
        document.body.classList[state === true ? 'add' : 'remove']('menu-state--opened');
    };
});